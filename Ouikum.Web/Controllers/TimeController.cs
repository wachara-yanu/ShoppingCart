using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ouikum.Web.Controllers
{
    public class TimeController : BaseController
    {
        //
        // GET: /Time/

        public ActionResult Index()
        {
            
            return View();
        }

        public ActionResult TestMail()
        {
            var mailTo = new List<string>();
            mailTo.Add("prasit@prosoft.co.th");
            mailTo.Add("stepmonkey2010@gmail.com");
            var mailCC = new List<string>();

            var isResult = OnSendByAlertEmail("test", "prasit@prosoft.co.th", mailTo, mailCC, "test");
            //OnSendMailInformUserName("test
            return Json(new { isresult = isResult , mailTo = mailTo }, JsonRequestBehavior.AllowGet);
        }
    }
}
