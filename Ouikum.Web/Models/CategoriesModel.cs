using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ouikum.Web.Models
{
    public class CategoryModel
    {
        public int CategoryID { get; set; }
        public string CategoryCode { get; set; }
        public string CategoryName { get; set; }
        public string CategoryNameEng { get; set; }
        public string ParentCategoryID { get; set; }
        public string ParentCategoryPath { get; set; }
        public string CategoryKeyword { get; set; }
        public string CategoryLevel { get; set; }
        public string CategoryImgPath { get; set; }
        public string CategoryType { get; set; }
        public string ListNo { get; set; }
        public string Remark { get; set; }
        public string ProductCount { get; set; }
        public string BuyLeadCount { get; set; }
        public string RowFlag { get; set; }
        public string CreatedDate { get; set; }
        public string IsShow { get; set; }
        public string IsDelete { get; set; }
    }
}