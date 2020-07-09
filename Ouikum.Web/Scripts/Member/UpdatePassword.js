/*--------------------------checkUserName------------------------------*/
function checkUserName() {
    $.ajax({
        url: GetUrl("Member/ValidateUserActivated"),
        data: { username: $('#username').val() },
        type: "POST",
        success: function (data) {
            if (data.username) {
                $("#username").closest('.control-group').removeClass('success');
                $("#username").closest('.control-group').addClass('error');
                $(".username > .error").text(label.vlduser_request);
            } //end if
        },
        error: function () {
          //  bootbox.alert(label.vldcannot_check_info);
        }
    });
}
/*--------------------------checkEmail------------------------------*/
function checkEmail() {
    $.ajax({
        url: GetUrl("Member/ValidateUserActivated"),
        data: { email: $('#email').val() },
        type: "POST",
        success: function (data) {
            if (data.email) {
                $("#email").closest('.control-group').removeClass('success');
                $("#email").closest('.control-group').addClass('error');
                $(".email > .error").text(label.vldemail_request);
            } //end if
        },
        error: function () {
           // bootbox.alert(label.vldcannot_check_info);
        }
    });
}

function checkError() {
    if ($(".control-group").hasClass("error")) {

        $("#submit").attr('disabled', true);
    } else {
        if (!$(".control-group").hasClass("no_chk")) {
            $("#submit").attr('disabled', false);
        }
    }
}
function chkNewPassword() {
    checkError();
}
function chkConfirmPassword() {
    checkError();
}

/*--------------------------------------------------------------------------*/
$(function () {

    $("#cancel").click(function () {
        $('#NewPassword').val('');
        $('#ConfirmPassword').val('');
        $("#NewPassword").closest('.control-group').removeClass('success error');
        $("#ConfirmPassword").closest('.control-group').removeClass('success error');
        $(".NewPassword > .error").text('');
        $(".ConfirmPassword > .error").text('');

    });

    $("ul.toggle > li").each(function (index) {
        if ($(this).attr("id") == 'tab_cPassword') {
            $(this).addClass('active_template');
        }
        else {
            $(this).removeClass('active_template');
        }
    });

    /*------------------------submit-----------------------------------*/
    $("#submit").click(function () {
        data = {
            NewPassword: $('#NewPassword').val(),
            ConfirmPassword: $('#ConfirmPassword').val(),
            member: $('#member').val()
        }
        OpenLoading(true);
        $.ajax({
            url: GetUrl("Member/UpdatePassword"),
            data: data,
            type: "POST",
            success: function (data) {
                if (data["result"]) {
                    OpenLoading(false);
                    bootbox.alert(label.vldchange_pass_success);
                    // $("#RowVersion").val(data["RowVersion"]);
                    $('#NewPassword').val('');
                    $('#ConfirmPassword').val('');
                    $("#NewPassword").closest('.control-group').removeClass('success error');
                    $("#ConfirmPassword").closest('.control-group').removeClass('success error');
                    window.location = GetUrl("Home/Index");
                } //end if
                else {
                    OpenLoading(false);
                    bootbox.alert(label.vldchange_password_error);
                }
            },
            error: function () {
                //  bootbox.alert(label.vldcannot_check_info);
            }
        });

    })

    /*-----------------------validateform-----------------------------*/

    $('#cPassword_form').validate(
         {
             onkeydown: false,
             onkeyup: false,
             rules: {
                 OldPassword: {
                     required: true
                 },
                 NewPassword: {
                     minlength: 6,
                     maxlength: 20,
                     required: true,
                     equalTo: "#ConfirmPassword"
                 },
                 ConfirmPassword: {
                     minlength: 6,
                     maxlength: 20,
                     required: true,
                     equalTo: "#NewPassword"
                 }
             },
             messages: {
                 OldPassword: {
                     required: label.vldrequired
                 },
                 NewPassword: {
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
                 }
             },
             highlight: function (label) {
                 //                     $('#submit').attr('disabled', true);
                 $(label).closest('.control-group').removeClass('success');
                 $(label).closest('.control-group').addClass('error');
                 checkError();

             },
             success: function (label) {
                 label.closest('.control-group').removeClass('error');
                 label.closest('.control-group').removeClass('no_chk');
                 label.closest('.control-group').addClass('success');
                 //                     $('#submit').removeAttrs('disabled');
                 checkError();
             }
         });
});

