using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ouikum.Order;
using Ouikum.Common;
using Ouikum.Company;
using Prosoft.Service;
//using Prosoft.Base;
using Ouikum;
using System.Transactions;
using System.Data.Linq;
using res = Prosoft.Resource.Web.Ouikum;

namespace Ouikum.Web.MyB2B
{
    public partial class OrderController : BaseSecurityController
    {
        CommonService svCommon = new CommonService();

        #region Index
        public ActionResult Index()
        {
            GetStatusUser();
            return View();
        }
        #endregion

        #region Package
        public ActionResult Package(int? PackageID)
        {
            RememberURL();

            if (!CheckIsLogin())
                return Redirect(res.Pageviews.PvMemberSignIn);
             
            ListDoLoadData();
            GetStatusUser();
            CommonService svCommon = new CommonService();
            ViewBag.EnumServiceType = svCommon.SelectEnum(CommonService.EnumType.SearchByServiceType);
            return View();
        }
        #endregion

        #region Summary
        public ActionResult Summary(int OrderID)
        { 
            ViewBag.OrderID = OrderID;
            GetStatusUser();
            CommonService svCommon = new CommonService();
            ViewBag.EnumServiceType = svCommon.SelectEnum(CommonService.EnumType.SearchByServiceType);
            return View();
        }

        [HttpPost]
        public ActionResult Summary(List<int> PackageID, List<int> Qty, List<bool> Package)
        {
            #region Using Service
            var svPaymentAcc = new PaymentAccountService();
            var svCompany = new CompanyService();
            var svPackage = new PackageService();
            var svOrder = new OrderService();
            #endregion

            #region Set Variable
            var model = new b2bOrder();
            var OrderDetails = new List<b2bOrderDetail>();
            var OrderNumber = 1;
            decimal OrderTotalPrice = 0;
            #endregion

            #region Set Model Order Detail
            for (var i = 0; i < Package.Count(); i++)
            {
                if (Package[i] == true)
                {
                    var GetPackage = svPackage.SelectData<b2bPackage>(" PackageID,Price ", " PackageID = " + PackageID[i]).First();

                    #region วน Add Package เข้า List Order Detail ตามจำนวน ที่สั่งซื้อ
                    for (var x = 1; x <= Qty[i]; x++)
                    {
                        var detail = new b2bOrderDetail();
                        detail.OrderType = 1;
                        detail.PackageID = PackageID[i];
                        detail.RowFlag = 1;
                        detail.IsDelete = true;
                        detail.OrderDetailCode = AutoGenCode(CreateOrderDetialCode((int)PackageID[i]), OrderNumber);
                        OrderNumber++;
                        OrderDetails.Add(detail);
                    }
                    #endregion
                    OrderTotalPrice += ((decimal)GetPackage.Price * Qty[i]);
                }
            }

            #endregion

            #region Set Model Order
            model.CompID = LogonCompID;
            model.OrderStatus = "C";
            model.RowFlag = 1;
            model.IsDelete = true;
            model.TotalPrice = OrderTotalPrice;
            #endregion

            #region InsertOrder
            svOrder.InsertOrder(model, OrderDetails);
            #endregion

            #region Set ViewBag
            ViewBag.OrderID = model.OrderID;

            var Company = svCompany.SelectData<view_Company>(" * ", " CompID = " + LogonCompID).First();
            ViewBag.Company = Company;

            //var PaymentAccounts = svPaymentAcc.SelectData<View_PaymentAccount>("  PaymentAccID, BankID, AccName, AccNo, BranchName, PaymentTypeID, BankName, LogoImgPath, PaymentTypeName", "");
            //ViewBag.PaymentAccounts = PaymentAccounts;
            CommonService svCommon = new CommonService();
            ViewBag.EnumServiceType = svCommon.SelectEnum(CommonService.EnumType.SearchByServiceType);
            #endregion
            ViewBag.EnumServiceType = svCommon.SelectEnum(CommonService.EnumType.SearchByServiceType);
            GetStatusUser();
            return View();
        }

        [HttpPost]
        public ActionResult SummaryContent(int OrderID)
        {
            List_DoloadSummary(OrderID);
            GetStatusUser();
            return PartialView("UC/SummaryContent");
        }

