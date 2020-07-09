using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using Prosoft.Base;
//using System.Web.Mvc;
using System.Transactions;
using Prosoft.Service;


namespace Ouikum.Common
{
    public partial class MemberService : BaseSC
    {

        #region Select

        #region MemberActive
        #region GetMemberActivate
        /// <summary>
        /// เรียกข้อมูล ตาราง MemberActivate ที่ RowFlag > 0 
        /// </summary>
        /// <returns>IQueryable</returns>
        public IQueryable<emMemberActivate> GetMemberActivate()
        {
            IQueryable<emMemberActivate> query = qDB.emMemberActivates.Where(it => it.RowFlag > 0);
            return query;
        }
        #endregion

        #region ListMemberActivate
        /// <summary>
        /// List ข้อมูล ตาราง MemberActivate ที่ RowFlag > 0 
        /// </summary>
        /// <returns>List</returns>
        public List<emMemberActivate> ListMemberActivate()
        {
            return GetMemberActivate().Select(it => new emMemberActivate
            {
                MemberActivateID = it.MemberActivateID,
                MemberActivateCode = it.MemberActivateCode,
                ActivateType = it.ActivateType,
                ActivateCode = it.ActivateCode,
                StartDate = it.StartDate,
                ExpireDate = it.ExpireDate,
                MemberID = it.MemberID,
                RowFlag = it.RowFlag,
                RowVersion = it.RowVersion
            }).ToList();
        }
        #endregion
        #endregion

        #region MemberWeb
        #region GetMemberWeb
        /// <summary>
        /// เรียกข้อมูล ตาราง MemberWeb ที่ RowFlag > 0 
        /// </summary>
        /// <returns>IQueryable</returns>
        public IQueryable<emMemberWeb> GetMemberWeb()
        {
            IQueryable<emMemberWeb> query = qDB.emMemberWebs.Where(it => it.RowFlag > 0);
            return query;
        }
        #endregion

        #region ListMemberWeb
        /// <summary>
        /// List ข้อมูล ตาราง MemberWeb ที่ RowFlag > 0 
        /// </summary>
        /// <returns>List</returns>
        public List<emMemberWeb> ListMemberWeb()
        {
            return GetMemberWeb().Select(it => new emMemberWeb
            {
                MemberWebID = it.MemberWebID,
                MemberWebCode = it.MemberWebCode,
                WebID = it.WebID,
                MemberID = it.MemberID,
                RowFlag = it.RowFlag,
                RowVersion = it.RowVersion
            }).ToList();
        }
        #endregion

        #endregion


        #region GetSecretID
        public string GetSecretID(SyncModel model)
        {
            return GetSecretID(model.username, model.password, model.webid);
        }

        public string GetSecretID(string username, string password, string webid)
        {
            username = username.Trim();
            password = password.Trim();
            webid = webid.Trim();
            var secretid = "";
            var svMember = new MemberService();
            var encrypt = new EncryptManager();
            var sqlWhere = " IsDelete = 0 AND UserName = N'" + username + "' AND Password = N'" + encrypt.EncryptData(password) + "'";
            var count = CountData<emMember>(" * ", sqlWhere);

            #region data
            if (count > 0)
            {
                var data = SelectData<emMember>(" * ", sqlWhere).First();
                IsResult = true;
                secretid = encrypt.EncryptData("commonweb~" + username + "~" + password + "~" + webid + "~" + data.MemberID);
            }
            #endregion

            return secretid;
        }
        #endregion
        #endregion

        #region Insert

        #region emMember
        public bool InsertMember(emMember model)
        { 
            #region set default
            model.RowFlag = 2;
            model.RowVersion = 1;
            model.CreatedBy = "sa";
            model.ModifiedBy = "sa";
            model.ModifiedDate = DateTimeNow;
            model.CreatedDate = DateTimeNow;
            #endregion
             
            qDB.emMembers.InsertOnSubmit(model);
            qDB.SubmitChanges();
            IsResult = true;

            return IsResult;
        }
        #endregion

