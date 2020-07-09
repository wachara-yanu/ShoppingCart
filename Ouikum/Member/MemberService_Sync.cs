using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using Prosoft.Base;
//using System.Web.Mvc;
using System.Transactions;
using Prosoft.Service;


namespace Ouikum.Common
{


    public partial class MemberService : BaseSC
    {
        #region Member
        #region GetMember
        public MemberModel GetMember(string secretid)
        {
            var model = new SyncModel();
            var member = new Ouikum.Common.MemberModel();
            var svMember = new MemberService();
            if (!string.IsNullOrEmpty(secretid))
            {
                var encrypt = new EncryptManager();
                var list = encrypt.DecryptData(secretid).Split('~');

                if (list != null)
                {
                    model.username = list[1];
                    model.password = list[2];
                    model.webid = list[3];
                    model.memberid = int.Parse(list[4]);
                    encrypt = new EncryptManager();
                    var pw = encrypt.EncryptData(model.password);
                    var data = svMember.SelectData<view_Member>(" * ", @" IsDelete = 0 AND ( UserName = N'" +
                    model.username + "' OR Email = N'" + model.username + "' ) AND Password = N'" + pw + "'").First();

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


                }
            }

            return member;
        }

        public emMember GetEmMember(string secretid)
        {
            var model = new SyncModel();
            var data = new emMember();

            var svMember = new MemberService();
            if (!string.IsNullOrEmpty(secretid))
            {
                var encrypt = new EncryptManager();
                var list = encrypt.DecryptData(secretid).Split('~');

                if (list != null)
                {
                    model.username = list[1];
                    model.password = list[2];
                    model.webid = list[3];
                    model.memberid = int.Parse(list[4]);
                    encrypt = new EncryptManager();
                    var pw = encrypt.EncryptData(model.password);
                    data = svMember.SelectData<emMember>(" * ", @" IsDelete = 0 AND ( UserName = N'" +
                    model.username + "' OR Email = N'" + model.username + "' ) AND Password = N'" + pw + "'").First();
                     
                }
            }

            return data;

        }
        #endregion

        #endregion

    }
}
