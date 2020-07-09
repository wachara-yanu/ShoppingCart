using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;

using Ouikum.Message;
using Ouikum.Company;
using Prosoft.Service;
using Ouikum.Common;
using Ouikum;
using res = Prosoft.Resource.Web.Ouikum;
using Ouikum.Quotation;
using System.Web.Mail;
using Ouikum.Product;
using Ouikum.Order;

namespace Ouikum.Controllers
{
    public partial class MyPackageController : BaseController
    {
        //
        // GET: /Message/
        string SqlSelect, SQLWhere, SQlOrderBy;
        MemberService svMember;

        public ActionResult Index()
        {
            var svMember = new MemberService();

            if (!CheckIsLogin())
            {
                return Redirect(res.Pageviews.PvMemberSignIn);
            }
            else
            {
                SelectList_PageSize();
                GetStatusUser();
                DoloadExpireMyPackageHotFeat();
                ViewBag.CateLevel1 = LogonCompLevel;
                ViewBag.MemberType = LogonMemberType;
                ViewBag.CompLevel = LogonCompLevel;
                ViewBag.PageType = "MyPackage";

                var OrderDetail = svMember.SelectData<view_OrderDetail>("*", "IsDelete = 0 AND CheckUpdate = 1 AND ODIsInactive = 0 AND CompID = " + LogonCompID, "CreatedDate ASC");
                ViewBag.OrderDetail = OrderDetail;
                ViewBag.OrderDetailCount = OrderDetail.Count();

                return View();
            }
        }

        #region Post: List
        [HttpPost]
        public ActionResult Index(FormCollection form)
        {
            var svOrder = new OrderService();
            SelectList_PageSize();
            SetPager(form);
            SetHotFeatPager(form);
            DoLoadComboBoxHotFeatStatus();
            List_DoloadData();
            
            return PartialView("UC/GridMyPackageHotFead");
        }
        #endregion

        #region SetHotFeatPager
        public void SetHotFeatPager(FormCollection form)
        {
            ViewBag.PStatus = (!string.IsNullOrEmpty(form["PStatus"])) ? DataManager.ConvertToString(form["PStatus"]) : "";
            ViewBag.SearchBy = DataManager.ConvertToString(form["SearchBy"]);
            ViewBag.SearchType = !string.IsNullOrEmpty(form["SearchType"]) ? form["SearchType"] : "";
            ViewBag.Period = !string.IsNullOrEmpty(form["Period"]) ? form["Period"] : "";
            ViewBag.ExpireStatus = !string.IsNullOrEmpty(form["ExpireStatus"]) ? form["ExpireStatus"] : "0";
            ViewBag.SearchStatus = !string.IsNullOrEmpty(form["SearchStatus"]) ? form["SearchStatus"] : "";
        }
        #endregion

