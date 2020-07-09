using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Text;
using res = Prosoft.Resource.Web.Ouikum;

namespace System.Web.Mvc
{


    public static class HtmlHelpers
    {
        private const string Nbsp = "&nbsp;";
        private const string SelectedAttribute = " selected='selected'";
        //  private const string url = "http://www.ouikum.com/Upload/Prosoft/"; 
        //private const string url = res.Pageviews.BlobStorageUrl;
        //private const string aboutUrl = res.Pageviews.AboutURL;
        // private const string url = "~/Upload/Prosoft/";


       // private const string cssUrl = "https://ouikumstorage.blob.core.windows.net/upload/Content/";
      //private const string jsUrl = "https://ouikumstorage.blob.core.windows.net/scripts/";
        private const string cssUrl = "~/Content/";
        private const string jsUrl = "~/Scripts/";
        private const string mobileUrl = "~/Content/Template/Mobile/";  
      public static string HomeBanner(this UrlHelper helper, string url)
      {
          url = "https://ouikumstorage.blob.core.windows.net/upload/Content/Default/banner/b2bthai/" + url;
          // return value;
          return helper.Content(url);
      }
      public static string Banner(this UrlHelper helper, int? BannerId, string ImgName)
      {
          string Url = res.Pageviews.BlobStorageUrl + "Banner/H1/" + BannerId +"/"+ ImgName;
          //return helper.Content(Url);
          return Url;
      }
      public static string Website(this UrlHelper helper, string url)
      {
          url = "https://ouikumstorage.blob.core.windows.net/upload/Content/Website/" + url;
          // return value;
          return helper.Content(url);
      }
      public static string Website1(this UrlHelper helper, string url)
      {
          url = cssUrl + url;
          // return value;
          return helper.Content(url);
      }
      public static string cssDefault(this UrlHelper helper, string url)
      {
          url = "~/Content/Default/" + url;
          // return value;
          return helper.Content(url);
      }
      public static string Less(this UrlHelper helper)
      {
          var url = cssUrl + "Default/Images/icon_No-Pic.jpg";
          // return value;
          return helper.Content(url);
      }
      public static string NoImage(this UrlHelper helper)
      {
          var url = cssUrl + "Default/Images/icon_No-Pic.jpg";
          // return value;
          return helper.Content(url);
      }
      public static string css(this UrlHelper helper, string value)
      {
          value = cssUrl + value;
          // return value;
          return helper.Content(value);
      }

      public static string js(this UrlHelper helper, string value)
      {
          value = jsUrl + value;
          // return value;
          return helper.Content(value);
      }

      public static string About(this UrlHelper helper, string value)
      {

          if (value == "Index")
          {

              value = res.Pageviews.AboutURL;
          }
          else
          {
              value = res.Pageviews.AboutURL +"/"+ value;
          }

          return helper.Content(value);
      }

      #region Url File

        
        public static string Upload(this UrlHelper helper,string value)
        {
            string Url = res.Pageviews.BlobStorageUrl + value;

           //return helper.Content(Url);
            return Url;
        }
        public static string Company(this UrlHelper helper, int? CompID, string ImgName)
        {
            string Url = res.Pageviews.BlobStorageUrl + "Companies/Image/" + CompID + "/" + ImgName;

           //return helper.Content(Url);
            return Url;
        }
        public static string CompanyMap(this UrlHelper helper, int? CompID, string ImgName)
        {
            string Url = res.Pageviews.BlobStorageUrl + "Companies/Map/" + CompID + "/" + ImgName; 
           //return helper.Content(Url);
            return Url;
        }

        public static string CompanyLogo(this UrlHelper helper, int? CompID, string ImgName)
        {
            string Url = res.Pageviews.BlobStorageUrl + "Companies/Logo/" + CompID + "/" + ImgName;
           //return helper.Content(Url);
            return Url;
        }

        public static string CompanyContact(this UrlHelper helper, int? CompID, string ImgName)
        {
            string Url = res.Pageviews.BlobStorageUrl + "Companies/Contact/" + CompID + "/" + ImgName;
           //return helper.Content(Url);
            return Url;
        }

        public static string CompanyCertify(this UrlHelper helper, int? CompID, int? CompCertifyID, string ImgName)
        {
            string Url = res.Pageviews.BlobStorageUrl + "CompanyCertify/" + CompID + "/" + CompCertifyID + "/" + ImgName;
           //return helper.Content(Url);
           return Url;
        }

        public static string Product(this UrlHelper helper, int? CompID, int? ProductID, string ImgName)
        {
            string Url = res.Pageviews.BlobStorageUrl + "Product/" + CompID + "/" + ProductID + "/" + ImgName;
           //return helper.Content(Url);
           return Url;
        }

