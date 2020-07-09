using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using Prosoft.Base;
using System.Transactions;
//using System.Web.Mvc;
using System.Data.Linq;
using Prosoft.Service;

using Ouikum.Company;

namespace Ouikum.Order
{

    #region enum
    public enum OrderAction
    {
        All,
        BackEnd,
        FrontEnd,
        Junk,
        Admin,
        WebSite
    }
    public enum MemberPaidStatus
    {
        NoApprove,//N 
        Approve,//A   
        Reject,//R
    }
    public enum OrderStatus
    {
        NoApprove,//N 
        Approve,//A 
        Wait,//W  
        Cancel,//C
        Reject,//R
    }
    #endregion

    public class OrderService : BaseSC
    {
        public int MaxHot { get; set; }
        public int MaxFeat { get; set; }
        public int MaxQty { get; set; }
        public int MaxHotQty { get; set; }
        public int MaxFeatQty { get; set; }

        #region Generate SQLWhere
        public string CreateWhereAction(OrderStatus status, int CompID = 0)
        {
            var sqlWhere = string.Empty;
            if (status == OrderStatus.NoApprove)
            {
                sqlWhere = "(IsDelete = 0   AND Status = 'N') ";
            }
            else if (status == OrderStatus.Approve)
            {
                //comprowflag มาจาก b2bcompany.rowflag
                sqlWhere = "(IsDelete = 0   AND Status = 'A' )";
            }
            else if (status == OrderStatus.Wait)
            {
                //comprowflag มาจาก b2bcompany.rowflag
                sqlWhere = "(IsDelete = 0   AND Status = 'W' )";
            }
            else if (status == OrderStatus.Cancel)
            {
                //comprowflag มาจาก b2bcompany.rowflag
                sqlWhere = "(IsDelete = 0   AND Status = 'C' )";
            }
            else if (status == OrderStatus.Reject)
            {
                //comprowflag มาจาก b2bcompany.rowflag
                sqlWhere = "(IsDelete = 0   AND Status = 'R' )";
            }


            if (CompID > 0)
                sqlWhere += "AND (CompID = " + CompID + ")";

            return sqlWhere;
        }

        #endregion

        #region CheckMemberPackage
        public bool CheckMemberPackage(int CompID, int CompLevel)
        {
            var IsShow = false;
            if (CompLevel == 1)
            {
                var sqlwhere = "RowFlag > 0 AND (OrderStatus = 'N' OR OrderStatus = 'W' ) AND CompID =" + CompID + " AND PackageID = 3";
                var count = CountData<view_OrderDetail>(" * ", sqlwhere);
                if (count > 0)
                {
                    IsShow = false;
                }
                else
                {
                    IsShow = true;
                }
            }
            else if (CompLevel == 3)
            {
                var svCompany = new Company.CompanyService();
                if (svCompany.ValidateExpireDate(CompID))
                {
                    IsShow = true;
                    // renew member package
                }
                else
                {
                    IsShow = false;
                }

            }
            return IsShow;
        }
        #endregion

        #region ChackHotPackage
        public bool ChackHotPackage(int CompID, int CompLevel)
        {
            var svPackage = new PackageService();
            var IsShow = false;
            var PackageOption = svPackage.SelectData<b2bPackageOption>(" PackageOptionlID  ,  OptionValue", "PackageID =" + CompLevel + " AND OptionCode = 'maxHotProduct' AND IsDelete = 0  ").First();
            MaxHot = int.Parse(PackageOption.OptionValue);
            var sqlwhere = "RowFlag > 0 AND (OrderStatus = 'N' OR OrderStatus = 'W' OR OrderStatus = 'A' ) AND CompID =" + CompID + " AND PackageID = 5";
            var count = CountData<view_OrderDetail>(" * ", sqlwhere);
            if (count >= MaxHot)
                IsShow = false;
            else
            {
                IsShow = true;
                MaxHotQty = MaxHot - count;
            }
            return IsShow;
        }
        #endregion