//$(function () {
//    alert('555555555555555555555');
//    //$('#newPassword').popover({ trigger: 'focus', content: 'กรุณากรอกรหัสผ่านใหม่', template: '<div class="popover_info"><div class="arrow"></div><div class="popover-inner"><div class="popover-content alert-info"><p></p></div></div></div>' });
//    //$('#ReNewPassword').popover({ trigger: 'focus', content: 'กรุณายืนยันรหัสผ่านใหม่', template: '<div class="popover_info"><div class="arrow"></div><div class="popover-inner"><div class="popover-content alert-info"><p></p></div></div></div>' });
//    $('#update_form').validate(
//    {
//        onkeydown: false,
//        onkeyup: false,
//        rules: {
//            newPassword: {
//                minlength: 6,
//                maxlength: 20,
//                required: true,
//                equalTo: "#ReNewPassword"
//            },
//            ReNewPassword: {
//                minlength: 6,
//                maxlength: 20,
//                required: true,
//                equalTo: "#newPassword"
//            }
//        },
//        messages: {
//            newPassword: {
//                minlength: label.vldmin_6_max_20char,
//                maxlength: label.vldmin_6_max_20char,
//                required: label.vldrequired,
//                equalTo: label.vldsame_value
//            },
//            ReNewPassword: {
//                minlength: label.vldmin_6_max_20char,
//                maxlength: label.vldmin_6_max_20char,
//                required: label.vldrequired,
//                equalTo: label.vldsame_value
//            }
//        },
//        highlight: function (label) {
//            $('#Update').attr('disabled', true);
//            $(label).closest('.control-group').removeClass('success');
//            $(label).closest('.control-group').addClass('error');

//        },
//        success: function (label) {
//            label.closest('.control-group').removeClass('error');
//            label.closest('.control-group').addClass('success');
//            $('#Update').removeAttrs('disabled');
//        }
//    });

//    $('#Update').click(function () {
//        data = {
//            NewPassword: $('#newPassword').val(),
//            ConfirmPassword: $('#ReNewPassword').val(),
//            member: $('#member').val()
//        }
//        if ($('#newPassword').val() == $('#ReNewPassword').val()) {
//            if ($('#newPassword').val() == $('#ReNewPassword').val()) {
//                OpenLoading(true);
//                $.ajax({
//                    url: GetUrl("Member/UpdatePassword"),
//                    data: data,
//                    type: "POST",
//                    success: function (data) {
//                        if (data["result"]) {
//                            OpenLoading(false);
//                            bootbox.alert(label.vldchange_pass_success);
//                            // $("#RowVersion").val(data["RowVersion"]);
//                            //$('#OldPassword').val('');
//                            //$('#NewPassword').val('');
//                            //$('#ConfirmPassword').val('');
//                            //$("#OldPassword").closest('.control-group').removeClass('success error');
//                            //$("#NewPassword").closest('.control-group').removeClass('success error');
//                            //$("#ConfirmPassword").closest('.control-group').removeClass('success error');
//                        } //end if
//                        else {
//                            bootbox.alert(label.vldchange_password_error);
//                        }
//                    },
//                    error: function () {
//                        //  bootbox.alert(label.vldcannot_check_info);
//                    }
//                })
//            } else {
//                $("span.ErrorMsg").html(label.vldpass_not_same);
//            }
//        } else {
//            if ($('#newPassword').val() == '') {
//                $('#newPassword').focus();
//            }
//            if ($('#ReNewPassword').val() == '') {
//                if ($('#newPassword').val() != '') {
//                    $('#ReNewPassword').focus();
//                }
//            }
//            $('#Update').button('reset')
//        };
//        //$('#Update').button('loading');
//        //if ($('#newPassword').val() != '' && $('#ReNewPassword').val() != '') {
//        //    if ($('#newPassword').val() == $('#ReNewPassword').val()) {
//        //        $('#Update').button('reset');
//        //        OpenLoading(true);
//        //        $.ajax({
//        //            url: GetUrl("Member/UpdatePassword"),
//        //            data: { newPW: $('#newPassword').val(), reNewPW: $('#ReNewPassword').val(), member: $('#member').val() },
//        //            type: "POST",
//        //            dataType: 'json',
//        //            success: function (data) {
//        //                OpenLoading(false);
//        //                $('#divContent').html(data);
//        //            }
//        //        });
//        //    }
//        //    else {
//        //        $("span.ErrorMsg").html(label.vldpass_not_same);
//        //    }
//        //} else {
//        //    if ($('#newPassword').val() == '') {
//        //        $('#newPassword').focus();
//        //    }
//        //    if ($('#ReNewPassword').val() == '') {
//        //        if ($('#newPassword').val() != '') {
//        //            $('#ReNewPassword').focus();
//        //        }
//        //    }
//        //    $('#Update').button('reset')
//        //}
//    });


