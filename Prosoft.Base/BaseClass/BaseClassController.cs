using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
//using System.Runtime.Caching;
using Prosoft.Service;
using System.IO;
using System.Web;

using System.Globalization;
using System.Web.UI;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Prosoft.Base
{
    public class BaseClassController : Controller
    {
        #region Member
        EncryptManager encrypt;
        string id = string.Empty;
        protected string strWhereDefault = "it.RowFlag > 0", strTab = string.Empty, strPartialViewString = string.Empty, strID = string.Empty, strMessage = "บันทึกสำเร็จ";
        protected int intObjectState = 0, intRowFlag = 1;
        public DateTime DateTimeNow
        {
            get
            {
                DateTime MyTime = DateTime.Now;            //Waktu Indonesia Bagian Barat
                DateTime AsiaTimeZone = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(MyTime, "SE Asia Standard Time");
                return AsiaTimeZone;
            }
        }
        
        #endregion

        #region GetAppSetting
        /// <summary>
        /// Get Application Setting in Web.config file.
        /// </summary>
        /// <param name="key">Key of AppSettings</param>
        /// <returns>value of specified AppSetting</returns>
        public string GetAppSetting(string key)
        {
            return System.Configuration.ConfigurationManager.AppSettings[key];
        }
        #endregion

        #region Language SoGoodWeb & B2BThai
        /// <summary>
        /// Set/Get Session for CultureInfo
        /// Support : SoGoodWeb , B2BThai (LastUpdate : Sep10,2012)
        /// </summary>
        public CultureInfo Culture
        {
            get
            {
                if (System.Web.HttpContext.Current.Request.Cookies["Language"] == null)
                {
                    System.Web.HttpContext.Current.Response.Cookies["Language"].Value = "th-TH";
                    System.Web.HttpContext.Current.Response.Cookies["Language"].Expires = DateTime.Today.AddMonths(1);
                }
                return new CultureInfo(System.Web.HttpContext.Current.Request.Cookies["Language"].Value);
            }
            set
            {
                System.Web.HttpContext.Current.Response.Cookies["Language"].Value = value.ToString();
                System.Web.HttpContext.Current.Response.Cookies["Language"].Expires = DateTime.Today.AddMonths(1);
            }
        }
        public bool RemoveCookieCulture()
        {
            var IsResult = false;
            try
            {
                if (System.Web.HttpContext.Current.Request.Cookies["Language"] != null)
                {
                    System.Web.HttpContext.Current.Response.Cookies["Language"].Value = null;
                    System.Web.HttpContext.Current.Response.Cookies["Language"].Expires = DateTime.Now.AddDays(-1);
                }
                IsResult = true;
            }
            catch (Exception ex)
            {
                IsResult = false;
            }
            return IsResult;
        }

        public bool SaveCookieCulture(string culture)
        {
            var IsResult = false;
            try
            {
                //Prosoft.Base.ApplicationHelper.PrimaryLanguage = culture;
                System.Web.HttpContext.Current.Response.Cookies["Language"].Value = culture;
                System.Web.HttpContext.Current.Response.Cookies["Language"].Expires = DateTime.Now.AddYears(10);
                IsResult = true;
            }
            catch (Exception ex)
            {
                IsResult = false;
            }
            return IsResult;
        }
        public virtual void SetResourceCulture()
        {

        }
        #endregion

        #region PartialViewToString
        /// <summary>
        /// Convert PartialView เป็น String ส่งค่า html ให้ javascript
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        protected string PartialViewToString(string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.RouteData.GetRequiredString("action");

            ViewData.Model = model;

            using (StringWriter sw = new StringWriter())
            {

                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }

        /// <summary>
        /// Convert PartialView to HTMLString
        /// สามารถรับค่า ViewBag ทั้งหมดได้
        /// By : SoGoodWeb
        /// </summary>
        /// <param name="partialViewName"></param>
        /// <returns></returns>
        protected string PartialViewToString(string partialViewName)
        {
            ControllerContext context = this.ControllerContext;
            ViewDataDictionary viewData = this.ViewData;
            TempDataDictionary tempData = this.TempData;

            ViewEngineResult result = ViewEngines.Engines.FindPartialView(context, partialViewName);

            if (result.View != null)
            {
                StringBuilder sb = new StringBuilder();
                using (StringWriter sw = new StringWriter(sb))
                {
                    using (HtmlTextWriter output = new HtmlTextWriter(sw))
                    {
                        ViewContext viewContext = new ViewContext(context, result.View, viewData, tempData, output);
                        result.View.Render(viewContext, output);
                    }
                }

                return sb.ToString();
            }

            return String.Empty;

        }
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

        #region Encode
        /// <summary>
        /// convert ค่ากลับคืนปกติ 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string DeCode(string text)
        {

            //if (MemoryCache.Default["EncryptManager"] != null)
            //{
            //    encrypt = (EncryptManager)MemoryCache.Default["EncryptManager"];
            //}
            //else
            //{
            //    encrypt = new EncryptManager();
            //    MemoryCache.Default.Add("EncryptManager", encrypt, DateTime.Now.AddMinutes(5));
            //}
            return DataManager.ConvertToString(encrypt.DecryptData(DeCodeForUrl(text)), "");
        }
        /// <summary>
        /// convert ค่ากลับคืนปกติ ใช้กรณี retrun ค่าเป็น int
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public int DeCodeID(string ID)
        {

            //if (MemoryCache.Default["EncryptManager"] != null)
            //{
            //    encrypt = (EncryptManager)MemoryCache.Default["EncryptManager"];
            //}
            //else
            //{
            //    encrypt = new EncryptManager();
            //    MemoryCache.Default.Add("EncryptManager", encrypt, DateTime.Now.AddMinutes(5));
            //}
            encrypt = new EncryptManager();
            return DataManager.ConvertToInteger(encrypt.DecryptData(DeCodeForUrl(ID)), 0);
        }
        /// <summary>
        /// convert ค่ากลับคืนปกติ ใช้กรณี retrun ค่าเป็น string 1,2,3... ใช้กับคำสั่ง where column in (DeCodeID(ID))
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public string DeCodeID(List<string> ID)
        {

            foreach (var item in ID)
            {
                id += "," + DeCodeID(item).ToString();
            }
            id = id.Remove(0, 1);
            return id;
        }
        /// <summary>
        /// convert string เป็นสายอักขระพิเศษ
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string EnCode(string text)
        {
            //if (MemoryCache.Default["EncryptManager"] != null)
            //{
            //    encrypt = (EncryptManager)MemoryCache.Default["EncryptManager"];
            //}
            //else
            //{
            //    encrypt = new EncryptManager();
            //    MemoryCache.Default.Add("EncryptManager", encrypt, DateTime.Now.AddMinutes(5));
            //}
            encrypt = new EncryptManager();
            return EnCodeForUrl(DataManager.ConvertToString(encrypt.EncryptData(text), ""));
        }
        /// <summary>
        /// convert int เป็นสายอักขระพิเศษ
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string EnCodeID(int ID)
        {

            return EnCode(ID.ToString());
        }

        /// <summary>
        /// กรณีส่งค่าผ่าน Url
        /// </summary>
        /// <param name="TextEnCode"></param>
        /// <returns></returns>
        public string EnCodeForUrl(string Text)
        {
            Text = Text.Replace("/", "4869dba4869");
            return Text;
        }

        /// <summary>
        /// กรณีส่งค่าผ่าน Url
        /// </summary>
        /// <param name="TextEnCode"></param>
        /// <returns></returns>
        public string DeCodeForUrl(string TextEnCode)
        {
            TextEnCode = TextEnCode.Replace("4869dba4869", "/");
            return TextEnCode;
        }
        #endregion

        #region MemoryCache
        /// <summary>
        /// ตรวจสอบว่ามี MemoryCache ชื่อนั้นๆอยู่หรือไม่
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        //public bool MemoryCacheContains(string name)
        //{
        //    return MemoryCache.Default[name] == null ? false : true;
        //}
        /// <summary>
        /// สร้าง MemoryCache ชื่อ name ด้วยข้อมูล Object
        /// </summary>
        /// <param name="name"></param>
        /// <param name="Object"></param>
        //public object MemoryCacheAdd(string name, object Object)
        //{
        //    MemoryCache.Default.Add(name, Object, DateTime.Now.AddMinutes(5));
        //    return MemoryCacheGet(name);

        //}
        /// <summary>
        /// รียกใช้งาน MemoryCache ชื่อ name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        //public object MemoryCacheGet(string name)
        //{
        //    return MemoryCache.Default[name];
        //}
        #endregion 

    
    }
}
