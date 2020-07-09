using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Transactions;
using System.IO;
using System.Text;
using Prosoft.Service;
//using Prosoft.Base;
using Ouikum.Common;
using Ouikum;
using System.Collections;
using Telerik.Web.Mvc;
using System.Threading;
using Ouikum.Product;
using Ouikum.Company;
using Ouikum.BizType;
using Ouikum.Article;
using System.Text.RegularExpressions;
using res = Prosoft.Resource.Web.Ouikum;
using Ouikum.Message;
using Ouikum.Quotation;
using Ouikum.Web.Models;
using Ouikum.Controllers;

namespace Ouikum.Web.MyB2B
{
    public class CompanyController : BaseSecurityController
    {

        #region Members
        //
        // GET: /MyB2B/Company/
        BizTypeService svBizType;
        MemberService svMember;
        AddressService svAddress;
        CompanyService svCompany;
        ProductService svProduct;
        #endregion

        #region Constructors
        public CompanyController()
        {
            svBizType = new BizTypeService();
            svMember = new MemberService();
            svAddress = new AddressService();
            svCompany = new CompanyService();
            svProduct = new ProductService();
        }
        #endregion

        #region SelectCompany
        public view_Company SelectCompany() {
            var company = new view_Company();
            if (CheckIsLogin())
            {
                var Companies = svCompany.SelectData<view_Company>("*", "IsDelete = 0 AND emCompID =" + LogonEMCompID, null, 0, 0, false);
                 if (Companies.Count() > 0)
                 {
                   company =  Companies.First();
                 }
               
            }
            CountMessage();
            CountQuotation();
            ViewBag.CateLevel1 = LogonCompLevel;
            ViewBag.MemberType = LogonMemberType;

            return company;
        }
        #endregion

        #region Company Account
        public ActionResult CompanyAccount()
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
                var Companies = SelectCompany();
                if (Companies.CompID != null && Companies.CompID > 0)
                {
                    var member = svMember.SelectData<emMember>("MemberID,MemberType", "IsDelete = 0 AND MemberID =" + Companies.MemberID).First();
                    string[] CompImg = new string[3];
                    string[] strImg = null;
                    var Provinces = svAddress.ListProvince().ToList();
                    var Districts = svAddress.SelectData<emDistrict>("DistrictID,DistrictName", "IsDelete = 0 AND ProvinceID = " + Companies.CompProvinceID);
                    var BizTypes = svBizType.SelectData<b2bBusinessType>("BizTypeID,BizTypeName", "IsDelete = 0");

                    if (!string.IsNullOrEmpty(Companies.CompImgPath))
                    {
                        strImg = Companies.CompImgPath.ToString().Split(',');
                    }
                    if (strImg != null)
                    {
                        for (int i = 0; i < strImg.Length; i++)
                        {
                            CompImg[i] = strImg[i];
                        }
                    }
                    ViewBag.EnumMemberType = svCommon.SelectEnum(CommonService.EnumType.MemberType);
                    ViewBag.EnumAdminType = svCommon.SelectEnum(CommonService.EnumType.AdminType);
                    ViewBag.CompLevel = LogonCompLevel;
                    ViewBag.CompImgName_1 = CompImg[0];
                    ViewBag.CompImgName_2 = CompImg[1];
                    ViewBag.CompImgName_3 = CompImg[2];
                    ViewBag.MemberType = member.MemberType;
                    ViewBag.Companies = Companies;
                    ViewBag.Provinces = Provinces;
                    ViewBag.Districts = Districts;
                    ViewBag.BizTypes = BizTypes;
                    ViewBag.PageType = "Company";
                    ViewBag.MenuName = "Account";
                    return View();
                }
                else
                {
                    return Redirect(res.Pageviews.PvNotFound);
                }
            }
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CompanyAccount(FormCollection form)
        {

            Hashtable data = new Hashtable();
            var b2bCompanies = new b2bCompany();
            var emCompanies = new emCompany();
            string compimg;
            string complogo;
            try
            {
                var Company = svCompany.SelectData<b2bCompany>("*", " CompID = " + LogonCompID + "");
                b2bCompanies = Company.First();
                if (Company.Count < 1)
                {
                    data.Add("result", false);
                    //data.Add("RowVersion", form["RowVersion"]);
                    return Json(data);
                }

                #region set ค่า b2bCompany
                compimg = b2bCompanies.CompImgPath;
                complogo = b2bCompanies.LogoImgPath;
                var type = Convert.ToInt32(form["Type"]);

                if (type == 0)
                {
                    b2bCompanies.IsCompSameAddr = DataManager.ConvertToBool(form["IsCompSameAddr"]);
                    b2bCompanies.ServiceType = Convert.ToByte(form["ServiceType"]);
                    b2bCompanies.DisplayName = form["DisplayName"];
                    b2bCompanies.LogoImgPath = form["LogoImgPath"];
                    b2bCompanies.CompShortDes = ReplaceText(form["CompShortDes"]);
                    b2bCompanies.MainCustomer = form["MainCustomer"];
                    b2bCompanies.SecondaryCustomer = form["SecondaryCustomer"];
                    b2bCompanies.CompName = form["CompName"];
                    b2bCompanies.CompNameEng = form["CompNameEng"];
                    b2bCompanies.CompImgPath = form["CompImgPath"];
                    b2bCompanies.CompAddrLine1 = form["CompAddrLine1"];
                    b2bCompanies.CompDistrictID = Convert.ToInt32(form["CompDistrictID"]);
                    b2bCompanies.CompProvinceID = Convert.ToInt32(form["CompProvinceID"]);
                    b2bCompanies.CompPostalCode = form["CompPostalCode"];
                    b2bCompanies.CompPhone = form["CompPhone"];
                    b2bCompanies.CompMobile = form["CompMobile"];
                    b2bCompanies.CompFax = form["CompFax"];
                    b2bCompanies.BizTypeID = Convert.ToInt32(form["BizTypeID"]);
                    if (!string.IsNullOrEmpty(form["BizTypeOther"]) && Convert.ToInt32(form["BizTypeID"]) == 13)
                    {
                        b2bCompanies.BizTypeOther = form["BizTypeOther"];
                    }
                    b2bCompanies.CompWebsiteUrl = form["CompWebsiteUrl"];
                    b2bCompanies.FacebookUrl = form["FacebookUrl"];
                    b2bCompanies.LineID = form["LineID"];
                }
                else if (type == 1){
                    b2bCompanies.ServiceType = Convert.ToByte(form["ServiceType"]);
                    b2bCompanies.DisplayName = form["DisplayName"];
                }
                else if (type == 2)
                {
                    b2bCompanies.CompShortDes = ReplaceText(form["CompShortDes"]);
                    b2bCompanies.CompImgPath = form["CompImgPath"];
                }
                else if (type == 3)
                {
                    b2bCompanies.MainCustomer = form["MainCustomer"];
                    b2bCompanies.SecondaryCustomer = form["SecondaryCustomer"];
                }
                else if (type == 4)
                {
                    b2bCompanies.IsCompSameAddr = DataManager.ConvertToBool(form["IsCompSameAddr"]);
                    b2bCompanies.LogoImgPath = form["LogoImgPath"];
                    b2bCompanies.CompName = form["CompName"];
                    b2bCompanies.CompNameEng = form["CompNameEng"];
                    b2bCompanies.CompAddrLine1 = form["CompAddrLine1"];
                    b2bCompanies.CompDistrictID = Convert.ToInt32(form["CompDistrictID"]);
                    b2bCompanies.CompProvinceID = Convert.ToInt32(form["CompProvinceID"]);
                    b2bCompanies.CompPostalCode = form["CompPostalCode"];
                    b2bCompanies.CompPhone = form["CompPhone"];
                    b2bCompanies.CompMobile = form["CompMobile"];
                    b2bCompanies.CompFax = form["CompFax"];
                    b2bCompanies.BizTypeID = Convert.ToInt32(form["BizTypeID"]);
                    if (!string.IsNullOrEmpty(form["BizTypeOther"]) && Convert.ToInt32(form["BizTypeID"]) == 13)
                    {
                        b2bCompanies.BizTypeOther = form["BizTypeOther"];
                    }
                    b2bCompanies.CompWebsiteUrl = form["CompWebsiteUrl"];
                    b2bCompanies.FacebookUrl = form["FacebookUrl"];
                    b2bCompanies.LineID = form["LineID"];
                }
                #endregion

                #region Update b2bCompany
                b2bCompanies = svCompany.SaveData<b2bCompany>(b2bCompanies, "CompID");
                //var mg = new KeywordMongo();
                //mg.UpdateCompNameByCompID(b2bCompanies.CompID, b2bCompanies.CompName, b2bCompanies.IsSME,b2bCompanies.CompLevel);
                #endregion

                if (svCompany.IsResult)
                {
                    emCompanies = svMember.SelectData<emCompany>("*", " CompID = " + b2bCompanies.emCompID).First();

                    #region set ค่า emCompany
                    emCompanies.LogoImgPath = b2bCompanies.LogoImgPath;
                    emCompanies.CompName = b2bCompanies.CompName;
                    emCompanies.DisplayName = b2bCompanies.DisplayName;
                    emCompanies.CompNameEng = b2bCompanies.CompNameEng;
                    emCompanies.CompAddrLine1 = b2bCompanies.CompAddrLine1;
                    emCompanies.CompDistrictID = b2bCompanies.CompDistrictID;
                    emCompanies.CompProvinceID = b2bCompanies.CompProvinceID;
                    emCompanies.CompPostalCode = b2bCompanies.CompPostalCode;
                    emCompanies.CompPhone = b2bCompanies.CompPhone;
                    emCompanies.CompMobile = b2bCompanies.CompMobile;
                    emCompanies.CompFax = b2bCompanies.CompFax;
                    emCompanies.BizTypeID = b2bCompanies.BizTypeID;
                    emCompanies.BizTypeOther = b2bCompanies.BizTypeOther;
                    emCompanies.CompWebsiteUrl = b2bCompanies.CompWebsiteUrl;
                    emCompanies.RowVersion = b2bCompanies.RowVersion;
                    #endregion

                    #region Update emCompany
                    emCompanies = svMember.SaveData<emCompany>(emCompanies, "CompID");
                    data.Add("result", svMember.IsResult);
                    #endregion

                    if (svCompany.IsResult && svMember.IsResult)
                    {
                        #region SaveLogo
                        if (form["LogoImgPath"] != "")
                        {
                            if (b2bCompanies.LogoImgPath != complogo)
                            {
                                imgManager = new FileHelper();

                                //#region Delete Folder
                                //imgManager.DeleteFilesInDir("Companies/Logo/" + b2bCompanies.CompID);
                                //#endregion
                                imgManager.DirPath = "Companies/Logo/" + b2bCompanies.CompID;
                                imgManager.DirTempPath = "Temp/Companies/Logo/" + b2bCompanies.CompID;
                                imgManager.ImageName = form["LogoImgPath"];
                                //imgManager.ImageThumbName = "Thumb_" + form["LogoImgPath"];
                                imgManager.FullHeight = 150;
                                imgManager.FullWidth = 150;
                                //imgManager.ThumbHeight = 150;
                                //imgManager.ThumbWidth = 150;

                                imgManager.SaveImageFromTemp();
                            }
                        }
                        #endregion

                        #region SaveCompImg
                        if (!string.IsNullOrEmpty(b2bCompanies.CompImgPath))
                        {
                            if (b2bCompanies.CompImgPath != compimg)
                            {

                                imgManager = new FileHelper();
                                var NewCompImgPath = form["CompImgPath"].ToString().Split(',');
                                var OldCompImgPath = b2bCompanies.CompImgPath.ToString().Split(',');

                                imgManager.DirPath = "Companies/Image/" + b2bCompanies.CompID;
                                imgManager.DirTempPath = "Temp/Companies/Image/" + b2bCompanies.CompID;
                                int sizeThumb = 150;
                                int sizeImg = 450;

                                #region Save File Image
                                SaveFileImage(imgManager.DirTempPath, imgManager.DirPath, NewCompImgPath, sizeThumb, sizeImg);
                                #endregion

                                #region Check & Delete File Image
                                imgManager.DirPath = "Companies/Image/" + b2bCompanies.CompID;
                                DeleteFileImage(imgManager.DirPath, OldCompImgPath, NewCompImgPath);
                                #endregion
                            }
                        }
                        else
                        {
                            imgManager = new FileHelper();
                            var NewCompImgPath = form["CompImgPath"].ToString().Split(',');

                            imgManager.DirPath = "Companies/Image/" + b2bCompanies.CompID;
                            imgManager.DirTempPath = "Temp/Companies/Image/" + b2bCompanies.CompID;
                            int sizeThumb = 150;
                            int sizeImg = 450;

                            #region Save File Image
                            SaveFileImage(imgManager.DirTempPath, imgManager.DirPath, NewCompImgPath, sizeThumb, sizeImg);
                            #endregion
                        }

                        #endregion

                        SetLogonStatus();

                        //var RowVersion = b2bCompanies.RowVersion + 1;
                        //data.Add("RowVersion", RowVersion);
                    }
                }
                else
                {
                    data.Add("RowVersion", b2bCompanies.RowVersion);
                }

                return Json(data);
            }
            catch (Exception ex)
            {
                CreateLogFiles(ex);
                return Json(data);
            }
        }
        #endregion

