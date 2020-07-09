using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Prosoft.Base;
using Prosoft.Service;
using Prosoft.Common;
using B2B;
namespace B2B.Web.Controllers
{
    public class WebAPIController : BaseController
    {

        [HttpGet]
        public JsonResult GetLogonSecretID()
        {
            var svSync = new SyncService();
            var username = "keropoy";
            var password = "123456";
            var webid = "1";
            var secretid = svSync.GetSecretID(username, password, webid);
            return Json(secretid);
        }

        [HttpPost]
        public JsonResult GetSecretID(string username, string password, string webid)
        {
            var svSync = new SyncService();
            var secretid = svSync.GetSecretID(username, password, webid);
            return Json(secretid);
        }

        [HttpPost]
        public JsonResult GetClientMember()
        {
            if (CheckIsLogin())
            {
                var svMember = new B2B.Common.MemberService();
                var data = svMember.SelectData<view_Member>(" * ", " IsDelete = 0 AND MemberID = " + LogonMemberID).First();
                var member = new B2B.Common.MemberModel(); 

                #region Set Data
                member.MemberID = data.MemberID;
                member.Username = data.UserName;
                member.Password = data.Password;
                member.Displayname = data.DisplayName;
                member.Email = data.Email;
                member.Mobile = data.Mobile;
                member.Phone = data.Phone;
                member.RowFlag = data.RowFlag;
                member.RowVersion = data.RowVersion;
                member.ProvinceID = data.ProvinceID;
                member.DistrictID = data.DistrictID;
                #endregion

                var encrypt = new EncryptManager();
                member.SecretID = svMember.GetSecretID(data.UserName,encrypt.DecryptData(data.Password),GetAppSetting("WebID"));
               return Json(member);
            }
            else
            {
                return Json(null);
            }
        }

        [HttpPost]
        public JsonResult GetMember(string secretid)
        {
            var svSync = new SyncService();
            var model = svSync.GetMember(secretid);
            return Json(model);
        }

        [HttpPost]
        public JsonResult SignUp(FormCollection form)
        {
            var svMember = new Prosoft.Common.MemberService();
            var member = new MemberModel();
            var svSync = new SyncService();
            var model = new Prosoft.Common.Register();

            #region
            model.UserName = form["UserName"].Trim();
            model.Password = form["Password"].Trim();
            model.CompName = form["CompName"].Trim();

            if (string.IsNullOrEmpty(form["DisplayName"])) 
                model.DisplayName = model.UserName;
            else
                model.DisplayName = form["DisplayName"].Trim();

            model.Email = form["Email"].Trim();
            model.Phone = form["Phone"];
            model.Mobile = form["Mobile"];
            model.FirstName = form["FirstName"];
            model.LastName = form["LastName"];
            model.AddrLine1 = form["AddrLine1"];

            if (!string.IsNullOrEmpty(form["DistrictID"]))
            {
                model.DistrictID = int.Parse(form["DistrictID"]);
            }

            if (!string.IsNullOrEmpty(form["ProvinceID"]))
            {
                model.ProvinceID = int.Parse(form["ProvinceID"]);
            }

            int WebID = int.Parse(form["WebID"]);

            svMember.UserSignUp(model, WebID);

            if (svMember.IsResult)
            {
                var secret = svSync.GetSecretID(model.UserName, form["password"], WebID.ToString());
                member = svSync.GetMember(secret);
            }
            #endregion

            return Json(member);
        }

        [HttpPost]
        public JsonResult SignUpWithAccount(FormCollection form)
        {
            var svMember = new Prosoft.Common.MemberService();
            var member = new MemberModel();
            var svSync = new SyncService();
            var model = new Prosoft.Common.Register();
            model.UserName = form["UserName"].Trim();
            model.Password = form["Password"].Trim();
            int WebID = int.Parse(form["WebID"]);
            svMember.UserSignUpWithAccount(model, WebID);

            if (svMember.IsResult)
            {
                var secret = svSync.GetSecretID(model.UserName, form["password"], WebID.ToString());
                member = svSync.GetMember(secret);
            }

            return Json(member);

        } 

