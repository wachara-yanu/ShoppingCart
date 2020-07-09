using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using res = Prosoft.Resource.Web.Ouikum;
//using Prosoft.Base;
using Ouikum.Product;
using Ouikum.Category;
using Ouikum.Company;
using Ouikum.Common;
using Ouikum;

namespace Ouikum.Web.Search
{
    public partial class ProductController : BaseController
    {
        ProductService svProduct;
        ProductImageService svProductImage;
        CategoryService svCategory;
        CompanyService svCompany;
        Ouikum.BizType.BizTypeService svBizType;
        AddressService svAddress;

        public ProductController()
        {
            svProduct = new ProductService();
            svProductImage = new ProductImageService();
            svCategory = new CategoryService();
            svBizType = new Ouikum.BizType.BizTypeService();
            svCompany = new CompanyService();
            svAddress = new AddressService();
        }

        #region LoadFeatureProduct
        [OutputCache(Duration = 900, VaryByParam = "FeatureProduct")]
        public void LoadFeatureProduct(string TextSearch,  int? Category,int? CateLevel)
        {
            string sqlWhere = "";
            string sqlSelect = "it.* ";
            string sqlOrderBy = " NEWID() ";
            var svFeatureProduct = new ProductService();
            //var sqlwherein = "";
            //switch (res.Common.lblWebsite)
            //{
            //    case "B2BThai": sqlwherein = " AND CategoryType IN (1,2)"; break;
            //    case "AntCart": sqlwherein = " AND CategoryType IN (3)"; break;
            //    case "myOtopThai": sqlwherein = " AND CategoryType IN (5)"; break;
            //    case "AppstoreThai": sqlwherein = " AND CategoryType IN (6)"; break;
            //    default: sqlwherein = ""; break;
            //}
            
            if (TextSearch != null)
            {
                sqlWhere = "(it.Status = 'F') AND (it.ExpiredDate > GETDATE()) AND (ProductID > 0) AND(it.RowFlag >= 3) AND IsShow=1 AND IsJunk=0 AND ProductDelete=0 AND ProductName LIKE N'%" + TextSearch + "%'";
                //sqlWhere += sqlwherein;
                var Features = svFeatureProduct.SelectData<view_HotFeaProduct>(sqlSelect, sqlWhere, sqlOrderBy, 1, 0);
                if (Features.Count == 0)
                {
                    //sqlWhere = "(it.Status = 'F') AND (it.ExpiredDate > GETDATE()) AND (ProductID > 0) AND(it.RowFlag >= 3)";
                    //var FeatureAll = svFeatureProduct.SelectData<view_HotFeaProduct>(sqlSelect, sqlWhere, sqlOrderBy, 1, 0);
                    //ViewBag.FeatureProducts = FeatureAll;
                    ViewBag.FeatureCount =0;
                }
                else
                {
                    ViewBag.FeatureProducts = Features;
                    ViewBag.FeatureCount = Features.Count;
                }

            }
            else if (CateLevel != null)
            {
                if (CateLevel == 1)
                {
                    sqlWhere = "(it.Status = 'F') AND (it.ExpiredDate > GETDATE()) AND (ProductID > 0) AND(it.RowFlag >= 3) AND IsShow=1 AND IsJunk=0 AND ProductDelete=0 AND (CategoryID = " + Category + " )";
                    //sqlWhere += sqlwherein;
                    var Features = svFeatureProduct.SelectData<view_HotFeaProduct>(sqlSelect, sqlWhere, sqlOrderBy, 1, 0);
                    if (Features.Count == 0)
                    {
                        //sqlWhere = "(it.Status = 'F') AND (it.ExpiredDate > GETDATE()) AND (ProductID > 0) AND(it.RowFlag >= 3)";
                        //var FeatureAll = svFeatureProduct.SelectData<view_HotFeaProduct>(sqlSelect, sqlWhere, sqlOrderBy, 1, 0);
                        //ViewBag.FeatureProducts = FeatureAll;
                        ViewBag.FeatureCount = 0;
                    }
                    else
                    {
                        ViewBag.FeatureProducts = Features;
                        ViewBag.FeatureCount = Features.Count;
                    }
                }else{
                    //sqlWhere = "(it.Status = 'F') AND (it.ExpiredDate > GETDATE()) AND (ProductID > 0) AND(it.RowFlag >= 3)";
                    //var FeatureAll = svFeatureProduct.SelectData<view_HotFeaProduct>(sqlSelect, sqlWhere, sqlOrderBy, 1, 0);
                    //ViewBag.FeatureProducts = FeatureAll;
                    //ViewBag.FeatureCount = FeatureAll.Count;
                    ViewBag.FeatureCount = 0;
                }
            }
            else {
                //sqlWhere = "(it.Status = 'F') AND (it.ExpiredDate > GETDATE()) AND (ProductID > 0) AND(it.RowFlag >= 3)";
                //var FeatureAll = svFeatureProduct.SelectData<view_HotFeaProduct>(sqlSelect, sqlWhere, sqlOrderBy, 1, 0);
                //ViewBag.FeatureProducts = FeatureAll;
                //ViewBag.FeatureCount = FeatureAll.Count;
                ViewBag.FeatureCount = 0;
            }
        }
        #endregion

