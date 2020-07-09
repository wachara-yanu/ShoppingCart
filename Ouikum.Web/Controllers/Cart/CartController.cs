using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ouikum.Common;
using Ouikum.Company;
using Prosoft.Service;
using System.Transactions;
using Ouikum.Message;
using Ouikum.Web.Models;
using res = Prosoft.Resource.Web.Ouikum;
using Ouikum.Cart;
using Ouikum.Product;
using Ouikum.Shipment;
using Ouikum.OrderPuchase;
using System.Collections;
using System.Text;
using System.Web.Security;
using System.Text.RegularExpressions;
using Ouikum;
//using Prosoft.Base;
using System.Configuration;
using System.Globalization;
using System.Web.ApplicationServices;
using Ouikum.Quotation;
using Ouikum.Order;

namespace Ouikum.Web.Controllers
{
    public class CartController : BaseSecurityController

    {
        // GET: Cart
        CartService svCart;
        ProductService svProduct;
        CompanyService svCompany;
        AddressService svAddress;
        MemberService svMember = new MemberService();
        ShipmentService svShipment;
        CommonService svCommon = new CommonService();
        OrderPurchaseService svOrderPur;
        WebService svWeb;
        // ----- signin--- //
        EmailManager emailManager = null;
        Mail mail = null;

        public CartController()
        {
            svCart = new CartService();
            svProduct = new ProductService();
            svCompany = new CompanyService();
            svAddress = new AddressService();
            svMember = new MemberService();
            svShipment = new ShipmentService();
            svCommon = new CommonService();
            svOrderPur = new OrderPurchaseService();
            svWeb = new WebService();
            AppName = res.Common.lblWebsite;
            RememberAppName = string.Concat("Remember", AppName);
            svAuthentication = new AuthenticationService();

            emailManager = new EmailManager(res.Config.SMTP_Server, res.Config.SMTP_UserName, res.Config.SMTP_Password, Convert.ToBoolean(res.Config.SMTP_IsAuthentication));
            mail = new Mail();
        }


        [HttpGet]
        public ActionResult Index()
        {

                TempCart temp = new TempCart();
                var svCart = new CartService();
                var svMember = new MemberService();
                var svCompany = new CompanyService();
                var svShipment = new ShipmentService();                var Temp = svCart.SelectData<View_TempCart>("*", "IsDelete = 0 and TempIDLogon = " + LogonCompID);
                var SumTempCart = svCart.SelectCountTemp<View_SumTempCart>("*", "IsDelete = 0 and TempIDLogon = " + LogonCompID);
                var Count = svCart.SelectCountTemp<View_CountTempCart>("*", "TempIDLogon = " + LogonCompID);
                GetStatusUser();
                //ViewBag.CompIDSC = Count.Add.comp
                AddressService svAddress = new AddressService();
                var Member = svMember.SelectData<view_emCompanyMember>("*", "IsDelete = 0 AND MemberWebID = " + LogonCompID, null, 0, 0, false).First();
                //var Provinces = svAddress.SelectData<emProvince>("*", "IsDelete = 0", "RegionID ASC");
                //var Districts = svAddress.ListDistrictByProvinceID((int)Member.ProvinceID);
                //var SubDistricts = svAddress.ListSubistrictByProvinceID((int)Member.DistrictID);

                var Shipments = svCompany.SelectCountTemp<View_TempShipmentCart>("*", "IsDelete = 0");
                var ShipmentProduct = svShipment.SelectData<View_ShipmentProduct>("*", "IsDelete = 0 and TempIDLogon = " + LogonCompID);
                var QueryComp = svCompany.SelectData<view_Company>("*", "CompID=" + LogonCompID, "", 1, 1, false).First();
                ViewBag.QueryComp = QueryComp;
                ViewBag.ShipmentProduct = ShipmentProduct;
                ViewBag.TempCart = Temp;
                ViewBag.CountProduct = Count;
                ViewBag.CompanyShipment = Shipments;
                //ViewBag.Provinces = Provinces;
                //ViewBag.Districts = Districts;
                //ViewBag.SubDistricts = SubDistricts;
                ViewBag.SumTempCart = SumTempCart;
                #region query District And Province
                ViewBag.QueryDistrict = svAddress.ListDistrictByProvinceID(0);
                ViewBag.QueryProvince = svAddress.GetProvinceAll();
                #endregion
                return View();
            
        }

