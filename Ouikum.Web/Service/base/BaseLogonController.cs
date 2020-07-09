#region using System
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Web.UI;
using System.Drawing;                       // Drawing Image
using System.Text;                          // StringBuilder
using System.IO;                            // StringWriter
using System.Text.RegularExpressions;       // Regex
//using System.Runtime.Caching;               // CacheFC
using System.Configuration;
using System.Collections;
using System.Security.Cryptography;
using System.Net.NetworkInformation;        // MacAddress
using System.Net;                           // GetIP4Address()
#endregion
using Prosoft.Base;
using Prosoft.Service;
using System.Threading;
using System.Globalization;
using Ouikum;
using Ouikum.Common;
using res = Prosoft.Resource.Web.Ouikum;

namespace System.Web.Mvc
{

    public partial class BaseController : BaseClassController
    {
        public AuthenticationService svAuthentication;
        public string RememberAppName;
        public string AppName;

        #region Member ,Login
        #region delCheckCaptcha
        public void delCheckCaptcha()
        {
            HttpCookie delCheckCaptcha = new HttpCookie("CheckCaptchaSession");
            delCheckCaptcha.Expires = DateTime.Now.AddMinutes(-5);
            delCheckCaptcha.Value = null;
            //System.Web.HttpContext.Current.Response.Cookies.Add(delCheckCaptcha);
        }
        #endregion

        #region AuthenticationService
        public class AuthenticationService
        {
            #region Member
            public string AppName;
            //EncryptManager encryptManager;

            #endregion

            #region Constructor
            public AuthenticationService()
            {
                AppName = res.Common.lblWebsite;
                //encryptManager = new EncryptManager();
            }
            #endregion

            #region GetCookieAuthentication
            public Hashtable GetCookieAuthentication()
            {
                Hashtable htAuthentication = new Hashtable();
                HttpCookie ckAuthentication = System.Web.HttpContext.Current.Request.Cookies[AppName];
                if (ckAuthentication != null)
                {
                    #region
                    System.Collections.Specialized.NameValueCollection authenticationCkCollection = ckAuthentication.Values;
                    htAuthentication.Add("MemberID", System.Web.HttpContext.Current.Server.HtmlEncode(authenticationCkCollection["MemberID"]));
                    htAuthentication.Add("UserName", System.Web.HttpContext.Current.Server.HtmlEncode(authenticationCkCollection["UserName"]));
                    htAuthentication.Add("Password", System.Web.HttpContext.Current.Server.HtmlEncode(authenticationCkCollection["Password"]));
                    htAuthentication.Add("IsRemember", System.Web.HttpContext.Current.Server.HtmlEncode(authenticationCkCollection["IsRemember"]));
                    htAuthentication.Add("ServiceType", System.Web.HttpContext.Current.Server.HtmlEncode(authenticationCkCollection["ServiceType"]));
                    htAuthentication.Add("DisplayName", System.Web.HttpContext.Current.Server.HtmlEncode(authenticationCkCollection["DisplayName"]));
                    htAuthentication.Add("Email", System.Web.HttpContext.Current.Server.HtmlEncode(authenticationCkCollection["Email"]));
                    htAuthentication.Add("CompID", System.Web.HttpContext.Current.Server.HtmlEncode(authenticationCkCollection["CompID"]));
                    htAuthentication.Add("CompLevel", System.Web.HttpContext.Current.Server.HtmlEncode(authenticationCkCollection["CompLevel"]));
                    htAuthentication.Add("CompCode", System.Web.HttpContext.Current.Server.HtmlEncode(authenticationCkCollection["CompCode"]));
                    htAuthentication.Add("emCompID", System.Web.HttpContext.Current.Server.HtmlEncode(authenticationCkCollection["emCompID"]));
                    htAuthentication.Add("LogoImgPath", System.Web.HttpContext.Current.Server.HtmlEncode(authenticationCkCollection["LogoImgPath"]));
                    #endregion
                }
                return htAuthentication;
            }
            #endregion

