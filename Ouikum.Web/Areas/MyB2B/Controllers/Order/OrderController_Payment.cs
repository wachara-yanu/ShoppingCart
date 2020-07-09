using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ouikum.Order;
using Ouikum.Company;
using Prosoft.Service;
//using Prosoft.Base;
using System.Transactions;
using System.Data.Linq;
using Ouikum.Common;
using Ouikum;
namespace Ouikum.Web.MyB2B
{
    public partial class OrderController : BaseSecurityController
    { 

        #region Payment

        #region Submit Payment
        public ActionResult Payment(int OrderID)
        {
            GetStatusUser();
            var svPaymentAccount = new PaymentAccountService();
            var svOrder = new OrderService();
            var PaymentAccount = svPaymentAccount.SelectData<b2bPaymentAccount>(" * ").First();
            var Order = svOrder.SelectData<b2bOrder>(" * ", "OrderID = " + OrderID + " AND CompID = " + LogonCompID).First();
            var NBank = svOrder.SelectData<emBank>(" * ", "IsDelete = 0 AND BankID = " + PaymentAccount.BankID).First();
            var Bank = svOrder.SelectData<emBank>(" * ", "IsDelete = 0", "BankName asc");
            var Count = svOrder.CountData<b2bOrder>(" * ", "IsDelete = 0 AND OrderStatus = 'N' AND CompID = " + LogonCompID);
            ViewBag.Count = Count;

            ViewBag.PaymentAccount = PaymentAccount;
            ViewBag.Order = Order;
            ViewBag.NBank = NBank;
            ViewBag.Bank = Bank;
            return View();
        }
        #endregion

        #region EditPayment
        public ActionResult EditPayment(int MemberPaidId)
        {
            GetStatusUser();
            var svPaymentAccount = new PaymentAccountService();
            var svOrder = new OrderService();

            var Order = svOrder.SelectData<b2bOrder>(" * ", "MemberPaidID = " + MemberPaidId).First();
            var PaymentAccount = svPaymentAccount.SelectData<b2bPaymentAccount>(" * ").First();
            var MemberPaid = svOrder.SelectData<b2bMemberPaid>(" * ", "MemberPaidID = " + MemberPaidId + " AND CompID = " + LogonCompID).First();
            MemberPaid.PaymentDate = DataManager.ConvertToDateTime(MemberPaid.PaymentDate);
            var BankName = svOrder.SelectData<emBank>(" * ", "IsDelete = 0 AND BankID = " + PaymentAccount.BankID).First();
            var ListBanks = svOrder.SelectData<emBank>(" * ", "IsDelete = 0", "BankName asc");

            var Count = svOrder.CountData<b2bOrder>(" * ", "IsDelete = 0 AND OrderStatus = 'N' AND CompID = " + LogonCompID);
            ViewBag.Count = Count;

            ViewBag.Order = Order;
            ViewBag.PaymentAccount = PaymentAccount;
            ViewBag.MemberPaid = MemberPaid;
            ViewBag.NBank = BankName;
            ViewBag.Bank = ListBanks;
            return View();
        }
        #endregion

