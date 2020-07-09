using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Prosoft.Service;
//using Prosoft.Base;
using Ouikum.Product;
using Ouikum.Category;
using Ouikum;
using System.Threading;
using res = Prosoft.Resource.Web.Ouikum;

namespace Ouikum.Web.MyB2B
{
    public partial class ProductController : BaseSecurityController
    {
        #region SaveImage
        [HttpPost]
        public ActionResult SaveProductImg(List<HttpPostedFileBase> FileProductImgPath,string TempID)
        {
            imgManager = new FileHelper();
            if(CheckIsLogin())
            {
                foreach (var img in FileProductImgPath)
                {

                    imgManager.UploadImage("Temp/Product/" + LogonCompID, img);
                    Response.Cookies["CompID"].Value = Request.Cookies[res.Common.lblWebsite].Values["CompID"];
                    while (!imgManager.IsSuccess)
                    {
                        Thread.Sleep(100);
                    }
                }
                return Json(new { newimage = imgManager.ImageName }, "text/plain");
            }
            else
            {
                DateTime dt = DateTime.Now;
                var folderName = String.Format("{0:yyyy-MM-dd}", dt);
                Random random = new Random();
                int randomNumber = random.Next(0, 100);

                HttpCookie myCookie = new HttpCookie("AddProduct");
                var path = "Temp/Product/UnRegis/" + folderName + "/" + TempID;
                myCookie["UnRegis"] = path;
                myCookie.Expires = DateTime.Now.AddDays(1d);
                Response.Cookies.Add(myCookie);

                foreach (var img in FileProductImgPath)
                { 
                    imgManager.UploadImage(path, img); 
                    while (!imgManager.IsSuccess)
                    {
                        Thread.Sleep(100);
                    }
                }
                return Json(new { newimage = imgManager.ImageName, pathName = path }, "text/plain");
            }
           
        }
        #endregion

        #region RemoveImage
        [HttpPost]
        public ActionResult RemoveProductImage()
        {
            imgManager = new FileHelper();
            imgManager.DeleteFilesInDir("Temp/Product/" + LogonCompID);
            return Json(new { newimage = imgManager.ImageName });
        }
        #endregion

    }
}
