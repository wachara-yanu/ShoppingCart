using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ouikum.Web.Models
{ 
    #region Product Model 

    public class ProductWinModel
    {
        public int productid { get; set; }
        public string productname { get; set; }
        public string shortdescription { get; set; }
        public string qtyunit { get; set; }
        public int qty { get; set; }
        public int companyid { get; set; }
        public int companylevel { get; set; }
        public string companyname { get; set; }
        public string address { get; set; }
        public int provinceid { get; set; }
        public string provincename { get; set; }
        public int districtid { get; set; }
        public string districtname { get; set; }
        public string contactname { get; set; }
        public string tel { get; set; }
        public string fax { get; set; }
        public string website { get; set; }
        public string email { get; set; }
        public string urldetail { get; set; }
        public string urlimage { get; set; }
        public string urllist { get; set; }  
        public string productdetail { get; set; }
        public decimal price { get; set; }
        public decimal promotionprice { get; set; }
        public bool ispromotion { get; set; }
        public DateTime createdate { get; set; }
        public DateTime modifieddate { get; set; }
    } 
    #endregion
     

}