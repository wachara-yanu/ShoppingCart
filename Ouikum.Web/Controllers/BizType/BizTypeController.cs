using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ouikum.BizType;

namespace Ouikum.Web.Controllers.BizType
{
    public class BizTypeController : BaseController
    {
        #region Member
        //
        // GET: /Address/
        BizTypeService svBiztype;
        #endregion

        #region Constructors
        public BizTypeController()
        {
            svBiztype = new BizTypeService();
        }
        #endregion

        #region GetBizType
        public ActionResult GetBizType(int b_id)
        {
            if (RedirectToProduction())
                return Redirect(UrlProduction);

            var BizTypes = svBiztype.GetBiztype().Where(it => it.BizTypeID == b_id).ToList();
            return Json(new { BizTypeID = BizTypes[0].BizTypeID, BizTypeName = BizTypes[0].BizTypeName });
        }
        #endregion

        #region ListBizType
        public ActionResult ListBizType()
        {
            var BizType = svBiztype.GetBiztype();
            var BizTypes = BizType.Select(it => new { BizTypeID = it.BizTypeID, BizTypeName = it.BizTypeName });
            return Json(new { BizTypes = BizTypes });
        }
        #endregion

    }
}