        #region ChackFeatPackage
        public bool ChackFeatPackage(int CompID, int CompLevel)
        {
            var svPackage = new PackageService();
            var IsShow = false;
            var PackageOption = svPackage.SelectData<b2bPackageOption>("PackageOptionlID  ,  OptionValue", "PackageID =" + CompLevel + "AND OptionCode = 'maxFeatProduct' AND IsDelete = 0  ").First();
            MaxFeat = int.Parse(PackageOption.OptionValue);

            var sqlwhere = "RowFlag > 0 AND (OrderStatus = 'N' OR OrderStatus = 'W' OR OrderStatus = 'A' ) AND CompID =" + CompID + " AND PackageID = 4";
            var count = CountData<view_OrderDetail>(" * ", sqlwhere);
            if (count >= MaxFeat)
                IsShow = false;
            else
            {
                IsShow = true;
                MaxFeatQty = MaxFeat - count;
            }
            return IsShow;
        }
        #endregion

        #region InsertOrder
        public bool InsertOrder(b2bOrder Order, List<b2bOrderDetail> OrderDetails)
        {
            var count = CountData<b2bOrder>(" * ", " CreatedDate = GetDate() AND IsDelete = 0  ");
            count = count + 1;
            Order.OrderCode = AutoGenCode("OD", count);
            Order.CreatedDate = DateTimeNow;
            Order.ModifiedDate = DateTimeNow;
            Order.CreatedBy = "sa";
            Order.ModifiedBy = "sa";

            using (var trans = new TransactionScope())
            {
                qDB.b2bOrders.InsertOnSubmit(Order); // insert order
                qDB.SubmitChanges();

                foreach (var it in OrderDetails)
                {
                    it.OrderID = Order.OrderID;
                    it.CreatedDate = DateTimeNow;
                    it.ModifiedDate = DateTimeNow;
                    it.CreatedBy = "sa";
                    it.ModifiedBy = "sa";
                    qDB.b2bOrderDetails.InsertOnSubmit(it);
                    qDB.SubmitChanges();
                }
                trans.Complete();
                IsResult = true;
            }
            return IsResult;
        }
        #endregion

        #region SubmitOrder
        public bool SubmitOrder(int OrderID)
        {
            using (var trans = new TransactionScope())
            {
                UpdateByCondition<b2bOrderDetail>("IsDelete = 0", "OrderID = " + OrderID);
                UpdateByCondition<b2bOrder>("IsDelete = 0,OrderStatus = 'N'", "OrderID = " + OrderID);
                trans.Complete();
                IsResult = true;
            }
            return IsResult;
        }
        #endregion

        #region CancelOrder
        public bool CancelOrder(int OrderID)
        {
            using (var trans = new TransactionScope())
            {
                UpdateByCondition<b2bOrderDetail>("IsDelete = 1", "OrderID = " + OrderID);
                UpdateByCondition<b2bOrder>("IsDelete = 1,OrderStatus = 'C'", "OrderID = " + OrderID);
                trans.Complete();
                IsResult = true;
            }
            return IsResult;
        }
        #endregion

        #region UpdateOrder
        public bool UpdateOrder(int OrderID, decimal OrderTotalPrice)
        {
            using (var trans = new TransactionScope())
            {
                UpdateByCondition<b2bOrder>("TotalPrice = " + OrderTotalPrice, "OrderID = " + OrderID);
                trans.Complete();
                IsResult = true;
            }
            return IsResult;
        }
        #endregion

        #region DeleteOrder
        public bool DeleteOrder(int OrderID)
        {
            using (var trans = new TransactionScope())
            {
                UpdateByCondition<b2bOrder>(" IsDelete = 1 , RowFlag = 0 ", " OrderID = " + OrderID);
                UpdateByCondition<b2bOrderDetail>(" IsDelete = 1 , RowFlag = 0 ", " OrderID = " + OrderID);
                trans.Complete();
                IsResult = true;
            }
            return IsResult;
        }
        #endregion

