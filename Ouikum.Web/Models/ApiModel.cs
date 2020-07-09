using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ouikum.Web.Models
{
    #region Address
    public class ProvinceModel
    {
        public int provinceid { get; set; }
        public string provincename { get; set; }
        public List<DistrictModel> districts { get; set; }

    }

    public class DistrictModel
    {
        public int districtid { get; set; }
        public string districtname { get; set; }
        public int provinceid { get; set; }


    }

    #endregion

    #region ProfileModel

    public class ProfileModel
    {
        public int memberid { get; set; }
        public string displayname { get; set; }

    }


    #endregion

    #region CateModel
    public class CateModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public int level { get; set; }
        public int parentid { get; set; }
        public string parentpath { get; set; }
        public List<CateModel> subcategory { get; set; }
    }

    public class FollowCateModel : CateModel
    {
        public int compid { get; set; }
    }
    #endregion

    #region Product Model
    #region CompanyMobileModel
    public class CompanyAppModel
    {
        public int compid { get; set; }
        public string compname { get; set; }
        public string compnameeng { get; set; }
        public int complevel { get; set; }
        public string logoimgpath { get; set; }
        public string image { get; set; }
        public string compaddress { get; set; }
        public string comptel { get; set; }
        public string compfax { get; set; }
        public string compemail { get; set; }
        public string compmobile { get; set; }
        public string compwebsite { get; set; }
        public bool isfav { get; set; }
        public string facebookid { get; set; }
        public int province { get; set; }
        public int district { get; set; }
        public List<ProductModel> product { get; set; }
    }
    #endregion

    public class ProductModel
    {
        public int productid { get; set; }
        public string productname { get; set; }
        public string imagepath { get; set; } 
        public string parentpath { get; set; }
        public string productcode { get; set; }     
        public string productnameeng { get; set; }
        public string shortdescription { get; set; }
        public string productdetail { get; set; }
        public decimal price { get; set; }
        public decimal promotionprice { get; set; }
        public bool ispromotion { get; set; } 
        public string qtyunit  { get; set; }
        public int minorderqty { get; set; }
        public DateTime createdate { get; set; }
        public DateTime modifieddate { get; set; }
        public string tel { get; set; }
        public int compid { get; set; }
        public int complevel { get; set; }
        public string compname { get; set; }
        public bool isfav { get; set; }
        public List<GalleryModel> gallery { get; set; } 
    }
    public class GalleryModel
    {
        public long imageid { get; set; }
        public string imagepath { get; set; }
    }
    #endregion

    #region MsgModel
    public class MsgModel
    {
        public int messageid { get; set; }
        public string messagecode { get; set; }
        public int rootmessageid { get; set; }
        public int fromcompid { get; set; }
        public int tocompid { get; set; }
        public string fromname { get; set; }
        public string toname { get; set; }
        public string subject { get; set; } 
        public string msgdetail { get; set; }
        public bool isfavorite { get; set; }
        public bool isread { get; set; }
        public bool isreply { get; set; }
        public bool issend { get; set; }
        public string fromcontactphone { get; set; }
        public DateTime createdate { get; set; }
        public DateTime modifieddate { get; set; }
        public List<MsgModel> submessage { get; set; }
    }
    #endregion
     

    #region QuotationModel
    public class QuotationModel 
    {
        public int quotaionid { get; set; }
        public string quotaioncode { get; set; }
        public int rootquotaionid { get; set; }
        public int fromcompid { get; set; }
        public int tocompid { get; set; }
        public string fromname { get; set; }
        public string toname { get; set; }
        public string subject { get; set; }
        public string msgdetail { get; set; }
        public bool isfavorite { get; set; }
        public bool isread { get; set; }
        public bool isreply { get; set; }
        public bool isoutbox { get; set; }
        public bool ispublic { get; set; }
        public bool ismatching { get; set; }
        public string status { get; set; }
        public string fromcontactphone { get; set; }
        public string fromemail { get; set; }
        public List<QuotationModel> subquotation { get; set; }

        public int productid { get; set; }
        public string productname { get; set; }
        public string rootquotationcode { get; set; }
        public string image { get; set; }
        public int qty { get; set; }
        public string qtyunit { get; set; }
        public DateTime createdate { get; set; }
        public DateTime modifieddate { get; set; }
    }

    #endregion

}