using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ouikum.Web.Models
{
    public class Cart
    {
        public b2bProduct Product { get; set; }
        public int Quantity { get; set; }
        public int CompIDsc { get; set; }
        public int ComIDbuy { get; set; }

        public int TotalRow { get; set; }
        
    }
}