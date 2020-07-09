using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

//usin other
//using Prosoft.Base;
using Ouikum.Common;
using Prosoft.Service;
using Ouikum.Category;
using Ouikum.Product;

namespace Ouikum.Controllers
{
    public partial class StoreController : BaseController
    {
        
        #region GetCompare
        [HttpGet]
        public ActionResult Compare(string ProID)
        {
            if (RedirectToProduction())
                return Redirect(UrlProduction);
            CommonService svCommon = new CommonService();
            GetStatusUser();
            if (string.IsNullOrEmpty(ProID))
            {
                return RedirectToAction("Index");
            }
            else
            {

                List<int> ProductID = new List<int>();
                foreach (string ID in ProID.Split(','))
                {
                    if (!string.IsNullOrEmpty(ID))
                    {
                        int intID;
                        bool isNum = int.TryParse(ID, out intID);
                        if (isNum)
                        {
                            ProductID.Add(intID);
                        }
                    }
                }
                string sqlSelect, sqlWhere, StrConcat = "";
                if (ProductID.Count > 0)
                {
                    int i;
                    int Sort = 1;
                    for ( i = 0; i < ProductID.Count; i++)
                    {
                        StrConcat += ProductID[i] + ",";
                    }

                    sqlSelect = "ProductID,ProductName,ProductImgPath,ShortDescription,MinOrderQty,QtyUnit,Price,";
                    sqlSelect += "ProductCount,CompID,CompName,CompLevel,BizTypeOther,BizTypeName,ProvinceName,CompSubDistrict,CompAddrLine1,CompAddrLine2,CompPostalCode,DistrictName";
                    sqlWhere = " RowFlag >= 4 AND (ProductID IN ( " + StrConcat.Substring(0, StrConcat.Length - 1) + "))";
                    sqlWhere += "ORDER BY CASE ProductID ";
                    for (int x = 0; x < ProductID.Count; x++)
                    {
                        sqlWhere += " WHEN " + ProductID[x] + " THEN " + Sort;
                        Sort++;
                    }
                    sqlWhere += " END";
                    ViewBag.EnumServiceType = svCommon.SelectEnum(CommonService.EnumType.SearchByServiceType);
                    var Products = svProduct.SelectData<view_Product>(sqlSelect, sqlWhere, null, 1, 0,false);
                    ViewBag.ProductCompares = Products;
                    ViewBag.CountProduct = i+1;
                  
                    }
                
                }

            ViewBag.ProductID = ProID;
            return View();
            }
        #endregion

    }
}
