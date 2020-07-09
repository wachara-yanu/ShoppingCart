using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ouikum.Common;
using Ouikum.BizType;
using Ouikum.Company;
using Prosoft.Service;
using Ouikum;
//using Prosoft.Base;
using System.Transactions;
using System.Configuration;
using System.Globalization;
using System.Web.ApplicationServices;
using System.Collections;
using System.Text;
using res = Prosoft.Resource.Web.Ouikum;
using System.Web.Security;
using Ouikum.Web.Models;
using System.Text.RegularExpressions;
using Ouikum.Message;
using Ouikum.Quotation;
using Ouikum.Order;

namespace Ouikum.Web.Controllers
{
    public class MemberController : BaseController
    {
        #region Members
        //
        // GET: /Member/ 
        BizTypeService svBizType;
        MemberService svMember;
        PackageService svPackage;
        OrderService svOrder;
        AddressService svAddress;
        emCompanyService svEmcompany;
        CompanyService svCompany;
        WebService svWeb;
        // ----- signin--- //
        EmailManager emailManager = null;
        Mail mail = null;
        #endregion

        #region Constructors
        public MemberController ()
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
            //if (MemoryCacheContains("EmailManager"))
            //{
            //    emailManager = (EmailManager)MemoryCacheGet("EmailManager");
            //}
            //else
            //{
            emailManager = new EmailManager(res.Config.SMTP_Server, res.Config.SMTP_UserName, res.Config.SMTP_Password, Convert.ToBoolean(res.Config.SMTP_IsAuthentication));
            //    MemoryCacheAdd("EmailManager", emailManager);
            //}
            //if (MemoryCacheContains("Mail"))
            //{
            //    mail = (Mail)MemoryCacheGet("Mail");
            //}
            //else
            //{
                mail = new Mail();
            //    MemoryCacheAdd("Mail", mail);
            //}
	    }
        #endregion