        #region Confirm
        [HttpPost]
        public ActionResult Confirm(bool? Check, int OrderID, int BankID, string BranchName, string PayerAccName, string PayerAccNo, int PaymentAccID
            , decimal PayAmount, string PaymentDate, string PaymentTime, string SlipImgPath, int? MemberPaidID)
        {
            GetStatusUser();
            var svOrder = new OrderService();
            var svComp = new CompanyService();
            var model = new b2bMemberPaid();
            Ouikum.Common.AddressService svAddress = new Ouikum.Common.AddressService();

            var companies = svComp.SelectData<b2bCompany>(" * ", "CompID = " + LogonCompID).First();
            var orders = svOrder.SelectData<b2bOrder>(" * ", "OrderID = " + OrderID).First();

            #region Set Model
            model.PayerName = companies.CompName;
            model.BillRecieverName = companies.CompName;
            model.PayerAddrLine1 = companies.CompAddrLine1;
            model.BillAddrLine1 = companies.CompAddrLine1;
            model.PayerDistrictID = companies.CompDistrictID;
            model.BillDistrictID = companies.CompDistrictID;
            model.PayerProvinceID = companies.CompProvinceID;
            model.BillProvinceID = companies.CompProvinceID;
            model.PayerPostalCode = companies.CompPostalCode;
            model.BillPostalCode = companies.CompPostalCode;
            model.PayerPhone = companies.CompPhone;
            model.BillPhone = companies.CompPhone;
            model.PayerFax = companies.CompFax;
            model.BillFax = companies.CompFax;
            model.PayerEmail = companies.ContactEmail;
            model.BillEmail = companies.ContactEmail;
            model.PayerMobile = companies.CompMobile;
            model.BillMobile = companies.CompMobile;
            model.BankID = BankID;
            model.BranchName = BranchName;
            model.PayerAccName = PayerAccName;
            model.PayerAccNo = PayerAccNo;
            model.PaymentAccID = PaymentAccID;
            model.PayAmount = PayAmount;
            model.PaymentTime = DataManager.ConvertToTimeSpan(PaymentTime);
            model.SlipImgPath = SlipImgPath;
            model.CompID = LogonCompID;
            if (Check == false)
            {
                model.PaymentStatus = "R";
            }
            else
            {
                model.PaymentStatus = "N";
            }
            model.RowFlag = 1;
            model.IsDelete = false;
            model.IsShow = true; 

            #endregion

            if (MemberPaidID > 0)
            {
                model.MemberPaidID = (int)MemberPaidID;
                model.PaymentDate = ConvertStringToDateTime(PaymentDate, false);
                svOrder.UpdatePayment(model);
            }
            else
            {
                model.PaymentDate = ConvertStringToDateTime(PaymentDate).AddYears(543);
                model.RowVersion = 1;
                svOrder.InsertPayment(model);
            }

            #region SaveSlipImg
            //if (model.SlipImgPath != SlipImgPath)
            //{
            imgManager = new FileHelper();
            imgManager.DirPath = "Companies/Slip/" + model.CompID;
            imgManager.DirTempPath = "Temp/Companies/Slip/" + model.CompID;
            imgManager.ImageName = SlipImgPath;
            imgManager.FullHeight = 0;
            imgManager.FullWidth = 0;
            imgManager.ThumbHeight = 150;
            imgManager.ThumbWidth = 150;

            imgManager.SaveImageFromTemp(); 
            //}
            #endregion

            var Provinces = svAddress.GetProvince().ToList();
            var Districts = svAddress.GetDistrict().Where(m => m.ProvinceID == companies.CompProvinceID).ToList();

            var Count = svOrder.CountData<b2bOrder>(" * ", "IsDelete = 0 AND OrderStatus = 'N' AND CompID = " + LogonCompID);
            ViewBag.Count = Count;

            ViewBag.companies = companies;
            ViewBag.orders = orders;
            ViewBag.District = Districts;
            ViewBag.Province = Provinces;
            ViewBag.MemberPaid = model;

            return View();
        }
        #endregion

