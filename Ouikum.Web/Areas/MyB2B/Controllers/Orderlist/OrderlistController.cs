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
    public partial class OrderlistController : BaseSecurityController
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
        public OrderlistController()
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


        // GET: MyB2B/Orderlist
        public ActionResult Index()
        {
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
            var svOrderlist = new OrderPurchaseService();
            SelectList_PageSize();
            SetPager(form);
            var GetOrder = svOrderlist.SelectData<View_HistoryOrder>("*", "IsDelete = 0 and CompID = " + LogonCompID, "CreatedDate DESC", (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
            ViewBag.GetOrder = GetOrder;
            ViewBag.TotalPage = svOrderlist.TotalPage;
            ViewBag.TotalRow = svOrderlist.TotalRow;
            //var products = svProduct.SelectData<view_SearchProduct>("ProductID,ProductName", "IsDelete = 0 and CompID =" + LogonCompID);
            //ViewBag.Products = products;

            return PartialView("UC/ListGrid");
        }
    }
}