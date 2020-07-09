using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using Prosoft.Base;
using System.Transactions;
//using System.Web.Mvc;
using Ouikum;

namespace Ouikum.Product
{
    public class ProductGroupService : BaseSC
    { 
         

        #region ProductGroup
         
        #region ValidateInsert
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool CheckDuplicate(b2bProductGroup data)
        {
            try
            {
                int count = 0;
                //if (data.ProductGroupID > 0)
                //{
                    count = qDB.b2bProductGroups
                        .Where(m => m.CompID == data.CompID && m.ProductGroupName == data.ProductGroupName && m.RowFlag > 0)
                        .Count();
                //}
                if (count > 0)
                {
                    var ex = new ArgumentException("can't save this item because item is exist");
                    MsgError.Add(ex);
                    IsResult = false;
                    return IsResult;
                }
                else
                {
                    IsResult = true;
                    return IsResult;
                }

            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region ValidateDelete
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool ValidateDelete(int ProductGroupID,int CompID)
        { 
                var svProduct = new ProductService();
                var sqlWhere = svProduct.CreateWhereAction(ProductAction.All,CompID);
                sqlWhere = svProduct.CreateWhereCause(0,"",ProductGroupID);
                int count = CountData<b2bProduct>(" ProductID ", sqlWhere);
                if (count > 0)
                {
                    IsResult = false;
                }
                else
                {
                    IsResult = true;
                } 

                return IsResult; 
        }
        #endregion

         
        #region GetProductGroup
        public List<b2bProductGroup> GetProductGroup(int CompID)
        {
            var sqlWhere = "IsDelete = 0 and CompID = "+CompID;
            var data = SelectData<b2bProductGroup>("ProductGroupID,ProductGroupName,RowVersion,ListNo", sqlWhere, "ListNo,ProductGroupName");
            return data;
        }
        #endregion

        #region SaveProductGroup

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool SaveProductGroup(b2bProductGroup model)
        {
            //default
            if (CheckDuplicate(model))
            {
                var data = new b2bProductGroup();
                //Update
                if (model.ProductGroupID > 0)
                {
                   data =  qDB.b2bProductGroups.Single(m => m.ProductGroupID == model.ProductGroupID);
                   data.ProductGroupName = model.ProductGroupName;
                   data.ListNo = data.ListNo;
                   data.CompID = model.CompID;
                }
                //Add
                else
                { 
                    data.ProductGroupName = model.ProductGroupName;
                    data.CompID = model.CompID;
                    data.ListNo = model.ListNo;
                    data.IsShow = true;
                    data.CreatedBy = "sa";
                    data.ModifiedBy = "sa";
                    data.CreatedDate = DateTimeNow;
                    data.ModifiedDate = DateTimeNow;
                }
                data.RowFlag = 1;
                SaveData<b2bProductGroup>(data, "ProductGroupID");

            }
            return IsResult;
        }
          
        #endregion 
         
        #region Update

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateProduct(b2bProduct model)
        {
            var data = qDB.b2bProducts.Single(q => q.ProductID == model.ProductID);

            // set ค่า model
            data.ProductName = model.ProductName;
            data.RowFlag = (short)model.RowFlag;

             //default
            data.RowVersion++;
            data.ModifiedBy = "sa";
            data.ModifiedDate = DateTimeNow;

            qDB.SubmitChanges();// บันทึกค่า
            return true;
        }

        public bool UpdateProduct(List<b2bProduct> model)
        {
            foreach (var m in model)
            {
                UpdateProduct(m);
            }
            return true;
        }


        public bool UpdateStatus(List<int> id, short RowFlag, bool IsShow, bool IsJunk)
        {
         //   var data = qDB.b2bProducts.Where(m => id.Contains(m.ProductID));
            
                    var str = "UPDATE b2bProduct SET RowFlag = {0} ,  IsJunk = {2} WHERE ProductID = {3}";
                    
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
                qDB.ExecuteCommand("UPDATE b2bProduct SET RowFlag = {0} , IsShow = {1} , IsJunk = {2} WHERE ProductID = {3}", RowFlag, IsShow, IsJunk, id[i]);

                qDB.SubmitChanges();
            }
            return IsResult;
        }
        public bool UpdateProductGroup(List<int> id, List<int> no)
        {
            var str = "UPDATE b2bProductGroup SET ListNo = {0}  WHERE ProductGroupID = {1}";

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
         
        #region Delete
        public bool Delete(int GroupID, int CompID)
        {
            var svProduct = new ProductService();

            svProduct.ChangeGroup(CompID, GroupID, 0);

            if (svProduct.IsResult)
            {
                UpdateByCondition<b2bProductGroup>(" RowFlag = -1  , IsDelete = 1 ","ProductGroupID = " + GroupID);
                return IsResult;
            }
            else
            {
                IsResult = false;
                return IsResult;
            }

        }
        #endregion 

        #endregion
    }
}