        #region DeleteOrderDetail
        public bool DeleteOrderDetail(int OrderDetailID, int OrderID)
        {
            using (var trans = new TransactionScope())
            {
                UpdateByCondition<b2bOrderDetail>(" IsDelete = 1 , RowFlag = 0 ", " OrderDetailID = " + OrderDetailID);
                trans.Complete();
                IsResult = true;
            }
            return IsResult;
        }
        #endregion

        #region InsertPayment
        public bool InsertPayment(b2bMemberPaid MemberPaid)
        {
            var count = CountData<b2bMemberPaid>(" * ", " CreatedDate = GetDate() ANDIsDelete = 0  ");
            count = count + 1;
            MemberPaid.MemberPaidCode = AutoGenCode("MP", count);
            MemberPaid.CreatedDate = DateTimeNow;
            MemberPaid.ModifiedDate = DateTimeNow;
            MemberPaid.CreatedBy = "sa";
            MemberPaid.ModifiedBy = "sa";

            using (var trans = new TransactionScope())
            {
                qDB.b2bMemberPaids.InsertOnSubmit(MemberPaid);
                qDB.SubmitChanges();
                trans.Complete();
                IsResult = true;
            }
            return IsResult;
        }


        #endregion

        #region CancelPayment
        public bool CancelPayment(int MemberPaidID)
        {
            using (var trans = new TransactionScope())
            {
                UpdateByCondition<b2bMemberPaid>("IsDelete = 1", "MemberPaidID = " + MemberPaidID);
                trans.Complete();
                IsResult = true;
            }
            return IsResult;
        }
        #endregion

        #region BackPayment
        public bool BackPayment(int MemberPaidID)
        {
            using (var trans = new TransactionScope())
            {
                UpdateByCondition<b2bMemberPaid>("IsDelete = 1", "MemberPaidID = " + MemberPaidID);
                trans.Complete();
                IsResult = true;
            }
            return IsResult;
        }
        #endregion

        #region UpdatePayment

        public bool UpdatePayment(b2bMemberPaid MemberPaid)
        {
            using (var trans = new TransactionScope())
            {
                UpdateByCondition<b2bMemberPaid>(@"BankID = '" + MemberPaid.BankID + "', "
                    + "BranchName = '" + MemberPaid.BranchName
                    + "',PayerAccName = '" + MemberPaid.PayerAccName
                    + "',PayerAccNo = '" + MemberPaid.PayerAccNo
                    + "',PaymentAccID = '" + MemberPaid.PaymentAccID
                    + "',PayAmount = '" + MemberPaid.PayAmount
                    + "',PaymentDate = '"
                    + MemberPaid.PaymentDate.Value.ToString("yyyy-MM-dd")
                    + "',PaymentTime = '" + MemberPaid.PaymentTime
                    + "',SlipImgPath = '" + MemberPaid.SlipImgPath
                    + "',PaymentStatus = '" + MemberPaid.PaymentStatus
                    + "'", "MemberPaidID = " + MemberPaid.MemberPaidID);
                trans.Complete();
                IsResult = true;
            }
            return IsResult;
        }
        public bool UpdatePayment(int OrderID, int MemberPaidID, string IsInvoice, string PayerName, string PayerAddrLine1, int PayerDistrictID, int PayerProvinceID, string PayerPostalCode, string PayerPhone, string PayerMobile, string PayerFax, string PayerEmail, string RejectComment, string BillRecieverName, string BillAddrLine1, int BillDistrictID, int BillProvinceID, string BillPostalCode, string BillPhone, string BillMobile, string BillFax, string BillEmail)
        {
            using (var trans = new TransactionScope())
            {
                UpdateByCondition<b2bMemberPaid>("IsInvoice = '" + IsInvoice + "' ,PayerName = '" + PayerName + "' ,PayerAddrLine1 = '" + PayerAddrLine1 + "' ,PayerDistrictID = '" + PayerDistrictID + "' ,PayerProvinceID = '" + PayerProvinceID + "' ,PayerPostalCode = '" + PayerPostalCode + "' ,PayerPhone = '" + PayerPhone + "' ,PayerMobile = '" + PayerMobile + "' ,PayerFax = '" + PayerFax + "' ,PayerEmail = '" + PayerEmail + "' ,RejectComment = '" + RejectComment + "' ,BillRecieverName = '" + BillRecieverName + "' ,BillAddrLine1 = '" + BillAddrLine1 + "' ,BillDistrictID = '" + BillDistrictID + "' ,BillProvinceID = '" + BillProvinceID + "' ,BillPostalCode = '" + BillPostalCode + "' ,BillPhone = '" + BillPhone + "' ,BillMobile = '" + BillMobile + "' ,BillFax = '" + BillFax + "' ,BillEmail = '" + BillEmail + "'", "MemberPaidID = " + MemberPaidID);
                UpdateByCondition<b2bOrder>(" OrderStatus = 'W' , MemberPaidID = '" + MemberPaidID + "'", "OrderID = " + OrderID);
                trans.Complete();
                IsResult = true;
            }
            return IsResult;
        }
        #endregion

