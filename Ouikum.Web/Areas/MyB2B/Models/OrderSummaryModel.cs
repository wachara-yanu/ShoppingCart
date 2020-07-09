using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ouikum.Order
{
    public class OrderSummaryModel
    {
        public int? OrderID { get; set; }
        public int? OrderDetailID { get; set; }
        public int PackageID { get; set; }
        public string PackageName { get; set; }
        public int Qty { get; set; }
        public decimal Price { get; set; }

    }
}