        public static string Buylead(this UrlHelper helper, int? CompID, int? BuyleadID, string ImgName)
        {
            string Url = res.Pageviews.BlobStorageUrl + "Buylead/" + CompID + "/" + BuyleadID + "/" + ImgName;
           //return helper.Content(Url);
           return Url;
        }

        public static string ThumbProduct(this UrlHelper helper, int? CompID, int? ProductID, string ImgName)
        {
            string Url = res.Pageviews.BlobStorageUrl + "Product/" + CompID + "/" + ProductID + "/Thumb_" + ImgName;
          //return helper.Content(Url);
           return Url;
        }

        public static string ThumbProductNotLogin(this UrlHelper helper, int? ProductID, string ImgName)
        {
            string Url = res.Pageviews.BlobStorageUrl + "Product/UnRegis-::1/" + ProductID + "/Thumb_" + ImgName;
            //return helper.Content(Url);
            return Url;
        }

        public static string ThumbBuylead(this UrlHelper helper, int? CompID, int? BuyleadID, string ImgName)
        {
            string Url = res.Pageviews.BlobStorageUrl + "Buylead/" + CompID + "/" + BuyleadID + "/Thumb_" + ImgName;
           //return helper.Content(Url);
            return Url;
        }
        public static string Website(this UrlHelper helper, string Action , int? CompID)
        {
            return helper.Content("~/WebSite/" + Action + "/" + CompID);
        }


        public static string Article(this UrlHelper helper, int? CompID, int? ArticleID, string ImgName)
        {
            string Url = res.Pageviews.BlobStorageUrl + "Article/" + CompID + "/" + ArticleID + "/" + ImgName;
            //return helper.Content(Url);
            return Url;
        }
        public static string Member(this UrlHelper helper, int? MemberID, string ImgName)
        {
            string Url = res.Pageviews.BlobStorageUrl + "Members/" + MemberID + "/" + ImgName;
            //return helper.Content(Url);
            return Url;
        }
      #endregion

        public static string ReplaceUrl(this UrlHelper helper, string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                s = s.Replace(" ", "");
                s = s.Replace(",", "-").Replace("+", "-").Replace("&", "-").Replace("#", "-").Replace("[", "-").Replace("]", "-").Replace("'", "-").Replace("/", "")
                      .Replace(".", "").Replace("%", " ").Replace(":", "-").Replace("*", "");
            }
            else
            {
                s = "";
            }
            return s;
        }


        public static HtmlString ListSplit(this HtmlHelper helper, string value)
        {
            string str = "";
            if (!string.IsNullOrEmpty(value))
            {
                var item = value.Split('~');
                for (var i = 0; i < item.Count();i++ )
                {
                    if (!string.IsNullOrEmpty(item[i]))
                    {
                        if (item[i].Length > 25)
                            str += "<span title=\"" + item[i] + "\">&nbsp;-&nbsp;&nbsp;" + item[i].Substring(0, 25) + "</span><br/>";
                        else
                            str += "<span title=\"" + item[i] + "\">&nbsp;-&nbsp;&nbsp;" + item[i] + "</span><br/>";
                    }
                }
            }
            return new HtmlString(str.ToString());
        }


        public static HtmlString FormatDate(this HtmlHelper helper, DateTime? value)
        {
            DateTime dt = (DateTime)value;
            string d = dt.ToShortDateString();

            return new HtmlString(d);
        }
         
        public static HtmlString SubText(this HtmlHelper helper, string value, int length)
        {
            if(!string.IsNullOrEmpty(value)){
                if (value.Length > length)
                {
                    value = value.Substring(0, length) + "...";
                }
            }
            return new HtmlString(value);
        }

        public static HtmlString ProductStatus(this HtmlHelper helper, int RowFlag )
        {
            var str = string.Empty;
            #region
            if (RowFlag == 2)
                str = "<span class='text-warning'>รออนุมัติ</span>";
            else if (RowFlag == 3)
                str = "<span class='text-error'>ไม่อนุมัติ</span>";
            else if (RowFlag == 4)
                str = "<span class='text-success'>อนุมัติ</span>";
            else if (RowFlag == 5)
                str = "<span class='text-success'>อนุมัติ[แก้ไข]</span>";
            else if (RowFlag == 6)
                str = "<span class='text-warning'>รอตรวจ</span>";
            else 
                str = "<span class='muted'>" + RowFlag + "</span>";
            #endregion
            return new HtmlString(str);
        }


        public static string Mobile(this UrlHelper helper, string strUrl)
        {
            string Url = mobileUrl + strUrl;
            //return helper.Content(Url);
            return Url;
        }
    }
} 