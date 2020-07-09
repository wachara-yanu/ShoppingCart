using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ouikum.Common;
using Prosoft.Service;
using Ouikum;
using System.Collections;
using res = Prosoft.Resource.Web.Ouikum;

namespace Ouikum.Web.Controllers.Address
{
    public class AddressController : BaseController
    {
        #region Member
        //
        // GET: /Address/
        AddressService svAddress;
        #endregion

        #region Constructors
        public AddressController()
        {
            svAddress = new AddressService();
        }
        #endregion

        #region GetDistrictByProvinceID
        [HttpPost]
        public ActionResult GetDistrictByProvinceID(int pid)
        {
            var Districts = svAddress.ListDistrictByProvinceID(pid);
            var District = Districts.Select(it => new { DistrictID = it.DistrictID, DistrictName = it.DistrictName });
            if (Districts.Count() > 0)
            {
                return Json(new { IsResult = true, Districts = District });
            }
            return Json(new { IsResult = false });
        }
        #endregion

        #region GetGMapByProvinceID
        [HttpPost, ValidateInput(false)]
        public ActionResult GetGMapByProvinceID(int pid)
        {
            Hashtable data = new Hashtable();
            var Provinces = svAddress.GetProvince().Where(it => it.ProvinceID == pid);
            if (Provinces.Count() > 0)
            {
                var Province = Provinces.First();
                data.Add("MapLatitude", Province.Latitude);
                data.Add("MapLongtitude", Province.Longitude);
                data.Add("PinLatitude", Province.Latitude);
                data.Add("PinLongtitude", Province.Longitude);
                return Json(new { IsResult = true, Provinces = data });
            }
            else {
                data.Add("MapLatitude", 13.750056809568887);
                data.Add("MapLongtitude", 100.49468994140625);
                data.Add("PinLatitude", 13.750056809568887);
                data.Add("PinLongtitude", 100.49468994140625);
                return Json(new { IsResult = true, Provinces = data });
            }
        }
        #endregion

        #region GetDistrict
        public ActionResult GetDistrict(int d_id)
        {
            var Districts = svAddress.GetDistrict().Where(it => it.DistrictID == d_id).ToList();
            if (Districts.Count() > 0)
            {
                return Json(new { DistrictID = Districts[0].DistrictID, DistrictName = Districts[0].DistrictName });
            }
            else {
                return Json(new { DistrictID = 0, DistrictName = "-----"+res.Common.chooseDistrict+"-----" });
            }
        }
        #endregion

        #region GetProvince
        public ActionResult GetProvince(int p_id)
        {
            var Provinces = svAddress.GetProvince().Where(it => it.ProvinceID == p_id).ToList();
            if (Provinces.Count() > 0)
            {
                return Json(new { ProvinceID = Provinces[0].ProvinceID, ProvinceName = Provinces[0].ProvinceName });
            }
            else
            {
                return Json(new { ProvinceID = 0, ProvinceName = "-----"+res.Common.chooseProvince+"-----" });
            }
        }
        #endregion

        #region ListProvince
        public ActionResult ListProvince()
        {
            var Province = svAddress.GetProvince();
            var Provinces = Province.Select(it => new { ProvinceID = it.ProvinceID, ProvinceName = it.ProvinceName });
            return Json(new { Provinces = Provinces });
        }
        public ActionResult Province(int value)
        {
            List<emProvince> Province = new List<emProvince>();
            if (value > 0)
            {
                 Province = svAddress.GetProvince().Where(it => it.ProvinceID == value).ToList();
            }
            else {
                 Province = svAddress.GetProvince().ToList();
            }
            var Provinces = Province.Select(it => new { ProvinceID = it.ProvinceID, ProvinceName = it.ProvinceName });
            return Json(new { Provinces = Provinces });
        }
        #endregion

        #region ListDistrict
        public ActionResult ListDistrict()
        {
            var District = svAddress.GetDistrict();
            var Districts = District.Select(it => new { DistrictID = it.DistrictID, DistrictName = it.DistrictName });
            return Json(new { Districts = Districts });
        }
        public ActionResult District(int value)
        {
            List<emDistrict> District = new List<emDistrict>();
            if (value > 0)
            {
                District = svAddress.GetDistrict().Where(it => it.DistrictID == value).ToList();
            }
            else {
                District = svAddress.GetDistrict().ToList();
            }
            var Districts = District.Select(it => new { DistrictID = it.DistrictID, DistrictName = it.DistrictName });
            return Json(new { Districts = Districts });
        }
        #endregion

        #region DistrictByProvinces
        [HttpPost]
        public ActionResult DistrictByProvinces(int province = 0, int district = 0)
        {
            List<emDistrict> District = new List<emDistrict>();
            if (district > 0)
            {
                District = svAddress.ListDistrictByProvinceID(province).Where(it => it.DistrictID == district).ToList();
            }
            else
            {
                District = svAddress.ListDistrictByProvinceID(province);
            }

            var Districts = District.Select(it => new { DistrictID = it.DistrictID, DistrictName = it.DistrictName });
         
            return Json(new { Districts = Districts });
        }
        #endregion

        #region Provinces
        public ActionResult Provinces(int value)
        {
            List<emProvince> Province = new List<emProvince>();
            if (value > 0)
            {
                Province = svAddress.GetProvince().Where(it => it.ProvinceID == value).ToList();
            }
            else
            {
                Province = svAddress.GetProvince().ToList();
            }
            var Provinces = Province.Select(it => new { ProvinceID = it.ProvinceID, ProvinceName = it.ProvinceName });
            return Json(new { Provinces = Provinces });
        }
        #endregion

        #region Districts
        public ActionResult Districts(int value)
        {
            List<emDistrict> District = new List<emDistrict>();
            if (value > 0)
            {
                District = svAddress.GetDistrict().Where(it => it.DistrictID == value).ToList();
            }
            else
            {
                District = svAddress.GetDistrict().ToList();
            }
            var Districts = District.Select(it => new { DistrictID = it.DistrictID, DistrictName = it.DistrictName });
            return Json(new { Districts = Districts });
        }
        #endregion

    }
}
