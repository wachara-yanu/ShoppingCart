using System;
using System.Collections.Generic;
using System.Linq;
using System.Web; 
using System.Web.Mvc;
using System.Globalization;
using Prosoft.Service;
//using Prosoft.Base;

namespace Ouikum.Common
{
    public class SyncService : BaseSC
    {

        public class SyncModel
        {
            public string username {get; set;}
            public string password {get; set;}
            public string webid { get; set; }
        }

        public string GenerateSecretID(SyncModel model)
        {
            var secreteid = string.Empty;
            var svMember = new MemberService();
            return secreteid;
        }

        public SyncModel GenerateSecretID(string secretid)
        {
            var model = new SyncModel();
            var svMember = new MemberService();
            return model;
        }

    }
}
