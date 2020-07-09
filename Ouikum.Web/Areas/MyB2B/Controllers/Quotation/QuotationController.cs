using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;
using System.IO;
using Ouikum.Quotation;
using Ouikum.Company;
using Ouikum.Product;
using Ouikum.QuotationAttach;
using Ouikum.Common;
using Prosoft.Service;
using Twitterizer;
using System.Xml.Linq;
using Ouikum;
using res = Prosoft.Resource.Web.Ouikum;
using Ouikum.Message;
using System.Threading;
namespace Ouikum.Web.MyB2B
{
    public partial class QuotationController : BaseController
    {
        #region Member
        QuotationService svQuotation;
        CompanyService svCompany;
        HotFeaProductService svHotFeaProduct;
        MemberService svMember;
        ProductService svProduct;
        AddressService svAddress;
        QuotationAttachService svQuotationAttach;
        #endregion

        string SqlSelect, SQLWhere, SQLUpdate = "";

        /*------------------------Quotation-List---------------------*/
        #region Get Quotation List
        [HttpGet]
        public ActionResult List(string Type)
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
                CountMessage();
                ViewBag.CateLevel1 = LogonCompLevel;
                ViewBag.MemberType = LogonMemberType;
                var svQuotation = new QuotationService();
                ViewBag.Inbox = svQuotation.CountData<b2bQuotation>("*", "( IsDelete = 0 ) AND (IsRead = 'False') AND (IsOutBox = 'False')  AND (IsRead = 0) AND (ToCompID = " + LogonCompID + ")");
                ViewBag.Importance = svQuotation.CountData<b2bQuotation>("*", "( ToCompID = " + LogonCompID + " AND IsOutBox = 0 AND IsDelete = 0 AND  IsImportance = 1 ) OR (FromCompID = " + LogonCompID + " AND IsOutBox = 1 AND IsDelete = 0 AND  IsImportance = 1 )");
                ViewBag.Sentbox = svQuotation.CountData<b2bQuotation>("*", "( IsDelete = 0 AND  IsOutbox = 1 AND FromCompID = " + LogonCompID + " )");

                var svCompany = new CompanyService();
                var DataCompany = svCompany.SelectData<view_Company>("CompID,ContactEmail,CompName", "CompID = " + LogonCompID, null, 1, 0, false).First();
                ViewBag.ContactEmail = DataCompany.ContactEmail;
                ViewBag.CompName = DataCompany.CompName;
                if (!string.IsNullOrEmpty(Type))
                {
                    ViewBag.Type = Type;
                    ViewBag.MenuName = Type;
                }
                else { 
                    ViewBag.Type = "Inbox";
                    ViewBag.MenuName = "Inbox";
                }
                ViewBag.PageType = "Quotation";
                