            #region RemoveCookieAuthentication
            public void RemoveCookieAuthentication()
            {
                HttpCookie ckAuthentication = new HttpCookie(AppName);
                ckAuthentication.Expires = DateTime.Now.AddDays(-1);
                ckAuthentication.Value = "";
                System.Web.HttpContext.Current.Response.Cookies.Add(ckAuthentication);
            }
            #endregion
        }
        #endregion

        #region Logon Member

        protected int LogonMemberType
        {
            get
            {
                if (System.Web.HttpContext.Current.Request.Cookies.AllKeys.Contains(res.Common.lblWebsite))
                {
                    var svMember = new MemberService();
                    //var query = svMember.SelectData<view_emCompanyMember>("MemberID", "UserName ='"+username+"' and Password ='"+password+"' and WebID ="+ res.Config.WebID,null);
                    var query = svMember.SelectData<view_emCompanyMember>("MemberType", "MemberID =" + DataManager.ConvertToInteger(System.Web.HttpContext.Current.Request.Cookies[res.Common.lblWebsite].Values["MemberID"]), null);
                    IEnumerable<view_emCompanyMember> list = query.ToList();
                    if (list.Count() > 0)
                    {
                        return (int)list.First().MemberType;
                    }
                    else return 0;
                }
                else
                {
                    return 0;
                }
            }
        }
        protected int LogonMemberID
        {
            get
            {
                if (System.Web.HttpContext.Current.Request.Cookies.AllKeys.Contains(res.Common.lblWebsite))
                {
                    return DataManager.ConvertToInteger(System.Web.HttpContext.Current.Request.Cookies[res.Common.lblWebsite].Values["MemberID"]);
                }
                else
                {
                    return 0;
                }
            }

        }
        protected int LogonCompID
        {
            get
            {
                if (System.Web.HttpContext.Current.Request.Cookies.AllKeys.Contains(res.Common.lblWebsite))
                {
                    return DataManager.ConvertToInteger(System.Web.HttpContext.Current.Request.Cookies[res.Common.lblWebsite].Values["CompID"]);
                }
                else
                {
                    return 0;
                }
            }

        }
        protected int LogonCompLevel
        {
            get
            {
                if (System.Web.HttpContext.Current.Request.Cookies.AllKeys.Contains(res.Common.lblWebsite))
                {
                    return DataManager.ConvertToInteger(System.Web.HttpContext.Current.Request.Cookies[res.Common.lblWebsite].Values["CompLevel"]);
                }
                else
                {
                    return 0;
                }
            }

        }
        protected string LogonLogoImgPath
        {
            get
            {
                if (System.Web.HttpContext.Current.Request.Cookies.AllKeys.Contains(res.Common.lblWebsite))
                {
                    return DataManager.ConvertToString(Request.Cookies[res.Common.lblWebsite].Values["LogoImgPath"]);
                }
                else
                {
                    return String.Empty;
                }
            }

        }
        protected int LogonEMCompID
        {
            get
            {
                if (System.Web.HttpContext.Current.Request.Cookies.AllKeys.Contains(res.Common.lblWebsite))
                {
                    return DataManager.ConvertToInteger(System.Web.HttpContext.Current.Request.Cookies[res.Common.lblWebsite].Values["emCompID"]);
                }
                else
                {
                    return 0;
                }
            }

        }
        protected int LogonServiceType
        {
            get
            {
                if (System.Web.HttpContext.Current.Request.Cookies.AllKeys.Contains(res.Common.lblWebsite))
                {
                    return DataManager.ConvertToInteger(System.Web.HttpContext.Current.Request.Cookies[res.Common.lblWebsite].Values["ServiceType"]);
                }
                else
                {
                    return 0;
                }
            }
        }
        public string LogonDisplayName
        {
            get
            {
                if (System.Web.HttpContext.Current.Request.Cookies.AllKeys.Contains(res.Common.lblWebsite))
                {
                    return DecryptText("&%#@?,:*", Request.Cookies[res.Common.lblWebsite].Values["DisplayName"]);
                }
                else
                {
                    return String.Empty;
                }
            }
        }
        public string LogonEmail
        {
            get
            {
                if (System.Web.HttpContext.Current.Request.Cookies.AllKeys.Contains(res.Common.lblWebsite))
                {
                    return DataManager.ConvertToString(Request.Cookies[res.Common.lblWebsite].Values["Email"]);
                }
                else
                {
                    return String.Empty;
                }
            }
        } 
        public string LogonCompName
        {
            get
            {
                if (System.Web.HttpContext.Current.Request.Cookies.AllKeys.Contains(res.Common.lblWebsite))
                { 

                    return  DecryptText("&%#@?,:*" , Request.Cookies[res.Common.lblWebsite].Values["CompName"]);
                }
                else
                {
                    return String.Empty;
                }
            }
        }
        public string LogonCompCode
        {
            get
            {
                if (System.Web.HttpContext.Current.Request.Cookies.AllKeys.Contains(res.Common.lblWebsite))
                {
                    return DataManager.ConvertToString(System.Web.HttpContext.Current.Request.Cookies[res.Common.lblWebsite].Values["CompCode"]);
                }
                else
                {
                    return String.Empty;
                }
            }
        }
        public int LogonRowFlag
        {
            get
            {
                if (System.Web.HttpContext.Current.Request.Cookies.AllKeys.Contains(res.Common.lblWebsite))
                {
                    return DataManager.ConvertToInteger(System.Web.HttpContext.Current.Request.Cookies[res.Common.lblWebsite].Values["RowFlag"]);
                }
                else
                {
                    return 0;
                }
            }
        }
        #endregion

