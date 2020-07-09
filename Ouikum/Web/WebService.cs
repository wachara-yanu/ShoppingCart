using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using Prosoft.Base;
//using System.Web.Mvc;
using System.Transactions;
using Prosoft.Service;
using System.Runtime.Caching;

namespace Ouikum.Common
{
    public class WebService : BaseSC
    {
        #region Select
        #region emWeb
        #region GetWebAll
        public List<emWeb> GetWebAll()
        {
            var webs = new List<emWeb>();
            if (MemoryCache.Default["WebAll"] != null)
            {
                webs = (List<emWeb>)MemoryCache.Default["WebAll"];
            }
            else
            {
                webs = SelectData<emWeb>("WebID,WebName", "Isdelete = 0");
                if (webs != null && TotalRow > 0)
                {
                    MemoryCache.Default.Add("WebAll", webs, DateTime.Now.AddHours(4));
                }; 
            }
            return webs;
        }
        
        #endregion

        #region GetWeb
        /// <summary>
        /// เรียกข้อมูล ตาราง Member ที่ RowFlag > 0 
        /// </summary>
        /// <returns>IQueryable</returns>
        public IQueryable<emWeb> GetWeb()
        {
            IQueryable<emWeb> query = qDB.emWebs.Where(it => it.RowFlag > 0);
            return query;
        }
        #endregion
        //#region ListWeb
        ///// <summary>
        ///// List ข้อมูล ตาราง Member ที่ RowFlag > 0 
        ///// </summary>
        ///// <returns>List</returns>
        //public List<emWeb> ListWeb()
        //{
        //    return GetWeb().Select(it => new Web
        //    {
        
        //    }).ToList();
        //}
        //#endregion
        #endregion
        #endregion
    }
}
