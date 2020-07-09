$(function () {
    $("#add").click(function () {
        $("h3#head_form").html("ADD Payment");
        $("#Add_Edit").slideDown(function () {
            $("#sidebar").height($("#autoHeight").height());
            $("#main").height($("#autoHeight").height());
        });
        $('#objState').val(1);
        $(".SearchPayment").val("");
        $('.show').hide();
        $('.hide').show();
        $('.icon-ShowHide.hide').removeAttr('style');
    });

    $(".close,#btn_cancle").click(function () {
        $('.show').show();
        $('.hide').hide();
        $('.icon-ShowHide.hide').removeAttr('style');
        close();
    });

    /*----------------------Ajax Submit-------------------------*/
    $("#btn_save").click(function () {
        if ($('#addPayment_form').valid()) {
            var LengthRemark = $('#Remark').val().length;
            data = {
                objState: $('#objState').val(),
                RowVersion: $('#RowVersion').val(),
                CompPaymentID: $('#CompPaymentID').val(),
                BankID: $("#BankID").val(),
                AccName: $('#AccName').val(),
                BranchName: $('#BranchName').val(),
                AccNo: $('#AccNo').val(),
                AccType: $('#AccType').val(),
                Remark: $('#Remark').val()
            }
            if (LengthRemark < 500) {
                OpenLoading(true);
                $.ajax({
                    url: GetUrl("Company/SaveCompanyPayment"),
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
                            close();
                            window.location = GetUrl('MyB2B/Company/CompanyPayment');

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
                                    bootbox.alert(label.vldEdit_unsuccess);
                                }
                            }

                            $('.show').show();
                            $('.hide').hide();
                            $('.icon-ShowHide.hide').removeAttr('style');

                            close();
                            window.location = GetUrl('MyB2B/Company/CompanyPayment');
                        } //end if
                        else {
                            OpenLoading(false);
                            bootbox.alert(label.vldsave_unsuccess);
                        }
                    },
                    error: function () {
                        OpenLoading(false);
                        // bootbox.alert(label.vldcannot_check_info);
                    }
                });
            } else {
                bootbox.alert(label.vldmessagelength);
            }
        } else {
            bootbox.alert(label.vldrequired_complete);
        }
    });

    $('#AddPayment').modal({
        show: false
    });

    $("ul.toggle > li").each(function (index) {
        if ($(this).attr("id") == 'tab_payment') {
            $(this).addClass('active_template');
        }
        else {
            $(this).removeClass('active_template');
        }
    });

    $('.btn-tootip-bottom').tooltip({ placement: 'bottom' });
    $('.btn-tootip-top').tooltip({ placement: 'top' });
});
//#region checkbox

function CheckBoxall(Obj) {
    if (Obj.attr("value") == true || Obj.attr("checked") == "checked") {
        $(".cbxCompID").attr("checked", "checked");
        $(".cbxCompID").attr("value", "true");
        Obj.attr("checked", "checked");
        Obj.attr("value", "true");

    } else {
        $(".cbxCompID").removeAttr("checked");
        $(".cbxCompID").attr("value", "false");
        Obj.removeAttr("checked");
        Obj.attr("value", "false");
    }
}

function CheckBox(id) {
    if ($("#" + id + "").attr("value") == true || $("#" + id + "").attr("checked") == "checked") {
        if ($(".checkboxAll").attr("value") == true || $(".checkboxAll").attr("checked") == "checked") {
            $("#" + id + "").attr("checked", "checked");
            $("#" + id + "").attr("value", "true");
        } else {
            $("#" + id + "").attr("checked", "checked");
            $("#" + id + "").attr("value", "true");
        }
    } else {
        $(".cbxCompID").removeAttr("checked");
        $(".cbxCompID").attr("value", "false");
    }
}

