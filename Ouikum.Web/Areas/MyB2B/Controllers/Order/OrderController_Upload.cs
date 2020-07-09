using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ouikum.Order;
using Ouikum.Company;
using Prosoft.Service;
//using Prosoft.Base;
using System.Transactions;
using System.Data.Linq;
using System.Threading;
using Ouikum;

namespace Ouikum.Web.MyB2B
{
    public partial class OrderController : BaseSecurityController
    {

        /*--------------------slipImg------------------------*/

        #region SaveSlipImg
        [HttpPost]
        public ActionResult SaveSlipImg(List<HttpPostedFileBase> FileSlipImgPath)
        {
            imgManager = new FileHelper();
            foreach (var img in FileSlipImgPath)
            {

                imgManager.UploadImage("Temp/Companies/Slip/" + LogonCompID, img); 
                while (!imgManager.IsSuccess)
                {
                    Thread.Sleep(100);
                }
            }
            return Json(new { newimage = imgManager.ImageName, compid = LogonCompID }, "text/plain");
         

        }
        #endregion

        #region RemoveSlipImg
        public ActionResult RemoveSlipImg()
        {
            imgManager = new FileHelper();
            imgManager.DeleteFilesInDir("Temp/Companies/Slip/" + LogonCompID);
            return Json(new { newimage = imgManager.ImageName });
        }
        #endregion

      

    }
}
