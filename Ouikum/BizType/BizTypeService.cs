using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
//using Prosoft.Base;

namespace Ouikum.BizType
{
    public class BizTypeService : BaseSC
    {
        #region Method

            #region Biztype

                 #region Method Select

                #region GetBiztypeAll
                /// <summary>
                /// เรียกข้อมูล ตาราง BusinessType ที่ RowFlag > 0 
                /// </summary>
                /// <returns>IQueryable</returns>
                public List<b2bBusinessType> GetBiztypeAll()
                {
                    var data = new List<b2bBusinessType>();
                    var sqlOrder = "BizTypeName ASC";
                    SQLSelect = " BizTypeID,BizTypeName,BizTypeCode";
                    //if (Prosoft.Base.Base.AppLang == "en-US"){

                    //    SQLSelect = "BizTypeID,BizTypeCode AS BizTypeName";
                    //    sqlOrder = "BizTypeCode ASC ";
                    //}
                    var name = "GetBiztypeAll-"+Prosoft.Base.Base.AppLang;
                    if (MemoryCache.Default[name] != null)
                    {
                        data = (List<b2bBusinessType>)MemoryCache.Default[name];
                    }
                    else
                    {
                        var sqlWhere = " IsDelete = 0 ";
                        data = SelectData<b2bBusinessType>(SQLSelect, sqlWhere, sqlOrder);
                        if (data != null && TotalRow > 0)
                        {
                            MemoryCache.Default.Add(name, data, DateTime.Now.AddDays(2));
                        };
                    }
                    return data;

                }
                #endregion


                 #region GetBiztype
                 /// <summary>
                 /// เรียกข้อมูล ตาราง BusinessType ที่ RowFlag > 0 
                 /// </summary>
                 /// <returns>IQueryable</returns>
                 public IQueryable<b2bBusinessType> GetBiztype()
                 {
                     IQueryable<b2bBusinessType> query = qDB.b2bBusinessTypes.Where(it => it.RowFlag > 0);//.OrderBy(it => it.BizTypeCode);
                     return query;
                 }
                 #endregion

                 //#region ListBiztype
                 ///// <summary>
                 ///// List ข้อมูล ตาราง BusinessType ที่ RowFlag > 0 
                 ///// </summary>
                 ///// <returns>List</returns>
                 //public List<BizType> ListBiztype()
                 //{
                 //    return GetBiztype().Select(it => new BizType
                 //    {
                 //        BizTypeID = it.BizTypeID,
                 //        BizTypeName = it.BizTypeName
                 //    }).ToList();
                 //}
                 //#endregion

                 #endregion

                 public List<b2bBusinessType> ListBiztype()
                    {
                        var SQLSelect_Biz = "";
                        //if (Base.AppLang == "en-US")
                        //    SQLSelect_Biz = "BizTypeID,BizTypeCode AS BizTypeName";
                        //else
                        SQLSelect_Biz = "BizTypeID,BizTypeName";
                        var data = SelectData<b2bBusinessType>(SQLSelect_Biz, "IsDelete = 0", "BizTypeName ASC");
                        return data.ToList();
                    }
   


            #endregion

        #endregion
    }
}