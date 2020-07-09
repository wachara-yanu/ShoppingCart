using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using Prosoft.Base;
using System.Transactions;
//using System.Web.Mvc;
using System.Data.Linq;
using Prosoft.Service;

namespace Ouikum.Buylead
{

    #region enum
    public enum BuyleadAction
    {
        All,
        BackEnd,
        FrontEnd,
        Junk,
        Admin,
        WebSite,
        Recommend
    }
    public enum BuyleadStatus
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

    public enum SearchBy
    {
        BuyleadName,
        CompanyName
    }

    #endregion


    public class BuyleadService : BaseSC
    {

        #region Buylead

        #region Property
        public int? ViewCount { get; set; }
        public int ContactCount { get; set; }
        #endregion

        #region Method Validate
        #region ValidateInsert
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool ValidateInsert(string BuyleadName, string BuyleadCode,string BuyleadExpire)
        {
            if (!string.IsNullOrEmpty(BuyleadName))
            {
                var count = CountData<b2bBuylead>(" * ", CreateWhereAction(BuyleadAction.BackEnd) + " AND BuyleadName = N' " + BuyleadName + " ' ");
                if (count > 0)
                    IsResult = false;
                else
                    IsResult = true;
            }
            if (!string.IsNullOrEmpty(BuyleadCode))
            {
                var count = CountData<b2bBuylead>(" * ", CreateWhereAction(BuyleadAction.BackEnd) + " AND BuyleadName = N' " + BuyleadCode + " ' ");
                if (count > 0)
                    IsResult = false;
                else
                    IsResult = true;
            }
            if (!string.IsNullOrEmpty(BuyleadExpire))
            {
                var dateNow = DateTime.Today.ToString("yyyyMMdd", new System.Globalization.CultureInfo("en-US"));
                int Exp = Convert.ToInt32(BuyleadExpire);
                int Now = Convert.ToInt32(dateNow);
                if (Now >= Exp)
                    IsResult = false;
                else
                    IsResult = true;
            }
            return IsResult;
        }
        #endregion

        #region ValidateFullBuylead
        /// <summary>
        /// ตรวจสอบ ว่า สินค้าของ user เต็ม หรือไม่
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns
        #endregion

        #region ValidateUpdate
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool ValidateUpdate(string Upd_BuyleadName, string Old_BuyleadName, string Upd_BuyleadCode, string Old_BuyleadCode, string BuyleadExpire,int BuyleadID)
        {
            if (!string.IsNullOrEmpty(Upd_BuyleadName))
            {
                var count = CountData<b2bBuylead>(" * ", CreateWhereAction(BuyleadAction.BackEnd) + "AND BuyleadID != " + BuyleadID + " AND BuyleadName = N'" + Upd_BuyleadName + "' ");

                if (count > 0)
                {
                    IsResult = false;
                    ArgumentException ex = new ArgumentException("This name has exist.");
                    MsgError.Add(ex);
                }
                else
                    IsResult = true;

            }

            if (!string.IsNullOrEmpty(Upd_BuyleadCode))
            {
                var count = CountData<b2bBuylead>(" * ", CreateWhereAction(BuyleadAction.BackEnd) + "AND BuyleadID != " + BuyleadID + " AND BuyleadCode = N'" + Upd_BuyleadCode + "' ");
                if (count > 0)
                {
                    IsResult = false;
                    ArgumentException ex = new ArgumentException("This code has exist.");
                    MsgError.Add(ex);
                }
                else
                    IsResult = true;

            }
            if (!string.IsNullOrEmpty(BuyleadExpire))
            {
                var dateNow = DateTime.Today.ToString("yyyyMMdd", new System.Globalization.CultureInfo("en-US"));
                int Exp = Convert.ToInt32(BuyleadExpire);
                int Now = Convert.ToInt32(dateNow);
                if (Now >= Exp)
                    IsResult = false;
                else
                    IsResult = true;
            }
            return IsResult;
        }
        #endregion

        #endregion

