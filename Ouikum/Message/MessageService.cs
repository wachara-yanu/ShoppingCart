using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using Prosoft.Base;
//using System.Web.Mvc; 

using Prosoft.Service; 
using System.Transactions;

using Ouikum.Company;
namespace Ouikum.Message
{
    #region enum
    public enum MessageStatus
    {
        All,
        Inbox,
        UnRead,
        Sentbox,
        Important,
        Draft,
        Report,
        Trash
    }

    public enum OrderBy
    {
        ModifiedDateDESC,
        ModifiedDate,
        CreatedDateDESC,
        CreatedDate
    }
    #endregion
     
    public class MessageService : BaseSC
    { 

            #region Members  
            
            #endregion
         
            #region Message

            #region Insert
            public emMessage InsertMessage(emMessage model)
            {
                #region set default
                model.IsDelete = false;
                model.IsRead = false;
                model.IsForward = false;
                model.IsReply = false;
                model.IsShow = true;
                model.RowFlag = 1;
                model.CreatedBy = "sa";
                model.ModifiedBy = "sa";
                model.SendDate = DateTimeNow;
                model.ModifiedDate = DateTimeNow;
                model.CreatedDate = DateTimeNow;
                #endregion

                qDB.emMessages.InsertOnSubmit(model);
                qDB.SubmitChanges();
                IsResult = true;
                return model;
            }
            #endregion

            #region Insert File
            public emMessageAttach InsertMessageFile(emMessageAttach model)
            {
                #region set default
                model.IsDelete = false;
                model.IsShow = true;
                model.RowFlag = 1;
                model.CreatedBy = "sa";
                model.ModifiedBy = "sa";
                model.ModifiedDate = DateTimeNow;
                model.CreatedDate = DateTimeNow;
                #endregion

                qDB.emMessageAttaches.InsertOnSubmit(model);
                qDB.SubmitChanges();
                IsResult = true;
                return model;
            }
            #endregion
            #region Insert Reply Forward
            public emMessage InsertMessageReply(emMessage model,string type)
            {
                #region set default
                if (type == "Reply")
                {
                    model.IsReply = true;
                }
                else
                {
                    model.IsReply = false;
                }

                if (type == "Forward")
                {
                    model.IsForward = true;
                }
                else
                {
                    model.IsForward = false;
                }

                model.IsDelete = false;
                model.IsRead = false;
                model.IsShow = true;
                model.RowFlag = 1;
                model.CreatedBy = "sa";
                model.ModifiedBy = "sa";
                model.SendDate = DateTimeNow;
                model.ModifiedDate = DateTimeNow;
                model.CreatedDate = DateTimeNow;
                #endregion

                qDB.emMessages.InsertOnSubmit(model);
                qDB.SubmitChanges();
                IsResult = true;
                return model;
            }
            #endregion

            #region Generate_MessageCode
            public string Generate_MessageCode()
            {
                int maxLength = 4;
                string allowedChars = "abcdefghijkmnopqrstuvwxyz0123456789";
                //string dash = "-";
                char[] chars = new char[maxLength];
                Random rd = new Random();

                for (int i = 0; i < maxLength; i++)
                {
                    chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
                }

                return new string(chars);
            }
            #endregion 
            #endregion
        #region GetProduct

        #region Generate SQLWhere
        public string CreateWhereAction(MessageStatus action, int? CompID = 0)
        {
            var sqlWhere = string.Empty;
            #region Condition
            if (action == MessageStatus.All)
            {
                sqlWhere = "( IsDelete = 0 ) ";
            }
            else if (action == MessageStatus.Inbox)
            {
                //comprowflag มาจาก b2bcompany.rowflag
                sqlWhere = "( IsDelete = 0 AND IsSend = 0 AND MsgFolderID = 1) ";
            }
            else if (action == MessageStatus.UnRead)
            {
                //comprowflag มาจาก b2bcompany.rowflag
                sqlWhere = "( IsDelete = 0 AND IsSend = 0 AND IsRead = 0 AND MsgFolderID = 1) ";
            }
            else if (action == MessageStatus.Sentbox)
            {
                sqlWhere = "( IsDelete = 0 AND IsSend = 1 AND MsgFolderID = 2) ";
            }
            else if (action == MessageStatus.Draft)
            {
                sqlWhere = "( IsDelete = 0  AND MsgFolderID = 3 ) ";
            }
            else if (action == MessageStatus.Important)
            {
                sqlWhere = "( IsDelete = 0 AND IsFavorite = 1 AND MsgFolderID != 4) ";
            }
            else if (action == MessageStatus.Report)
            {
                sqlWhere = "( IsDelete = 0 AND IsSend = 0 AND MsgFolderID = 1) ";
            }
            else if (action == MessageStatus.Trash)
            {
                sqlWhere = "( IsDelete = 0 AND MsgFolderID = 4) ";
            }
           

            if (CompID > 0)
            {
                if (action == MessageStatus.Sentbox )
                {
                    sqlWhere += " AND (FromCompID = " + CompID + ") ";
                }
                else if (action == MessageStatus.Draft)
                {
                    sqlWhere += " AND (FromCompID = " + CompID + ") ";//AND ToCompID = " + CompID + " 
                }
                else if (action == MessageStatus.Trash)
                {
                    sqlWhere += " AND (FromCompID = " + CompID + " OR ToCompID = " + CompID + ") ";//AND ToCompID = " + CompID + " 
                }

               //else if (action == MessageStatus.Important){
                //    sqlWhere += "AND ((FromCompID = " + CompID + ") OR (ToCompID = " + CompID + "))";
                //}
                else if (action == MessageStatus.Report)
                {
                    sqlWhere += "";
                }
                else
                {
                    sqlWhere += " AND (ToCompID = " + CompID + ") ";
                }
            }
            #endregion

            return sqlWhere;
        }

        public string CreateWhereCause(
            int CompID = 0, string txtSearch = "", int PStatus = 0, int GroupID = 0,
            int CateLevel = 0, int CateID = 0, int BizTypeID = 0, int CompLevel = 0,
            int CompProvinceID = 0
            )
        {
            #region DoWhereCause
            if (CompID > 0)
                SQLWhere += " AND CompID = " + CompID;

            if (!string.IsNullOrEmpty(txtSearch))
                SQLWhere += " AND ProductName LIKE N'" + txtSearch + "%' ";


            if (PStatus > 0)
                SQLWhere += " And RowFlag = " + PStatus;


            if (GroupID > 0)
                SQLWhere += " AND ProductGroupID  =  " + GroupID;
             

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
            }
            #endregion
            return SqlOrderBy;
        }
        #endregion
        #endregion

        #region MessageAttach
        public emMessageAttach InsertMessageAttach(emMessageAttach model)
        {
            #region set default
            model.IsDelete = false; 
            model.IsShow = true;
            model.RowFlag = 1;
            model.CreatedBy = "sa";
            model.ModifiedBy = "sa";
            model.ModifiedDate = DateTimeNow;
            model.CreatedDate = DateTimeNow;
            #endregion

            qDB.emMessageAttaches.InsertOnSubmit(model);
            qDB.SubmitChanges();
            IsResult = true;
            return model;
        }

        #endregion




    }
}