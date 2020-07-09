using System.Web.Mvc;
namespace Ouikum.Web.MyB2B
{
    public class MyB2BAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "MyB2B";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {

            #region MyB2B_QuotationList
            context.MapRoute(
              "MyB2B_QuotationList",
              "MyB2B/Quotation/List/{Type}",
              new { controller = "Quotation", action = "List", Code = UrlParameter.Optional }
          );

            context.MapRoute(
           "MyB2B_QuotationList_Lang",
           "{lang}/MyB2B/Quotation/List/{Type}",
           new { controller = "Quotation", action = "List", Code = UrlParameter.Optional }
           , new { lang = @"[a-zA-Z]{2}" }
       );
            #endregion

            #region MyB2B_FromCode
            context.MapRoute(
              "MyB2B_FromCode",
              "MyB2B/Quotation/Detail/{ID}",
              new { controller = "Quotation", action = "Detail", Code = UrlParameter.Optional }
          );

            context.MapRoute(
           "MyB2B_FromCode_Lang",
           "{lang}/MyB2B/Quotation/Detail/{ID}",
           new { controller = "Quotation", action = "Detail", Code = UrlParameter.Optional }
           , new { lang = @"[a-zA-Z]{2}" }
       );
            #endregion


            context.MapRoute(
              "MyB2B_Reply",
              "MyB2B/Quotation/Reply/{ID}",
              new { controller = "Quotation", action = "Reply", Code = UrlParameter.Optional }
          );

            #region MyB2B_ReplyDetail


            context.MapRoute(
             "MyB2B_ReplyDetail_Lang",
             "{lang}/MyB2B/Quotation/BidProduct/{Code}",
             new { controller = "Quotation", action = "BidProduct", Code = UrlParameter.Optional }
             , new { lang = @"[a-zA-Z]{2}" }
            );
            context.MapRoute(
              "MyB2B_ReplyDetail",
              "MyB2B/Quotation/BidProduct/{Code}",
              new { controller = "Quotation", action = "BidProduct", Code = UrlParameter.Optional }
          );


            #endregion

            #region MyB2B_default


            context.MapRoute(
                "MyB2B_default_Lang",
                "{lang}/MyB2B/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
                , new { lang = @"[a-zA-Z]{2}" }
                );
            context.MapRoute(
                "MyB2B_default",
                "MyB2B/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );


            #endregion
            
        }
    }
}
