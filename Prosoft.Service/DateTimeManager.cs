// ===============================================================================
// DateTimeManager
//
// file : DateTimeManager.cs
//
// This file contains the DateTime Helper
//
// For more information contact Somporn Thongcharoen
// ===============================================================================
// Release history
// VERSION	DESCRIPTION
//   1.0	Added support for 
//          - Null      date time null
//          - ToDay     get today ERA
//          - 
// ===============================================================================
// Copyright (C) 2005 Prosoft Comtech Co,.Ltd
// All rights reserved.
// ===============================================================================
// PROPERTY DESCRIPTION
//  1. 
//  2. 
//  3. 
//  4. 
// ==============================================================================
// METHOD DESCRIPTION
// ==============================================================================
// Example.
// DateTimeManager dtHlp = new DateTimeManager(ApplicationManager.CurrentERA);//date time helper
// this.caledarComboPicker1.Format = dtHlp.DateFormat;//set date format to CalendarCombo control
// ==============================================================================
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using System.Transactions;
//using res = Prosoft.Resource.Web.Ouikum;

namespace Prosoft.Service
{
    #region Range Date Struct
    /// <summary>
    /// Struct of Range Date
    /// </summary>
    public struct RangeDate
    {
        /// <summary>
        /// Start DateTime
        /// </summary>
        public DateTime FormDate;
        /// <summary>
        /// End DateTime
        /// </summary>
        public DateTime ToDate;
    }
    #endregion

    #region DateOption Enumeration
    /// <summary>
    /// Date Option
    /// </summary>
    public enum DateOption
    {
        /// <summary>
        /// ��ǧ�ѹ���
        /// </summary>
        Today,
        /// <summary>
        /// ��ǧ���駹��
        /// </summary>
        Tomorrow,
        /// <summary>
        /// ��ǧ�ѻ������
        /// </summary>
        ThisWeek,
        /// <summary>
        /// ��ǧ��͹���
        /// </summary>
        ThisMonth, 
        /// <summary>
        /// ��ǧ����ʹ��
        /// </summary>
        ThisQuarter, 
        /// <summary>
        /// ��ǧ�չ��
        /// </summary>
        ThisYear,
        /// <summary>
        /// ��ǧ������ҹ
        /// </summary>
        Yesterday, 
        /// <summary>
        /// ��ǧ�ѻ����������
        /// </summary>
        LastWeek,
        Last7Days, 
        /// <summary>
        /// ��ǧ��͹�������
        /// </summary>
        LastMonth, 
        /// <summary>
        /// ��ǧ����ʷ������
        /// </summary>
        LastQuarter, 
        /// <summary>
        /// ��ǧ�շ������
        /// </summary>
        LastYear,
        /// <summary>
        /// ��ǧ��͹���㹻շ������
        /// </summary>
        ThisMonthInLastYear, 
        /// <summary>
        /// ��ǧ����ʹ��㹻շ������
        /// </summary>
        ThisQuarterInLastYear,
        /// <summary>
        /// ��ǧ�ѻ����˹��
        /// </summary>
        //##23/11/2552=Beau ���� NextWeek,NextMonth,NextQuarter
        //����Ѻ����˹�� Opportunity List Filter ������� Expect Close Date
        NextWeek,
        /// <summary>
        /// ��ǧ��͹˹��
        /// </summary>
        NextMonth,
        /// <summary>
        /// ��ǧ�����˹��
        /// </summary>
        NextQuarter
    }
    #endregion

    #region DateTimeManager
    /// <summary>
    /// Class for manage DateTime use with calendar combopicker control and ultraGrid control 
    /// </summary>
    public class DateTimeManager
    {
        #region Member         
        static private DateTime serverDate = DateTime.Now;
        //static private Timer timer = new Timer();
        #endregion       
        
        #region Property

        #region Null
        /// <summary>
        /// Used for get null date, 1900-01-01
        /// </summary>
        /// <remarks></remarks>
        /// <example>
        /// 
        /// <code>
        ///  DateTime date = dtHelper.Null;
        /// </code>
        /// <c>Result</c><br/>
        /// year=1900/month=1/day=1
        /// </example>				
        public static DateTime Null
        {
            get { return new DateTime(9999, 12, 31, 0, 0, 0); }
        }
        #endregion

        #region Now
        /// <summary>
        /// Used for get current date of application.
        /// </summary>
        /// <remarks></remarks>
        /// <example>
        /// 06-06-2006 12.30.45.0000
        /// <code>
        ///  DateTime date = dtHelper.Now;
        /// </code>
        /// <c>Result</c><br/>
        /// Now of computer
        /// </example>				
        public static DateTime Now
        {
            get 
            {
                #if(DEBUG)
                //return DateTime.Now;  /*Ẻ���*/
                return DateTimeManager.GetServerDateTime();  /*2012-08-28 (Golf) : �� ServerDateTime ᷹*/
                #endif

                #if(!DEBUG)//������繵���ѹ
                return serverDate; 
                #endif
            }
        }
        #endregion

        #region ToDay
        /// <summary>
        /// Used for get current date of application.
        /// ** Contain Time **
        /// </summary>
        /// <remarks></remarks>
        /// <example>
        /// 06-06-2006 00.00.00.0000
        /// <code>
        ///  DateTime date = dtHelper.ToDay;
        /// </code>
        /// <c>Result</c><br/>
        /// Today of computer
        /// </example>				
        public static DateTime ToDay
        {
            get 
            {
                return new DateTime(Now.Year, Now.Month, Now.Day, 0, 0, 0); 
            }
        }
        #endregion

        #region DateFormat
        /// <summary>
        /// Get date format for calendar combopicker control        
        /// </summary>
        /// <remarks></remarks>
        /// <example>
        /// 
        /// <code>
        /// string format = dtHelper.DateFormat;
        /// </code>
        /// <c>Result</c><br/>
        /// Dateformat of DateTimeManager
        /// </example>        
        public static string DateFormat
        {
            get { return ApplicationManager.DateFormat; }
        }
        #endregion

        #region TimeFormat
        /// <summary>
        /// Get time format for ultraGrid control        
        /// </summary>
        /// <remarks></remarks>
        /// <example>
        /// 
        /// <code>
        /// string format = dtHelper.TimeFormat;
        /// </code>
        /// <c>Result</c><br/>
        /// Timeformat of DateTimeManager
        /// </example>				
        public static string TimeFormat
        {
            get { return ApplicationManager.TimeFormat; }
        }
        #endregion
              
        #region Year
        /// <summary>
        /// Used for get current year
        /// </summary>
        /// <remarks></remarks>
        /// <example>
        /// 
        /// <code>
        /// int year = dtHelper.Year;
        /// </code>
        /// <c>Result</c><br/>
        /// year of today
        /// </example>				
        public static int Year
        {
            get 
            {
                int year = Now.Year;
                if (ApplicationManager.CurrentERA == ApplicationManager.ERA.Buddhist)
                {
                    year += 543;
                }
                return year;            
            }
        }
        #endregion

        #region MaxYear
        /// <summary>
        /// Used for get max year
        /// </summary>
        /// <remarks></remarks>
        /// <example>
        /// 
        /// <code>
        ///  int maxYear = dtHelper.MaxYear;
        /// </code>
        /// <c>Result</c><br/>
        /// max year of System.DateTime
        /// </example>				
        public static int MaxYear {            
            get{return System.DateTime.MaxValue.Year;}
        }
        #endregion

        #region MinYear
        /// <summary>
        /// Used for get min year
        /// </summary>
        /// <remarks></remarks>
        /// <example>
        /// 
        /// <code>
        /// int maxYear = dtHelper.MinYear;
        /// </code>
        /// <c>Result</c><br/>
        /// min year of System.DateTime
        /// </example>				
        public static int MinYear {
            get { return System.DateTime.MinValue.Year; }
        }
        #endregion     

        #endregion