/*-----------------------validateform-----------------------------*/
$.validator.addMethod("detail", function (value, element, arg) {
    var editorContent = tinyMCE.get('detail').getContent();
    return arg != editorContent;
}, label.vldrequired);

$('#addPayment_form').validate(
{
    rules: {
        BankID: {
            selectBank: 0
        },
        AccName: {
            required: true,
            minlength: 4
        },
        AccType: {
            selectAcctype: 0
        },
        BranchName: {
            required: true,
            minlength: 4
        },
        AccNo: {
            required: true,
            minlength: 4
        }
    },
    messages: {
        BankID: {
            selectBank: label.vldselectbank
        },
        AccName: {
            required: label.vldrequired,
            minlength: label.vldless_4char
        },
        AccType: {
            selectAcctype: label.vldselectacctype
        },
        BranchName: {
            required: label.vldrequired,
            minlength: label.vldless_4char
        },
        AccNo: {
            required: label.vldrequired,
            minlength: label.vldless_4char
        }
    },
    highlight: function (label) {
        $('#btn_save').attr('disabled', true);
        $(label).closest('.control-group').removeClass('success');
        $(label).closest('.control-group').addClass('error');

    },
    success: function (label) {
        label.closest('.control-group').removeClass('error');
        label.closest('.control-group').addClass('success');
    }
});

/*---------------------------EditPayment---------------------------------*/
function EditPayment(id) {
    $.ajax({
        url: GetUrl("Company/EditPayment"),
        data: { CompPaymentID: id },
        type: "POST",
        datatype: "json",
        traditional: true,
        success: function (data) {
            if (data != null) {
                $('#btn_save').removeAttrs('disabled');
                $("h3#head_form").html("EDIT Payment");
                $(".SearchPayment").val("");
                $('#objState').val(2);
                $('#RowVersion').val(data.RowVersion);
                $('#CompPaymentID').val(data.CompPaymentID);
                $("#BankID option[value=" + data.BankID + "]").attr("selected", "selected");
                $('#BranchName').val(data.BranchName);
                $('#AccName').val(data.AccName);
                $('#AccNo').val(data.AccNo);
                $("#AccType option[value=" + data.AccType + "]").attr("selected", "selected");
                $('#Remark').val(data.Remark);
                $('.control-group').removeClass("success error");
                $("#Add_Edit").slideDown(function () {
                    $("#sidebar").height($("#autoHeight").height());
                    $("#main").height($("#autoHeight").height());
                });
                $('.show').hide();
                $('.hide').show();
                $('.icon-ShowHide.hide').removeAttr('style');
            } //end if
            else {
                bootbox.alert(label.vldsave_unsuccess);
            }
        },
        error: function () {
            //bootbox.alert(label.vldcannot_check_info);
        }
    });
}

function close() {
    $("#Add_Edit").slideUp(function () {
        $("#sidebar").height($("#autoHeight").height());
        $("#main").height($("#autoHeight").height());
    });
    $('#btn_save').attr('disabled', 'disabled');
    $('#objState').val("");
    $('#RowVersion').val("");
    $('#CompPaymentID').val("");
    $('#AccName').val("");
    $('#BranchName').val("");
    $("#BankID option[value=0]").attr("selected", "selected");
    $('#AccNo').val("");
    $("#AccType option[value=0]").attr("selected", "selected");
    $('#Remark').val("");
    $(".SearchPayment").val("");
    $('.control-group').removeClass("success error");
    $(".error").text("");
}

function checkError() {
    if ($('.control-group').hasClass('error')) {
        $("#btn_save").attr('disabled', true);
        return false;
    } else {
        $("#btn_save").attr('disabled', false);
        return true;
    }
}

function delPayment() {
    if ($(".cbxItem").is(':checked')) {
        if (confirm('ต้องการลบข้อมูลหรือไม่')) { DelData('', '', 'CompPaymentID', 'Company'); }
    } else {
        bootbox.alert(label.vldnotice_del);
    }
}
