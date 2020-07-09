$(function () {

    $('#submit').click(function () {
        OpenLoading(true);
        checkUserName();
        checkEmails();
        checkCompName();
        checkDisplayName();
        checkError();
        if ($('#NewUser_Form').valid()) {
            OpenLoading(false);
            bootbox.alert(label.vldsave_success);
            return true;
        }
        else {
            OpenLoading(false);
            return false;
        }
    });

    $('#CompLevel').change(function () {
        if ($(this).val() == 3) {
            $('.exp').show();
            $("#ExpireDate").removeAttr('disabled');
            $('.Ist').hide();
            $('#IsTrust').val(0);
            $('#IsTrust').removeAttr('checked');
        } else if ($(this).val() == 1) {
            $("#ExpireDate option[value=0]").attr("selected", "selected");
            $("#ExpireDate").attr('disabled', 'disabled');
            $('.exp').hide();
            $('.Ist').show();
        } else {
            $("#ExpireDate option[value=0]").attr("selected", "selected");
            $("#ExpireDate").attr('disabled', 'disabled');
            $('.exp').hide();
            $('.Ist').hide();
            $('#IsTrust').val(0);
            $('#IsTrust').removeAttr('checked');
        }
    });

    if ($("#ProvinceID").val() != 0) {
        GetDistrictByProvince($("#ProvinceID").val(), 0, "DistrictID");
    }
    $.validator.addMethod("select", function (value, element, arg) {
        return arg != value;
    });

    $('#NewUser_Form').validate(
             {
                 onkeydown: false,
                 onkeyup: false,
                 rules: {
                     UserName: {
                         minlength: 4,
                         maxlength: 16,
                         required: true
                     },
                     Password: {
                         minlength: 6,
                         maxlength: 20,
                         required: true,
                         equalTo: "#ConfirmPassword"
                     },
                     ConfirmPassword: {
                         required: true,
                         maxlength: 20,
                         minlength: 6,
                         equalTo: "#Password"
                     },
                     Emails: {
                         required: true,
                         email: true
                     },
                     DisplayName: {
                         required: true,
                         minlength: 3
                     },
                     FirstName_register: {
                         required: true,
                         minlength: 3
                     },
                     LastName: {
                         required: true,
                         minlength: 3
                     },
                     CompName: {
                         minlength: 3,
                         maxlength: 50,
                         required: true
                     },
                     BizTypeID: {
                         selectBiztype: 0
                     },
                     AddrLine1: {
                         required: true
                     },
                     DistrictID: {
                         selectDistrict: 0
                     },
                     ProvinceID: {
                         selectProvince: 0
                     },
                     CompLevel: {
                         select: 0
                     },
                     ExpireDate: {
                         select: 0
                     },
                     ServiceType: {
                         select: 0
                     },
                     PostalCode: {
                         required: true,
                         number: true,
                         minlength: 5,
                         maxlength: 5
                     },
                     Phone: {
                         required: true,
                         minlength: 9
                     }
                 },
                 messages: {
                     UserName: {
                         minlength: label.vldmin_4_max_16char,
                         maxlength: label.vldmin_4_max_16char,
                         required: label.vldrequired
                     },
                     Password: {
                         minlength: label.vldmin_6_max_20char,
                         maxlength: label.vldmin_6_max_20char,
                         required: label.vldrequired,
                         equalTo: label.vldsame_value
                     },
                     ConfirmPassword: {
                         minlength: label.vldmin_6_max_20char,
                         maxlength: label.vldmin_6_max_20char,
                         required: label.vldrequired,
                         equalTo: label.vldsame_value
                     },
                     Emails: {
                         required: label.vldrequired,
                         email: label.vldfix_format_email
                     },
                     DisplayName: {
                         required: label.vldrequired,
                         minlength: label.vldless_3char
                     },
                     FirstName_register: {
                         required: label.vldrequired,
                         minlength: label.vldless_3char
                     },
                     LastName: {
                         required: label.vldrequired,
                         minlength: label.vldless_3char
                     },
                     CompName: {
                         required: label.vldrequired,
                         minlength: label.vldless_3char
                     },
                     BizTypeID: {
                         selectBiztype: label.vldselectbiztype
                     },
                     AddrLine1: {
                         required: label.vldrequired
                     },
                     DistrictID: {
                         selectDistrict: label.vldselectdistrict
                     },
                     ProvinceID: {
                         selectProvince: label.vldselectprovince
                     },
                     CompLevel: {
                         select: label.vldselectcomplevel
                     },
                     ExpireDate: {
                         select: label.vldrequired
                     },
                     ServiceType: {
                         select: label.vldselectservicetype
                     },
                     PostalCode: {
                         required: label.vldrequired,
                         number: label.vldfix_format_number,
                         minlength: label.vldless_5char,
                         maxlength: label.vldmore_5char
                     },
                     Phone: {
                         required: label.vldrequired,
                         minlength: label.vldless_9char
                     }
                 },
                 highlight: function (label) {
                     $(label).closest('.control-group').removeClass('success');
                     $(label).closest('.control-group').addClass('error');

                 },
                 success: function (label) {
                     label.closest('.control-group').removeClass('error');
                     label.closest('.control-group').addClass('success');
                 }

             });
    $(".MemberType").click(function () {
        if ($(this).val() == 1) {
            $("#M_ServiceType").removeClass('hide');
            $("#M_ServiceType").attr('disabled', false);
            $("#A_ServiceType").addClass('hide');
            $("#A_ServiceType").attr('disabled', true);
        } else {
            $("#M_ServiceType").addClass('hide');
            $("#M_ServiceType").attr('disabled', true);
            $("#A_ServiceType").removeClass('hide');
            $("#A_ServiceType").attr('disabled', false);
        }
    });

    $("#ProvinceID").change(function () {
        GetDistrictByProvince($(this).val(), 0, "DistrictID");
    });

    $("#BizTypeID").change(function () {
        if ($(this).val() == 13) {
            $("#BizTypeOther").fadeIn();
            $("#BizTypeOther").addClass('mar_tb20');
        }
        else {
            $("#BizTypeOther").val("");
            $("#BizTypeOther").fadeOut();
            $("#BizTypeOther").removeClass('mar_tb20');
        }
    });
});
/*--------------------------checkUserName------------------------------*/
function checkUserName() {
    var result;
    if (checkEng($('#UserName').val())) {
        $.ajax({
            url: GetUrl("Member/ValidateRegister"),
            data: { username: $('#UserName').val() },
            type: "POST",
            async: false,
            success: function (data) {
                if (!data) {
                    //OpenLoading(false);
                    $("#UserName").closest('.control-group').removeClass('success');
                    $("#UserName").closest('.control-group').addClass('error');
                    $(".UserName > .error").text(label.vldusername_exists);
                    $(".UserName > .error").css('text-align', 'left');
                    checkError();
                    result = false;
                } //end if
                else {
                    checkError();
                    result = true;
                    //OpenLoading(false); 
                }
            },
            error: function () {
                // bootbox.alert(label.cannot_check_info);
            }
        });
    } else {
        $("#UserName").closest('.control-group').removeClass('success');
        $("#UserName").closest('.control-group').addClass('error');
        $(".UserName > .error").text(label.vldengonly);
        $(".UserName > .error").css('text-align', 'left');
        checkError();
        result = false;
    }
    return result;
}
/*--------------------------checkEmail------------------------------*/
function checkEmails() {
    var result;
    if (checkEmailEng($('#Emails').val())) {
        $.ajax({
            url: GetUrl("Member/ValidateRegister"),
            data: { email: $('#Emails').val() },
            type: "POST",
            async: false,
            success: function (data) {
                if (!data) {
                    //OpenLoading(false);
                    $("#Emails").closest('.control-group').removeClass('success');
                    $("#Emails").closest('.control-group').addClass('error');
                    $(".Emails > .error").text(label.vldemail_exists);
                    $(".Emails > .error").css('text-align', 'left');
                    checkError();
                    result = false;
                } //end if
                else {
                    checkError();
                    result = true;
                    //OpenLoading(false);
                }
            },
            error: function () {
                // bootbox.alert(label.cannot_check_info);
            }
        });
    } else {
        $("#Emails").closest('.control-group').removeClass('success');
        $("#Emails").closest('.control-group').addClass('error');
        $(".Emails > .error").text(label.vldengonly);
        $(".Emails > .error").css('text-align', 'left');
        checkError();
        result = false;
    }
    return result;
}
/*-------------------------------checkCompName--------------------------------*/
function checkCompName() {
    var result;
    if (checkDisclaimer($('#CompName').val())) {
        $.ajax({
            url: GetUrl("Member/ValidateRegister"),
            data: { compname: $('#CompName').val() },
            type: "POST",
            async: false,
            success: function (data) {
                if (!data) {
                    //OpenLoading(false);
                    $("#CompName").closest('.control-group').removeClass('success');
                    $("#CompName").closest('.control-group').addClass('error');
                    $(".CompName > .error").text(label.vldcompname_exists);
                    $(".CompName > .error").css('text-align', 'left');
                    checkError();
                    result = false;
                } //end if
                else {
                    checkError();
                    result = true;
                    // OpenLoading(false);
                }
            },
            error: function () {
                //  bootbox.alert(label.cannot_check_info);
            }
        });
    } else {
        $("#CompName").closest('.control-group').removeClass('success');
        $("#CompName").closest('.control-group').addClass('error');
        $(".CompName > .error").text("กรุณากรอกข้อมูลให้ถูกต้อง ไม่ควรมีเครื่องหมาย \!@#$^฿|{}[]'\"<>=:;~");
        $(".CompName > .error").css('text-align', 'left');
        checkError();
        result = false;
    }
    return result;
}
/*--------------------------checkDisplayName------------------------------*/
function checkDisplayName() {
    var result;
    if (checkDisclaimer($('#DisplayName').val())) {
        $.ajax({
            url: GetUrl("Member/ValidateRegister"),
            data: { displayname: $('#DisplayName').val() },
            type: "POST",
            async: false,
            success: function (data) {
                if (!data) {
                    //OpenLoading(false);
                    $("#DisplayName").closest('.control-group').removeClass('success');
                    $("#DisplayName").closest('.control-group').addClass('error');
                    $(".DisplayName > .error").text(label.vlddisplayname_exists);
                    $(".DisplayName > .error").css('text-align', 'left');
                    checkError();
                    result = false;
                } //end if
                else {
                    checkError();
                    result = true;
                    //OpenLoading(false); 
                }
            },
            error: function () {
                //  bootbox.alert(label.cannot_check_info);
            }
        });
    } else {
        $("#DisplayName").closest('.control-group').removeClass('success');
        $("#DisplayName").closest('.control-group').addClass('error');
        $(".DisplayName > .error").text("กรุณากรอกข้อมูลให้ถูกต้อง ไม่ควรมีเครื่องหมาย \!@#$^฿|{}[]'\"<>=:;~");
        $(".DisplayName > .error").css('text-align', 'left');
        checkError();
        result = false;
    }
    return result;
}
/*--------------------------checkBizTypeOther------------------------------*/
function checkBizTypeOther() {
    var result;
    $.ajax({
        url: GetUrl("Member/ValidateRegister"),
        data: { BizTypeOther: $('#BizTypeOther').val() },
        type: "POST",
        async: false,
        success: function (data) {
            if (!data) {
                //OpenLoading(false);
                $("#BizTypeOther").closest('.control-group').removeClass('success');
                $("#BizTypeOther").closest('.control-group').addClass('error');
                $(".BizTypeOther > .error").text(label.vldbiztypeother_exists);
                $(".BizTypeOther > .error").css('text-align', 'left');
                checkError();
                result = false;
            } //end if
            else {
                checkError();
                result = true;
                //OpenLoading(false); 
            }
        },
        error: function () {
            //  bootbox.alert(label.cannot_check_info);
        }
    });
    return result;
}
/*--------------------------checkError------------------------------*/
function checkError() {
    var result;
    var check = "";
    check = $("label.error").text();
    if (check != "") {
        $("#submit").attr('disabled', true);
        result = false;
    } else {
        $("#submit").attr('disabled', false);
        result = true;
    }
    return result;
}
function istrust() {
    if ($('#IsTrust').val() == 0) {
        $('#IsTrust').attr("checked", "checked");
        $('#IsTrust').attr("value", "1");

    } else {
        $('#IsTrust').removeAttr("checked");
        $('#IsTrust').attr("value", "0");
    }
}

function issme() {
    if ($('#IsSME').val() == 0) {
        $('#IsSME').attr("checked", "checked");
        $('#IsSME').attr("value", "1");

    } else {
        $('#IsSME').removeAttr("checked");
        $('#IsSME').attr("value", "0");
    }
}