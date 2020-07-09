using System;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Collections.Generic;

//Using Other
using Autofac;


namespace Prosoft.Base
{
    #region Common
    public static class Common
    {
        public static IContainer container;
        public static string AppCode = (ConfigurationManager.AppSettings["AppCode"] != null) ? ConfigurationManager.AppSettings["AppCode"].ToString() : string.Empty;
        public static string ConnectionCookieKey = (ConfigurationManager.AppSettings["AppCode"] != null) ? ConfigurationManager.AppSettings["AppCode"].ToString() + "_ConnectionName" : "ConnectionName";
    
    }
    #endregion
}
