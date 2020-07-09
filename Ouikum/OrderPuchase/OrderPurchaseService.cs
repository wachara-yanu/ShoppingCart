using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using Prosoft.Base;
using System.Transactions;
//using System.Web.Mvc;
using System.Data.Linq;


namespace Ouikum.OrderPuchase
{
    #region enum
    public enum OrderPurchaseAction
    {
        All, BackEnd, Admin, Important, Sentbox, Trash, myRequest, Draft, FrontEnd, CountOrderPurchase
    }
    #endregion

   

    public class OrderPurchaseService : BaseSC
    {
        #region Proprety
        public string OrderCode { get; set; }
        public decimal Total { get; set; }
        #endregion

        public string CreateWhereAction(OrderPurchaseAction action, int? ToCompID = 0)
        {
            var sqlWhere = string.Empty;
            if (action == OrderPurchaseAction.All)
            {
                sqlWhere = " ( IsDelete = 0 )   ";
            }
            else if (action == OrderPurchaseAction.FrontEnd)
            {
                sqlWhere = " ( IsDelete = 0 AND  RowFlag  = 1 AND IsPublic = 1 AND IsClosed = 0)";
            }
            else if (action == OrderPurchaseAction.BackEnd)
            {
                sqlWhere = " ( IsDelete = 0 AND  RowFlag  = 1 AND IsOutBox = 0 AND QuotationFolderID = 1)  ";
            }
            else if (action == OrderPurchaseAction.CountOrderPurchase)
            {
                sqlWhere = " ( IsDelete = 0 AND  RowFlag  = 1 AND StatusProduct = 'A' AND CompSCID = "+ ToCompID + " ) ";
            }
            else if (action == OrderPurchaseAction.Admin)
            {
                sqlWhere = " ( IsDelete = 0 AND  RowFlag = 1 ) AND IsOutbox = 1 AND QuotationFolderID = 2";
            }
            else if (action == OrderPurchaseAction.Important)
            {
                sqlWhere = " ( IsDelete = 0 AND  IsImportance = 1 ) AND (ToCompID = " + ToCompID + ") ";
            }
            else if (action == OrderPurchaseAction.Sentbox)
            {
                sqlWhere = " ( IsDelete = 0 AND QuotationFolderID = 2 AND IsOutbox = 1 AND FromCompID = " + ToCompID + " ) ";
            }
            else if (action == OrderPurchaseAction.Trash)
            {
                sqlWhere = " ( IsDelete = 0 AND QuotationFolderID = 4 ";
            }
            else
            {
                sqlWhere = " ( IsDelete = 0 )   ";
            }

            if (action != OrderPurchaseAction.Sentbox)
            {
                if (action != OrderPurchaseAction.Trash)
                {
                    if (ToCompID > 0)
                    {
                        sqlWhere += " AND CompSCID  = " + ToCompID + " ";
                    }
                }
                //else
                //{
                //    sqlWhere += " AND (FromCompID = " + ToCompID + " OR ToCompID = " + ToCompID + ")) ";
                //}
            }

            return sqlWhere;
        }


        public bool UpdateStatusOrder(OuikumOrderDetail orDetial)
        {
            using (var trans = new TransactionScope())
            {
                string sqlUpdate = "StatusProduct = '" + orDetial.StatusProduct+"'";
                string sqlWhere = "OrDetailID = " + orDetial.OrDetailID + " AND ProductID = " + orDetial.ProductID;
                UpdateByCondition<OuikumOrderDetail>(sqlUpdate, sqlWhere);

                trans.Complete();
                IsResult = true;
            }
            return IsResult;
        }

    }
}