        #region SignIn
        public ActionResult SignIn()
        {
            if (RedirectToProduction())
                return Redirect(UrlProduction);

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
            //and WebID =" + res.Config.WebID;
            var Webs = svWeb.GetWebAll();
            ViewBag.Web = Webs;
            ViewBag.FBUrl = GetFacebookLoginUrl(Request["returnUrl"]);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignIn(SignInModel model)
        {
            EncryptManager encrypt = new EncryptManager();
            var Url = new UrlHelper(System.Web.HttpContext.Current.Request.RequestContext);
            var query = svMember.SelectData<view_emCompanyMember>("MemberID,UserName,Password,DisplayName,Email,CountLogin,CompID", " (UserName = N'" + model.UserName + "' or Email = N'" + model.UserName + "') and Password = N'" + encrypt.EncryptData(model.Password) + "' and RowFlagWeb = 2 and IsDelete = 0 ", null); //and WebID =" + res.Config.WebID

            if (svMember.TotalRow > 0)
            {
                #region find user
                var CompMember = query.First();
                var b2bCompany = svCompany.SelectData<b2bCompany>("CompID", "emCompID =" + CompMember.CompID).First();
                CompMember.CountLogin = CompMember.CountLogin != null ? CompMember.CountLogin : 0;
                if (CompMember.CountLogin < 500)
                {
                    #region
                    RegisterSessionLogon(query);
                    RegisterRememberLogon(query, Convert.ToBoolean(model.Remember));

                    if (CompMember.CountLogin != 0)
                    {
                        //Update Count LogOn 
                        if (!svMember.UpdateByCondition<emMemberWeb>("CountLogin = 0", "MemberID =" + CompMember.MemberID))
                        {
                            return Json(new { IsSuccess = false, Result = res.Common.lblerror_tryagian });
                        }
                        else
                            return Json(new { IsSuccess = false, Result = res.Common.lblerror_tryagian });
                    }
                    else
                    {
                        svCompany.UpdateCompanySignIn(b2bCompany.CompID, true);
                        #region Logon Pass & Check is Admin or Member
                        bool IsAdmin = CheckIsAdmin();
                        string url = "";
                        bool resul = true;
                        if (IsAdmin == true)
                        {
                            if (LogonServiceType == 9)
                            {
                                url = res.Pageviews.PvAdminIndex; 
                                //if (Prosoft.Base.Base.AppLang == "en-US") { url = Regex.Replace(url, "~/", "~/en/"); }
                                //else { url = res.Pageviews.PvAdminIndex; }
                                resul = true;
                                //return Json(new { IsSuccess = true, Result = RedirectLastUrl(res.Pageviews.PvAdminIndex) });
                            }
                            else if (LogonServiceType == 10)
                            {
                                url = res.Pageviews.PvAdminMember; 
                                //if (Prosoft.Base.Base.AppLang == "en-US") { url = Regex.Replace(url, "~/", "~/en/"); }
                                //else { url = res.Pageviews.PvAdminMember; }
                                resul = true;
                                //return Json(new { IsSuccess = true, Result = RedirectLastUrl(res.Pageviews.PvAdminMember)});
                            }
                            else if (LogonServiceType == 11)
                            {
                                url = res.Pageviews.PvAdminIndex; 
                                //if (Prosoft.Base.Base.AppLang == "en-US") { url = Regex.Replace(url, "~/", "~/en/"); }
                                //else { url = res.Pageviews.PvAdminIndex; }
                                resul = true;
                                //return Json(new { IsSuccess = true, Result = RedirectLastUrl(res.Pageviews.PvAdminIndex) });
                            }
                            else if (LogonServiceType == 12)
                            {
                                url = res.Pageviews.PvApproveProduct; 
                                //if (Prosoft.Base.Base.AppLang == "en-US") { url = Regex.Replace(url, "~/", "~/en/"); }
                                //else { url = res.Pageviews.PvApproveProduct; }
                                resul = true;
                                //return Json(new { IsSuccess = true, Result = RedirectLastUrl(res.Pageviews.PvApproveProduct) });
                            }
                            else if (LogonServiceType == 13)
                            {
                                url = res.Pageviews.PvAdminIndex; 
                                //if (Prosoft.Base.Base.AppLang == "en-US") { url = Regex.Replace(url, "~/", "~/en/"); }
                                //else { url = res.Pageviews.PvAdminIndex; }
                                resul = true;
                                //return Json(new { IsSuccess = true, Result = RedirectLastUrl(res.Pageviews.PvApproveMemberPaid) });
                            }
                            else if (LogonServiceType == 14)
                                url = res.Pageviews.PvAdminIndex; 
                            //if (Prosoft.Base.Base.AppLang == "en-US") { url = Regex.Replace(url, "~/", "~/en/"); }
                            //else { url = res.Pageviews.PvAdminIndex; }
                            resul = true;
                                //return Json(new { IsSuccess = true, Result = RedirectLastUrl(res.Pageviews.PvHomeIndex) });
                        }
                        //else
                        //{
                        //    if (Request.Browser.IsMobileDevice)
                                
                        //    {
                        //        url = res.Pageviews.PvHomeIndex; 
                        //        //if (Prosoft.Base.Base.AppLang == "en-US") { url = Regex.Replace(url, "~/", "~/en/"); }
                        //        //else { url = res.Pageviews.PvHomeIndex; }
                        //        resul = true;
                        //        //return Json(new { IsSuccess = true, Result = RedirectLastUrl(res.Pageviews.PvHomeIndex) });
                        //    }
                        //    else
                              
                        //    {
                        //        url = res.Pageviews.PvAfterSignUp; 
                        //        //if (Prosoft.Base.Base.AppLang == "en-US") { url = Regex.Replace(url, "~/", "~/en/"); }
                        //        //else { url = res.Pageviews.PvAfterSignUp; }
                        //        resul = true;
                        //        //return Json(new { IsSuccess = true, Result = RedirectLastUrl(res.Pageviews.PvAfterSignUp) });
                        //    }

                        //}

                        string urlSignIn = url; 
                        //if (Prosoft.Base.Base.AppLang == "en-US") { urlSignIn = Regex.Replace(urlSignIn, "~/", "~/en/"); }
                        //else { urlSignIn = url; }

                        return Json(new { IsSuccess = resul, Result = RedirectLastUrl(urlSignIn) });
                        #endregion
                    }

                    #endregion
                }
                else
                {
                    return Json(new { IsSuccess = false, Result = Url.Content(res.Member.lblcannotlogin5time) });
                }
                #endregion
            }
            else
            {
                #region
                query = svMember.SelectData<view_emCompanyMember>("MemberID,CountLogin,FirstName,LastName,Email,CompID", " (UserName ='" + model.UserName + "' or Email ='" + model.UserName + "') and RowFlagWeb > 1 ", null);//and WebID =" + res.Config.WebID


                if (svMember.TotalRow > 0)
                {
                    var CompMember = new view_emCompanyMember();
                    CompMember.CountLogin = CompMember.CountLogin != null ? CompMember.CountLogin : 0;
                    if (CompMember.CountLogin < 500)
                    {
                        #region
                        if ((4 - CompMember.CountLogin) == 0)
                        {
                            if (!svMember.UpdateByCondition<emMemberWeb>("CountLogin = 500", "MemberID =" + CompMember.MemberID))
                            {
                                return Json(new { IsSuccess = false, Result = Url.Content(res.Common.lblerror_tryagian) });
                            }
                            else
                            {
                                if (!svMember.UpdateByCondition<emMemberActivate>("StartDate ='" + DateTime.Now + "',ExpireDate ='" + DateTime.Now.AddHours(24) + "',ActivateType = 3, ActivateCode = '" + emailManager.GenActivateCode() + "'", "MemberID =" + CompMember.MemberID))
                                {
                                    return Json(new { IsSuccess = false, Result = Url.Content(res.Common.lblerror_tryagian) });
                                }
                                else
                                {

                                    //if (!SendEmail(listUser))
                                    //{
                                    return Json(new { IsSuccess = false, Result = Url.Content(res.Member.lblerror5time) });
                                    //}
                                    //else
                                    //    return Json(new { IsSuccess = false, Result = Url.Content("คุณเข้าสู่ระบบไม่ผ่าน 5 ครั้ง บัญชีของคุณถูกปิดเพื่อความปลอดภัย กรุณาตรวจสอบอีเมลล์เพื่อยืนยันตัวตน") });
                                }
                            }
                        }
                        else
                        {
                            int count = (int)CompMember.CountLogin + 1;
                            if (!svMember.UpdateByCondition<emMemberWeb>("CountLogin = " + count, "MemberID =" + CompMember.MemberID))
                            {
                                return Json(new { IsSuccess = false, Result = Url.Content(res.Common.lblerror_tryagian) });
                            }
                            else
                            {
                                return Json(new { IsSuccess = false, Result = Url.Content(res.Member.lblPassword_Incorrect + " " + (5 - count) + " " + res.Member.lblMoreTimes) });
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        return Json(new { IsSuccess = false, Result = Url.Content(res.Member.lblcannotlogin5time) });
                    }
                }
                #endregion
            }
            return Json(new { IsSuccess = false, Result = Url.Content(res.Member.lblUserEmailPass_Incorrect) });

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
            if (htAuthentication.Count > 0)
            {
                svCompany.UpdateCompanySignIn(LogonCompID, false);
                string SessionID = (htAuthentication.Contains("SessionID")) ? DataManager.ConvertToString(htAuthentication["SessionID"]) : string.Empty;
                string MemberID = (htAuthentication.Contains("MemberID")) ? htAuthentication["MemberID"].ToString() : string.Empty;
                UnRegisterSessionLogon(SessionID, MemberID);
            }
            #region

            Session.RemoveAll();
            Session["LastURLAdmin"] = LastURLAdmin;


            #endregion
            string urlIndex = res.Pageviews.PvHomeIndex;
            //if (Base.AppLang == "en-US")
            //{
            //    urlIndex = Regex.Replace(urlIndex, "~/", "~/en/");
            //}
                
            return Redirect(urlIndex);

        }
        #endregion

        #region SignUp
        public ActionResult SignUp()
        {
            if (RedirectToProduction())
                return Redirect(UrlProduction);

            CommonService svCommon = new CommonService();
            var Province = svAddress.GetProvinceAll().OrderBy(m => m.ProvinceName).ToList();
            var Biztype = svBizType.GetBiztypeAll();
            var Webs = svWeb.SelectData<emWeb>("WebID,WebName", "Isdelete = 0"); //and WebID =" + res.Config.WebID
            ViewBag.Web = Webs.ToList();
            ViewBag.Province = Province;
            ViewBag.Biztype = Biztype;
            ViewBag.EnumMemberType = svCommon.SelectEnum(CommonService.EnumType.MemberType);
            return View();
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        [HttpPost, ValidateInput(false)]
        public ActionResult SignUp(
        #region Param
            string ServiceType,
            string UserName,
            string Password,
            string Email,
            string CompName,
            string BizTypeOther,
            int BizTypeID,
            string FirstName,
            string LastName,
            string Phone,
            int ProvinceID,
            int DistrictID,
            string captcha,
            string captcha_id
        #endregion
        )
        {
            #region set value to Member
            var model = new Register();
            var isResult = false;
            if (captcha == HttpContext.Session["captcha_" + captcha_id].ToString())
            {
                model.UserName = UserName;
                model.Password = Password;
                model.DisplayName = UserName;
                model.AddrLine1 = null;
                model.Emails = Email;
                model.CompName = CompName;
                model.BizTypeID = BizTypeID;
                model.FirstName_register = FirstName;
                model.LastName = LastName;
                model.CountryID = 0;
                model.Phone = Phone;
                model.MemberType = 1;
                model.ProvinceID = ProvinceID;
                model.DistrictID = DistrictID;
                model.CompLevel = 1; // 1 is free , 3 is gold
                model.PostalCode = null;
                model.Mobile = Phone;
                model.Fax = null;
                model.FacebookID = null;
                model.WebID = int.Parse(res.Config.WebID);
                model.ServiceType = Convert.ToInt32(ServiceType);
            #endregion

                model.WebID = int.Parse(res.Config.WebID);
                svMember.UserRegister(model);
                if (svMember.IsResult)
                {
                    svCompany.InsertCompany(model);
                }
                if (svCompany.IsResult)
                {
                    isResult = OnSendMailInformUserName(model.UserName, model.Password, model.FirstName_register, model.Emails, model.CompLevel);
                    //isResult = OnSendMailActivate(model.FirstName, model.Email, model.CompName); 
                    FirstLogin(model.UserName, model.Password);
                }

                
                //svMember.UserRegister(model);

                //if (svMember.IsResult)
                //{
                //    svCompany.InsertCompany(model);
                //}
                //if (svCompany.IsResult)
                //{
                //    // ส่ง Email เพื่อให้สมาชิกกดยืนยันการสมัครสมาชิก (Send Email Activate)
                //    //var isResult = OnSendMailActivate(model.FirstName, model.Email, model.CompName); 
                //    isResult = OnSendMailInformUserName(model.UserName, model.Password, model.FirstName_register, model.Emails, model.CompLevel);
                //    FirstLogin(model.UserName, model.Password);
                //    return Redirect(RedirectLastUrl(res.Pageviews.PvB2BMainIndex));
                //}
            }
            else
            {
                return Json(new { IsResult = svCompany.IsResult, IsSendMail = isResult }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { IsResult = svCompany.IsResult, IsSendMail = isResult }, JsonRequestBehavior.AllowGet);
        }

        #region
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult SignUp(Register model)
        //{
        //    var isResult = false;
        //    if (model.captcha == HttpContext.Session["captcha"].ToString())
        //    {
        //        model.WebID = int.Parse(res.Config.WebID);
        //        svMember.UserRegister(model);

        //        if (svMember.IsResult)
        //        {
        //            svCompany.InsertCompany(model);
        //        }
        //        if (svCompany.IsResult)
        //        {
        //            // ส่ง Email เพื่อให้สมาชิกกดยืนยันการสมัครสมาชิก (Send Email Activate)
        //            //var isResult = OnSendMailActivate(model.FirstName, model.Email, model.CompName); 
        //            isResult = OnSendMailInformUserName(model.UserName, model.Password, model.FirstName_register, model.Emails, model.CompLevel);
        //            FirstLogin(model.UserName, model.Password);
        //            return Redirect(RedirectLastUrl(res.Pageviews.PvB2BMainIndex));
        //        }
        //    }
        //    else
        //    {
        //        return Json(new { IsResult = svCompany.IsResult, IsSendMail = isResult }, JsonRequestBehavior.AllowGet);
        //    }
        //    return Json(new { IsResult = svCompany.IsResult, IsSendMail = isResult }, JsonRequestBehavior.AllowGet);
        //}
        #endregion

        #region PostAddRegister
        [HttpPost, ValidateInput(false)]
        public ActionResult AddRegister
        (
        #region Param
            string UserName,
            string Password,
            string Email,
            string CompName,
            string BizTypeOther,
            int BizTypeID,
            string FirstName,
            string LastName,
            string Phone,
            int ProvinceID,
            int DistrictID,
            string captcha,
            string captcha_id
        #endregion
)
{
            #region set value to Member
            var model = new Register();
            var isResult = false;
            if (captcha == HttpContext.Session["captcha_" + captcha_id].ToString())
            {
                model.UserName = UserName;
                model.Password = Password;
                model.DisplayName = UserName;
                model.AddrLine1 = null;
                model.Emails = Email;
                model.CompName = CompName;
                model.BizTypeID = BizTypeID;
                model.FirstName_register = FirstName;
                model.LastName = LastName;
                model.CountryID = 0;
                model.Phone = Phone;
                model.MemberType = 1;
                model.ProvinceID = ProvinceID;
                model.DistrictID = DistrictID;
                model.CompLevel = 1; // 1 is free , 3 is gold
                model.PostalCode = null;
                model.Mobile = Phone;
                model.Fax = null;
                model.FacebookID = null;
                model.WebID = int.Parse(res.Config.WebID);
            #endregion

                svMember.UserRegister(model);
                if (svMember.IsResult)
                {
                    svCompany.InsertCompany(model);
                }
                if (svCompany.IsResult)
                {
                    isResult = OnSendMailInformUserName(model.UserName, model.Password, model.FirstName_register, model.Emails, model.CompLevel);
                    //isResult = OnSendMailActivate(model.FirstName, model.Email, model.CompName); 
                    FirstLogin(model.UserName, model.Password);
                }
            }
            else
            {
                return Json(new { IsResult = svCompany.IsResult, IsSendMail = isResult, OpenLoading = false }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { IsResult = svCompany.IsResult, IsSendMail = isResult }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        [HttpGet]
        public ActionResult SignUp(string state, string code)
        {
            AuthenticateLogon();

            CommonService svCommon = new CommonService();
            var Province = svAddress.ListProvince();
            var Biztype = svBizType.ListBiztype();
            var Webs = svWeb.SelectData<emWeb>("WebID,WebName", "Isdelete = 0 "); //and WebID =" + res.Config.WebID
            ViewBag.Web = Webs.ToList();
            ViewBag.Province = Province.ToList();
            ViewBag.Biztype = Biztype.ToList();
            ViewBag.EnumMemberType = svCommon.SelectEnum(CommonService.EnumType.MemberType);

            if (state != null || code != null)
            {
                if (Request[BaseController.FACEBOOK_ERROR_REASON] == BaseController.FACEBOOK_USER_DENIED)
                {
                    //this is not implemented. For reference only.
                    return RedirectToAction("SignIn", "Member");
                }

                if (string.IsNullOrEmpty(code))
                {
                    ViewBag.Error = res.Member.lblLoginFBError;
                    return RedirectToAction("SignIn", "Member");
                }

                var returnUrl = state;
                var token = FacebookHelper.GetFacebookAccessToken(code, returnUrl, res.Pageviews.UrlWeb + "/Member/SignUp");
                dynamic response = FacebookHelper.GetFacebookResponse("me", token);
                var picture = String.Format("https://graph.facebook.com/{0}/picture", response.id);

                ViewBag.id = response.id;
                ViewBag.picture = picture;
                ViewBag.email = response.email;
                ViewBag.name = response.name;
                ViewBag.username = response.username;
                ViewBag.first_name = response.first_name;
                ViewBag.last_name = response.last_name;
                string[] province = response.location.name.Split(',');
                var FBProvince = svCompany.SelectData<emProvince>(" * ", "ProvinceNameEng like '%" + province[0] + "%'").First();
                ViewBag.FBProvince = FBProvince.ProvinceID;

                if (!string.IsNullOrEmpty(response.id))
                {
                    string userData = String.Format("{0}|{1}", response.name, response.email);
                    Session["facebooktoken"] = token; //store for future use.
                    SaveFacebookCookie(response.email, response.name, response.id);
                    return View();
                }
                else
                {
                    return View();
                }
            }
            else
            {
                return View();
            }
        }
        #endregion

        #region SignUpPackage
        public ActionResult SignUpPackage(int? ID, string PackageID)
        {
            RememberURL();
            if (!CheckIsLogin())
            {
                return Redirect(res.Pageviews.PvMemberSignIn);
            }
            else
            {
                CommonService svCommon = new CommonService();
                GetStatusUser();

                var Package = svMember.SelectData<b2bPackage>("*", "IsShow = 1 and IsDelete = 0", "ListNo ASC");

                ViewBag.EnumServiceType = svCommon.SelectEnum(CommonService.EnumType.SearchByServiceType);
                ViewBag.Package = Package;
                ViewBag.ID = ID == null ? 0 : ID;

                if (PackageID != null || PackageID != "")
                {
                    PackageID += ",19";
                }
                var OrderDetailIDArr = svMember.SelectData<b2bPackage>("*", "IsShow = 1 and IsDelete = 0 AND NOT PackageID IN (" + PackageID + ")", "ListNo ASC");
                ViewBag.OrderDetailIDArr = OrderDetailIDArr == null ? null : OrderDetailIDArr;

                return View();
            }
        }
        #endregion

        #region Save Package
        [HttpPost, ValidateInput(false)]
        public ActionResult SavePackage(string PackageID,string TotalPrice,string Type)
        {
            b2bMemberPaid mem = new b2bMemberPaid();
            b2bOrder order = new b2bOrder();
            b2bOrderDetail orderDe = new b2bOrderDetail();
            var svOrder = new OrderService();
            try
            {
                var orderIsNull = svOrder.SelectData<b2bOrder>("OrderID", "IsDelete = 0 AND CompID = " + LogonCompID);
                if (orderIsNull.Count() > 0)
                {
                    string sqlWhere = "IsDelete = 0 AND CompID = " + LogonCompID;

                    List<int> PackagesId = new List<int>();
                    foreach (string ID in PackageID.Split(','))
                    {
                        if (!string.IsNullOrEmpty(ID))
                        {
                            int intID;
                            bool isNum = int.TryParse(ID, out intID);
                            if (isNum)
                            {
                                if (intID != 19)
                                {
                                    PackagesId.Add(intID);
                                }
                            }
                        }
                    }

                    for (var i = 0; i < PackagesId.Count(); i++)
                    {
                        sqlWhere += " AND PackageID = " + PackagesId[i];
                        if (PackagesId[i] >= 23 && PackagesId[i] <= 25)
                        {
                            sqlWhere += " AND ExpiredDate > '" + DateTime.Now.ToShortDateString() + "'";
                        }
                        var data = svOrder.SelectData<view_OrderDetail>("PackageID", sqlWhere);
                        if (data.Count() > 0)
                        {
                            return Json(new { IsSuccess = false });
                        }
                    }

                    #region Set b2bMemberPaid
                    var countMem = svMember.SelectData<b2bMemberPaid>("*", " CreatedDate = GetDate() AND RowFlag > 0");
                    int CountMem = countMem.Count + 1;
                    mem.MemberPaidCode = AutoGenCode("MPC", CountMem);
                    mem.CompID = LogonCompID;
                    mem.PaymentStatus = "B";
                    mem.IsShow = true;
                    mem.IsDelete = false;
                    #endregion

                    #region Insert b2bMemberPaid
                    svMember.InsertMemberPaid(mem);
                    #endregion

                    #region Set b2bOrder
                    order.CompID = LogonCompID;
                    order.MemberPaidID = mem.MemberPaidID;
                    order.OrderStatus = "B";
                    order.TotalPrice = decimal.Parse(TotalPrice);
                    order.IsShow = true;
                    order.IsDelete = false;
                    order.RowFlag = 1;
                    order.IsInactive = false;
                    order.IsSend = false;
                    #endregion

                    var OrderDetails = new List<b2bOrderDetail>();
                    #region Set Model Order Detail
                    var countOrder = svMember.SelectData<b2bMemberPaid>("*", " CreatedDate = GetDate() AND RowFlag > 0");
                    int CountOrder = countOrder.Count + 1;

                    for (var i = 0; i < PackagesId.Count(); i++)
                    {
                        var GetPackage = svMember.SelectData<b2bPackage>("*", "PackageID = " + PackagesId[i]).First();

                        var detail = new b2bOrderDetail();
                        detail.OrderType = 1;
                        detail.PackageID = PackagesId[i];
                        detail.RowFlag = 1;
                        detail.IsDelete = false;
                        detail.IsInactive = false;
                        detail.OrderDetailCode = AutoGenCode("ORT", CountOrder);
                        detail.PackagePrice = GetPackage.Price;
                        detail.OrderCount = 0;
                        detail.OptionValue = GetPackage.OptionValue;
                        detail.OptionValueUnit = GetPackage.OptionValueUnit;
                        detail.Duration = GetPackage.Duratrion;

                        CountOrder++;
                        OrderDetails.Add(detail);

                    }
                    #endregion

                    #region Insert b2bOrder
                    svOrder.InsertOrder(order, OrderDetails);
                    #endregion

                    if (svOrder.IsResult)
                    {
                        SendEmailOrderPackage(TotalPrice);
                    }
                }
                else
                {
                    #region Set b2bMemberPaid
                    var countMem = svMember.SelectData<b2bMemberPaid>("*", " CreatedDate = GetDate() AND RowFlag > 0");
                    int CountMem = countMem.Count + 1;
                    mem.MemberPaidCode = AutoGenCode("MPC", CountMem);
                    mem.CompID = LogonCompID;
                    mem.PaymentStatus = "B";
                    mem.IsShow = true;
                    mem.IsDelete = false;
                    #endregion

                    #region Insert b2bMemberPaid
                    svMember.InsertMemberPaid(mem);
                    #endregion

                    #region Set b2bOrder
                    order.CompID = LogonCompID;
                    order.MemberPaidID = mem.MemberPaidID;
                    order.OrderStatus = "B";
                    order.TotalPrice = decimal.Parse(TotalPrice);
                    order.IsShow = true;
                    order.IsDelete = false;
                    order.RowFlag = 1;
                    order.IsInactive = false;
                    order.IsSend = false;
                    #endregion

                    var OrderDetails = new List<b2bOrderDetail>();
                    
                    #region Set Model Order Detail
                    var countOrder = svMember.SelectData<b2bMemberPaid>("*", " CreatedDate = GetDate() AND RowFlag > 0");
                    int CountOrder = countOrder.Count + 1;
                    List<int> PackagesId = new List<int>();

                    foreach (string ID in PackageID.Split(','))
                    {
                        if (!string.IsNullOrEmpty(ID))
                        {
                            int intID;
                            bool isNum = int.TryParse(ID, out intID);
                            if (isNum)
                            {
                                PackagesId.Add(intID);
                            }
                        }
                    }

                    for (var i = 0; i < PackagesId.Count(); i++)
                    {
                        var GetPackage = svMember.SelectData<b2bPackage>("*", "PackageID = " + PackagesId[i]).First();

                        var detail = new b2bOrderDetail();
                        detail.OrderType = 1;
                        detail.PackageID = PackagesId[i];
                        detail.RowFlag = 1;
                        detail.IsDelete = false;
                        detail.IsInactive = false;
                        detail.OrderDetailCode = AutoGenCode("ORT", CountOrder);
                        detail.PackagePrice = GetPackage.Price;
                        detail.OrderCount = 0;
                        detail.OptionValue = GetPackage.OptionValue;
                        detail.OptionValueUnit = GetPackage.OptionValueUnit;
                        detail.Duration = GetPackage.Duratrion;

                        CountOrder++;
                        OrderDetails.Add(detail);

                    }
                    #endregion

                    #region Insert b2bOrder
                    svOrder.InsertOrder(order, OrderDetails);
                    #endregion

                    if (svOrder.IsResult)
                    {
                        SendEmailOrderPackage(TotalPrice);
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false});
            }
            return Json(new { IsSuccess = true});
        }
        #endregion

        #region AfterSignUp
        public ActionResult AfterSignUp()
        {
             RememberURL();
             if (!CheckIsLogin())
             {
                 return Redirect(res.Pageviews.PvMemberSignUp);
             }
             else
             {
                 ViewBag.Page = "AfterSignUp";
                 GetStatusUser();
                 return View();
             }
        }
        public string FirstLogin(string UserName, string Password) {
            SignInModel model = new SignInModel();
            model.UserName = UserName;
            model.Password = Password;
            model.Remember = false;
            SignIn(model);
            return LastURL = res.Pageviews.PvAfterSignUp;
        }
        #endregion

        #region CheckAccount
        [HttpPost]
        public ActionResult CheckAccount(string UserName, string Password, int WebID)
        {
            EncryptManager encrypt = new EncryptManager();
            var username = UserName;
            var password = encrypt.EncryptData(Password);
            var webid = WebID;
            var sqlSelect = "MemberID";
            var sqlWhere1 = " and RowflagWeb = 2 and IsDelete = 0 "; //and WebID =" + WebID
            var sqlWhere2 = " and RowflagWeb = 2 and IsDelete = 0 "; //and WebID =" + res.Config.WebID

            var CompanyMember = svMember.SelectData<view_emCompanyMember>(sqlSelect, "username = '" + username + "' and password = '" + password + "'" + sqlWhere1);
                List<view_emCompanyMember> compmember = CompanyMember.ToList();

                var check = svMember.SelectData<view_emCompanyMember>(sqlSelect, "username = '" + username + "' and password = '" + password + "'" + sqlWhere2);
                List<view_emCompanyMember> checkaccount = check.ToList();

                if (compmember.Count() > 0 && checkaccount.Count() == 0)
                { 
                    Response.Cookies["MemberID"].Value = compmember[0].MemberID.ToString();
                    Response.Cookies["MemberID"].Expires = DateTime.Now.AddHours(1);
                    return Redirect(res.Pageviews.PvSignupWithAccount);
                }

                Hashtable data = new Hashtable();
                data.Add("comp", compmember.Count());
                data.Add("check", checkaccount.Count());
                return Json(data); 

        }
        #endregion

        #region SignupWithAccount
        public ActionResult SignupWithAccount()
        {
            if(Request.Cookies["MemberID"] != null){

                var CompanyMember = svMember.SelectData<view_emCompanyMember>("*", "and RowflagWeb = 2 and IsDelete = 0 and MemberID  =  " + Request.Cookies["MemberID"].Value);
                List<view_emCompanyMember> compmember = CompanyMember.ToList();
                ViewBag.CompanyMember = compmember;

                var Province = svAddress.SelectData<emProvince>("ProvinceID,ProvinceName", "IsDelete = 0", "RegionID");
                var Biztype = svBizType.SelectData<b2bBusinessType>("BizTypeID,BizTypeName", "IsDelete = 0");
                var District = svAddress.SelectData<emDistrict>("DistrictID,DistrictName", "IsDelete = 0 and ProvinceID = " + compmember[0].ProvinceID);
                ViewBag.Province = Province.ToList();
                ViewBag.Biztype = Biztype.ToList();
                ViewBag.District = District.ToList();
                return View();
                }
            return Redirect(System.Configuration.ConfigurationManager.AppSettings["LoginPage"]);
        }

        [HttpPost]
        public ActionResult SignupWithAccount(FormCollection collection)
        {
            #region select emCompanyMember
            var CompanyMember = svMember.SelectData<view_emCompanyMember>("*", "and RowflagWeb = 2 and IsDelete = 0 and UserName  =  " + collection["UserName"]).First();
            #endregion

            #region set ค่า memberweb
            Ouikum.emMemberWeb memberweb = new Ouikum.emMemberWeb();
            memberweb.MemberID = Convert.ToInt32(DeCodeID(collection["MemberID"]));
            memberweb.WebID = Convert.ToInt32(res.Config.WebID);
            #endregion

            #region set ค่า b2bCompany
            b2bCompany company = new b2bCompany();
            company.MemberID = Convert.ToInt32(collection["MemberID"]);
            company.emCompID = Convert.ToInt32(collection["emCompID"]);
            company.ServiceType = Convert.ToByte(collection["ServiceType"]);
            company.BizTypeID = Convert.ToByte(collection["BizTypeID"]);
            company.DisplayName = collection["DisplayName"];
            company.CompName = collection["CompName"];
            company.CompAddrLine1 = collection["AddrLine1"];
            company.CompDistrictID = Convert.ToInt32(collection["DistrictID"]);
            company.CompProvinceID = Convert.ToInt32(collection["ProvinceID"]);
            company.CompPostalCode = collection["PostalCode"];
            company.CompPhone = collection["Phone"];
            company.CompMobile = collection["Mobile"];
            company.CompFax = collection["Fax"];
            company.emCompID = Convert.ToInt32(DeCodeID(collection["emCompID"]));
            company.CompLevel = 1;
            #endregion

            #region Set ค่า เข้า companyProfile
            b2bCompanyProfile compProfile = new b2bCompanyProfile();
            compProfile.CompBizType = Convert.ToByte(collection["BizTypeID"]);
            compProfile.CompName = collection["CompName"];
            compProfile.ProvinceID = Convert.ToInt32(collection["ProvinceID"]);

            #endregion

            #region Insert
            using (var trans = new TransactionScope())
            {
                svMember.InsertMemberWeb(memberweb);
                svCompany.InsertCompany(company);
                compProfile.emCompProfileID = company.CompID;
                svCompany.InsertCompanyProfile(compProfile);
                trans.Complete();
             }
            #endregion

                if (svCompany.IsResult)
                {
                    SignInModel model = new SignInModel();
                    model.UserName = CompanyMember.UserName;
                    model.Password = CompanyMember.Password;
                    model.Remember = false;
                    SignIn(model);
                    //return Redirect("~/MyB2B/Main/Index");
                }

                return Redirect(res.Pageviews.PvB2BMainIndex);
        }
        #endregion

        #region MemberProfile
        public ActionResult MemberProfile()
        {
            RememberURL();
            if (!CheckIsLogin())
            {
                return Redirect(res.Pageviews.PvMemberSignUp);
            }
            else
            {
                GetStatusUser();
                var memberid = LogonMemberID;
                var Members = svMember.SelectData<view_emMember>("MemberID,MemberType,AvatarImgPath,FirstName,LastName,AddrLine1,Email,DistrictID,ProvinceID,DistrictName,ProvinceName,PostalCode,Phone,Mobile,Fax,RegisDate,Rowversion", "IsDelete = 0 and MemberID = " + memberid).First();
                var Comps = svCompany.SelectData<b2bCompany>("CompID,MemberID,ExpireDate", "IsDelete = 0 and MemberID = " + memberid).First();
                
                var Provinces = svAddress.ListProvince().ToList();
                var Districts = svAddress.GetDistrict().Where(it => it.ProvinceID == Members.ProvinceID).ToList();

                ViewBag.Provinces = Provinces;
                ViewBag.Districts = Districts;
                ViewBag.Members = Members;
                ViewBag.ExpireDate = Comps.ExpireDate;
                ViewBag.CompLevel = LogonCompLevel;
                CountMessage();
                CountQuotation();
                ViewBag.CateLevel1 = LogonCompLevel;
                ViewBag.PageType = "Profile";
                ViewBag.MenuName = "MemberProfile";
                return View();
            }
        }
        [HttpPost]
        public ActionResult MemberProfile(FormCollection form)
        {
            Hashtable data = new Hashtable();
            try
            {
                if (Request.Cookies[res.Common.lblWebsite] != null)
                {                   
                    var member = new Ouikum.emMember();
                    var emMembers = svMember.SelectData<emMember>("*", " MemberID = " + LogonMemberID + "");
                    member = emMembers.First();
                    if (emMembers.Count < 1)
                    {
                        data.Add("result", false);
                        //data.Add("RowVersion", form["RowVersion"]);
                        return Json(data);
                    }

                    #region SaveAvatarImg
                    if (!string.IsNullOrEmpty(form["AvatarImgPath"]))
                    {
                        if (member.AvatarImgPath != form["AvatarImgPath"])
                        {
                            imgManager = new FileHelper();
                            //#region Delete Folder
                            //imgManager.DeleteFilesInDir("Member/" + LogonCompID);
                            //#endregion
                            imgManager.DirPath = "Members/" + LogonMemberID;
                            imgManager.DirTempPath = "Temp/Members/" + LogonMemberID;
                            imgManager.ImageName = form["AvatarImgPath"];
                            imgManager.FullHeight = 0;
                            imgManager.FullWidth = 0;

                            imgManager.ThumbHeight = 150;
                            imgManager.ThumbWidth = 150;

                            imgManager.SaveImageFromTemp();
                        }
                    }
                    #endregion

                    var memberid = LogonMemberID;
                    member.MemberID = memberid;
                    member.AvatarImgPath = form["AvatarImgPath"];
                    member.FirstName = form["FirstName"];
                    member.LastName = form["LastName"];
                    member.AddrLine1 = form["AddrLine1"];
                    member.Email = form["Email"];
                    member.DistrictID = DataManager.ConvertToInteger(form["DistrictID"]);
                    member.ProvinceID = DataManager.ConvertToInteger(form["ProvinceID"]);
                    member.PostalCode = form["PostalCode"];
                    member.Phone = form["Phone"];
                    member.Mobile = form["Mobile"];
                    member.Fax = form["Fax"];
                    //member.RowVersion = DataManager.ConvertToShort(form["RowVersion"]);

                    member = svMember.SaveData<emMember>(member, "MemberID");
                    data.Add("result", svMember.IsResult);
                    if (svMember.IsResult)
                    {
                        var RowVersion = member.RowVersion+1;
                        //data.Add("RowVersion", RowVersion);
                    }
                    else
                    {
                        //data.Add("RowVersion", member.RowVersion);
                    }

                    return Json(data);
                }
                else
                {
                    return Json(data);
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles(ex);
                return Json(data);
            }
        }
        #endregion

        #region MemberAccount
        public ActionResult MemberAccount()
        {
            GetStatusUser();
            Authenticate();

            return View();
        }
        [HttpPost]
        public ActionResult MemberAccount(FormCollection collection)
        {
            return View();
        }
        #endregion

        #region ChangePassword
        public ActionResult ChangePassword()
        {
            RememberURL();
            if (!CheckIsLogin())
            {
                return Redirect(res.Pageviews.PvMemberSignUp);
            }
            else
            {
                GetStatusUser();
                string ID = DataManager.ConvertToString(LogonMemberID);
                var emMembers = svMember.SelectData<emMember>("MemberID,MemberType,Password,RowVersion", " IsDelete = 0 and MemberID = " + ID).First();
                ViewBag.Members = emMembers;
                CountMessage();
                CountQuotation();
                ViewBag.CateLevel1 = LogonCompLevel;
                ViewBag.PageType = "Profile";
                ViewBag.MenuName = "ChangePassword";
                return View();
            }
        }
        [HttpPost]
        public ActionResult ChangePassword(FormCollection form)
        {
            Hashtable data = new Hashtable();          
            EncryptManager encrypt = new EncryptManager();

            try
            {
                if (form["NewPassword"] == form["ConfirmPassword"])
                {
                    if (CheckIsLogin())
                    {
                        string ID = DataManager.ConvertToString(LogonMemberID);
                        var member = new Ouikum.emMember();
                        var emMembers = svMember.SelectData<emMember>("*", " MemberID = " + ID + "");

                        if (emMembers.Count < 1)
                        {
                            data.Add("result", false);
                            //data.Add("RowVersion", form["RowVersion"]);
                            return Json(data);
                        }

                        member = emMembers.First();
                        var memberid = DataManager.ConvertToInteger(ID);
                        member.MemberID = memberid;
                        member.Password = encrypt.EncryptData(form["NewPassword"]);
                       // member.RowVersion = DataManager.ConvertToShort(form["RowVersion"]);

                        member = svMember.SaveData<emMember>(member, "MemberID");
                        data.Add("result", svMember.IsResult);
                        if (svMember.IsResult)
                        {
                            var RowVersion = member.RowVersion + 1;
                            //data.Add("RowVersion", RowVersion);
                        }
                        else
                        {
                            //data.Add("RowVersion", member.RowVersion);
                        }

                        return Json(data);
                    }
                    else
                    {
                        return Json(data);
                    }
                }else{
                        data.Add("result", false);
                        //data.Add("RowVersion", form["RowVersion"]);
                        return Json(data);
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles(ex);
                return Json(data);
            }
        }
        #endregion

        #region Update Password
        public ActionResult UpdatePassword(string ActivateCode, string MemberID)
        {
            var query = svMember.SelectData<view_Member>("MemberID,ExpireDate", "ActivateCode = '" + ActivateCode + "' AND MemberID = " + MemberID + " and IsDelete = 0 ", null); //and WebID =" + res.Config.WebID //AND RowFlagActivate = 1 
            if(query.Count() > 0){
                IEnumerable<view_Member> list = query.ToList();
                if (list.Count() > 0)
                {
                    if (list.First().ExpireDate < DateTime.Now)
                    {
                        ViewBag.Message = "0";
                        return RedirectToAction("ForgetPassword", "Member");
                    }
                    else
                    {
                        ViewBag.Result = 1; //Activate pass  
                        ViewBag.Title = "Update Password";
                    }
                }
                else
                {
                    ViewBag.ErrorMsg = res.Common.lblerror_tryagian;
                    ViewBag.Result = 4; //Error
                }
                GetStatusUser();
                ViewBag.MemberID = MemberID;
                return View();
            }else{
                return Redirect(res.Pageviews.PvNotFound);
            }
        }

        [HttpPost]
        public ActionResult UpdatePassword(FormCollection form)
        {
            var newPass = form["NewPassword"] ;
            var comPass = form["ConfirmPassword"];
            var mem = form["member"];

            EncryptManager encrypt = new EncryptManager();
            Hashtable data = new Hashtable(); 
            try
            {
                if (newPass == comPass)
                {
                    //Rowflag = 2                    
                    //if (svMember.UpdateByCondition<emMemberWeb>("RowFlag = 2", "MemberID=" + member + " AND RowFlag =1"))
                    //{
                    if (svMember.UpdateByCondition<emMemberActivate>("RowFlag = 2", "MemberID =" + mem + " AND RowFlag =1"))
                        {
                            if (svMember.UpdateByCondition<emMember>("Password ='" + encrypt.EncryptData(newPass) + "'", "MemberID =" + mem + ""))
                            {
                                var member = new Ouikum.emMember();
                                var emMembers = svMember.SelectData<emMember>("*", " MemberID = " + mem + "");
                                member = emMembers.First();
                                FirstLogin(member.UserName, newPass);

                                data.Add("result", svMember.IsResult);
                                return Json(data);
                            }
                            else
                            {
                                data.Add("result", false);
                                ViewBag.ErrorMsg = res.Common.lblerror_tryagian;
                                ViewBag.Result = 4; //Error
                            }
                        }
                        else
                        {
                            data.Add("result", false);
                            ViewBag.ErrorMsg = res.Common.lblerror_tryagian;
                            ViewBag.Result = 4; //Error
                        }
                    //}
                    //else
                    //{
                    //    ViewBag.ErrorMsg = "เกิดข้อผิดพลาด กรุณาตรวจสอบ";
                    //    ViewBag.Result = 4; //Error
                    //}
                }
                else
                {
                    data.Add("result", false);
                    ViewBag.ErrorMsg = res.JS.vldsame_value;
                    ViewBag.Result = 4; //Error
                }
            }
            catch (Exception ex)
            {
                data.Add("result", false);
                ViewBag.ErrorMsg = res.JS.vldsame_value;
                ViewBag.Result = 4; //Error
            }
            return Content(PartialViewToString("ActivateUC"));
        }
        #endregion

        #region CloseAccount
        public ActionResult CloseAccount()
        {
            GetStatusUser();
            return View();
        }
        [HttpPost]
        public ActionResult CloseAccount(FormCollection collection)
        {
            return View();
        }
        #endregion

        #region ForgetPassword
        public ActionResult ForgetPassword()
        {
            GetStatusUser();
            return View();
       }

        #region ForgetPasswordOld
        //[HttpPost]
        //public ActionResult ForgetPassword(string EmailorUsername)
        //{
        //    var Url = new UrlHelper(System.Web.HttpContext.Current.Request.RequestContext);
        //    var query = svMember.SelectData<emMember>("*", " (UserName = N'" + EmailorUsername + "' or Email = N'" + EmailorUsername + "')");
        //    IEnumerable<emMember> list = query.ToList(); 
        //    if (list.Count() > 0)
        //    {
        //        var listMember = list.First();
        //        var Mem = svMember.SelectData<view_Member>("*", " (UserName = N'" + listMember.UserName + "' or Email = N'" + listMember.Email + "') and Rowflag = 2 and IsDelete = 0 ", null);//and WebID =" + res.Config.WebID
        //        IEnumerable<view_Member> Memlist = Mem.ToList();
        //        if (Memlist.Count() > 0)
        //        {
        //            //var MemAc = svMember.SelectData<view_Member>("*", " (UserName = N'" + listMember.UserName + "' or Email = N'" + listMember.Email + "') and RowflagActivate = 2 AND Rowflag = 2 and IsDelete = 0 ", null);//and WebID =" + res.Config.WebID
        //            //IEnumerable<view_Member> MemAclist = MemAc.ToList();
        //            //if (MemAclist.Count() > 0)
        //            //{
        //                #region Update RowFlag กรณีแจ้งลืมรหัสผ่าน
        //                DateTime Expire = DateTime.Now.AddHours(24);
        //                var memberActivate = svMember.SelectData<emMemberActivate>("*", "MemberID=" + listMember.MemberID + " and IsDelete = 0");

        //                if (memberActivate.Count() > 0)
        //                {
        //                    if (svMember.UpdateByCondition<emMemberActivate>("RowFlag = 1,ActivateType = 2,ActivateCode = '" + emailManager.GenActivateCode() + "',ExpireDate='" + Expire.ToString("yyyy/MM/dd HH:mm:ss", new System.Globalization.CultureInfo("en-US")) + "'", "MemberID =" + listMember.MemberID + " AND RowFlag =2 AND IsDelete = 0"))
        //                    {
        //                        //if (svMember.UpdateByCondition<emMemberWeb>("RowFlag = 1", "MemberID =" + listMember.MemberID + " AND RowFlag = 2 AND IsDelete = 0"))
        //                        //{
        //                        var members = svMember.SelectData<view_Member>("MemberID,CountLogin,FirstName,LastName,ActivateCode,Email", " MemberID=" + listMember.MemberID + " and  IsDelete = 0 ", null);//and WebID =" + res.Config.WebID  // RowflagActivate = 1 and
        //                        IEnumerable<view_Member> listEmail = members.ToList();
        //                        if (!SendEmailForgetPw(listEmail))
        //                        {
        //                            return Json(new { IsSuccess = false, Result = res.Member.lblErrorSendEmail });
        //                        }
        //                        else
        //                            return Json(new { IsSuccess = true, Result = res.Member.lblforgetpass_success });
        //                        //}
        //                    }
        //                }
        //                else
        //                {
        //                    var member = new Ouikum.emMemberActivate();
        //                    member.MemberID = listMember.MemberID;
        //                    member.ActivateCode = emailManager.GenActivateCode();
        //                    member.StartDate = DateTime.Now.Date;
        //                    member.ExpireDate = Expire;
        //                    member.ActivateType = 2;
        //                    member.RowFlag = 1;
        //                    member.RowVersion = 1;
        //                    svMember.SaveData<emMemberActivate>(member, "MemberActivateID");
        //                    if (svMember.IsResult)
        //                    {
        //                        if (svMember.UpdateByCondition<emMemberWeb>("RowFlag = 1", "MemberID =" + listMember.MemberID + " AND RowFlag =2 AND IsDelete = 0"))
        //                        {
        //                            var members = svMember.SelectData<view_Member>("MemberID,CountLogin,FirstName,LastName,ActivateCode,Email", " MemberID=" + listMember.MemberID + " and RowFlag = 1 and IsDelete = 0 and ", null);//WebID =" + res.Config.WebID
        //                            IEnumerable<view_Member> listEmail = members.ToList();
        //                            if (!SendEmailForgetPw(listEmail))
        //                            {
        //                                return Json(new { IsSuccess = false, Result = res.Member.lblErrorSendEmail });
        //                            }
        //                            else
        //                                return Json(new { IsSuccess = true, Result = res.Member.lblforgetpass_success });
        //                        }
        //                    }
        //                }
        //                #endregion
        //            //}
        //            //else {
        //            //    return Json(new { IsSuccess = false, Result = res.Member.lblRequestPassAlready });
        //            //}
        //        }
        //        else {
        //            return Json(new { IsSuccess = false, Result = res.Member.lblHaveAccbutnotMember });
        //        }
        //    }
        //    return Json(new { IsSuccess = false, Result = res.Member.lblNotregister });

        //}
        #endregion

        [HttpPost]
        public ActionResult ForgetPassword(string EmailorUsername)
        {
            
            if (!string.IsNullOrEmpty(EmailorUsername))
            {
                bool emailCheck = Regex.IsMatch(EmailorUsername, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
                if (emailCheck)
                {
                    var Email = svMember.SelectData<emMember>("*", "Email = N'" + EmailorUsername + "' and Rowflag = 2 and IsDelete = 0", null);
                    IEnumerable<emMember> listEmail = Email.ToList();
                    if (listEmail.Count() > 0)
                    {
                        var listMember = listEmail.First();
                        var Mem = svMember.SelectData<view_Member>("*", "Email = N'" + listMember.Email + "' and Rowflag = 2 and IsDelete = 0 ", null);//and WebID =" + res.Config.WebID
                        IEnumerable<view_Member> Memlist = Mem.ToList();
                        if (Memlist.Count() > 0)
                        {
                            var MemberForget = Memlist.First();
                            var member = new Ouikum.emMemberActivate();
                            DateTime Expire = DateTime.Now.AddHours(24);

                            member.MemberID = MemberForget.MemberID;
                            member.ActivateCode = emailManager.GenActivateCode();
                            member.StartDate = DateTime.Now;
                            member.ExpireDate = Expire;
                            member.ActivateType = 2;
                            member.RowFlag = 1;
                            member.RowVersion = 1;
                            svMember.SaveData<emMemberActivate>(member, "MemberActivateID");
                            if (svMember.IsResult)
                            {
                                //if (svMember.UpdateByCondition<emMemberWeb>("RowFlag = 1", "MemberID =" + MemberForget.MemberID + " AND RowFlag =2 AND IsDelete = 0"))
                                //{
                                    var members = svCompany.SelectData<emMember>("*", "MemberID =" + MemberForget.MemberID + "and RowFlag = 2 and IsDelete = 0", null, 1, 1).First();
                                    if (!SendEmailForgetPw(members.FirstName, members.Email, member.ActivateCode, MemberForget.MemberID))
                                    {
                                        return Json(new { IsSuccess = false, Result = res.Member.lblErrorSendEmail });
                                    }
                                    else
                                    {
                                        return Json(new { IsSuccess = true, Result = res.Member.lblforgetpass_success });
                                    }
                                //}
                            }

                        }
                        else
                        {
                            return Json(new { IsSuccess = false, Result = res.Member.lblHaveAccbutnotMember });
                        }
                    }
                    else
                    {
                        return Json(new { IsSuccess = false, Result = res.Member.lblHaveAccbutnotMember });
                    }
                }
                else
                {
                    var UserName = svMember.SelectData<emMember>("*", "UserName = N'" + EmailorUsername + "' and Rowflag = 2 and IsDelete = 0", null);
                    IEnumerable<emMember> listEmail = UserName.ToList();
                    if (listEmail.Count() > 0)
                    {
                        var listMember = listEmail.First();
                        var Mem = svMember.SelectData<view_Member>("*", "UserName = N'" + listMember.UserName + "' and Rowflag = 2 and IsDelete = 0 ", null);//and WebID =" + res.Config.WebID
                        IEnumerable<view_Member> Memlist = Mem.ToList();
                        if (Memlist.Count() > 0)
                        {
                            var MemberForget = Memlist.First();
                            var member = new Ouikum.emMemberActivate();
                            DateTime Expire = DateTime.Now.AddHours(24);

                            member.MemberID = MemberForget.MemberID;
                            member.ActivateCode = emailManager.GenActivateCode();
                            member.StartDate = DateTime.Now;
                            member.ExpireDate = Expire;
                            member.ActivateType = 2;
                            member.RowFlag = 1;
                            member.RowVersion = 1;
                            svMember.SaveData<emMemberActivate>(member, "MemberActivateID");
                            if (svMember.IsResult)
                            {
                                //if (svMember.UpdateByCondition<emMemberWeb>("RowFlag = 1", "MemberID =" + MemberForget.MemberID + " AND RowFlag =2 AND IsDelete = 0"))
                                //{
                                    var members = svCompany.SelectData<emMember>("*", "MemberID =" + MemberForget.MemberID + "and RowFlag = 2 and IsDelete = 0", null, 1, 1).First();
                                    if (!SendEmailForgetPw(members.FirstName, members.Email, member.ActivateCode, MemberForget.MemberID))
                                    {
                                        return Json(new { IsSuccess = false, Result = res.Member.lblErrorSendEmail });
                                    }
                                    else
                                    {
                                        return Json(new { IsSuccess = true, Result = res.Member.lblforgetpass_success });
                                    }
                                //}
                            }

                        }
                        else
                        {
                            return Json(new { IsSuccess = false, Result = res.Member.lblHaveAccbutnotMember });
                        }
                    }
                    else
                    {
                        return Json(new { IsSuccess = false, Result = res.Member.lblHaveAccbutnotMember });
                    }
                }
            }

            //var Url = new UrlHelper(System.Web.HttpContext.Current.Request.RequestContext);
            //var query = svMember.SelectData<emMember>("*", " (UserName = N'" + EmailorUsername + "' or Email = N'" + EmailorUsername + "')");
            //IEnumerable<emMember> list = query.ToList();
            //if (list.Count() > 0)
            //{
            //    var listMember = list.First();
            //    var Mem = svMember.SelectData<view_Member>("*", " (Email = N'" + listMember.Email + "' or UserName = N'" + listMember.UserName + "') and Rowflag = 2 and IsDelete = 0 ", null);//and WebID =" + res.Config.WebID
            //    IEnumerable<view_Member> Memlist = Mem.ToList();
            //    if (Memlist.Count() > 0)
            //    {
            //        //var MemAc = svMember.SelectData<view_Member>("*", " (UserName = N'" + listMember.UserName + "' or Email = N'" + listMember.Email + "') and RowflagActivate = 2 AND Rowflag = 2 and IsDelete = 0 ", null);//and WebID =" + res.Config.WebID
            //        //IEnumerable<view_Member> MemAclist = MemAc.ToList();
            //        //if (MemAclist.Count() > 0)
            //        //{
            //        #region Update RowFlag กรณีแจ้งลืมรหัสผ่าน
            //        DateTime Expire = DateTime.Now.AddHours(24);
            //        var memberActivate = svMember.SelectData<emMemberActivate>("*", "MemberID=" + listMember.MemberID + " and IsDelete = 0");

            //        if (memberActivate.Count() > 0)
            //        {
            //            if (svMember.UpdateByCondition<emMemberActivate>("RowFlag = 1,ActivateType = 2,ActivateCode = '" + emailManager.GenActivateCode() + "',ExpireDate='" + Expire.ToString("yyyy/MM/dd HH:mm:ss", new System.Globalization.CultureInfo("en-US")) + "'", "MemberID =" + listMember.MemberID + " AND RowFlag =2 AND IsDelete = 0"))
            //            {
            //                //if (svMember.UpdateByCondition<emMemberWeb>("RowFlag = 1", "MemberID =" + listMember.MemberID + " AND RowFlag = 2 AND IsDelete = 0"))
            //                //{
            //                var members = svMember.SelectData<view_Member>("MemberID,CountLogin,FirstName,LastName,ActivateCode,Email", " MemberID=" + listMember.MemberID + " and  IsDelete = 0 ", null);//and WebID =" + res.Config.WebID  // RowflagActivate = 1 and
            //                IEnumerable<view_Member> listEmail = members.ToList();
            //                if (!SendEmailForgetPw(listEmail))
            //                {
            //                    return Json(new { IsSuccess = false, Result = res.Member.lblErrorSendEmail });
            //                }
            //                else
            //                    return Json(new { IsSuccess = true, Result = res.Member.lblforgetpass_success });
            //                //}
            //            }
            //        }
            //        else
            //        {
            //            var member = new Ouikum.emMemberActivate();
            //            member.MemberID = listMember.MemberID;
            //            member.ActivateCode = emailManager.GenActivateCode();
            //            member.StartDate = DateTime.Now.Date;
            //            member.ExpireDate = Expire;
            //            member.ActivateType = 2;
            //            member.RowFlag = 1;
            //            member.RowVersion = 1;
            //            svMember.SaveData<emMemberActivate>(member, "MemberActivateID");
            //            if (svMember.IsResult)
            //            {
            //                if (svMember.UpdateByCondition<emMemberWeb>("RowFlag = 1", "MemberID =" + listMember.MemberID + " AND RowFlag =2 AND IsDelete = 0"))
            //                {
            //                    var members = svMember.SelectData<view_Member>("MemberID,CountLogin,FirstName,LastName,ActivateCode,Email", " MemberID=" + listMember.MemberID + " and RowFlag = 1 and IsDelete = 0 and ", null);//WebID =" + res.Config.WebID
            //                    IEnumerable<view_Member> listEmail = members.ToList();
            //                    if (!SendEmailForgetPw(listEmail))
            //                    {
            //                        return Json(new { IsSuccess = false, Result = res.Member.lblErrorSendEmail });
            //                    }
            //                    else
            //                        return Json(new { IsSuccess = true, Result = res.Member.lblforgetpass_success });
            //                }
            //            }
            //        }
            //        #endregion
            //        //}
            //        //else {
            //        //    return Json(new { IsSuccess = false, Result = res.Member.lblRequestPassAlready });
            //        //}
            //    }
            //    else
            //    {
            //        return Json(new { IsSuccess = false, Result = res.Member.lblHaveAccbutnotMember });
            //    }
            //}
            return Json(new { IsSuccess = false, Result = res.Member.lblNotregister });

        }
        #endregion

        #region HttpPost RecoverActivate
        [HttpPost]
        public ActionResult RecoverActivate(string username, string email)
        {
            var query = svMember.SelectData<view_Member>("MemberID", " RowFlagActivate = 1 AND IsDelete = 0 AND UserName = '" + username + "' AND Email = '" + email + "' ", null);//and WebID =" + res.Config.WebID
            IEnumerable<view_Member> list = query.ToList();
            if (list.Count() > 0)
            {
                DateTime Expire = DateTime.Now.AddHours(24);
                if (svMember.UpdateByCondition<emMemberActivate>("ActivateType = 2,ActivateCode = '" + emailManager.GenActivateCode() + "',ExpireDate='" + Expire.ToString("yyyy/MM/dd HH:mm:ss", new System.Globalization.CultureInfo("en-US")) + "'", "MemberID =" + list.First().MemberID + " AND RowFlag = 1 AND IsDelete = 0"))
                {
                    query = svMember.SelectData<view_Member>("MemberID,CountLogin,FirstName,LastName,ActivateCode,Email", " MemberID=" + list.First().MemberID + " and RowFlagActivate = 1 and RowflagWeb = 2 and IsDelete = 0 ", null); //and WebID =" + res.Config.WebID
                    IEnumerable<view_Member> listEmail = query.ToList();
                    if (!SendEmailRecover(listEmail))
                    {
                        return Json(new { IsSuccess = false, ErrorMsg = res.Member.lblErrorSendEmail });
                        //ViewBag.ActivateResult = 4;
                        //ViewBag.ErrorMsg = "เกิดข้อผิดพลาดขณะส่งอีเมลล์ กรุณาลองอีกครั้ง";
                    }
                    else
                    {
                        return Json(new { IsSuccess = true, ErrorMsg = res.Member.lblforgetpass_success });
                        //ViewBag.ActivateResult = 3;
                        //ViewBag.ErrorMsg = "คุณแจ้งยืนยันลืมรหัสผ่านเสร็จแล้ว กรุณาตรวจสอบอีเมลล์";
                    }
                }
                else
                {
                    return Json(new { IsSuccess = false, ErrorMsg = res.Common.lblerror_tryagian });
                    //ViewBag.ActivateResult = 4;
                    //ViewBag.ErrorMsg = "เกิดข้อผิดพลาด กรุณาตรวจสอบ";
                }
            }
            else
            {
                return Json(new { IsSuccess = false, ErrorMsg = res.Member.lbluser_and_email_notcorrect });
                //ViewBag.ActivateResult = 3; //Error
                //ViewBag.ErrorMsg = "คุณยังไม่ได้เป็นสมาชิกของเว็บ" + GetAppSetting("AppName") + " กรุณา<a href=\"~/Register\">สมัครสมาชิก</a>";
            }
        }
        #endregion        

        #region ConfirmLogOn
        [HttpGet]
        public ActionResult ConfirmLogOn(String ActivateCode, String MemberID)
        {
            var query = svMember.SelectData<view_Member>("MemberID", " IsDelete = 0 AND RowFlag = 500 AND ActivateCode = '" + ActivateCode + "' AND MemberID = " + DeCodeID(MemberID), null); //and WebID =" + res.Config.WebID
            IEnumerable<view_Member> list = query.ToList();
            if (list.Count() > 0)
            {
                #region Update RowFlag ยืนยันตัวตน
                if (!svMember.UpdateByCondition<emMemberWeb>("RowFlag = 2,CountLogin = 0", "MemberID =" + list.First().MemberID + " AND RowFlag = 500"))
                {
                    ViewBag.Result = 2; //Error
                }
                else
                    ViewBag.Result = 1; //Complete
                #endregion
            }
            else
                ViewBag.ErrorMessage = res.Common.lblerror_tryagian;
            return View();
        }
        #endregion

        #region AccessDenied
        public ActionResult AccessDenied() {

            GetStatusUser();
            CommonService svCommon = new Common.CommonService();
            ViewBag.EnumServiceType = svCommon.SelectEnum(CommonService.EnumType.SearchByServiceType);
            ViewBag.LastUrl = LastURL;
            return View();
        }
        #endregion

        /*--------------------------logon--------------------------*/

        #region SendEmail คิดว่าจะเอามารวมกันอยู่ แต่ยังไม่ได้ทำ
        public bool SendEmail(IEnumerable<view_Member> list)
        {
            #region SendEmail
            mail.Subject = res.Member.lblAccSuspended;
            //Thai
            mail.Body = "<div style='font-family:tahoma;color:Black;font-size:12px;width:600px'>";
            mail.Body += "<b>"+res.Common.lblDear+" " + list.First().FirstName + " </b><br />";
            mail.Body += "<div style='margin-top:15px;text-indent:40px'>" + res.Member.lblAccSuspended;
            mail.Body += "<a href='" + res.Pageviews.UrlWeb + "/Member/ConfirmLogOn?ActivateCode=" + list.First().ActivateCode;
            mail.Body += "&MemberID=" + EnCode(list.First().MemberID.ToString()) + "'>"+res.Common.lblClick_here+"</a> ";
            mail.Body += res.Member.lblConfirmActive + "</div>";
            mail.Body += "<div style='margin-top:10px;'> " + res.Member.lblCopyurl_activate + " </div>";
            mail.Body += "<div style='margin-top:10px;'>" + res.Pageviews.UrlWeb + "/Member/ConfirmLogOn?ActivateCode=" + list.First().ActivateCode;
            mail.Body += "&MemberID=" + EnCode(list.First().MemberID.ToString()) + "</div>";
            mail.Body += "<div style='margin-top:10px;text-indent:40px'>" + res.Member.lblAftervarifyownership;
            mail.Body += res.Pageviews.UrlWeb + "</div> <br/>";
            mail.Body += "<div style='margin-top:10px;text-indent:40px'>*" + res.Common.lblNote + " : " + res.Member.lblConfirmpass24h + "</div>";
            mail.Body += "<div style='margin-top:5px;text-indent:40px'>" + res.Member.lblNotifyRenewpass + "</div>";
            mail.Body += "<br/><br/>";
            mail.Body += "<div style='margin-top:25px;'>" + res.Member.lblBestRegards + "</div>";
            mail.Body += "<div style='margin-top:10px;'>" + res.Common.lblCustomerService + "</div>";
            mail.Body += "<div style='margin-top:10px;'>" + res.Common.lblDomainShortName + "</div>";
            //Eng
            //mail.Body += "<br/><br/><br/>";
            //mail.Body += "<b>Dear , " + list.First().FirstName + " </b><br />";
            //mail.Body += "<div style='margin-top:15px;text-indent:40px'>Because your ID has block please ";
            //mail.Body += "<a href='" + res.Pageviews.UrlWeb + "/Member/ConfirmLogOn?ActivateCode=" + list.First().ActivateCode;
            //mail.Body += "&MemberID=" + EnCode(list.First().MemberID.ToString()) + "'>Click here</a>";
            //mail.Body += "to confirm your account </div>";
            //mail.Body += "<div style='margin-top:10px;'> If the above link does not work, please copy URL into your browser for activate register link.</div>";
            //mail.Body += "<div style='margin-top:10px;'>" + res.Pageviews.UrlWeb + "/Member/ConfirmLogOn?ActivateCode=" + list.First().ActivateCode;
            //mail.Body += "&MemberID=" + EnCode(list.First().MemberID.ToString()) + "</div>";
            //mail.Body += "<div style='margin-top:10px;text-indent:40px'>When your account has been confirmed. ";
            //mail.Body += "You can sign up at " + res.Pageviews.UrlWeb + "</div> <br />";
            //mail.Body += "<div style='margin-top:10px;text-indent:40px'>*Note : Please activate link and sign in within 24 hours after you finished confirm your account on" + GetAppSetting("AppName");
            //mail.Body += "</div>";
            //mail.Body += "<div style='margin-top:5px;text-indent:40px'>Otherwise,  your data will be delete from system.</div><br /><br />";
            //mail.Body += "<div style='margin-top:25px;'>Best Regards, ";
            //mail.Body += "</div>";
            //mail.Body += "<div style='margin-top:10px;'>" + GetAppSetting("AppName") + " Service</div>";
            //mail.Body += "<div style='margin-top:10px;'>" + res.Common.lblDomainShortName + "</div>";
            //mail.Body += "</div>";

            emailManager.Form = res.Config.SMTP_UserName;
            emailManager.To = list.First().Email;
            emailManager.Subject = mail.Subject;
            emailManager.Body = mail.Body;
            emailManager.BodyEncoding = Encoding.UTF8;
            emailManager.SubjectEncoding = Encoding.UTF8;
            try
            {
                return emailManager.SendEmail();

            }
            catch
            {
                return false;
            }
            #endregion
        }

        #region SendEmailForgetPwOld
        //public bool SendEmailForgetPw(IEnumerable<view_Member> listEmail)
        //{
        //    #region SendEmail
        //    mail.Subject = res.Member.lblNotifyActivateCode + res.Common.lblWebsite;
        //    //Thai
        //    mail.Body = "<div style='font-family:tahoma;color:Black;font-size:12px;width:600px'>";
        //    mail.Body += "<b>"+res.Common.lblDear+" " + listEmail.First().FirstName + " </b><br />";
        //    mail.Body += "<div style='margin-top:15px;text-indent:40px'>"+res.Member.lblNotifyActivateCode + res.Common.lblWebsite;
        //    mail.Body += "<a href='" + res.Pageviews.UrlWeb + "/Member/UpdatePassword?ActivateCode=" + listEmail.First().ActivateCode;
        //    mail.Body += "&MemberID=" + listEmail.First().MemberID.ToString() + "'>"+res.Common.lblClick_here+"</a> ";
        //    mail.Body += res.Member.lblconfirm_set_newpass + " </div>";
        //    mail.Body += "<div style='margin-top:10px;'> " + res.Member.lblCopyurl_activate + " </div>";
        //    mail.Body += "<div style='margin-top:10px;'>" + res.Pageviews.UrlWeb + "/Member/UpdatePassword?ActivateCode=" + listEmail.First().ActivateCode;
        //    mail.Body += "&MemberID=" + listEmail.First().MemberID.ToString() + "</div>";
        //    mail.Body += "<div style='margin-top:10px;text-indent:40px'>" + res.Member.lblAfterSetpass;
        //    mail.Body += "</div> <br/>";
        //    mail.Body += "<div style='margin-top:10px;text-indent:40px'>*"+res.Common.lblNote+" : "+res.Member.lblConfirmpass24h + "</div>";
        //    mail.Body += "<div style='margin-top:5px;text-indent:40px'>" + res.Member.lblNotifyRenewpass + "</div>";
        //    mail.Body += "<br/><br/>";
        //    mail.Body += "<div style='margin-top:25px;'>" + res.Member.lblBestRegards + "</div>";
        //    mail.Body += "<div style='margin-top:10px;'>" + res.Common.lblCustomerService + "</div>";
        //    mail.Body += "<div style='margin-top:10px;'>" + res.Common.lblDomainShortName + "</div>";
        //    //Eng
        //    //mail.Body += "<br/><br/><br/>";
        //    //mail.Body += "<b>Dear , " + listEmail.First().FirstName + " </b><br />";
        //    //mail.Body += "<div style='margin-top:15px;text-indent:40px'>Now, you have to forget the password.";
        //    //mail.Body += "<a href='" + res.Pageviews.UrlWeb + "/Member/UpdatePassword?ActivateCode=" + listEmail.First().ActivateCode;
        //    //mail.Body += "&MemberID=" + listEmail.First().MemberID.ToString() + "'>Click here</a>";
        //    //mail.Body += "to confirm and set a new password. </div>";
        //    //mail.Body += "<div style='margin-top:10px;'> If the above link does not work, please copy URL into your browser window. I forgot my password to confirm and set a new password.</div>";
        //    //mail.Body += "<div style='margin-top:10px;'>" + res.Pageviews.UrlWeb + "/Member/UpdatePassword?ActivateCode=" + listEmail.First().ActivateCode;
        //    //mail.Body += "&MemberID=" + listEmail.First().MemberID.ToString() + "</div>";
        //    //mail.Body += "<div style='margin-top:10px;text-indent:40px'>After successfully setting a new password. You can sign a new password.";
        //    //mail.Body += "You can sign up at " + res.Pageviews.UrlWeb + "</div> <br />";
        //    //mail.Body += "<div style='margin-top:10px;text-indent:40px'>*Note : Please confirm forgot password and set a new password within 24 hours. Otherwise, ";
        //    //mail.Body += "you must contact your new password.</div>";
        //    //mail.Body += "<div style='margin-top:5px;text-indent:40px'> Otherwise,  your data will be delete from system.</div><br /><br />";
        //    //mail.Body += "<div style='margin-top:25px;'>Best Regards, ";
        //    //mail.Body += "</div>";
        //    //mail.Body += "<div style='margin-top:10px;'>" + GetAppSetting("AppName") + " Service</div>";
        //    //mail.Body += "<div style='margin-top:10px;'>" + res.Common.lblDomainShortName + "</div>";
        //    //mail.Body += "</div>";

        //    emailManager.Form = res.Config.SMTP_UserName;
        //    emailManager.To = listEmail.First().Email;
        //    emailManager.Subject = mail.Subject;
        //    emailManager.Body = mail.Body;
        //    emailManager.BodyEncoding = Encoding.UTF8;
        //    emailManager.SubjectEncoding = Encoding.UTF8;
        //    try
        //    {
        //        return emailManager.SendEmail();

        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //    #endregion
        //}
        #endregion

        public bool SendEmailForgetPw(string FirstName, string email, string ActivateCode, int memberid)
        {
            #region variable
            var Detail = "";
            var mailTo = new List<string>();
            var mailCC = new List<string>();
            Hashtable EmailDetail = new Hashtable();
            #endregion

            #region Set Content & Value For Send Email
            string b2bthai_url = res.Pageviews.UrlWeb;
            string pathlogo = b2bthai_url + "/Content/Default/logo/Ouikum/img_Logo120x74.png";

            EmailDetail["b2bthaiUrl"] = b2bthai_url;
            EmailDetail["pathLogo"] = pathlogo;

            EmailDetail["FirstName"] = FirstName;
            EmailDetail["UrlUpdatePassword"] = res.Pageviews.UrlWeb + "/Member/UpdatePassword?ActivateCode=" + ActivateCode + "&MemberID=" + memberid;
            mailTo.Add(email);
            ViewBag.Data = EmailDetail;
            Detail = PartialViewToString("UC/Email/SendMailForgetPassword");

            var subject = res.Member.lblNotifyActivateCode + " " + res.Common.lblWebsite;
            var mailFrom = res.Config.EmailNoReply;
            #endregion

            try
            {
                return OnSendByAlertEmail(subject, mailFrom, mailTo, mailCC, Detail);
            }
            catch
            {
                return false;
            }
        }

        public bool SendEmailRecover(IEnumerable<view_Member> listEmail)
        {
            #region SendEmail
            mail.Subject = res.Member.lblConfirmActivatePass;
            //Thai
            mail.Body = "<div style='font-family:tahoma;color:Black;font-size:12px;width:600px'>";
            mail.Body += "<b>" + res.Common.lblDear + " " + listEmail.First().FirstName + " </b><br />";
            mail.Body += "<div style='margin-top:15px;text-indent:40px'>" + res.Member.lblNotifyActivateCode + res.Common.lblWebsite;
            mail.Body += "<a href='" + res.Pageviews.UrlWeb + "/Member/UpdatePassword?ActivateCode=" + listEmail.First().ActivateCode;
            mail.Body += "&MemberID=" + listEmail.First().MemberID.ToString() + "'>" + res.Common.lblClick_here + "</a> ";
            mail.Body += res.Member.lblconfirm_set_newpass + " </div>";
            mail.Body += "<div style='margin-top:10px;'> " + res.Member.lblCopyurl_activate + " </div>";
            mail.Body += "<div style='margin-top:10px;'>" + res.Pageviews.UrlWeb + "/Member/UpdatePassword?ActivateCode=" + listEmail.First().ActivateCode;
            mail.Body += "&MemberID=" + listEmail.First().MemberID.ToString() + "</div>";
            mail.Body += "<div style='margin-top:10px;text-indent:40px'>" + res.Member.lblAfterSetpass;
            mail.Body += "</div> <br/>";
            mail.Body += "<div style='margin-top:10px;text-indent:40px'>*" + res.Common.lblNote + " : " + res.Member.lblConfirmpass24h + "</div>";
            mail.Body += "<div style='margin-top:5px;text-indent:40px'>" + res.Member.lblNotifyRenewpass + "</div>";
            mail.Body += "<br/><br/>";
            mail.Body += "<div style='margin-top:25px;'>" + res.Member.lblBestRegards + "</div>";
            mail.Body += "<div style='margin-top:10px;'>" + res.Common.lblCustomerService + "</div>";
            mail.Body += "<div style='margin-top:10px;'>" + res.Common.lblDomainShortName + "</div>";
            //Eng
            //mail.Body += "<br/><br/><br/>";
            //mail.Body += "<b>Dear , " + listEmail.First().FirstName + " </b><br />";
            //mail.Body += "<div style='margin-top:15px;text-indent:40px'>Because the link has expired please ";
            //mail.Body += "<a href='" + res.Pageviews.UrlWeb + "/Member/UpdatePassword?ActivateCode=" + listEmail.First().ActivateCode;
            //mail.Body += "&MemberID=" + EnCode(listEmail.First().MemberID.ToString()) + "'>Click here</a>";
            //mail.Body += "to confirm your account </div>";
            //mail.Body += "<div style='margin-top:10px;'>  If the above link does not work, please copy URL into your browser window. I forgot my password to confirm and set a new password.</div>";
            //mail.Body += "<div style='margin-top:10px;'>" + res.Pageviews.UrlWeb + "/Member/UpdatePassword?ActivateCode=" + listEmail.First().ActivateCode;
            //mail.Body += "&MemberID=" + EnCode(listEmail.First().MemberID.ToString()) + "</div>";
            //mail.Body += "<div style='margin-top:10px;text-indent:40px'>After successfully setting a new password. You can sign a new password.";
            //mail.Body += "You can sign up at " + res.Pageviews.UrlWeb + "</div> <br />";
            //mail.Body += "<div style='margin-top:10px;text-indent:40px'>*Note : Please confirm forgot password and set a new password within 24 hours. Otherwise,";
            //mail.Body += "you must contact your new password.</div>";
            //mail.Body += "</div>";
            //mail.Body += "<div style='margin-top:5px;text-indent:40px'>Otherwise,  your data will be delete from system.</div><br /><br />";
            //mail.Body += "<div style='margin-top:25px;'>Best Regards, ";
            //mail.Body += "</div>";
            //mail.Body += "<div style='margin-top:10px;'>" + GetAppSetting("AppName") + " Service</div>";
            //mail.Body += "<div style='margin-top:10px;'>" + res.Common.lblDomainShortName + "</div>";
            //mail.Body += "</div>";

            emailManager.Form = res.Config.SMTP_UserName;
            emailManager.To = listEmail.First().Email;
            emailManager.Subject = mail.Subject;
            emailManager.Body = mail.Body;
            emailManager.BodyEncoding = Encoding.UTF8;
            emailManager.SubjectEncoding = Encoding.UTF8;
            try
            {
                return emailManager.SendEmail();
            }
            catch
            {
                return false;
            }
            #endregion
        }

        public bool SendEmailOrderPackage(string TotalPrice)
        {
            var svOrder = new OrderService();
            var svMember = new MemberService();
            #region variable
            var Detail = "";
            var mailTo = new List<string>();
            var mailCC = new List<string>();
            Hashtable EmailDetail = new Hashtable();
            #endregion

            #region Set Content & Value For Send Email
            string b2bthai_url = res.Pageviews.UrlWeb;
            string pathlogo = b2bthai_url + "/Content/Default/logo/Ouikum/img_Logo120x74.png";

            var mem = svMember.SelectData<view_CompMember>("*", "IsDelete = 0 and CompID = " + LogonCompID).First();
            var OrderDetail = svOrder.SelectData<view_OrderDetail>("*", "IsDelete = 0 and CompID = " + LogonCompID, "CreatedDate ASC");

            EmailDetail["b2bthaiUrl"] = b2bthai_url;
            EmailDetail["pathLogo"] = pathlogo;

            EmailDetail["FirstName"] = mem.FirstName;
            EmailDetail["CompName"] = mem.CompName;
            EmailDetail["Phone"] = mem.Phone;
            EmailDetail["Email"] = mem.Email;
            EmailDetail["TotalPrice"] = decimal.Parse(TotalPrice);
            EmailDetail["UrlMyPackage"] = res.Pageviews.UrlWeb + "/MyPackage";
            mailTo.Add(mem.Email);
            ViewBag.Data = EmailDetail;
            ViewBag.OrderDetail = OrderDetail;
            Detail = PartialViewToString("UC/Email/OrderPackage");

            var subject = "คุณ"+mem.FirstName + " แจ้งความต้องการสมัครใช้บริการเสริม - ouikum.com";
            var mailFrom = res.Config.EmailNoReply;
            #endregion

            try
            {
                return OnSendByAlertEmail(subject, mailFrom, mailTo, mailCC, Detail);
            }
            catch
            {
                return false;
            }
        }

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
        public ActionResult ValidateRegister(string username, string email, string compname, string compnameeng, string displayname, string captcha, string captcha_id)
        {
            var IsResult = true;
            var sqlSelect = "compid,memberid,username,email,compname";
            var sqlWhere = " and RowflagWeb = 2 and IsDelete = 0 "; //and WebID =" + res.Config.WebID

            if (!string.IsNullOrEmpty(username))
            {
                var Member = svMember.SelectData<view_emCompanyMember>(sqlSelect, "username = N'" + username + "'"+sqlWhere);
                if (Member.Count() > 0)
                {
                    IsResult = false;
                }
            }

            if (!string.IsNullOrEmpty(email))
            {
                var Member = svMember.SelectData<view_emCompanyMember>(sqlSelect, "email = N'" + email + "'" + sqlWhere);
                if (Member.Count() > 0)
                {
                    IsResult = false;
                }
            }
            if (!string.IsNullOrEmpty(compname))
            {
                var Member = svMember.SelectData<view_emCompanyMember>(sqlSelect, "compname = N'" + compname + "'" + sqlWhere);
                if (Member.Count() > 0)
                {
                    IsResult = false;
                }
            }
            if (!string.IsNullOrEmpty(compnameeng))
            {
                var Member = svMember.SelectData<view_emCompanyMember>(sqlSelect, "compnameeng = N'" + compnameeng + "'" + sqlWhere);
                if (Member.Count() > 0)
                {
                    IsResult = false;
                }
            }
            if (!string.IsNullOrEmpty(displayname))
            {
                var Company = svCompany.SelectData<b2bCompany>("compid,displayname", "displayname = N'" + displayname + "' AND IsDelete = 0");
                if (Company.Count() > 0)
                {
                    IsResult = false;
                }
            }
            if (!string.IsNullOrEmpty(captcha))
            {
                if (captcha == HttpContext.Session["captcha_" + captcha_id].ToString())
                {
                    IsResult = true;
                }
                else
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
            var sqlWhere = " and RowflagWeb = 2 and IsDelete = 0 "; //and WebID =" + res.Config.WebID
            if (Request.Cookies[res.Common.lblWebsite] != null)
            {
                member = svMember.SelectData<emMember>("memberid,username,password", " MemberID = " + LogonMemberID).First();
            }
            if (!string.IsNullOrEmpty(password))
            {
                var Password = encrypt.EncryptData(password);
                var Member = svMember.SelectData<view_emCompanyMember>(sqlSelect, "username = N'" + member.UserName + "' and Password = N'" + Password + "'" + sqlWhere);
                if (Member.Count() > 0)
                {
                    IsResult = false;
                }
            }
            return Json(IsResult);
        }
        #endregion

        #region ValidateLogin
        public ActionResult ValidateLogin(string username, string password)
        {
            EncryptManager encrypt = new EncryptManager();
            var IsResult = true;
            var TypeError = 0;
            var sqlSelect = "compid,memberid,username,email,compname";
            var sqlWhere = " and RowflagWeb = 2 and IsDelete = 0 "; //and WebID =" + res.Config.WebID

            var member = svMember.SelectData<view_emCompanyMember>("memberid,username,password,Email", "IsDelete = 0 AND (UserName = N'" + username + "' OR Email = N'" + username + "') ");
            if (member.Count() > 0)
            {
                if (!string.IsNullOrEmpty(username))
                {
                    var Password = encrypt.EncryptData(password);
                    var Member = svMember.SelectData<view_emCompanyMember>(sqlSelect, "(UserName = N'" + username + "' OR Email = N'" + username + "')" + sqlWhere);
                    if (Member.Count() == 0)
                    {
                        IsResult = false;
                        TypeError = 3;//USER
                    }
                }

                if (!string.IsNullOrEmpty(password))
                {
                    var Password = encrypt.EncryptData(password);
                    var Member = svMember.SelectData<view_emCompanyMember>(sqlSelect, "(UserName = N'" + username + "' OR Email = N'" + username + "') and Password = N'" + Password + "'" + sqlWhere);
                    if (Member.Count() == 0 && TypeError != 3)
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

        #region ValidateUserActivated
        public ActionResult ValidateUserActivated(string username, string email)
        {
            var IsResult = true;
            var sqlSelect = "compid,memberid,username,email,compname";
            var sqlWhere = " and RowflagWeb = 1 and IsDelete = 0 "; //and WebID =" + res.Config.WebID

            if (!string.IsNullOrEmpty(username))
            {
                var Member = svMember.SelectData<view_emCompanyMember>(sqlSelect, "username = N'" + username + "'" + sqlWhere);
                if (Member.Count() > 0)
                {
                    IsResult = false;
                }
            }

            if (!string.IsNullOrEmpty(email))
            {
                var Member = svMember.SelectData<view_emCompanyMember>(sqlSelect, "email = N'" + email + "'" + sqlWhere);
                if (Member.Count() > 0)
                {
                    IsResult = false;
                }
            }

            return Json(IsResult);
        }
        #endregion

        #endregion

        /*--------------------AvatarImg------------------------*/
        #region SaveAvatarImg
        [HttpPost]
        public ActionResult SaveAvatarImg(HttpPostedFileBase FileAvatarImgPath)
        {
            imgManager = new FileHelper();
            #region Delete Folder
            imgManager.DeleteFilesInDir("Temp/Members/" + LogonMemberID);
            #endregion
            imgManager.UploadImage("Temp/Members/" + LogonMemberID, FileAvatarImgPath);
            Response.Cookies["MemberID"].Value = DataManager.ConvertToString(LogonMemberID);
            return Json(new { newimage = imgManager.ImageName }, "text/plain");

        }
        #endregion

        #region RemoveAvatarImg
        public ActionResult RemoveAvatarImg()
        {
            imgManager = new FileHelper();
            imgManager.DeleteFilesInDir("Temp/Members/" + LogonMemberID);
            return Json(new { newimage = imgManager.ImageName });
        }
        #endregion

        #region SaveFacebookCookie
        private void SaveFacebookCookie(string email, string uname, string uid)
        {
            string userData = String.Format("{0}", uname);

            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1,
              uid.ToString(),
              DateTime.Now,
              DateTime.Now.AddMinutes(60), //add your own value here
              true,
              userData,
              FormsAuthentication.FormsCookiePath);

            // Encrypt the ticket.
            string encTicket = FormsAuthentication.Encrypt(ticket);

            // Create the cookie.
            var ck = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket) { Expires = ticket.Expiration };
            Response.Cookies.Add(ck);

        }
        #endregion

        #region CountMessage
        public void CountMessage()
        {
            MessageService svMessage = new MessageService();
            ViewBag.CountInbox = svMessage.SelectData<emMessage>("*", svMessage.CreateWhereAction(MessageStatus.UnRead, LogonCompID), null, 0, 0).Count();
            ViewBag.CountImportance = svMessage.SelectData<emMessage>("*", svMessage.CreateWhereAction(MessageStatus.Important, LogonCompID), null, 0, 0).Count();
            ViewBag.CountDraftbox = svMessage.SelectData<emMessage>("*", svMessage.CreateWhereAction(MessageStatus.Draft, LogonCompID), null, 0, 0).Count();
            ViewBag.CountSentbox = svMessage.SelectData<emMessage>("*", svMessage.CreateWhereAction(MessageStatus.Sentbox, LogonCompID), null, 0, 0).Count();
        }
        #endregion

        #region CountQuotation
        public void CountQuotation()
        {
            var svQuotation = new QuotationService();
            ViewBag.Inbox = svQuotation.CountData<b2bQuotation>("*", "( IsDelete = 0 ) AND (IsRead = 'False') AND (IsOutBox = 'False')  AND (IsRead = 0) AND (ToCompID = " + LogonCompID + ")");
            ViewBag.Importance = svQuotation.CountData<b2bQuotation>("*", "( ToCompID = " + LogonCompID + " AND IsOutBox = 0 AND IsDelete = 0 AND  IsImportance = 1 ) OR (FromCompID = " + LogonCompID + " AND IsOutBox = 1 AND IsDelete = 0 AND  IsImportance = 1 )");
            ViewBag.Sentbox = svQuotation.CountData<b2bQuotation>("*", "( IsDelete = 0 AND  IsOutbox = 1 AND FromCompID = " + LogonCompID + " )");
        }
        #endregion

        //public void CountOrderPurchase()
        //{
        //    var svOrderPurchase = new Orderpu
        //}
    }
}