using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ouikum.Web.Models
{
    public class SignInModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool Remember { get; set; }
    }

   

}