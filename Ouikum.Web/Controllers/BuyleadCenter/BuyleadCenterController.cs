using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Prosoft.Service;
using Ouikum.Buylead;
using Ouikum.Category;
using Ouikum.Company;
using Ouikum;
using Ouikum.Common;
using Ouikum.BizType;
using res = Prosoft.Resource.Web.Ouikum;

namespace Ouikum.Web.Controllers
{
    public partial class BuyleadCenterController : BaseController
    {

        public string sqlwherein;
        // GET: /Buylead/
        BuyleadService svBuylead;
        CategoryService svCategory;
        CompanyService svCompany;
        BizTypeService svBizType;
        AddressService svAddress;
        CommonService svCommon = new CommonService();
        public BuyleadCenterController()
        {
            svBuylead = new BuyleadService();
            svCategory = new CategoryService();
            svBizType = new BizTypeService();
            svCompany = new CompanyService();
            svAddress = new AddressService();
        }
        
            
        #region GenCode
        private string GenCode(int Comp)
        { 
            System.Random random = new System.Random();
            string Code = "";
            if (Comp > 0)
            {
                if (Comp > 0 && Comp < 10)
                {
                    Code += Convert.ToString(random.Next(1, 9999999)) +Convert.ToString(Comp);
                }
                else if (Comp >= 10 && Comp < 100)
                {
                    Code += Convert.ToString(random.Next(1, 999999)) + Convert.ToString(Comp);
                }
                else if (Comp >= 100 && Comp < 1000)
                {
                    Code += Convert.ToString(random.Next(1, 99999)) + Convert.ToString(Comp);
                }
                else if (Comp >= 1000 && Comp < 10000)
                {
                    Code += Convert.ToString(random.Next(1, 9999)) + Convert.ToString(Comp);
                }
                else if (Comp >= 10000 && Comp < 100000)
                {
                    Code +=Convert.ToString(random.Next(1, 999))+ Convert.ToString(Comp);
                }
            }
            else {
                Code = Convert.ToString(random.Next(1, 99999999));
            }
            return Code;
        }
        #endregion

        #region GetIndex
        public ActionResult Index()
        {
            if (RedirectToProduction())
                return Redirect(UrlProduction);

            GetStatusUser();
            ViewBag.EnumServiceType = svCommon.SelectEnum(CommonService.EnumType.SearchByServiceType);
            return View();
        }
        #endregion

        #region GetChannel2
        public ActionResult Channel2(string ID, string Comp)
        {
            if (RedirectToProduction())
                return Redirect(UrlProduction); 

            GetStatusUser();
            ViewBag.EnumServiceType = svCommon.SelectEnum(CommonService.EnumType.SearchByServiceType);
            ViewBag.QtyUnits = svCommon.SelectEnum(CommonService.EnumType.QtyUnits);
            string SqlSelect, SqlWhere = "";
            
            #region Prepare Edit Buylead

            int BuyleadID = Convert.ToInt32(ID);
            int CompID = Convert.ToInt32(Comp);
            if (LogonCompID > 0)
            {
                if (BuyleadID > 0)
                {
                    if (CompID != LogonCompID)
                        return Redirect(res.Pageviews.PvMemberSignIn);
                    var Buylead = svBuylead.SelectData<b2bBuylead>(" * ", "BuyleadID=" + ID, null, 1, 1).First();
                    ViewBag.Buylead = Buylead;
                    ViewBag.BuyleadCode = Buylead.BuyleadCode;
                    ViewBag.Catename = svCategory.SearchCategoryByID((int)Buylead.CateLV3);
                }
                else
                {
                    #region query Company
                    var svCompany = new CompanyService();
                    var QueryComp = svCompany.SelectData<view_Company>("*", "CompID=" + LogonCompID, "", 1, 1, false).First();
                    ViewBag.QueryComp = QueryComp;
                    ViewBag.BuyleadCode = GenCode(LogonCompID);
                    #endregion

                }
            }
            else {
                if (BuyleadID > 0)
                {
                    RememberURL();
                    if (!CheckIsLogin())
                        return Redirect(res.Pageviews.PvMemberSignIn);
                }
                else {
                    ViewBag.BuyleadCode = GenCode(0);
                }
            }
            #endregion
            //switch (res.Common.lblWebsite)
            //{
            //    case "B2BThai": sqlwherein = " AND CategoryType IN (1,2)"; break;
            //    case "AntCart": sqlwherein = " AND CategoryType IN (3)"; break;
            //    case "myOtopThai": sqlwherein = " AND CategoryType IN (5)"; break;
            //    case "AppstoreThai": sqlwherein = " AND CategoryType IN (6)"; break;
            //    default: sqlwherein = ""; break;
            //}
            #region Category 

            var cates1 = svCategory.GetCategoryByLevel(1);
            var cates2 = svCategory.GetCategoryByLevel(2);
            var cates3 = svCategory.GetCategoryByLevel(3);
            ViewBag.Category = cates1;
            #endregion

            #region query District And Province
            ViewBag.QueryDistrict = svAddress.ListDistrictByProvinceID(0);
            ViewBag.QueryProvince = svAddress.GetProvinceAll();
            #endregion

            #region FindCateAll  
            ViewBag.SelectCateLV1 = cates1;
            ViewBag.SelectCateLV2 = cates2;
            ViewBag.SelectCateLV3 = cates3;
            #endregion

            #region query Buylead

            #region Set ViewBag
            ViewBag.BuyleadID = BuyleadID;
            ViewBag.CompID = CompID;
            ViewBag.CateID = 0;
            ViewBag.ProvinceID = 0;
            ViewBag.CreatedDate = 0;
            ViewBag.BuyleadExpDate = 0;
            ViewBag.BuyleadType = 0;
            ViewBag.PageIndex = 1;
            ViewBag.PageSize = 20;
            #endregion
            #endregion

            return View();
        }
        #endregion

