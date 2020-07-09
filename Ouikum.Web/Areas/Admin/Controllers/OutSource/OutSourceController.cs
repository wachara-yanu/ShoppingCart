using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ouikum.Common;
using Prosoft.Service;
using Ouikum;
using Ouikum.BizType;
using Ouikum.Company;
//using Prosoft.Base;
using System.Transactions;
using System.Configuration;
using System.Globalization;
using System.Web.ApplicationServices;
using System.Collections;
using System.Text.RegularExpressions;
using res = Prosoft.Resource.Web.Ouikum;

namespace Ouikum.Web.Admin
{
    public class OutSourceController : BaseController
    {
        #region Members
        //
        // GET: /Member/ 
        BizTypeService svBizType;
        MemberService svMember;
        AddressService svAddress;
        emCompanyService svEmcompany;
        CompanyService svCompany;
        WebService svWeb;
        // ----- signin--- //
        AuthenticationService svAuthentication;
        public string AppName;
        string RememberAppName;
        EmailManager emailManager = null;
        Mail mail = null;
        #endregion

        #region Constructors
        public OutSourceController ()
	    {
            svBizType = new BizTypeService();
            svMember = new MemberService();
            svAddress = new AddressService();
            svEmcompany = new emCompanyService();
            svCompany = new CompanyService();
            svWeb = new WebService();

            AppName = res.Common.lblWebsite;
            RememberAppName = string.Concat("Remember", AppName);
            svAuthentication = new AuthenticationService();
            emailManager = new EmailManager(res.Config.SMTP_Server, res.Config.SMTP_UserName, res.Config.SMTP_Password, Convert.ToBoolean(res.Config.SMTP_IsAuthentication));
            
                mail = new Ouikum.Common.Mail();

	    }
        #endregion

