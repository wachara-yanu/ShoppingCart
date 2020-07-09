using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;
using Ouikum;
using Ouikum.Product;
using Ouikum.Company;
using Ouikum.Purchase;
//using Prosoft.Base;
using Ouikum.Common;
using Ouikum.BizType;
using Prosoft.Service;
using res = Prosoft.Resource.Web.Ouikum;

namespace Ouikum.Web.MyB2B
{
    public partial class PurchaseController : BaseController
    {
        #region Member
        PurchaseService svPurchase;
        CompanyService svCompany;
        MemberService svMember;
        AddressService svAddress;
        BizTypeService svBizType;
        emCompanyService svEmCompany;
        #endregion

        #region Get List
        [HttpGet]
        public ActionResult List(string Type)
        {
            RememberURL();
            if (!CheckIsLogin())
            {
                return Redirect(res.Pageviews.PvMemberSignIn);
            }
            else
            {
                GetStatusUser();
                if (LogonServiceType == 9)
                {
                    SetPager();
                    CountLead();
                    ViewBag.hidPageType = Type;
                    Response.Cookies["PurchasePageAction"].Value = Type;
                    Response.Cookies["PurchasePageAction"].Expires = DateTime.Now.AddHours(1);
                    return View();
                }
                else
                {
                    return Redirect(PathHome);
                }
            }
            
        }
        #endregion

