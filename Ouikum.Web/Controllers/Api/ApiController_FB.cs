using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Prosoft.Service;
//using Prosoft.Base;
using Ouikum.Web.Models;
using Ouikum.Common;
using Ouikum.Company;
using res = Prosoft.Resource.Web.Ouikum;
using Facebook;

namespace Ouikum.Web.Controllers
{
    public partial class ApiController : BaseController
    {
        #region facebook

        [HttpGet]
        public ActionResult linkwithfacebook(string token, int compid)
        {
            var svCompany = new CompanyService();

            dynamic response = GetFacebookResponse("me", token);
            if (response != null)
            {
                var facebookid = response["id"];
                svCompany.LinkWithFacebook(facebookid, compid);
            }

            var companies = svCompany.SelectData<b2bCompany>(" * ", " CompID = " + compid);
            var comp = SetModelCompany(companies);



            return Json(new { 
                status = svCompany.IsResult, 
                compid = compid, 
                facebookid = comp.First().facebookid,
                msgerror = GenerateMsgError(svCompany.MsgError)
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult unlinkwithfacebook(int compid)
        {
            var svCompany = new CompanyService();

            svCompany.UnlinkWithFacebook(compid);
            var isresult = svCompany.IsResult;
            var companies = svCompany.SelectData<b2bCompany>(" * ", " CompID = " + compid);
            var comp = SetModelCompany(companies);

            //if (company != null)
            //{
            //    var cop
            //}
            return Json(new { status = svCompany.IsResult, compid = compid, facebookid = comp.First().facebookid }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult facebook(string token)
        {
            bool result = false;
            dynamic response = GetFacebookResponse("me", token);
            var compid = 0;
            if (response != null)
            {
                var email = response["email"];
                var bcMember = new MemberService();
                var member = new List<view_CompMember>();
                member = bcMember.SelectData<view_CompMember>(" * ", " IsDelete = 0 AND  CompRowflag = 2 AND Email = N'" + email + "' ");

                if (bcMember.TotalRow > 0)
                {
                    var company = member.First();
                    compid = company.CompID;
                    result = true;
                }
            }

            return Json(new {status = result ,compid = compid},JsonRequestBehavior.AllowGet);
        }

        public static dynamic GetFacebookResponse(string actionUrl, string accessToken)
        {
            FacebookClient FbApp;
            if (string.IsNullOrEmpty(accessToken))
            {
                FbApp = new FacebookClient();
            }
            else
            {
                FbApp = new FacebookClient(accessToken);
            }
            return FbApp.Get(actionUrl) as JsonObject;
        }
        #endregion

    }
}
