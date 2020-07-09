using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ouikum.Product;
using Ouikum.Company;
using Prosoft.Service;
using Ouikum.Category;
using res = Prosoft.Resource.Web.Ouikum;

namespace Ouikum.Web.Admin
{
    public partial class CategoryController : BaseSecurityAdminController
    {

        #region Get: Index
        public ActionResult Index()
        {
            RememberURL();
            if (!CheckIsAdmin(12))
                return Redirect(res.Pageviews.PvMemberSignIn);

            #region Set Default
            GetStatusUser();
            SetPager();
            #endregion

            ListDoLoadData();
            return View();
        }
        #endregion

        #region CategoryType
        #region HttpGet: LoadCateType
        [HttpGet]
        public ActionResult LoadCateType()
        {
            var svCate = new CategoryService();
            var CateTypes = svCate.SelectData<b2bCategoryType>(" CategoryType,CategoryTypeName ", " IsDelete = 0 ", "CategoryType");
            ViewBag.CateTypes = CateTypes;
            return PartialView("UC/CategoryType");
        }
        #endregion 

        #region HttpPost: InsertCateType 
        [HttpPost]
        public ActionResult InsertCateType(string catetypename)
        {
            var svCategory = new CategoryService();
            try
            {
                var model = new b2bCategoryType();

                model.CategoryTypeName = catetypename;
                model.RowFlag = 1;
                model.RowVersion = 1;
                model.IsDelete = false;
                model.IsShow = true;

                svCategory.InsertCategoryType(model);
            }
            catch (Exception ex)
            {
                CreateLogFiles(ex);
            }
            return Json(new { IsResult = svCategory.IsResult, MsgError = res.Admin.lblCate_duplicate});
        }
        #endregion
        #endregion

        #region HttpGet: LoadCateByType
        [HttpGet]
        public ActionResult LoadCateByType(string keyword)
        {
            var svCate = new CategoryService();
            var sqlWhere = " IsDelete = 0 ";

            if (!string.IsNullOrEmpty(keyword))
            {
                sqlWhere += " AND CategoryTypeName Like N'%" + keyword + "%' ";
            }
            var CateTypes = svCate.SelectData<b2bCategoryType>("  CategoryType,CategoryTypeName", sqlWhere, "CategoryType");
            ViewBag.CateTypes = CateTypes;
            return PartialView("UC/CategoryType");
        }
        #endregion 

        #region HttpGet: LoadCateByType1
        [HttpGet]
        public ActionResult LoadCateByType1(string catetype,string keyword)
        {
            var svCate = new CategoryService();
            var sqlWhere = " IsDelete = 0 AND CategoryLevel = 1 "; 

            if (!string.IsNullOrEmpty(catetype))
            {
                sqlWhere += "AND CategoryType = " + catetype + " ";
            }
            if(!string.IsNullOrEmpty(keyword)){
                sqlWhere += " AND CategoryName Like N'%"+keyword+"%' ";
            }
            var CateByTypes = svCate.SelectData<b2bCategory>("  CategoryID,ParentCategoryID,CategoryName,CategoryNameEng,ParentCategoryPath,CategoryType,ProductCount", sqlWhere, "CategoryName");
            ViewBag.CateByTypes = CateByTypes;
            return PartialView("UC/CategoryLevel1");
        }
        #endregion 

        #region HttpGet: LoadCateByType2
        [HttpGet]
        public ActionResult LoadCateByType2(string cateid,string keyword)
        {
            var svCate = new CategoryService();
            var sqlWhere = " IsDelete = 0 AND CategoryLevel = 2 ";

            if (!string.IsNullOrEmpty(cateid))
            {
                sqlWhere += "AND ParentCategoryID = " + cateid + " ";
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                sqlWhere += " AND CategoryName Like N'%" + keyword + "%' ";
            }
            var CateByTypes = svCate.SelectData<b2bCategory>(" CategoryID,ParentCategoryID,CategoryName,CategoryNameEng,ParentCategoryPath,CategoryType,ProductCount ", sqlWhere, "CategoryName");
            ViewBag.CateByTypes = CateByTypes;
            return PartialView("UC/CategoryLevel2");
        }
        #endregion

        #region HttpGet: LoadCateByType3
        [HttpGet]
        public ActionResult LoadCateByType3(string cateid,string keyword)
        {
            var svCate = new CategoryService();
            var sqlWhere = " IsDelete = 0 AND CategoryLevel = 3 ";

            if (!string.IsNullOrEmpty(cateid))
            {
                sqlWhere += "AND ParentCategoryID = " + cateid + " ";
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                sqlWhere += " AND CategoryName Like N'%" + keyword + "%' ";
            }
            var CateByTypes = svCate.SelectData<b2bCategory>("  CategoryID,ParentCategoryID,CategoryName,CategoryNameEng,ParentCategoryPath,CategoryType,ProductCount", sqlWhere, "CategoryName");
            ViewBag.CateByTypes = CateByTypes;
            return PartialView("UC/CategoryLevel3");
        }
        #endregion

