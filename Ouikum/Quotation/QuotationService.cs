using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using Prosoft.Base;
using System.Transactions;
//using System.Web.Mvc;
using System.Data.Linq;

using Prosoft.Service;
using Ouikum.Product;

namespace Ouikum.Quotation
{

    #region enum
    public enum QuotationAction
    {
        All, BackEnd, Admin, Important, Sentbox, Trash, myRequest, Draft, FrontEnd, CountQuotation
    }
    #endregion

    public class QuotationService : BaseSC
    {

        #region Quotation
        #region Property
        public int? ViewCount { get; set; }
        #endregion
        #region Method Validate
        #region ValidateInsert
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool ValidateInsert(b2bQuotation data)
        {
            try
            {
                b2bQuotation model = qDB.b2bQuotations.Single(q => q.QuotationID == data.QuotationID);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region ValidateUpdate
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool ValidateUpdateQuotation(b2bQuotation data)
        {
            try
            {
                b2bQuotation model = qDB.b2bQuotations.Single(q => q.QuotationID == data.QuotationID);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
        #endregion

        #region Method Select

        #region GetQuotation
        #region Generate SQLWhere
        public string CreateWhereAction(QuotationAction action, int? ToCompID = 0)
        {
            var sqlWhere = string.Empty;
            if (action == QuotationAction.All)
            {
                sqlWhere = " ( IsDelete = 0 )   ";
            }
            else if (action == QuotationAction.FrontEnd)
            {
                sqlWhere = " ( IsDelete = 0 AND  RowFlag  = 1 AND IsPublic = 1 AND IsClosed = 0) ";
            }
            else if (action == QuotationAction.BackEnd)
            {
                sqlWhere = " ( IsDelete = 0 AND  RowFlag  = 1 AND IsOutBox = 0 AND QuotationFolderID = 1)  ";
            }
            else if (action == QuotationAction.CountQuotation)
            {
                sqlWhere = " ( IsDelete = 0 AND  RowFlag  = 1 AND IsOutBox = 0 AND IsRead = 0 AND QuotationFolderID = 2)  ";
            }
            else if (action == QuotationAction.Admin)
            {
                sqlWhere = " ( IsDelete = 0 AND  RowFlag = 1 ) AND IsOutbox = 1 AND QuotationFolderID = 2";
            }
            else if (action == QuotationAction.Important)
            {
                sqlWhere = " ( IsDelete = 0 AND  IsImportance = 1 ) AND (ToCompID = " + ToCompID + ") ";
            }
            else if (action == QuotationAction.Sentbox)
            {
                sqlWhere = " ( IsDelete = 0 AND QuotationFolderID = 2 AND IsOutbox = 1 AND FromCompID = " + ToCompID + " ) ";
            }
            else if (action == QuotationAction.Trash)
            {
                sqlWhere = " ( IsDelete = 0 AND QuotationFolderID = 4 ";
            }
            else
            {
                sqlWhere = " ( IsDelete = 0 )   ";
            }

            if (action != QuotationAction.Sentbox)
            {
                if (action != QuotationAction.Trash)
                {
                if (ToCompID > 0)
                {
                    sqlWhere += " AND ToCompID  = " + ToCompID + " ";
                }
                }else{
                    sqlWhere += " AND (FromCompID = " + ToCompID + " OR ToCompID = " + ToCompID + ")) ";
                }
            }
            return sqlWhere;
        }

        public string CreateWhereCause(string txtSearch = "")
        {
            #region DoWhereCause

            if (txtSearch != "")
            {
                SQLWhere += " AND (QuotationCode LIKE N'%" + txtSearch + "%' ";
                SQLWhere += " OR CompanyName LIKE N'%" + txtSearch + "%' ";
                SQLWhere += " OR ProductName LIKE N'%" + txtSearch + "%' ";
                SQLWhere += " OR ReqFirstName LIKE N'%" + txtSearch + "%' ) ";
            }

            #endregion

            return SQLWhere;
        }

        #endregion
        #endregion

        #endregion

        #region Update
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateQuotation(b2bQuotation model)
        {
            var data = qDB.b2bQuotations.Single(q => q.QuotationID == model.QuotationID);

            // set ค่า model
            data.QuotationCode = model.QuotationCode;
            data.QuotationID = model.QuotationID;
            data.Qty = model.Qty;
            data.QtyUnit = model.QtyUnit;
            data.IsAttachQuote = model.IsAttach;
            data.IsEmail = model.IsEmail;
            data.IsTelephone = model.IsTelephone;
            data.RootQuotationCode = model.RootQuotationCode;
            data.FromCompID = model.FromCompID;
            data.ToCompID = model.ToCompID;
            data.CompanyName = model.CompanyName;
            data.ReqFirstName = model.ReqFirstName;
            data.ReqLastName = model.ReqLastName;
            data.ReqAddrLine1 = model.ReqAddrLine1;
            data.ReqAddrLine2 = model.ReqAddrLine1;
            data.ReqSubDistrict = model.ReqSubDistrict;
            data.ReqDistrictID = model.ReqDistrictID;
            data.ReqPostalCode = model.ReqPostalCode;
            data.ReqPhone = model.ReqPhone;
            data.ReqEmail = model.ReqEmail;
            data.QuotationFolderID = model.QuotationFolderID;
            data.SaleName = model.SaleName;
            data.SalePhone = model.SalePhone;
            data.SaleCompany = model.SaleCompany;
            data.SaleEmail = model.SaleEmail;
            data.PricePerPiece = model.PricePerPiece;
            data.TotalPrice = model.TotalPrice;
            data.Discount = model.Discount;
            data.Vat = model.Vat;
            data.IsPDFView = model.IsPDFView;
            data.IsSentEmail = model.IsSentEmail;
            data.IsReject = model.IsReject;
            data.RejectDetail = model.RejectDetail;
            data.IsAttach = model.IsAttach;
            data.IsImportance = model.IsImportance;
            data.IsRead = model.IsRead;
            data.IsReply = model.IsReply;
            data.QuotationStatus = model.QuotationStatus;
            data.Remark = model.Remark;
            data.SendDate = model.SendDate;
            data.OpenDate = model.OpenDate;
            // default
            data.RowVersion++;
            data.ModifiedBy = "sa";
            data.ModifiedDate = DateTime.Now;

            qDB.SubmitChanges();// บันทึกค่า
            return true;
        }

        public bool UpdateQuotation(List<b2bQuotation> model)
        {
            foreach (var m in model)
            {
                UpdateQuotation(m);
            }
            return true;
        }

        public bool CloseBidProductByCode(string code, bool IsClose = true)
        {
            if (IsClose)
            {
                UpdateByCondition<b2bQuotation>(" IsDelete = 1 ", " QuotationCode = N'" + code + "' ");
            }
            else
            {
                UpdateByCondition<b2bQuotation>(" IsDelete = 0 ", " QuotationCode = N'" + code + "' ");
            }

            return IsResult;
        }
        #endregion

        #region Delete
        public bool Delete(List<int> QuotationID)
        {
            var svCompany = new Company.CompanyService();

            //var Contains = SQLWhereListInt(QuotationID, "QuotationID");
            IsResult = UpdateByCondition<b2bQuotation>(" IsDelete = 1", " QuotationID = " + QuotationID);

            // IsResult = svCompany.UpdateQuotationCount(ToCompID);

            return IsResult;
        }
        #endregion

        #region Method Insert

        #region InsertQuotation

        #region ValidateSave
        #region ValidateQuotation
        private bool ValidateQuotation(b2bQuotation model)
        {
            //Example
            if (model.QuotationCode == null)
            {
                IsResult = false;
            }
            else if (model.ProductID == null)
            {
                IsResult = false;
            }

            return IsResult;
        }
        #endregion
        #endregion

        #region SaveQuotation
        #region Save Model
        public bool SaveQuotation(b2bQuotation model)
        {


            IsResult = true;
            if (!ValidateQuotation(model))
                return IsResult;

            using (var trans = new TransactionScope())
            {
                try
                {
                    if (model.QuotationID > 0)
                    {
                        qDB.b2bQuotations.Context.Refresh(RefreshMode.KeepCurrentValues, model);
                        qDB.b2bQuotations.InsertOnSubmit(model);// ทำการ save
                        qDB.SubmitChanges();

                    }

                    trans.Complete();
                }
                catch (Exception ex)
                {
                    IsResult = false;
                    MsgError.Add(ex);
                }
            }

            return IsResult;
        }
        #endregion

        #region Save Model List
        public bool SaveQuotation(List<b2bQuotation> lstmodel)
        {
            IsResult = true;
            foreach (var model in lstmodel)
            {
                if (!ValidateQuotation(model))
                    return IsResult;
            }

            using (var trans = new TransactionScope())
            {
                try
                {
                    foreach (var model in lstmodel)
                    {
                        if (model.QuotationID > 0)
                            qDB.b2bQuotations.Context.Refresh(RefreshMode.KeepCurrentValues, model);
                        else
                            qDB.b2bQuotations.InsertOnSubmit(model);

                        qDB.SubmitChanges();
                    }
                    trans.Complete();
                }
                catch (Exception ex)
                {
                    IsResult = false;
                    MsgError.Add(ex);
                }
            }

            return IsResult;
        }
        #endregion

        #region InsertQuotation
        public bool InsertQuotation(b2bQuotation Quotation)
        {
            try
            {
                Quotation.CreatedDate = DateTime.Now; ;
                Quotation.ModifiedDate = DateTime.Now; ;
                Quotation.CreatedBy = "sa";
                Quotation.ModifiedBy = "sa";
                Quotation.RowFlag = 1;
                Quotation.RowVersion = 1;
                Quotation.IsDelete = false;

                qDB.b2bQuotations.InsertOnSubmit(Quotation); // insert Quotation
                qDB.SubmitChanges();
                IsResult = true;
            }
            catch (Exception ex)
            {
                IsResult = false;
            }
            return IsResult;
        }
        #endregion
        #endregion

        #endregion

        #endregion

        #endregion

        #region Matching
        #region Get Mathing Company
        public List<b2bCompany> GetMatchingCompany(int productid)
        {
            var sqlwhere = @"IsDelete = 0 AND compid in (  select  DISTINCT compid from b2bproduct " +
                "where CateLV3 = (select CateLV3 from b2bproduct where productid = " + productid + "))";
            var companies = SelectData<b2bCompany>(" * ", sqlwhere, " modifieddate desc ", 1, 10);

            return companies;
        }
        #endregion

        public class QuotationModel
        {
            public int productid { get; set; }
            public int qty { get; set; }
            public string qtyunit { get; set; }
            public int tocompid { get; set; }
            public string compname { get; set; }
            public int fromcompid { get; set; }
            public bool ispublic { get; set; }
            public string phone { get; set; }
            public string email { get; set; }
            public string detail { get; set; }
            public string remark { get; set; }
            public bool isMatching { get; set; }
        }


        public bool SendMatching(
            string rootquotationcode,
            int productid,
            int qty,
            string qtyunit,
            int tocompid,
            string compname,
            int fromcompid,
            bool ispublic,
            string phone,
            string email,
            string detail = "",
            string status = "R",
            bool isMatching = false)
        {
            var outboxModel = new b2bQuotation();
            var inboxModel = new b2bQuotation();

            try
            {
                if (status != "R")
                {
                    status = "Q";
                }


                using (var trans = new TransactionScope())
                {
                    var count = CountData<b2bQuotation>(" * ", " CreatedDate = GetDate() AND RowFlag > 0");
                    var product = SelectData<view_ProductMobileApp>("* ", " productid = " + productid).First();

                    #region Set Remark
                    if (detail == null)
                    {
                        detail = compname + " ติดต่อคุณ เพื่อขอราคา " + product.ProductName;
                        if (qty != null && qty > 0)
                            detail += " จำนวน " + qty + " " + qtyunit;
                        detail += " ติดต่อกลับได้ที่  " + email + " หรือ โทร " + phone;
                    }
                    #endregion

                    #region Request Price
                    string Code = "";
                    string RootCode = "";
                    if (!string.IsNullOrEmpty(rootquotationcode))
                    {
                        Code = rootquotationcode;
                        RootCode = rootquotationcode;
                    }
                    else
                    {
                        Code = AutoGenCode("QO", count);
                        count = count + 1;
                    }

                    #region ข้อมูลร้องขอราคาสินค้า [Outbox]
                    outboxModel.ProductID = productid;
                    outboxModel.Qty = (short)DataManager.ConvertToSingle(qty);
                    outboxModel.QtyUnit = qtyunit;
                    outboxModel.ToCompID = tocompid;
                    outboxModel.QuotationCode = Code;
                    outboxModel.RootQuotationCode = RootCode;

                    outboxModel.CompanyName = compname;
                    outboxModel.ReqFirstName = compname;
                    outboxModel.ReqAddrLine1 = "";
                    outboxModel.ReqAddrLine2 = "";
                    outboxModel.ReqSubDistrict = "";
                    outboxModel.ReqPhone = phone;
                    outboxModel.ReqEmail = email;
                    outboxModel.FromCompID = fromcompid;
                    outboxModel.ReqRemark = detail;

                    outboxModel.IsSentEmail = false;
                    outboxModel.IsTelephone = false;
                    outboxModel.IsAttach = false;
                    outboxModel.IsAttachQuote = false;
                    outboxModel.IsEmail = false;
                    outboxModel.IsReply = false;
                    outboxModel.IsRead = false;
                    outboxModel.IsReject = false;
                    outboxModel.IsImportance = false;
                    outboxModel.IsPDFView = false;
                    outboxModel.IsOutbox = true;
                    outboxModel.SendDate = DateTimeNow;
                    outboxModel.IsClosed = false;
                    outboxModel.IsPublic = ispublic;

                    outboxModel.QuotationStatus = "R";
                    outboxModel.IsMatching = false;
                    //  outboxModel.Is
                    #endregion

                    #region ข้อมูลร้องขอราคาสินค้า[Inbpx]
                    inboxModel.ProductID = productid;
                    inboxModel.Qty = (short)qty;
                    inboxModel.QtyUnit = qtyunit;
                    inboxModel.ToCompID = tocompid;

                    inboxModel.CompanyName = compname;
                    inboxModel.ReqFirstName = compname;
                    inboxModel.ReqLastName = "";
                    inboxModel.ReqAddrLine1 = "";
                    inboxModel.ReqAddrLine2 = "";
                    inboxModel.ReqSubDistrict = "";
                    inboxModel.ReqPostalCode = "";
                    inboxModel.ReqPhone = phone;
                    inboxModel.ReqEmail = email;
                    inboxModel.FromCompID = fromcompid;
                    inboxModel.ReqRemark = detail;

                    inboxModel.IsSentEmail = false;
                    inboxModel.IsTelephone = false;
                    inboxModel.IsAttach = false;
                    inboxModel.IsAttachQuote = false;
                    inboxModel.IsEmail = false;
                    inboxModel.IsReply = false;
                    inboxModel.IsRead = false;
                    inboxModel.IsReject = false;
                    inboxModel.IsImportance = false;
                    inboxModel.IsPDFView = false;
                    inboxModel.IsOutbox = false;
                    inboxModel.SendDate = DateTimeNow;
                    inboxModel.IsClosed = false;
                    inboxModel.IsPublic = ispublic;
                    inboxModel.QuotationStatus = "R";
                    inboxModel.QuotationCode = Code;
                    inboxModel.RootQuotationCode = RootCode;
                    inboxModel.IsMatching = false;
                    #endregion

                    #region Insert Quotation
                    InsertQuotation(outboxModel);
                    InsertQuotation(inboxModel);
                    #endregion

                    #endregion


                    #region Matching
                    if (ispublic && string.IsNullOrEmpty(rootquotationcode))
                    {
                        var companies = GetMatchingCompany(productid);
                        companies.RemoveAll(x => x.CompID == tocompid);

                        #region loop for insert quotation
                        foreach (var comp in companies)
                        {
                            var isMatch = false;
                            #region ตั้ง ข้อความ
                            if (comp.CompID == tocompid)
                            {
                                isMatch = true;
                                detail = compname + " ติดต่อคุณ เพื่อขอราคา " + product.ProductName;

                                if (qty > 0)
                                    detail += " จำนวน " + qty + " " + qtyunit;
                            }
                            else
                            {
                                detail = compname + " กำลังสนใจ " + product.ProductName;
                                if (qty > 0)
                                    detail += " จำนวน " + qty + " " + qtyunit;

                                detail += " หากคุณมีสินค้าที่ใกล้เคียง สามารถเสนอราคากลับได้ที่ " + email + " หรือ โทร " + phone;
                            }
                            #endregion

                            outboxModel = new b2bQuotation();
                            inboxModel = new b2bQuotation();
                            Code = AutoGenCode("MC", count);
                            count++;

                            #region ข้อมูลร้องขอราคาสินค้า [Outbox]
                            outboxModel.ProductID = productid;
                            outboxModel.Qty = (short)DataManager.ConvertToSingle(qty);
                            outboxModel.QtyUnit = qtyunit;
                            outboxModel.ToCompID = comp.CompID;
                            outboxModel.ReqRemark = detail;

                            outboxModel.CompanyName = compname;
                            outboxModel.ReqFirstName = compname;
                            outboxModel.ReqAddrLine1 = "";
                            outboxModel.ReqAddrLine2 = "";
                            outboxModel.ReqSubDistrict = "";
                            outboxModel.ReqPhone = phone;
                            outboxModel.ReqEmail = email;
                            outboxModel.FromCompID = fromcompid;

                            outboxModel.IsSentEmail = false;
                            outboxModel.IsTelephone = false;
                            outboxModel.IsAttach = false;
                            outboxModel.IsAttachQuote = false;
                            outboxModel.IsEmail = false;
                            outboxModel.IsReply = false;
                            outboxModel.IsRead = false;
                            outboxModel.IsReject = false;
                            outboxModel.IsImportance = false;
                            outboxModel.IsPDFView = false;
                            outboxModel.IsOutbox = true;
                            outboxModel.SendDate = DateTimeNow;
                            outboxModel.IsClosed = false;
                            outboxModel.IsPublic = ispublic;
                            outboxModel.QuotationStatus = "R";
                            outboxModel.IsMatching = false;
                            outboxModel.QuotationCode = Code;
                            outboxModel.RootQuotationCode = RootCode;

                            #endregion

                            #region ข้อมูลร้องขอราคาสินค้า[Inbpx]
                            inboxModel.ProductID = productid;
                            inboxModel.Qty = (short)qty;
                            inboxModel.QtyUnit = qtyunit;
                            inboxModel.ToCompID = comp.CompID;
                            inboxModel.ReqRemark = detail;

                            inboxModel.FromCompID = fromcompid;
                            inboxModel.CompanyName = compname;
                            inboxModel.ReqFirstName = compname;
                            inboxModel.ReqLastName = "";
                            inboxModel.ReqAddrLine1 = "";
                            inboxModel.ReqAddrLine2 = "";
                            inboxModel.ReqSubDistrict = "";
                            inboxModel.ReqPostalCode = "";
                            inboxModel.ReqPhone = phone;
                            inboxModel.ReqEmail = email;

                            inboxModel.IsSentEmail = false;
                            inboxModel.IsTelephone = false;
                            inboxModel.IsAttach = false;
                            inboxModel.IsAttachQuote = false;
                            inboxModel.IsEmail = false;
                            inboxModel.IsReply = false;
                            inboxModel.IsRead = false;
                            inboxModel.IsReject = false;
                            inboxModel.IsImportance = false;
                            inboxModel.IsPDFView = false;
                            inboxModel.IsOutbox = false;
                            inboxModel.SendDate = DateTimeNow;
                            inboxModel.IsClosed = false;
                            inboxModel.IsPublic = ispublic;
                            inboxModel.QuotationStatus = "R";
                            inboxModel.QuotationCode = Code;
                            inboxModel.RootQuotationCode = RootCode;
                            inboxModel.IsMatching = true;
                            #endregion

                            #region Insert Quotation
                            InsertQuotation(outboxModel);
                            InsertQuotation(inboxModel);
                            #endregion

                        }
                        #endregion
                        // Share fb.
                        //PostFeedQuotationOnFacebook(model);

                        // Share tw.
                        //PostTweetsQuotationOnTwitter(model);
                    }
                    #endregion

                    trans.Complete();

                }

                return IsResult;
            }
            catch (Exception ex)
            {
                MsgError.Add(ex);
                return false;
            }
        }

        #endregion

        #region UpdatQuotationCount
        public bool UpdateQuotationCount(int QuotationID)
        {
            var Quotation = SelectData<b2bQuotation>("ViewCount,QuotationID", "QuotationID = " + QuotationID).First();

            if (Quotation.ViewCount != null)
            {
                if (Quotation.ViewCount == 0)
                    ViewCount = 1;
                else
                    ViewCount = (int)Quotation.ViewCount + 1;
            }

            string sqlUpdate = " ViewCount = " + ViewCount + ",ModifiedDate =GETDATE()";
            string sqlWhere = " QuotationID = " + QuotationID;
            UpdateByCondition<b2bQuotation>(sqlUpdate, sqlWhere);
            return IsResult;
        }
        #endregion
    }
}