using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Prosoft.Base;
using System.Transactions;
//using System.Web.Mvc;
using Ouikum.Product;
using Ouikum.Buylead;
using Ouikum.Category;
using System.Runtime.Caching;

namespace Ouikum.Category
{
    #region enum
    public enum CateStatus
    {
        All,
        Home
    }
    #endregion

    public class CategoryService : BaseSC
    {
        OuikumDataContext qDB;

        #region Constructor
        public CategoryService()
        {
            qDB = new OuikumDataContext(ConnectionString);
        }
        public CategoryService(OuikumDataContext conn)
        {
            qDB = conn;
        }
        #endregion

        #region Category
        public List<b2bCategory> ListCategoryAll()
        {
            var data = new List<b2bCategory>();
            SQLWhere = "IsDelete = 0";
            SQLSelect = "CategoryID,CategoryName,CategoryNameEng, CategoryCode,ParentCategoryPath,CategoryLevel,ParentCategoryID";
            var name = "ListCategoryAll-"+Base.AppLang; 

             if (Base.AppLang == "eng") 
                 SQLSelect = "CategoryID, CategoryNameEng AS CategoryName,CategoryCode,ParentCategoryPath,CategoryLevel,ParentCategoryID"; 
                  
                if (MemoryCache.Default[name] != null)
                {
                    // get value from cache
                    data = (List<b2bCategory>)MemoryCache.Default[name]; 
                }
                else
                {
                    data = SelectData<b2bCategory>(SQLSelect, SQLWhere);
                    if (data != null && TotalRow > 0)
                    {
                        MemoryCache.Default.Add(name, data, DateTime.Now.AddDays(1));
                    } 
                }

                #endregion
            return data;
        }
        public List<b2bCategory> GetCategoryFooter()
        {
            var cates = new List<b2bCategory>();
            var name = "CateFooter";
            if (MemoryCache.Default[name] != null)
            {
                cates = (List<b2bCategory>)MemoryCache.Default[name];
            }
            else
            {
                #region Load Category
                var svCategory = new CategoryService();
                cates = svCategory.GetCategoryByLevel(1);
                var cateid = new int[] { 3689, 2180, 2598, 4030, 2064, 1189, 1795, 1416, 7102, 1416 };
                cates = cates.Where(m => cateid.Contains(m.CategoryID)).ToList();
                if (cates != null && cates.Count() > 0)
                {
                    MemoryCache.Default.Add(name, cates, DateTime.Now.AddDays(4));
                }
                #endregion
            }
            return cates;
        }

        public List<b2bCategory> GetCategoryByLevelAndBuyleadCount(int Level = 1)
        {
            var cates = new List<b2bCategory>();
            var name = "GetCategoryByLevelAndBuyleadCount" + Level;
            if (MemoryCache.Default[name] != null)
            {
                cates = (List<b2bCategory>)MemoryCache.Default[name + Base.AppLang];
            }
            else
            {
                var SQLSelect_list = "";
                //if (Base.AppLang == "en-US")
                //    SQLSelect_list = "CategoryID,CategoryNameEng AS CategoryName,CategoryLevel";
                //else
                SQLSelect_list = " CategoryID,CategoryName,CategoryLevel";
                var sqlWhere = "CategoryLevel = " + Level + " AND RowFlag = 1 AND BuyleadCount > 0 AND IsDelete = 0";
                cates = SelectData<b2bCategory>(SQLSelect_list, sqlWhere, "CategoryName");
                if (cates != null && TotalRow > 0)
                {
                    MemoryCache.Default.Add(name + Base.AppLang, cates, DateTime.Now.AddDays(5));
                };
            }
            return cates;
        }
        public List<b2bCategory> GetCategoryByLevel(int Level = 1)
        {
            var cates = new List<b2bCategory>();
            if (MemoryCache.Default["GetCategoryByLevel" + Level] != null)
            {
                cates = (List<b2bCategory>)MemoryCache.Default["GetCategoryByLevel" + Base.AppLang + Level];
            }
            else
            {
                var SQLSelect_CateList = "";
                //if (Base.AppLang == "en-US")
                //    SQLSelect_CateList = "CategoryID,CategoryNameEng As CategoryName,CategoryNameEng,CategoryLevel";
                //else
                SQLSelect_CateList = "CategoryID,CategoryName,CategoryNameEng,CategoryLevel";
                var sqlWhere = "CategoryLevel = " + Level + " AND RowFlag  = 1 AND IsDelete = 0 ";
                cates = SelectData<b2bCategory>(SQLSelect_CateList, sqlWhere, "CategoryName");
                if (cates != null && TotalRow > 0)
                {
                    MemoryCache.Default.Add("GetCategoryByLevel" + Base.AppLang + Level, cates, DateTime.Now.AddDays(5));
                };
            }
            return cates;
        }

