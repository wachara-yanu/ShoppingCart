using System.Text;

namespace System.Web.Mvc
{
    public static class UrlEncoder
    {
        #region FriendlyEncode
        /// <summary>
        /// สำหรับ Encode ข้อความ
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="Keyword">ข้อความที่ต้องการทำ Friendly Url</param>
        /// <returns></returns>
        public static string FriendlyEncode(this UrlHelper helper, string Keyword)
        {
            StringBuilder url = new StringBuilder();

            foreach (char ch in Keyword)
            {
                switch (ch)
                {
                    case '\"':              // Illegal characters in path.
                    case '|':               // Illegal characters in path.
                        break;

                    case ' ':               // %20
                    case '`':               // %60
                    case '\\':              // The resource cannot be found.
                    case '/':               // The resource cannot be found.
                    case '?':               // ERR_TOO_MANY_REDIRECTS
                    case '<':               // HTTP Error 400 - Bad Request.
                    case '>':               // HTTP Error 400 - Bad Request.
                    case '.':               // HTTP Error 400 - Bad Request. 
                        url.Append('-');
                        break;

                    case '&':               // A potentially dangerous Request.Path value was detected from the client (&).
                        url.Append("and");
                        break;

                    case '{':               // %7B
                    case '[':               // %5B
                        url.Append('(');
                        break;

                    case '}':               // %7D
                    case ']':               // %5D
                        url.Append(')');
                        break;

                    //allow char
                    case '\'':
                    default:
                        url.Append(ch);
                        break;
                }
            }

            return url.ToString();
        }
        #endregion

        #region FriendlyEncode
        // ปิดใช้งานเนื่องจากมีปัญหากับ Url ใน CMWEB
        //public static string FriendlyEncode(this UrlHelper helper, string preUrl, string Keyword)
        //{
        //    StringBuilder url = new StringBuilder();

        //    foreach (char ch in Keyword)
        //    {
        //        switch (ch)
        //        {
        //            case '\"':              // Illegal characters in path.
        //            case '|':               // Illegal characters in path.
        //                break;

        //            case ' ':               // %20
        //            case '`':               // %60
        //            case '\\':              // The resource cannot be found.
        //            case '/':               // The resource cannot be found.
        //            case '?':               // ERR_TOO_MANY_REDIRECTS
        //            case '<':               // HTTP Error 400 - Bad Request.
        //            case '>':               // HTTP Error 400 - Bad Request.
        //            case '.':               // HTTP Error 400 - Bad Request. 
        //                url.Append('-');
        //                break;

        //            case '&':               // A potentially dangerous Request.Path value was detected from the client (&).
        //                url.Append("and");
        //                break;

        //            case '{':               // %7B
        //            case '[':               // %5B
        //                url.Append('(');
        //                break;

        //            case '}':               // %7D
        //            case ']':               // %5D
        //                url.Append(')');
        //                break;

        //            //allow char
        //            case '\'':
        //            default:
        //                url.Append(ch);
        //                break;
        //        }
        //    }

        //    preUrl = (preUrl ?? "").Trim();
        //    if (!preUrl.StartsWith("~/") && !preUrl.StartsWith("http"))
        //        preUrl = "~/" + preUrl;
        //    if (!preUrl.EndsWith("/"))
        //        preUrl += "/";

        //    return helper.Content(preUrl + url);
        //}
        #endregion
    }
}