$(function () {
    $('#ShipmentDuration').bind('keypress', function (e) {
        if (this.value.match(/[^a-zA-Z0-9 ]/g)) {
            bootbox.alert('กรุณากรอกตัวเลขให้ถูกต้อง');
            $('#ShipmentDuration').val(0);
        }
        return (e.which != 8 && e.which != 0 && (e.which > 57)) ? false : true;
    });

    $("#add").click(function () {
        $("h3#head_form").html("ADD Shipment");
        $('#objState').val(1);
        $("#Add_Edit").slideDown(function () {
            $("#sidebar").height($("#autoHeight").height());
            $("#main").height($("#autoHeight").height());
        });
        $(".SearchShipmentName").val("");
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
        if ($('#addShipment_form').valid()) {
            var LengthRemark = $('#Remark').val().length;
            data = {
                objState: $('#objState').val(),
                RowVersion: $('#RowVersion').val(),
                CompShipmentID: $('#CompShipmentID').val(),
                ShipmentName: $('#ShipmentName').val(),
                PackingName: $('#PackingName').val(),
                ShipmentDuration: $('#ShipmentDuration').val(),
                Remark: $('#Remark').val()
            }
            if (LengthRemark < 500) {
                OpenLoading(true);
                $.ajax({
                    url: GetUrl("Company/SaveCompanyShipment"),
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
                                    bootbox.alert(label.vldEdit_unsuccess);
                                }
                            }

                            $('.show').show();
                            $('.hide').hide();
                            $('.icon-ShowHide.hide').removeAttr('style');

                            close();
                            window.location = GetUrl('MyB2B/Company/CompanyShipment');
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
    /*----------------------enter search companyshiment-------------------------*/
    //    $('.SearchShipmentName').keypress(function (e) {
    //        if (e.which == 13) {
    //            $(".btnsearch").submit();
    //        }
    //    });

    $('#AddShipmentment').modal({
        show: false
    });

    $("ul.toggle > li").each(function (index) {
        if ($(this).attr("id") == 'tab_shipment') {
            $(this).addClass('active_template');
        }
        else {
            $(this).removeClass('active_template');
        }
    });

    $('.btn-tootip-bottom').tooltip({ placement: 'bottom' });
    $('.btn-tootip-top').tooltip({ placement: 'top' });
});

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
    var editorContent = tinyMCE.get('ShipmentDuration').getContent();
    return arg != editorContent;
}, label.vldrequired);

$('#addShipment_form').validate(
    {
        rules: {
            ShipmentName: {
                required: true,
                minlength: 4,
                maxlength: 50
            },
            PackingName: {
                required: true,
                minlength: 4,
                maxlength: 50
            },
            dateShipment: {
                required: true,
                number: true
            },
            ShipmentDuration: {
                required: true
            }
        },
        messages: {
            ShipmentName: {
                required: label.vldrequired,
                minlength: label.vldless_4char,
                maxlength: "กรุณากรอกไม่เกิน 50 ตัวอักษร"
            },
            PackingName: {
                required: label.vldrequired,
                minlength: label.vldless_4char,
                maxlength: "กรุณากรอกไม่เกิน 50 ตัวอักษร"
            },
            dateShipment: {
                required: label.vldrequired,
                number: label.vldfix_format_number
            },
            ShipmentDuration: {
                required: label.vldrequired
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
            $('#btn_save').removeAttrs('disabled');
        }
    });

/*---------------------------EditShipment---------------------------------*/
function EditShipment(id) {
    //var CompCertifyID = $("#CompCertify_"+id).val();
    $.ajax({
        url: GetUrl("Company/EditShipment"),
        data: { CompShipmentID: id },
        type: "POST",
        datatype: "json",
        traditional: true,
        success: function (data) {
            if (data != null) {
                $('#submit').removeAttrs('disabled');
                $("h3#head_form").html("EDIT Shipment");
                $(".SearchShipmentName").val("");
                $('#objState').val(2);
                $('#RowVersion').val(data.RowVersion);
                $('#CompShipmentID').val(data.CompShipmentID);
                $('#ShipmentName').val(data.ShipmentName);
                $('#PackingName').val(data.PackingName);
                $('#ShipmentDuration').val(data.ShipmentDuration);
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
            //  bootbox.alert(label.vldcannot_check_info);
        }
    });
}

function close() {
    $("#Add_Edit").slideUp(function () {
        $("#sidebar").height($("#autoHeight").height());
        $("#main").height($("#autoHeight").height());
    });
    $('#btn_save').attr('disabled', 'disabled');
    $('#RowVersion').val("");
    $('#CompShimentID').val("");
    $('#ShipmentName').val("");
    $('#PackingName').val("");
    $('#ShipmentDuration').val("");
    $('#Remark').val("");
    $(".SearchShipmentName").val("");
    $('.control-group').removeClass("success error");
    $(".error").text("");
}

function delShipment() {
    if ($(".cbxItem").is(':checked')) {
        if (confirm(label.vldconfirm_del_data)) { DelData('', '', 'CompShipmentID', 'Company'); }
    } else {
        bootbox.alert(label.vldnotice_del);
    }
}

/*---------------------- Check Error ---------------------------*/
function checkError() {
    if ($('.control-group').hasClass('error')) {
        $("#btn_save").attr('disabled', true);
        return false;
    } else {
        $("#btn_save").attr('disabled', false);
        return true;
    }
}