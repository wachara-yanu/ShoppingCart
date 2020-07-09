using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;
using Ouikum.Common;
using Prosoft.Service;
using Ouikum.Company;
using Ouikum.BizType;
using res = Prosoft.Resource.Web.Ouikum;
using Ouikum.Purchase;
using Ouikum.Product;
using Ouikum.Buylead;
using Ouikum.Web.Models;

namespace Ouikum.Web.Admin
{
    public class UserController : BaseController
    {
        //
        // GET: /Admin/Member/
        #region List
        #region Get: List Member
        [HttpGet]
        public ActionResult MemberList()
        {
            CompanyService svCompany = new CompanyService();
            CommonService svCommon = new CommonService();
            RememberURL();
            if (!CheckIsLogin())
            {
                return Redirect(res.Pageviews.PvMemberSignIn);
            }
            else
            {
                if (CheckIsAdmin(10) || CheckIsAdmin(15))
                {
                    GetStatusUser();
                    SetPager();
                    ViewBag.EnumCompLevels = svCommon.SelectEnum(CommonService.EnumType.CompLevel);
                    ViewBag.EnumUserStatus = svCommon.SelectEnum(CommonService.EnumType.UserStatus);
                    ViewBag.EnumSearchByMember = svCommon.SelectEnum(CommonService.EnumType.SearchByMember);
                    ViewBag.EnumSearchByMemberType = svCommon.SelectEnum(CommonService.EnumType.MemberType);

                    var Count_to_Exp = svCompany.SelectData<view_CompMember>("CompID,ExpireDate", "IsDelete = 0 AND MemIsDelete = 0 AND CompIsDelete = 0 AND WebID = 1 AND MemberType = 1 AND CompLevel = 3 AND ExpireDate Between '" + DateTime.Now.AddDays(1).ToShortDateString() + "' AND '" + DateTime.Now.AddDays(7).ToShortDateString() + "'");
                    var Count_Exp_today = svCompany.SelectData<view_CompMember>("CompID,ExpireDate", "IsDelete = 0 AND MemIsDelete = 0 AND CompIsDelete = 0 AND WebID = 1 AND MemberType = 1 AND CompLevel = 3 AND ExpireDate Between '" + DateTime.Now.ToShortDateString() + " 00:00:00' AND '" + DateTime.Now.ToShortDateString() + " 23:59:59'");
                    var Count_Exp = svCompany.SelectData<view_CompMember>("CompID,ExpireDate", "IsDelete = 0 AND MemIsDelete = 0 AND CompIsDelete = 0 AND WebID = 1 AND MemberType = 1 AND CompLevel = 3 AND ExpireDate < '" + DateTime.Now.Date.ToShortDateString() + "'");
                    if (Count_to_Exp != null)
                    {
                        ViewBag.Count_to_Exp = Count_to_Exp.Count();
                    }
                    else {
                        ViewBag.Count_to_Exp = 0;
                    }
                    if (Count_Exp_today != null)
                    {
                        ViewBag.Count_Exp_today = Count_Exp_today.Count();
                    }
                    else {
                        ViewBag.Count_Exp_today = 0;
                    }
                    if (Count_Exp != null)
                    {
                        ViewBag.Count_Exp = Count_Exp.Count();
                    }
                    else {
                        ViewBag.Count_Exp = 0;
                    }

                    ViewBag.Active = "Member";
                    return View();
                }
                else {
                    return Redirect(res.Pageviews.PvAccessDenied);
                }
            }
        }
        #endregion

