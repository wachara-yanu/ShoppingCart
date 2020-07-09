using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ouikum.Buylead;
using Ouikum.Category;
using Ouikum.Company;
using Ouikum.Common;
using Ouikum;
using res = Prosoft.Resource.Web.Ouikum;

namespace Ouikum.Web.Purchase
{
    public partial class SearchController : BaseController
    {
        //
        // GET: /Purchase/Search/

        BuyleadService svBuylead;
        CategoryService svCategory;
        CompanyService svCompany;
        Ouikum.BizType.BizTypeService svBizType;
        AddressService svAddress;

        public SearchController()
        {
            svBuylead = new BuyleadService();
            svCategory = new CategoryService();
            svBizType = new Ouikum.BizType.BizTypeService();
            svCompany = new CompanyService();
            svAddress = new AddressService();
        }


        public void List_DoloadData(BuyleadAction action)
        {
            ViewBag.Title = res.Product.lblBuyleadList;
            string sqlSelect, sqlWhere, sqlOrderBy = "";
            var svBuylead = new BuyleadService();

            sqlSelect = "BuyleadID,Qty,QtyUnit,ModifiedDate,BuyleadName,BuyleadNameEng,BuyleadExpDate,BuyleadType,CompBuyleadCount,CompID,CompName,CompLevel,BizTypeName,BizTypeOther,BuyleadImgPath,CateLV3,CategoryName,LogoImgPath";

            #region DoWhereCause
            sqlWhere = svBuylead.CreateWhereAction(action);
            sqlWhere += svBuylead.CreateWhereCause(0, ViewBag.txtSearch, 0, 1, (int)ViewBag.CateID, (int)ViewBag.BuyleadType, 0, (int)ViewBag.ProvinceID);
            //var sqlwherein = "";
            //switch (res.Common.lblWebsite)
            //{
            //    case "B2BThai": sqlwherein = " AND CategoryType IN (1,2)"; break;
            //    case "AntCart": sqlwherein = " AND CategoryType IN (3)"; break;
            //    case "myOtopThai": sqlwherein = " AND CategoryType IN (5)"; break;
            //    case "AppstoreThai": sqlwherein = " AND CategoryType IN (6) "; break;
            //    default: sqlwherein = ""; break;
            //}
            //sqlWhere += sqlwherein;

            if (ViewBag.BuyleadExpDate != 0)
                sqlWhere += " ";
            else if (ViewBag.BuyleadNotExpDate != 0)
                sqlWhere += " AND (convert(nvarchar(20), BuyleadExpDate,112) > '" + DateTime.Today.ToString("yyyyMMdd", new System.Globalization.CultureInfo("en-US")) + "')";
            #endregion

            #region Order By
            if (ViewBag.BuyleadNotExpDate != 0)
                sqlOrderBy = "CreatedDate DESC";
            else
                sqlOrderBy = "BuyleadExpDate DESC";
            //if (ViewBag.CreatedDate != 0)
            //    if (ViewBag.CreatedDate == 1)            
            //        sqlOrderBy = "CreatedDate DESC";
            #endregion

            #region query
            var Buyleads = svBuylead.SelectData<view_BuyLead>(sqlSelect, sqlWhere, sqlOrderBy, (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
            ViewBag.Buyleads = Buyleads;
            if (ViewBag.txtSearch != "")
            {
                ViewBag.Title += ViewBag.txtSearch;
            }
            if (svBuylead.TotalRow > 0)
            {
                if ((int)ViewBag.CateID > 0)
                {
                    ViewBag.Title += " | " + Buyleads.First().CategoryName;
                }
                if ((int)ViewBag.ProvinceID > 0)
                {
                    ViewBag.Title += " | " + Buyleads.First().ProvinceName;
                }
            }
            ViewBag.Title += " | " + res.Common.lblDomainShortName;
            ViewBag.MetaKeyword = ViewBag.Title;
            ViewBag.MetaDescription=ViewBag.Title;
            ViewBag.TotalRow = svBuylead.TotalRow;
            //if (svBuylead.TotalRow.ToString().Length > 2)
            //    ViewBag.TotalRow = String.Format("{0:0,0}", svBuylead.TotalRow);

            ViewBag.TotalPage = svBuylead.TotalPage;
            if (svBuylead.TotalPage.ToString().Length > 2)
                ViewBag.TotalPage = String.Format("{0:0,0}", svBuylead.TotalPage);
            #endregion

            //#region Buyer
            //string CateLV3 = string.Empty;
            //if (ViewBag.Buyleads != null)
            //{
            //    if (Buyleads.Count > 0)
            //    {
            //        for (int x = 0; x < Buyleads.Count; x++)
            //        {
            //            CateLV3 += Buyleads[x].CateLV3 + ",";
            //        }
            //        CateLV3 = CateLV3.Substring(0, CateLV3.Length - 1);
            //        var Buyer = svBuylead.SelectData<view_PurchaseComp>("*", "CateLV3 IN (" + CateLV3 + ") AND RowFlag IN(2,4) AND ProductCount > 0 ", "CompID",0,0);
            //        ViewBag.Buyer = Buyer;
            //    }
            //}
            //#endregion

        }


    }
}
