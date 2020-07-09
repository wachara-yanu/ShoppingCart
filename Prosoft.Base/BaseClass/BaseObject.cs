using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Prosoft.Base
{
    #region BaseObject
    /// <summary>
    /// กำหนดฟิลด์มาตรฐานที่ต้องมีในทุกๆ ตาราง
    /// </summary>
    public class BaseObject
    {
        public string ID { get; set; }
        public short RowFlag { get; set; }
        public short RowVersion { get; set; }
        //public string CreatedBy { get; set; }     //เลิกใช้ ณ 09/10/2555
        public DateTime CreatedDate { get; set; }
        //public string ModifiedBy { get; set; }    //เลิกใช้ ณ 09/10/2555
        public DateTime ModifiedDate { get; set; }
        //public int ObjectState { get; set; }      //เลิกใช้ ณ 09/10/2555
    }
    #endregion
}