        #region Post: UpdatePayment
        [HttpPost]
        public ActionResult UpdatePayment(FormCollection form)
        {
            var svOrder = new OrderService();

            #region Set Model
            var OrderID = DataManager.ConvertToInteger(form["OrderID"]);
            var MemberPaidID = DataManager.ConvertToInteger(form["MemberPaidID"]);
            var IsInvoice = form["IsInvoice"];
            var PayerName = form["PayerName"];
            var PayerAddrLine1 = form["PayerAddrLine1"];
            var PayerDistrictID = DataManager.ConvertToInteger(form["PayerDistrictID"]);
            var PayerProvinceID = DataManager.ConvertToInteger(form["PayerProvinceID"]);
            var PayerPostalCode = form["PayerPostalCode"];
            var PayerPhone = form["PayerPhone"];
            var PayerMobile = form["PayerMobile"];
            var PayerFax = form["PayerFax"];
            var PayerEmail = form["PayerEmail"];
            var RejectComment = form["RejectComment"];
            var BillRecieverName = form["BillRecieverName"];
            var BillAddrLine1 = form["BillAddrLine1"];
            var BillDistrictID = DataManager.ConvertToInteger(form["BillDistrictID"]);
            var BillProvinceID = DataManager.ConvertToInteger(form["BillProvinceID"]);
            var BillPostalCode = form["BillPostalCode"];
            var BillPhone = form["BillPhone"];
            var BillMobile = form["BillMobile"];
            var BillFax = form["BillFax"];
            var BillEmail = form["BillEmail"];
            #endregion

            try
            {
                svOrder.UpdatePayment(OrderID, MemberPaidID, IsInvoice, PayerName, PayerAddrLine1, PayerDistrictID, PayerProvinceID, PayerPostalCode, PayerPhone, PayerMobile, PayerFax, PayerEmail, RejectComment, BillRecieverName, BillAddrLine1, BillDistrictID, BillProvinceID, BillPostalCode, BillPhone, BillMobile, BillFax, BillEmail);
            }
            catch (Exception ex)
            {
                CreateLogFiles(ex);
            }
            return Redirect("~/MyB2B/Order/PaymentList");
        }
        #endregion

        #region Payment List

        #region Get: PaymentList
        public ActionResult PaymentList()
        {
            RememberURL();
            SetPager();
            GetStatusUser();
            var svOrder = new OrderService();
            var CountApprove = svOrder.CountData<b2bMemberPaid>(" * ", "IsDelete = 0 AND PaymentStatus = 'A' AND CompID = " + LogonCompID);
            var CountWait = svOrder.CountData<b2bMemberPaid>(" * ", "IsDelete = 0 AND PaymentStatus = 'N' AND CompID = " + LogonCompID);
            ViewBag.CountApprove = CountApprove;
            ViewBag.CountWait = CountWait;
            
            return View();
        }
        #endregion

        #region Post: PaymentList
        [HttpPost]
        public ActionResult PaymentList(FormCollection form)
        {
            var svOrder = new OrderService();

            SelectList_PageSize();
            SetPager(form);

            var MemberPaids = svOrder.SelectData<b2bMemberPaid>(" * ", "IsDelete = 0 AND CompID = " + LogonCompID + " AND MemberPaidCode LIKE N'" + form["MemberPaidCode"] + "%' and CompID = " + LogonCompID, null, 1, (int)ViewBag.PageSize);
           
            ViewBag.MemberPaid = MemberPaids;
            ViewBag.TotalPage = svOrder.TotalPage;
            ViewBag.TotalRow = svOrder.TotalRow;
            CommonService svCommon = new CommonService();
            ViewBag.EnumMemberPaidStatus = svCommon.SelectEnum(CommonService.EnumType.MemberPaidStatus);
            return PartialView("MyB2B/Order/Grid/PaymentGrid");
        }
        #endregion

        #endregion

        #region CancelPayment
        public ActionResult CancelPayment(int MemberPaidID)
        {
            var svOrder = new OrderService();
            try
            {
                svOrder.CancelPayment(MemberPaidID);
            }
            catch (Exception ex)
            {
                CreateLogFiles(ex);
            }
            return Json(new { IsResult = svOrder.IsResult, MsgError = "" });
        }
        #endregion

