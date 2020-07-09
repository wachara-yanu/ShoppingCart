using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;
//using Prosoft.Base;
using Ouikum.Common;
using Prosoft.Service;
using Ouikum.Product;
using Ouikum.Category;
using res = Prosoft.Resource.Web.Ouikum;
using System.Globalization;
using Ouikum.BizType;

namespace Ouikum.Web.Controllers
{
    public class DefaultController : BaseController
    {
        CommonService svCommon = new Common.CommonService();
        // GET: /Default/
        #region Member
        AddressService svAddress;
        #endregion

        #region GetDistrict
        [HttpGet]
        public ActionResult TestFB()
        {
            var fb = new FacebookHelper();
            var model = new FacebookModel();
            model.Link = "http://www.ouikum.com/MyB2B/quotation/bidproduct/QO-ACB-130521-2";
            model.Picture = "https://ouikumstorage.blob.core.windows.net/upload/Product/3523/151289/CHANG-509.jpg";
            model.Message = "มีลูกค้าต้องการ $ชื่อสินค้า$ $จำนวน$ สำหรับ Suppliers ท่านไหนสนใจ สามารถเสนอราคา ติดต่อที่";
            var isResult = fb.PostFeed(model);
            return View();
        }

        [HttpGet]
        public ActionResult TestUserFB()
        { 
            try
            { 
                var data = svAddress.SelectData<emProvince>(" * ", "IsDeleted =");
            }
            catch (Exception ex)
            {
                CreateLogFiles(ex);
            }
            return View();
        }
        #endregion

        #region GetDistrict
        [HttpPost]
        public ActionResult GetDistrict(int id)
        {
            svAddress = new AddressService();
            string SQLWhere = "(it.IsDelete = 0) AND ( it.ProvinceID = " + id + ")";
            var Distrincts = svAddress.SelectData<emDistrict>("DistrictID,DistrictName,ProvinceID ", SQLWhere, "DistrictID", 1, 0);

            string htmlReturn = string.Empty;
            foreach (var d in Distrincts)
            {
                htmlReturn += String.Format("<option value=\"{0}\">{1}</option>", d.DistrictID, d.DistrictName);
            }
            return Content(htmlReturn);
        }
        #endregion

        #region GetProvince
        [HttpPost]
        public JsonResult GetProvince()
        {
            svAddress = new AddressService();
            string SQLWhere = "(it.IsDelete = 0)  ";
            var provinces = svAddress.SelectData<emProvince>("ProvinceID,ProvinceName", SQLWhere, "RegionID", 0, 0, false);

            return Json(provinces);
        }
        #endregion

        #region POST : Index
        [HttpPost]
        public ActionResult ContactUs(FormCollection form)
        {

            var IsSuccess = OnSendMailContactUs(form);


            return Json(new { IsSuccess = IsSuccess });
        }
        #endregion

        #region ErrorPages
        [HttpGet]
        public ActionResult ErrorPages()
        {
            GetStatusUser();
            ViewBag.EnumServiceType = svCommon.SelectEnum(CommonService.EnumType.SearchByServiceType);
            return View();
        }

        #endregion 
         
        public ActionResult ClearCaching()
        {
            var isResult = false;
            try
            {

                isResult = true;
            }
            catch (Exception ex)
            {
                CreateLogFiles(ex);
                isResult = false;
                throw;
            }

            return Json(new { IsResult = isResult }, JsonRequestBehavior.AllowGet);
        }
         
        public ActionResult Test()
        {
            var url = System.Web.HttpContext.Current.Request.Url.AbsoluteUri; 

             

            return View();
        }