        #region PostAddBuylead
        [HttpPost, ValidateInput(false)]
        public ActionResult AddBuylead(FormCollection form, List<string> BuyleadImgPath, List<string> BuyLeadImgOldfile)
        {
            var svBuylead = new BuyleadService();
            b2bBuylead model = new b2bBuylead();
            try
            {
                #region Set Buylead Model
                var date = form["BuyleadExpire"];
                model.BuyleadName = form["BuyleadName"];
                model.BuyleadCode = form["BuyleadCode"];
                model.BuyleadType = DataManager.ConvertToInteger(form["BuyleadType"]);
                //model.BuyleadExpDate = DateTime.Parse(form["BuyleadExpire"]);
                var d = form["BuyleadExpire"];
                DateTime t = DataManager.ConvertToDateTime(d);
                model.BuyleadExpDate = t;
                model.BuyleadKeyword = form["BuyleadKeyword"];
                if (form["BuyleadDetail"] != "")
                {
                    model.BuyleadDetail = form["BuyleadDetail"];
                }
                
                model.Qty = DataManager.ConvertToDecimal(form["Qty"]);
                model.QtyUnit = form["QtyUnit"];
                if (BuyleadImgPath != null)
                {
                    model.BuyleadIMGPath = BuyleadImgPath[0];
                }
                model.CompID = LogonCompID;
                model.CateLV1 = Convert.ToInt32(form["Catecode"].Substring(9, 4));
                model.CateLV2 = Convert.ToInt32(form["Catecode"].Substring(14, 4));
                model.CateLV3 = Convert.ToInt32(form["CateLV3"]);
                model.IsShow = true;
                model.IsJunk = false;


                #region Check ว่าเป็น OutSource Add หรือ User Add
                model.RowFlag = (LogonServiceType >= 9) ? (short)2 : (short)6;
                #endregion

                if (LogonServiceType >= 9)
                {
                    model.CompCode = LogonCompCode;
                }

                #region Company Information
                model.BuyleadCompanyName = form["CompName"];
                model.BuyleadContactPerson = form["ContactName"]; 
                model.BuyleadContactPosition = form["Position"];
                model.BuyleadTel = form["Phone"]; 
                model.BuyleadEmail = form["Email"];
                if (form["Mobile"] != "")
                {
                    model.BuyleadMobilePhone = form["Mobile"];
                }
                if (form["Fax"] != "")
                {
                    model.BuyleadFax = form["Fax"];
                }
                if (form["Address"] != "")
                {
                    model.BuyleadAddressLine1 = form["Address"];
                }
                model.DistrictID = Convert.ToInt32(form["District"]);
                model.ProvinceID = Convert.ToInt32(form["Province"]);
                if (form["Postal"] != "")
                {
                    model.BuyleadPostelCode = form["Postal"];
                }
                #endregion

                #endregion
                model.ContactCount = 0;
                model.ListNo = 0;
                model.ViewCount = 0;
                model.CreatedDate = DateTime.Now;
                model.ModifiedDate = DateTime.Now;
                model.CreatedBy = "sa";
                model.ModifiedBy = "sa";

                if (form["BuyleadID"]==null)
                {
                    #region Insert Buylead And BuyleadImage
                    svBuylead.InsertBuylead(model, LogonCompID);
                    #endregion
                }
                else {
                    #region Update Data
                    if (Convert.ToInt32(form["RowFlag"]) == 3)
                    {
                        model.RowFlag = 2;
                    }
                    model.BuyleadID = Convert.ToInt32(form["BuyleadID"]);
                    svBuylead.UpdateBuylead(model);
                    #endregion
                }
                var Floder = 0;
                if (LogonCompID > 0)
                {
                    Floder = LogonCompID;
                }
                else {
                    Floder = 2;
                }
                #region Save Image Files
                if (BuyleadImgPath != null)
                {
                        imgManager = new FileHelper();
                        imgManager.DirPath = "Buylead/" + Floder + "/" + model.BuyleadID;
                        imgManager.DirTempPath = "Temp/Buylead/" + Floder;
                        SaveFileImage(
                            imgManager.DirTempPath,
                            imgManager.DirPath,
                            BuyleadImgPath, 150, 450);
                        if (BuyLeadImgOldfile!=null && (BuyleadImgPath[0] != BuyLeadImgOldfile[0]))
                        {
                            DeleteFileImage(imgManager.DirPath, BuyLeadImgOldfile, BuyleadImgPath);
                        }
                } 
                
                #endregion
            }
            catch (Exception ex)
            {
                CreateLogFiles(ex);
            }

            return Json(new { IsResult = svBuylead.IsResult, MsgError = GenerateMsgError(svBuylead.MsgError), ID = model.BuyleadID });
        }
        #endregion

