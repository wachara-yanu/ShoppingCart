using Ouikum.BizType;
using Ouikum.Category;
using Ouikum.Common;
using Ouikum.Company;
using Ouikum.Message;
using Ouikum.Product;
using Ouikum.Quotation;
using Ouikum.Web.Models;
using Prosoft.Base;
using Prosoft.Service;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using res = Prosoft.Resource.Web.Ouikum;

namespace Ouikum.Web.Controllers.B2BMobile
{
    public class B2BMobileController : BaseController
    {
        #region B2BThai on mobile
            AddressService svAddress = new AddressService();
            MemberService svMember = new MemberService();
            CompanyService svCompany = new CompanyService();
            ProductService svProduct = new ProductService();
            WebService svWeb = new WebService();
            EmailManager emailManager;
            BizTypeService svBizType = new BizTypeService();
            CommonService svCommon = new CommonService();
            QuotationService svQuotation = new QuotationService();
            MessageService svMessage = new MessageService();
        #endregion
        #region Index

        public ActionResult Index()
        {
            GetStatusUser();
            GetCookie();
            SearchType();
            StatusMenu();

            var svCategory = new CategoryService();
            var CateTypes = svCategory.GetCategoryByLevel(1);
            ViewBag.CateTypes = CateTypes;

            return View();
        }

        #endregion

        // Member System //

        #region Register
            public ActionResult Register()
            {
                var Province = svAddress.GetProvinceAll().OrderBy(m => m.ProvinceName).ToList(); ;
                var Biztype = svBizType.GetBiztypeAll();
                ViewBag.Biztype = Biztype;
                ViewBag.Province = Province;
                return View();
            }
            [HttpPost]
            public ActionResult Register(
                string UserName,
                string Password,
                int BizTypeID,
                string CompName,
                string FirstName,
                string LastName,
                string Phone,
                string Emails,
                int ProvinceID,
                int DistrictID
            ) 
            {
                try
                {
                    var model = new Register();
                    var isResult = false;

                    var checkUser = svMember.SelectData<emMember>("*", "IsDelete = 0 and UserName = N'" + UserName + "'");
                    if (checkUser.Count > 0)
                    {
                        return Json(new { IsSuccess = false, Result = "ขออภัยชื่อผู้ใช้นี้มีในระบบแล้ว" });
                    }
                    else
                    {
                        model.UserName = UserName;
                        model.Password = Password;
                        model.BizTypeID = BizTypeID;
                        model.CompName = CompName;
                        model.CompLevel = 1;
                        model.DisplayName = UserName;
                        model.FirstName_register = FirstName;
                        model.LastName = LastName;
                        model.Phone = Phone;
                        model.Emails = Emails;
                        model.ProvinceID = ProvinceID;
                        model.DistrictID = DistrictID;
                        model.MemberType = 1;
                        model.WebID = int.Parse(res.Config.WebID);
                        svMember.UserRegister(model);
                    
                        if (svMember.IsResult)
                        {
                            svCompany.InsertCompany(model);            
                        }
                        if (svCompany.IsResult)
                        {
                            isResult = OnSendMailInformUserName(model.UserName, model.Password, model.FirstName_register, model.Emails, model.CompLevel);
                            FirstLogin(model.UserName, model.Password);
                        }
                        return Json(new { IsSuccess = true, Result = "Index" });
                    }
                }
                catch (Exception)
                {
                    return Json(new { IsSuccess = false, Result = "ขออภัยเกิดข้อผิดพลาดในระบบ" });
                    throw;
                } 
            }
            #region Get FirstLogin
                [HttpGet]
                public string FirstLogin(string UserName, string Password)
                {
                    SignInModel model = new SignInModel();
                    model.UserName = UserName;
                    model.Password = Password;
                    model.Remember = false;
                    SignIn(model);
                    return LastURL = res.Pageviews.PvAfterSignUp;
                }
            #endregion

        #endregion
        #region SignIn

        public ActionResult SignIn()
        {       
            var Webs = svWeb.GetWebAll();
            ViewBag.Web = Webs;
            return View();
        }
        [HttpPost]     
        public ActionResult SignIn(SignInModel model)
        {
            EncryptManager encrypt = new EncryptManager();
            var Url = new UrlHelper(System.Web.HttpContext.Current.Request.RequestContext);
            var query = svMember.SelectData<view_emCompanyMember>("MemberID,UserName,Password,DisplayName,Email,CountLogin,CompID,MemberType", " (UserName = N'" + model.UserName + "' or Email = N'" + model.UserName + "') and Password = N'" + encrypt.EncryptData(model.Password) + "' and RowFlagWeb = 2 and IsDelete = 0 ", null); //and WebID =" + res.Config.WebID
            if (query.Count > 0)
            {                             
                var memID = Convert.ToInt32(query.First().MemberID);
                var memType =  Convert.ToInt32(query.First().MemberType);
                if (!CheckAdmin(memID,memType))
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
                                return Json(new { IsSuccess = false, result = res.Common.lblerror_tryagian });
                            }
                            else
                                return Json(new { IsSuccess = false, result = res.Common.lblerror_tryagian });
                        }
                        else
                        {                  
                            svCompany.UpdateCompanySignIn(b2bCompany.CompID, true);      
                            return Json(new { IsSuccess = true, result = "Index" });
                        }

