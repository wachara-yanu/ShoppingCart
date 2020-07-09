$.validator.addMethod("NumberOnly", function (value, element) {
    var i = /^[0-9]+$/;
    return this.optional(element) || (i.test(value) > 0);
});

$.validator.addMethod("PhoneOnly", function (value, element) {
    var i = /^[0-9-ต,่อ ]+$/;
    return this.optional(element) || (i.test(value) > 0);
});

$.validator.addMethod("EngOnly", function (value, element) {
    var i = /^[0-9A-Za-z@._-]+$/;
    return this.optional(element) || (i.test(value) > 0);
});

$.validator.addMethod("EmailOnly",function (value, element){
    var i = /^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z]{2,4})+$/;
    return this.optional(element) || (i.test(value) > 0);
});

$.validator.addMethod("NoZero", function (value, element) {
    var i = 0;
    return this.optional(element) || (i.test(value) != 0);
});

//////////////////////////////////// Validate Register ////////////////////////////////////
$(function () {
    $("#Form-Register").validate({
        rules: {
            UserName: {
                required: true,
                minlength: 4,
                maxlength: 16,
                EngOnly: true
            },
            Password: {
                required: true,
                minlength: 6,
                maxlength: 20
            },
            CFPassword: {
                required: true,
                minlength: 6,
                maxlength: 20,
                equalTo: "#Password"
            },
            BizTypeID: {
                required: true
            },
            CompName: {
                required: true,
                minlength: 4
            },
            FirstName: {
                required: true,
                minlength: 3,
            },
            LastName: {
                required: true,
                minlength: 3,
            },
            Phone: {
                required: true,
                minlength: 9
            },
            Emails: {
                required: true,
                EmailOnly: true
            },
            ProvinceID: {
                required: true,
            },
            DistrictID: {
                required: true,
            }
        },
        messages: {
            UserName: {
                required: "กรุณากรอกข้อมูล",
                minlength: "กรุณากรอกอย่างน้อย 4 ตัวอักษรและไม่เกิน 16 ตัวอักษร",
                maxlength: "กรุณากรอกอย่างน้อย 4 ตัวอักษรและไม่เกิน 16 ตัวอักษร",
                EngOnly: "รูปแบบไม่ถูกต้อง (ภาษาอังกฤษเท่านั้น)"
            },
            Password: {
                required: "กรุณากรอกข้อมูล",
                minlength: "กรุณากรอกอย่างน้อย 6 ตัวอักษรและไม่เกิน 20 ตัวอักษร",
                maxlength: "กรุณากรอกอย่างน้อย 6 ตัวอักษรและไม่เกิน 20 ตัวอักษร"
            },
            CFPassword: {
                required: "กรุณากรอกข้อมูล",
                minlength: "กรุณากรอกอย่างน้อย 6 ตัวอักษรและไม่เกิน 20 ตัวอักษร",
                maxlength: "กรุณากรอกอย่างน้อย 6 ตัวอักษรและไม่เกิน 20 ตัวอักษร",
                equalTo: "กรุณากรอกข้อมูลรหัสผ่านให้ตรงกัน"
            },
            BizTypeID: {
                required: "กรุณาเลือกประเภทธุรกิจ"
            },
            CompName: {
                required: "กรุณากรอกข้อมูล",
                minlength: "กรุณากรอกอย่างน้อย 4 ตัวอักษร"
            },
            FirstName: {
                required: "กรุณากรอกข้อมูล",
                minlength: "กรุณากรอกอย่างน้อย 3 ตัวอักษร",
            },
            LastName: {
                required: "กรุณากรอกข้อมูล",
                minlength: "กรุณากรอกอย่างน้อย 3 ตัวอักษร",
            },
            Phone: {
                required: "กรุณากรอกข้อมูล",
                minlength: "กรุณากรอกอย่างน้อย 9 ตัวอักษร"
            },
            Emails: {
                required: "กรุณากรอกข้อมูล",
                EmailOnly: "รูปแบบอีเมล์ไม่ถูกต้อง"
            },
            ProvinceID: {
                required: "กรุณาเลือกจังหวัด"
            },
            DistrictID: {
                required: "กรุณาเลือกอำเภอ/เขต",
            }
        },
        highlight: function (label) {
            $(label).closest('.form-group').removeClass('has-success success');
            $(label).closest('.form-group').addClass('has-error');
        },
        success: function (label) {
            label.closest('.form-group').removeClass('has-error');
            label.closest('.form-group').addClass('has-success success');
        }
    });

    ////////////////////////////////////// Validate Sign In //////////////////////////////////////////
    $("#Form-Signin").validate({
        rules: {
            UserName: {
                required: true,
                minlength: 4,
                maxlength: 16,
                EngOnly: true
            },
            Password: {
                required: true,
                minlength: 6,
                maxlength: 20
            }
        },
        messages: {
            UserName: {
                required: "กรุณากรอกข้อมูล",
                minlength: "กรุณากรอกอย่างน้อย 4 ตัวอักษรและไม่เกิน 16 ตัวอักษร",
                maxlength: "กรุณากรอกอย่างน้อย 4 ตัวอักษรและไม่เกิน 16 ตัวอักษร",
                EngOnly: "รูปแบบไม่ถูกต้อง (ภาษาอังกฤษเท่านั้น)"
            },
            Password: {
                required: "กรุณากรอกข้อมูล",
                minlength: "กรุณากรอกอย่างน้อย 6 ตัวอักษรและไม่เกิน 20 ตัวอักษร",
                maxlength: "กรุณากรอกอย่างน้อย 6 ตัวอักษรและไม่เกิน 20 ตัวอักษร"
            }
        },
        highlight: function (label) {
            $(label).closest('.form-group').removeClass('has-success success');
            $(label).closest('.form-group').addClass('has-error');
        },
        success: function (label) {
            label.closest('.form-group').removeClass('has-error');
            label.closest('.form-group').addClass('has-success success');
        }
    });

    $("#btn-save").click(function () {
        if ($("#Form-Register").valid()) {
            console.log("True");
        }
    });
});

