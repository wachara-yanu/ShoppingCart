using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Prosoft.Base;
using Prosoft.Service;
//using System.Web.Mvc;
using System.Transactions;
using System.Reflection;
using System.Data.Linq;
using System.Runtime.Serialization;
using System.IO;
using System.ComponentModel;
   

namespace Ouikum
{

    public class BaseSC : BaseService
    {
        public int CodeError { get; set; }
        #region Members
        public OuikumDataContext qDB;
        #endregion

        #region Contructor
        public BaseSC()
        {
            qDB = new OuikumDataContext(ConnectionString);
        }

        public BaseSC(OuikumDataContext context)
        {
            qDB = context;
        }
        #endregion

        #region SelectData : Update by Bom 14/11/55

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

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">Type (ตาราง) เช่น Member</typeparam>
        /// <param name="sqlSelect"></param>
        /// <param name="sqlWhere"></param>
        /// <param name="sqlOrderBy"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <param name="sqlGroupby"></param>
        /// <param name="IsOrderBy">If No need Order by</param>
        /// <returns></returns>
        public List<T> SelectDataProduct<T>(string sqlSelect, string sqlWhere = "IsDelete = 0", string sqlOrderBy = "CreatedDate DESC", int PageIndex = 1, int PageSize = 0, bool IsOrderBy = true)
        {
            try
            {
                Type t = typeof(T);
                var lstModel = new List<T>();
                string strQuery = "";

                #region Default Where Cause
                strQuery = "SELECT top " + PageSize + sqlSelect + " FROM " + t.Name + " AS it WHERE " + sqlWhere;
                lstModel = qDB.ExecuteQuery<T>(strQuery).ToList();
                TotalRow = lstModel.Count();
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

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">Type (ตาราง) เช่น Member</typeparam>
        /// <param name="sqlSelect"></param>
        /// <param name="sqlWhere"></param>
     
        /// <param name="sqlGroupby"></param>

        /// <returns></returns>
        public List<T> SelectDataEmailtoSeller<T>(string sqlSelect, string sqlWhere = "IsDelete = 0",string sqlGroupby = " Group by emailseller,OrderID,compidseller")
        {
            try
            {
                Type t = typeof(T);
                var lstModel = new List<T>();
                string strQuery = "";

                #region Default Where Cause
                strQuery = "SELECT  " + sqlSelect + " FROM " + t.Name + " AS it WHERE " + sqlWhere + sqlGroupby;
                lstModel = qDB.ExecuteQuery<T>(strQuery).ToList();
                TotalRow = lstModel.Count();
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
        public List<T> SelectDataCachingRecentProduct<T>(string sqlSelect)
        {
            try
            {
                Type t = typeof(T);
                var lstModel = new List<T>();

                #region Default Where Cause
                lstModel = qDB.ExecuteQuery<T>(sqlSelect).ToList();
                TotalRow = lstModel.Count();
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

        #endregion

        #region CountTempCart
        public List<T> SelectCountTemp<T>(string sqlSelect, string sqlWhere = "IsDelete = 0")
        {
            Type t = typeof(T);
            var lstModel = new List<T>();
            string strQuery = "";
            try 
	        {
                strQuery = "SELECT " + sqlSelect + " FROM " + t.Name + " AS it WHERE " + sqlWhere;
                lstModel = qDB.ExecuteQuery<T>(strQuery).ToList();
                TotalRow = lstModel.Count();

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

        #endregion

        #region SelectHotProduct : by Katak 14/08/57

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
        public List<T> SelectHotProduct<T>(string sqlSelect, string sqlWhere = "IsDelete = 0", string sqlOrderBy = "CreatedDate DESC", int PageIndex = 1, int PageSize = 0, bool IsOrderBy = true)
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
                    if(sqlOrderBy == "NEWID(),HotPrice DESC"){
                        strQuery = @"SELECT * FROM "
                            + "(SELECT row_number() OVER (ORDER BY " + sqlOrderBy + ") AS RowNumber," + sqlSelect + " FROM " + t.Name + " AS it WHERE " + sqlWhere + ")"

                            + " AS Paging WHERE (Paging.RowNumber BETWEEN " + start + " AND " + PageSize + ") order by HotPrice desc";
                    }else{
                        strQuery = @"SELECT * FROM "
                            + "(SELECT row_number() OVER (ORDER BY " + sqlOrderBy + ") AS RowNumber," + sqlSelect + " FROM " + t.Name + " AS it WHERE " + sqlWhere + ")"

                            + " AS Paging WHERE (Paging.RowNumber BETWEEN " + start + " AND " + PageSize + ") order by Price desc";
                    }

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
        #endregion

        #region GetByID : Update by Bom 20/5/56

        #region GetByID : string
        public T GetByID<T>(string PrimaryKey, string ID) where T : class
        {

            try
            {
                var type = typeof(T);
                var instance = (T)Activator.CreateInstance(type);

                #region Validate
                if (string.IsNullOrEmpty(PrimaryKey))
                {
                    IsResult = false;
                    MsgError.Add(new Exception("Not PrimaryKey")); 
                    return instance;
                }
                if (string.IsNullOrEmpty(ID))
                {
                    IsResult = false;
                    MsgError.Add(new Exception("Not ID")); 
                    return instance;
                }

                #endregion

                string strQuery = "";
                #region Query
                ID = ID.Replace("'", "").Trim();
                strQuery = "select * from " + type.Name + " Where " + PrimaryKey + " = N'" + ID + "'";
                var model = qDB.ExecuteQuery<T>(strQuery).ToList();
                if (model != null && model.Count > 0)
                {
                    TotalRow = model.Count;
                    return model.First();
                }
                else
                {
                    TotalRow = 0;
                    return instance;
                }

                #endregion

            }
            catch (Exception ex)
            {
                MsgError.Add(ex); 
                return null;
            }

        }
        #endregion

        #region GetByID : int

        public T GetByID<T>(string PrimaryKey, int ID) where T : class
        {
            try
            {
                #region Validate
                if (string.IsNullOrEmpty(PrimaryKey))
                {
                    IsResult = false; 
                    return null;
                }
                if (ID < 0)
                {
                    IsResult = false; 
                    return null;
                }

                #endregion

                string strQuery = "";
                var type = typeof(T);

                #region Query
                strQuery = "select * from " + type.Name + " Where " + PrimaryKey + " = " + ID + "";
                var model = qDB.ExecuteQuery<T>(strQuery).ToList();
                if (model != null && model.Count > 0)
                {
                    TotalRow = model.Count;
                    return model.First();
                }
                else
                {
                    TotalRow = 0;
                    return null;
                }

                #endregion
            }
            catch (Exception ex)
            {
                IsResult = false;
                MsgError.Add(ex); 
                return null;
            }

        }
        #endregion

        #endregion


        #region GetByID : Update by Bom 16/10/56

        #region GetByID : string
        public T GetByID<T>(string SqlSelect, string PrimaryKey, string ID) where T : class
        {

            try
            {
                var type = typeof(T);
                var instance = (T)Activator.CreateInstance(type);

                #region Validate
                if (string.IsNullOrEmpty(PrimaryKey))
                {
                    IsResult = false;
                    MsgError.Add(new Exception("Not PrimaryKey"));
                    // ExceptionHelper.AddException(ExceptionLayer.DA, ExceptionSeverity.Error, "Not PrimaryKey");
                    return instance;
                }
                if (string.IsNullOrEmpty(ID))
                {
                    IsResult = false;
                    MsgError.Add(new Exception("Not ID"));
                    // ExceptionHelper.AddException(ExceptionLayer.DA, ExceptionSeverity.Error, "Not ID");
                    return instance;
                }

                #endregion

                string strQuery = "";
                #region Query
                ID = ID.Replace("'", "").Trim();
                if (string.IsNullOrEmpty(SqlSelect))
                {
                    SqlSelect = " * ";
                }
                strQuery = "select " + SqlSelect + " from " + type.Name + " Where " + PrimaryKey + " = N'" + ID + "' ";
                var model = qDB.ExecuteQuery<T>(strQuery).ToList();
                if (model != null && model.Count > 0)
                {
                    TotalRow = model.Count;
                    return model.First();
                }
                else
                {
                    TotalRow = 0;
                    return instance;
                }

                #endregion

            }
            catch (Exception ex)
            {
                MsgError.Add(ex); 
                return null;
            }

        }
        #endregion

        #region GetByID : int

        public T GetByID<T>(string SqlSelect, string PrimaryKey, int ID) where T : class
        {
            try
            {
                #region Validate
                if (string.IsNullOrEmpty(PrimaryKey))
                {
                    IsResult = false;
                    MsgError.Add(new Exception("Not PrimaryKey")); 
                    return null;
                }
                if (ID < 0)
                {
                    IsResult = false;
                    MsgError.Add(new Exception("Dont'have ID"));  
                    return null;
                }

                #endregion

                string strQuery = "";
                var type = typeof(T);

                #region Query
                if (string.IsNullOrEmpty(SqlSelect))
                {
                    SqlSelect = " * ";
                }
                strQuery = "select " + SqlSelect + " from " + type.Name + " Where " + PrimaryKey + " = " + ID + "";
                var model = qDB.ExecuteQuery<T>(strQuery).ToList();
                if (model != null && model.Count > 0)
                {
                    TotalRow = model.Count;
                    return model.First();
                }
                else
                {
                    TotalRow = 0;
                    return null;
                }

                #endregion
            }
            catch (Exception ex)
            {
                IsResult = false;
                MsgError.Add(ex); 
                return null;
            }

        }
        #endregion

        #endregion
        #region CountData : Create by Golf 17/10/55
        public int CountData<T>(string sqlSelect, string sqlWhere)
        {
            try
            {
                Type t = typeof(T);

                #region Default Where Cause
                if (string.IsNullOrEmpty(sqlWhere))
                    sqlWhere = " RowFlag > 0";
                #endregion

                #region Select Count + Total Page
                string strQuery = "SELECT COUNT(*) as Count FROM " + t.Name + " as it WHERE " + sqlWhere;
                TotalRow = qDB.ExecuteQuery<int>(strQuery).ToList()[0];
                #endregion

                return TotalRow;
            }
            catch (Exception ex)
            {
                MsgError.Add(ex);
                return -1;
            }
        }
        #endregion

        #region SaveData
        #region SaveData Model : Update by Golf 17/10/55
        public T SaveData<T>(T Model, string PrimaryKeyName) where T : class
        {
            #region Members
            IsResult = true;
            Type serviceType = this.GetType();
            #endregion

            #region Validation
            MethodInfo validateMethod = serviceType.GetMethod(Model.GetType().Name + "Validate");
            if (validateMethod != null)
            {
                if (false == (bool)validateMethod.Invoke(this, new object[] { Model }))
                    return Model;
            }
            #endregion

            #region DoSave
            using (var trans = new TransactionScope())
            {
                try
                {
                    var table = qDB.GetTable<T>();

                    int result = 0;
                    Int32.TryParse(Model.GetType().GetProperty(PrimaryKeyName).GetValue(Model, null).ToString(), out result);
                    if (result > 0)
                        qDB.Refresh(RefreshMode.KeepCurrentValues, Model);
                    else
                        table.InsertOnSubmit(Model);

                    qDB.SubmitChanges();
                    trans.Complete();
                }
                catch (Exception ex)
                {
                    IsResult = false;
                    MsgError.Add(ex);
                }
            }
            #endregion

            return Model;
        }
        #endregion

        #region SaveData Model List : Update by Golf 17/10/55
        public List<T> SaveData<T>(List<T> lstModel, string PrimaryKeyName) where T : class
        {
            #region Members
            IsResult = true;
            Type serviceType = this.GetType();
            #endregion

            #region Validation
            foreach (var Model in lstModel)
            {
                Type modelType = Model.GetType();
                MethodInfo validateMethod = serviceType.GetMethod(modelType.Name + "Validate");

                if (validateMethod != null)
                {
                    if (false == (bool)validateMethod.Invoke(this, new object[] { Model }))
                        return lstModel;
                }
            }
            #endregion

            #region DoSave
            using (var trans = new TransactionScope())
            {
                try
                {
                    var table = qDB.GetTable<T>();
                    foreach (var Model in lstModel)
                    {
                        if ((int)Model.GetType().GetProperty(PrimaryKeyName).GetValue(Model, null) > 0)
                            qDB.Refresh(RefreshMode.KeepCurrentValues, Model);
                        else
                        {
                            table.InsertOnSubmit(Model);
                        }
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
            #endregion
            return lstModel;
        }
        #endregion

        #endregion

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
            catch(Exception ex)
            {
                MsgError.Add(ex);
                return IsResult = false;
            }
        }
        #endregion

        #region DeleteData
        #region DeleteData Model : Update by Golf 17/10/55
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Model"></param>
        /// <param name="PrimaryKeyName"></param>
        /// <param name="sqlWhere"></param>
        /// <returns></returns>
        public T DeleteData<T>(T Model, string PrimaryKeyName, string sqlWhere = "")
        {
            Type t = typeof(T);
            if (!string.IsNullOrEmpty(sqlWhere)) sqlWhere = " AND " + sqlWhere;

            bool IsDelete = DataManager.ConvertToBool(Model.GetType().GetProperty("IsDelete").GetValue(Model, null));
            IsDelete = IsDelete == false ? true : IsDelete;

            string query = "UPDATE " + t.Name + " SET IsDelete = " + IsDelete + " WHERE RowVersion = ";
            query += Model.GetType().GetProperty("RowVersion").GetValue(Model, null) + " AND ";
            query += PrimaryKeyName + "=" + Model.GetType().GetProperty(PrimaryKeyName).GetValue(Model, null) + sqlWhere;

            //sqlWhere;
            try
            {
                qDB.ExecuteCommand(query);
                IsResult = true;
                return Model;
            }
            catch (Exception ex)
            {
                MsgError.Add(ex);
                IsResult = false;
                return Model;
            }
        }
        #endregion

        #region DeleteData Model List : Update by Golf 17/10/55
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lstModel"></param>
        /// <param name="PrimaryKeyName"></param>
        /// <param name="sqlWhere"></param>
        /// <returns></returns>
        public List<T> DeleteData<T>(List<T> lstModel, string PrimaryKeyName, string sqlWhere = "")
        {
            try
            {
                Type t = typeof(T);
                string query = "UPDATE " + t.Name + " SET IsDelete = CASE ";
                if (!string.IsNullOrEmpty(sqlWhere))
                    sqlWhere += sqlWhere + " AND ";
                foreach (var item in lstModel)
                {
                    bool IsDelete = DataManager.ConvertToBool(item.GetType().GetProperty("IsDelete").GetValue(item, null));
                    IsDelete = IsDelete == false ? true : IsDelete;
                    string PrimaryKey = DataManager.ConvertToString(item.GetType().GetProperty(PrimaryKeyName).GetValue(item, null));

                    query += " WHEN " + PrimaryKeyName + "=" + PrimaryKey;
                    query += " THEN " + DataManager.ConvertToInteger(IsDelete);
                    sqlWhere += " (" + PrimaryKeyName + "=" + PrimaryKey + " AND RowVersion=" + item.GetType().GetProperty("RowVersion").GetValue(item, null) + ") OR";
                }

                query += " END WHERE " + sqlWhere.Substring(0, sqlWhere.Length - 2);

                qDB.ExecuteCommand(query);
                IsResult = true;
                return lstModel;
            }
            catch (Exception ex)
            {
                IsResult = false;
                MsgError.Add(ex);
                return lstModel;
            }
        }
        #endregion

        #region DeleteData เปลี่ยนจาก List เป็น Model ให้ : Update by Golf 17/10/55
        /// <summary>
        /// Create by Boss
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ID"></param>
        /// <param name="RowVersion"></param>
        /// <param name="PrimaryKeyName"></param>
        /// <param name="sqlWhere"></param>
        /// <param name="RowFlag"></param>
        /// <returns></returns>
        public List<T> DeleteData<T>(List<int> ID, List<short> RowVersion, string PrimaryKeyName = "", string sqlWhere = "")
        {
            var lstModel = new List<T>();
            try
            {
                for (int i = 0; i < ID.Count(); i++)
                {
                    var Model = Activator.CreateInstance<T>();
                    Model.GetType().GetProperty(PrimaryKeyName).SetValue(Model, ID[i], null);
                    Model.GetType().GetProperty("RowVersion").SetValue(Model, RowVersion[i], null);
                    Model.GetType().GetProperty("IsDelete").SetValue(Model, true, null);
                    lstModel.Add(Model);
                }

                return DeleteData<T>(lstModel, PrimaryKeyName, sqlWhere);
            }
            catch (Exception ex)
            {
                IsResult = false;
                MsgError.Add(ex);
                return lstModel;
            }
        }
        #endregion
        #endregion

        #region SaveStatus : Update By Golf 17/10/55
        public bool SaveStatus<T>(List<int> ID, List<short> RowVersion, string PrimaryKeyName, int sqlKeyValue, string sqlFieldKey = "IsDelete", string sqlWhere = "")
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
                    query += " WHEN " + PrimaryKeyName + "=" + PrimaryKey;
                    query += " THEN " + sqlKeyValue;
                    sqlWhere += " (" + PrimaryKeyName + "=" + PrimaryKey + " AND RowVersion=" + RowVersion[i] + ") OR";
                }
                query += " END WHERE " + sqlWhere.Substring(0, sqlWhere.Length - 2);
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

        #region ChangeRead : Update By Zam 22/11/55
        public bool ChangeRead<T>(List<bool> Check ,List<int> ID, List<short> RowVersion, string PrimaryKeyName , string sqlFieldKey = "IsDelete", string sqlWhere = "")
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
                    if(Check[i] == true){
                        query += " WHEN " + PrimaryKeyName + "=" + PrimaryKey;
                        query += " THEN " + DataManager.ConvertToInteger(false);
                        sqlWhere += " (" + PrimaryKeyName + "=" + PrimaryKey + " AND RowVersion=" + RowVersion[i] + ") OR";
                    }
                }
                query += " END WHERE " + sqlWhere.Substring(0, sqlWhere.Length - 2);
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

        #region DelData : Update By Zam 22/11/55
        public bool DelData<T>(List<bool> Check, List<int> ID, List<short> RowVersion, string PrimaryKeyName, string sqlFieldKey = "IsDelete", string sqlWhere = "")
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
                query += " END WHERE " + sqlWhere.Substring(0, sqlWhere.Length - 2);
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

        #region ChangeListNo : Update By Golf 17/10/55
        public bool ChangeListNo<T>(List<int> ID, List<short> RowVersion, string PrimaryKeyName, List<int> sqlKeyValue, string sqlFieldKey = "RowFlag", string sqlWhere = "")
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
                    query += " WHEN " + PrimaryKeyName + "=" + PrimaryKey;
                    query += " THEN " + ((sqlKeyValue[i] < 0) ? 1 : sqlKeyValue[i]);
                    sqlWhere += " (" + PrimaryKeyName + "=" + PrimaryKey + " AND RowVersion=" + RowVersion[i] + ") OR";
                }
                query += " END WHERE " + sqlWhere.Substring(0, sqlWhere.Length - 2);
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

        #region CalculateTotalPage
        protected int CalculateTotalPage(int TotalRow, int SizePage)
        {
            int TotalPage = 0;
            if (TotalRow > 0)
            {
                if (SizePage > 0)
                {
                    TotalPage = TotalRow / SizePage;
                    if (TotalRow % SizePage != 0) TotalPage++; //ถ้าหารไม่ลงตัวก็ให้เพิ่มมา 1 หน้า
                    TotalPage = (TotalPage == 0) ? 1 : TotalPage; //ถ้าหารลงตัวแล้วเป็นหน้าแรก
                }
                else
                    TotalPage = TotalRow;
            }
            return TotalPage;
        }
        #endregion

        #region SQLWhereList
        public string SQLWhereListInt(List<int> IDs, string FieldKey)
        {
            List<string> strIDs = new List<string>();
            foreach (int id in IDs)
                strIDs.Add(id.ToString());
            return SQLWhereListString(strIDs, FieldKey);
        }
        public string SQLWhereListInt(IEnumerable<int> IDs, string FieldKey)
        {
            List<string> strIDs = new List<string>();
            foreach (int id in IDs)
                strIDs.Add(id.ToString());
            return SQLWhereListString(strIDs, FieldKey);
        }
        public string SQLWhereListString(List<string> strIDs, string FieldKey)
        {
            string Seperated = string.Join(",", strIDs.ToArray());
            return " (" + FieldKey + " IN ( " + (!string.IsNullOrEmpty(Seperated) ? Seperated : "0") + ")) ";
        }
        #endregion       

        #region InitialWhere
        public string InitialWhere()
        {
            return " IsDelete = 0 AND ActivatedDate IS NOT NULL";
        }
        #endregion

        #region AutoGenCode
        public string AutoGenCode(string HCode, int Count)
        { 
            string FullGen = HCode;
            FullGen = FullGen + "-" + RandomCharecter(3) + "-" + DateTime.Now.ToString("yyMMdd") + "-" + (Count + 1);

            return FullGen;
        }

        private string RandomCharecter(int Size)
        {
            Random ran = new Random();
            string chars = "ABCDEFGHIJKLMNOPQESTUVWXYZ";
            char[] buffer = new char[Size];
            for (int i = 0; i < Size; i++)
            {
                buffer[i] = chars[ran.Next(chars.Length)];
            }
            return new string(buffer);
        }
        private string RandomCharInt(int Size)
        {
            Random ran = new Random();
            string chars = "0123456789";
            char[] buffer = new char[Size];
            for (int i = 0; i < Size; i++)
            {
                buffer[i] = chars[ran.Next(chars.Length)];
            }
            return new string(buffer);
        }
        #endregion                


    
     
    }
}