        #region Method Select
        #region ListIndrustryCategory

        public List<b2bCategory> ListIndrustryCategory()
       {
            return ListIndrustryCategory(0);
        }

        #region ListIndrustryCategory
        public List<b2bCategory> ListIndrustryCategory(int CompID = 0, string Tname = "")
        {
            var sqlWhere = " CategoryLevel = 1 AND IsDelete = 0 and CategoryType = 1";
            var data = new List<b2bCategory>();
            var name = "cate-indust-"+CompID+"-"+Tname;
            if (MemoryCache.Default[name] != null)
            {
                data = (List<b2bCategory>)MemoryCache.Default[name + Base.AppLang];
            }

            else
            {
                if (CompID > 0)
                {
                    Tname = Tname != "" ? Tname : "b2bProduct";
                    sqlWhere += "and categoryid in (select  catelv1 from " + Tname + " where compid = " + CompID + " and IsDelete = 0 and IsJunk = 0  group by catelv1 )";
                }


                var SQLSelect_CateMenu = "";
                //if (Base.AppLang == "en-US")
                //    SQLSelect_CateMenu = "CategoryID,CategoryNameEng As CategoryName,CategoryNameEng,CategoryLevel";
                //else
                SQLSelect_CateMenu = "CategoryID,CategoryName,CategoryNameEng,CategoryLevel";
                data = SelectData<b2bCategory>(SQLSelect_CateMenu, sqlWhere, "CategoryID,CategoryName");

                if (data != null && TotalRow > 0)
                    MemoryCache.Default.Add(name + Base.AppLang, data, DateTime.Now.AddDays(1)); 

            }

            return data;

        }

        #endregion

        #region ListIndrustryCategory
        public List<b2bCategory> ListMenuProductCategory(int CompID = 0, string Tname = "")
        {
            var sqlWhere = " CategoryLevel = 1 AND IsDelete = 0";
            var data = new List<b2bCategory>();
            var name = "cate-indust-" + CompID + "-" + Tname;
            //if (MemoryCache.Default[name] != null)
            //{
            //    data = (List<b2bCategory>)MemoryCache.Default[name + Base.AppLang];
            //}

            //else
            //{
                if (CompID > 0)
                {
                    Tname = Tname != "" ? Tname : "b2bProduct";
                    sqlWhere += "and categoryid in (select  catelv1 from " + Tname + " where compid = " + CompID + " and IsDelete = 0 and IsJunk = 0  group by catelv1 )";
                }


                var SQLSelect_CateMenu = "";
                //if (Base.AppLang == "en-US")
                //    SQLSelect_CateMenu = "CategoryID,CategoryNameEng As CategoryName,CategoryNameEng,CategoryLevel";
                //else
                SQLSelect_CateMenu = "CategoryID,CategoryName,CategoryNameEng,CategoryLevel";
                data = SelectData<b2bCategory>(SQLSelect_CateMenu, sqlWhere, "CategoryName");

                if (data != null && TotalRow > 0)
                    MemoryCache.Default.Add(name + Base.AppLang, data, DateTime.Now.AddDays(1));

            //}

            return data;

        }

        #endregion

        #region ListIndrustryCategory
        public List<b2bCategory> ListMenuJunkCategory(int CompID = 0, string Tname = "")
        {
            var sqlWhere = " CategoryLevel = 1 AND IsDelete = 0";
            var data = new List<b2bCategory>();
            var name = "cate-indust-" + CompID + "-" + Tname;
            //if (MemoryCache.Default[name] != null)
            //{
            //    data = (List<b2bCategory>)MemoryCache.Default[name + Base.AppLang];
            //}

            //else
            //{
                if (CompID > 0)
                {
                    Tname = Tname != "" ? Tname : "b2bProduct";
                    sqlWhere += "and categoryid in (select  catelv1 from " + Tname + " where compid = " + CompID + " and IsDelete = 0 and IsJunk = 1  group by catelv1 )";
                }


                var SQLSelect_CateMenu = "";
                //if (Base.AppLang == "en-US")
                //    SQLSelect_CateMenu = "CategoryID,CategoryNameEng As CategoryName,CategoryNameEng,CategoryLevel";
                //else
                SQLSelect_CateMenu = "CategoryID,CategoryName,CategoryNameEng,CategoryLevel";
                data = SelectData<b2bCategory>(SQLSelect_CateMenu, sqlWhere, "CategoryName");

                if (data != null && TotalRow > 0)
                    MemoryCache.Default.Add(name + Base.AppLang, data, DateTime.Now.AddDays(1));

            //}

            return data;

        }