        #region CheckIsLogin
        public bool CheckIsAdmin(int ServiceType = 9)
        {
            bool IsAdmin = false;
            int? MemberType = 0;
            MemberType = LogonMemberType != 0 ? LogonMemberType : GetMemberType();


            if (Request.Cookies.Count <= 0)
            {
                #region Get Cookie
                AuthenticationService svAuthentication = new AuthenticationService();
                Hashtable htCookieInfo = svAuthentication.GetCookieAuthentication();
                if (htCookieInfo.Count > 0)
                {
                    HttpCookie ck = new HttpCookie("MemberID");
                    ck.Value = Convert.ToString(htCookieInfo["MemberID"]);
                    ck.Expires = DateTime.Now.AddDays(1);
                    Request.Cookies.Add(ck);
                }
                else
                {
                    IsAdmin = false;
                }
                #endregion
                if (MemberType == 2)
                {
                    IsAdmin = true;
                }
            }
            else
            {
                if (MemberType == 2)
                {
                    IsAdmin = true;
                }
            } 

            if (LogonServiceType == 9)
            {
                IsAdmin = true;
            }
            else if (ServiceType == 9)
            {
                if (LogonServiceType == 9)
                    IsAdmin = true;
                else
                    IsAdmin = false;
            }
            else if (LogonServiceType == ServiceType)
            {
                IsAdmin = true;
            }          
            else
            {
                IsAdmin = false;
            }
            return IsAdmin;
        }

        public bool CheckIsLogin(int compid = 0)
        {
            bool IsLogin = false;
            AuthenticationService svAuthentication = new AuthenticationService();
            Hashtable htCookieInfo = svAuthentication.GetCookieAuthentication();
            if (htCookieInfo.Count > 0)
            {
                IsLogin = true;
            }
            if (compid > 0)
            {
                if (compid == LogonCompID)
                {
                    IsLogin = true;
                }
            }
            return IsLogin;
        }
        #endregion

        #region Authenticate
        public void Authenticate()
        {
            string action = System.Web.HttpContext.Current.Request.RequestContext.RouteData.GetRequiredString("action");
            if (CheckIsLogin() == false)
            {
                if (action == "PrepareAddProductNotLogin" || action == "SearchCategory" || action == "SaveProductImg")
                {
                    
                }
                else
                {
                    System.Web.HttpContext.Current.Response.Redirect(System.Configuration.ConfigurationManager.AppSettings["LoginPage"]);
                }
                
            }
        }
        #endregion

