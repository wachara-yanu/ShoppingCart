using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using Prosoft.Base;
using System.Transactions;
//using System.Web.Mvc;
using System.Data.Linq;

using Prosoft.Service;

namespace Ouikum.Order
{

    #region enum
    public enum PackageAction
    {
        All,
        MaxProduct,
        MaxHot,
        MaxFeat
    }
    public enum PackageStatus
    {
        WaitApprove,
        NoApprove,
        Approve,
        Edited,
        WaitDetect 
    }

    public enum OrderBy
    {
        ModifiedDateDESC,
        ModifiedDate ,
        CreatedDateDESC ,
        CreatedDate,
        ViewCountDESC,
        ViewCount,
    }
    #endregion
     
    public class PackageService : BaseSC
    {
        #region Generate SQLWhere
        public string CreateWhereAction(PackageAction action, int? PackageID = 1)
        {
            var sqlWhere = string.Empty;
            if (action == PackageAction.All)
            {
                sqlWhere = "( IsDelete = 0 ) ";
            }
            else if (action == PackageAction.MaxProduct)
            {
                //comprowflag มาจาก b2bcompany.rowflag
                sqlWhere = "( IsDelete = 0 AND OptionCode = 'maxProduct' )";
            }
            else if (action == PackageAction.MaxHot)
            {
                sqlWhere = "( IsDelete = 0 AND OptionCode = 'maxHotProduct' ) ";
            }
            else if (action == PackageAction.MaxFeat)
            {
                sqlWhere = "( IsDelete = 0 AND OptionCode = 'maxFeatProduct' ) ";
            }
            
            if (PackageID > 0)
                sqlWhere += "AND (PackageID = " + PackageID + ")";

            return sqlWhere;
        }

        #endregion

        public int GetMaxProduct(int PackageID = 1)
        {
            if (PackageID != 1) 
                PackageID = 3; 
            var Package = SelectData<b2bPackageOption>(" * ", " PackageID = "+PackageID+" AND OptionCode = 'maxProduct'").First();
            return int.Parse(Package.OptionValue);
        }
    }
}