using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using res = Prosoft.Resource.Web.Ouikum;

namespace Ouikum.Web.Models
{
    public class AdsBannerModel
    {
        public string ImageData { get; set; }
        public string Label { get; set; }
        public string Url { get; set; }

        public List<AdsBannerModel> GetAdsBanner()
        {
            var banners = new List<AdsBannerModel>();
            #region Set Banner Lion job
            var _banner = new AdsBannerModel();
            _banner.ImageData = "img_lion.png";
            _banner.Label = res.Benefit.lblfindjob;
            _banner.Url = res.Pageviews.PvLionJob;
            banners.Add(_banner);
            #endregion

            #region Set Banner Sogood
            _banner = new AdsBannerModel();
            _banner.ImageData = "img_sogood.png";
            _banner.Label =  res.Benefit.lblcreatewebsitefree;
            _banner.Url = res.Pageviews.PvSogood;
            banners.Add(_banner);
            #endregion           

            #region Set Banner Isara
            _banner = new AdsBannerModel();
            _banner.ImageData = "img_Isara.png";
            _banner.Label = res.Benefit.lblIsara;
            _banner.Url = "http://www.isaraprinting.com/";
            banners.Add(_banner);
            #endregion

            return banners;
        }
    }

   

}