        #region LoadHotProduct
        [OutputCache(Duration = 900, VaryByParam = "HotProduct")]
        public void LoadHotProduct(string TextSearch, int? Category, int? CateLevel)
        {
            string sqlWhere = "";
            string sqlSelect = "it.* ";
            string sqlOrderBy = " HotPrice DESC";
            var svHotProduct = new ProductService();
            //var sqlwherein = "";
            //switch (res.Common.lblWebsite)
            //{
            //    case "B2BThai": sqlwherein = " AND CategoryType IN (1,2)"; break;
            //    case "AppstoreThai": sqlwherein = " AND CategoryType IN (1,2)"; break;
            //    case "AntCart": sqlwherein = " AND CategoryType IN (2,3)"; break;
            //    case "myOtopThai": sqlwherein = " AND CategoryType IN (2,3)"; break;
            //    default: sqlwherein = ""; break;
            //}
            if (TextSearch != null)  //AND (it.ExpiredDate > GETDATE()) 
            {
                sqlWhere = "(it.Status = 'H') AND (ProductID > 0) AND(it.RowFlag >= 3) AND IsShow=1 AND IsJunk=0  AND ProductDelete=0 AND ProductName LIKE N'%" + TextSearch + "%'";
                //sqlWhere += sqlwherein;
                var HotProducts = svHotProduct.SelectData<view_HotFeaProduct>(sqlSelect, sqlWhere, sqlOrderBy, 1, 2);
                if (svHotProduct.TotalRow == 0)
                {
                    ViewBag.HotCount = 0;
                }
                else
                {
                    ViewBag.HotProducts = HotProducts;
                    ViewBag.HotCount = svHotProduct.TotalRow;
                }
            }
            else if (CateLevel != null)
            {
                if (CateLevel == 1) //AND (it.ExpiredDate > GETDATE())
                {
                    sqlWhere = "(it.Status = 'H')  AND (ProductID > 0) AND(it.RowFlag >= 3) AND IsShow=1 AND IsJunk=0  AND ProductDelete=0 AND (CategoryID = " + Category + " )";
                    //sqlWhere += sqlwherein;
                    var HotProducts = svHotProduct.SelectData<view_HotFeaProduct>(sqlSelect, sqlWhere, sqlOrderBy, 1, 2);
                    if (svHotProduct.TotalRow == 0)
                    {
                        //sqlWhere = "(it.Status = 'H') AND (it.ExpiredDate > GETDATE()) AND (ProductID > 0) AND(it.RowFlag >= 3)";
                        //var HotProductAll = svHotProduct.SelectData<view_HotFeaProduct>(sqlSelect, sqlWhere, sqlOrderBy, 1, 0);
                        //ViewBag.HotProducts = HotProductAll;
                        //ViewBag.HotCount = HotProductAll.Count;
                        ViewBag.HotCount = 0;
                    }
                    else
                    {
                        ViewBag.HotProducts = HotProducts;
                        ViewBag.HotCount = svHotProduct.TotalRow;
                    }
                }else {
                            //sqlWhere = "(it.Status = 'H') AND (it.ExpiredDate > GETDATE()) AND (ProductID > 0) AND(it.RowFlag >= 3)";
                            //var HotProductAll = svHotProduct.SelectData<view_HotFeaProduct>(sqlSelect, sqlWhere, sqlOrderBy, 1, 0);
                            //ViewBag.HotProducts = HotProductAll;
                            //ViewBag.HotCount = HotProductAll.Count;
                            ViewBag.HotCount = 0;
                        }
            }
            else {
                //sqlWhere = "(it.Status = 'H') AND (it.ExpiredDate > GETDATE()) AND (ProductID > 0) AND(it.RowFlag >= 3)";
                //var HotProductAll = svHotProduct.SelectData<view_HotFeaProduct>(sqlSelect, sqlWhere, sqlOrderBy, 1, 0);
                //ViewBag.HotProducts = HotProductAll;
                //ViewBag.HotCount = HotProductAll.Count;
                ViewBag.HotCount = 0;
            }
        }
        #endregion 

        #region Post Add Tel Count
        [HttpPost]
        public ActionResult PostAddTelCount(int ProductID)
        {
            var svProduct = new ProductService();
            try
            {
                svProduct.UpdateTelCount(ProductID);
                return Json(svProduct);
            }
            catch (Exception ex)
            {
                CreateLogFiles(ex);
                return Json(svProduct);
            }
        }
        #endregion 
    }
}
