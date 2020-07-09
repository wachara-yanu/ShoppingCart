using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using Prosoft.Service;
using System.Data.Objects;
namespace Prosoft.Base
{
    #region Class : DbParametersHelper
    public class DbParametersHelper
    {
        #region GetTypeToSqlDbType
        /// <summary>
        /// ทำการตรวจสอบ DataType ของข้อมูล และ ส่งค่ากลับมาเป็น DB-DataType
        /// </summary>
        /// <param name="Value">ข้อมูล</param>
        /// <returns>DB-Type</returns>
        public SqlDbType GetTypeToSqlDbType(object Value)
        {
            SqlDbType sqlDbType = new SqlDbType();
            sqlDbType = SqlDbType.NVarChar;
            if (Value != null)
            {
                switch (Value.GetType().FullName)
                {
                    case "System.Int16": sqlDbType = SqlDbType.SmallInt; break;
                    case "System.Int32": sqlDbType = SqlDbType.Int; break;
                    case "System.Int64": sqlDbType = SqlDbType.BigInt; break;
                    case "System.String": sqlDbType = SqlDbType.NVarChar; break;
                    case "System.DateTime": sqlDbType = SqlDbType.DateTime; break;
                    case "System.Boolean": sqlDbType = SqlDbType.Bit; break;
                    case "System.Decimal": sqlDbType = SqlDbType.Decimal; break;
                }
            }
            return sqlDbType;
        }
        #endregion

        #region DoValidateValue
        /// <summary>
        /// ทำการตรวจสอบ ข้อมูลที่ส่งเข้ามาว่าเป็น Null หรือ เปล่าถ้าเป็น Null จะส่งค่ากลับมาเป็น DB-Null
        /// </summary>
        /// <param name="objectValue">ข้อมูล</param>
        /// <returns>object</returns>
        public object DoValidateValue(object objectValue)
        {
            //ตรวจสอบ ข้อมูล
            if (objectValue != null)
            {
                //เป็นข้อมูลประเภท DateTime หรือไม่ ?
                if (objectValue is DateTime)
                {
                    //ถ้าเป็น DateTime แล้วมีค่าเท่ากับ MinValue จะต้องเปลี่ยนค่าเป็น Null ไม่เช่นนั้นจะ Error
                    if (((DateTime)objectValue).Equals(DateTime.MinValue))
                        objectValue = System.Data.SqlTypes.SqlDateTime.Null;
                }
            }
            else
                //เป็น Null จะ Assign เป็นค่า DB-Null ของฐานข้อมูล เพื่อป้องกัน Error
                objectValue = System.Data.SqlTypes.SqlString.Null;

            return objectValue;
        }
        #endregion
    }
    #endregion

    #region Class : ParametersCollection
    public class ParametersCollection
    {
        #region Member
        private ArrayList arrParameter = new ArrayList();

        DbParametersHelper dbParametersHelper;
        #endregion

        #region Property
        public int Count
        {
            get { return arrParameter.Count; }
        }
        #endregion

        #region Constructor
        public ParametersCollection()
        {
            dbParametersHelper = new DbParametersHelper();
        }
        #endregion

        #region AddParameters
        /// <summary>
        /// เพิ่ม Parameter เข้า Collection ทั้งหมด
        /// </summary>
        /// <param name="tdsData">DataSet</param>
        /// <param name="TableName">ชื่อตาราง</param>
        public void AddParameters(DataSet tdsData, string TableName)
        {
            AddParameters(tdsData, TableName, "");
        }

        /// <summary>
        /// เพิ่ม Parameter เข้า Collection ทั้งหมดของคอลัมน์ใน DataSet
        /// </summary>
        /// <param name="tdsData">DataSet</param>
        /// <param name="TableName">ชื่อตาราง</param>
        /// <param name="NotAddColumn">คอลัมน์ที่ไม่ต้องการเพิ่ม</param>
        public void AddParameters(DataSet tdsData, string TableName, string NotAddColumn)
        {
            if (tdsData != null)
            {
                if (tdsData.Tables[TableName] != null)
                {
                    if (tdsData.Tables[TableName].Rows.Count > 0)
                    {
                        foreach (DataColumn dataColumn in tdsData.Tables[TableName].Columns)
                        {
                            //ไม่ทำการ Add Column นี้
                            if (!dataColumn.ColumnName.Equals(NotAddColumn))
                            {
                                //Generate SQL Column
                                AddParameters(dataColumn.ColumnName, dbParametersHelper.GetTypeToSqlDbType(tdsData.Tables[TableName].Rows[0][dataColumn.ColumnName]), tdsData.Tables[TableName].Rows[0][dataColumn.ColumnName], ParameterDirection.Input);
                            }
                        }
                    }
                }
            }
        }

