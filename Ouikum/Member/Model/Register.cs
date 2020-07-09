using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Globalization; 
namespace Ouikum.Common
{
    public class Register  
    { 
        public int? MemberID { get; set; }
        public string MemberCode { get; set; }
        public string CompCode { get; set; }
        public byte CompLevel { get; set; }
        public byte MemberType { get; set; }
        public string FacebookID { get; set; }
        public string CompName { get; set; }
        public int? BizTypeID { get;set; }
        public string IsTrust { get; set; }
        public string IsSME { get; set; }
        public string BizTypeOther { get; set; }
        public int? ServiceType { get; set; }
        public int? emCompID { get; set; }
        public int? emCompProfileID { get; set; }
        public string captcha { get; set; }

        [Required(ErrorMessage = "กรุณากรอกชื่อผู้ใช้ในระบบ")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "กรุณากรอกรหัสผ่าน")]
        public string Password { get; set; }

        [Required(ErrorMessage = "กรุณากรอกชื่อที่ต้องการแสดง")]
        public string DisplayName { get; set; }

        [Required(ErrorMessage = "กรุณากรอกชื่อ")]
        public string FirstName_register { get; set; }

        [Required(ErrorMessage = "กรุณากรอกนามสกุล")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "กรุณากรอกรหัสบัตรประชาชน")]
        [StringLength(13)]
        public string IdentityNo { get; set; }
  
        public int? Gender { get; set; }
        public byte Sex { get; set; }

        [Required(ErrorMessage = "กรุณาระบุวันเกิด")]
        public DateTime? BirthDate { get; set; }
        public int? District { get; set; }
        public int? Province { get; set; }
        public int? Country { get; set; }

        [Required(ErrorMessage = "กรุณากรอกรหัสไปรษณีย์")]
        public string PostalCode { get; set; }

        [Required(ErrorMessage = "กรุณาระบุอีเมลที่ต้องการใช้ในการติดต่อ")]
        public string Emails { get; set; }

        [Required(ErrorMessage = "กรุณาระบุเบอร์โทรศัพท์ที่ต้องการใช้ในการติดต่อ")]
        public string Phone { get; set; }

        public string Mobile { get; set; }
        public string Fax { get; set; }
        public string AvatarImgPath { get; set; }
        public DateTime? RegisDate { get; set; }
        public string AddrLine1 { get; set; }
        public string AddrLine2 { get; set; }
        public string SubDistrict { get; set; }
        public int? DistrictID { get; set; }
        public int? ProvinceID { get; set; }
        public int? CountryID { get; set; }
        public string FirstNameEng { get; set; }
        public string LastNameEng { get; set; }
        public string AddrLine1Eng { get; set; }
        public string AddrLine2Eng { get; set; }
        public string SubDistrictEng { get; set; }
        public bool? IsReceived { get; set; }
        public bool? IsShowImg { get; set; }
        public int? ObjectState { get; set; }

        public int? RowFlag{get; set;}
        public int? RowVersion { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ExpireDate { get; set; }
        public int? WebID { get; set; }
        }
    }