        #region AuthenticateLogon
        public void AuthenticateLogon()
        {
            //ถ้า Login อยู่ไม่ให้เข้าหน้า SignIn , Register ให้กลับไปหน้า Home
            if (CheckIsLogin() == true)
            {
                System.Web.HttpContext.Current.Response.Redirect("~/");
            }
        }
        #endregion

        #region GetMemberType
        public int GetMemberType()
        {
            int MemberType = 0;
            var svMember = new MemberService();
            var query = svMember.SelectData<view_emCompanyMember>("MemberType", "MemberID =" + DataManager.ConvertToInteger(System.Web.HttpContext.Current.Request.Cookies[res.Common.lblWebsite].Values["MemberID"]), null);
            IEnumerable<view_emCompanyMember> list = query.ToList();
            if (list.Count() > 0)
            {
                MemberType = (int)list.First().MemberType;
            }
            return MemberType;
        }
        #endregion



        public string EncryptText(string key, string text)
        {
            try
            {
                byte[] IV = { 18, 52, 86, 120, 144, 171, 205, 239 };

                byte[] byKey = System.Text.Encoding.UTF8.GetBytes(key.Substring(0, 8));
                byte[] InputByteArray = System.Text.Encoding.UTF8.GetBytes(text);
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(byKey, IV), CryptoStreamMode.Write);
                cs.Write(InputByteArray, 0, InputByteArray.Length);
                cs.FlushFinalBlock();
                return Convert.ToBase64String(ms.ToArray());
            }
            catch (Exception ex)
            {
                return "";
            }

        }