        public void List_DoloadSummary(int OrderID)
        {
            var svOrder = new OrderService();
            var viewOrder = svOrder.SelectData<b2bOrder>(" * ", "CompID = " + LogonCompID + " AND OrderID = " + OrderID).First();
            ViewBag.viewOrder = viewOrder;

            var Summaries = new List<OrderSummaryModel>();

            for (var i = 3; i <= 5; i++)
            {
                if (i == 3)
                {
                    var sqlwhere = "RowFlag = 1 AND PackageID = 3 AND CompID = " + LogonCompID + " AND OrderID = " + OrderID + "";

                    var details = svOrder.SelectData<view_OrderDetail>(" * ", sqlwhere);
                    if (details.Count > 0)
                    {
                        #region Set Model to List OrderSummaryModel
                        var model = new OrderSummaryModel();
                        model.OrderDetailID = details[0].OrderDetailID;
                        model.Price = (decimal)details[0].Price;
                        model.PackageName = details[0].PackageName;
                        model.Qty = 1;
                        model.PackageID = (int)details[0].PackageID;
                        model.OrderID = details[0].OrderID;
                        Summaries.Add(model);
                        #endregion
                    }
                }
                else if (i == 4)
                {
                    var sqlwhere = "RowFlag = 1 AND PackageID = 4 AND CompID = " + LogonCompID + " AND OrderID = " + OrderID + "";
                    var count = svOrder.CountData<view_OrderDetail>(" * ", sqlwhere);

                    if (count > 0)
                    {
                        var details = svOrder.SelectData<view_OrderDetail>(" * ", sqlwhere, "", 1, 1, false);
                        var model = new OrderSummaryModel();

                        model.OrderDetailID = details[0].OrderDetailID;
                        model.Price = (decimal)details[0].Price;
                        model.PackageName = details[0].PackageName; model.Qty = count;
                        model.PackageID = (int)details[0].PackageID;
                        model.OrderID = details[0].OrderID;
                        Summaries.Add(model);
                    }

                }
                else if (i == 5)
                {
                    var sqlwhere = "RowFlag = 1 AND PackageID = 5 AND CompID = " + LogonCompID + " AND OrderID = " + OrderID + "";
                    var count = svOrder.CountData<view_OrderDetail>(" * ", sqlwhere);

                    if (count > 0)
                    {
                        var details = svOrder.SelectData<view_OrderDetail>(" * ", sqlwhere, "", 1, 1, false);
                        var model = new OrderSummaryModel();

                        model.OrderDetailID = details[0].OrderDetailID;
                        model.Price = (decimal)details[0].Price;
                        model.PackageName = details[0].PackageName;
                        model.Qty = count;
                        model.PackageID = (int)details[0].PackageID;
                        model.OrderID = details[0].OrderID;
                        Summaries.Add(model);
                    }
                }
            }
            ViewBag.OrderDetails = Summaries;
        }
        #endregion

        #region SubmitOrder
        public ActionResult SubmitOrder(int OrderID)
        {
            GetStatusUser();
            var svOrder = new OrderService();
            try
            {
                svOrder.SubmitOrder(OrderID);
            }
            catch (Exception ex)
            {
                CreateLogFiles(ex);
            }
            return Json(new { IsResult = svOrder.IsResult, MsgError = "" });
        }
        #endregion

        #region CancelOrder
        public ActionResult CancelOrder(int OrderID)
        {
            GetStatusUser();
            var svOrder = new OrderService();
            try
            {
                svOrder.CancelOrder(OrderID);
            }
            catch (Exception ex)
            {
                CreateLogFiles(ex);
            }
            return Json(new { IsResult = svOrder.IsResult, MsgError = "" });
        }
        #endregion

        #region DeleteOrder
        public ActionResult DeleteOrder(int OrderID)
        {
            var svOrder = new OrderService();
            try
            {
                svOrder.DeleteOrder(OrderID);
            }
            catch (Exception ex)
            {
                CreateLogFiles(ex);
            }
            return Json(new { IsResult = svOrder.IsResult, MsgError = "" });
        }
        #endregion

        #region DeleteOrderDetail
        public ActionResult DeleteOrderDetail(int OrderDetailID, int OrderID)
        {
            GetStatusUser();
            var svOrder = new OrderService();
            try
            {
                decimal OrderTotalPrice = 0;

                svOrder.DeleteOrderDetail(OrderDetailID, OrderID);

                var svOrderDetail = new OrderService();
                var svPackage = new PackageService();

                var OrderDetail = svOrderDetail.SelectData<b2bOrderDetail>(" * ", "IsDelete = 0 AND RowFlag > 0 AND OrderID = " + OrderID);
                ViewBag.OrderDetail = OrderDetail;

                foreach (var OrderDetails in ((List<b2bOrderDetail>)ViewBag.OrderDetail))
                {
                    var Package = svPackage.SelectData<b2bPackage>(" PackageID,Price ", " PackageID = " + OrderDetails.PackageID).First();
                    ViewBag.Package = Package;
                    var Packages = (b2bPackage)ViewBag.Package;

                    OrderTotalPrice += (decimal)Packages.Price;
                }
                svOrder.UpdateOrder(OrderID, OrderTotalPrice);
            }
            catch (Exception ex)
            {
                CreateLogFiles(ex);
            }
            return Json(new { IsResult = svOrder.IsResult, MsgError = "" });
        }
        #endregion

        #region Order List

