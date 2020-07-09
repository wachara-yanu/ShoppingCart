using System;
using System.Web;

namespace Prosoft.Service
{
    public class SessionManager
    {
        #region Constructor
        /// <summary>
        /// สร้างตัวแปร Session และ เก็บข้อมูล
        /// </summary>
        /// <param name="SessionName">ชื่อ Session</param>
        /// <param name="Object">ข้อมูลที่ต้องการเก็บ</param>
        public SessionManager()
        {

        }

        public SessionManager(string SessionName, object Object)
        {
            if (!string.IsNullOrEmpty(SessionName))
                HttpContext.Current.Session.Add(SessionName, Object);
        }
        #endregion

        #region CheckSession
        /// <summary>
        /// ตรวจสอบ Session
        /// </summary>
        /// <param name="SessionName">ชื่อ Session</param>
        /// <returns>มี หรือ ไม่มี ?</returns>
        public bool CheckSession(string SessionName)
        {
            bool isValid = false;
            if (HttpContext.Current.Session[SessionName] != null)
                isValid = true;
            return isValid;
        }
        #endregion

        #region GetSession
        /// <summary>
        /// Get Session
        /// </summary>
        /// <param name="SessionName">ชื่อ Session</param>
        /// <returns>ข้อมูลที่เก็บใน Session</returns>
        public object GetSession(string SessionName)
        {
            return GetSession(SessionName, false); ;
        }

        /// <summary>
        /// Get Session
        /// </summary>
        /// <param name="SessionName">ชื่อ Session</param>
        /// <param name="Delete">จะทำการลบ Session นี้เลย หรือไม่ ?</param>
        /// <returns>ข้อมูลที่เก็บใน Session</returns>
        public object GetSession(string SessionName, bool Delete)
        {
            object objSession = null;
            if (HttpContext.Current.Session[SessionName] != null)
            {
                objSession = HttpContext.Current.Session[SessionName];
                if (Delete)
                    HttpContext.Current.Session.Remove(SessionName);
            }
            return objSession;
        }
        #endregion

        #region AddSession
        /// <summary>
        /// สร้างตัวแปร Session และ เก็บข้อมูล
        /// </summary>
        /// <param name="SessionName">ชื่อ Session</param>
        /// <param name="Object">ข้อมูลที่ต้องการเก็บ</param>
        public void AddSession(string SessionName, object Object)
        {
            if (!string.IsNullOrEmpty(SessionName))
                HttpContext.Current.Session.Add(SessionName, Object);
        }

        /// <summary>
        /// สร้างตัวแปร Session และ เก็บข้อมูล
        /// </summary>
        /// <param name="SessionName">ชื่อ Session</param>
        /// <param name="Object">ข้อมูลที่ต้องการเก็บ</param>
        /// <param name="SessionID">SessionID</param>
        public void AddSession(string SessionName, object Object, ref string SessionID)
        {
            string strSessionID = string.Empty;
            if (!string.IsNullOrEmpty(SessionName))
            {
                HttpContext.Current.Session.Add(SessionName, Object);
                SessionID = HttpContext.Current.Session.SessionID;
            }
        }
        #endregion

        #region RemoveSession
        /// <summary>
        /// Remove Session
        /// </summary>
        /// <param name="SessionName">ชื่อ Session</param>
        public void RemoveSession(string SessionName)
        {
            if (HttpContext.Current.Session[SessionName] != null)
            {
                HttpContext.Current.Session.Remove(SessionName);
            }
        }

        /// <summary>
        /// Remove All
        /// </summary>
        public void RemoveAllSession()
        {
            HttpContext.Current.Session.RemoveAll();
        }
        #endregion
    }
}