        #region Method Select
        #region GetBuylead

        #region Generate SQLWhere
        public string CreateWhereAction(BuyleadAction action, int? CompID = 0)
        {
            var sqlWhere = string.Empty;
            #region Condition
            if (action == BuyleadAction.All)
            {
                sqlWhere = "( IsDelete = 0 ) ";
            }
            else if (action == BuyleadAction.FrontEnd)
            {
                //comprowflag มาจาก b2bcompany.rowflag
                sqlWhere = "( IsDelete = 0 AND RowFlag IN (4)) AND ( IsShow = 1 AND IsJunk = 0 )";
            }
            else if (action == BuyleadAction.BackEnd)
            {
                sqlWhere = "( IsDelete = 0 AND  ( RowFlag >= 2 AND RowFlag <=6 ) AND  IsJunk = 0  )  ";
            }
            else if (action == BuyleadAction.Junk)
            {
                sqlWhere = "( IsDelete = 0 AND  ( RowFlag >= 2 AND RowFlag <=6 ) AND  IsJunk = 1 ) ";
            }
            else if (action == BuyleadAction.Admin)
            {
                sqlWhere = "( IsDelete = 0 AND   ( RowFlag >= 2 AND RowFlag <=6 ) AND  IsJunk = 0 ) ";
            }
            else if (action == BuyleadAction.WebSite)
            {
                sqlWhere = "( IsDelete = 0 AND  RowFlag IN (2,4,5,6) ) AND ( IsShow = 1 AND IsJunk = 0 ) ";
            }
            else if (action == BuyleadAction.Recommend)
            {
                sqlWhere = "( IsDelete = 0 AND RowFlag IN (4,5,6)) AND ( IsShow = 1 AND IsJunk = 0 ) AND (ListNo > 0)";
            }
            if (CompID > 0)
                sqlWhere += "AND (CompID = " + CompID + ")";
            #endregion

            return sqlWhere;
        }

        public string CreateWhereByCategory(int CategoryLevel, int CategoryID)
        {
            var sqlwhere = string.Empty;
            if (CategoryLevel == 1)
                sqlwhere = "AND CateLV1 = " + CategoryID;
            else if (CategoryLevel == 2)
                sqlwhere = "AND CateLV2 = " + CategoryID;
            else if (CategoryLevel == 3)
                sqlwhere = "AND CateLV3 = " + CategoryID;
            return sqlwhere;
        }


        public string CreateWhereSearchBy(string txtSearch = "", string SearchType = "ProductName")
        { 
            txtSearch = txtSearch.Trim();
            if (!string.IsNullOrEmpty(txtSearch))
            {
                if (SearchType == "CompName")
                {
                    SQLWhere += " AND CompName LIKE N'%" + txtSearch + "%' ";
                }
                else if (SearchType == "BuyleadName")
                {
                    SQLWhere += " AND BuyleadName LIKE N'%" + txtSearch + "%' ";
                }
                else if (SearchType == "CompCode")
                {
                    SQLWhere += " AND CompCode LIKE N'%" + txtSearch + "%' ";
                }
                else if (SearchType == "AdminCode")
                {
                    SQLWhere += " AND AdminCode LIKE N'%" + txtSearch + "%' ";
                }
                else if (SearchType == "Email")
                {
                    SQLWhere += " AND BuyleadEmail LIKE N'%" + txtSearch + "%' ";
                }
            }
            return SQLWhere;

        } 

