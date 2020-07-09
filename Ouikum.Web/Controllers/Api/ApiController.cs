using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Prosoft.Service;
//using Prosoft.Base;
using Ouikum.Web.Models;
using Ouikum.Common;
using Ouikum.Company;
using res = Prosoft.Resource.Web.Ouikum;
using Ouikum.Product;
using Ouikum.Category;
using System.Runtime.Caching;
//using IOS.Service;
namespace Ouikum.Web.Controllers
{
    public partial class ApiController : BaseController
    {
        //
        // GET: /Api/
        public int webid = 1;


        [HttpGet]
        public ActionResult productowner(int productid)
        {
            EncryptManager encrypt = new EncryptManager();
            
            var svProduct = new ProductService();
            var svMember = new MemberService();
            var product = svProduct.SelectData<b2bProduct>(" * ", "productid = " + productid);
            if (product != null && product.Count > 0)
            {
                var comp = svMember.SelectData<view_CompMember>(" * ", " compid = " + product[0].CompID);
                
                var model = new view_CompMember();
                model = comp[0];
                model.Password = encrypt.DecryptData(comp[0].Password);
                return Json(new { 
                    productid = product[0].ProductID,
                    productname = product[0].ProductName,
                    compid = model.CompID, 
                    memberid = model.MemberID,
                    name = model.CompName,
                    email = model.Email,
                    user = model.UserName,
                    password = model.Password
                }, JsonRequestBehavior.AllowGet);
            }  
            else
            {
                return Json(new { compid = 0, memberid = 0, name = "", email = "", password = "" }, JsonRequestBehavior.AllowGet);
            }
            
        }
        [HttpGet]
        public ActionResult checkpassword(int compid)
        {
            EncryptManager encrypt = new EncryptManager();

            var svMember = new MemberService();
            var data = svMember.SelectData<view_CompMember>("MemberID,UserName,Password,CompName,Email,CompID", "CompID = "+compid, null);
            if (data != null && data.Count > 0)
            {
                var model = new view_CompMember();
                model = data[0];
                model.Password = encrypt.DecryptData(data[0].Password);
                return Json(new { 
                    compid = model.CompID, 
                    memberid = model.MemberID, 
                    name = model.CompName,
                    email = model.Email,
                    user = model.UserName,
                    password = model.Password
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { compid = 0, memberid = 0, name = "", email = "", password = "" }, JsonRequestBehavior.AllowGet);
        }

        #region login
        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult login(string username, string password)
        {
            EncryptManager encrypt = new EncryptManager();
            var msgerror = "";
            var svMember = new MemberService();
            var Url = new UrlHelper(System.Web.HttpContext.Current.Request.RequestContext);
            var sqlWhere = @" (UserName = N'" + username + "' or Email = N'" + username + "') AND " +
                " ( RowFlagWeb = 2 and IsDelete = 0 and WebID =" + webid + " ) ";

            var query = svMember.SelectData<view_CompMember>("MemberID,UserName,Password,DisplayName,Email,CompID", sqlWhere, null);

            var result = false;
            var compid = 0;
            if (svMember.TotalRow > 0)
            {
                result = true;
                var comp = query.First();
                if(comp.Password == encrypt.EncryptData(password))
                {
                    compid = comp.CompID;
                    result = true;
                }else
	            {
                    result = false;
                    msgerror = "รหัสผ่านของคุณ ไม่ตรงกัน";
	            } 
            }
            else
            {
                result = true;
                msgerror = "ไม่พบ ผู้ใช้ของคุณ";
            }
            return Json(new { result = result, compid = compid, msgerror = msgerror }, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region province
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult province()
        {
            var svAddress = new AddressService();
            var pv = new List<ProvinceModel>();

            if (MemoryCache.Default["provinceapp"] != null)
            {
                pv = (List<ProvinceModel>)MemoryCache.Default["provinceapp"];
            }
            else
            {
                #region set provinces

                var provinces = svAddress.SelectData<emProvince>(" * ", " IsDelete = 0 ");
                var districtes = svAddress.SelectData<emDistrict>(" * ", " IsDelete = 0 ");
                foreach (var p in provinces)
                {
                    var m = new ProvinceModel();
                    m.provinceid = p.ProvinceID;
                    m.provincename = p.ProvinceName;
                    m.districts = new List<DistrictModel>();
                    foreach (var d in districtes.Where(it => it.ProvinceID == p.ProvinceID))
                    {
                        var dd = new DistrictModel();
                        dd.districtid = d.DistrictID;
                        dd.districtname = d.DistrictName;
                        dd.provinceid = (int)d.ProvinceID;
                        m.districts.Add(dd);
                    }
                    pv.Add(m);
                }
                #endregion

                MemoryCache.Default.Add("provinceapp", pv, DateTime.Now.AddDays(1));
            }
            return Json(new { province = pv }, JsonRequestBehavior.AllowGet);

        }
        #endregion
        
        #region signup
        [HttpGet]
        public ActionResult signup(string email, string password, string firstname, string lastname, string tel, int province, int district)
        {
            var model = new Register();
            var compid = 0;
            var result = false;
            var msg = "";
            model.DisplayName = firstname + " "+  lastname;
            model.FirstName_register = firstname;
            model.LastName = lastname;
            model.CompLevel = 1;
            model.CompName = model.DisplayName;
            
            model.UserName = email;
            model.Password = password;
            model.Emails = email;
            model.Phone = tel;
            model.ProvinceID = province;
            model.DistrictID = district;
            model.CompLevel = 1;
            model.BizTypeID = 13;
            model.BizTypeOther = "อื่นๆ";
            model.ServiceType = 2;
            model.WebID = webid;

            var svMember = new MemberService();
            var svCompany = new CompanyService();
            var count = svMember.CountData<emMember>(" * ", "( IsDelete = 0 AND RowFlag = 2 ) AND (UserName = N'" + email + "' or Email = N'" + email + "' )");
            if (count == 0)
            {
                svMember.UserRegister(model);

                if (svMember.IsResult)
                {
                    svCompany.InsertCompany(model);
                    var comp = svCompany.SelectData<b2bCompany>(" * ", " MemberID = " + model.MemberID);
                    if (svCompany.TotalRow > 0)
                    {
                        compid = comp.First().CompID;
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
                else
                {
                    result = false;
                }
            }
            else
            {
                result = false;
                msg = "ผู้ใช้นี้มีอยู่แล้วในระบบ";
            } 

            return Json(new { status =result, compid = compid, msg = msg }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region isMember
        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult isMember(string email)
        {
            EncryptManager encrypt = new EncryptManager();
            var svComp = new CompanyService();
            var Url = new UrlHelper(System.Web.HttpContext.Current.Request.RequestContext);
            var result = false;
            var compid = 0;
            try
            {
                var sqlWhere = " CompIsDelete = 0 AND CompRowFlag IN (2,4) ";
                sqlWhere += " AND Email = N'" + email.Replace("\'", "") + "'";
                var data = svComp.SelectData<view_CompMember>(" * ", sqlWhere, null);

                if (svComp.TotalRow > 0)
                {
                    result = true;
                    compid = data.First().CompID;
                }
            }
            catch (Exception ex)
            {
                result = false;
            }
         
            return Json(new { result = result, compid = compid }, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region logout
        public ActionResult logout(string appkey,int compid)
        {
            var svComp = new CompanyService();
            svComp.DeleteNotification(appkey, compid);
            return Json(new { result = svComp.IsResult}, JsonRequestBehavior.AllowGet);

        }

        #endregion



        [HttpGet]
        public ActionResult changepassword(int compid,string password)
        { 
            var svCompany = new CompanyService();
            var svMember = new MemberService();
            var result = false;
            var companies = svCompany.SelectData<view_CompMember>(" * ", " CompID = " + compid);
            if (companies != null && companies.Count > 0)
            {
                EncryptManager encrypt = new EncryptManager();
                var pw = encrypt.EncryptData(password); 
                    svMember.UpdateByCondition<emMember>("Password = N'"+pw+ "' "," MemberID = "+ companies[0].MemberID);

                    result = true;  
            } 
            return Json(new { status = result }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult contactus(string topic,string name,string compname,string email,string tel,string message)
        {
            var form = new FormCollection();
            form["Name"]  = name;
            form["CompName"] = compname;
            form["Email"] = email;
            form["Phone"] = tel;
            form["Subject"] = topic;
            form["Detail"] = message;
          var result=   OnSendMailContactUs(form);
          return Json(new { status = result }, JsonRequestBehavior.AllowGet);
        }



    }
}