        #region List_DoloadData
        public void List_DoloadData()
        {
            var svOrder = new OrderService();
           
            #region Filter : ExpireStatus
            var SQLWhere = "";
            SQLWhere = "IsDelete = 0 AND CheckUpdate = 2 AND ODIsInactive = 0 AND CompID = " + LogonCompID;
            if (int.Parse(ViewBag.ExpireStatus) == 1)
            {
                SQLWhere += " AND ExpiredDate Between '" + DateTime.Now.AddDays(1).ToShortDateString() + "' AND '" + DateTime.Now.AddDays(7).ToShortDateString() + "'";
            }
            else if (int.Parse(ViewBag.ExpireStatus) == 2)
            {
                SQLWhere += " AND ExpiredDate Between '" + DateTime.Now.ToShortDateString() + " 00:00:00' AND '" + DateTime.Now.ToShortDateString() + " 23:59:59'";
            }
            else if (int.Parse(ViewBag.ExpireStatus) == 3)
            {
                SQLWhere += " AND ExpiredDate < '" + DateTime.Now.ToShortDateString() + "'";
            }
            #endregion

            var OrderDetailHot = svOrder.SelectData<view_OrderDetailHotProduct>("*", SQLWhere, "CreatedDate ASC", (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
            ViewBag.OrderDetailHot = OrderDetailHot;
            ViewBag.TotalRow = svOrder.TotalRow;
            ViewBag.TotalPage = svOrder.TotalPage;
        }
        #endregion


        #region Edit Package Gold
        [HttpPost, ValidateInput(false)]
        public ActionResult EditPackageGold(string PackageID, string Price, string Duration, string OrderDetailID)
        {
            b2bMemberPaid mem = new b2bMemberPaid();
            b2bOrder order = new b2bOrder();
            b2bOrderDetail orderDe = new b2bOrderDetail();
            var svOrder = new OrderService();
            var svMember = new MemberService();
            try
            {
                #region Set b2bMemberPaid
                var countMem = svMember.SelectData<b2bMemberPaid>("*", " CreatedDate = GetDate() AND RowFlag > 0");
                int CountMem = countMem.Count + 1;
                mem.MemberPaidCode = AutoGenCode("MPC", CountMem);
                mem.CompID = LogonCompID;
                mem.PaymentStatus = "B";
                mem.IsShow = true;
                mem.IsDelete = false;
                #endregion
                #region Insert b2bMemberPaid
                svMember.InsertMemberPaid(mem);
                #endregion

                #region Set b2bOrder
                order.CompID = LogonCompID;
                order.MemberPaidID = mem.MemberPaidID;
                order.OrderStatus = "B";
                order.TotalPrice = decimal.Parse(Price);
                order.IsShow = true;
                order.IsDelete = false;
                order.RowFlag = 1;
                order.IsInactive = false;
                order.IsSend = false;
                #endregion

                var OrderDetails = new List<b2bOrderDetail>();
                #region Set Model Order Detail
                var countOrder = svMember.SelectData<b2bOrderDetail>("*", " CreatedDate = GetDate() AND RowFlag > 0");
                int CountOrder = countOrder.Count + 1;
                List<int> PackagesId = new List<int>();

                foreach (string ID in PackageID.Split(','))
                {
                    if (!string.IsNullOrEmpty(ID))
                    {
                        int intID;
                        bool isNum = int.TryParse(ID, out intID);
                        if (isNum)
                        {
                            PackagesId.Add(intID);
                        }
                    }
                }
                for (var i = 0; i < PackagesId.Count(); i++)
                {
                    var GetPackage = svMember.SelectData<b2bPackage>("*", "PackageID = " + PackagesId[i]).First();
                    var GetOrderDetail = svMember.SelectData<b2bOrderDetail>("*", "IsDelete = 0 AND PackageID = " + PackagesId[i]).First();

                    var detail = new b2bOrderDetail();
                    detail.OrderType = 2;
                    detail.PackageID = PackagesId[i];
                    detail.RowFlag = 1;
                    detail.IsDelete = false;
                    detail.IsInactive = false;
                    detail.OrderDetailCode = AutoGenCode("ORT", CountOrder);
                    detail.PackagePrice = decimal.Parse(Price);
                    detail.OrderCount = (byte)(DataManager.ConvertToInteger(GetOrderDetail.OrderCount) + 1);
                    detail.OptionValue = GetPackage.OptionValue;
                    detail.OptionValueUnit = GetPackage.OptionValueUnit;
                    detail.Duration = DataManager.ConvertToInteger(Duration);

                    CountOrder++;
                    OrderDetails.Add(detail);

                }
                #endregion

                #region Insert b2bOrder
                svOrder.InsertOrder(order, OrderDetails);
                #endregion

                if (svOrder.IsResult)
                {
                    UpdateInactive(OrderDetailID);
                    SendEmailOrderPackage(Price);
                }
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false });
            }
            return Json(new { IsSuccess = true});
        }
        #endregion

        #region GetEditByID
        public JsonResult GetEditByID(string OrderDetailID = "")
        {
            var svOrder = new OrderService();
            CommonService svCommon = new CommonService();

            var EnumHotFeatStatus = svCommon.SelectEnum(CommonService.EnumType.HotFeatStatus);
            var orderIsShow = svOrder.SelectData<b2bPackage>("PackageID,PackageName,Price,Duratrion", "IsDelete = 0 AND IsShow = 0 AND CheckUpdate = 1", "PackageID ASC");
            var data = svOrder.SelectData<b2bOrderDetail>("OrderDetailID,PackagePrice,Duration,PackageID", "IsDelete = 0 AND OrderDetailID=" + OrderDetailID);

            return Json(new { IsResult = true, Price = (String.Format("{0:##,###.00}", data.First().PackagePrice)), Duration = data.First().Duration, PackageID = data.First().PackageID, EnumHotFeatStatus = EnumHotFeatStatus, orderIsShow = orderIsShow });
        }
        #endregion

        #region GetExpiredDate
        public JsonResult GetExpiredDate()
        {
            var svOrder = new OrderService();
            var PackageID = "";
            var order = svOrder.SelectData<b2bOrder>("OrderID", "IsDelete = 0 AND CompID = " + LogonCompID);

            if (order.Count() > 0)
            {
                string sqlWhere = "IsDelete = 0 AND NOT PackageID IN(19) AND ODIsInactive = 0 AND CompID = " + LogonCompID;
                var dataB = svOrder.SelectData<view_OrderDetail>("PackageID", sqlWhere);

                for (var i = 0; i < dataB.Count(); i++)
                {
                    PackageID += dataB[i].PackageID + ",";
                }

                sqlWhere += " AND ExpiredDate < '" + DateTime.Now.ToShortDateString() + "'";
                var dataE = svOrder.SelectData<view_OrderDetail>("PackageID", sqlWhere);
                for (var i = 0; i < dataE.Count(); i++)
                {
                    PackageID += dataE[i].PackageID + ",";
                }
            }
            return Json(new { IsResult = true, PackageID = PackageID != "" ? PackageID.Substring(0, PackageID.Length - 1) : PackageID, Order = order != null ? order.Count() : 0 });
        }
        #endregion