        #region HttpGet: DoLoadCategory
        [HttpPost]
        public ActionResult DoLoadCategory(string catelevel, int? parentcateid)
        {
            var svCate = new CategoryService();
            var sqlWhere = " IsDelete = 0 AND CategoryLevel = " + catelevel;

            if(parentcateid > 0)
            {
                sqlWhere += "AND ParentCategoryID = "+parentcateid;
            }

            var cate = svCate.SelectData<b2bCategory>(@" CategoryID,ParentCategoryID,CategoryName,CategoryNameEng,ParentCategoryPath,CategoryType,ProductCount",
                sqlWhere, "CategoryName");
          
             return Json(cate);
        }
        #endregion

        #region HttpPost: InsertCategory
        [HttpPost]
        public ActionResult InsertCategory(int ParentCateID, int CateLV, string CateName, string CateNameEN, int CateType)
        {
            var svCategory = new CategoryService();
            
            try
            {
                var model = new b2bCategory();

                if (ParentCateID != 0)
                {
                    var Category = svCategory.SelectData<b2bCategory>("CategoryID,ParentCategoryPath", "CategoryID = " + ParentCateID).First();
                    model.ParentCategoryPath = Category.ParentCategoryPath + " >> " + CateName;
                    model.CategoryType = CateType;
                }
                else
                {
                    model.ParentCategoryPath = CateName;
                    model.CategoryType = CateType;
                }

                string NumParent = "0000";
                string NumID = "0000";
                if (CateLV != 1)
                {
                    var svCate = new CategoryService();
                    if (CateLV == 2)
                    {
                        var CategoryLV2 = svCate.SelectData<b2bCategory>("CategoryID,ParentCategoryPath", "CategoryID = " + ParentCateID).First();
                        NumParent = String.Format("{0:0000}", CategoryLV2.CategoryID);
                    }
                    if (CateLV == 3)
                    { 
                        var Categories = svCate.SelectData<b2bCategory>(" * ", "CategoryID = " + ParentCateID).First();
                        var CategoryLV3 = svCate.SelectData<b2bCategory>("CategoryID", "CategoryID = " + Categories.ParentCategoryID).First();
                        NumParent = String.Format("{0:0000}", CategoryLV3.CategoryID);
                        NumID = String.Format("{0:0000}", ParentCateID);
                    }
                }

                if (CateNameEN != "")
                {
                    var CateNameENTrim = CateNameEN.Trim();
                    var NameEN = CateNameENTrim.ToUpper().Substring(0, 4);
                    model.CategoryCode = NameEN + "01-" + CateLV + "-" + NumParent + "-" + NumID;
                    model.CategoryNameEng = CateNameEN;
                }
                model.CategoryName = CateName;
                model.RowFlag = 1;
                model.RowVersion = 1;
                model.BuyLeadCount = 0;
                model.ProductCount = 0;
                model.SupplierCount = 0;
                model.CategoryKeyword = CateName;
                model.CategoryLevel = (byte)CateLV;
                model.ParentCategoryID = ParentCateID;
                model.IsDelete = false;
                model.IsShow = false;

                svCategory.InsertCategory(model);
            }
            catch (Exception ex)
            {
                CreateLogFiles(ex);
            }
            return Json(new { IsResult = svCategory.IsResult, MsgError = res.Admin.lblCate_duplicate });
        }
        #endregion

        #region MoveCategory
        public JsonResult MoveCategory(int oldcatelv1, int oldcatelv2, int newcatelv1, int newcatelv2, int newcatelv3)
        {

            var svCate = new CategoryService();
            svCate.MoveCategory(oldcatelv1, oldcatelv2, newcatelv1, newcatelv2, newcatelv3);

            return Json(svCate.IsResult);
        }
        #endregion

        #region MoveProduct
        public JsonResult MoveProduct(int oldcatelv1, int oldcatelv2, int oldcatelv3, int newcatelv1, int newcatelv2, int newcatelv3)
        {

            var svCate = new CategoryService();
            svCate.MoveProduct(oldcatelv1, oldcatelv2, oldcatelv3, newcatelv1, newcatelv2, newcatelv3);

            return Json(svCate.IsResult);
        }
        #endregion

        #region DeleteCategory
        public JsonResult DeleteCategory(int oldcatelv3)
        {
            var svCate = new CategoryService();
            svCate.DeleteCategory(oldcatelv3);

            return Json(svCate.IsResult);
        }
        #endregion

