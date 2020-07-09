using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ouikum.Common;
using Ouikum;
using Ouikum.Web.Models;
namespace Ouikum.Web.Controllers.Benefit
{
    public class BenefitController : BaseController
    {
        //
        // GET: /Benefit/
        CommonService svCommon = new Common.CommonService();
        MemberService svMember;
        CommonService svComp;
        public ActionResult Gold()
        {
            GetStatusUser();
            ViewBag.EnumServiceType = svCommon.SelectEnum(CommonService.EnumType.SearchByServiceType);
            return View();
        }
        public ActionResult Compare()
        {
            GetStatusUser();
            ViewBag.EnumServiceType = svCommon.SelectEnum(CommonService.EnumType.SearchByServiceType);
            return View();
        }
        public ActionResult AddmoreProduct()
        {
            GetStatusUser();
            ViewBag.EnumServiceType = svCommon.SelectEnum(CommonService.EnumType.SearchByServiceType);
            return View();
        }
        public ActionResult ShowProduct()
        {
            GetStatusUser();
            ViewBag.EnumServiceType = svCommon.SelectEnum(CommonService.EnumType.SearchByServiceType);
            return View();
        }
        public ActionResult MoreCompanyWeb()
        {
            GetStatusUser();
            ViewBag.EnumServiceType = svCommon.SelectEnum(CommonService.EnumType.SearchByServiceType);
            return View();
        }
        public ActionResult PurchasingMatching()
        {
            GetStatusUser();
            ViewBag.EnumServiceType = svCommon.SelectEnum(CommonService.EnumType.SearchByServiceType);
            return View();
        }
        public ActionResult ProsoftProgramFree()
        {
            GetStatusUser();
            ViewBag.EnumServiceType = svCommon.SelectEnum(CommonService.EnumType.SearchByServiceType);
            return View();
        }
        public ActionResult ServiceRate()
        {
            GetStatusUser();
            ViewBag.EnumServiceType = svCommon.SelectEnum(CommonService.EnumType.SearchByServiceType);
            return View();
        }
        public ActionResult AdsRate(int? tab)
        {
            GetStatusUser();
            ViewBag.EnumServiceType = svCommon.SelectEnum(CommonService.EnumType.SearchByServiceType);
            if (tab == null) {
                tab = 0;
            }
            ViewBag.tab = tab; 
            return View();
        }

        public ActionResult Rank()
        {
            GetStatusUser();
            ViewBag.EnumServiceType = svCommon.SelectEnum(CommonService.EnumType.SearchByServiceType);
            return View();
        }

        public ActionResult PackageHot()
        {
            //var memberData = new view_CompMember();
            //AddressService svAddress = new AddressService();

            GetStatusUser();       
            ViewBag.EnumServiceType = svCommon.SelectEnum(CommonService.EnumType.SearchByServiceType);

            //if (CheckIsLogin())
            //{
            //    var Member = svCommon.SelectData<view_CompMember>("*", "IsDelete = 0 AND MemberID =" + LogonMemberID, null, 0, 0, false);
            //    if (Member.Count() > 0)
            //    {
            //        memberData = Member.First();
            //    }
            //}
            //var Districts = svAddress.ListDistrict();

            //ViewBag.District = Districts;
            //ViewBag.member = memberData;
            return View();
        }

        public ActionResult PackagePremium()
        {
            //var memberData = new view_CompMember();
            //AddressService svAddress = new AddressService();

            GetStatusUser();
            ViewBag.EnumServiceType = svCommon.SelectEnum(CommonService.EnumType.SearchByServiceType);

            //if (CheckIsLogin())
            //{
            //    var Member = svCommon.SelectData<view_CompMember>("*", "IsDelete = 0 AND MemberID =" + LogonMemberID, null, 0, 0, false);
            //    if (Member.Count() > 0)
            //    {
            //        memberData = Member.First();
            //    }
            //}
            //var Districts = svAddress.ListDistrict();

            //ViewBag.District = Districts;
            //ViewBag.member = memberData;
            return View();
        }

        public ActionResult PackageFeature()
        {
            //var memberData = new view_CompMember();
            //AddressService svAddress = new AddressService();

            GetStatusUser();
            ViewBag.EnumServiceType = svCommon.SelectEnum(CommonService.EnumType.SearchByServiceType);

            //if (CheckIsLogin())
            //{
            //    var Member = svCommon.SelectData<view_CompMember>("*", "IsDelete = 0 AND MemberID =" + LogonMemberID, null, 0, 0, false);
            //    if (Member.Count() > 0)
            //    {
            //        memberData = Member.First();
            //    }
            //}
            //var Districts = svAddress.ListDistrict();

            //ViewBag.District = Districts;
            //ViewBag.member = memberData;
            return View();
        }

        public ActionResult PackageList()
        {
            GetStatusUser();
            ViewBag.EnumServiceType = svCommon.SelectEnum(CommonService.EnumType.SearchByServiceType);
            return View();
        }

    }
}