        #region emMemberWeb
        public bool InsertMemberWeb(emMemberWeb model)
        {
            #region set default
            model.RowFlag = 2;
            model.RowVersion = 1;
            model.CreatedBy = "sa";
            model.ModifiedBy = "sa";
            model.ModifiedDate = DateTimeNow;
            model.CreatedDate = DateTimeNow;
            #endregion

            qDB.emMemberWebs.InsertOnSubmit(model);
            qDB.SubmitChanges();
            IsResult = true;

            return IsResult;
        }
        #endregion

        #region emMemberActivate
        public bool InsertMemberActivate(emMemberActivate model)
        {
            #region set default
            model.ActivateCode = GenActivateCode();
            model.StartDate = DateTime.Now;
            model.ExpireDate = DateTime.Today.AddDays(1);
            model.ActivateType = 1;
            model.RowFlag = 1;
            model.RowVersion = 1;
            model.CreatedBy = "sa";
            model.ModifiedBy = "sa";
            model.ModifiedDate = DateTimeNow;
            model.CreatedDate = DateTimeNow;
            model.IsDelete = false;
            model.IsShow = true;
            #endregion

            qDB.emMemberActivates.InsertOnSubmit(model);
            qDB.SubmitChanges();
            IsResult = true;

            return IsResult;
        }
        #endregion

        #region b2bMemberPaid
        public bool InsertMemberPaid(b2bMemberPaid model)
        {
            #region set default
            model.RowFlag = 1;
            model.RowVersion = 1;
            model.CreatedBy = "sa";
            model.ModifiedBy = "sa";
            model.ModifiedDate = DateTimeNow;
            model.CreatedDate = DateTimeNow;
            #endregion

            qDB.b2bMemberPaids.InsertOnSubmit(model);
            qDB.SubmitChanges();
            IsResult = true;

            return IsResult;
        }
        #endregion

        #region GenActivateCode
        public string GenActivateCode()
        {
            string ActivateCode = Guid.NewGuid().ToString();

            return ActivateCode;
        }

        #endregion


        #region Register
        public bool UserRegister(Ouikum.Common.Register model)
        {
            EncryptManager encrypt = new EncryptManager();
            emMember member = new emMember();
            emMemberWeb memberWeb = new emMemberWeb();
            //emMemberActivate memberActivate = new emMemberActivate();
            emCompany company = new emCompany();
            emCompanyProfile compProfile = new emCompanyProfile();
            Ouikum.Common.emCompanyService svCompany = new Ouikum.Common.emCompanyService(qDB);

            #region Set ค่า เข้า Member
            if (model.MemberID > 0)
            {
                member.emMemberID = (int)model.MemberID;
            }
            member.UserName = model.UserName.Trim();
            member.Password = encrypt.EncryptData(model.Password);
            member.DisplayName = model.DisplayName.Trim();
            member.AddrLine1 = model.AddrLine1;
            member.Email = model.Emails.Trim();
            member.FirstName = model.FirstName_register.Trim();
            member.LastName = model.LastName.Trim();
            member.CountryID = model.CountryID;
            member.ProvinceID = model.ProvinceID;
            member.DistrictID = model.DistrictID;
            member.MemberType = model.MemberType;
            member.Phone = model.Phone;
            member.PostalCode = model.PostalCode;
            member.Mobile = model.Mobile;
            member.Fax = model.Fax;
            member.IsShow = true;
            member.RegisDate = DateTimeNow;
            member.FacebookID = model.FacebookID;
            #endregion

            #region Set ค่า เข้า memberWeb
            if (model.WebID > 0) 
                memberWeb.WebID = model.WebID; 
            else
                memberWeb.WebID = 1;
            #endregion

            #region Set ค่า เข้า company
            company.CompName = model.CompName.Trim();
            company.DisplayName = model.DisplayName.Trim();
            company.CompEmail = model.Emails.Trim();
            company.BizTypeID = Convert.ToInt32(model.BizTypeID);
            company.CompAddrLine1 = model.AddrLine1;
            company.CompCountryID = model.CountryID;
            company.CompProvinceID = model.ProvinceID;
            company.CompDistrictID = model.DistrictID;
            company.CompPostalCode = model.PostalCode;
            company.CompPhone = model.Phone;
            company.CompMobile = model.Mobile;
            company.CompFax = model.Fax;
            company.IsShow = true;
            if (model.BizTypeID == 13 && !string.IsNullOrEmpty(model.BizTypeOther))
            {
                company.BizTypeOther = model.BizTypeOther;
            }
            #endregion

            #region Set ค่า เข้า companyProfile
            compProfile.CompName = model.CompName.Trim();
            compProfile.AddrLine1 = model.AddrLine1;
            compProfile.CountryID = model.CountryID;
            compProfile.ProvinceID = model.ProvinceID;
            compProfile.DistrictID = model.DistrictID;
            compProfile.PostalCode = model.PostalCode;
            compProfile.IsShow = true;

            if (model.BizTypeID > 0)
            {
                compProfile.CompBizType = (byte)model.BizTypeID;
            }
            else
            {
                compProfile.CompBizType = 13;
            }

            if (model.BizTypeID == 13 && !string.IsNullOrEmpty(model.BizTypeOther))
            {
                compProfile.CompBizTypeOther = model.BizTypeOther.Trim();
            }
            #endregion


            try
            {
                using (var trans = new TransactionScope())
                {
                    InsertMember(member);

                    memberWeb.MemberID = member.MemberID;
                    InsertMemberWeb(memberWeb);

                    //memberActivate.MemberID = member.MemberID;
                    //InsertMemberActivate(memberActivate);

                    company.MemberID = member.MemberID;
                    svCompany.InsertCompany(company);

                    compProfile.CompID = company.CompID;
                    svCompany.InsertCompanyProfile(compProfile);

                    #region Set ID ที่ได้ เข้า Register Model กลับไป
                    model.MemberID = member.MemberID;
                    model.emCompID = company.CompID;
                    model.emCompProfileID = compProfile.CompProfileID;
                    #endregion

                    trans.Complete();
                    IsResult = true;
                }

                if (!IsResult)
                {
                    DeleteMember(member.MemberID);
                    DeleteMemberWeb(member.MemberID);
                    DeleteMemberActivate(member.MemberID);
                    DeleteCompany(member.MemberID);
                }
            }
            catch (Exception ex)
            {

                IsResult = false;
            }

            return IsResult;
        }
        #endregion