////////////////////////////////////// Validate Search //////////////////////////////////////////

$("#Form-Search").validate({
    rules: {
        textSearch: {
            minlength: 3
        }
    },
    messages: {
        textSearch: {
            minlength: "กรุณากรอกอย่างน้อย 3 ตัวอักษร"
        }
    },
    highlight: function (label) {
        $(label).closest('.form-group').removeClass('has-success success');
        $(label).closest('.form-group').addClass('has-error');
    },
    success: function (label) {
        label.closest('.form-group').removeClass('has-error');
        label.closest('.form-group').addClass('has-success success');
    }
});

//////////////////////////////////// Validate EditProfile ////////////////////////////////////////

$("#Form-EditProfile").validate({
    rules: {
        CompName: {
            required: true,
            minlength: 4
        },
        UserName: {
            required: true,
            minlength: 4,
            maxlength: 16,
            EngOnly: true
        },
        FirstName: {
            required: true,
            minlength: 3,
        },
        LastName: {
            required: true,
            minlength: 3,
        },
        Phone: {
            required: true,
            minlength: 9,
            PhoneOnly: true
        },
        Emails: {
            required: true,
            EmailOnly: true
        },
        ProvinceIDEdit: {
            required: true
        },
        DistrictID: {
            required: true
        }   
    },
    messages: {
        CompName: {
            required: "กรุณากรอกข้อมูล",
            minlength: "กรุณากรอกอย่างน้อย 4 ตัวอักษร"
        },
        UserName: {
            required: "กรุณากรอกข้อมูล",
            minlength: "กรุณากรอกอย่างน้อย 4 ตัวอักษรและไม่เกิน 16 ตัวอักษร",
            maxlength: "กรุณากรอกอย่างน้อย 4 ตัวอักษรและไม่เกิน 16 ตัวอักษร",
            EngOnly: "รูปแบบไม่ถูกต้อง (ภาษาอังกฤษเท่านั้น)"
        },
        FirstName: {
            required: "กรุณากรอกข้อมูล",
            minlength: "กรุณากรอกอย่างน้อย 3 ตัวอักษร",
        },
        LastName: {
            required: "กรุณากรอกข้อมูล",
            minlength: "กรุณากรอกอย่างน้อย 3 ตัวอักษร",
        },
        Phone: {
            required: "กรุณากรอกข้อมูล",
            minlength: "กรุณากรอกอย่างน้อย 9 ตัวอักษร",
            PhoneOnly: "กรุณากรอกข้อมูลให้ถูกต้อง"
        },
        Emails: {
            required: "กรุณากรอกข้อมูล",
            EmailOnly: "รูปแบบอีเมล์ไม่ถูกต้อง"
        },
        ProvinceIDEdit: {
            required: "กรุณาเลือกจังหวัด"
        },
        DistrictID: {
            required: "กรุณาเลือกอำเภอ/เขต"
        }
    },
    highlight: function (label) {
        $(label).closest('.form-group').removeClass('has-success success');
        $(label).closest('.form-group').addClass('has-error');
    },
    success: function (label) {
        label.closest('.form-group').removeClass('has-error');
        label.closest('.form-group').addClass('has-success success');
    }
});

//////////////////////////////////// Validate Change Password ////////////////////////////////////
$.validator.addMethod('CheckOldPassword',
function (value, element) {
    var result = false;
    $.ajax({
        url: GetUrl("B2BMobile/validatePassword"),
        data: {password: value},
        type: "POST",
        async: false,
        success: function (e) {
            result = e.IsResult;
        }
    });                    
    return result;
});

