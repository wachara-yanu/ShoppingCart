using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ouikum.Product;
using Ouikum.Web.Models;
namespace Ouikum.Web.Controllers.WinApi
{

    public partial class WinApiController : BaseController
    {
        //
        // GET: /WinApi/
        ProductService svProduct;
        public WinApiController()
        {
            svProduct = new ProductService();
        }

        #region product list
        public ActionResult productlist(string search, int cateid = 0, int catelv = 0, int pageindex = 0, int pagesize = 0, int compid = 0)
        {
            var sqlWhere = svProduct.CreateWhereAction(ProductAction.FrontEnd, compid);
            sqlWhere += svProduct.CreateWhereByCategory(catelv, cateid);
            if (!string.IsNullOrEmpty(search))
            {
                sqlWhere += "AND ( ProductName LIKE N'%" + search + "%' ) ";
            }

            var products = svProduct.SelectData<view_SearchProduct>(" * ", sqlWhere, "ModifiedDate DESC", pageindex, pagesize);
            var totalrow = svProduct.TotalRow;
            var totalpage = svProduct.TotalPage;
            var product = new List<ProductWinModel>();

            #region product gallery
            if (svProduct.TotalRow > 0)
            { 
                foreach (var p in products)
                {
                    var item = new ProductWinModel();
                    #region set product model
                    item.productid = p.ProductID;
                    item.productname = p.ProductName;
                    item.price = (decimal)p.Price;
                    item.promotionprice = (p.PromotionPrice > 0) ? (decimal)p.PromotionPrice : 0;
                    item.ispromotion = (p.IsPromotion != null) ? (bool)p.IsPromotion : false;
                    item.companyname = p.CompName;
                    item.companylevel = (int)p.CompLevel;
                    item.companyid = (int)p.CompID; 
                    item.createdate = p.CreatedDate;
                    item.modifieddate = p.ModifiedDate;
                    item.urlimage = p.ProductImgPath;
                    item.shortdescription = p.ShortDescription;
                    item.provinceid = (int)p.provinceid;
                    item.provincename =  p.ProvinceName; 
                    #endregion
 
                    product.Add(item);
                }
            }
            #endregion


            // var gallery = svProduct.SelectData<b2bProductImage>(" * ", sqlWhere);

            return Json(new { product = product, totalrow = totalrow, totalpage = totalpage }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        

        public ActionResult CompanyExportExcel(List<int> ProductID)
        {
            var sqlWhere = svProduct.CreateWhereAction(ProductAction.FrontEnd);
            sqlWhere += CreateWhereIN(ProductID, "ProductID");
            var products = svProduct.SelectData<view_Product>(" * ",sqlWhere);
            var data = new System.Data.DataTable("teste");
            data.Columns.Add("ProductID", typeof(string));
            data.Columns.Add("ProductCode", typeof(string));
            data.Columns.Add("ProductName", typeof(string));
            data.Columns.Add("CompName", typeof(string));

            foreach (var item in products)
            {
                data.Rows.Add(item.ProductID, item.ProductCode, item.ProductName, item.CompName);
            }

            var grid = new GridView();
            grid.DataSource = data;
            grid.DataBind();

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=B2BThaiProducts.xls");
            Response.ContentType = "application/ms-excel";

            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            grid.RenderControl(htw);

            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return View("MyView");
        }

    }
}
