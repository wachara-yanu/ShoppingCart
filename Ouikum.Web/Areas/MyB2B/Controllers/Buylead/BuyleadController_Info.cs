using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Prosoft.Service;
//using Prosoft.Base;
using Ouikum.Buylead;
using Ouikum;
using Ouikum.Category;
using Ouikum.Company;
using System.Globalization;

namespace Ouikum.Web.MyB2B
{
    public partial class BuyleadController : BaseSecurityController
    {
        #region PrepareAndPostBuylead(Add)
        /*
        #region PrepareAddBuylead
        [HttpPost]
        public ActionResult PrepareAddBuylead(string GenCode)
        {
            ViewBag.QtyUnits = DoLoadQtyUnits();
            ViewBag.BuyleadCode = GenCode;
            ViewBag.AddProduct = true;
            return PartialView("UC/AddBuylead");
        }
        #endregion
         * 

        #region PostAddBuylead
        [HttpPost, ValidateInput(false)]
        public ActionResult AddBuylead
        (
        #region Param
            string BuyleadName,
            string BuyleadCode,
            int BuyleadType,
            string BuyleadExpire,
            string Keyword,
            string FullDetail,
            string Qty,
            string QtyUnit,
            string Catecode,
            string CateLV3,
            List<string> BuyleadImgPath
        )
        #endregion)
        {
            var svBuylead = new BuyleadService();
            b2bBuylead model = new b2bBuylead();
            try
            {
                #region Set Buylead Model
                model.BuyleadName = BuyleadName;
                model.BuyleadCode = BuyleadCode;
                model.BuyleadType = BuyleadType;
                model.BuyleadExpDate = ConvertStringToDateTime(BuyleadExpire);
                model.BuyleadKeyword = Keyword;
                model.BuyleadDetail = FullDetail;
                model.Qty = DataManager.ConvertToDecimal(Qty);
                model.QtyUnit = QtyUnit;
                if (BuyleadImgPath != null)
                {
                    model.BuyleadIMGPath = BuyleadImgPath[0];
                }
                model.CompID = LogonCompID;
                model.CateLV1 = Convert.ToInt32(Catecode.Substring(9, 4));
                model.CateLV2 = Convert.ToInt32(Catecode.Substring(14, 4));
                model.CateLV3 = Convert.ToInt32(CateLV3);
                model.IsShow = true;
                model.IsJunk = false;

                #region Check ว่าเป็น OutSource Add หรือ User Add
                if (LogonServiceType >= 9)
                {
                    model.RowFlag = (short)2;
                    model.CompCode = LogonCompCode;
                }
                else
                    model.RowFlag = (short)6;
                #endregion

                model.ContactCount = 0; 
                model.ListNo = 0;
                model.ViewCount = 0;
                model.CreatedDate = DateTime.Now;
                model.ModifiedDate = DateTime.Now;
                model.CreatedBy = LogonCompCode;
                model.ModifiedBy = LogonCompCode;

                #region Company Information
                #region query Company
                var svCompany = new CompanyService();
                sqlSelect = "*";
                sqlWhere += "CompID="+LogonCompID;
                var QueryComp = svCompany.SelectData<view_Company>(sqlSelect, sqlWhere, "", 1, 1,false).First();

                model.BuyleadCompanyName = LogonCompName;
                model.BuyleadContactPerson = QueryComp.ContactFirstName + "  " + QueryComp.ContactLastName;
                model.BuyleadContactPosition = QueryComp.ContactPositionName;
                model.BuyleadTel = QueryComp.CompPhone;
                model.BuyleadEmail = QueryComp.ContactEmail;
                model.BuyleadMobilePhone = QueryComp.CompMobile;
                model.BuyleadFax = QueryComp.CompFax;
                model.BuyleadAddressLine1 = QueryComp.CompAddrLine1 + " " + QueryComp.CompSubDistrict;
                model.BuyleadSubDistrict = QueryComp.CompSubDistrict;
                model.DistrictID = QueryComp.CompDistrictID;
                model.ProvinceID = QueryComp.CompProvinceID;
                model.BuyleadPostelCode = QueryComp.CompPostalCode;
                #endregion

                #endregion 

                #endregion

                #region Insert Buylead And BuyleadImage
                svBuylead.InsertBuylead(model, LogonCompID);
                #endregion

                #region Save Image Files
                if (BuyleadImgPath != null)
                {
                    imgManager = new FileHelper();
                    imgManager.DirPath = "Buylead/" + LogonCompID + "/" + model.BuyleadID;
                    imgManager.DirTempPath = "Temp/Buylead/" + LogonCompID;
                    SaveFileImage(
                        imgManager.DirTempPath,
                        imgManager.DirPath,
                        BuyleadImgPath, 150, 450);
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
        */
        #endregion

