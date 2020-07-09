/*--------------------------checkValue------------------------------*/
function checkValue() {
    if ($('#User').val() != '' && $('#Pass').val() != '' && $('#WebID').val() != 0) {
        $('#next_step').removeAttrs('disabled');
    }
}
/*--------------------------checkUserEmail------------------------------*/
function checkUserEmail() {
    $.ajax({
        url: GetUrl("Member/ValidateSignUpWithAccount"),
        data: { useremail: $('#User').val(), webid: $('#WebID').val() },
        type: "POST",
        success: function (data) {
            if (!data) {
//                $("#User").closest('.control-group').removeClass('success');
//                $("#User").closest('.control-group').addClass('error');
                $(".User > .error").text(label.vldnoUseremail);
            } //end if
        },
        error: function () {
          //  bootbox.alert(label.vldcannot_check_info);
        }
    });
}
/*------------------------------login---------------------------------------*/
function login() {
    if ($('#UserName').val() != '' && $('#Password').val() != '' && $('#CompCode').val() != '') {
        OpenLoading(true);
        $.ajax({
            url: GetUrl("Member/ValidatePasswordLogin"),
            data: { username: $('#UserName').val(), password: $('#Password').val() },
            type: "POST",
            dataType: 'json',
            success: function (data) {
                if (data != null) {
                    if (data.TypeError == 2) {
                        OpenLoading(false);
                        $("#validateError").show();
                        $("#LoginError").html("<span class='ui-icon ui-icon-alert' style='float: left; margin-right: .3em; font-size:14px;'></span>ไม่พบบัญชีผู้ใช้นี้ในระบบ");
                    } else if (data.TypeError == 1) {
                        OpenLoading(false);
                        $("#validateError").show();
                        $("#LoginError").html("<span class='ui-icon ui-icon-alert' style='float: left; margin-right: .3em; font-size:14px;'></span>รหัสผ่านไม่ถูกต้อง");
                    }
                    else if (data.TypeError == 0) {
                        $.ajax({
                            url: GetUrl("Outsource/SignIn"),
                            data: { username: $('#UserName').val(), password: $('#Password').val(), remember: $('#RememberUser').val(), admincode: $('#CompCode').val() },
                            type: "POST",
                            dataType: 'json',
                            success: function (data) {
                                if (data != null) {
                                    if (data.IsSuccess == true) {
                                        window.location = data.Result;
                                    } else {
                                        if (data.Result == false) {
                                            OpenLoading(false);
                                            $("#validateError").show();
                                            $("#LoginError").html("<span class='ui-icon ui-icon-alert' style='float: left; margin-right: .3em; font-size:14px;'></span>AdminCode ไม่ถูกต้อง");
                                        } else {
                                            OpenLoading(false);
                                        }
                                    }
                                }
                            },
                            error: function () {
                                OpenLoading(false);
                                $("#validateError").show();
                                $("#LoginError").html("<span class='ui-icon ui-icon-alert' style='float: left; margin-right: .3em; font-size:14px;'></span>เกิดข้อผิดพลาด กรุณาลองใหม่อีกครั้ง");
                            }
                        });
                    }
                }
            },
            error: function () {
            }
        });

    } else {
        if ($('#UserName').val() == '') {
            $('#UserName').focus();
        }
        else if ($('#Password').val() == '') {
            $('#Password').focus();
        }
         else if ($('#CompCode').val() == '') {
             $('#CompCode').focus();
        }
    }
}
/*----------------------------------------------------------------------*/
$(function () {
    $('#myModal').modal({
        show: false
    });

    document.getElementById('login_form').onsubmit = function () {
        return false;
    };
    $('#login').click(function () {
        login();
    });
    $('#Password').keypress(function (e) {
        if (e.which == 13) {
            login();
        }
    });

    $('#login_form').validate(
             {
                 rules: {
                     UserName: {
                         required: true,
                         minlength: 4,
                         maxlength: 12
                     },
                     Password: {
                         required: true,
                         minlength: 6,
                         maxlength:20
                     },
                     CompCode: {
                         required: true
                     }
                 },
                 messages: {
                     UserName: {
                         required: label.vldrequired,
                         minlength: label.vldmin_4_max_12char,
                         maxlength: label.vldmin_4_max_12char
                     },
                     Password: {
                         required: true,
                         minlength: label.vldmin_6_max_20char,
                         maxlength: label.vldmin_6_max_20char
                     },
                     CompCode: {
                         required: label.vldrequired
                     }
                 },
                 highlight: function (label) {
//                     $(label).closest('.control-group').removeClass('success');
//                     $(label).closest('.control-group').addClass('error');

                 },
                 success: function (label) {
//                     label.closest('.control-group').removeClass('error');
//                     label.closest('.control-group').addClass('success');
                 }
             });

    $("#RememberUser").click(function () {
        if ($(this).val() == "True") {
            $(this).val("False")
            $(this).removeAttr("checked");
        } else if ($(this).val() == "False") {
            $(this).val("True")
            $(this).attr("checked", "checked");
        }
    });
});