        public string CreateWhereCause(
            int CompID = 0, string txtSearch = "", int PStatus = 0, 
            int CateLevel = 0, int CateID = 0, int BuyleadType = 0, int CompLevel = 0,
            int CompProvinceID = 0
            )
        {
            #region DoWhereCause
            if (CompID > 0)
                SQLWhere += " AND CompID = " + CompID;

            if (!string.IsNullOrEmpty(txtSearch))
                SQLWhere += " AND BuyleadName LIKE N'" + txtSearch + "%' ";


            if (PStatus > 0)
                SQLWhere += " And RowFlag = " + PStatus;


            if (CateID > 0)
                SQLWhere += CreateWhereByCategory(CateLevel, CateID);

            if (BuyleadType > 0)
                SQLWhere += " AND (BuyleadType = " + BuyleadType + ")";

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
        #region InsertBuylead
        public bool InsertBuylead(b2bBuylead Buylead,  int CompID)
        {
            // bom ยังไม่ได้ทดสอบ method ครับ
           
                using (var trans = new TransactionScope())
                {
                    qDB.b2bBuyleads.InsertOnSubmit(Buylead);
                    qDB.SubmitChanges();

                    trans.Complete();
                    IsResult = true;
                }
            return IsResult;
        }

        #region ValidateSave
        #region ValidateBuylead
        private bool ValidateBuylead(b2bBuylead model)
        {
            //Example
            if (model.BuyleadName == null)
            {
                IsResult = false;
            }
            else if (model.BuyleadNameEng == null)
            {
                IsResult = false;
            }

            return IsResult;
        }
        #endregion
        #endregion

        #region SaveBuylead
        #region Save Model
        public bool SaveBuylead(b2bBuylead model)
        {


            IsResult = true;
            if (!ValidateBuylead(model))
                return IsResult;

            using (var trans = new TransactionScope())
            {
                try
                {
                    if (model.BuyleadID > 0)
                    {
                        qDB.b2bBuyleads.Context.Refresh(RefreshMode.KeepCurrentValues, model);
                        qDB.b2bBuyleads.InsertOnSubmit(model);// ทำการ save
                        qDB.SubmitChanges();


                    }
                    else
                    {
                        qDB.b2bBuyleads.InsertOnSubmit(model);
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

        #region Save Model List
        public bool SaveBuylead(List<b2bBuylead> lstmodel)
        {
            IsResult = true;
            foreach (var model in lstmodel)
            {
                if (!ValidateBuylead(model))
                    return IsResult;
            }

            using (var trans = new TransactionScope())
            {
                try
                {
                    foreach (var model in lstmodel)
                    {
                        if (model.BuyleadID > 0)
                            qDB.b2bBuyleads.Context.Refresh(RefreshMode.KeepCurrentValues, model);
                        else
                            qDB.b2bBuyleads.InsertOnSubmit(model);

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
        public bool UpdateBuylead(b2bBuylead model)
        {
            var data = qDB.b2bBuyleads.Single(q => q.BuyleadID == model.BuyleadID);

            #region Set Model
            // set ค่า model BuyleadInfo
            data.BuyleadName = model.BuyleadName;
            data.BuyleadCode = model.BuyleadCode;
            data.BuyleadType = model.BuyleadType;
            data.BuyleadExpDate=model.BuyleadExpDate;
            data.BuyleadKeyword = model.BuyleadKeyword;
            data.BuyleadDetail = model.BuyleadDetail;
            data.Qty = model.Qty;
            data.QtyUnit = model.QtyUnit;
            data.CateLV1 = model.CateLV1;
            data.CateLV2 = model.CateLV2;
            data.CateLV3 = model.CateLV3;
            data.BuyleadIMGPath = model.BuyleadIMGPath;
            //set ค่า  Model Contact Info
            data.BuyleadCompanyName = model.BuyleadCompanyName;
            data.BuyleadContactPerson = model.BuyleadContactPerson;
            data.BuyleadContactPosition = model.BuyleadContactPosition;
            data.BuyleadTel=model.BuyleadTel;
            data.BuyleadEmail=model.BuyleadEmail;
            data.BuyleadMobilePhone=model.BuyleadMobilePhone;
            data.BuyleadFax=model.BuyleadFax;
            data.BuyleadAddressLine1=model.BuyleadAddressLine1;
            data.DistrictID=model.DistrictID;
            data.ProvinceID = model.ProvinceID;
            data.BuyleadPostelCode= model.BuyleadPostelCode; 
            // default
            data.RowVersion++;
            data.ModifiedBy = model.ModifiedBy;
            data.ModifiedDate = DateTimeNow;
            data.RowFlag = model.RowFlag;
            #endregion

            #region Save
            using (var trans = new TransactionScope())
            {

                qDB.SubmitChanges();// บันทึกค่า Buylead
                trans.Complete();
                IsResult = true;
            }
            #endregion

            return IsResult;
        }

        public List<int> OldID { get; set; }
        public List<string> OldFiles { get; set; }
        public List<string> NewFiles { get; set; }

        #region UpdateBuyleadCountInCategories

        public bool UpdateBuyleadCountInCategories(int CateLV1, int CateLV2, int CateLV3)
        {
            var svCategory = new Category.CategoryService();

            IsResult = svCategory.UpdateBuyleadCount(3, CateLV3);

            IsResult = svCategory.UpdateBuyleadCount(2, CateLV2);

            IsResult = svCategory.UpdateBuyleadCount(1, CateLV1);

            return IsResult;
        }

        public bool UpdateBuyleadCountInCategories(List<int> CateLV1, List<int> CateLV2, List<int> CateLV3)
        {
            var svCategory = new Category.CategoryService();

            var lstCateLV1 = CateLV1.Distinct();
            var lstCateLV2 = CateLV2.Distinct();
            var lstCateLV3 = CateLV3.Distinct();

            foreach (var it in lstCateLV3)
            {
                IsResult = svCategory.UpdateBuyleadCount(3, it);
            }

            foreach (var it in lstCateLV2)
            {
                IsResult = svCategory.UpdateBuyleadCount(2, it);
            }

            foreach (var it in lstCateLV1)
            {
                IsResult = svCategory.UpdateBuyleadCount(1, it);
            }

            return IsResult;
        }
        #endregion

        #region UpdateBuyleadViewCount
        public bool UpdateBuyleadViewCount(int BuyleadID)
        {
            var Buylead = SelectData<b2bBuylead>("ViewCount,BuyleadID", "BuyleadID = " + BuyleadID).First();
            if (Buylead.ViewCount == 0)
                ViewCount = 1;
            else
                ViewCount = (int)Buylead.ViewCount + 1;

            string sqlUpdate = " ViewCount = " + ViewCount;
            string sqlWhere = " BuyleadID = " + BuyleadID;
            UpdateByCondition<b2bBuylead>(sqlUpdate, sqlWhere);
            return IsResult;
        }
        #endregion

        #region Update ContactCount
        public void AddContactCount(int BuyleadID)
        {
            var SelectViewCount = SelectData<b2bBuylead>("ContactCount,BuyleadID", "BuyleadID = " + BuyleadID).First();
            if (SelectViewCount.ContactCount == 0)
                ContactCount = 1;
            else
                ContactCount = (int)SelectViewCount.ContactCount + 1;

            string sqlUpdate = " ContactCount = " + ContactCount;
            string sqlWhere = " BuyleadID = " + BuyleadID;
            UpdateByCondition<b2bBuylead>(sqlUpdate, sqlWhere);
        }
        #endregion

        #region ReStore
        public bool ReStore(List<int> BuyleadID, List<int> CateLV1, List<int> CateLV2, List<int> CateLV3, int CompID)
        {
            var svCompany = new Company.CompanyService();

            var Contains = SQLWhereListInt(BuyleadID, "BuyleadID");
            UpdateByCondition<b2bBuylead>(" IsJunk = 0 , ListNo = 0 ", " CompID = " + CompID + " AND " + Contains);

            IsResult = svCompany.UpdateBuyleadCount(CompID);
            UpdateBuyleadCountInCategories(CateLV1, CateLV2, CateLV3);

            return IsResult;
        }
        #endregion

        #region MoveToJunk
        public bool MoveToJunk(List<int> BuyleadID, List<int> CateLV1, List<int> CateLV2, List<int> CateLV3, int CompID)
        {
            var svCompany = new Company.CompanyService();

            var Contains = SQLWhereListInt(BuyleadID, "BuyleadID");
            UpdateByCondition<b2bBuylead>(" IsJunk = 1 , ListNo = 0 ", " CompID = " + CompID + " AND " + Contains);

            IsResult = svCompany.UpdateBuyleadCount(CompID);
            UpdateBuyleadCountInCategories(CateLV1, CateLV2, CateLV3);

            return IsResult;
        }
        #endregion

        #region Delete
        public bool Delete(List<int> BuyleadID, List<int> CateLV1, List<int> CateLV2, List<int> CateLV3, int CompID = 0)
        {
            var svCompany = new Company.CompanyService();
            var svCategory = new Category.CategoryService(); 

            var sqlWhere = SQLWhereListInt(BuyleadID, "BuyleadID");

            if (CompID > 0)
                sqlWhere += " AND CompID = " + CompID;

            UpdateByCondition<b2bBuylead>(" IsDelete = 1   , ListNo = 0 ", sqlWhere);

            if (CompID > 0)
            {
                IsResult = svCompany.UpdateBuyleadCount(CompID);
            }
            UpdateBuyleadCountInCategories(CateLV1, CateLV2, CateLV3);

            return IsResult;
        }
        #endregion

        #region ChangeGroup
        public bool ChangeGroup(int CompID, int OldGroupID, int NewGroupID)
        {
            UpdateByCondition<b2bBuylead>(" BuyleadGroupID = " + NewGroupID + " ", " CompID = " + CompID + " AND BuyleadGroupID = " + OldGroupID);
            return IsResult;
        }
        #endregion

        #region SaveShow
        public bool SaveIsShow(List<int> BuyleadID, List<int> CateLV1, List<int> CateLV2, List<int> CateLV3, int CompID, int IsShow)
        {
            var Contains = SQLWhereListInt(BuyleadID, "BuyleadID");

            if (IsShow > 1)
                IsShow = 0;

            UpdateByCondition<b2bBuylead>(" IsShow = " + IsShow, " CompID = " + CompID + " AND " + Contains);
            UpdateBuyleadCountInCategories(CateLV1, CateLV2, CateLV3);
            return IsResult;
        }
        #endregion

        #region ApproveBuylead
        public bool ApproveBuylead(List<int> BuyleadID, List<int> CateLV1, List<int> CateLV2, List<int> CateLV3, string CompCode)
        {
            var Contains = SQLWhereListInt(BuyleadID, "BuyleadID");
            UpdateByCondition<b2bBuylead>(" RowFlag = 4 , modifiedby = N'" + CompCode + "'", Contains);
            UpdateBuyleadCountInCategories(CateLV1, CateLV2, CateLV3);
            return IsResult;
        }
        #endregion

        #region RejectBuylead
        public bool RejectBuylead(List<int> BuyleadID, List<int> CateLV1, List<int> CateLV2, List<int> CateLV3, string Remark, string CompCode)
        {
            var Contains = SQLWhereListInt(BuyleadID, "BuyleadID");
            UpdateByCondition<b2bBuylead>(" RowFlag = 3 , remark = N'" + Remark + "', modifiedby = N'" + CompCode + "'", Contains);
            UpdateBuyleadCountInCategories(CateLV1, CateLV2, CateLV3);
            return IsResult;
        }
        #endregion

        #region Buylead Recommend
        public int RecommendCount { get; set; }
        #region DuplicateRecommend
        public bool DuplicateRecommend(int BuyleadID, int CompID)
        {
            var sqlWhere = CreateWhereAction(BuyleadAction.BackEnd, CompID);
            sqlWhere += " And ListNo > 0 AND BuyleadID = " + BuyleadID;
            var CountBuylead = CountData<b2bBuylead>(" BuyleadID ", sqlWhere);
            if (CountBuylead > 0)
            {
                IsResult = false;
                ArgumentException ex = new ArgumentException(" Item is Duplicate. ");
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
            RecommendCount = CountData<b2bBuylead>(" * ", CreateWhereAction(BuyleadAction.Recommend, CompID));
            if (RecommendCount >= 20)
            {
                IsResult = false;
                ArgumentException ex = new ArgumentException("Buylead Recommend has full. ");
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
        public bool MoveToRecommend(List<int> BuyleadID, int CompID)
        {
            if (ValidateFullRecommend(CompID))
            {
                #region Check Over Recommend
                var RecommendBalance = 20 - RecommendCount;
                if (BuyleadID.Count() > RecommendBalance)
                {
                    IsResult = false;
                    ArgumentException ex = new ArgumentException(" Your Can Add " + RecommendBalance + " Items");
                    MsgError.Add(ex);
                }
                #endregion

                if (IsResult)
                {
                    var sqlWhere = SQLWhereListInt(BuyleadID, "BuyleadID");
                    UpdateByCondition<b2bBuylead>(" ListNo = 21 , IsShow = 1 ", sqlWhere);
                }

            }

            return IsResult;
        }
        public bool MoveToRecommend(int BuyleadID, int CateLV1, int CateLV2, int CateLV3, int CompID)
        {
            if (DuplicateRecommend(BuyleadID, CompID))
            {
                ValidateFullRecommend(CompID);//ตรวจสอบว่า recommend เต็ม หรือเปล่า
                if (IsResult)
                {
                    UpdateByCondition<b2bBuylead>(" ListNo = 21 , IsShow = 1", " BuyleadID = " + BuyleadID);
                    UpdateBuyleadCountInCategories(CateLV1, CateLV2, CateLV3);
                }
            }
            return IsResult;
        }
        #endregion


        public bool SaveChangeListNo(List<int> BuyleadID, int CompID)
        {

            using (var trans = new TransactionScope())
            {

                var sqlWhere = SQLWhereListInt(BuyleadID, "BuyleadID");
                for (var i = 1; i <= BuyleadID.Count(); i++)
                {
                    UpdateByCondition<b2bBuylead>(" ListNo =" + i + " , IsShow = 1 , IsDelete = 0 ", "CompID = " + CompID + " AND BuyleadID = " + BuyleadID[i - 1]);

                }

                trans.Complete();
            }

            return IsResult;
        }


        #region MoveToStore
        public bool MoveToStore(int BuyleadID, int CompID)
        {
            UpdateByCondition<b2bBuylead>(" ListNo = 0 , IsShow = 1", " BuyleadID = " + BuyleadID);
            return IsResult;
        }
        #endregion

        #endregion

        #region MoveBuyleadInCate
        public bool MoveBuyleadInCate(int CateLV, int OldCateID, int NewCateID, int CateLV3ID)
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

        #region MoveBuyleadInCateLV
        public bool MoveBuyleadInCateLV(int oldcatelv1, int oldcatelv2, int oldcatelv3, int newcatelv1, int newcatelv2, int newcatelv3)
        {
            var svCategory = new Category.CategoryService();

            using (var trans = new TransactionScope())
            {
                MoveBuyleadInCate(3, oldcatelv3, newcatelv3, oldcatelv3);
                MoveBuyleadInCate(2, oldcatelv2, newcatelv2, newcatelv3);
                MoveBuyleadInCate(1, oldcatelv1, newcatelv1, newcatelv3);

                trans.Complete();
                IsResult = true;
            }

            using (var trans = new TransactionScope())
            {

                #region Update Buylead Count
                svCategory.UpdateBuyleadCount(3, oldcatelv3);
                svCategory.UpdateBuyleadCount(2, oldcatelv2);
                svCategory.UpdateBuyleadCount(1, oldcatelv1);
                svCategory.UpdateBuyleadCount(3, newcatelv3);
                svCategory.UpdateBuyleadCount(2, newcatelv2);
                svCategory.UpdateBuyleadCount(1, newcatelv1);
                #endregion
                trans.Complete();
                IsResult = true;
            }

            return IsResult;
        }
        #endregion

        #endregion

        #endregion

    }
}