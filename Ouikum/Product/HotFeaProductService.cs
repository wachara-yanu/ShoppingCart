using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using Prosoft.Base;
using System.Transactions;
//using System.Web.Mvc;
using System.Data.Linq;
using Ouikum.Common;
using System.Collections;

namespace Ouikum.Product
{
    public class HotFeaProductService : BaseSC
    { 
         
        #region Product

        #region Method Validate
        #region ValidateInsert
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool ValidateInsert(b2bHotFeaProduct data)
        {
            try
            {
                b2bHotFeaProduct model = qDB.b2bHotFeaProducts.Single(q => q.ProductID == data.ProductID);
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
        public bool ValidateUpdateProduct(b2bHotFeaProduct data)
        {
            try
            {
                b2bHotFeaProduct model = qDB.b2bHotFeaProducts.Single(q => q.ProductID == data.ProductID);
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

        #region GetProduct

        #region GetByID
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public b2bHotFeaProduct GetByID(int id)
        {
            return qDB.b2bHotFeaProducts.Single(m => m.ProductID == id);
        }
        #endregion

        #region ListHotFeaProductByCompID
        /// <summary>
        /// select ตาราง view_HotFeaProduct ใช้สำหรับ Front End
        /// </summary>
        /// <param name="CompID"></param>
        /// <param name="IsHot"></param>
        /// <param name="IsFeat"></param>
        /// <param name="IsOwner"></param>
        /// <param name="SQLWhere"></param>
        /// <param name="SQLOrderBy"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public IEnumerable<view_HotFeaProduct> ListHotFeaProductForFronEnd(int CompID = 0,bool IsHot = false,bool IsFeat = true,bool IsOwner = false,string SQLWhere = "",string SQLOrderBy = "",int PageIndex = 0 ,int PageSize = 10)
        {

            string sqlSelect = "CompID, ProductImgPath, CompName, ProductName , RowFlag ";
            string sqlWhere = "(ExpiredDate > GETDATE()) AND (ProductID > 0)";

            string sqlOrderby = "ModifiedDate DESC";
            #region Check Hot or Feat
            if (IsHot)
                SQLWhere += "AND (Status = 'H')";
            else if (IsFeat)
                SQLWhere += "AND (Status = 'F')";
            else
                SQLWhere += "AND (Status = 'F')";
            #endregion

            #region Check Owner
            /// check ว่า เจ้าของเว็บเข้า ดูเว็บไซต์ตัวเองหรือเปล่า
            if (IsOwner)
                SQLWhere += "AND (RowFlag > 0)";
            else
                SQLWhere += "AND (RowFlag >= 3)";
            #endregion
                 

            if (!string.IsNullOrEmpty(SQLWhere))
                sqlWhere += "AND " + SQLWhere;

            if (!string.IsNullOrEmpty(SQLOrderBy))
                sqlOrderby = SQLOrderBy;
              
            return SelectData<view_HotFeaProduct>(sqlSelect, sqlWhere, sqlOrderby, PageIndex, PageSize);
        }
        #endregion

        #endregion 

        #endregion

        #region Method Insert
        #region InsertProduct  
        #endregion
        #endregion 
        #endregion

        #region CreateWhereCauseHotFeat
        public string CreateWhereCauseHotFeat(string txtSearch = "", string TypeSearch = "ProductName", string PStatus = "H", int ExpireStatus = 0, string statusSearch = "")
        {
            #region DoWhereCause
            if (!string.IsNullOrEmpty(txtSearch))
                if (TypeSearch == "CompName")
                {
                    SQLWhere += " AND CompName LIKE N'%" + txtSearch + "%' ";
                }
                else if (TypeSearch == "ProductName")
                {
                    SQLWhere += " AND ProductName LIKE N'%" + txtSearch + "%' ";
                }
                else if (TypeSearch == "ProductID")
                {
                    SQLWhere += " AND ProductID = " + txtSearch + " ";
                }

            #region Filter : PStatus
            if (!string.IsNullOrEmpty(PStatus))
            {
                if (PStatus == "H")
                {
                    SQLWhere += " AND Status = '" + PStatus + "' ";
                }
                else if (PStatus == "F")
                {
                    SQLWhere += " AND Status = '" + PStatus + "' ";
                }
                else if (PStatus == "P")
                {
                    SQLWhere += " AND Status = '" + PStatus + "' ";
                }
                else
                {
                    SQLWhere += " AND (Status = 'H' OR Status = 'F' OR Status = 'P')";
                }
            }
            #endregion
            if (!string.IsNullOrEmpty(statusSearch))
                if (statusSearch == "Active")
                {
                    SQLWhere += " AND (convert(nvarchar(20), ExpiredDate,112) > '" + DateTime.Today.ToString("yyyyMMdd", new System.Globalization.CultureInfo("en-US")) + "')";
                }
                else if (statusSearch == "Expired")
                {
                    SQLWhere += " AND (convert(nvarchar(20), ExpiredDate,112) < '" + DateTime.Today.ToString("yyyyMMdd", new System.Globalization.CultureInfo("en-US")) + "')";
                }
                else
                {
                    SQLWhere += "";
                }

            #region Filter : ExpireStatus 
            if (ExpireStatus == 1)
            {
                SQLWhere += " AND ExpiredDate Between '" + DateTime.Now.AddDays(1).ToShortDateString() + "' AND '" + DateTime.Now.AddDays(7).ToShortDateString() + "'";
            }
            else if (ExpireStatus == 2)
            {
                SQLWhere += " AND ExpiredDate Between '" + DateTime.Now.ToShortDateString() + " 00:00:00' AND '" + DateTime.Now.ToShortDateString() + " 23:59:59'";
            }
            else if (ExpireStatus == 3)
            {
                SQLWhere += " AND ExpiredDate < '" + DateTime.Now.ToShortDateString() + "'";
            } 
            #endregion
            #endregion

            return SQLWhere;
        }
        #endregion

        #region DeleteHotFeat
        public bool DeleteHotFeat(int? HotFeaProductID)
        {
            using (var trans = new TransactionScope())
            {
                UpdateByCondition<b2bHotFeaProduct>("IsDelete = 1 , RowFlag = -1 ", " HotFeaProductID = " + HotFeaProductID);
                trans.Complete();
                IsResult = true;
            }
            return IsResult;
        }
        #endregion

        #region DeleteHotFeat
        public bool UpdateProductHotFeat(string ProductID, string HotFeaProductID)
        {
            using (var trans = new TransactionScope())
            {
                UpdateByCondition<b2bHotFeaProduct>("ProductID = " + ProductID, " HotFeaProductID = " + HotFeaProductID);
                trans.Complete();
                IsResult = true;
            }
            return IsResult;
        }
        #endregion


        #region EditExpiredDate
        public bool EditExpiredDate(int HotFeaProductID, int NumMonth)
        {
            if (NumMonth > 0 && NumMonth <= 12)
            {
                var svHotFeat = new HotFeaProductService();
                int it = svHotFeat.CountData<view_HotFeaProduct>(" * ", "HotFeaProductID = " + HotFeaProductID + " AND ExpiredDate < GetDate() ");
                if (it > 0)
                {
                    UpdateByCondition<view_HotFeaProduct>(" ExpiredDate = DATEADD(month," + NumMonth + ",getdate()) , PackageCount = PackageCount + 1 ", 
                        " HotFeaProductID = " + HotFeaProductID);
                    IsResult = true;
                }
                else
                { 
                    UpdateByCondition<view_HotFeaProduct>(" ExpiredDate = DATEADD(month," + NumMonth + ",ExpiredDate)  , PackageCount = PackageCount + 1  ", 
                        " HotFeaProductID = " + HotFeaProductID);
                    IsResult = true;
                }
            }
            else
            {
                IsResult = false;
                MsgError.Add(new Exception(" Invalid month number. "));
            }
            return IsResult;
        }
        #endregion

        #region EditStatusHotprice
        public bool EditStatusHotprice(int HotFeaProductID,string EditStatus,string EditHotprice)
        {
                var svHotFeat = new HotFeaProductService();
                int it = svHotFeat.CountData<view_HotFeaProduct>(" * ", "HotFeaProductID = " + HotFeaProductID);
                if (EditStatus != null)
                {
                    UpdateByCondition<view_HotFeaProduct>(" Status = N'" + EditStatus + "'",
                        " HotFeaProductID = " + HotFeaProductID);
                    IsResult = true;
                }
                else { IsResult = false;  }
                if (EditHotprice != null)
                {
                    UpdateByCondition<view_HotFeaProduct>(" Hotprice = " + Convert.ToDecimal(EditHotprice),
                        " HotFeaProductID = " + HotFeaProductID);
                    IsResult = true;
                }
                else{ IsResult = false; }
            return IsResult;
        }
        #endregion


        #region DeleteHotFeatAll
        public bool DeleteHotFeatAll(List<int> HotFeaProductID, string CompCode)
        {
            using (var trans = new TransactionScope())
            {
                UpdateByCondition<view_HotFeaProduct>("IsDelete = 1 , RowFlag = -1 , ModifiedBy = '" + CompCode + "'", SQLWhereListInt(HotFeaProductID, "HotFeaProductID"));
                trans.Complete();
            }
            return IsResult;
        }
        #endregion

        #region SaveHotFeat
        public string Msg { get; set; }
        public int CountSuccess { get; set; }
        public bool CheckExistProductInHotFeat(string status, int productid)
        {
            var count = CountData<b2bHotFeaProduct>(" HotFeaProductID ", InitialWhere()+"AND Status = '"+status.ToUpper()+"' AND ProductID = "+productid +" AND RowFlag = 3");
            if (count > 0)
            {
                Msg += productid + ",";
                IsResult = false;
            }else{
                IsResult = true;
            }
            return IsResult;
        }

        public bool SaveHotFeat(List<int> CompID, List<int> Expire, List<int> ProductID,List<string> Status,List<string> HotPrice,string CompCode)
        {
            using (var trans = new TransactionScope())
            {
                CountSuccess = 0;
                var i = 0;
                foreach (var it in ProductID)
                {
                    if (CheckExistProductInHotFeat(Status[i], (int)it))
                    {
                        var data = new b2bHotFeaProduct();
                        data.ProductID = (int)it;
                        data.CompID = CompID[i];
                        data.ActivatedDate = DateTimeNow;
                        data.ExpiredDate = DateTime.Now.AddMonths((int)Expire[i]);
                        data.Status = Status[i].ToString().ToUpper();
                        data.HotPrice = Convert.ToDecimal(HotPrice[i]);
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
                        CountSuccess++;
                    }
                    i++;
                }
                trans.Complete();
                IsResult = true;
            }
            return IsResult;
        }
        
        #endregion

        #region ExtendLifeTime
        public bool ExtendLifeTime(List<int> ID,int month)
        {
            var sqlWhere = " IsDelete = 0 ";
            sqlWhere += " AND "+SQLWhereListInt(ID, "HotFeaProductID");

            UpdateByCondition<b2bHotFeaProduct>(" ExpiredDate = DATEADD( month, " + month + ", ExpiredDate ) ", sqlWhere);

            return IsResult;
        }
        #endregion

        #region UpdateStatusHotFeat
        public bool UpdateStatusHotFeat(int HotFeaProductID,string Status)
        {
            using (var trans = new TransactionScope())
            {
                UpdateByCondition<b2bHotFeaProduct>("Status = '"+ Status+"'", " HotFeaProductID = " + HotFeaProductID);
                trans.Complete();
                IsResult = true;
            }
            return IsResult;
        }
        #endregion
    }
}