        #region GetDuration
        /// <summary>
        /// Used for calculate Duration of time1 and time2, return as TimeSpan
        /// </summary>
        /// <param name="time1">1900-01-01Thh:mm:ss</param>
        /// <param name="time2">1900-01-01Thh:mm:ss</param>
        /// <returns>TimeSpan data type</returns>
        /// <remarks></remarks>
        /// <example>
        /// 
        /// <code>
        ///  DateTime time1 = new DateTime(2005, 1, 1);
        ///  DateTime time2 = new DateTime(2005, 12, 31);
        ///  TimeSpan timeSpan = dtHelper.GetDuration(time1,time2);
        ///  string str= timeSpan.ToString();
        /// </code>
        /// <c>Result</c><br/>
        /// str = 364.00:00:00 
        /// </example>				
        public static TimeSpan GetDuration(DateTime time1, DateTime time2) {
            // diff1 gets 185 days, 14 hours, and 47 minutes.           
            TimeSpan diff1 = time2.Subtract(time1);
            return (diff1);
        }
        #endregion

        #region ConcateDateTime
        /// <summary>
        /// Used for concate Date and Time
        /// </summary>
        /// <param name="date">2005-01-31T00:00:00</param>
        /// <param name="time">1900-01-01T15:30:00</param>
        /// <returns>2005-01-31T15:30:00</returns>
        /// <remarks></remarks>
        /// <example>
        /// 
        /// <code>
        ///  DateTime time1 = new DateTime(2005, 1, 1);
        ///  DateTime time2 = new DateTime(2005, 12, 31,10,25,30);
        ///  DateTime time3 = new dtHelper.ConcateDateTime(time1,time2);
        ///  string str = time3.ToString();
        /// </code>
        /// <c>Result</c><br/>
        /// str = 1/1/2548 10:25:30
        /// </example>				
        public static DateTime ConcateDateTime(DateTime date, DateTime time) {
            return (new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second));
        }
        #endregion

        #region ConvertDateStringToDateTime
        /// <summary>
        /// Convert date string from CalendarComboPicker to DateTime datatype with current
        /// </summary>
        /// <param name="dateString">16/03/2548,16/03/2005</param>
        /// <param name="currentDateFormat">ApplicationManager.DateFormat, dd/MM/yyyy</param>        
        /// <returns>Datetime value in current format</returns>
        /// <remarks></remarks>
        /// <example>
        /// 
        /// <code>
        ///  string dateString = "30/12/2005";
        ///  string dateFormat = "dd/mm/yyyy";
        ///  DateTime time = dtHelper.ConvertDateStringToDateTime(dateString,dateFormat);
        ///  string year = time.Year.ToString();
        ///  string month = time.Month.ToString();
        ///  string day = time.Day.ToString();
        /// </code>
        /// <c>Result</c><br/>
        ///  year = 2005
        ///  month = 12
        ///  day = 30
        /// </example>	
        public static DateTime ConvertDateStringToDateTime(string dateString, string currentDateFormat)
        {
            return ConvertDateStringToDateTime(dateString, currentDateFormat, ApplicationManager.CurrentERA);
        }
        /// <summary>
        /// Convert date string from CalendarComboPicker to DateTime datatype with current
        /// </summary>
        /// <param name="dateString">16/03/2548,16/03/2005</param>
        /// <param name="currentDateFormat">ApplicationManager.DateFormat, dd/MM/yyyy</param>  
        /// <param name="ERA">ApplicationManager.ERA need convert to</param>  
        /// <returns>Datetime value in current format</returns>
        /// <remarks></remarks>
        /// <example>
        /// 
        /// <code>
        ///  string dateString = "30/12/2005";
        ///  string dateFormat = "dd/mm/yyyy";
        ///  DateTime time = dtHelper.ConvertDateStringToDateTime(dateString,dateFormat);
        ///  string year = time.Year.ToString();
        ///  string month = time.Month.ToString();
        ///  string day = time.Day.ToString();
        /// </code>
        /// <c>Result</c><br/>
        ///  year = 2005
        ///  month = 12
        ///  day = 30
        /// </example>				
        public static DateTime ConvertDateStringToDateTime(string dateString, string currentDateFormat, ApplicationManager.ERA ERA)
        {                                    
            if(string.IsNullOrEmpty(dateString)|(dateString.Length != currentDateFormat.Length)){
                //Text on control is null, return datate on 1900-1-1
                return Null;
            }
            int[] index;
            index = IndexLengthOfDay(currentDateFormat);
            int day = Int32.Parse(dateString.Substring(index[0], index[1]));
            index = IndexLengthOfMonth(currentDateFormat);
            int month = Int32.Parse(dateString.Substring(index[0], index[1]));
            index = IndexLengthOfYear(currentDateFormat);
            int year = Int32.Parse(dateString.Substring(index[0], index[1]));
            int hour = DateTime.Now.Hour;
            int minute = DateTime.Now.Minute;
            int second = DateTime.Now.Second;
            if (dateString.IndexOf(" ") > -1)
            {
                string time = dateString.Remove(0, dateString.IndexOf(" ") + 1);
                string[] arr = time.Split(':');
                try { hour = int.Parse(arr[0]); }
                catch { hour = 0; }
                try { minute = int.Parse(arr[1]); }
                catch { minute = 0; }
                try { second = int.Parse(arr[2]); }
                catch { second = 0; }
            } 

            DateTime dtResult;
            try
            {
                //prevent cause dateString can't convert to datetime data type
                dtResult = new DateTime(year, month, day, hour, minute, second);
            }
            catch
            {
                dtResult = DateTimeManager.Null;
            }
            year = int.Parse(dtResult.ToShortDateString().Split('/')[2]);
            if (ERA == ApplicationManager.ERA.Buddhist && year < 2400)
            {
                dtResult = dtResult.AddYears(543);
            }
            else if (ERA == ApplicationManager.ERA.Christian && year > 2400)
            {
                dtResult = dtResult.AddYears(-543);
            }            
            return dtResult;
        }
        #endregion

        #region ConvertDateTimeToDateString
        /// <summary>
        /// ��˹�ҷ��㹡���ŧ DateTime ����� String ����繻� �.�.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ConvertDateTimeToDateString(DateTime date)
        {
            string dateStr = string.Empty;

            if (date.Year > 2400)
            {
                int year = date.Year - 543;
                dateStr = year.ToString();
            }
            else
            {
                dateStr += date.Year.ToString();
            }           
            dateStr += date.Month.ToString("00");           
            dateStr += date.Day.ToString("00");

            return dateStr;
        }

        /// <summary>
        /// convert DateTime to String by format
        /// </summary>
        /// <param name="datetime">DateTime need convert</param>
        /// <param name="currentDateFormat">format of DateTime need convert</param>
        /// <returns>string of DateTime after convert</returns>
        /// <remarks></remarks>
        /// <example>
        /// 
        /// <code>
        ///  DateTime time = new DateTime(10,12,2005);
        ///  string format = "dd/mm/yyyy";
        ///  string str = dtHelper.ConvertDateTimeToDateString(time,format);
        /// </code>
        /// <c>Result</c><br/>
        ///  str = 10/12/2005
        /// </example>				
        public static string ConvertDateTimeToDateString(DateTime datetime, string currentDateFormat)
        {
            string dateString = null;
            if (datetime != null) {
                dateString = currentDateFormat;
                int[] index;
                //Day : replace "dd" with datetime.Day
                index = IndexLengthOfDay(currentDateFormat);
                dateString = dateString.Replace(currentDateFormat.Substring(index[0], index[1]),
                    ((datetime.Day < 10) & (index[1] == 2) ? "0" + datetime.Day.ToString() : datetime.Day.ToString()));

                //Month: replace "mm" with datetime.Month
                index = IndexLengthOfMonth(currentDateFormat);
                dateString = dateString.Replace(currentDateFormat.Substring(index[0], index[1]),
                    ((datetime.Month < 10) & (index[1] == 2) ? "0" + datetime.Month.ToString() : datetime.Month.ToString()));

                //Year: replace "dd" with datetime.Year
                index = IndexLengthOfYear(currentDateFormat);
                int year = (Int16)datetime.Year;
                string yearString;
                if (ApplicationManager.CurrentERA != ApplicationManager.ERA.Christian && year < 2400)
                {
                    year += 543;
                }
                yearString = year.ToString();
                if (index[1] == 2) {
                    yearString = yearString.Substring(2, 2);
                }
                dateString = dateString.Replace(currentDateFormat.Substring(index[0], index[1]), yearString);
            }
            return dateString;
        }