        #region PostSearchCategory
        [HttpPost]
        public ActionResult SearchCategory(string CategoryName)
        {
            if (!string.IsNullOrEmpty(CategoryName))
            {
                var svCategory = new CategoryService();
                //switch (res.Common.lblWebsite)
                //{
                //    case "B2BThai": sqlwherein = " AND CategoryType IN (1,2)"; break;
                //    case "AntCart": sqlwherein = " AND CategoryType IN (3)"; break;
                //    case "myOtopThai": sqlwherein = " AND CategoryType IN (5)"; break;
                //    case "AppstoreThai": sqlwherein = " AND CategoryType IN (6)"; break;
                //    default: sqlwherein = ""; break;
                //}
                var data = svCategory.SearchCategoryByName(CategoryName, sqlwherein);
                ViewBag.Categories = data;
                ViewBag.Countcategory = svCategory.TotalRow;
            }
            return PartialView("UC/CategoryList");
        }

        #endregion 

        #region ValidateAddBuylead
        public ActionResult ValidateAddBuylead(string BuyleadName, string BuyleadCode, string BuyleadExpire)
        {
            var svBuylead = new BuyleadService();
            svBuylead.ValidateInsert(BuyleadName, BuyleadCode, BuyleadExpire);
            return Json(svBuylead.IsResult);
        }

        #endregion

        #region ValidateAddBuyleadExpire
        public ActionResult ValidateAddBuyleadExpire(string BuyleadName, string BuyleadCode, string BuyleadExpire)
        {
            bool IsResult = false;
            if (!string.IsNullOrEmpty(BuyleadExpire))
            {
                var dateNow = DateTime.Today.ToString("yyyyMMdd", new System.Globalization.CultureInfo("en-US"));
                int Exp = Convert.ToInt32(BuyleadExpire);
                int Now = Convert.ToInt32(dateNow);
                if (Now >= Exp)
                    IsResult = false;
                else
                    IsResult = true;
            }
            return Json(new { IsResult = IsResult});
        }

        #endregion

        #region ValidateEditBuylead
        public ActionResult ValidateEditBuylead(string Upd_BuyleadName, string Chk_BuyleadName, string Upd_BuyleadCode, string Chk_BuyleadCode, string BuyleadExpire, int BuyleadID)
        {
            var svBuylead = new BuyleadService();
            svBuylead.ValidateUpdate(Upd_BuyleadName, Chk_BuyleadName, Upd_BuyleadCode, Chk_BuyleadCode, BuyleadExpire, BuyleadID);
            return Json(svBuylead.IsResult);
        }

        #endregion


    }
}
