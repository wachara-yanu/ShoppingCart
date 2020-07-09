using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Prosoft.Service;
//using Prosoft.Base;
using Ouikum;
using Ouikum.Buylead;
using Ouikum.Category;
using System.Threading;
using res = Prosoft.Resource.Web.Ouikum;

namespace Ouikum.Web.MyB2B
{
    public partial class BuyleadController : BaseSecurityController
    {
        #region SaveImage
        [HttpPost]
        public ActionResult SaveBuyleadImg(HttpPostedFileBase FileBuyleadImgPath)
        {
            imgManager = new FileHelper();
            imgManager.UploadImage("Temp/Buylead/" + LogonCompID, FileBuyleadImgPath);
            Response.Cookies["CompID"].Value = Request.Cookies[res.Common.lblWebsite].Values["CompID"];
            while (!imgManager.IsSuccess)
            {
                Thread.Sleep(100);
            }
            return Json(new { newimage = imgManager.ImageName }, "text/plain");
        }
        #endregion

        #region RemoveImage
        [HttpPost]
        public ActionResult RemoveBuyleadImage()
        {
            imgManager = new FileHelper();
            imgManager.DeleteFilesInDir("Temp/Buylead/" + LogonCompID);
            return Json(new { newimage = imgManager.ImageName });
        }
        #endregion

    }
}
