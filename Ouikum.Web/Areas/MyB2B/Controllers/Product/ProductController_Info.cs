using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Prosoft.Service;
//using Prosoft.Base;
using Ouikum.Product;
using Ouikum.Category;
using Ouikum;
using Ouikum.Common;
using res = Prosoft.Resource.Web.Ouikum;
using Ouikum.Web.Models;

namespace Ouikum.Web.MyB2B
{
    public partial class ProductController : BaseSecurityController
    {
        #region PrepareAddProduct
        [HttpPost]
        public ActionResult PrepareAddProduct(string GenCode)
        {
            if (LogonServiceType != 2 && LogonServiceType != 3)
                return Redirect(res.Pageviews.PvAccessDenied);
            CommonService svCommon = new CommonService();
            var svProductGroup = new ProductGroupService();
            var ProductGroups = svProductGroup.GetProductGroup(LogonCompID);
            if (ProductGroups.Count() > 0)
            {
                ViewBag.ProductGroups = ProductGroups;
            }
            else
            {
                ViewBag.ProductGroups = null;
            }

            #region FindCateAll
            var svCategory = new CategoryService();
            var data = svCategory.GetCategoryByLevel(1);
            ViewBag.SelectCateLV1 = data;
            #endregion

            ViewBag.QtyUnits = svCommon.SelectEnum(CommonService.EnumType.QtyUnits);
            ViewBag.ProductCode = GenCode;
            return PartialView("UC/AddProduct");
        }
        #endregion