        #endregion

        #region ListIndrustryCategory
        public List<b2bCategory> ListMenuRecommendCategory(int CompID = 0, string Tname = "")
        {
            var sqlWhere = " CategoryLevel = 1 AND IsDelete = 0";
            var data = new List<b2bCategory>();
            var name = "cate-indust-" + CompID + "-" + Tname;
            //if (MemoryCache.Default[name] != null)
            //{
            //    data = (List<b2bCategory>)MemoryCache.Default[name + Base.AppLang];
            //}

            //else
            //{
            if (CompID > 0)
            {
                Tname = Tname != "" ? Tname : "b2bProduct";
                sqlWhere += "and categoryid in (select  catelv1 from " + Tname + " where compid = " + CompID + " and IsDelete = 0 and ListNo != 0  group by catelv1 )";
            }


            var SQLSelect_CateMenu = "";
            //if (Base.AppLang == "en-US")
            //    SQLSelect_CateMenu = "CategoryID,CategoryNameEng As CategoryName,CategoryNameEng,CategoryLevel";
            //else
            SQLSelect_CateMenu = "CategoryID,CategoryName,CategoryNameEng,CategoryLevel";
            data = SelectData<b2bCategory>(SQLSelect_CateMenu, sqlWhere, "CategoryName");

            if (data != null && TotalRow > 0)
                MemoryCache.Default.Add(name + Base.AppLang, data, DateTime.Now.AddDays(1));

            //}

            return data;

        }

        #endregion
        #endregion

        #region ListWholesaleCategory
        public List<b2bCategory> ListWholesaleCategory()
        {
            
            return ListWholesaleCategory(0);
        }

        public List<b2bCategory> ListWholesaleCategory(int CompID = 0, string Tname = "")
        {
            var data = new List<b2bCategory>();

            var sqlWhere = " CategoryLevel = 1 AND IsDelete = 0 and CategoryType = 2";

            SQLSelect = " CategoryID , CategoryName, CategoryNameEng , CategoryLevel ";

            if (CompID > 0)
            {
                Tname = Tname != "" ? Tname : "b2bProduct";
                sqlWhere += "and categoryid in (select  catelv1 from " + Tname + " where compid = " + CompID + "  and IsDelete = 0 and IsJunk = 0  group by catelv1 )";
            }

            //if (Base.AppLang == "en-US")
            //{
            //    SQLSelect = "CategoryID ,CategoryNameEng AS CategoryName,CategoryLevel";

            //    if (CompID > 0)
            //    {
            //        data = SelectData<b2bCategory>(SQLSelect, sqlWhere);
            //    }
            //    else
            //    {
            //        #region Cache Eng Category
            //        var name = "ListWholesaleCategory-US";
            //        if (MemoryCache.Default[name] != null)
            //        {
            //            // get value from cache
            //            data = (List<b2bCategory>)MemoryCache.Default[name + Base.AppLang];

            //        }
            //        else
            //        {
            //            data = SelectData<b2bCategory>(SQLSelect, sqlWhere);
            //            if (data != null && TotalRow > 0)
            //            {
            //                MemoryCache.Default.Add(name + Base.AppLang, data, DateTime.Now.AddHours(12));
            //            }

            //        }

            //        #endregion
            //    }
            //}
            //else
            //{

                //#region Cache Eng Category
                //var name = "ListWholesaleCategory-TH";
                //if (MemoryCache.Default[name] != null)
                //{
                //    // get value from cache
                //    data = (List<b2bCategory>)MemoryCache.Default[name + Base.AppLang];
                //}
                //else
                //{
                //    data = SelectData<b2bCategory>(SQLSelect, sqlWhere,"",1,10);
                //    if (data != null && TotalRow > 0)
                //    {
                //        MemoryCache.Default.Add(name + Base.AppLang, data, DateTime.Now.AddHours(12));
                //    }

                //}

                //#endregion
            //}

            return SelectData<b2bCategory>("CategoryID,CategoryName,CategoryNameEng,CategoryLevel", sqlWhere, "CategoryName",1,11);
        }
        #endregion

