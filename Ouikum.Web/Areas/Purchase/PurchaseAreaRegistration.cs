using System.Web.Mvc;

namespace Ouikum.Web.Purchase
{
    public class PurchaseAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Purchase";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            #region Search_BuyleadNameAuto

            context.MapRoute(
           "Search_BuyleadNameAuto_Lang",
           "{lang}/Purchase/Search/GetBuyleadName", // URL with parameters
           new { controller = "Search", action = "GetBuyleadName" } // Parameter defaults 
           , new { lang = @"[a-zA-Z]{2}" }
       );
            context.MapRoute(
                "Search_BuyleadNameAuto",
                "Purchase/Search/GetBuyleadName", // URL with parameters
                new { controller = "Search", action = "GetBuyleadName" } // Parameter defaults 
            );
            #endregion

            #region Search_Buylead_BuyleadName

            context.MapRoute(
                 "Search_Buylead_BuyleadName_Lang",
                      "{lang}/Purchase/Search/{BuyleadName}", // URL with parameters
                    new { controller = "Search", action = "Index", BuyleadName = UrlParameter.Optional } // Parameter defaults
                    , new { lang = @"[a-zA-Z]{2}" }
                    );
            context.MapRoute(
               "Search_Buylead_BuyleadName",
               "Purchase/Search/{BuyleadName}", // URL with parameters
               new { controller = "Search", action = "Index", BuyleadName = UrlParameter.Optional } // Parameter defaults
           );

   

            #endregion

            #region Purchase_default

            context.MapRoute(
                "Purchase_default_Lang",
                "{lang}/Purchase/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
                , new { lang = @"[a-zA-Z]{2}" }
            );

            context.MapRoute(
                "Purchase_default",
                "Purchase/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );

   
            #endregion

            #region search_PurchaseDetail

            context.MapRoute(
                "search_PurchaseDetail_Lang",
                "{lang}/Purchase/Search/Detail/{id}/{proname}", // URL with parameters
                new { controller = "Search", action = "Detail", id = UrlParameter.Optional, proname = UrlParameter.Optional } // Parameter defaults
                , new { lang = @"[a-zA-Z]{2}" }
                );
            context.MapRoute(
             "search_PurchaseDetail",
             "Purchase/Search/Detail/{id}/{proname}", // URL with parameters
             new { controller = "Search", action = "Detail", id = UrlParameter.Optional, proname = UrlParameter.Optional } // Parameter defaults
         );

  
            #endregion
        }
    }
}