        #region Register With Account
        public bool UserRegisterWithAccount(view_emCompanyMember model)
        {
           
            emMember member = new emMember();
            emMemberWeb memberWeb = new emMemberWeb();

            #region Set ค่า เข้า memberWeb
            memberWeb.MemberID = model.MemberID;
            if (model.WebID > 0)
                memberWeb.WebID = model.WebID;
            else
                memberWeb.WebID = 1;
            #endregion

            try
            {
                using (var trans = new TransactionScope())
                {
                    InsertMemberWeb(memberWeb);
                    trans.Complete();
                    IsResult = true;
                }

                if (!IsResult)
                {
                    DeleteMemberWeb(member.MemberID);
                }
            }
            catch (Exception ex)
            {

                IsResult = false;
            }

            return IsResult;
        }
        #endregion

        #endregion

        #region Update

        public bool UpdateMember(emMember model)
        {
            var data = qDB.emMembers.Single(m => m.MemberID == model.MemberID);

            qDB.SubmitChanges();
            return IsResult;
        }
        public bool UpdateCompany(emCompany model)
        {
            return IsResult;
        }
        public bool UpdateMemberWeb(emMemberWeb model)
        {
            return IsResult;
        }
        public bool UpdateMemberActivate(emMemberActivate model)
        {
            return IsResult;
        } 
        #endregion

        #region Delete

        public bool DeleteMember(int id)
        {
            qDB.ExecuteCommand("update emMember set Isdelete = 1 where MemberID = {0}", id);
            IsResult = true;
            return IsResult;
        }
        public bool DeleteCompany(int id)
        {
            qDB.ExecuteCommand("update emCompany set Isdelete = 1 where MemberID = {0}", id);
            IsResult = true;
            return IsResult;
        }
        public bool DeleteMemberWeb(int id)
        {
            qDB.ExecuteCommand("update emMemberWeb set Isdelete = 1 where MemberID = {0}", id);
            IsResult = true;
            return IsResult;
        }
        public bool DeleteMemberActivate(int id)
        {
            qDB.ExecuteCommand("update emMemberActivate set Isdelete = 1 where MemberID = {0}", id);
            IsResult = true;
            return IsResult;
        }
        #endregion
    }
}
