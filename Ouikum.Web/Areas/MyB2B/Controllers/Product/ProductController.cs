using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Ouikum;
//using Prosoft.Base;
using Ouikum.Product;
using Ouikum.Category;
using res = Prosoft.Resource.Web.Ouikum;
using Ouikum.Message;
using Ouikum.Quotation;

namespace Ouikum.Web.MyB2B
{
    public partial class ProductController : BaseSecurityController
    {
        public string sqlSelect, sqlWhere, sqlOrderBy, GenProductCode = "",sqlwherein;
        
        public void GetStatusByCompID(int CompID)
        {
            var svProduct = new ProductService();
            //switch (res.Common.lblWebsite)
            //{
            //    case "B2BThai": sqlwherein = " AND CategoryType IN (1,2)"; break;
            //    case "AntCart": sqlwherein = " AND CategoryType IN (3)"; break;
            //    case "myOtopThai": sqlwherein = " AND CategoryType IN (5)"; break;
            //    case "AppstoreThai": sqlwherein = " AND CategoryType IN (6)"; break;
            //    default: sqlwherein = ""; break;
            //}
            string sqlwhere = svProduct.CreateWhereAction(ProductAction.Junk, CompID);
            var CountJunk = svProduct.CountData<view_SearchProduct>("ProductID", sqlwhere);
            ViewBag.CountJunk = CountJunk;

            sqlwhere = svProduct.CreateWhereAction(ProductAction.BackEnd, CompID);
            var CountProduct = svProduct.CountData<view_SearchProduct>("ProductID", sqlwhere);
            ViewBag.CountProduct = CountProduct;

            sqlwhere = svProduct.CreateWhereAction(ProductAction.Recommend, CompID);
            var CountProductRecomm = svProduct.CountData<view_SearchProduct>("ProductID", sqlwhere);
            ViewBag.CountProductRecomm = CountProductRecomm;

            sqlwhere = svProduct.CreateWhereAction(ProductAction.All, CompID);
            var CountAllitem = svProduct.CountData<view_SearchProduct>("ProductID", sqlwhere);
            ViewBag.CountAllitem = CountAllitem; 

            sqlwhere += svProduct.CreateWhereCause(0,"",4);
            var CountProductApprove = svProduct.CountData<view_SearchProduct>("ProductID", sqlwhere);
            ViewBag.CountProductApprove = CountProductApprove;
        }

        public ActionResult GetStatus()
        {
            GetStatusByCompID(LogonCompID);
            return Json(new
            {
                CountJunk = (int)ViewBag.CountJunk,
                CountProduct = (int)ViewBag.CountProduct,
                CountProductRecomm = (int)ViewBag.CountProductRecomm,
                CountProductApprove= (int)ViewBag.CountProductApprove,
                CountProductWait = ((int)ViewBag.CountAllitem - (int)ViewBag.CountProductApprove),
                CountAllitem = (int)ViewBag.CountAllitem
            });
        }