        #region PrepareAndPostBuylead(Edit)
		 //ไม่ใช้แล้ว ไปเปิด New Tab ไป BuyleadCenter/Index
	   
        /*
        #region PrepareEditByID
        [HttpPost]
        public ActionResult PrepareEditByID(int ID, string GenCode)
        {
            var svBuylead = new BuyleadService();
            var svCategory = new CategoryService();

            ViewBag.QtyUnits = DoLoadQtyUnits();

            var Buylead = svBuylead.SelectData<b2bBuylead>(" * ", "BuyleadID=" + ID, null, 1, 1).First();
            ViewBag.Buylead = Buylead;

            ViewBag.Catename = svCategory.SearchCategoryByID((int)Buylead.CateLV3);
            ViewBag.BuyleadCode = GenCode;
            return PartialView("UC/EditBuylead");
        }
        #endregion


        #region PostEditBuyleadByID
        [HttpPost, ValidateInput(false)]
        public ActionResult EditBuyleadByID
        (
        #region Param
int BuyleadID,
            string BuyleadName,
            string BuyleadCode,
            int BuyleadType,
            string BuyleadExpire,
            string Keyword,
            string FullDetail,
            string Qty,
            string QtyUnit,
            string Catecode,
            string CateLV3,
            List<string> BuyLeadImgOldfile,
            List<string> BuyleadImgPath
        #endregion
)
        {

            var svBuylead = new BuyleadService();

            b2bBuylead model = new b2bBuylead();

            if (LogonCompID > 0)
            {
                try
                {
                    #region Set Model
                    model.BuyleadID = BuyleadID;
                    model.BuyleadName = BuyleadName;
                    model.BuyleadType = BuyleadType;
                    model.BuyleadCode = BuyleadCode;
                    model.BuyleadExpDate = DateTime.Parse(BuyleadExpire);
                    model.BuyleadKeyword = Keyword;
                    model.BuyleadDetail = FullDetail;
                    model.BuyleadIMGPath = BuyleadImgPath[0];
                    model.Qty = decimal.Parse(Qty);
                    model.QtyUnit = QtyUnit;
                    model.ModifiedBy = LogonCompCode;

                    if (Catecode != null)
                    {
                        model.CateLV1 = Convert.ToInt32(Catecode.Substring(9, 4));
                        model.CateLV2 = Convert.ToInt32(Catecode.Substring(14, 4));
                    }
                    model.CateLV3 = Convert.ToInt32(CateLV3);

                    #region OurSource Update or Member Update
                    if (LogonServiceType >= 9)
                    {
                        model.CompCode = LogonCompCode;
                        model.RowFlag = 2;
                    }
                    else
                    {
                        model.CompCode = LogonCompCode;
                        model.RowFlag = 6;
                    }
                    #endregion

                    #endregion

                    #region Update Data
                    svBuylead.UpdateBuylead(model);
                    #endregion

                    #region Check And Update Files
                    if (svBuylead.IsResult)
                    {
                        imgManager = new FileHelper();
                        imgManager.DirPath = "Buylead/" + LogonCompID + "/" + BuyleadID;
                        imgManager.DirTempPath = "Temp/Buylead/" + LogonCompID;
                        SaveFileImage(imgManager.DirTempPath, imgManager.DirPath, BuyleadImgPath, 150, 450);

                        imgManager.DirPath = "Buylead/" + LogonCompID + "/" + BuyleadID;
                        if (BuyLeadImgOldfile != null)
                        {
                            DeleteFileImage(imgManager.DirPath, BuyLeadImgOldfile, BuyleadImgPath);
                        }

                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    svBuylead.MsgError.Add(ex);
                    CreateLogFiles(ex);
                }
            }

            return Json(new { IsResult = svBuylead.IsResult, MsgError = GenerateMsgError(svBuylead.MsgError), ID = model.BuyleadID });
        }

        #endregion
         */
        #endregion
    }
}
