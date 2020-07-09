using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using Prosoft.Base;
using System.Transactions;
//using System.Web.Mvc;
using System.Data.Linq;
using Prosoft.Service;

namespace Ouikum.Product
{

    #region enum
    public enum ProductAction
    {
        All,
        BackEnd,
        FrontEnd,
        Junk,
        Admin,
        WebSite,
        Recommend,
        Home
    }
    public enum BlogAction
    {
        All,
        BackEnd,
        FrontEnd,
        Junk,
        Admin,
        WebSite,
        Recommend
    }
    public enum ProductStatus
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
        ComplevelDESC,
        ModifiedDateDESC,
        ModifiedDate,
        CreatedDateDESC,
        CreatedDate,
        ViewCountDESC,
        ViewCount,
        ContactCountDESC,
        ContactCount
    }

    public enum SearchBy
    {
        ProductName,
        CompanyName
    }

    #endregion


    public class ProductService : BaseSC
    {

        #region Product

        #region Property
        public int? ViewCount { get; set; }
        public int? QuotationCount { get; set; }
        public int? TelCount { get; set; }
        public int ContactCount { get; set; }

        public decimal PriceShipment { get; private set; }
        #endregion

        #region Method Validate
        #region ValidateInsert
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool ValidateInsert(string ProductName, string ProductCode)
        {
            if (!string.IsNullOrEmpty(ProductName))
            {
                var count = CountData<b2bProduct>(" * ", CreateWhereAction(ProductAction.BackEnd) + " AND ProductName = N' " + ProductName + " ' ");
                if (count > 0)
                    IsResult = false;
                else
                    IsResult = true;
            }
            if (!string.IsNullOrEmpty(ProductCode))
            {
                var count = CountData<b2bProduct>(" * ", CreateWhereAction(ProductAction.BackEnd) + " AND ProductName = N' " + ProductCode + " ' ");
                if (count > 0)
                    IsResult = false;
                else
                    IsResult = true;
            }
            return IsResult;
        }
        #endregion

        #region ValidateFullProduct
        /// <summary>
        /// ตรวจสอบ ว่า สินค้าของ user เต็ม หรือไม่
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool ValidateFullProduct(int CompID, int CompLevel)
        {
            if (CompLevel == 1)
            {
                #region Check Is Max Product In Free Package
                var svPackage = new Order.PackageService();
                var max = svPackage.GetMaxProduct(1);
                var count = CountData<b2bProduct>(" * ", CreateWhereAction(ProductAction.All, CompID));

                if (count < max)
                    IsResult = true;
                else
                {
                    IsResult = false;
                    Exception ex = new Exception("Your package have full product. ");
                    MsgError.Add(ex);
                }
                #endregion
            }
            else
                IsResult = true;

            return IsResult;
        }
        #endregion

        #region ValidateUpdate
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool ValidateUpdate(string Upd_ProductName, string Old_ProductName, string Upd_ProductCode, string Old_ProductCode, int ProductID)
        {
            if (!string.IsNullOrEmpty(Upd_ProductName))
            {
                var count = CountData<b2bProduct>(" * ", CreateWhereAction(ProductAction.BackEnd) + "AND ProductID != " + ProductID + " AND ProductName = N'" + Upd_ProductName + "' ");

                if (count > 0)
                {
                    IsResult = false;
                    ArgumentException ex = new ArgumentException("This name has exist.");
                    MsgError.Add(ex);
                }
                else
                    IsResult = true;

            }

            if (!string.IsNullOrEmpty(Upd_ProductCode))
            {
                var count = CountData<b2bProduct>(" * ", CreateWhereAction(ProductAction.BackEnd) + "AND ProductID != " + ProductID + " AND ProductCode = N'" + Upd_ProductCode + "' ");
                if (count > 0)
                {
                    IsResult = false;
                    ArgumentException ex = new ArgumentException("This code has exist.");
                    MsgError.Add(ex);
                }
                else
                    IsResult = true;

            }
            return IsResult;
        }
        #endregion

        #endregion

        #region Method Select

        #region GetProduct
        #region Search

        public List<view_ProductMobileApp> SearchProduct(
            ProductAction action,
            string textsearch,
            int CateLV = 0,
            int CateID = 0,
            int Biztype = 0,
            int ProvinceID = 0,
            int CompID = 0,
            int pageindex = 1,
            int pagesize = 20,
            string SQLOrderBy = " Complevel DESC,   ListNo DESC  , ModifiedDate DESC  ")
        {
            SQLWhere = CreateWhereAction(action, 0);
            SQLWhere += CreateWhereCause(0, textsearch, 0, 0, CateLV, CateID, Biztype, 0, ProvinceID);



            SQLSelect = @" ProductID, ProductName, ShortDescription,   Price, Price_One, Qty, QtyUnit,";
            SQLSelect += " ProductImgPath,   CompID, ViewCount,ListNo , ";
            SQLSelect += "  CateLV3,  CreatedDate, ModifiedDate,  RowFlag, ";
            SQLSelect += "CompName, ContactMobile, Complevel,   ";
            SQLSelect += "ServiceType, CompProvinceID, CompDistrictID, BizTypeID, BizTypeOther, ContactCount, IsSME ";

            var Products = SelectData<view_ProductMobileApp>(SQLSelect, SQLWhere, SQLOrderBy, pageindex, pagesize);

            return Products;
        }
        #endregion

        #region Generate SQLWhere
        public string CreateWhereAction(ProductAction action, int? CompID = 0)
        {
            var sqlWhere = string.Empty;
            #region Condition
            if (action == ProductAction.All)
            {
                sqlWhere = "( IsDelete = 0 ) ";
            }
            else if (action == ProductAction.FrontEnd)
            {
                //comprowflag มาจาก b2bcompany.rowflag
                sqlWhere = "( IsDelete = 0 AND RowFlag IN (4,5,6) AND CompRowFlag IN (2,4) AND CompIsDelete = 0) AND ( IsShow = 1 AND IsJunk = 0 )";
            }
            else if (action == ProductAction.Home)
            {
                //comprowflag มาจาก b2bcompany.rowflag
                sqlWhere = "( IsDelete = 0 AND RowFlag IN (4,6) AND CompRowFlag IN (2,4) AND CompIsDelete = 0) AND ( IsShow = 1 AND IsJunk = 0 )";
            }
            else if (action == ProductAction.BackEnd)
            {
                sqlWhere = "( IsDelete = 0 AND  ( RowFlag >= 2 AND RowFlag <=6 ) AND  IsJunk = 0  )  ";
            }
            else if (action == ProductAction.Junk)
            {
                sqlWhere = "( IsDelete = 0 AND  ( RowFlag >= 2 AND RowFlag <=6 ) AND  IsJunk = 1 ) ";
            }
            else if (action == ProductAction.Admin)
            {
                sqlWhere = "( IsDelete = 0 AND   ( RowFlag >= 2 AND RowFlag <=6 ) AND CompRowFlag IN (2,4)  AND  IsJunk = 0 ) ";
            }
            else if (action == ProductAction.WebSite)
            {
                sqlWhere = "IsDelete = 0 ";
            }
            else if (action == ProductAction.Recommend)
            {
                sqlWhere = "( IsDelete = 0 AND RowFlag IN (4,5,6)) AND (IsShow =1 AND IsJunk = 0 AND ListNo > 0) ";
            }
            if (CompID > 0)
            {
                sqlWhere += "AND (CompID = " + CompID + ")";
            }

            #endregion

            return sqlWhere;
        }

        public string CreateWhereByCategory(int CategoryLevel, int CategoryID)
        {
            var sqlwhere = string.Empty;
            if (CategoryLevel == 1)
                sqlwhere = " AND CateLV1 = " + CategoryID;
            else if (CategoryLevel == 2)
                sqlwhere = " AND CateLV2 = " + CategoryID;
            else if (CategoryLevel == 3)
                sqlwhere = " AND CateLV3 = " + CategoryID;
            return sqlwhere;
        }

        public string CreateWhereByCategory(int CategoryLevel, List<int?> CategoryID)
        {
            var sqlwhere = string.Empty;
            if (CategoryID != null && CategoryID.Count > 0)
            {
                sqlwhere = "AND CateLV" + CategoryLevel + " IN ( ";
                var str = "";
                foreach (var item in CategoryID)
                {
                    str += item + ",";
                }
                str = str.Substring(0, str.Length - 1);
                sqlwhere += str + ") ";
            }

            return sqlwhere;
        }

        public string CreateWhereSearchBy(string txtSearch = "", string SearchType = "ProductName")
        {
            var SQLWhere = "";
            txtSearch = txtSearch.Trim();
            if (!string.IsNullOrEmpty(txtSearch))
            {
                if (SearchType == "CompName")
                {
                    SQLWhere += " AND CompName LIKE N'%" + txtSearch + "%' ";
                }
                else if (SearchType == "ProductName")
                {
                    SQLWhere += " AND ProductKeyword LIKE N'%" + txtSearch + "%' ";
                }
                else if (SearchType == "ProductID")
                {
                    SQLWhere += " AND ProductID = " + txtSearch + " ";
                }
                else if (SearchType == "CompCode")
                {
                    SQLWhere += " AND CompCode LIKE N'" + txtSearch + "%' ";
                }
                else if (SearchType == "AdminCode")
                {
                    SQLWhere += " AND AdminCode LIKE N'" + txtSearch + "%' ";
                }
                else
                {
                    SQLWhere += " AND CompName LIKE N'%" + txtSearch + "%' OR ProductName LIKE N'%" + txtSearch + "%' ";
                }
            }
            return SQLWhere;
        }

        public string CreateWhereCause(
            int CompID = 0, string txtSearch = "", int PStatus = 0, int GroupID = 0,
            int CateLevel = 0, int CateID = 0, int BizTypeID = 0, int CompLevel = 0,
            int CompProvinceID = 0
            )
        {
            #region DoWhereCause
            SQLWhere = string.Empty;
            if (CompID > 0)
                SQLWhere += " AND CompID = " + CompID;

            if (!string.IsNullOrEmpty(txtSearch))
            {
                string[] keywords = txtSearch.Split(' ');
                foreach (string keyword in keywords)
                {
                    //SQLWhere += " AND ProductName LIKE N'%" + keyword + "%' ";
                    SQLWhere += "AND (ProductName LIKE N'%" + keyword + "%')";//ProductKeyword LIKE N'%" + keyword + "%' or 
                }
            }

            if (PStatus > 0)
                SQLWhere += " And RowFlag = " + PStatus;


            if (GroupID > 0)
                SQLWhere += " AND ProductGroupID  =  " + GroupID;

            if (CateID > 0)
                SQLWhere += CreateWhereByCategory(CateLevel, CateID);

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
                case OrderBy.ContactCountDESC:
                    SqlOrderBy = "ContactCount DESC";
                    break;
                case OrderBy.ContactCount:
                    SqlOrderBy = "ContactCount";
                    break;
                //case OrderBy.ComplevelDESC:
                //    SqlOrderBy = ",Complevel DESC";
                //    break;
            }
            #endregion
            return SqlOrderBy;
        }
        #endregion

        #region  GetProductRelateByProduct
        /// <summary>
        /// 
        /// 
        /// </summary>
        /// <param name="productid"></param>
        /// <param name="numtake">จำนวนที่ต้องการกี่ ชิ้น</param>
        /// <returns></returns>
        public List<view_SearchProduct> GetProductRelateByProduct(int productid, int numtake)
        {

            var search = new List<view_SearchProduct>();
            var products = SelectData<b2bProduct>("*", " ProductID = " + productid, null, 1, 1);
            if (products.Count != null && products.Count > 0)
            {
                var p = products.First();
                var keyword = p.ProductKeyword.Split('~').ToList();
                var whereKeyword = SQLWhereListString(keyword, "ProductKeyword");
                search = SelectData<view_SearchProduct>("*", "IsDelete = 0 AND ProductKeyword LIKE N'%" + keyword[0] + "%' and productID != " + productid, null, 1, numtake);

            }
            return search;
        }
        #endregion

        #region  GetProductRByProductID
        /// <summary>
        /// 
        /// 
        /// </summary>
        /// <param name="productid"></param>
        /// <param name="numtake">จำนวนที่ต้องการกี่ ชิ้น</param>
        /// <returns></returns>
        public List<view_SearchProduct> GetProductRByProductID(int productid, int numtake)
        {

            var search = new List<view_SearchProduct>();
            //var products = SelectData<b2bProduct>("*", " ProductID = " + productid, null, 1, 1);
            //if (products.Count != null && products.Count > 0)
            //{
            return search = SelectData<view_SearchProduct>("*", "IsDelete = 0 AND productID = " + productid, null, 1, numtake);

            //}

        }
        #endregion
        #endregion

        #endregion

        #region Method Insert
        #region InsertProduct
        public bool InsertProduct(b2bProduct product, List<b2bProductImage> productImages, int CompID, int CompLevel)
        {
            // bom ยังไม่ได้ทดสอบ method ครับ
            if (ValidateFullProduct(CompID, CompLevel))
            {
                using (var trans = new TransactionScope())
                {
                    qDB.b2bProducts.InsertOnSubmit(product);
                    qDB.SubmitChanges();
                    foreach (var img in productImages)
                    {
                        img.ProductID = product.ProductID;
                        qDB.b2bProductImages.InsertOnSubmit(img);
                        qDB.SubmitChanges();
                    }

                    trans.Complete();
                    IsResult = true;
                }

            }
            return IsResult;
        }

        #region ValidateSave
        #region ValidateProduct
        private bool ValidateProduct(b2bProduct model)
        {
            //Example
            if (model.ProductName == null)
            {
                IsResult = false;
            }
            else if (model.ProductNameEng == null)
            {
                IsResult = false;
            }

            return IsResult;
        }
        #endregion
        #endregion

        #region SaveProduct
        #region Save Model
        public bool SaveProduct(b2bProduct model)
        {
            IsResult = true;
            if (!ValidateProduct(model))
                return IsResult;

            using (var trans = new TransactionScope())
            {
                try
                {
                    if (model.ProductID > 0)
                    {
                        qDB.b2bProducts.Context.Refresh(RefreshMode.KeepCurrentValues, model);
                        qDB.b2bProducts.InsertOnSubmit(model);// ทำการ save
                        qDB.SubmitChanges();


                    }
                    else
                    {
                        qDB.b2bProducts.InsertOnSubmit(model);
                        qDB.SubmitChanges();

                        foreach (var m in model.b2bProductImages)
                        {
                            m.ProductID = model.ProductID;
                            qDB.b2bProductImages.InsertOnSubmit(m);
                            qDB.SubmitChanges();
                        }
                    }
                    trans.Complete();
                }
                catch (Exception ex)
                {
                    IsResult = false;
                    MsgError.Add(ex);
                }
            }

            return IsResult;
        }
        #endregion

        #region Save Model List
        public bool SaveProduct(List<b2bProduct> lstmodel)
        {
            IsResult = true;
            foreach (var model in lstmodel)
            {
                if (!ValidateProduct(model))
                    return IsResult;
            }

            using (var trans = new TransactionScope())
            {
                try
                {
                    foreach (var model in lstmodel)
                    {
                        if (model.ProductID > 0)
                            qDB.b2bProducts.Context.Refresh(RefreshMode.KeepCurrentValues, model);
                        else
                            qDB.b2bProducts.InsertOnSubmit(model);

                        qDB.SubmitChanges();
                    }
                    trans.Complete();
                }
                catch (Exception ex)
                {
                    IsResult = false;
                    MsgError.Add(ex);
                }
            }

            return IsResult;
        }
        #endregion
        #endregion
        #endregion
        #endregion

        #region Update

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateProduct(b2bProduct model, List<int> ImgID, List<string> ImgName)
        {
            var data = qDB.b2bProducts.Single(q => q.ProductID == model.ProductID);

            #region Set Model
            // set ค่า model
            data.ProductName = model.ProductName;
            data.ProductGroupID = model.ProductGroupID;
            data.ProductCode = model.ProductCode;
            data.Price = model.Price;
            data.Price_One = model.Price_One;
            data.ProductKeyword = model.ProductKeyword;
            data.ShortDescription = model.ShortDescription;
            data.ProductDetail = model.ProductDetail;
            data.Qty = model.Qty;
            data.QtyUnit = model.QtyUnit;
            if (model.CateLV1 != null)
            {
                data.CateLV1 = model.CateLV1;
            }
            if (model.CateLV2 != null)
            {
                data.CateLV2 = model.CateLV2;
            }
            if (model.CateLV3 != null)
            {
                data.CateLV3 = model.CateLV3;
            }
            data.ProductImgPath = model.ProductImgPath;
            data.RowFlag = model.RowFlag;
            data.CompCode = model.CompCode;
            // default
            data.RowVersion++;
            data.ModifiedBy = model.ModifiedBy;
            data.ModifiedDate = DateTimeNow;
            #endregion

            #region Save
            using (var trans = new TransactionScope())
            {

                qDB.SubmitChanges();// บันทึกค่า Product

                var OldImgs = SelectData<b2bProductImage>(" * ", " IsDelete = 0 AND ProductID =" + model.ProductID);
                OldFiles = new List<string>();
                NewFiles = new List<string>();
                OldID = new List<int>();

                foreach (var it in OldImgs)
                {
                    OldID.Add((int)it.ProductImageID);
                    OldFiles.Add(it.ImgPath);
                }


                for (var i = 0; i < ImgName.Count; i++)
                {

                    if (ImgID[i] > 0)
                    {
                        if (OldID.Exists(m => m.Equals(ImgID[i])))
                            OldID.Remove(ImgID[i]);

                        UpdateByCondition<b2bProductImage>(@" ImgPath = N'" + ImgName[i] + "' , ListNo = " + i + ", "
                        + " ImgDetail = N'" + model.ProductName + "' ",
                            " ProductImageID = " + ImgID[i]);
                    }
                    else
                    {
                        #region กรณีเป็นรูป ใหม่ ให้ insert ข้อมูล
                        var it = new b2bProductImage();
                        it.ProductID = model.ProductID;
                        it.ImgPath = ImgName[i];
                        it.ImgDetail = model.ProductName;
                        it.ListNo = i;
                        it.IsShow = true;
                        it.IsDelete = false;
                        it.CreatedDate = DateTimeNow;
                        it.ModifiedDate = DateTimeNow;
                        it.CreatedBy = "sa";
                        it.ModifiedBy = "sa";
                        qDB.b2bProductImages.InsertOnSubmit(it);
                        qDB.SubmitChanges();
                        #endregion
                    }
                }

                foreach (var it in OldID)
                {
                    UpdateByCondition<b2bProductImage>("IsDelete = 1 ", " ProductImageID = " + it);
                }

                trans.Complete();
                IsResult = true;
            }
            #endregion

            return IsResult;
        }

        public List<int> OldID { get; set; }
        public List<string> OldFiles { get; set; }
        public List<string> NewFiles { get; set; }

        #region UpdateProductCountInCategories

        public bool UpdateProductCountInCategories(int CateLV1, int CateLV2, int CateLV3)
        {
            var svCategory = new Category.CategoryService();
            if (CateLV3 != 0)
            {
                IsResult = svCategory.UpdateProductCount(3, CateLV3);
            }
            if (CateLV1 != 0)
            {
                IsResult = svCategory.UpdateProductCount(2, CateLV2);
            }
            if (CateLV2 != 0)
            {
                IsResult = svCategory.UpdateProductCount(1, CateLV1);
            }
            return IsResult;
        }

        public bool UpdateProductCountInCategories(List<int> CateLV1, List<int> CateLV2, List<int> CateLV3)
        {
            var svCategory = new Category.CategoryService();

            var lstCateLV1 = CateLV1.Distinct();
            var lstCateLV2 = CateLV2.Distinct();
            var lstCateLV3 = CateLV3.Distinct();

            foreach (var it in lstCateLV3)
            {
                IsResult = svCategory.UpdateProductCount(3, it);
            }

            foreach (var it in lstCateLV2)
            {
                IsResult = svCategory.UpdateProductCount(2, it);
            }

            foreach (var it in lstCateLV1)
            {
                IsResult = svCategory.UpdateProductCount(1, it);
            }

            return IsResult;
        }
        #endregion

        #region UpdateProductCountInCompany
        public bool UpdateProductCountInCompany(int CompID)
        {
            var sqlupdate = @"productcount = ( select COUNT(productid) from b2bProduct " +
                "where CompID = " + CompID + " and RowFlag IN ( 4,5,6) and IsDelete = 0)";

            UpdateByCondition<b2bCompany>(sqlupdate, "CompID = " + CompID);

            return IsResult;
        }

        public bool UpdateProductCountInCompany(List<int> CompID)
        {

            var sqlupdate = string.Empty;
            foreach (var item in CompID)
            {
                UpdateProductCountInCompany(item);
            }

            return IsResult;
        }
        #endregion

        #region UpdateProductViewCount
        public bool UpdateProductViewCount(int ProductID)
        {
            var Product = SelectData<b2bProduct>("ViewCount,ProductID", "ProductID = " + ProductID).First();

            if (Product.ViewCount == 0)
                ViewCount = 1;
            else
                ViewCount = (int)Product.ViewCount + 1;

            string sqlUpdate = " ViewCount = " + ViewCount + ",ModifiedDate =GETDATE()";
            string sqlWhere = " ProductID = " + ProductID;
            UpdateByCondition<b2bProduct>(sqlUpdate, sqlWhere);
            return IsResult;
        }
        #endregion

        #region UpdateTelCount
        public bool UpdateTelCount(int ProductID)
        {
            var Product = SelectData<b2bProduct>("TelCount,ProductID", "ProductID = " + ProductID).First();

            if (Product.TelCount == 0)
                TelCount = 1;
            else
                TelCount = (int)Product.TelCount + 1;

            string sqlUpdate = " TelCount = " + TelCount + ",ModifiedDate =GETDATE()";
            string sqlWhere = " ProductID = " + ProductID;
            UpdateByCondition<b2bProduct>(sqlUpdate, sqlWhere);
            return IsResult;
        }
        #endregion

        #region UpdatQuotationCount
        public bool UpdateQuotationCount(int ProductID)
        {
            var Product = SelectData<b2bProduct>("QuotationCount,ProductID", "ProductID = " + ProductID).First();

            if (Product.QuotationCount == 0)
                QuotationCount = 1;
            else
                QuotationCount = (int)Product.QuotationCount + 1;

            string sqlUpdate = " QuotationCount = " + QuotationCount + ",ModifiedDate =GETDATE()";
            string sqlWhere = " ProductID = " + ProductID;
            UpdateByCondition<b2bProduct>(sqlUpdate, sqlWhere);
            return IsResult;
        }
        #endregion

        #region Update ContactCount
        public void AddContactCount(int ProductID)
        {
            var SelectViewCount = SelectData<b2bProduct>("ContactCount,ProductID", "ProductID = " + ProductID).First();
            if (SelectViewCount.ContactCount == 0)
                ContactCount = 1;
            else
                ContactCount = (int)SelectViewCount.ContactCount + 1;

            string sqlUpdate = " ContactCount = " + ContactCount;
            string sqlWhere = " ProductID = " + ProductID;
            UpdateByCondition<b2bProduct>(sqlUpdate, sqlWhere);
        }
        #endregion

        #region ReStore
        public bool ReStore(List<int> ProductID, List<int> CateLV1, List<int> CateLV2, List<int> CateLV3, int CompID)
        {
            var svCompany = new Company.CompanyService();

            var Contains = SQLWhereListInt(ProductID, "ProductID");
            UpdateByCondition<b2bProduct>(" ModifiedDate  = GETDATE()  , IsJunk = 0 , ListNo = 0 ", " CompID = " + CompID + " AND " + Contains);

            IsResult = svCompany.UpdateProductCount(CompID);
            UpdateProductCountInCategories(CateLV1, CateLV2, CateLV3);
            UpdateProductCountInCompany(CompID);

            return IsResult;
        }
        #endregion

        #region MoveToJunk
        public bool MoveToJunk(List<int> ProductID, List<int> CateLV1, List<int> CateLV2, List<int> CateLV3, int CompID)
        {
            var svCompany = new Company.CompanyService();

            var Contains = SQLWhereListInt(ProductID, "ProductID");
            UpdateByCondition<b2bProduct>(" ModifiedDate  = GETDATE()  , IsJunk = 1 , ListNo = 0 ", " CompID = " + CompID + " AND " + Contains);

            IsResult = svCompany.UpdateProductCount(CompID);
            UpdateProductCountInCategories(CateLV1, CateLV2, CateLV3);
            UpdateProductCountInCompany(CompID);

            return IsResult;
        }
        #endregion

        #region Delete

        public bool Delete(int ProductID, int CateLV1, int CateLV2, int CateLV3, int CompID = 0)
        {
            var svCompany = new Company.CompanyService();
            var svCategory = new Category.CategoryService();

            var sqlWhere = "ProductID = " + ProductID;

            if (CompID > 0)
                sqlWhere += " AND CompID = " + CompID;

            UpdateByCondition<b2bProduct>(" ModifiedDate  = GETDATE()  , IsDelete = 1   , ListNo = 0  ", sqlWhere);
            if (CompID > 0)
            {
                IsResult = svCompany.UpdateProductCount(CompID);
            }
            UpdateProductCountInCategories(CateLV1, CateLV2, CateLV3);
            UpdateProductCountInCompany(CompID);


            return IsResult;
        }

        public bool Delete(List<int> ProductID, List<int> CateLV1, List<int> CateLV2, List<int> CateLV3, int CompID = 0)
        {
            var svCompany = new Company.CompanyService();
            var svCategory = new Category.CategoryService();

            var sqlWhere = SQLWhereListInt(ProductID, "ProductID");

            if (CompID > 0)
                sqlWhere += " AND CompID = " + CompID;

            UpdateByCondition<b2bProduct>(" ModifiedDate  = GETDATE()  , IsDelete = 1   , ListNo = 0  ", sqlWhere);
            if (CompID > 0)
            {
                IsResult = svCompany.UpdateProductCount(CompID);
            }
            UpdateProductCountInCategories(CateLV1, CateLV2, CateLV3);
            UpdateProductCountInCompany(CompID);


            return IsResult;
        }
        #endregion

        #region ChangeGroup
        public bool ChangeGroup(int CompID, int OldGroupID, int NewGroupID)
        {
            UpdateByCondition<b2bProduct>("  ProductGroupID = " + NewGroupID + " ", " CompID = " + CompID + " AND ProductGroupID = " + OldGroupID);
            return IsResult;
        }
        #endregion

        #region SaveShow
        public bool SaveIsShow(List<int> ProductID, List<int> CateLV1, List<int> CateLV2, List<int> CateLV3, int CompID, int IsShow)
        {
            var Contains = SQLWhereListInt(ProductID, "ProductID");

            if (IsShow > 1)
                IsShow = 0;

            UpdateByCondition<b2bProduct>(" ModifiedDate  = GETDATE()  , IsShow = " + IsShow, " CompID = " + CompID + " AND " + Contains);
            UpdateProductCountInCategories(CateLV1, CateLV2, CateLV3);
            UpdateProductCountInCompany(CompID);
            return IsResult;
        }
        #endregion

        #region Changegroup
        public bool ChangeGroup(int ProductID, int GroupID)
        {
            UpdateByCondition<b2bProduct>("  ProductGroupID = " + GroupID, " ProductID = " + ProductID);
            return IsResult;
        }
        #endregion

        #region ApproveProduct
        public bool ApproveProduct(List<int> ProductID, List<int> CateLV1, List<int> CateLV2, List<int> CateLV3, string CompCode, List<int> CompID = null)
        {
            var Contains = SQLWhereListInt(ProductID, "ProductID");
            UpdateByCondition<b2bProduct>("  ModifiedDate  = GETDATE() , RowFlag = 4 , modifiedby = N'" + CompCode + "'", Contains);
            UpdateProductCountInCategories(CateLV1, CateLV2, CateLV3);
            if (CompID != null)
            {
                UpdateProductCountInCompany(CompID);
            }

            return IsResult;
        }
        #endregion

        #region RejectProduct
        public bool RejectProduct(List<int> ProductID, List<int> CateLV1, List<int> CateLV2, List<int> CateLV3, string Remark, string CompCode, List<int> CompID = null)
        {
            var Contains = SQLWhereListInt(ProductID, "ProductID");
            UpdateByCondition<b2bProduct>(" ModifiedDate  = GETDATE()  , RowFlag = 3 , remark = N'" + Remark + "', modifiedby = N'" + CompCode + "'", Contains);
            UpdateProductCountInCategories(CateLV1, CateLV2, CateLV3);
            return IsResult;
        }
        #endregion

        #region Product Recommend
        public int RecommendCount { get; set; }
        #region DuplicateRecommend
        public bool DuplicateRecommend(int ProductID, int CompID)
        {
            var sqlWhere = CreateWhereAction(ProductAction.BackEnd, CompID);
            sqlWhere += " And ListNo > 0 AND ProductID = " + ProductID;
            var CountProduct = CountData<b2bProduct>(" ProductID ", sqlWhere);
            if (CountProduct > 0)
            {
                IsResult = false;
                ArgumentException ex = new ArgumentException(" สินค้าแนะนำซ้ำ. ");
                MsgError.Add(ex);
            }
            else
                IsResult = true;

            return IsResult;
        }
        #endregion

        #region ValidateFullRecommend
        public bool ValidateFullRecommend(int CompID)
        {
            RecommendCount = CountData<b2bProduct>(" * ", CreateWhereAction(ProductAction.Recommend, CompID));
            if (RecommendCount >= 20)
            {
                IsResult = false;
                ArgumentException ex = new ArgumentException("สินค้าแนะเกินจำนวนที่ได้กำหนด. ");
                MsgError.Add(ex);
            }
            else
            {
                IsResult = true;
            }

            return IsResult;
        }
        #endregion

        #region MoveToRecommend
        public bool MoveToRecommend(List<int> ProductID, int CompID)
        {
            if (ValidateFullRecommend(CompID))
            {
                #region Check Over Recommend
                var RecommendBalance = 20 - RecommendCount;
                if (ProductID.Count() > RecommendBalance)
                {
                    IsResult = false;
                    ArgumentException ex = new ArgumentException(" Your Can Add " + RecommendBalance + " Items");
                    MsgError.Add(ex);
                }
                #endregion

                if (IsResult)
                {
                    var sqlWhere = SQLWhereListInt(ProductID, "ProductID");
                    UpdateByCondition<b2bProduct>(" ModifiedDate  = GETDATE()  , ListNo = 21 , IsShow = 1 ", sqlWhere);
                }

            }

            return IsResult;
        }
        public bool MoveToRecommend(int ProductID, int CateLV1, int CateLV2, int CateLV3, int CompID)
        {
            if (DuplicateRecommend(ProductID, CompID))
            {
                ValidateFullRecommend(CompID);//ตรวจสอบว่า recommend เต็ม หรือเปล่า
                if (IsResult)
                {
                    UpdateByCondition<b2bProduct>(" ModifiedDate  = GETDATE()  , ListNo = 21 , IsShow = 1", " ProductID = " + ProductID);
                    UpdateProductCountInCategories(CateLV1, CateLV2, CateLV3);
                }
            }
            return IsResult;
        }
        #endregion


        public bool SaveChangeListNo(List<int> ProductID, int CompID)
        {

            using (var trans = new TransactionScope())
            {

                var sqlWhere = SQLWhereListInt(ProductID, "ProductID");
                for (var i = 1; i <= ProductID.Count(); i++)
                {
                    UpdateByCondition<b2bProduct>(" ListNo =" + i + " , IsShow = 1 , IsDelete = 0 ", "CompID = " + CompID + " AND ProductID = " + ProductID[i - 1]);

                }

                trans.Complete();
            }

            return IsResult;
        }


        #region MoveToStore
        public bool MoveToStore(int ProductID, int CompID)
        {
            UpdateByCondition<b2bProduct>(" ModifiedDate  = GETDATE()  , ListNo = 0 , IsShow = 1", " ProductID = " + ProductID);
            return IsResult;
        }
        #endregion

        #endregion

        #region MoveProductInCate
        public bool MoveProductInCate(int CateLV, int OldCateID, int NewCateID, int CateLV3ID)
        {
            if (OldCateID > 0 && CateLV > 0)
            {
                var sqlWhere = "IsDelete = 0 ";
                if (CateLV == 3)
                {
                    sqlWhere += "AND CateLV3 = " + OldCateID;
                }
                else if (CateLV == 2)
                {
                    sqlWhere += "AND CateLV3 = " + CateLV3ID + " AND CateLV2 = " + OldCateID;
                }
                else if (CateLV == 1)
                {
                    sqlWhere += "AND CateLV3 = " + CateLV3ID + " AND CateLV1 = " + OldCateID;
                }

                UpdateByCondition<b2bProduct>("CateLV" + CateLV + " = " + NewCateID, sqlWhere);

            }
            return IsResult;
        }
        #endregion

        #region MoveProductInCateLV
        public bool MoveProductInCateLV(int oldcatelv1, int oldcatelv2, int oldcatelv3, int newcatelv1, int newcatelv2, int newcatelv3)
        {
            var svCategory = new Category.CategoryService();

            using (var trans = new TransactionScope())
            {
                MoveProductInCate(3, oldcatelv3, newcatelv3, oldcatelv3);
                MoveProductInCate(2, oldcatelv2, newcatelv2, newcatelv3);
                MoveProductInCate(1, oldcatelv1, newcatelv1, newcatelv3);

                trans.Complete();
                IsResult = true;
            }

            using (var trans = new TransactionScope())
            {

                #region Update Product Count

                svCategory.UpdateProductCount(3, oldcatelv3);
                svCategory.UpdateProductCount(2, oldcatelv2);
                svCategory.UpdateProductCount(1, oldcatelv1);
                svCategory.UpdateProductCount(3, newcatelv3);
                svCategory.UpdateProductCount(2, newcatelv2);
                svCategory.UpdateProductCount(1, newcatelv1);

                #endregion

                trans.Complete();
                IsResult = true;
            }

            return IsResult;
        }
        #endregion

        #endregion

        #region Save Keyword
        public bool InsertKeyword(string Keyword, int ProductID)
        {
            try
            {
                #region validate

                var model = new tbKeyword();

                Keyword = Keyword.Replace(" ", "").Replace("\n", "");
                tbKeyword data = qDB.tbKeywords.Where(m => m.KeywordName == Keyword).FirstOrDefault();

                if (!string.IsNullOrEmpty(Keyword))
                {
                    if (data != null)
                    {
                        SaveProductKeyword(data.KeywordID, ProductID);
                    }
                    else
                    {
                        model.KeywordName = Keyword;
                        model.IsDelete = false;
                        model.RowVersion = 1;
                        model.ModifiedBy = "sa";
                        model.CreatedBy = "sa";
                        model.CreatedDate = DateTimeNow;
                        model.ModifiedDate = DateTimeNow;
                        qDB.tbKeywords.InsertOnSubmit(model);
                        qDB.SubmitChanges();
                        SaveProductKeyword(model.KeywordID, ProductID);
                    }
                }
                //    if(data != null && data.
                #endregion
            }
            catch (Exception ex)
            {

                throw;
            }
            return IsResult;
        }
        #endregion

        #endregion
        public bool SaveProductKeyword(int KeywordID, int ProductID)
        {
            try
            {
                #region Set & Save
                var data = qDB.tbProductKeywords.Where(m => m.KeywordID == KeywordID && m.ProductID == ProductID).Count();
                if (data == 0)
                {
                    var model = new tbProductKeyword();
                    model.ProductID = ProductID;
                    model.KeywordID = KeywordID;
                    model.CreatedBy = "sa";
                    model.CreatedDate = DateTimeNow;
                    model.ModifiedBy = "sa";
                    model.ModifiedDate = DateTimeNow;
                    qDB.tbProductKeywords.InsertOnSubmit(model);
                    qDB.SubmitChanges();
                    IsResult = true;
                }
                #endregion
            }
            catch (Exception ex)
            {
                IsResult = false;
                throw;
            }
            return IsResult;
        }
    }
}