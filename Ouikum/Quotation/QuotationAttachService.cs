using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using Prosoft.Base;
using System.Transactions;
//using System.Web.Mvc;
using System.Data.Linq;

using Prosoft.Service;

namespace Ouikum.QuotationAttach
{

    #region enum
    public enum QuotationAttachAction
    {
        All, BackEnd, Admin, Important, Sentbox, myRequest, Draft
    }
    #endregion

    public class QuotationAttachService : BaseSC
    {

        #region QuotationAttach

        #region Method Validate
        #region ValidateInsert
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool ValidateInsert(b2bQuotationAttach data)
        {
            try
            {
                b2bQuotationAttach model = qDB.b2bQuotationAttaches.Single(q => q.FileID == data.FileID);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region ValidateUpdate
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool ValidateUpdateQuotationAttach(b2bQuotationAttach data)
        {
            try
            {
                b2bQuotationAttach model = qDB.b2bQuotationAttaches.Single(q => q.FileID == data.FileID);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
        #endregion

        #region Method Select

        #region GetQuotationAttach
        #region Generate SQLWhere
        public string CreateWhereAction(QuotationAttachAction action, int? ToCompID = 0)
        {
            var sqlWhere = string.Empty;
            if (action == QuotationAttachAction.All)
            {
                sqlWhere = "( IsDelete = 0 ) ";
            }
            else if (action == QuotationAttachAction.BackEnd)
            {
                sqlWhere = "( IsDelete = 0 AND  RowFlag  = 1 )  ";
            }
            else if (action == QuotationAttachAction.Admin)
            {
                sqlWhere = "( IsDelete = 0 AND  RowFlag = 1 ) ";
            }
            else if (action == QuotationAttachAction.Important)
            {
                sqlWhere = "( IsDelete = 0 AND  IsImportance = 1 ) ";
            }
            else if (action == QuotationAttachAction.Sentbox)
            {
                sqlWhere = "( IsDelete = 0 AND  IsOutbox = 1 ) ";
            }

            if (ToCompID > 0)
                sqlWhere += "AND (ToCompID = " + ToCompID + ")";
            return sqlWhere;
        }

        public string CreateWhereCause(int ToCompID = 0, string txtSearch = "")
        {
            #region DoWhereCause
            if (ToCompID > 0)
                SQLWhere += " AND ToCompID = " + ToCompID;

            if (txtSearch != "")
            {
                SQLWhere += " AND (QuotationAttachCode LIKE N'" + txtSearch + "%' ";
                SQLWhere += " OR CompanyName LIKE N'" + txtSearch + "%' ";
                SQLWhere += " OR ReqFirstName LIKE N'" + txtSearch + "%' )";
            }

            #endregion

            return SQLWhere;
        }

        #endregion
        #endregion

        #endregion

        #region Method Insert

        #region InsertQuotationAttach

        #region ValidateSave
        #region ValidateQuotationAttach
        private bool ValidateQuotationAttach(b2bQuotationAttach model)
        {
            //Example
            if (model.FileCode == null)
            {
                IsResult = false;
            }
            else if (model.QuotationID == null)
            {
                IsResult = false;
            }

            return IsResult;
        }
        #endregion
        #endregion

        #region SaveQuotationAttach

        #region InsertQuotationAttach
        public bool InsertQuotationAttach(b2bQuotationAttach QuotationAttach)
        {
            try
            {
                var count = CountData<b2bQuotationAttach>(" * ", " CreatedDate = GetDate() AND RowFlag > 0");
                count = count + 1;
                QuotationAttach.FileCode = AutoGenCode("FC", count);
                QuotationAttach.CreatedDate = DateTime.Now;
                QuotationAttach.ModifiedDate = DateTime.Now;
                QuotationAttach.CreatedBy = "sa";
                QuotationAttach.ModifiedBy = "sa";
                QuotationAttach.RowFlag = 1;
                QuotationAttach.RowVersion = 1;
                QuotationAttach.IsDelete = false;

                qDB.b2bQuotationAttaches.InsertOnSubmit(QuotationAttach); // insert QuotationAttach
                qDB.SubmitChanges();
                IsResult = true;
            }
            catch (Exception e)
            {
                IsResult = false;
            }
            

            return IsResult;
        }
        #endregion
        #endregion

        #endregion

        #endregion

        #endregion

    }
}