        #region AddCart
        [HttpGet]
        public ActionResult List(int? ID)
        {
            TempCart tCart = new TempCart();
            var svCart = new CartService();
            RememberURL();
            if (!CheckIsLogin())
            {

                return Redirect(res.Pageviews.PvMemberSignIn); //Redirct กลับไปหน้า Login
            }
            else
            {
                CommonService svCommon = new CommonService();
                ViewBag.EnumServiceType = svCommon.SelectEnum(CommonService.EnumType.SearchByServiceType);
                GetStatusUser();

                #region SelectProduct
                var svProduct = new ProductService();
                var Products = svProduct.SelectData<view_Product>("*", "ProductID = " + ID, null, 1, 0, false).First();
                int CompID = Convert.ToInt32(Products.CompID);
                ViewBag.ProductDetail = Products;
                ViewBag.ProductID = ID;
                #endregion
                AddTemp(Convert.ToString(ID));
                return RedirectToAction("Index");
            }

        }

        #endregion

        #region DeltempCart

        [HttpPost]
        public ActionResult SendDel(int ID)
        {
            TempCart tCart = new TempCart();
            var svCart = new CartService();
            Deletet(ID);
            ViewBag.Message = "ลบสินค้าในตะกร้าของคุณเรียบร้อยแล้ว";
            return RedirectToAction("Index");
        }

        #endregion


        #region PostAddFavProduct
        [HttpPost]
        public ActionResult DelProduct(int id)
        {
            var CartService = new CartService();
            OuikumTempOrder model = new OuikumTempOrder();

            try
            {
                TempCart tCart = new TempCart();
                var svCart = new CartService();
                Deletet(id);
               
            }
            catch (Exception ex)
            {
                CreateLogFiles(ex);
            }


            return Json(new { IsResult = svCart.IsResult, MsgError = GenerateMsgError(svCart.MsgError), ID = model.TOrderID });
        }

        private bool ValidateFav(int Pro)
        {
            throw new NotImplementedException();
        }
        #endregion



        #region DelCountTempCart
        public ActionResult DelCount(int ID)
        {
            TempCart tCart = new TempCart();
            var svCart = new CartService();
            var svProduct = new ProductService();

                var getProductCount = svProduct.SelectData<TempCart>("*", " IsDelete = 0 and TempID = " + ID + " and TempIDLogon = " + LogonCompID, null, 1, 0, false).First();
                var countProduct = getProductCount.TempCountProduct - 1;
                int LogOnuser = 0;
                int TempProductID = 0;
                LogOnuser = LogonCompID;
                TempProductID = Convert.ToInt32(getProductCount.TempProdcutID);
                svCart.MinusTempCart(LogOnuser, TempProductID, countProduct);
                return RedirectToAction("Index");

        }
        #endregion


        #region PlusCountTempProduct
        public ActionResult PlusCount(int ID)
        {
            TempCart tCart = new TempCart();
            var svCart = new CartService();
            var svProduct = new ProductService();

            var getProductCount = svProduct.SelectData<TempCart>("*", " IsDelete = 0 and TempID = " + ID + " and TempIDLogon = " + LogonCompID, null, 1, 0, false).First();
            var countProduct = getProductCount.TempCountProduct + 1;
            int LogOnuser = 0;
            int TempProductID = 0;
            LogOnuser = LogonCompID;
            TempProductID = Convert.ToInt32(getProductCount.TempProdcutID);
            svCart.MinusTempCart(LogOnuser, TempProductID, countProduct);
            return RedirectToAction("Index");
        }
        #endregion
         

