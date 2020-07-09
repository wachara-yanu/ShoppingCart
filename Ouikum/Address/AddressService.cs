using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Prosoft.Base;
using Ouikum;
using System.Runtime.Caching;

namespace Ouikum.Common
{
    public class AddressService : BaseSC
    {

        #region Method


        #region SubDistrict

        #region GetSubDistrict
        /// <summary>
        /// เรียกข้อมูล ตาราง District ที่ RowFlag > 0 
        /// </summary>
        /// <returns>IQueryable</returns>
        public IQueryable<emSubDistrict> GetSubDistrict()
        {
            IQueryable<emSubDistrict> query = qDB.emSubDistricts.Where(it => it.IsDelete == Convert.ToBoolean(0)).OrderBy(it => it.DISTRICT_NAME);
            return query;
        }
        #endregion

        #region ListSubDistrict
        /// <summary>
        /// List ข้อมูล ตาราง District ที่ RowFlag > 0 
        /// </summary>
        /// <returns>List</returns>
        public List<emSubDistrict> ListSubDistrict()
        {
            var data = SelectData<emSubDistrict>("*", "IsDelete = 0", "DISTRICT_NAME ASC");
            return data.ToList();
        }
        #endregion

        #region GetSubDistrictByProvinceID
        /// <summary>
        /// เรียกข้อมูล ตาราง District ตาม ProvinceID
        /// </summary>
        /// <param name="id">ProvinceID</param>
        /// <returns>IQueryable</returns>
        //public IQueryable<emDistrict> GetDistrictByProvinceID(int id)
        //{
        //        IQueryable<emDistrict> query = qDB.emDistricts.Where(it => it.ProvinceID == id).OrderBy(it => it.DistrictName);
        //        return query;
        //}
        #endregion

        #region ListSubDistrictByProvinceID
        /// <summary>
        /// List ข้อมูล ตาราง District ตาม ProvinceID
        /// </summary>
        /// <param name="id">ProvinceID</param>
        /// <returns>List</returns>
        public List<emSubDistrict> ListSubistrictByProvinceID(int id, string OrderBy = "DISTRICT_NAME ASC")
        {
            var data = new List<emSubDistrict>();
            SQLSelect = " DISTRICT_ID,DISTRICT_NAME,DISTRICT_NAMEEng ";
            //if (Base.AppLang == "en-US")
            //{
            //    SQLSelect = " DistrictID, DistrictNameEng AS DistrictName";
            //    OrderBy = " DistrictNameEng ASC ";
            //}

            var name = "GetSubDistrict-" + Base.AppLang;

            if (MemoryCache.Default[name] != null)
            {
                data = (List<emSubDistrict>)MemoryCache.Default[name];
            }
            else
            {
                var sqlWhere = " IsDelete = 0 ";
                data = SelectData<emSubDistrict>(" * ", sqlWhere, OrderBy);
                if (data != null && TotalRow > 0)
                {
                    MemoryCache.Default.Add(name, data, DateTime.Now.AddDays(5));
                };

            }

            if (id > 0)
                data = data.Where(m => m.AMPHUR_ID == id).ToList();

            return data;

        }
        #endregion

        #endregion


        #region District

        #region Method Select

        #region GetDistrict
        /// <summary>
        /// เรียกข้อมูล ตาราง District ที่ RowFlag > 0 
        /// </summary>
        /// <returns>IQueryable</returns>
        public IQueryable<emDistrict> GetDistrict()
        {
            IQueryable<emDistrict> query = qDB.emDistricts.Where(it => it.IsDelete == Convert.ToBoolean(0)).OrderBy(it => it.DistrictName);
            return query;
        }
        #endregion

        #region ListDistrict
        /// <summary>
        /// List ข้อมูล ตาราง District ที่ RowFlag > 0 
        /// </summary>
        /// <returns>List</returns>
        public List<emDistrict> ListDistrict()
        {
            var data = SelectData<emDistrict>("*", "IsDelete = 0", "DistrictName ASC");
            return data.ToList();
        }
        #endregion

        #region GetDistrictByProvinceID
        /// <summary>
        /// เรียกข้อมูล ตาราง District ตาม ProvinceID
        /// </summary>
        /// <param name="id">ProvinceID</param>
        /// <returns>IQueryable</returns>
        //public IQueryable<emDistrict> GetDistrictByProvinceID(int id)
        //{
        //        IQueryable<emDistrict> query = qDB.emDistricts.Where(it => it.ProvinceID == id).OrderBy(it => it.DistrictName);
        //        return query;
        //}
        #endregion