        #region ListB2CCategory
        public List<b2bCategory> ListB2CCategory()
        {
            return ListB2CCategory(0);
        }
        public List<b2bCategory> ListB2CCategory(int CompID = 0, string Tname = "")
        {
            var sqlWhere = " CategoryLevel = 1 AND IsDelete = 0 and CategoryType = 3";
            if (CompID > 0)
            {
                Tname = Tname != "" ? Tname : "b2bProduct";
                sqlWhere += "and categoryid in (select  catelv1 from " + Tname + " where compid = " + CompID + "  and IsDelete = 0 and IsJunk = 0 group by catelv1 )";
            }

            return SelectData<b2bCategory>("CategoryID,CategoryName,CategoryNameEng,CategoryLevel", sqlWhere, "CategoryName");
        }
        #endregion

        #region ListWholesaleConsumerCategory
        public List<b2bCategory> ListWholesaleConsumerCategory()
        {
            return ListWholesaleConsumerCategory(CateStatus.Home, 0, "");
        }
        public List<b2bCategory> ListWholesaleConsumerCategory(CateStatus status, int CompID = 0, string Tname = "")
        {
            var sqlWhere = " CategoryLevel = 1 AND IsDelete = 0 and CategoryType IN (2,3)";
            if (CompID > 0)
            {
                Tname = Tname != "" ? Tname : "b2bProduct";
                sqlWhere += "and categoryid in (select  catelv1 from " + Tname + " where compid = " + CompID + "  and IsDelete = 0  group by catelv1 )";
            }
            if (status == CateStatus.Home)
            {
                return SelectData<b2bCategory>("CategoryID,CategoryName,CategoryNameEng,CategoryLevel", sqlWhere, "CategoryName ASC", 1, 13);
            }
            else
            {
                return SelectData<b2bCategory>("CategoryID,CategoryName,CategoryNameEng,CategoryLevel", sqlWhere, "CategoryName ASC");
            }
        }
        #endregion

        #region ListWholesaleConsumerCategoryLv2
        public List<b2bCategory> ListWholesaleConsumerCategoryLv2(int ParentCateID, int Size)
        {
            var sqlWhere = "CategoryLevel = 2 AND IsDelete = 0 AND ParentCategoryID = " + ParentCateID;

            return SelectData<b2bCategory>("CategoryID,CategoryName,CategoryNameEng,CategoryLevel,ParentCategoryID", sqlWhere, "CategoryName ASC", 1, Size);

        }
        #endregion

        #region LoadSubCategory
        public List<b2bCategory> LoadSubCategory(int ParentCateID, int Size, int Level)
        { 
            var cates = new List<b2bCategory>();

            if (Base.AppLang == "eng")
            {
                SQLSelect = "CategoryID,CategoryNameEng AS CategoryName, CategoryLevel,ParentCategoryID";
                SQLOrderBy = "CategoryNameEng ASC ";
            } 
            var name = "LoadSubCategory-"+Base.AppLang+"-"+ParentCateID+"-"+Size+"-"+Level;
            if (MemoryCache.Default[name] != null)
            {
                cates = (List<b2bCategory>)MemoryCache.Default[name];
            }
            else
            { 
                cates = ListCategoryAll(); 
                if (cates != null && cates.Count > 0)
                { 
                    cates = cates.Where(m=>m.CategoryLevel == Level && m.ParentCategoryID == ParentCateID).OrderBy(m=>m.CategoryName).ToList();
                    if(Size > 0){
                    cates = cates.Take(Size).ToList();
                    }
                    MemoryCache.Default.Add(name, cates, DateTime.Now.AddDays(15));
                };
            }
            return cates;

        }
        #endregion