                return View();
            }

        }
        #endregion

        #region Post Quotation List
        [HttpPost]
        public ActionResult List(FormCollection form)
        {
            SelectList_PageSize();
            SetPager(form);

            int CompID = LogonCompID;
            string type = form["TypeQuotation"];
            string status = form["QuotationStatus"];

            if (DataManager.ConvertToInteger(form["PIndex"]) == 1)
            {
                ViewBag.PageIndex = DataManager.ConvertToInteger(form["PIndex"]);
            }


            var svQuotation = new QuotationService();
            SqlSelect = "QuotationID,QuotationCode,FromCompID,CompanyName,ReqFirstName,ReqLastName,ReqEmail,SendDate,RowFlag,RowVersion,IsDelete,IsRead,IsReject,IsImportance,QuotationStatus,IsOutbox,ProductName,Qty,QtyUnit,QuotationFolderID";

            if (!string.IsNullOrEmpty(type))
            {
                if (type == "Importance")
                {
                    SQLWhere = "( ToCompID = " + LogonCompID + " AND IsOutBox = 0 AND IsDelete = 0 AND  IsImportance = 1 AND QuotationFolderID != 4 ) OR (FromCompID = " + LogonCompID + " AND IsOutBox = 1 AND IsDelete = 0 AND  IsImportance = 1) ";
                    SQLWhere += svQuotation.CreateWhereCause(form["TextSearch"]);
                }
                else if (type == "Sentbox")
                {
                    SQLWhere = svQuotation.CreateWhereAction(QuotationAction.Sentbox, CompID);
                    SQLWhere += svQuotation.CreateWhereCause(form["TextSearch"]);
                }
                else if (type == "Trash")
                {
                    SQLWhere = svQuotation.CreateWhereAction(QuotationAction.Trash, CompID);
                    SQLWhere += svQuotation.CreateWhereCause(form["TextSearch"]);
                }
                else if (type == "Inbox")
                {
                    SQLWhere = svQuotation.CreateWhereAction(QuotationAction.BackEnd, CompID);
                    SQLWhere += svQuotation.CreateWhereCause(form["TextSearch"]);
                }
            }
            if (!string.IsNullOrEmpty(status))
            {
                if (status == "J")
                {
                    SQLWhere += " AND IsReject = 1 ";
                }
                else
                {
                    SQLWhere += " AND QuotationStatus ='" + status + "' ";
                }
               
            }

            ViewBag.Quotations = svQuotation.SelectData<View_QuotationList>(SqlSelect, SQLWhere, "CreatedDate DESC", (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
            ViewBag.TotalPage = svQuotation.TotalPage;
            ViewBag.TotalRow = svQuotation.TotalRow;
            return PartialView("UC/QuotationListUC");

        }
        #endregion

        /*--------------MarkRead---------------*/
        #region MarkRead
        public bool MarkRead(List<bool> Check, List<int> ID, List<short> RowVersion, string PrimaryKeyName)
        {
            var svQuotation = new QuotationService();
            svQuotation.ChangeRead<b2bQuotation>(Check, ID, RowVersion, PrimaryKeyName, "IsRead");
            if (svQuotation.IsResult)
            {
                return svQuotation.IsResult;
            }
            else
            {
                return svQuotation.IsResult;
            }
        }
        #endregion

        /*--------------DelData---------------*/
        #region DelData
        public ActionResult DelData(List<bool> Check, List<int> ID, List<short> RowVersion, string PrimaryKeyName)
        {
            var svQuotation = new QuotationService();
            for (int i = 0; i < ID.Count(); i++)
            {
                if (Check[i] == true)
                {
                    svQuotation.UpdateByCondition<b2bQuotation>("QuotationFolderID = 4", " QuotationID = " + ID[i]);
                }
            }
            //svQuotation.DelData<b2bQuotation>(Check, ID, RowVersion, PrimaryKeyName);
            if (svQuotation.IsResult)
            {
                var CountInbox = svQuotation.CountData<b2bQuotation>("*", "( IsDelete = 0 ) AND (IsRead = 'False') AND (IsOutBox = 'False') AND (ToCompID = " + LogonCompID + ")");
                var CountImportance = svQuotation.CountData<b2bQuotation>("*", "( ToCompID = " + LogonCompID + " AND IsOutBox = 0 AND IsDelete = 0 AND  IsImportance = 1 ) OR (FromCompID = " + LogonCompID + " AND IsOutBox = 1 AND IsDelete = 0 AND  IsImportance = 1 )");
                var CountSentbox = svQuotation.CountData<b2bQuotation>("*", "( IsDelete = 0 AND  IsOutbox = 1 AND FromCompID = " + LogonCompID + " )");

                return Json(new
                {
                    Result = true,
                    CountInbox = CountInbox,
                    CountImportance = CountImportance,
                    CountSentbox = CountSentbox,
                });
            }
            else
            {
                return Json(new { Result = false });
            }
        }
        #endregion
        #region DelQuoData
        public ActionResult DelQuoData(List<bool> Check, List<int> ID, List<short> RowVersion, string PrimaryKeyName)
        {
            var svQuotation = new QuotationService();
            for (int i = 0; i < ID.Count(); i++)
            {
                if (Check[i] == true)
                {
                    svQuotation.UpdateByCondition<b2bQuotation>("IsDelete = 1", " QuotationID = " + ID[i]);
                }
            }

            if (svQuotation.IsResult)
            {
                var CountInbox = svQuotation.CountData<b2bQuotation>("*", "( IsDelete = 0 ) AND (IsRead = 'False') AND (IsOutBox = 'False') AND (ToCompID = " + LogonCompID + ")");
                var CountImportance = svQuotation.CountData<b2bQuotation>("*", "( ToCompID = " + LogonCompID + " AND IsOutBox = 0 AND IsDelete = 0 AND  IsImportance = 1 ) OR (FromCompID = " + LogonCompID + " AND IsOutBox = 1 AND IsDelete = 0 AND  IsImportance = 1 )");
                var CountSentbox = svQuotation.CountData<b2bQuotation>("*", "( IsDelete = 0 AND  IsOutbox = 1 AND FromCompID = " + LogonCompID + " )");

                return Json(new
                {
                    Result = true,
                    CountInbox = CountInbox,
                    CountImportance = CountImportance,
                    CountSentbox = CountSentbox,
                });
            }
            else
            {
                return Json(new { Result = false });
            }
        }
        #endregion

        #region DelAllData
        public ActionResult DelAllData()
        {
            var svQuotation = new QuotationService();
            var ID = svQuotation.SelectData<b2bQuotation>("*", svQuotation.CreateWhereAction(QuotationAction.Trash, LogonCompID), null, 0, 0);
            if (ID.Count() > 0)
            {
                for (int i = 0; i < ID.Count(); i++)
                {
                    svQuotation.UpdateByCondition<b2bQuotation>("IsDelete = 1", " QuotationID = " + ID[i].QuotationID);
                }
            }

            if (svQuotation.IsResult)
            {
                var CountInbox = svQuotation.CountData<b2bQuotation>("*", "( IsDelete = 0 ) AND (IsRead = 'False') AND (IsOutBox = 'False') AND (ToCompID = " + LogonCompID + ")");
                var CountImportance = svQuotation.CountData<b2bQuotation>("*", "( ToCompID = " + LogonCompID + " AND IsOutBox = 0 AND IsDelete = 0 AND  IsImportance = 1 ) OR (FromCompID = " + LogonCompID + " AND IsOutBox = 1 AND IsDelete = 0 AND  IsImportance = 1 )");
                var CountSentbox = svQuotation.CountData<b2bQuotation>("*", "( IsDelete = 0 AND  IsOutbox = 1 AND FromCompID = " + LogonCompID + " )");

                return Json(new
                {
                    Result = true,
                    CountInbox = CountInbox,
                    CountImportance = CountImportance,
                    CountSentbox = CountSentbox,
                });
            }
            else
            {
                return Json(new { Result = false });
            }
        }
        #endregion

        #region MoveData
        public ActionResult MoveData(List<bool> Check, List<int> ID, List<short> RowVersion, string PrimaryKeyName)
        {
            var svQuotation = new QuotationService();
            for (int i = 0; i < ID.Count(); i++)
            {
                if (Check[i] == true)
                {
                    int IsOutbox = 1;
                    var isSend = svQuotation.SelectData<b2bQuotation>("*", "QuotationID = " + ID[i], null, 0, 0);
                    if (isSend[0].IsOutbox == true)
                    {
                        IsOutbox = 2;
                    }
                    svQuotation.UpdateByCondition<b2bQuotation>("QuotationFolderID = " + IsOutbox, " QuotationID = " + ID[i]);
                }
            }

            if (svQuotation.IsResult)
            {
                var CountInbox = svQuotation.CountData<b2bQuotation>("*", "( IsDelete = 0 ) AND (IsRead = 'False') AND (IsOutBox = 'False') AND (ToCompID = " + LogonCompID + ")");
                var CountImportance = svQuotation.CountData<b2bQuotation>("*", "( ToCompID = " + LogonCompID + " AND IsOutBox = 0 AND IsDelete = 0 AND  IsImportance = 1 ) OR (FromCompID = " + LogonCompID + " AND IsOutBox = 1 AND IsDelete = 0 AND  IsImportance = 1 )");
                var CountSentbox = svQuotation.CountData<b2bQuotation>("*", "( IsDelete = 0 AND  IsOutbox = 1 AND FromCompID = " + LogonCompID + " )");

                return Json(new
                {
                    Result = true,
                    CountInbox = CountInbox,
                    CountImportance = CountImportance,
                    CountSentbox = CountSentbox,
                });
            }
            else
            {
                return Json(new { Result = false });
            }
        }

        public ActionResult MoveDataDetail(int ID, short RowVersion, string PrimaryKeyName)
        {
            var svQuotation = new QuotationService();
            int IsOutbox = 1;
            var isSend = svQuotation.SelectData<b2bQuotation>("*", "QuotationID = " + ID, null, 0, 0);
            if (isSend[0].IsOutbox == true)
            {
                IsOutbox = 2;
            }
            svQuotation.UpdateByCondition<b2bQuotation>("QuotationFolderID = " + IsOutbox ," QuotationID = " + ID);


            if (svQuotation.IsResult)
            {
                var CountInbox = svQuotation.CountData<b2bQuotation>("*", "( IsDelete = 0 ) AND (IsRead = 'False') AND (IsOutBox = 'False') AND (ToCompID = " + LogonCompID + ")");
                var CountImportance = svQuotation.CountData<b2bQuotation>("*", "( ToCompID = " + LogonCompID + " AND IsOutBox = 0 AND IsDelete = 0 AND  IsImportance = 1 ) OR (FromCompID = " + LogonCompID + " AND IsOutBox = 1 AND IsDelete = 0 AND  IsImportance = 1 )");
                var CountSentbox = svQuotation.CountData<b2bQuotation>("*", "( IsDelete = 0 AND  IsOutbox = 1 AND FromCompID = " + LogonCompID + " )");

                return Json(new
                {
                    Result = true,
                    CountInbox = CountInbox,
                    CountImportance = CountImportance,
                    CountSentbox = CountSentbox,
                });
            }
            else
            {
                return Json(new { Result = false });
            }
        }
        #endregion

        /*---------------Reject---------------*/
        #region Reject
        [HttpPost]
        public ActionResult Reject(
            int? QuotationID,
            //string PricePerPiece,
            //string TotalPrice,
            //decimal Discount,
            //decimal Vat,
            //int? IsSentEmail,
            //string Remark,
            string SaleName,
            string SaleCompany,
            string SaleEmail,
            string SalePhone,
            string hidQuotationFileName,
            string hidQuotationFilePath,
            string hidQuotationUploadPath,
            string hidImgSize,
            string RejectDetail,
            string Type
            )
        {
            Hashtable data = new Hashtable();
            svQuotation = new QuotationService();
            var model = new b2bQuotation();
            var model2 = new b2bQuotation();
            var model3 = new b2bQuotation();
            int FromCompID = 0;
            int MaxID = 0;
            string ReqFirstName = "";
            string ReqPhone = "";
            string ReqEmail = "";
            string FromCompName = "";


            var Quotation = svQuotation.SelectData<b2bQuotation>("*", "QuotationID = " + QuotationID, "CreatedDate", 1, 0);
            model = Quotation.First();

            if (Type == "Reject")
            {
                FromCompID = Convert.ToInt16(model.ToCompID);
                FromCompName = model.CompanyName;
                ReqFirstName = model.ReqFirstName;
                ReqPhone = model.ReqPhone;
                ReqEmail = model.ReqEmail;
            }

            try
            {

                #region เสนอราคากลับ[Outbox]
                var count = svQuotation.SelectData<b2bQuotation>(" * ", " CreatedDate = GetDate() AND RowFlag > 0");
                int Count = count.Count + 1;
                string Code = AutoGenCode("QO", Count);
                model2.QuotationCode = Code;
                model2.RootQuotationCode = model.RootQuotationCode;
                //model2.PricePerPiece = Convert.ToDecimal(PricePerPiece.Replace(",", ""));
                //model2.Discount = Discount;
                //model2.Vat = Vat;
                //model2.TotalPrice = Convert.ToDecimal(TotalPrice.Replace(",", ""));
                model2.IsSentEmail = Convert.ToBoolean(0);
                model2.IsReject = Convert.ToBoolean(1);
                model2.RejectDetail = RejectDetail;
                //if (!string.IsNullOrEmpty(Remark)) { model2.Remark = Remark; }
                model2.SendDate = DateTimeNow;

                model2.ProductID = model.ProductID;
                model2.Qty = model.Qty;
                model2.QtyUnit = model.QtyUnit;
                model2.ToCompID = model.FromCompID;

                model2.CompanyName = model.CompanyName;
                model2.ReqFirstName = model.ReqFirstName;
                model2.ReqPhone = model.ReqPhone;
                model2.ReqEmail = model.ReqEmail;
                model2.ReqRemark = model.Remark;
                model2.FromCompID = FromCompID;

                //if (!string.IsNullOrEmpty(Remark)) { model2.SaleCompany = SaleCompany; }
                if (!string.IsNullOrEmpty(SaleEmail)) { model2.SaleEmail = SaleEmail; }
                if (!string.IsNullOrEmpty(SaleName)) { model2.SaleName = SaleName; }
                if (!string.IsNullOrEmpty(SaleCompany)) { model2.SaleCompany = SaleCompany; }
                if (!string.IsNullOrEmpty(SalePhone)) { model2.SalePhone = SalePhone; }

                model2.IsTelephone = false;
                model2.IsAttach = false;
                model2.IsAttachQuote = false;
                model2.IsEmail = false;
                model2.IsReply = false;
                model2.IsRead = false;
                model2.IsImportance = false;
                model2.IsPDFView = false;
                model2.IsOutbox = true;
                model2.QuotationFolderID = 2;
                model2.IsClosed = model.IsClosed;
                model2.IsPublic = model.IsPublic;
                model2.QuotationStatus = "Q";
                svQuotation.InsertQuotation(model2);
                #endregion

                #region เสนอราคากลับ[Inbpx]
                model3.QuotationCode = Code;
                model3.RootQuotationCode = model.RootQuotationCode;
                //model3.PricePerPiece = Convert.ToDecimal(PricePerPiece.Replace(",", ""));
                //model3.Discount = Discount;
                //model3.Vat = Vat;
                //model3.TotalPrice = Convert.ToDecimal(TotalPrice.Replace(",", ""));
                model3.IsSentEmail = Convert.ToBoolean(0);
                model3.IsReject = Convert.ToBoolean(1);
                model3.RejectDetail = RejectDetail;
                //if (!string.IsNullOrEmpty(Remark)) { model3.Remark = Remark; }
                model3.SendDate = DateTimeNow;

                model3.ProductID = model.ProductID;
                model3.Qty = model.Qty;
                model3.QtyUnit = model.QtyUnit;
                model3.ToCompID = model.FromCompID;

                model3.CompanyName = FromCompName;
                model3.ReqFirstName = ReqFirstName;
                model3.ReqPhone = ReqPhone;
                model3.ReqEmail = ReqEmail;
                model3.ReqRemark = model.Remark;
                model3.FromCompID = FromCompID;

                //if (!string.IsNullOrEmpty(Remark)) { model3.SaleCompany = SaleCompany; }
                if (!string.IsNullOrEmpty(SaleEmail)) { model3.SaleEmail = SaleEmail; }
                if (!string.IsNullOrEmpty(SaleName)) { model3.SaleName = SaleName; }
                if (!string.IsNullOrEmpty(SaleCompany)) { model3.SaleCompany = SaleCompany; }
                if (!string.IsNullOrEmpty(SalePhone)) { model3.SalePhone = SalePhone; }

                model3.IsTelephone = false;
                model3.IsAttach = false;
                model3.IsAttachQuote = false;
                model3.IsEmail = false;
                model3.IsReply = true;
                model3.IsRead = false;
                model3.IsImportance = false;
                model3.IsPDFView = false;
                model3.IsOutbox = false;
                model3.QuotationFolderID = 1;
                model3.IsClosed = model.IsClosed;
                model3.IsPublic = model.IsPublic;
                model3.QuotationStatus = "Q";
                svQuotation.InsertQuotation(model3);
                #endregion

                #region QuotationAttach
                if (svQuotation.IsResult)
                {
                    var MaxQuotationID = svQuotation.SelectData<b2bQuotation>("MAX(QuotationID) AS QuotationID", "QuotationID != ''", "", 1, 0, false).First().QuotationID;
                    MaxID = MaxQuotationID;
                    int Max_QuotationID = MaxQuotationID - 1;
                    /*----------Insert-QuotationAttach(Inbox)-----------*/
                    if (!string.IsNullOrEmpty(hidQuotationFileName))
                    {
                        #region Insert QuotationAttch
                        string[] item = hidQuotationFileName.Split('.');
                        svQuotationAttach = new QuotationAttachService();
                        var Model = new b2bQuotationAttach();
                        Model.FileName = hidQuotationFileName;
                        Model.FilePath = hidQuotationFilePath;
                        Model.FileSize = Convert.ToDecimal(hidImgSize);
                        Model.QuotationID = Max_QuotationID;
                        model.RowFlag = 2;
                        Model.FileType = item[item.Length - 1];
                        svQuotationAttach.InsertQuotationAttach(Model);
                        #endregion

                        if (svQuotationAttach.IsResult)
                        {
                            #region Save Files
                            imgManager = new FileHelper();
                            imgManager.DirPath = "Quotation/" + Max_QuotationID;
                            imgManager.DirTempPath = hidQuotationFilePath;
                            SaveFileImage(
                                imgManager.DirTempPath,
                                imgManager.DirPath,
                                hidQuotationFileName);
                            #endregion

                            /*----------Insert-QuotationAttach(Outbox)------------*/
                            #region Insert QuotationAttch
                            string[] it = hidQuotationFileName.Split('.');
                            svQuotationAttach = new QuotationAttachService();
                            var ModelAttach = new b2bQuotationAttach();
                            ModelAttach.FileName = hidQuotationFileName;
                            ModelAttach.FilePath = hidQuotationFilePath;
                            ModelAttach.FileSize = Convert.ToDecimal(hidImgSize);
                            ModelAttach.QuotationID = MaxQuotationID;
                            ModelAttach.RowFlag = 2;
                            ModelAttach.FileType = it[it.Length - 1];
                            svQuotationAttach.InsertQuotationAttach(ModelAttach);
                            #endregion

                            if (svQuotationAttach.IsResult)
                            {

                                #region Save Image Files
                                imgManager = new FileHelper();
                                imgManager.DirPath = "Quotation/" + MaxQuotationID;
                                imgManager.DirTempPath = hidQuotationFilePath;
                                SaveFileImage(
                                    imgManager.DirTempPath,
                                    imgManager.DirPath,
                                    hidQuotationFileName);
                                #endregion
                            }
                        }
                    }
                }
                #endregion

                if (svQuotation.IsResult)
                {
                    return Json(new
                    {
                        Result = true,
                        ID = model3.QuotationID
                    });
                }
                else
                {
                    return Json(new { Result = false });
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles(ex);
                return Json(data);
            }
            //var svQuotation = new QuotationService();
            //for (int i = 0; i < ID.Count; i++)
            //{
            //    svQuotation.UpdateByCondition<b2bQuotation>("IsReject = 1 " + ", RejectDetail = '" + RejectDetail + "' , RowVersion =" + (RowVersion[i] + 1) + " , IsOutbox = 1", "QuotationID = " + ID[i]);
            //}

            //if (svQuotation.IsResult)
            //{
            //    var CountInbox = svQuotation.CountData<b2bQuotation>("*", "( IsDelete = 0 ) AND (IsRead = 'False') AND (IsOutBox = 'False') AND (ToCompID = " + LogonCompID + ")");
            //    var CountImportance = svQuotation.CountData<b2bQuotation>("*", "( ToCompID = " + LogonCompID + " AND IsOutBox = 0 AND IsDelete = 0 AND  IsImportance = 1 ) OR (FromCompID = " + LogonCompID + " AND IsOutBox = 1 AND IsDelete = 0 AND  IsImportance = 1 )");
            //    var CountSentbox = svQuotation.CountData<b2bQuotation>("*", "( IsDelete = 0 AND  IsOutbox = 1 AND FromCompID = " + LogonCompID + " )");

            //    return Json(new
            //    {
            //        Result = true,
            //        CountImportance = CountImportance,
            //        CountInbox = CountInbox,
            //        CountSentbox = CountSentbox,
            //    });
            //}
            //else
            //{
            //    return Json(new { Result = false });
            //}
        }
        #endregion
        /*------------------------Quotation-Detail---------------------*/

        #region Get Quotation Detail
        [HttpGet]
        public ActionResult Detail(string ID)
        {
            GetStatusUser();
            RememberURL();
            if (!CheckIsLogin())
                return Redirect(res.Pageviews.PvMemberSignIn);

            #region Quotation
            int CompID = 0;
            int ProductID = 0;
            int QuotationID = 0;
            var svQuotation = new QuotationService();
            var Quotation = svQuotation.SelectData<b2bQuotation>("*", "QuotationID = '" + ID + "'", "CreatedDate", 1, 0);
            if (Quotation.Count > 0)
            {
                if (Quotation.First().FromCompID != 0)
                    CompID = Convert.ToInt32(Quotation.First().FromCompID);
                if (Quotation.First().ProductID != 0)
                    ProductID = Convert.ToInt32(Quotation.First().ProductID);
                if (!string.IsNullOrEmpty(Quotation.First().Remark))
                    ViewBag.Remark = Quotation.First().Remark;
                QuotationID = DataManager.ConvertToInteger(Quotation.First().QuotationID);
                ViewBag.QuotDetail = Quotation.First();
                if (Quotation.First().QuotationFolderID == 4)
                {
                    ViewBag.QuotationType = "Trash";
                }
                else
                {
                    ViewBag.QuotationType = "Inbox";
                }
            }

            #endregion

            #region Product
            svProduct = new ProductService();
            var Product = svProduct.SelectData<b2bProduct>("ProductID,ProductName,ProductCode,ProductImgPath", "ProductID = " + ProductID, "ProductName", 1, 0, false).First();
            if (Product.ProductName.Length >= 35)
            {
                ViewBag.ProNameShort = Product.ProductName.Substring(0, 35) + "...";
            }
            else
            {
                ViewBag.ProNameShort = Product.ProductName;
            }
            ViewBag.ProductName = Product.ProductName;
            ViewBag.ProductCode = Product.ProductCode;
            ViewBag.ProductImgPath = Product.ProductImgPath;
            #endregion

            #region Company
            svCompany = new CompanyService();

            if (CompID != 0)
            {
                ViewBag.Company = svCompany.SelectData<view_Company>("CompID,CompName,CompImgPath,BizTypeName,ProvinceName,ContactEmail", "CompID = " + CompID, null, 1, 0, false).First();
                var CompanyLogin = svCompany.SelectData<view_Company>("CompID,CompName,ContactEmail,ContactFirstName,ContactLastName", "CompID = " + LogonCompID, null, 1, 0, false).First();
                ViewBag.CompName = CompanyLogin.CompName;
                ViewBag.ContactEmail = CompanyLogin.ContactEmail;
                ViewBag.ContactName = CompanyLogin.ContactFirstName + "  " + CompanyLogin.ContactLastName;
            }
            else { ViewBag.Company = null; }
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

            #region Set ViewBag
            ViewBag.Inbox = svQuotation.CountData<b2bQuotation>("*", "( IsDelete = 0 ) AND (IsRead = 'False') AND (IsOutBox = 'False') AND (IsRead = 0) AND (ToCompID = " + LogonCompID + ")");
            ViewBag.Importance = svQuotation.CountData<b2bQuotation>("*", "( ToCompID = " + LogonCompID + " AND IsOutBox = 0 AND IsDelete = 0 AND  IsImportance = 1 ) OR (FromCompID = " + LogonCompID + " AND IsOutBox = 1 AND IsDelete = 0 AND  IsImportance = 1 )");
            ViewBag.Sentbox = svQuotation.CountData<b2bQuotation>("*", "( IsDelete = 0 AND  IsOutbox = 1 AND FromCompID = " + LogonCompID + " )");
            #endregion

            #region ChangeIsRead
            svQuotation = new QuotationService();
            var ChangeIsRead = svQuotation.UpdateByCondition<b2bQuotation>("IsRead = 1", "QuotationID = '" + ID + "'");
            #endregion

            #region IsSend
            var IsSend = svQuotation.SelectData<b2bQuotation>("*", "QuotationID = " + ID + " AND FromCompID = " + LogonCompID + " ");
            if (IsSend.Count > 0)
            {
                ViewBag.IsSend = "1";
            }
            else
            {
                ViewBag.IsSend = "0";
            }
            #endregion
            ViewBag.PageType = "Quotation";

            return View();
        }
        #endregion

        #region Post Quotation Detail
        [HttpPost]
        public ActionResult SaveDetail(
            int? QuotationID, 
            string PricePerPiece, 
            string TotalPrice, 
            decimal Discount, 
            decimal Vat, 
            int? IsSentEmail, 
            string Remark,
            string SaleName, 
            string SaleCompany, 
            string SaleEmail, 
            string SalePhone, 
            string hidQuotationFileName, 
            string hidQuotationFilePath, 
            string hidQuotationUploadPath,
            string hidImgSize, 
            string Type
        )
        {

            Hashtable data = new Hashtable();
            svQuotation = new QuotationService();
            var model = new b2bQuotation();
            var model2 = new b2bQuotation();
            var model3 = new b2bQuotation();
            int FromCompID = 0;
            int MaxID = 0;
            string ReqFirstName = "";
            string ReqPhone = "";
            string ReqEmail = "";
            string FromCompName = "";
            string LastName = "";
            string AddrLine1 = "";
            string AddrLine2 = "";
            string SubDistrict = "";
            int? DistrictID = 0;
            string PostalCode = "";

            var Quotation = svQuotation.SelectData<b2bQuotation>("*", "QuotationID = " + QuotationID, "CreatedDate", 1, 0);
            model = Quotation.First();

            if (Type == "Quotation")
            {
                FromCompID = Convert.ToInt16(model.ToCompID);
                FromCompName = model.CompanyName;
                ReqFirstName = model.ReqFirstName;
                ReqPhone = model.ReqPhone;
                ReqEmail = model.ReqEmail;
                LastName = model.ReqLastName;
                AddrLine1 = model.ReqAddrLine1;
                AddrLine2 = model.ReqAddrLine2;
                SubDistrict = model.ReqSubDistrict;
                DistrictID = model.ReqDistrictID;
                PostalCode = model.ReqPostalCode;
            }
            if (Type == "Bid")
            {
                svCompany = new CompanyService();
                if (LogonCompID != 0)
                {
                    var From = svCompany.SelectData<b2bCompany>("*", "CompID = " + LogonCompID).First();
                    ReqFirstName = From.ContactFirstName;
                    ReqPhone = From.ContactPhone;
                    ReqEmail = From.ContactEmail;
                    FromCompID = LogonCompID;
                    FromCompName = From.CompName;
                }
                else
                {
                    ReqFirstName = SaleName;
                    FromCompName = SaleCompany;
                    ReqPhone = SalePhone;
                    ReqEmail = SaleEmail;
                    FromCompID = 0;

                }
            }

            try
            {

                #region เสนอราคากลับ[Outbox]
                var count = svQuotation.SelectData<b2bQuotation>(" * ", " CreatedDate = GetDate() AND RowFlag > 0");
                int Count = count.Count + 1;
                string Code = AutoGenCode("QO", Count);
                model2.QuotationCode = Code;
                model2.RootQuotationCode = model.RootQuotationCode;
                model2.PricePerPiece = Convert.ToDecimal(PricePerPiece.Replace(",", ""));
                model2.Discount = Discount;
                model2.Vat = Vat;
                model2.TotalPrice = Convert.ToDecimal(TotalPrice.Replace(",", ""));
                model2.IsSentEmail = Convert.ToBoolean(IsSentEmail);
                if (!string.IsNullOrEmpty(Remark)) { model2.Remark = Remark; }
                model2.SendDate = DateTimeNow;
                model2.IsOutbox = true;
                model2.QuotationFolderID = 2;

                model2.ProductID = model.ProductID;
                model2.Qty = model.Qty;
                model2.QtyUnit = model.QtyUnit;
                model2.ToCompID = model.FromCompID;

                model2.CompanyName = model.CompanyName;
                model2.ReqFirstName = model.ReqFirstName;
                model2.ReqPhone = model.ReqPhone;
                model2.ReqEmail = model.ReqEmail;
                model2.ReqRemark = model.Remark;
                model2.FromCompID = FromCompID;

                model2.ReqLastName = LastName;
                model2.ReqAddrLine1 = AddrLine1;
                model2.ReqAddrLine2 = AddrLine2;
                model2.ReqSubDistrict = SubDistrict;
                model2.ReqDistrictID = DistrictID;
                model2.ReqPostalCode = PostalCode;

                if (!string.IsNullOrEmpty(Remark)) { model2.SaleCompany = SaleCompany; }
                if (!string.IsNullOrEmpty(SaleEmail)) { model2.SaleEmail = SaleEmail; }
                if (!string.IsNullOrEmpty(SaleName)) { model2.SaleName = SaleName; }
                if (!string.IsNullOrEmpty(SaleCompany)) { model2.SaleCompany = SaleCompany; }
                if (!string.IsNullOrEmpty(SalePhone)) { model2.SalePhone = SalePhone; }

                model2.IsTelephone = false;
                if (!string.IsNullOrEmpty(hidQuotationFileName))
                {
                    model2.IsAttach = true;
                }
                else
                {
                    model2.IsAttach = false;
                }
                model2.IsMatching = false;
                model2.IsAttachQuote = false;
                model2.IsEmail = false;
                model2.IsReply = false;
                model2.IsRead = false;
                model2.IsReject = false;
                model2.IsImportance = false;
                model2.IsPDFView = false;
                model2.IsClosed = model.IsClosed;
                model2.IsPublic = model.IsPublic;
                model2.QuotationStatus = "Q";
                model2.ViewCount = 0;
                svQuotation.InsertQuotation(model2);
                #endregion

                #region เสนอราคากลับ[Inbpx]
                model3.QuotationCode = Code;
                model3.RootQuotationCode = model.RootQuotationCode;
                model3.PricePerPiece = Convert.ToDecimal(PricePerPiece.Replace(",", ""));
                model3.Discount = Discount;
                model3.Vat = Vat;
                model3.TotalPrice = Convert.ToDecimal(TotalPrice.Replace(",", ""));
                model3.IsSentEmail = Convert.ToBoolean(IsSentEmail);
                if (!string.IsNullOrEmpty(Remark)) { model3.Remark = Remark; }
                model3.SendDate = DateTimeNow;

                model3.ProductID = model.ProductID;
                model3.Qty = model.Qty;
                model3.QtyUnit = model.QtyUnit;
                model3.ToCompID = model.FromCompID;

                model3.CompanyName = FromCompName;
                model3.ReqFirstName = ReqFirstName;
                model3.ReqPhone = ReqPhone;
                model3.ReqEmail = ReqEmail;
                model3.ReqRemark = model.Remark;
                model3.FromCompID = FromCompID;

                model3.ReqLastName = LastName;
                model3.ReqAddrLine1 = AddrLine1;
                model3.ReqAddrLine2 = AddrLine2;
                model3.ReqSubDistrict = SubDistrict;
                model3.ReqDistrictID = DistrictID;
                model3.ReqPostalCode = PostalCode;

                if (!string.IsNullOrEmpty(Remark)) { model3.SaleCompany = SaleCompany; }
                if (!string.IsNullOrEmpty(SaleEmail)) { model3.SaleEmail = SaleEmail; }
                if (!string.IsNullOrEmpty(SaleName)) { model3.SaleName = SaleName; }
                if (!string.IsNullOrEmpty(SaleCompany)) { model3.SaleCompany = SaleCompany; }
                if (!string.IsNullOrEmpty(SalePhone)) { model3.SalePhone = SalePhone; }

                model3.IsTelephone = false;
                if (!string.IsNullOrEmpty(hidQuotationFileName))
                {
                    model3.IsAttach = true;
                }
                else
                {
                    model3.IsAttach = false;
                }
                model3.IsMatching = false;
                model3.IsAttachQuote = false;
                model3.IsEmail = false;
                model3.IsReply = true;
                model3.IsRead = false;
                model3.IsReject = false;
                model3.IsImportance = false;
                model3.IsPDFView = false;
                model3.IsOutbox = false;
                model3.QuotationFolderID = 1;
                model3.IsClosed = model.IsClosed;
                model3.IsPublic = model.IsPublic;
                model3.QuotationStatus = "Q";
                model3.ViewCount = 0;
                svQuotation.InsertQuotation(model3);
                #endregion

                #region QuotationAttach
                // เช็คไฟล์แนบ
                if (!string.IsNullOrEmpty(hidQuotationFileName))
                {
                    // บันทึกข้อมูลลงกล่องขาออกของผู้ส่ง
                    #region Save Image Files
                    svQuotationAttach = new QuotationAttachService();
                    imgManager = new FileHelper();
                    imgManager.DirPath = "QuotationFile/" + LogonCompID + "/" + model2.QuotationID;
                    imgManager.DirTempPath = "Temp/QuotationFile/" + LogonCompID;
                    SaveFileImage(
                        imgManager.DirTempPath,
                        imgManager.DirPath,
                        hidQuotationFileName);
                    #endregion

                    string strFileName1 = hidQuotationFileName;
                    string[] ArrayFileName1 = strFileName1.Split('.');

                    var Attach1 = new b2bQuotationAttach();
                    Attach1.QuotationID = model2.QuotationID;
                    Attach1.FileName = hidQuotationFileName;
                    Attach1.FileType = "." + ArrayFileName1[1];
                    Attach1.FilePath = "https://ouikumstorage.blob.core.windows.net/upload/QuotationFile/" + LogonCompID + "/" + model2.QuotationID + "/" + hidQuotationFileName;
                    Attach1.FileSize = 0;
                    svQuotationAttach.InsertQuotationAttach(Attach1);

                    // บันทึกข้อมูลลงกล่องขาเข้าของผู้รับ
                    #region Save Image Files
                    imgManager = new FileHelper();
                    imgManager.DirPath = "QuotationFile/" + LogonCompID + "/" + model3.QuotationID;
                    imgManager.DirTempPath = "Temp/QuotationFile/" + LogonCompID;
                    SaveFileImage(
                        imgManager.DirTempPath,
                        imgManager.DirPath,
                        hidQuotationFileName);
                    #endregion

                    string strFileName2 = hidQuotationFileName;
                    string[] ArrayFileName2 = strFileName2.Split('.');

                    var Attach2 = new b2bQuotationAttach();
                    Attach2.QuotationID = model3.QuotationID;
                    Attach2.FileName = hidQuotationFileName;
                    Attach2.FileType = "." + ArrayFileName2[1];
                    Attach2.FilePath = "https://ouikumstorage.blob.core.windows.net/upload/QuotationFile/" + LogonCompID + "/" + model3.QuotationID + "/" + hidQuotationFileName;
                    Attach2.FileSize = 0;
                    svQuotationAttach.InsertQuotationAttach(Attach2);
                }
                #endregion

                if (svQuotation.IsResult)
                {
                    return Json(new
                    {
                        Result = true,
                        ID = Convert.ToInt32(model3.QuotationID)
                    });
                }
                else
                {
                    return Json(new { Result = false });
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles(ex);
                return Json(data);
            }


        }
        #endregion

        #region ChangSataus
        [HttpPost]
        public ActionResult ChangSataus(int? ID, int? RowVersion, string Condition, int? IsImportance)
        {
            string Status = "";
            int IsDelete = 0;
            int Importance = 0;
            if (IsImportance == 0 || IsImportance == null)
            {
                Importance = 1;
            }
            switch (Condition)
            {
                case "Delete":
                    IsDelete = 1;
                    SQLWhere = " QuotationID = " + ID;
                    SQLUpdate = "QuotationFolderID = 4 , RowVersion = " + (RowVersion + 1);
                    break;

                case "Importance":
                    Status = Condition;
                    SQLWhere = " QuotationID = " + ID;
                    SQLUpdate = "IsImportance = '" + Importance + "' , RowVersion = " + (RowVersion + 1);
                    break;

                case "Unimportance":
                    Status = Condition;
                    SQLWhere = " QuotationID = " + ID;
                    SQLUpdate = "IsImportance = '" + 0 + "' , RowVersion = " + (RowVersion + 1);
                    Importance = 0;
                    break;
            }

            var svQuotation = new QuotationService();
            svQuotation.UpdateByCondition<b2bQuotation>(SQLUpdate, SQLWhere);

            if (svQuotation.IsResult)
            {
                var CountInbox = svQuotation.CountData<b2bQuotation>("*", "( IsDelete = 0 ) AND (IsRead = 'False') AND (IsOutBox = 'False') AND (ToCompID = " + LogonCompID + ")");
                var CountImportance = svQuotation.CountData<b2bQuotation>("*", "( ToCompID = " + LogonCompID + " AND IsOutBox = 0 AND IsDelete = 0 AND  IsImportance = 1 ) OR (FromCompID = " + LogonCompID + " AND IsOutBox = 1 AND IsDelete = 0 AND  IsImportance = 1 )");
                var CountSentbox = svQuotation.CountData<b2bQuotation>("*", "( IsDelete = 0 AND  IsOutbox = 1 AND FromCompID = " + LogonCompID + " )");
                return Json(new
                {
                    Result = true,
                    IsDelete = IsDelete,
                    Status = Importance,
                    RowVersion = (RowVersion + 1),
                    CountImportance = CountImportance,
                    CountInbox = CountInbox,
                    CountSentbox = CountSentbox,
                });
            }
            else
            {
                return Json(new { Result = false });
            }

        }

        public ActionResult ChangeTagList(List<bool> Check, List<int> ID, List<short> RowVersion, string PrimaryKeyName)
        {
            var svQuotation = new QuotationService();

            for (int i = 0; i < ID.Count(); i++)
            {
                if (Check[i] == true)
                {
                    var Quotation = svQuotation.SelectData<b2bQuotation>("*", "IsDelete = 0 AND QuotationID = " + ID[i] + " AND (ToCompID = " + LogonCompID + " OR FromCompID = " + LogonCompID + ")", null, 0, 0);

                    #region set ค่า  & Update b2bQuotation
                    Quotation[0].IsImportance = DataManager.ConvertToBool(0);
                    Quotation = svQuotation.SaveData<b2bQuotation>(Quotation, "QuotationID");
                    #endregion
                }
            }

            if (svQuotation.IsResult)
            {
                var CountInbox = svQuotation.CountData<b2bQuotation>("*", "( IsDelete = 0 ) AND (IsRead = 'False') AND (IsOutBox = 'False') AND (ToCompID = " + LogonCompID + ")");
                var CountImportance = svQuotation.CountData<b2bQuotation>("*", "( ToCompID = " + LogonCompID + " AND IsOutBox = 0 AND IsDelete = 0 AND  IsImportance = 1 ) OR (FromCompID = " + LogonCompID + " AND IsOutBox = 1 AND IsDelete = 0 AND  IsImportance = 1 )");
                var CountSentbox = svQuotation.CountData<b2bQuotation>("*", "( IsDelete = 0 AND  IsOutbox = 1 AND FromCompID = " + LogonCompID + " )");

                return Json(new
                {
                    Result = true,
                    CountInbox = CountInbox,
                    CountImportance = CountImportance,
                    CountSentbox = CountSentbox,
                });
            }
            else
            {
                return Json(new { Result = false });
            }
        }

        #endregion

        /*------------------------Request-Price-----------------------*/
        #region Get RequestPrice
        [HttpGet]
        public ActionResult RequestPrice(int? ID)
        {
            CommonService svCommon = new CommonService();
            ViewBag.EnumServiceType = svCommon.SelectEnum(CommonService.EnumType.SearchByServiceType);
            GetStatusUser();

            #region SelectProduct
            svProduct = new ProductService();
            var Products = svProduct.SelectData<view_Product>("*", "ProductID = " + ID, null, 1, 0, false).First();
            int CompID = Convert.ToInt32(Products.CompID);
            ViewBag.ProductDetail = Products;
            ViewBag.ProductID = ID;
            ViewBag.QtyUnit = Products.QtyUnit;
            //if (Products.ProvinceName == "กรุงเทพมหานคร")
            //{
            //    Products.ProvinceName = "กรุงเทพ";
            //}
            ViewBag.Title = res.Quotation.lblQuotation + Products.ProductName + " | " + Products.ProvinceName + " | " + res.Common.lblDomainShortName;
            if (!string.IsNullOrEmpty(Products.ProductKeyword))
            {
                ViewBag.MetaKeyword = res.Quotation.lblQuotation + Products.ProductName + " | " + Products.ProductKeyword.Replace('~', ',').Substring(0, Products.ProductKeyword.Length - 1) + " | " + Products.ProvinceName + " | " + res.Common.lblDomainShortName;
            }
            else
            {
                ViewBag.MetaKeyword = ViewBag.Title;
            }
            if (!string.IsNullOrEmpty(Products.ShortDescription))
            {
                ViewBag.MetaDescription = res.Quotation.lblQuotation + Products.ProductName + " | " + Products.ShortDescription.Replace('~', ',').Substring(0, Products.ShortDescription.Length - 1) + " | " + Products.ProvinceName + " | " + res.Common.lblDomainShortName;
            }
            else
            {
                ViewBag.MetaDescription = ViewBag.Title;
            }
            #endregion

            #region Company
            svCompany = new CompanyService();
            ViewBag.Company = svCompany.SelectData<view_Company>("*", "CompID = " + CompID, null, 1, 0, false).First();
            #endregion

            /*
            #region Province
            svAddress = new AddressService();
            ViewBag.Provinces = svAddress.SelectData<emProvince>("ProvinceID,ProvinceName", "Rowflag > 0", "ProvinceName ASC");
            #endregion

            #region District
            ViewBag.Districts = svAddress.SelectData<emDistrict>("DistrictID,DistrictName", "Rowflag > 0 and IsDelete = 0");
            #endregion
            */

            DoLoadQtyUnits();

            return View();
        }
        #endregion

        #region Post RequestDataMember
        [HttpPost]
        public ActionResult RequestDataMember()
        {
            int CompID;
            GetStatusUser();

            CompID = LogonCompID;

            if (CompID != 0)
            {
                svCompany = new CompanyService();
                var Company = svCompany.SelectData<b2bCompany>("MemberID,CompID,CompName", "CompID = " + CompID, null, 1, 0, false).First();
                var MemberID = Company.MemberID;
                var CompNamp = Company.CompName;

                svMember = new MemberService();
                string SQLWhere = "MemberID,FirstName,LastName,AddrLine1,AddrLine2,Email,SubDistrict,DistrictID,ProvinceID,PostalCode,Phone,Mobile,Fax,DistrictName,ProvinceName";
                var Member = svMember.SelectData<view_emMember>(SQLWhere, "MemberID = " + MemberID, null, 1, 0).First();

                return Json(new
                {
                    CompID = CompID,
                    CompName = CompNamp,
                    FirstName = Member.FirstName,
                    LastName = Member.LastName,
                    Address1 = Member.AddrLine1,
                    Address2 = Member.AddrLine2,
                    SubDistrict = Member.SubDistrict,
                    DistrictName = Member.DistrictName,
                    ProvinceName = Member.ProvinceName,
                    ProvinceID = Member.ProvinceID,
                    DistrictID = Member.DistrictID,
                    ZipCode = Member.PostalCode,
                    Mobile = Member.Phone,
                    Email = Member.Email,
                });
            }
            else
            {

                return Json(new { CompID = 0 });
            }

        }
        #endregion

        #region Post RequestPrice
        [HttpPost]
        public ActionResult PostRequestPrice(
            int ProductID, 
            string Qty, 
            string QtyUnit, 
            int ToCompID, 
            string CompanyName, 
            int? FromCompID, 
            string ReqFirstName, 
            string ReqLastName,
            string ReqAddrLine1,
            string ReqAddrLine2,
            string ReqSubDistrict, 
            int? ReqDistrictID, 
            string ReqPostalCode, 
            string ReqPhone, 
            string ReqEmail,
            int IsSentEmail, 
            int IsTelephone, 
            int IsAttach, 
            string Remark, 
            string hidQuotationFileName, 
            string hidQuotationFilePath, 
            string hidQuotationUploadPath,
            string hidImgSize, 
            int IsPublic
            )
        {
            Hashtable data = new Hashtable();
            var model = new b2bQuotation();
            var model2 = new b2bQuotation();
            try
            {

                svQuotation = new QuotationService();
                var count = svQuotation.SelectData<b2bQuotation>(" * ", " CreatedDate = GetDate() AND RowFlag > 0");
                int Count = count.Count + 1;
                string Code = AutoGenCode("QO", Count);
                model.QuotationCode = Code;
                model.RootQuotationCode = Code;

                #region ข้อมูลร้องขอราคาสินค้า[Outbox]
                model.ProductID = ProductID;
                model.Qty = (short)DataManager.ConvertToSingle(Qty);
                model.QtyUnit = QtyUnit;
                model.ToCompID = ToCompID;

                model.CompanyName = CompanyName;
                model.ReqFirstName = ReqFirstName;
                model.ReqLastName = ReqLastName;
                model.ReqAddrLine1 = ReqAddrLine1;
                model.ReqAddrLine2 = ReqAddrLine2;
                model.ReqSubDistrict = ReqSubDistrict;
                model.ReqDistrictID = Convert.ToInt32(ReqDistrictID);
                model.ReqPostalCode = ReqPostalCode;
                model.ReqPhone = ReqPhone;
                model.ReqEmail = ReqEmail;
                model.FromCompID = FromCompID;
                model.IsMatching = false;
                if (!string.IsNullOrEmpty(Remark)) { model.ReqRemark = Remark; }

                model.IsSentEmail = false;
                model.IsTelephone = Convert.ToBoolean(IsTelephone);
                model.IsAttach = Convert.ToBoolean(IsAttach);
                model.IsAttachQuote = false;
                model.IsEmail = false;
                model.IsReply = false;
                model.IsRead = false;
                model.IsReject = false;
                model.IsImportance = false;
                model.IsPDFView = false;
                model.IsOutbox = true;
                model.QuotationFolderID = 2;
                model.SendDate = DateTimeNow;
                model.IsClosed = false;
                model.IsPublic = Convert.ToBoolean(IsPublic);
                model.QuotationStatus = "R";
                model.ViewCount = 0;
                #endregion

                #region ข้อมูลร้องขอราคาสินค้า[Inbpx]
                model2.ProductID = ProductID;
                model2.Qty = (short)DataManager.ConvertToSingle(Qty);
                model2.QtyUnit = QtyUnit;
                model2.ToCompID = ToCompID;

                model2.CompanyName = CompanyName;
                model2.ReqFirstName = ReqFirstName;
                model2.ReqLastName = ReqLastName;
                model2.ReqAddrLine1 = ReqAddrLine1;
                model2.ReqAddrLine2 = ReqAddrLine2;
                model2.ReqSubDistrict = ReqSubDistrict;
                model2.ReqDistrictID = Convert.ToInt32(ReqDistrictID);
                model2.ReqPostalCode = ReqPostalCode;
                model2.ReqPhone = ReqPhone;
                model2.ReqEmail = ReqEmail;
                model2.FromCompID = FromCompID;
                model2.IsMatching = false;
                if (!string.IsNullOrEmpty(Remark)) { model2.ReqRemark = Remark; }

                model2.IsSentEmail = false;
                model2.IsTelephone = Convert.ToBoolean(IsTelephone);
                model2.IsAttach = Convert.ToBoolean(IsAttach);
                model2.IsAttachQuote = false;
                model2.IsEmail = false;
                model2.IsReply = false;
                model2.IsRead = false;
                model2.IsReject = false;
                model2.IsImportance = false;
                model2.IsPDFView = false;
                model2.IsOutbox = false;
                model2.QuotationFolderID = 1;
                model2.SendDate = DateTimeNow;
                model2.IsClosed = false;
                model2.IsPublic = Convert.ToBoolean(IsPublic);
                model2.QuotationStatus = "R";
                model2.QuotationCode = Code;
                model2.RootQuotationCode = Code;
                model2.ViewCount = 0;
                #endregion

                #region Insert Quotation
                svQuotation.InsertQuotation(model);
                svQuotation.InsertQuotation(model2);
                #endregion

                #region Update ContactCount
                if (ProductID > 0)
                {
                    AddContactCount(ProductID);
                }
                #endregion

                // Share fb.
                //PostFeedQuotationOnFacebook(model);

                // Share tw.
                //PostTweetsQuotationOnTwitter(model);

                return Json(new { QuotationID = model2.QuotationID, });
            }
            catch (Exception ex)
            {
                CreateLogFiles(ex);
                return Json(data);
            }
        }
        #endregion

        #region PostFeedQuotationOnFacebook
        public void PostFeedQuotationOnFacebook(b2bQuotation quotaion)
        {
            //var fb = new FacebookHelper();
            //var model = new FacebookModel();

            var Url = res.Pageviews.UrlWeb;
            var UrlPicture = res.Pageviews.BlobStorageUrl;
            var Qty = res.Quotation.lblQty + " " + quotaion.Qty + " " + quotaion.QtyUnit;
            var svProduct = new ProductService();
            var Product = svProduct.SelectData<b2bProduct>(" * ", "ProductID = " + quotaion.ProductID);

            if (Product != null && Product.Count > 0)
            {
                //foreach (var it in Product)
                //{
                //    model.Link = Url + "/BidProduct/Detail/" + quotaion.QuotationCode;
                //    model.Picture = UrlPicture + "/Product/" + it.CompID + "/" + quotaion.ProductID + "/" + it.ProductImgPath;
                //    model.Message = res.Quotation.lblSomeoneWants+" " + it.ProductName + " "+res.Quotation.lblForSupplier;
                //}
            }

            // var isResult = fb.PostB2BThaiGroup(model);
        }
        #endregion

        #region PostTweetsQuotationOnTwitter
        private void PostTweetsQuotationOnTwitter(b2bQuotation quotaion)
        {
            var tw = new TwitterHelper();
            var model = new TwitterModel();
            var shortUrl = ShortenUrl(res.Pageviews.UrlWeb + "/BidProduct/Detail/" + quotaion.QuotationCode).ShortUrl;
            var svProduct = new ProductService();
            var product = svProduct.SelectData<b2bProduct>(" * ", "ProductID = " + quotaion.ProductID);

            if (product != null && product.Count > 0)
            {
                foreach (var it in product)
                {
                    model.Message = "@" + res.Quotation.lblSomeoneWants + " " + it.ProductName + " " + res.Quotation.lblQty + " " + quotaion.Qty + " " + quotaion.QtyUnit;
                    model.Message += " " + res.Quotation.lblForSupplier + " " + shortUrl;
                    var isResult = tw.PostTweets(model);
                }
            }
        }
        #endregion

        /*---------------------Send-Reject-List----------------------*/
        #region Get ReplyRequest
        [ValidateInput(false)]
        public ActionResult ReplyRequest(string ProductName, string QuotationID, string ProductCode, string Status, string MsgDetail, string RJFromName, string RJFromEmail, int IsSupplierRelated)
        {

            Ouikum.Message.MessageService svMessage = new Ouikum.Message.MessageService();
            if (!string.IsNullOrEmpty(QuotationID))
            {
                var svQuotation = new QuotationService();
                var quotation = new b2bQuotation();
                var svCompany = new CompanyService();
                var Comp = new b2bCompany();
                var feature = new List<view_HotFeaProduct>();
                var SupRelate = new List<b2bProduct>();
                var CompSupRelate = new List<b2bCompany>();

                if (Status != "")
                {
                    #region Set model
                    var request = svQuotation.SelectData<b2bQuotation>("*", "QuotationID = " + QuotationID, null, 1, 1).First();
                    var Product = svQuotation.SelectData<b2bProduct>("*", "ProductID = " + request.ProductID, null, 1, 1).First();
                    var ProductImg = Product;
                    if (request.ToCompID != 0)
                    {
                        Comp = svCompany.SelectData<b2bCompany>("*", "CompID = '" + request.ToCompID + "'", null, 1, 1).First();
                    }

                    if (IsSupplierRelated == 1)
                    {
                        SupRelate = svQuotation.SelectData<b2bProduct>(" TOP 5 ProductID, CompID", " CateLV1 = " + Product.CateLV1 + " AND CateLV2 = " + Product.CateLV2 + " AND CateLV3 = " + Product.CateLV3 + " AND IsDelete = 0 AND IsShow = 1 AND IsJunk = 0 ORDER BY NewID()", "", 1, 0, false);
                        var ID = new List<string>();
                        foreach (var it in SupRelate)
                        {
                            ID.Add(it.CompID.ToString());
                        }
                        var sqlWhere = SQLWhereListString(ID, "CompID");
                        CompSupRelate = svCompany.SelectData<b2bCompany>(" CompID, CompName, ContactEmail", sqlWhere);
                    }

                    var svHotFeat = new HotFeaProductService();
                    var SQLSelect_Feat = "";

                    SQLSelect_Feat = " ProductID,ProductName,CompID,ProductImgPath,ProRowFlag,CompRowFlag,ProvinceName,Price,Ispromotion,PromotionPrice,HotPrice";
                    feature = svHotFeat.SelectHotProduct<view_HotFeaProduct>(SQLSelect_Feat, "Rowflag = 3 AND Status = 'H' AND ProductID > 0 AND ProRowFlag in(2,4) AND CompRowFlag in(2,4) AND ProductDelete = 0", "NEWID(),HotPrice DESC", 1, 4);//""

                    quotation = request;
                    #endregion

                    #region Send Email
                    switch (Status)
                    {
                        // ส่งอีเมล ขอราคาสินค้า
                        case "Request":
                            if (request.ToCompID != 0)
                            {
                                SendEmail(quotation, Comp.ContactFirstName, Comp.CompName, Comp.ContactEmail, ProductCode, ProductName, Status, MsgDetail, Convert.ToInt32(QuotationID), Comp.CompPhone, Convert.ToString(Comp.CompLevel), Comp.CompID, Convert.ToInt32(request.ProductID), ProductImg.ProductImgPath, Convert.ToInt32(ProductImg.CompID), feature, CompSupRelate);
                            }
                            else
                            {
                                SendEmail(quotation, request.ReqFirstName, request.CompanyName, request.ReqEmail, ProductCode, ProductName, Status, MsgDetail, Convert.ToInt32(QuotationID), request.ReqPhone, "", Convert.ToInt32(request.FromCompID), Convert.ToInt32(request.ProductID), ProductImg.ProductImgPath, Convert.ToInt32(ProductImg.CompID), feature, CompSupRelate);
                            }
                            if (quotation.IsPublic.Equals(true))
                            {
                                //IsSupplieRelated(quotation, Comp.ContactFirstName, Comp.CompName, Comp.ContactEmail, ProductName, Status, Comp.CompID, Convert.ToInt32(request.ProductID), ProductImg.ImgPath);
                            }
                            else if (quotation.IsPublic.Equals(true))
                            {
                                //ToBuyerRelatedProducts(quotation, Convert.ToInt32(request.ProductID), ProductName);
                            }
                            break;
                        // ส่งเมล เสนอราคาสินค้า
                        case "Reply":
                            var rootQuotation = svQuotation.SelectData<b2bQuotation>("*", "QuotationCode = '" + request.QuotationCode + "'", "QuotationID ASC", 1, 1).First();
                            if (request.ToCompID != 0)
                            {
                                SendEmail(quotation, Comp.ContactFirstName, Comp.CompName, Comp.ContactEmail, ProductCode, ProductName, Status, MsgDetail, Convert.ToInt32(QuotationID), Comp.CompPhone, Convert.ToString(Comp.CompLevel), Comp.CompID, Convert.ToInt32(request.ProductID), ProductImg.ProductImgPath, Convert.ToInt32(ProductImg.CompID), feature, CompSupRelate);
                            }
                            else
                            {
                                SendEmail(quotation, request.ReqFirstName, request.CompanyName, request.ReqEmail, ProductCode, ProductName, Status, MsgDetail, Convert.ToInt32(QuotationID), request.ReqPhone, "", Convert.ToInt32(request.FromCompID), Convert.ToInt32(request.ProductID), ProductImg.ProductImgPath, Convert.ToInt32(ProductImg.CompID), feature, CompSupRelate);
                            }
                            break;
                        case "Reject":
                            if (request.ToCompID != 0)
                            {
                                SendEmail(quotation, Comp.ContactFirstName, Comp.CompName, Comp.ContactEmail, ProductCode, ProductName, Status, MsgDetail, Convert.ToInt32(QuotationID), Comp.CompPhone, Convert.ToString(Comp.CompLevel), Comp.CompID, Convert.ToInt32(request.ProductID), ProductImg.ProductImgPath, Convert.ToInt32(ProductImg.CompID), feature, CompSupRelate);
                            }
                            else
                            {
                                SendEmail(quotation, request.ReqFirstName, request.CompanyName, request.ReqEmail, ProductCode, ProductName, Status, MsgDetail, Convert.ToInt32(QuotationID), request.ReqPhone, "", Convert.ToInt32(request.FromCompID), Convert.ToInt32(request.ProductID), ProductImg.ProductImgPath, Convert.ToInt32(ProductImg.CompID), feature, CompSupRelate);
                            }
                            break;
                    }
                    #endregion

                }
                else
                {
                    return Json(false);
                }

            }
            return Json(true);
        }

        #endregion

        #region SendEmail
        public bool SendEmail(b2bQuotation model, string ContactFirstName, string toCompName, string toCompEmail, string ProductCode, string ProductName, string Status, string MsgDetail, int QuotationID, string CompPhone, string CompLevel, int CompID, int ProductID, string ProductImgPath, int CompProductImg, List<view_HotFeaProduct> HotProduct, List<b2bCompany> CompSupRelate)
        {
            #region variable
            //int type = 0;
            bool IsSend = true;
            var Detail = "";
            var mailTo = new List<string>();
            var mailToSeller = new List<string>();
            var mailToBuyer = new List<string>();
            var mailToSupRelate = new List<string>();
            List<string> mailToAdmin = GetMailListB2BAdmin();
            var mailCC = new List<string>();
            var Sender = "";
            var Receiver = "";
            svCompany = new CompanyService();
            svHotFeaProduct = new HotFeaProductService();
            Hashtable EmailDetail = new Hashtable();
            var mailFrom = res.Config.EmailNoReply;
            Receiver = toCompName;
            #endregion

            #region Set Content & Value For Send Email

            string urlb2bthai = res.Pageviews.UrlWeb;
            string pathlogo = urlb2bthai + "/Content/Default/logo/Ouikum/img_Logo120x74.png";
            string pathF = urlb2bthai + "/Content/Default/images/icon_freesmall.png";
            string pathG = urlb2bthai + "/Content/Default/images/icon_goldsmall.png";
            string pathView = urlb2bthai + "/MyB2B/Quotation/List";
            string pathPro = urlb2bthai + "/Search/Product/Detail/" + ProductID + "/" + ProductName;
            string pathBid = urlb2bthai + "/MyB2B/Quotation/Detail/" + model.QuotationID;

            string Subject = "";
            switch (Status)
            {
                // ส่งอีเมล ขอราคาสินค้า
                case "Request":
                    #region ขอราคาสินค้า
                    EmailDetail["b2bthaiUrl"] = urlb2bthai;
                    EmailDetail["pathLogo"] = pathlogo;
                    EmailDetail["pathView"] = pathView;
                    EmailDetail["pathPro"] = pathPro;
                    EmailDetail["pathBid"] = pathBid;

                    EmailDetail["Qty"] = model.Qty;
                    EmailDetail["QtyUnit"] = model.QtyUnit;
                    EmailDetail["ReqEmail"] = model.ReqEmail;
                    EmailDetail["ProductName"] = ProductName;
                    EmailDetail["ProductCode"] = ProductCode;
                    EmailDetail["toCompEmail"] = toCompEmail;
                    EmailDetail["CompPhone"] = CompPhone;

                    // ข้อมูลรูปภาพ
                    EmailDetail["CompID"] = CompID;
                    EmailDetail["ProductID"] = ProductID;
                    EmailDetail["ImgPath"] = ProductImgPath;

                    // ชื่อเป็นค่าว่าง ให้ใช้ชื่อบริษัทแทน
                    if (string.IsNullOrEmpty(ContactFirstName))
                    {
                        EmailDetail["ContactFirstName"] = toCompName;
                    }
                    else
                    {
                        EmailDetail["ContactFirstName"] = ContactFirstName;
                    }
                    if (string.IsNullOrEmpty(model.ReqFirstName))
                    {
                        EmailDetail["ReqFirstName"] = model.CompanyName;
                    }
                    else
                    {
                        EmailDetail["ReqFirstName"] = model.ReqFirstName;
                    }

                    // ไม่ได้ระบุชื่อบริษัท ให้ว่างไว้
                    if (model.CompanyName.Equals("ไม่ระบุชื่อบริษัท") || model.CompanyName == null)
                    {
                        EmailDetail["CompanyName"] = "";
                    }
                    else
                    {
                        EmailDetail["CompanyName"] = "[" + model.CompanyName + "]";
                    }
                    if (toCompName.Equals("ไม่ระบุชื่อบริษัท") || toCompName == null)
                    {
                        EmailDetail["toCompName"] = "";
                    }
                    else
                    {
                        EmailDetail["toCompName"] = "[" + toCompName + "]";
                    }

                    if (model.ReqPhone == "")
                    {
                        EmailDetail["ReqPhone"] = "ไม่ระบุข้อมูล";
                    }
                    else
                    {
                        EmailDetail["ReqPhone"] = model.ReqPhone;
                    }

                    ViewBag.Data = EmailDetail;
                    ViewBag.HotProduct = HotProduct;

                    var SubjectSeller = res.Email.lblRequestedPrices + " '" + ProductName + "' " + res.Email.lblWeb_B2BThai;
                    mailToSeller.Add(model.ReqEmail);
                    var DetailSeller = PartialViewToString("UC/Email/ProductDetailsRequest");
                    IsSend = OnSendByAlertEmail(SubjectSeller, mailFrom, mailToSeller, mailCC, DetailSeller);

                    var SubjectBuyer = res.Quotation.lblReqPriceSubject1 + " '" + ProductName + "' " + res.Email.lblWeb_B2BThai;
                    mailToBuyer.Add(toCompEmail);
                    var DetailBuyer = PartialViewToString("UC/Email/ProductInformationRequests");
                    IsSend = OnSendByAlertEmail(SubjectBuyer, mailFrom, mailToBuyer, mailCC, DetailBuyer);

                    if (CompSupRelate.Count > 0)
                    {
                        foreach (var it in CompSupRelate)
                        {
                            mailToSupRelate.Add(it.ContactEmail);
                        }

                        if (mailToSupRelate.Count > 0)
                        {
                            IsSend = OnSendByAlertEmail(SubjectBuyer, mailFrom, mailToSupRelate, mailCC, DetailBuyer);
                        }
                    }

                    //type = 1;
                    break;
                    #endregion
                // ส่งเมล เสนอราคาสินค้า
                case "Reply":
                    #region เสนอราคาสินค้า
                    var Comp = svCompany.SelectData<b2bCompany>("*", "CompID = '" + model.FromCompID + "'", null, 1, 1).First();
                    EmailDetail["b2bthaiUrl"] = urlb2bthai;
                    EmailDetail["pathLogo"] = pathlogo;
                    EmailDetail["pathView"] = pathView;
                    EmailDetail["pathPro"] = pathPro;
                    EmailDetail["pathSeller"] = urlb2bthai + "/Message/Contact/?ToCompID=" + model.FromCompID + "&ProductID=" + ProductID;
                    EmailDetail["pathIsClose"] = urlb2bthai + "/MyB2B/Quotation/Detail/" + model.QuotationID;

                    EmailDetail["Qty"] = model.Qty;
                    EmailDetail["QtyUnit"] = model.QtyUnit;
                    EmailDetail["PricePerPiece"] = model.PricePerPiece;
                    EmailDetail["TotalPrice"] = model.TotalPrice;
                    // ส่วนลด เป็นค่าว่าง
                    if (model.Discount == null)
                    {
                        EmailDetail["Discount"] = 0;
                    }
                    else
                    {
                        EmailDetail["Discount"] = model.Discount;
                    }
                    // ภาษี เป็นค่าว่าง
                    if (model.Vat == null)
                    {
                        EmailDetail["Vat"] = 0;
                    }
                    else
                    {
                        EmailDetail["Vat"] = model.Vat;
                    }

                    var SumPriceUnit = model.Qty * model.PricePerPiece;
                    var SumDiscount = SumPriceUnit - (model.Discount == null ? 0 : model.Discount);
                    var SumVat = (SumDiscount * (model.Vat == null ? 0 : model.Vat)) / 100;
                    EmailDetail["SumPriceUnit"] = SumPriceUnit;
                    EmailDetail["SumVat"] = SumVat;

                    EmailDetail["ProductName"] = ProductName;
                    EmailDetail["ProductCode"] = ProductCode;
                    EmailDetail["toCompEmail"] = Comp.ContactEmail;
                    EmailDetail["ToTel"] = Comp.CompPhone;

                    // ข้อมูลรูปภาพ
                    EmailDetail["CompID"] = CompProductImg;
                    EmailDetail["ProductID"] = ProductID;
                    EmailDetail["ImgPath"] = ProductImgPath;

                    // ชื่อเป็นค่าว่าง ให้ใช้ชื่อบริษัทแทน
                    if (string.IsNullOrEmpty(ContactFirstName))
                    {
                        EmailDetail["ContactFirstName"] = Comp.CompName;
                    }
                    else
                    {
                        EmailDetail["ContactFirstName"] = Comp.ContactFirstName;
                    }
                    if (string.IsNullOrEmpty(model.ReqFirstName))
                    {
                        EmailDetail["ReqFirstName"] = model.SaleCompany;
                    }
                    else
                    {
                        EmailDetail["ReqFirstName"] = model.SaleName;
                    }

                    // ไม่ได้ระบุชื่อบริษัท ให้ว่างไว้
                    if (model.CompanyName.Equals("ไม่ระบุชื่อบริษัท") || model.CompanyName == null)
                    {
                        EmailDetail["CompanyName"] = "";
                    }
                    else
                    {
                        EmailDetail["CompanyName"] = "[" + model.SaleCompany+ "]";
                    }
                    if (toCompName.Equals("ไม่ระบุชื่อบริษัท") || toCompName == null)
                    {
                        EmailDetail["toCompName"] = "";
                    }
                    else
                    {
                        EmailDetail["toCompName"] = "[" + Comp.CompName + "]";
                    }

                    ViewBag.Data = EmailDetail;
                    Detail = PartialViewToString("UC/Email/Quotation");
                    Subject = res.Email.lblSubjectQuotation1 + " '" + ProductName + "' " + res.Email.lblSubjectQuotation2 + " " + res.Email.lblWeb_B2BThai;
                    mailTo.Add(model.SaleEmail);
                    IsSend = OnSendByAlertEmail(Subject, mailFrom, mailTo, mailCC, Detail);

                    //type = 1;
                    break;
                    #endregion
                // ปฏิเสธ
                case "Reject":
                    #region ปฏิเสธ
                    var CompReject = svCompany.SelectData<b2bCompany>("*", "CompID = '" + model.FromCompID + "'", null, 1, 1).First();
                    EmailDetail["b2bthaiUrl"] = urlb2bthai;
                    EmailDetail["pathLogo"] = pathlogo;
                    EmailDetail["pathView"] = pathView;
                    EmailDetail["pathPro"] = pathPro;
                    EmailDetail["pathSeller"] = urlb2bthai + "/Message/Contact/?ToCompID=" + model.FromCompID + "&ProductID=" + ProductID;
                    EmailDetail["pathIsClose"] = urlb2bthai + "/MyB2B/Quotation/Detail/" + model.QuotationID;

                    EmailDetail["Qty"] = model.Qty;
                    EmailDetail["QtyUnit"] = model.QtyUnit;

                    EmailDetail["ProductName"] = ProductName;
                    EmailDetail["ProductCode"] = ProductCode;
                    EmailDetail["toCompEmail"] = CompReject.ContactEmail;
                    EmailDetail["ToTel"] = CompReject.CompPhone;

                    // ข้อมูลรูปภาพ
                    EmailDetail["CompID"] = CompProductImg;
                    EmailDetail["ProductID"] = ProductID;
                    EmailDetail["ImgPath"] = ProductImgPath;

                    // ชื่อเป็นค่าว่าง ให้ใช้ชื่อบริษัทแทน
                    if (string.IsNullOrEmpty(ContactFirstName))
                    {
                        EmailDetail["ContactFirstName"] = CompReject.CompName;
                    }
                    else
                    {
                        EmailDetail["ContactFirstName"] = CompReject.ContactFirstName;
                    }
                    if (string.IsNullOrEmpty(model.ReqFirstName))
                    {
                        EmailDetail["ReqFirstName"] = model.SaleCompany;
                    }
                    else
                    {
                        EmailDetail["ReqFirstName"] = model.SaleName;
                    }

                    // ไม่ได้ระบุชื่อบริษัท ให้ว่างไว้
                    if (model.CompanyName.Equals("Not specified company") || model.CompanyName == null)
                    {
                        EmailDetail["CompanyName"] = "";
                    }
                    else
                    {
                        EmailDetail["CompanyName"] = "[" + model.SaleCompany + "]";
                    }
                    if (toCompName.Equals("Not specified company") || toCompName == null)
                    {
                        EmailDetail["toCompName"] = "";
                    }
                    else
                    {
                        EmailDetail["toCompName"] = "[" + CompReject.CompName + "]";
                    }

                    EmailDetail["MsgDetail"] = MsgDetail;

                    ViewBag.Data = EmailDetail;
                    Detail = PartialViewToString("UC/Email/Rejects");
                    Subject = res.Email.lblReject1 + " '" + ProductName + "' " + res.Email.lblSubjectQuotation3 + " " + res.Email.lblWeb_B2BThai;
                    mailTo.Add(model.SaleEmail);
                    IsSend = OnSendByAlertEmail(Subject, mailFrom, mailTo, mailCC, Detail);

                    //type = 1;
                    break;
                    #endregion
            }
            #endregion

            return IsSend;
        }

        #endregion

        #region TestSendEmail1
        // ส่ง Email ถึงผู้ซื้อ (รายละเอียดสินค้าที่ขอ)  กับ ส่ง Email ถึงผู้ขาย (เจ้าของสินค้า)
        public bool TestSendEmail1(b2bQuotation model, string ContactFirstName, string toCompName, string toCompEmail, string ProductName, string Status, int QuotationID, string CompPhone, string CompLevel, int CompID, int ProductID, string ProductImgPath)
        {
            #region variable
            string Subject = "";
            var Detail = "";
            bool IsSend = true;
            var mailToSeller = new List<string>();
            var mailToBuyer = new List<string>();
            List<string> mailToAdmin = GetMailListB2BAdmin();
            var mailCC = new List<string>();
            var Sender = "";
            var Receiver = "";
            svCompany = new CompanyService();
            Hashtable EmailDetail = new Hashtable();

            #endregion

            #region Set Content & Value For Send Email

            string urlb2bthai = res.Pageviews.UrlWeb;
            string pathlogo = urlb2bthai + "/Content/Default/logo/Ouikum/img_Logo120x74.png";
            string pathF = urlb2bthai + "/Content/Default/images/icon_freesmall.png";
            string pathG = urlb2bthai + "/Content/Default/images/icon_goldsmall.png";
            string pathView = urlb2bthai + "/MyB2B/Quotation/List";
            string pathPro = urlb2bthai + "/Search/Product/Detail/" + ProductID + "/" + ProductName;
            string pathBid = urlb2bthai + "/MyB2B/Quotation/Detail/" + model.QuotationID;

            EmailDetail["b2bthaiUrl"] = urlb2bthai;
            EmailDetail["pathLogo"] = pathlogo;
            EmailDetail["pathView"] = pathView;
            EmailDetail["pathPro"] = pathPro;
            EmailDetail["pathBid"] = pathBid;

            EmailDetail["Qty"] = model.Qty;
            EmailDetail["QtyUnit"] = model.QtyUnit;
            EmailDetail["ReqEmail"] = model.ReqEmail;
            EmailDetail["ReqPhone"] = model.ReqPhone;
            EmailDetail["ProductName"] = ProductName;
            EmailDetail["toCompEmail"] = toCompEmail;
            EmailDetail["CompPhone"] = CompPhone;

            // ข้อมูลรูปภาพ
            EmailDetail["CompID"] = CompID;
            EmailDetail["ProductID"] = ProductID;
            EmailDetail["ImgPath"] = ProductImgPath;

            switch (Status)
            {
                case "Request":
                    // ชื่อเป็นค่าว่าง ให้ใช้ชื่อบริษัทแทน
                    if (string.IsNullOrEmpty(ContactFirstName))
                    {
                        EmailDetail["ContactFirstName"] = toCompName;
                    }
                    else
                    {
                        EmailDetail["ContactFirstName"] = ContactFirstName;
                    }
                    if (string.IsNullOrEmpty(model.ReqFirstName))
                    {
                        EmailDetail["ReqFirstName"] = model.CompanyName;
                    }
                    else
                    {
                        EmailDetail["ReqFirstName"] = model.ReqFirstName;
                    }

                    // ไม่ได้ระบุชื่อบริษัท ให้ว่างไว้
                    if (model.CompanyName.Equals("ไม่ระบุชื่อบริษัท") || model.CompanyName == null)
                    {
                        EmailDetail["CompanyName"] = "";
                    }
                    else
                    {
                        EmailDetail["CompanyName"] = "[" + model.CompanyName + "]";
                    }
                    if (toCompName.Equals("ไม่ระบุชื่อบริษัท") || toCompName == null)
                    {
                        EmailDetail["toCompName"] = "";
                    }
                    else
                    {
                        EmailDetail["toCompName"] = "[" + toCompName + "]";
                    }

                    //var svProduct = new ProductService();
                    //var data = svProduct.GetProductRelateByProduct(ProductID, 4);

                    //if (data.Count() != null && data.Count() > 0)
                    //{
                    //    for (int i = 0; i < data.Count(); i++)
                    //    {
                    //        EmailDetail["ProductName" + i] = data[i].ProductName;

                    //        // ข้อมูลรูปภาพ
                    //        EmailDetail["CompID" + i] = data[i].CompID;
                    //        EmailDetail["ProductID" + i] = data[i].ProductID;
                    //        EmailDetail["ImgPath" + i] = data[i].ProductImgPath;
                    //        EmailDetail["parthProducts" + i] = urlb2bthai + "/Search/Product/Detail/" + data[i].ProductID + "/" + data[i].ProductName;
                    //    }
                    //}

                    ViewBag.Data = EmailDetail;
                    var mailFrom = res.Config.EmailNoReply;

                    var SubjectSeller = res.Email.lblRequestedPrices + " '" + ProductName + "' " + res.Email.lblWeb_B2BThai;
                    mailToSeller.Add(model.ReqEmail);
                    var DetailSeller = PartialViewToString("UC/Email/ProductDetailsRequest");
                    IsSend = OnSendByAlertEmail(SubjectSeller, mailFrom, mailToSeller, mailCC, DetailSeller);

                    var SubjectBuyer = res.Quotation.lblReqPriceSubject1 + " '" + ProductName + "' " + res.Email.lblWeb_B2BThai;
                    mailToBuyer.Add(toCompEmail);
                    var DetailBuyer = PartialViewToString("UC/Email/ProductInformationRequests");
                    IsSend = OnSendByAlertEmail(SubjectBuyer, mailFrom, mailToBuyer, mailCC, DetailBuyer);

                    break;

                case "Reply":
                    string Phone = "";
                    string Email = "";
                    string FirstName = "";
                    string LastName = "";
                    if (model.FromCompID == 0)
                    {
                        var svQuotation = new QuotationService();
                        var FromComp = svQuotation.SelectData<b2bQuotation>("*", "QuotationID = " + QuotationID, "CreatedDate", 1, 0).First();
                        Sender = FromComp.SaleCompany;
                        Phone = FromComp.SalePhone;
                        Email = FromComp.SaleEmail;
                        FirstName = FromComp.SaleName;
                    }
                    else
                    {
                        var Comp = svCompany.SelectData<b2bCompany>("*", "CompID = '" + model.FromCompID + "'", null, 1, 1).First();
                        Sender = Comp.CompName;
                        if (!string.IsNullOrEmpty(Comp.ContactPhone))
                        { Phone = Comp.ContactPhone; }
                        else
                        { Phone = Comp.CompPhone; }
                        Email = Comp.ContactEmail;
                        FirstName = Comp.ContactFirstName;
                        LastName = Comp.ContactLastName;

                    }
                    Subject = res.Quotation.lblQuotationSubject1 + " " + ProductName + " " + res.Quotation.lblQuotationSubject2;
                    Detail = "<table><tr><td>" + res.Common.lblDear + " " + ContactFirstName + " [ " + Receiver + " ] </td></tr>";
                    Detail += "<tr><td><br>" + res.Message_Center.lblKhun + " " + FirstName + " " + LastName + "  [" + Sender + "] </td></tr>";
                    Detail += "<tr><td>" + res.Common.lblTel + " " + Phone + "</td></tr>";

                    Detail += "<tr><td>" + res.Common.lblEmail + " " + Email + "</td></tr>";
                    Detail += "<tr><td><br>" + res.Quotation.lblQuotationDetailFollow1 + " " + ProductName + " " + res.Quotation.lblQuotationDetailFollow2 + " :</td></tr>";

                    Detail += "<tr><td><br>" + res.Quotation.lblProductName + " " + ProductName + "</td></tr>";
                    Detail += "<tr><td>" + res.Quotation.lblQty + " " + model.Qty + " " + model.QtyUnit + "</td></tr>";

                    Detail += "<tr><td><br>" + res.Quotation.lblPricePerPiece + " : " + String.Format("{0:#,###.##}", model.PricePerPiece) + " " + res.Order.lblBaht + "</td></tr>";
                    Detail += "<tr><td>" + res.Order.lblDiscount + " : " + String.Format("{0:#,###.##}", model.Discount) + " " + res.Order.lblBaht + "</td></tr>";
                    Detail += "<tr><td>" + res.Quotation.lblVat + " : " + String.Format("{0:#,###.##}", model.Vat) + "</td></tr>";
                    Detail += "<tr><td>" + res.Quotation.lblTotalAmount + " : " + String.Format("{0:#,###.##}", model.TotalPrice) + "</td></tr>";

                    Detail += "<tr><td><br>-------------------------------------------------------------------------------------------------------------------</td></tr>";
                    if (model.ToCompID != 0)
                    {
                        Detail += "<tr><td>" + res.Quotation.lblDes_QuotationLink2 + " : <a href=\"" + urlb2bthai + "/MyB2B/Quotation/Detail/" + model.QuotationID + "\" target=\"_blank\">" + urlb2bthai + "/MyB2B/Quotation/Detail/" + model.QuotationID + "</a><br></td></tr>";
                    }
                    else
                    {
                        Detail += "<tr><td>" + res.Quotation.lblDes_QuotationLink2 + " : <a href=\"" + urlb2bthai + "/BidProduct/Reply/" + model.QuotationID + "\" target=\"_blank\">" + urlb2bthai + "/BidProduct/Reply/" + model.QuotationID + "</a><br></td></tr>";
                    }
                    Detail += "<tr><td><br><br><b>" + res.Message_Center.lblEmailNote + " </b>" + res.Message_Center.lblEmailNoteDetail + " </td></tr>";
                    Detail += "<tr><td>" + res.Common.lblForMoreInfo + " " + res.Common.lblSupportCustomer + " " + res.Common.phone_support1 + " " + res.Common.lblOr + " " + res.Common.phone_sale1 + "</td></tr>";
                    Detail += "<tr><td><br>" + res.Message_Center.lblSincerely + "<br><a href=\"" + urlb2bthai + "\" ><img src=\"" + pathlogo + "\"/></a><br>" + "<a href=\"" + urlb2bthai + "\" >" + res.Message_Center.lblTeam + "</a></td></tr></table>";
                    break;

                case "Reject":
                    Subject = "Request Price Reject";
                    //Detail = MsgDetail;
                    Detail += "<br><br><b>" + res.Message_Center.lblEmailNote + " </b>" + res.Message_Center.lblEmailNoteDetail;
                    Detail += "<br>" + res.Message_Center.lblSincerely + "<br><a href=\"" + urlb2bthai + "\" ><img src=\"" + pathlogo + "\"/></a><br>" + "<a href=\"" + urlb2bthai + "\" >" + res.Message_Center.lblTeam + "</a>";
                    break;
            }
            //var mailFrom = res.Config.EmailNoReply;
            //mailTo.Add(toCompEmail);
            #endregion


            //IsSend = OnSendByAlertEmail(Subject, mailFrom, mailTo, mailCC, Detail);

            if (model.ToCompID > 0)
            {
                Subject = Sender + "  " + res.Message_Center.lblContacted + " " + Receiver + " " + res.Message_Center.lblVia;
            }
            else
            {
                Subject = Sender + " " + res.Message_Center.lblContacted + " " + ContactFirstName + " (" + toCompEmail + ") " + res.Message_Center.lblVia;
            }
            //IsSend = OnSendByAlertEmail(Subject, mailFrom, mailToAdmin, mailCC, Detail);

            return IsSend;
        }

        #endregion

        #region IsSupplieRelated
        public bool IsSupplieRelated(b2bQuotation model, string ContactFirstName, string toCompName, string toCompEmail, string ProductName, string Status, int CompID, int ProductID, string ProductImgPath)
        {
            #region variable
            bool IsSend = true;
            var Detail = "";
            var mailTo = new List<string>();
            List<string> mailToAdmin = GetMailListB2BAdmin();
            var mailCC = new List<string>();
            string Subject = "";
            svCompany = new CompanyService();
            Hashtable EmailDetail = new Hashtable();

            #endregion

            #region Set Content & Value For Send Email

            string urlb2bthai = res.Pageviews.UrlWeb;
            string pathlogo = urlb2bthai + "/Content/Default/logo/Ouikum/img_Logo120x74.png";
            string pathView = urlb2bthai + "/MyB2B/Quotation/List";
            string pathPro = urlb2bthai + "/Search/Product/Detail/" + ProductID + "/" + ProductName;
            string pathBid = urlb2bthai + "/MyB2B/Quotation/Detail/" + model.QuotationID;

            EmailDetail["b2bthaiUrl"] = urlb2bthai;
            EmailDetail["pathLogo"] = pathlogo;
            EmailDetail["pathView"] = pathView;
            EmailDetail["pathPro"] = pathPro;
            EmailDetail["pathBid"] = pathBid;

            EmailDetail["Qty"] = model.Qty;
            EmailDetail["QtyUnit"] = model.QtyUnit;
            EmailDetail["ReqEmail"] = model.ReqEmail;
            EmailDetail["ReqPhone"] = model.ReqPhone;
            EmailDetail["ProductName"] = ProductName;
            EmailDetail["toCompEmail"] = toCompEmail;

            // ข้อมูลรูปภาพ
            EmailDetail["CompID"] = CompID;
            EmailDetail["ProductID"] = ProductID;
            EmailDetail["ImgPath"] = ProductImgPath;

            // ชื่อเป็นค่าว่าง ให้ใช้ชื่อบริษัทแทน
            if (string.IsNullOrEmpty(ContactFirstName))
            {
                EmailDetail["ContactFirstName"] = toCompName;
            }
            else
            {
                EmailDetail["ContactFirstName"] = ContactFirstName;
            }
            if (string.IsNullOrEmpty(model.ReqFirstName))
            {
                EmailDetail["ReqFirstName"] = model.CompanyName;
            }
            else
            {
                EmailDetail["ReqFirstName"] = model.ReqFirstName;
            }

            // ไม่ได้ระบุชื่อบริษัท ให้ว่างไว้
            if (model.CompanyName.Equals("ไม่ระบุชื่อบริษัท") || model.CompanyName == null)
            {
                EmailDetail["CompanyName"] = "";
            }
            else
            {
                EmailDetail["CompanyName"] = "[" + model.CompanyName + "]";
            }

            #endregion

            ViewBag.Data = EmailDetail;
            var mailFrom = res.Config.EmailNoReply;
            mailTo.Add(toCompEmail);

            Subject = res.Email.lblSubjectIsSupplieRelated + " '" + ProductName + "' " + res.Email.lblWeb_B2BThai;

            Detail = PartialViewToString("UC/Email/ProductInformationRequestOwnership");
            IsSend = OnSendByAlertEmail(Subject, mailFrom, mailTo, mailCC, Detail);
            return IsSend;
        }
        #endregion

        #region ToBuyerRelatedProducts
        // ข้อมูลสินค้าที่เกี่ยวข้อง ถึงผู้ซื้อ(คนที่เข้ามาร้องขอราคา)
        public bool ToBuyerRelatedProducts(b2bQuotation model, int ProductID, string ProductName)
        {
            #region variable
            bool IsSend = true;
            var Detail = "";
            var mailTo = new List<string>();
            List<string> mailToAdmin = GetMailListB2BAdmin();
            var mailCC = new List<string>();
            var Subject = "";
            Hashtable EmailDetail = new Hashtable();

            mailTo.Add(model.ReqEmail);
            var mailFrom = res.Config.EmailNoReply;
            #endregion

            string urlb2bthai = res.Pageviews.UrlWeb;
            string pathlogo = urlb2bthai + "/Content/Default/logo/Ouikum/img_Logo120x74.png";
            string pathF = urlb2bthai + "/Content/Default/images/icon_freesmall.png";
            string pathG = urlb2bthai + "/Content/Default/images/icon_goldsmall.png";

            EmailDetail["b2bthaiUrl"] = urlb2bthai;
            EmailDetail["pathLogo"] = pathlogo;
            EmailDetail["ReqFirstName"] = model.ReqFirstName;

            // ไม่ได้ระบุชื่อบริษัท ให้ว่างไว้
            if (model.CompanyName.Equals("ไม่ระบุชื่อบริษัท") || model.CompanyName == null)
            {
                EmailDetail["CompanyName"] = "";
            }
            else
            {
                EmailDetail["CompanyName"] = "[" + model.CompanyName + "]";
            }

            var svProduct = new ProductService();
            var data = svProduct.GetProductRelateByProduct(ProductID, 5);

            if (data.Count() != null && data.Count() > 0)
            {
                Subject = res.Email.lblItemsSuitable + " '" + model.ReqFirstName + "' " + res.Email.lblWeb_B2BThai;
                EmailDetail["TextSearch"] = urlb2bthai + "/search/product/list?textsearch=" + ProductName;

                for (int i = 0; i < data.Count(); i++)
                {
                    EmailDetail["ProductName" + i] = data[i].ProductName;
                    EmailDetail["TextSearch" + i] = urlb2bthai + "/Search/Product/List/?TextSearch=" + data[i].ProductKeyword;
                    EmailDetail["pathSeller"] = urlb2bthai + "/Message/Contact/?ToCompID=" + data[i].CompID + "&ProductID=" + data[i].ProductID;
                    EmailDetail["pathQuotation"] = urlb2bthai + "/MyB2B/Quotation/RequestPrice/" + data[i].ProductID;
                    EmailDetail["pathCompName" + i] = urlb2bthai + "/CompanyWebsite/" + data[i].CompName + "/Main/Index/" + data[i].CompID;
                    // ข้อมูลรูปภาพ
                    EmailDetail["CompID" + i] = data[i].CompID;
                    EmailDetail["ProductID" + i] = data[i].ProductID;
                    EmailDetail["ImgPath" + i] = data[i].ProductImgPath;
                    EmailDetail["CompName" + i] = data[i].CompName;

                    // ตรวจสอบราคาสินค้า กับ จำนวนสั่งซื้อเบื้องต้นว่าเป็น 0 หรือไม่
                    if (data[i].Price == 0)
                    {
                        EmailDetail["Price" + i] = "ไม่ระบุ";
                        EmailDetail["Qty" + i] = "ไม่ระบุ";
                    }
                    else
                    {
                        EmailDetail["Price" + i] = data[i].Price;
                        EmailDetail["Qty" + i] = data[i].Qty;
                    }
                    // ตรวจสอบแพคเกจ
                    if (data[i].CompLevel == 3)
                    {
                        EmailDetail["pathPackage" + i] = pathG;
                        EmailDetail["pathGF" + i] = " Gold Member";
                        EmailDetail["color" + i] = "#FF9933";
                    }
                    else
                    {
                        EmailDetail["pathPackage" + i] = pathF;
                        EmailDetail["pathGF" + i] = " Free Member";
                        EmailDetail["color" + i] = "#0099CC";
                    }
                }
            }

            ViewBag.Data = EmailDetail;
            Detail = PartialViewToString("UC/Email/RelatedProducts");
            IsSend = OnSendByAlertEmail(Subject, mailFrom, mailTo, mailCC, Detail);
            return IsSend;
        }


        #endregion

        #region IsQuotation
        // ถึงผู้ซื้อ(ใบเสนอราคาสินค้า)
        public bool IsQuotation(b2bQuotation model, string ContactFirstName, string toCompName, string toCompEmail, string ProductName, string Status, int QuotationID, string CompPhone, string CompLevel, int CompID, int ProductID, string ProductImgPath)
        {
            #region variable
            bool IsSend = true;
            var Detail = "";
            var mailTo = new List<string>();
            List<string> mailToAdmin = GetMailListB2BAdmin();
            var mailCC = new List<string>();
            var Subject = "";
            svCompany = new CompanyService();
            Hashtable EmailDetail = new Hashtable();

            #endregion

            #region Set Content & Value For Send Email

            string urlb2bthai = res.Pageviews.UrlWeb;
            string pathlogo = urlb2bthai + "/Content/Default/logo/Ouikum/img_Logo120x74.png";
            string pathF = urlb2bthai + "/Content/Default/images/icon_freesmall.png";

            EmailDetail["b2bthaiUrl"] = urlb2bthai;
            EmailDetail["pathLogo"] = pathlogo;

            EmailDetail["Qty"] = model.Qty;
            EmailDetail["QtyUnit"] = model.QtyUnit;
            EmailDetail["ProductName"] = ProductName;
            EmailDetail["toCompEmail"] = toCompEmail;

            //EmailDetail["PricePerPiece"] = model.Qty;
            //EmailDetail["Discount"] = model.QtyUnit;
            //EmailDetail["Vat"] = ProductName;
            //EmailDetail["TotalPrice"] = toCompEmail;
            //EmailDetail["Totle"] = ProductName;

            // ข้อมูลรูปภาพ
            EmailDetail["CompID"] = CompID;
            EmailDetail["ProductID"] = ProductID;
            EmailDetail["ImgPath"] = ProductImgPath;


            // ชื่อเป็นค่าว่าง ให้ใช้ชื่อบริษัทแทน
            if (string.IsNullOrEmpty(ContactFirstName))
            {
                EmailDetail["ContactFirstName"] = toCompName;
            }
            else
            {
                EmailDetail["ContactFirstName"] = ContactFirstName;
            }
            if (string.IsNullOrEmpty(model.ReqFirstName))
            {
                EmailDetail["ReqFirstName"] = model.CompanyName;
            }
            else
            {
                EmailDetail["ReqFirstName"] = model.ReqFirstName;
            }

            // ไม่ได้ระบุชื่อบริษัท ให้ว่างไว้
            if (model.CompanyName.Equals("ไม่ระบุชื่อบริษัท") || model.CompanyName == null)
            {
                EmailDetail["CompanyName"] = "";
            }
            else
            {
                EmailDetail["CompanyName"] = "[" + model.CompanyName + "]";
            }
            if (toCompName.Equals("ไม่ระบุชื่อบริษัท") || toCompName == null)
            {
                EmailDetail["toCompName"] = "";
            }
            else
            {
                EmailDetail["toCompName"] = "[" + toCompName + "]";
            }

            //var svProduct = new ProductService();
            //var data = svProduct.GetProductRelateByProduct(ProductID, 4);

            //if (data.Count() != null && data.Count() > 0)
            //{
            //    for (int i = 0; i < data.Count(); i++)
            //    {
            //        EmailDetail["ProductName" + i] = data[i].ProductName;

            //        // ข้อมูลรูปภาพ
            //        EmailDetail["CompID" + i] = data[i].CompID;
            //        EmailDetail["ProductID" + i] = data[i].ProductID;
            //        EmailDetail["ImgPath" + i] = data[i].ProductImgPath;
            //        EmailDetail["parthProducts" + i] = urlb2bthai + "/Search/Product/Detail/" + data[i].ProductID + "/" + data[i].ProductName;
            //    }
            //}
            #endregion

            ViewBag.Data = EmailDetail;
            var mailFrom = res.Config.EmailNoReply;
            mailTo.Add(model.ReqEmail);

            Subject = res.Email.lblRequestedPrices + " '" + ProductName + "' " + res.Email.lblWeb_B2BThai;
            Detail = PartialViewToString("UC/Email/ProductDetailsRequest");
            IsSend = OnSendByAlertEmail(Subject, mailFrom, mailTo, mailCC, Detail);
            return IsSend;
        }


        #endregion

        #region SendEmail2
        public bool SendEmail2(b2bQuotation model, string ContactFirstName, string toCompName, string toCompEmail, string ProductName, string Status, int QuotationID, string ToTel, string CompLevel, int CompID, int ProductID, string ProductImgPath)
        {
            #region variable
            bool IsSend = true;
            var Detail = "";
            var mailTo = new List<string>();
            List<string> mailToAdmin = GetMailListB2BAdmin();
            var mailCC = new List<string>();
            var Sender = "";
            var Receiver = "";
            svCompany = new CompanyService();
            Hashtable EmailDetail = new Hashtable();

            #endregion

            #region Set Content & Value For Send Email

            string urlb2bthai = res.Pageviews.UrlWeb;
            string pathlogo = urlb2bthai + "/Content/Default/logo/Ouikum/img_Logo120x74.png";
            string pathF = urlb2bthai + "/Content/Default/images/icon_freesmall.png";
            string pathG = urlb2bthai + "/Content/Default/images/icon_goldsmall.png";

            EmailDetail["b2bthaiUrl"] = urlb2bthai;
            EmailDetail["pathLogo"] = pathlogo;

            // ตรวจสอบแพคเกจ
            if (CompLevel.Equals(3))
            {
                EmailDetail["pathPackage"] = pathG;
                EmailDetail["pathGF"] = " Gold Member";
                EmailDetail["color"] = "#FF9933";
            }
            else
            {
                EmailDetail["pathPackage"] = pathF;
                EmailDetail["pathGF"] = " Free Member";
                EmailDetail["color"] = "#0099CC";
            }

            // ข้อมูลผู้ซื้อ
            EmailDetail["Qty"] = model.Qty;
            EmailDetail["QtyUnit"] = model.QtyUnit;
            EmailDetail["ReqEmail"] = model.ReqEmail;
            EmailDetail["ReqPhone"] = model.ReqPhone;

            // ข้อมูลผู้ขาย
            EmailDetail["ProductName"] = ProductName;
            EmailDetail["ToTel"] = ToTel;
            EmailDetail["toCompEmail"] = toCompEmail;

            // ข้อมูลเสนอราคา
            EmailDetail["PricePerPiece"] = model.PricePerPiece;
            EmailDetail["Discount"] = model.Discount;
            EmailDetail["Vat"] = model.Vat;
            EmailDetail["TotalPrice"] = model.TotalPrice;

            // ข้อมูลรูปภาพ
            EmailDetail["CompID"] = CompID;
            EmailDetail["ProductID"] = ProductID;
            EmailDetail["ImgPath"] = ProductImgPath;



            string Subject = "";
            switch (Status)
            {
                case "Request":
                    //Sender = model.CompanyName;
                    //Subject = res.Quotation.lblReqPriceSubject1 + " '" + ProductName + "' " + res.Quotation.lblReqPriceSubject2;

                    #region อันเก่า
                    //if (string.IsNullOrEmpty(ContactFirstName))
                    //{
                    //    Detail = "<table><tr><td>" + res.Common.lblDear + " " + Receiver + " </td></tr>";
                    //}
                    //else
                    //{
                    //    Detail = "<table><tr><td>" + res.Common.lblDear + " " + ContactFirstName + " [ " + Receiver + " ]  </td></tr>";
                    //}
                    //if (Sender != res.Common.lblNonSpecific)
                    //{
                    //    Detail += "<tr><td><br>" + res.Message_Center.lblKhun + " " + model.ReqFirstName + " " + model.ReqLastName + "  [" + Sender + "] </td></tr>";
                    //}
                    //else
                    //{
                    //    Sender = model.ReqFirstName + " " + model.ReqLastName;
                    //    Detail += "<tr><td><br>" + res.Message_Center.lblKhun + " " + model.ReqFirstName + " " + model.ReqLastName + "</td></tr>";
                    //}
                    //Detail += "<tr><td>" + res.Common.lblTel + " " + model.ReqPhone + "</td></tr>";
                    //Detail += "<tr><td>" + res.Common.lblEmail + " " + model.ReqEmail + "</td></tr>";

                    //Detail += "<tr><td><br>" + res.Quotation.lblReqDetailFollow1 + " " + ProductName + " " + res.Quotation.lblReqDetailFollow2 + "</td></tr>";

                    //Detail += "<tr><td><br>" + res.Quotation.lblProductName + " " + ProductName + "</td></tr>";
                    //Detail += "<tr><td>" + res.Quotation.lblQty + " " + model.Qty + " " + model.QtyUnit + "</td></tr>";

                    //Detail += "<tr><td><br>-------------------------------------------------------------------------------------------------------------------</td></tr>";
                    //Detail += "<tr><td>" + res.Quotation.lblDes_QuotationLink + " : <a href=\"" + urlb2bthai + "/MyB2B/Quotation/Detail/" + model.QuotationID + "\" target=\"_blank\">" + urlb2bthai + "/MyB2B/Quotation/Detail/" + model.QuotationID + "</a></td></tr>";

                    //Detail += "<tr><td><br><br><b>" + res.Message_Center.lblEmailNote + " </b>" + res.Message_Center.lblEmailNoteDetail + " </td></tr>";
                    //Detail += "<tr><td>" + res.Common.lblForMoreInfo + " " + res.Common.lblSupportCustomer + " " + res.Common.phone_support1 + " " + res.Common.lblOr + " " + res.Common.phone_sale1 + "</td></tr>";
                    //Detail += "<tr><td><br>" + res.Message_Center.lblSincerely + "<br><a href=\"" + urlb2bthai + "\" ><img src=\"" + pathlogo + "\"/></a><br>" + "<a href=\"" + urlb2bthai + "\" >" + res.Message_Center.lblTeam + "</a></td></tr></table>";
                    #endregion
                    // ชื่อเป็นค่าว่าง ให้ใช้ชื่อบริษัทแทน
                    if (string.IsNullOrEmpty(ContactFirstName))
                    {
                        EmailDetail["ContactFirstName"] = toCompName;
                    }
                    else
                    {
                        EmailDetail["ContactFirstName"] = ContactFirstName;
                    }
                    if (string.IsNullOrEmpty(model.ReqFirstName))
                    {
                        EmailDetail["ReqFirstName"] = model.CompanyName;
                    }
                    else
                    {
                        EmailDetail["ReqFirstName"] = model.ReqFirstName;
                    }

                    // ไม่ได้ระบุชื่อบริษัท ให้ว่างไว้
                    if (model.CompanyName.Equals("ไม่ระบุชื่อบริษัท") || model.CompanyName == null)
                    {
                        EmailDetail["CompanyName"] = "";
                    }
                    else
                    {
                        EmailDetail["CompanyName"] = "[" + model.CompanyName + "]";
                    }
                    if (toCompName.Equals("ไม่ระบุชื่อบริษัท") || toCompName == null)
                    {
                        EmailDetail["toCompName"] = "";
                    }
                    else
                    {
                        EmailDetail["toCompName"] = "[" + toCompName + "]";
                    }

                    //var svProduct = new ProductService();
                    //var data = svProduct.GetProductRelateByProduct(ProductID, 4);

                    //if (data.Count() != null && data.Count() > 0)
                    //{
                    //    EmailDetail["TextSearch"] = urlb2bthai + "/search/product/list?textsearch=" + ProductName;

                    //    for (int i = 0; i < data.Count(); i++)
                    //    {
                    //        EmailDetail["ProductName" + i] = data[i].ProductName;

                    //        // ข้อมูลรูปภาพ
                    //        EmailDetail["CompID" + i] = data[i].CompID;
                    //        EmailDetail["ProductID" + i] = data[i].ProductID;
                    //        EmailDetail["ImgPath" + i] = data[i].ProductImgPath;
                    //        EmailDetail["CompName" + i] = data[i].CompName;
                    //    }
                    //}

                    ViewBag.Data = EmailDetail;
                    var mailFrom = res.Config.EmailNoReply;
                    mailTo.Add(toCompEmail);

                    //if (status == 1)
                    //{
                    Subject = res.Email.lblRequestedPrices + " '" + ProductName + "' " + res.Email.lblVia + res.Email.lblWeb_B2BThai;
                    Detail = PartialViewToString("UC/Email/ProductDetailsRequest");
                    IsSend = OnSendByAlertEmail(Subject, mailFrom, mailTo, mailCC, Detail);

                    Subject = res.Quotation.lblReqPriceSubject1 + " '" + ProductName + "' " + res.Email.lblYour + res.Email.lblVia + res.Email.lblWeb_B2BThai;
                    Detail = PartialViewToString("UC/Email/ProductInformationRequests");
                    IsSend = OnSendByAlertEmail(Subject, mailFrom, mailTo, mailCC, Detail);
                    //}
                    //else
                    //{
                    //Subject = res.Quotation.lblReqPriceSubject1 + " '" + ProductName + "' " + res.Email.lblWeb_B2BThai;
                    //Detail = PartialViewToString("UC/Email/ProductInformationRequestOwnership");
                    //IsSend = OnSendByAlertEmail(Subject, mailFrom, mailTo, mailCC, Detail);
                    //}
                    break;

                case "Reply":
                    string Phone = "";
                    string Email = "";
                    string FirstName = "";
                    string LastName = "";
                    if (model.FromCompID == 0)
                    {
                        var svQuotation = new QuotationService();
                        var FromComp = svQuotation.SelectData<b2bQuotation>("*", "QuotationID = " + QuotationID, "CreatedDate", 1, 0).First();
                        Sender = FromComp.SaleCompany;
                        Phone = FromComp.SalePhone;
                        Email = FromComp.SaleEmail;
                        FirstName = FromComp.SaleName;
                    }
                    else
                    {
                        var Comp = svCompany.SelectData<b2bCompany>("*", "CompID = '" + model.FromCompID + "'", null, 1, 1).First();
                        Sender = Comp.CompName;
                        if (!string.IsNullOrEmpty(Comp.ContactPhone))
                        { Phone = Comp.ContactPhone; }
                        else
                        { Phone = Comp.CompPhone; }
                        Email = Comp.ContactEmail;
                        FirstName = Comp.ContactFirstName;
                        LastName = Comp.ContactLastName;

                    }
                    Subject = res.Quotation.lblQuotationSubject1 + " " + ProductName + " " + res.Quotation.lblQuotationSubject2;
                    Detail = "<table><tr><td>" + res.Common.lblDear + " " + ContactFirstName + " [ " + Receiver + " ] </td></tr>";
                    Detail += "<tr><td><br>" + res.Message_Center.lblKhun + " " + FirstName + " " + LastName + "  [" + Sender + "] </td></tr>";
                    Detail += "<tr><td>" + res.Common.lblTel + " " + Phone + "</td></tr>";

                    Detail += "<tr><td>" + res.Common.lblEmail + " " + Email + "</td></tr>";
                    Detail += "<tr><td><br>" + res.Quotation.lblQuotationDetailFollow1 + " " + ProductName + " " + res.Quotation.lblQuotationDetailFollow2 + " :</td></tr>";

                    Detail += "<tr><td><br>" + res.Quotation.lblProductName + " " + ProductName + "</td></tr>";
                    Detail += "<tr><td>" + res.Quotation.lblQty + " " + model.Qty + " " + model.QtyUnit + "</td></tr>";

                    Detail += "<tr><td><br>" + res.Quotation.lblPricePerPiece + " : " + String.Format("{0:#,###.##}", model.PricePerPiece) + " " + res.Order.lblBaht + "</td></tr>";
                    Detail += "<tr><td>" + res.Order.lblDiscount + " : " + String.Format("{0:#,###.##}", model.Discount) + " " + res.Order.lblBaht + "</td></tr>";
                    Detail += "<tr><td>" + res.Quotation.lblVat + " : " + String.Format("{0:#,###.##}", model.Vat) + "</td></tr>";
                    Detail += "<tr><td>" + res.Quotation.lblTotalAmount + " : " + String.Format("{0:#,###.##}", model.TotalPrice) + "</td></tr>";

                    Detail += "<tr><td><br>-------------------------------------------------------------------------------------------------------------------</td></tr>";
                    if (model.ToCompID != 0)
                    {
                        Detail += "<tr><td>" + res.Quotation.lblDes_QuotationLink2 + " : <a href=\"" + urlb2bthai + "/MyB2B/Quotation/Detail/" + model.QuotationID + "\" target=\"_blank\">" + urlb2bthai + "/MyB2B/Quotation/Detail/" + model.QuotationID + "</a><br></td></tr>";
                    }
                    else
                    {
                        Detail += "<tr><td>" + res.Quotation.lblDes_QuotationLink2 + " : <a href=\"" + urlb2bthai + "/BidProduct/Reply/" + model.QuotationID + "\" target=\"_blank\">" + urlb2bthai + "/BidProduct/Reply/" + model.QuotationID + "</a><br></td></tr>";
                    }
                    Detail += "<tr><td><br><br><b>" + res.Message_Center.lblEmailNote + " </b>" + res.Message_Center.lblEmailNoteDetail + " </td></tr>";
                    Detail += "<tr><td>" + res.Common.lblForMoreInfo + " " + res.Common.lblSupportCustomer + " " + res.Common.phone_support1 + " " + res.Common.lblOr + " " + res.Common.phone_sale1 + "</td></tr>";
                    Detail += "<tr><td><br>" + res.Message_Center.lblSincerely + "<br><a href=\"" + urlb2bthai + "\" ><img src=\"" + pathlogo + "\"/></a><br>" + "<a href=\"" + urlb2bthai + "\" >" + res.Message_Center.lblTeam + "</a></td></tr></table>";
                    break;

                case "Reject":
                    Subject = "Request Price Reject";
                    //Detail = MsgDetail;
                    Detail += "<br><br><b>" + res.Message_Center.lblEmailNote + " </b>" + res.Message_Center.lblEmailNoteDetail;
                    Detail += "<br>" + res.Message_Center.lblSincerely + "<br><a href=\"" + urlb2bthai + "\" ><img src=\"" + pathlogo + "\"/></a><br>" + "<a href=\"" + urlb2bthai + "\" >" + res.Message_Center.lblTeam + "</a>";
                    break;
            }
            //var mailFrom = res.Config.EmailNoReply;
            //mailTo.Add(toCompEmail);
            #endregion


            //IsSend = OnSendByAlertEmail(Subject, mailFrom, mailTo, mailCC, Detail);

            if (model.ToCompID > 0)
            {
                Subject = Sender + "  " + res.Message_Center.lblContacted + " " + Receiver + " " + res.Message_Center.lblVia;
            }
            else
            {
                Subject = Sender + " " + res.Message_Center.lblContacted + " " + ContactFirstName + " (" + toCompEmail + ") " + res.Message_Center.lblVia;
            }
            //IsSend = OnSendByAlertEmail(Subject, mailFrom, mailToAdmin, mailCC, Detail);

            return IsSend;
        }

        #endregion

        #region TestSendEmail2
        public bool TestSendEmail2(b2bQuotation model, string ContactFirstName, string toCompName, string toCompEmail, string ProductName, string Status, string MsgDetail, int QuotationID)
        {
            #region variable
            bool IsSend = true;
            var Detail = "";
            var mailTo = new List<string>();
            List<string> mailToAdmin = GetMailListB2BAdmin();
            var mailCC = new List<string>();
            var Sender = "";
            var Receiver = "";
            svCompany = new CompanyService();
            #endregion

            Receiver = toCompName;

            #region Set Content & Value For Send Email

            string urlb2bthai = res.Pageviews.UrlWeb;
            string pathlogo = urlb2bthai + "/Content/Default/Images/img_LogoB2BThai120x74.png";
            string Subject = "";
            switch (Status)
            {
                case "Request":
                    Sender = model.CompanyName;
                    Subject = res.Quotation.lblReqPriceSubject1 + " '" + ProductName + "' " + res.Quotation.lblReqPriceSubject2;
                    if (string.IsNullOrEmpty(ContactFirstName))
                    {
                        Detail = "<table><tr><td>" + res.Common.lblDear + " " + Receiver + " </td></tr>";
                    }
                    else
                    {
                        Detail = "<table><tr><td>" + res.Common.lblDear + " " + ContactFirstName + " [ " + Receiver + " ]  </td></tr>";
                    }
                    if (Sender != res.Common.lblNonSpecific)
                    {
                        Detail += "<tr><td><br>" + res.Message_Center.lblKhun + " " + model.ReqFirstName + " " + model.ReqLastName + "  [" + Sender + "] </td></tr>";
                    }
                    else
                    {
                        Sender = model.ReqFirstName + " " + model.ReqLastName;
                        Detail += "<tr><td><br>" + res.Message_Center.lblKhun + " " + model.ReqFirstName + " " + model.ReqLastName + "</td></tr>";
                    }
                    Detail += "<tr><td>" + res.Common.lblTel + " " + model.ReqPhone + "</td></tr>";
                    Detail += "<tr><td>" + res.Common.lblEmail + " " + model.ReqEmail + "</td></tr>";

                    Detail += "<tr><td><br>" + res.Quotation.lblReqDetailFollow1 + " " + ProductName + " " + res.Quotation.lblReqDetailFollow2 + "</td></tr>";

                    Detail += "<tr><td><br>" + res.Quotation.lblProductName + " " + ProductName + "</td></tr>";
                    Detail += "<tr><td>" + res.Quotation.lblQty + " " + model.Qty + " " + model.QtyUnit + "</td></tr>";

                    Detail += "<tr><td><br>-------------------------------------------------------------------------------------------------------------------</td></tr>";
                    Detail += "<tr><td>" + res.Quotation.lblDes_QuotationLink + " : <a href=\"" + urlb2bthai + "/MyB2B/Quotation/Detail/" + model.QuotationID + "\" target=\"_blank\">" + urlb2bthai + "/MyB2B/Quotation/Detail/" + model.QuotationID + "</a></td></tr>";

                    Detail += "<tr><td><br><br><b>" + res.Message_Center.lblEmailNote + " </b>" + res.Message_Center.lblEmailNoteDetail + " </td></tr>";
                    Detail += "<tr><td>" + res.Common.lblForMoreInfo + " " + res.Common.lblSupportCustomer + " " + res.Common.phone_support1 + " " + res.Common.lblOr + " " + res.Common.phone_sale1 + "</td></tr>";
                    Detail += "<tr><td><br>" + res.Message_Center.lblSincerely + "<br><a href=\"" + urlb2bthai + "\" ><img src=\"" + pathlogo + "\"/></a><br>" + "<a href=\"" + urlb2bthai + "\" >" + res.Message_Center.lblTeam + "</a></td></tr></table>";
                    break;

                case "Reply":
                    string Phone = "";
                    string Email = "";
                    string FirstName = "";
                    string LastName = "";
                    if (model.FromCompID == 0)
                    {
                        var svQuotation = new QuotationService();
                        var FromComp = svQuotation.SelectData<b2bQuotation>("*", "QuotationID = " + QuotationID, "CreatedDate", 1, 0).First();
                        Sender = FromComp.SaleCompany;
                        Phone = FromComp.SalePhone;
                        Email = FromComp.SaleEmail;
                        FirstName = FromComp.SaleName;
                    }
                    else
                    {
                        var Comp = svCompany.SelectData<b2bCompany>("*", "CompID = '" + model.FromCompID + "'", null, 1, 1).First();
                        Sender = Comp.CompName;
                        if (!string.IsNullOrEmpty(Comp.ContactPhone))
                        { Phone = Comp.ContactPhone; }
                        else
                        { Phone = Comp.CompPhone; }
                        Email = Comp.ContactEmail;
                        FirstName = Comp.ContactFirstName;
                        LastName = Comp.ContactLastName;

                    }
                    Subject = res.Quotation.lblQuotationSubject1 + " " + ProductName + " " + res.Quotation.lblQuotationSubject2;
                    Detail = "<table><tr><td>" + res.Common.lblDear + " " + ContactFirstName + " [ " + Receiver + " ] </td></tr>";
                    Detail += "<tr><td><br>" + res.Message_Center.lblKhun + " " + FirstName + " " + LastName + "  [" + Sender + "] </td></tr>";
                    Detail += "<tr><td>" + res.Common.lblTel + " " + Phone + "</td></tr>";

                    Detail += "<tr><td>" + res.Common.lblEmail + " " + Email + "</td></tr>";
                    Detail += "<tr><td><br>" + res.Quotation.lblQuotationDetailFollow1 + " " + ProductName + " " + res.Quotation.lblQuotationDetailFollow2 + " :</td></tr>";

                    Detail += "<tr><td><br>" + res.Quotation.lblProductName + " " + ProductName + "</td></tr>";
                    Detail += "<tr><td>" + res.Quotation.lblQty + " " + model.Qty + " " + model.QtyUnit + "</td></tr>";

                    Detail += "<tr><td><br>" + res.Quotation.lblPricePerPiece + " : " + String.Format("{0:#,###.##}", model.PricePerPiece) + " " + res.Order.lblBaht + "</td></tr>";
                    Detail += "<tr><td>" + res.Order.lblDiscount + " : " + String.Format("{0:#,###.##}", model.Discount) + " " + res.Order.lblBaht + "</td></tr>";
                    Detail += "<tr><td>" + res.Quotation.lblVat + " : " + String.Format("{0:#,###.##}", model.Vat) + "</td></tr>";
                    Detail += "<tr><td>" + res.Quotation.lblTotalAmount + " : " + String.Format("{0:#,###.##}", model.TotalPrice) + "</td></tr>";

                    Detail += "<tr><td><br>-------------------------------------------------------------------------------------------------------------------</td></tr>";
                    if (model.ToCompID != 0)
                    {
                        Detail += "<tr><td>" + res.Quotation.lblDes_QuotationLink2 + " : <a href=\"" + urlb2bthai + "/MyB2B/Quotation/Detail/" + model.QuotationID + "\" target=\"_blank\">" + urlb2bthai + "/MyB2B/Quotation/Detail/" + model.QuotationID + "</a><br></td></tr>";
                    }
                    else
                    {
                        Detail += "<tr><td>" + res.Quotation.lblDes_QuotationLink2 + " : <a href=\"" + urlb2bthai + "/BidProduct/Reply/" + model.QuotationID + "\" target=\"_blank\">" + urlb2bthai + "/BidProduct/Reply/" + model.QuotationID + "</a><br></td></tr>";
                    }
                    Detail += "<tr><td><br><br><b>" + res.Message_Center.lblEmailNote + " </b>" + res.Message_Center.lblEmailNoteDetail + " </td></tr>";
                    Detail += "<tr><td>" + res.Common.lblForMoreInfo + " " + res.Common.lblSupportCustomer + " " + res.Common.phone_support1 + " " + res.Common.lblOr + " " + res.Common.phone_sale1 + "</td></tr>";
                    Detail += "<tr><td><br>" + res.Message_Center.lblSincerely + "<br><a href=\"" + urlb2bthai + "\" ><img src=\"" + pathlogo + "\"/></a><br>" + "<a href=\"" + urlb2bthai + "\" >" + res.Message_Center.lblTeam + "</a></td></tr></table>";
                    break;

                case "Reject":
                    Subject = "Request Price Reject";
                    Detail = MsgDetail;
                    Detail += "<br><br><b>" + res.Message_Center.lblEmailNote + " </b>" + res.Message_Center.lblEmailNoteDetail;
                    Detail += "<br>" + res.Message_Center.lblSincerely + "<br><a href=\"" + urlb2bthai + "\" ><img src=\"" + pathlogo + "\"/></a><br>" + "<a href=\"" + urlb2bthai + "\" >" + res.Message_Center.lblTeam + "</a>";
                    break;
            }
            var mailFrom = res.Config.EmailNoReply;
            mailTo.Add(toCompEmail);
            mailTo.Add(model.ReqEmail);
            #endregion

            IsSend = OnSendByAlertEmail(Subject, mailFrom, mailTo, mailCC, Detail);

            if (model.ToCompID > 0)
            {
                Subject = Sender + "  " + res.Message_Center.lblContacted + " " + Receiver + " " + res.Message_Center.lblVia;
            }
            else
            {
                Subject = Sender + " " + res.Message_Center.lblContacted + " " + ContactFirstName + " (" + toCompEmail + ") " + res.Message_Center.lblVia;
            }
            //IsSend = OnSendByAlertEmail(Subject, mailFrom, mailToAdmin, mailCC, Detail);

            return IsSend;
        }

        #endregion

        #region TestSendEmail
        public bool TestSendEmail(b2bQuotation model, string ContactFirstName, string toCompName, string toCompEmail, string ProductName, string Status, string MsgDetail, int QuotationID)
        {
            #region variable
            bool IsSend = true;
            var Detail = "";
            var mailTo = new List<string>();
            List<string> mailToAdmin = GetMailListB2BAdmin();
            var mailCC = new List<string>();
            var Sender = "";
            var Receiver = "";
            svCompany = new CompanyService();
            #endregion

            Receiver = toCompName;

            #region Set Content & Value For Send Email

            string urlb2bthai = res.Pageviews.UrlWeb;
            string pathlogo = urlb2bthai + "/Content/Default/Images/img_LogoB2BThai120x74.png";
            string Subject = "";
            switch (Status)
            {
                case "Request":
                    Sender = model.CompanyName;
                    Subject = res.Quotation.lblReqPriceSubject1 + " '" + ProductName + "' " + res.Quotation.lblReqPriceSubject2;
                    if (string.IsNullOrEmpty(ContactFirstName))
                    {
                        Detail = "<table><tr><td>" + res.Common.lblDear + " " + Receiver + " </td></tr>";
                    }
                    else
                    {
                        Detail = "<table><tr><td>" + res.Common.lblDear + " " + ContactFirstName + " [ " + Receiver + " ]  </td></tr>";
                    }
                    if (Sender != res.Common.lblNonSpecific)
                    {
                        Detail += "<tr><td><br>" + res.Message_Center.lblKhun + " " + model.ReqFirstName + " " + model.ReqLastName + "  [" + Sender + "] </td></tr>";
                    }
                    else
                    {
                        Sender = model.ReqFirstName + " " + model.ReqLastName;
                        Detail += "<tr><td><br>" + res.Message_Center.lblKhun + " " + model.ReqFirstName + " " + model.ReqLastName + "</td></tr>";
                    }
                    Detail += "<tr><td>" + res.Common.lblTel + " " + model.ReqPhone + "</td></tr>";
                    Detail += "<tr><td>" + res.Common.lblEmail + " " + model.ReqEmail + "</td></tr>";

                    Detail += "<tr><td><br>" + res.Quotation.lblReqDetailFollow1 + " " + ProductName + " " + res.Quotation.lblReqDetailFollow2 + "</td></tr>";

                    Detail += "<tr><td><br>" + res.Quotation.lblProductName + " " + ProductName + "</td></tr>";
                    Detail += "<tr><td>" + res.Quotation.lblQty + " " + model.Qty + " " + model.QtyUnit + "</td></tr>";

                    Detail += "<tr><td><br>-------------------------------------------------------------------------------------------------------------------</td></tr>";
                    Detail += "<tr><td>" + res.Quotation.lblDes_QuotationLink + " : <a href=\"" + urlb2bthai + "/MyB2B/Quotation/Detail/" + model.QuotationID + "\" target=\"_blank\">" + urlb2bthai + "/MyB2B/Quotation/Detail/" + model.QuotationID + "</a></td></tr>";

                    Detail += "<tr><td><br><br><b>" + res.Message_Center.lblEmailNote + " </b>" + res.Message_Center.lblEmailNoteDetail + " </td></tr>";
                    Detail += "<tr><td>" + res.Common.lblForMoreInfo + " " + res.Common.lblSupportCustomer + " " + res.Common.phone_support1 + " " + res.Common.lblOr + " " + res.Common.phone_sale1 + "</td></tr>";
                    Detail += "<tr><td><br>" + res.Message_Center.lblSincerely + "<br><a href=\"" + urlb2bthai + "\" ><img src=\"" + pathlogo + "\"/></a><br>" + "<a href=\"" + urlb2bthai + "\" >" + res.Message_Center.lblTeam + "</a></td></tr></table>";
                    break;

                case "Reply":
                    string Phone = "";
                    string Email = "";
                    string FirstName = "";
                    string LastName = "";
                    if (model.FromCompID == 0)
                    {
                        var svQuotation = new QuotationService();
                        var FromComp = svQuotation.SelectData<b2bQuotation>("*", "QuotationID = " + QuotationID, "CreatedDate", 1, 0).First();
                        Sender = FromComp.SaleCompany;
                        Phone = FromComp.SalePhone;
                        Email = FromComp.SaleEmail;
                        FirstName = FromComp.SaleName;
                    }
                    else
                    {
                        var Comp = svCompany.SelectData<b2bCompany>("*", "CompID = '" + model.FromCompID + "'", null, 1, 1).First();
                        Sender = Comp.CompName;
                        if (!string.IsNullOrEmpty(Comp.ContactPhone))
                        { Phone = Comp.ContactPhone; }
                        else
                        { Phone = Comp.CompPhone; }
                        Email = Comp.ContactEmail;
                        FirstName = Comp.ContactFirstName;
                        LastName = Comp.ContactLastName;

                    }
                    Subject = res.Quotation.lblQuotationSubject1 + " " + ProductName + " " + res.Quotation.lblQuotationSubject2;
                    Detail = "<table><tr><td>" + res.Common.lblDear + " " + ContactFirstName + " [ " + Receiver + " ] </td></tr>";
                    Detail += "<tr><td><br>" + res.Message_Center.lblKhun + " " + FirstName + " " + LastName + "  [" + Sender + "] </td></tr>";
                    Detail += "<tr><td>" + res.Common.lblTel + " " + Phone + "</td></tr>";

                    Detail += "<tr><td>" + res.Common.lblEmail + " " + Email + "</td></tr>";
                    Detail += "<tr><td><br>" + res.Quotation.lblQuotationDetailFollow1 + " " + ProductName + " " + res.Quotation.lblQuotationDetailFollow2 + " :</td></tr>";

                    Detail += "<tr><td><br>" + res.Quotation.lblProductName + " " + ProductName + "</td></tr>";
                    Detail += "<tr><td>" + res.Quotation.lblQty + " " + model.Qty + " " + model.QtyUnit + "</td></tr>";

                    Detail += "<tr><td><br>" + res.Quotation.lblPricePerPiece + " : " + String.Format("{0:#,###.##}", model.PricePerPiece) + " " + res.Order.lblBaht + "</td></tr>";
                    Detail += "<tr><td>" + res.Order.lblDiscount + " : " + String.Format("{0:#,###.##}", model.Discount) + " " + res.Order.lblBaht + "</td></tr>";
                    Detail += "<tr><td>" + res.Quotation.lblVat + " : " + String.Format("{0:#,###.##}", model.Vat) + "</td></tr>";
                    Detail += "<tr><td>" + res.Quotation.lblTotalAmount + " : " + String.Format("{0:#,###.##}", model.TotalPrice) + "</td></tr>";

                    Detail += "<tr><td><br>-------------------------------------------------------------------------------------------------------------------</td></tr>";
                    if (model.ToCompID != 0)
                    {
                        Detail += "<tr><td>" + res.Quotation.lblDes_QuotationLink2 + " : <a href=\"" + urlb2bthai + "/MyB2B/Quotation/Detail/" + model.QuotationID + "\" target=\"_blank\">" + urlb2bthai + "/MyB2B/Quotation/Detail/" + model.QuotationID + "</a><br></td></tr>";
                    }
                    else
                    {
                        Detail += "<tr><td>" + res.Quotation.lblDes_QuotationLink2 + " : <a href=\"" + urlb2bthai + "/BidProduct/Reply/" + model.QuotationID + "\" target=\"_blank\">" + urlb2bthai + "/BidProduct/Reply/" + model.QuotationID + "</a><br></td></tr>";
                    }
                    Detail += "<tr><td><br><br><b>" + res.Message_Center.lblEmailNote + " </b>" + res.Message_Center.lblEmailNoteDetail + " </td></tr>";
                    Detail += "<tr><td>" + res.Common.lblForMoreInfo + " " + res.Common.lblSupportCustomer + " " + res.Common.phone_support1 + " " + res.Common.lblOr + " " + res.Common.phone_sale1 + "</td></tr>";
                    Detail += "<tr><td><br>" + res.Message_Center.lblSincerely + "<br><a href=\"" + urlb2bthai + "\" ><img src=\"" + pathlogo + "\"/></a><br>" + "<a href=\"" + urlb2bthai + "\" >" + res.Message_Center.lblTeam + "</a></td></tr></table>";
                    break;

                case "Reject":
                    Subject = "Request Price Reject";
                    Detail = MsgDetail;
                    Detail += "<br><br><b>" + res.Message_Center.lblEmailNote + " </b>" + res.Message_Center.lblEmailNoteDetail;
                    Detail += "<br>" + res.Message_Center.lblSincerely + "<br><a href=\"" + urlb2bthai + "\" ><img src=\"" + pathlogo + "\"/></a><br>" + "<a href=\"" + urlb2bthai + "\" >" + res.Message_Center.lblTeam + "</a>";
                    break;
            }
            var mailFrom = res.Config.EmailNoReply;
            mailTo.Add(toCompEmail);
            mailTo.Add(model.ReqEmail);
            #endregion

            IsSend = OnSendByAlertEmail(Subject, mailFrom, mailTo, mailCC, Detail);

            if (model.ToCompID > 0)
            {
                Subject = Sender + "  " + res.Message_Center.lblContacted + " " + Receiver + " " + res.Message_Center.lblVia;
            }
            else
            {
                Subject = Sender + " " + res.Message_Center.lblContacted + " " + ContactFirstName + " (" + toCompEmail + ") " + res.Message_Center.lblVia;
            }
            //IsSend = OnSendByAlertEmail(Subject, mailFrom, mailToAdmin, mailCC, Detail);

            return IsSend;
        }

        #endregion

        #region Update IsPublic
        public bool Update_IsPublic(string QuotationCode)
        {
            var svQuotation = new QuotationService();
            var Quotation = svQuotation.SelectData<b2bQuotation>("*", "QuotationCode = '" + QuotationCode + "'", null, 1, 1).First();

            svQuotation.UpdateByCondition<b2bQuotation>("IsClosed = 1", "QuotationCode = '" + QuotationCode + "'");
            svQuotation.UpdateByCondition<b2bQuotation>("IsClosed = 1", "QuotationCode = '" + Quotation.RootQuotationCode + "'");
            if (svQuotation.IsResult)
            {
                return svQuotation.IsResult;
            }
            else
            {
                return svQuotation.IsResult;
            }

        }
        #endregion

        #region Get BidProduct
        [HttpGet]
        public ActionResult BidProduct(string Code)
        {
            return Redirect("~/BidProduct/Detail/" + Code);
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

        #region Save File Create Message
        [HttpPost]
        public ActionResult SaveFileCreateQuotation(HttpPostedFileBase FileCreateQuotationPath)
        {
            imgManager = new FileHelper();

            if (LogonCompID > 0)
            {
                imgManager.UploadImage("Temp/QuotationFile/" + LogonCompID, FileCreateQuotationPath);
                Response.Cookies["CompID"].Value = Request.Cookies[res.Common.lblWebsite].Values["CompID"];
            }
            else
            {
                imgManager.UploadImage("Temp/QuotationFile/0", FileCreateQuotationPath);
                Response.Cookies["CompID"].Value = "0";
            }
            while (!imgManager.IsSuccess)
            {
                Thread.Sleep(100);
            }
            return Json(new { newimage = imgManager.ImageName }, "text/plain");
        }
        #endregion

        #region RemoveImage
        [HttpPost]
        public ActionResult RemoveFileCreateQuotation()
        {
            imgManager = new FileHelper();
            imgManager.DeleteFilesInDir("Temp/QuotationFile/" + LogonCompID);
            return Json(new { newimage = imgManager.ImageName });
        }
        #endregion

    }
}
