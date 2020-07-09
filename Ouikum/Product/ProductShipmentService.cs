using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using Prosoft.Base;
using System.Transactions;
//using System.Web.Mvc;

namespace Ouikum.Product
{
    public class ProductShipmentServices : BaseSC
    {
        OuikumDataContext qDB;

        #region Constructor
        public ProductShipmentServices()
        {
            qDB = new OuikumDataContext(ConnectionString);
        }
        public ProductShipmentServices(OuikumDataContext conn)
        {
            qDB = conn;
        }
        #endregion

        #region ProductShipment

        #region Method Validate
        #region ValidateInsert
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool ValidateInsert(b2bProductShipment data)
        {
            try
            {
                b2bProductShipment model = qDB.b2bProductShipments.Single(q => q.ProductShipmentID == data.ProductShipmentID);
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
        public bool ValidateUpdateProductShipment(b2bProductShipment data)
        {
            try
            {
                b2bProductShipment model = qDB.b2bProductShipments.Single(q => q.ProductShipmentID == data.ProductShipmentID);
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

        #region GetProductShipment

        #region GetByID
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public b2bProductShipment GetByID(int id)
        {
            var data = qDB.view_ProductShipments.Single(m => m.ProductShipmentID == id);
            var model = new b2bProductShipment();
            model.ProductShipmentID = data.ProductShipmentID;
            model.CompID = data.CompID; 
           
            model.CompID = (int)data.CompID;
            model.RowFlag = data.RowFlag;
            return model;
        }
        #endregion

        public IQueryable<view_ProductShipment> GetProductShipment()
        {
            return qDB.view_ProductShipments.Where(it => it.RowFlag > 0);
        }
         

        #endregion

        #region ListProductShipment

        #endregion

        #endregion

        #region Method Insert
        #region InsertProductShipment

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool InsertProductShipment(b2bProductShipment model)
        {
            // default
            model.RowFlag = 1;
            model.RowVersion = 1;
            model.CreatedBy = "sa";
            model.CreatedDate = DateTimeNow;
            model.ModifiedBy = "sa";
            model.ModifiedDate = DateTimeNow;
            qDB.b2bProductShipments.InsertOnSubmit(model);// ทำการ save
            qDB.SubmitChanges();
            IsResult = true;
            return IsResult;
        }
         
         
        public bool InsertProductShipment(List<b2bProductShipment> model)
        {
            using (var trans = new TransactionScope())
            {

                foreach (var i in model)
                    InsertProductShipment(i);
            }
            return true;
        }

        //public bool InsertProductShipment(FormCollection form)
        //{
        //    return IsResult;
        //}
        #endregion
        #endregion

        #region Method Update
        #region Update

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateProductShipment(b2bProductShipment model)
        {
            var data = qDB.b2bProductShipments.Single(q => q.ProductShipmentID == model.ProductShipmentID);

            // set ค่า model
            data.ProductID = model.ProductID;
            data.CompID = model.CompID;
            data.RowFlag = (short)model.RowFlag;

            // default
            data.RowVersion++;
            data.ModifiedBy = "sa";
            data.ModifiedDate = DateTimeNow;

            qDB.SubmitChanges();// บันทึกค่า
            return true;
        }

        public bool UpdateProductShipment(List<b2bProductShipment> model)
        {
            foreach (var m in model)
            {
                UpdateProductShipment(m);
            }
            return true;
        }


        public bool UpdateStatus(List<int> id, short RowFlag, bool IsShow, bool IsJunk)
        {
         //   var data = qDB.view_ProductShipments.Where(m => id.Contains(m.ProductShipmentID));
            
                    var str = "UPDATE b2bProductShipment SET RowFlag = {0}  WHERE ProductShipmentID = {3}";
                    
            using (var trans = new TransactionScope())
            {
                for (var i = 0; i < id.Count(); i++)
                {
                    qDB.ExecuteCommand(str, RowFlag, IsShow, IsJunk, id[i]);

                    qDB.SubmitChanges();
                }
                trans.Complete();
            }


            for (var i = 0; i < id.Count(); i++)
            {
                qDB.ExecuteCommand("UPDATE b2bProductShipment SET RowFlag = {0} , IsShow = {1} , IsJunk = {2} WHERE ProductShipmentID = {3}", RowFlag, IsShow, IsJunk, id[i]);

                qDB.SubmitChanges();
            }
            return IsResult;
        }



        #endregion
        #endregion

        #region Method Delete
        #region Delete
        public bool Delete(int id)
        {
            try
            {
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
        #endregion
        
        #endregion


    }
}