        public void AddParameters(string Parameter, SqlDbType DataType, ParameterDirection paramDirection)
        {
            AddParameters(Parameter, DataType, null, paramDirection);
        }

        public void AddParameters(string Parameter, SqlDbType DataType, object Value)
        {
            AddParameters(Parameter, DataType, Value, ParameterDirection.Input);
        }

        /// <summary>
        /// เพิ่ม Parameter เข้า Collection
        /// </summary>
        /// <param name="Parameter">ชื่อพารามิเตอร์</param>
        /// <param name="Value">ข้อมูล</param>
        public void AddParameters(string Parameter, object Value)
        {
            AddParameters(Parameter, dbParametersHelper.GetTypeToSqlDbType(Value), Value, ParameterDirection.Input);
        }

        /// <summary>
        /// เพิ่ม Parameter เข้า Collection
        /// </summary>
        /// <param name="Parameter">ชื่อพารามิเตอร์</param>
        /// <param name="DataType">ชนิดตัวแปร</param>
        /// <param name="Value">ข้อมูล</param>
        public void AddParameters(string Parameter, SqlDbType DataType, object Value, ParameterDirection paramDirection)
        {
            SqlParameter sqlParameter = new SqlParameter();
            sqlParameter.SqlDbType = DataType;
            sqlParameter.ParameterName = Parameter;
            //ตรวจสอบ ข้อมูลที่ส่งเข้ามาก่อนเพราะ ถ้าเป็น Null ต้องทำการ Assign เป็นค่า DBNull
            sqlParameter.Direction = paramDirection;
            if (!paramDirection.Equals(ParameterDirection.Output))
                sqlParameter.Value = dbParametersHelper.DoValidateValue(Value);
            else
                sqlParameter.Value = Value;

            arrParameter.Add(sqlParameter);
        }
        #endregion

        #region GetParameter
        /// <summary>
        /// Get ข้อมูลในพารามิเตอร์ ใน Index ที่ต้องการ
        /// </summary>
        /// <param name="Index">ลำดับ</param>
        /// <returns>SqlParameter</returns>
        public SqlParameter GetParameter(int Index)
        {
            SqlParameter paramCollection = new SqlParameter();
            if (arrParameter.Count > 0 && Index < arrParameter.Count)
                paramCollection = (SqlParameter)arrParameter[Index];

            return paramCollection;
        }
        #endregion
    }
    #endregion


    #region Class : DBHelper
    public class DBHelper :Base
    {
        #region Member
        protected CommandType dbCommandType = CommandType.StoredProcedure;

        private int timeout = 2000;
        #endregion