        public void List_DoloadData(ProductAction action)
        {
            ViewBag.PageSize = 19;
            var svProduct = new ProductService();
            sqlSelect = "ProductID,ProductName,Price_One,Qty,QtyUnit,Price,ProductImgPath,Remark,RowFlag,CompID,CateLV1,CateLV2,CateLV3,RowVersion";
            //if ((int)ViewBag.GroupID > 0) 
            //{   }
                sqlWhere = svProduct.CreateWhereAction(action); 
            //else{
            //    sqlWhere = svProduct.CreateWhereAction(action) + " AND ListNo = 0 ";
            //}
            //switch (res.Common.lblWebsite)
            //{
            //    case "B2BThai": sqlwherein = " AND CategoryType IN (1,2)"; break;
            //    case "AntCart": sqlwherein = " AND CategoryType IN (3)"; break;
            //    case "myOtopThai": sqlwherein = " AND CategoryType IN (5)"; break;
            //    case "AppstoreThai": sqlwherein = " AND CategoryType IN (6)"; break;
            //    default: sqlwherein = ""; break;
            //}
            //sqlWhere += sqlwherein;
            sqlOrderBy = " ModifiedDate DESC ";

            #region DoWhereCause
            sqlWhere += svProduct.CreateWhereCause(LogonCompID, ViewBag.TextSearch, (int)ViewBag.PStatus, (int)ViewBag.GroupID,
                (int)ViewBag.CateLevel, (int)ViewBag.CateID);

            if (!string.IsNullOrEmpty(ViewBag.Period))
                sqlWhere += SQLWhereDateTimeFromPeriod(ViewBag.Period, "ModifiedDate");
            #endregion

            var products = svProduct.SelectData<view_SearchProduct>(sqlSelect, sqlWhere, sqlOrderBy, (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
            ViewBag.Products = products;
            ViewBag.LogonCompID = LogonCompID;
            ViewBag.TotalRow = svProduct.TotalRow;
            ViewBag.TotalPage = svProduct.TotalPage;

            if (LogonCompLevel == 1)
            {
                svProduct.ValidateFullProduct(LogonCompID, LogonCompLevel);
                if (svProduct.IsResult)
                    ViewBag.IsMaxProduct = true;
            }
        }


        #region PostSearchCategory
        [HttpPost]
        public ActionResult SearchCategory(string CategoryName)
        {
            if (!string.IsNullOrEmpty(CategoryName))
            {
                var svCategory = new CategoryService();
                //switch (res.Common.lblWebsite)
                //{
                //    case "B2BThai": sqlwherein = " AND CategoryType IN (1,2)"; break;
                //    case "AntCart": sqlwherein = " AND CategoryType IN (3)"; break;
                //    case "myOtopThai": sqlwherein = " AND CategoryType IN (5)"; break;
                //    case "AppstoreThai": sqlwherein = " AND CategoryType IN (6)"; break;
                //    default: sqlwherein = ""; break;
                //}
                //if (Base.AppLang == "en-Us") { sqlSelect = "CategoryNameEng AS CategoryName"; }
                //else { sqlSelect = "CategoryName"; }
                //var data = svCategory.SearchCategoryByName(sqlSelect, sqlwherein);
                //ViewBag.Categories = data;
                //ViewBag.Countcategory = data.Count();
                var data = svCategory.SearchCategoryByName(CategoryName, sqlwherein);
                ViewBag.Categories = data;
                ViewBag.Countcategory = svCategory.TotalRow;
            }
            return PartialView("UC/CategoryList");
        }

        #endregion 

        #region SearchCategoryByID
        [HttpPost]
        public ActionResult SearchCategoryByID(int CategoryID)
        {
            var svCategory = new CategoryService();
            var cate3 = svCategory.SearchCategoryByID((int)CategoryID);
            var cate2 = svCategory.SearchCategoryByID((int)cate3[0].ParentCategoryID);
            var cate1 = svCategory.SearchCategoryByID((int)cate2[0].ParentCategoryID);
            return Json(new { IsResult = svCategory.IsResult, cate1 = cate1[0].CategoryID, cate2 = cate2[0].CategoryID });
        }

        #endregion 

        #region CountMessage
        public void CountMessage()
        {
            MessageService svMessage = new MessageService();
            ViewBag.CountInbox = svMessage.SelectData<emMessage>("*", svMessage.CreateWhereAction(MessageStatus.UnRead, LogonCompID), null, 0, 0).Count();
            ViewBag.CountImportance = svMessage.SelectData<emMessage>("*", svMessage.CreateWhereAction(MessageStatus.Important, LogonCompID), null, 0, 0).Count();
            ViewBag.CountDraftbox = svMessage.SelectData<emMessage>("*", svMessage.CreateWhereAction(MessageStatus.Draft, LogonCompID), null, 0, 0).Count();
            ViewBag.CountSentbox = svMessage.SelectData<emMessage>("*", svMessage.CreateWhereAction(MessageStatus.Sentbox, LogonCompID), null, 0, 0).Count();
        }
        #endregion

        #region CountQuotation
        public void CountQuotation()
        {
            var svQuotation = new QuotationService();
            ViewBag.Inbox = svQuotation.CountData<b2bQuotation>("*", "( IsDelete = 0 ) AND (IsRead = 'False') AND (IsOutBox = 'False')  AND (IsRead = 0) AND (ToCompID = " + LogonCompID + ")");
            ViewBag.Importance = svQuotation.CountData<b2bQuotation>("*", "( ToCompID = " + LogonCompID + " AND IsOutBox = 0 AND IsDelete = 0 AND  IsImportance = 1 ) OR (FromCompID = " + LogonCompID + " AND IsOutBox = 1 AND IsDelete = 0 AND  IsImportance = 1 )");
            ViewBag.Sentbox = svQuotation.CountData<b2bQuotation>("*", "( IsDelete = 0 AND  IsOutbox = 1 AND FromCompID = " + LogonCompID + " )");
        }
        #endregion
    }
}
