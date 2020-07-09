using System.Web.Mvc;

namespace Ouikum.Web.Admin
{
    public class AdminAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                    "Admin_default_Lang",
                    "{lang}/Admin/{controller}/{action}/{id}",
                    new { action = "Index", id = UrlParameter.Optional }
                    , new { lang = @"[a-zA-Z]{2}" }
                );


            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );

            
        }
    }
}