                        #endregion
                    }
                    else
                    {
                        return Json(new { IsSuccess = false, result = Url.Content(res.Member.lblcannotlogin5time) });
                    }
                    #endregion
                }
                else
                {
                    return Json(new { IsSuccess = false, result = "ระบบไม่รองรับการเข้าถึงของ Admin" });
                }
                
            }
            else
            {
                #region
                query = svMember.SelectData<view_emCompanyMember>("MemberID,CountLogin,FirstName,LastName,Email,CompID", " (UserName ='" + model.UserName + "' or Email ='" + model.UserName + "') and RowFlagWeb > 1 and IsDelete = 0 ", null);//and WebID =" + res.Config.WebID


                if (svMember.TotalRow > 0)
                {
                    var CompMember = query.First();
                    CompMember.CountLogin = CompMember.CountLogin != null ? CompMember.CountLogin : 0;
                    if (CompMember.CountLogin < 500)
                    {
                        #region Check Error
                        if ((4 - CompMember.CountLogin) == 0)
                        {
                            if (!svMember.UpdateByCondition<emMemberWeb>("CountLogin = 500", "MemberID =" + CompMember.MemberID))
                            {
                                return Json(new { IsSuccess = false, result = Url.Content(res.Common.lblerror_tryagian) });
                            }
                            else
                            {
                                if (!svMember.UpdateByCondition<emMemberActivate>("StartDate ='" + DateTime.Now + "',ExpireDate ='" + DateTime.Now.AddHours(24) + "',ActivateType = 3, ActivateCode = '" + emailManager.GenActivateCode() + "'", "MemberID =" + CompMember.MemberID))
                                {
                                    return Json(new { IsSuccess = false, result = Url.Content(res.Common.lblerror_tryagian) });
                                }
                                else
                                {
                                    return Json(new { IsSuccess = false, result = Url.Content(res.Member.lblerror5time) });
                                }
                            }
                        }
                        else
                        {
                            int count = (int)CompMember.CountLogin + 1;
                            if (!svMember.UpdateByCondition<emMemberWeb>("CountLogin = " + count, "MemberID =" + CompMember.MemberID))
                            {
                                return Json(new { IsSuccess = false, result = Url.Content(res.Common.lblerror_tryagian) });
                            }
                            else
                            {
                                return Json(new { IsSuccess = false, result = Url.Content(res.Member.lblPassword_Incorrect + " " + (5 - count) + " " + res.Member.lblMoreTimes) });
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        return Json(new { IsSuccess = false, result = Url.Content(res.Member.lblcannotlogin5time) });
                    }
                }
                #endregion
            }
            return Json(new { IsSuccess = false, result = Url.Content(res.Member.lblUserEmailPass_Incorrect) });

        }
        #endregion
        #region Edit Profile

        public ActionResult EditProfile()
        {
            RememberURL();
            if (!CheckIsLogin())
            {
                return Redirect("SignIn");
            }
            else
            {
                GetStatusUser();
                GetCookie();
                SearchType();
                StatusMenu();

                var memberid = LogonMemberID;
                var compid = LogonCompID;
                var Members = svMember.SelectData<view_emMember>("MemberID,FirstName,LastName,Email,DistrictID,ProvinceID,DistrictName,ProvinceName,Phone", "IsDelete = 0 and MemberID = " + memberid).First();
                var Comps = svCompany.SelectData<b2bCompany>("CompID,MemberID,CompName", "IsDelete = 0 and MemberID = " + memberid).First();
                var CompPro = svCompany.SelectData<b2bCompanyProfile>("CompProfileID,CompID", "IsDelete = 0 and CompID = " + compid).First();

                var Provinces = svAddress.ListProvince().ToList();
                var Districts = svAddress.GetDistrict().Where(it => it.ProvinceID == Members.ProvinceID).ToList();

                ViewBag.Provinces = Provinces;
                ViewBag.Districts = Districts;
                ViewBag.Members = Members;
                ViewBag.Comps = Comps;
                ViewBag.Comppro = CompPro;
                return View();
            }
        }
            #region Post Edit
        [HttpPost]
        public ActionResult EditProfile(FormCollection form)
        {
            try
            {
                if (Request.Cookies[res.Common.lblWebsite] != null)
                {
                    var memberid = LogonMemberID;
                    var compid = LogonCompID;
                    var member = svMember.GetByID<emMember>("MemberID", memberid);
                    var comps = svCompany.GetByID<b2bCompany>("CompID", compid);
                    var comppro = svCompany.GetByID<b2bCompanyProfile>("CompID", compid);

                    member.MemberID = memberid;
                    member.FirstName = form["FirstName"];
                    member.Phone = form["Phone"];
                    member.ProvinceID = DataManager.ConvertToInteger(form["ProvinceID"]);
                    member.LastName = form["LastName"];
                    member.Email = form["Emails"];
                    member.DistrictID = DataManager.ConvertToInteger(form["DistrictID"]);

                    comps.CompID = Convert.ToInt32(form["CompID"]);
                    comps.CompName = form["CompName"];
                    comppro.CompProfileID = Convert.ToInt32(form["CompproID"]);

                    member = svMember.SaveData<emMember>(member,"MemberID");
                    comps = svCompany.SaveData<b2bCompany>(comps,"CompID");
                    comppro = svCompany.SaveData<b2bCompanyProfile>(comppro,"CompProfileID");

                    return Json(new { IsSuccess = true, Result = "แก้ไขข้อมูลสำเร็จ" });
                }
                else
                {
                    return Json(new { IsSuccess = false, Result = "การแก้ไขข้อมูลเกิดข้อผิดพลาด" });
                }
            }
            catch (Exception)
            {
                return Json(new { IsSuccess = false, Result = "ขออภัยเกิดข้อผิดพลาดในระบบ" });
                throw;
            }
        }
        #endregion

        #endregion
        #region Change Password

        public ActionResult ChangePass()
        {
            RememberURL();
            if (!CheckIsLogin())
            {
                return Redirect("SignIn");
            }
            else
            {
                GetStatusUser();
                GetCookie();
                SearchType();
                StatusMenu();

                var memberid = LogonMemberID;
                var Members = svMember.SelectData<emMember>("MemberID,Password", " IsDelete = 0 and MemberID = " + memberid).First();
                ViewBag.Members = Members;
                return View();
            }
        }
        [HttpPost]
        public ActionResult ChangePass(FormCollection form)
        {
            Hashtable data = new Hashtable();
            EncryptManager encrypt = new EncryptManager();

            try
            {
                if (form["NewPassword"] == form["ConfirmPassword"])
                {
                    if (CheckIsLogin())
                    {
                        var memberid = LogonMemberID;
                        var member = new Ouikum.emMember();
                        var emMembers = svMember.SelectData<emMember>("*", " MemberID = " + memberid + "");

                        if (emMembers.Count < 1)
                        {
                            data.Add("result", false);
                            return Json(data);
                        }

                        member = emMembers.First();
                        var ID = DataManager.ConvertToInteger(memberid);
                        member.MemberID = ID;
                        member.Password = encrypt.EncryptData(form["NewPassword"]);

                        member = svMember.SaveData<emMember>(member, "MemberID");

                        data.Add("result", true);     
                        return Json(data);

                    }
                    else
                    {
                        data.Add("result", false);
                        return Json(data);
                    }     
                }
                else
                {
                    data.Add("result", false);
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
        #region Forget Password
        public ActionResult ForgetPassword()
        {
            GetStatusUser();
            RememberURL();
            StatusMenu();
            GetCookie();

            return View();
        }
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
                        var Mem = svMember.SelectData<view_Member>("*", "Email = N'" + listMember.Email + "' and Rowflag = 2 and IsDelete = 0 ", null);
                        IEnumerable<view_Member> Memlist = Mem.ToList();
                        if (Memlist.Count() > 0)
                        {
                            var MemberForget = Memlist.First();
                            var member = new Ouikum.emMemberActivate();
                            DateTime Expire = DateTime.Now.AddHours(24);

                            member.MemberID = MemberForget.MemberID;
                            member.ActivateCode = Guid.NewGuid().ToString();
                            member.StartDate = DateTime.Now;
                            member.ExpireDate = Expire;
                            member.ActivateType = 2;
                            member.RowFlag = 1;
                            member.RowVersion = 1;
                            svMember.SaveData<emMemberActivate>(member, "MemberActivateID");
                            if (svMember.IsResult)
                            {
                                var members = svCompany.SelectData<emMember>("*", "MemberID =" + MemberForget.MemberID + "and RowFlag = 2 and IsDelete = 0", null, 1, 1).First();
                                if (!SendEmailForgetPw(members.FirstName, members.Email, member.ActivateCode, MemberForget.MemberID))
                                {
                                    return Json(new { IsSuccess = false, Result = res.Member.lblErrorSendEmail });
                                }
                                else
                                {
                                    return Json(new { IsSuccess = true, Result = res.Member.lblforgetpass_success });
                                }
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
                        var Mem = svMember.SelectData<view_Member>("*", "UserName = N'" + listMember.UserName + "' and Rowflag = 2 and IsDelete = 0 ", null);
                        IEnumerable<view_Member> Memlist = Mem.ToList();
                        if (Memlist.Count() > 0)
                        {
                            var MemberForget = Memlist.First();
                            var member = new Ouikum.emMemberActivate();
                            DateTime Expire = DateTime.Now.AddHours(24);

                            member.MemberID = MemberForget.MemberID;
                            member.ActivateCode = Guid.NewGuid().ToString();
                            member.StartDate = DateTime.Now;
                            member.ExpireDate = Expire;
                            member.ActivateType = 2;
                            member.RowFlag = 1;
                            member.RowVersion = 1;
                            svMember.SaveData<emMemberActivate>(member, "MemberActivateID");
                            if (svMember.IsResult)
                            {
                                var members = svCompany.SelectData<emMember>("*", "MemberID =" + MemberForget.MemberID + "and RowFlag = 2 and IsDelete = 0", null, 1, 1).First();
                                if (!SendEmailForgetPw(members.FirstName, members.Email, member.ActivateCode, MemberForget.MemberID))
                                {
                                    return Json(new { IsSuccess = false, Result = res.Member.lblErrorSendEmail });
                                }
                                else
                                {
                                    return Json(new { IsSuccess = true, Result = res.Member.lblforgetpass_success });
                                }
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
            return Json(new { IsSuccess = false, Result = res.Member.lblNotregister });
        }
        #endregion
        #region Update Password

        public ActionResult UpdatePassword(string ActivateCode, string MemberID)
        {
            var query = svMember.SelectData<view_Member>("MemberID,ExpireDate", "ActivateCode = '" + ActivateCode + "' AND MemberID = " + MemberID + " and IsDelete = 0 ", null);
            if (query.Count() > 0)
            {
                IEnumerable<view_Member> list = query.ToList();
                if (list.Count() > 0)
                {
                    if (list.First().ExpireDate < DateTime.Now)
                    {
                        return RedirectToAction("ForgetPassword", "B2BMobile");
                    }
                    else
                    {
                        ViewBag.Result = 1; //Activate pass  
                    }
                }
                else
                {
                    ViewBag.Result = 4; //Error
                }
                GetStatusUser();
                GetCookie();
                ViewBag.MemberID = MemberID;
                return View();
            }
            else
            {
                return Redirect("ErrorPage");
            }
        }

        [HttpPost]
        public ActionResult UpdatePassword(FormCollection form)
        {
            var newPass = form["NewPassword"];
            var comPass = form["ConfirmPassword"];
            var mem = form["member"];

            EncryptManager encrypt = new EncryptManager();
            Hashtable data = new Hashtable();
            try
            {
                if (newPass == comPass)
                {
                    if (svMember.UpdateByCondition<emMemberActivate>("RowFlag = 2", "MemberID =" + mem + " AND RowFlag =1"))
                    {
                        if (svMember.UpdateByCondition<emMember>("Password ='" + encrypt.EncryptData(newPass) + "'", "MemberID =" + mem + ""))
                        {
                            var member = new Ouikum.emMember();
                            var emMembers = svMember.SelectData<emMember>("*", " MemberID = " + mem + "");
                            member = emMembers.First();
                            FirstLogin(member.UserName, newPass);

                            data.Add("result", svMember.IsResult);
                            data.Add("ErrorMsg", "เปลี่ยนรหัสผ่านเสร็จเรียบร้อย");
                            return Json(data);
                        }
                        else
                        {
                            data.Add("result", false);
                            data.Add("ErrorMsg", res.Common.lblerror_tryagian);
                            ViewBag.Result = 4; //Error
                        }
                    }
                    else
                    {
                        data.Add("result", false);
                        data.Add("ErrorMsg",res.Common.lblerror_tryagian);
                        ViewBag.Result = 4; //Error
                    }
                }
                else
                {
                    data.Add("result", false);
                    data.Add("ErrorMsg",res.JS.vldsame_value);
                    ViewBag.Result = 4; //Error
                }
            }
            catch (Exception ex)
            {
                data.Add("result", false);
                data.Add("ErrorMsg",res.JS.vldsame_value);
                ViewBag.Result = 4; //Error
            }
            return Content(PartialViewToString("ActivateUC"));
        }

        #endregion
        #region SignOut

        public ActionResult SignOut()
        {
            RemoveCookie();                                
            return Redirect("Index");
        }

        #endregion

        // Product //
       
        #region Product Detail

        public ActionResult ProductDetail(int id, int CompID)
        {
            GetStatusUser();
            GetCookie();
            RememberURL();
            SearchType();
            StatusMenu();

            int PageCheck = 2;
            CheckPage(id, PageCheck);

            var pd = svProduct.SelectData<view_Product>("*", "IsDelete = 0 and ProductID = " + id);

            var ProdImg = new List<view_ProductImage>();
            var Company = new view_Company();
            var Product = new view_Product();
            //var emMessage = new emMessage();
            var emMembers = new emMember();
            var b2bCompany = new b2bCompany();

            #region Get Cookie Check Sign In
            string MemberID = DataManager.ConvertToString(LogonMemberID);
            if (MemberID != "0")
            {
                emMembers = svMember.SelectData<emMember>("MemberID,UserName,FirstName,LastName,AddrLine1,AddrLine2,SubDistrict,DistrictID,PostalCode,Email,Phone", "IsDelete = 0 AND MemberID =" + MemberID, null, 1, 1).First();
                b2bCompany = svCompany.SelectData<b2bCompany>("MemberID,CompID,CompName", "IsDelete = 0 AND MemberID =" + MemberID, null, 1, 1).First();
                //emMessage = svMessage.SelectData<emMessage>("*", "IsDelete = 0 and FromCompID = " + b2bCompany.CompID, null, 1, 1).First();
            }
            #endregion

            if (pd.Count > 0)
            {
                ProdImg = svProduct.SelectData<view_ProductImage>("*", "IsDelete = 0 and ProductID = " + id);
                Product = pd.First();

                var ListCompany = svCompany.SelectData<view_Company>("*", "IsDelete = 0 and CompID = " + CompID);
                if (ListCompany.Count > 0)
                {
                    Company = ListCompany.First();
                }
            }
            ViewBag.QtyUnits = svCommon.SelectEnum(CommonService.EnumType.QtyUnits);
            ViewBag.Status = emMembers;
            ViewBag.StatusComp = b2bCompany;
            //ViewBag.StatusMenu = emMessage;
            ViewBag.Product = Product;
            ViewBag.ProdImg = ProdImg;
            ViewBag.Company = Company;
            ViewBag.Title = Product.ProductName + " | " + Product.CategoryName + " | " + Product.ShortDescription.Replace('~', ' ').Substring(0, Product.ShortDescription.Length - 1) + " | " + Product.CompName + " | " + Product.ProvinceName + " | " + res.Common.lblDomainShortName;

            return View();
        }

        #endregion

        // Company //

        #region Company Detail

        public ActionResult CompanyDetail(int? id)
        {
            GetStatusUser();
            GetCookie();
            RememberURL();
            SearchType();
            StatusMenu();

            int PageCheck = 1;
            CheckPage(id, PageCheck);

            var pd = svProduct.SelectData<view_Product>("*", "IsDelete = 0 and CompID = " + id);

            var ProdImg = new List<view_SearchProduct>();
            var Company = new view_Company();
            var Product = new view_Product();
            //var emMessage = new emMessage();
            var emMembers = new emMember();
            var b2bCompany = new b2bCompany();

            #region Get Cookie Check Sign In
            string MemberID = DataManager.ConvertToString(LogonMemberID);
            if (MemberID != "0")
            {
                emMembers = svMember.SelectData<emMember>("MemberID,UserName,FirstName,LastName,Email,Phone", "IsDelete = 0 AND MemberID =" + MemberID, null, 1, 1).First();
                b2bCompany = svCompany.SelectData<b2bCompany>("MemberID,CompID,CompName", "ISDelete = 0 AND MemberID =" + MemberID, null, 1, 1).First();
                //emMessage = svMessage.SelectData<emMessage>("*", "IsDelete = 0 and FromCompID = " + b2bCompany.CompID, null, 1, 1).First();
            }
            #endregion

            var ListCompany = svCompany.SelectData<view_Company>("*", "IsDelete = 0 and CompID = " + id);

            if (ListCompany.Count > 0)
            {
                Company = ListCompany.First();

                if (pd.Count > 0)
                {
                    ProdImg = svProduct.SelectData<view_SearchProduct>("*", "IsDelete = 0 and IsShow = 1 and IsJunk = 0 and CompID = " + id);

                    Product = pd.First();

                }
            }

            ViewBag.QtyUnits = svCommon.SelectEnum(CommonService.EnumType.QtyUnits);
            ViewBag.Status = emMembers;
            ViewBag.StatusComp = b2bCompany;
            //ViewBag.StatusMenu = emMessage;
            ViewBag.Product = Product;
            ViewBag.ProdImg = ProdImg;
            ViewBag.Company = Company;
            ViewBag.title = Company.CompName + " | " + Company.ProvinceName + " | " + res.Common.lblDomainShortName;
            ViewBag.SearchType = "Company";

            return View();
        }

        #endregion

        // Message Center //

        #region Message List

        [HttpGet]
        public ActionResult ListMessage(string MsgType = "Inbox", int? Sort = 1, int PIndex = 1, int PSize = 20)
        {
            MessageService svMessage = new MessageService();
            RememberURL();
            if (!CheckIsLogin())
            {
                return Redirect("SignIn");
            }
            else
            {
                GetStatusUser();
                GetCookie();
                SearchType();
                StatusMenu();
                CountMobileMessage();
                SetPager();

                var emMembers = new emMember();
                var b2bCompany = new b2bCompany();

                #region Get Cookie Check Sign In
                string MemberID = DataManager.ConvertToString(LogonMemberID);
                if (MemberID != "0")
                {
                    emMembers = svMember.SelectData<emMember>("MemberID,UserName,FirstName,LastName,Email,Phone", "IsDelete = 0 AND MemberID =" + MemberID, null, 1, 1).First();
                    b2bCompany = svCompany.SelectData<b2bCompany>("MemberID,CompID,CompName", "ISDelete = 0 AND MemberID =" + MemberID, null, 1, 1).First();
                }
                #endregion 

                List<view_Message> Messages;
                if (MsgType == "Inbox")
                {
                    var SQLWhere = svMessage.CreateWhereAction(MessageStatus.Inbox, LogonCompID);
                    Messages = svMessage.SelectData<view_Message>("*", SQLWhere, null, PIndex, PSize);
                }
                else
                {
                    var SQLWhere = svMessage.CreateWhereAction(MessageStatus.All, LogonCompID);
                    Messages = svMessage.SelectData<view_Message>("*", SQLWhere, null, PIndex, PSize);
                }

                ViewBag.Messages = Messages;
                ViewBag.PageIndex = PIndex;
                ViewBag.TotalPage = svMessage.TotalPage;
                ViewBag.TotalRow = svMessage.TotalRow;
                return View();
            }
        }

        #endregion
        #region Message Detail

        [HttpGet]
        public ActionResult MessageDetail(int MessageID = 0, string MessageCode = "", string MsgType = "Inbox")
        {
            RememberURL();
            if (!CheckIsLogin())
            {
                return Redirect("SignIn");
            }
            else
            {
                GetStatusUser();
                GetCookie();
                SearchType();
                StatusMenu();

                var emMembers = new emMember();
                var b2bCompany = new b2bCompany();
                var ViewMessage = new view_Message();
                var emMessage = new emMessage();
                var Company = new view_Company();
                var QuoProduct = new view_Quotation();
                var Product = new view_Product();

                #region Get Cookie Check Sign In
                string MemberID = DataManager.ConvertToString(LogonMemberID);
                if (MemberID != "0")
                {
                    emMembers = svMember.SelectData<emMember>("MemberID,UserName,FirstName,LastName,Email,Phone", "IsDelete = 0 AND MemberID =" + MemberID, null, 1, 1).First();
                    b2bCompany = svCompany.SelectData<b2bCompany>("MemberID,CompID,CompName", "ISDelete = 0 AND MemberID =" + MemberID, null, 1, 1).First();
                }
                #endregion                              

                if (MessageID > 0)
                {
                    var MessageList = svMessage.SelectData<view_Message>("*", "IsDelete = 0 AND MessageID =" + MessageID);
                    if (MessageList.Count > 0)
                    {
                        ViewMessage = MessageList.First();
                        ViewBag.Message = ViewMessage;

                        Company = svCompany.GetByID<view_Company>("CompID", ViewMessage.FromCompID.ToString());
                        Product = svProduct.GetByID<view_Product>("ProductID", QuoProduct.ProductID.ToString());  
                    }

                    #region Update IsRead(Status)
                    var Update_MessageList = svMessage.SelectData<emMessage>("*", "IsDelete = 0 AND MessageID =" + MessageID);
                    if (Update_MessageList.Count > 0)
                    {
                        emMessage = Update_MessageList.First();
                        emMessage.IsRead = true;
                        svMessage.SaveData<emMessage>(emMessage, "MessageID");   
                    }
                    #endregion

                    ViewBag.QtyUnits = svCommon.SelectEnum(CommonService.EnumType.QtyUnits);
                    ViewBag.Company = Company;
                    ViewBag.Product = Product;
                    ViewBag.Status = emMembers;
                    ViewBag.StatusComp = b2bCompany;
                    ViewBag.MsgType = MsgType;
                    ViewBag.msgtitle = ViewMessage.Subject;
                    CountMessage();
                    return View();
                }
                else
                {
                    return Redirect("ListMessage?MsgType=Inbox");
                }
            }
        }

        #endregion
        #region Delete Message
        
        [HttpPost]
        public ActionResult DelMessage(int ID)
        {
            var emMessage = new emMessage();

            svMessage.UpdateByCondition<emMessage>("IsDelete = 1 ", "MessageID = " + ID);
            return Json(new{Result = true});
        }

        #endregion
        #region Get Message for Reply
        public ActionResult ReplyMessage(int msgid, string type = "Reply")
        {
            Hashtable data = new Hashtable();
            MessageService svMessage = new MessageService();
            string SqlWhere = "MessageID = " + msgid + " AND IsDelete = 0";
            var Message = svMessage.SelectData<view_Message>("*", SqlWhere).First();

            #region Prefix for Reply
            string prefixType = "";
            //check have prefix  // Check Subject
            var subSubject = Message.Subject;
            subSubject = subSubject.Substring(0, 4);

            if (type == "Reply" && subSubject != "RE: ")
            {
                prefixType = "RE: ";//4 character
                data.Add("msgSubject", prefixType + Message.Subject);
            }
            else
            {
                data.Add("msgSubject", Message.Subject);
            }
            #endregion

            #region set history message

            if (type == "Reply")
            {
                string toEmail = "";
                string toName = "";
                if (Message.FromCompID == 0)
                {
                    toEmail = Message.FromName;
                    string[] strEmail = toEmail.Split('(');
                    toName = strEmail[0].Substring(0, strEmail[0].Length - 1);
                    toEmail = strEmail[1].Substring(0, strEmail[1].Length - 1);
                }
                string historydetail = "<br><hr>From : " + Message.FromName;
                historydetail += "<br>Sent : " + Message.SendDate;
                historydetail += "<br>To : " + Message.ToCompName;
                historydetail += "<br>Subject : " + Message.Subject;
                historydetail += "<br><br>" + Message.MsgDetail;

                data.Add("msgDetail", historydetail);
                data.Add("msgToCompID", Message.ToCompID);
                data.Add("msgToCompName", Message.ToCompName);
                data.Add("msgFromCompID", Message.FromCompID);
                data.Add("msgFromCompName", Message.FromCompName);
                data.Add("emailNotMember", toEmail);
                data.Add("nameNotMember", toName);
            }
            else
            {
                string historydetail = "<br><hr>From : " + Message.FromName;
                historydetail += "<br>Sent : " + Message.SendDate;
                historydetail += "<br>To : " + Message.ToCompName;
                historydetail += "<br>Subject : " + Message.Subject;
                historydetail += "<br><br>" + Message.MsgDetail;

                data.Add("msgDetail", historydetail);
            }

            #endregion

            return Json(data);
        }
        #endregion
        #region Post Contact for New
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult MessageContact(
            string hidToCompID,
            string MessageDetail, 
            string IsImportance,
            string txtSubject,
            string txtFromName,
            string txtFromContactPhone,
            string txtToCompName,
            string hidToCompEmail,
            string txtFromEmail
            )
        {
            Ouikum.Message.MessageService svMessage = new Ouikum.Message.MessageService();
            var emMessageSender = new emMessage();
            var emMessageReceiver = new emMessage();
            int ToCompID = DataManager.ConvertToInteger(hidToCompID);
            string msgdetail = MessageDetail;
            if (DataManager.ConvertToBool(IsImportance) == true)
            {
                msgdetail += "<p><strong>" + res.Message_Center.lblContactImmediately + "</strong></p>";
            }

            #region Set Message

            if (LogonCompID > 0)
            {
                #region Sender
                emMessageSender.ToCompID = ToCompID;
                emMessageSender.Subject = txtSubject;
                emMessageSender.MsgDetail = msgdetail;
                emMessageSender.RootMessageID = 0;
                emMessageSender.MessageCode = emMessageSender.ToCompID + "-" + GetTimeStamp() + "-" + svMessage.Generate_MessageCode();
                emMessageSender.MsgStatus = "2";
                emMessageSender.IsSend = true;
                emMessageSender.SendDate = DateTimeNow;
                emMessageSender.IsFavorite = DataManager.ConvertToBool(IsImportance);
                emMessageSender.FromCompID = LogonCompID;
                emMessageSender.FromName = txtFromName;
                emMessageSender.FromContactPhone = txtFromContactPhone;
                //new//
                emMessageSender.MsgFolderID = 2;
                emMessageSender.FromEmail = LogonEmail;
                emMessageSender.IsAttach = false;

                emMessageSender = svMessage.InsertMessage(emMessageSender);
                #endregion

                #region Receiver
                emMessageReceiver.ToCompID = ToCompID;
                emMessageReceiver.Subject = txtSubject;
                emMessageReceiver.MsgDetail = msgdetail;
                emMessageReceiver.RootMessageID = 0;
                emMessageReceiver.MessageCode = emMessageSender.MessageCode;
                emMessageReceiver.MsgStatus = "1";
                emMessageReceiver.IsSend = false;
                emMessageReceiver.SendDate = DateTimeNow;
                emMessageReceiver.IsFavorite = DataManager.ConvertToBool(IsImportance);
                emMessageReceiver.FromCompID = LogonCompID;
                emMessageReceiver.FromName = txtFromName;
                //new//
                emMessageReceiver.FromEmail = LogonEmail;
                emMessageReceiver.MsgFolderID = 1;
                emMessageReceiver.IsAttach = false;
                emMessageReceiver.FromContactPhone = txtFromContactPhone;
                #endregion

                if (ToCompID > 0)
                {
                    #region Save Message
                    emMessageReceiver = svMessage.InsertMessage(emMessageReceiver);
                    #endregion

                    #region Send Email
                    SendEmail(emMessageReceiver, txtToCompName, hidToCompEmail);
                    #endregion
                }
            }
            else
            {
                #region Receiver
                emMessageReceiver.ToCompID = ToCompID;
                emMessageReceiver.Subject = txtSubject;
                emMessageReceiver.MsgDetail = msgdetail;
                emMessageReceiver.RootMessageID = 0;
                emMessageReceiver.MessageCode = emMessageReceiver.ToCompID + "-" + GetTimeStamp() + "-" + svMessage.Generate_MessageCode();
                emMessageReceiver.MsgStatus = "1";
                emMessageReceiver.IsSend = false;
                emMessageReceiver.SendDate = DateTimeNow;
                emMessageReceiver.IsFavorite = DataManager.ConvertToBool(IsImportance);
                emMessageReceiver.FromCompID = 0;
                //new//
                emMessageReceiver.MsgFolderID = 1;
                emMessageReceiver.FromContactPhone = txtFromContactPhone;
                emMessageReceiver.FromEmail = txtFromEmail;
                emMessageReceiver.IsAttach = false;

                if (!string.IsNullOrEmpty(txtFromName))
                {
                    emMessageReceiver.FromName = txtFromName + ",(" + txtFromEmail + ")";
                }
                else
                {
                    emMessageReceiver.FromName = txtFromEmail;
                }
                emMessageReceiver.FromContactPhone = txtFromContactPhone;
                #endregion

                if (ToCompID > 0)
                {
                    #region Save Message
                    emMessageReceiver = svMessage.InsertMessage(emMessageReceiver);
                    #endregion

                    #region Send Email
                    SendEmail(emMessageReceiver, txtToCompName, hidToCompEmail);
                    #endregion
                }
            }

            #endregion

            return Json(true);
        }

        #endregion
        #region Post Contact for Reply
        [ValidateInput(false)]
        [HttpPost]
        public bool MessageNew
            (
            string hidToCompID, 
            string hidMsgID, 
            string MsgDetail, 
            string txtSubject
            )
        {
            var IsResult = false;
            if (LogonCompID > 0)
            {
                Ouikum.Message.MessageService svMessage = new Ouikum.Message.MessageService();
                svCompany = new CompanyService();
                string ToCompID = hidToCompID;
                string[] sub_ToCompID = ToCompID.Split(',');
                int rootMsgID = DataManager.ConvertToInteger(hidMsgID);
                string sqlWhere = "";
                string msgdetail = MsgDetail;
                string msgStatus = "Reply";
                var comp = svCompany.SelectData<b2bCompany>("CompID,CompPhone,CompName,ContactFirstName,ContactEmail", "CompID = " + LogonCompID).First();
                for (int i = 0; i < sub_ToCompID.Length; i++)
                {
                    var emMessageSender = new emMessage();
                    var emMessageReceiver = new emMessage();

                    #region Sender Message
                    emMessageSender.ToCompID = DataManager.ConvertToInteger(sub_ToCompID[i]);
                    emMessageSender.FromCompID = LogonCompID;
                    emMessageSender.IsFavorite = DataManager.ConvertToBool(false);
                    emMessageSender.Subject = txtSubject;
                    emMessageSender.MsgDetail = msgdetail;
                    emMessageSender.RootMessageID = (rootMsgID != 0) ? rootMsgID : 0;
                    emMessageSender.MessageCode = emMessageSender.ToCompID + "-" + GetTimeStamp() + "-" + svMessage.Generate_MessageCode();
                    emMessageSender.MsgStatus = "2";
                    emMessageSender.IsSend = true;
                    emMessageSender.FromName = comp.ContactFirstName != null ? comp.ContactFirstName : comp.CompName;
                    //new//
                    emMessageSender.MsgFolderID = 2;
                    emMessageSender.FromEmail = comp.ContactEmail;
                    emMessageSender.IsAttach = false;
                    emMessageSender.FromContactPhone = comp.CompPhone;
                    #region Save Sender Message
                    if (msgStatus == "Reply")
                    {
                        emMessageSender = svMessage.InsertMessageReply(emMessageSender, "Reply");
                    }
                    else
                    {
                        emMessageSender = svMessage.InsertMessage(emMessageSender);
                    }
                    #endregion

                    #endregion

                    #region Receiver Message
                    emMessageReceiver.ToCompID = DataManager.ConvertToInteger(sub_ToCompID[i]);
                    emMessageReceiver.FromCompID = LogonCompID;
                    emMessageReceiver.Subject = txtSubject;
                    emMessageReceiver.MsgDetail = MsgDetail;
                    emMessageReceiver.IsFavorite = DataManager.ConvertToBool(false);
                    emMessageReceiver.RootMessageID = (rootMsgID != 0) ? rootMsgID : 0;
                    emMessageReceiver.MessageCode = emMessageSender.MessageCode;
                    emMessageReceiver.MsgStatus = "1";
                    emMessageReceiver.IsSend = false;
                    emMessageReceiver.FromName = comp.ContactFirstName != null ? comp.ContactFirstName : comp.CompName;

                    //new//
                    emMessageReceiver.MsgFolderID = 1;
                    emMessageReceiver.FromContactPhone = comp.CompPhone;
                    emMessageReceiver.FromEmail = comp.ContactEmail;
                    emMessageReceiver.IsAttach = false;

                    #region Save Receiver Message

                    if (msgStatus == "Reply")
                    {
                        emMessageReceiver = svMessage.InsertMessageReply(emMessageReceiver, "Reply");
                    }
                    else
                    {
                        emMessageReceiver = svMessage.InsertMessage(emMessageReceiver);
                    }
                    #endregion

                    #endregion

                    if (emMessageReceiver.ToCompID > 0)
                    {
                        #region GetToCompName
                        sqlWhere = "CompID = " + emMessageReceiver.ToCompID + " AND IsDelete = 0";
                        var Company = svCompany.SelectData<b2bCompany>("CompID,CompName,ContactEmail", sqlWhere).First();
                        var toCompName = Company.CompName;
                        var toCompEmail = Company.ContactEmail;
                        #endregion

                        #region Send Email
                        if (svMessage.IsResult)
                        {
                            IsResult = SendEmail(emMessageReceiver, toCompName, toCompEmail);
                        }
                        #endregion
                    }
                }

                GetStatusUser();
                GetCookie();
                CountMessage();       
            }
            else
            {         
                IsResult = false;         
            }

            return IsResult;
        }
        #endregion
        #region SendEmail
        public bool SendEmail(emMessage model, string toCompName, string toCompEmail)
        {
            #region variable
            bool IsSend = true;
            var Detail = "";
            var url = "";
            var mailTo = new List<string>();
            List<string> mailToAdmin = GetMailListB2BAdmin();
            var mailCC = new List<string>();
            var Sender = "";
            var Receiver = "";
            svCompany = new CompanyService();
            string fromName = "";
            string fromPhone = "";
            string fromEmail = "";
            #endregion
            #region set from info
            if (model.FromCompID > 0)
            {
                string sqlselect = "CompID,CompName,ContactFirstName,ContactLastName,ContactEmail,ContactPhone,CompPhone";
                string sqlwhere = "CompID = " + model.FromCompID + " AND IsDelete = 0";
                var fromcomp = svCompany.SelectData<b2bCompany>(sqlselect, sqlwhere).First();
                if (!string.IsNullOrEmpty(model.FromContactPhone))
                {
                    fromPhone = model.FromContactPhone;
                }
                else if (!string.IsNullOrEmpty(fromcomp.ContactPhone))
                {
                    fromPhone = fromcomp.ContactPhone;
                }
                else
                {
                    fromPhone = fromcomp.CompPhone;
                }
            }
            else
            {
                fromPhone = model.FromContactPhone;
            }
            Sender = model.FromName;
            string[] subName = model.FromName.Split('(');
            if (subName.Length > 1)
            {
                fromName = subName[0].Substring(0, subName[0].Length);
                fromEmail = subName[1].Substring(0, subName[1].Length);
            }
            else
            {
                fromName = subName[0].Substring(0, subName[0].Length);
                fromEmail = "";
            }

            #endregion

            Receiver = toCompName;

            #region Set Content & Value For Send Email
            string urlb2bthai = res.Pageviews.UrlWeb;
            url = urlb2bthai + "/Message/Detail?MessageID=" + model.MessageID + "&MessageCode=" + model.MessageCode + "&MsgType=Inbox";
            //test path logo
            string pathlogo = urlb2bthai + res.Message_Center.path_logo;

            string Subject = res.Message_Center.lblKhun + " " + fromName + " " + res.Message_Center.lblSuject_3;
            Detail = "<table ><tr><td>" + res.Common.lblDear + " " + toCompName + ",</td></tr>";
            Detail += "<tr><td><br>" + res.Message_Center.lblKhun + " " + fromName + "</td></tr>";
            if (!string.IsNullOrEmpty(fromPhone))
            {
                Detail += "<tr><td>" + res.Common.lblTel + " " + fromPhone + "</td></tr>";
            }
            Detail += "<tr><td>" + res.Common.lblEmail + " " + fromEmail + "</td></tr>";
            Detail += "<tr><td><br>" + res.Message_Center.lblMsg_follows + " </td></tr>";
            Detail += "<tr><td>" + model.MsgDetail + "</td></tr>";
            Detail += "<tr><td><br><br>" + res.Message_Center.lblWantToViewMsg + " <a href=\"" + url + "\" >" + url + "</a><br></td></tr>";
            Detail += "<tr><td><br><b>" + res.Message_Center.lblEmailNote + " </b>" + res.Message_Center.lblEmailNoteDetail + " </td></tr>";
            Detail += "<tr><td><br></td></tr>";
            Detail += "<tr><td><br>" + res.Message_Center.lblSincerely + "<br><a href=\"" + urlb2bthai + "\" ><img src=\"" + pathlogo + "\"/></a><br>" + "<a href=\"" + urlb2bthai + "\" >" + res.Message_Center.lblTeam + "</a></td></tr></table>";

            var mailFrom = res.Config.EmailNoReply;
            mailTo.Add(toCompEmail);
            #endregion

            IsSend = OnSendByAlertEmail(Subject, mailFrom, mailTo, mailCC, Detail);

            url = urlb2bthai + "/Admin/Stat/MessageDetail?MessageID=" + model.MessageID + "&MessageCode=" + model.MessageCode;
            Subject = res.Message_Center.lblKhun + " " + Sender + " " + res.Message_Center.lblContacted + " " + Receiver + " " + res.Message_Center.lblVia;
            Detail = "<table ><tr><td>" + res.Message_Center.lblKhun + " " + Sender + " " + res.Message_Center.lblContacted + " " + Receiver + " " + res.Message_Center.lblVia + " </td></tr>";
            Detail += "<tr><td><br>" + model.MsgDetail + "</td></tr>";
            Detail += "<tr><td><br><br>" + res.Message_Center.lblWantToViewMsg + " <a href=\"" + url + "\" >" + url + "</a><br></td></tr>";
            Detail += "</table>";
            IsSend = OnSendByAlertEmail(Subject, mailFrom, mailToAdmin, mailCC, Detail);

            return IsSend;
        }
        #endregion

        // Quotation //

        #region Request Price

        [HttpPost]
        public ActionResult RequestPrice(
            int ProductID,
            string Qty,
            string QtyUnit,
            int ToCompID,
            int? FromCompID,
            string CompanyName,
            string ReqFirstName,
            string ReqEmail,
            string ReqPhone,
            string ReqLastName,
            string ReqAddrLine1,
            string ReqAddrLine2,
            string ReqSubDistrict,
            int? ReqDistrictID,
            string ReqPostalCode, 
            string Remark,
            int IsSentEmail,
            int IsTelephone,
            int IsAttach,
            int IsPublic
            )
        {
            Hashtable data = new Hashtable();
            var model = new b2bQuotation();
            var model2 = new b2bQuotation();
            try
            {
                #region Insert Quotation Code
                svQuotation = new QuotationService();
                var count = svQuotation.SelectData<b2bQuotation>(" * ", " CreatedDate = GetDate() AND RowFlag > 0");
                int Count = count.Count + 1;
                string Code = AutoGenCode("QO", Count);
                model.QuotationCode = Code;
                model.RootQuotationCode = Code;
                #endregion

                #region Check Company
                var CompName = "";
                if (CompanyName == "" || CompanyName == null)
                {
                    CompName = "ไม่ระบุชื่อบริษัท";
                }
                else
                {
                    CompName = CompanyName;
                }
                model.CompanyName = CompName;
                model2.CompanyName = CompName;
                #endregion

                #region ข้อมูลร้องขอราคาสินค้า[Outbox]
                model.ProductID = ProductID;
                model.Qty = (short)DataManager.ConvertToSingle(Qty);
                model.QtyUnit = QtyUnit;
                model.ToCompID = ToCompID;
                model.FromCompID = FromCompID;
                model.ReqFirstName = ReqFirstName;
                model.ReqEmail = ReqEmail;
                model.ReqPhone = ReqPhone;
                model.ReqLastName = ReqLastName;
                model.ReqAddrLine1 = ReqAddrLine1;
                model.ReqAddrLine2 = ReqAddrLine2;
                model.ReqSubDistrict = ReqSubDistrict;
                model.ReqDistrictID = Convert.ToInt32(ReqDistrictID);
                model.ReqPostalCode = ReqPostalCode;
                model.IsMatching = false;
                if (!string.IsNullOrEmpty(Remark)) { model.ReqRemark = Remark; }
                model.IsSentEmail = false;
                model.IsTelephone = Convert.ToBoolean(IsTelephone);
                model.IsAttach = Convert.ToBoolean(IsAttach);
                model.IsAttachQuote = false;
                model.IsEmail = false;
                model.IsReply = false;
                model.IsRead = false;
                model.IsReject = false;
                model.IsImportance = false;
                model.IsPDFView = false;
                model.IsOutbox = true;
                model.SendDate = DateTimeNow;
                model.IsClosed = false;
                model.IsPublic = Convert.ToBoolean(IsPublic);
                model.QuotationStatus = "R";
                model.QuotationFolderID = 2;
                #endregion

                #region ข้อมูลร้องขอราคาสินค้า[Inbox]
                model2.ProductID = ProductID;
                model2.Qty = (short)DataManager.ConvertToSingle(Qty);
                model2.QtyUnit = QtyUnit;
                model2.ToCompID = ToCompID;
                model2.FromCompID = FromCompID;
                model2.ReqFirstName = ReqFirstName;
                model2.ReqEmail = ReqEmail;
                model2.ReqPhone = ReqPhone;
                model2.ReqLastName = ReqLastName;
                model2.ReqAddrLine1 = ReqAddrLine1;
                model2.ReqAddrLine2 = ReqAddrLine2;
                model2.ReqSubDistrict = ReqSubDistrict;
                model2.ReqDistrictID = Convert.ToInt32(ReqDistrictID);
                model2.ReqPostalCode = ReqPostalCode;
                model2.IsMatching = false;
                if (!string.IsNullOrEmpty(Remark)) { model2.ReqRemark = Remark; }
                model2.IsSentEmail = false;
                model2.IsTelephone = Convert.ToBoolean(IsTelephone);
                model2.IsAttach = Convert.ToBoolean(IsAttach);
                model2.IsAttachQuote = false;
                model2.IsEmail = false;
                model2.IsReply = false;
                model2.IsRead = false;
                model2.IsReject = false;
                model2.IsImportance = false;
                model2.IsPDFView = false;
                model2.IsOutbox = false;
                model2.SendDate = DateTimeNow;
                model2.IsClosed = false;
                model2.IsPublic = Convert.ToBoolean(IsPublic);
                model2.QuotationStatus = "R";
                model2.QuotationCode = Code;
                model2.RootQuotationCode = Code;
                model2.QuotationFolderID = 1;
                #endregion

                #region Insert Quotation
                svQuotation.InsertQuotation(model);
                svQuotation.InsertQuotation(model2);
                #endregion

                #region Update ContactCount
                if (ProductID > 0)
                {
                    AddContactCount(ProductID);
                }
                #endregion

                return Json(new { IsSuccess = true, Result = "ร้องขอราคาสินค้าสำเร็จ กรุณารอการเสนอราคาสินค้ากลับจากผู้ขาย ผ่านข้อมูลการติดต่อของคุณ" });
            }
            catch (Exception ex)
            {
                CreateLogFiles(ex);
                return Json(new { IsSuccess = false, Result = "เกิดข้อผิดพลาดในการร้องขอราคาสินค้า โปรดร้องขอราคาอีกครั้ง" });
            }
        }

        #endregion
        #region Quotation Detail

        public ActionResult QuotationDetail(int? ID, int? QuoID)
        {
            GetStatusUser();
            GetCookie();
            RememberURL();
            SearchType();
            StatusMenu();

            var emMembers = new emMember();
            var b2bCompany = new b2bCompany();
            var Company = new view_Company();
            var QuoProduct = new view_Quotation();
            var b2bQuotation = new b2bQuotation();
            var Product = new view_Product();

            //var QuotationDetail = new view_Quotation();
                                                                                                                 

            #region Get Cookie Check Sign In
            string MemberID = DataManager.ConvertToString(LogonMemberID);
            if (MemberID != "0")
            {
                emMembers = svMember.SelectData<emMember>("MemberID,UserName,FirstName,LastName,Email,Phone", "IsDelete = 0 AND MemberID =" + MemberID, null, 1, 1).First();
                b2bCompany = svCompany.SelectData<b2bCompany>("MemberID,CompID,CompName", "ISDelete = 0 AND MemberID =" + MemberID, null, 1, 1).First();
            }
            #endregion                              

            var ListQuotation = svQuotation.SelectData<view_Quotation>("*", "IsDelete = 0 and QuotationID = " + QuoID);
            if (ListQuotation.Count > 0)
            {
                QuoProduct = ListQuotation.First();
                Company = svCompany.GetByID<view_Company>("CompID", QuoProduct.FromCompID.ToString());
                Product = svProduct.GetByID<view_Product>("ProductID", QuoProduct.ProductID.ToString());
            }

            #region Update Status
            var UpdateQuo = svQuotation.SelectData<b2bQuotation>("*", "IsDelete = 0 and QuotationID = " + QuoID);
            if (UpdateQuo.Count > 0)
            {
                UpdateQuo.First().IsRead = true;
                svQuotation.SaveData<b2bQuotation>(UpdateQuo, "QuotationID");
            }
            #endregion



            ViewBag.QtyUnits = svCommon.SelectEnum(CommonService.EnumType.QtyUnits);
            ViewBag.Status = emMembers;
            ViewBag.StatusComp = b2bCompany;      
            ViewBag.Company = Company;
            ViewBag.QuoDetail = QuoProduct;
            ViewBag.Product = Product;
            ViewBag.MemID = MemberID;
            ViewBag.SearchType = "Request";

            return View();
        }

        #endregion
        #region Post Quotation Detail

        [HttpPost]
        public ActionResult SaveQuotation(
            int? QuotationID,
            string PricePerPiece,
            string TotalPrice,
            decimal? Discount,
            decimal? Vat,
            int? IsSentEmail,
            string SaleName,
            string SaleEmail,
            string SalePhone
        )
        {
            Hashtable data = new Hashtable();
            svQuotation = new QuotationService();
            var model = new b2bQuotation();
            var model2 = new b2bQuotation();
            var model3 = new b2bQuotation();
            int FromCompID = 0;
            string FromCompName = "";
            string ReqFirstName = "";
            string ReqPhone = "";
            string ReqEmail = "";
            string LastName = "";
            string AddrLine1 = "";
            string AddrLine2 = "";
            string SubDistrict = "";
            int? DistrictID = 0;
            string PostalCode = "";

            var Quotation = svQuotation.SelectData<b2bQuotation>("*", "QuotationID = " + QuotationID, "CreatedDate", 1, 0);

            if (Quotation.Count > 0)
            {
                model = Quotation.First();
                FromCompID = Convert.ToInt16(model.ToCompID);
                FromCompName = model.CompanyName;
                ReqFirstName = model.ReqFirstName;
                ReqPhone = model.ReqPhone;
                ReqEmail = model.ReqEmail;
                LastName = model.ReqLastName;
                AddrLine1 = model.ReqAddrLine1;
                AddrLine2 = model.ReqAddrLine2;
                SubDistrict = model.ReqSubDistrict;
                DistrictID = model.ReqDistrictID;
                PostalCode = model.ReqPostalCode;
            }
            try
            {
                #region เสนอราคากลับ[Outbox]
                var count = svQuotation.SelectData<b2bQuotation>(" * ", " CreatedDate = GetDate() AND RowFlag > 0");
                int Count = count.Count + 1;
                string Code = AutoGenCode("QO", Count);

                model2.QuotationCode = Code;
                model2.RootQuotationCode = model.RootQuotationCode;
                model2.PricePerPiece = Convert.ToDecimal(PricePerPiece.Replace(",", ""));
                model2.Discount = Discount;
                model2.Vat = Vat;
                model2.TotalPrice = Convert.ToDecimal(TotalPrice.Replace(",", ""));
                model2.IsSentEmail = Convert.ToBoolean(IsSentEmail);
                model2.SendDate = DateTimeNow;
                model2.IsOutbox = true;

                model2.ProductID = model.ProductID;
                model2.Qty = model.Qty;
                model2.QtyUnit = model.QtyUnit;
                model2.ToCompID = model.FromCompID;

                model2.CompanyName = model.CompanyName;
                model2.ReqFirstName = model.ReqFirstName;
                model2.ReqPhone = model.ReqPhone;
                model2.ReqEmail = model.ReqEmail;
                model2.ReqLastName = LastName;
                model2.ReqAddrLine1 = AddrLine1;
                model2.ReqAddrLine2 = AddrLine2;
                model2.ReqSubDistrict = SubDistrict;
                model2.ReqDistrictID = DistrictID;
                model2.ReqPostalCode = PostalCode;
                model2.FromCompID = FromCompID;

                if (!string.IsNullOrEmpty(SaleEmail)) { model2.SaleEmail = SaleEmail; }
                if (!string.IsNullOrEmpty(SaleName)) { model2.SaleName = SaleName; }
                if (!string.IsNullOrEmpty(SalePhone)) { model2.SalePhone = SalePhone; }

                model2.IsMatching = false;
                model2.IsTelephone = false;
                model2.IsAttach = false;
                model2.IsAttachQuote = false;
                model2.IsEmail = false;
                model2.IsReply = false;
                model2.IsRead = false;
                model2.IsReject = false;
                model2.IsImportance = false;
                model2.IsPDFView = false;
                model2.IsOutbox = true;
                model2.IsClosed = model.IsClosed;
                model2.IsPublic = model.IsPublic;
                model2.QuotationStatus = "Q";
                model2.QuotationFolderID = 2;
                svQuotation.InsertQuotation(model2);
                #endregion

                #region เสนอราคากลับ[Inbox]
                model3.QuotationCode = Code;
                model3.RootQuotationCode = model.RootQuotationCode;
                model3.PricePerPiece = Convert.ToDecimal(PricePerPiece.Replace(",", ""));
                model3.Discount = Discount;
                model3.Vat = Vat;
                model3.TotalPrice = Convert.ToDecimal(TotalPrice.Replace(",", ""));
                model3.IsSentEmail = Convert.ToBoolean(IsSentEmail);
                model3.SendDate = DateTimeNow;
                model3.IsOutbox = true;

                model3.ProductID = model.ProductID;
                model3.Qty = model.Qty;
                model3.QtyUnit = model.QtyUnit;
                model3.ToCompID = model.FromCompID;

                model3.CompanyName = FromCompName;
                model3.ReqFirstName = ReqFirstName;
                model3.ReqPhone = ReqPhone;
                model3.ReqEmail = ReqEmail;
                model3.ReqLastName = LastName;
                model3.ReqAddrLine1 = AddrLine1;
                model3.ReqAddrLine2 = AddrLine2;
                model3.ReqSubDistrict = SubDistrict;
                model3.ReqDistrictID = DistrictID;
                model3.ReqPostalCode = PostalCode;
                model3.FromCompID = FromCompID;

                if (!string.IsNullOrEmpty(SaleEmail)) { model3.SaleEmail = SaleEmail; }
                if (!string.IsNullOrEmpty(SaleName)) { model3.SaleName = SaleName; }
                if (!string.IsNullOrEmpty(SalePhone)) { model3.SalePhone = SalePhone; }

                model3.IsMatching = false;
                model3.IsTelephone = false;
                model3.IsAttach = false;
                model3.IsAttachQuote = false;
                model3.IsEmail = false;
                model3.IsReply = true;
                model3.IsRead = false;
                model3.IsReject = false;
                model3.IsImportance = false;
                model3.IsPDFView = false;
                model3.IsOutbox = false;
                model3.IsClosed = model.IsClosed;
                model3.IsPublic = model.IsPublic;
                model3.QuotationStatus = "Q";
                model3.QuotationFolderID = 1;
                svQuotation.InsertQuotation(model3);
                #endregion

                if (svQuotation.IsResult)
                {
                    return Json(new { IsSuccess = true, Result = "ส่งข้อมูลเรียบร้อยแล้ว คุณสามารถตรวจสอบข้อมูลการเสนอราคาของคุณได้ที่กล่อง 'ส่งแล้ว'" });
                }
                else
                {
                    return Json(new { IsSuccess = false, Result = "เกิดข้อผิดพลาดในการร้องขอราคาสินค้า โปรดร้องขอราคาอีกครั้ง" });
                }
            }
            catch (Exception ex)
            {
                CreateLogFiles(ex);
                return Json(new { IsSuccess = false, Result = "เกิดข้อผิดพลาดในการร้องขอราคาสินค้า โปรดร้องขอราคาอีกครั้ง" });
            }
        }

        #endregion
        #region Delete Quotation

        [HttpPost]
        public ActionResult DelQuotation(int ID)
        {
            if (ID != null)
            {
                svQuotation.UpdateByCondition<view_Quotation>("IsDelete = 1 ", "QuotationID = " + ID);
                return Json(new { IsSuccess = true, Result = "ลบใบเสนอราคาสำเร็จ" });
            }
            else
            {
                return Json(new { IsSuccess = false, Result = "ลบใบเสนอราคาไม่สำเร็จ โปรดลองอีกครั้ง" });
            }
            
        }

        #endregion

        // Search //

        #region Search Product

        public ActionResult ListProduct(
            string textSearch,
            int? ProvinceID,
            int? BizTypeID,
            int? CategoryID,
            int? CheckGold
        )
        {
            GetStatusUser();
            GetCookie();
            RememberURL();
            SearchType();
            StatusMenu();

            string txtSearch = (textSearch != null) ? textSearch.Trim() : "";
            int ProvinID = (ProvinceID != null || ProvinceID == 0) ? (int)ProvinceID : 0;
            int CatID = (CategoryID != null || CategoryID == 0) ? (int)CategoryID : 0;
            int Complevel = (CheckGold != null || CheckGold == 0) ? (int)CheckGold : 0;
            int CateLevel = 1;

            if (CatID == 0)
            {
                CateLevel = 0;
            }

            int? Sort = 1; //Fix
            int PIndex = 1;
            int PSize = 20;

            string sqlSelect, sqlWhere, sqlOrderBy = "";

            sqlSelect = @"ProductID,ProductName,ProductCount,CompID,CompName,CompLevel,BizTypeName,CompProvinceID," +
                    "BizTypeOther,ShortDescription,ProductImgPath,ProvinceName,CateLV3,ViewCount,ModifiedDate,CreatedDate,ContactCount";

            #region DoWhereCause
            sqlWhere = svProduct.CreateWhereAction(ProductAction.FrontEnd, 0);
            sqlWhere += svProduct.CreateWhereCause(0, txtSearch, 0, 0, (int)CateLevel, (int)CategoryID, (int)BizTypeID, (int)Complevel, (int)ProvinceID);

            #endregion

            #region Sort By
            switch (Sort)
            {
                case 1:
                    sqlOrderBy += svProduct.CreateOrderby(Ouikum.Product.OrderBy.ModifiedDateDESC);
                    break;
                case 2:
                    sqlOrderBy += svProduct.CreateOrderby(Ouikum.Product.OrderBy.CreatedDateDESC);
                    break;
                case 3:
                    sqlOrderBy += svProduct.CreateOrderby(Ouikum.Product.OrderBy.ViewCountDESC);
                    break;
                case 4:
                    sqlOrderBy += svProduct.CreateOrderby(Ouikum.Product.OrderBy.ContactCountDESC);
                    break;
            }
            #endregion

            #region Get b2bProduct
            var b2bProducts = svProduct.SelectData<view_SearchProduct>(sqlSelect, sqlWhere, "Complevel DESC," + sqlOrderBy, PIndex, PSize);
            #endregion

            ViewBag.Status = svProduct.CodeError;
            ViewBag.SearchList = b2bProducts;
            ViewBag.TotalRow = svProduct.TotalRow;
            ViewBag.TextSearch = txtSearch;
            ViewBag.BizTypeID = BizTypeID;
            ViewBag.ProvinceID = ProvinceID;
            ViewBag.CategoryID = CategoryID;

            #region query Product Mongo
            //var mgKeywod = new KeywordMongo();
            //var mongoProducts = mgKeywod.SearchProductMongo(1, 1, 20, txtSearch, BizTypeID, CateLevel, CatID, Complevel, ProvinID);
            //var ProdSearchAll = MappingProductMongo(mongoProducts);
            //ViewBag.Status = svProduct.CodeError;
            //ViewBag.SearchList = ProdSearchAll;
            //ViewBag.TotalRow = mgKeywod.TotalRow;
            //ViewBag.TextSearch = txtSearch;
            //ViewBag.BizTypeID = BizTypeID;
            //ViewBag.ProvinceID = ProvinceID;
            //ViewBag.CategoryID = CategoryID;
            #endregion

            return View();
        }

        #endregion
        #region Search Company

        public ActionResult ListCompany(
            string textSearch,
            int? CategoryID,
            int? ProvinceID,
            int? BizTypeID,
            int? CheckGold,
            int? Sort = 1,
            int PIndex = 1,
            int PSize = 20
        )
        {
            GetStatusUser();
            GetCookie();
            RememberURL();
            SearchType();
            StatusMenu();

            #region Search Type Company
            var SqlWhere = "IsDelete = 0";

            if (textSearch != null && textSearch != "")
            {
                // Case 1
                if (ProvinceID != null && BizTypeID != null)
                {
                    #region SQL Where
                    SqlWhere = "IsDelete = 0";
                    SqlWhere += "AND CompProvinceID = " + ProvinceID;
                    SqlWhere += "AND BizTypeID = " + BizTypeID;
                    SqlWhere += "AND CompName Like N'%" + textSearch + "%'";
                    #endregion
                }
                // Case 2
                else if (ProvinceID != null)
                {
                    #region SQL Where
                    SqlWhere = "IsDelete = 0";
                    SqlWhere += "AND CompProvinceID = " + ProvinceID;
                    SqlWhere += "AND CompName Like N'%" + textSearch + "%'";
                    #endregion
                }
                else if (BizTypeID != null)
                {
                    #region SQL Where
                    SqlWhere = "IsDelete = 0";
                    SqlWhere += "AND BizTypeID = " + BizTypeID;
                    SqlWhere += "AND CompName Like N'%" + textSearch + "%'";
                    #endregion
                }
                else
                {
                    #region SQL Where
                    SqlWhere = "IsDelete = 0";
                    SqlWhere += "AND CompName Like N'%" + textSearch + "%'";
                    #endregion
                }
            }
            else
            {
                // Case 1
                if (ProvinceID != null && BizTypeID != null)
                {
                    #region SQL Where
                    SqlWhere = "IsDelete = 0";
                    SqlWhere += "AND CompProvinceID = " + ProvinceID;
                    SqlWhere += "AND BizTypeID = " + BizTypeID;
                    #endregion
                }
                // Case 2
                else if (ProvinceID != null)
                {
                    #region SQL Where
                    SqlWhere = "IsDelete = 0";
                    SqlWhere += "AND CompProvinceID = " + ProvinceID;
                    #endregion
                }
                else if (BizTypeID != null)
                {
                    #region SQL Where
                    SqlWhere = "IsDelete = 0";
                    SqlWhere += "AND BizTypeID = " + BizTypeID;
                    #endregion
                }
                else
                {
                    #region SQL Where
                    SqlWhere = "IsDelete = 0";
                    #endregion
                }
            }
            #endregion

            #region Check Gold Member
            if (CheckGold == 3)
            {
                var CompSearchAll = svCompany.SelectData<view_Company>("*", "CompLevel = 3 AND " + SqlWhere, "CompLevel DESC,CreatedDate DESC", PIndex, PSize);
                ViewBag.SearchList = CompSearchAll;
            }
            else
            {
                var CompSearchAll = svCompany.SelectData<view_Company>("*", SqlWhere, "CompLevel DESC,CreatedDate DESC", PIndex, PSize);
                ViewBag.SearchList = CompSearchAll;
            }
            #endregion

            ViewBag.PageIndex = PIndex;
            ViewBag.PageSize = PSize;
            ViewBag.TotalPage = svCompany.TotalPage;
            ViewBag.TotalRow = svCompany.TotalRow;
            ViewBag.Status = svProduct.CodeError;
            ViewBag.SearchType = "Company";
            ViewBag.TextSearch = textSearch;
            ViewBag.BizTypeID = BizTypeID;
            ViewBag.ProvinceID = ProvinceID;
            ViewBag.CategoryID = CategoryID;
            return View();
        }

        #endregion
        #region Search Quotation

        public ActionResult ListQuotation(
            string textSearch,
            int? CategoryID,
            int? Sort = 1,
            int PIndex = 1,
            int PSize = 20
        )
        {
            RememberURL();

            //if (!CheckIsLogin())
            //{
            //    return Redirect("SignIn");
            //} 
            GetStatusUser();
            GetCookie();
            SearchType();
            StatusMenu();

            var emMembers = new emMember();
            var b2bCompany = new b2bCompany();

            #region Get Cookie Check Sign In
            string MemberID = DataManager.ConvertToString(LogonMemberID);
            if (MemberID != "0")
            {
                emMembers = svMember.SelectData<emMember>("MemberID,UserName,FirstName,LastName,Email,Phone", "IsDelete = 0 AND MemberID =" + MemberID, null, 1, 1).First();
                b2bCompany = svCompany.SelectData<b2bCompany>("MemberID,CompID,CompName", "ISDelete = 0 AND MemberID =" + MemberID, null, 1, 1).First();
            }
            #endregion

            #region Search Type Quotation
            var SqlWhere = "IsDelete = 0 AND  RowFlag  = 1 AND IsOutBox = 0 AND QuotationFolderID = 1";
            if (LogonCompID > 0)
            {
                SqlWhere += " AND ToCompID =" + b2bCompany.CompID;
            }
            else
            {
                SqlWhere += " AND IsPublic = 1";
            }
            if (textSearch != null && textSearch != "")
            {
                SqlWhere += " AND ProductName Like N'%" + textSearch + "%'";
            }
            if (CategoryID != null)
            {
                SqlWhere += " AND CategoryID = " + CategoryID;
            }
            #endregion

            #region Query
            var QuoSearchAll = svQuotation.SelectData<View_QuotationList>("*", SqlWhere, "CreatedDate DESC", PIndex, PSize);
            ViewBag.SearchList = QuoSearchAll;
            #endregion

            ViewBag.PageIndex = PIndex;
            ViewBag.PageSize = PSize;
            ViewBag.TotalPage = svQuotation.TotalPage;
            ViewBag.TotalRow = svQuotation.TotalRow;
            ViewBag.Status = svProduct.CodeError;
            ViewBag.SearchType = "Request";
            return View();
        }

        #endregion

        // Function //

        #region function Check Admin

        public bool CheckAdmin(int MemberID,int MemberType)
        {
            bool IsAdmin = false;
            if (MemberType == 1)
            {
                IsAdmin = false;
            }
            else if (MemberType == 2)
            {
                IsAdmin = true;
            }                 
            
            return IsAdmin;
        }

        #endregion
        #region funtion Check Login

        public bool CheckLogin(int compid = 0)
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
        #region Check Cookie
        public Hashtable CheckCookie()
        {
            Hashtable htAuthentication = new Hashtable();
            HttpCookie ckAuthentication = System.Web.HttpContext.Current.Request.Cookies["B2BThai.com"];       
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
                htAuthentication.Add("CompPhone", System.Web.HttpContext.Current.Server.HtmlEncode(authenticationCkCollection["CompPhone"]));
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
        #region Get Data Cookie

        public void GetCookie()
        { 
            var dataCookie = CheckCookie();

            ViewBag.dataCookie = dataCookie;   
        }

        #endregion
        #region funtion RemoveCookie

        public bool RemoveCookie()
        {
            bool IsResult = false;
            try
            {
                HttpCookie ckAuthentication = System.Web.HttpContext.Current.Request.Cookies["B2BThai.com"];
                if (ckAuthentication != null)
                {
                    ckAuthentication.Expires = DateTime.Now.AddDays(-1);//คำสั่งลบ คุ๊กกี้ภายใน 1 วัน
                    this.ControllerContext.HttpContext.Response.Cookies.Add(ckAuthentication);//ใช้ไว้เผื่อเพิ่มตัวใหม่        
                }
                IsResult = true;
            }
            catch (Exception)
            {
                IsResult = false;
            }

            return IsResult;
        }

        #endregion
        #region validatePassword

        [HttpPost]
        public ActionResult validatePassword(string password)
        {
            EncryptManager encrypt = new EncryptManager();
            var member = new List<emMember>();
            if (Request.Cookies[res.Common.lblWebsite] != null)
            {               
                member = svMember.SelectData<emMember>("memberid,username,password", " MemberID = " + LogonMemberID);
            }
            //Hashtable data = new Hashtable();
            //var sqlSelect = "compid,memberid,username,email,compname";
            //var sqlWhere = " and RowflagWeb = 2 and IsDelete = 0 "; //and WebID =" + res.Config.WebID
           
            //if (!string.IsNullOrEmpty(password))
            //{

            var Password = encrypt.EncryptData(password);
            if (member.Count() > 0 && member!=null)
            {                                                            
                svMember.IsResult = member.First().Password == Password ? true : false;      
            }     
            return Json(new { IsResult = svMember.IsResult }, JsonRequestBehavior.AllowGet); 
        }

        #endregion
        #region Send Email ForgetPass

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
            //mailTo.Add("supawadee@prosoft.co.th");
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

        #endregion
        #region Count Message

        public void CountMessage()
        {
            MessageService svMessage = new MessageService();
            ViewBag.CountInbox = svMessage.SelectData<emMessage>("*", svMessage.CreateWhereAction(MessageStatus.UnRead, LogonCompID), null, 0, 0).Count();
            ViewBag.CountImportance = svMessage.SelectData<emMessage>("*", svMessage.CreateWhereAction(MessageStatus.Important, LogonCompID), null, 0, 0).Count();
            ViewBag.CountDraftbox = svMessage.SelectData<emMessage>("*", svMessage.CreateWhereAction(MessageStatus.Draft, LogonCompID), null, 0, 0).Count();
            ViewBag.CountSentbox = svMessage.SelectData<emMessage>("*", svMessage.CreateWhereAction(MessageStatus.Sentbox, LogonCompID), null, 0, 0).Count();
        }

        #endregion
        #region Status Nav_Header Menu

        public void StatusMenu()
        {
            MessageService svMessage = new MessageService();
            QuotationService svQuotation = new QuotationService();
            var CompID = LogonCompID;
            var CountStatusMsg = 0;
            var CountStatusReq = 0;

            if (CompID != 0)
            {
                CountStatusMsg = svMessage.CountData<emMessage>("*", "IsDelete = 0 AND IsRead = 0 AND MsgFolderID = 1 AND ToCompID = " + CompID);
                ViewBag.CountMsg = CountStatusMsg;
                CountStatusReq = svQuotation.CountData<b2bQuotation>("*", "IsDelete = 0 AND IsOutbox = 0 AND IsRead = 0 AND ToCompID = " + CompID);
                ViewBag.CountReq = CountStatusReq;
            }
        }

        #endregion
        #region Search Type

        public void SearchType() {
            var svCategory = new CategoryService();
            var ProvinceSearch = svAddress.GetProvinceAll().OrderBy(m => m.ProvinceName).ToList(); 
            var BizSearch = svBizType.GetBiztypeAll();

            var CateSearch = svCategory.GetCategoryByLevel(1);

            ViewBag.CateSearch = CateSearch;
            ViewBag.ProvinceSearch = ProvinceSearch;
            ViewBag.BizSearch = BizSearch;  
        }
 
        #endregion
        #region CountMessage
        public void CountMobileMessage()
        {
            MessageService svMessage = new MessageService();
            ViewBag.CountInbox = svMessage.CountData<emMessage>("*", svMessage.CreateWhereAction(MessageStatus.UnRead, LogonCompID));
        }
        #endregion
        #region Log On CompPhone
        public string LogonCompPhone
        {
            get
            {
                if (System.Web.HttpContext.Current.Request.Cookies.AllKeys.Contains(res.Common.lblWebsite))
                {
                    return DataManager.ConvertToString(Request.Cookies[res.Common.lblWebsite].Values["CompPhone"]);
                }
                else
                {
                    return String.Empty;
                }
            }
        }
        #endregion
    }
}