        #region DeleteCategoryManyLevel
        public JsonResult DeleteCategoryManyLevel(int CategoryID,string Type)
        {
            var svCate = new CategoryService();
            var svProduct = new ProductService();
            var product = svCate.SelectData<b2bProduct>("ProductID", "IsDelete = 0 AND " + Type + " = " + CategoryID);

            if (product.Count == 0)
            {
                var cate = svCate.SelectData<b2bCategory>("CategoryID", "IsDelete = 0 AND ParentCategoryID = " + CategoryID);
                if (cate.Count == 0)
                {
                    svCate.DeleteCategoryManyLevel(CategoryID);
                    var CateType = svCate.SelectData<b2bCategory>("CategoryID,CategoryType", "CategoryID = " + CategoryID).First();
                    return Json(new { svCate.IsResult, CateType = CateType.CategoryType });
                }
                else
                {
                    return Json(new { IsResult = svCate.IsResult, MsgError = "ไม่สามารถลบหมวดหมู่ได้ กรุณาลบหมวดหมู่ทั้งหมดที่อยู่ภายใต้หมวดหมู่นี้ก่อน" });
                }
            }
            else
            {
                return Json(new { IsResult = svProduct.IsResult, MsgError = "ไม่สามารถลบหมวดหมู่ได้ กรุณาย้ายสินค้าไปหมวดอื่นก่อนการลบ" });
            }
        }
        #endregion

        #region DeleteCategoryType
        public JsonResult DeleteCategoryType(int CategoryType)
        {
            var svCate = new CategoryService();
            var cate = svCate.SelectData<b2bCategory>("CategoryID", "IsDelete = 0 AND CategoryType = " + CategoryType);

            if (cate.Count == 0)
            {
                svCate.DeleteCategoryType(CategoryType);
                var CateType = svCate.SelectData<b2bCategoryType>("CategoryType,CategoryTypeName", "CategoryType = " + CategoryType).First();
                return Json(new { svCate.IsResult, CateType = CateType.CategoryType });
            }
            else
            {
                return Json(new { IsResult = svCate.IsResult, MsgError = "ไม่สามารถลบประเภทหมวดหมู่ได้ กรุณาลบหมวดหมู่ทั้งหมดที่อยู่ภายใต้ประเภทหมวดหมู่นี้ก่อน" });
            }
        }
        #endregion

        #region UpdateCategoryType
        public JsonResult UpdateCategoryType(int CategoryType, string CategoryTypeName)
        {
            var svCate = new CategoryService();
            svCate.UpdateCategoryType(CategoryType, CategoryTypeName);

            return Json(svCate.IsResult);
        }
        #endregion

        #region UpdateCategoryLevel
        public JsonResult UpdateCategoryLevel(int CategoryID, string CategoryName, string CategoryNameEng, int CategoryType)
        {
            var svCate = new CategoryService();
            svCate.UpdateCategoryLevel(CategoryID, CategoryName, CategoryNameEng, CategoryType);
            var parentCate = svCate.SelectData<b2bCategory>("CategoryID,CategoryName,ParentCategoryID,ParentCategoryPath", "ParentCategoryID = " + CategoryID + " AND IsDelete = 0");
            ViewBag.parentCate = parentCate;
            foreach (var Cate in (List<b2bCategory>)ViewBag.parentCate)
            {
                svCate.UpdateByCondition<b2bCategory>("ParentCategoryPath = N'" + CategoryName + " >> " + Cate.CategoryName + "'", "CategoryID = " + Cate.CategoryID + " AND IsDelete = 0");
                var parentCate1 = svCate.SelectData<b2bCategory>("CategoryID,CategoryName,ParentCategoryID,ParentCategoryPath", "ParentCategoryID = " + Cate.CategoryID + " AND IsDelete = 0");
                ViewBag.parentCate1 = parentCate1;
                foreach (var Cate1 in (List<b2bCategory>)ViewBag.parentCate1)
                {
                    svCate.UpdateByCondition<b2bCategory>("ParentCategoryPath = N'" + CategoryName + " >> " + Cate.CategoryName + " >> " + Cate1.CategoryName + "'", "CategoryID = " + Cate1.CategoryID + " AND IsDelete = 0");
                }
            }
            return Json(new { IsResult = svCate.IsResult, CategoryType = CategoryType }); ;
        }
        #endregion

        #region UpdateCategoryLevelParentID
        public JsonResult UpdateCategoryLevelParentID(int CategoryID, string CategoryName, string CategoryNameEng, int ParentCategoryID)
        {
            var svCate = new CategoryService();
            svCate.UpdateCategoryLevelParentID(CategoryID, CategoryName, CategoryNameEng, ParentCategoryID);
            var parentPath = svCate.SelectData<b2bCategory>("CategoryID,CategoryName,ParentCategoryID,ParentCategoryPath", "CategoryID = " + ParentCategoryID).First();
            var parentCate = svCate.SelectData<b2bCategory>("CategoryID,CategoryName,ParentCategoryID,ParentCategoryPath", "ParentCategoryID = " + CategoryID + " AND IsDelete = 0");
            ViewBag.parentCate = parentCate;
            foreach (var Cate in (List<b2bCategory>)ViewBag.parentCate)
            {
                svCate.UpdateByCondition<b2bCategory>("ParentCategoryPath = N'" + parentPath.ParentCategoryPath + " >> " + CategoryName + " >> " + Cate.CategoryName + "'", "CategoryID = " + Cate.CategoryID + " AND IsDelete = 0");
            }
            return Json(new { IsResult = svCate.IsResult, ParentCategoryID = ParentCategoryID }); ;
        }
        #endregion

    }
}