        #region AddTempCart
        public ActionResult AddTemp(string ProductID)
        {
            TempCart tCart = new TempCart();
            var svCart = new CartService();
            var svProduct = new ProductService();
            try
            {
                string TempData = string.Empty;

                var Temp = svCart.SelectData<TempCart>("*", " IsDelete = 0 and TempIDLogon = " + LogonCompID + " and TempProdcutID = " + ProductID);
                var getProduct = svProduct.SelectData<view_Product>("*", " IsDelete = 0 and ProductID = " + ProductID, null, 1, 0, false).First();
                //int countProduct = Temp.Count + 1;
                if (Temp.Count > 0) //ถ้ามีสินค้าให้ให้อัพเดท countProduct
                {
                    var getProductCount = svProduct.SelectData<TempCart>("*", " IsDelete = 0 and TempProdcutID = " + ProductID + " and TempIDLogon = " + LogonCompID , null, 1, 0, false).First();
                    var countProduct = getProductCount.TempCountProduct+1 ;
                    int LogOnuser = 0;
                    int TempProductID = 0;
                    LogOnuser = LogonCompID;
                    TempProductID = int.Parse(ProductID);
                    svCart.UpdateTempCart(LogOnuser,TempProductID, countProduct);
                    ViewBag.Message = "อัพเดทสินค้าในตะกร้าของคุณเรียบร้อยแล้ว";
                   
                }
                else { //ถ้ามีสินค้าให้ insertTempCart

                    #region Set TempCart
                    tCart.TempProdcutID = getProduct.ProductID;
                    tCart.TempPrice = getProduct.Price_One;
                    tCart.TempQua = 1;
                    tCart.TempCountProduct = 1;
                    tCart.TempIDLogon = LogonCompID;
                    tCart.IsShow = true;
                    tCart.IsDelete = false;
                    #endregion

                    svCart.InserTempCart(tCart);
                    ViewBag.Message = "เพิ่มสินค้าในตะกร้าของคุณเรียบร้อยแล้ว";
                }

            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false});
            }

