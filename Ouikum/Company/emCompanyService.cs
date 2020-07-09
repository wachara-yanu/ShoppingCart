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
    public class emCompanyService : BaseSC
    {
        #region Contrustor
        public emCompanyService()
        {
            qDB = new OuikumDataContext(ConnectionString);

        }
        public emCompanyService(OuikumDataContext conn)
        {
            qDB = conn;
        }
        #endregion

        #region Select

        #region Company
        #region GetCompany
        /// <summary>
        /// เรียกข้อมูล ตาราง Company ที่ RowFlag > 0 
        /// </summary>
        /// <returns>IQueryable</returns>
        public IQueryable<emCompany> GetCompany()
        {
            IQueryable<emCompany> query = qDB.emCompanies.Where(it => it.IsDelete == DataManager.ConvertToBool(0));
            return query;
        }
        #endregion 

        #region ListCompany
        public List<emCompany> ListCompany()
        {
            var data = SelectData<emCompany>("*", "IsDelete = 0");
            return data.ToList();
        }
        #endregion

        #endregion

        #endregion

        #region Insert
        public bool InsertCompany(emCompany model)
        { 
                #region set default
                model.RowFlag = 2;
                model.RowVersion = 1;
                model.CreatedBy = "sa";
                model.ModifiedBy = "sa";
                model.ModifiedDate = DateTimeNow;
                model.CreatedDate = DateTimeNow;
                #endregion

                qDB.emCompanies.InsertOnSubmit(model);
                qDB.SubmitChanges();

                IsResult = true;

            return IsResult;
        }
        public bool InsertCompanyProfile(emCompanyProfile model)
        { 

            #region set default
            model.RowFlag = 2;
            model.RowVersion = 1;
            model.CreatedBy = "sa";
            model.ModifiedBy = "sa";
            model.ModifiedDate = DateTimeNow;
            model.CreatedDate = DateTimeNow;
            #endregion

            qDB.emCompanyProfiles.InsertOnSubmit(model);
            qDB.SubmitChanges();
            IsResult = true;
             

            return IsResult;
        }
        #endregion

        #region Update

        public bool UpdateCompany(emCompany model)
        {
            return IsResult;
        }
        public bool UpdateCompanyProfile(emCompanyProfile model)
        {
            return IsResult;
        }

        #endregion

        #region Delete

        public bool DeleteCompany(int id)
        {
            qDB.ExecuteCommand("update emCompany set rowflag = -1 where CompID = {0}", id);
            IsResult = true;
            return IsResult;
        }

        #endregion
    }
}