        #region Search Category
        public List<b2bCategory> SearchCategoryByName(string CategoryName, string type = "")
        {
            type = type != "" ? type : "";
            var sqlWhere = "CategoryName Like N'%" + CategoryName + "%' AND CategoryLevel = 3 AND RowFlag > 0 AND Isdelete=0" + type;
            var sqlSelect = "";
            //if(Base.AppLang == "en-US")
            //{
            //    sqlSelect = "CategoryID,CategoryNameEng AS CategoryName,CategoryCode, ParentCategoryPath,CategoryLevel"; 
            //}
            //else
            //{
                sqlSelect = "CategoryID,CategoryName,CategoryNameEng,CategoryCode, ParentCategoryPath,CategoryLevel"; 
            //}
            return SelectData<b2bCategory>(sqlSelect, sqlWhere, "CategoryName,CategoryNameEng");
        }

      
        public List<b2bCategory> SearchCategoryByID(int CateID)
        {
            var name = "SearchCategoryByID-" + CateID;
            var cates = new List<b2bCategory>();
            if (MemoryCache.Default[name] != null)
            {
                cates = (List<b2bCategory>)MemoryCache.Default[name];
            }
            else
            {
                var sqlWhere = "CategoryID = " + CateID;
                
                var sqlSelect = "";
                //if (Base.AppLang == "en-US") { sqlSelect = "*,CategoryNameEng AS C ategoryName"; }
                //else { sqlSelect = "*"; }
                sqlSelect = "*";
                cates = SelectData<b2bCategory>(sqlSelect, sqlWhere);

                if (cates != null && TotalRow > 0)
                {
                    MemoryCache.Default.Add(name, cates, DateTime.Now.AddDays(15));
                };
            }
            return cates;
        }
        #endregion

        #endregion

        #region Method Update
        public bool saveprod(b2bProduct p)
        {
            var a = GetByID<b2bProduct>("productid", p.ProductID);
            a.ProductName = p.ProductName;
            SaveData<b2bProduct>(a, "productid");
            return IsResult;
        }
        #region Update Product Count
        public bool UpdateProductCount(int CategoryLevel, int CategoryID)
        {
            var svProduct = new Product.ProductService();

            if (CategoryLevel == 3)
            {
                var sqlWhere = svProduct.CreateWhereAction(ProductAction.FrontEnd) + "AND CateLV3 = " + CategoryID;
                var count = CountData<view_SearchProduct>(" * ", sqlWhere);
                var sqlUpdate = " ProductCount = " + count;
                sqlWhere = "CategoryLevel = 3 AND CategoryID = " + CategoryID;
                UpdateByCondition<b2bCategory>(sqlUpdate, sqlWhere);
            }
            else if (CategoryLevel == 2)
            {
                var sqlUpdate = " ProductCount = (Select SUM(ProductCount) from b2bCategory where ParentCategoryID = " + CategoryID + " ) ";
                var sqlWhere = " CategoryID = " + CategoryID;
                UpdateByCondition<b2bCategory>(sqlUpdate, sqlWhere);

            }
            else if (CategoryLevel == 1)
            {
                var sqlUpdate = " ProductCount = (Select SUM(ProductCount) from b2bCategory where ParentCategoryID = " + CategoryID + " ) ";
                var sqlWhere = " CategoryID = " + CategoryID;
                UpdateByCondition<b2bCategory>(sqlUpdate, sqlWhere);
            }


            return IsResult;
        }
        #endregion

        #region Update Buylead Count
        public bool UpdateBuyleadCount(int CategoryLevel, int CategoryID)
        {
            var svBuylead = new Buylead.BuyleadService();
            if (CategoryLevel == 3)
            {
                var sqlWhere = svBuylead.CreateWhereAction(BuyleadAction.FrontEnd) + "AND CateLV3 = " + CategoryID;
                var count = CountData<Ouikum.view_BuyLead>(" * ", sqlWhere);
                var sqlUpdate = " BuyleadCount = " + count;
                sqlWhere = "CategoryLevel = 3 AND CategoryID = " + CategoryID;
                UpdateByCondition<b2bCategory>(sqlUpdate, sqlWhere);
            }
            else if (CategoryLevel == 2)
            {
                var sqlUpdate = " BuyleadCount = (Select SUM(BuyleadCount) from b2bCategory where ParentCategoryID = " + CategoryID + " ) ";
                var sqlWhere = " CategoryID = " + CategoryID;
                UpdateByCondition<b2bCategory>(sqlUpdate, sqlWhere);

            }
            else if (CategoryLevel == 1)
            {
                var sqlUpdate = " BuyleadCount = (Select SUM(BuyleadCount) from b2bCategory where ParentCategoryID = " + CategoryID + " ) ";
                var sqlWhere = " CategoryID = " + CategoryID;
                UpdateByCondition<b2bCategory>(sqlUpdate, sqlWhere);
            }


            return IsResult;
        }
        #endregion

