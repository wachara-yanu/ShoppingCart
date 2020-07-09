using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ouikum.Quotation;
using Ouikum.Company;
using Ouikum.Common;
using Ouikum.QuotationAttach;
using Ouikum.Product;
using res = Prosoft.Resource.Web.Ouikum;

namespace Ouikum.Web.Admin
{
    public partial class StatController : BaseSecurityAdminController
    {
        string SqlSelect, SQLWhere, SQlOrderBy;

        #region Quotation
        public ActionResult Quotation()
        {
            CommonService svCommon = new CommonService();
            RememberURL();
            if (!CheckIsLogin())
            {
                return Redirect(res.Pageviews.PvMemberSignIn);
            }
            else
            {
                if (CheckIsAdmin(9) || CheckIsAdmin(15))
                {
                    GetStatusUser();
                    ViewBag.EnumSearchByQuotation = svCommon.SelectEnum(CommonService.EnumType.SearchByQuotation);
                    return View();
                }
                else
                {
                    return Redirect(res.Pageviews.PvAccessDenied);
                }

            }
        }
        #endregion

        #region Get Quotation List
        [HttpGet]
        public ActionResult QuotationList()
        {
            GetStatusUser();
            RememberURL();
            if (!CheckIsLogin())
            {
                return Redirect(res.Pageviews.PvMemberSignIn);
            }
            else
            {
                GetStatusUser();
                SetPager();
                var svQuotation = new QuotationService();
                ViewBag.Inbox = svQuotation.CountData<b2bQuotation>("*", svQuotation.CreateWhereAction(QuotationAction.All));
                ViewBag.Importance = svQuotation.CountData<b2bQuotation>("*", svQuotation.CreateWhereAction(QuotationAction.Important));
                ViewBag.Sentbox = svQuotation.CountData<b2bQuotation>("*", svQuotation.CreateWhereAction(QuotationAction.Sentbox));

                var svCompany = new CompanyService();
                var DataCompany = svCompany.SelectData<view_Company>("CompID,ContactEmail,CompName", "CompID = " + LogonCompID, null, 1, 0, false).First();
                ViewBag.ContactEmail = DataCompany.ContactEmail;
                ViewBag.CompName = DataCompany.CompName;
                return View();
            }

        }
        #endregion

        #region Post Quotation List

        #region SqlWhereQuotationSearchBy
        //Request Price
        public string RSqlWhereQuotationSearchBy(int SearchBy, string SearchText)
        {
            string sqlWhere = string.Empty;
            if (!string.IsNullOrEmpty(SearchText))
            {
                if (SearchBy == 1)
                {
                    sqlWhere = "AND CompanyName Like N'%" + SearchText +"%' ";
                }
                else if (SearchBy == 2)
                {
                    sqlWhere = "AND  ToCompName Like N'%" + SearchText + "%'  ";
                }
                else if (SearchBy == 3)
                {
                    sqlWhere = "AND QuotationCode Like N'%" + SearchText + "%' ";
                }
                else
                {
                    sqlWhere = @"AND (CompanyName Like N'%" + SearchText + "%' OR QuotationCode Like N'%" + SearchText +
                    "%' OR ToCompName Like N'%" + SearchText + "%') ";
                }
            }
            sqlWhere += "AND QuotationStatus ='R' ";
            return sqlWhere;
        }
        //Quotation
        public string QSqlWhereQuotationSearchBy(int SearchBy, string SearchText)
        {
            string sqlWhere = string.Empty;
            if (!string.IsNullOrEmpty(SearchText))
            {
                if (SearchBy == 1)
                {
                    sqlWhere = "AND FromCompName Like N'%" + SearchText + "%' ";
                }
                else if (SearchBy == 2)
                {
                    sqlWhere = "AND  CompanyName Like N'%" + SearchText + "%'  ";
                }
                else if (SearchBy == 3)
                {
                    sqlWhere = "AND QuotationCode Like N'%" + SearchText + "%' ";
                }
                else
                {
                    sqlWhere = @"AND (CompanyName Like N'%" + SearchText + "%' OR QuotationCode Like N'%" + SearchText +
                    "%' OR FromCompName Like N'%" + SearchText + "%') ";
                }
            }
            sqlWhere += "AND QuotationStatus ='Q' ";
            return sqlWhere;
        }
        #endregion

