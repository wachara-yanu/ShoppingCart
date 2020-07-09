using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Prosoft.Base;
using Ouikum;
using Prosoft.Service;
using System.Transactions;
using System.Runtime.Caching;
namespace Ouikum.Common
{
    public class CommonService : BaseSC
    {
        public string GetNameCache(string name)
        {
            //if (Base.AppLang == "en-US")
            //{
            //    name = name + "-"+Base.AppLang;
            //}
            //else
            //{
                name = name + "-" + Base.AppLang;
            //}
            return name;
        }
        public enum EnumType
        {
            HotFeatStatus,
            MemberPaidStatus,
            ShowStatus,
            ApproveStatus,
            ProductStatus,
            PackageStatus,
            PackageType,
            QtyUnits,
            PageSize,
            Gender,
            Period,
            AdminType,
            MemberType,
            JobType,
            UserStatus,
            CompLevel,
            DegreeLevel,
            SearchByMember,
            SearchByAdmin,
            SearchByProduct,
            SearchByBuylaed,
            SearchByPackage,
            SearchByMemberPaid,
            SearchByHotFeat,
            SearchByHotFeatStatus,
            SearchByQuotation,
            SearchByServiceType,
            SearchByPurchase,
            SortByProduct,
            SearchByMessage,
            SortBySupplier,
            OrderStatus,
            SearchProductByComp                                         
        }