        #region UpdateInactive
        public JsonResult UpdateInactive(string OrderDetailID = "")
        {
            var svOrder = new OrderService();
            var order = svOrder.SelectData<b2bOrderDetail>(" * ", "OrderDetailID = " + OrderDetailID);
            svOrder.UpdateByCondition<b2bOrderDetail>("IsInactive = 1", "OrderDetailID =" + OrderDetailID);

            return Json(new { IsResult = true, PackageID = order.First().PackageID, TotalPrice = order.First().PackagePrice });
        }
        #endregion

        #region SendEmailOrderPackage
        public bool SendEmailOrderPackage(string TotalPrice)
        {
            var svOrder = new OrderService();
            var svMember = new MemberService();
            #region variable
            var Detail = "";
            var mailTo = new List<string>();
            var mailCC = new List<string>();
            Hashtable EmailDetail = new Hashtable();
            #endregion

            #region Set Content & Value For Send Email
            string b2bthai_url = res.Pageviews.UrlWeb;
            string pathlogo = b2bthai_url + "/Content/Default/logo/Ouikum/img_Logo120x74.png";

            var mem = svMember.SelectData<view_CompMember>("*", "IsDelete = 0 and CompID = " + LogonCompID).First();
            var OrderDetail = svOrder.SelectData<view_OrderDetail>("*", "IsDelete = 0 and CompID = " + LogonCompID, "CreatedDate ASC");

            EmailDetail["b2bthaiUrl"] = b2bthai_url;
            EmailDetail["pathLogo"] = pathlogo;

            EmailDetail["FirstName"] = mem.FirstName;
            EmailDetail["CompName"] = mem.CompName;
            EmailDetail["Phone"] = mem.Phone;
            EmailDetail["Email"] = mem.Email;
            EmailDetail["TotalPrice"] = decimal.Parse(TotalPrice);
            EmailDetail["UrlMyPackage"] = res.Pageviews.UrlWeb + "/MyPackage";
            mailTo.Add(mem.Email);
            ViewBag.Data = EmailDetail;
            ViewBag.OrderDetail = OrderDetail;
            Detail = PartialViewToString("UC/Email/OrderPackage");

            var subject = "คุณ" + mem.FirstName + " แจ้งความต้องการสมัครใช้บริการเสริม - B2BThai.com";
            var mailFrom = res.Config.EmailNoReply;
            #endregion

            try
            {
                return OnSendByAlertEmail(subject, mailFrom, mailTo, mailCC, Detail);
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region DoloadExpireMyPackageHotFeat
        public void DoloadExpireMyPackageHotFeat()
        {
            var svOrder = new OrderService();

            #region Count_Exp
            ViewBag.Count_to_Exp = svOrder.CountData<view_OrderDetailHotProduct>(" * ", @" IsDelete = 0 AND CheckUpdate = 2 AND ODIsInactive = 0 AND CompID =" + LogonCompID + " AND ExpiredDate Between '" + DateTime.Now.AddDays(1).ToShortDateString()
                + "' AND '" + DateTime.Now.AddDays(7).ToShortDateString() + "'");
            #endregion

            #region Count_Exp_today
            ViewBag.Count_Exp_today = svOrder.CountData<view_OrderDetailHotProduct>(" * ", @"IsDelete = 0 AND CheckUpdate = 2 AND ODIsInactive = 0 AND CompID =" + LogonCompID + " AND ExpiredDate Between '" + DateTime.Now.ToShortDateString()
             + " 00:00:00' AND '" + DateTime.Now.ToShortDateString() + " 23:59:59'");
            #endregion

            #region Count_Exp
            ViewBag.Count_Exp = svOrder.CountData<view_OrderDetailHotProduct>(" * ", "IsDelete = 0 AND CheckUpdate = 2 AND ODIsInactive = 0 AND CompID =" + LogonCompID + " AND  ExpiredDate < '" + DateTime.Now.Date.ToShortDateString() + "'");
            #endregion
        }
        #endregion

        #region Delete
        [HttpPost]
        public ActionResult Delete(List<int> ID)
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

    }
}