            return Json(new { IsSuccess = true });
        }

        #endregion

        #region DeleteProduct TempCart

        [HttpPost]
        public ActionResult Deletet(int TempID)
        {
            GetStatusUser();
           
            var svCart = new CartService();
            try
            {
                svCart.DeleteCart(TempID);
            }
            catch (Exception ex)
            {
                CreateLogFiles(ex);
            }
            return Json(new { IsResult = svCart.IsResult, MsgError = "" });
        }

        #endregion


        #region AddOrderOuikum
        [HttpPost, ValidateInput(false)]
        public ActionResult AddOrderProduct(FormCollection form)
        {

            OuikumOrder tOrder = new OuikumOrder();
            OuikumOrderDetail tOrderDetail = new OuikumOrderDetail();
            var svCart = new CartService();
            var svShipment = new ShipmentService();

            #region set Order
            tOrder.SendName = form["SendName"];
            tOrder.SendSername = form["SendSername"];
            tOrder.SendTel = form["SendTel"];
            tOrder.Email = form["Email"];

            if (form["AddrLine1"] != null)
            {
                tOrder.AddrLine1 = form["AddrLine1"];
            }

            if (form["AddrLine2"] != null)
            {
                tOrder.AddrLine2 = form["AddrLine2"];
            }

            tOrder.SubDirtrict = form["SubDirtrict"];
            tOrder.DirtrictID = Convert.ToInt32(form["DirtrictID"]);
            tOrder.ProviceID = Convert.ToInt32(form["ProviceID"]);
            tOrder.Postal = form["Postal"];
            tOrder.CompID = LogonCompID;
            tOrder.OrderStatus = "A";
            tOrder.Total = Convert.ToDecimal(form["Total"]);
            tOrder.RowFlag = 1;
            tOrder.RowVersion = 1;
            tOrder.IsDelete = false;
            tOrder.IsShow = true;
            tOrder.OrderDate = DateTime.Now;
            tOrder.CreatedDate = DateTime.Now;
            tOrder.ModifiedDate = DateTime.Now;
            tOrder.CreatedBy = Convert.ToString(LogonCompID);
            tOrder.ModifiedBy = Convert.ToString(LogonCompID);
            #endregion

            var getTempCart = svCart.SelectData<View_TempCart>("*", "IsDelete = 0 and TempIDLogon = " + LogonCompID);
            var OrderNumber = 1;
            var TempOrderDetail = new List<OuikumOrderDetail>();
            var ShipmentProduct = svShipment.SelectData<View_ShipmentProduct>("*", "IsDelete = 0 and TempIDLogon = " + LogonCompID);
            ViewBag.ShipmentProduct = ShipmentProduct;

            if (getTempCart.Count > 0)
            {
                for (int i = 0; i <= getTempCart.Count-1; i++)
                {
                    decimal? TotalPriceProduct = 0;
                    #region set OrderDetail
                    var detail = new OuikumOrderDetail();
                    detail.ProductID = getTempCart[i].TempProdcutID;
                    detail.CompSCID = getTempCart[i].CompID;
                    detail.Qty = getTempCart[i].TempCountProduct;
                    detail.Qty_Price = getTempCart[i].TempPrice;

                    foreach (var getShipmentPrice in (List<View_ShipmentProduct>)ViewBag.ShipmentProduct)
                    {
                        if (getShipmentPrice.CompID == getTempCart[i].CompID && getShipmentPrice.ProductID == getTempCart[i].TempProdcutID)
                        {
                            if (getTempCart[i].TempCountProduct <= getShipmentPrice.BuyMaximum && getTempCart[i].TempCountProduct >= getShipmentPrice.BuyMinimum)
                            {
                                detail.PriceShipment = getShipmentPrice.PriceShipment;
                                TotalPriceProduct = getShipmentPrice.PriceShipment;
                            }
                        }
                    }

                    var SumPriceUnit1 = getTempCart[i].TempCountProduct * getTempCart[i].TempPrice;
                    detail.TotalPriceproduct = SumPriceUnit1;
                    detail.TotalSum = SumPriceUnit1+TotalPriceProduct;
                    detail.StatusProduct = "A";
                    detail.OrDetailCode = AutoGenCode("Or-D", OrderNumber);
                    detail.IsShow = true;
                    detail.IsDelete = false;
                    detail.RowFlag = 1;
                    detail.RowVersion = 1;
                    detail.CreatedDate = DateTime.Now;
                    detail.ModifiedDate = DateTime.Now;
                    detail.CreatedBy = Convert.ToString(LogonCompID);
                    detail.ModifiedBy = Convert.ToString(LogonCompID);
                    OrderNumber++;
                    TempOrderDetail.Add(detail);
                    TotalPriceProduct = 0;
                    #endregion
                }

                #region Insert b2bOrder
                svCart.InsertOrder(tOrder, TempOrderDetail);
                #endregion

            }
            UpdateTempCart();

            if (svCart.IsResult)
            {

                var compBuyer = svCompany.SelectData<View_getSendmailOrder>("Email, namebuyer , CompID ,OrderID", " OrderID = " + tOrder.OrderID, null).First();

                if (!SendEmailOrdertoBuyer(compBuyer.namebuyer, compBuyer.Email, compBuyer.OrderID))
                {
                    return Json(new { IsSuccess = false, Result = res.EmaiOrderToBuyer.lblresulttobuyerfalse });
                }
                else
                {
                    //return Json(new { IsSuccess = true, Result = res.EmaiOrderToBuyer.lblresulttobuyer });
                    var compSeller = svCompany.SelectDataEmailtoSeller<view_getMailOrdertoSeller>(" emailseller, OrderID, compidseller ", " OrderID = " + tOrder.OrderID );
                    var Mailseller = new List<view_getMailOrdertoSeller>();
                    if (compSeller.Count > 0)
                    {
                        for (int a = 0; a <= compSeller.Count - 1; a++)
                        {
                            var email = new view_getMailOrdertoSeller();
                            email.emailseller = compSeller[a].emailseller;
                            email.OrderID = compSeller[a].OrderID;
                            email.compidseller = compSeller[a].compidseller;
                            Mailseller.Add(email);
                        }
                        if (!SendEmailOrdertoSeller(Mailseller,tOrder.OrderID))
                        {
                            return Json(new { IsSuccess = false, Result = res.EmaiOrderToBuyer.lblresulttobuyerfalse });
                        }
                        else
                        {
                            return Json(new { IsSuccess = true, Result = res.EmaiOrderToBuyer.lblresulttobuyer });
                        }
                    }
                }
            }


            return Json(new { IsSuccess = true, Result = "บันทึกรายการสั่งซื้อของคุณเรียบร้อยแล้ว" });

           // return svCart.IsResult;
        }
        #endregion


        #region UpdateTempcart
        public ActionResult UpdateTempCart()
        {
            GetStatusUser();

            var svCart = new CartService();
            try
            {
                svCart.UpdateCheckOutTempCart(LogonCompID);
            }
            catch (Exception ex)
            {
                CreateLogFiles(ex);
            }
            return Json(new { IsResult = svCart.IsResult, MsgError = "" });
        }
        #endregion

        public bool SendEmailOrdertoBuyer(string FirstName, string email, int orderid)
        {
            #region variable
            var Detail = "";
            var DetailMail = new List<view_EmailToBuyer>();
            var mailTo = new List<string>();
            var mailCC = new List<string>();
            //var DetailOrder = new List<string>();
            Hashtable EmailDetail = new Hashtable();
            #endregion
            //string mailSupport = "suppot@ouikum.com";
            //mailCC.Add(mailSupport);
            var getheadmailtoBuyer = svCart.SelectData<View_getSendmailOrder>("*", " OrderID = " + orderid,null).First();
            var getDetailmailtoBuyer = svCart.SelectData<view_EmailToBuyer>("*", " OrderID = " + orderid);
            string ouikumthai_url = res.Pageviews.UrlWeb;
            string pathlogo = ouikumthai_url + "/Content/Default/logo/Ouikum/img_Logo120x74.png";
            EmailDetail["pathLogo"] = pathlogo;
            EmailDetail["FirstName"] = FirstName;
            EmailDetail["NameBuyer"] = getheadmailtoBuyer.namebuyer;
            EmailDetail["TelBuyer"] = getheadmailtoBuyer.SendTel;
            EmailDetail["EmailBuyer"] = getheadmailtoBuyer.Email;
            EmailDetail["Addres"] = getheadmailtoBuyer.addr;
            EmailDetail["Subdistrict"] = getheadmailtoBuyer.SubDirtrict;
            EmailDetail["District"] = getheadmailtoBuyer.DistrictName;
            EmailDetail["Province"] = getheadmailtoBuyer.ProvinceName;
            EmailDetail["Postal"] = getheadmailtoBuyer.Postal;
            EmailDetail["OrderDate"] = getheadmailtoBuyer.OrderDate;
            EmailDetail["TotalSum"] = getheadmailtoBuyer.Total;

            if (getDetailmailtoBuyer.Count > 0)
            {
                for (int i = 0; i <= getDetailmailtoBuyer.Count-1; i++)
                {
                    var detail = new view_EmailToBuyer();
                    detail.ProductName = getDetailmailtoBuyer[i].ProductName;
                    detail.OrDetailCode = getDetailmailtoBuyer[i].OrDetailCode;
                    detail.Qty = getDetailmailtoBuyer[i].Qty;
                    detail.Qty_Price = getDetailmailtoBuyer[i].Qty_Price;
                    detail.PriceShipment = getDetailmailtoBuyer[i].PriceShipment;
                    detail.Total = getDetailmailtoBuyer[i].Total;
                    detail.TotalSum = getDetailmailtoBuyer[i].TotalSum;
                    detail.NameStatus = getDetailmailtoBuyer[i].NameStatus;
                    DetailMail.Add(detail);
                }
    
            }
            mailTo.Add("Support@ouikum.com");
            mailTo.Add(email);
            ViewBag.Data = EmailDetail;
            ViewBag.DataDetail = DetailMail;
            //ViewBag.getDetailtobuyer = getDetailmailtoBuyer;
            Detail = PartialViewToString("UC/Email/OrdertoBuyer");
            var subject = res.Email.lblheadorderotbuyer + " " +res.Common.lblWebsite ;
            var mailFrom = res.Config.EmailNoReply;
            return OnSendByAlertEmail(subject, mailFrom, mailTo, mailCC, Detail);
           
        }

        public bool SendEmailOrdertoSeller(List<view_getMailOrdertoSeller> Mailseller, int OrderID)
        {
            #region variable
            
            var email = "";
            var mailTo = "";
            var mailCC = new List<string>();
            var Detailmail = new List<view_EmailToSeller>();
            Hashtable EmailDetail = new Hashtable();
            #endregion
            var getheadmailtoBuyer = svCart.SelectData<View_getSendmailOrder>("*", " OrderID = " + OrderID, null).First();


            string ouikumthai_url2 = res.Pageviews.UrlWeb;
            string pathlogo2 = ouikumthai_url2 + "/Content/Default/logo/Ouikum/img_Logo120x74.png";
            EmailDetail["pathLogo"] = pathlogo2;
            EmailDetail["FirstName"] = getheadmailtoBuyer;
            EmailDetail["NameBuyer"] = getheadmailtoBuyer.namebuyer;
            EmailDetail["TelBuyer"] = getheadmailtoBuyer.SendTel;
            EmailDetail["EmailBuyer"] = getheadmailtoBuyer.Email;
            EmailDetail["Addres"] = getheadmailtoBuyer.addr;
            EmailDetail["Subdistrict"] = getheadmailtoBuyer.SubDirtrict;
            EmailDetail["District"] = getheadmailtoBuyer.DistrictName;
            EmailDetail["Province"] = getheadmailtoBuyer.ProvinceName;
            EmailDetail["Postal"] = getheadmailtoBuyer.Postal;
            EmailDetail["OrderDate"] = getheadmailtoBuyer.OrderDate;
            EmailDetail["TotalSum"] = getheadmailtoBuyer.Total;

            foreach (var it in Mailseller)
            {
                var Detail = "";
                var getheadmailSeller = svCart.SelectData<view_getMailOrdertoSeller>("*", "OrderID = " + it.OrderID + "and emailseller = '" + it.emailseller + "'", null).First();
                EmailDetail["CompName"] = getheadmailSeller.CompName;
                var getDetailtoSeller = svCart.SelectData<view_EmailToSeller>("*", " OrderID = " + it.OrderID + " and emailseller = '" + it.emailseller + "'");
                var chkemail = svCart.SelectData<view_getMailOrdertoSeller>("*", "OrderID = " + it.OrderID + "and emailseller = '" + it.emailseller + "'");
    
                    for (int i = 0; i <= getDetailtoSeller.Count - 1; i++)
                    {
                            var detail = new view_EmailToSeller();
                            detail.ProductName = getDetailtoSeller[i].ProductName;
                            detail.OrDetailCode = getDetailtoSeller[i].OrDetailCode;
                            detail.Qty = getDetailtoSeller[i].Qty;
                            detail.Qty_Price = getDetailtoSeller[i].Qty_Price;
                            detail.PriceShipment = getDetailtoSeller[i].PriceShipment;
                            detail.Total = getDetailtoSeller[i].Total;
                            detail.TotalSum = getDetailtoSeller[i].TotalSum;
                            detail.NameStatus = getDetailtoSeller[i].NameStatus;
                            Detailmail.Add(detail);
                            email = getDetailtoSeller[i].emailseller;
                    }

                    mailTo = email;
                
                    EmailDetail["Emailto"] = email;
                    ViewBag.DataDetail = Detailmail;
                    ViewBag.Data = EmailDetail;
                    Detail = PartialViewToString("UC/Email/OrdertoSeller");
                    var subject = res.Email.lblheadordertoseller + " " + res.Common.lblWebsite;
                    var mailFrom = res.Config.EmailNoReply;
       
                    OnSendByAlertEmail2(subject, mailFrom, mailTo, mailCC, Detail);
                    Detailmail.Clear();
                
            
            }
            
            return true;
            
        }
    }
}
        