$("#Form-ChangePassword").validate({
    onkeydown: false,
    onkeyup: false,
    rules: {
        OldPassword: {
            required: true,
            minlength: 6,
            maxlength: 20,
            CheckOldPassword: true
        },
        NewPassword: {
            required: true,
            minlength: 6,
            maxlength: 20,
            equalTo: "#ConfirmPassword"
        },
        ConfirmPassword: {
            required: true,
            minlength: 6,
            maxlength: 20,
            equalTo: "#NewPassword"
        }
    },
    messages: {
        OldPassword: {
            required: "กรุณากรอกข้อมูล",
            minlength: "กรุณากรอกอย่างน้อย 6 ตัวอักษรและไม่เกิน 20 ตัวอักษร",
            maxlength: "กรุณากรอกอย่างน้อย 6 ตัวอักษรและไม่เกิน 20 ตัวอักษร",
            CheckOldPassword:"กรุณากรอกรหัสผ่านให้ถูกต้อง"
        },
        NewPassword: {
            required: "กรุณากรอกข้อมูล",
            minlength: "กรุณากรอกอย่างน้อย 6 ตัวอักษรและไม่เกิน 20 ตัวอักษร",
            maxlength: "กรุณากรอกอย่างน้อย 6 ตัวอักษรและไม่เกิน 20 ตัวอักษร",
            equalTo: "กรุณากรอกข้อมูลรหัสผ่านให้ตรงกัน"
        },
        ConfirmPassword: {
            required: "กรุณากรอกข้อมูล",
            minlength: "กรุณากรอกอย่างน้อย 6 ตัวอักษรและไม่เกิน 20 ตัวอักษร",
            maxlength: "กรุณากรอกอย่างน้อย 6 ตัวอักษรและไม่เกิน 20 ตัวอักษร",
            equalTo: "กรุณากรอกข้อมูลรหัสผ่านให้ตรงกัน"
        }
    },
    highlight: function (label) {
        $(label).closest('.form-group').removeClass('has-success success');
        $(label).closest('.form-group').addClass('has-error');
    },
    success: function (label) {
        label.closest('.form-group').removeClass('has-error');
        label.closest('.form-group').addClass('has-success success');
    }
});

//////////////////////////////////// Validate Forget Password ////////////////////////////////////

$("#Form-forgetPass").validate({
    rules: {
        EmailorUsername: {
            required: true,
            minlength: 4
        }
    },
    messages: {
        EmailorUsername: {
            required: "กรุณากรอกข้อมูล",
            minlength: "กรุณากรอกอย่างน้อย 4 ตัวอักษร"
        }
    },
    highlight: function (label) {
        $(label).closest('.form-group').removeClass('has-success success');
        $(label).closest('.form-group').addClass('has-error');
    },
    success: function (label) {
        label.closest('.form-group').removeClass('has-error');
        label.closest('.form-group').addClass('has-success success');
    }
});

//////////////////////////////////// Validate Update Password ////////////////////////////////////

$("#Form-UpdatePass").validate({
    onkeydown: false,
    onkeyup: false,
    rules: {
        NewUpdatePass: {
            required: true,
            minlength: 6,
            maxlength: 20,
            equalTo: "#CfUpdatePass"
        },
        CfUpdatePass: {
            required: true,
            minlength: 6,
            maxlength: 20,
            equalTo: "#NewUpdatePass"
        }
    },
    messages: {
        NewUpdatePass: {
            required: "กรุณากรอกข้อมูล",
            minlength: "กรุณากรอกอย่างน้อย 6 ตัวอักษรและไม่เกิน 20 ตัวอักษร",
            maxlength: "กรุณากรอกอย่างน้อย 6 ตัวอักษรและไม่เกิน 20 ตัวอักษร",
            equalTo: "กรุณากรอกข้อมูลรหัสผ่านให้ตรงกัน"
        },
        CfUpdatePass: {
            required: "กรุณากรอกข้อมูล",
            minlength: "กรุณากรอกอย่างน้อย 6 ตัวอักษรและไม่เกิน 20 ตัวอักษร",
            maxlength: "กรุณากรอกอย่างน้อย 6 ตัวอักษรและไม่เกิน 20 ตัวอักษร",
            equalTo: "กรุณากรอกข้อมูลรหัสผ่านให้ตรงกัน"
        }
    },
    highlight: function (label) {
        $(label).closest('.form-group').removeClass('has-success success');
        $(label).closest('.form-group').addClass('has-error');
    },
    success: function (label) {
        label.closest('.form-group').removeClass('has-error');
        label.closest('.form-group').addClass('has-success success');
    }
});

//////////////////////////////////// Validate Contact-message ////////////////////////////////////