        #region MoveCategory
        public bool MoveCategory(int oldcatelv1, int oldcatelv2, int newcatelv1, int newcatelv2, int newcatelv3)
        {

            var svProduct = new ProductService();
            var svBuylead = new BuyleadService();
            var svCategory = new CategoryService();

            using (var trans = new TransactionScope())
            {
                svProduct.MoveProductInCate(2, oldcatelv2, newcatelv2, newcatelv3);
                svProduct.MoveProductInCate(1, oldcatelv1, newcatelv1, newcatelv3);
                trans.Complete();
            }
            using (var trans = new TransactionScope())
            {
                svBuylead.MoveBuyleadInCate(2, oldcatelv2, newcatelv2, newcatelv3);
                svBuylead.MoveBuyleadInCate(1, oldcatelv1, newcatelv1, newcatelv3);
                trans.Complete();
            }

            if (svProduct.IsResult)
            {
                var categoryNameLV1 = svCategory.SelectData<b2bCategory>("CategoryID,CategoryName", "CategoryID = " + newcatelv1).First();
                var categoryNameLV2 = svCategory.SelectData<b2bCategory>("CategoryID,CategoryName", "CategoryID = " + newcatelv2).First();
                var categoryNameLV3 = svCategory.SelectData<b2bCategory>("CategoryID,CategoryName", "CategoryID = " + newcatelv3).First();
                using (var trans = new TransactionScope())
                {
                    var strParent = @"ParentCategoryPath = N'" + categoryNameLV1.CategoryName
                        + " >> " + categoryNameLV2.CategoryName + " >> " + categoryNameLV3.CategoryName + "'";
                    UpdateByCondition<b2bCategory>("ParentCategoryID = " + newcatelv2 + " , " + strParent, " CategoryID = " + newcatelv3);

                    trans.Complete();
                }

                #region Update Product Count
                UpdateProductCount(3, newcatelv3);
                UpdateProductCount(2, oldcatelv2);
                UpdateProductCount(1, oldcatelv1);
                UpdateProductCount(3, newcatelv3);
                UpdateProductCount(2, newcatelv2);
                UpdateProductCount(1, newcatelv1);

                #endregion

                #region Update Buylead Count
                UpdateBuyleadCount(3, newcatelv3);
                UpdateBuyleadCount(2, oldcatelv2);
                UpdateBuyleadCount(1, oldcatelv1);
                UpdateBuyleadCount(3, newcatelv3);
                UpdateBuyleadCount(2, newcatelv2);
                UpdateBuyleadCount(1, newcatelv1);
                #endregion
            }

            return IsResult;
        }
        #endregion

        #region MoveProduct
        public bool MoveProduct(int oldcatelv1, int oldcatelv2, int oldcatelv3, int newcatelv1, int newcatelv2, int newcatelv3)
        {

            var svProduct = new ProductService();
            var svBuylead = new BuyleadService();

            svProduct.MoveProductInCateLV(oldcatelv1, oldcatelv2, oldcatelv3, newcatelv1, newcatelv2, newcatelv3);
            svBuylead.MoveBuyleadInCateLV(oldcatelv1, oldcatelv2, oldcatelv3, newcatelv1, newcatelv2, newcatelv3);

            return IsResult;
        }
        #endregion

        #region DeleteCategory
        public bool DeleteCategory(int CategoryID)
        {
            var svCategory = new CategoryService();
            var sqlUpdate = "IsDelete = 1";
            var sqlWhere = "CategoryLevel = 3 AND CategoryID = " + CategoryID;
            UpdateByCondition<b2bCategory>(sqlUpdate, sqlWhere);
            return IsResult;
        }
        #endregion

