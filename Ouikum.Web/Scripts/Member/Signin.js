/*--------------------------checkValue------------------------------*/
function checkValue() {
    if ($('#User').val() != '' && $('#Pass').val() != '' && $('#WebID').val() != 0) {
        $('#next_step').removeAttrs('disabled');
     }
}
/*--------------------------checkUserEmail------------------------------*/
function checkUserEmail() {
    if (checkEmailEng($('#User').val())) {
        $.ajax({
            url: GetUrl("Member/ValidateSignUpWithAccount"),
            data: { useremail: $('#User').val(), webid: $('#WebID').val() },
            type: "POST",
            success: function (data) {
                if (!data) {
                    // $("#User").closest('.control-group').removeClass('success');
                    $("#User").closest('.control-group').addClass('error');
                    $(".User > .error").text(label.vldnouseremail);
                    $(".User > .error").css('text-align', 'left');
                } //end if
            },
            error: function () {
                //  bootbox.alert(label.vldcannot_check_info);
            }
        });
    } else {
//        $("#User").closest('.control-group').removeClass('success');
        $("#User").closest('.control-group').addClass('error');
        $(".User > .error").text(label.vldnouseremail);
        $(".User > .error").css('text-align', 'left');
        checkError();
    }
}
/*------------------------------login---------------------------------------*/
function login() {
    if ($('#UserName').val() != '' && $('#Password').val() != '') {
        $('#UserName').focus();
        OpenLoading(true);
        $.ajax({
            url: GetUrl("Member/ValidateLogin"),
            data: { username: $('#UserName').val(), password: $('#Password').val() },
            type: "POST",
            dataType: 'json',
            success: function (data) {
                if (data != null) {
                    if (data.TypeError == 2) {
                        OpenLoading(false);
                        $("#validateError").show();
                        $("#LoginError").html("<span class='ui-icon ui-icon-alert' style='float: left; margin-right: .3em; margin-top:.1em; font-size:14px;'></span><div style='text-align:left;'>" + label.vldnouser + "</div>");
                    } else if (data.TypeError == 1) {
                        OpenLoading(false);
                        $("#validateError").show();
                        $("#LoginError").html("<span class='ui-icon ui-icon-alert' style='float: left; margin-right: .3em; margin-top:.1em; font-size:14px;'></span><div style='text-align:left;'>" + label.vldpassword_incorrect + "</div>");
                    }
                    else if (data.TypeError == 3) {
                        OpenLoading(false);
                        $("#validateError").show();
                        $("#LoginError").html("<span class='ui-icon ui-icon-alert' style='float: left; margin-right: .3em; margin-top:.1em; font-size:14px;'></span><div style='text-align:left;'>" + label.vldUser_incorrect + "</div>");
                    }
                    else if (data.TypeError == 0) {
                        $.ajax({
                            url: GetUrl("Member/SignIn"),
                            data: AddAntiForgeryToken({ username: $('#UserName').val(), password: $('#Password').val(), remember: $('#Remember').val() }),
                            type: "POST",
                            dataType: 'json',
                            success: function (data) {
                                if (data != null) {
                                    if (data.IsSuccess == true) {
                                        window.location = data.Result;
                                    } else {
                                        OpenLoading(false);
                                    }
                                }
                            },
                            error: function () {
                                OpenLoading(false);
                                $("#validateError").show();
                                $("#LoginError").html("<span class='ui-icon ui-icon-alert' style='float: left; margin-right: .3em; font-size:14px;'></span>" + label.vldcannot_check_info + "");
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
    }
}
/*----------------------------------------------------------------------*/
$(function () {
    $('#UserName').focus();
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
        $('#validateError').hide();
        if (e.which == 13) {
            login();
        }
    });

    $('#UserName').keypress(function (e) {
        $('#validateError').hide();
    });

    $('#login_form').validate(
             {
                 rules: {
                     UserName: {
                         required: true,
                         minlength: 4
                     },
                     Password: {
                         required: true,
                         minlength: 6,
                         maxlength: 20
                     }
                 },
                 messages: {
                     UserName: {
                         required: label.vldrequired,
                         minlength: label.vldless_4char
                     },
                     Password: {
                         required: label.vldrequired,
                         minlength: label.vldmin_6_max_20char,
                         maxlength: label.vldmin_6_max_20char
                     }
                 },
                 highlight: function (label) {
                     //$(label).closest('.control-group').removeClass('success');
                     $(label).closest('.control-group').addClass('error');
                     //$(label).closest('.control-group').css('text-align', 'left');

                 },
                 success: function (label) {
                     label.closest('.control-group').removeClass('error');
                     // label.closest('.control-group').addClass('success');
                 }
             });

    $("#Remember").click(function () {
        if ($(this).attr("value") == true || $(this).attr("checked") == "checked") 
        {
            $(this).attr("checked", "checked");
            $(this).attr("value", "true");
        } else {
            $(this).removeAttr("checked");
            $(this).attr("value", "false");
        }
    });
    if ($("#isRemember").val() == 'True') {
        $("#Remember").attr('checked', 'checked');
    }
    /*---------------------------next_step--------------------------*/
    //    document.getElementById('signup2_form').onsubmit = function () {
    //        return false;
    //    };
    $("#next_step").click(function () {
        if ($('#User').val() != '' && $('#Pass').val() != '' && $('#WebID').val() != 0) {
            $.ajax({
                url: GetUrl("Member/CheckAccount"),
                data: { UserName: $("#User").val(), Password: $("#Pass").val(), WebID: $("#WebID").val() },
                type: "POST",
                //dataType: 'json',
                success: function (data) {
                    if (data["comp"] == 0) {
                        $("#validate1").fadeIn();
                        $("#validate2").hide();
                    }
                    if (data["check"] != 0) {
                        $("#validate2").fadeIn();
                        $("#validate1").hide();
                    }
                },
                error: function () {
                    // bootbox.alert(label.vldcannot_check_info);
                }
            });
        } else {
            if ($('#User').val() == '') {
                $('#User').focus();
            }
            else if ($('#Pass').val() == '') {
                $('#Pass').focus();
            }
            else if ($('#WebID').val() == 0) {

                $('#WebID').focus();

            }
        }

    });


    ///*---------------------------------------------------------------------------------------------*/
    //$.validator.addMethod("WebIDRequired", function (value, element, arg) {
    //    return arg != value;
    //}, "เลือกบัญชี");

    //$('#signup2_form').validate(
    //                     {
    //                         rules: {
    //                             User: {
    //                                 required: true,
    //                                 minlength: 4
    //                             },
    //                             Pass: {
    //                                 required: true,
    //                                 minlength: 6,
    //                                 maxlength: 20
    //                             },
    //                             WebID: {
    //                                 WebIDRequired: 0
    //                             }
    //                         },
    //                         Message: {
    //                             User: {
    //                                 required: label.vldrequired,
    //                                 minlength: label.vldless_4char
    //                             },
    //                             Pass: {
    //                                 required: label.vldrequired,
    //                                 maxlength: label.vldmin_6_max_20char,
    //                                 minlength: label.vldmin_6_max_20char
    //                             },
    //                             WebID: {
    //                                 WebIDRequired: 0
    //                             }
    //                         },
    //                         highlight: function (label) {
    //                             //$(label).closest('.control-group').removeClass('success');
    //                             $(label).closest('.control-group').addClass('error');

    //                         },
    //                         success: function (label) {
    //                             label.closest('.control-group').removeClass('error');
    //                             //label.closest('.control-group').addClass('success');
    //                         }
    //                     });
});