$("#Form-Message").validate({
    rules: {
        UserName: {
            required: true
        },
        Emails: {
            required: true,
            EmailOnly: true
        },
        Phone: {
            required: "required",
            minlength: 9,
            PhoneOnly: true
        }
    },
    messages: {
        UserName: {
            required: "กรุณากรอกข้อมูล"
        },
        Emails: {
            required: "กรุณากรอกข้อมูล",
            EmailOnly: "รูปแบบอีเมล์ไม่ถูกต้อง"
        },
        Phone: {
            required: "กรุณากรอกข้อมูล",
            minlength: "กรุณากรอกอย่างน้อย 9 ตัวอักษร",
            PhoneOnly: "กรุณากรอกข้อมูลให้ถูกต้อง"
        }
    },
    highlight: function (label) {
        $(label).closest('.form-group').removeClass('has-success success');
        $(label).closest('.form-group').addClass('has-error');
    },
    success: function (label) {
        label.closest('.form-group').removeClass('has-error');
        label.closest('.form-group').addClass('has-success success');
    }
});

//////////////////////////////////// Validate Contact-reply message ////////////////////////////////////

$("#Form-replyMsg").validate({
    rules: {
        ReMsgDetail: {
            required: true
        }
    },
    messages: {
        ReMsgDetail: {
            required: "กรุณากรอกข้อความ"
        }
    },
    highlight: function (label) {
        $(label).closest('.form-group').removeClass('has-success success');
        $(label).closest('.form-group').addClass('has-error');
    },
    success: function (label) {
        label.closest('.form-group').removeClass('has-error');
        label.closest('.form-group').addClass('has-success success');
    }
});

//////////////////////////////////// Validate Contact-Requestprice ////////////////////////////////////

$("#Form-Requestprice").validate({
    rules: {
        ReqFirstName: "required",
        CompanyName: "required",
        ReqEmail:{
            required: true,
            EmailOnly: true
        },
        ReqPhone: {
            minlength: 9,
            PhoneOnly: true
        }
    },
    messages: {
        ReqFirstName: "กรุณากรอกข้อมูล",
        CompanyName: "กรุณากรอกข้อมูล",
        ReqEmail: {
            required: "กรุณากรอกข้อมูล",
            EmailOnly: "รูปแบบอีเมล์ไม่ถูกต้อง"
        },
        ReqPhone: {
            minlength: "กรุณากรอกอย่างน้อย 9 ตัวอักษร",
            PhoneOnly: "กรุณากรอกข้อมูลให้ถูกต้อง"
        }
    },
    highlight: function (label) {
        $(label).closest('.form-group').removeClass('has-success success');
        $(label).closest('.form-group').addClass('has-error');
    },
    success: function (label) {
        label.closest('.form-group').removeClass('has-error');
        label.closest('.form-group').addClass('has-success success');
    }
});

//////////////////////////////////// Validate Contact-Quotation ////////////////////////////////////

$("#Form-Quotation").validate({
    rules: {
        RequestPrice: {
            required: true,
            NumberOnly: true
        },
        DisCount: {
            required: true,
            NumberOnly: true
        },
        VAT: {
            required: true,
            NumberOnly: true
        },
        FirstName: {
            required: true,
            minlength: 3,
        },
        Emails: {
            required: true,
            EmailOnly: true
        },
        Phone: {
            minlength: 9,
            PhoneOnly: true
        }
    },
    messages: {
        RequestPrice: {
            required: "กรุณากรอกข้อมูล",
            NumberOnly: "กรุณากรอกเป็นตัวเลขเท่านั้น"
        },
        DisCount: {
            required: "กรุณากรอกข้อมูล",
            NumberOnly: "กรุณากรอกเป็นตัวเลขเท่านั้น"
        },
        VAT: {
            required: "กรุณากรอกข้อมูล",
            NumberOnly: "กรุณากรอกเป็นตัวเลขเท่านั้น"
        },
        FirstName: {
            required: "กรุณากรอกข้อมูล",
            minlength: "กรุณากรอกอย่างน้อย 3 ตัวอักษร",
        },
        Emails: {
            required: "กรุณากรอกข้อมูล",
            EmailOnly: "รูปแบบอีเมล์ไม่ถูกต้อง"
        },
        Phone: {
            minlength: "กรุณากรอกอย่างน้อย 9 ตัวอักษร",
            PhoneOnly: "กรุณากรอกข้อมูลให้ถูกต้อง"
        }
    },
    highlight: function (label) {
        $(label).closest('.form-group').removeClass('has-success success');
        $(label).closest('.form-group').addClass('has-error');
    },
    success: function (label) {
        label.closest('.form-group').removeClass('has-error');
        label.closest('.form-group').addClass('has-success success');
    }
});

