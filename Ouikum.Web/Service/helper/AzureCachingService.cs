using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.ApplicationServer.Caching;

namespace System.Web.Mvc
{
    public class AzureCachingService
    {
        public string GetCaching()
        {
            DataCacheFactory cacheFactory = new DataCacheFactory();
            
            DataCache cache = cacheFactory.GetDefaultCache();
            object result = cache.Get("Item");

            string str = " ";
            if (result == null)
            {
                // "Item" not in cache. Obtain it from specified data source
                // and add it.
                var times = TimeSpan.FromMinutes(2);
                str = "no cache..."+times.ToString();
                cache.Add("item", str, times);
            }
            else
            {
                str = (string)result;
                // "Item" is in cache, cast result to correct type.
            }

            return str;
        }

    }
}