using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Transactions;
using System.IO;
using System.Text;
using Prosoft.Service;
//using Prosoft.Base;
using Ouikum.Common;
using Ouikum;
using System.Collections;
using Telerik.Web.Mvc;
using System.Threading;
using Ouikum.Product;
using Ouikum.Company;
using Ouikum.BizType;
using Ouikum.Article;
using System.Text.RegularExpressions;
using res = Prosoft.Resource.Web.Ouikum;
using Ouikum.Message;
using Ouikum.Quotation;
using Ouikum.Web.Models;
using Ouikum.Controllers;
using Ouikum.Shipment;

namespace Ouikum.Web.MyB2B
{
    public class ShipmentController : BaseSecurityController
    {

        #region Members
        // GET: /MyB2B/Company/
        BizTypeService svBizType;
        MemberService svMember;
        AddressService svAddress;
        CompanyService svCompany;
        ProductService svProduct;
        ShipmentService svShipment;
        #endregion

        #region Constructors
        public ShipmentController()
        {
            svBizType = new BizTypeService();
            svMember = new MemberService();
            svAddress = new AddressService();
            svCompany = new CompanyService();
            svProduct = new ProductService();
            svShipment = new ShipmentService();
        }
        #endregion

        // GET: MyB2B/Shipment
        public ActionResult Index()
        {
            var svProduct = new ProductService();
            RememberURL();
            if (!CheckIsLogin())
            {
                return Redirect(res.Pageviews.PvMemberSignIn);
            }
            else
            {
                ViewBag.CateLevel1 = LogonCompLevel;
                ViewBag.MemberType = LogonMemberType;
                var Shipments = svShipment.SelectData<View_ListemShipmentProduct>("*", "IsDelete = 0 and CompID = " + LogonCompID ,"CreatedDate DESC");
                ViewBag.CompLevel = LogonCompLevel;
                ViewBag.ShipmentProduct = Shipments;
                var products = svProduct.SelectData<view_SearchProduct>("ProductID,ProductName", "IsDelete = 0 and CompID =" + LogonCompID);
                ViewBag.Products = products;
                //ViewBag.PageType = "Company";
                //ViewBag.MenuName = "Shipment";
                GetStatusUser();
                SetPager();
                return View();
              
            }
        }

