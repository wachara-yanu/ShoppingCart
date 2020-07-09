using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using Prosoft.Base;
//using System.Web.Mvc; 
using Ouikum.Product;
using Prosoft.Service; 
using System.Transactions;

using Ouikum.Buylead;
using Ouikum.Company;

namespace Ouikum.Company
{
    #region enum
    public enum CompStatus
    {
        All,
        NonActivate,
        Activate,
        BlockInfo,
        Expire,
        Online,
        HaveProduct
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
    #endregion
     
    public class CompanyService : BaseSC
    { 

            #region Members 
            ProductService svProduct;
            public int? ViewCount { get; set; }
            #endregion
         
            #region Company
            public view_CompMember GetCompMemberByCompID(int CompID)
            {
                var comp = new view_CompMember();
                var data = SelectData<view_CompMember>(" * ", "CompID = " + CompID);
                if (data != null && TotalRow > 0)
                {
                    comp = data.First();
                    EncryptManager encrypt = new EncryptManager();
                    comp.Password = encrypt.DecryptData(comp.Password);
                }
                return comp;
            }

            #region Validate
            public bool ValidateExpireDate(int CompID)
            {
                var count = CountData<b2bCompany>(" * ", " RowFlag > 0 AND CompID = " + CompID + " AND GETDATE() > ExpireDate ");
                if (count > 0)
                { 
                    IsResult = true;
                    //ArgumentException ex =  new ArgumentException(" this company has expired ");
                    //MsgError.Add(ex);
                }
                else
                    IsResult = false;

                return IsResult;
            }
            public bool ValidateInsertCompany()
            {
                //do some thing
                return true;
            }

            public bool ValidateB2BUpdateCompany()
            {
                //do some thing
                return true;
            }
            #endregion


            #region Generate SQLWhere
            public string CreateWhereAction(CompStatus status, int? CompID = 0)
            {
                var sqlWhere = string.Empty;
                if (status == CompStatus.NonActivate)
                {
                    sqlWhere = "IsDelete = 0 AND ( RowFlag = 1 ) ";
                }
                else if (status == CompStatus.Activate)
                {
                    sqlWhere = "IsDelete = 0 AND ( RowFlag = 2  ) ";
                }
                else if (status == CompStatus.BlockInfo)
                {
                    sqlWhere = "IsDelete = 0 AND ( RowFlag = 3) ";
                }
                else if (status == CompStatus.Expire)
                {
                    sqlWhere = "IsDelete = 0 AND ( RowFlag = 4) ";
                }
                else if (status == CompStatus.Online)
                {
                    sqlWhere = "IsDelete = 0 AND ( RowFlag IN (2,4) ) ";
                }
                else if (status == CompStatus.HaveProduct)
                {
                    sqlWhere = "IsDelete = 0 AND (( RowFlag IN (2,4) ) AND ProductCount > 0 ) ";
                }
                else if (status == CompStatus.All)
                {
                    sqlWhere = "IsDelete = 0  ";
                }

                if (CompID > 0)
                    sqlWhere += "AND (CompID = " + CompID + ")";

                return sqlWhere;
            }

