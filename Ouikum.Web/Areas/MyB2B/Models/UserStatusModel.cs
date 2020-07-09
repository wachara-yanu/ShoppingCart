
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ouikum.Company
{
    public class UserStatusModel
    {
        public int? CompID { get; set; }
        public int? CompLevel { get; set; }
        public int? ServiceType { get; set; }
        public string Email { get; set; }
        public string CompName { get; set; }
        public string DisplayName { get; set; }
        public int MemId { get; set; }
        public string MemImgPath { get; set; }
        public string MemEmail { get; set; }
        public string MemFirstName { get; set; }
        public string MemLastName { get; set; }

        public int TotalTempCart { get; set; }

        public string MemPhone { get; set; }

        public string MemAddrLine1 { get; set; }
        
        public string MemAddrLine2 { get; set; }

        public int? MemProviceID { get; set; }

        public int? MemDirtriceID { get; set; }

        public string MembSubDirticeID { get; set; }

        public string MemPostalCode { get; set; }

    }
}