        #region DeleteCategoryManyLevel
        public bool DeleteCategoryManyLevel(int CategoryID)
        {
            var svCategory = new CategoryService();
            var sqlUpdate = "IsDelete = 1";
            var sqlWhere = "CategoryID = " + CategoryID;
            UpdateByCondition<b2bCategory>(sqlUpdate, sqlWhere);
            return IsResult;
        }
        #endregion
        #region DeleteCategoryType
        public bool DeleteCategoryType(int CategoryType)
        {
            var svCategory = new CategoryService();
            var sqlUpdate = "IsDelete = 1";
            var sqlWhere = "CategoryType = " + CategoryType;
            UpdateByCondition<b2bCategoryType>(sqlUpdate, sqlWhere);
            return IsResult;
        }
        #endregion
        #endregion
         
        #region InsertCategoryType
        public bool InsertCategoryType(b2bCategoryType CategoryType)
        {
            var svCategory = new CategoryService();
            var Cate = svCategory.SelectData<b2bCategoryType>(" * ", "IsDelete = 0 AND RowFlag > 0 AND CategoryTypeName = '" + CategoryType.CategoryTypeName + "'");
            if (Cate.Count() != 1)
            {
                CategoryType.CreatedDate = DateTimeNow;
                CategoryType.ModifiedDate = DateTimeNow;
                CategoryType.CreatedBy = "sa";
                CategoryType.ModifiedBy = "sa";

                using (var trans = new TransactionScope())
                {
                    qDB.b2bCategoryTypes.InsertOnSubmit(CategoryType);
                    qDB.SubmitChanges();
                    trans.Complete();
                    IsResult = true;
                }
            }
            else
            {
                IsResult = false;
            }
            return IsResult;
        }
        #endregion

        #region InsertCategory
        public bool InsertCategory(b2bCategory Category)
        {
            var svCategory = new CategoryService();
            var Cate = svCategory.SelectData<b2bCategory>(" * ", "IsDelete = 0 AND RowFlag > 0 AND CategoryLevel = " + Category.CategoryLevel + " AND CategoryName = '" + Category.CategoryName + "'");
            if (Cate.Count() != 1)
            {
                Category.CreatedDate = DateTimeNow;
                Category.ModifiedDate = DateTimeNow;
                Category.CreatedBy = "sa";
                Category.ModifiedBy = "sa";

                using (var trans = new TransactionScope())
                {
                    qDB.b2bCategories.InsertOnSubmit(Category);
                    qDB.SubmitChanges();
                    trans.Complete();
                    IsResult = true;
                }
            }
            else
            {
                IsResult = false;
            }
            return IsResult;
        }
        #endregion

        #region UpdateCategoryType
        public bool UpdateCategoryType(int CategoryType, string CategoryTypeName)
        {
            var svCategory = new CategoryService();
            var sqlUpdate = "CategoryTypeName = N'" + CategoryTypeName + "'";
            var sqlWhere = "CategoryType = " + CategoryType;
            UpdateByCondition<b2bCategoryType>(sqlUpdate, sqlWhere);
            return IsResult;
        }
        #endregion

        #region UpdateCategoryLevel
        public bool UpdateCategoryLevel(int CategoryID, string CategoryName, string CategoryNameEng, int CategoryType)
        {
            var svCategory = new CategoryService();
            var sqlUpdate = "CategoryName = N'" + CategoryName + "' , CategoryNameEng = N'" + CategoryNameEng + "' , ParentCategoryPath = N'" + CategoryName + "'";
            var sqlWhere = "CategoryID = " + CategoryID + " AND CategoryType = " + CategoryType + " AND IsDelete = 0";
            UpdateByCondition<b2bCategory>(sqlUpdate, sqlWhere);
            return IsResult;
        }
        #endregion

        #region UpdateCategoryLevelParentID
        public bool UpdateCategoryLevelParentID(int CategoryID, string CategoryName, string CategoryNameEng, int ParentCategoryID)
        {
            var svCategory = new CategoryService();
            var parentCate = svCategory.SelectData<b2bCategory>("CategoryID,ParentCategoryPath", "CategoryID = " + ParentCategoryID).First();
            var sqlUpdate = "CategoryName = N'" + CategoryName + "' , CategoryNameEng = N'" + CategoryNameEng + "' , ParentCategoryPath = N'" + parentCate.ParentCategoryPath + " >> " + CategoryName + "'";
            var sqlWhere = "CategoryID = " + CategoryID + " AND ParentCategoryID = " + ParentCategoryID + " AND IsDelete = 0";
            UpdateByCondition<b2bCategory>(sqlUpdate, sqlWhere);
            return IsResult;
        }
        #endregion

    }
}