        #region Company Contact
        public ActionResult CompanyContact()
        {
            RememberURL();
            if (!CheckIsLogin())
            {
                return Redirect(res.Pageviews.PvMemberSignIn);
            }
            else
            {
                GetStatusUser();
                var Companies = SelectCompany();
                var Provinces = svAddress.ListProvince().ToList();
                var Districts = svAddress.SelectData<emDistrict>("DistrictID,DistrictName", "IsDelete = 0 AND ProvinceID = " + Companies.ContactProvinceID);

                if (Companies.ContactDistrictID != null)
                {
                    foreach (var it in Districts.Where(m => m.DistrictID == Companies.ContactDistrictID))
                    {
                        ViewBag.ContactDistrictName = it.DistrictName;
                    }
                }
                if (Companies.ContactDistrictID != null)
                {
                    foreach (var it in Provinces.Where(m => m.ProvinceID == Companies.ContactProvinceID))
                    {
                        ViewBag.ContactProvinceName = it.ProvinceName;
                        if (Companies.GMapLatitude == null && Companies.GMapLongtitude == null && Companies.GPinLatitude == null && Companies.GPinLongtitude == null && (Companies.GZoom == null || Companies.GZoom == 0))
                        {
                            ViewBag.GMapLatitude = it.Latitude;
                            ViewBag.GPinLatitude = it.Latitude;
                            ViewBag.GMapLongtitude = it.Longitude;
                            ViewBag.GPinLongtitude = it.Longitude;
                            ViewBag.GZoom = 12;
                        }
                    }
                }
                else
                {
                    ViewBag.GMapLatitude = 13.750056809568887;
                    ViewBag.GPinLatitude = 13.750056809568887;
                    ViewBag.GMapLongtitude = 100.49468994140625;
                    ViewBag.GPinLongtitude = 100.49468994140625;
                    ViewBag.GZoom = 12;
                }

                ViewBag.CompLevel = LogonCompLevel;
                ViewBag.Companies = Companies;
                ViewBag.Provinces = Provinces;
                ViewBag.Districts = Districts;
                ViewBag.PageType = "Company";
                ViewBag.MenuName = "Contact";
                return View();
            }
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult CompanyContact(FormCollection form)
        {
            Hashtable data = new Hashtable();
            var b2bCompanies = new b2bCompany();
            var emCompanies = new emCompany();
            string Mapimg;
            string Contimg;

            try
            {
                var Company = svCompany.SelectData<b2bCompany>("*", " CompID = " + LogonCompID + "");
                b2bCompanies = Company.First();
                if (Company.Count < 1)
                {
                    data.Add("result", false);
                    //data.Add("RowVersion", form["RowVersion"]);
                    return Json(data);
                }

                #region set ค่า b2bCompany
                Mapimg = b2bCompanies.MapImgPath;
                Contimg = b2bCompanies.ContactImgPath;
                var type = Convert.ToInt32(form["Type"]);

                if (type == 0)
                {
                    b2bCompanies.IsContSameAddr = DataManager.ConvertToBool(form["IsContSameAddr"]);
                    b2bCompanies.ContactFirstName = form["ContactFirstName"];
                    b2bCompanies.ContactLastName = form["ContactLastName"];
                    b2bCompanies.ContactPositionName = form["ContactPositionName"];
                    b2bCompanies.ContactImgPath = form["ContactImgPath"];
                    b2bCompanies.ContactAddrLine1 = form["ContactAddrLine1"];
                    b2bCompanies.ContactDistrictID = Convert.ToInt32(form["ContactDistrictID"]);
                    b2bCompanies.ContactProvinceID = Convert.ToInt32(form["ContactProvinceID"]);
                    b2bCompanies.ContactPostalCode = form["ContactPostalCode"];
                    b2bCompanies.ContactEmail = form["ContactEmail"];
                    b2bCompanies.ContactPhone = form["ContactPhone"];
                    b2bCompanies.ContactMobile = form["ContactMobile"];
                    b2bCompanies.ContactFax = form["ContactFax"];
                    b2bCompanies.MapImgPath = form["MapImgPath"];
                    b2bCompanies.CompMapDetail = ReplaceText(form["CompMapDetail"]);
                    b2bCompanies.GMapLatitude = form["GMapLatitude"];
                    b2bCompanies.GMapLongtitude = form["GMapLongtitude"];
                    b2bCompanies.GPinLatitude = form["GPinLatitude"];
                    b2bCompanies.GPinLongtitude = form["GPinLongtitude"];
                    b2bCompanies.GZoom = DataManager.ConvertToShort(form["GZoom"]);
                }
                else if (type == 1)
                {
                    b2bCompanies.IsContSameAddr = DataManager.ConvertToBool(form["IsContSameAddr"]);
                    b2bCompanies.ContactFirstName = form["ContactFirstName"];
                    b2bCompanies.ContactLastName = form["ContactLastName"];
                    b2bCompanies.ContactPositionName = form["ContactPositionName"];
                    b2bCompanies.ContactImgPath = form["ContactImgPath"];
                    b2bCompanies.ContactAddrLine1 = form["ContactAddrLine1"];
                    b2bCompanies.ContactDistrictID = Convert.ToInt32(form["ContactDistrictID"]);
                    b2bCompanies.ContactProvinceID = Convert.ToInt32(form["ContactProvinceID"]);
                    b2bCompanies.ContactPostalCode = form["ContactPostalCode"];
                    b2bCompanies.ContactEmail = form["ContactEmail"];
                    b2bCompanies.ContactPhone = form["ContactPhone"];
                    b2bCompanies.ContactMobile = form["ContactMobile"];
                    b2bCompanies.ContactFax = form["ContactFax"];
                }
                else if (type == 2)
                {
                    b2bCompanies.MapImgPath = form["MapImgPath"];
                    b2bCompanies.CompMapDetail = ReplaceText(form["CompMapDetail"]);
                }
                else if (type == 3)
                {
                    b2bCompanies.GMapLatitude = form["GMapLatitude"];
                    b2bCompanies.GMapLongtitude = form["GMapLongtitude"];
                    b2bCompanies.GPinLatitude = form["GPinLatitude"];
                    b2bCompanies.GPinLongtitude = form["GPinLongtitude"];
                    b2bCompanies.GZoom = DataManager.ConvertToShort(form["GZoom"]);
                }
                #endregion

                #region Update b2bCompany
                b2bCompanies = svCompany.SaveData<b2bCompany>(b2bCompanies, "CompID");
                #endregion

                if (svCompany.IsResult)
                {
                    emCompanies = svMember.SelectData<emCompany>("*", " CompID = " + b2bCompanies.emCompID).First();
                    #region set ค่า emCompany
                    emCompanies.MapImgPath = b2bCompanies.MapImgPath;
                    emCompanies.CompEmail = b2bCompanies.ContactEmail;
                    emCompanies.GMapLatitude = b2bCompanies.GMapLatitude;
                    emCompanies.GMapLongtitude = b2bCompanies.GMapLongtitude;
                    emCompanies.GPinLatitude = b2bCompanies.GPinLatitude;
                    emCompanies.GPinLongtitude = b2bCompanies.GPinLongtitude;
                    emCompanies.GZoom = b2bCompanies.GZoom;
                    //emCompanies.RowVersion = b2bCompanies.RowVersion;
                    #endregion

                    #region Update emCompany
                    emCompanies = svMember.SaveData<emCompany>(emCompanies, "CompID");
                    data.Add("result", svMember.IsResult);
                    #endregion

                    if (svCompany.IsResult && svMember.IsResult)
                    {
                        #region SaveContImg
                        if (form["ContactImgPath"] != "")
                        {
                            if (b2bCompanies.ContactImgPath != Contimg)
                            {
                                imgManager = new FileHelper();
                                imgManager.DirPath = "Companies/Contact/" + b2bCompanies.CompID;
                                imgManager.DirTempPath = "Temp/Companies/Contact/" + b2bCompanies.CompID;
                                imgManager.ImageName = form["ContactImgPath"];
                                //imgManager.ImageThumbName = "Thumb_" + form["ContactImgPath"];
                                imgManager.FullHeight = 150;
                                imgManager.FullWidth = 150;
                                //imgManager.ThumbHeight = 150;
                                //imgManager.ThumbWidth = 150;

                                imgManager.SaveImageFromTemp();
                            }
                        }
                        #endregion

                        #region SaveMapImg
                        if (form["MapImgPath"] != "")
                        {
                            if (b2bCompanies.MapImgPath != Mapimg)
                            {
                                imgManager = new FileHelper();
                                imgManager.DirPath = "Companies/Map/" + b2bCompanies.CompID;
                                imgManager.DirTempPath = "Temp/Companies/Map/" + b2bCompanies.CompID;
                                imgManager.ImageName = form["MapImgPath"];
                                //imgManager.ImageThumbName = "Thumb_" + form["MapImgPath"];
                                imgManager.FullHeight = 0;
                                imgManager.FullWidth = 0;
                                imgManager.ThumbHeight = 350;
                                imgManager.ThumbWidth = 500;

                                imgManager.SaveImageFromTemp();
                            }
                        }
                        #endregion

                        var RowVersion = b2bCompanies.RowVersion + 1;
                        //data.Add("RowVersion", RowVersion);
                    }
                }
                else
                {
                    //data.Add("RowVersion", b2bCompanies.RowVersion);
                }

                return Json(data);
            }
            catch (Exception ex)
            {
                CreateLogFiles(ex);
                return Json(data);
            }
        }
        #endregion

        #region Company Profile
        public ActionResult CompanyProfile()
        {
            RememberURL();
            if (!CheckIsLogin())
            {
                return Redirect(res.Pageviews.PvMemberSignIn);
            }
            else
            {
                GetStatusUser();
                var Companies = SelectCompany();
                var CompProfiles = svCompany.SelectData<view_CompanyProfile>("*", "IsDelete = 0 AND CompID =" + Companies.CompID).First();
                var Provinces = svAddress.ListProvince().ToList();
                var Districts = svAddress.ListDistrict().Where(m => m.ProvinceID == CompProfiles.ProvinceID).ToList();
                var BizTypes = svBizType.GetBiztype().ToList();
                if (CompProfiles.CompRegisDate != null)
                {
                    CompProfiles.CompRegisDate = DataManager.ConvertToDateTime(CompProfiles.CompRegisDate).AddYears(-543);
                }

                if (CompProfiles.CompBizType == 13)
                {
                    ViewBag.CompBizTypeOther = CompProfiles.CompBizTypeOther;
                }

                ViewBag.CompLevel = LogonCompLevel;
                ViewBag.CompProfiles = CompProfiles;
                ViewBag.Provinces = Provinces;
                ViewBag.Districts = Districts;
                ViewBag.BizTypes = BizTypes;
                ViewBag.PageType = "Company";
                ViewBag.MenuName = "Profile";
                return View();
            }
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult CompanyProfile(FormCollection form)
        {
            Hashtable data = new Hashtable();
            var b2bCompProfile = new b2bCompanyProfile();
            var emCompProfile = new emCompanyProfile();
            try
            {
                var Companies = SelectCompany();
                var CompProfiles = svCompany.SelectData<b2bCompanyProfile>("*", "IsDelete = 0 AND CompID =" + Companies.CompID);
                if (CompProfiles.Count < 1)
                {
                    data.Add("result", false);
                    data.Add("RowVersion", form["RowVersion"]);
                    return Json(data);
                }
                #region set ค่า b2bCompProfile
                b2bCompProfile = CompProfiles.First();
                b2bCompProfile.IsSameAddr = DataManager.ConvertToBool(form["IsSameAddr"]);
                b2bCompProfile.CompName = form["CompName"];
                b2bCompProfile.AddrLine1 = form["AddrLine1"];
                b2bCompProfile.DistrictID = Convert.ToInt32(form["DistrictID"]);
                b2bCompProfile.ProvinceID = Convert.ToInt32(form["ProvinceID"]);
                b2bCompProfile.PostalCode = form["PostalCode"];
                b2bCompProfile.CeoName = form["CeoName"];
                b2bCompProfile.CompBizType = Convert.ToInt32(form["CompBizType"]);
                if (b2bCompProfile.CompBizType == 13)
                {
                    b2bCompProfile.CompBizTypeOther = form["CompBizTypeOther"];
                }
                b2bCompProfile.ComercialNo = form["ComercialNo"];
                if (form["CompRegisDate"] != null && form["CompRegisDate"] != "")
                {
                    b2bCompProfile.CompRegisDate = DataManager.ConvertToDateTime(form["CompRegisDate"]).AddYears(543);
                }
                else
                {
                    b2bCompProfile.CompRegisDate = null;
                }
                b2bCompProfile.RowVersion = DataManager.ConvertToShort(form["RowVersion"]);
                #endregion

                #region Update b2bCompanyprofile
                b2bCompProfile = svCompany.SaveData<b2bCompanyProfile>(b2bCompProfile, "CompProfileID");
                #endregion

                if (svCompany.IsResult)
                {
                    emCompProfile = svMember.SelectData<emCompanyProfile>("*", " CompProfileID = " + b2bCompProfile.emCompProfileID).First();
                    #region set ค่า emCompProfile
                    emCompProfile.IsSameAddr = b2bCompProfile.IsSameAddr;
                    emCompProfile.CompName = b2bCompProfile.CompName;
                    emCompProfile.AddrLine1 = b2bCompProfile.AddrLine1;
                    emCompProfile.DistrictID = b2bCompProfile.DistrictID;
                    emCompProfile.ProvinceID = b2bCompProfile.ProvinceID;
                    emCompProfile.PostalCode = b2bCompProfile.PostalCode;
                    emCompProfile.CeoName = b2bCompProfile.CeoName;
                    emCompProfile.CompBizType = DataManager.ConvertToByte(b2bCompProfile.CompBizType);
                    if (emCompProfile.CompBizType == 13)
                    {
                        emCompProfile.CompBizTypeOther = b2bCompProfile.CompBizTypeOther;
                    }
                    emCompProfile.ComercialNo = b2bCompProfile.ComercialNo;
                    emCompProfile.CompRegisDate = b2bCompProfile.CompRegisDate;
                    if (b2bCompProfile.CompRegisDate != null)
                    {
                        emCompProfile.CompRegisDate = DataManager.ConvertToDateTime(b2bCompProfile.CompRegisDate).AddYears(543);
                    }
                    else
                    {
                        emCompProfile.CompRegisDate = null;
                    }
                    emCompProfile.RowVersion = b2bCompProfile.RowVersion;

                    #endregion

                    #region Update emCompProfile
                    using (var trans = new TransactionScope())
                    {
                        emCompProfile = svMember.SaveData<emCompanyProfile>(emCompProfile, "CompProfileID");
                        data.Add("result", svMember.IsResult);
                        trans.Complete();
                    }
                    #endregion

                    if (svCompany.IsResult && svMember.IsResult)
                    {
                        var RowVersion = b2bCompProfile.RowVersion + 1;
                        data.Add("RowVersion", RowVersion);
                    }
                }
                else
                {
                    data.Add("RowVersion", b2bCompProfile.RowVersion);
                }

                return Json(data);
            }
            catch (Exception ex)
            {
                CreateLogFiles(ex);
                return Json(data);
            }
        }
        #endregion

        #region Company History
        public ActionResult CompanyHistory()
        {
            RememberURL();
            if (!CheckIsLogin())
            {
                return Redirect(res.Pageviews.PvMemberSignIn);
            }
            else
            {
                ViewBag.CompLevel = LogonCompLevel;
                GetStatusUser();
                var Companies = SelectCompany();
                ViewBag.Companies = Companies;
                ViewBag.PageType = "Company";
                ViewBag.MenuName = "History";
                return View();
            }
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult CompanyHistory(FormCollection form)
        {
            Hashtable data = new Hashtable();
            var b2bCompanies = new b2bCompany();
            var emCompanies = new emCompany();
            try
            {
                var Company = svCompany.SelectData<b2bCompany>("*", " CompID = " + LogonCompID + "");

                if (Company.Count < 1)
                {
                    data.Add("result", false);
                    //data.Add("RowVersion", form["RowVersion"]);
                    return Json(data);
                }
                #region set ค่า b2bCompany
                b2bCompanies = Company.First();
                var type = Convert.ToInt32(form["Type"]);

                if (type == 0)
                {
                    b2bCompanies.CompHistory = ReplaceText(form["CompHistory"]);
                    b2bCompanies.CompFounder = form["CompFounder"];
                    b2bCompanies.CompOwner = form["CompOwner"];
                    b2bCompanies.YearEstablished = form["YearEstablished"];
                    b2bCompanies.EmployeeCount = form["EmployeeCount"];
                }
                else if (type == 1)
                {
                    b2bCompanies.CompHistory = ReplaceText(form["CompHistory"]);
                }
                else if (type == 1)
                {
                    b2bCompanies.CompFounder = form["CompFounder"];
                    b2bCompanies.CompOwner = form["CompOwner"];
                    b2bCompanies.YearEstablished = form["YearEstablished"];
                    b2bCompanies.EmployeeCount = form["EmployeeCount"];
                }
                #endregion

                #region Update b2bCompany
                b2bCompanies = svCompany.SaveData<b2bCompany>(b2bCompanies, "CompID");
                #endregion

                if (svCompany.IsResult)
                {
                    var RowVersion = b2bCompanies.RowVersion + 1;
                    data.Add("result", true);
                    //data.Add("RowVersion", RowVersion);

                }
                else
                {
                    //data.Add("RowVersion", b2bCompanies.RowVersion);
                }

                return Json(data);
            }
            catch (Exception ex)
            {
                CreateLogFiles(ex);
                return Json(data);
            }
        }
        #endregion

        #region Company Certify

        #region Get: Certify
        public ActionResult CompanyCertify()
        {
            RememberURL();
            if (!CheckIsLogin())
            {
                return Redirect(res.Pageviews.PvMemberSignIn);
            }
            else
            {
                ViewBag.CateLevel1 = LogonCompLevel;
                ViewBag.MemberType = LogonMemberType;
                var Certifies = svCompany.SelectData<b2bCompanyCertify>("*", "IsDelete = 0 and CompID = " + LogonCompID, "ListNo ASC");
                ViewBag.CompLevel = LogonCompLevel;
                ViewBag.ListNoMax = Certifies;
                ViewBag.PageType = "Company";
                ViewBag.MenuName = "Certify";
                GetStatusUser();
                if (LogonCompLevel == 3)
                {
                    SetPager();
                    return View();
                }
                else
                {
                    return Redirect(res.Pageviews.PvAccessDenied);
                }
            }
        }
        #endregion

        #region Post: Certify
        [HttpPost]
        public ActionResult CompanyCertify(FormCollection form)
        {
            SelectList_PageSize();
            SetPager(form);
            var Certifies = svCompany.SelectData<b2bCompanyCertify>("*", "IsDelete = 0 and CertifyName LIKE N'%" + form["CertifyName_search"] + "%' and CompID = " + LogonCompID, "ListNo ASC", (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
            ViewBag.Certifies = Certifies;
            ViewBag.TotalPage = svCompany.TotalPage;
            ViewBag.TotalRow = svCompany.TotalRow;
            return PartialView("MyB2B/Company/Grid/CertifyGrid");
        }
        #endregion

        #region SaveCompanyCertify
        [HttpPost, ValidateInput(false)]
        public ActionResult SaveCompanyCertify(FormCollection form)
        {
            int objState = DataManager.ConvertToInteger(form["objState"]);//objState 1 คือ insert objState 2 คือ update
            var CompCertifies = new b2bCompanyCertify();
            if (objState == 2)// update
            {
                CompCertifies = svCompany.SelectData<b2bCompanyCertify>("*", " CompCertifyID = " + form["CompCertifyID"] + " AND RowVersion = " + form["RowVersion"]).First();
            }

            #region set ค่า b2bCompanycertify
            var CertifyImgPath = CompCertifies.CertifyImgPath;
            var ListNo = svCompany.SelectData<b2bCompanyCertify>("*", "IsDelete = 0 and CompID = " + LogonCompID, "ListNo ASC");
            CompCertifies.CompID = DataManager.ConvertToInteger(LogonCompID);
            CompCertifies.CertifyName = form["CertifyName"];
            CompCertifies.Remark = form["Remark"];
            CompCertifies.CertifyImgPath = form["CertifyImgPath"];
            if (objState == 2)// update
            {

                CompCertifies.RowVersion = DataManager.ConvertToShort(CompCertifies.RowVersion + 1);
            }
            else
            {
                CompCertifies.RowFlag = 1;
                CompCertifies.RowVersion = 1;
                CompCertifies.CreatedBy = "sa";
                CompCertifies.ModifiedBy = "sa";
                CompCertifies.ModifiedDate = DateTime.Now;
                CompCertifies.CreatedDate = DateTime.Now;
                CompCertifies.ListNo = ListNo.Count() + 1;
            }
            #endregion

            #region Save b2bCompanyCertify
            CompCertifies = svCompany.SaveData<b2bCompanyCertify>(CompCertifies, "CompCertifyID");
            #endregion

            if (svCompany.IsResult)
            {
                #region SaveCertifyImg
                if (!string.IsNullOrEmpty(form["CertifyImgPath"]))
                {
                    if (CompCertifies.CertifyImgPath != CertifyImgPath)
                    {
                        imgManager = new FileHelper();
                        imgManager.DirPath = "CompanyCertify/" + LogonCompID + "/" + CompCertifies.CompCertifyID;
                        imgManager.DirTempPath = "Temp/CompanyCertify/" + LogonCompID;
                        imgManager.ImageName = form["CertifyImgPath"];
                        //imgManager.ImageThumbName = "Thumb_" + form["CertifyImgPath"]; 
                        imgManager.FullHeight = 0;
                        imgManager.FullWidth = 0;
                        imgManager.ThumbHeight = 150;
                        imgManager.ThumbWidth = 150;

                        imgManager.SaveImageFromTemp();
                    }
                }
                #endregion

                return Content("true");
            }
            else
            {
                return Content(null);
            }
        }
        #endregion

        #region EditCertify
        [HttpPost, ValidateInput(false)]
        public ActionResult EditCertify(FormCollection form)
        {
            var CompCertifies = new b2bCompanyCertify();
            if (!string.IsNullOrEmpty(form["CompCertifyID"]))
            {
                CompCertifies = svCompany.SelectData<b2bCompanyCertify>("CompCertifyID,CompID,CertifyName,CertifyImgPath,Remark,RowVersion", " IsDelete = 0 AND CompCertifyID =" + form["CompCertifyID"]).First();
            }
            return Json(new { CompCertifyID = CompCertifies.CompCertifyID, CompID = CompCertifies.CompID, CertifyName = CompCertifies.CertifyName, CertifyImgPath = CompCertifies.CertifyImgPath, Remark = CompCertifies.Remark, RowVersion = CompCertifies.RowVersion });
        }
        #endregion

        #region Listcertify

        [HttpPost]
        public ActionResult ChangeListNoCertify(List<int> id)
        {
            if (!CheckIsLogin())
                return Redirect(res.Pageviews.PvMemberSignIn);
            var svCertify = new CompanyService();
            var certify = new b2bCompanyCertify();
            try
            {
                List<int> list = new List<int>();
                var id_banner1 = id[1];
                var id_banner2 = id[0];
                certify = svCertify.SelectData<b2bCompanyCertify>("*", " CompCertifyID = " + id_banner1).First();
                list.Add(Convert.ToInt32(certify.ListNo));
                certify = svCertify.SelectData<b2bCompanyCertify>("*", " CompCertifyID = " + id_banner2).First();
                list.Add(Convert.ToInt32(certify.ListNo));

                svCertify.UpdateCertifyListNo(id, list);
            }
            catch (Exception ex)
            {
                CreateLogFiles(ex);
            }

            return Json(new { IsResult = svCertify.IsResult, MsgError = GenerateMsgError(svCertify.MsgError), ID = id });
        }

        #endregion

        #endregion

        #region Company Payment

        #region Get: Payment
        public ActionResult CompanyPayment()
        {
            RememberURL();
            if (!CheckIsLogin())
            {
                return Redirect(res.Pageviews.PvMemberSignIn);
            }
            else
            {
                var Payments = svCompany.SelectData<b2bCompanyPayment>("*", "IsDelete = 0 and CompID = " + LogonCompID, "ListNo ASC");
                ViewBag.CateLevel1 = LogonCompLevel;
                ViewBag.MemberType = LogonMemberType;
                ViewBag.CompLevel = LogonCompLevel;
                ViewBag.PageType = "Company";
                ViewBag.MenuName = "Payment";
                GetStatusUser();
                //if (LogonCompLevel == 3)
                //{
                    var Banks = svAddress.SelectData<emBank>("BankID,BankName", "IsDelete = 0");
                    ViewBag.Banks = Banks;
                    SetPager();
                    return View();
                //}
                //else
                //{
                //    return Redirect(res.Pageviews.PvAccessDenied);
                //}
            }
        }
        #endregion

        #region Post: Payment
        [HttpPost, ValidateInput(false)]
        public ActionResult CompanyPayment(FormCollection form)
        {
            List<b2bCompanyPayment> Payments;
            var BankID = "";
            SelectList_PageSize();
            SetPager(form);
            if (form["SearchType"] == "BankName")
            {
                var searchBank = svAddress.SelectData<emBank>("*", "BankName LIKE N'%" + form["SearchPayment"] + "%'");
                foreach (var it in (List<emBank>)searchBank)
                {
                    BankID = BankID + "BankID = " + it.BankID.ToString() + " or ";
                }
                if (BankID != "")
                {
                    BankID = BankID.Substring(0, BankID.Length - 4);
                    Payments = svCompany.SelectData<b2bCompanyPayment>("*", "IsDelete = 0 and (" + BankID + ") and CompID = " + LogonCompID, "ListNo ASC", (int)ViewBag.PageIndex, (int)ViewBag.PageSize); //ลบ null ออกแล้วเอา ListNo ไปใส่แทน
                }
                else
                {
                    Payments = svCompany.SelectData<b2bCompanyPayment>("*", "IsDelete = 0 and BankID = 0 and CompID = " + LogonCompID, "ListNo ASC", (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
                }
            }
            else if (!string.IsNullOrEmpty(form["SearchType"]))
            {
                Payments = svCompany.SelectData<b2bCompanyPayment>("*", "IsDelete = 0 and " + form["SearchType"] + " LIKE N'%" + form["SearchPayment"] + "%' and CompID = " + LogonCompID, "ListNo ASC", (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
            }
            else
            {
                Payments = svCompany.SelectData<b2bCompanyPayment>("*", "IsDelete = 0 and CompID = " + LogonCompID, "ListNo ASC", (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
            }
            var Banks = svAddress.SelectData<emBank>("*", "IsDelete = 0");
            var listpayment = svCompany.SelectData<b2bCompanyPayment>("*", "IsDelete = 0 and AccName LIKE N'%" + form["AccName"] + "%' and CompID = " + LogonCompID, "ListNo ASC", (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
            ViewBag.listpayment = listpayment;
            ViewBag.Banks = Banks.ToList();
            ViewBag.Payments = Payments;
            ViewBag.TotalPage = svCompany.TotalPage;
            ViewBag.TotalRow = svCompany.TotalRow;
            return PartialView("MyB2B/Company/Grid/PaymentGrid");
        }
        #endregion

        #region SaveCompanyPayment
        [HttpPost, ValidateInput(false)]
        public bool SaveCompanyPayment(FormCollection form)
        {
            int objState = DataManager.ConvertToInteger(form["objState"]);//objState 1 คือ insert objState 2 คือ update
            var CompPayments = new b2bCompanyPayment();
            if (objState == 2)// update
            {
                CompPayments = svCompany.SelectData<b2bCompanyPayment>("*", " CompPaymentID = " + form["CompPaymentID"] + " AND RowVersion = " + form["RowVersion"]).First();
            }

            #region set ค่า b2bCompanyPayment
            var ListNo = svCompany.SelectData<b2bCompanyPayment>("*", "IsDelete = 0 and CompID = " + LogonCompID, "ListNo ASC"); //ประกาศตัวแปรมารับค่า
            int a = ListNo.Count() + 1; // กรณีที่ ListNo มีค่าตัวแปรไม่เหมือนกัน
            CompPayments.CompID = LogonCompID;
            CompPayments.AccName = form["AccName"];
            CompPayments.AccNo = form["AccNo"];
            CompPayments.BankID = DataManager.ConvertToInteger(form["BankID"]);
            CompPayments.BranchName = form["BranchName"];
            CompPayments.AccType = DataManager.ConvertToByte(form["AccType"]);
            CompPayments.Remark = form["Remark"];
            if (objState == 2)// update
            {

                CompPayments.RowVersion = DataManager.ConvertToShort(CompPayments.RowVersion + 1);
            }
            else
            {
                CompPayments.RowFlag = 1;
                CompPayments.RowVersion = 1;
                CompPayments.CreatedBy = "sa";
                CompPayments.ModifiedBy = "sa";
                CompPayments.ModifiedDate = DateTime.Now;
                CompPayments.CreatedDate = DateTime.Now;
                CompPayments.ListNo = DataManager.ConvertToByte(a); // อ้างมาจากตัวแปรด้านบน
            }
            #endregion

            #region Save b2bCompanyPayment
            CompPayments = svCompany.SaveData<b2bCompanyPayment>(CompPayments, "CompPaymentID");
            #endregion

            return svCompany.IsResult;
        }
        #endregion

        #region EditPayment
        [HttpPost, ValidateInput(false)]
        public ActionResult EditPayment(FormCollection form)
        {
            var CompPayments = new b2bCompanyPayment();
            if (!string.IsNullOrEmpty(form["CompPaymentID"]))
            {
                CompPayments = svCompany.SelectData<b2bCompanyPayment>("*", " IsDelete = 0 AND CompPaymentID =" + form["CompPaymentID"]).First();
            }
            return Json(new { CompPaymentID = CompPayments.CompPaymentID, AccName = CompPayments.AccName, CompID = CompPayments.CompID, AccNo = CompPayments.AccNo, BankID = CompPayments.BankID, AccType = CompPayments.AccType, BranchName = CompPayments.BranchName, Remark = CompPayments.Remark, RowVersion = CompPayments.RowVersion });
        }
        #endregion

        #region Listpayment

        [HttpPost]
        public ActionResult ChangeListNoPayment(List<int> id)
        {
            if (!CheckIsLogin())
                return Redirect(res.Pageviews.PvMemberSignIn);
            var svPayment = new CompanyService();
            var payment = new b2bCompanyPayment();
            try
            {
                List<int> list = new List<int>();
                var id_banner1 = id[1];
                var id_banner2 = id[0];
                payment = svPayment.SelectData<b2bCompanyPayment>("*", " CompPaymentID = " + id_banner1).First();
                list.Add(Convert.ToInt32(payment.ListNo));
                payment = svPayment.SelectData<b2bCompanyPayment>("*", " CompPaymentID = " + id_banner2).First();
                list.Add(Convert.ToInt32(payment.ListNo));

                svPayment.UpdatePaymentListNo(id, list);
            }
            catch (Exception ex)
            {
                CreateLogFiles(ex);
            }

            return Json(new { IsResult = svPayment.IsResult, MsgError = GenerateMsgError(svPayment.MsgError), ID = id });
        }

        #endregion

        #endregion

        #region Company Shipment

        #region Get : CompanyShipment
        public ActionResult CompanyShipment()
        {
            RememberURL();
            if (!CheckIsLogin())
            {
                return Redirect(res.Pageviews.PvMemberSignIn);
            }
            else
            {
                ViewBag.CateLevel1 = LogonCompLevel;
                ViewBag.MemberType = LogonMemberType;
                var Shipments = svCompany.SelectData<b2bCompanyShipment>("*", "IsDelete = 0 and CompID = " + LogonCompID, "ListNo ASC");
                ViewBag.CompLevel = LogonCompLevel;
                ViewBag.PageType = "Company";
                ViewBag.MenuName = "Shipment";
                GetStatusUser();
                //if (LogonCompLevel == 3)
                //{
                    SetPager();
                    return View();
                //}
                //else
                //{
                //    return Redirect(res.Pageviews.PvAccessDenied);
                //}
            }
        }
        #endregion

        #region Post : CompanyShipment
        [HttpPost]
        public ActionResult CompanyShipment(FormCollection form)
        {
            SelectList_PageSize();
            SetPager(form);
            var CompShipments = svCompany.SelectData<b2bCompanyShipment>("*", "IsDelete = 0 and ShipmentName LIKE N'%" + form["ShipmentName"] + "%' and CompID = " + LogonCompID, "ListNo ASC", (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
            ViewBag.CompShipments = CompShipments;
            ViewBag.TotalPage = svCompany.TotalPage;
            ViewBag.TotalRow = svCompany.TotalRow;
            return PartialView("MyB2B/Company/Grid/ShipmentGrid");
        }
        #endregion

        #region SaveCompanyShipment
        [HttpPost, ValidateInput(false)]
        public bool SaveCompanyShipment(FormCollection form)
        {
            int objState = DataManager.ConvertToInteger(form["objState"]);//objState 1 คือ insert objState 2 คือ update
            var CompShipments = new b2bCompanyShipment();
            if (objState == 2)// update
            {
                CompShipments = svCompany.SelectData<b2bCompanyShipment>("*", " CompShipmentID = " + form["CompShipmentID"] + " AND RowVersion = " + form["RowVersion"]).First();
            }

            #region set ค่า b2bCompanyShipment
            var ListNo = svCompany.SelectData<b2bCompanyShipment>("*", "IsDelete = 0 and CompID = " + LogonCompID, "ListNo ASC");
            int shipment = ListNo.Count() + 1;
            CompShipments.CompID = LogonCompID;
            CompShipments.ShipmentName = form["ShipmentName"];
            CompShipments.PackingName = form["PackingName"];
            CompShipments.ShipmentDuration = form["ShipmentDuration"];
            CompShipments.Remark = form["Remark"];
            if (objState == 2)// update
            {
                CompShipments.RowVersion = DataManager.ConvertToShort(CompShipments.RowVersion + 1);
            }
            else
            {
                CompShipments.RowFlag = 1;
                CompShipments.RowVersion = 1;
                CompShipments.CreatedBy = "sa";
                CompShipments.ModifiedBy = "sa";
                CompShipments.ModifiedDate = DateTime.Now;
                CompShipments.CreatedDate = DateTime.Now;
                CompShipments.ListNo = DataManager.ConvertToByte(shipment);
            }
            #endregion

            #region Save b2bCompanyShipment
            CompShipments = svCompany.SaveData<b2bCompanyShipment>(CompShipments, "CompShipmentID");
            #endregion

            return svCompany.IsResult;
        }
        #endregion

        #region EditShipment
        [HttpPost, ValidateInput(false)]
        public ActionResult EditShipment(FormCollection form)
        {
            var CompShipments = new b2bCompanyShipment();
            if (!string.IsNullOrEmpty(form["CompShipmentID"]))
            {
                CompShipments = svCompany.SelectData<b2bCompanyShipment>("CompShipmentID,CompID,ShipmentName,PackingName,Remark,ShipmentDuration,RowVersion", " IsDelete = 0 AND CompShipmentID =" + form["CompShipmentID"]).First();
            }
            return Json(new { CompShipmentID = CompShipments.CompShipmentID, CompID = CompShipments.CompID, ShipmentName = CompShipments.ShipmentName, PackingName = CompShipments.PackingName, ShipmentDuration = CompShipments.ShipmentDuration, Remark = CompShipments.Remark, RowVersion = CompShipments.RowVersion });
        }
        #endregion

        #region Listshipment

        [HttpPost]
        public ActionResult ChangeListNoShipment(List<int> id)
        {
            if (!CheckIsLogin())
                return Redirect(res.Pageviews.PvMemberSignIn);
            var svShipment = new CompanyService();
            var shipment = new b2bCompanyShipment();
            try
            {
                List<int> list = new List<int>();
                var id_banner1 = id[1];
                var id_banner2 = id[0];
                shipment = svShipment.SelectData<b2bCompanyShipment>("*", " CompShipmentID = " + id_banner1).First();
                list.Add(Convert.ToInt32(shipment.ListNo));
                shipment = svShipment.SelectData<b2bCompanyShipment>("*", " CompShipmentID = " + id_banner2).First();
                list.Add(Convert.ToInt32(shipment.ListNo));

                svShipment.UpdateShipmentListNo(id, list);
            }
            catch (Exception ex)
            {
                CreateLogFiles(ex);
            }

            return Json(new { IsResult = svShipment.IsResult, MsgError = GenerateMsgError(svShipment.MsgError), ID = id });
        }

        #endregion

        #endregion

        #region Company Partner
        public ActionResult CompanyPartner()
        {
            RememberURL();
            return View();
        }
        [HttpPost]
        public ActionResult CompanyPartner(FormCollection form)
        {
            return View();
        }
        #endregion

        #region Company Production
        public ActionResult CompanyProduction()
        {
            RememberURL();
            if (!CheckIsLogin())
            {
                return Redirect(res.Pageviews.PvMemberSignIn);
            }
            else
            {
                GetStatusUser();
                var Companies = SelectCompany();
                var Provinces = svAddress.ListProvince().ToList();
                var Districts = svAddress.SelectData<emDistrict>("DistrictID,DistrictName", "IsDelete = 0 AND ProvinceID = " + Companies.FactoryProvinceID);

                foreach (var it in (List<emDistrict>)Districts)
                {
                    if (Companies.FactoryDistrictID == it.DistrictID)
                    {
                        ViewBag.FactoryDistrictName = it.DistrictName;
                    }
                }
                foreach (var it in (List<emProvince>)Provinces)
                {
                    if (Companies.FactoryProvinceID == it.ProvinceID)
                    {
                        ViewBag.FactoryProvinceName = it.ProvinceName;
                    }
                }
                ViewBag.CompLevel = LogonCompLevel;
                ViewBag.Companies = Companies;
                ViewBag.Provinces = Provinces;
                ViewBag.Districts = Districts;
                ViewBag.PageType = "Company";
                ViewBag.MenuName = "Production";
                return View();
            }
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult CompanyProduction(FormCollection form)
        {
            Hashtable data = new Hashtable();
            var b2bCompanies = new b2bCompany();
            var emCompanies = new emCompany();
            try
            {
                var Company = svCompany.SelectData<b2bCompany>("*", " CompID = " + LogonCompID + "");

                if (Company.Count < 1)
                {
                    data.Add("result", false);
                    // data.Add("RowVersion", form["RowVersion"]);
                    return Json(data);
                }
                #region set ค่า b2bCompany
                b2bCompanies = Company.First();
                b2bCompanies.IsFactSameAddr = DataManager.ConvertToBool(form["IsFactSameAddr"]);
                b2bCompanies.FactoryAddrLine1 = form["FactoryAddrLine1"];
                b2bCompanies.FactoryDistrictID = Convert.ToInt32(form["FactoryDistrictID"]);
                b2bCompanies.FactoryProvinceID = Convert.ToInt32(form["FactoryProvinceID"]);
                b2bCompanies.FactoryPostalCode = form["FactoryPostalCode"];
                b2bCompanies.FactoryPhone = form["FactoryPhone"];
                b2bCompanies.FactoryMobile = form["FactoryMobile"];
                b2bCompanies.FactoryFax = form["FactoryFax"];
                b2bCompanies.FactorySize = form["FactorySize"];
                b2bCompanies.RESEmployeeCount = DataManager.ConvertToByte(form["RESEmployeeCount"]);
                b2bCompanies.QCEmployeeCount = DataManager.ConvertToByte(form["QCEmployeeCount"]);
                b2bCompanies.FactoryRemark = ReplaceText(form["FactoryRemark"]);
                //b2bCompanies.RowVersion = DataManager.ConvertToShort(form["RowVersion"]);
                #endregion

                #region Update b2bCompany
                b2bCompanies = svCompany.SaveData<b2bCompany>(b2bCompanies, "CompID");
                #endregion

                if (svCompany.IsResult)
                {
                    var RowVersion = b2bCompanies.RowVersion + 1;
                    data.Add("result", true);
                    //data.Add("RowVersion", RowVersion);

                }
                else
                {
                    //data.Add("RowVersion", b2bCompanies.RowVersion);
                }

                return Json(data);
            }
            catch (Exception ex)
            {
                CreateLogFiles(ex);
                return Json(data);
            }
        }
        #endregion

        #region WebsiteTemplate

        #region Get : WebsiteTemplate
        public ActionResult WebsiteTemplate()
        {
            RememberURL();
            if (!CheckIsLogin())
            {
                return Redirect(res.Pageviews.PvMemberSignIn);
            }
            else
            {
                ViewBag.CompLevel = LogonCompLevel;
                GetStatusUser();
                ViewBag.Company = SelectCompany();
                ViewBag.PageType = "Company";
                ViewBag.MenuName = "Website";
                return View();
            }
        }
        #endregion

        #region Post : WebsiteTemplate
        [HttpPost, ValidateInput(false)]
        public ActionResult WebsiteTemplate(FormCollection form)
        {
            Hashtable data = new Hashtable();
            var b2bCompanies = new b2bCompany();
            try
            {
                var Company = svCompany.SelectData<b2bCompany>("*", " CompID = " + LogonCompID + "");
                b2bCompanies = Company.First();
                if (Company.Count < 1)
                {
                    data.Add("result", false);
                    //data.Add("RowVersion", form["RowVersion"]);
                    return Json(data);
                }

                #region set ค่า b2bCompany
                b2bCompanies.CompWebsiteTemplate = DataManager.ConvertToByte(form["CompWebsiteTemplate"]);
                b2bCompanies.CompWebsiteCss = DataManager.ConvertToByte(form["CompWebsiteCss"]);
                //b2bCompanies.RowVersion = DataManager.ConvertToShort(form["RowVersion"]);
                #endregion

                #region Update b2bCompany
                if (b2bCompanies.CompLevel == 1 && b2bCompanies.CompWebsiteTemplate < 5)
                {
                    b2bCompanies = svCompany.SaveData<b2bCompany>(b2bCompanies, "CompID");
                }
                else if (b2bCompanies.CompLevel == 3 || b2bCompanies.CompLevel == 2)
                {
                    b2bCompanies = svCompany.SaveData<b2bCompany>(b2bCompanies, "CompID");
                }
                #endregion

                if (svCompany.IsResult)
                {
                    data.Add("result", true);
                    var RowVersion = b2bCompanies.RowVersion + 1;
                    //data.Add("RowVersion", RowVersion);
                }
                else
                {
                    data.Add("result", false);
                    //data.Add("RowVersion", b2bCompanies.RowVersion);
                }

                return Json(data);
            }
            catch (Exception ex)
            {
                CreateLogFiles(ex);
                return Json(data);
            }
        }
        #endregion

        #region PreviewTemplate
        [HttpGet]
        public ActionResult PreviewTemplate(int template = -1, int css = -1)
        {
            if (template >= 0)
            {
                GetStatusUser();
                ViewBag.PageIndex = 1;
                ViewBag.PageSize = 20;
                string page = "PreviewTemplate";
                var Company = SelectCompany();
                svProduct = new ProductService();
                ViewBag.PageType = "Company";
                ViewBag.Css = css;

                if (css == 0)
                {
                    TemplateWebsite((int)Company.CompID, page, template);
                    string sqlSelect = "CompID,CompName,ProductID,ProductName,ProductImgPath,RowFlag,ListNo";

                    #region DoWhereCause
                    string sqlWhere = svProduct.CreateWhereAction(ProductAction.Recommend, (int)Company.CompID);
                    #endregion

                    var countproducts = svProduct.CountData<view_Product>(sqlSelect, sqlWhere);
                    if (countproducts != 0)
                    {
                        var Products = svProduct.SelectData<view_Product>(sqlSelect, sqlWhere, " ListNo ASC ", ViewBag.PageIndex, ViewBag.PageSize);
                        ViewBag.Products = Products;
                        ViewBag.CountProducts = countproducts;
                    }
                    else
                    {
                        sqlWhere = svProduct.CreateWhereAction(ProductAction.FrontEnd, (int)Company.CompID);
                        countproducts = svProduct.CountData<view_Product>(sqlSelect, sqlWhere);
                        var Products = svProduct.SelectData<view_Product>(sqlSelect, sqlWhere, "ModifiedDate DESC", ViewBag.PageIndex, ViewBag.PageSize);
                        ViewBag.Products = Products;
                        ViewBag.CountProducts = countproducts;
                    }


                    sqlSelect = "CompID,CompName,CompCode,CompLevel,LogoImgPath,CompAddrLine1,CompPostalCode,CompPhone,CompImgPath,CompShortDes,DistrictName,ProvinceName,CreatedDate";
                    sqlWhere = "CompID =" + Company.CompID;
                    var company = svCompany.SelectData<view_Company>(sqlSelect, sqlWhere).First();
                    ViewBag.Company = company;
                }
                else if (css == 1)
                {
                    if (RedirectToProduction())
                        return Redirect(UrlProduction);

                    RememberURL();
                    int id = Company.CompID;
                    int compid = DataManager.ConvertToInteger(id);

                    if (id > 0)
                    {
                        int countcompany = DefaultWebsite(compid, page);

                        if (countcompany > 0)
                        {
                            SetPager();
                            SelectList_PageSize();
                            ViewBag.TotalRow = 0;
                            ViewBag.GroupID = 0;
                            ViewBag.CateID = 0;
                            ViewBag.CateLevel = 0;
                            ViewBag.CompID = id;

                            if (LogonCompID == ViewBag.CompID)
                            {
                                List_DoloadData(ProductAction.WebSite);
                            }
                            else
                            {
                                List_DoloadData(ProductAction.FrontEnd);
                            }

                            int CompID = compid;

                            #region Select Recommand Product
                            string sqlSelect = "CompID,CompName,ProductID,ProductName,ProductImgPath,RowFlag,ListNo,ViewCount,Price,Qty";

                            #region DoWhereCause
                            string sqlWhere = svProduct.CreateWhereAction(ProductAction.Recommend, CompID);
                            #endregion

                            var countproducts = svProduct.CountData<view_SearchProduct>(sqlSelect, sqlWhere);
                            if (countproducts != 0)
                            {
                                var Products = svProduct.SelectData<view_SearchProduct>(sqlSelect, sqlWhere, " ListNo ASC ", (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
                                ViewBag.Products = Products;
                                ViewBag.CountProducts = svProduct.TotalRow;
                            }
                            else
                            {
                                sqlWhere = svProduct.CreateWhereAction(ProductAction.FrontEnd, CompID);
                                countproducts = svProduct.CountData<view_SearchProduct>(sqlSelect, sqlWhere);
                                var Products = svProduct.SelectData<view_SearchProduct>(sqlSelect, sqlWhere, "ModifiedDate DESC", (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
                                ViewBag.Products = Products;
                                ViewBag.CountProducts = countproducts;
                            }
                            #endregion

                            #region Select Company Info
                            sqlSelect = "CompID,ServiceType,CompName,CompCode,CompLevel,LogoImgPath,CompAddrLine1,CompPostalCode,CompPhone,CompImgPath,CompShortDes,CompSubDistrict,CompDistrictName,CompProvinceName,ContactEmail,BizTypeOther,BizTypeName,CreatedDate";
                            sqlWhere = "CompID =" + id;
                            var company = svCompany.SelectData<view_Company>(sqlSelect, sqlWhere).First();
                            ViewBag.Company = company;
                            ViewBag.CompID = id;
                            return View();
                            #endregion
                        }
                        else
                        {
                            return Redirect(res.Pageviews.PvNotFound);
                        }
                    }
                    else
                    {
                        return Redirect(res.Pageviews.PvErrorPages);
                    }
                }
                return View();
            }else{
                return Redirect("~/MyB2B/Company/WebsiteTemplate");
            }
        }
        #endregion

        #region DefaultWebsite
        public int DefaultWebsite(int id, string page)
        {
            svCompany = new CompanyService();
            string sqlWhere = string.Empty;
            string sqlSelect = "CompID,CompName,CompCode,CompLevel,emCompID,LogoImgPath,CompWebsiteTemplate,ViewCount,ProductCount,CompHistory,IsOnline,CompAddrLine1,CompPhone,ProvinceName";
            if (LogonRowFlag == 3)
            {
                sqlWhere = svCompany.CreateWhereAction(CompStatus.All, id);
            }
            else
            {
                sqlWhere = svCompany.CreateWhereAction(CompStatus.Online, id);
            }
            //var sqlwhereinweb = "";
            //switch (res.Common.lblWebsite)
            //{
            //    case "B2BThai": sqlwhereinweb = " AND WebID = 1 "; break;
            //    case "AntCart": sqlwhereinweb = " AND WebID = 3 "; break;
            //    case "myOtopThai": sqlwhereinweb = " AND WebID = 5 "; break;
            //    case "AppstoreThai": sqlwhereinweb = " AND WebID = 6 "; break;
            //    default: sqlwhereinweb = ""; break;
            //}
            //sqlWhere += sqlwhereinweb;

            var countcompany = svCompany.CountData<view_Company>(sqlSelect, sqlWhere);


            if (countcompany > 0)
            {
                AddViewCount(id, "Supplier");
                var company = svCompany.SelectData<view_Company>(sqlSelect, sqlWhere).First();
                ViewBag.WebCompID = company.CompID;
                ViewBag.WebCompName = company.CompName;
                ViewBag.CompCode = company.CompCode;
                ViewBag.WebCompLevel = company.CompLevel;
                ViewBag.WebLogoImgPath = company.LogoImgPath;
                ViewBag.ContactPhone = company.ContactPhone;
                ViewBag.Template = company.CompWebsiteTemplate;
                ViewBag.WebEmCompID = company.emCompID;
                ViewBag.PageAction = page;
                //if (company.ProvinceName == "กรุงเทพมหานคร")
                //{
                //    company.ProvinceName = "กรุงเทพ";
                //}
                ViewBag.title = company.CompName + " | " + company.ProvinceName + " | " + res.Common.lblDomainShortName;
                ViewBag.ViewCount = company.ViewCount;
                ViewBag.WebCompHistory = company.CompHistory;
                ViewBag.IsOnline = company.IsOnline;
                ViewBag.MetaDescription = company.CompName + " | " + company.CompAddrLine1 + " | " + company.ProvinceName + " | " + company.CompPhone;
                ViewBag.MetaKeyword = ViewBag.title;
                //ViewBag.ProductCount = company.ProductCount;
                ProductCount(ViewBag.WebCompID);
                BlogCount(ViewBag.WebCompID);
                if (ViewBag.WebCompLevel == 3)
                {
                    OrderCount(ViewBag.WebCompID);
                    CertifyCount(ViewBag.WebCompID);
                    JobCount(ViewBag.WebEmCompID);
                }

                SetStatusWebsite((int)company.CompID, company.CompName);

                //var SetMenu = svCompany.SelectData<b2bCompanyMenu>("*", "IsDelete = 0 ", "ListNo ASC");
                var SettingMenu = svCompany.SelectData<b2bCompanyMenu>("*", "IsDelete = 0  and CompID = " + id, "ListNo ASC");
                if (SettingMenu.Count == 0)
                {
                    SettingMenu = svCompany.SelectData<b2bCompanyMenu>("*", "IsDelete = 0 and IsDefaultMenu = 1", "ListNo ASC");
                }
                else
                {
                    var where = "IsDelete = 0 and ([IsDefaultMenu] = 1 ";
                    foreach (var list in SettingMenu)
                    {
                        where += " or (FromMenuID = " + list.FromMenuID + " AND CompID = " + id + " AND IsShow = 1)";
                    }
                    where += ")";
                    foreach (var list in SettingMenu)
                    {
                        where += "AND CompMenuID != " + list.FromMenuID;
                    }
                    SettingMenu = svCompany.SelectData<b2bCompanyMenu>("*", where, "ListNo ASC");
                }

                ViewBag.SettingMenu = SettingMenu;

            }
            return countcompany;
        }
        #endregion

        #region List_DoloadData
        public void List_DoloadData(ProductAction action)
        {
            svProduct = new ProductService();
            //var sqlwherein = "";
            //switch (res.Common.lblWebsite)
            //{
            //    case "B2BThai": sqlwherein = " AND CategoryType IN (1,2)"; break;
            //    case "AntCart": sqlwherein = " AND CategoryType IN (3)"; break;
            //    case "myOtopThai": sqlwherein = " AND CategoryType IN (5)"; break;
            //    case "AppstoreThai": sqlwherein = " AND CategoryType IN (6)"; break;
            //    default: sqlwherein = ""; break;
            //}
            string sqlSelect = "CompID,CompName,ProductID,ProductName,Price,Qty,ProductImgPath,ViewCount";
            string sqlWhere = svProduct.CreateWhereAction(ProductAction.FrontEnd, (int)ViewBag.CompID);

            string sqlOrderBy = " ModifiedDate DESC ";

            #region DoWhereCause
            sqlWhere += svProduct.CreateWhereCause((int)ViewBag.CompID, ViewBag.TextSearch, 0, (int)ViewBag.GroupID,
                (int)ViewBag.CateLevel, (int)ViewBag.CateID);

            #endregion

            var products = svProduct.SelectData<view_SearchProduct>(sqlSelect, sqlWhere, sqlOrderBy, (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
            if (products != null)
            {
                if (products.Count() > 0)
                {
                    ViewBag.Products = products;
                    ViewBag.TotalRow = svProduct.TotalRow;
                    ViewBag.TotalPage = svProduct.TotalPage;
                }
                else
                {
                    ViewBag.Products = null;
                    ViewBag.Message = "ไม่มีสินค้า";
                }
            }
            else
            {
                ViewBag.Products = null;
                ViewBag.Message = res.Message_Center.lblNoresult;
            }
            int size = 1;
            if (products.Count() <= 6)
            {
                size = 2;
            }
            else if (products.Count() <= 9)
            {
                size = 3;
            }
            else if (products.Count() <= 12)
            {
                size = 4;
            }
            else if (products.Count() <= 15)
            {
                size = 5;
            }
            else
            {
                size = 7;
            }
            var slideproduct = svProduct.SelectData<view_SearchProduct>("CompID,CompName,ProductID,ProductName,ProductImgPath", sqlWhere, "ProductName,ProductImgPath", 1, size, true);
            if (slideproduct != null)
            {
                if (slideproduct.Count() > 0)
                {
                    ViewBag.SlideProduct = slideproduct;
                }
                else
                {
                    ViewBag.SlideProduct = null;
                    ViewBag.Message = "ไม่มีสินค้า";
                }
            }
            else
            {
                ViewBag.SlideProduct = null;
                ViewBag.Message = res.Message_Center.lblNoresult;
            }
        }
        #endregion

        #region DefaultWebsite
        public void TemplateWebsite(int id, string page,int template)
        {
            string sqlSelect = "CompID,CompName,CompCode,CompLevel,emCompID,LogoImgPath,CompWebsiteTemplate,ViewCount,ProductCount,CompHistory";
            string sqlWhere = "CompID = " + id;
            var countcompany = svCompany.CountData<view_Company>(sqlSelect, sqlWhere);
            if (countcompany > 0)
            {
                AddViewCount(id, "Supplier");
                var company = svCompany.SelectData<view_Company>(sqlSelect, sqlWhere).First();
                ViewBag.WebCompID = company.CompID;
                ViewBag.WebCompName = company.CompName;
                ViewBag.CompCode = company.CompCode;
                ViewBag.WebCompLevel = company.CompLevel;
                ViewBag.WebLogoImgPath = company.LogoImgPath;
                ViewBag.Template = template;
                ViewBag.WebEmCompID = company.emCompID;
                ViewBag.PageAction = page;
                ViewBag.title = page + " - " + company.CompName + res.Common.lblDomainShortName;
                ViewBag.ViewCount = company.ViewCount;
                ViewBag.WebCompHistory = company.CompHistory;
                //ViewBag.ProductCount = company.ProductCount;

                ProductCount(ViewBag.WebCompID);
                BlogCount(ViewBag.WebCompID);
                if (ViewBag.WebCompLevel == 3)
                {
                    OrderCount(ViewBag.WebCompID);
                    CertifyCount(ViewBag.WebCompID);
                    JobCount(ViewBag.WebEmCompID);
                }

                Response.Cookies["WebsiteCompID"].Value = id.ToString();
                Response.Cookies["WebsiteCompID"].Expires = DateTime.Now.AddHours(1);
                Response.Cookies["WebsiteCompName"].Value = ViewBag.WebCompName;
                Response.Cookies["WebsiteCompName"].Expires = DateTime.Now.AddHours(1);
            }
        }
        #endregion

        #region JobCount
        public void JobCount(int? emCompID)
        {
            JobService svJob = new JobService();
            string sqlSelect = "JobID, JobName";
            string sqlWhere = "CompID =" + emCompID + " AND IsDelete = 0";
            var emJobs = svJob.CountData<view_emJob>(sqlSelect, sqlWhere);
            ViewBag.CountJob = emJobs;

        }
        #endregion

        #region BlogCount
        public void BlogCount(int? emCompID)
        {
            var svArticle = new emArticleService();
            string sqlSelect = "ArticleID, ArticleName";
            string sqlWhere = "CompID =" + emCompID + " AND IsDelete = 0";
            var emArticles = svArticle.CountData<view_emArticle>(sqlSelect, sqlWhere);
            ViewBag.CountArticle = emArticles;

        }
        #endregion

        #endregion

        #region Company Setting

        #region Get: Setting

        public ActionResult CompanySetting()
        {
            RememberURL();
            if (!CheckIsLogin())
            {
                return Redirect(res.Pageviews.PvMemberSignIn);
            }
            else
            {
                ViewBag.CompLevel = LogonCompLevel;
                ViewBag.CateLevel1 = LogonCompLevel;
                GetStatusUser();
                SetPager();
                if (LogonCompLevel == 3)
                {
                    ViewBag.PageType = "Company";
                    ViewBag.MenuName = "CompanySetting";
                    return View();
                }
                else
                {
                    return Redirect(res.Pageviews.PvMemberSignIn);
                }


            }

        }

        #endregion

        #region Post: Setting
        [HttpPost, ValidateInput(false)]
        public ActionResult CompanySetting(FormCollection form)
        {
            SelectList_PageSize();
            SetPager(form);
            var SetMenu = svCompany.SelectData<b2bCompanyMenu>("*", "IsDelete = 0 and CompID = " + LogonCompID, "ListNo ASC", (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
            var SettingMenu = svCompany.SelectData<b2bCompanyMenu>("*", "IsDelete = 0 and MenuName LIKE N'%" + form["MenuName"] + "%' and CompID = " + LogonCompID, "ListNo ASC" + form["LinkUrl"], (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
            if (SettingMenu.Count == 0)
            {
                SettingMenu = svCompany.SelectData<b2bCompanyMenu>("*", "IsDelete = 0 and IsDefaultMenu = 1", "ListNo ASC", (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
            }
            else
            {
                var where = "IsDelete = 0 and (IsDefaultMenu = 1 ";
                foreach (var list in SettingMenu)
                {
                    where += " or (FromMenuID = " + list.FromMenuID + " AND CompID = " + LogonCompID + ")";
                }
                where += ")";
                foreach (var list in SettingMenu)
                {
                    where += "AND CompMenuID != " + list.FromMenuID;
                }
                SettingMenu = svCompany.SelectData<b2bCompanyMenu>("*", where, "ListNo ASC", (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
            }
            string sqlWhere = string.Empty;
            string sqlSelect = "CompID,CompName,CompCode,CompLevel,emCompID,LogoImgPath,CompWebsiteTemplate,ViewCount,ProductCount,CompHistory,IsOnline,CompAddrLine1,CompPhone,ProvinceName";
            if (LogonRowFlag == 3)
            {
                sqlWhere = svCompany.CreateWhereAction(CompStatus.All, LogonCompID);
            }
            else
            {
                sqlWhere = svCompany.CreateWhereAction(CompStatus.Online, LogonCompID);
            }
            var company = svCompany.SelectData<view_Company>(sqlSelect, sqlWhere).First();
            ViewBag.WebCompID = company.CompID;
            ViewBag.WebCompName = company.CompName;
            ViewBag.SettingMenu = SettingMenu;
            ViewBag.TotalPage = svCompany.TotalPage;
            ViewBag.TotalRow = svCompany.TotalRow;
            return PartialView("MyB2B/Company/Grid/SettingGrid");
        }

        #endregion

        #region EditSetting
        [HttpPost, ValidateInput(false)]
        public ActionResult EditSetting(FormCollection form)
        {
            var CompMenu = new b2bCompanyMenu();
            if (!string.IsNullOrEmpty(form["CompMenuID"]))
            {
                var a = form["CompMenuID"];
                CompMenu = svCompany.SelectData<b2bCompanyMenu>("CompMenuID,CompID,MenuName,Remark,RowVersion,FromMenuID,ListNo,LinkUrl", " IsDelete = 0 AND CompMenuID =" + form["CompMenuID"]).First();
            }
            return Json(new { CompMenuID = CompMenu.CompMenuID, FromMenuID = CompMenu.FromMenuID, CompID = CompMenu.CompID, MenuName = CompMenu.MenuName, Remark = CompMenu.Remark, RowVersion = CompMenu.RowVersion, ListNo = CompMenu.ListNo, LinkUrl = CompMenu.LinkUrl });
        }
        #endregion

        #region SaveSetting

        [HttpPost, ValidateInput(false)]
        public bool SaveSettingMenu(FormCollection form)
        {
            //int objState = DataManager.ConvertToInteger(form["objState"]);
            var CompSetting = new b2bCompanyMenu();
            var count = svCompany.SelectData<b2bCompanyMenu>("*", " CreatedDate = GetDate() AND RowFlag > 0");
            int Count = count.Count + 1;
            string Code = AutoGenCode("CM", Count);
            //if (objState == 2)
            //{
            //    CompSetting = svCompany.SelectData<b2bCompanyMenu>("*", "CompMenuID = " + form["CompMenuID"] + "AND RowVersion = " + form["Rowversion"]).First();
            //}

            //#region set ค่า b2bCompanyMenu
            #region Save b2bCompanyMenu
            var b = form["FromMenuID"];
            var ListNo = svCompany.SelectData<b2bCompanyMenu>("*", "IsDelete = 0 and CompID = " + LogonCompID, "ListNo ASC");
            var CompID = svCompany.SelectData<b2bCompanyMenu>("*", "IsDelete = 0 and CompID = " + LogonCompID + "AND FromMenuID = " + DataManager.ConvertToInteger(form["FromMenuID"]), "ListNo ASC");

            if (CompID.Count == 0)
            {
                CompSetting.CompID = LogonCompID;
                //CompSetting.CompMenuID = DataManager.ConvertToInteger(form["CompMenuID"]);
                CompSetting.MenuName = form["MenuName"];
                CompSetting.CompMenuCode = Code; //
                CompSetting.MenuNameEng = "";
                CompSetting.FromMenuID = DataManager.ConvertToInteger(form["CompMenuID"]);
                CompSetting.MenuImgPath = "";
                CompSetting.LinkUrl = form["LinkUrl"];
                CompSetting.Remark = form["Remark"];
                CompSetting.CreatedDate = DateTime.Now;
                //CompSetting.ListNo = ListNo.Count() + 1;
                CompSetting.ListNo = DataManager.ConvertToInteger(form["ListNo"]);
                CompSetting.IsDefaultMenu = false;
                CompSetting.ParentMenuID = 0;
                CompSetting.IsShow = true;
                CompSetting.IsDelete = false;
                CompSetting.RowFlag = 1;
                CompSetting.RowVersion = 1;
                CompSetting.CreatedBy = "sa";
                CompSetting.ModifiedDate = DateTime.Now;
                CompSetting.ModifiedBy = "sa";

                CompSetting = svCompany.InsertCompanysetting(CompSetting);
            }
            else
            {
                var CompMenuID = svCompany.SelectData<b2bCompanyMenu>("*", "IsDelete = 0 and CompMenuID = " + DataManager.ConvertToInteger(form["CompMenuID"]), "ListNo ASC");
                var update = "Remark = N'" + form["Remark"] + "' , ModifiedDate = '" + DateTime.Now + "'";
                var where = "CompMenuID = " + CompMenuID.First().CompMenuID + "AND CompID = " + LogonCompID;
                var a = svCompany.UpdateByCondition<b2bCompanyMenu>(update, where);
            }

            return svCompany.IsResult;
        }
            #endregion

        #endregion

        #region Listsetting

        [HttpPost]
        public ActionResult ChangeListNoSetting(List<int> id)
        {
            if (!CheckIsLogin())
                return Redirect(res.Pageviews.PvMemberSignIn);
            var svSetting = new CompanyService();
            var setting = new b2bCompanyMenu();
            try
            {
                List<int> list = new List<int>();
                var id_banner1 = id[1]; // ตัวที่จะต้องสลับ 7
                var id_banner2 = id[0]; // ตัวที่กด 49


                var Menu1 = svCompany.SelectData<b2bCompanyMenu>("*", "IsDelete = 0 and CompMenuID = " + id_banner1); // หาข้อมูลตัวที่จะต้องสลับ 
                var Menu2 = svCompany.SelectData<b2bCompanyMenu>("*", "IsDelete = 0 and CompMenuID = " + id_banner2); // หาข้อมูลตัวที่กด 

                // เช็คว่าเคยเปลี่ยนชื่อเมนูเป็นของตนเองละยัง ถ้ายังให้ไป Insert ใหม่ แต่ถ้ามีแล้วให้ไป Update
                var SetMenu1 = svCompany.SelectData<b2bCompanyMenu>("*", "IsDelete = 0 and IsDefaultMenu = 0 and CompID = " + LogonCompID + " and CompMenuID = " + id_banner1);
                if (SetMenu1.Count > 0)
                {
                    var update = "ListNo = " + Menu2[0].ListNo + " , ModifiedDate = '" + DateTime.Now + "'";
                    var where = "CompMenuID = " + SetMenu1.First().CompMenuID + " AND CompID = " + LogonCompID;
                    var MenuUpdate = svCompany.UpdateByCondition<b2bCompanyMenu>(update, where);
                }
                else
                {
                    var CompSetting = new b2bCompanyMenu();
                    var count = svCompany.SelectData<b2bCompanyMenu>("*", " CreatedDate = GetDate() AND RowFlag > 0");
                    int Count = count.Count + 1;
                    string Code = AutoGenCode("CM", Count);

                    CompSetting.CompID = LogonCompID;
                    CompSetting.MenuName = Menu1[0].MenuName;
                    CompSetting.CompMenuCode = Code; //
                    CompSetting.MenuNameEng = "";
                    CompSetting.FromMenuID = Menu1[0].CompMenuID;
                    CompSetting.MenuImgPath = "";
                    CompSetting.LinkUrl = Menu1[0].LinkUrl;
                    CompSetting.Remark = Menu1[0].Remark;
                    CompSetting.CreatedDate = DateTime.Now;
                    CompSetting.ListNo = Menu2[0].ListNo;
                    CompSetting.IsDefaultMenu = false;
                    CompSetting.ParentMenuID = 0;
                    CompSetting.IsShow = true;
                    CompSetting.IsDelete = false;
                    CompSetting.RowFlag = 1;
                    CompSetting.RowVersion = 1;
                    CompSetting.CreatedBy = "sa";
                    CompSetting.ModifiedDate = DateTime.Now;
                    CompSetting.ModifiedBy = "sa";

                    CompSetting = svCompany.InsertCompanysetting(CompSetting);
                }

                var SetMenu2 = svCompany.SelectData<b2bCompanyMenu>("*", "IsDelete = 0 and IsDefaultMenu = 0 and CompID = " + LogonCompID + " and CompMenuID = " + id_banner2);
                if (SetMenu2.Count > 0)
                {
                    var update = "ListNo = " + Menu1[0].ListNo + " , ModifiedDate = '" + DateTime.Now + "'";
                    var where = "CompMenuID = " + SetMenu2.First().CompMenuID + " AND CompID = " + LogonCompID;
                    var MenuUpdate = svCompany.UpdateByCondition<b2bCompanyMenu>(update, where);
                }
                else
                {
                    var CompSetting = new b2bCompanyMenu();
                    var count = svCompany.SelectData<b2bCompanyMenu>("*", " CreatedDate = GetDate() AND RowFlag > 0");
                    int Count = count.Count + 1;
                    string Code = AutoGenCode("CM", Count);


                    CompSetting.CompID = LogonCompID;
                    CompSetting.MenuName = Menu2[0].MenuName;
                    CompSetting.CompMenuCode = Code; //
                    CompSetting.MenuNameEng = "";
                    CompSetting.FromMenuID = Menu2[0].CompMenuID;
                    CompSetting.MenuImgPath = "";
                    CompSetting.LinkUrl = Menu2[0].LinkUrl;
                    CompSetting.Remark = Menu2[0].Remark;
                    CompSetting.CreatedDate = DateTime.Now;
                    CompSetting.ListNo = Menu1[0].ListNo;
                    CompSetting.IsDefaultMenu = false;
                    CompSetting.ParentMenuID = 0;
                    CompSetting.IsShow = true;
                    CompSetting.IsDelete = false;
                    CompSetting.RowFlag = 1;
                    CompSetting.RowVersion = 1;
                    CompSetting.CreatedBy = "sa";
                    CompSetting.ModifiedDate = DateTime.Now;
                    CompSetting.ModifiedBy = "sa";

                    CompSetting = svCompany.InsertCompanysetting(CompSetting);
                }

                //setting = svSetting.SelectData<b2bCompanyMenu>("*", " CompMenuID = " + id_banner1).First();
                //list.Add(Convert.ToInt32(setting.ListNo));
                //setting = svSetting.SelectData<b2bCompanyMenu>("*", " CompMenuID = " + id_banner2).First();
                //list.Add(Convert.ToInt32(setting.ListNo));

                //svSetting.UpdateSettingListNo(id, list);
            }
            catch (Exception ex)
            {
                CreateLogFiles(ex);
            }

            return Json(new { IsResult = true, MsgError = GenerateMsgError(svSetting.MsgError), ID = id });
        }

        #endregion

        #region ChangeIsShow

        [HttpPost]
        public bool ChangeIsShow(int CompMenuID, int istrust)
        {
            CompanyService svSetting = new CompanyService();
            bool IsResult = false;
            try
            {
                IsResult = svSetting.UpdateByCondition<b2bCompanyMenu>("IsShow = " + istrust + "", "CompMenuID = " + CompMenuID);
            }
            catch (Exception ex)
            {
                CreateLogFiles(ex);
            }

            return IsResult;
        }

        #endregion

        #region Refresh
        [HttpPost]
        public ActionResult refreshSet()
        {
            var SetMenu = svCompany.SelectData<b2bCompanyMenu>("*", "IsDelete = 0 ", "ListNo ASC");
            if (SetMenu.Count != 0)
            {
                SetMenu = svCompany.SelectData<b2bCompanyMenu>("*", "IsDelete = 0 and IsDefaultMenu = 1", "ListNo ASC");

                var Delete = svCompany.qDB.ExecuteCommand("delete from b2bCompanyMenu where IsDefaultMenu = 0 and CompID = " + LogonCompID);

                ViewBag.SetMenu = SetMenu;
                return Json(new { IsSuccess = true, Result = "แก้ไขเมนูสำเร็จ" });
            }
            else
            {
                return Json(new { IsSuccess = false, Result = "การแก้ไขเมนูผิดพลาด กรุณาลองใหม่อีกครั้ง" });
            }

        }

        #endregion


        #endregion

        #region Job

        #region Get : Job
        public ActionResult Job()
        {
            RememberURL();
            if (!CheckIsLogin())
            {
                return Redirect(res.Pageviews.PvMemberSignIn);
            }
            else
            {
                GetStatusUser();
                SetPager();
                if (LogonCompLevel == 3)
                {
                    var lstJobCate = svAddress.SelectData<emJobCategory>("JobCateID,JobCateName", "IsDelete = 0");
                    ViewBag.CompLevel = LogonCompLevel;
                    ViewBag.JobCaties = lstJobCate;
                    ListJobType();
                    ListDegreeLevel();
                    ViewBag.Provinces = svAddress.ListProvince();
                    ViewBag.Districts = svAddress.ListDistrict();
                    SetPager();
                    return View();
                }
                else
                {
                    return Redirect(res.Pageviews.PvAccessDenied);
                }
            }
        }
        #endregion

        #region Post : Job
        [HttpPost]
        public ActionResult Job(FormCollection form)
        {
            SelectList_PageSize();
            SetPager(form);
            var emJobs = svAddress.SelectData<view_emJob>("*", "IsDelete = 0 and JobName LIKE N'%" + form["JobName"] + "%' and CompID = " + Request.Cookies[res.Common.lblWebsite].Values["emCompID"], null, (int)ViewBag.PageIndex, (int)ViewBag.PageSize); 
            ViewBag.emJobs = emJobs;
            ViewBag.TotalPage = svAddress.TotalPage;
            ViewBag.TotalRow = svAddress.TotalRow;
            return PartialView("MyB2B/Company/Grid/JobGrid");
        }
        #endregion

        #region SaveJob
        [HttpPost, ValidateInput(false)]
        public bool SaveJob(FormCollection form)
        {
            int objState = DataManager.ConvertToInteger(form["objState"]);//objState 1 คือ insert objState 2 คือ update
            var emJobs = new emJob();
            if (objState == 2)// update
            {
                emJobs = svAddress.SelectData<emJob>("*", " JobID = " + form["JobID"] + " AND RowVersion = " + form["RowVersion"]).First();
            }

            #region set ค่า emJob
            emJobs.CompID = DataManager.ConvertToInteger(Request.Cookies[res.Common.lblWebsite].Values["emCompID"]);
            emJobs.JobName = form["JobName"];
            emJobs.JobCateID = DataManager.ConvertToInteger(form["JobCateID"]);
            emJobs.JobType = DataManager.ConvertToByte(form["JobType"]);
            emJobs.JobDescription = ReplaceText(form["JobDescription"]);
            emJobs.JobRequired = ReplaceText(form["JobRequired"]);
            emJobs.ReqEducation = DataManager.ConvertToByte(form["ReqEducation"]);
            emJobs.ReqExp = DataManager.ConvertToByte(form["ReqExp"]);
            emJobs.RecruitAmount = DataManager.ConvertToByte(form["RecruitAmount"]);
            emJobs.WorkAddr = form["WorkAddr"];
            emJobs.DistrictID = DataManager.ConvertToInteger(form["DistrictID"]);
            emJobs.ProvinceID = DataManager.ConvertToInteger(form["ProvinceID"]);
            emJobs.JobSalary = form["JobSalary"];
            emJobs.IsAttachResume = DataManager.ConvertToBool(form["IsAttachResume"]);
            emJobs.Remark = form["Remark"];
            emJobs.ViewCount = 0;
            if (objState == 2)// update
            {
                emJobs.RowVersion = DataManager.ConvertToShort(emJobs.RowVersion + 1);
            }
            else
            {
                emJobs.RowFlag = 1;
                emJobs.RowVersion = 1;
                emJobs.CreatedBy = "sa";
                emJobs.ModifiedBy = "sa";
                emJobs.ModifiedDate = DateTime.Now;
                emJobs.CreatedDate = DateTime.Now;
            }
            #endregion

            #region Save emJob
            emJobs = svAddress.SaveData<emJob>(emJobs, "JobID");
            #endregion

            return svAddress.IsResult;
        }
        #endregion

        #region EditJob
        [HttpPost, ValidateInput(false)]
        public ActionResult EditJob(FormCollection form)
        {
            var emJob = new emJob();
            if (!string.IsNullOrEmpty(form["JobID"]))
            {
                emJob = svAddress.SelectData<emJob>("*", " IsDelete = 0 AND JobID =" + form["JobID"]).First();
            }
            return Json(
                new { 
            JobID = emJob.JobID,
            RowVersion = emJob.RowVersion,
           CompID = emJob.CompID,
           JobName = emJob.JobName,
           JobCateID = emJob.JobCateID,
           JobType = emJob.JobType,
           JobDescription= emJob.JobDescription,
           JobRequired = emJob.JobRequired,
           ReqEducation = emJob.ReqEducation,
           ReqExp = emJob.ReqExp,
           RecruitAmount = emJob.RecruitAmount,
           WorkAddr = emJob.WorkAddr,
           DistrictID = emJob.DistrictID,
           ProvinceID = emJob.ProvinceID,
           JobSalary = emJob.JobSalary,
           IsAttachResume = emJob.IsAttachResume,
           Remark = emJob.Remark,
           ViewCount = emJob.ViewCount
            });
        }
        #endregion

        #endregion

        #region Blog

        #region Get : Blog
        public ActionResult Blog()
        {
            RememberURL();
            if (!CheckIsLogin())
            {
                return Redirect(res.Pageviews.PvMemberSignIn);
            }
            else
            {
                var Article = svCompany.SelectData<b2bArticle>("*", "IsDelete = 0 and CompID = " + LogonCompID, "ListNo ASC");
                var ArticleTypes = svAddress.SelectData<b2bArticleType>("ArticleTypeID,ArticleTypeName", "IsDelete = 0");
                ViewBag.ArticleTypes = ArticleTypes;
                ViewBag.PageType = "Company";
                ViewBag.MenuName = "Blog";
                ViewBag.CompLevel = LogonCompLevel;
                ViewBag.CateLevel1 = LogonCompLevel;
                ViewBag.MemberType = LogonMemberType;
                GetStatusUser();

                return View();
            }
        }
        #endregion

        #region Post : Blog
        [HttpPost]
        public ActionResult Blog(FormCollection form)
        {
            SelectList_PageSize();
            List<view_b2bArticle> Blogs;
            SetPager(form);
            if (!string.IsNullOrEmpty(form["SearchBlog"]))
            {
                var ArticleTypeID = "";
                if (form["SearchType"] == "ArticleName")
                {
                    Blogs = svCompany.SelectData<view_b2bArticle>("*", "IsDelete = 0 and ArticleName LIKE N'%" + form["SearchBlog"] + "%' and CompID = " + LogonCompID, "ListNo ASC", (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
                }
                else
                {
                    var articletype = svAddress.SelectData<b2bArticleType>("ArticleTypeID,ArticleTypeName", "ArticleTypeName LIKE N'%" + form["SearchBlog"] + "%'");
                    foreach (var it in (List<b2bArticleType>)articletype)
                    {
                        ArticleTypeID = ArticleTypeID + "ArticleTypeID = " + it.ArticleTypeID.ToString() + " or ";
                    }
                    if (ArticleTypeID != "")
                    {
                        ArticleTypeID = ArticleTypeID.Substring(0, ArticleTypeID.Length - 4);
                        Blogs = svCompany.SelectData<view_b2bArticle>("*", "IsDelete = 0 and (" + ArticleTypeID + ") and CompID = " + LogonCompID, "ListNo ASC", (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
                    }
                    else
                    {
                        Blogs = svCompany.SelectData<view_b2bArticle>("*", "IsDelete = 0 and ArticleType = 0 and CompID = " + LogonCompID, "ListNo ASC", (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
                    }
                }
            }
            else
            {
                Blogs = svCompany.SelectData<view_b2bArticle>("*", "IsDelete = 0 and CompID = " + LogonCompID, "ListNo ASC", (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
            }


            ViewBag.Blogs = Blogs;
            ViewBag.TotalPage = svCompany.TotalPage;
            ViewBag.TotalRow = svCompany.TotalRow;
            return PartialView("MyB2B/Company/Grid/BlogGrid");
        }
        #endregion

        #region SaveBlog
        [HttpPost, ValidateInput(false)]
        public bool SaveBlog(FormCollection form)
        {
            int objState = DataManager.ConvertToInteger(form["objState"]);//objState 1 คือ insert objState 2 คือ update
            var Articles = new b2bArticle();
            var emArticles = new emArticle();
            if (objState == 2)// update
            {
                Articles = svCompany.SelectData<b2bArticle>("*", " ArticleID = " + form["ArticleID"] + " AND RowVersion = " + form["RowVersion"]).First();
            }

            #region set ค่า b2bArticle
            var ArticleImgPath = Articles.ImgPath;
            var ListNo = svCompany.SelectData<b2bArticle>("*", "IsDelete = 0 and CompID = " + LogonCompID, "ListNo ASC");
            Articles.CompID = DataManager.ConvertToInteger(LogonCompID);
            Articles.ArticleName = form["ArticleName"];
            Articles.ArticleTypeID = DataManager.ConvertToInteger(form["ArticleTypeID"]);
            Articles.Description = ReplaceText(form["Description"]);
            Articles.ShortDescription = form["ShortDescription"];
            Articles.ImgPath = form["ImgPath"];
            Articles.PageTitle = form["PageTitle"];
            if (objState == 2)// update
            {
                Articles.RowVersion = DataManager.ConvertToShort(Articles.RowVersion + 1);
                emArticles.RowVersion = DataManager.ConvertToShort(emArticles.RowVersion + 1);
            }
            else
            {
                Articles.ViewCount = 0;
                Articles.RowFlag = 1;
                Articles.RowVersion = 1;
                Articles.CreatedBy = "sa";
                Articles.ModifiedBy = "sa";
                Articles.ModifiedDate = DateTime.Now;
                Articles.CreatedDate = DateTime.Now;
                Articles.ListNo = ListNo.Count() + 1;
            }
            #endregion

            #region Save b2bArticle
            Articles = svCompany.SaveData<b2bArticle>(Articles, "ArticleID");
            if (svCompany.IsResult)
            {
                if (objState == 2)// update
                {
                    emArticles.RowVersion = Articles.RowVersion;
                }
                else
                {
                    emArticles.CompID = DataManager.ConvertToInteger(Request.Cookies[res.Common.lblWebsite].Values["emCompID"]);
                    emArticles.ArticleName = Articles.ArticleName;
                    emArticles.ArticleTypeID = Articles.ArticleTypeID;
                    emArticles.Description = Articles.Description;
                    emArticles.ShortDescription = Articles.ShortDescription;
                    emArticles.ImgPath = Articles.ImgPath;
                    emArticles.PageTitle = Articles.PageTitle;
                    emArticles.ViewCount = Articles.ViewCount;
                }

                #region Save emArticle
                emArticles = svMember.SaveData<emArticle>(emArticles, "ArticleID");
                #endregion

                if (svCompany.IsResult && svMember.IsResult)
                {
                    #region SaveArticleImg
                    if (!string.IsNullOrEmpty(form["ImgPath"]))
                    {
                        if (Articles.ImgPath != ArticleImgPath)
                        {
                            imgManager = new FileHelper();
                            imgManager.DirPath = "Article/" + LogonCompID + "/" + Articles.ArticleID;
                            imgManager.DirTempPath = "Temp/Article/" + LogonCompID;
                            imgManager.ImageName = form["ImgPath"];
                            //imgManager.ImageThumbName = "Thumb_" + form["ImgPath"];
                            imgManager.FullHeight = 0;
                            imgManager.FullWidth = 0;
                            imgManager.ThumbHeight = 150;
                            imgManager.ThumbWidth = 150;

                            imgManager.SaveImageFromTemp();
                        }
                    }
                    #endregion
                }
            }
            #endregion

            return svCompany.IsResult;
        }
        #endregion

        #region EditBlog
        [HttpPost, ValidateInput(false)]
        public ActionResult EditBlog(FormCollection form)
        {
            var Articles = new b2bArticle();
            if (!string.IsNullOrEmpty(form["ArticleID"]))
            {
                Articles = svCompany.SelectData<b2bArticle>("*", " IsDelete = 0 AND ArticleID =" + form["ArticleID"]).First();
            }
            return Json(new { ArticleID = Articles.ArticleID, CompID = Articles.CompID, RowVersion = Articles.RowVersion, ArticleTypeID = Articles.ArticleTypeID, ArticleName = Articles.ArticleName, pageTitle = Articles.PageTitle, ImgPath = Articles.ImgPath, Description = Articles.Description, ShortDescription = Articles.ShortDescription });
        }
        #endregion

        #region Listarticles

        [HttpPost]
        public ActionResult ChangeListNoArticle(List<int> id)
        {
            if (!CheckIsLogin())
                return Redirect(res.Pageviews.PvMemberSignIn);
            var svArticle = new CompanyService();
            var Article = new b2bArticle();
            try
            {
                List<int> list = new List<int>();
                var id_banner1 = id[1];
                var id_banner2 = id[0];
                Article = svArticle.SelectData<b2bArticle>("*", " ArticleID = " + id_banner1).First();
                list.Add(Convert.ToInt32(Article.ListNo));
                Article = svArticle.SelectData<b2bArticle>("*", " ArticleID = " + id_banner2).First();
                list.Add(Convert.ToInt32(Article.ListNo));

                svArticle.UpdateBlogListNo(id, list);
            }
            catch (Exception ex)
            {
                CreateLogFiles(ex);
            }

            return Json(new { IsResult = svArticle.IsResult, MsgError = GenerateMsgError(svArticle.MsgError), ID = id });
        }

        #endregion

        #endregion

        /*-----------------------IsSame----------------------*/

        #region IsMemberSameAddr
        [HttpPost]
        public ActionResult IsMemberSameAddr()
        {
            var sqlSelect = "MemberID,AddrLine1,PostalCode,Phone,Mobile,Fax,ProvinceID,DistrictID";
            var sqlWhere = " and IsDelete = 0 and WebID = " + res.Config.WebID;
            var Members = svMember.SelectData<view_Member>(sqlSelect, "MemberID = " + LogonMemberID + "" + sqlWhere).First();
            if (Members != null)
            {
                return Json(new { IsResult = true, Member = Members });
            }
            return Json(new { IsResult = false});
        }
        #endregion

        #region IsCompSameAddr
        [HttpPost]
        public ActionResult IsCompSameAddr()
        {
            var sqlSelect = "*";
            var sqlWhere = " and IsDelete = 0";
            var companies = svCompany.SelectData<view_Company>(sqlSelect, "MemberID = " + LogonMemberID + "" + sqlWhere).First();
            if (companies != null)
            {
                return Json(new { IsResult = true, Company = companies });
            }
            return Json(new { IsResult = false });
        }
        #endregion

        #region IsCompProSameAddr
        [HttpPost]
        public ActionResult IsCompProSameAddr() 
        {
            var companies = svCompany.SelectData<view_Company>("CompID", "MemberID = " + LogonMemberID + " and IsDelete = 0").First();
            Hashtable data = new Hashtable();
            var sqlSelect = "*";
            var sqlWhere = " and IsDelete = 0";
            var compProfiles = svCompany.SelectData<b2bCompanyProfile>(sqlSelect, "CompID = "+companies.CompID+"" + sqlWhere).First();
            if (compProfiles != null)
            {
                data.Add("CompName", compProfiles.CompName);
                data.Add("AddrLine1",compProfiles.AddrLine1);
                data.Add("ProvinceID", compProfiles.ProvinceID);
                data.Add("DistrictID", compProfiles.DistrictID);
                data.Add("PostalCode", compProfiles.PostalCode);
                data.Add("CompBizType", compProfiles.CompBizType);
                return Json(new { IsResult = true, CompProfile = data });
            }
            return Json(new { IsResult = false });
        }
        #endregion

        #region ValidateCompany
        public ActionResult ValidateCompany(string email, string compname, string compnameeng, string displayname)
        {
            var IsResult = true;
            var sqlSelect = "*";
            var sqlWhere = " AND IsDelete = 0 AND MemberID != " + LogonMemberID + "";

            if (!string.IsNullOrEmpty(email))
            {
                var Company = svCompany.SelectData<b2bCompany>(sqlSelect, "ContactEmail = N'" + email + "'" + sqlWhere);
                if (Company.Count() > 0)
                {
                    IsResult = false;
                }
            }
            if (!string.IsNullOrEmpty(compname))
            {
                var Company = svCompany.SelectData<b2bCompany>(sqlSelect, "compname = N'" + compname + "'" + sqlWhere);
                if (Company.Count() > 0)
                {
                    IsResult = false;
                }
            }
            if (!string.IsNullOrEmpty(compnameeng))
            {
                var Company = svCompany.SelectData<b2bCompany>(sqlSelect, "compnameeng = N'" + compnameeng + "'" + sqlWhere);
                if (Company.Count() > 0)
                {
                    IsResult = false;
                }
            }
            if (!string.IsNullOrEmpty(displayname))
            {
                var Company = svCompany.SelectData<b2bCompany>(sqlSelect, "displayname = N'" + displayname + "'" + sqlWhere);
                if (Company.Count() > 0)
                {
                    IsResult = false;
                }
            }
            return Json(IsResult);
        }
        #endregion

        /*--------------------CompImage----------------------*/
        #region SaveImage
        [HttpPost]
        public ActionResult SaveCompImg(HttpPostedFileBase FileCompImgPath)
        {
            imgManager = new FileHelper();
            imgManager.UploadImage("Temp/Companies/Image/" + LogonCompID, FileCompImgPath);
            Response.Cookies["CompID"].Value = DataManager.ConvertToString(LogonCompID);
            while(!imgManager.IsSuccess){
                Thread.Sleep(100);
            }
            return Json(new { newimage = imgManager.ImageName }, "text/plain");
        }
        #endregion

        #region RemoveImage
        [HttpPost]
        public ActionResult RemoveCompImage()
        {
            imgManager = new FileHelper();
            imgManager.DeleteFilesInDir("Temp/Companies/image/" + LogonCompID);    
            return Json(new { newimage = imgManager.ImageName });
        }
        #endregion

        /*--------------------LogoImg------------------------*/
        #region SaveLogo
        [HttpPost]
        public ActionResult SaveLogo(HttpPostedFileBase FileLogoImgPath)
        {
            imgManager = new FileHelper();
            #region Delete Folder
            imgManager.DeleteFilesInDir("Temp/Companies/Logo/" + LogonCompID);
            #endregion
            imgManager.UploadImage("Temp/Companies/Logo/" + LogonCompID, FileLogoImgPath);
            Response.Cookies["CompID"].Value = DataManager.ConvertToString(LogonCompID);
            return Json(new { newimage = imgManager.ImageName }, "text/plain");

        }
        #endregion

        #region RemoveLogo
        public ActionResult RemoveLogo()
        {
            imgManager = new FileHelper();
            imgManager.DeleteFilesInDir("Temp/Companies/Logo/" + LogonCompID);    
            return Json(new { newimage = imgManager.ImageName });
        }
        #endregion

        /*--------------------ContactImg---------------------*/
        #region SaveContactImg
        [HttpPost]
        public ActionResult SaveContactImg(HttpPostedFileBase FileContactImgPath)
        {
            imgManager = new FileHelper();
            #region Delete Folder
            imgManager.DeleteFilesInDir("Temp/Companies/Contact/" + LogonCompID);
            #endregion
            imgManager.UploadImage("Temp/Companies/Contact/" + LogonCompID, FileContactImgPath);
            Response.Cookies["CompID"].Value = DataManager.ConvertToString(LogonCompID);
            return Json(new { newimage = imgManager.ImageName }, "text/plain");

        }
        #endregion

        #region RemoveContactImg
        public ActionResult RemoveContactImg()
        {
            imgManager = new FileHelper();
            imgManager.DeleteFilesInDir("Temp/Companies/Contact/" + LogonCompID);  
            return Json(new { newimage = imgManager.ImageName });
        }
        #endregion

        /*--------------------ContactMapImg---------------------*/
        #region SaveMapImg
        [HttpPost]
        public ActionResult SaveMapImg(HttpPostedFileBase FileMapImgPath)
        {
            imgManager = new FileHelper();
            #region Delete Folder
            imgManager.DeleteFilesInDir("Temp/Companies/Map/" + LogonCompID);
            #endregion
            imgManager.UploadImage("Temp/Companies/Map/" + LogonCompID, FileMapImgPath);
            Response.Cookies["CompID"].Value = DataManager.ConvertToString(LogonCompID);
            return Json(new { newimage = imgManager.ImageName }, "text/plain");

        }
        #endregion

        #region RemoveMapImg
        public ActionResult RemoveMapImg(string filenames)
        {
            imgManager = new FileHelper();
            imgManager.DeleteFilesInDir("Temp/Companies/Map/" + LogonCompID);  
            return Json(new { newimage = imgManager.ImageName });
        }
        #endregion

        /*--------------------CertifyImg---------------------*/
        #region SaveCompCertifyImg
        [HttpPost]
        public ActionResult SaveCompCertifyImg(HttpPostedFileBase FileCertifyImgPath)
        {
            imgManager = new FileHelper();
            #region Delete Folder
            imgManager.DeleteFilesInDir("Temp/CompanyCertify/" + LogonCompID);
            #endregion
            imgManager.UploadImage("Temp/CompanyCertify/" + LogonCompID, FileCertifyImgPath);
            Response.Cookies["CompID"].Value = DataManager.ConvertToString(LogonCompID);
            return Json(new { newimage = imgManager.ImageName }, "text/plain");

        }
        #endregion

        #region RemoveCompCertifyImg
        public ActionResult RemoveCompCertifyImg(string filenames)
        {
            imgManager = new FileHelper();
            imgManager.DeleteFilesInDir("Temp/CompanyCertify/" + LogonCompID);
            return Json(new { newimage = imgManager.ImageName });
        }
        #endregion

        /*--------------------BlogImg---------------------*/
        #region SaveBlogImg
        [HttpPost]
        public ActionResult SaveBlogImg(HttpPostedFileBase FileImgPath)
        {
            imgManager = new FileHelper();
            #region Delete Folder
            imgManager.DeleteFilesInDir("Temp/Article/" + LogonCompID);
            #endregion
            imgManager.UploadImage("Temp/Article/" + LogonCompID, FileImgPath);
            Response.Cookies["CompID"].Value = DataManager.ConvertToString(LogonCompID);
            return Json(new { newimage = imgManager.ImageName }, "text/plain");

        }
        #endregion

        #region RemoveBlogImg
        public ActionResult RemoveBlogImg(string filenames)
        {
            imgManager = new FileHelper();
            imgManager.DeleteFilesInDir("Temp/Article/" + LogonCompID);
            return Json(new { newimage = imgManager.ImageName });
        }
        #endregion
        /*------------------------DeleteData-----------------------------*/

        #region DeleteData
        public bool DeleteData(List<int> ID, List<short> RowVersion, int KeyValue, string PrimaryKeyName)
        {
            if(PrimaryKeyName == "CompCertifyID")
            {
                svCompany.DeleteData<b2bCompanyCertify>(ID, RowVersion, PrimaryKeyName);
            }
            else if(PrimaryKeyName == "CompPaymentID")
            {
                svCompany.DeleteData<b2bCompanyPayment>(ID, RowVersion, PrimaryKeyName);
            }
            else if(PrimaryKeyName == "CompShipmentID")
            {
                svCompany.DeleteData<b2bCompanyShipment>(ID, RowVersion, PrimaryKeyName);
            }
            else if (PrimaryKeyName == "JobID")
            {
                svAddress.DeleteData<emJob>(ID, RowVersion, PrimaryKeyName);
                svCompany.IsResult = svAddress.IsResult;
            }
            else if (PrimaryKeyName == "ArticleID")
            {
                svCompany.DeleteData<b2bArticle>(ID, RowVersion, PrimaryKeyName);
                if (svCompany.IsResult)
                {
                    svAddress.DeleteData<emArticle>(ID, RowVersion, PrimaryKeyName);
                    svCompany.IsResult = svAddress.IsResult;
                }
            }
            return svCompany.IsResult;
        }
        #endregion
        
        #region DelData
        public ActionResult DelData(List<bool> Check, List<int> ID, List<int> MemID, List<short> RowVersion, string PrimaryKeyName)
        {
            CompanyService svCompany = new CompanyService();

            svCompany.DelData<b2bCompanyCertify>(Check, ID, RowVersion, PrimaryKeyName);


            if (PrimaryKeyName == "CompCertifyID")
            {
                svCompany.DelData<b2bCompanyCertify>(Check, ID, RowVersion, PrimaryKeyName);
            }
            else if (PrimaryKeyName == "CompPaymentID")
            {
                svCompany.DelData<b2bCompanyPayment>(Check, ID, RowVersion, PrimaryKeyName);
            }
            else if (PrimaryKeyName == "CompShipmentID")
            {
                svCompany.DelData<b2bCompanyShipment>(Check, ID, RowVersion, PrimaryKeyName);
            }
            else if (PrimaryKeyName == "JobID")
            {
                JobService svJob = new JobService();
                svJob.DelData<emJob>(Check, ID, RowVersion, PrimaryKeyName);
                svCompany.IsResult = svJob.IsResult;
            }
            else if (PrimaryKeyName == "ArticleID")
            {
                svCompany.DelData<b2bArticle>(Check, ID, RowVersion, PrimaryKeyName);
                if (svCompany.IsResult)
                {
                    ArticleService svArticle = new ArticleService();
                    svArticle.DeleteData<emArticle>(ID, RowVersion, PrimaryKeyName);
                    svCompany.IsResult = svArticle.IsResult;
                }
            }

            if (svCompany.IsResult)
            {
                return Json(new { Result = true });
            }
            else
            {
                return Json(new { Result = false });
            }
        }
        #endregion

        /*------------------------ChangeListNo-----------------------------*/
        #region ChangeListNo
        public bool ChangeListNo(List<int> ID, List<short> RowVersion, List<int> KeyValue, string PrimaryKeyName = "")
        {
            if (PrimaryKeyName == "CompCertifyID")
            {
                svCompany.ChangeListNo<b2bCompanyCertify>(ID, RowVersion, PrimaryKeyName, KeyValue, "ListNo");
            }
            else if (PrimaryKeyName == "CompPaymentID")
            {
                svCompany.ChangeListNo<b2bCompanyPayment>(ID, RowVersion, PrimaryKeyName, KeyValue, "ListNo");
            }
            else if (PrimaryKeyName == "CompShipmentID")
            {
                svCompany.ChangeListNo<b2bCompanyShipment>(ID, RowVersion, PrimaryKeyName, KeyValue, "ListNo");
            }
            else if (PrimaryKeyName == "ArticleID")
            {
                svCompany.ChangeListNo<b2bArticle>(ID, RowVersion, PrimaryKeyName, KeyValue, "ListNo");
                if (svCompany.IsResult)
                {
                    svAddress.ChangeListNo<emArticle>(ID, RowVersion, PrimaryKeyName, KeyValue, "ListNo");
                    svCompany.IsResult = svAddress.IsResult;
                }
            }
            else if (PrimaryKeyName == "CompMenuID")
            {
                svCompany.ChangeListNo<b2bCompanyMenu>(ID, RowVersion, PrimaryKeyName, KeyValue, "ListNo");
            }
            return svCompany.IsResult;
        }
        #endregion

        /*------------------------------------------------------------------*/

        #region Check Count Data

        #region Product
        public void ProductCount(int? id){
            var products = 0;
            //var sqlwherein = "";
            //switch (res.Common.lblWebsite)
            //{
            //    case "B2BThai": sqlwherein = " AND CategoryType IN (1,2)"; break;
            //    case "AntCart": sqlwherein = " AND CategoryType IN (3)"; break;
            //    case "myOtopThai": sqlwherein = " AND CategoryType IN (5)"; break;
            //    case "AppstoreThai": sqlwherein = " AND CategoryType IN (6)"; break;
            //    default: sqlwherein = ""; break;}
            
            if (LogonCompID == id)
            {
                string sqlSelect = "CompID,ProductID,ProductName";
                string sqlWhere = svProduct.CreateWhereAction(ProductAction.WebSite, id);
                products = svProduct.CountData<view_SearchProduct>(sqlSelect, sqlWhere);
                if (products > 0)
                {
                    ViewBag.ProductCount = products;
                    ViewBag.ProductType = 1;
                }
                else
                {
                    sqlWhere = svProduct.CreateWhereAction(ProductAction.WebSite, id);
                    products = svProduct.CountData<view_SearchProduct>(sqlSelect, sqlWhere);
                    ViewBag.ProductCount = products;
                    ViewBag.ProductType = 2;
                }
            }
            else
            {
                string sqlSelect = "CompID,ProductID,ProductName";
                string sqlWhere = svProduct.CreateWhereAction(ProductAction.FrontEnd, id);
                products = svProduct.CountData<view_SearchProduct>(sqlSelect, sqlWhere);
                if (products > 0)
                {
                    ViewBag.ProductCount = products;
                    ViewBag.ProductType = 1;
                }
                else
                {
                    sqlWhere = svProduct.CreateWhereAction(ProductAction.FrontEnd, id) ;
                    products = svProduct.CountData<view_SearchProduct>(sqlSelect, sqlWhere);
                    ViewBag.ProductCount = products;
                    ViewBag.ProductType = 2;
                }
            }
           
        }
        #endregion

        #region Order
        public void OrderCount(int? id)
        {
            svCompany = new CompanyService();

            #region Order
            string sqlSelectOrder = "CompPaymentID,AccNo,AccName,AccType,BankName,BranchName";
            string sqlWhereOrder = "CompID =" + id + " AND IsDelete = 0";
            var payment = svCompany.SelectData<view_CompanyPayment>(sqlSelectOrder, sqlWhereOrder).Count();
            ViewBag.PaymentCount = payment;

            sqlSelectOrder = "CompShipmentID,ShipmentName,ShipmentDuration,PackingName,Remark";
            sqlWhereOrder = "CompID =" + id + " AND IsDelete = 0";
            var shipment = svCompany.SelectData<b2bCompanyShipment>(sqlSelectOrder, sqlWhereOrder).Count();
            ViewBag.ShipmentCount = shipment;
            #endregion

        }
        #endregion

        #region Certify
        public void CertifyCount(int? id)
        {
            #region Certify
            string sqlSelectCertify = "CompCertifyID,CompID,CompName,CertifyName,CertifyImgPath";
            string sqlWhereCertify = "IsDelete = 0 AND CompID =" + id;
            var certify = svCompany.SelectData<view_CompanyCertify>(sqlSelectCertify, sqlWhereCertify).Count();
            ViewBag.CertifyCount = certify;
            #endregion
        }
        #endregion

        #endregion

        #region ChangeLogon
        public void SetLogonStatus() {

            var list = svCompany.SelectData<view_emCompanyMember>(" * ", "MemberID = " + LogonMemberID);
            RegisterSessionLogon(list); 
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
    }
}