        [HttpPost]
        public ActionResult QuotationList(FormCollection form)
        {
            SelectList_PageSize();
            SetPager(form);
            int CompID = LogonCompID;

            var svQuotation = new QuotationService();

            SqlSelect = "QuotationID,QuotationCode,FromCompID,ToCompID,CompanyName,ReqFirstName,ReqLastName,ReqEmail,SendDate,RowFlag,RowVersion,IsDelete,IsRead,IsOutbox,IsReject,IsImportance,QuotationStatus,FromCompName,ToCompName,CategoryType,WebID,FromWebID";
            SQLWhere = svQuotation.CreateWhereAction(QuotationAction.Admin, 0);
            if (form["QuoStatus"] != "Q")
            {
                //Require Price
                SQLWhere += RSqlWhereQuotationSearchBy(int.Parse(form["SearchBy"]), form["SearchText"]);
            }
            else
            {   
                //Quotation
                SQLWhere += QSqlWhereQuotationSearchBy(int.Parse(form["SearchBy"]), form["SearchText"]);
            }
            if (!string.IsNullOrEmpty(form["Period"]))
            {
                SQLWhere += SQLWhereDateTimeFromPeriod(form["Period"], "SendDate");
            }

            ViewBag.Quotations = svQuotation.SelectData<view_Quotation>(SqlSelect, SQLWhere, "CreatedDate DESC", (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
            ViewBag.TotalPage = svQuotation.TotalPage;
            ViewBag.TotalRow = svQuotation.TotalRow;

            return PartialView("UC/QuotationList");

        }
        #endregion

        #region Detail
        public ActionResult QuotationDetail(int QuotationID = 0, string QuotationCode = "")
        {
            int CompID = 0;
            int ProductID = 0;
            RememberURL();
            if (!CheckIsLogin())
            {
                return Redirect(res.Pageviews.PvMemberSignIn);
            }
            else
            {
                if (CheckIsAdmin(9))
                {
                    GetStatusUser();
                    if (QuotationID > 0)
                    {
                        Ouikum.Quotation.QuotationService svQuotation = new Ouikum.Quotation.QuotationService();
                        var Quotation = svQuotation.SelectData<view_Quotation>("*", "IsDelete = 0 AND QuotationID =" + QuotationID).First();
                        ViewBag.Quotation = Quotation;
                        if (Quotation.QuotationStatus == "Q")
                        {
                            CompID = Convert.ToInt32(Quotation.FromCompID);
                        }
                        else
                        {
                            CompID = Convert.ToInt32(Quotation.ToCompID);
                        }
                        if (Quotation.ProductID != 0)
                            ProductID = Convert.ToInt32(Quotation.ProductID);

                        #region ProductName
                        var svProduct = new ProductService();
                        var ProductName = svProduct.SelectData<view_SearchProduct>("ProductID,ProductName", "ProductID = " + ProductID, "ProductName", 1, 0, false).First().ProductName;
                        if (ProductName.Length >= 40)
                        {
                            ViewBag.ProductName = ProductName.Substring(0, 40) + "...";
                        }
                        else
                        {
                            ViewBag.ProductName = ProductName;
                        }
                        #endregion

                        #region Company
                        var svCompany = new CompanyService();
                        if (CompID != 0)
                        {
                            var ToComp = svCompany.SelectData<view_Company>("CompID,CompName,CompImgPath,BizTypeName,ProvinceName,ContactEmail", "CompID = " + CompID, null, 1, 0, false).First();
                            ViewBag.ReqEmail = ToComp.ContactEmail;
                            ViewBag.ReqPhone = ToComp.ContactPhone;
                        }
                        else
                        {
                            ViewBag.ReqEmail = null;
                            ViewBag.ReqPhone = null;
                        }
                       
                        #endregion

                        #region File Attach
                        var svQuotationAttach = new QuotationAttachService();
                        var AttachFile = svQuotationAttach.SelectData<b2bQuotationAttach>("*", "QuotationID = " + QuotationID, null, 0, 0, false);
                        if (AttachFile.Count() > 0)
                        {
                            ViewBag.AttachFile = AttachFile.First();
                            ViewBag.AttachRemark = AttachFile.First().Remark;
                        }
                        #endregion

                        return View();
                    }
                    else
                    {
                        return Redirect("~/Report/List");
                    }
                }
                else
                {
                    return Redirect(res.Pageviews.PvAccessDenied);
                }
            }
        }
        #endregion
    }
}
