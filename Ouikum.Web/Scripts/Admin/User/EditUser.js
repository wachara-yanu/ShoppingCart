$(function () {

    $('#submit').click(function () {
        OpenLoading(true);
        checkError();
        if ($('#Edit_form').valid()) {
            
            bootbox.alert(label.vldedit_success);
            return true;
        }
        else {
            OpenLoading(false);
            return false;
        }
    });

    $.validator.addMethod("select", function (value, element, arg) {
        return arg != value;
    });
    $.validator.addMethod("checkEmail", function (value, element, arg) {
        checkEmail();
        var Check = $("#Email").attr("title");
        return arg != Check;
    });
    $('#Edit_form').validate(
             {
                 onkeydown: false,
                 onkeyup: false,
                 rules: {
                     ServiceType: {
                         select: 0
                     },
                     CompLevel: {
                         select: 0
                     },
                     Email: {
                         required: true,
                         email: true
                     },
                     DisplayName: {
                         required: true,
                         minlength: 3
                     },
                     FirstName: {
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
                     //                     AddrLine1: {
                     //                         required: true
                     //                     },
                     DistrictID: {
                         selectDistrict: 0
                     },
                     ProvinceID: {
                         selectProvince: 0
                     },
                     //                     PostalCode: {
                     //                         required: true,
                     //                         number: true,
                     //                         minlength: 5,
                     //                         maxlength: 5
                     //                     },
                     Phone: {
                         required: true,
                         minlength: 9
                     }
                 },
                 messages: {
                     ServiceType: {
                         select: label.vldselectservicetype
                     },
                     CompLevel: {
                         select: label.vldselectcomplevel
                     },
                     Email: {
                         required: label.vldrequired,
                         email: label.vldfix_format_email
                     },
                     DisplayName: {
                         required: label.vldrequired,
                         minlength: label.vldless_3char
                     },
                     FirstName: {
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
                     //                     AddrLine1: {
                     //                         required: true
                     //                     },
                     DistrictID: {
                         selectDistrict: label.vldselectdistrict
                     },
                     ProvinceID: {
                         selectProvince: label.vldselectprovince
                     },
                     //                     PostalCode: {
                     //                         required: true,
                     //                         number: true,
                     //                         minlength: 5,
                     //                         maxlength: 5
                     //                     },
                     Phone: {
                         number: label.vldrequired,
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

    $('#edit').click(function () {
        if ($('#BizTypeID').val() == 13) {
            $('#BizTypeOther').show();
        }
        if ($("#Remark").val() != "" && $("#Remark").val() != null) {
            $("#IsCompRowflag").show();
            $('#IsCompRowflagBox').attr('checked', 'checked');
        }
        else if ($(".CompRowFlag").val() == 0 || $(".CompRowFlag").val() == 3) {
            $("#IsCompRowflag").show();
        }
        $(this).hide();
        $('.show').hide();
        $('.hide').show();
        $('.btn-file').show();
        $("#autoHeight").slideDown(function () {
            $("#sidebar").height($("#autoHeight").height());
            $("#main").height($("#autoHeight").height());
        });
        if ($('#CompLevel').val() == "2") {
            $('.exp1').hide();
            
        }
    });

    $('#cancel').click(function () {
        $('#edit').show();
        $('.show').show();
        $('.hide').hide();
        $('#BizTypeOther').hide();
        $('.btn-file').hide();
        $('label.error').remove();
        $('div,label').removeClass('error');
        $("#autoHeight").slideDown(function () {
            $("#sidebar").height($("#autoHeight").height());
            $("#main").height($("#autoHeight").height());
        });

    });

    $("#BizTypeID").change(function () {
        if ($(this).val() == 13) {
            $("#BizTypeOther").fadeIn();
            $(".BizType").addClass('mar_b20');
        }
        else {
            $("#BizTypeOther").val("");
            $("#BizTypeOther").fadeOut();
            $(".BizType").RemoveClass('mar_b20');
        }
    });

    $("#ProvinceID").change(function () {
        GetDistrictByProvince($(this).val(), 0, "DistrictID");
    });

    if ($("#CompLevel").val() == 1) {
        $('.exp').css("display", "none");
    }

    $('#CompLevel').change(function () {
        if ($(this).val() == 3) {
            $('.exp').show();
            $('.exp').css("display", "block");
            $('.Istrust').hide();
        } else if ($(this).val() == 2) {
            $('.exp2').show();
            $('.exp2').css("display", "block");
            $('.exp1').hide();
            $('.exp1').css("display", "none");
            $('.Istrust').hide();
        } else {

            $("#ExpireDate option[value=0]").attr("selected", "selected");
            $('.exp').hide();
            $('.Istrust').show();
        }
    });

    if ($('#Remark').val() != "" && $('#Remark').val() != null) {
        $('#IsCompRowflagBox').val(1);
        $('#IsCompRowflagBox').attr('checked', 'checked');
    } else {
        $('#IsCompRowflagBox').val(0);
        $('#IsCompRowflagBox').removeAttr('checked');
    }
});
/*--------------------------checkEmail------------------------------*/
function checkEmail() {
    $.ajax({
        url: GetUrl("User/Validate"),
        data: { email: $('#Email').val(), MemberID: $("#MemberID").val() },
        type: "POST",
        success: function (data) {
            if (!data) {
                $("#Email").closest('.control-group').removeClass('success');
                $("#Email").closest('.control-group').addClass('error');
                $(".Email > .error").text(label.vldemail_exists);
                checkError();

            } //end if
            else {
                checkError();
            }
        },
        error: function () {
            //bootbox.alert(label.vldcannot_check_info);
        }
    });
    checkError();
}
/*-------------------------------checkCompName--------------------------------*/
function checkCompName() {
    if (checkDisclaimer($('#CompName').val())) {
        $.ajax({
            url: GetUrl("User/Validate"),
            data: { compname: $('#CompName').val(), MemberID: $("#MemberID").val() },
            type: "POST",
            success: function (data) {
                if (!data) {
                    //OpenLoading(false);
                    $("#CompName").closest('.control-group').removeClass('success');
                    $("#CompName").closest('.control-group').addClass('error');
                    $(".CompName > .error").text(label.vldcompName_exists);
                    checkError();
                } //end if
                else {
                    checkError();
                    // OpenLoading(false);
                }
            },
            error: function () {
                //bootbox.alert(label.vldcannot_check_info);
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
}
/*--------------------------checkDisplayName------------------------------*/
function checkDisplayName() {
    if (checkDisclaimer($('#DisplayName').val())) {
        $.ajax({
            url: GetUrl("User/Validate"),
            data: { displayname: $('#DisplayName').val(), MemberID: $("#MemberID").val() },
            type: "POST",
            success: function (data) {
                if (!data) {
                    //OpenLoading(false);
                    $("#DisplayName").closest('.control-group').removeClass('success');
                    $("#DisplayName").closest('.control-group').addClass('error');
                    $(".DisplayName > .error").text(label.vlddisplayname_exists);
                    checkError();
                } //end if
                else {
                    checkError();
                    //OpenLoading(false); 
                }
            },
            error: function () {
                //bootbox.alert(label.vldcannot_check_info);
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
    if ($('.control-group').hasClass('error')) {
        $("#submit").attr('disabled', true);
        bootbox.alert('ไม่สามารถบันทึกได้');
    } else {
        $("#submit").attr('disabled', false);
    }
}
function ChangeRowFlag(obj) {
    if (obj.val() == 0) {
        $('#IsCompRowflag').show();
        $('#IsCompRowflagBox').val(0).removeAttr('checked');
        obj.addClass("text_BlackList");
        obj.removeClass("text_NonActivated text_Activated text_BlockInfo text_Expire");
        $('#divRemark').removeClass('Remark_show');
        $('#divRemark').addClass('Remark_hide');
    } else if (obj.val() == 1) {
        $('#IsCompRowflag').hide();
        $('#IsCompRowflagBox').val(0).removeAttr('checked');
        obj.addClass("text_NonActivated");
        obj.removeClass("text_BlackList text_Activated text_BlockInfo text_Expire");
        $('#divRemark').removeClass('Remark_show');
        $('#divRemark').addClass('Remark_hide');
    } else if (obj.val() == 2) {
        $('#IsCompRowflag').hide();
        $('#IsCompRowflagBox').val(0).removeAttr('checked');
        obj.addClass("text_Activated");
        obj.removeClass("text_BlackList text_NonActivated text_BlockInfo text_Expire");
        $('#divRemark').removeClass('Remark_show');
        $('#divRemark').addClass('Remark_hide');
    } else if (obj.val() == 3) {
        $('#IsCompRowflag').show();
        $('#IsCompRowflagBox').val(0).removeAttr('checked');
        obj.addClass("text_BlockInfo");
        obj.removeClass("text_BlackList text_NonActivated text_Activated text_Expire");
        $('#divRemark').removeClass('Remark_show');
        $('#divRemark').addClass('Remark_hide');
    } else if (obj.val() == 4) {
        $('#IsCompRowflag').hide();
        obj.addClass("text_Expire");
        $('#IsCompRowflagBox').val(0).removeAttr('checked');
        obj.removeClass("text_BlackList text_NonActivated text_Activated text_BlockInfo");
        $('#divRemark').removeClass('Remark_show');
        $('#divRemark').addClass('Remark_hide');
    }
}

function IsRemark(obj) {
    if (obj.attr('checked') == 'checked') {
        $('#divRemark').removeClass('Remark_hide');
        $('#divRemark').addClass('Remark_show');
        obj.val(1);
    } else  {
        $('#divRemark').removeClass('Remark_show');
        $('#divRemark').addClass('Remark_hide');
        obj.val(0);
    }
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

function CheckCompareExpire() {
    //var formatdate = $('#dp3').val().substring(6, 10) + $('#dp3').val().substring(0, 2) + $('#dp3').val().substring(3, 5);
    //$.ajax({
    //    url: GetUrl("BuyleadCenter/ValidateAddBuylead"),
    //    data: { BuyleadExpire: formatdate },
    //    type: "POST",
    //    dataType: 'json',
    //    success: function (data) {
            var newexpiredate = $('#dp3').val().substring(3, 6) + $('#dp3').val().substring(0, 3) + $('#dp3').val().substring(6, 10);
            //if (!data) {
            $("#date").val(newexpiredate);
                $("#dp3").val(newexpiredate);
            //    $("#dp3").closest('.control-group').removeClass('success');
            //    $("#dp3").closest('.control-group').addClass('error');
            //    $(".CreatedDate > .error").text(label.vldvalid_BuyleadExpire);
            //    $("#submit").attr('disabled', true);

            //    return false;
            //} else {
            //    $("#date").val($('#dp3').val());
            //    $("#dp3").val(newexpiredate);
            //    $("#dp3").closest('.control-group').addClass('success');
            //    $("#dp3").closest('.control-group').removeClass('error');
            //    $(".CreatedDate > .error").text('');
            //    $("#submit").attr('disabled', false);
            //    return true;
            //}
    //    },
    //    error: function () {
    //        bootbox.alert(label.vldcannot_check_info);
    //    }
    //});
}

function OpenLoading(isLoad, img, obj) {
    if (isLoad == true) {
        if (img == null) {
            img = '<div class="icon-loader"></div>';
        } else {
            img = '<img src=\"' + img + '\" >';
        }
        if ($('#loading').length == 0) {
            if (obj == null || obj == undefined) {
                $('body').prepend('<div id="loading">&nbsp;</div><div id="imgloading" style="top: 350px; left: 779.5px;">' + img + '</div>');
            } else {
                obj.prepend('<div id="loading">&nbsp;</div><div id="imgloading" style="top: 350px; left: 779.5px;">' + img + '</div>');
            }
            $("#loading").css({ 'filter': 'alpha(opacity=50)', 'opacity': '0.5' });
            $("#imgloading").position({ my: "center", at: "center", of: "#loading" });
        }

    } else {
        $('#loading').remove(); $('#imgloading').remove();
    }
}