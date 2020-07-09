using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ouikum.Web.Models
{
    public class tbProduct
    {
        //public ObjectId Id { get; set; }
        public int? ProductID { get; set; }
        public string ProductKeyword { get; set; }
        public int? ProductGroupID { get; set; }
        public string ProductName { get; set; }
        public string ShortDescription { get; set; }
        public decimal Price { get; set; }
        public decimal Qty { get; set; }
        public string QtyUnit { get; set; }
        public string ProductImgPath { get; set; }
        public short ViewCount { get; set; }
        public int ListNo { get; set; }
        public int CateLV1 { get; set; }
        public int CateLV2 { get; set; }
        public int CateLV3 { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int RowFlag { get; set; }
        public string ContactMobile { get; set; }
        public int CompID { get; set; }
        public string CompName { get; set; }
        public int Complevel { get; set; }
        public int ServiceType { get; set; }
        public int CompProvinceID { get; set; }
        public string ProvinceName { get; set; }
        public int CompDistrictID { get; set; }
        public int BizTypeID { get; set; }
        public string BizTypeName { get; set; }
        public string BizTypeOther { get; set; }
        public int ContactCount { get; set; }
        public bool? IsSME { get; set; }
        public bool? IsDelete { get; set; }
        public List<string> Keyword { get; set; }
    }
}