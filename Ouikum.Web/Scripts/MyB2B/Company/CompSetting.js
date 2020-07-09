/*--------------------------Edit Setting -----------------------------*/

function EditSetting(id) {
    $.ajax({
        url: GetUrl("Company/EditSetting"),
        data: { CompMenuID: id },
        type: "POST",
        datatype: "json",
        traditional: true,
        success: function (data) {
            if (data != null) {
                $('#submit').removeAttrs('disabled');
                $("h3#head_form").html("EDIT SETTING MENU");
                $('#objState').val(2);
                $('#RowVersion').val(data.RowVersion);
                $('#CompMenuID').val(data.CompMenuID);
                $('#FromMenuID').val(data.FromMenuID);
                $('#MenuName').val(data.MenuName);
                $('#Remark').val(data.Remark);
                $('#ListNo').val(data.ListNo);
                $('#LinkUrl').val(data.LinkUrl);
                $('.control-group').removeClass("success error");
                $("#Add_Edit").slideDown(function () {
                    $("#sidebar").height($("#autoHeight").height());
                    $("#main").height($("#autoHeight").height());
                });
            } // end if
            else {
                bootbox.alert(label.vldsave_unsuccess);
            }
        },
        error: function () {
            //  bootbox.alert(label.vldcannot_check_info);
        }
    });
}

function close() {
    $("#Add_Edit").slideUp(function () {
        $("#sidebar").height($("#autoHeight").height());
        $("#main").height($("#autoHeight").height());
    });
    $('#submit').attr('disabled', 'disabled');
    $('#RowVersion').val("");
    $('#CompMenuID').val("");
    $('#MenuName').val("");
    $('#Remark').val("");
    $('#LinkUrl').val("");
    $('.control-group').removeClass("success error");
    $(".error").text("");
}

function delSettingMenu() {
    if ($("cbxItem").is(':checked')) {
        if (confirm(label.vldconfirm_del_data)) { DelData('', '', 'CompMenuID', 'Company'); }
    } else {
        bootbox.alert(label.vldconfirm_del_data);
    }
}


/*------------------------------ Delete ----------------------------*/

function delSetting() {
    if ($(".cbxItem").is(':checked')) {
        if (confirm(label.vldconfirm_del_data)) { DelData('', '', 'CompMenuID', 'Company'); }
    } else {
        bootbox.alert(label.vldnotice_del);
    }
}

/*-------------------------------- validateform ---------------------------*/
$('#addSetting_form').validate(
    {
        onkeydown: false,
        onkeyup: false,
        rules: {
            MenuName: {
                required: true,
                minlength: 4,
                maxlength: 20
            },
            Remark: {
                required: true,
                minlength: 4,
                maxlength: 20
            }
        },
        messages: {
            MenuName: {
                required: label.vldrequired,
                minlength: label.vldmin_4_max_20char,
                maxlength: label.vldmin_4_max_20char
            },
            Remark: {
                required: label.vldrequired,
                minlength: label.vldmin_4_max_20char,
                maxlength: label.vldmin_4_max_20char
            }
        },
        highlight: function (label) {
            $('#submit').attr('disabled', true);
            $(label).closest('.control-group').removeClass('success');
            $(label).closest('.control-group').addClass('error');

        },
        success: function (label) {
            $('#submit').removeAttrs('disabled');
            label.closest('.control-group').removeClass('error');
            label.closest('.control-group').addClass('success');
        }
    });

/*---------------------------------- Ajax Submit -----------------------------*/
function saveSetting() {
    // var LengthRemark = $('#Remark').val().length;
    data = {
        objState: $('#objState').val(),
        RowVersion: $('#RowVersion').val(),
        CompMenuID: $('#CompMenuID').val(),
        FromMenuID: $('#FromMenuID').val(),
        MenuName: $('#MenuName').val(),
        Remark: $('#Remark').val(),
        ListNo: $('#ListNo').val(),
        LinkUrl: $('#LinkUrl').val()
    }
    //if (LengthRemark < 500) {
    if ($('#addSetting_form').valid()) {
        OpenLoading(true);
        $.ajax({
            url: GetUrl("Company/SaveSettingMenu"),
            data: data,
            type: "POST",
            traditional: true,
            success: function (data) {
                if (data != null) {
                    OpenLoading(false);
                    $("#Add_Edit").slideUp(function () {
                        $("#sidebar").height($("#autoHeight").height());
                        $("#main").height($("#autoHeight").height());
                    });
                    $(g_hidsubmit).eq(g_no).click();
                    if ($('#objState').val() == 1) {
                        if (data) {
                            bootbox.alert(label.vldsave_success);
                        } else {
                            bootbox.alert(label.vldsave_unsuccess);
                        }
                    } else {
                        if (data) {
                            bootbox.alert(label.vldedit_success);
                        } else {
                            bootbox.alert(label.vldedit_unsuccess);
                        }
                    }
                    close();
                    window.location = GetUrl('MyB2B/Company/CompanySetting');
                }
                else {
                    OpenLoading(false);
                    bootbox.alert(label.vldsave_unsuccess);
                }
            },
            error: function () {
                OpenLoading(false);
            }
        });
    }
    //}

    //else {
    //    bootbox.alert(label.vldmessagelength);
    // }
}

/*--------------------------------------------- close & cancel ----------------------------------------------*/

$(function () {

    $(".close,#cancel").click(function () {
        close();
    });

});

/*--------------------------------------------- Hide & Show -------------------------------------------------*/

function ChangeIsShow(id, obj) {
    var Icon = $(obj).find("i")
    if (Icon.hasClass("icon-eye-open")) {
        istrust = 0;
        $(obj).attr('aria-valuetext', false);
        $(obj).children().removeClass('icon-eye-open');
        $(obj).children().addClass('icon-eye-close');
    } else {
        istrust = 1;
        $(obj).attr('aria-valuetext', true);
        $(obj).children().removeClass('icon-eye-close');
        $(obj).children().addClass('icon-eye-open');
    }
    OpenLoading(true);
    $.ajax({
        url: url + "Company/ChangeIsShow",
        data: { CompMenuID: id, istrust: istrust },
        success: function (data) {
            OpenLoading(false);
            if (data) {
                bootbox.alert('success');
            } else {
                bootbox.alert('fail');
            }
        },
        type: "POST", traditional: true
    });
}

/*----------------------------------- กลับไปใช้ข้อมูลเริ่มต้น ------------------------------*/

$("#Refresh").click(function () {
    OpenLoading(true);
    $.ajax({
        url: url + "Company/refreshSet",
        success: function (data) {
            OpenLoading(false);
            if (data.IsSuccess == true) {
                bootbox.alert(data.Result, function () {
                    location.reload();
                });
            } else {
                bootbox.alert(data.Result);
            }
        },
        type: "POST", traditional: true
    });
});





