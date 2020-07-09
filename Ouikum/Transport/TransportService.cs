using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Transactions;


namespace Ouikum.Transport
{
    public class TransportService : BaseSC
    {

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

        public List<T>SelectData<T>(string sqlSelect, string sqlWhere = "IsDelete = 0", string sqlOrderBy = "CreatedDate DESC", int PageIndex = 1, int PageSize = 0, bool IsOrderBy = true)
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
    }
}