        #region PrepareAddProductNotLogin
        [HttpPost]
        public ActionResult PrepareAddProductNotLogin()
        {
            var isLogin = CheckIsLogin();
            if (!CheckIsLogin())
            {
                CommonService svCommon = new CommonService();
                var svProductGroup = new ProductGroupService();

                ViewBag.ProductGroups = null;
                ViewBag.NewTemp = GetRandomCode(5);
                #region FindCateAll
                var svCategory = new CategoryService();
                var data = svCategory.GetCategoryByLevel(1);

                ViewBag.SelectCateLV1 = data;
                #endregion
                ViewBag.ProductCode = "NPD-" + DateTime.Now.ToString("yyyyMMdd") + "-" + GetRandomCode(5);
                ViewBag.QtyUnits = svCommon.SelectEnum(CommonService.EnumType.QtyUnits);
                LoadProvinces();
                LoadBiztype();
                LoadCategory();
                var view = PartialViewToString("UC/AddProductIsNotLogin");
                //    return PartialView("UC/AddProductIsNotLogin");
                return Json(new { view = view, isLogin = isLogin }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { view = "", isLogin = isLogin }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region PostAddProduct
        [HttpPost, ValidateInput(false)]
        public ActionResult AddProduct
        (
        #region Param
            string ProductName,
            int? ProductGroup,
            string ProductCode,
            string Price,
            string Price_One,
            string Keyword,
            string QuickDetail,
            string FullDetail,
            string Qty,
            string QtyUnit,
            string Catecode,
            string CateLV3,
            List<string> ProductImgPath,
            int Ispromotion,
            string PromotionPrice
        #endregion
)
        {
            var svProduct = new ProductService();
            var svProductImage = new ProductImageService();

            b2bProduct model = new b2bProduct();
            try
            {
                #region Set Product Model
                model.ProductName = ProductName;
                model.ProductGroupID = (ProductGroup > 0) ? ProductGroup : 0;
                model.ProductCode = ProductCode;
                model.Price = DataManager.ConvertToDecimal(Price);
                model.Price_One = DataManager.ConvertToDecimal(Price_One); 
                model.ProductKeyword = Keyword;
                model.ShortDescription = QuickDetail;
                model.ProductDetail = FullDetail;
                model.Qty = DataManager.ConvertToDecimal(Qty);
                //model.QtyUnit = QtyUnit;
                model.ProductImgPath = ProductImgPath[0];
                model.IsPromotion = DataManager.ConvertToBool(Ispromotion);
                model.PromotionPrice = DataManager.ConvertToDecimal(PromotionPrice);
                model.CompID = LogonCompID;
                model.CateLV1 = Convert.ToInt32(Catecode.Substring(9, 4));
                model.CateLV2 = Convert.ToInt32(Catecode.Substring(14, 4));
                model.CateLV3 = Convert.ToInt32(CateLV3);
                model.IsShow = true;
                model.IsJunk = false;
                model.Complevel = DataManager.ConvertToShort(LogonCompLevel);
                if (QtyUnit == "0")
                {
                    model.QtyUnit = "รายการ";
                }
                else
                {
                    model.QtyUnit = QtyUnit;
                }
                #region Check ว่าเป็น OutSource Add หรือ User Add
                if (LogonServiceType >= 9)
                {
                    model.RowFlag = (short)2;
                    model.CompCode = LogonCompCode;
                }
                else
                    model.RowFlag = (short)6;
                #endregion

                model.ContactCount = 0;
                model.ListNo = 0;
                model.ViewCount = 0;
                model.QuotationCount = 0;
                model.TelCount = 0;
                model.CreatedDate = DateTime.Now;
                model.ModifiedDate = DateTime.Now;
                model.CreatedBy = LogonCompCode;
                model.ModifiedBy = LogonCompCode;
                #endregion

                #region Set ProductImage Model
                int i = 1;
                var ProductImgs = new List<b2bProductImage>();
                foreach (var imgname in ProductImgPath)
                {
                    var modelImg = new b2bProductImage();
                    modelImg.ImgDetail = imgname.Substring(0, 15);
                    modelImg.ListNo = i;
                    modelImg.ImgPath = imgname;
                    modelImg.RowVersion = 1;
                    modelImg.RowFlag = 1;
                    modelImg.CreatedDate = DateTime.Now;
                    modelImg.ModifiedDate = DateTime.Now;
                    modelImg.CreatedBy = "sa";
                    modelImg.ModifiedBy = "sa";
                    ProductImgs.Add(modelImg);
                    i++;
                }
                #endregion

                #region Insert Product And ProductImage
                svProduct.InsertProduct(model, ProductImgs, LogonCompID, LogonCompLevel);
                svProduct.UpdateProductCountInCategories(Convert.ToInt32(Catecode.Substring(9, 4)), Convert.ToInt32(Catecode.Substring(14, 4)), Convert.ToInt32(CateLV3));
                svProduct.UpdateProductCountInCompany(LogonCompID);
                #endregion

                #region Save Image Files
                imgManager = new FileHelper();
                imgManager.DirPath = "Product/" + LogonCompID + "/" + model.ProductID;


                if (Request.Cookies["AddProduct"] != null)
                {
                    imgManager.DirTempPath = Request.Cookies["AddProduct"]["UnRegis"];
                    HttpCookie myCookie = new HttpCookie("AddProduct");
                    myCookie.Expires = DateTime.Now.AddDays(-1d);
                    Response.Cookies.Add(myCookie);
                }
                else
                {
                    imgManager.DirTempPath = "Temp/Product/" + LogonCompID;
                }


                SaveFileImage(
                    imgManager.DirTempPath,
                    imgManager.DirPath,
                    ProductImgPath, 150, 450);
                #endregion

                //var mgKeyword = new KeywordMongo();
                //var isresult = mgKeyword.UpdateMongoProduct(model.ProductID);
            }
            catch (Exception ex)
            {
                CreateLogFiles(ex);
            }


            return Json(new { IsResult = svProduct.IsResult, MsgError = GenerateMsgError(svProduct.MsgError), ID = model.ProductID });
        }
        #endregion

        #region ValidateAddProduct
        public ActionResult ValidateAddProduct(string ProductName, string ProductCode)
        {
            var svProduct = new ProductService();
            svProduct.ValidateInsert(ProductName, ProductCode);
            return Json(svProduct.IsResult);
        }

        #endregion

        #region ValidateEditProduct
        public ActionResult ValidateEditProduct(string Upd_ProductName, string Chk_ProductName, string Upd_ProductCode, string Chk_ProductCode, int ProductID)
        {
            var svProduct = new ProductService();
            svProduct.ValidateUpdate(Upd_ProductName, Chk_ProductName, Upd_ProductCode, Chk_ProductCode, ProductID);
            return Json(svProduct.IsResult);
        }

        #endregion

        #region ValidateFullProduct
        public ActionResult ValidateFullProduct()
        {
            var svProduct = new ProductService();
            svProduct.ValidateFullProduct(LogonCompID, LogonCompLevel);
            return Json(new { IsResult = svProduct.IsResult, CompLevel = LogonCompLevel });
        }
        #endregion

        #region PrepareEditByID
        [HttpPost]
        public ActionResult PrepareEditByID(int ID, string GenCode)
        {
            var svProduct = new ProductService();
            var svCategory = new CategoryService();
            var svProductGroup = new ProductGroupService();
            var svProductImage = new ProductImageService();
            CommonService svCommon = new CommonService();
            #region FindCateAll

            //var sqlwherein = "";
            //switch (res.Common.lblWebsite)
            //{
            //    case "B2BThai": sqlwherein = " AND CategoryType IN (1,2)"; break;
            //    case "AntCart": sqlwherein = " AND CategoryType IN (3)"; break;
            //    case "myOtopThai": sqlwherein = " AND CategoryType IN (5)"; break;
            //    case "AppstoreThai": sqlwherein = " AND CategoryType IN (6)"; break;
            //    default: sqlwherein = ""; break;
            //}

            var sqlSelect = "CategoryID,CategoryName";
            var sqlWhere = "CategoryLevel=1 AND RowFlag > 0 AND IsDelete=0";
            var data1 = svCategory.GetCategoryByLevel(1);
            //var data2 = svCategory.GetCategoryByLevel(2);
            //var data3 = svCategory.GetCategoryByLevel(3);
            ViewBag.SelectCateLV1 = data1;
            //ViewBag.SelectCateLV2 = data2;
            //ViewBag.SelectCateLV3 = data3;
            #endregion

            ViewBag.QtyUnits = svCommon.SelectEnum(CommonService.EnumType.QtyUnits);

            #region Set Product Group
            var ProductGroups = svProductGroup.GetProductGroup(LogonCompID);
            if (ProductGroups.Count() > 0)
                ViewBag.ProductGroups = ProductGroups;
            else
                ViewBag.ProductGroups = null;
            #endregion
            var Product = svProduct.SelectData<b2bProduct>(" * ", "ProductID=" + ID + " And IsDelete=0", null, 1, 1).First();
            ViewBag.Product = Product;

            sqlSelect = "ProductID,ProductImageID,ImgPath,ListNo";
            sqlWhere = svProductImage.CreateWhereCause(ID);

            var Images = svProductImage.SelectData<b2bProductImage>(sqlSelect, sqlWhere, " ListNo ASC");
            var countImg = 0;

            if (svProductImage.TotalRow > 0)
                countImg = Images.Where(m => m.ImgPath == Product.ProductImgPath).Count();

            if (countImg == 0)
            {
                var img = new b2bProductImage();
                img.ProductID = Product.ProductID;
                img.ListNo = 1;
                img.ImgPath = Product.ProductImgPath;
                img.ImgDetail = Product.ProductName;
                svProductImage.InsertProductImage(img);

                Images = svProductImage.SelectData<b2bProductImage>(sqlSelect, sqlWhere, " ListNo DESC");

            }

            ViewBag.ProductImg = Images;
            ViewBag.ImgCount = Images.Count();

            var data2 = new List<b2bCategory>();
            var data3 = new List<b2bCategory>();
            data2 = svCategory.LoadSubCategory(DataManager.ConvertToInteger(Product.CateLV1), 500, 2);
            data3 = svCategory.LoadSubCategory(DataManager.ConvertToInteger(Product.CateLV2), 500, 3); 
            ViewBag.SelectCateLV2 = data2;
            ViewBag.SelectCateLV3 = data3;

            var a = svCategory.SearchCategoryByID((int)Product.CateLV3);
            //var a = svCategory.SelectData<b2bCategory>("*", "CategoryID = " + (int)Product.CateLV3, null, 1, 1,);
            ViewBag.Catename = a;
            ViewBag.ProductCode = GenCode;
            return PartialView("UC/EditProduct");
        }
        #endregion

        #region PostEditProductByID
        [HttpPost, ValidateInput(false)]
        public ActionResult EditProductByID
        (
        #region Param
            int ProductID,
            int Rowflag,
            string ProductName,
            int? ProductGroup,
            string ProductCode,
            string Price,
            string Price_One,
            string Keyword,
            string QuickDetail,
            string FullDetail,
            string Qty,
            string QtyUnit,
            string Catecode,
            string CateLV3,
            List<string> ProductImgPath,
            List<int> ProductImgID,
            int Ispromotion,
            string PromotionPrice
        #endregion
)
        {

            var svProduct = new ProductService();
            b2bProduct model = new b2bProduct();

            if (LogonCompID > 0)
            {
                try
                {
                    #region Set Model
                    model.ProductID = ProductID;
                    model.ProductName = ProductName;
                    model.ProductImgPath = ProductImgPath[0];
                    model.ProductGroupID = ProductGroup;
                    model.ProductCode = ProductCode;
                    model.Price = decimal.Parse(Price);
                    model.Price_One = decimal.Parse(Price_One);
                    model.ProductKeyword = Keyword;
                    model.ShortDescription = QuickDetail;
                    model.ProductDetail = FullDetail;
                    model.Qty = decimal.Parse(Qty);
                    model.QtyUnit = QtyUnit;
                    model.IsPromotion = DataManager.ConvertToBool(Ispromotion);
                    model.PromotionPrice = DataManager.ConvertToDecimal(PromotionPrice);
                    model.ModifiedBy = LogonCompCode;

                    if (Catecode == null)
                    {
                        svProduct.UpdateProductCountInCategories(0, 0, Convert.ToInt32(CateLV3));
                    }
                    else if (Catecode == "")
                    {
                        svProduct.UpdateProductCountInCategories(0, 0, Convert.ToInt32(CateLV3));
                    }
                    else
                    {
                        model.CateLV1 = Convert.ToInt32(Catecode.Substring(9, 4));
                        model.CateLV2 = Convert.ToInt32(Catecode.Substring(14, 4));
                        svProduct.UpdateProductCountInCategories(Convert.ToInt32(Catecode.Substring(9, 4)), Convert.ToInt32(Catecode.Substring(14, 4)), Convert.ToInt32(CateLV3));
                    }
                    if (Convert.ToInt32(CateLV3) != 0) 
                    {
                        model.CateLV3 = Convert.ToInt32(CateLV3);
                    }

                    #region OurSource Update or Member Update
                    if (LogonServiceType >= 9)
                    {
                        model.RowFlag = 2;
                        model.CompCode = LogonCompCode;
                    }
                    else
                    {
                        // เช็ค ถ้า สินค้าไม่อนุมัติมาก่อนให้เปลี่ยนเป็น รออนุมัติ
                        if (Rowflag == 3)
                        {
                            model.RowFlag = 2;
                        }
                        else
                        {
                            model.RowFlag = 6;
                        }
                    }
                    #endregion

                    #endregion

                    #region Update Data
                    svProduct.UpdateProduct(model, ProductImgID, ProductImgPath);
                    svProduct.UpdateProductCountInCompany(LogonCompID);
                    #endregion


                    //var mgkeyword = new KeywordMongo();
                    //var isresult = mgkeyword.UpdateMongoProduct(model.ProductID);

                    #region Check And Update Files
                    if (svProduct.IsResult)
                    {
                        imgManager = new FileHelper();
                        imgManager.DirPath = "Product/" + LogonCompID + "/" + ProductID;
                        imgManager.DirTempPath = "Temp/Product/" + LogonCompID;
                        SaveFileImage(imgManager.DirTempPath, imgManager.DirPath, ProductImgPath, 150, 450);

                        imgManager.DirPath = "Product/" + LogonCompID + "/" + ProductID;
                        DeleteFileImage(imgManager.DirPath, svProduct.OldFiles, ProductImgPath);

                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    svProduct.MsgError.Add(ex);
                    CreateLogFiles(ex);
                }
            }

            return Json(new { IsResult = svProduct.IsResult, MsgError = GenerateMsgError(svProduct.MsgError), ID = model.ProductID });
        }

        #endregion
    }
}
