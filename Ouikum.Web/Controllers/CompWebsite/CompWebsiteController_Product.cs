using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Ouikum.Category;
using Ouikum.Company;
using Ouikum.Product;
//using Prosoft.Base;
using Prosoft.Service;
using res = Prosoft.Resource.Web.Ouikum;

namespace Ouikum.Controllers
{
    public partial class CompWebsiteController : BaseController
    {
        #region Get Product
        [HttpGet]
        public ActionResult Product(int? id, int? GroupID, int? CateID, int? CateLevel, string textsearch)
        {
            if (RedirectToProduction())
                return Redirect(UrlProduction);

            string page = "Product";
            int countcompany = DefaultWebsite((int)id, page);

            if (countcompany > 0)
            {
                SetPager();
                SelectList_PageSize();
                ViewBag.PageIndex = 1;
                ViewBag.PageSize = 20;
                ViewBag.TotalRow = 0;
                ViewBag.GroupID = GroupID != null ? GroupID : 0;
                ViewBag.CateID = CateID != null ? CateID : 0;
                ViewBag.CateLevel = CateLevel != null ? CateLevel : 0;
                ViewBag.CompID = id;
                ViewBag.TextSearch = textsearch;


                //if (ViewBag.ProductCount > 0)
                //{
                    if (LogonCompID == ViewBag.CompID)
                    {
                        List_DoloadData(ProductAction.WebSite);
                    }
                    else
                    {
                        List_DoloadData(ProductAction.FrontEnd);
                    }

                    string sqlSelect = "CompID,CompName,CompLevel,LogoImgPath,ProvinceName,CompAddrLine1,CompPhone,ContactEmail,CompCode,BizTypeOther,BizTypeName,CreatedDate,ServiceType";
                    string sqlWhere = "IsDelete = 0 AND CompID =" + id;
                    //var sqlwhereinweb = "";
                    //switch (res.Common.lblWebsite)
                    //{
                    //    case "B2BThai": sqlwhereinweb = " AND WebID = 1 "; break;
                    //    case "AntCart": sqlwhereinweb = " AND WebID = 3 "; break;
                    //    case "myOtopThai": sqlwhereinweb = " AND WebID = 5 "; break;
                    //    case "AppstoreThai": sqlwhereinweb = " AND WebID = 6 "; break;
                    //    default: sqlwhereinweb = ""; break;
                    //}
                    //sqlWhere += sqlwhereinweb;
                    var company = svCompany.SelectData<view_Company>(sqlSelect, sqlWhere).First();
                    ViewBag.Company = company;
                    ViewBag.PageType = "Product";
                    GetStatusUser();
                    if (company.ProvinceName == "กรุงเทพมหานคร")
                    {
                        company.ProvinceName = "กรุงเทพ";
                    }
                    ViewBag.title = res.Common.lblProduct + " " + company.CompName + " | " + company.ProvinceName + " | " + res.Common.lblDomainShortName;
                    ViewBag.MetaDescription = res.Common.lblProduct + " " + company.CompName + " | " + company.CompAddrLine1 + " | " + company.ProvinceName + " | " + company.CompPhone;
                    ViewBag.MetaKeyword = ViewBag.title;

                    var Comp = SelectCompany();
                    ViewBag.Comp = Comp;
                    ViewBag.PageType = "Product";
                    return View();
                //}
                //else
                //{
                //    GetStatusUser();
                //    LinkPathCompanyWebsite((string)ViewBag.WebCompName, (int)id);
                //    return Redirect(PathWebsiteHome);
                //}
            }
            else
            {
                return Redirect(res.Pageviews.PvNotFound);
            }
        }
        #endregion

        #region Post Product
        [HttpPost]
        public ActionResult Product(FormCollection form)
        {
            SetPager(form);
            SetProductPager(form);

            svProduct = new ProductService();
            var products = 0;
            if (LogonCompID == Convert.ToInt32(form["CompID"]))
            {
                string sqlSelect = "CompID,ProductID,ProductName";
                string sqlWhere = svProduct.CreateWhereAction(ProductAction.WebSite, Convert.ToInt32(form["CompID"]));
                products = svProduct.CountData<view_SearchProduct>(sqlSelect, sqlWhere);
                if (products > 0)
                {
                    ViewBag.ProductCount = products;
                    ViewBag.ProductType = 1;
                }
            }
            else
            {
                string sqlSelect = "CompID,ProductID,ProductName";
                string sqlWhere = svProduct.CreateWhereAction(ProductAction.FrontEnd, Convert.ToInt32(form["CompID"]));
                products = svProduct.CountData<view_SearchProduct>(sqlSelect, sqlWhere);
                if (products > 0)
                {
                    ViewBag.ProductCount = products;
                    ViewBag.ProductType = 1;
                }
            }

            if (LogonCompID == ViewBag.CompID)
            {
                List_DoloadData(ProductAction.WebSite);
            }
            else
            {
                List_DoloadData(ProductAction.FrontEnd);
            }

            return PartialView("UC/ProductGalleryUC");
        }

        #endregion

        #region Product Detail
        [HttpGet]
        public ActionResult ProductDetail(int? id)
        {
            if (RedirectToProduction())
                return Redirect(UrlProduction);

            GetStatusUser();
            svProductImage = new ProductImageService();
            svProduct = new ProductService();
            svCompany = new CompanyService();
            string page = "Product";
            int CompID = Convert.ToInt32(Request.Cookies["WebsiteCompID"].Value);
            DefaultWebsite((int)CompID, page);

            var Products = svProduct.SelectData<view_Product>("*", "ProductID = " + id, null, 1, 0);
            ViewBag.Product = Products;

            if (Products != null)
            {
                ViewBag.ProductDetail = Products.First();

                string sqlWhere = "CompID = " + CompID + " AND RowFlag > 0 AND IsShow = 1 AND IsJunk = 0";
                var ImgProducts = svProduct.SelectData<view_Product>("ProductID,ProductImgPath,ProductName", sqlWhere, null, 1, 0, false);
                ViewBag.ProductImageAll = ImgProducts;
                ViewBag.ImageCount = ImgProducts.Count;

                var ProductImages = svProductImage.SelectData<view_ProductImage>("*", "ProductID = " + id, null, 1, 0, false);
                ViewBag.ProductImage = ProductImages;

                var Companies = svCompany.SelectData<view_Company>("*", "CompID = " + CompID, null, 1, 0, false);
                ViewBag.Company = Companies.First();

                var Shipments = svProduct.SelectData<view_ProductShipment>("*", "CompID = " + CompID, null, 1, 0, false);
                ViewBag.ProductShipment = Shipments;

                var Payments = svProduct.SelectData<view_ProductPayment>("*", "CompID = " + CompID, null, 1, 0, false);
                ViewBag.ProductShipment = Payments;

                ViewBag.CompID = CompID;

            }
            ViewBag.ProID = (int)id;


            return View();

        }
        #endregion
    }
}