        #region DeletePayment
        public bool DeletePayment(int OrderID, int MemberPaidID)
        {
            using (var trans = new TransactionScope())
            {
                UpdateByCondition<b2bMemberPaid>(" IsDelete = 1", " MemberPaidID = " + MemberPaidID);
                UpdateByCondition<b2bOrder>(" OrderStatus = 'N' , MemberPaidID = NULL", "OrderID = " + OrderID);
                trans.Complete();
                IsResult = true;
            }
            return IsResult;
        }

        public bool DeletePaymentID(int MemberPaidID)
        {
            using (var trans = new TransactionScope())
            {
                UpdateByCondition<b2bMemberPaid>(" IsDelete = 1", " MemberPaidID = " + MemberPaidID);
                trans.Complete();
                IsResult = true;
            }
            return IsResult;
        }
        #endregion

        #region ApprovePayment 
        public bool ApprovePayment(List<int> MemberPaidID, string CompCode)
        { 
            using (var trans = new TransactionScope())
            {
                var contain = SQLWhereListInt(MemberPaidID, "MemberPaidID");
                UpdateByCondition<b2bMemberPaid>("PaymentStatus = 'A' , ModifiedBy = '" + CompCode + "'", contain);
                UpdateByCondition<b2bOrder>("OrderStatus = 'A'", contain);

                #region Update Package
                var Details = SelectData<view_OrderDetail>(" * ", " IsDelete  = 0 AND ( OrderID IN (select OrderID from b2bOrder where "+contain+" ) ) ");
                foreach (var it in Details)
                {
                    if (it.PackageID == 3)
                    {
                        #region Upgrade Gold Package
                        var svCompany = new CompanyService();
                        UpdateByCondition<b2bCompany>("ExpireDate = DATEADD(year, 1, GETDATE()) , CompLevel = 3 , ModifiedBy = '" + CompCode + "' ", " CompID = " + it.CompID);
                        #endregion
                    }
                    else if (it.PackageID == 4)
                    {
                        #region Featured Package
                        #endregion
                    }
                    else if (it.PackageID == 5)
                    {
                        #region Hot Package
                        #endregion
                    }
                }
                #endregion

                trans.Complete();
            }
            return IsResult;
        }
        #endregion

        #region RejectPayment
        public bool RejectPayment(List<int> MemberPaidID, string Remark, string CompCode)
        {

            using (var trans = new TransactionScope())
            {
                UpdateByCondition<b2bMemberPaid>("PaymentStatus = 'R' , Remark = N'" + Remark + "' , ModifiedBy = N'" + CompCode + "' ",
                    SQLWhereListInt(MemberPaidID, "MemberPaidID"));
                trans.Complete();
            }
            return IsResult;
        }
        #endregion
        
