using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using res = Prosoft.Resource.Web.Ouikum;
//using Prosoft.Base;
using Ouikum.Product;
using Ouikum.Category;
using Ouikum.Company;
using Ouikum.Common;
using Ouikum.BizType;

namespace Ouikum.Web.Search
{
    public partial class SupplierController : BaseController
    {
        CategoryService svCategory;
        CompanyService svCompany;
        BizTypeService svBizType;
        AddressService svAddress;
        ProductService svProduct;

        public SupplierController()
        {
            svCategory = new CategoryService();
            svBizType = new BizTypeService();
            svCompany = new CompanyService();
            svAddress = new AddressService();
            svProduct = new ProductService();
        }

        #region LoadFeatureProduct
        [OutputCache(Duration = 900, VaryByParam = "InterestSupplier")]
        public void LoadInterestSupplier()
        {
            var svCompany = new CompanyService();
            sqlSelect = "CompID,CompName,LogoImgPath,BizTypeName,ProvinceName,CompLevel,ProductCount";
            sqlWhere = " CompLevel = 3 AND ProductCount > 20 AND RowFlag = 2 AND Webid = 1";
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
            var Interest = svCompany.SelectData<view_Company>(sqlSelect, sqlWhere, sqlOrderBy, 1,20);
            ViewBag.Interest = Interest;
            ViewBag.InterestCount = svCompany.TotalRow;
        }
        #endregion
    }
}