        #region Post: List Member
        [HttpPost]
        public ActionResult MemberList(FormCollection form)
        {
            MemberService svMember = new MemberService();
            CommonService svCommon = new CommonService();
            CompanyService svCompany = new CompanyService();

            string SQLWhere = "IsDelete = 0 AND MemIsDelete = 0 AND CompIsDelete = 0 AND MemberType = 1";
            SelectList_PageSize();
            SetPager(form);
            if (DataManager.ConvertToInteger(form["PIndex"]) == 1)
            {
                ViewBag.PageIndex = DataManager.ConvertToInteger(form["PIndex"]);
            }
            if (!string.IsNullOrEmpty(form["SearchText"]))
            {
                SQLWhere += " AND ((UserName LIKE N'%" + form["SearchText"].Trim() + "%') OR (CompName LIKE N'%" + form["SearchText"].Trim() + "%') OR (FirstName LIKE N'%" + form["SearchText"].Trim() + "%') OR (CompCode LIKE N'%" + form["SearchText"].Trim() + "%'))";
            }
            if (!string.IsNullOrEmpty(form["ProductCount"]))
            {
                SQLWhere += " AND ProductCount >= 0";
            }
            if (!string.IsNullOrEmpty(form["CompLevel"]))
            {
                if (DataManager.ConvertToInteger(form["CompLevel"]) != 0)
                {
                    SQLWhere += " AND CompLevel = " + form["CompLevel"];
                }
            }
            if (!string.IsNullOrEmpty(form["Memberstatus"]))
            {
                if(DataManager.ConvertToInteger(form["Memberstatus"]) == 1)
                {
                    SQLWhere += " AND CompLevel = 3 AND ExpireDate Between '" + DateTime.Now.AddDays(1).ToShortDateString() + "' AND '" + DateTime.Now.AddDays(7).ToShortDateString() + "'";
                }
                else if (DataManager.ConvertToInteger(form["Memberstatus"]) == 2)
                {
                    SQLWhere += " AND CompLevel = 3 AND ExpireDate Between '" + DateTime.Now.ToShortDateString() + " 00:00:00' AND '" + DateTime.Now.ToShortDateString() + " 23:59:59'";
                }
                else if (DataManager.ConvertToInteger(form["Memberstatus"]) == 3)
                {
                    SQLWhere += " AND CompLevel = 3 AND ExpireDate < '" + DateTime.Now.ToShortDateString() + "'";
                }
            }
            if (!string.IsNullOrEmpty(form["CompRowFlag"]))
            {
                if (DataManager.ConvertToInteger(form["CompRowFlag"]) != 5)
                {
                    SQLWhere += " AND CompRowFlag = " + form["CompRowFlag"];
                }
            }
            if (!string.IsNullOrEmpty(form["SearchType"]))
            {
                if (form["SearchType"] != "All")
                {
                    SQLWhere += " AND " + form["SearchType"] + " LIKE N'%" + form["SearchText"].Trim() + "%'";
                }
            }
            if (!string.IsNullOrEmpty(form["Period"]))
            {
                SQLWhere += SQLWhereDateTimeFromPeriod(form["Period"], "RegisDate");
            }
            if (!string.IsNullOrEmpty(form["ServiceType"]))
            {
                if (form["ServiceType"] != "All")
                {
                    SQLWhere += " AND ServiceType = " + form["ServiceType"];
                }
            }
            if (LogonServiceType == 15)
                SQLWhere += " AND IsSME = 1";

            var CompMembers = svMember.SelectData<view_CompMember>("*", SQLWhere, "RegisDate DESC", (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
            ViewBag.TotalPage = svMember.TotalPage;
            ViewBag.TotalRow = svMember.TotalRow;
            ViewBag.EnumUserStatus = svCommon.SelectEnum(CommonService.EnumType.UserStatus);
            ViewBag.CompMembers = CompMembers;

            return PartialView("UC/MemberGrid");
        }
        #endregion

        #region Get: List Admin
        [HttpGet]
        public ActionResult AdminList()
        {
            RememberURL();
            CommonService svCommon = new CommonService();
            if (!CheckIsLogin())
            {
                return Redirect(res.Pageviews.PvMemberSignIn);
            }
            else
            {
                if (CheckIsAdmin())
                {
                    GetStatusUser();
                    SetPager();
                    ViewBag.EnumAdminType = svCommon.SelectEnum(CommonService.EnumType.AdminType);
                    ViewBag.EnumUserStatus = svCommon.SelectEnum(CommonService.EnumType.UserStatus);
                    ViewBag.EnumSearchByAdmin = svCommon.SelectEnum(CommonService.EnumType.SearchByAdmin);

                    ViewBag.Active = "Admin";
                    return View();
                }
                else
                {
                    return Redirect(res.Pageviews.PvAccessDenied);
                }
            }
        }
        #endregion

        #region Post: List Admin
        [HttpPost]
        public ActionResult AdminList(FormCollection form)
        {
            MemberService svMember = new MemberService();
            CompanyService svCompany = new CompanyService();
            string SQLWhere = "IsDelete = 0 AND WebID = 1 AND MemberType = 2";
            SelectList_PageSize();
            SetPager(form);
            if (DataManager.ConvertToInteger(form["PIndex"]) == 1)
            {
                ViewBag.PageIndex = DataManager.ConvertToInteger(form["PIndex"]);
            }
            if (!string.IsNullOrEmpty(form["SearchText"]))
            {
                SQLWhere += " AND ((UserName LIKE N'%" + form["SearchText"].Trim() + "%') OR (CompName LIKE N'%" + form["SearchText"].Trim() + "%') OR (FirstName LIKE N'%" + form["SearchText"].Trim() + "%') OR (CompCode LIKE N'%" + form["SearchText"].Trim() + "%'))";
            }
            if (!string.IsNullOrEmpty(form["ServiceType"]))
            {
                if (DataManager.ConvertToInteger(form["ServiceType"]) != 0)
                {
                    SQLWhere += " AND ServiceType = " + form["ServiceType"];
                }
            }
            if (!string.IsNullOrEmpty(form["CompRowFlag"]))
            {
                if (DataManager.ConvertToInteger(form["CompRowFlag"]) != 5)
                {
                    SQLWhere += " AND CompRowFlag = " + form["CompRowFlag"];
                }
            }
            if (!string.IsNullOrEmpty(form["SearchType"]))
            {
                if (form["SearchType"] != "All")
                {
                    SQLWhere += " AND " + form["SearchType"] + " LIKE N'%" + form["SearchText"].Trim() + "%'";
                }
            }
            if (!string.IsNullOrEmpty(form["Period"]))
            {
                SQLWhere += SQLWhereDateTimeFromPeriod(form["Period"], "RegisDate");
            }
            var CompMembers = svMember.SelectData<view_CompMember>("*", SQLWhere, "RegisDate DESC", (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
            ViewBag.TotalPage = svMember.TotalPage;
            ViewBag.TotalRow = svMember.TotalRow;
            UserStatus();
            ViewBag.CompMembers = CompMembers;

            return PartialView("UC/AdminGrid");
        }
        #endregion

        #endregion

        #region New
        #region NewMember
        public ActionResult NewMember()
        {
            RememberURL();
            if (!CheckIsLogin())
            {
                return Redirect(res.Pageviews.PvMemberSignIn);
            }
            else
            {
                if (CheckIsAdmin(10))
                {
                    GetStatusUser();
                    ViewBag.Active = "Member";
                    CommonService svCommon = new CommonService();
                    AddressService svAddress = new AddressService();
                    BizTypeService svBizType = new BizTypeService();

                    var Provinces = svAddress.SelectData<emProvince>("*", "IsDelete = 0", "RegionID ASC");
                    var Biztypes = svBizType.SelectData<b2bBusinessType>("BizTypeID,BizTypeName,BizTypeCode", "IsDelete = 0", "BizTypeName ASC");
                    ViewBag.EnumMemberType = svCommon.SelectEnum(CommonService.EnumType.MemberType);
                    ViewBag.EnumCompLevels = svCommon.SelectEnum(CommonService.EnumType.CompLevel);
                    ViewBag.Provinces = Provinces;
                    ViewBag.Biztypes = Biztypes;
                    return View();
                }
                else
                {
                    return Redirect(res.Pageviews.PvAccessDenied);
                }
            }
        }
        [HttpPost]
        public ActionResult NewMember(Register model)
        {
            MemberService svMember = new MemberService();
            CompanyService svCompany = new CompanyService();
            svMember.UserRegister(model);
            svCompany.InsertCompany(model);
            if (svCompany.IsResult && svMember.IsResult)
            {
                return Redirect("~/Admin/User/MemberList");
            }
            else
            {
                return Redirect("~/Admin/User/MemberList");
            }
        }
        #endregion

        #region NewAdmin
        public ActionResult NewAdmin()
        {
            RememberURL();
            if (!CheckIsLogin())
            {
                return Redirect(res.Pageviews.PvMemberSignIn);
            }
            else
            {
                if (CheckIsAdmin(9))
                {
                    GetStatusUser();
                    ViewBag.Active = "Admin";
                    CommonService svCommon = new CommonService();
                    AddressService svAddress = new AddressService();
                    BizTypeService svBizType = new BizTypeService();

                    var Provinces = svAddress.SelectData<emProvince>("ProvinceID,ProvinceName", "IsDelete = 0", "RegionID ASC");
                    var Biztypes = svBizType.SelectData<b2bBusinessType>("BizTypeID,BizTypeName,BizTypeCode", "IsDelete = 0", "BizTypeName ASC");
                    ViewBag.EnumAdminType = svCommon.SelectEnum(CommonService.EnumType.AdminType);
                    ViewBag.Provinces = Provinces;
                    ViewBag.Biztypes = Biztypes;
                    return View();
                }
                else
                {
                    return Redirect(res.Pageviews.PvAccessDenied);
                }
            }
        }
        [HttpPost]
        public ActionResult NewAdmin(Register model)
        {
            MemberService svMember = new MemberService();
            CompanyService svCompany = new CompanyService();
            svMember.UserRegister(model);
            svCompany.InsertCompany(model);
            if (svCompany.IsResult && svMember.IsResult)
            {
                return Redirect("~/Admin/User/AdminList");
            }
            else
            {
                return Redirect("~/Admin/User/AdminList");
            }
        }
        #endregion
        #endregion

        #region Edit/Detail
        #region EditMember
        public ActionResult EditMember(int MemberWebID = 0)
        {
            RememberURL();
            if (!CheckIsLogin())
            {
                return Redirect(res.Pageviews.PvMemberSignIn);
            }
            else
            {
                if (CheckIsAdmin(10))
                {
                    if (MemberWebID > 0)
                    {
                        GetStatusUser();
                        ViewBag.Active = "Member";
                        MemberService svMember = new MemberService();
                        AddressService svAddress = new AddressService();
                        BizTypeService svBizType = new BizTypeService();
                        CompanyService svCompany = new CompanyService();
                        CommonService svCommon = new CommonService();
                        
                        var Member = svMember.SelectData<view_emCompanyMember>("*", "IsDelete = 0 AND MemberWebID = " + MemberWebID, null, 0, 0, false).First();
                        if (Member.MemberType == 1)
                        {
                            var Company = svCompany.SelectData<b2bCompany>("*", "IsDelete = 0 AND MemberID = " + Member.MemberID).First();
                            var Provinces = svAddress.GetProvinceAll();
                            var Districts = svAddress.ListDistrictByProvinceID((int)Member.ProvinceID);
                              
                            var Biztypes = svBizType.GetBiztypeAll();

                            ViewBag.EnumUserStatus = svCommon.SelectEnum(CommonService.EnumType.UserStatus);
                            ViewBag.EnumMemberType = svCommon.SelectEnum(CommonService.EnumType.MemberType);
                            ViewBag.EnumCompLevels = svCommon.SelectEnum(CommonService.EnumType.CompLevel);
                            ViewBag.CompMember = Member;
                            ViewBag.Company = Company;
                            ViewBag.Provinces = Provinces;
                            ViewBag.Districts = Districts;
                            ViewBag.Biztypes = Biztypes;
                            return View();
                        }
                        else
                        {
                            return Redirect(res.Pageviews.PvAdminMember);
                        }
                    }
                    else
                    {
                        return Redirect(res.Pageviews.PvAdminMember);
                    }
                }
                else
                {
                    return Redirect(res.Pageviews.PvAccessDenied);
                }
            }
        }
        [HttpPost]
        public ActionResult EditMember(FormCollection form)
        {
            //var BuyleadExpire = $('#dp3').val().substring(3, 6) + $('#dp3').val().substring(0, 3) + $('#dp3').val().substring(6, 10);
            var date = form["date"]; 
            DateTime CreatedDate = DataManager.ToDateTimeSQL(date);

            var b2bCompanies = new b2bCompany();
            MemberService svMember = new MemberService();
            CompanyService svCompany = new CompanyService();
            int CompanyLevel;
            int CompRowflag;
            var Member = svMember.SelectData<emMember>("*", "IsDelete = 0 AND MemberID = " + form["MemberID"], null, 0, 0, false).First();
            var Company = svCompany.SelectData<b2bCompany>("*", "IsDelete = 0 AND MemberID = " + form["MemberID"], null, 0, 0, false).First();
            var emCompany = svMember.SelectData<emCompany>("*", "IsDelete = 0 AND MemberID = " + form["MemberID"], null, 0, 0, false).First();
            var emMemberWeb = svMember.SelectData<emMemberWeb>("*", "IsDelete = 0 AND MemberID = " + form["MemberID"], null, 0, 0, false).First();
            var MemberOld = Member.RegisDate;
            #region set ค่า emMember
            Member.Email = form["Email"];
            Member.FirstName = form["FirstName"];
            Member.LastName = form["LastName"];
            Member.DisplayName = form["DisplayName"];
            Member.AddrLine1 = form["AddrLine1"];
            Member.ProvinceID = DataManager.ConvertToInteger(form["ProvinceID"]);
            Member.DistrictID = DataManager.ConvertToInteger(form["DistrictID"]);
            Member.PostalCode = form["PostalCode"];
            Member.Phone = form["Phone"];
            Member.Mobile = form["Mobile"];
            Member.Fax = form["Fax"];
            Member.CreatedDate = CreatedDate;
            Member.RegisDate = CreatedDate;
            Member.ModifiedDate = DateTime.Now;
            #endregion 

            #region set ค่า emMemberWeb
            emMemberWeb.CreatedDate = CreatedDate;
            emMemberWeb.ModifiedDate = DateTime.Now;
            #endregion

            #region set ค่า emCompany
            emCompany.BizTypeID = DataManager.ConvertToInteger(form["BizTypeID"]);
            emCompany.DisplayName = form["DisplayName"];
            emCompany.CompName = form["CompName"];
            emCompany.RowFlag = DataManager.ConvertToShort(form["CompRowFlag"]);
            emCompany.CreatedDate = CreatedDate;
            emCompany.ModifiedDate = DateTime.Now;
            if (Company.BizTypeID == 13)
            {
                emCompany.BizTypeOther = form["BizTypeOther"];
            }
            #endregion

            #region set ค่า b2bCompany
            CompanyLevel = Convert.ToInt32(Company.CompLevel);
            CompRowflag = DataManager.ConvertToShort(Company.RowFlag);
            Company.CompLevel = DataManager.ConvertToByte(form["CompLevel"]);
            Company.ServiceType = DataManager.ConvertToByte(form["ServiceType"]);
            Company.BizTypeID = DataManager.ConvertToInteger(form["BizTypeID"]);
            Company.CompName = form["CompName"];
            Company.DisplayName = form["DisplayName"];
            Company.RowFlag = DataManager.ConvertToShort(form["CompRowFlag"]);
            Company.AdminNote = form["AdminNote"];
            Company.IsTrust = DataManager.ConvertToBool(DataManager.ConvertToInteger(form["IsTrust"]));
            Company.IsSME = DataManager.ConvertToBool(DataManager.ConvertToInteger(form["IsSME"]));
            Company.CreatedDate = CreatedDate;
            Company.ModifiedDate = DateTime.Now;
            if (Convert.ToInt32(form["IsCompRowflagBox"]) == 1)
            {
                Company.Remark = form["Remark"];
            }
            else
            {
                Company.Remark = null;
            }
            if (Company.CompLevel == 3 && CompanyLevel == 1)
            {
                if (Convert.ToInt32(form["ExpireDate"]) != 0)
                {
                    Company.ExpireDate = CreatedDate.AddMonths(Convert.ToInt32(form["ExpireDate"]));
                }
                else
                {
                    Company.ExpireDate = CreatedDate.AddYears(1);
                }
            }
            else if ((Company.CompLevel == 3 && CompanyLevel == 3) || (Company.CompLevel == 2 && CompanyLevel == 2))
            {
                if (Convert.ToInt32(form["ExpireDate"]) != 0)
                {
                    var MemRegisDate = (DateTime.Parse(MemberOld.ToString()).ToString("MM/dd/yyyy")).ToString();
                    if (date.ToString() != MemRegisDate)
                    {
                        Company.ExpireDate = Convert.ToDateTime(form["date"]).AddMonths(Convert.ToInt32(form["ExpireDate"]));
                    }
                    else
                    {
                        var ExpireDateOld = form["ExpireDateOld"];
                        DateTime ExpireDateOldCon = DataManager.ToDateTimeSQL(ExpireDateOld);

                        Company.ExpireDate = ExpireDateOldCon.AddMonths(Convert.ToInt32(form["ExpireDate"]));
                    }
                }
            }
            if (Company.CompLevel == 1)
            {
                Company.ExpireDate = null;
            }
            if (Company.BizTypeID == 13)
            {
                Company.BizTypeOther = form["BizTypeOther"];
            }
            if (!string.IsNullOrEmpty(form["Reasons"]))
            {
                Company.CompAddrLine2 = form["Reasons"];
            }

            #endregion

            #region UpdateData
            svMember.SaveData<emMember>(Member, "MemberID");
            if (svMember.IsResult)
            {
                svCompany.SaveData<b2bCompany>(Company, "CompID");
                //var mg = new KeywordMongo();
                //mg.UpdateCompNameByCompID(Company.CompID, Company.CompName, Company.IsSME, Company.CompLevel);
                if (svCompany.IsResult)
                {
                    svMember.SaveData<emCompany>(emCompany, "CompID");
                    if (svMember.IsResult)
                    {
                        svMember.SaveData<emMemberWeb>(emMemberWeb, "MemberID");

                        #region sendEmail

                        #region variable
                        bool IsSend = true;
                        var Detail = "";
                        var mailTo = new List<string>();
                        var mailCC = new List<string>();
                        Hashtable EmailDetail = new Hashtable();
                        var IsCompRowflagBox = Convert.ToInt32(form["IsCompRowflagBox"]);
                        #endregion

                        #region Set Content & Value For Send Email

                        string Subject;
                        string b2bthai_url = res.Pageviews.UrlWeb;
                        string pathlogo = b2bthai_url + "/Content/Default/logo/Ouikum/img_Logo120x74.png";
                        string pathGold = "https://ouikumstorage.blob.core.windows.net/upload/Content/Email/images/icon_Gold130.png";
                        string pathF = b2bthai_url + "/Content/Default/images/icon_freesmall.png";
                        string pathG = b2bthai_url + "/Content/Default/images/icon_goldsmall.png";
                        string pathGE = b2bthai_url + "/Content/Default/images/icon_goldExpiresmall.png";

                        EmailDetail["b2bthaiUrl"] = b2bthai_url;
                        EmailDetail["CompName"] = Company.CompName;
                        EmailDetail["FirstName"] = Member.FirstName;
                        EmailDetail["pathLogo"] = pathlogo;
                        EmailDetail["pathGold"] = pathGold;
                        EmailDetail["pathF"] = pathF;
                        EmailDetail["pathG"] = pathG;
                        EmailDetail["pathGE"] = pathGE;
                        if (Company.ExpireDate != null)
                        {
                            EmailDetail["ExpireDate"] = Company.ExpireDate.Value.ToString("dd/MM/yyyy");
                        }
                        EmailDetail["month"] = form["ExpireDate"];
                        var mailFrom = res.Config.EmailNoReply;
                        #endregion

                        #region userstatus
                        if (Company.RowFlag == 0 && IsCompRowflagBox == 1)//BlackList
                        {
                            EncryptManager encrypt = new EncryptManager();
                            Subject = res.Email.SubjectBlockInfo;
                            EmailDetail["UserName"] = Member.UserName;
                            EmailDetail["Pass"] = encrypt.DecryptData(Member.Password);
                            if (!string.IsNullOrEmpty(Company.Remark))
                            {
                                EmailDetail["Remark"] = Company.Remark;
                            }
                            else
                            {
                                EmailDetail["Remark"] = "-";
                            }
                            // data for set msg detail
                            ViewBag.Data = EmailDetail;
                            Detail = PartialViewToString("UC/Email/SendBlockList");
                            if (!string.IsNullOrEmpty(Member.Email))
                            {
                                mailTo.Add(Member.Email);
                            }
                            else
                            {
                                mailTo.Add(Company.ContactEmail);
                            }

                            IsSend = OnSendByAlertEmail(Subject, mailFrom, mailTo, mailCC, Detail);
                        }
                        else if (Company.RowFlag == 3 && IsCompRowflagBox == 1)//BlockInfo
                        {
                            EncryptManager encrypt = new EncryptManager();
                            Subject = res.Email.SubjectBlockInfo;
                            EmailDetail["UserName"] = Member.UserName;
                            EmailDetail["Pass"] = encrypt.DecryptData(Member.Password);
                            if (!string.IsNullOrEmpty(Company.Remark))
                            {
                                EmailDetail["Remark"] = Company.Remark;
                            }
                            else
                            {
                                EmailDetail["Remark"] = "-";
                            }
                            // data for set msg detail
                            ViewBag.Data = EmailDetail;
                            Detail = PartialViewToString("UC/Email/SendBlockInfo");
                            if (!string.IsNullOrEmpty(Member.Email))
                            {
                                mailTo.Add(Member.Email);
                            }
                            else
                            {
                                mailTo.Add(Company.ContactEmail);
                            }

                            IsSend = OnSendByAlertEmail(Subject, mailFrom, mailTo, mailCC, Detail);
                        }
                        #endregion

                        #region upgradeGoldMember
                        if (Company.CompLevel == 3)// && CompanyLevel != Company.CompLevel
                        {
                            if (CompanyLevel != Company.CompLevel)// อัพเกรดจาก free เป็น Gold
                             {
                                 Subject = res.Email.SubjectUpgradeGold + " – " + res.Common.lblDomainShortName;
                                 // data for set msg detail
                                 ViewBag.Data = EmailDetail;
                                 Detail = PartialViewToString("UC/Email/SendGoldMember");
                                 if (!string.IsNullOrEmpty(Member.Email))
                                 {
                                     mailTo.Add(Member.Email);
                                 }
                                 else
                                 {
                                     mailTo.Add(Company.ContactEmail);
                                 }
                                 IsSend = OnSendByAlertEmail(Subject, mailFrom, mailTo, mailCC, Detail);
                             }
                            if (Convert.ToInt32(form["ExpireDate"]) != 0 && CompanyLevel == Company.CompLevel) // ต่ออายุ Gold
                            {
                                 Subject = res.Email.SubjectUpgradePackage;
                                 // data for set msg detail
                                 ViewBag.Data = EmailDetail;
                                 Detail = PartialViewToString("UC/Email/SendUpgradGold");
                                 if (!string.IsNullOrEmpty(Member.Email))
                                 {
                                     mailTo.Add(Member.Email);
                                 }
                                 else
                                 {
                                     mailTo.Add(Company.ContactEmail);
                                 }
                                 IsSend = OnSendByAlertEmail(Subject, mailFrom, mailTo, mailCC, Detail);
                             }

                        }
                        else if (Company.CompLevel == 2)
                        {
                            if (CompanyLevel != Company.CompLevel)// อัพเกรดจาก Gold เป็น Gold(Expire)
                            {
                                Subject = res.Email.SubjectUpgradeGoldExpire;
                                // data for set msg detail
                                ViewBag.Data = EmailDetail;
                                Detail = PartialViewToString("UC/Email/SendGoldMemberExpire");
                                if (!string.IsNullOrEmpty(Member.Email))
                                {
                                    mailTo.Add(Member.Email);
                                }
                                else
                                {
                                    mailTo.Add(Company.ContactEmail);
                                }
                                IsSend = OnSendByAlertEmail(Subject, mailFrom, mailTo, mailCC, Detail);
                            }
                        }
                        #endregion

                        #region upgradeFreeMember
                        else if (Company.CompLevel == 1 && CompanyLevel != Company.CompLevel)// && CompanyLevel != Company.CompLevel
                        {
                            Subject = res.Email.SubjectUpgradeFree + " – " + res.Common.lblWebsite;
                            // data for set msg detail
                            ViewBag.Data = EmailDetail;
                            Detail = PartialViewToString("UC/Email/SendFreeMember");
                            if (!string.IsNullOrEmpty(Member.Email))
                            {
                                mailTo.Add(Member.Email);
                            }
                            else
                            {
                                mailTo.Add(Company.ContactEmail);
                            }

                            IsSend = OnSendByAlertEmail(Subject, mailFrom, mailTo, mailCC, Detail);
                        }
                        #endregion

                        #endregion
                    }
                    else
                    {
                        var MemberWeb3 = svMember.SelectData<view_emCompanyMember>("MemberWebID", "IsDelete = 0 AND MemberID = " + Member.MemberID, null, 0, 0, false).First();
                        return Redirect(res.Pageviews.PvEditmember + "?MemberWebID=" + MemberWeb3.MemberWebID);
                    }
                }
                else
                {
                    var MemberWeb3 = svMember.SelectData<view_emCompanyMember>("MemberWebID", "IsDelete = 0 AND MemberID = " + Member.MemberID, null, 0, 0, false).First();
                    return Redirect(res.Pageviews.PvEditmember + "?MemberWebID=" + MemberWeb3.MemberWebID);
                }
            }
            else
            {
                var MemberWeb3 = svMember.SelectData<view_emCompanyMember>("MemberWebID", "IsDelete = 0 AND MemberID = " + Member.MemberID, null, 0, 0, false).First();
                return Redirect(res.Pageviews.PvEditmember + "?MemberWebID=" + MemberWeb3.MemberWebID);
                //return Json(new { IsResult = svMember.IsResult, MsgError = "เกิดข้อผิดพลาดในการบันมึกข้อมูล" });
            }
            #endregion

            var MemberWeb = svMember.SelectData<view_emCompanyMember>("MemberWebID", "IsDelete = 0 AND MemberID = " + Member.MemberID, null, 0, 0, false).First();
            return Redirect(res.Pageviews.PvEditmember+"?MemberWebID=" + MemberWeb.MemberWebID);
        }
        #endregion

        #region EditAdmin
        public ActionResult EditAdmin(int MemberWebID = 0)
        {
            RememberURL();
            if (!CheckIsLogin() && CheckIsAdmin())
            {
                return Redirect(res.Pageviews.PvLoginPage);
            }
            else
            {
                if (CheckIsAdmin(10))
                {
                    if (MemberWebID > 0)
                    {
                        GetStatusUser();
                        ViewBag.Active = "Admin";
                        MemberService svMember = new MemberService();
                        AddressService svAddress = new AddressService();
                        BizTypeService svBizType = new BizTypeService();
                        CompanyService svCompany = new CompanyService();
                        CommonService svCommon = new CommonService();

                        var Member = svMember.SelectData<view_emCompanyMember>("*", "IsDelete = 0 AND MemberWebID = " + MemberWebID, null, 0, 0, false).First();
                        var Company = svCompany.SelectData<b2bCompany>("*", "IsDelete = 0 AND MemberID = " + Member.MemberID).First();
                        var Provinces = svAddress.GetProvince().ToList();
                        var Districts = svAddress.GetDistrict().Where(m => m.ProvinceID == Member.ProvinceID).ToList();
                        var Biztypes = svBizType.SelectData<b2bBusinessType>("BizTypeID,BizTypeName,BizTypeCode", "IsDelete = 0", "BizTypeName ASC");

                        ViewBag.EnumUserStatus = svCommon.SelectEnum(CommonService.EnumType.UserStatus);
                        ViewBag.EnumAdminType = svCommon.SelectEnum(CommonService.EnumType.AdminType);
                        ViewBag.EnumCompLevels = svCommon.SelectEnum(CommonService.EnumType.CompLevel);
                        ViewBag.CompMember = Member;
                        ViewBag.Company = Company;
                        ViewBag.Provinces = Provinces;
                        ViewBag.Districts = Districts;
                        ViewBag.Biztypes = Biztypes;

                        return View();
                    }
                    else
                    {
                        return Redirect(res.Pageviews.PvAdminList);
                    }
                }
                else
                {
                    return Redirect(res.Pageviews.PvAccessDenied);
                }
            }
        }
        [HttpPost]
        public ActionResult EditAdmin(FormCollection form)
        {
            MemberService svMember = new MemberService();
            CompanyService svCompany = new CompanyService();
            int CompanyLevel;
            int CompRowflag;
            var Member = svMember.SelectData<emMember>("*", "IsDelete = 0 AND MemberID = " + form["MemberID"], null, 0, 0, false).First();
            var Company = svCompany.SelectData<b2bCompany>("*", "IsDelete = 0 AND MemberID = " + form["MemberID"], null, 0, 0, false).First();
            var emCompany = svMember.SelectData<emCompany>("*", "IsDelete = 0 AND MemberID = " + form["MemberID"], null, 0, 0, false).First();

            #region set ค่า emMember
            Member.Email = form["Email"];
            Member.FirstName = form["FirstName"];
            Member.LastName = form["LastName"];
            Member.DisplayName = form["DisplayName"];
            Member.AddrLine1 = form["AddrLine1"];
            Member.ProvinceID = DataManager.ConvertToInteger(form["ProvinceID"]);
            Member.DistrictID = DataManager.ConvertToInteger(form["DistrictID"]);
            Member.PostalCode = form["PostalCode"];
            Member.Phone = form["Phone"];
            Member.Mobile = form["Mobile"];
            Member.Fax = form["Fax"];
            #endregion

            #region set ค่า emCompany
            emCompany.BizTypeID = DataManager.ConvertToInteger(form["BizTypeID"]);
            emCompany.DisplayName = form["DisplayName"];
            emCompany.CompName = form["CompName"];
            emCompany.RowFlag = DataManager.ConvertToShort(form["CompRowFlag"]);
            if (Company.BizTypeID == 13)
            {
                emCompany.BizTypeOther = form["BizTypeOther"];
            }
            #endregion

            #region set ค่า b2bCompany
            CompanyLevel = Convert.ToInt32(Company.CompLevel);
            CompRowflag = DataManager.ConvertToShort(Company.RowFlag);
            Company.CompLevel = DataManager.ConvertToByte(form["CompLevel"]);
            Company.ServiceType = DataManager.ConvertToByte(form["ServiceType"]);
            Company.BizTypeID = DataManager.ConvertToInteger(form["BizTypeID"]);
            Company.CompName = form["CompName"];
            Company.DisplayName = form["DisplayName"];
            Company.RowFlag = DataManager.ConvertToShort(form["CompRowFlag"]);
            Company.AdminNote = form["AdminNote"];
            if (Convert.ToInt32(form["IsCompRowflagBox"]) == 1)
            {
                Company.Remark = form["Remark"];
            }
            else
            {
                Company.Remark = null;
            }
            if (Company.CompLevel == 3)
            {
                Company.ExpireDate = DateTime.Now.AddYears(1);
                //Company.ExpireDate = DateTime.Now.AddMonths(3);
                //Company.ExpireDate = null;
            }
            if (Company.CompLevel == 1)
            {
                Company.ExpireDate = null;
            }
            if (Company.BizTypeID == 13)
            {
                Company.BizTypeOther = form["BizTypeOther"];
            }
            if (!string.IsNullOrEmpty(form["Reasons"]))
            {
                Company.CompAddrLine2 = form["Reasons"];
            }

            #endregion

            #region UpdateData
            svMember.SaveData<emMember>(Member, "MemberID");
            if (svMember.IsResult)
            {
                svCompany.SaveData<b2bCompany>(Company, "CompID");
                if (svCompany.IsResult)
                {
                    svMember.SaveData<emCompany>(emCompany, "CompID");

                    #region sendEmail

                    #region variable
                    bool IsSend = true;
                    var Detail = "";
                    var mailTo = new List<string>();
                    var mailCC = new List<string>();
                    Hashtable EmailDetail = new Hashtable();
                    #endregion

                    #region Set Content & Value For Send Email

                    string Subject;
                    string b2bthai_url = res.Pageviews.UrlWeb;
                    string pathlogo = b2bthai_url + "/Content/Default/logo/Ouikum/img_Logo120x74.png";

                    EmailDetail["b2bthaiUrl"] = b2bthai_url;
                    EmailDetail["CompName"] = Company.CompName;
                    EmailDetail["FirstName"] = Member.FirstName;
                    EmailDetail["pathLogo"] = pathlogo;
                    var mailFrom = res.Config.EmailNoReply;
                    #endregion

                    #region userstatus
                    if (Company.RowFlag == 0 && CompRowflag != Company.RowFlag && Convert.ToInt32(form["IsCompRowflagBox"]) == 1)//BlackList
                    {
                        Subject = res.Admin.lblDisables_acc+res.Common.lblDomainShortName;
                        EmailDetail["Main"] = "<a href='" + b2bthai_url + "'>" + res.Common.lblDomainShortName + "</a>" + res.Admin.lblRight_to_suspended;
                        if (!string.IsNullOrEmpty(Company.Remark))
                        {
                            EmailDetail["Detail"] = Company.Remark;
                        }
                        // data for set msg detail
                        ViewBag.Data = EmailDetail;
                        Detail = PartialViewToString("UC/Email/SendStatus");
                        if (!string.IsNullOrEmpty(Member.Email))
                        {
                            mailTo.Add(Member.Email);
                        }
                        else
                        {
                            mailTo.Add(Company.ContactEmail);
                        }

                        IsSend = OnSendByAlertEmail(Subject, mailFrom, mailTo, mailCC, Detail);
                    }
                    else if (Company.RowFlag == 3 && CompRowflag != Company.RowFlag && Convert.ToInt32(form["IsCompRowflagBox"]) == 1)//BlockInfo
                    {
                        Subject = res.Admin.lblDisables_acc + res.Common.lblDomainShortName;
                        EmailDetail["Main"] = "<a href='" + b2bthai_url + "'>" + res.Common.lblDomainShortName + "</a>" + res.Admin.lblRight_to_suspended;
                        if (!string.IsNullOrEmpty(Company.Remark))
                        {
                            EmailDetail["Detail"] = Company.Remark;
                        }
                        // data for set msg detail
                        ViewBag.Data = EmailDetail;
                        Detail = PartialViewToString("UC/Email/SendStatus");
                        if (!string.IsNullOrEmpty(Member.Email))
                        {
                            mailTo.Add(Member.Email);
                        }
                        else
                        {
                            mailTo.Add(Company.ContactEmail);
                        }

                        IsSend = OnSendByAlertEmail(Subject, mailFrom, mailTo, mailCC, Detail);
                    }
                    #endregion

                    #region upgradeGoldMember
                    if (Company.CompLevel == 3 && CompanyLevel != Company.CompLevel)
                    {
                        Subject = res.Admin.lblUpgradeGoldMember + " – " + res.Common.lblDomainShortName;
                        EmailDetail["Main"] = res.Common.lblDomainShortName + " " + res.Admin.lblUpgradeGoldMember;
                        EmailDetail["Detail"] = res.Admin.lblCheck_rights+" "+res.Common.lblGoldMember+" <a href='"+res.Pageviews.UrlAbout_ArticleID93+"'> "+res.Pageviews.UrlAbout_ArticleID93+" </a>";
                        // data for set msg detail
                        ViewBag.Data = EmailDetail;
                        Detail = PartialViewToString("UC/Email/SendGoldMember");
                        if (!string.IsNullOrEmpty(Member.Email))
                        {
                            mailTo.Add(Member.Email);
                        }
                        else
                        {
                            mailTo.Add(Company.ContactEmail);
                        }

                        IsSend = OnSendByAlertEmail(Subject, mailFrom, mailTo, mailCC, Detail);
                    }
                    else if (Company.CompLevel == 1 && CompanyLevel != Company.CompLevel)
                    {
                        Subject = res.Admin.lblUpgradeFreeMember + " – " + res.Common.lblDomainShortName;
                        EmailDetail["Main"] = res.Common.lblDomainShortName + " " + res.Admin.lblUpgradeFreeMember;
                        EmailDetail["Detail"] = res.Admin.lblCheck_rights + " " + res.Common.lblGoldMember + " <a href='"+res.Pageviews.UrlAbout_ArticleID93+"'> "+res.Pageviews.UrlAbout_ArticleID93+" </a>";
                        EmailDetail["Detail"] += res.Admin.lblExtendlifetime + " <a href='" + b2bthai_url + "/Benefit/Adsrate'>" + b2bthai_url + "/Benefit/Adsrate</a>";
                        // data for set msg detail
                        ViewBag.Data = EmailDetail;
                        Detail = PartialViewToString("UC/Email/SendGoldMember");
                        if (!string.IsNullOrEmpty(Member.Email))
                        {
                            mailTo.Add(Member.Email);
                        }
                        else
                        {
                            mailTo.Add(Company.ContactEmail);
                        }

                        IsSend = OnSendByAlertEmail(Subject, mailFrom, mailTo, mailCC, Detail);
                    }
                    #endregion

                    #endregion

                }
            }
            #endregion

            var MemberWeb = svMember.SelectData<view_emCompanyMember>("*", "IsDelete = 0 AND MemberID = " + Member.MemberID, null, 0, 0, false).First();
            return Redirect(res.Pageviews.PvEditmember+"?MemberWebID=" + MemberWeb.MemberWebID);
        }
        #endregion
        #endregion

        #region SendUser
        [HttpPost]
        public JsonResult SendUser(int MemberWebID ,string Email)
        {
            MemberService svMember = new MemberService();
            EncryptManager encrypt = new EncryptManager();
            var UserData = svMember.SelectData<view_emCompanyMember>("MemberWebID,UserName,Password,FirstName,Email,CompName","Isdelete = 0 AND WebID = 1 AND MemberWebID = " + MemberWebID).First();

            #region SendEmail

            #region variable
            bool IsSend = true;
            var Detail = "";
            var mailTo = new List<string>();
            var mailCC = new List<string>();
            Hashtable EmailDetail = new Hashtable();
            #endregion

            #region Set Content & Value For Send Email

            string Subject = res.Admin.lblCustomer_website + res.Common.lblDomainShortName + " (" + UserData.CompName + ")";
            string b2bthai_url = res.Pageviews.UrlWeb;
            string pathlogo = b2bthai_url + "/Content/Default/logo/Ouikum/img_Logo120x74.png";


            EmailDetail["b2bthaiUrl"] = b2bthai_url;
            EmailDetail["CompName"] = UserData.CompName;
            EmailDetail["FirstName"] = UserData.FirstName;
            EmailDetail["pathLogo"] = pathlogo;
            EmailDetail["Username"] = UserData.UserName;
            EmailDetail["Password"] = encrypt.DecryptData(UserData.Password);
            // data for set msg detail
            ViewBag.Data = EmailDetail;
            Detail = PartialViewToString("UC/Email/SendUser");

            var mailFrom = res.Config.EmailNoReply;
            mailTo.Add(Email);
            #endregion

            IsSend = OnSendByAlertEmail(Subject, mailFrom, mailTo, mailCC, Detail);

            #endregion

            return Json(new { IsSend = IsSend});
        }
        #endregion

        #region SendMailAlerts
        [HttpPost]
        public JsonResult SendMailAlerts(int MemberID, string Email, int status, int LifeTime = 0)
        {
            MemberService svMember = new MemberService();
            EncryptManager encrypt = new EncryptManager();
            var UserData = svMember.SelectData<view_CompMember>("MemberID,UserName,Password,FirstName,Email,CompName,ExpireDate", "Isdelete = 0 AND MemberID = " + MemberID).First();

            #region variable
            bool IsSend = true;
            var Detail = "";
            string Subject = "";
            var mailTo = new List<string>();
            var mailCC = new List<string>();
            var mailFrom = res.Config.EmailNoReply;
            Hashtable EmailDetail = new Hashtable();

            #region Set Content & Value For Send Email
            string b2bthai_url = res.Pageviews.UrlWeb;
            string pathlogo = b2bthai_url + "/Content/Default/logo/Ouikum/img_Logo120x74.png";
            string pathGold = "https://ouikumstorage.blob.core.windows.net/upload/Content/Email/images/icon_Gold130.png";
            string pathF = b2bthai_url + "/Content/Default/images/icon_freesmall.png";
            string pathG = b2bthai_url + "/Content/Default/images/icon_goldsmall.png";
            string pathGE = b2bthai_url + "/Content/Default/images/icon_goldExpiresmall.png";

            EmailDetail["b2bthaiUrl"] = b2bthai_url;
            EmailDetail["CompName"] = UserData.CompName;
            EmailDetail["FirstName"] = UserData.FirstName;
            EmailDetail["pathLogo"] = pathlogo;
            EmailDetail["pathGold"] = pathGold;
            EmailDetail["pathF"] = pathF;
            EmailDetail["pathG"] = pathG;
            EmailDetail["pathGE"] = pathGE;
            #endregion

            if(status == 1)//NearExpireDate ใกล้หมดอายุ
            {
                #region Value For Send Email
                Subject = "แจ้ง B2BThai Package ของคุณใกล้หมดอายุ";
                TimeSpan time = Convert.ToDateTime(UserData.ExpireDate) - Convert.ToDateTime(DateTime.Now);
                if (UserData.ExpireDate != null)
                {
                    EmailDetail["ExpireDate"] = UserData.ExpireDate.Value.ToString("dd/MM/yyyy");
                }

                // data for set msg detail
                EmailDetail["Time"] = " (เหลือเวลาอีก " + time.Days.ToString() + " วัน)";
                ViewBag.Data = EmailDetail;
                Detail = PartialViewToString("UC/Email/SendGoldMemberNearExpire");

                mailTo.Add(Email);
                #endregion
            }
            else if (status == 2)//ExpireDate หมดอายุวันนี้
            {
                #region Value For Send Email
                Subject = "แจ้ง B2BThai Package ของคุณหมดอายุ";
                if (UserData.ExpireDate != null)
                {
                    EmailDetail["ExpireDate"] = UserData.ExpireDate.Value.ToString("dd/MM/yyyy");
                }
                // data for set msg detail
                ViewBag.Data = EmailDetail;
                Detail = PartialViewToString("UC/Email/SendGoldMemberExpire");

                mailTo.Add(Email);
                #endregion
            }
            else if (status == 3)//ExpireDate ต่ออายุการใช้งาน
            {
                #region Set Content & Value For Send Email

                Subject = res.Admin.lblMsgExtendGlodMember + " - " + res.Common.lblDomainShortName;
                string strLifetime = "";
                if (LifeTime == 12)
                {
                    strLifetime = "1 "+res.Common.lblYear;
                }else{
                    strLifetime = Convert.ToString(LifeTime) + res.Common.lblMonth;
                }
                EmailDetail["Main"] = res.Admin.lblGold_status+" (" + strLifetime + ")<br />";
                EmailDetail["Main"] += res.Admin.lblStarted_date+": " + Convert.ToDateTime(UserData.ExpireDate).AddMonths(-LifeTime).ToShortDateString() + "<br />";
                EmailDetail["Main"] += res.Admin.lblExpiry_date+": " + Convert.ToDateTime(UserData.ExpireDate).ToShortDateString();
                EmailDetail["b2bthaiUrl"] = b2bthai_url;
                EmailDetail["CompName"] = UserData.CompName;
                EmailDetail["FirstName"] = UserData.FirstName;
                EmailDetail["pathLogo"] = pathlogo;
                 //data for set msg detail
                ViewBag.Data = EmailDetail;
                Detail = PartialViewToString("UC/Email/SendMailAlerts");

                mailTo.Add(Email);
                #endregion
            }

            IsSend = OnSendByAlertEmail(Subject, mailFrom, mailTo, mailCC, Detail);

            #endregion

            return Json(new { IsSend = IsSend });
        }
        #endregion

        #region DelData
        public ActionResult DelData(List<bool> Check, List<int> ID, List<int> MemID, List<short> RowVersion, string PrimaryKeyName)
        {
            MemberService svMember = new MemberService();
            CompanyService svCompany = new CompanyService();
            ProductService svProduct = new ProductService();
            BuyleadService svBuylead = new BuyleadService();
            var MemberID = "";
            var CompID = "";
            svMember.DelData<emMemberWeb>(Check, ID, RowVersion, PrimaryKeyName);
            if (svMember.IsResult)
            {
                for (int i = 0; i < MemID.Count();i++ )
                {
                    if (Check[i] == true)
                    {
                        MemberID = MemberID + "MemberID = " + MemID[i].ToString() + " or ";
                    }
                }
                if (MemberID != "")
                {
                    MemberID = MemberID.Substring(0, MemberID.Length - 4);
                    
                }
                var Company = svCompany.SelectData<b2bCompany>("*", "IsDelete = 0 and (" + MemberID + ")",null,0,0);
                svCompany.DeleteData<b2bCompany>(Company, "CompID");

                for (int i = 0; i < Company.Count(); i++)
                {
                    CompID = CompID + "CompID = " + Company[i].CompID + " or ";
                }
                if (CompID != "")
                {
                    CompID = CompID.Substring(0, CompID.Length - 4);

                }
                var CompanyPro = svCompany.SelectData<b2bCompanyProfile>("*", "IsDelete = 0 and (" + CompID + ")", null, 0, 0);
                if (CompanyPro.Count() > 0)
                {
                    svCompany.DeleteData<b2bCompanyProfile>(CompanyPro, "CompID");
                }

                var CompanyPay = svCompany.SelectData<b2bCompanyPayment>("*", "IsDelete = 0 and (" + CompID + ")", null, 0, 0);
                if (CompanyPay.Count() > 0)
                {
                    svCompany.DeleteData<b2bCompanyPayment>(CompanyPay, "CompID");
                }
                var CompanyMenu = svCompany.SelectData<b2bCompanyMenu>("*", "IsDelete = 0 and (" + CompID + ")", null, 0, 0);
                if (CompanyMenu.Count() > 0)
                {
                    svCompany.DeleteData<b2bCompanyMenu>(CompanyMenu, "CompID");
                }
                var CompanyCertify = svCompany.SelectData<b2bCompanyCertify>("*", "IsDelete = 0 and (" + CompID + ")", null, 0, 0);
                if (CompanyCertify.Count() > 0)
                {
                    svCompany.DeleteData<b2bCompanyCertify>(CompanyCertify, "CompID");
                }
                var CompanyShip = svCompany.SelectData<b2bCompanyShipment>("*", "IsDelete = 0 and (" + CompID + ")", null, 0, 0);
                if (CompanyShip.Count() > 0)
                {
                    svCompany.DeleteData<b2bCompanyShipment>(CompanyShip, "CompID");
                }
                var CompanyVat = svCompany.SelectData<b2bCompanyVATCertify>("*", "IsDelete = 0 and (" + CompID + ")", null, 0, 0);
                if (CompanyVat.Count() > 0)
                {
                    svCompany.DeleteData<b2bCompanyVATCertify>(CompanyVat, "CompID");
                }
                var CompanyLead = svCompany.SelectData<b2bCompanyLead>("*", "IsDelete = 0 and (" + CompID + ")", null, 0, 0);
                if (CompanyLead.Count() > 0)
                {
                    svCompany.DeleteData<b2bCompanyLead>(CompanyLead, "CompID");
                }
                var product = svProduct.SelectData<b2bProduct>("*", "IsDelete = 0 and (" + CompID + ")", null, 0, 0);
                if (product.Count() > 0)
                {
                    svProduct.DeleteData<b2bProduct>(product, "CompID");
                }
                var buylead = svBuylead.SelectData<b2bBuylead>("*", "IsDelete = 0 and (" + CompID + ")", null, 0, 0);
                if (buylead.Count() > 0)
                {
                    svBuylead.DeleteData<b2bBuylead>(buylead, "CompID");
                }
                    

                return Json(new {Result = true });
            }
            else
            {
                return Json(new { Result = false });
            }
        }
        #endregion

        #region ExtendLifetime
        public ActionResult ExtendLifetime(List<bool> Check, int Value, List<int> MemID, List<short> RowVersion)
        {
            CompanyService svCompany = new CompanyService();
            var MemberID = "";
            for (int i = 0; i < MemID.Count(); i++)
            {
                if (Check[i] == true)
                {
                    MemberID = MemberID + "MemberID = " + MemID[i].ToString() + " or ";
                }
            }
            if (MemberID != "")
            {
                MemberID = MemberID.Substring(0, MemberID.Length - 4);
            }
            var Company = svCompany.SelectData<view_CompMember>("*", "IsDelete = 0 and (" + MemberID + ")", null, 0, 0);
            for (int i = 0; i < Company.Count(); i++)
            {
                if (Company[i].ExpireDate < DateTime.Now)
                {
                    Company[i].ExpireDate = DateTime.Now.AddMonths(Value);
                }
                else {
                    Company[i].ExpireDate = Convert.ToDateTime(Company[i].ExpireDate).AddMonths(Value);
                }
            }
            svCompany.SaveData<view_CompMember>(Company, "CompID");
            if (svCompany.IsResult)
            {
                 for (int i = 0; i < Company.Count(); i++)
                {
                    SendMailAlerts(Company[i].MemberID, Company[i].Email, 3, Value);
                }
                return Json(new { Result = true });
            }
            else
            {
                return Json(new { Result = false });
            }
        }
        #endregion

        #region SendMailStatus
        public ActionResult SendMailStatus(List<bool> Check, int Status, List<int> MemID, List<short> RowVersion)
        {
            CompanyService svCompany = new CompanyService();
            var MemberID = "";
            for (int i = 0; i < MemID.Count(); i++)
            {
                if (Check[i] == true)
                {
                    MemberID = MemberID + "MemberID = " + MemID[i].ToString() + " or ";
                }
            }
            if (MemberID != "")
            {
                MemberID = MemberID.Substring(0, MemberID.Length - 4);
            }
            var Company = svCompany.SelectData<view_CompMember>("*", "IsDelete = 0 and (" + MemberID + ")", null, 0, 0);
            //sendmail
            if (Status == 1)//เมล์แจ้งเตือนใกล้หมดอายุ
            {
                for (int i = 0; i < Company.Count(); i++)
                {
                    SendMailAlerts(Company[i].MemberID, Company[i].Email, Status);
                }
                return Json(new { Result = true });
            }
            else if (Status == 2)//เมล์แจ้งเตือนหมดอายุ
            {
                for (int i = 0; i < Company.Count(); i++)
                {
                    SendMailAlerts(Company[i].MemberID, Company[i].Email, Status);
                }
                return Json(new { Result = true });
            }
            else
            {
                return Json(new { Result = false });
            }
        
        }
        #endregion

        #region ChangeIsTrust
        [HttpPost]
        public bool ChangeIsTrust(int compid,int istrust) {
            CompanyService svCompany = new CompanyService();
            var IsResult = svCompany.UpdateByCondition<b2bCompany>("IsTrust = " + istrust + "", "CompID = " + compid);
            return IsResult;
        }
        #endregion

        #region Validate
        public ActionResult Validate(string username, string email, string compname, string compnameeng, string displayname,int MemberID)
        {
            MemberService svMember = new MemberService();
            CompanyService svCompany = new CompanyService();
            var IsResult = true;
            var sqlSelect = "compid,memberid,username,email,compname";
            var sqlWhere = " and RowflagWeb = 2 and IsDelete = 0 and WebID = " + res.Config.WebID;

            if (!string.IsNullOrEmpty(username))
            {
                var Member = svMember.SelectData<view_emCompanyMember>(sqlSelect, "username = N'" + username + "' AND MemberID != " + MemberID + sqlWhere);
                if (Member.Count() > 0)
                {
                    IsResult = false;
                }
            }

            if (!string.IsNullOrEmpty(email))
            {
                var Member = svMember.SelectData<view_emCompanyMember>(sqlSelect, "email = N'" + email + "' AND MemberID != " + MemberID + sqlWhere);
                if (Member.Count() > 0)
                {
                    IsResult = false;
                }
            }
            if (!string.IsNullOrEmpty(compname))
            {
                var Member = svMember.SelectData<view_emCompanyMember>(sqlSelect, "compname = N'" + compname + "' AND MemberID != " + MemberID + sqlWhere);
                if (Member.Count() > 0)
                {
                    IsResult = false;
                }
            }
            if (!string.IsNullOrEmpty(displayname))
            {
                var Company = svCompany.SelectData<b2bCompany>("compid,displayname", "displayname = N'" + displayname + "' AND IsDelete = 0 AND MemberID != " + MemberID);
                if (Company.Count() > 0)
                {
                    IsResult = false;
                }
            }
            return Json(IsResult);
        }

        #endregion

    }
}