        #region CreateWhereCauseMemberPaid
         public string CreateWhereMemberPaidSearchType(string TextSearch, string SearchType)
        {

             TextSearch = TextSearch.Trim();
             if (!string.IsNullOrEmpty(TextSearch))
             {
                 if (SearchType == "CompName")
                 {
                     SQLWhere += " AND CompName LIKE N'%" + TextSearch + "%' ";
                 }
                 else if (SearchType == "MemberPaidCode")
                 {
                     SQLWhere += " AND MemberPaidCode LIKE N'%" + TextSearch + "%' ";
                 }
                 else if (SearchType == "CompCode")
                 {
                     SQLWhere += " AND CompCode LIKE N'%" + TextSearch + "%' ";
                 }
             }
             return SQLWhere;
        } 

        public string CreateWhereCauseMemberPaid(string TextSearch = "", string SearchType="", string PStatus = "N")
        {
            #region DoWhereCause
            SQLWhere += CreateWhereMemberPaidSearchType(TextSearch, SearchType);

            #region Filter : PStatus
            if (!string.IsNullOrEmpty(PStatus))
            {
                if (PStatus == "N")
                {
                    SQLWhere += " AND PaymentStatus = N'" + PStatus + "' ";
                }
                else if (PStatus == "A")
                {
                    SQLWhere += " AND PaymentStatus = N'" + PStatus + "' ";
                }
                else if (PStatus == "R")
                {
                    SQLWhere += " AND PaymentStatus = N'" + PStatus + "' ";
                }
            }
            #endregion

            #endregion

            return SQLWhere;
        }
        #endregion

        public string CreateWhereSearchBy(string txtSearch = "", string SearchType = "CompName")
        {
            txtSearch = txtSearch.Trim();
            if (!string.IsNullOrEmpty(txtSearch))
            {
                if (SearchType == "CompName")
                {
                    SQLWhere += " AND CompName LIKE N'%" + txtSearch + "%' ";
                }
                else if (SearchType == "OrderCode")
                {
                    SQLWhere += " AND OrderCode LIKE N'%" + txtSearch + "%' ";
                }
            }
            return SQLWhere;

        }

        public string CreateWhereCause(string txtSearch = "", string PStatus = "", string PSType = "")
        {
            #region DoWhereCause
            if (!string.IsNullOrEmpty(txtSearch))
                SQLWhere += " AND CompName LIKE N'" + txtSearch + "%' ";

            if (!string.IsNullOrEmpty(PStatus))
                if (PStatus == "B")
                {
                    SQLWhere += " AND OrderStatus LIKE N'" + txtSearch + "%' ";
                }
                else if (PStatus == "A")
                {
                    SQLWhere += " AND OrderStatus LIKE N'" + txtSearch + "%' ";
                }
                else if (PStatus == "R")
                {
                    SQLWhere += " AND OrderStatus LIKE N'" + txtSearch + "%' ";
                }

            if (!string.IsNullOrEmpty(PSType))
                if (PSType == "New")
                {
                    SQLWhere += " AND (OrderType = 1) ";
                }
                else if (PSType == "ReNew")
                {
                    SQLWhere += " AND (OrderType = 2) ";
                }
            #endregion

            return SQLWhere;
        }