        /// <summary>
        /// Convert DateTime to String by DataTable
        /// </summary>
        /// <param name="dataSource">DateTable need convert DateTime to String</param>
        /// <param name="columnSource">Column name on DataTable need convert</param>
        /// <param name="currentDateFormat">Format of DateTime need convert</param>
        /// <remarks>After convert result will is on column columnSoure+"Str"</remarks>
        /// <example>
        /// 
        /// <code>
        /// //dataTable is DataTable on DataSet has column StartDate and column StartDateStr 
        /// //dataTable.Rows[0]["StartDate"] = 22/12/2005
        /// dtHelper.ConvertDateTimeToDateString(dataTable,"StartDate","mm/dd/yyyy");
        /// string str = dataTable.Rows[0]["StartDateStr"].ToString();
        /// </code>
        /// <c>Result</c><br/>
        /// str = 12/22/2005
        /// </example>	        
        public static void ConvertDateTimeToDateString(DataTable dataSource, string columnSource, string currentDateFormat)
        {
            string columnDestination = "";
            if (dataSource != null && !string.IsNullOrEmpty(columnSource))
            {
                columnDestination = columnSource + "Str";
                if(dataSource.Columns.Contains(columnDestination))
                {
                    ConvertDateTimeToDateString(dataSource, columnSource, columnDestination, currentDateFormat);
                }
                else throw new Exception("Can't convert because not found column " + columnDestination + " for contrain data after convert complete!");
            }
        }

        /// <summary>
        /// Convert DateTime to String by DataTable
        /// </summary>
        /// <param name="dataSource">DateTable need convert DateTime to String</param>
        /// <param name="columnSource">Column name on DataTable need convert</param>
        /// <param name="columnDestination">Column name on DataTable need keep value after convert</param>
        /// <param name="currentDateFormat">Format of DateTime need convert</param>
        /// <remarks></remarks>
        /// <example>
        /// 
        /// <code>
        /// //dataTable is DataTable on DataSet has column StartDate and column OutputDate 
        /// //dataTable.Rows[0]["StartDate"] = 22/12/2005
        /// dtHelper.ConvertDateTimeToDateString(dataTable,"StartDate","OutputDate","mm/dd/yyyy");
        /// string str = dataTable.Rows[0]["OutputDate"].ToString();
        /// </code>
        /// <c>Result</c><br/>
        /// str = 12/22/2005
        /// </example>				
        public static void ConvertDateTimeToDateString(DataTable dataSource, string columnSource, string columnDestination, string currentDateFormat)
        {
            if (dataSource != null && !string.IsNullOrEmpty(columnSource) && !string.IsNullOrEmpty(columnDestination))
            {
                if (dataSource.Columns[columnSource].DataType.ToString() == "System.DateTime")
                {
                    if (dataSource.Columns[columnDestination].DataType.ToString() == "System.String")
                    {
                        try
                        {
                            foreach (DataRow row in dataSource.Rows)
                            {
                                if (row[columnSource] != System.Convert.DBNull && !string.IsNullOrEmpty(row[columnSource].ToString()) && (DateTime)row[columnSource] != Null)
                                {
                                    row[columnDestination] = ConvertDateTimeToDateString(DateTime.Parse(row[columnSource].ToString()), currentDateFormat);                                        
                                }
                                else row[columnDestination] = string.Empty;
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.Message);
                        }
                    }
                    else throw new Exception("Can't convert because data type of column " + columnDestination + " is not String!");
                }
                else throw new Exception("Can't convert because data type of column " + columnSource + " is not DateTime!");
            }
        }