        #region ClientMemberSync
        [HttpPost]
        public JsonResult ClientMemberSync(FormCollection form)
        {
            var svMember = new B2B.Common.MemberService();
            int ID = int.Parse(form["MemberID"].Trim());

            var model = svMember.GetEmMember(form["SecretID"]);
            var sqlUpdate = string.Empty; 
            var sqlWhere= " MemberID = " + model.MemberID;

            if (model != null)
            {
                #region Set Model
                sqlUpdate = " UserName = '" + form["UserName"].Trim() + "' ";
                sqlUpdate += ", Password = '" + form["Password"].Trim() + "' ";
                sqlUpdate += ", DisplayName = '" + form["DisplayName"] + "' ";
                sqlUpdate += ", Email = '" + form["Email"] + "' ";
                sqlUpdate += ", Phone = '" + form["Phone"] + "' ";
                sqlUpdate += ", Mobile = '" + form["Mobile"] + "' ";

                if (!string.IsNullOrEmpty(form["FirstName"])) 
                    sqlUpdate += ", FirstName = '" + form["FirstName"] + "' "; 

                if (!string.IsNullOrEmpty(form["LastName"])) 
                    sqlUpdate += ", LastName = '" + form["LastName"] + "' ";

                if (!string.IsNullOrEmpty(form["AddrLine1"]))
                    sqlUpdate += ", AddrLine1 = '" + form["AddrLine1"] + "' ";  

                if (!string.IsNullOrEmpty(form["DistrictID"]))
                    sqlUpdate += ", DistrictID =  " + form["DistrictID"].Trim() + " ";

                if (!string.IsNullOrEmpty(form["ProvinceID"]))
                    sqlUpdate += ", ProvinceID =  " + form["ProvinceID"].Trim() + " ";

                if (!string.IsNullOrEmpty(form["RowVersion"]))
                {
                    var rowversion = int.Parse(form["RowVersion"]); 
                    sqlUpdate += ", RowVersion =  " + rowversion + " ";
                }


                svMember.UpdateByCondition<emMember>(sqlUpdate, sqlWhere);
                #endregion
            }
             

            return Json(svMember.IsResult);

        }
        #endregion

        #region CommonMemberSync
        [HttpPost]
        public JsonResult CommonMemberSync(FormCollection form)
        {
            var svMember = new Prosoft.Common.SyncService(); 

            var model = svMember.GetEmMember(form["SecretID"]);
            var sqlUpdate = string.Empty;
            var sqlWhere = " MemberID = " + model.MemberID;

            if (model != null)
            {
                #region Set Model
                sqlUpdate = " UserName = '" + form["UserName"].Trim() + "' ";
                sqlUpdate += ", Password = '" + form["Password"].Trim() + "' ";
                sqlUpdate += ", DisplayName = '" + form["DisplayName"] + "' ";
                sqlUpdate += ", Email = '" + form["Email"] + "' ";
                sqlUpdate += ", Phone = '" + form["Phone"] + "' ";
                sqlUpdate += ", Mobile = '" + form["Mobile"] + "' ";

                if (!string.IsNullOrEmpty(form["FirstName"]))
                    sqlUpdate += ", FirstName = '" + form["FirstName"] + "' ";

                if (!string.IsNullOrEmpty(form["LastName"]))
                    sqlUpdate += ", LastName = '" + form["LastName"] + "' ";

                if (!string.IsNullOrEmpty(form["AddrLine1"]))
                    sqlUpdate += ", AddrLine1 = '" + form["AddrLine1"] + "' ";

                if (!string.IsNullOrEmpty(form["DistrictID"]))
                    sqlUpdate += ", DistrictID =  " + form["DistrictID"].Trim() + " ";

                if (!string.IsNullOrEmpty(form["ProvinceID"]))
                    sqlUpdate += ", ProvinceID =  " + form["ProvinceID"].Trim() + " ";

                if (!string.IsNullOrEmpty(form["RowVersion"]))
                {
                    var rowversion = int.Parse(form["RowVersion"]);
                    sqlUpdate += ", RowVersion =  " + rowversion + " ";
                }

                svMember.UpdateByCondition<Prosoft.Common.emMember>(sqlUpdate, sqlWhere);
                #endregion
            }

            return Json(svMember.IsResult);

        }
        #endregion

        #region Connect
        [HttpPost]
        public JsonResult Connect()
        {
            return Json(new { isSuccess = true});
        }
        #endregion


    }
}
