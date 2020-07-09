using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using Prosoft.Base;
using System.Transactions;
//using System.Web.Mvc;
using System.Data.Linq;
using Prosoft.Service;

namespace Ouikum.Favorite
{

    #region enum
    public enum FavAction
    {
        All, 
        IsFav,
        NotFav
    }
    public enum FavStatus
    {
        All,
        WaitApprove,
        NoApprove,
        Approve,
        Edited,
        WaitDetect
    }

    public enum OrderBy
    {
        ModifiedDateDESC,
        ModifiedDate,
        CreatedDateDESC,
        CreatedDate,
        ViewCountDESC,
        ViewCount,
    }

    public  enum SearchBy
    {
        ProductName,
        CompanyName,
        BuyleadName
    }

    #endregion
     
    public class FavProductService : BaseSC
    { 

        #region Product

        #region Property
        public int? ViewCount { get; set; }
        public int ContactCount { get; set; }
        #endregion

        #region Method Validate
        #region ValidateInsert

        #region ValidateInsert
        public bool ValidateInsert(int ID,int CompID)
        {
            if (ID>0)
            {
                var count = CountData<view_FavProduct>(" * ", CreateWhereAction(FavAction.IsFav,CompID) + " AND ProductID = " + ID);

                if (count == 0)
                    IsResult = true;
                else
                    IsResult = false;
            }
            return IsResult;
        }

        #endregion
       
        #endregion

        #region ValidateFullProduct
        /// <summary>
        /// ตรวจสอบ ว่า สินค้าของ user เต็ม หรือไม่
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
      
        #endregion

        #region ValidateUpdate
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        
        #endregion

        #endregion

        #region Method Select 

        #region GetProduct

        #region Generate SQLWhere
        public string CreateWhereAction(FavAction action, int? CompID = 0)
        {
            var sqlWhere = string.Empty;
            #region Condition
            if (action == FavAction.All)
            {
                sqlWhere = "( IsDelete = 0 ) ";
            }
            else if (action == FavAction.IsFav)
            {
                //comprowflag มาจาก b2bcompany.rowflag
                sqlWhere = "( IsDelete = 0 AND RowFlag=1 AND CompRowFlag IN (2,4)) AND ( IsShow = 1 AND IsJunk = 0 )";
            }
            else if (action == FavAction.NotFav)
            {
                sqlWhere = "( IsDelete = 0 AND RowFlag=-1) AND  IsJunk = 0  )  ";
            }
            
            if (CompID > 0)
                sqlWhere += "AND (CompID = " + CompID + ")";
            #endregion

            return sqlWhere;
        }


        public string CreateWhereSearchBy(string txtSearch = "", SearchBy action = SearchBy.ProductName)
        {
            var sqlwhere = string.Empty;

            if (action == SearchBy.ProductName)
            {
                sqlwhere = " ProductName = " + txtSearch;
            }
            if (action == SearchBy.BuyleadName)
            {
                sqlwhere = " BuyleadName = " + txtSearch;
            }
            else if (action == SearchBy.CompanyName)
            {
                sqlwhere = " CompName = " + txtSearch;
            }

            return sqlwhere;

        }

        public string CreateWhereCause(
            int CompID = 0, string txtSearch = "", int PStatus = 0, int GroupID = 0,
            int CateLevel = 0, int CateID = 0,int BizTypeID = 0,int CompLevel = 0,
            int CompProvinceID = 0
            )
        { 
            #region DoWhereCause
            if (CompID > 0) 
                SQLWhere += " AND CompID = "+CompID;

            if (!string.IsNullOrEmpty(txtSearch))
                SQLWhere += " AND ProductName LIKE N'" + txtSearch + "%' ";


            if (PStatus > 0)
                SQLWhere += " And RowFlag = " + PStatus; 

            if (BizTypeID > 0) 
                SQLWhere += " AND (BizTypeID = " + BizTypeID + ")";

            if (CompLevel > 0)
                SQLWhere += " AND (CompLevel = " + CompLevel + ")";

            if (CompProvinceID > 0)
                SQLWhere += " AND (CompProvinceID = " + CompProvinceID + ")";  
            #endregion

            return SQLWhere;
        }

        #endregion

        #region Generate Orderby
        public string CreateOrderby(OrderBy sort)
        {
            string SqlOrderBy = string.Empty;

             #region Sort By
            switch (sort)
            {
                case OrderBy.CreatedDateDESC:
                    SqlOrderBy = "CreatedDate DESC";
                    break;
                case OrderBy.CreatedDate:
                    SqlOrderBy = "CreatedDate";
                    break;
                case OrderBy.ModifiedDateDESC:
                    SqlOrderBy = "ModifiedDate DESC";
                    break;
                case OrderBy.ModifiedDate:
                    SqlOrderBy = "ModifiedDate";
                    break;
                case OrderBy.ViewCountDESC:
                    SqlOrderBy = "ViewCount DESC";
                    break;
                case OrderBy.ViewCount:
                    SqlOrderBy = "ViewCount ";
                    break;
            }
            #endregion 
            return SqlOrderBy;
        }
        #endregion
        #endregion


        #endregion

        #region Method Insert
        public bool InsertFavProduct(int Pro, int CompID)
        {
            var svFavProduct = new FavProductService();
            b2bFavProduct model = new b2bFavProduct();
                bool ChkProduct = ValidateInsert(Pro,CompID);
                if (ChkProduct == true)
                {
                    model.ProductID = Pro;
                    model.CompID = CompID;
                    model.FavGroupProductID = 0;
                    model.RowFlag = 1;
                    model.IsShow = true;
                    model.IsDelete = false;
                    model.CreatedDate = DateTimeNow;
                    model.ModifiedDate = DateTimeNow;
                    model.CreatedBy = "sa";
                    model.ModifiedBy = "sa";
                    svFavProduct.SaveData(model, "FavProductID");
                    IsResult = true;
                }
                else
                {
                    IsResult = false;
                    MsgError.Add(new Exception("product exist"));
                }
            return IsResult;
        }

        #endregion
         
        #region Update

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>



        #region Delete
        public bool Delete(List<int> ProductID, int CompID)
        {
            var svCompany = new Company.CompanyService();

            var Contains = SQLWhereListInt(ProductID, "ProductID");
            UpdateByCondition<b2bFavProduct>(" IsDelete = 1 ", " CompID = " + CompID + " AND " + Contains);

            IsResult = svCompany.UpdateProductCount(CompID);

            return IsResult;
        }
        #endregion

        #endregion


        #endregion

    }
}