        #region Get: List
        public ActionResult List()
        {
            RememberURL();
            SetPager();
            GetStatusUser();
            var svOrder = new OrderService();
            var sqlWhere = " IsDelete = 0 AND OrderStatus = 'N' AND CompID = " + LogonCompID;
            var Count = svOrder.CountData<b2bOrder>(" * ", sqlWhere);
            ViewBag.Count = Count;
            return View();
        }
        #endregion

        #region Post: List
        [HttpPost]
        public ActionResult List(FormCollection form)
        {
            SelectList_PageSize();
            SetPager(form);
            List_DoloadDataOrder(form);
            CommonService svCommon = new CommonService();
            ViewBag.EnumOrderStatus = svCommon.SelectEnum(CommonService.EnumType.OrderStatus);
            return PartialView("MyB2B/Order/Grid/OrderGrid");
        }
        #endregion
         

        public void List_DoloadDataOrder(FormCollection form)
        {
            var svOrder = new OrderService();
            var sqlWhere = " IsDelete = 0 AND CompID = " + LogonCompID;

            if (!string.IsNullOrEmpty(form["OrderCode"]))
            {
                sqlWhere += " AND OrderCode LIKE N'" + form["OrderCode"] + "%' ";
            }
            var Orders = svOrder.SelectData<b2bOrder>(" * ", sqlWhere, null, (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
            
            ViewBag.Orders = Orders;
            ViewBag.TotalPage = svOrder.TotalPage;
            ViewBag.TotalRow = svOrder.TotalRow;
        }

        #region SaveOrderList
        [HttpPost, ValidateInput(false)]
        public bool SaveOrderList(FormCollection form)
        {
            int objState = DataManager.ConvertToInteger(form["objState"]);//objState 1 คือ insert objState 2 คือ update
            var svOrder = new OrderService();
            var OrderList = new b2bOrder();
            if (objState == 2)// update
            {
                OrderList = svOrder.SelectData<b2bOrder>("*", " OrderID = " + form["OrderID"] + " AND RowVersion = " + form["RowVersion"]).First();
            }

            #region set ค่า b2bOrder
            //OrderList.CompID = DataManager.ConvertToInteger(Request.Cookies[res.Common.lblWebsite].Values["CompID"]);
            //OrderList.TotalPrice = DataManager.ConvertToDecimal(form["TotalPrice"]);
            //OrderList.CreatedDate = DataManager.ConvertToDateTime(form["CreatedDate"]);
            OrderList.OrderStatus = form["OrderStatus"];
            if (objState == 2)// update
            {
                OrderList.RowVersion = DataManager.ConvertToShort(OrderList.RowVersion + 1);
            }
            else
            {
                OrderList.RowFlag = 1;
                OrderList.RowVersion = 1;
                OrderList.CreatedBy = "sa";
                OrderList.ModifiedBy = "sa";
                OrderList.ModifiedDate = DateTime.Now;
                OrderList.CreatedDate = DateTime.Now;
            }
            #endregion

            #region Save b2bOrder
            OrderList = svOrder.SaveData<b2bOrder>(OrderList, "OrderID");
            #endregion

            return svOrder.IsResult;
        }
        #endregion

        #region EditOrder
        [HttpGet]
        public ActionResult EditOrder(int OrderID)
        {
            var svOrder = new OrderService();

            GetStatusUser();
            var Orders = svOrder.SelectData<b2bOrder>(" * ", "RowFlag > 0 AND IsDelete = 0 AND OrderID = " + OrderID).First();
            var ViewOrderDetails = svOrder.SelectData<view_OrderDetail>(" * ", "RowFlag > 0 AND OrderID = " + OrderID);
            var sqlWhere = " IsDelete = 0 AND OrderStatus = 'N' AND CompID = " + LogonCompID;
            var Count = svOrder.CountData<b2bOrder>(" * ", sqlWhere);
            ViewBag.Count = Count;

            ViewBag.Order = Orders;
            ViewBag.ViewOrderDetail = ViewOrderDetails;

            GetStatusUser();
            return View();
        }
        #endregion

        #endregion

        #region Detail
        public ActionResult Detail(int OrderID)
        {
            GetStatusUser();
            var svOrderDetail = new OrderService();
            var svCompany = new CompanyService();
            var svOrder = new OrderService();
            var Order = svOrder.SelectData<b2bOrder>(" * ", "IsDelete = 0 AND RowFlag > 0 AND OrderID = " + OrderID).First();
            var Count = svOrder.CountData<b2bOrder>(" * ", " IsDelete = 0 AND OrderStatus = 'N' AND  CompID = " + LogonCompID);
            var OrderDetail = svOrderDetail.SelectData<view_OrderDetail>(" * ", "RowFlag > 0 AND OrderID = " + OrderID, "Price desc");
            var Company = svCompany.SelectData<view_Company>(" * ", "CompID = " + LogonCompID).First();
            ViewBag.Count = Count;
            ViewBag.Order = Order;
            ViewBag.Company = Company;
            ViewBag.OrderDetail = OrderDetail;

            return View();
        }
        #endregion 
    }
}