        #region ApproveOrderPackage
        public bool ApproveOrderPackage(List<int> OrderID, string CompCode)
        {
            using (var trans = new TransactionScope())
            {
                for (var i = 0; i < OrderID.Count(); i++)
                {
                    UpdateByCondition<b2bOrder>("OrderStatus = N'A' , ApproveDate = N'" + DateTimeNow + "' , modifiedby = N'" + CompCode + "'", "OrderID = " + OrderID[i]);

                    var Details = SelectData<view_OrderDetail>(" * ", " IsDelete  = 0 AND OrderID = " + OrderID[i]);
                    for (var j = 0; j < Details.Count(); j++)
                    {
                        DateTime date = new DateTime();
                        if (Details[j].ODDuration > 0)
                        {
                            date = DateTime.Now.AddDays((int)Details[j].ODDuration);
                        }
                        else
                        {
                            TimeSpan time = new TimeSpan(2020, 12, 31);
                            date.Add(time);
                        }
                        UpdateByCondition<b2bOrderDetail>("StartDate = N'" + DateTimeNow + "' , ExpiredDate = N'" + date + "' , modifiedby = N'" + CompCode + "'", "OrderDetailID = " + Details[j].OrderDetailID);

                        if (Details[j].PackageID >= 26 && Details[j].PackageID <= 28)
                        {
                            var HotFeat = SelectData<b2bHotFeaProduct>(" * ", "OrderDetailID = " + Details[j].OrderDetailID);
                            if (HotFeat.Count() > 0)
                            {
                                UpdateByCondition<b2bHotFeaProduct>("IsDelete  = 0", "OrderDetailID = " + Details[j].OrderDetailID);
                            }
                            else
                            {
                                var data = new b2bHotFeaProduct();
                                data.OrderDetailID = Details[j].OrderDetailID;
                                data.CompID = Details[j].CompID;
                                data.ActivatedDate = DateTimeNow;
                                data.ExpiredDate = date;
                                if (Details[j].PackageID == 26)
                                {
                                    data.Status = "F";
                                }
                                else if (Details[j].PackageID == 27)
                                {
                                    data.Status = "H";
                                }
                                else if (Details[j].PackageID == 28)
                                {
                                    data.Status = "P";
                                }
                                data.HotPrice = Convert.ToDecimal(Details[j].PackagePrice);
                                data.IsShow = true;
                                data.IsDelete = false;
                                data.RowFlag = 3;
                                data.ModifiedBy = CompCode;
                                data.CreatedBy = CompCode;
                                data.PackageCount = 1;
                                data.CreatedDate = DateTimeNow;
                                data.ModifiedDate = DateTimeNow;
                                data.RowVersion = 1;
                                qDB.b2bHotFeaProducts.InsertOnSubmit(data);
                                qDB.SubmitChanges();
                            }
                        }
                    }
                }
                trans.Complete();
                return IsResult;
            }
        }
        #endregion

        #region RejectOrder
        public bool RejectOrder(List<int> OrderID, string Remark, string CompCode)
        {
            for (var i = 0; i < OrderID.Count(); i++)
            {
                UpdateByCondition<b2bOrder>("OrderStatus = N'R', RejectDate = N'" + DateTimeNow + "' , RejectComment = N'" + Remark + "', modifiedby = N'" + CompCode + "'", "OrderID = " + OrderID[i]);
                var Details = SelectData<b2bOrderDetail>(" * ", " IsDelete  = 0 AND PackageID IN(26,27,28) AND OrderID = " + OrderID[i]);
                for (var j = 0; j < Details.Count(); j++)
                {
                    UpdateByCondition<b2bHotFeaProduct>("IsDelete = 1", "OrderDetailID = " + Details[i].OrderDetailID);
                }
            }
            return IsResult;
        }
        #endregion

        #region Delete
        public bool Delete(List<int> OrderID)
        {
            for (var i = 0; i < OrderID.Count(); i++)
            {
                var Details = SelectData<b2bOrderDetail>(" * ", " IsDelete  = 0 AND OrderID = " + OrderID[i]);
                for (var j = 0; j < Details.Count(); j++)
                {
                    UpdateByCondition<b2bHotFeaProduct>(" IsDelete = 1", "OrderDetailID = " + Details[i].OrderDetailID);
                }

                UpdateByCondition<b2bOrderDetail>(" IsDelete = 1", "OrderID = " + OrderID[i]);
                UpdateByCondition<b2bOrder>(" IsDelete = 1", "OrderID = " + OrderID[i]);

            }
            return IsResult;
        }
        #endregion

    }
}