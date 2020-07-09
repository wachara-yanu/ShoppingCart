using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using Prosoft.Base;

namespace Ouikum.Purchase
{
    public class PurchaseService : BaseSC
    {
        #region Insert

        #region AssignLead
        public int InsertAssignLead(b2bAssignLead model)
        {
            #region set default
            model.RowFlag = 1;
            model.CreatedBy = "sa";
            model.ModifiedBy = "sa";
            model.ModifiedDate = DateTimeNow;
            model.CreatedDate = DateTimeNow;
            model.IsDelete = false;
            model.RowVersion = 1;
            #endregion

            qDB.b2bAssignLeads.InsertOnSubmit(model);
            qDB.SubmitChanges();
            IsResult = true;
            return model.AssignLeadID;
        }
        #endregion

        #region CompanyLead
        public bool InsertCompanyLead(b2bCompanyLead model)
        {
            #region set default
            model.RowFlag = 1;
            model.CreatedBy = "sa";
            model.ModifiedBy = "sa";
            model.ModifiedDate = DateTimeNow;
            model.CreatedDate = DateTimeNow;
            model.IsDelete = false;
            model.RowVersion = 1;
            #endregion

            qDB.b2bCompanyLeads.InsertOnSubmit(model);
            qDB.SubmitChanges();
            IsResult = true;
            return IsResult;
        }
        #endregion

        #region EmLead
        public int InsertEmLead(b2bEmLead model)
        {
            #region set default
            model.RowFlag = 1;
            model.CreatedBy = "sa";
            model.ModifiedBy = "sa";
            model.ModifiedDate = DateTimeNow;
            model.CreatedDate = DateTimeNow;
            model.IsDelete = false;
            model.IsMark = false;
            model.IsImportance = false;
            model.RowVersion = 1;
            #endregion

            qDB.b2bEmLeads.InsertOnSubmit(model);
            qDB.SubmitChanges();
            IsResult = true;
            return model.EmLeadID;
        }
        #endregion

        #region EmCompanyLead
        public bool InsertEmCompanyLead(b2bEmCompanyLead model)
        {
            #region set default
            model.RowFlag = 1;
            model.CreatedBy = "sa";
            model.ModifiedBy = "sa";
            model.ModifiedDate = DateTimeNow;
            model.CreatedDate = DateTimeNow;
            model.IsDelete = false;
            model.RowVersion = 1;
            #endregion

            qDB.b2bEmCompanyLeads.InsertOnSubmit(model);
            qDB.SubmitChanges();
            IsResult = true;
            return IsResult;
        }
        #endregion

        #endregion

        #region GenLeadCode
        public string GenLeadCode(int CompID,int LeadType)
        {
            string LeadCode = "";
            if(LeadType == 1)
            {
                LeadCode = "AL";
            }
            else if(LeadType == 2)
            {
                LeadCode = "EM";
            }
            else if (LeadType == 3) 
            {
                LeadCode = "EC";
            }

            if (CompID > 0) {
                LeadCode = LeadCode.ToUpper() + "-" + CompID.ToString("00000") + "-" + RandomCharInt(3);
            } 
            else {
                LeadCode = LeadCode.ToUpper() + "-" + RandomCharInt(5) + "-" + RandomCharInt(3);
            }

            return LeadCode;
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
    }
}
