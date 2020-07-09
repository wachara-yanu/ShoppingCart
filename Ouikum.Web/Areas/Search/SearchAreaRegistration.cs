using System.Web.Mvc;

namespace Ouikum.Web.Search
{
    public class SearchAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Search";
            }
        }
        public override void RegisterArea(AreaRegistrationContext context)

        {
            //---------------Product-----------------------------

            #region Search_Product_List

            context.MapRoute(
               "Search_Product_List_Lang",
               "{lang}/Search/Product/List", // URL with parameters
               new { controller = "Product", action = "Index" } // Parameter defaults 
               , new { lang = @"[a-zA-Z]{2}" }
           );
            context.MapRoute(
                "Search_Product_List",
                "Search/Product/List", // URL with parameters
                new { controller = "Product", action = "Index" } // Parameter defaults 
            );
            #endregion

            #region Home_Search_Category
            context.MapRoute(
             "Home_Search_Category_Lang",
             "{lang}/Home/Search/Product/List/Category/{Category}/{CateLevel}/{CategoryName}", // URL with parameters 
             new { controller = "Product", action = "Index", Category = UrlParameter.Optional, CateLevel = UrlParameter.Optional, CategoryName = UrlParameter.Optional } // Parameter defaults
         , new { lang = @"[a-zA-Z]{2}" }
             );
            context.MapRoute(
                "Home_Search_Category",
                "Home/Search/Product/List/Category/{Category}/{CateLevel}/{CategoryName}", // URL with parameters 
                new { controller = "Product", action = "Index", Category = UrlParameter.Optional, CateLevel = UrlParameter.Optional, CategoryName = UrlParameter.Optional } // Parameter defaults
            );

            #endregion



            #region Search_Category
            context.MapRoute(
             "Search_Category_Lang",
             "{lang}/Search/Product/List/Category/{Category}/{CateLevel}/{CategoryName}", // URL with parameters 
             new { controller = "Product", action = "Index", Category = UrlParameter.Optional, CateLevel = UrlParameter.Optional, CategoryName = UrlParameter.Optional } // Parameter defaults
         , new { lang = @"[a-zA-Z]{2}" }
             );
            context.MapRoute(
                "Search_Category",
                "Search/Product/List/Category/{Category}/{CateLevel}/{CategoryName}", // URL with parameters 
                new { controller = "Product", action = "Index", Category = UrlParameter.Optional, CateLevel = UrlParameter.Optional, CategoryName = UrlParameter.Optional } // Parameter defaults
            );

            #endregion

            #region Search_Product_PostList

            context.MapRoute(
                 "Search_Product_PostList_Lang",
                 "{lang}/Search/Product/PostList", // URL with parameters
                 new { controller = "Product", action = "List" } // Parameter defaults 
                 , new { lang = @"[a-zA-Z]{2}" }
             );

            context.MapRoute(
                "Search_Product_PostList",
                "Search/Product/PostList", // URL with parameters
                new { controller = "Product", action = "List" } // Parameter defaults
            );



            #endregion

            #region Search_Category_Menu

            context.MapRoute(
               "Search_Category_Menu_Lang",
               "{lang}/Search/Product/Menu", // URL with parameters
               new { controller = "Product", action = "ChildCateMenu" } // Parameter defaults
               , new { lang = @"[a-zA-Z]{2}" }
            );
            context.MapRoute(
                "Search_Category_Menu",
                "Search/Product/Menu", // URL with parameters
                new { controller = "Product", action = "ChildCateMenu" } // Parameter defaults
            );
            #endregion

            #region Search_ProductNameAuto


            context.MapRoute(
                   "Search_ProductNameAuto_Lang",
                   "{lang}/Search/Product/GetProductName", // URL with parameters
                   new { controller = "Product", action = "GetProductName" } // Parameter defaults
                   , new { lang = @"[a-zA-Z]{2}" }
               );
            context.MapRoute(
                "Search_ProductNameAuto",
                "Search/Product/GetProductName", // URL with parameters
                new { controller = "Product", action = "GetProductName" } // Parameter defaults
            );

            #endregion

            #region Search_Product_ProductName


            context.MapRoute(
                "Search_Product_ProductName_Lang",
                "{lang}/Search/Product/List/{TextSearch}", // URL with parameters
                new { controller = "Product", action = "Index", ProductName = UrlParameter.Optional } // Parameter defaults
                , new { lang = @"[a-zA-Z]{2}" }
            );

            context.MapRoute(
               "Search_Product_ProductName",
               "Search/Product/List/{TextSearch}", // URL with parameters
               new { controller = "Product", action = "Index", ProductName = UrlParameter.Optional } // Parameter defaults
           );


            #endregion


            #region Website_ProductDetail

            context.MapRoute(
             "Website_ProductDetail_Lang",
             "{lang}/CompanyWebsite/{CompName}/Product/Detail/{CompID}", // URL with parameters
             new { controller = "Product", action = "Detail", CompName = UrlParameter.Optional, CompID = UrlParameter.Optional } // Parameter defaults
             , new { lang = @"[a-zA-Z]{2}" }
         );
            context.MapRoute(
                "Website_ProductDetail",
                "CompanyWebsite/{CompName}/Product/Detail/{CompID}", // URL with parameters
                new { controller = "Product", action = "Detail", CompName = UrlParameter.Optional, CompID = UrlParameter.Optional } // Parameter defaults

            );
        
            #endregion


            #region search_ProductDetail
            context.MapRoute(
            "search_ProductDetail_Lang",
            "{lang}/Search/Product/Detail/{id}/{proname}", // URL with parameters
            new { controller = "Product", action = "Detail", id = UrlParameter.Optional, proname = UrlParameter.Optional } // Parameter defaults
            , new { lang = @"[a-zA-Z]{2}" }
        );
            context.MapRoute(
              "search_ProductDetail",
              "Search/Product/Detail/{id}/{proname}", // URL with parameters
              new { controller = "Product", action = "Detail", id = UrlParameter.Optional, proname = UrlParameter.Optional } // Parameter defaults

          );
            #endregion

        
       
            #region search_ProductGallery

            context.MapRoute(
                 "search_ProductGallery_Lang",
                 "{lang}/Search/Product/Gallery/{id}/{proname}", // URL with parameters
                 new { controller = "Product", action = "Gallery", id = UrlParameter.Optional, proname = UrlParameter.Optional } // Parameter defaults
              , new { lang = @"[a-zA-Z]{2}" }
              );

            context.MapRoute(
              "search_ProductGallery",
              "Search/Product/Gallery/{id}/{proname}", // URL with parameters
              new { controller = "Product", action = "Gallery", id = UrlParameter.Optional, proname = UrlParameter.Optional } // Parameter defaults
          );
            #endregion

            #region Search Supplier List
            //----------------Supplier---------------------

            context.MapRoute(
              "Search_Supplier_List_lang",
              "{lang}/Search/Supplier/List", // URL with parameters
              new { controller = "Supplier", action = "Index" } // Parameter defaults
                 , new { lang = @"[a-zA-Z]{2}" }
            );

            context.MapRoute(
                "Search_Supplier_List",
                "Search/Supplier/List", // URL with parameters
                new { controller = "Supplier", action = "Index" } // Parameter defaults
            );

            #endregion

            #region Search_Category_Supplier

            context.MapRoute(
             "Search_Category_Supplier_Lang",
             "{lang}/Search/Supplier/List/Category/{Category}/{CateLevel}/{CategoryName}", // URL with parameters 
             new { controller = "Supplier", action = "Index", Category = UrlParameter.Optional, CateLevel = UrlParameter.Optional, CategoryName = UrlParameter.Optional } // Parameter defaults
             , new { lang = @"[a-zA-Z]{2}" }
             );
            context.MapRoute(
                "Search_Category_Supplier",
                "Search/Supplier/List/Category/{Category}/{CateLevel}/{CategoryName}", // URL with parameters 
                new { controller = "Supplier", action = "Index", Category = UrlParameter.Optional, CateLevel = UrlParameter.Optional, CategoryName = UrlParameter.Optional } // Parameter defaults
            );
         
            #endregion

            #region Search_Supplier_PostList 
            
            context.MapRoute(
                "Search_Supplier_PostList_Lang",
                "{lang}/Search/Supplier/PostList", // URL with parameters
                new { controller = "Supplier", action = "List" } // Parameter defaults 
                , new { lang = @"[a-zA-Z]{2}" }
            );

            context.MapRoute(
                "Search_Supplier_PostList",
                "Search/Supplier/PostList", // URL with parameters
                new { controller = "Supplier", action = "List" } // Parameter defaults
            );


           
            #endregion

            #region Search_Category_SupplierMenu

            context.MapRoute(
                "Search_Category_SupplierMenu_Lang",
                "{lang}/Search/Supplier/Menu", // URL with parameters
                new { controller = "Supplier", action = "ChildCateMenu" } // Parameter defaults  
                , new { lang = @"[a-zA-Z]{2}" }
                );

            context.MapRoute(
               "Search_Category_SupplierMenu",
               "Search/Supplier/Menu", // URL with parameters
               new { controller = "Supplier", action = "ChildCateMenu" } // Parameter defaults  
           );



            #endregion

            #region Search_SupplierNameAuto
            
            context.MapRoute(
                 "Search_SupplierNameAuto_Lang",
                 "{lang}/Search/Supplier/GetCompName", // URL with parameters
                 new { controller = "Supplier", action = "GetCompName" } // Parameter defaults 
                 , new { lang = @"[a-zA-Z]{2}" }
             );

            context.MapRoute(
                "Search_SupplierNameAuto",
                "Search/Supplier/GetCompName", // URL with parameters
                new { controller = "Supplier", action = "GetCompName" } // Parameter defaults 
            );
         
            #endregion

            #region Search_Supplier_SuppplierName  
            context.MapRoute(
            "Search_Supplier_SuppplierName_Lang",
            "{lang}/Search/Supplier/List/{TextSearch}", // URL with parameters
            new { controller = "Supplier", action = "Index", CompName = UrlParameter.Optional } // Parameter defaults
            , new { lang = @"[a-zA-Z]{2}" }
        );
            context.MapRoute(
               "Search_Supplier_SuppplierName",
               "Search/Supplier/List/{TextSearch}", // URL with parameters
               new { controller = "Supplier", action = "Index", CompName = UrlParameter.Optional } // Parameter defaults 
           );
      
            #endregion

            #region search_SupplierDetail
            context.MapRoute(
                 "search_SupplierDetail_Lang",
                 "{lang}/Search/Supplier/Detail/{id}/{compname}", // URL with parameters
                 new { controller = "Supplier", action = "Detail", id = UrlParameter.Optional, compname = UrlParameter.Optional } // Parameter defaults
            , new { lang = @"[a-zA-Z]{2}" }
                 );

            context.MapRoute(
              "search_SupplierDetail",
              "Search/Supplier/Detail/{id}/{compname}", // URL with parameters
              new { controller = "Supplier", action = "Detail", id = UrlParameter.Optional, compname = UrlParameter.Optional } // Parameter defaults

              );

            
            #endregion

            #region Search_default
            context.MapRoute(
                "Search_default_Lang",
                "{lang}/Search/{controller}/{action}/{id}/{name}",
                new { action = "Index", id = UrlParameter.Optional, name = UrlParameter.Optional }
                , new { lang = @"[a-zA-Z]{2}" }

            );
             context.MapRoute(
                "Search_default",
                "Search/{controller}/{action}/{id}/{name}",
                new { action = "Index", id = UrlParameter.Optional, name = UrlParameter.Optional }
                );
            #endregion
           
        }
    }
}