//    /////////////////////////////////////
//    //$('#username').popover({ trigger: 'focus', content: 'กรุณากรอกชื่อผู้ใช้', template: '<div class="popover_info"><div class="arrow"></div><div class="popover-inner"><div class="popover-content alert-info"><p></p></div></div></div>' });
//    //$('#email').popover({ trigger: 'focus', content: 'กรุณากรอก Email', template: '<div class="popover_info"><div class="arrow"></div><div class="popover-inner"><div class="popover-content alert-info"><p></p></div></div></div>' });

//    $('#recover_form').validate(
//             {
//                 onkeydown: false,
//                 onkeyup: false,
//                 rules: {
//                     username: {
//                         minlength: 4,
//                         maxlength:12,
//                         required: true
//                     },
//                     email: {
//                         required: true,
//                         email: true
//                     }
//                 },
//                 messages: {
//                     username: {
//                         minlength: label.vldmin_4_max_12char,
//                         maxlength: label.vldmin_4_max_12char,
//                         required: label.vldrequired
//                     },
//                     email: {
//                         required: label.vldrequired,
//                         email: label.vldfix_format_email
//                     }
//                 },
//                 highlight: function (label) {
//                     $('#Recover').attr('disabled', true);
//                     $(label).closest('.control-group').removeClass('success');
//                     $(label).closest('.control-group').addClass('error');

//                 },
//                 success: function (label) {
//                     label.closest('.control-group').removeClass('error');
//                     label.closest('.control-group').addClass('success');
//                     $("#validateUserEmail").hide();
//                     $('#Recover').removeAttrs('disabled');
//                 }
//             });
//    $('#Recover').click(function () {
//        $('#Recover').button('loading')
//        if ($('#username').val() != '' && $('#email').val() != '') {
//            $('#Recover').button('reset')           
//            $.ajax({
//                url: GetUrl("Member/RecoverActivate"),
//                data: { username: $('#username').val(), email: $('#email').val() },
//                type: "POST",
//                //dataType: 'json',
//                success: function (data) {
//                    if (data.IsSuccess) {
//                        $('#InfoUserEmail').fadeIn();
//                    }else{
//                    $('#validateUserEmail').fadeIn();
//                    }
//                },
//                error: function () {
//                   // bootbox.alert(label.vldcannot_check_info);
//                    $('#Recover').button('reset')
//                }
//            });
//        } else {
//            if ($('#username').val() == '') {
//                $('#username').focus();
//            }
//            if ($('#email').val() == '') {
//                if ($('#username').val() != '') {
//                    $('#email').focus();
//                }
//            }
//            $('#Recover').button('reset')
//        }
//    });
//});