        public string DecryptText(string key, string text)
        {
            try
            {
                byte[] IV = { 18, 52, 86, 120, 144, 171, 205, 239 };
                byte[] inputByteArray = new byte[text.Length];

                byte[] byKey = System.Text.Encoding.UTF8.GetBytes(key.Substring(0, 8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                inputByteArray = Convert.FromBase64String(text);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(byKey, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                System.Text.Encoding encoding = System.Text.Encoding.UTF8;
                return encoding.GetString(ms.ToArray());
            }
            catch (Exception ex)
            {
                return "MyB2B";
            }
        }
        #endregion

        #region Process

        #region GetRememberLogon
        public Hashtable GetRememberLogon()
        {
            Hashtable htRemember = new Hashtable();
            HttpCookie ckRemember = System.Web.HttpContext.Current.Request.Cookies["RememberB2BThai"];
            if (ckRemember != null)
            {
                System.Collections.Specialized.NameValueCollection rememberCollection = ckRemember.Values;
                htRemember.Add("IsRemember", rememberCollection["IsRemember"]);
                htRemember.Add("RememberUserName", rememberCollection["RememberUserName"]);
                htRemember.Add("RememberPassword", rememberCollection["RememberPassword"]);
            }
            return htRemember;
        }
        #endregion

        #region RegisterSessionLogon
        public bool RegisterSessionLogon(IEnumerable<view_emCompanyMember> list)
        {
            if (list.Count() > 0)
            {
                //ตรวจสอบข้อมูล Cookie ว่ามีการ Login ค้างอยู่ในระบบหรือเปล่า ??
                 AuthenticationService svAuthentication = new AuthenticationService();
                Hashtable htAuthentication = svAuthentication.GetCookieAuthentication();
                if (htAuthentication.Count > 0)
                {
                    //ทำการ Clear ข้อมูลเดิมก่อน
                    UnRegisterSessionLogon(DataManager.ConvertToString(htAuthentication["SessionID"]), DataManager.ConvertToString(htAuthentication["MemberID"]));
                }
                //New SessionID
                string strSessionID = Guid.NewGuid().ToString();
                //Add Data To Cookie
                AddCookieAuthentication(list, strSessionID);
            }
            return true;
        }
        #endregion

        #endregion


        #region RegisterRememberLogon
        public void RegisterRememberLogon(IEnumerable<view_emCompanyMember> list, bool IsRemember)
        {
            Hashtable htRemember = GetRememberLogon();
            if (htRemember.Count > 0)
            {
                //ทำการ Clear ข้อมูลเดิมก่อน
                UnRegisterRememberLogon();
            }

            if (!IsRemember)//ถ้าไม่จำ User + Password
            {
                IsRemember = false; //กำหนดให้ไม่จำ 
                list.First().UserName = string.Empty;
            }
            else
            {
                //  Add ข้อมูล กรณีจำชื่อผู้ใช้
                HttpCookie ckRememberApp = new System.Web.HttpCookie(RememberAppName);
                ckRememberApp.Values["IsRemember"] = IsRemember.ToString();
                ckRememberApp.Values["RememberUserName"] = list.First().UserName;
                ckRememberApp.Values["RememberPassword"] = list.First().Password;

                ckRememberApp.Expires = DateTime.Now.AddYears(200);
                System.Web.HttpContext.Current.Response.Cookies.Add(ckRememberApp);
            }
        }
        #endregion

        #region UnRegisterSessionLogon
        public bool UnRegisterSessionLogon(string sessionID, string MemberID)
        {
            Hashtable htCookieInfo = new Hashtable();
            if (MemberID != null)
            {
                try
                {
                    svAuthentication.RemoveCookieAuthentication();
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }
        #endregion

        #region AddCookieAuthentication
        private void AddCookieAuthentication(IEnumerable<view_emCompanyMember> list, string SessionID)
        {
            Ouikum.Company.CompanyService svCompany = new Ouikum.Company.CompanyService();
            HttpCookie ckAuthentication = new HttpCookie(res.Common.lblWebsite);
            var member = list.First();
            var company = svCompany.SelectData<b2bCompany>("CompID,CompName,ServiceType,CompCode,CompLevel,LogoImgPath,DisplayName,RowFlag", "MemberID = " + member.MemberID).First();
            ckAuthentication.Values["SessionID"] = SessionID;
            ckAuthentication.Values["MemberID"] = Convert.ToString(member.MemberID);
            ckAuthentication.Values["UserName"] = member.UserName;
            ckAuthentication.Values["DisplayName"] = EncryptText("&%#@?,:*", company.DisplayName);
            ckAuthentication.Values["Email"] = member.Email;
            ckAuthentication.Values["emCompID"] = Convert.ToString(member.CompID);
            ckAuthentication.Values["CompID"] = Convert.ToString(company.CompID);
            ckAuthentication.Values["ServiceType"] = Convert.ToString(company.ServiceType);
            ckAuthentication.Values["CompCode"] = Convert.ToString(company.CompCode);
            ckAuthentication.Values["CompLevel"] = Convert.ToString(company.CompLevel);
            ckAuthentication.Values["CompName"] = EncryptText("&%#@?,:*", company.CompName);
            ckAuthentication.Values["LogoImgPath"] = company.LogoImgPath;
            ckAuthentication.Values["RowFlag"] = Convert.ToString(company.RowFlag);

            ckAuthentication.Expires = DateTime.Now.AddDays(1);

            System.Web.HttpContext.Current.Response.Cookies.Add(ckAuthentication);
        }
        #endregion

        #region UnRegisterRememberLogon
        public void UnRegisterRememberLogon()
        {
            HttpCookie ckRemember = new HttpCookie(RememberAppName);
            ckRemember.Expires = DateTime.Now.AddDays(-1);
            ckRemember.Value = "";
            System.Web.HttpContext.Current.Response.Cookies.Add(ckRemember);
        }
        #endregion
         


    }

    public class BaseSecurityController : BaseController
    {
        public BaseSecurityController()
            : base()
        {
            Authenticate();

            try
            {
                ViewBag.LogonMemberID = LogonMemberID;
            }
            catch
            {

            }
            
        }
    }

    public class BaseSecurityAdminController : BaseController
    {
        public BaseSecurityAdminController()
            : base()
        {


            Authenticate();
            int? MemberType = 0;
            MemberType = LogonMemberType != 0 ? LogonMemberType : GetMemberType();
            if (MemberType != 2)
            {
                System.Web.HttpContext.Current.Response.Redirect(System.Configuration.ConfigurationManager.AppSettings["AcessDenied"]);
            }
        }
    }
}