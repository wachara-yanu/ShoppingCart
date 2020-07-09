using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using res = Prosoft.Resource.Web.Ouikum;
using Ouikum;
using Ouikum.Cart;
using Ouikum.Common;
using Ouikum.BizType;
using Ouikum.Company;
using Ouikum.Product;
using Ouikum.Shipment;
using Ouikum.OrderPuchase;


namespace Ouikum.Web.MyB2B
{
    public partial class OrderPurchaseController : BaseSecurityController
    {
        #region Members
        // GET: /MyB2B/Company/
        BizTypeService svBizType;
        MemberService svMember;
        AddressService svAddress;
        CompanyService svCompany;
        ProductService svProduct;
        ShipmentService svShipment;
        OrderPurchaseService svOrderPur;
        #endregion

        #region Constructors
        public OrderPurchaseController()
        {
            svBizType = new BizTypeService();
            svMember = new MemberService();
            svAddress = new AddressService();
            svCompany = new CompanyService();
            svProduct = new ProductService();
            svShipment = new ShipmentService();
            svOrderPur = new OrderPurchaseService();
        }
        #endregion

        // GET: MyB2B/OrderPurchase
        public ActionResult Index()
        {
            var svOrderPur = new OrderPurchaseService();
            RememberURL();
            if (!CheckIsLogin())
            {
                return Redirect(res.Pageviews.PvMemberSignIn);
            }
            else
            {
                GetStatusUser();
                return View();

            }
        }

        [HttpPost]
        public ActionResult Index(FormCollection form)
        {
            var svOrderPur = new OrderPurchaseService();
            SelectList_PageSize();
            SetPager(form);
            var getOrderDetail = svOrderPur.SelectData<View_OuikumOrderDetailSC>("*", "StatusProduct != 'D' AND IsDelete = 0 and CompSCID = " + LogonCompID, "StatusProduct,ModifiedDate asc", (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
            ViewBag.OrderDetail = getOrderDetail;
            ViewBag.TotalPage = svOrderPur.TotalPage;
            ViewBag.TotalRow = svOrderPur.TotalRow;
            //var products = svProduct.SelectData<view_SearchProduct>("ProductID,ProductName", "IsDelete = 0 and CompID =" + LogonCompID);
            //ViewBag.Products = products;

            return PartialView("Grid/GridOrderPurchase");
        }

        #region GetEditOrderStatus
        public JsonResult GetEditOrderStatus(string OrDetailID)
         {
            var svOrderPurchase = new OrderPurchaseService();
            var svOrderStauts = new OrderStatusService();
            CommonService svCommon = new CommonService();
            string SQLWhere = string.Empty;
            //var EnumHotFeatStatus = svCommon.SelectEnum(CommonService.EnumType.HotFeatStatus);
            //var orderIsShow = svOrder.SelectData<b2bPackage>("PackageID,PackageName,Price,Duratrion", "IsDelete = 0 AND IsShow = 0 AND CheckUpdate = 1", "PackageID ASC");
            var data = svOrderPurchase.SelectData<View_OuikumOrderDetailSC>("OrDetailID,OrDetailcode,ProductName,StatusProduct", "IsDelete = 0 AND OrDetailID = " + OrDetailID );
            SQLWhere = svOrderStauts.CreateWhereAction(OrderStatusAction.ConfirmBySC, LogonCompID);
            var Status = svOrderStauts.SelectData<OuikumStausOrder>("NameStatus", SQLWhere, "NameStatus", 1, 0);


            //return Json(new { IsResult = true, Price = (String.Format("{0:##,###.00}", data.First().PackagePrice)), Duration = data.First().Duration, PackageID = data.First().PackageID, EnumHotFeatStatus = EnumHotFeatStatus, orderIsShow = orderIsShow });
            return Json(new { IsResult = true, OrDetailCode = data.First().OrDetailCode, ProductName = data.First().ProductName, StatusProduct = data.First().StatusProduct});

        }
        #endregion

        #region Update OrderDetial
        [HttpPost, ValidateInput(false)]
        public ActionResult SaveStatus(string OrDetailCode, string StatusProduct)
        {
            OuikumOrderDetail orDetial = new OuikumOrderDetail();
            var svOrderPur = new OrderPurchaseService();
            var svOrderStauts = new OrderStatusService();
            string SQLWhere = string.Empty;
            try
            {

                var GetorderDetail = svOrderPur.SelectData<View_OuikumOrderDetailSC>("OrDetailID,OrDetailcode,ProductID,ProductName,StatusProduct", "IsDelete = 0 AND OrDetailCode = '" + OrDetailCode + "'").First();
                SQLWhere = svOrderStauts.CreateWhereAction(OrderStatusAction.ConfirmBySC, LogonCompID);
                var Status = svOrderPur.SelectData<OuikumStausOrder>("NameStatus", SQLWhere, "NameStatus", 1, 0);

                orDetial.OrDetailID = GetorderDetail.OrDetailID;
                if (GetorderDetail.StatusProduct == "A")
                {
                    orDetial.StatusProduct = "B";
                }
                else if (GetorderDetail.StatusProduct == "B")
                {
                    orDetial.StatusProduct = "C";
                }
                else{
                    orDetial.StatusProduct = "D";
                }
                orDetial.ProductID = GetorderDetail.ProductID;
                orDetial.ModifiedDate = DateTime.Now;

                #region Update Satatus OrderDetail

                svOrderPur.UpdateStatusOrder(orDetial);
                #endregion


            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false });
            }

            return Json(new { IsSuccess = true });
        }
        #endregion

    }
}