            public string CreateWhereCause(
                int CompID = 0, string CompCode = "", string txtSearch = "",
                int CompLevel = 0, int BizTypeID = 0, int ServiceType = 0, int ProvinceID=0
                )
            {
                #region DoWhereCause
                if (CompID > 0)
                    SQLWhere += " AND CompID = " + CompID;

                if (!string.IsNullOrEmpty(txtSearch))
                {
                    SQLWhere += " AND CompName LIKE N'%" + txtSearch + "%' ";
                }
                if (!string.IsNullOrEmpty(CompCode))
                {
                    SQLWhere += " AND CompCode LIKE N'" + CompCode + "' ";
                }
                if (BizTypeID > 0)
                {
                    SQLWhere += " AND (BizTypeID = " + BizTypeID + ")";
                }
                if (CompLevel > 0)
                {
                    SQLWhere += " AND ((CompLevel = 1 AND Istrust = 1) OR CompLevel = 3)";
                }
                if (ServiceType > 0)
                {
                    SQLWhere += CreateWhereServiceType(ServiceType);
                }
                if (ProvinceID > 0)
                {
                    SQLWhere += " AND (CompProvinceID = " + ProvinceID + ")";
                }
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

            #region CreateWhereServiceType
            public string CreateWhereServiceType(int ServiceType)
            {
                SQLWhere = "";
                if (ServiceType == 1)
                {
                    SQLWhere += " AND (ServiceType IN (1,2) )";
                }
                else if (ServiceType == 3)
                {
                    SQLWhere += " AND (ServiceType  IN (2,3) )";
                }
                return SQLWhere;
            }
            #endregion

            #region Company
         
            #region Insert

            public bool InsertCompany(b2bCompany model)
            {
                //model.CompCode = GenCompCode((int)model.ServiceType,(int)model.emCompID);
                #region set default
                model.isNotification = true;
                model.RowFlag = 2;
                model.RowVersion = 1;
                model.CreatedBy = "sa";
                model.ModifiedBy = "sa";
                model.ModifiedDate = DateTimeNow;
                model.CreatedDate = DateTimeNow;

               

                #endregion

                if (ValidateInsertCompany())
                {
                    qDB.b2bCompanies.InsertOnSubmit(model);
                    qDB.SubmitChanges();
                    IsResult = true;
                }
                else
                {
                    IsResult = false;
                }
                return IsResult;
            }

            public bool InsertCompany(Ouikum.Common.Register model)
            {
                b2bCompany data = new b2bCompany();
                b2bCompanyProfile compProfile = new b2bCompanyProfile();

                if (model.ServiceType < 1 || model.ServiceType == null) { model.ServiceType = 3; }

                #region set b2bCompany 
                if (model.CompLevel == 0) { model.CompLevel = 1; }
                data.MemberID = model.MemberID;
                data.emCompID = model.emCompID;
                if (model.ServiceType > 0)
                {
                    data.CompCode = GenCompCode((int)model.ServiceType, (int)model.emCompID).ToString();
                }
                else
                {
                    data.CompCode = GenCompCode(2, (int)model.emCompID).ToString();
                }
                data.CompLevel = model.CompLevel;
                if (model.CompLevel == 3)
                {
                    data.ExpireDate = model.ExpireDate;
                }
                data.CompWebsiteTemplate = 0;
                data.DisplayName = model.DisplayName.Trim();
                data.ContactFirstName = model.FirstName_register.Trim();
                data.ContactLastName = model.LastName.Trim();
                data.ServiceType = DataManager.ConvertToByte(model.ServiceType);
                data.CompName = model.CompName.Trim();
                data.BizTypeID = model.BizTypeID;
                data.CompCountryID = model.CountryID;
                data.CompProvinceID = model.ProvinceID;
                data.CompDistrictID = model.DistrictID;
                data.CompPhone = model.Phone;
                data.ContactCountryID = model.CountryID;
                data.ContactProvinceID = model.ProvinceID;
                data.ContactDistrictID = model.DistrictID; 
               
               
                data.ContactEmail = model.Emails.Trim();
                data.ContactPhone = model.Phone;
                data.FactoryCountryID = model.CountryID;
                data.FactoryProvinceID = model.ProvinceID;
                data.FactoryDistrictID = model.DistrictID;
                data.FactoryPhone = model.Phone;
                data.IsShow = true;
                data.CompPostalCode = model.PostalCode;
                data.ContactPostalCode = model.PostalCode;
                data.FactoryPostalCode = model.PostalCode;
                data.CompFax = model.Fax;
                data.ContactFax = model.Fax;
                data.FactoryFax = model.Fax;
                data.ProductCount = 0;
                data.ViewCount = 0;
                data.BuyLeadCount = 0;
                data.isNotification = true;
                data.IsTrust = DataManager.ConvertToBool(DataManager.ConvertToInteger(model.IsTrust));
                data.IsSME = DataManager.ConvertToBool(DataManager.ConvertToInteger(model.IsSME));
                if (!string.IsNullOrEmpty(model.BizTypeOther) && model.BizTypeID == 13)
                {
                    data.BizTypeOther = model.BizTypeOther.Trim();
                }
                #endregion

                #region set b2bCompanyProfile
                compProfile.emCompProfileID = model.emCompProfileID;
                compProfile.CompBizType = (byte)model.BizTypeID;
                compProfile.CompName = model.CompName.Trim();
                compProfile.AddrLine1 = model.AddrLine1;
                compProfile.CountryID = model.CountryID;
                compProfile.ProvinceID = model.ProvinceID;
                compProfile.DistrictID = model.DistrictID;
                compProfile.PostalCode = model.PostalCode;
                compProfile.IsShow = true;
                if (!string.IsNullOrEmpty(compProfile.CompBizTypeOther) && compProfile.CompBizType == 13)
                {
                    compProfile.CompBizTypeOther = model.BizTypeOther.Trim();
                }
                #endregion

                using (var trans = new TransactionScope())
                {
                    InsertCompany(data);

                    compProfile.CompID = data.CompID;
                    InsertCompanyProfile(compProfile);
                    UpdateCompanySignIn(data.CompID, false);

                    #region Set CompCode ที่ได้ เข้า Register Model กลับไป
                    model.CompCode = data.CompCode;
                    #endregion

                    trans.Complete();
                    IsResult = true;
                }

                return IsResult;
            }
         
            #endregion

            #region Update
            public bool UpdateCompany(b2bCompany model)
            {
                var data = qDB.b2bCompanies.Single(q => q.CompID == model.CompID);

                #region Set Value
                data = model;

                data.RowVersion++;
                data.ModifiedBy = "sa";
                data.ModifiedDate = DateTimeNow;
                #endregion
                qDB.SubmitChanges();
                IsResult = true;
                return IsResult;
            }


            #region UpdateProductCount
            public bool UpdateProductCount(int CompID)
            {
                var svProduct = new Product.ProductService();
                var sqlWhere = svProduct.CreateWhereAction(ProductAction.FrontEnd, CompID);
                var count = CountData<view_SearchProduct>(" * ", sqlWhere);
                var svCompany = new Company.CompanyService();
                IsResult = svCompany.UpdateByCondition<b2bCompany>(" ProductCount = " + count, "CompID = " + CompID);
                return IsResult;
            }
            #endregion

            #region UpdateBuyleadCount
            public bool UpdateBuyleadCount(int CompID)
            {
                var svBuylead = new Buylead.BuyleadService();
                var sqlWhere = svBuylead.CreateWhereAction(BuyleadAction.FrontEnd, CompID);
                var count = CountData<view_BuyLead>(" * ", sqlWhere);
                var svCompany = new Company.CompanyService();
                IsResult = svCompany.UpdateByCondition<b2bCompany>(" BuyLeadCount = " + count, "CompID = " + CompID);
                return IsResult;
            }
            #endregion

            #region UpdateCompanyViewCount
            public bool UpdateCompanyViewCount(int CompID)
            {
                var Company = SelectData<b2bCompany>("CompID,ViewCount", " CompID = " + CompID).First();

                if (Company.ViewCount == 0)
                    ViewCount = 1;
                else
                    ViewCount = (int)Company.ViewCount + 1;


                string sqlUpdate = " ViewCount = " + ViewCount;
                string sqlWhere = " CompID = " + CompID;
                UpdateByCondition<b2bCompany>(sqlUpdate, sqlWhere);
                return IsResult;
            }
            #endregion
         
            public bool DeleteCompany(List<b2bCompany> model)
            {
                try
                {
                    var id = model.Where(m => model.Select(c => c.CompID).Contains(m.CompID));


                    qDB.SubmitChanges();
                    return true;
                }
                catch
                {
                    return false;
                }
            }


            #endregion

            #region Delete
            public bool Delete(int id)
            {
                var data = qDB.b2bCompanies.Single(m => m.CompID == id);
                data.RowFlag = -1;
                data.ModifiedBy = "sa";
                data.RowVersion++;
                data.ModifiedDate = DateTimeNow;

                return true;
            }
            #endregion

            #endregion

            #region b2bCompanyProfile
            #region Method Get 
            #region GetCompanyProfile
            /// <summary>
            /// เรียกข้อมูล ตาราง CompanyProfile ที่ RowFlag > 0 
            /// </summary>
            /// <returns>IQueryable</returns>
            public IQueryable<b2bCompanyProfile> GetCompanyProfile()
            {
                IQueryable<b2bCompanyProfile> query = qDB.b2bCompanyProfiles.Where(it => it.RowFlag > 0);
                return query;
            }
            #endregion 
            #endregion
         
            #region Insert
            public bool InsertCompanyProfile(b2bCompanyProfile model)
            {
                #region set default
                model.RowFlag = 2;
                model.RowVersion = 1;
                model.CreatedBy = "sa";
                model.ModifiedBy = "sa";
                model.ModifiedDate = DateTimeNow;
                model.CreatedDate = DateTimeNow;
                #endregion

                qDB.b2bCompanyProfiles.InsertOnSubmit(model);
                qDB.SubmitChanges();
                IsResult = true; 
                return IsResult; 
            }  
         
            #endregion
          
            #endregion

            #region GenCompCode
            public string GenCompCode(int ServiceType, int CompID)
            {
                string CompCode = string.Empty;
                #region check service type
                if (ServiceType < 9)
                {
                    if (ServiceType == 1)
                        CompCode = "BY";
                    else if (ServiceType == 2)
                        CompCode = "SB";
                    else if (ServiceType == 3)
                        CompCode = "SP";

                #endregion
                    CompCode = CompCode.ToUpper() + "-" + CompID.ToString("00000") + "-" + RandomCharInt(3);
                }
                else
                {
                    CompCode = "AM" + "-" + CompID.ToString("00000") + "-" + RandomCharInt(3);
                }

                return CompCode;
            }
            #endregion

            #region RandomCharecter & RandomCharInt


            private string RandomCharecter(int Size)
            {
                Random ran = new Random();
                string chars = "ABCDEFGHIJKLMNOPQESTUVWXYZ";
                char[] buffer = new char[Size];
                for (int i = 0; i < Size; i++)
                {
                    buffer[i] = chars[ran.Next(chars.Length)];
                }
                return new string(buffer);
            }
            private string RandomCharInt(int Size)
            {
                Random ran = new Random();
                string chars = "0123456789";
                char[] buffer = new char[Size];
                for (int i = 0; i < Size; i++)
                {
                    buffer[i] = chars[ran.Next(chars.Length)];
                }
                return new string(buffer);
            }
            #endregion                
         
            #endregion

            #region Add Follow Category
            public void AddFollowCategory(List<int> CategoryID, List<int> CategoryLevel, int CompID)
            {

                using (var trans = new TransactionScope())
                {
                    for (var i = 0; i < CategoryID.Count(); i++)
                    {
                        var count = CountData<b2bFollowCategory>(" * ", "IsDelete = 0 AND CategoryID = " + CategoryID[i] + " AND CompID = " + CompID);
                        if (count == 0)
                        {
                            var model = new b2bFollowCategory();
                            model.CategoryID = CategoryID[i];
                            model.CategoryLevel = (short)CategoryLevel[i];
                            model.CompID = CompID;
                            model.IsDelete = false;
                            model.IsShow = true;
                            model.RowFlag = 1;
                            model.RowVersion = 1;
                            model.CreatedDate = DateTimeNow;
                            model.ModifiedDate = DateTimeNow;
                            model.CreatedBy = CompID.ToString();
                            model.ModifiedBy = CompID.ToString();
                            SaveData<b2bFollowCategory>(model, "FollowCatgoryID");
                        }
                    }

                    trans.Complete();
                }

            }
            #endregion

            #region DeleteFollowCategory
            public void DeleteFollowCategory(List<int> CategoryID, int CompID)
            { 
                    var sqlwhere = SQLWhereListInt(CategoryID, "CategoryID");
                    UpdateByCondition<b2bFollowCategory>(" IsDelete = 1 ",  sqlwhere + " AND CompID = " + CompID);  
            }
            #endregion

            #region Add Notification
            public void AddNotification(string AppKey,  bool? Status,int CompID= 0)
            {
                //บอมครับ พี่ส่ง Push Notification มาให้นะครับ ตรง Service http://test.b2bthai.com/api/addnotification 
                //ขอไม่ส่ง compid กับ status ขอส่งเป็นค่า null ได้นะครับ โดยเปิด app ครั้งแรกจะส่ง appkey กับ status:true ส่วน 
                //compid เป็น null ถ้าหาก user login แล้วจะส่ง  appkey กับ compid ส่วน status จะส่งเป็นค่า null
                //โดยทางบอมจะต้องตรวจสอบว่า compid
                //นี้ได้ตั้งค่าไว้ในระบบว่าเป็น true|false เพราะเค้าอาจออนไว้ที่เครื่องอื่น แล้วเอาค่า status มาเติมให้ในค่าอันที่ login เข้ามาใหม่
                if (CompID > 0)
                {
                    var data = SelectData<b2bCompany>(" * ", " CompID = " + CompID);
                    if (data != null && data.Count > 0)
                    {
                        Status = data.First().isNotification;
                        var stat = "0";
                        if ((bool)Status)
                            stat = "1";

                        var count = CountData<emNotification>(" * ", "IsDelete = 0 AND AppKey = N'" + AppKey + "' ");
                        if (count == 0)
                        {
                            var model = new emNotification();
                            model.AppKey = AppKey;
                            model.CompID = CompID;
                            model.status = Status;
                            model.CreatedDate = DateTimeNow;
                            model.ModifiedDate = DateTimeNow;
                            model.CreatedBy = CompID.ToString();
                            model.ModifiedBy = CompID.ToString();
                            model.IsDelete = false;
                            model.IsShow = true;
                            model.RowFlag = 1;
                            model.RowVersion = 1;
                            SaveData<emNotification>(model, "NoticeID");
                        }
                        else
                        { 
                            UpdateByCondition<emNotification>("Status = " + stat + " , CompID = " + CompID, " IsDelete = 0 AND AppKey = N'" + AppKey + "' ");
                        }
                    }
                    else
                    {
                        IsResult = false;
                        MsgError.Add(new Exception("Not Found This User"));
                    }
                       
                }
                else
                {
                    if (Status == null) 
                        Status = true; 
                    var count = CountData<emNotification>(" * ", "IsDelete = 0 AND AppKey = N'" + AppKey + "' "); 
                    if (count == 0)
                    {
                        var model = new emNotification();
                        model.AppKey = AppKey;
                        model.CompID = 0;
                        model.status = Status;
                        model.CreatedDate = DateTimeNow;
                        model.ModifiedDate = DateTimeNow;
                        model.CreatedBy = CompID.ToString();
                        model.ModifiedBy = CompID.ToString();
                        model.IsDelete = false;
                        model.IsShow = true;
                        model.RowFlag = 1;
                        model.RowVersion = 1;
                        SaveData<emNotification>(model, "NoticeID");
                    } 
                } 
            }
            #endregion

            #region Setting Notification  
        
            public void SettingNotification(bool status, int compid)
            {
                var sqlUpdate = "status = 0";
                var notice = " isNotification = 0 ";
                if (status)
                {
                    sqlUpdate = "status = 1";
                    notice = " isNotification = 1 ";
                }
                try
                {
                    UpdateByCondition<b2bCompany>(notice, "compid = " + compid);
                    UpdateByCondition<emNotification>(sqlUpdate, "IsDelete = 0 AND compid = " + compid);
                }
                catch (Exception ex)
                {
                    IsResult = false;
                    MsgError.Add(ex);
                }

            }
   
            #endregion

            #region Delete Notification
            public void DeleteNotification(string AppKey, int CompID)
            { 
                UpdateByCondition<emNotification>("CompID = 0" ,"AppKey = N'" + AppKey + "' AND CompID = " + CompID); 
            }
            #endregion
         
            #region UpdateCompanySignIn
            public bool UpdateCompanySignIn(int CompID, bool IsOnline)
            {

                var count = CountData<b2bLogOn>(" * ", " IsDelete = 0 AND CompID = " + CompID);
                if (count > 0)
                {
                    if (IsOnline)
                        UpdateByCondition<b2bLogOn>(" IsOnline = 1 ", " CompID = " + CompID);
                    else
                        UpdateByCondition<b2bLogOn>(" IsOnline = 0 ", " CompID = " + CompID);
                }
                else
                {
                    var model = new b2bLogOn();
                    model.CompID = CompID;
                    model.IsOnline = (IsOnline == true) ? true : false;
                    model.RowFlag = 1;
                    model.IsShow = true;
                    model.IsDelete = false;
                    model.CreatedBy = "sa";
                    model.ModifiedBy = "sa";
                    SaveData<b2bLogOn>(model, "LogonID");
                }
                return IsResult;
            }
            #endregion

            #region LinkWithFacebook
            public void LinkWithFacebook(string facebookid, int compid)
            {
                try
                {
                    var count = CountData<b2bCompany>(" * ", " IsDelete = 0 AND FacebookUrl = N'" + facebookid + "' ");
                    if (count == 0)
                    {
                        UpdateByCondition<b2bCompany>(" FacebookUrl = '" + facebookid + "' ", " compid = " + compid);
                    }
                    else
                    {
                        MsgError.Add(new Exception("facebookid has exist"));
                        IsResult = false;
                    }
                }
                catch (Exception ex)
                {
                    MsgError.Add(ex);
                    IsResult = false;
                }
            }
            #endregion

            #region UnlinkWithFacebook
            public void UnlinkWithFacebook(int compid)
            {
                try
                {
                    UpdateByCondition<b2bCompany>(" FacebookUrl = '' ", " compid = " + compid);
                }
                catch (Exception ex)
                {
                    MsgError.Add(ex);
                    IsResult = false;
                }
            }
            #endregion

            #region UpdateBlogListNo
            public bool UpdateBlogListNo(List<int> id, List<int> no)
            {
                var str = "UPDATE b2bArticle SET ListNo = {0}  WHERE ArticleID = {1}";

                using (var trans = new TransactionScope())
                {
                    for (var i = 0; i < id.Count(); i++)
                    {
                        qDB.ExecuteCommand(str, no[i], id[i]);

                        qDB.SubmitChanges();
                    }
                    trans.Complete();
                    IsResult = true;
                }
                return IsResult;
            }
            #endregion

            #region UpdateCertifyListNo
            public bool UpdateCertifyListNo(List<int> id, List<int> no)
            {
                var str = "UPDATE b2bCompanyCertify SET ListNo = {0}  WHERE CompCertifyID = {1}";

                using (var trans = new TransactionScope())
                {
                    for (var i = 0; i < id.Count(); i++)
                    {
                        qDB.ExecuteCommand(str, no[i], id[i]);

                        qDB.SubmitChanges();
                    }
                    trans.Complete();
                    IsResult = true;
                }
                return IsResult;
            }
            #endregion

            #region UpdatePaymentListNo
            public bool UpdatePaymentListNo(List<int> id, List<int> no)
            {
                var str = "UPDATE b2bCompanyPayment SET ListNo = {0}  WHERE CompPaymentID = {1}";

                using (var trans = new TransactionScope())
                {
                    for (var i = 0; i < id.Count(); i++)
                    {
                        qDB.ExecuteCommand(str, no[i], id[i]);

                        qDB.SubmitChanges();
                    }
                    trans.Complete();
                    IsResult = true;
                }
                return IsResult;
            }
            #endregion

            #region UpdateShipmentListNo
            public bool UpdateShipmentListNo(List<int> id, List<int> no)
            {
                var str = "UPDATE b2bCompanyShipment SET ListNo = {0}  WHERE CompShipmentID = {1}";

                using (var trans = new TransactionScope())
                {
                    for (var i = 0; i < id.Count(); i++)
                    {
                        qDB.ExecuteCommand(str, no[i], id[i]);

                        qDB.SubmitChanges();
                    }
                    trans.Complete();
                    IsResult = true;
                }
                return IsResult;
            }
            #endregion

            #region UpdateSettingListNo
            public bool UpdateSettingListNo(List<int> id, List<int> no)
            {
                var str = "UPDATE b2bCompanyMenu SET ListNo = {0}  WHERE CompMenuID = {1}";

                using (var trans = new TransactionScope())
                {
                    for (var i = 0; i < id.Count(); i++)
                    {
                        qDB.ExecuteCommand(str, no[i], id[i]);

                        qDB.SubmitChanges();
                    }
                    trans.Complete();
                    IsResult = true;
                }
                return IsResult;
            }
            #endregion

            #region Insert SettingMenu

            public b2bCompanyMenu InsertCompanysetting(b2bCompanyMenu model)
            {
                model.IsDefaultMenu = false;
                model.ParentMenuID = 0;
                model.IsDelete = false;
                model.IsShow = true;
                model.RowFlag = 1;
                model.RowVersion = 1;
                model.CreatedBy = "sa";
                model.ModifiedBy = "sa";
                model.ModifiedDate = DateTimeNow;
                model.CreatedDate = DateTimeNow;


                qDB.b2bCompanyMenus.InsertOnSubmit(model);
                qDB.SubmitChanges();
                IsResult = true;
                return model;
            }

            #endregion
            
            
    }
}