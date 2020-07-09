using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ouikum;
//using Prosoft.Base;
using System.Transactions;
//using System.Web.Mvc;
using System.Data.Linq;
using Prosoft.Service;
using Ouikum.Cart;
using Ouikum.Company;
namespace Ouikum.Cart
{
    public class CartService : BaseSC
    {
        public int CountCart { get; set; }

        #region Method Insert
        public bool InsertTempOrder(OuikumTempOrder tempOrder, List<OuikumTempOrderDetail> tempOrderDetails)
        {

            var Count = CountData<OuikumTempOrder>(" * ", " CreatedDate = GetDate() AND IsDelete = 0  ");
            Count = Count + 1;
            tempOrder.TOrderCode = AutoGenCode("OD", Count);
            tempOrder.CreatedDate = DateTime.Now;
            tempOrder.ModifiedDate = DateTime.Now;
            tempOrder.CreatedBy = "sa";
            tempOrder.ModifiedBy = "sa";

            using (var trans = new TransactionScope())
            {
                qDB.OuikumTempOrders.InsertOnSubmit(tempOrder); //insertTempOrder
                qDB.SubmitChanges();

                foreach (var it in tempOrderDetails)
                {
                    it.TOrderID = tempOrder.TOrderID;
                    it.CreatedDate = DateTime.Now;
                    it.ModifiedDate = DateTime.Now;
                    it.CreatedBy = "SA";
                    it.ModifiedBy = "SA";
                    qDB.OuikumTempOrderDetails.InsertOnSubmit(it);
                    qDB.SubmitChanges();     
                }

                IsResult = true;
            }

            return IsResult;
        }
        #endregion

        #region CheckMemberPackage
        public bool CheckMemberPackage(int CompID, int CompLevel)
        {
            var IsShow = false;
            if (CompLevel == 1)
            {
                var sqlwhere = "RowFlag > 0 AND (OrderStatus = 'N' OR OrderStatus = 'W' ) AND CompID =" + CompID + " AND PackageID = 3";
                var count = CountData<view_OrderDetail>(" * ", sqlwhere);
                if (count > 0)
                {
                    IsShow = false;
                }
                else
                {
                    IsShow = true;
                }
            }
            else if (CompLevel == 3)
            {
                var svCompany = new Company.CompanyService();
                if (svCompany.ValidateExpireDate(CompID))
                {
                    IsShow = true;
                    // renew member package
                }
                else
                {
                    IsShow = false;
                }

            }
            return IsShow;
        }
        #endregion

        #region tempcart
        public bool InserTempCart(TempCart temp)
        {
            temp.CreatedDate = DateTime.Now;
            temp.ModifiedDate = DateTime.Now;

            using (var trans = new TransactionScope())
            {
                qDB.TempCarts.InsertOnSubmit(temp); //insertTempOrder
                qDB.SubmitChanges();
                trans.Complete();
                IsResult = true;
            }

            return IsResult;
        }
        #endregion

        #region EditTempCart
        public bool UpdateTempCart(int LogOnuser, int ProductID, int? countProduct)
        {
            using (var trans = new TransactionScope())
            {
                string sqlUpdate = " TempCountProduct = " + countProduct;
                string sqlWhere = "TempIDLogon = " + LogOnuser+ "and TempProdcutID = " + ProductID;
                UpdateByCondition<TempCart>(sqlUpdate, sqlWhere);

                trans.Complete();
                IsResult = true;
            }
            return IsResult;
        }
        #endregion

        #region DeleteCart
        public bool DeleteCart(int TempID)
        {
            using (var trans = new TransactionScope())
            {
                string sqlUpdate = " IsDelete = 1";
                string sqlWhere = "TempID = " + TempID;
                UpdateByCondition<TempCart>(sqlUpdate, sqlWhere);
                trans.Complete();
                IsResult = true;
            }
            return IsResult;
        }
        #endregion

        #region MinusTempCart
        public bool MinusTempCart(int LogOnuser, int ProductID, int? countProduct)
        {

            using (var trans = new TransactionScope())
                {
                    string sqlUpdate = " TempCountProduct = " + countProduct;
                    string sqlWhere = "TempIDLogon = " + LogOnuser + "and TempProdcutID = " + ProductID;
                    UpdateByCondition<TempCart>(sqlUpdate, sqlWhere);

                    trans.Complete();
                    IsResult = true;
                }
            return IsResult;
        }
        #endregion


        public bool InsertOrder(OuikumOrder order, List<OuikumOrderDetail> orderDetail)
        {
            var Count = CountData<OuikumOrder>(" * ", " IsDelete = 0  AND CompID = " + order.CompID);

            order.OrderCode = AutoGenCode("Order-", Count);
            using (var trans = new TransactionScope())
            {
                qDB.OuikumOrders.InsertOnSubmit(order); // insert order
                qDB.SubmitChanges();

                var TempOrderID = order.OrderID;

                foreach (var it in orderDetail)
                {
                    it.OrderID = order.OrderID;
                    qDB.OuikumOrderDetails.InsertOnSubmit(it);
                    qDB.SubmitChanges();
                }
                trans.Complete();
                IsResult = true;
            }
            return IsResult;
        }

        #region UpdateCheckout TempCart
        public bool UpdateCheckOutTempCart(int LogOnuser)
        {
            using (var trans = new TransactionScope())
            {
                string sqlUpdate = " IsDelete = 1, IsShow = 0";
                string sqlWhere = "TempIDLogon = " + LogOnuser;
                UpdateByCondition<TempCart>(sqlUpdate, sqlWhere);

                trans.Complete();
                IsResult = true;
            }
            return IsResult;
        }
        #endregion


    }
}