        /// <summary>
        /// Convert DateTime to String by DataTable
        /// </summary>
        /// <param name="dataSource">DateTable need convert DateTime to String</param>
        /// <param name="columnsSource">Array of column name on DataTable need convert</param>
        /// <param name="currentDateFormat">Format of DateTime need convert</param>
        /// <remarks></remarks>
        /// <example>
        /// <code>
        /// //dataTable is DataTable on DataSet has column StartDate,EndDate,ModifyDate,StartDateStr,EndDateStr,ModifyDateStr
        /// //dataTable.Rows[0]["StartDate"] = 22/12/2005
        /// //dataTable.Rows[0]["EndDate"] = 30/12/2005
        /// //dataTable.Rows[0]["ModifyDate"] = 01/12/2005
        /// string[] ArrSource = new string{"StartDate","EndDate","ModifyDate"}
        /// dtHelper.ConvertDateTimeToDateString(dataTable,ArrSource,"mm/dd/yyyy");
        /// string strStartDate = dataTable.Rows[0]["StartDateStr"].ToString();
        /// string strEndDate = dataTable.Rows[0]["EndDateStr"].ToString();
        /// string strModifyDate = dataTable.Rows[0]["ModifyDateStr"].ToString();               
        /// </code>
        /// <c>Result</c><br/>
        /// strStartDate = 12/22/2005
        /// strEndDate = 12/30/2005
        /// strModifyDate = 12/01/2005
        /// </example>				
        public static void ConvertDateTimeToDateString(DataTable dataSource, string[] columnsSource, string currentDateFormat)
        {
            foreach (string str in columnsSource)
            {
                try
                {
                    ConvertDateTimeToDateString(dataSource, str, currentDateFormat);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        /// <summary>
        /// Convert DateTime to String by DataTable
        /// </summary>
        /// <param name="dataSource">DateTable need convert DateTime to String</param>
        /// <param name="columnsSource">Array of column name on DataTable need convert</param>
        /// <param name="columnsDestination">Array of column name on DataTable need keep value after convert</param>
        /// <param name="currentDateFormat">Format of DateTime need convert</param>
        /// <remarks></remarks>
        /// <example>
        /// 
        /// <code>
        /// //dataTable is DataTable on DataSet has column StartDate,EndDate,ModifyDate,OutputStartDate,OutputEndDate,OutputModifyDate
        /// //dataTable.Rows[0]["StartDate"] = 22/12/2005
        /// //dataTable.Rows[0]["EndDate"] = 30/12/2005
        /// //dataTable.Rows[0]["ModifyDate"] = 01/12/2005
        /// string[] ArrSource = new string{"StartDate","EndDate","ModifyDate"}
        /// string[] ArrDestinatin = new string{"OutputStartDate","OutputEndDate","OutputModifyDate"}
        /// dtHelper.ConvertDateTimeToDateString(dataTable,ArrSource,ArrDestinatin,"mm/dd/yyyy");
        /// string strStartDate = dataTable.Rows[0]["OutputStartDate"].ToString();
        /// string strEndDate = dataTable.Rows[0]["OutputEndDate"].ToString();
        /// string strModifyDate = dataTable.Rows[0]["OutputModifyDate"].ToString(); 
        /// </code>
        /// <c>Result</c><br/>
        /// strStartDate = 12/22/2005
        /// strEndDate = 12/30/2005
        /// strModifyDate = 12/01/2005
        /// </example>				
        public static void ConvertDateTimeToDateString(DataTable dataSource, string[] columnsSource, string[] columnsDestination, string currentDateFormat)
        {
            if (columnsDestination.Length == columnsSource.Length)
            {
                try
                {
                    for (int i = 0; i < columnsDestination.Length; i++)
                    {
                        ConvertDateTimeToDateString(dataSource, columnsSource[i].ToString(), columnsDestination[i].ToString(), currentDateFormat);
                    }
                }
                catch(Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            else throw new Exception("Length of array columnsSource and array columnsDestination not equal");
        }
        #endregion

        #region ConvertDatetimeToDateTimeERA
        /// <summary>
        /// �ŧ��� datetime ���ç��� format �ͧ era
        /// 2012-08-28 (Golf) : ����¹���� ConvertDatetimeToDatetimeERA �� ConvertDatetimeToDateTimeERA
        /// </summary>
        /// <param name="datetime">��ҷ���ͧ����ŧ</param>
        /// <param name="era">��ҷ���ͧ����ŧ</param>
        /// <returns></returns>
        public static DateTime ConvertDatetimeToDateTimeERA(DateTime datetime, ApplicationManager.ERA era)
        {
            try
            {
                if (era == ApplicationManager.ERA.Buddhist && datetime.Year < 2400)
                {
                    return new DateTime(datetime.Year + 543, datetime.Month, datetime.Day, datetime.Hour, datetime.Minute, datetime.Second);
                }
                else if (era == ApplicationManager.ERA.Christian && datetime.Year > 2400)
                {
                    return new DateTime(datetime.Year - 543, datetime.Month, datetime.Day, datetime.Hour, datetime.Minute, datetime.Second);
                }
                else return datetime;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region GetMonthName
        /// <summary>
        /// Used for get month name by localization
        /// </summary>
        /// <param name="month">Number of month</param>
        /// <returns>Month name by culture</returns>
        /// <remarks></remarks>
        /// <example>
        /// 
        /// <code>
        /// string monthName = dtHelper.GetMonthName(12);
        /// </code>
        /// <c>Result</c><br/>
        /// Language is English monthName = December
        /// Language is Thai monthName = month name 12 language thai
        /// </example>				
        public static string GetMonthName(int month) 
        {
            string monthName = "";
            switch (month) {
                case 1:
                    if (ApplicationManager.CurrentLanguage == ApplicationManager.DefaultLanguage) monthName = "���Ҥ�";
                    else monthName = "January";                    
                    break;
                case 2:
                    if (ApplicationManager.CurrentLanguage == ApplicationManager.DefaultLanguage) monthName = "����Ҿѹ��";
                    else monthName = "February";
                    break;
                case 3: 
                    if (ApplicationManager.CurrentLanguage == ApplicationManager.DefaultLanguage) monthName = "�չҤ�";
                    else monthName = "March";
                    break;
                case 4:
                    if (ApplicationManager.CurrentLanguage == ApplicationManager.DefaultLanguage) monthName = "����¹";
                    else monthName = "Apirl";
                    break;
                case 5:
                    if (ApplicationManager.CurrentLanguage == ApplicationManager.DefaultLanguage) monthName = "����Ҥ�";
                    else monthName = "May";
                    break;
                case 6:
                    if (ApplicationManager.CurrentLanguage == ApplicationManager.DefaultLanguage) monthName = "�Զع�¹";
                    else monthName = "June";
                    break;
                case 7:
                    if (ApplicationManager.CurrentLanguage == ApplicationManager.DefaultLanguage) monthName = "�á�Ҥ�";
                    else monthName = "July";
                    break;
                case 8:
                    if (ApplicationManager.CurrentLanguage == ApplicationManager.DefaultLanguage) monthName = "�ԧ�Ҥ�";
                    else monthName = "August";
                    break;
                case 9:
                    if (ApplicationManager.CurrentLanguage == ApplicationManager.DefaultLanguage) monthName = "�ѹ��¹";
                    else monthName = "September";
                    break;
                case 10:
                    if (ApplicationManager.CurrentLanguage == ApplicationManager.DefaultLanguage) monthName = "���Ҥ�";
                    else monthName = "October";
                    break;
                case 11:
                    if (ApplicationManager.CurrentLanguage == ApplicationManager.DefaultLanguage) monthName = "��Ȩԡ�¹";
                    else monthName = "November";
                    break;
                case 12:
                    if (ApplicationManager.CurrentLanguage == ApplicationManager.DefaultLanguage) monthName = "�ѹ�Ҥ�";
                    else monthName = "December";
                    break;
            }
            return monthName;
        }
        #endregion

        #region GetMonthNumber
        /// <summary>
        /// Used for get month Number by localization
        /// </summary>
        /// <param name="monthName">monthName of month</param>
        /// <returns>Month name by culture</returns>
        /// <remarks></remarks>
        /// <example>
        /// 
        /// <code>
        /// int monthNumber = dtHelper.GetMonthNumber("January");
        /// </code>
        /// <c>Result</c><br/>
        /// 1
        /// </example>				
        public static int GetMonthNumber(string monthName)
        {
            int monthNumber = 0;
            switch (monthName)
            {
                case "January":
                    monthNumber = 1;
                    break;
                case "February":
                    monthNumber = 2;
                    break;
                case "March":
                    monthNumber = 3;
                    break;
                case "April":
                    monthNumber = 4;
                    break;
                case "May":
                    monthNumber = 5;
                    break;
                case "June":
                    monthNumber = 6;
                    break;
                case "July":
                    monthNumber = 7;
                    break;
                case "August":
                    monthNumber = 8;
                    break;
                case "September":
                    monthNumber = 9;
                    break;
                case "October":
                    monthNumber = 10;
                    break;
                case "November":
                    monthNumber = 11;
                    break;
                case "December":
                    monthNumber = 12;
                    break;
            }
            return monthNumber;
        }
        #endregion

        #region GetDayPerMonth
        /// <summary>
        /// ��ӡ���Ҩӹǹ�ѹ�����͹�ͧ�շ���к�
        /// </summary>
        /// <param name="month">��͹����ͧ���</param>
        /// <returns>�ӹǹ�ѹ�����͹</returns>
        public static int GetDayPerMonth(int month)
        {
            return DateTime.DaysInMonth(DateTime.Today.Year, month);
        }
        /// <summary>
        /// ��ӡ���Ҩӹǹ�ѹ�����͹�ͧ�ջѨ�غѹ
        /// </summary>
        /// <param name="currentYear">�շ���ͧ���</param>
        /// <param name="month">��͹����ͧ���</param>
        /// <returns>�ӹǹ�ѹ�����͹</returns>
        public static int GetDayPerMonth(DateTime currentYear, int month)
        {
            return DateTime.DaysInMonth(currentYear.Year, month);
        }

        public static int GetDayPerMonth(int year, int month)
        {
            return DateTime.DaysInMonth(year, month);
        }

        #endregion

        #region ConvertHourToMinutes
        /// <summary>
        /// ��㹡���ŧ�ӹǹ ������� ��� �ҷ�
        /// </summary>
        /// <param name="Hour">�ӹǹ�������</param>
        /// <param name="Minutes">�ӹǹ�ҷ�</param>
        /// <returns>�ӹǹ�ҷշ�����</returns>
        public static double ConvertHourToMinutes(int Hour, int Minutes)
        {
            TimeSpan tmpTime = new TimeSpan(Hour, Minutes, 0);
            return tmpTime.TotalMinutes;
        }
        #endregion 

        #region ConvertMinutesToHour
        /// <summary>
        /// ��㹡���ŧ�ӹǹ�ҷ� ��� �������
        /// </summary>
        /// <param name="Minutes">�ӹǹ�ҷ�</param>
        /// <returns>�ӹǹ�������</returns>
        public static double ConvertMinutesToHour(int Minutes)
        {
            int timeHour = Minutes / 60;
            int timeMinute = timeHour * 60;
            timeMinute = Minutes - timeMinute;
            string tmpStr;
            if (timeMinute == 0)
            {
                tmpStr = timeHour.ToString() + "." + timeMinute.ToString() + "0";
            }
            else
            {
                tmpStr = timeHour.ToString() + "." + timeMinute.ToString();
            }
            double tmpdouble = double.Parse(tmpStr);
            return tmpdouble;
        }
        #endregion 

        #region ConvertMinutesToFormateTime
        /// <summary>
        /// ��㹡���ŧ�ӹǹ�ҷ� ��� ������� ��ٻẺ �ӹǹ������� : �ӹǹ�ҷ�
        /// </summary>
        /// <param name="Minutes">�ӹǹ�ҷ�</param>
        /// <returns>�ٻẺ�� String �ӹǹ������� : �ҷ�</returns>
        /// <modified>Boonma Noijunvong 2006-10-17 15:00</modified>
        public static string ConvertMinutesToFormateTime(int Minutes)
        {
            int timeHour = Minutes / 60;
            int timeMinute = timeHour * 60;
            timeMinute = Minutes - timeMinute;
            string tmpStr;
            if (timeMinute == 0)
            {
                if (timeHour < 10)
                {
                    tmpStr = "0" + timeHour.ToString() + ":" + timeMinute.ToString() + "0";
                }
                else
                {
                    tmpStr = timeHour.ToString() + ":" + timeMinute.ToString() + "0";
                }
            }
            else
            {
                if (timeHour < 10)
                {
                    tmpStr = "0" + timeHour.ToString() + ":" + timeMinute.ToString();
                }
                else
                {
                    tmpStr = timeHour.ToString() + ":" + timeMinute.ToString();
                }
            }
            return tmpStr;
        }
        #endregion

        #region ConvertTimeToMinute
        /// <summary>
        /// ��㹡���ŧ�����繨ӹǹ�ҷ�
        /// </summary>
        /// <param name="Time">���ҷ���ͧ��� Convet</param>
        /// <returns>�ӹǹ�ҷ�</returns>
        /// <modified>Boonma Noijunvong 2006-10-17 15:00</modified>
        public static int ConvertTimeToMinute(string Time)
        {
            int minute = 0;
            try
            {
                //int minute = 0;
                string[] strTime = Time.Trim().Split(':');
                string TypeTime = strTime[2].Substring(strTime[2].Length - 2,2);

                if (!string.IsNullOrEmpty(Time) && Time.Length > 0)
                {
                    if (TypeTime == "AM")
                    {
                        minute = (Convert.ToInt32(strTime[0]) * 60) + Convert.ToInt32(strTime[1]);
                    }
                    else
                    {
                        minute = (Convert.ToInt32(strTime[0]) * 60) + 720 + Convert.ToInt32(strTime[1]);
                    }
                }
            }
            catch
            {
            }
            return minute;
        }
        #endregion

        #region DbDateTime
        /// <summary>
        /// used for convert datetime to string datetime in SqlServer Database format
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <example>
        /// 
        /// <code>
        /// 
        /// </code>
        /// <c>Result</c><br/>
        /// 
        /// </example>				
        public static string DbDateTime(DateTime datetime) 
        {
            string year = datetime.Year.ToString();
            if (datetime.Year > 2400)
            {
                int yearValue = datetime.Year - 543;
                year = yearValue.ToString();
            }
            StringBuilder strBuilder = new StringBuilder();            
            strBuilder.Append(year).Append(datetime.Month.ToString("0#")).Append(datetime.Day.ToString("0#"));            
            return strBuilder.ToString();
        }
        #endregion      
        
        #region Finding Index

        #region IndexLengthOfDay
        /// <summary>
        /// Used for find day position and day length format
        /// </summary>
        /// <param name="source">Format of DateTime "dd/mm/yyyy","mm/dd/yyyy", ...</param>
        /// <returns>Array of int store index of day in string source</returns>
        private static int[] IndexLengthOfDay(string source)
        {
            source = source.ToLower();
            int[] index = new int[] { 0, 0 };
            if (source.IndexOf("dd") > -1)
            {
                index[0] = source.IndexOf("dd");
                index[1] = 2;
            }
            else if (source.IndexOf('d') > -1)
            {
                index[0] = source.IndexOf('d');
                index[1] = 1;
            }
            return index;
        }
        #endregion

        #region IndexLengthOfMonth
        /// <summary>
        /// used for find month position and month length format
        /// </summary>
        /// <param name="source"></param>
        /// <returns>Array of int store index of month in string source</returns>
        private static int[] IndexLengthOfMonth(string source)
        {
            source = source.ToLower();
            int[] index = new int[] { 0, 0 };
            if (source.IndexOf("mm") > -1)
            {
                index[0] = source.IndexOf("mm");
                index[1] = 2;
            }
            else if (source.IndexOf('m') > -1)
            {
                index[0] = source.IndexOf('m');
                index[1] = 1;
            }
            return index;
        }
        #endregion

        #region IndexLengthOfYear
        /// <summary>
        /// Used for find year position and year length format
        /// </summary>
        /// <param name="source">Format of DateTime "dd/mm/yyyy","mm/dd/yyyy", ...</param>
        /// <returns>Array of int store index of year in string source</returns>
        private static int[] IndexLengthOfYear(string source)
        {
            source = source.ToLower();
            int[] index = new int[] { 0, 0 };
            if (source.IndexOf("yyyy") > -1)
            {
                index[0] = source.IndexOf("yyyy");
                index[1] = 4;
            }
            else if (source.IndexOf("yy") > -1)
            {
                index[0] = source.IndexOf("yy");
                index[1] = 2;
            }
            return index;
        }
        #endregion

        #endregion                

        #region ������͡������Ẻ�繪�ǧ�ѹ��� ���� DateOption

        #region Get Date Option
        /// <summary>
        /// 
        /// </summary>
        /// <param name="optionValue"></param>
        /// <returns></returns>
        public static string GetDateOptionName(DateOption optionValue)
        {
            string optionName = "";
            switch (optionValue)
            {
                case DateOption.Today:
                    optionName = "Today";
                    break;
                case DateOption.Tomorrow:
                    optionName = "Tomorrow";
                    break;
                case DateOption.ThisWeek:
                    optionName = "This Week";
                    break;
                case DateOption.ThisMonth:
                    optionName = "This Month";
                    break;
                case DateOption.ThisQuarter:
                    optionName = "This Quarter";
                    break;
                case DateOption.ThisYear:
                    optionName = "This Year";
                    break;
                case DateOption.NextWeek:
                    optionName = "Next Week";
                    break;
                case DateOption.NextMonth:
                    optionName = "Next Month";
                    break;
                case DateOption.NextQuarter:
                    optionName = "Next Quarter";
                    break;
                case DateOption.Yesterday:
                    optionName = "Yesterday";
                    break;
                case DateOption.LastWeek:
                    optionName = "Last Week";
                    break;
                case DateOption.LastMonth:
                    optionName = "Last Month";
                    break;
                case DateOption.LastQuarter:
                    optionName = "Last Quarter";
                    break;
                case DateOption.LastYear:
                    optionName = "Last Year";
                    break;
                case DateOption.ThisMonthInLastYear:
                    optionName = "This Month in Last Year";
                    break;
                case DateOption.ThisQuarterInLastYear:
                    optionName = "This Quarter in Last Year";
                    break;
                default:
                    break;
            }
            return optionName;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="optionName"></param>
        /// <returns></returns>
        public static DateOption GetDateOptionValue(string optionName)
        {
            DateOption optionValue;
            switch (optionName.Trim())
            {
                case "Today":
                    optionValue = DateOption.Today;
                    break;
                case "Tomorrow":
                    optionValue = DateOption.Tomorrow;
                    break;
                case "This Week":
                    optionValue = DateOption.ThisWeek;
                    break;
                case "This Month":
                    optionValue = DateOption.ThisMonth;
                    break;
                case "Next Week":
                    optionValue = DateOption.NextWeek;
                    break;
                case "Next Month":
                    optionValue = DateOption.NextMonth;
                    break;
                case "Next Quarter":
                    optionValue = DateOption.NextQuarter;
                    break;
                case "This Quarter":
                case "Current Quarter":
                    optionValue = DateOption.ThisQuarter;
                    break;
                case "This Year":
                    optionValue = DateOption.ThisYear;
                    break;
                case "Yesterday":
                    optionValue = DateOption.Yesterday;
                    break;
                case "Last Week":
                    optionValue = DateOption.LastWeek;
                    break;
                case "Last 7 Days":
                    optionValue = DateOption.Last7Days;
                    break;
                case "Last Month":
                    optionValue = DateOption.LastMonth;
                    break;
                case "Last Quarter":
                    optionValue = DateOption.LastQuarter;
                    break;
                case "Last Year":
                    optionValue = DateOption.LastYear;
                    break;
                case "This Month in Last Year":
                    optionValue = DateOption.ThisMonthInLastYear;
                    break;
                case "This Quarter in Last Year":
                    optionValue = DateOption.ThisQuarterInLastYear;
                    break;
                default:
                    optionValue = DateOption.Today;
                    break;
            }
            return optionValue;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string[] GetDateOption()
        {
            string[] dateOptionList = {
			GetDateOptionName(DateOption.Today), 
            GetDateOptionName(DateOption.Tomorrow), 
			GetDateOptionName(DateOption.ThisWeek), 
			GetDateOptionName(DateOption.ThisMonth), 
			GetDateOptionName(DateOption.ThisQuarter), 
			GetDateOptionName(DateOption.ThisYear),
            GetDateOptionName(DateOption.NextWeek), 
			GetDateOptionName(DateOption.NextMonth), 
			GetDateOptionName(DateOption.NextQuarter),
			GetDateOptionName(DateOption.Yesterday), 
			GetDateOptionName(DateOption.LastWeek), 
			GetDateOptionName(DateOption.LastMonth), 
			GetDateOptionName(DateOption.LastQuarter), 
			GetDateOptionName(DateOption.LastYear),
			GetDateOptionName(DateOption.ThisMonthInLastYear), 
			GetDateOptionName(DateOption.ThisQuarterInLastYear)};
            return dateOptionList;
        }
        #endregion

        #region Get Range Date
        /// <summary>
        /// Get Range Date
        /// </summary>
        /// <param name="refDate">Current Date</param>
        /// <param name="optionName">DateOption which you want to get range</param>
        /// <returns>Range Date</returns>
        public RangeDate GetRangeDate(DateTime refDate, string optionName)
        {
            return GetRangeDate(refDate, GetDateOptionValue(optionName));
        }       
        /// <summary>
        /// Get Range Date
        /// </summary>
        /// <param name="refDate">Current Date</param>
        /// <param name="optionValue">DateOption which you want to get range</param>
        /// <returns>Range Date</returns>
        public RangeDate GetRangeDate(DateTime refDate, DateOption optionValue)
        {
            RangeDate result = new RangeDate();
            int quarter;
            int startMonth;
            int endMonth;
            DateTime lastDate;

            switch (optionValue)
            {
                case DateOption.Today:
                    result.FormDate = refDate;
                    result.ToDate = refDate;
                    break;
                case DateOption.Tomorrow:
                    result.FormDate = refDate.AddDays(+1);
                    result.ToDate = result.FormDate;
                    break;
                case DateOption.ThisWeek:
                    result.FormDate = refDate.AddDays(((int)refDate.DayOfWeek) * -1);
                    result.ToDate = refDate.AddDays(6 - ((int)refDate.DayOfWeek));
                    break;
                case DateOption.ThisMonth:
                    result.FormDate = new DateTime(refDate.Year, refDate.Month, 1);
                    result.ToDate = new DateTime(refDate.Year, refDate.Month, GetMaxDayInMonth(refDate.Year, refDate.Month));
                    break;
                case DateOption.ThisQuarter:
                    quarter = GetQuarterByMonth(refDate.Month);
                    startMonth = GetStartMonthInQuarter(quarter);
                    endMonth = GetEndMonthInQuarter(quarter);
                    result.FormDate = new DateTime(refDate.Year, startMonth, 1);
                    result.ToDate = new DateTime(refDate.Year, endMonth, GetMaxDayInMonth(refDate.Year, endMonth));
                    break;               
                case DateOption.ThisYear:
                    result.FormDate = new DateTime(refDate.Year, 1, 1);
                    result.ToDate = new DateTime(refDate.Year, 12, 31);
                    break;
                    //##23/11/2552=Beau ���� NextWeek,NextMonth,NextQuarter
                    //����Ѻ����˹�� Opportunity List Filter ������� Expect Close Date
                case DateOption.NextWeek:
                    lastDate = refDate.AddDays(7);
                    result.FormDate = lastDate.AddDays((((int)lastDate.DayOfWeek)) * -1);
                    result.ToDate = lastDate.AddDays(6 - ((int)lastDate.DayOfWeek));
                    break;
                case DateOption.NextMonth:
                    lastDate = refDate.AddMonths(1);
                    result.FormDate = new DateTime(lastDate.Year, lastDate.Month, 1);
                    result.ToDate = new DateTime(lastDate.Year, lastDate.Month, GetMaxDayInMonth(lastDate.Year, lastDate.Month));
                    break;
                case DateOption.NextQuarter:
                    lastDate = refDate.AddMonths(3);
                    quarter = GetQuarterByMonth(lastDate.Month);
                    startMonth = GetStartMonthInQuarter(quarter);
                    endMonth = GetEndMonthInQuarter(quarter);
                    result.FormDate = new DateTime(lastDate.Year, startMonth, 1);
                    result.ToDate = new DateTime(lastDate.Year, endMonth, GetMaxDayInMonth(lastDate.Year, endMonth));
                    break;

                case DateOption.Yesterday:
                    result.FormDate = refDate.AddDays(-1);
                    result.ToDate = result.FormDate;
                    break;
                case DateOption.LastWeek:
                    lastDate = refDate.AddDays(-7);
                    result.FormDate = lastDate.AddDays((((int)lastDate.DayOfWeek)) * -1);
                    int ss = (int)lastDate.DayOfWeek;
                    result.ToDate = lastDate.AddDays(6 - ((int)lastDate.DayOfWeek));
                    break;
                case DateOption.Last7Days:
                    result.FormDate = refDate.AddDays(-7);
                    result.ToDate = refDate;
                    break;
                case DateOption.LastMonth:
                    lastDate = refDate.AddMonths(-1);
                    result.FormDate = new DateTime(lastDate.Year, lastDate.Month, 1);
                    result.ToDate = new DateTime(lastDate.Year, lastDate.Month, GetMaxDayInMonth(lastDate.Year, lastDate.Month));
                    break;
                case DateOption.LastQuarter:
                    lastDate = refDate.AddMonths(-3);
                    quarter = GetQuarterByMonth(lastDate.Month);
                    startMonth = GetStartMonthInQuarter(quarter);
                    endMonth = GetEndMonthInQuarter(quarter);
                    result.FormDate = new DateTime(lastDate.Year, startMonth, 1);
                    result.ToDate = new DateTime(lastDate.Year, endMonth, GetMaxDayInMonth(lastDate.Year, endMonth));
                    break;
                case DateOption.LastYear:
                    lastDate = refDate.AddYears(-1);
                    result.FormDate = new DateTime(lastDate.Year, 1, 1);
                    result.ToDate = new DateTime(lastDate.Year, 12, 31);
                    break;
                case DateOption.ThisMonthInLastYear:
                    result.FormDate = new DateTime(refDate.Year - 1, refDate.Month, 1);
                    result.ToDate = new DateTime(refDate.Year - 1, refDate.Month, GetMaxDayInMonth(refDate.Year - 1, refDate.Month));
                    break;
                case DateOption.ThisQuarterInLastYear:
                    lastDate = refDate.AddYears(-1);
                    quarter = GetQuarterByMonth(lastDate.Month);
                    startMonth = GetStartMonthInQuarter(quarter);
                    endMonth = GetEndMonthInQuarter(quarter);
                    result.FormDate = new DateTime(lastDate.Year, startMonth, 1);
                    result.ToDate = new DateTime(lastDate.Year, endMonth, GetMaxDayInMonth(lastDate.Year, endMonth));
                    break;
                default:
                    result.FormDate = refDate;
                    result.ToDate = refDate;
                    break;
            }
            result.FormDate = new DateTime(result.FormDate.Year, result.FormDate.Month, result.FormDate.Day, 0, 0, 0);
            result.ToDate = new DateTime(result.ToDate.Year, result.ToDate.Month, result.ToDate.Day, 23, 59, 59);
            return result;
        }

        #region Private Method
        private int GetQuarterByMonth(int month)
        {
            int quarter;
            if (month >= 1 && month <= 3)
            {
                quarter = 1;
            }
            else if (month >= 4 && month <= 6)
            {
                quarter = 2;
            }
            else if (month >= 7 && month <= 9)
            {
                quarter = 3;
            }
            else
            {
                quarter = 4;
            }
            return quarter;
        }

        private int GetStartMonthInQuarter(int quarter)
        {
            if (quarter > 0 && quarter <= 4)
            {
                return ((quarter - 1) * 3) + 1;
            }
            else
            {
                return 1;
            }
        }
        private int GetEndMonthInQuarter(int quarter)
        {
            if (quarter > 0 && quarter <= 4)
            {
                return quarter * 3;
            }
            else
            {
                return 1;
            }
        }
        /// <summary>
        /// get ��Ҩӹǹ�ѹ�٧�ش�ͧ��͹����ͧ���
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public int GetMaxDayInMonth(int year, int month)
        {
            DateTime startDateTime;
            startDateTime = new DateTime(year, month, 1);
            return (new DateTime(year, month, startDateTime.AddMonths(1).AddDays(-1).Day)).Day;
        }
        #endregion
        #endregion

        #endregion

        #region CalculateTime
        /// <summary>
        /// Calculate datetime.
        /// </summary>
        /// <returns>string value</returns>
        /// <example>
        /// 
        /// <code>
        ///  public string CalculateTime()
        ///  {
        ///    return new DataHelper().CalculateTime(startDate,endDate);
        ///  }
        /// </code>
        /// <c>Result</c><br/>
        /// 
        /// </example>
        /// <modify>Mr.Boonma Noijunvong 2006-08-14 13.20</modify>
        public static string CalculateTime(DateTime startDate, DateTime endDate)
        {
            string strValue = string.Empty;
            //Create Type DataSet
            DataSet tdsData = new DataSet();
            DataTable tbData = new DataTable("tbData");
            tdsData.Tables.Add(tbData);
            try
            {
                //Create database
                //DALHelper.CreateDatabaseManager();
                string strDate = DateTimeManager.ConvertDateTimeToDateString(startDate);
                string strEndDate = DateTimeManager.ConvertDateTimeToDateString(endDate);
                //call method for retrive data from database
//                DALHelper.ExecuteNonQuery("SELECT dbo.udf_DateDiff('" + startDate.ToString("yyyy/MM/dd") + "','" + endDate.ToString("yyyy/MM/dd") + "') As ResultValue", tdsData, "tbData");
                //DALHelper.ExecuteNonQuery("SELECT dbo.udf_DateDiff('" + strDate + "','" + strEndDate + "') As ResultValue", tdsData, "tbData");
                if (tdsData.Tables["tbData"].Rows.Count > 0)
                {
                    strValue = tdsData.Tables["tbData"].Rows[0]["ResultValue"].ToString();
                }
                ////Return data set
                return strValue;
            }
            catch (Exception ex)
            {
                //Add exception if select fails
                //ExceptionHelper.AddException(ExceptionLayer.DAL, ExceptionModule.Select, ExceptionSeverity.Error, ex.Message);
                return strValue;
            }
        }
        #endregion

        #region ResetServerDatetime
        /// <summary>
        /// �ӡ�� set ��� DateTime �ҡ����ͧ Server ������������ class
        /// </summary>
        public static void ResetServerDatetime()
        {
            #if(DEBUG)
            serverDate = DateTime.Now;
            #endif
        }
        #endregion


        #region GetServerDateTime
        /// <summary>
        /// used for get current DateTime from database server.
        /// </summary>
        /// <returns>datetime from database server.</returns>
        private static DateTime GetServerDateTime()
        {
            try
            {
                //don't create database manager or open connection and other.because this function used in scope of AbsFacade.
                DateTime serverDateTime = DateTime.Now;

                

                  
                #region GetServerDateTime
                using (var trans = new TransactionScope())
                {
                    using (SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["DB_ConnectionString"].ConnectionString))
                    {
                        sqlConn.Open();

                        #region Get Data
                        using (SqlCommand sqlComm = new SqlCommand("SELECT GETDATE() AS ServerDateTime", sqlConn))
                        {
                            SqlDataReader data = sqlComm.ExecuteReader();
                            while (data.Read())
                            {
                                serverDateTime = (DateTime)data["ServerDateTime"];
                            }
                            data.Close();
                        }
                        #endregion

                        sqlConn.Close();
                    }
                    trans.Complete();
                }
                #endregion

                return serverDateTime;
            }
            catch (Exception ex)
            {
                return DateTimeManager.ToDay;
            }
        }
        #endregion

        #region CalculateDate
        /// <summary>
        /// 
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static object CalculateDate(DateTime startDate, DateTime endDate)
        {
            object result = null;
            try
            {
                //Create database
                //DALHelper.CreateDatabaseManager();
                //Create command select
                //DALHelper.CreateSelectCommand("");
                ////Add parameter for select
                //DALHelper.AddInParameterSelectCommand("StartDate", DbType.DateTime, startDate);
                //DALHelper.AddInParameterSelectCommand("EndDate", DbType.DateTime, endDate);
                //call method for retrive data from database
               //// result = DALHelper.ExecuteNonQuery("udf_DateDiff", startDate, endDate);
                //Return data set
                return result;
            }
            catch (Exception ex)
            {
                //Add exception if select fails
             ////   ExceptionHelper.AddException(ExceptionLayer.DAL, ExceptionModule.Select, ExceptionSeverity.Error, ex.Message);
                return result;
            }
        }

        #endregion       
    }
    #endregion

    #region ApplicationManager
    /// <summary>
    /// Class for manage Application data
    /// </summary>
    public class ApplicationManager
    {
        #region Class Member        
        //private static string defaultLanguage = res.Config.Language;
        //private static string defaultLanguage = "en-US";
        private static string defaultLanguage = "th-TH";


        #region Number and Precision
        //Default Currency Amount Quantity.
        private static int qtyPrecesionDigit = 4;
        private static int amountPrecesionDigit = 3;
        private static int percentPrecesionDigit = 2;
        private static int floatingDigit = 12;
        private static int integerDigit = 9;
        private static string currencySymbol = "";
        #endregion //umber and Precision

        #region Regional and Exception Resource
        //Default CultureInfo and UICultureInfo.
        private static string uiCulture = "en-US";
        private static string cultureInfo = "en-US";
        //Default ERA, Cristian.
        private static ERA currentERA = ERA.Christian;
        private static string dateFormat = "dd/mm/yyyy";
        
        #endregion //Regional and Exception Resource
        
        #endregion               

        #region CultureInfo and UICultureInfo
        /// <summary>
        /// Get/set CultureInfo(format)
        /// </summary>
        /// <remarks></remarks>
        /// <example>
        /// 
        /// <code>
        /// ApplicationManager.CurrentLanguage = "th-TH";
        /// string str = ApplicationManager.CurrentLanguage;
        /// // or
        /// CultureInfoHelper.CultureInfo(ApplicationManager.CurrentLanguage); 
        /// </code>
        /// <c>Result</c><br/>
        /// str = th-TH
        /// </example>
        public static string DefaultLanguage
        {
            get { return defaultLanguage; }
        }
        public static string CurrentLanguage
        {
            get { return cultureInfo; }
            set { cultureInfo = value; }
        }
        /// <summary>
        /// Get/set ERA year type. 
        /// </summary>
        /// <remarks></remarks>
        /// <example>
        /// 
        /// <code>
        /// DateTimeManager dtHelper = new DateTimeManager(ApplicationManager.CurrentERA);
        /// </code>
        /// <c>Result</c><br/>
        /// Set ERA to DateTimeManager
        /// </example>				
        public static ERA CurrentERA
        {
            get { return currentERA; }
            set { currentERA = value; }
        }
        /// <summary>
        /// Used for ERA year type
        /// </summary>
        public enum ERA
        {
            Buddhist = '1',
            Christian = '2',
        }
        /// <summary>
        /// Get/set current language 
        /// </summary>
        /// <remarks></remarks>
        /// <example>
        /// 
        /// <code>
        /// ApplicationManager.Language = "basLanguageEng";
        /// </code>
        /// <c>Result</c><br/>
        /// Set language to ApplicationManager
        /// </example>				
        public static string Language
        {
            get
            {
                return "basLanguageEng";
            }
            set
            {
                //read language for user 

            }
        }

        /// <summary>
        /// used for get and set current date format
        /// </summary>
        /// <remarks></remarks>
        /// <example>
        /// 
        /// <code>
        /// ApplicationManager.DateFormat = "dd/mm/yyyy";
        /// string str = ApplicationManager.DateFormat;
        /// </code>
        /// <c>Result</c><br/>
        /// str = dd/MM/yyyy
        /// </example>				
        public static string DateFormat
        {
            get
            {
                return dateFormat.Replace("mm", "MM");
            }
            set
            {
                dateFormat = value;
            }
        }

        /// <summary>
        /// Get/set current time format
        /// </summary>
        /// <remarks></remarks>
        /// <example>
        /// 
        /// <code>
        /// ApplicationManager.TimeFormat = "hh:mm:ss tt";
        /// string str = ApplicationManager.TimeFormat;
        /// </code>
        /// <c>Result</c><br/>
        /// str = hh:mm:ss tt
        /// </example>				
        public static string TimeFormat
        {
            get
            {
                return "hh:mm:ss tt";
            }
            set
            {
            }
        }

        /// <summary>
        /// Get/set default floating digit
        /// </summary>
        /// <remarks></remarks>
        /// <example>
        /// 
        /// <code>
        /// ApplicationManager.FloatingDigit = 4;
        /// int value = ApplicationManager.FloatingDigit;
        /// </code>
        /// <c>Result</c><br/>
        /// value = 4
        /// </example>				
        public static int FloatingDigit
        {
            get { return floatingDigit; }
            set { floatingDigit = value; }
        }

        /// <summary>
        /// Get/set quantity precision digit	
        /// </summary>
        /// <remarks></remarks>
        /// <example>
        /// 
        /// <code>
        /// ApplicationManager.QuantityPrecisionDigit = 5;
        /// int value = ApplicationManager.QuantityPrecisionDigit;
        /// </code>
        /// <c>Result</c><br/>
        /// value = 5
        /// </example>				
        public static int QuantityPrecisionDigit
        {
            get { return qtyPrecesionDigit; }
            set { qtyPrecesionDigit = value; }
        }

        /// <summary>
        /// Get/set amount precision digit 		
        /// </summary>
        /// <remarks></remarks>
        /// <example>
        /// 
        /// <code>
        /// ApplicationManager.AmountPrecisionDigit = 6;
        /// int value = ApplicationManager.AmountPrecisionDigit;
        /// </code>
        /// <c>Result</c><br/>
        /// value = 6
        /// </example>				
        public static int AmountPrecisionDigit
        {
            get { return amountPrecesionDigit; }
            set { amountPrecesionDigit = value; }
        }


        /// <summary>
        /// Get/set percentage precision digit		
        /// </summary>
        /// <remarks></remarks>
        /// <example>
        /// 
        /// <code>
        /// ApplicationManager.PercentPrecisionDigit = 2;
        /// int value = ApplicationManager.PercentPrecisionDigit;
        /// </code>
        /// <c>Result</c><br/>
        /// value = 2
        /// </example>				
        public static int PercentPrecisionDigit
        {
            get { return percentPrecesionDigit; }
            set { percentPrecesionDigit = value; }
        }

        /// <summary>
        /// Get/set integer digit		
        /// </summary>
        /// <remarks></remarks>
        /// <example>
        /// 
        /// <code>
        /// ApplicationManager.IntegerDigit = 3;
        /// int value = ApplicationManager.IntegerDigit;
        /// </code>
        /// <c>Result</c><br/>
        /// value = 3
        /// </example>				
        public static int IntegerDigit
        {
            get { return integerDigit; }
            set { integerDigit = value; }
        }

        /// <summary>
        /// Get/set currency symbole		
        /// </summary>
        /// <remarks></remarks>
        /// <example>
        /// 
        /// <code>
        /// //cultureinfo is en-EN
        /// strng str = AppilcationHelper.CurrencySymbol;
        /// </code>
        /// <c>Result</c><br/>
        /// str = $
        /// </example>				
        public static string CurrencySymbol
        {
            get { return currencySymbol; }
        }

        /// <summary>
        /// Get application datetime
        /// </summary>
        /// <remarks></remarks>
        /// <example>
        /// 
        /// <code>
        /// //today is 22/10/2005
        /// DateTime time = ApplicationManager.ToDay;
        /// </code>
        /// <c>Result</c><br/>
        /// time = 22/10/2005
        /// </example>				
        public static DateTime ToDay
        {
            get
            {
                return System.DateTime.Today;
            }
            set
            {
                //update today ŧ�ҹ������
            }

        }
        #endregion
                
        #region Method

        #region SetApplicationDateTimeFormatERA
        /// <summary>
        /// Change DateTime format by ERA
        /// </summary>
        /// <param name="era"></param>
        /// <returns></returns>
        public static bool SetApplicationDateTimeFormatERA(ERA era)
        {
            try
            {
                string culture = (era == ERA.Buddhist) ? "th-TH" : "en-US";

                #region Comment 20090915
                //System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat = new System.Globalization.CultureInfo(culture, true).DateTimeFormat;
                string longDatePattern = "dd MMMM yyyy";
                string fullDateTimePattern = "dd MMMM yyyy H:mm:ss";
                string monthDayPattern = "dd MMMM";
                string shortDatePattern = "dd/MM/yyyy";
                string yearMonthPattern = "MMMM yyyy";
                System.Globalization.DateTimeFormatInfo cultureFormat = new System.Globalization.CultureInfo(culture, false).DateTimeFormat;
                if (era == ERA.Buddhist)
                {
                    cultureFormat.ShortestDayNames = new string[] { "��", "�", "�", "�", "��", "�", "�" };
                }
                System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat = cultureFormat;
                System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.AbbreviatedDayNames = cultureFormat.AbbreviatedDayNames;
                System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.AbbreviatedMonthGenitiveNames = cultureFormat.AbbreviatedMonthGenitiveNames;
                System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.AbbreviatedMonthNames = cultureFormat.AbbreviatedMonthNames;
                System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.DayNames = cultureFormat.DayNames;
                System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.FirstDayOfWeek = cultureFormat.FirstDayOfWeek;
                System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.FullDateTimePattern = fullDateTimePattern;//cultureFormat.FullDateTimePattern
                System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.LongDatePattern = longDatePattern;//cultureFormat.LongDatePattern
                System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.LongTimePattern = cultureFormat.LongTimePattern;
                System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.MonthDayPattern = monthDayPattern;//cultureFormat.MonthDayPattern
                System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.MonthGenitiveNames = cultureFormat.MonthGenitiveNames;
                System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.MonthNames = cultureFormat.MonthNames;
                System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern = shortDatePattern;
                System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortestDayNames = cultureFormat.ShortestDayNames;
                System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortTimePattern = cultureFormat.ShortTimePattern;
                System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.TimeSeparator = cultureFormat.TimeSeparator;
                System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.YearMonthPattern = yearMonthPattern;// cultureFormat.YearMonthPattern;                
                #endregion
                
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion SetApplicationDateTimeFormatERA

        #endregion Method
    }
    #endregion ApplicationManager
}