        public ActionResult DownLoads(string dir,string name)
        {
            dir = dir.Replace("-", "/");
            BlobStorageService svBlob = new BlobStorageService();
            var container = svBlob.GetCloudBlobContainer();
            Response.AddHeader("Content-Disposition", "attachment; filename=" + name); // force download

            container.GetBlockBlobReference(dir+"/" + name).DownloadToStream(Response.OutputStream);
            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult GetFBGroup()
        {
            var fbHelper = new FacebookHelper();
            var data = fbHelper.FeedB2BThaiGroup();

            return Json(new {
                FeedGroup = data
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult TestBlob(string name)
        {
            BlobStorageService svBlob = new BlobStorageService();
            
            svBlob.DeleteBlobInDir(name);
            ViewBag.Path = "Success";
            ViewBag.Exist = true;

            return View();
        }

        [HttpPost]
        public ActionResult CheckExist(string name)
        {
            BlobStorageService svBlob = new BlobStorageService(); 
            ViewBag.Path = name;
            ViewBag.Exist =  svBlob.Exists(name); 


            return View("TestBlob");
        } 

        /* ส่งข้อความถึงแอดมิน */
        #region Suggestion
        [HttpPost]
        public ActionResult SendEmailToAdmin(FormCollection form,string Type, string Detail)
        {
            #region variable
            bool IsSend = true;
            var MsgDetail = "";
            var mailToAdmin = new List<string>();
            var mailCC = new List<string>();
            #endregion

            #region Set Content & Value For Send Email
            //Subject Content
            string Subject = res.Message_Center.lblMsgFromCustomer+" (" + form["fromName"] + " ," + form["fromEmail"] + ")";

            #region Message Detail

            //Buddhist Era datetime
            string today = System.DateTime.Now.ToShortDateString();
            string timenow = System.DateTime.Now.ToLongTimeString();
            string thisTime = timenow.Substring(0, timenow.Length - 3);
            //Detail Content
            switch(Type){
                case "SendSuggestion"://ข้อความถึงแอดมิน
                    MsgDetail = res.Common.lblFirstName + " : " + form["fromName"] + "<br>";
                    MsgDetail += res.Common.lblEmail+" : " + form["fromEmail"] + "<br><br>";
                    MsgDetail += res.Common.lblDetails+" : <br>" + form["Detail"] + "<br><br>";
                    MsgDetail += res.Message_Center.lblGiveScore1+res.Message_Center.lblGiveScore2+"<br>";
                    MsgDetail += "= " + form["Score"] + "<br><br>";
                    MsgDetail += "URL : " + form["CurrentUrl"] + "<br>";
                    MsgDetail += res.Message_Center.lblSendDate+" : " + today + "  " + thisTime + " น.";
                break;
                case "SendSurvey"://แบบสำรวจ
                    MsgDetail = res.Common.lblFirstName+" : " + form["fromName"] + "<br>";
                    MsgDetail += res.Common.lblEmail + " : " + form["fromEmail"] + "<br><br>";
                    MsgDetail += Detail;
                    break;
            }
            

            #endregion

            var mailFrom = res.Config.EmailNoReply;

            #endregion
            mailToAdmin = GetMailListB2BAdminSubport();
            IsSend = OnSendByAlertEmail(Subject, mailFrom, mailToAdmin, mailCC, MsgDetail);

            return Json(IsSend);

        }
        #endregion

        /*หา Categorylevel2*/
        #region PostSelectCategoryLV2
        [HttpPost]
        public ActionResult SelectCategoryLV2(int lv1)
        {
            string htmlReturn = String.Format("<option value=\"0\" selected=\"selected\">---"+res.Common.selectCatelv2+"---</option>");
            if (lv1 > 0)
            {
             
                var svCategory = new CategoryService(); 
                var data = svCategory.LoadSubCategory(lv1, 0, 2);
                foreach (var cate in data)
                {
                    //if(Base.AppLang == "en-US"){htmlReturn += String.Format("<option value=\"{0}\">{1}</option>", cate.CategoryID, cate.CategoryNameEng);}
                    //else { htmlReturn += String.Format("<option value=\"{0}\">{1}</option>", cate.CategoryID, cate.CategoryName); }
                    htmlReturn += String.Format("<option value=\"{0}\">{1}</option>", cate.CategoryID, cate.CategoryName); 
                }
            }

            return Content(htmlReturn);
        }
        #endregion

        /*หา Categorylevel3*/
        #region PostSelectCategoryLV3
        [HttpPost]
        public ActionResult SelectCategoryLV3(int lv2)
        {
            string htmlReturn = String.Format("<option value=\"0\" selected=\"selected\">---" + res.Common.selectCatelv3 + "---</option>");
            if (lv2 > 0)
            { 
                var svCategory = new CategoryService(); 
                var data = svCategory.LoadSubCategory(lv2, 0, 3); 

                foreach (var cate in data)
                {
                    //if(Base.AppLang == "en-US"){htmlReturn += String.Format("<option value=\"{0}\" class=\"{1}\" catecode=\"{2}\" catepath=\"{3}\">{1}</option>", cate.CategoryID, cate.CategoryNameEng, cate.CategoryCode, cate.ParentCategoryPath);}
                    //else{htmlReturn += String.Format("<option value=\"{0}\" class=\"{1}\" catecode=\"{2}\" catepath=\"{3}\">{1}</option>", cate.CategoryID, cate.CategoryName, cate.CategoryCode, cate.ParentCategoryPath);}
                    htmlReturn += String.Format("<option value=\"{0}\" class=\"{1}\" catecode=\"{2}\" catepath=\"{3}\">{1}</option>", cate.CategoryID, cate.CategoryName, cate.CategoryCode, cate.ParentCategoryPath);
                }
            }
            return Content(htmlReturn);
        }
        #endregion

        #region 
        
        
        public ActionResult NotFound()
        {
            GetStatusUser();
            ViewBag.EnumServiceType = svCommon.SelectEnum(CommonService.EnumType.SearchByServiceType);

            return View();
        }

        public ActionResult TansferCertifyImage(int CompID)
        {
            FileHelper filemanage = new FileHelper();
            var sqlWhere = "IsDelete = 0 AND CompID = " + CompID;
            var lst = new List<b2bCompanyCertify>();
            lst = svAddress.SelectData<b2bCompanyCertify>("*", sqlWhere);
            foreach (var item in lst) {
                filemanage.DirPath = "CompanyCertify/" + item.CompID + "/" + item.CompCertifyID;
                filemanage.DirTempPath = "CompanyCertify/" + item.CompID;
                filemanage.ImageName = item.CertifyImgPath;
                filemanage.FullHeight = 150;
                filemanage.FullWidth = 150;

                SaveFileImage(filemanage.DirTempPath, filemanage.DirPath, filemanage.ImageName);
            }
            return View();
        }
        //public void TansferImg(string DirTempPath,string DirPath, string imageName)
        //{
        //    FileHelper filemanage = new FileHelper();
        //    filemanage.DirPath = DirPath;
        //    filemanage.DirTempPath = DirTempPath;
        //    filemanage.ImageName = imageName;
        //    filemanage.FullHeight = 150;
        //    filemanage.FullWidth = 150;

        //    SaveFileImage(filemanage.DirTempPath, filemanage.DirPath, filemanage.ImageName);
        //}

        #endregion

        #region BindSaveLanguage
        [HttpPost]
        public JsonResult BindSaveLanguage(string Language)
        {
            var IsResult = false;
            try
            {
                IsResult = RemoveCookieCulture();
                IsResult = SaveCookieCulture(Language);
            }
            catch (Exception ex)
            {
                CreateLogFiles(ex);
            }

            #region Return Json
            return Json(new
            {
                IsResult = IsResult,
                MsgError = res.Common.btnCancel,
                MsgSuccess = res.Common.lblPleased_success
            });
            #endregion
        }
        #endregion

        #region CultureChange        
        //public ActionResult CultureChange(string culture, string returnUrl)
        //{
        //    try
        //    {
        //        Culture = new System.Globalization.CultureInfo(culture);
        //        CultureInfo myCulture = new CultureInfo(culture);
        //    }
        //    catch (Exception ex)
        //    {
        //        Culture = new System.Globalization.CultureInfo("th-TH");
        //    }

        //    returnUrl = returnUrl.Replace("%2f", "/");

        //    returnUrl = returnUrl.Replace("%2f", "/").Replace("/th", "").Replace("/en", "");

        //    string[] newLang = culture.Split('-');  // split en-US , th-TH , etc  with dash (-)

        //    if (newLang[0] != ApplicationManager.DefaultLanguage)
        //        returnUrl = "~/" + newLang[0] + returnUrl;

        //    if (returnUrl.EndsWith("/") && !returnUrl.Equals("/"))
        //        returnUrl = returnUrl.Substring(0, returnUrl.Length - 1);

        //    if (string.IsNullOrEmpty(returnUrl))
        //        returnUrl = "/";

        //    return Redirect(HttpUtility.UrlDecode(returnUrl));
        //}
        #endregion

    }
}