        #region ListDistrictByProvinceID
        /// <summary>
        /// List ข้อมูล ตาราง District ตาม ProvinceID
        /// </summary>
        /// <param name="id">ProvinceID</param>
        /// <returns>List</returns>
        public List<emDistrict> ListDistrictByProvinceID(int id ,string OrderBy = "DistrictName ASC")
        {
            var data = new List<emDistrict>();
            SQLSelect = " DistrictID,DistrictName,DistrictNameEng ";
            //if (Base.AppLang == "en-US")
            //{
            //    SQLSelect = " DistrictID, DistrictNameEng AS DistrictName";
            //    OrderBy = " DistrictNameEng ASC ";
            //}

            var name = "GetDistrict-" + Base.AppLang ;

            if (MemoryCache.Default[name] != null)
            {
                data = (List<emDistrict>)MemoryCache.Default[name];
            }
            else
            {
                var sqlWhere = " IsDelete = 0 ";
                data = SelectData<emDistrict>(" * ", sqlWhere, OrderBy);
                if (data != null && TotalRow > 0)
                {
                    MemoryCache.Default.Add(name, data, DateTime.Now.AddDays(5));
                };
                
            }

            if (id > 0) 
                data = data.Where(m => m.ProvinceID == id).ToList();  

            return data;

        }
        #endregion

        #endregion

        #endregion

        #region Province

        #region Method Select

        #region GetCountry
        /// <summary>
        /// เรียกข้อมูล ตาราง Province ที่ RowFlag > 0 
        /// </summary>
        /// <returns>IQueryable</returns>
        public IQueryable<emCountry> GetCountry()
        {
            IQueryable<emCountry> query = qDB.emCountries.Where(it => it.IsDelete == Convert.ToBoolean(0)).OrderBy(it => it.CountryName);

            return query;
        }

        #endregion

        #region GetProvince
        /// <summary>
        /// เรียกข้อมูล ตาราง Province ที่ RowFlag > 0 
        /// </summary>
        /// <returns>IQueryable</returns>
        public IQueryable<emProvince> GetProvince()
        {
            IQueryable<emProvince> query = qDB.emProvinces.Where(it => it.IsDelete == Convert.ToBoolean(0)).OrderBy(it => it.ProvinceName);

            return query;
        }

        #endregion

        #region GetProvinceAll
        /// <summary>
        /// เรียกข้อมูล ตาราง Province ที่ RowFlag > 0 
        /// </summary>
        /// <returns>IQueryable</returns>
        public List<emProvince> GetProvinceAll(string OrderBy = "ProvinceName ASC")
        {
            var data = new List<emProvince>();
            //SQLSelect = " ProvinceID,ProvinceName,ProvinceNameEng ";
            //if (Base.AppLang == "en-US")
            //{
            //    SQLSelect = " ProvinceID,ProvinceNameEng AS ProvinceName";
            //    OrderBy = " ProvinceNameEng ASC ";
            //}

            //var name = "GetProvinceAll"+Base.AppLang;
            if (MemoryCache.Default["GetProvinceAll"] != null)
            {
                data = (List<emProvince>)MemoryCache.Default["GetProvinceAll" + Base.AppLang];
            }
            else
            {
                var SQLSelect_Pro = "";
                //if (Base.AppLang == "en-US")
                //    SQLSelect_Pro = "ProvinceID,ProvinceNameEng AS ProvinceName";
                //else
                SQLSelect_Pro = "ProvinceID,ProvinceName";
                var sqlWhere = " IsDelete = 0 ";
                data = SelectData<emProvince>(SQLSelect_Pro, sqlWhere, OrderBy);
                if (data != null && TotalRow > 0)
                {
                    MemoryCache.Default.Add("GetProvinceAll" + Base.AppLang, data, DateTime.Now.AddDays(5));
                };
            }

            return data;
        }

        #endregion

        #region ListProvince
        /// <summary>
        /// List ข้อมูล ตาราง Province ที่ RowFlag > 0 
        /// </summary>
        /// <returns>List</returns>
        public List<emProvince> ListProvince()
        {
            var SQLSelect_Prov = "";
            //if (Base.AppLang == "en-US")
            //    SQLSelect_Prov = "ProvinceID,ProvinceNameEng AS ProvinceName";
            //else
            SQLSelect_Prov = "ProvinceID,ProvinceName";
            var data = SelectData<emProvince>(SQLSelect_Prov, "IsDelete = 0", "ProvinceName ASC");
            return data.ToList();
        }
        #endregion

        #endregion

        #endregion



        #endregion
    }
}