        [HttpPost]
        public ActionResult Index(FormCollection form)
        {
            var svProduct = new ProductService();
            SelectList_PageSize();
            SetPager(form);
            var ShipmentProduct = svShipment.SelectData<View_ListemShipmentProduct>("*", "IsDelete = 0 and ProductID LIKE N'%" + form["ID"] + "%' and CompID = " + LogonCompID, "CreatedDate DESC", (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
            ViewBag.ShipmentProduct = ShipmentProduct;
            ViewBag.TotalPage = svShipment.TotalPage;
            ViewBag.TotalRow = svShipment.TotalRow;
            var products = svProduct.SelectData<view_SearchProduct>("ProductID,ProductName", "IsDelete = 0 and CompID =" + LogonCompID);
            ViewBag.Products = products;

            return PartialView("Grid/ShipmentGrid");
        }

        public void LoadShipmentProduct()
        {
            var svShipment = new ShipmentService();
            var ShipmentProduct = svShipment.SelectData<View_ShipmentProduct>("*", "IsDelete = 0 and CompID =" + LogonCompID);
            ViewBag.ShipmentProduct = ShipmentProduct;
        }

        #region SaveShipmentProductID
        [HttpPost, ValidateInput(false)]
        public bool SaveShipmentProduct(FormCollection form)
        {
            int objState = DataManager.ConvertToInteger(form["objState"]);
            var ShipmentPro = new emShipmentProduct();
            if (objState == 2)
            {
                ShipmentPro = svShipment.SelectData<emShipmentProduct>("*", " ID = " + form["ID"] + " AND RowVersion = " + form["RowVersion"]).First();
            }

            //var ListNo = svShipment.SelectData<emShipmentProduct>("*", "IsDelete = 0 and CompID = " + LogonCompID);
            //int shipment = ListNo.Count() + 1;
            ShipmentPro.ProductID = int.Parse(form["ProductName"]);
            ShipmentPro.CompID = LogonCompID;
            ShipmentPro.BuyMinimum = short.Parse(form["ProductBuyMinimum"]);
            ShipmentPro.BuyMaximum = short.Parse(form["ProductBuyMaximum"]);
            ShipmentPro.PriceShipment = decimal.Parse(form["ProductPriceShippment"]);

            if (objState == 2) {
                ShipmentPro.RowVersion = DataManager.ConvertToShort(ShipmentPro.RowVersion + 1);
            }
            else
            {
                ShipmentPro.RowFlag = 1;
                ShipmentPro.RowVersion = 1;
                ShipmentPro.CreatedBy = "sa";
                ShipmentPro.ModifiedBy = "sa";
                ShipmentPro.ModifiedDate = DateTime.Now;
                ShipmentPro.CreatedDate = DateTime.Now;
                ShipmentPro.ListNo = 1;
                ShipmentPro.IsShow = true;
                ShipmentPro.IsDelete = false;

            }

            #region Save emShipmentProduct
            ShipmentPro = svShipment.SaveData<emShipmentProduct>(ShipmentPro, "ID");
            #endregion

            return svShipment.IsResult;

        }

        #endregion

        #region EditShipmentProduct
        [HttpPost, ValidateInput(false)]
        public ActionResult EditShipmentProduct(FormCollection form)
        { 
            var ShipmentProduct = new emShipmentProduct();
            if (!string.IsNullOrEmpty(form["ID"]))
            {
                ShipmentProduct = svShipment.SelectData<emShipmentProduct>("ID,ProductID,CompID,BuyMinimum,BuyMaximum,PriceShipment,RowVersion", " IsDelete = 0 AND ID =" + form["ID"]).First();
            }
            return Json(new { ID = ShipmentProduct.ID, ProductName = ShipmentProduct.ProductID, ProductBuyMinimum = ShipmentProduct.BuyMinimum, ProductBuyMaximum = ShipmentProduct.BuyMaximum, ProductPriceShippment = ShipmentProduct.PriceShipment, RowVersion = ShipmentProduct.RowVersion });
        }
        #endregion

        /*------------------------DeleteData-----------------------------*/

        #region DeleteData
        public bool DeleteData(List<int> ID, List<short> RowVersion, int KeyValue, string PrimaryKeyName)
        {
            if (PrimaryKeyName == "CompCertifyID")
            {
                svCompany.DeleteData<b2bCompanyCertify>(ID, RowVersion, PrimaryKeyName);
            }
            else if (PrimaryKeyName == "CompPaymentID")
            {
                svCompany.DeleteData<b2bCompanyPayment>(ID, RowVersion, PrimaryKeyName);
            }
            else if (PrimaryKeyName == "CompShipmentID")
            {
                svCompany.DeleteData<b2bCompanyShipment>(ID, RowVersion, PrimaryKeyName);
            }
            else if (PrimaryKeyName == "JobID")
            {
                svAddress.DeleteData<emJob>(ID, RowVersion, PrimaryKeyName);
                svCompany.IsResult = svAddress.IsResult;
            }
            else if (PrimaryKeyName == "ArticleID")
            {
                svCompany.DeleteData<b2bArticle>(ID, RowVersion, PrimaryKeyName);
                if (svCompany.IsResult)
                {
                    svAddress.DeleteData<emArticle>(ID, RowVersion, PrimaryKeyName);
                    svCompany.IsResult = svAddress.IsResult;
                }
            }
            return svCompany.IsResult;
        }
        #endregion

        #region DelData
        public ActionResult DelData(List<bool> Check, List<int> ID, List<int> MemID, List<short> RowVersion, string PrimaryKeyName)
        {
            ShipmentService svCompany = new ShipmentService();

            svCompany.DelData<b2bCompanyCertify>(Check, ID, RowVersion, PrimaryKeyName);


            if (PrimaryKeyName == "CompCertifyID")
            {
                svCompany.DelData<b2bCompanyCertify>(Check, ID, RowVersion, PrimaryKeyName);
            }
            else if (PrimaryKeyName == "CompPaymentID")
            {
                svShipment.DelData<emShipmentProduct>(Check, ID, RowVersion, PrimaryKeyName);
            }
            else if (PrimaryKeyName == "ID")
            {
                svCompany.DelData<emShipmentProduct>(Check, ID, RowVersion, PrimaryKeyName);
            }
            else if (PrimaryKeyName == "JobID")
            {
                JobService svJob = new JobService();
                svJob.DelData<emJob>(Check, ID, RowVersion, PrimaryKeyName);
                svCompany.IsResult = svJob.IsResult;
            }
            else if (PrimaryKeyName == "ArticleID")
            {
                svCompany.DelData<b2bArticle>(Check, ID, RowVersion, PrimaryKeyName);
                if (svCompany.IsResult)
                {
                    ArticleService svArticle = new ArticleService();
                    svArticle.DeleteData<emArticle>(ID, RowVersion, PrimaryKeyName);
                    svCompany.IsResult = svArticle.IsResult;
                }
            }

            if (svCompany.IsResult)
            {
                return Json(new { Result = true });
            }
            else
            {
                return Json(new { Result = false });
            }
        }
        #endregion

    }
}