        #region Post List
        [HttpPost]
        public ActionResult List(FormCollection form)
        {
            svPurchase = new PurchaseService();
            string sqlWhere = "";
            string sqlSelect = "";
            SelectList_PageSize();
            SetPager(form);
            string pagetype = Request.Cookies["PurchasePageAction"].Value;

            if (pagetype == "emLead")
            {
                sqlWhere = "IsDelete = 0";
                sqlSelect = "EmLeadID,EmLeadCode,EmLeadName,EmLeadKeyword,Remark,LeadCount,IsMark,IsImportance,LeadFolderID,CreatedDate,RowVersion";
                var emLeads = svPurchase.SelectData<b2bEmLead>(sqlSelect, sqlWhere, "CreatedDate DESC",(int)ViewBag.PageIndex, (int)ViewBag.PageSize);

                ViewBag.EmLeads = emLeads;
                ViewBag.TotalPage = svPurchase.TotalPage;
                ViewBag.TotalRow = svPurchase.TotalRow;
                ViewBag.hidPageType = pagetype;
                return PartialView("UC/ImportEmLeadGrid");
            }
            else
            {
                sqlWhere = "IsDelete = 0";
                sqlSelect = "AssignLeadID,AssignLeadName,ToContactName,ToContactEmail,ToContactTel,OtherEmail,IsDefaultLead,LeadFolderID,CreatedDate,RowVersion";
                var AssignLeads = svPurchase.SelectData<b2bAssignLead>(sqlSelect, sqlWhere, "CreatedDate DESC", (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
                ViewBag.AssignLeads = AssignLeads;
                ViewBag.TotalPage = svPurchase.TotalPage;
                ViewBag.TotalRow = svPurchase.TotalRow;
                ViewBag.hidPageType = pagetype;
                return PartialView("UC/AssignLeadGrid");

            }
            
        }
        #endregion

        #region Get CreateLead
        [HttpGet]
        public ActionResult CreateLead()
        {
            RememberURL();
            if (!CheckIsLogin())
            {
                return Redirect(res.Pageviews.PvMemberSignIn);
            }
            else
            {

                GetStatusUser();
                if(LogonServiceType == 9)
                {
                    svAddress = new AddressService();
                    svBizType = new BizType.BizTypeService();
                    svPurchase = new PurchaseService();

                    SetPager();
                    CountLead();
                    ViewBag.AssignLeadCode = svPurchase.GenLeadCode(0, 1);
                    ViewBag.Provinces = svAddress.GetProvinceAll().ToList();
                    ViewBag.Biztypes = svBizType.GetBiztype().ToList();
                    ViewBag.setEmLeadID = "";
                    return View();
                }
                else
                {
                    return Redirect(PathHome);
                }
            }
        }
        #endregion

        #region Post CreateLead
        [HttpPost]
        public ActionResult CreateLead(FormCollection form)
        {
            svCompany = new CompanyService();
            svPurchase = new PurchaseService();
            SelectList_PageSize();
            SetPager(form);
            SetProductPager(form);

            if (!string.IsNullOrEmpty(ViewBag.TextSearch))
            {

                if (!string.IsNullOrEmpty(form["hidEmLeadID"]))
                {
                    int emlead_id = DataManager.ConvertToInteger(form["hidEmLeadID"]);
                    string sqlWhere = "EmLeadID = " + emlead_id + " AND IsDelete = 0 AND CompIsDelete = 0";
                    string sqlSelect = "EmLeadID,CompID,CompName,ContactFirstName,ContactLastName,ContactPhone,CompPhone,ContactEmail,DistrictName,ProvinceName";
                    var Companies = svPurchase.SelectData<view_EmCompanyLead>(sqlSelect, sqlWhere, "CompID ASC", ViewBag.PageIndex, ViewBag.PageSize);
                    ViewBag.Suppliers = Companies;
                    ViewBag.TotalPage = svPurchase.TotalPage;
                    ViewBag.TotalRow = svPurchase.TotalRow;
                    ViewBag.setEmLeadID = emlead_id;
                    return PartialView("UC/SearchEmLeadGrid");

                }
                else
                {

                    var SupplierCompID = SearchSupplier(ProductAction.FrontEnd);
                    if (SupplierCompID.Count() > 0)
                    {
                        string WhereIN = CreateWhereIN(SupplierCompID, "CompID");
                        string sqlWhere = "Webid = 1 AND IsDelete = 0" + WhereIN;
                        string sqlSelect = "CompID,CompName,ContactFirstName,ContactLastName,ContactPhone,CompPhone,ContactEmail,DistrictName,ProvinceName";
                        var Companies = svCompany.SelectData<view_Company>(sqlSelect, sqlWhere, "CompID ASC", ViewBag.PageIndex, ViewBag.PageSize);
                        ViewBag.Suppliers = Companies;
                        ViewBag.TotalPage = svCompany.TotalPage;
                        ViewBag.TotalRow = svCompany.TotalRow;
                        ViewBag.TextSearch = ViewBag.TextSearch;
                    }
                    ViewBag.setEmLeadID = null;
                    return PartialView("UC/CreateLeadGrid");

                }
                   
            }
            else
            {
                ViewBag.setEmLeadID = null;
                return PartialView("UC/CreateLeadGrid");
            }

        }
        #endregion

        #region Save AssignLead
        [HttpPost, ValidateInput(false)]
        public ActionResult SaveAssignLead(FormCollection form)
        {
            var b2bAssignLead = new b2bAssignLead();
            svPurchase = new PurchaseService();
            svCompany = new CompanyService();
            int SupplierCompID = 0;
            try
            {
                int CompID = DataManager.ConvertToInteger(form["hidToCompID"]);
                var AssignLeadCode = form["hidAssignLeadCode"];

                if (CompID > 0)
                {
                    b2bAssignLead.ToCompID = CompID;
                }
                else 
                {
                    b2bAssignLead.ToCompID = null;
                }

                #region set ค่า b2bAssignLead
                b2bAssignLead.AssignLeadCode = AssignLeadCode;
                b2bAssignLead.AssignLeadName = form["hidProductName"];
                b2bAssignLead.ToCompCode = form["hidToCompCode"];
                b2bAssignLead.ToCompName = form["ToCompName"];
                b2bAssignLead.ToAddrLine1 = form["ToAddress"];
                b2bAssignLead.ToDistrictID = DataManager.ConvertToInteger(form["ToDistrictID"]);
                b2bAssignLead.ToContactName = form["ToContactName"];
                b2bAssignLead.ToContactEmail = form["ToContactEmail"];
                b2bAssignLead.ToContactTel = form["ToContactTel"];
                //b2bAssignLead.IsSentEmail = DataManager.ConvertToBool(form["chkSendEmail"]);
                b2bAssignLead.OtherEmail = form["ToSendEmail"];
                b2bAssignLead.IsDefaultLead = DataManager.ConvertToBool(form["chkSetDefaultLead"]);
                b2bAssignLead.IsRegisterMember = DataManager.ConvertToBool(form["chkRegMember"]);
                b2bAssignLead.CompLeadCount = DataManager.ConvertToShort(form["hidCountCompLead"]);
                #endregion

                #region Set Email

                var Email = "";
                if (!string.IsNullOrEmpty(form["ToSendEmail"]))
                {
                    Email = form["ToSendEmail"];
                }
                else if (!string.IsNullOrEmpty(form["ToContactEmail"]))
                {
                    Email = form["ToContactEmail"];
                }
                
                b2bAssignLead.IsSentEmail = true;

                #endregion

                #region Save AssignLead
                int AssignLeadID = svPurchase.InsertAssignLead(b2bAssignLead);
                #endregion

                #region set and save b2bCompanyLead

                if (!string.IsNullOrEmpty(form["hidAllSupplierCompID"]))
                {
                    string supplierID = form["hidAllSupplierCompID"];
                    string[] subSupplierCompID = supplierID.Split(',');


                    for (int i = 0; i < subSupplierCompID.Length; i++)
                    {
                        var b2bCompanyLead = new b2bCompanyLead();
                        //Get CompID and CompCode
                        SupplierCompID = DataManager.ConvertToInteger(subSupplierCompID[i]);
                        var Company = svCompany.SelectData<b2bCompany>("CompID,CompCode", "CompID = " + SupplierCompID).First();
                        b2bCompanyLead.AssignLeadID = AssignLeadID;
                        b2bCompanyLead.CompID = SupplierCompID;
                        b2bCompanyLead.CompCode = Company.CompCode;
                        svPurchase.InsertCompanyLead(b2bCompanyLead);
                    }

                }
                #endregion

                #region Save b2bEmLead and b2bEmCompanyLead
                if (b2bAssignLead.IsDefaultLead == true)
                {

                    #region set ค่า b2bEmLead
                    var b2bEmLead = new b2bEmLead();

                    if (LogonCompID > 0)
                    {
                        svMember = new MemberService();
                        string sqlWhere = "IsDelete = 0 AND CompID = " + LogonCompID;
                        var Member = svMember.SelectData<view_CompMember>("MemberID,CompID,UserName", sqlWhere).First();
                        b2bEmLead.MemberID = Member.MemberID;
                    }

                    var EmLeadCode = svPurchase.GenLeadCode(CompID, 2);
                    b2bEmLead.EmLeadCode = EmLeadCode;
                    b2bEmLead.EmLeadName = form["txtSetDefaultLead"];
                    b2bEmLead.EmLeadKeyword = form["hidProductName"];
                    b2bEmLead.LeadCount = DataManager.ConvertToShort(form["hidCountCompLead"]);
                    #endregion

                    #region Save EmLead then return EmLeadID
                    int EmLeadID = svPurchase.InsertEmLead(b2bEmLead);
                    #endregion

                    #region set and save b2bEmCompanyLead

                    if (!string.IsNullOrEmpty(form["hidAllSupplierCompID"]))
                    {
                        string supplierID = form["hidAllSupplierCompID"];
                        string[] subSupplierCompID = supplierID.Split(',');


                        for (int i = 0; i < subSupplierCompID.Length; i++)
                        {
                            var b2bEmCompanyLead = new b2bEmCompanyLead();
                            SupplierCompID = DataManager.ConvertToInteger(subSupplierCompID[i]);
                            b2bEmCompanyLead.EmLeadID = EmLeadID;
                            b2bEmCompanyLead.CompID = SupplierCompID;
                            b2bEmCompanyLead.EmCompLeadCode = svPurchase.GenLeadCode(SupplierCompID, 3);
                            svPurchase.InsertEmCompanyLead(b2bEmCompanyLead);
                        }

                    }
                    #endregion
                    
                }
                #endregion

                #region Register Member
                if (b2bAssignLead.IsRegisterMember == true)
                {
                    RegisterNewMember(form);
                }
                #endregion

                #region Send Email
                SendEmailToBuyer(Email, AssignLeadID, form, SupplierCompID);
                #endregion

                return Json(true);
            }
            catch (Exception ex)
            {
                CreateLogFiles(ex);
                return Redirect("~/MyB2B/Purchase/CreateLead");
            }
            
        }
        #endregion

        #region  ListEmLead
        [HttpPost]
        public ActionResult ImportEmLead(FormCollection form)
        {
            svPurchase = new PurchaseService();
            SetPager(form);
            if (ViewBag.TextSearch != null && ViewBag.TextSearch != "")
            {

                string sqlWhere = "EmLeadKeyword LIKE N'" + ViewBag.TextSearch + "%' AND IsDelete = 0";
                string sqlSelect = "EmLeadID,EmLeadCode,EmLeadName,EmLeadKeyword,Remark,LeadCount,LeadFolderID";
                var emLeads = svPurchase.SelectData<b2bEmLead>(sqlSelect, sqlWhere, "CreatedDate DESC", ViewBag.PageIndex, ViewBag.PageSize);

                ViewBag.EmLeads = emLeads;
                ViewBag.TotalPage = svPurchase.TotalPage;
                ViewBag.TotalRow = svPurchase.TotalRow;
            }
            else
            {
                string sqlWhere = "IsDelete = 0";
                string sqlSelect = "EmLeadID,EmLeadCode,EmLeadName,EmLeadKeyword,Remark,LeadCount,LeadFolderID";
                var emLeads = svPurchase.SelectData<b2bEmLead>(sqlSelect, sqlWhere, "CreatedDate DESC", ViewBag.PageIndex, ViewBag.PageSize);

                ViewBag.EmLeads = emLeads;
                ViewBag.TotalPage = svPurchase.TotalPage;
                ViewBag.TotalRow = svPurchase.TotalRow;
            }
            return PartialView("UC/ImportEmLeadGrid");
        }
        #endregion

        #region Get CreateEmLead
        [HttpGet]
        public ActionResult CreateEmLead()
        {
            RememberURL();
            if (!CheckIsLogin())
            {
                return Redirect(res.Pageviews.PvMemberSignIn);
            }
            else
            {

                GetStatusUser();
                if (LogonServiceType == 9)
                {
                    svAddress = new AddressService();
                    svBizType = new BizType.BizTypeService();
                    svPurchase = new PurchaseService();

                    SetPager();
                    CountLead();
                    ViewBag.EmLeadCode = svPurchase.GenLeadCode(0, 2);
                    ViewBag.actionEmLead = 0;
                    return View();
                }
                else
                {
                    return Redirect(PathHome);
                }
            }
        }
        #endregion

        #region Post CreateEmLead
        [HttpPost]
        public ActionResult CreateEmLead(FormCollection form)
        {
            if (LogonCompID > 0)
            {
                var b2bEmLead = new b2bEmLead();
                svPurchase = new PurchaseService();

                //svMember = new MemberService();
                //string sqlWhere = "IsDelete = 0 AND CompID = " + LogonCompID;
                //var Member = svMember.SelectData<emMember>("MemberID,UserName", sqlWhere).First();
                //b2bEmLead.MemberID = Member.MemberID;

                var EmLeadCode = form["hidEmLeadCode"];
                b2bEmLead.EmLeadCode = EmLeadCode;
                b2bEmLead.EmLeadName = form["txtLeadName"];
                b2bEmLead.EmLeadKeyword = form["txtKeyword"];
                b2bEmLead.Remark = form["txtRemark"];
                b2bEmLead.LeadCount = DataManager.ConvertToShort(form["hidCountCompLead"]);


                #region Save EmLead then return EmLeadID
                int EmLeadID = svPurchase.InsertEmLead(b2bEmLead);
                #endregion

                #region set and save b2bEmCompanyLead

                if (!string.IsNullOrEmpty(form["hidAllSupplierCompID"]))
                {
                    string supplierID = form["hidAllSupplierCompID"];
                    string[] subSupplierCompID = supplierID.Split(',');


                    for (int i = 0; i < subSupplierCompID.Length; i++)
                    {
                        var b2bEmCompanyLead = new b2bEmCompanyLead();
                        int SupplierCompID = DataManager.ConvertToInteger(subSupplierCompID[i]);
                        b2bEmCompanyLead.EmLeadID = EmLeadID;
                        b2bEmCompanyLead.CompID = SupplierCompID;
                        b2bEmCompanyLead.EmCompLeadCode = svPurchase.GenLeadCode(SupplierCompID, 3);
                        svPurchase.InsertEmCompanyLead(b2bEmCompanyLead);
                    }

                }
                #endregion
                
            }

            ViewBag.actionEmLead = 0;
            return Redirect("~/MyB2B/Purchase/CreateEmLead");
        }
        #endregion

        #region OpenEmLead
        public ActionResult OpenEmLead(int? id)
        {
            RememberURL();
            if (!CheckIsLogin())
            {
                return Redirect(res.Pageviews.PvMemberSignIn);
            }
            else
            {
                GetStatusUser();
                if(LogonServiceType == 9){
                    CountLead();
                    ViewBag.actionEmLead = 1;
                    svPurchase = new PurchaseService();

                    if (id > 0)
                    {
                        string sqlWhere = "EmLeadID = " + id + " AND IsDelete = 0";
                        string sqlSelect = "EmLeadID,EmLeadCode,EmLeadName,EmLeadKeyword,Remark,LeadCount";
                        var emLeads = svPurchase.SelectData<b2bEmLead>(sqlSelect, sqlWhere).First();
                        ViewBag.EmLeads = emLeads;

                        sqlWhere = "EmLeadID = " + id + " AND IsDelete = 0 AND CompIsDelete = 0";
                        sqlSelect = "EmCompLeadID,EmLeadID,CompID,CompName";
                        var emCompLeads = svPurchase.SelectData<view_EmCompanyLead>(sqlSelect, sqlWhere,"CompName ASC");
                        ViewBag.EmCompLeads = emCompLeads;
                        return View();

                    }
                    else 
                    {
                        return Redirect("~/MyB2B/Purchase/CreateEmLead");
                    }
                }else{
                    return Redirect(PathHome);
                }

                
            }

        }
        #endregion

        #region RegisterNewMember
        public void RegisterNewMember(FormCollection form)
        {
            MemberService svMember = new MemberService();
            CompanyService svCompany = new CompanyService();
            Register model = new Register();

            model.CompName = form["txtRegCompName"];
            model.DisplayName = form["txtRegCompName"];
            model.ProvinceID = DataManager.ConvertToInteger(form["ToProvinceID"]);
            model.DistrictID  = DataManager.ConvertToInteger(form["ToDistrictID"]);
            model.AddrLine1 = form["ToAddress"];
            model.FirstName_register = form["ToContactName"];
            model.LastName = form["ToContactLastName"];
            model.Emails = form["ToContactEmail"];
            model.Phone = form["ToContactTel"];
            model.UserName = form["txtRegUsername"];
            model.Password = form["txtRegPassword"];
            model.ServiceType = DataManager.ConvertToShort(form["regServiceType"]);
            model.BizTypeID = DataManager.ConvertToShort(form["RegBusinessTypeID"]);
            model.MemberType = 1;

            svMember.UserRegister(model);
            if (svMember.IsResult)
            {
                svCompany.InsertCompany(model);
            }

            #region Send Email
                OnSendMailInformUserName(model.UserName,model.Password,model.FirstName_register,model.Emails,model.CompLevel);
            #endregion
        }
        #endregion

        #region GetUsername
        [HttpPost]
        public ActionResult GetUsername(string query)
        {
                svMember = new MemberService();

                string sqlWhere = "IsDelete = 0 AND UserName LIKE N'" + query + "%' ";
                var allMembers = svMember.SelectData<emMember>("MemberID,Username", sqlWhere, "Username ASC");
                var usernames = allMembers.Select(it => it.UserName).ToList();

                return Json(usernames);
           
        }
        #endregion

        #region GetCompCode
        [HttpPost]
        public ActionResult GetCompCode(string query)
        {
            svCompany = new CompanyService();

            string sqlWhere = "IsDelete = 0 AND CompCode LIKE N'" + query + "%' ";
            var allCompCodes = svCompany.SelectData<b2bCompany>("CompID,CompCode", sqlWhere, "CompCode ASC");
            var CompCodes = allCompCodes.Select(it => it.CompCode).ToList();

            return Json(CompCodes);
        }
        #endregion

        #region GetCompName
        [HttpPost]
        public ActionResult GetCompName(string query)
        {
            svCompany = new CompanyService();

            string sqlWhere = "IsDelete = 0 AND CompName LIKE N'" + query + "%' ";
            var allCompNames = svCompany.SelectData<b2bCompany>("CompID,CompName", sqlWhere, "CompName ASC");
            var CompNames = allCompNames.Select(it => it.CompName).ToList();

            return Json(CompNames);
        }
        #endregion

        #region SearchCompByUsername
        [HttpPost]
        public ActionResult SearchCompByUsername(string txtSearch)
        {
            svEmCompany = new emCompanyService();
            Hashtable data = new Hashtable();

            string sqlSelect = "CompID,UserName,CompName,CompCode,FirstName,LastName,CompDistrictID,CompProvinceID,CompAddrLine1,CompAddrLine2,CompPhone,Email";
            string sqlWhere = "IsDelete = 0 AND UserName ='"+txtSearch+"'";
            var emCompany = svEmCompany.SelectData<view_emCompanyMember>(sqlSelect, sqlWhere).First();


            var ContactName = emCompany.FirstName + " " + emCompany.LastName;
            var CompAddr = emCompany.CompAddrLine1  ;
            data.Add("UserName", emCompany.UserName);
            data.Add("CompID", emCompany.CompID);
            data.Add("CompCode", emCompany.CompCode);
            data.Add("CompName", emCompany.CompName);
            data.Add("ContactName", ContactName);
            data.Add("CompDistrictID", emCompany.CompDistrictID);
            data.Add("CompProvinceID", emCompany.CompProvinceID);
            data.Add("CompAddr", CompAddr);
            data.Add("CompPhone", emCompany.CompPhone);
            data.Add("CompEmail", emCompany.Email);


            return Json(data);
        }
        #endregion

        #region SearchCompByCompCode
        [HttpPost]
        public ActionResult SearchCompByCompCode(string txtSearch)
        {
            svCompany = new CompanyService();
            Hashtable data = new Hashtable();

            string sqlSelect = "CompID,CompCode,CompName,ContactFirstName,ContactLastName,CompDistrictID,CompProvinceID,CompAddrLine1,CompAddrLine2,ContactPhone,ContactEmail";
            string sqlWhere = "IsDelete = 0 AND CompCode ='" + txtSearch + "'";
            var Companys = svCompany.SelectData<view_Company>(sqlSelect, sqlWhere).First();


            var ContactName = Companys.ContactFirstName + " " + Companys.ContactLastName;
            var CompAddr = Companys.CompAddrLine1;
            data.Add("CompID", Companys.CompID);
            data.Add("CompCode", Companys.CompCode);
            data.Add("CompName", Companys.CompName);
            data.Add("ContactName", ContactName);
            data.Add("CompDistrictID", Companys.CompDistrictID);
            data.Add("CompProvinceID", Companys.CompProvinceID);
            data.Add("CompAddr", CompAddr);
            data.Add("CompPhone", Companys.ContactPhone);
            data.Add("CompEmail", Companys.ContactEmail);


            return Json(data);
        }
        #endregion

        #region SearchCompByCompName
        [HttpPost]
        public ActionResult SearchCompByCompName(string txtSearch)
        {
            svEmCompany = new emCompanyService();
            Hashtable data = new Hashtable();

            string sqlSelect = "CompID,UserName,CompName,CompCode,FirstName,LastName,CompDistrictID,CompProvinceID,CompAddrLine1,CompAddrLine2,CompPhone,Email";
            string sqlWhere = "IsDelete = 0 AND CompName ='" + txtSearch + "'";
            var emCompany = svEmCompany.SelectData<view_emCompanyMember>(sqlSelect, sqlWhere).First();


            var ContactName = emCompany.FirstName + " " + emCompany.LastName;
            var CompAddr = emCompany.CompAddrLine1;
            data.Add("UserName", emCompany.UserName);
            data.Add("CompID", emCompany.CompID);
            data.Add("CompCode", emCompany.CompCode);
            data.Add("CompName", emCompany.CompName);
            data.Add("ContactName", ContactName);
            data.Add("CompDistrictID", emCompany.CompDistrictID);
            data.Add("CompProvinceID", emCompany.CompProvinceID);
            data.Add("CompAddr", CompAddr);
            data.Add("CompPhone", emCompany.CompPhone);
            data.Add("CompEmail", emCompany.Email);


            return Json(data);
        }
        #endregion

        #region Send Email to a Buyer
        public bool SendEmailToBuyer(string Email, int AssignLeadID, FormCollection form, int SupplierCompID)
        {
            #region variable
            bool IsSend = true;
            var Detail = "";
            var mailTo = new List<string>();
            var mailCC = new List<string>();
            Hashtable EmailDetail = new Hashtable();
            #endregion

            #region Set Content & Value For Send Email
            //string encrypt_id = EnCodeID(AssignLeadID);
            string Subject = res.Email.lblSubjectSuppliers1 + " : " + form["hidProductName"] + " - " + res.Common.lblDomainShortName;
            string b2bthai_url = res.Pageviews.UrlWeb;
            string pathlogo = b2bthai_url + "/Content/Default/logo/Ouikum/img_Logo120x74.png";
            string report_url = b2bthai_url + "/MyB2B/Purchase/Report/" + AssignLeadID;
            string productlist_url = b2bthai_url + "/Search/Product";
            string buylead_url = form["txtBuyleadUrl"];

            //var CompName = svPurchase.SelectData<view_AssignLeadForReport>("ToCompName", "AssignLeadID" + AssignLeadID);
            //var sqlSelect = "AssignLeadID,ToCompName";
            //var sqlWhere = "AssignLeadID = " + AssignLeadID;
            //var Companies = svPurchase.SelectData<view_AssignLeadForReport>(sqlSelect, sqlWhere, "");

            var Comp = svCompany.SelectData<b2bAssignLead>("*", "AssignLeadID = " + AssignLeadID, null, 1, 1).First();

            EmailDetail["b2bthaiUrl"] = b2bthai_url;
            EmailDetail["pathLogo"] = pathlogo;
            EmailDetail["buyleadUrl"] = buylead_url;
            EmailDetail["reportUrl"] = report_url;
            EmailDetail["contactName"] = form["ToContactName"];
            EmailDetail["productlistUrl"] = productlist_url;
            EmailDetail["productName"] = form["hidProductName"];
            EmailDetail["compName"] = Comp.ToCompName;
            // data for set msg detail
            ViewBag.Data = EmailDetail;
            Detail = PartialViewToString("UC/Email/PurchaseToBuyer");


            var mailFrom = res.Config.EmailNoReply;
            mailTo.Add(Email);
            #endregion

            IsSend = OnSendByAlertEmail(Subject, mailFrom, mailTo, mailCC, Detail);

            if (IsSend)
            {
                SendEmailToSuppliers(form);
            }
            return IsSend;
        }
        #endregion

        #region Send Email to Suppliers
        public void SendEmailToSuppliers(FormCollection form)
        {
            if (!string.IsNullOrEmpty(form["hidAllSupplierCompID"]))
            {
                #region variable
                bool IsSend = true;
                var Detail = "";
                var mailCC = new List<string>();
                svCompany = new CompanyService();
                svEmCompany = new emCompanyService();
               
                Hashtable EmailDetail = new Hashtable();
                
                #endregion

                #region Set Content & Value For Send Email

                string Subject = res.Email.lblSubjectSuppliers3 + " : " + form["hidProductName"] + " - " + res.Email.lblWeb_B2BThai;
                string b2bthai_url = res.Pageviews.UrlWeb;
                string pathlogo = b2bthai_url + "/Content/Default/logo/Ouikum/img_Logo120x74.png";
                string buylead_url = form["txtBuyleadUrl"];

                #region Select Suppliers
                //Convert string to int
                var getCompID = form["hidAllSupplierCompID"];
                string[] strCompID = getCompID.Split(',');
                string WhereIN = CreateWhereIN(strCompID,"CompID");
                string sqlWhere = WhereIN + " AND IsDelete = 0";
                string sqlSelect = "CompID,CompName,ContactFirstName,ContactLastName,ContactPhone,ContactEmail,emCompID";
                var Companies = svCompany.SelectData<view_Company>(sqlSelect, sqlWhere);
                #endregion

               
                var getAttachCompID = form["hidAttachEmailCompID"];
                string[] strAttachCompID = getAttachCompID.Split(',');
                var mailFrom = res.Config.EmailNoReply;
                var mailTo = new List<string>();

                foreach (var it in Companies)
                {
                    #region use emCompID to query Username and Password
                    EmailDetail["Username"] = "";
                    EmailDetail["Password"] = "";
                    for (int i = 0; i < strAttachCompID.Length; i++){
                        if (DataManager.ConvertToInteger(strAttachCompID[i]) == it.CompID)
                        {
                            EncryptManager encrypt = new EncryptManager();
                            sqlWhere = "IsDelete = 0 AND CompID = " + it.emCompID;
                            sqlSelect = "MemberID,CompID,UserName,Password";
                            var Member = svEmCompany.SelectData<view_emCompanyMember>(sqlSelect, sqlWhere).First();
                            EmailDetail["Username"] = Member.UserName;
                            EmailDetail["Password"] = encrypt.DecryptData(Member.Password);
                        }
                        
                    }
                     #endregion

                    //string contectname = "";
                    //if (!string.IsNullOrEmpty(it.ContactFirstName))
                    //{
                    //    contectname = it.ContactFirstName + " " + it.ContactLastName;
                    //}
                    //else
                    //{
                    //    contectname = it.CompName;
                    //}

                    string CompWebsite_url = b2bthai_url+"/CompanyWebsite/"+ Url.ReplaceUrl(it.CompName) +"/Main/Index/" + it.CompID ;

                    EmailDetail["b2bthaiUrl"] = b2bthai_url;
                    EmailDetail["pathLogo"] = pathlogo;
                    EmailDetail["buyleadUrl"] = buylead_url;
                    EmailDetail["websiteUrl"] = CompWebsite_url;
                    EmailDetail["compName"] = it.CompName;
                    EmailDetail["ContactFirstName"] = it.ContactFirstName;
                    EmailDetail["ContactLastName"] = it.ContactLastName;
                    EmailDetail["ContactPhone"] = it.ContactPhone;
                    EmailDetail["ContactEmail"] = it.ContactEmail;
                    EmailDetail["keyword"] = form["hidProductName"];

                    // data for set msg detail
                    ViewBag.Data = EmailDetail;
                    Detail = PartialViewToString("UC/Email/PurchaseToSupplier");

                    
                    mailTo.Add(it.ContactEmail);

                    // send email
                    

                }
                IsSend = OnSendByAlertEmail(Subject, mailFrom, mailTo, mailCC, Detail);
                #endregion
            }

        }
        #endregion

        #region Send Email to Member
        public void SendEmailToMember(Register model)
        {
            #region variable
            bool IsSend = true;
            var Detail = "";
            var mailTo = new List<string>();
            var mailCC = new List<string>();
            Hashtable EmailDetail = new Hashtable();
            #endregion

            #region Set Content & Value For Send Email

            string b2bthai_url = res.Pageviews.UrlWeb;
            string pathlogo = b2bthai_url + "/Content/Default/logo/Ouikum/img_Logo120x74.png";
            string pathSME = "https://ouikumstorage.blob.core.windows.net/upload/Content/Email/images/img_sme_mail.png";
            string pathF = b2bthai_url + "/Content/Default/images/icon_freesmall.png";
            string pathG = b2bthai_url + "/Content/Default/images/icon_goldsmall.png";
            var subject = res.Email.lblSubject;

            EmailDetail["b2bthaiUrl"] = b2bthai_url;
            EmailDetail["pathLogo"] = pathlogo;
            EmailDetail["pathSME"] = pathSME;

            EmailDetail["pathPackage"] = pathF;
            EmailDetail["pathGF"] = " Free Member";
            EmailDetail["color"] = "#0099CC";

            EmailDetail["FirstName"] = model.FirstName_register;
            EmailDetail["UserName"] = model.UserName;
            EmailDetail["Password"] = model.Password;

            mailTo.Add(model.Emails);
            //mailTo.Add("supawadee@prosoft.co.th");
            ViewBag.Data = EmailDetail;
            Detail = PartialViewToString("UC/Email/SendMailInformUserName");

            var mailFrom = res.Config.EmailNoReply;
            #endregion

            IsSend = OnSendByAlertEmail(subject, mailFrom, mailTo, mailCC, Detail);
        }
        #endregion

        #region Update Supplier Email
        public void UpdateContactEmail(int CompID, string Email)
        {

            if (CompID > 0)
            {
                svCompany = new CompanyService();
                svEmCompany = new emCompanyService();

                var Company = svCompany.SelectData<b2bCompany>("*", "IsDelete = 0 AND CompID =" + CompID).First();
                Company.ContactEmail = Email;
                Company = svCompany.SaveData<b2bCompany>(Company, "CompID");

                if (svCompany.IsResult)
                {
                    var emCompany = svEmCompany.SelectData<emCompany>("*", "IsDelete = 0 AND CompID = " + Company.emCompID).First();
                    emCompany.CompEmail = Email;
                    emCompany = svEmCompany.SaveData<emCompany>(emCompany, "CompID");
                }

            }
        }
        #endregion

        #region Update EmLead EmCompanyLead
        public void UpdateEmLead(FormCollection form)
        {
            svPurchase = new PurchaseService();

            var EmLead = svPurchase.SelectData<b2bEmLead>("*", "IsDelete = 0 AND EmLeadID =" + form["hidEmLeadID"]).First();
            EmLead.EmLeadName = form["txtLeadName"];
            EmLead.EmLeadKeyword = form["txtKeyword"];
            EmLead.Remark = form["txtRemark"];
            EmLead.LeadCount = DataManager.ConvertToShort(form["hidCountCompLead"]);
            EmLead = svPurchase.SaveData<b2bEmLead>(EmLead, "EmLeadID");

            if(svPurchase.IsResult)
            {
                
                #region Insert New EmCompLead
                if (!string.IsNullOrEmpty(form["hidAllSupplierCompID"]))
                {
                    int EmLeadID = EmLead.EmLeadID;
                    var supplierCompID = form["hidAllSupplierCompID"];
                    string[] subSupplierCompID = supplierCompID.Split(',');

                    for (int i = 0; i < subSupplierCompID.Length; i++)
                    {
                        var b2bEmCompanyLead = new b2bEmCompanyLead();
                        int SupplierCompID = DataManager.ConvertToInteger(subSupplierCompID[i]);

                        b2bEmCompanyLead.EmLeadID = EmLeadID;
                        b2bEmCompanyLead.CompID = SupplierCompID;
                        b2bEmCompanyLead.EmCompLeadCode = svPurchase.GenLeadCode(SupplierCompID, 3);
                        svPurchase.InsertEmCompanyLead(b2bEmCompanyLead);
                    }
                }
                #endregion
            }

        }
        #endregion

        #region ChangeStatus
        public void ChangeStatus(int id, bool status, string type)
        {
            if (id > 0)
            {
                svPurchase = new PurchaseService();

                var EmLead = svPurchase.SelectData<b2bEmLead>("*", "IsDelete = 0 AND EmLeadID =" + id).First();
                if (type == "Mark")
                {
                    EmLead.IsMark = status;
                }
                else
                {
                    EmLead.IsImportance = status;
                }
               
                EmLead = svPurchase.SaveData<b2bEmLead>(EmLead, "EmLeadID");
            }

        }
        #endregion

        #region DeleteEmlead
        public ActionResult DeleteEmlead(List<bool> Check, List<int> ID, List<short> RowVersion)
        {
            svPurchase = new PurchaseService();
            int CountAssignLead = 0;
            int CountEmLead = 0;

            svPurchase.DelData<b2bEmLead>(Check, ID, RowVersion, "EmLeadID");
            if (svPurchase.IsResult)
            {
                for (int i = 0; i < ID.Count();i++)
                {
                    if(Check[i] == true){
                        var countEmCompLead = svPurchase.CountData<b2bEmCompanyLead>("AssignLeadID,AssignLeadName", "IsDelete = 0 AND EmLeadID =" + ID[i]);
                        if (countEmCompLead > 0)
                        {
                            var EmCompLead = svPurchase.SelectData<b2bEmCompanyLead>("*", "IsDelete = 0 AND EmLeadID =" + ID[i]);
                            svPurchase.DeleteData<b2bEmCompanyLead>(EmCompLead, "EmCompLeadID", "");
                        }
                    }
                }

                CountAssignLead = svPurchase.CountData<b2bAssignLead>("AssignLeadID,AssignLeadName", "IsDelete = 0");
                CountEmLead = svPurchase.CountData<b2bEmLead>("EmLeadID,EmLeadName", "IsDelete = 0");

            }
            return Json(new
            {
                Result = svPurchase.IsResult,
                CountAssignLead = CountAssignLead,
                CountEmLead = CountEmLead
               
            });
            
        }
        #endregion

        #region DeleteEmCompLead
        public void DeleteEmCompLead(int EmCompLeadID)
        {
            svPurchase = new PurchaseService();

            var EmCompLead = svPurchase.SelectData<b2bEmCompanyLead>("*", "IsDelete = 0 AND EmCompLeadID =" + EmCompLeadID);
            svPurchase.DeleteData<b2bEmCompanyLead>(EmCompLead, "EmCompLeadID", "");
                        

        }
        #endregion

        #region DeleteAssignLead
        public ActionResult DeleteAssignLead(List<bool> Check, List<int> ID, List<short> RowVersion)
        {
            svPurchase = new PurchaseService();
            int CountAssignLead = 0;
            int CountEmLead = 0;

            svPurchase.DelData<b2bAssignLead>(Check, ID, RowVersion, "AssignLeadID");
            if (svPurchase.IsResult)
            {

                for (int i = 0; i < ID.Count(); i++)
                {
                    if (Check[i] == true)
                    {
                        var countCompLead = svPurchase.CountData<b2bCompanyLead>("AssignLeadID,AssignLeadName", "IsDelete = 0 AND AssignLeadID =" + ID[i]);
                        if (countCompLead > 0)
                        {
                            var CompLead = svPurchase.SelectData<b2bCompanyLead>("*", "IsDelete = 0 AND AssignLeadID =" + ID[i]);
                            svPurchase.DeleteData<b2bCompanyLead>(CompLead, "CompLead", "");
                        }
                    }

                    CountAssignLead = svPurchase.CountData<b2bAssignLead>("AssignLeadID,AssignLeadName", "IsDelete = 0");
                    CountEmLead = svPurchase.CountData<b2bEmLead>("EmLeadID,EmLeadName", "IsDelete = 0");
                }

            }
            return Json(new
            {
                Result = svPurchase.IsResult,
                CountAssignLead = CountAssignLead,
                CountEmLead = CountEmLead

            });

        }
        #endregion

        #region CountLead
        public void CountLead()
        {
            svPurchase = new PurchaseService();
            ViewBag.CountAssignLead = svPurchase.CountData<b2bAssignLead>("AssignLeadID,AssignLeadName", "IsDelete = 0");
            ViewBag.CountEmLead = svPurchase.CountData<b2bEmLead>("EmLeadID,EmLeadName", "IsDelete = 0");
        }
        #endregion

    }
}
