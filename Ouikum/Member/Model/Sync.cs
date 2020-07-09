using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ouikum.Common
{
    #region Model

    #region SyncModel
    public class SyncModel
    {
        public string username { get; set; }
        public string password { get; set; }
        public string webid { get; set; }
        public int? memberid { get; set; }
    }
    #endregion

    #region MemberModel
    public class MemberModel
    {
        public int? MemberID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Displayname { get; set; }
        public string WebID { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Phone { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public int? DistrictID { get; set; }
        public int? ProvinceID { get; set; }
        public int? CountryID { get; set; }

        public int? RowVersion { get; set; }
        public int? RowFlag { get; set; }
        public string SecretID { get; set; }

    }
    #endregion


    #region MemberWebModel
    public class MemberWebModel
    {
        public int? MemberID { get; set; }
        public int? WebID { get; set; }
        public string WebName { get; set; }
        public int? RowFlag { get; set; }
        public int? RowVersion { get; set; }
    }
    #endregion

    #region CompanyModel
    public class CompanyModel
    {
        public int? MemberID { get; set; }
        public int? CompID { get; set; }
        public string CompName { get; set; }
        public string Displayname { get; set; }
        public int? DistrictID { get; set; }
        public int? ProvinceID { get; set; }
        public int? CountryID { get; set; }

        public string webid { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }

        public int? RowVersion { get; set; }
        public int? RowFlag { get; set; }

    }
    #endregion

    #endregion

}