        #region SelectEnum
        public List<view_EnumData> SelectEnum(EnumType _type)
        {
            var EnumData = new List<view_EnumData>();
           
                var name = "";
                name = GetNameCache(_type.ToString());
            if (_type == EnumType.HotFeatStatus)
            {
                #region Select AND Caching HotFeatStatus
                if (MemoryCache.Default[name] != null)
                {
                    EnumData = (List<view_EnumData>)MemoryCache.Default[name];
                }
                else
                {
                    EnumData = SelectData<view_EnumData>("*", "EnumTypeID = 1", null, 1, 1, false);
                    if (EnumData != null && TotalRow > 0)
                    {
                        MemoryCache.Default.Add(name, EnumData, DateTime.Now.AddDays(5));
                    };
                }
                #endregion
            }
            else if (_type == EnumType.MemberPaidStatus)
            {
                #region Select AND Caching MemberPaidStatus
                if (MemoryCache.Default["MemberPaidStatus"] != null)
                {
                    EnumData = (List<view_EnumData>)MemoryCache.Default["MemberPaidStatus" + Base.AppLang];
                }
                else
                {
                    var SQLSelect_Enum = "";
                    //if (Base.AppLang == "en-US")
                    //    SQLSelect_Enum = "*,EnumTextEng AS EnumText";
                    //else
                    SQLSelect_Enum = "*";
                    EnumData = SelectData<view_EnumData>(SQLSelect_Enum, "EnumTypeID = 2", null, 1, 1, false);
                    if (EnumData != null && TotalRow > 0)
                    {
                        MemoryCache.Default.Add("MemberPaidStatus" + Base.AppLang , EnumData, DateTime.Now.AddDays(5));
                    };
                }
                #endregion
            }
            else if (_type == EnumType.ShowStatus)
            {
                #region Select AND Caching ShowStatus
                if (MemoryCache.Default["ShowStatus"] != null)
                {
                    EnumData = (List<view_EnumData>)MemoryCache.Default["ShowStatus"];
                }
                else
                {
                    EnumData = SelectData<view_EnumData>("*", "EnumTypeID = 3", null, 1, 1, false);
                    if (EnumData != null && TotalRow > 0)
                    {
                        MemoryCache.Default.Add("ShowStatus", EnumData, DateTime.Now.AddDays(5));
                    };
                }
                #endregion
            }
            else if (_type == EnumType.ApproveStatus)
            {
                #region Select AND Caching ApproveStatus
                if (MemoryCache.Default["ApproveStatus"] != null)
                {
                    EnumData = (List<view_EnumData>)MemoryCache.Default["ApproveStatus" + Base.AppLang];
                }
                else
                {
                    var SQLSelect_Enum = "";
                    //if (Base.AppLang == "en-US")
                    //    SQLSelect_Enum = "*,EnumTextEng AS EnumText";
                    //else
                    SQLSelect_Enum = "*";
                    EnumData = SelectData<view_EnumData>(SQLSelect_Enum, "EnumTypeID = 4", null, 1, 1, false);
                    if (EnumData != null && TotalRow > 0)
                    {
                        MemoryCache.Default.Add("ApproveStatus" + Base.AppLang, EnumData, DateTime.Now.AddDays(5));
                    };
                }
                #endregion
            }
            else if (_type == EnumType.ProductStatus)
            {
                #region Select AND Caching ProductStatus
                if (MemoryCache.Default["ProductStatus"] != null)
                {
                    EnumData = (List<view_EnumData>)MemoryCache.Default["ProductStatus" + Base.AppLang];
                }
                else
                {
                    var SQLSelect_Enum = "";
                    //if (Base.AppLang == "en-US")
                    //    SQLSelect_Enum = "*,EnumTextEng AS EnumText";
                    //else
                    SQLSelect_Enum = "*";
                    EnumData = SelectData<view_EnumData>(SQLSelect_Enum, "EnumTypeID = 5", null, 1, 1, false);
                    if (EnumData != null && TotalRow > 0)
                    {
                        MemoryCache.Default.Add("ProductStatus"+ Base.AppLang, EnumData, DateTime.Now.AddDays(5));
                    };
                }
                #endregion
            }
            else if (_type == EnumType.QtyUnits)
            {
                #region Select AND Caching QtyUnits
                if (MemoryCache.Default[name] != null)
                {
                    EnumData = (List<view_EnumData>)MemoryCache.Default[name];
                }
                else
                {
                    var SQLSelect_ListQtyUnits = "";
                    //if (Base.AppLang == "en-US")
                    //    SQLSelect_ListQtyUnits = "*,EnumTextEng AS EnumText";
                    //else
                    SQLSelect_ListQtyUnits = "*";
                    EnumData = SelectData<view_EnumData>(SQLSelect_ListQtyUnits, "EnumTypeID = 6", null, 1, 1, false);
                    if (EnumData != null && TotalRow > 0)
                    {
                        MemoryCache.Default.Add(name, EnumData, DateTime.Now.AddDays(5));
                    };
                }
                #endregion
            }
            else if (_type == EnumType.PageSize)
            {
                #region Select AND Caching PageSize
                if (MemoryCache.Default["PageSize"] != null)
                {
                    EnumData = (List<view_EnumData>)MemoryCache.Default["PageSize"];
                }
                else
                {
                    EnumData = SelectData<view_EnumData>("*", "EnumTypeID = 7", null, 1, 1, false);
                    if (EnumData != null && TotalRow > 0)
                    {
                        MemoryCache.Default.Add("PageSize", EnumData, DateTime.Now.AddDays(5));
                    };
                }
                #endregion
            }
            else if (_type == EnumType.Gender)
            {
                #region Select AND Caching Gender
                if (MemoryCache.Default["Gender"] != null)
                {
                    EnumData = (List<view_EnumData>)MemoryCache.Default["Gender"];
                }
                else
                {
                    EnumData = SelectData<view_EnumData>("*", "EnumTypeID = 8", null, 1, 1, false);
                    if (EnumData != null && TotalRow > 0)
                    {
                        MemoryCache.Default.Add("Gender", EnumData, DateTime.Now.AddDays(5));
                    };
                }
                #endregion
            }
            else if (_type == EnumType.Period)
            {
                #region Select AND Caching Period
                if (MemoryCache.Default["Period"] != null)
                {
                    EnumData = (List<view_EnumData>)MemoryCache.Default["Period"];
                }
                else
                {
                    EnumData = SelectData<view_EnumData>("*", "EnumTypeID = 9", null, 1, 1, false);
                    if (EnumData != null && TotalRow > 0)
                    {
                        MemoryCache.Default.Add("Period", EnumData, DateTime.Now.AddDays(5));
                    };
                }
                #endregion
            }
            else if (_type == EnumType.AdminType)
            {
                #region Select AND Caching AdminType
                if (MemoryCache.Default["AdminType"] != null)
                {
                    EnumData = (List<view_EnumData>)MemoryCache.Default["AdminType"];
                }
                else
                {
                    EnumData = SelectData<view_EnumData>("*", "EnumTypeID = 10", null, 1, 1, false);
                    if (EnumData != null && TotalRow > 0)
                    {
                        MemoryCache.Default.Add("AdminType", EnumData, DateTime.Now.AddDays(5));
                    };
                }
                #endregion
            }
            else if (_type == EnumType.MemberType)
            {
                #region Select AND Caching MemberType
                if (MemoryCache.Default["MemberType"] != null)
                {
                    EnumData = (List<view_EnumData>)MemoryCache.Default["MemberType"];
                }
                else
                {
                    EnumData = SelectData<view_EnumData>("*", "EnumTypeID = 11", null, 1, 1, false);
                    if (EnumData != null && TotalRow > 0)
                    {
                        MemoryCache.Default.Add("MemberType", EnumData, DateTime.Now.AddDays(5));
                    };
                }
                #endregion
            }
            else if (_type == EnumType.JobType)
            {
                #region Select AND Caching JobType
                if (MemoryCache.Default["JobType"] != null)
                {
                    EnumData = (List<view_EnumData>)MemoryCache.Default["JobType"];
                }
                else
                {
                    EnumData = SelectData<view_EnumData>("*", "EnumTypeID = 12", null, 1, 1, false);
                    if (EnumData != null && TotalRow > 0)
                    {
                        MemoryCache.Default.Add("JobType", EnumData, DateTime.Now.AddDays(5));
                    };
                }
                #endregion
            }
            else if (_type == EnumType.UserStatus)
            {
                #region Select AND Caching UserStatus
                if (MemoryCache.Default["UserStatus"] != null)
                {
                    EnumData = (List<view_EnumData>)MemoryCache.Default["UserStatus"];
                }
                else
                {
                    EnumData = SelectData<view_EnumData>("*", "EnumTypeID = 13", null, 1, 1, false);
                    if (EnumData != null && TotalRow > 0)
                    {
                        MemoryCache.Default.Add("UserStatus", EnumData, DateTime.Now.AddDays(5));
                    };
                }
                #endregion
            }
            else if (_type == EnumType.CompLevel)
            {
                #region Select AND Caching CompLevel
                if (MemoryCache.Default["CompLevel"] != null)
                {
                    EnumData = (List<view_EnumData>)MemoryCache.Default["CompLevel"];
                }
                else
                {
                    EnumData = SelectData<view_EnumData>("*", "EnumTypeID = 14", "ListNo ASC", 1, 3, true);
                    if (EnumData != null && TotalRow > 0)
                    {
                        MemoryCache.Default.Add("CompLevel", EnumData, DateTime.Now.AddDays(5));
                    };
                }
                #endregion
            }
            else if (_type == EnumType.DegreeLevel)
            {
                #region Select AND Caching DegreeLevel
                if (MemoryCache.Default["DegreeLevel"] != null)
                {
                    EnumData = (List<view_EnumData>)MemoryCache.Default["DegreeLevel"];
                }
                else
                {
                    EnumData = SelectData<view_EnumData>("*", "EnumTypeID = 15", null, 1, 1, false);
                    if (EnumData != null && TotalRow > 0)
                    {
                        MemoryCache.Default.Add("DegreeLevel", EnumData, DateTime.Now.AddDays(5));
                    };
                }
                #endregion

            }
            else if (_type == EnumType.SearchByMember)
            {
                #region Select AND Caching SearchByMember
                if (MemoryCache.Default["SearchByMember"] != null)
                {
                    EnumData = (List<view_EnumData>)MemoryCache.Default["SearchByMember" + Base.AppLang];
                }
                else
                {
                    var SQLSelect_ListMemberList = "";
                    //if (Base.AppLang == "en-US")
                    //    SQLSelect_ListMemberList = "*,EnumTextEng AS EnumText";
                    //else
                    SQLSelect_ListMemberList = "*";
                    EnumData = SelectData<view_EnumData>(SQLSelect_ListMemberList, "EnumTypeID = 16", null, 1, 1, false);
                    if (EnumData != null && TotalRow > 0)
                    {
                        MemoryCache.Default.Add("SearchByMember" + Base.AppLang, EnumData, DateTime.Now.AddDays(5));
                    };
                }
                #endregion
            }
            else if (_type == EnumType.SearchByAdmin)
            {
                #region Select AND Caching SearchByAdmin
                if (MemoryCache.Default["SearchByAdmin"] != null)
                {
                    EnumData = (List<view_EnumData>)MemoryCache.Default["SearchByAdmin"];
                }
                else
                {
                    EnumData = SelectData<view_EnumData>("*", "EnumTypeID = 17", null, 1, 1, false);
                    if (EnumData != null && TotalRow > 0)
                    {
                        MemoryCache.Default.Add("SearchByAdmin", EnumData, DateTime.Now.AddDays(5));
                    };
                }
                #endregion
            }
            else if (_type == EnumType.SearchByProduct)
            {
                #region Select AND Caching SearchByProduct
                if (MemoryCache.Default["SearchByProduct"] != null)
                {
                    EnumData = (List<view_EnumData>)MemoryCache.Default["SearchByProduct" + Base.AppLang];
                }
                else
                {
                    var SQLSelect_Listapproveproduct = "";
                    //if (Base.AppLang == "en-US")
                    //    SQLSelect_Listapproveproduct = "*,EnumTextEng AS EnumText";
                    //else
                    SQLSelect_Listapproveproduct = "*";
                    EnumData = SelectData<view_EnumData>(SQLSelect_Listapproveproduct, "EnumTypeID = 18", null, 1, 1, false);
                    if (EnumData != null && TotalRow > 0)
                    {
                        MemoryCache.Default.Add("SearchByProduct" + Base.AppLang, EnumData, DateTime.Now.AddDays(5));
                    };
                }
                #endregion
            }
            else if (_type == EnumType.SearchByBuylaed)
            { 
                #region Select AND Caching SearchByBuylaed
                if (MemoryCache.Default["SearchByBuylaed"] != null)
                {
                    EnumData = (List<view_EnumData>)MemoryCache.Default[name];
                }
                else
                {
                    var SQLSelect_ListBuylead = "";
                    //if (Base.AppLang == "en-US")
                    //    SQLSelect_ListBuylead = "*,EnumTextEng AS EnumText";
                    //else
                    SQLSelect_ListBuylead = "*";
                    EnumData = SelectData<view_EnumData>(SQLSelect_ListBuylead, "EnumTypeID = 19", null, 1, 1, false);
                    if (EnumData != null && TotalRow > 0)
                    {
                        MemoryCache.Default.Add(name, EnumData, DateTime.Now.AddDays(5));
                    };
                }
                #endregion
            }
            else if (_type == EnumType.SearchByMemberPaid)
            {
                #region Select AND Caching SearchByMemberPaid
                if (MemoryCache.Default["SearchByMemberPaid"] != null)
                {
                    EnumData = (List<view_EnumData>)MemoryCache.Default["SearchByMemberPaid" + Base.AppLang];
                }
                else
                {
                    var SQLSelect_ListBuylaed = "";
                    //if (Base.AppLang == "en-US")
                    //    SQLSelect_ListBuylaed = "*,EnumTextEng AS EnumText";
                    //else
                    SQLSelect_ListBuylaed = "*";
                    EnumData = SelectData<view_EnumData>(SQLSelect_ListBuylaed, "EnumTypeID = 20", null, 1, 1, false);
                    if (EnumData != null && TotalRow > 0)
                    {
                        MemoryCache.Default.Add("SearchByMemberPaid" + Base.AppLang, EnumData, DateTime.Now.AddDays(5));
                    };
                }
                #endregion
            }
            else if (_type == EnumType.SearchByHotFeat)
            {
                #region Select AND Caching SearchByHotFeat
                if (MemoryCache.Default["SearchByHotFeat"] != null)
                {
                    EnumData = (List<view_EnumData>)MemoryCache.Default["SearchByHotFeat"];
                }
                else
                {
                    EnumData = SelectData<view_EnumData>("*", "EnumTypeID = 21", null, 1, 1, false);
                    if (EnumData != null && TotalRow > 0)
                    {
                        MemoryCache.Default.Add("SearchByHotFeat", EnumData, DateTime.Now.AddDays(5));
                    };
                }
                #endregion
            }
            else if (_type == EnumType.SearchByHotFeatStatus)
            {
                #region Select AND Caching SearchByHotFeatStatus
                if (MemoryCache.Default["SearchByHotFeatStatus"] != null)
                {
                    EnumData = (List<view_EnumData>)MemoryCache.Default["SearchByHotFeatStatus"];
                }
                else
                {
                    EnumData = SelectData<view_EnumData>("*", "EnumTypeID = 30", "ListNo ASC",1, 3, true);
                    if (EnumData != null && TotalRow > 0)
                    {
                        MemoryCache.Default.Add("SearchByHotFeatStatus", EnumData, DateTime.Now.AddDays(5));
                    };
                }
                #endregion
            }
            else if (_type == EnumType.SearchByQuotation)
            {

                #region Select AND Caching SearchByQuotation
                if (MemoryCache.Default[name] != null)
                {
                    EnumData = (List<view_EnumData>)MemoryCache.Default[name];
                }
                else
                {
                    var SQLSelect_ListQuotation = "";
                    //if (Base.AppLang == "en-US")
                    //    SQLSelect_ListQuotation = "*,EnumTextEng AS EnumText";
                    //else
                    SQLSelect_ListQuotation = "*";
                    EnumData = SelectData<view_EnumData>(SQLSelect_ListQuotation, "EnumTypeID = 22", null, 1, 1, false);
                    if (EnumData != null && TotalRow > 0)
                    {
                        MemoryCache.Default.Add(name, EnumData, DateTime.Now.AddDays(5));
                    };
                }
                #endregion
            }
            else if (_type == EnumType.SearchByServiceType)
            {
                
                #region Select AND Caching SearchByServiceType
                if (MemoryCache.Default[name] != null)
                {
                    EnumData = (List<view_EnumData>)MemoryCache.Default[name];
                }
                else
                {
                    var SQLSelect_ListEnum = "";
                    //if (Base.AppLang == "en-US")
                    //    SQLSelect_ListEnum = "*,EnumTextEng AS EnumText";
                    //else
                    SQLSelect_ListEnum = "*";
                    EnumData = SelectData<view_EnumData>(SQLSelect_ListEnum, "EnumTypeID = 23", null, 1, 1, false);
                    if (EnumData != null && TotalRow > 0)
                    {
                        MemoryCache.Default.Add(name, EnumData, DateTime.Now.AddDays(5));
                    };
                }
                #endregion
            }
            else if (_type == EnumType.SearchByPurchase)
            {
                #region Select AND Caching SearchByPurchase
                if (MemoryCache.Default["SearchByPurchase"] != null)
                {
                    EnumData = (List<view_EnumData>)MemoryCache.Default["SearchByPurchase"];
                }
                else
                {
                    EnumData = SelectData<view_EnumData>("*", "EnumTypeID = 24", null, 1, 1, false);
                    if (EnumData != null && TotalRow > 0)
                    {
                        MemoryCache.Default.Add("SearchByPurchase", EnumData, DateTime.Now.AddDays(5));
                    };
                }
                #endregion
            }
            else if (_type == EnumType.SortByProduct)
            {
                #region Select AND Caching SortByProduct
                if (MemoryCache.Default["SortByProduct"] != null)
                {
                    EnumData = (List<view_EnumData>)MemoryCache.Default["SortByProduct"];
                }
                else
                {
                    EnumData = SelectData<view_EnumData>("*", "EnumTypeID = 25", null, 1, 1, false);
                    if (EnumData != null && TotalRow > 0)
                    {
                        MemoryCache.Default.Add("SortByProduct", EnumData, DateTime.Now.AddDays(5));
                    };
                }
                #endregion
            }
            else if (_type == EnumType.SearchByMessage)
            {
                #region Select AND Caching SearchByMessage
                if (MemoryCache.Default["SearchByMessage"] != null)
                {
                    EnumData = (List<view_EnumData>)MemoryCache.Default["SearchByMessage" + Base.AppLang];
                }
                else
                {
                    var SQLSelect_ListMessage = "";
                    //if (Base.AppLang == "en-US")
                    //    SQLSelect_ListMessage = "*,EnumTextEng AS EnumText";
                    //else
                    SQLSelect_ListMessage = "*";
                    EnumData = SelectData<view_EnumData>(SQLSelect_ListMessage, "EnumTypeID = 26", null, 1, 1, false);
                    if (EnumData != null && TotalRow > 0)
                    {
                        MemoryCache.Default.Add("SearchByMessage" + Base.AppLang, EnumData, DateTime.Now.AddDays(5));
                    };
                }
                #endregion
            }
            else if (_type == EnumType.SortBySupplier)
            {
                #region Select AND Caching SortBySupplier
                if (MemoryCache.Default["SortBySupplier"] != null)
                {
                    EnumData = (List<view_EnumData>)MemoryCache.Default["SortBySupplier"];
                }
                else
                {
                    EnumData = SelectData<view_EnumData>("*", "EnumTypeID = 27", null, 1, 1, false);
                    if (EnumData != null && TotalRow > 0)
                    {
                        MemoryCache.Default.Add("SortBySupplier", EnumData, DateTime.Now.AddDays(5));
                    };
                }
                #endregion
            }
            else if (_type == EnumType.OrderStatus)
            {
                #region Select AND Caching OrderStatus
                if (MemoryCache.Default["OrderStatus"] != null)
                {
                    EnumData = (List<view_EnumData>)MemoryCache.Default["OrderStatus"];
                }
                else
                {
                    EnumData = SelectData<view_EnumData>("*", "EnumTypeID = 28", null, 1, 1, false);
                    if (EnumData != null && TotalRow > 0)
                    {
                        MemoryCache.Default.Add("OrderStatus", EnumData, DateTime.Now.AddDays(5));
                    };
                }
                #endregion
            }
            else if (_type == EnumType.SearchProductByComp)
            {
                #region Select AND Caching OrderStatus
                if (MemoryCache.Default["OrderStatus"] != null)
                {
                    EnumData = (List<view_EnumData>)MemoryCache.Default["OrderStatus"];
                }
                else
                {
                    EnumData = SelectData<view_EnumData>("*", "EnumTypeID = 31", null, 1, 1, false);
                    if (EnumData != null && TotalRow > 0)
                    {
                        MemoryCache.Default.Add("OrderStatus", EnumData, DateTime.Now.AddDays(5));
                    };
                }
                #endregion
            }
            else if (_type == EnumType.SearchByPackage)
            {
                #region Select AND Caching OrderStatus
                if (MemoryCache.Default["SearchByPackage"] != null)
                {
                    EnumData = (List<view_EnumData>)MemoryCache.Default["SearchByPackage"];
                }
                else
                {
                    EnumData = SelectData<view_EnumData>("*", "EnumTypeID = 32", null, 1, 1, false);
                    if (EnumData != null && TotalRow > 0)
                    {
                        MemoryCache.Default.Add("SearchByPackage", EnumData, DateTime.Now.AddDays(5));
                    };
                }
                #endregion
            }
            else if (_type == EnumType.PackageType)
            {
                #region Select AND Caching OrderStatus
                if (MemoryCache.Default["PackageType"] != null)
                {
                    EnumData = (List<view_EnumData>)MemoryCache.Default["PackageType"];
                }
                else
                {
                    EnumData = SelectData<view_EnumData>("*", "EnumTypeID = 33", null, 1, 1, false);
                    if (EnumData != null && TotalRow > 0)
                    {
                        MemoryCache.Default.Add("PackageType", EnumData, DateTime.Now.AddDays(5));
                    };
                }
                #endregion
            }
            else if (_type == EnumType.PackageStatus)
            {
                #region Select AND Caching OrderStatus
                if (MemoryCache.Default["PackageStatus"] != null)
                {
                    EnumData = (List<view_EnumData>)MemoryCache.Default["PackageStatus"];
                }
                else
                {
                    EnumData = SelectData<view_EnumData>("*", "EnumTypeID = 34", null, 1, 1, false);
                    if (EnumData != null && TotalRow > 0)
                    {
                        MemoryCache.Default.Add("PackageStatus", EnumData, DateTime.Now.AddDays(5));
                    };
                }
                #endregion
            }
            return EnumData;
        }
        #endregion

    }
}

