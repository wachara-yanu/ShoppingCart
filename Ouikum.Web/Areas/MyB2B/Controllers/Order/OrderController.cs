using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ouikum.Order;
using Ouikum.Company;
using Prosoft.Service;
//using Prosoft.Base;
using System.Transactions;
using System.Data.Linq;
using Ouikum;

namespace Ouikum.Web.MyB2B
{
    public partial class OrderController : BaseSecurityController
    {
        #region CreateOrderDetialCode
        public string CreateOrderDetialCode(int PackageID)
        {
            var str = string.Empty;
            if (PackageID == 3)
            {
                str = "OD-GM";
            }
            else if (PackageID == 4)
            {
                str = "OD-FEA";
            }
            else if (PackageID == 5)
            {
                str = "OD-HOT";
            }
            return str;
        }
        #endregion

        #region ListDoLoadData
        public void ListDoLoadData()
        {
            var svOrder = new OrderService();
            var svPackage = new PackageService();
            if (svOrder.CheckMemberPackage(LogonCompID, LogonCompLevel))
            {
                var Package = svPackage.SelectData<b2bPackage>("PackageID,PackageName,Price,Duratrion", "PackageID = 3").First();
                ViewBag.MemberPackage = Package;
            }
            if (svOrder.ChackHotPackage(LogonCompID, LogonCompLevel))
            {
                if (svOrder.MaxHotQty > 0)
                {
                    var HotPackage = svPackage.SelectData<b2bPackage>("PackageID,PackageName,Price,Duratrion", "PackageID = 5").First();
                    ViewBag.HotPackage = HotPackage;
                    ViewBag.MaxHotQty = svOrder.MaxHotQty;
                }
            }
            if (svOrder.ChackFeatPackage(LogonCompID, LogonCompLevel))
            {
                if (svOrder.MaxFeatQty > 0)
                {
                    var FeatPackage = svPackage.SelectData<b2bPackage>("PackageID,PackageName,Price,Duratrion", "PackageID = 4").First();
                    ViewBag.FeatPackage = FeatPackage;
                    ViewBag.MaxFeatQty = svOrder.MaxFeatQty;
                }
            }
        }
        #endregion

    }
}
