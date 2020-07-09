using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ouikum.Product;
using Ouikum.Category;
//using Prosoft.Base;

namespace Ouikum.Web.Admin
{
    public partial class CategoryController : BaseSecurityAdminController
    {
        #region ListDoLoadData
        public void ListDoLoadData()
        {
            var svCategory = new CategoryService();
            var Category = svCategory.SelectData<b2bCategoryType>(" * ", "IsDelete = 0 AND RowFlag > 0","CategoryTypeName");
            
          
        }
        #endregion

        #region CategoryType
        #region ListDoLoadCategory
        public void ListDoLoadCategory(int CategoryType)
        {
            var svCategory = new CategoryService();
            var Categories = svCategory.SelectData<b2bCategory>(" * ", "IsDelete = 0 AND RowFlag > 0 AND CategoryType = '" + CategoryType + "'", "CategoryName");
            ViewBag.Categories = Categories;
        } 
        #endregion
         
        #endregion
    }
}
