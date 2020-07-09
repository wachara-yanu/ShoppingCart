using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ouikum.Buylead;
using Ouikum.Order;
using Ouikum.Product;
using System.Collections;
using res = Prosoft.Resource.Web.Ouikum;
namespace Ouikum.Web.Admin
{
    public partial class ApprovePackageController : BaseSecurityAdminController
    {
        //
        // GET: /Admin/ApproveBuylead/ 
        public void List_DoloadData(BuyleadAction action)
        {
            var svOrder = new OrderService();
            string  sqlWhere, sqlOrderBy = "";
            sqlWhere = "IsDelete = 0 ";

            sqlOrderBy = " CreatedDate DESC ";

            #region DoWhereCause
            sqlWhere += svOrder.CreateWhereCause("", ViewBag.PStatus, ViewBag.PSType);
            sqlWhere += svOrder.CreateWhereSearchBy(ViewBag.TextSearch, ViewBag.SearchType);

            if (!string.IsNullOrEmpty(ViewBag.Period))
                sqlWhere += SQLWhereDateTimeFromPeriod(ViewBag.Period, "CreatedDate");
            #endregion

            var OrderComp = svOrder.SelectData<view_OrderDetailComp>("*", sqlWhere, sqlOrderBy, (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
            ViewBag.OrderComp = OrderComp;
            ViewBag.LogonCompID = LogonCompID;
            ViewBag.TotalRow = svOrder.TotalRow;
            ViewBag.TotalPage = svOrder.TotalPage;
        }

        #region Approve
        [HttpPost]
        public ActionResult Approve(List<int> ID, List<int> CateLV1, List<int> CateLV2, List<int> CateLV3)
        {
            var svOrder = new OrderService();
            try
            {
                svOrder.ApproveOrderPackage(ID, LogonCompCode);
                //if (svBuylead.IsResult)
                //{
                //    for (var i = 0; i < ID.Count(); i++)
                //    {
                //        var Buylead = svBuylead.SelectData<b2bBuylead>("BuyleadID, BuyleadCode, BuyleadName, BuyleadCompanyName, BuyleadContactPerson, BuyleadEmail, RowFlag, Remark", "BuyleadID = N'" + ID[i] + "'");
                //        if (Buylead.Count > 0)
                //        {
                //            var model = Buylead.First();
                //            SendEmailApproveBuylead(model);
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                CreateLogFiles(ex);
            }
            return Json(new { IsResult = svOrder.IsResult, MsgError = "" });
        }
        #endregion

        #region Reject
        [HttpPost]
        public ActionResult Reject(List<int> ID, List<int> CateLV1, List<int> CateLV2, List<int> CateLV3, string Remark)
        {
            var svOrder = new OrderService();
            try
            {
                svOrder.RejectOrder(ID, Remark, LogonCompCode);
                //if (svBuylead.IsResult)
                //{
                //    for (var i = 0; i < ID.Count(); i++)
                //    {
                //        var Buylead = svBuylead.SelectData<b2bBuylead>("BuyleadID, BuyleadCode, BuyleadName, BuyleadCompanyName, BuyleadContactPerson, BuyleadEmail, RowFlag, Remark", "BuyleadID = N'" + ID[i] + "'");
                //        if (Buylead.Count > 0)
                //        {
                //            var model = Buylead.First();
                //            SendEmailApproveBuylead(model);
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                CreateLogFiles(ex);
            }
            return Json(new { IsResult = svOrder.IsResult, MsgError = "" });
        }
        #endregion

        #region Delete
        [HttpPost]
        public ActionResult Delete(List<int> ID, List<int> CateLV1, List<int> CateLV2, List<int> CateLV3, List<int> CompID)
        {
            var svOrder = new OrderService();
            try
            {
                svOrder.Delete(ID);
            }
            catch (Exception ex)
            {
                svOrder.MsgError.Add(ex);
                CreateLogFiles(ex);
            }
            return Json(new { IsResult = svOrder.IsResult, MsgError = GenerateMsgError(svOrder.MsgError) });
        }
        #endregion

        #region CheckHotFeat
        [HttpPost]
        public ActionResult CheckHotFeat(List<int> OrderID)
        {
            var svOrder = new OrderService();
            for (var i = 0; i < OrderID.Count(); i++)
            {
                var OrderDetail = svOrder.SelectData<b2bOrderDetail>("*", "OrderID = " + OrderID[i]);
                for (var j = 0; j < OrderDetail.Count(); j++)
                {
                    var HotFeat = svOrder.SelectData<b2bHotFeaProduct>("*", "OrderDetailID = " + OrderDetail[j].OrderDetailID);
                    if (HotFeat.Count() > 0)
                    {
                        return Json(new { IsResult = false, MsgError = GenerateMsgError(svOrder.MsgError) });
                    }
                }
            }
            return Json(new { IsResult = true, MsgError = GenerateMsgError(svOrder.MsgError) });
        }
        #endregion


        #region SendEmailApproveBuylead
        public bool SendEmailApproveBuylead(b2bBuylead model)
        {
            #region variable
            bool IsSend = true;
            var Detail = "";
            var url = "";
            var urlBuylead = "";
            var mailTo = new List<string>();
            var mailCC = new List<string>();

            var svHotFeat = new HotFeaProductService();
            var SQLSelect_Feat = "";

            SQLSelect_Feat = " ProductID,ProductName,CompID,ProductImgPath,ProRowFlag,CompRowFlag,ProvinceName,Price,Ispromotion,PromotionPrice,HotPrice";
            var HotProduct = svHotFeat.SelectHotProduct<view_HotFeaProduct>(SQLSelect_Feat, "Rowflag = 3 AND Status = 'H' AND ProductID > 0 AND ProRowFlag in(2,4) AND CompRowFlag in(2,4) AND ProductDelete = 0", "NEWID(),HotPrice DESC", 1, 4);
            #endregion

            #region Set Content & Value For Send Email
            string urlb2bthai = res.Pageviews.UrlWeb;
            url = urlb2bthai + "/MyB2B/buylead";
            urlBuylead = urlb2bthai + "/Purchase/Search/Detail/" + model.BuyleadID + "?name=" + model.BuyleadName;

            //test path logo
            string b2bthai_url = res.Pageviews.UrlWeb;
            string pathlogo = b2bthai_url + "/Content/Default/logo/Ouikum/img_Logo120x74.png";

            Hashtable EmailDetail = new Hashtable();
            EmailDetail["Name"] = model.BuyleadContactPerson;
            EmailDetail["CompName"] = model.BuyleadCompanyName;
            EmailDetail["BuyleadCode"] = model.BuyleadCode;
            EmailDetail["BuyleadName"] = model.BuyleadName;
            EmailDetail["Remark"] = model.Remark;
            EmailDetail["pathLogo"] = pathlogo;
            EmailDetail["url"] = url;
            EmailDetail["urlBuylead"] = urlBuylead;

            if (model.RowFlag == 4)
            {
                EmailDetail["Title"] = "แจ้งผลอนุมัติประกาศซื้อสินค้าของคุณ";
                EmailDetail["Result"] = res.Admin.lblApprove;
            }
            else if (model.RowFlag == 3)
            {
                EmailDetail["Title"] = "แจ้งผลไม่อนุมัติประกาศซื้อสินค้าของคุณ";
                EmailDetail["Result"] = res.Admin.lblReject;
            }

            ViewBag.Data = EmailDetail;
            ViewBag.HotProduct = HotProduct;

            string Subject = "แจ้งผลการตรวจสอบประกาศซื้อสินค้า B2BThai.com";

            Detail = PartialViewToString("UC/Email/SendApproveBuylead");

            var mailFrom = res.Config.EmailNoReply;
            mailTo.Add(model.BuyleadEmail);
            #endregion

            IsSend = OnSendByAlertEmail(Subject, mailFrom, mailTo, mailCC, Detail);

            return IsSend;
        }
        #endregion

    }
}
