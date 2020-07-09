using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using Prosoft.Base;
using System.Transactions;
//using System.Web.Mvc;
using System.Data.Linq;

using Prosoft.Service;

namespace Ouikum.Product
{

    public class ProductImageService : BaseSC
    {

        public string CreateWhereCause(
            int ProductID = 0
            )
        {
            #region DoWhereCause
            SQLWhere = "IsDelete = 0 ";

            if (ProductID > 0)
                SQLWhere += " AND ProductID = " + ProductID;
 
            #endregion

            return SQLWhere;
        }

        #region InsertProductImage
        public bool InsertProductImage(b2bProductImage productImages)
        { 
            productImages.IsDelete = false;
            productImages.RowVersion = 1;
            productImages.CreatedDate = DateTimeNow;
            productImages.ModifiedDate = DateTimeNow;
            productImages.CreatedBy = "sa";
            productImages.ModifiedBy = "sa";
            qDB.b2bProductImages.InsertOnSubmit(productImages);
            IsResult = true;
            SaveData<b2bProductImage>(productImages,"ProductImageID");
            return IsResult;
        }
         
        #endregion


    }
}