using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ouikum;
using Ouikum.Article;
using Prosoft.Service;
using res = Prosoft.Resource.Web.Ouikum;
using Ouikum.Banner;
using System.Collections;

namespace Ouikum.Web.Admin
{
    public class BannerController : BaseController
    {
        //
        // GET: /Admin/Banner/
        BannerService svBanner;
        

        public ActionResult Index()
        {
            var svBanner = new BannerService();
            RememberURL();
            SelectList_PageSize();
            if (!CheckIsLogin())
            {
                return Redirect(res.Pageviews.PvMemberSignIn);
            }
            else
            {
                GetStatusUser();
                SetPager();
                var Banner = svBanner.SelectData<b2bBanner>("*", "IsDelete = 0 AND WebID = 1");
                ViewBag.Banner = Banner;
                ViewBag.TotalPage = svBanner.TotalPage;
                ViewBag.TotalRow = svBanner.TotalRow;

                return View();
            }
        }

        [HttpPost]
        public ActionResult BannerList(FormCollection form)
        {
            var svBanner = new BannerService();
            string SQLWhere = "IsDelete = 0 AND WebID = 1";
            SelectList_PageSize();
            SetPager(form);
            SetBannerPager(form);

            if (!string.IsNullOrEmpty(ViewBag.Period)){
                SQLWhere += SQLWhereDateTimeFromPeriod(ViewBag.Period, "ModifiedDate");
            }

            if (!string.IsNullOrEmpty(form["SearchText"]))
            {
                SQLWhere += " AND BannerTitle LIKE N'%" + form["SearchText"].Trim() + "%'";
            }
            else
            {
                GetStatusUser();
                SetPager(form);
                var Banner = svBanner.SelectData<b2bBanner>("*", "IsDelete = 0 AND WebID = 1", "ListNo ASC", (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
                ViewBag.Banner = Banner;
                ViewBag.TotalPage = svBanner.TotalPage;
                ViewBag.TotalRow = svBanner.TotalRow;
            }
            var banner = svBanner.SelectData<b2bBanner>("*", SQLWhere, "ListNo ASC", (int)ViewBag.PageIndex, (int)ViewBag.PageSize);
            ViewBag.TotalPage = svBanner.TotalPage;
            ViewBag.TotalRow = svBanner.TotalRow;
            ViewBag.Banner = banner;

            return PartialView("UC/BannerGrid");
        }
        public ActionResult AddBanner()
        {
            var svBanner = new BannerService();
            List<b2bBanner> bannerAll = new List<b2bBanner>();
            RememberURL();
            if (!CheckIsLogin())
            {
                return Redirect(res.Pageviews.PvMemberSignIn);
            }
            else
            {
                GetStatusUser();
                SetPager();
                bannerAll = svBanner.SelectData<b2bBanner>("BannerID, ListNo", "ListNo != 0 AND WebID = 1");
                ViewBag.Count = bannerAll.Count()+1;

                return View();
            }
        }


        #region Post : Add
        [HttpPost, ValidateInput(false)]
        public ActionResult AddBanner(FormCollection form)
        {
            var svBanner = new BannerService();
            var banner = new b2bBanner();
            var BannerImgPath = banner.BannerImgPath;
            List<b2bBanner> bannerAll = new List<b2bBanner>();
            List<int> listID = new List<int>();
            List<int> listListNo = new List<int>();
            int listnoNew = Convert.ToInt32(form["ListNo"]);
            var count = svBanner.SelectData<b2bBanner>(" * ", " CreatedDate = GetDate() AND RowFlag > 0 AND WebID = 1");
            int Count = count.Count + 1;
            string Code = AutoGenCode("B", Count);

            bannerAll = svBanner.SelectData<b2bBanner>("BannerID, ListNo", "ListNo >= " + listnoNew + " AND WebID = 1");
            if (bannerAll.Count() != 0)
            {
                for (var i = 0; i < bannerAll.Count(); i++)
                {
                    listID.Add(bannerAll[i].BannerID);
                    listListNo.Add(bannerAll[i].ListNo + 1);
                }
                svBanner.UpdateBannerListNo(listID, listListNo);
            }

            var IsShow = false;
            if (form["isShow"] == "1")
            {
                IsShow = true;
            }
            else
            {
                IsShow = false;
            }

            banner.BannerCode = Code;
            banner.BannerTitle = form["Title"];
            banner.BannerLink = form["Link"];
            banner.BannerImgPath = form["ImgPath"];
            banner.WebID = 1;
            banner.PagePosition = "H1";
            banner.IsShow = IsShow;
            banner.ListNo = Convert.ToInt32(form["ListNo"]);
            banner.Rowflag = "1";
            banner.CreatedDate = DateTime.Now;
            banner.CreatedBy = LogonCompName;
            banner.IsDelete = false;
            banner.ModifiedDate = DateTime.Now;
            banner.ModifiedBy = LogonCompName;
            banner.RowVersion = 1;

            #region Save b2bBanner
            svBanner.SaveData<b2bBanner>(banner, "BannerID");
            #endregion

            #region SaveBannerImg
            if (svBanner.IsResult && svBanner.IsResult)
            {
                if (!string.IsNullOrEmpty(form["ImgPath"]))
                {
                    if (banner.BannerImgPath != BannerImgPath)
                    {
                        imgManager = new FileHelper();

                        imgManager.DirPath = "Banner/H1/" + banner.BannerID;
                        imgManager.DirTempPath = "Temp/Banner/" + "BannerHome";
                        imgManager.ImageName = form["ImgPath"];
                        imgManager.FullHeight = 0;
                        imgManager.FullWidth = 0;
                        imgManager.ThumbHeight = 150;
                        imgManager.ThumbWidth = 150;

                        imgManager.SaveImageFromTemp();
                    }
                }
            }
            #endregion
            
            return Redirect("~/Admin/Banner");
        }
        #endregion

        public ActionResult EditBanner(string BannerId)
        {
            var svBanner = new BannerService();
            var banner = new b2bBanner();
            List<b2bBanner> bannerAll = new List<b2bBanner>();
            RememberURL();
            if (!CheckIsLogin())
            {
                return Redirect(res.Pageviews.PvMemberSignIn);
            }
            else
            {
                GetStatusUser();
                SetPager();
                banner = svBanner.SelectData<b2bBanner>(" * ", "IsDelete = 0 AND WebID = 1 And BannerID =" + BannerId).First();
                bannerAll = svBanner.SelectData<b2bBanner>("BannerID, ListNo", "ListNo != 0 AND WebID = 1");
                ViewBag.Count = bannerAll.Count();
                ViewBag.data = banner;
            }
            return View();
        }

        #region Post : Edit
        [HttpPost, ValidateInput(false)]
        public ActionResult EditBanner(FormCollection form)
        {
            var svBanner = new BannerService();
            var banner = new b2bBanner();
            List<b2bBanner> bannerAll = new List<b2bBanner>();
            List<int> listID = new List<int>();
            List<int> listListNo = new List<int>();
            int listnoNew = Convert.ToInt32(form["ListNo"]);

            banner = svBanner.SelectData<b2bBanner>("BannerID, ListNo,BannerImgPath", " BannerID = " + form["BannerID"] + " AND WebID = 1").First();
            
            if (banner.ListNo > listnoNew)
            {
                bannerAll = svBanner.SelectData<b2bBanner>("BannerID, ListNo", "ListNo BETWEEN " + listnoNew + " AND " + (banner.ListNo - 1) + " AND WebID = 1");
                for (var i = 0; i < bannerAll.Count(); i++)
                {
                    listID.Add(bannerAll[i].BannerID);
                    listListNo.Add(bannerAll[i].ListNo+1 );
                }
                svBanner.UpdateBannerListNo(listID, listListNo);
            }
            else
            {
                bannerAll = svBanner.SelectData<b2bBanner>("BannerID, ListNo", "ListNo BETWEEN " + (banner.ListNo + 1) + " AND " + listnoNew + " AND WebID = 1");
                for (var i = 0; i < bannerAll.Count(); i++)
                {
                    listID.Add(bannerAll[i].BannerID);
                    listListNo.Add(bannerAll[i].ListNo-1);
                }
                svBanner.UpdateBannerListNo(listID, listListNo);
            }

            var sqlUpdate = "";
            var RowVersion = DataManager.ConvertToShort(Convert.ToInt32(form["RowVersion"]) + 1);
            sqlUpdate = "BannerTitle = N'" + form["Title"] + "' , BannerLink = N'" + form["Link"] + "' , IsShow = " + form["isShow"] + ", ListNo = " + Convert.ToInt32(form["ListNo"])
                + ", ModifiedDate = '" + DateTime.Now + "' , ModifiedBy = '" + LogonCompName + "' , RowVersion = " + RowVersion;
            if (banner.BannerImgPath != form["ImgPath"])
            {
                sqlUpdate += ", BannerImgPath = '" + form["ImgPath"] + "'";
            }

            try{
                svBanner.UpdateByCondition<b2bBanner>(sqlUpdate, " BannerID = " + banner.BannerID);
            }
            catch (Exception ex)
            {
                CreateLogFiles(ex);
            }

            /*  New Img */
            if (svBanner.IsResult == true)
            {
                if (!string.IsNullOrEmpty(form["ImgPath"]))
                {
                    if (banner.BannerImgPath != form["ImgPath"])
                    {
                        imgManager = new FileHelper();
                        imgManager.DirPath = "Banner/H1/" + banner.BannerID;
                        imgManager.DirTempPath = "Temp/Banner/" + "BannerHome";
                        imgManager.ImageName = form["ImgPath"];
                        imgManager.FullHeight = 0;
                        imgManager.FullWidth = 0;
                        imgManager.ThumbHeight = 150;
                        imgManager.ThumbWidth = 150;

                        imgManager.SaveImageFromTemp();
                    }
                }
            }
            return Redirect("~/Admin/Banner");
        }
        #endregion

        #region ChangeIsShow
        [HttpPost]
        public bool ChangeIsShow(int bannerId, int istrust)
        {
            BannerService svBanner = new BannerService();
            bool IsResult = false;
            try{
                IsResult = svBanner.UpdateByCondition<b2bBanner>("IsShow = " + istrust + "", "BannerID = " + bannerId);
            }
            catch (Exception ex)
            {
                CreateLogFiles(ex);
            }

            return IsResult;
        }
        #endregion

        #region DelData
        public ActionResult DelData(List<bool> Check, List<int> ID, List<short> RowVersion, string PrimaryKeyName)
        {
            BannerService svBanner = new BannerService();
            var banner = new b2bBanner();
            List<b2bBanner> bannerAll = new List<b2bBanner>();
            List<int> listID = new List<int>();
            List<int> listListNo = new List<int>();

            if (PrimaryKeyName == "BannerId")
            {
                if (Check.Count() == 1)
                {
                    banner = svBanner.SelectData<b2bBanner>("BannerID, ListNo", " BannerID = " + ID[0] + " AND WebID = 1").First();

                    try
                    {
                        svBanner.DelData<b2bBanner>(Check, ID, RowVersion, PrimaryKeyName);
                    }
                    catch (Exception ex)
                    {
                        CreateLogFiles(ex);
                    }

                    bannerAll = svBanner.SelectData<b2bBanner>("BannerID, ListNo", "ListNo > " + banner.ListNo + " AND WebID = 1");
                    for (var i = 0; i < bannerAll.Count(); i++)
                    {
                        listID.Add(bannerAll[i].BannerID);
                        listListNo.Add(bannerAll[i].ListNo - 1);
                    }
                    svBanner.UpdateBannerListNo(listID, listListNo);
                }
                else
                {
                    //for (var a = 0; a < Check.Count(); a++)
                    //{
                    var num = 1;
                        for (var i = 0; i < ID.Count(); i++)
                        {
                            if (Check[i] == true)
                            {
                                BannerService svBanner1 = new BannerService();
                                banner = svBanner1.SelectData<b2bBanner>("BannerID, ListNo", " BannerID = " + ID[i] + " AND WebID = 1").First();
                                svBanner.UpdateByCondition<b2bBanner>("ListNo = 0 , IsDelete = 1", "BannerID = " + ID[i]);
                                //svBanner.DelData<b2bBanner>(Check, ID, RowVersion, PrimaryKeyName);

                                bannerAll = svBanner.SelectData<b2bBanner>("BannerID, ListNo", "ListNo > " + banner.ListNo + " AND WebID = 1");
                                for (var j = 0; j < bannerAll.Count(); j++)
                                {
                                    listID.Add(bannerAll[j].BannerID);
                                    listListNo.Add(bannerAll[j].ListNo - num);
                                }
                                svBanner.UpdateBannerListNo(listID, listListNo);
                                listID.Clear();
                                listListNo.Clear();
                                num++;
                            }
                        }
                        //break;
                    //}
                }
            }
            if (svBanner.IsResult)
            {
                return Json(new { Result = true });
            }
            else
            {
                return Json(new { Result = false });
            }
        }
        #endregion

        [HttpPost]
        public ActionResult ChangeListNoBanner(List<int> id)
        {
            if (!CheckIsLogin())
                return Redirect(res.Pageviews.PvMemberSignIn);
            var svBanner = new BannerService();
            var banner = new b2bBanner();
            try
            {
                List<int> list = new List<int>();
                var id_banner1 = id[1];
                var id_banner2 = id[0];
                banner = svBanner.SelectData<b2bBanner>("*", " BannerID = " + id_banner1 + " AND WebID = 1").First();
                list.Add(banner.ListNo);
                banner = svBanner.SelectData<b2bBanner>("*", " BannerID = " + id_banner2 + " AND WebID = 1").First();
                list.Add(banner.ListNo);
                
                svBanner.UpdateBannerListNo(id, list);
            }
            catch (Exception ex)
            {
                CreateLogFiles(ex);
            }

            return Json(new { IsResult = svBanner.IsResult, MsgError = GenerateMsgError(svBanner.MsgError), ID = id });
        }

        #region SaveBannerImg
        [HttpPost]
        public ActionResult SaveBannerImg(HttpPostedFileBase FileImgPath)
        {
            imgManager = new FileHelper();
            #region Delete Folder
            imgManager.DeleteFilesInDir("Temp/Banner/" + FileImgPath);
            #endregion
            imgManager.UploadImage("Temp/Banner/" + "BannerHome", FileImgPath);
            Response.Cookies["CompID"].Value = DataManager.ConvertToString(LogonCompID);
            return Json(new { newimage = imgManager.ImageName }, "text/plain");
        }
        #endregion

        #region RemoveBannerImg
        public ActionResult RemoveBannerImg(string filenames)
        {
            imgManager = new FileHelper();
            imgManager.DeleteFilesInDir("Temp/Banner/" + filenames);
            return Json(new { newimage = imgManager.ImageName });
        }
        #endregion

        #region SetHotFeatPager
        public void SetBannerPager(FormCollection form)
        {
            ViewBag.Period = !string.IsNullOrEmpty(form["Period"]) ? form["Period"] : "";
            
        }
        #endregion
    }
}
