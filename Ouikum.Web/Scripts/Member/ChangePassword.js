$(function () {
    $("#cancel,#btn-cancel").click(function () {
        $('.show').show();
        $('.hide').hide();
        $('.icon-ShowHide.hide').removeAttr('style');
        $('.btn-file').hide();
        $('label.error').remove();
        $('div,label').removeClass('error');
    });
    $('.edit').click(function () {
        $('.show').hide();
        $('.hide').show();
        $('.icon-ShowHide.hide').removeAttr('style');
        $('.btn-file').show();
        $('#submit').css('display', 'inline-block');
        $('#cancel').css('display', 'inline-block');
        $("#content_sys").slideDown(function () {
            $("#sidebar").height($("#auroHeight").height());
            $("#main").height($("#auroHeight").height());
        });
    });
});

/*--------------------------checkPassword------------------------------*/
function checkPassword() {
    OpenLoading(true);
    $.ajax({
        url: GetUrl("Member/validatePassword"),
        data: { password: $('#OldPassword').val() },
        type: "POST",
        success: function (data) {

            if (data) {
                $("#OldPassword").closest('.control-group').removeClass('success');
                $("#OldPassword").closest('.control-group').addClass('error');
                $(".OldPassword > .error").text(label.vldpassword_incorrect);
                checkError();
                OpenLoading(false);

            } //end if
            else {
                checkError();
                OpenLoading(false); 
            }
        },
        error: function () {
           // bootbox.alert(label.vldcannot_check_info);
        }
    });
}
function checkError() {
    if ($(".control-group").hasClass("error")) {
        $("#submit").css('cursor', 'default');
        $("#submit").css('background-color', '#88B993');
        $("#submit").css('color', '#f2f2f2');
        $("#submit").attr('disabled', true);
    } else {
        if (!$(".control-group").hasClass("no_chk")) {
            $("#submit").css('cursor', 'pointer');
            $("#submit").css('background-color', '#009900');
            $("#submit").css('color', '#ffffff');
            $("#submit").attr('disabled', false);
        }
    }
}
function chkNewPassword(){
checkError();
}
function chkConfirmPassword() {
    checkError();
}
/*------------------------------------------------------------*/
$(function () {

    $("#cancel").click(function () {
        $('#OldPassword').val('');
        $('#NewPassword').val('');
        $('#ConfirmPassword').val('');
        $("#OldPassword").closest('.control-group').removeClass('success error');
        $("#NewPassword").closest('.control-group').removeClass('success error');
        $("#ConfirmPassword").closest('.control-group').removeClass('success error');
        $(".OldPassword > .error").text('');
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
    $("#submit,#btn-save").click(function () {
        if ($('#OldPassword').val() != "") {
            if ($('#NewPassword').val() != "") {
                if ($('#ConfirmPassword').val() != "") {
                    if (!$(".control-group").hasClass("error")) {
                        data = {
                            NewPassword: $('#NewPassword').val(),
                            ConfirmPassword: $('#ConfirmPassword').val(),
                            RowVersion: $('#RowVersion').val()
                        }
                        OpenLoading(true);
                        $.ajax({
                            url: GetUrl("Member/ChangePassword"),
                            data: data,
                            type: "POST",
                            success: function (data) {
                                if (data["result"]) {
                                    OpenLoading(false);
                                    bootbox.alert(label.vldchange_pass_success);
                                    // $("#RowVersion").val(data["RowVersion"]);
                                    $('#OldPassword').val('');
                                    $('#NewPassword').val('');
                                    $('#ConfirmPassword').val('');
                                    $("#OldPassword").closest('.control-group').removeClass('success error');
                                    $("#NewPassword").closest('.control-group').removeClass('success error');
                                    $("#ConfirmPassword").closest('.control-group').removeClass('success error');
                                } //end if
                                else {
                                    bootbox.alert(label.vldchange_password_error);
                                }
                            },
                            error: function () {
                                //  bootbox.alert(label.vldcannot_check_info);
                            }
                        });
                    }
                }
            }
        }

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