        #region Property
        public CommandType DBCommandType
        {
            set { dbCommandType = value; }
        }
        public int Timeout
        {
            set { timeout = value; }
        }
        public string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["DB_ConnectionString"].ConnectionString;
            }
        }
        public string ConnectionStringBase
        {
            get { return ConfigurationManager.ConnectionStrings["DB_ConnectionString_Base"].ConnectionString; }
        }

        #endregion
    #endregion

        #region OpenConnection
        public void OpenConnection(BaseDTO DTO)
        {
            //กรณียังไม่มี Connection
            if (DTO.DataClient.SqlConnection == null && DTO.DataClient.SqlCommand == null)
            {
                string connectionString = ConnectionString;

                //จะต้องเขียนเงื่อนไขตรงนี้เพื่อสามารถใช้หลาย Database ได้

                if (DTO.DataClient.BaseName.Equals(BaseConnectionName.BaseCommon))
                {
                    connectionString = ConnectionStringBase;
                }
                else
                {
                    connectionString = ConnectionString;
                }



                // new Connection
                DTO.DataClient.SqlConnection = new SqlConnection(connectionString);
                if (DTO.DataClient.SqlConnection.State != ConnectionState.Open)
                    DTO.DataClient.SqlConnection.Close();
                DTO.DataClient.SqlConnection.Open();

                if (DTO.DataClient.SqlCommand == null)
                    DTO.DataClient.SqlCommand = DTO.DataClient.SqlConnection.CreateCommand();

                // begin transaction
                DTO.DataClient.SqlCommand.Connection = DTO.DataClient.SqlConnection;
                DTO.DataClient.SqlCommand.CommandType = dbCommandType;
                DTO.DataClient.SqlCommand.CommandTimeout = timeout;
            }
            DTO.DataClient.CountOpenConnection();//เมื่อมีการเรียกใช้ OpenConnection() จะทำการนับจำนวนการเปิด Connection
        }
        #endregion

        #region BeginTransaction
        public void BeginTransaction(BaseDTO DTO)
        {
            if (DTO.DataClient.SqlConnection == null)
                throw new Exception("sqlConnection is null");
            if (DTO.DataClient.SqlCommand == null)
                throw new Exception("sqlCommand is null");
            //เปิด Transaction กรณีที่เป็นการเปิด Connection ที่ตัวหลัก
            if (DTO.DataClient.SqlTransaction == null && DTO.DataClient.SqlCountConnection == 1)
            {
                DTO.DataClient.SqlTransaction = DTO.DataClient.SqlConnection.BeginTransaction();
                DTO.DataClient.SqlCommand.Transaction = DTO.DataClient.SqlTransaction;
            }
        }
        #endregion

        #region CommitTransaction
        public void CommitTransaction(BaseDTO DTO)
        {
            //Commint Transaction กรณีที่เป็นการเปิด Connection ที่ตัวหลัก
            if (DTO.DataClient.SqlTransaction != null && DTO.DataClient.SqlCountConnection == 1)
            {
                DTO.DataClient.SqlTransaction.Commit();
            }
        }
        #endregion

        #region RollbackTrasaction
        public void RollbackTrasaction(BaseDTO DTO)
        {
            if (DTO.DataClient.SqlTransaction != null)
            {
                DTO.DataClient.SqlTransaction.Rollback();
            }
        }
        #endregion

        #region CloseConnection
        public void CloseConnection(BaseDTO DTO)
        {
            DTO.DataClient.CountCloseConnection();
            if (DTO.DataClient.SqlConnection != null && DTO.DataClient.SqlCountConnection == 0)
            {
                DTO.DataClient.SqlCommand.Dispose();
                DTO.DataClient.SqlConnection.Close();
                DTO.DataClient.SqlConnection.Dispose();

                DTO.DataClient.SqlConnection = null;
                DTO.DataClient.SqlCommand = null;
            }
        }
        #endregion
    }


    #region Class : DBManager - Main Class
    public class DBManager : DBHelper
    {
        #region Member
        private BaseListDTO objListDTO = null;
        private BaseSaveDTO objSaveDTO = null;

        DbParametersHelper dbParametersHelper;
        #endregion

        #region Property
        public BaseListDTO DBManagerListDTO
        {
            get { return objListDTO; }
            set { objListDTO = value; }
        }
        public BaseSaveDTO DBManagerSaveDTO 
        {
            get { return objSaveDTO; }
            set { objSaveDTO = value; }
        }

        #endregion

        #region Constructor
        public DBManager()
        {
            dbParametersHelper = new DbParametersHelper();
        }
        #endregion

        #region Add ParametersInCommand
        /// <summary>
        /// Add ParametersInCommand
        /// </summary>
        /// <param name="sqlParameter">ชื่อพารามิเตอร์ ,ประเภทตัวแปร ,ข้อมูล</param>
        public void AddParametersInCommand(SqlParameter sqlParameter)
        {
            // Add Parameter            
            objListDTO.DataClient.SqlCommand.Parameters.Add(sqlParameter.ParameterName, sqlParameter.SqlDbType);
            objListDTO.DataClient.SqlCommand.Parameters[sqlParameter.ParameterName].Direction = sqlParameter.Direction;
            objListDTO.DataClient.SqlCommand.Parameters[sqlParameter.ParameterName].Value = sqlParameter.Value;
        }

        /// <summary>
        /// Add ParametersInCommand
        /// </summary>
        /// <param name="sqlComm">SqlCommand</param>
        /// <param name="ColumnName">คอลัมน์ของ พารามิเตอร์</param>
        /// <param name="objectValue">ข้อมูล</param>
        public void AddParametersInCommand(string ColumnName, object objectValue)
        {
            AddParametersInCommand(objListDTO.DataClient.SqlCommand, ColumnName, dbParametersHelper.GetTypeToSqlDbType(objectValue), objectValue);
        }

        /// <summary>
        /// Add ParametersInCommand
        /// </summary>
        /// <param name="sqlComm">SqlCommand</param>
        /// <param name="ColumnName">คอลัมน์ของ พารามิเตอร์</param>
        /// <param name="objectValue">ข้อมูล</param>
        public void AddParametersInCommand(SqlCommand sqlComm, string ColumnName, object objectValue)
        {
            AddParametersInCommand(sqlComm, ColumnName, dbParametersHelper.GetTypeToSqlDbType(objectValue), objectValue);
        }

        /// <summary>
        /// Add ParametersInCommand
        /// </summary>
        /// <param name="sqlComm">SqlCommand</param>
        /// <param name="ColumnName">คอลัมน์ของ พารามิเตอร์</param>
        /// <param name="sqlDbType">Type ของ พารามิเตอร์</param>
        /// <param name="objectValue">ข้อมูล</param>
        public void AddParametersInCommand(SqlCommand sqlComm, string ColumnName, SqlDbType sqlDbType, object objectValue)
        {
            // Add Parameter
            sqlComm.Parameters.Add(new SqlParameter(ColumnName, sqlDbType));
            //Assign Value
            sqlComm.Parameters[ColumnName].Value = dbParametersHelper.DoValidateValue(objectValue);
        }

        /// <summary>
        /// Add ParametersInCommand
        /// </summary>
        /// <param name="sqlComm">SqlCommand</param>
        /// <param name="sqlParameter">ชื่อพารามิเตอร์ ,ประเภทตัวแปร ,ข้อมูล</param>
        public void AddParametersInCommand(SqlCommand sqlComm, SqlParameter sqlParameter)
        {
            // Add Parameter
            sqlComm.Parameters.Add(sqlParameter);
        }
        #endregion

        #region DoSave
        protected bool DoSave(string StoreProcedure)
        {
            return DoSave(StoreProcedure, null);
        }
        protected bool DoSave(string StoreProcedure, ParametersCollection ParametersCollection)
        {
            bool isValid = true;

            if (!string.IsNullOrEmpty(StoreProcedure))
            {
                try
                {
                    int paramCount = 0;
                    if (ParametersCollection != null)
                        paramCount = ParametersCollection.Count;

                    //กำหนดการใช้ Store Procedure หรือ Query
                    objSaveDTO.DataClient.SqlCommand.CommandText = StoreProcedure;
                    objSaveDTO.DataClient.SqlCommand.CommandType = dbCommandType;

                    #region Add Parameter
                    SqlParameter sqlParameter = null;
                    objSaveDTO.DataClient.SqlCommand.Parameters.Clear();
                    for (int i = 0; i < paramCount; i++)
                    {
                        sqlParameter = ParametersCollection.GetParameter(i);
                        AddParametersInCommand(sqlParameter);
                    }
                    #endregion
                    objSaveDTO.DataClient.SqlCommand.ExecuteNonQuery();
                    objSaveDTO.Result.ActionResult = 1;
                }
                catch (SqlException sqlEx)
                {
                    isValid = false;
                    objSaveDTO.Result.ActionResult = -1;
                }
                catch (Exception Ex)
                {
                    isValid = false;
                    objSaveDTO.Result.ActionResult = -1;
                }
            }
            return isValid;
        }
        #endregion

        #region DoSaveReturnValue
        protected object DoSaveReturnValue(string StoreProcedure, ParametersCollection ParametersCollection, string Key)
        {
            object objReturn = null;
            if (!string.IsNullOrEmpty(StoreProcedure))
            {
                try
                {
                    int paramCount = 0;
                    if (ParametersCollection != null)
                        paramCount = ParametersCollection.Count;

                    //กำหนดการใช้ Store Procedure หรือ Query
                    objSaveDTO.DataClient.SqlCommand.CommandText = StoreProcedure;
                    objSaveDTO.DataClient.SqlCommand.CommandType = dbCommandType;

                    #region Add Parameter
                    SqlParameter sqlParameter = null;
                    objSaveDTO.DataClient.SqlCommand.Parameters.Clear();
                    for (int i = 0; i < paramCount; i++)
                    {
                        sqlParameter = ParametersCollection.GetParameter(i);
                        AddParametersInCommand(sqlParameter);
                    }
                    #endregion

                    objSaveDTO.DataClient.SqlCommand.ExecuteNonQuery();

                    objReturn = objSaveDTO.DataClient.SqlCommand.Parameters[Key].Value;
                    if (objReturn != null)
                        objSaveDTO.Result.ActionResult += 1;
                }
                catch (SqlException sqlEx)
                {
                    objSaveDTO.Result.ActionResult = -1;
                }
                catch (Exception Ex)
                {
                    objSaveDTO.Result.ActionResult = -1;
                }
            }
            return objReturn;
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

        #region ReturnValue
        protected int? GetInt(object obj)
        {
            int? objReturn = null;
            if (obj != null)
                if (!string.IsNullOrEmpty(obj.ToString()))
                    objReturn = Convert.ToInt32(obj);
            return objReturn;
        }
        protected short? GetShort(object obj)
        {
            short? objReturn = null;
            if (obj != null)
                if (!string.IsNullOrEmpty(obj.ToString()))
                    objReturn = Convert.ToInt16(obj);
            return objReturn;
        }
        protected byte? GetByte(object obj)
        {
            byte? objReturn = null;
            if (obj != null)
                if (!string.IsNullOrEmpty(obj.ToString()))
                    objReturn = Convert.ToByte(obj);
            return objReturn;
        }
        protected long? GetLong(object obj)
        {
            long? objReturn = null;
            if (obj != null)
                if (!string.IsNullOrEmpty(obj.ToString()))
                    objReturn = Convert.ToInt64(obj);
            return objReturn;
        }
        protected decimal? GetDecimal(object obj)
        {
            decimal? objReturn = null;
            if (obj != null)
                if (!string.IsNullOrEmpty(obj.ToString()))
                    objReturn = Convert.ToDecimal(obj);
            return objReturn;
        }
        protected float? GetFloat(object obj)
        {
            float? objReturn = null;
            if (obj != null)
                if (!string.IsNullOrEmpty(obj.ToString()))
                    objReturn = Convert.ToSingle(obj);
            return objReturn;
        }
        protected bool? GetBoolean(object obj)
        {
            bool? objReturn = null;
            if (obj != null)
                if (!string.IsNullOrEmpty(obj.ToString()))
                    objReturn = Convert.ToBoolean(obj);
            return objReturn;
        }

        protected string GetString(object obj)
        {
            string objReturn = null;
            if (obj != null)
                objReturn = Convert.ToString(obj);
            return objReturn;
        }
        protected DateTime? GetDateTime(object obj)
        {
            DateTime? objReturn = null;
            if (obj != null)
                if (!string.IsNullOrEmpty(obj.ToString()))
                    objReturn = Convert.ToDateTime(obj);
            return objReturn;
        }

        protected TimeSpan? GetTime(object obj)
        {
            TimeSpan? objReturn = null;
            if (obj != null)
                if (!string.IsNullOrEmpty(obj.ToString()))
                    objReturn = TimeSpan.Parse(obj.ToString());
            return objReturn;
        }
        #endregion

        // SQLCommand Select
        #region GetStringSQLFormDate
        protected string GetStringSQLFormDate(DateTime? datetime)
        {
            string objReturn = string.Empty;
            if (datetime != null)
            {
                objReturn = string.Concat(datetime.Value.Year.ToString(), datetime.Value.ToString("-MM-dd HH:mm:ss.000"));
            }
            return objReturn;
        }
        #endregion

        #region DoSelect
        public SqlDataReader DoSelect()
        {
            SqlDataReader objReturn = null;
                try
                {
                    objListDTO.DataClient.SqlCommand.CommandText = "up_SysSelect_ByCondition";
                    objListDTO.DataClient.SqlCommand.CommandType = CommandType.StoredProcedure;
                    objListDTO.DataClient.SqlCommand.Parameters.Clear();

                    AddParametersInCommand("SQLSelect", objListDTO.Pager.SQLSelect);
                    AddParametersInCommand("SQLFrom", objListDTO.Pager.SQLFrom);
                    AddParametersInCommand("SQLWhere", objListDTO.Pager.SQLWhere);
                    AddParametersInCommand("SQLOrder", objListDTO.Pager.SQLOrder);
                    AddParametersInCommand("SQLGroupBy", objListDTO.Pager.SQLGroupBy);
                    AddParametersInCommand("PageIndex", objListDTO.Pager.PageIndex);
                    AddParametersInCommand("PageSize", objListDTO.Pager.PageSize);

                    objReturn = objListDTO.DataClient.SqlCommand.ExecuteReader();
                }
                catch (SqlException sqlEx)
                {
                    objListDTO.Result.ActionResult = -1;
                }
                catch (Exception Ex)
                {
                    objListDTO.Result.ActionResult = -1;
                }
            return objReturn;
        }
        #endregion

    }
    #endregion
}