        #region BackPayment
        public ActionResult BackPayment(int MemberPaidID, int OrderID)
        {
            var svOrder = new OrderService();
            try
            {
                svOrder.BackPayment(MemberPaidID);
            }
            catch (Exception ex)
            {
                CreateLogFiles(ex);
            }

            GetStatusUser();
            var svPaymentAccount = new PaymentAccountService();
            var PaymentAccount = svPaymentAccount.SelectData<b2bPaymentAccount>(" * ").First();
            var Order = svOrder.SelectData<b2bOrder>(" * ", "OrderID = " + OrderID + " AND CompID = " + LogonCompID).First();
            var NBank = svOrder.SelectData<emBank>(" * ", "IsDelete = 0 AND BankID = " + PaymentAccount.BankID).First();
            var Bank = svOrder.SelectData<emBank>(" * ", "IsDelete = 0", "BankName asc");
            var Count = svOrder.CountData<b2bOrder>(" * ", "IsDelete = 0 AND OrderStatus = 'N' AND CompID = " + LogonCompID);
            ViewBag.Count = Count;

            ViewBag.PaymentAccount = PaymentAccount;
            ViewBag.Order = Order;
            ViewBag.NBank = NBank;
            ViewBag.Bank = Bank;
            return Json(new { IsResult = svOrder.IsResult, MsgError = "" });
        }
        #endregion

        #region DeletePayment
        public ActionResult DeletePayment(int MemberPaidID)
        {
            var svOrder = new OrderService();
            var Count = svOrder.CountData<b2bOrder>(" * ", "MemberPaidID = " + MemberPaidID);
            if (Count != 0)
            {
                var Order = svOrder.SelectData<b2bOrder>(" * ", "MemberPaidID = " + MemberPaidID).First();

                try
                {
                    svOrder.DeletePayment(Order.OrderID, MemberPaidID);
                }
                catch (Exception ex)
                {
                    CreateLogFiles(ex);
                }
            }
            else
            {
                try
                {
                    svOrder.DeletePaymentID(MemberPaidID);
                }
                catch (Exception ex)
                {
                    CreateLogFiles(ex);
                }
            }
            return Json(new { IsResult = svOrder.IsResult, MsgError = "" });
        }
        #endregion

        #region PaymentDetail
        public ActionResult PaymentDetail(int MemberPaidID)
        {
            GetStatusUser();
            var svOrder = new OrderService();
            var svCompany = new CompanyService();

            var MemberPaid = svOrder.SelectData<b2bMemberPaid>(" * ", "IsDelete = 0 AND RowFlag > 0 AND MemberPaidID = " + MemberPaidID).First();
            var Order = svOrder.SelectData<b2bOrder>(" * ", "IsDelete = 0 AND RowFlag > 0 AND MemberPaidID = " + MemberPaidID).First();
            var Bank = svOrder.SelectData<emBank>(" * ", "BankID = " + MemberPaid.BankID).First();
            var PaymentAccount = svOrder.SelectData<b2bPaymentAccount>(" * ", "PaymentAccID = " + MemberPaid.PaymentAccID).First();
            var BankAcc = svOrder.SelectData<emBank>(" * ", "BankID = " + PaymentAccount.BankID).First();
            var OrderDetail = svOrder.SelectData<view_OrderDetail>(" * ", "RowFlag > 0 AND OrderID = " + Order.OrderID, "Price desc");
            var Company = svCompany.SelectData<view_Company>(" * ", "CompID = " + MemberPaid.CompID).First();

            var CountApprove = svOrder.CountData<b2bMemberPaid>(" * ", "IsDelete = 0 AND PaymentStatus = 'A' AND CompID = " + LogonCompID);
            var CountWait = svOrder.CountData<b2bMemberPaid>(" * ", "IsDelete = 0 AND PaymentStatus = 'N' AND CompID = " + LogonCompID);
            ViewBag.CountApprove = CountApprove;
            ViewBag.CountWait = CountWait;

            ViewBag.MemberPaid = MemberPaid;
            ViewBag.Bank = Bank;
            ViewBag.BankAcc = BankAcc;
            ViewBag.PaymentAccount = PaymentAccount;
            ViewBag.Order = Order;
            ViewBag.Company = Company;
            ViewBag.OrderDetail = OrderDetail;

            return View();
        }
        #endregion

        #endregion

    }
}
