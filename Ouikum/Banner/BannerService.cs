using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Transactions;
//using Prosoft.Base;

namespace Ouikum.Banner
{
    public class BannerService : BaseSC
    {
        #region Method

        #region GetCodeError
        private void GetCodeError(Exception e)
        {
            var code = 0;
            var w32ex = e as Win32Exception;
            if (w32ex == null)
            {
                w32ex = e.InnerException as Win32Exception;
            }

            if (w32ex != null)
            {
                code = w32ex.ErrorCode;
                CodeError = code;
                // do stuff
            }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">Type (ตาราง) เช่น Member</typeparam>
        /// <param name="sqlSelect"></param>
        /// <param name="sqlWhere"></param>
        /// <param name="sqlOrderBy"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <param name="IsOrderBy">If No need Order by</param>
        /// <returns></returns>
        /// 
        public List<T> SelectData<T>(string sqlSelect, string sqlWhere = "IsDelete = 0", string sqlOrderBy = "CreatedDate DESC", int PageIndex = 1, int PageSize = 0, bool IsOrderBy = true)
        {
            try
            {
                Type t = typeof(T);
                var lstModel = new List<T>();
                string strQuery = "";
                #region Default Where Cause

                if (IsOrderBy)
                {
                    int MaxPageSize = 50;

                    PageSize = PageSize < MaxPageSize ? PageSize : MaxPageSize;
                    var start = ((PageIndex - 1) * PageSize) + 1;

                    #region Select Count + Total Page
                    strQuery = "SELECT COUNT(*) AS Count FROM " + t.Name + " AS it WHERE " + sqlWhere;
                    TotalRow = qDB.ExecuteQuery<int>(strQuery).ToList()[0];
                    TotalPage = CalculateTotalPage(TotalRow, PageSize);
                    if (sqlOrderBy == null || sqlOrderBy == "")
                    {
                        sqlOrderBy = "CreatedDate DESC";
                    }

                    if (TotalRow < 1)
                        return new List<T>();

                    if (PageSize == 0)
                        PageSize = TotalRow;
                    else
                    {
                        PageSize = (PageIndex * PageSize);
                    }
                    #endregion

                    #region Sql Query

                    strQuery = @"SELECT * FROM "
                    + "(SELECT row_number() OVER (ORDER BY " + sqlOrderBy + ") AS RowNumber," + sqlSelect + " FROM " + t.Name + " AS it WHERE " + sqlWhere + ")"

                    + " AS Paging WHERE (Paging.RowNumber BETWEEN " + start + " AND " + PageSize + ")";

                    lstModel = qDB.ExecuteQuery<T>(strQuery).ToList();
                    #endregion
                }
                else
                {
                    strQuery = "SELECT " + sqlSelect + " FROM " + t.Name + " AS it WHERE " + sqlWhere;
                    lstModel = qDB.ExecuteQuery<T>(strQuery).ToList();
                    TotalRow = lstModel.Count();
                }
                #endregion
                return lstModel;
            }
            catch (Exception ex)
            {
                IsResult = false;
                GetCodeError(ex);
                MsgError.Add(ex);
                return null;
            }
        }

        #region UpdateByCondition : Update by Golf 17/10/55
        public bool UpdateByCondition<T>(string sqlUpdate, string sqlWhere)
        {
            Type t = typeof(T);
            string query = "UPDATE " + t.Name + " SET " + sqlUpdate + " WHERE " + sqlWhere;
            try
            {
                qDB.ExecuteCommand(@query);
                return IsResult = true;
            }
            catch (Exception ex)
            {
                MsgError.Add(ex);
                return IsResult = false;
            }
        }
        #endregion

        #region DelData : Update By katak 28/08/57
        public bool DelData<T>(List<bool> Check, List<int> ID, List<short> RowVersion, string PrimaryKeyName, string sqlFieldKey = "IsDelete", string sqlListNoKey = "ListNo", string sqlWhere = "")
        {
            try
            {
                if (string.IsNullOrEmpty(PrimaryKeyName))
                    return IsResult = false;

                Type t = typeof(T);
                string query = "UPDATE " + t.Name + " SET " + sqlFieldKey + " = CASE ";
                if (!string.IsNullOrEmpty(sqlWhere))
                    sqlWhere = " " + sqlWhere + " AND ";
                for (int i = 0; i < ID.Count(); i++)
                {
                    int PrimaryKey = ID[i];
                    if (Check[i] == true)
                    {
                        query += " WHEN " + PrimaryKeyName + "=" + PrimaryKey;
                        query += " THEN " + DataManager.ConvertToInteger(true);
                        sqlWhere += " (" + PrimaryKeyName + "=" + PrimaryKey + " AND RowVersion=" + RowVersion[i] + ") OR";
                    }
                }
                query += " END ," + sqlListNoKey + " = 0  WHERE " + sqlWhere.Substring(0, sqlWhere.Length - 2);
                qDB.ExecuteCommand(query);
                return IsResult = true;
            }
            catch (Exception ex)
            {
                MsgError.Add(ex);
                return IsResult = false;
            }
        }
        #endregion


        #region UpdateBannerListNo
        public bool UpdateBannerListNo(List<int> id, List<int> no)
        {
            var str = "UPDATE b2bBanner SET ListNo = {0}  WHERE BannerID = {1}";

            using (var trans = new TransactionScope())
            {
                for (var i = 0; i < id.Count(); i++)
                {
                    qDB.ExecuteCommand(str, no[i], id[i]);

                    qDB.SubmitChanges();
                }
                trans.Complete();
                IsResult = true;
            }
            return IsResult;
        }
        #endregion

       
        #endregion
    }
}