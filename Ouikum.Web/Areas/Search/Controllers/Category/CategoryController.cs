using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

//usin other
using Ouikum;
//using Prosoft.Base;
using Ouikum.Product;
using Ouikum.Category;
using Ouikum.Company;
using Ouikum.Common;
using res = Prosoft.Resource.Web.Ouikum;
namespace Ouikum.Web.Search
{
    public partial class CategoriesController : BaseController
    {
        #region Index
        public ActionResult Index(string Prefix, int cateLevel = 1)
        {
            if (RedirectToProduction())
                return Redirect(UrlProduction);

            var svCategory = new CategoryService();
            CommonService svCommon = new CommonService();
            ViewBag.EnumServiceType = svCommon.SelectEnum(CommonService.EnumType.SearchByServiceType);

            #region set text resource
            var text = res.Product.lblvowel1;
            var vowel1 = text.Split(',');
            text = res.Product.lblvowel2;
            var vowel2 = text.Split(',');
            text = res.Product.lblCategories1;
            var Cate1 = text.Split(',');
            text = res.Product.lblCategories2;
            var Cate2 = text.Split(',');
            text = res.Product.lblCategories3;
            var Cate3 = text.Split(',');
            text = res.Product.lblCategories4;
            var Cate4 = text.Split(',');
            text = res.Product.lblCategories5;
            var Cate5 = text.Split(',');
            text = res.Product.lblCategories6;
            var Cate6 = text.Split(',');
            text = res.Product.lblCategories7;
            var Cate7 = text.Split(',');
            text = res.Product.lblCharsearch;
            var Cate_All = text.Split(',');

            #endregion

            LoadFeatureProduct();
            LoadRandomCompany();
            GetStatusUser();

            if (Prefix != null && Prefix != "")
            {
                #region SearchIndex
                #region sqlWhere
                string sqlWhere = "(CategoryName LIKE N'" + Prefix + "%' ";
                 foreach(var it in vowel1)
                 {
                     if (!string.IsNullOrEmpty(it))
                     {
                         sqlWhere += "OR CategoryName LIKE N'"+ it + Prefix + "%' ";
                     }
                 }
                 foreach (var it in vowel2)
                 {
                     if (!string.IsNullOrEmpty(it))
                     {
                         sqlWhere += " OR CategoryName LIKE N'" + Prefix + it + "%'";
                     }
                 }
                sqlWhere += " ) ";
                sqlWhere += " AND ProductCount > 0 ";
                sqlWhere += " AND CategoryLevel = " + cateLevel;
                sqlWhere += " AND IsDelete = 0";
                #endregion

                #region SelectData
                var Categories = svCategory.SelectData<b2bCategory>("CategoryID,CategoryLevel,CategoryName,CategoryNameEng,ProductCount", sqlWhere, "CategoryName");
                #endregion

                #region ViewBag
                ViewBag.Categories = Categories;
                ViewBag.TotalRow = svCategory.TotalRow;
                #endregion
                #endregion
            }
            else
            {
                #region SearchAll
                #region sqlWhere
                #region sqlWhere1
                string sqlWhere1 = "(CategoryName LIKE N'[" + res.Product.lblCategories1 + "]%' ";
                foreach (var it in vowel1)
                {
                    if (!string.IsNullOrEmpty(it))
                    {
                        sqlWhere1 += "OR CategoryName LIKE N'" + it + "[" + res.Product.lblCategories1 + "]%' ";
                    }
                }
                foreach (var it in vowel2)
                {
                    if (!string.IsNullOrEmpty(it))
                    {
                        int i = 0;
                        sqlWhere1 += " OR CategoryName LIKE N'[";
                        foreach (var cate in Cate1)
                        {
                            if (!string.IsNullOrEmpty(cate))
                            {
                                if(i > 0){
                                    sqlWhere1 += "," + cate + it;
                                }
                                else{
                                    sqlWhere1 += cate + it;
                                }
                            }
                            i++;
                        }
                        sqlWhere1 +="]%'";
                    }
                }
                sqlWhere1 += " ) ";
                sqlWhere1 += " AND ProductCount > 0 ";
                sqlWhere1 += " AND CategoryLevel = " + cateLevel;
                sqlWhere1 += " AND IsDelete = 0";
                #endregion
                #region sqlWhere2
                string sqlWhere2 = "(CategoryName LIKE N'[" + res.Product.lblCategories2 + "]%' ";
                foreach (var it in vowel1)
                {
                    if (!string.IsNullOrEmpty(it))
                    {
                        sqlWhere2 += "OR CategoryName LIKE N'" + it + "[" + res.Product.lblCategories2 + "]%' ";
                    }
                }
                foreach (var it in vowel2)
                {
                    if (!string.IsNullOrEmpty(it))
                    {
                        int i = 0;
                        sqlWhere2 += " OR CategoryName LIKE N'[";
                        foreach (var cate in Cate2)
                        {
                            if (!string.IsNullOrEmpty(cate))
                            {
                                if (i > 0)
                                {
                                    sqlWhere2 += "," + cate + it;
                                }
                                else
                                {
                                    sqlWhere2 += cate + it;
                                }
                            }
                            i++;
                        }
                        sqlWhere2 += "]%'";
                    }
                }
                sqlWhere2 += " ) ";
                sqlWhere2 += " AND ProductCount > 0 ";
                sqlWhere2 += " AND CategoryLevel = " + cateLevel;
                sqlWhere2 += " AND IsDelete = 0";
                #endregion
                #region sqlWhere3
                string sqlWhere3 = "(CategoryName LIKE N'[" + res.Product.lblCategories3 + "]%' ";
                foreach (var it in vowel1)
                {
                    if (!string.IsNullOrEmpty(it))
                    {
                        sqlWhere3 += "OR CategoryName LIKE N'" + it + "[" + res.Product.lblCategories3 + "]%' ";
                    }
                }
                foreach (var it in vowel2)
                {
                    if (!string.IsNullOrEmpty(it))
                    {
                        int i = 0;
                        sqlWhere3 += " OR CategoryName LIKE N'[";
                        foreach (var cate in Cate3)
                        {
                            if (!string.IsNullOrEmpty(cate))
                            {
                                if (i > 0)
                                {
                                    sqlWhere3 += "," + cate + it;
                                }
                                else
                                {
                                    sqlWhere3 += cate + it;
                                }
                            }
                            i++;
                        }
                        sqlWhere3 += "]%'";
                    }
                }
                sqlWhere3 += " ) ";
                sqlWhere3 += " AND ProductCount > 0 ";
                sqlWhere3 += " AND CategoryLevel = " + cateLevel;
                sqlWhere3 += " AND IsDelete = 0";
                #endregion
                #region sqlWhere4
                string sqlWhere4 = "(CategoryName LIKE N'[" + res.Product.lblCategories4 + "]%' ";
                foreach (var it in vowel1)
                {
                    if (!string.IsNullOrEmpty(it))
                    {
                        sqlWhere4 += "OR CategoryName LIKE N'" + it + "[" + res.Product.lblCategories4 + "]%' ";
                    }
                }
                foreach (var it in vowel2)
                {
                    if (!string.IsNullOrEmpty(it))
                    {
                        int i = 0;
                        sqlWhere4 += " OR CategoryName LIKE N'[";
                        foreach (var cate in Cate4)
                        {
                            if (!string.IsNullOrEmpty(cate))
                            {
                                if (i > 0)
                                {
                                    sqlWhere4 += "," + cate + it;
                                }
                                else
                                {
                                    sqlWhere4 += cate + it;
                                }
                            }
                            i++;
                        }
                        sqlWhere4 += "]%'";
                    }
                }
                sqlWhere4 += " ) ";
                sqlWhere4 += " AND ProductCount > 0 ";
                sqlWhere4 += " AND CategoryLevel = " + cateLevel;
                sqlWhere4 += " AND IsDelete = 0";
                #endregion
                #region sqlWhere5
                string sqlWhere5 = "(CategoryName LIKE N'[" + res.Product.lblCategories5 + "]%' ";
                foreach (var it in vowel1)
                {
                    if (!string.IsNullOrEmpty(it))
                    {
                        sqlWhere5 += "OR CategoryName LIKE N'" + it + "[" + res.Product.lblCategories5 + "]%' ";
                    }
                }
                foreach (var it in vowel2)
                {
                    if (!string.IsNullOrEmpty(it))
                    {
                        int i = 0;
                        sqlWhere5 += " OR CategoryName LIKE N'[";
                        foreach (var cate in Cate5)
                        {
                            if (!string.IsNullOrEmpty(cate))
                            {
                                if (i > 0)
                                {
                                    sqlWhere5 += "," + cate + it;
                                }
                                else
                                {
                                    sqlWhere5 += cate + it;
                                }
                            }
                            i++;
                        }
                        sqlWhere5 += "]%'";
                    }
                }
                sqlWhere5 += " ) ";
                sqlWhere5 += " AND ProductCount > 0 ";
                sqlWhere5 += " AND CategoryLevel = " + cateLevel;
                sqlWhere5 += " AND IsDelete = 0";
                #endregion
                #region sqlWhere6
                string sqlWhere6 = "(CategoryName LIKE N'[" + res.Product.lblCategories6 + "]%' ";
                foreach (var it in vowel1)
                {
                    if (!string.IsNullOrEmpty(it))
                    {
                        sqlWhere6 += "OR CategoryName LIKE N'" + it + "[" + res.Product.lblCategories6 + "]%' ";
                    }
                }
                foreach (var it in vowel2)
                {
                    if (!string.IsNullOrEmpty(it))
                    {
                        int i = 0;
                        sqlWhere6 += " OR CategoryName LIKE N'[";
                        foreach (var cate in Cate6)
                        {
                            if (!string.IsNullOrEmpty(cate))
                            {
                                if (i > 0)
                                {
                                    sqlWhere6 += "," + cate + it;
                                }
                                else
                                {
                                    sqlWhere6 += cate + it;
                                }
                            }
                            i++;
                        }
                        sqlWhere6 += "]%'";
                    }
                }
                sqlWhere6 += " ) ";
                sqlWhere6 += " AND ProductCount > 0 ";
                sqlWhere6 += " AND CategoryLevel = " + cateLevel;
                sqlWhere6 += " AND IsDelete = 0";
                #endregion
                #region sqlWhere7
                string sqlWhere7 = "(CategoryName LIKE N'[" + res.Product.lblCategories7 + "]%' ";
                foreach (var it in vowel1)
                {
                    if (!string.IsNullOrEmpty(it))
                    {
                        sqlWhere7 += "OR CategoryName LIKE N'" + it + "[" + res.Product.lblCategories7 + "]%' ";
                    }
                }
                foreach (var it in vowel2)
                {
                    if (!string.IsNullOrEmpty(it))
                    {
                        int i = 0;
                        sqlWhere7 += " OR CategoryName LIKE N'[";
                        foreach (var cate in Cate7)
                        {
                            if (!string.IsNullOrEmpty(cate))
                            {
                                if (i > 0)
                                {
                                    sqlWhere7 += "," + cate + it;
                                }
                                else
                                {
                                    sqlWhere7 += cate + it;
                                }
                            }
                            i++;
                        }
                        sqlWhere7 += "]%'";
                    }
                }
                sqlWhere7 += " ) ";
                sqlWhere7 += " AND ProductCount > 0 ";
                sqlWhere7 += " AND CategoryLevel = " + cateLevel;
                sqlWhere7 += " AND IsDelete = 0";
                #endregion
                #region sqlWhere8
                string sqlWhere8 = "(CategoryName LIKE N'[" + res.Product.lblCharsearch + "]%' ";
                foreach (var it in vowel1)
                {
                    if (!string.IsNullOrEmpty(it))
                    {
                        sqlWhere8 += "OR CategoryName LIKE N'" + it + "[" + res.Product.lblCharsearch + "]%' ";
                    }
                }
                foreach (var it in vowel2)
                {
                    if (!string.IsNullOrEmpty(it))
                    {
                        int i = 0;
                        sqlWhere8 += " OR CategoryName LIKE N'[";
                        foreach (var cate in Cate_All)
                        {
                            if (!string.IsNullOrEmpty(cate))
                            {
                                if (i > 0)
                                {
                                    sqlWhere8 += "," + cate + it;
                                }
                                else
                                {
                                    sqlWhere8 += cate + it;
                                }
                            }
                            i++;
                        }
                        sqlWhere8 += "]%'";
                    }
                }
                sqlWhere8 += " ) ";
                sqlWhere8 += " AND ProductCount > 0 ";
                sqlWhere8 += " AND CategoryLevel = " + cateLevel;
                sqlWhere8 += " AND IsDelete = 0";
                #endregion
                #endregion

                #region SelectData
                var Categories1 = svCategory.SelectData<b2bCategory>("CategoryID,CategoryLevel,CategoryName,CategoryNameEng,ProductCount", sqlWhere1, "CategoryName");
                var Categories2 = svCategory.SelectData<b2bCategory>("CategoryID,CategoryLevel,CategoryName,CategoryNameEng,ProductCount", sqlWhere2, "CategoryName");
                var Categories3 = svCategory.SelectData<b2bCategory>("CategoryID,CategoryLevel,CategoryName,CategoryNameEng,ProductCount", sqlWhere3, "CategoryName");
                var Categories4 = svCategory.SelectData<b2bCategory>("CategoryID,CategoryLevel,CategoryName,CategoryNameEng,ProductCount", sqlWhere4, "CategoryName");
                var Categories5 = svCategory.SelectData<b2bCategory>("CategoryID,CategoryLevel,CategoryName,CategoryNameEng,ProductCount", sqlWhere5, "CategoryName");
                var Categories6 = svCategory.SelectData<b2bCategory>("CategoryID,CategoryLevel,CategoryName,CategoryNameEng,ProductCount", sqlWhere6, "CategoryName");
                var Categories7 = svCategory.SelectData<b2bCategory>("CategoryID,CategoryLevel,CategoryName,CategoryNameEng,ProductCount", sqlWhere7, "CategoryName");
                var Categories8 = svCategory.SelectData<b2bCategory>("CategoryID,CategoryLevel,CategoryName,CategoryNameEng,ProductCount", sqlWhere8, "CategoryName");
                #endregion

                #region ViewBag
                ViewBag.Categories1 = Categories1;
                ViewBag.Categories2 = Categories2;
                ViewBag.Categories3 = Categories3;
                ViewBag.Categories4 = Categories4;
                ViewBag.Categories5 = Categories5;
                ViewBag.Categories6 = Categories6;
                ViewBag.Categories7 = Categories7;
                ViewBag.Categories8 = Categories8;
                #endregion
                #endregion
            }
            ViewBag.Prefix = Prefix;
            return View();
        }
        #endregion

        #region LoadFeatureProduct
        public void LoadFeatureProduct()
        {
            #region Load Feature
            var svHotFeat = new HotFeaProductService();
            var FeatProducts = svHotFeat.SelectData<view_HotFeaProduct>(" ProductID,ProductName,CompID,ProductImgPath ", "Rowflag = 3 AND Status = 'F' AND ProductID > 0", "   NEWID() ", 1, 10);
            ViewBag.FeatProducts = FeatProducts;
            #endregion
        }
        #endregion

        #region Load Random Company
        public void LoadRandomCompany()
        {
            var svCompany = new CompanyService();
            var sqlWhere = svCompany.CreateWhereAction(CompStatus.HaveProduct) + "AND LogoImgPath != '' AND CompLevel = 3";
            var Company = svCompany.SelectData<view_Company>(
                @" CompID,CompName,LogoImgPath,CompLevel,CompPhone,ProductCount,
                ContactFirstName,ContactLastName,BizTypeID,BizTypeName,BizTypeOther,ProvinceName",
                sqlWhere,
                "  NEWID()", 1, 1).First();
            ViewBag.Company = Company;
        }
        #endregion
    }
}