        #region SignIn
        public ActionResult SignIn()
        {     
            AuthenticateLogon();

            bool IsRemember = false;
            Hashtable htRemember = GetRememberLogon();
            EncryptManager encrypt = new EncryptManager();
            if (htRemember.Count > 0)
            {
                if (htRemember.ContainsKey("IsRemember"))
                    bool.TryParse(htRemember["IsRemember"].ToString(), out IsRemember);

                ViewBag.IsRemember = IsRemember;
                ViewBag.UserName = Server.HtmlDecode(htRemember["RememberUserName"].ToString());
                if (htRemember["RememberPassword"] != null)
                {
                    ViewBag.Password = encrypt.DecryptData(Server.HtmlDecode(htRemember["RememberPassword"].ToString()));
                }
            }           

            var Webs = svWeb.SelectData<emWeb>("WebID,WebName", "Isdelete = 0 and WebID =" + res.Config.WebID);
            ViewBag.Web = Webs.ToList();

            return View();
        }
        [HttpPost]
        public ActionResult SignIn(string username, string password, string remember, string admincode)
        {
            EncryptManager encrypt = new EncryptManager();
            var Url = new UrlHelper(System.Web.HttpContext.Current.Request.RequestContext);

            #region Check Admin
            var svCompany = new CompanyService();
            var sqlWhere= svCompany.CreateWhereAction(Company.CompStatus.Activate);
            sqlWhere += " AND CompCode = '"+admincode+ "' AND ServiceType = 14 ";
            var companies = svCompany.SelectData<b2bCompany>(" * ", sqlWhere);
            var company = new b2bCompany(); 
            var isOutSource = false;    

            if (svCompany.TotalRow > 0)
            {
                isOutSource = true;   
                foreach (var it in companies)
                {
                    company.CompCode = it.CompCode;
                    company.DisplayName = it.DisplayName;
                    company.ServiceType = it.ServiceType;
                }
            }
            else
            {
                return Json(new { IsSuccess = false, Result = false });
            }
            #endregion
             
            var query = svMember.SelectData<view_emCompanyMember>("MemberID,UserName,Password,DisplayName,Email,CountLogin,CompID", " (UserName ='" + username + "' or Email ='" + username + "') and Password ='" + encrypt.EncryptData(password) + "' and RowFlagWeb = 2 and IsDelete = 0 and WebID =" + res.Config.WebID, null);
            List<view_emCompanyMember> list = query;
            if (svMember.TotalRow > 0)
            {
                var data = query.First();

                data.CountLogin = data.CountLogin != null ? data.CountLogin : 0;
                if (data.CountLogin < 500)
                {
                    #region
                    RegisterSessionLogon(list,company, isOutSource  );
                    RegisterRememberLogon(list, Convert.ToBoolean(remember));

                    if (data.CountLogin != 0)
                    {
                        //Update Count LogOn 
                        if (!svMember.UpdateByCondition<emMemberWeb>("CountLogin = 0", "MemberID =" + data.MemberID))
                        {
                            return Json(new { IsSuccess = false, Result = "เกิดข้อผิดพลาด กรุณาลองอีกครั้ง" });
                        }
                        else
                            return Json(new { IsSuccess = false, Result = "เกิดข้อผิดพลาด กรุณาลองอีกครั้ง" });
                    }
                    else
                    {
                        #region Redirect to HomePage
                        return Json(new { IsSuccess = true, Result = Url.Action("Index","Home") });
                        #endregion
                    }
                    #endregion
                }
                else
                {
                    return Json(new { IsSuccess = false, Result = Url.Content("คุณเข้าสู่ระบบไม่ผ่าน 5 ครั้ง บัญชีของคุณถูกปิดเพื่อความปลอดภัย กรุณาตรวจสอบอีเมลล์เพื่อยืนยันตัวตน") });
                }
            }
            else
            {
                #region
                query = svMember.SelectData<view_emCompanyMember>("MemberID,CountLogin,FirstName,LastName,Email,CompID", " (UserName ='" + username + "' or Email ='" + username + "') and RowFlagWeb > 1 and WebID =" + res.Config.WebID, null);
                IEnumerable<view_emCompanyMember> listUser = query.ToList();
                if (listUser.Count() > 0)
                {
                    listUser.First().CountLogin = listUser.First().CountLogin != null ? listUser.First().CountLogin : 0;
                    if (listUser.First().CountLogin < 500)
                    {
                        #region
                        if ((4 - listUser.First().CountLogin) == 0)
                        {
                            if (!svMember.UpdateByCondition<emMemberWeb>("CountLogin = 500", "MemberID =" + listUser.First().MemberID))
                            {
                                return Json(new { IsSuccess = false, Result = Url.Content("เกิดข้อผิดพลาด กรุณาลองอีกครั้ง") });
                            }
                            else
                            {
                                if (!svMember.UpdateByCondition<emMemberActivate>("StartDate ='" + DateTime.Now + "',ExpireDate ='" + DateTime.Now.AddHours(24) + "',ActivateType = 3, ActivateCode = '" + emailManager.GenActivateCode() + "'", "MemberID =" + listUser.First().MemberID))
                                {
                                    return Json(new { IsSuccess = false, Result = Url.Content("เกิดข้อผิดพลาด กรุณาลองอีกครั้ง") });
                                }
                                else
                                {

                                    //if (!SendEmail(listUser))
                                    //{
                                       return Json(new { IsSuccess = false, Result = Url.Content("เกิดข้อผิดพลาด 5 ครั้ง") });
                                    //}
                                    //else
                                    //    return Json(new { IsSuccess = false, Result = Url.Content("คุณเข้าสู่ระบบไม่ผ่าน 5 ครั้ง บัญชีของคุณถูกปิดเพื่อความปลอดภัย กรุณาตรวจสอบอีเมลล์เพื่อยืนยันตัวตน") });
                                }
                            }
                        }
                        else
                        {
                            int count = (int)listUser.First().CountLogin + 1;
                            if (!svMember.UpdateByCondition<emMemberWeb>("CountLogin = " + count, "MemberID =" + listUser.First().MemberID))
                            {
                                return Json(new { IsSuccess = false, Result = Url.Content("เกิดข้อผิดพลาด กรุณาลองอีกครั้ง") });
                            }
                            else
                            {
                                return Json(new { IsSuccess = false, Result = Url.Content("รหัสผ่านไม่ถูกต้อง คุณสามารถลงชื่อเข้าใช้ได้อีก " + (5 - count) + " ครั้ง") });
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        return Json(new { IsSuccess = false, Result = Url.Content("คุณเข้าสู่ระบบไม่ผ่าน 5 ครั้ง บัญชีของคุณถูกปิดเพื่อความปลอดภัย กรุณาตรวจสอบอีเมลล์เพื่อยืนยันตัวตน") });
                    }
                }
                #endregion
            } 

            return Json(new { IsSuccess = false, Result = Url.Content("ชื่อผู้ใช้ หรือ Eamil หรือ รหัสผ่านไม่ถูกต้อง") });

        }
        #endregion
         
        #region SignOut
        public ActionResult SignOut()
        {
            AuthenticationService authenticationSV = new AuthenticationService();
            Hashtable htAuthentication = authenticationSV.GetCookieAuthentication();
            string LastURLAdmin = string.Empty;
            string LastURLEmployer = string.Empty;
            string LastURLSeeker = string.Empty;
            if (Session["LastURLAdmin"] != null)
            {
                LastURLAdmin = Session["LastURLAdmin"].ToString();
            }
            //else if (Session["LastURLEmployer"] != null)
            //{
            //    LastURLEmployer = Session["LastURLEmployer"].ToString();
            //}

            if (htAuthentication.Count > 0)
            {
                string SessionID = (htAuthentication.Contains("SessionID")) ? DataManager.ConvertToString(htAuthentication["SessionID"]) : string.Empty;
                string MemberID = (htAuthentication.Contains("MemberID")) ? htAuthentication["MemberID"].ToString() : string.Empty;
                UnRegisterSessionLogon(SessionID, MemberID);
            }
            #region
            Session.RemoveAll();
            Session["LastURLAdmin"] = LastURLAdmin;

            #endregion  
            
            string url = "/Home/Index";
            //if (Base.AppLang == "en-US")
            //    {
            //        url = Regex.Replace(url, "~/", "~/en/");
            //    }
                return Redirect(url);
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
        public bool RegisterSessionLogon(IEnumerable<view_emCompanyMember> list,b2bCompany model,bool IsOutSource = false)
        {
            if (list.Count() > 0)
            {
                //ตรวจสอบข้อมูล Cookie ว่ามีการ Login ค้างอยู่ในระบบหรือเปล่า ??
                Hashtable htAuthentication = svAuthentication.GetCookieAuthentication();
                if (htAuthentication.Count > 0)
                {
                    //ทำการ Clear ข้อมูลเดิมก่อน
                    UnRegisterSessionLogon(DataManager.ConvertToString(htAuthentication["SessionID"]), DataManager.ConvertToString(htAuthentication["MemberID"]));
                }
                //New SessionID
                string strSessionID = Guid.NewGuid().ToString();
                //Add Data To Cookie
                if (IsOutSource) 
                    AddCookieAuthentication(list,model, strSessionID); 
            }
            return true;
        }
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
        private void AddCookieAuthentication(IEnumerable<view_emCompanyMember> list,b2bCompany model, string SessionID)
        {
            HttpCookie ckAuthentication = new HttpCookie(AppName);
            var member = list.First();
            var company = svCompany.SelectData<b2bCompany>("CompID,CompName,ServiceType,CompCode,CompLevel,DisplayName", "MemberID = " + member.MemberID).First();
            ckAuthentication.Values["SessionID"] = SessionID;
            ckAuthentication.Values["MemberID"] = Convert.ToString(member.MemberID);
            ckAuthentication.Values["UserName"] = member.UserName;
            ckAuthentication.Values["DisplayName"] = EncryptText("&%#@?,:*", model.DisplayName);
            ckAuthentication.Values["Email"] =  member.Email ;
            ckAuthentication.Values["emCompID"] = Convert.ToString(member.CompID);
            ckAuthentication.Values["CompID"] = Convert.ToString(company.CompID);
            ckAuthentication.Values["ServiceType"] = Convert.ToString(model.ServiceType);
            ckAuthentication.Values["CompCode"] = Convert.ToString(model.CompCode);
            ckAuthentication.Values["CompLevel"] = Convert.ToString(company.CompLevel);
            ckAuthentication.Values["CompName"] = EncryptText("&%#@?,:*", company.CompName);
            
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
        #endregion

        /*------------------------- Other ------------------------*/

        #region CheckCaptcha
        [HttpPost]
        public int CheckCaptchaSession(string password)
        {
            var IsPass = 0;
            if (password == "B2BThaiCaptcha")
            {
                Response.Cookies["CheckCaptchaSession"].Value = res.Common.lblWebsite;
                Response.Cookies["CheckCaptchaSession"].Expires = DateTime.Now.AddMinutes(15);
                Session["CheckCaptchaSession"] = true;
                IsPass = 1;
            }
            else
            {
                IsPass = 0;
            }
            return IsPass;
        }
        #endregion

        #region Validation

        #region ValidateRegister
        public ActionResult ValidateRegister(string username ,string email, string compname,string compnameeng,string displayname)
        {
            var IsResult = true;
            var sqlSelect = "compid,memberid,username,email,compname";
            var sqlWhere = " and RowflagWeb = 2 and IsDelete = 0 and WebID = " + res.Config.WebID;

            if (!string.IsNullOrEmpty(username))
            {
                var Member = svMember.SelectData<view_emCompanyMember>(sqlSelect, "username = '" + username + "'"+sqlWhere);
                if (Member.Count() > 0)
                {
                    IsResult = false;
                }
            }

            if (!string.IsNullOrEmpty(email))
            {
                var Member = svMember.SelectData<view_emCompanyMember>(sqlSelect, "email = '" + email + "'" + sqlWhere);
                if (Member.Count() > 0)
                {
                    IsResult = false;
                }
            }
            if (!string.IsNullOrEmpty(compname))
            {
                var Member = svMember.SelectData<view_emCompanyMember>(sqlSelect, "compname = '" + compname + "'" + sqlWhere);
                if (Member.Count() > 0)
                {
                    IsResult = false;
                }
            }
            if (!string.IsNullOrEmpty(compnameeng))
            {
                var Member = svMember.SelectData<view_emCompanyMember>(sqlSelect, "compnameeng = '" + compnameeng + "'" + sqlWhere);
                if (Member.Count() > 0)
                {
                    IsResult = false;
                }
            }
            if (!string.IsNullOrEmpty(displayname))
            {
                var Company = svCompany.SelectData<b2bCompany>("compid,displayname", "displayname = '" + displayname + "' and Rowflag = 2 AND IsDelete = 0");
                if (Company.Count() > 0)
                {
                    IsResult = false;
                }
            }
            return Json(IsResult);
        }

        #endregion

        #region validatePassword
        public ActionResult validatePassword(string password)
        {
            EncryptManager encrypt = new EncryptManager();
            var member = new Ouikum.emMember();
            var IsResult = true;
            var sqlSelect = "compid,memberid,username,email,compname";
            var sqlWhere = " and RowflagWeb = 2 and IsDelete = 0 and WebID = " + res.Config.WebID;
            if (Request.Cookies[res.Common.lblWebsite] != null)
            {
                member = svMember.SelectData<emMember>("memberid,username,password", " MemberID = " + Request.Cookies[res.Common.lblWebsite].Values["MemberID"]).First();
            }
            if (!string.IsNullOrEmpty(password))
            {
                var Password = encrypt.EncryptData(password);
                var Member = svMember.SelectData<view_emCompanyMember>(sqlSelect, "username = '" + member.UserName + "' and Password = '" + Password + "'" + sqlWhere);
                if (Member.Count() > 0)
                {
                    IsResult = false;
                }
            }
            return Json(IsResult);
        }
        #endregion

        #region ValidatePasswordLogin
        public ActionResult ValidatePasswordLogin(string username, string password)
        {
            EncryptManager encrypt = new EncryptManager();
            var IsResult = true;
            var TypeError = 0;
            var sqlSelect = "compid,memberid,username,email,compname";
            var sqlWhere = " and RowflagWeb = 2 and IsDelete = 0 and WebID = 1";

            var member = svMember.SelectData<view_emCompanyMember>("memberid,username,password", " UserName = '" + username + "'" + sqlWhere);
            if (member.Count() > 0)
            {
                if (!string.IsNullOrEmpty(password))
                {
                    var Password = encrypt.EncryptData(password);
                    var Member = svMember.SelectData<view_emCompanyMember>(sqlSelect, "username = '" + username + "' and Password = '" + Password + "'" + sqlWhere);
                    if (Member.Count() == 0)
                    {
                        IsResult = false;
                        TypeError = 1;//password
                    }
                }
            }
            else {
                IsResult = false;
                TypeError = 2;//user
            }
            return Json(new { IsResult = IsResult, TypeError = TypeError });
        }
        #endregion

        #endregion
    }
}