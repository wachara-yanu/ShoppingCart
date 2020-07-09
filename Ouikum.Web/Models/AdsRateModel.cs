using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using res = Prosoft.Resource.Web.Ouikum;

namespace Ouikum.Web.Models
{
    public static class AdsRateModel
    { 
        public static double PackageS  = 2000;
        public static double PackageM  = 3500;
        public static double PackageL = 5000;
        public static string PackageCal(string size = "s")
        {
            double money = 0;
            if (size.ToLower() == "s")
            {
                money = (PackageS + (PackageS * 7) / 100);
            }
            else if (size.ToLower() == "m")
            {
                money = (PackageM + (PackageM * 7) / 100);
            }
            else if (size.ToLower() == "l")
            {
                money = (PackageL + (PackageL * 7) / 100);
            }

            return money.ToString("N2");
        }
    }   

}