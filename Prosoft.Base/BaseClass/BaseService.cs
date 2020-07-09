using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Prosoft.Base
{
    public class BaseService
    {
        #region Members
        public int TotalRow { get; set; }
        public int TotalPage { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        private string _ConnectionString = string.Empty;
        private string _ConnectionStringCommon = string.Empty;
        private string _ConnectionStringCommonBase = string.Empty;
        private List<Exception> _MsgError;
        #endregion

        #region Properties
        public string SQLSelect { get; set; }
        public string SQLWhere { get; set; }
        public string SQLOrderBy { get; set; }
        public string SQLGroupBy { get; set; }
        public bool IsResult { get; set; }
        private int CodeError { get; set; }

        public DateTime DateTimeNow
        {
            get
            {
                DateTime MyTime = DateTime.Now;            //Waktu Indonesia Bagian Barat
                DateTime AsiaTimeZone = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(MyTime, "SE Asia Standard Time");
                return AsiaTimeZone;
            }
        }
        
        public List<Exception> MsgError { 
            get{
                if (_MsgError == null) 
                    _MsgError = new List<Exception>();
                return _MsgError;
            }
            set
            {
                if (value == null)
                    _MsgError = new List<Exception>();
                else
                    _MsgError = value;
            }
        }

 

        public string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(_ConnectionString))
                    _ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DB_ConnectionString"].ConnectionString;
                return _ConnectionString;
            }
        }
        public string ConnectionStringCommon
        {
            get
            {
                if (string.IsNullOrEmpty(_ConnectionStringCommon))
                    _ConnectionStringCommon = System.Configuration.ConfigurationManager.ConnectionStrings["DB_ConnectionString_Base"].ConnectionString;
                return _ConnectionStringCommon;
            }
        }
        public string ConnectionStringCommonBase
        {
            get
            {
                if (string.IsNullOrEmpty(_ConnectionStringCommonBase))
                    _ConnectionStringCommonBase = System.Configuration.ConfigurationManager.ConnectionStrings["DB_ConnectionString_Common"].ConnectionString;
                return _ConnectionStringCommonBase;
            }
        }
        #endregion

        #region Method

        #region Clear
        /// <summary>
        /// clear ค่า ใน property SQLOrderBy , SQLWhere , SQLSelect
        /// </summary>
        public void Clear()
        {
            SQLOrderBy = string.Empty;
            SQLWhere = string.Empty;
            SQLSelect = string.Empty;
            SQLGroupBy = string.Empty;
        }
        #endregion

        #endregion

        #region enum RowFlagAction
        /// <summary>
        /// สำหรับกำหนดค่า SQLWhere : RowFlag
        /// </summary>
        public enum RowFlagAction
        {
            /// <summary>
            /// FrontEnd All  -> RowFlag > 0
            /// </summary>
            FrontEnd,
            /// <summary>
            /// BackEnd Index -> RowFlag >= 0
            /// </summary>
            BackEnd,
            /// <summary>
            /// BackEnd Trash  -> RowFlag = -2
            /// </summary>
            Trash
        }
        #endregion
    }
}
