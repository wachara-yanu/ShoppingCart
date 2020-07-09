using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Ouikum.Common
{
    public class Mail : Prosoft.Base.BaseObject
    {
        public string MailForm { get; set; }
        public string MailTo { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string LinkActivate { get; set; }
    }
}
