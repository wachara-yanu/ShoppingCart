using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

//usin other
//using Prosoft.Base;
using Ouikum.Common;
using Prosoft.Service;
using Ouikum.Product;
using Ouikum.Category;
using Ouikum.Company;


namespace Ouikum.Controllers
{
    public partial class StoreController : BaseController
    {
        //
        // GET: /Search/
        ProductService svProduct;
        ProductImageService svProductImage;
        CategoryService svCategory;
        CompanyService svCompany;
        Ouikum.BizType.BizTypeService svBizType;

        public StoreController()
        {
            svProduct = new Product.ProductService();
            svProductImage = new ProductImageService();
            svCategory = new CategoryService();
            svBizType = new Ouikum.BizType.BizTypeService();
            svCompany = new CompanyService();
        }


    }
}
