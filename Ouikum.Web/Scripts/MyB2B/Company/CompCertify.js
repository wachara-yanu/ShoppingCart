$(function () {
    $("em").remove();
    $(".t-upload-button > span").remove();
    /*------------------------------------------------*/
    $("#add").click(function () {
        $("h3#head_form").html("ADD CERTIFY");
        $('#objState').val(1);
        $("#Add_Edit").slideDown(function () {
            $("#sidebar").height($("#autoHeight").height());
            $("#main").height($("#autoHeight").height());
        });
        $(".SearchCertifyName").val("");
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

    /*----------------------enter search companycertify-------------------------*/
    //    $('.SearchCertifyName').keypress(function (e) {
    //        if (e.which == 13) {
    //            $(".btnsearch").submit();
    //        }
    //    });

    /*----------------------Ajax Submit-------------------------*/
    $("#btn_save").click(function () {
        if ($('#CompanyCertify_form').valid() && checkData()) {
            var LengthRemark = $('#Remark').val().length;
            data = {
                objState: $('#objState').val(),
                RowVersion: $('#RowVersion').val(),
                CompCertifyID: $('#CompCertifyID').val(),
                CertifyName: $('#CertifyName').val(),
                Remark: $('#Remark').val(),
                CertifyImgPath: $("#CertifyImgPath").val()
            }
            if (LengthRemark < 500) {
                OpenLoading(true);
                $.ajax({
                    url: GetUrl("Company/SaveCompanyCertify"),
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
                            window.location=GetUrl('MyB2B/Company/CompanyCertify')

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
                            window.location = GetUrl('MyB2B/Company/CompanyCertify');
                        } //end if
                        else {
                            OpenLoading(false);
                            bootbox.alert(label.vldsave_unsuccess);
                        }
                    },
                    error: function () {
                        OpenLoading(false);
                        //bootbox.alert(label.vldcannot_check_info);
                    }
                });
            }
            else {
                bootbox.alert(label.vldmessagelength);
            }
        } 
    });

    /*------------------scrollbars-----------------------------*/
    $('#content_modal').scrollbars();

    $("ul.toggle > li").each(function (index) {
        if ($(this).attr("id") == 'tab_certify') {
            $(this).addClass('active_template');
        }
        else {
            $(this).removeClass('active_template');
        }
    });
    $(".fileupload").fileupload({});
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

/*---------------------------EditCertify---------------------------------*/
function EditCertify(id) {
    $.ajax({
        url: GetUrl("Company/EditCertify"),
        data: { CompCertifyID: id },
        type: "POST",
        datatype: "json",
        traditional: true,
        success: function (data) {
            if (data != null) {
                $('#btn_save').removeAttrs('disabled');
                var urlCertify = GetUpload("CompanyCertify/" + data.CompID + "/" + data.CompCertifyID + "/" + data.CertifyImgPath + "");
                $("h3#head_form").html("EDIT CERTIFY");
                $(".SearchCertifyName").val("");
                $('#objState').val(2);
                $('#RowVersion').val(data.RowVersion);
                $('#CompCertifyID').val(data.CompCertifyID);
                $('#CertifyName').val(data.CertifyName);
                $('#Remark').val(data.Remark);
                //tinyMCE.get('Remark').setContent(data.Remark);
                $("#CertifyImgPath").val(data.CertifyImgPath);
                if (data.CertifyImgPath != null && data.CertifyImgPath != "") {
                    $("#img_CertifyImgPath").attr("src", urlCertify);
                    $("#img_CertifyImgPath").css("height", "120px");
                } else {
                    $("#img_CertifyImgPath").attr("src", "http://www.placehold.it/180x120/EFEFEF/AAAAAA&text=no+image");
                }
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

/*-----------------------validateform-----------------------------*/
$.validator.addMethod("detail", function (value, element, arg) {
    var editorContent = tinyMCE.get('detail').getContent();
    return arg != editorContent;
}, label.vldrequired);

$('#CompanyCertify_form').validate(
{
    onkeydown: false,
    onkeyup: false,
    rules: {
        CertifyName: {
            required: true,
            minlength: 4,
            maxlength: 50
        },
        CertifyImgPath: {
            required: true
        }
    },
    messages: {
        CertifyName: {
            required: label.vldrequired,
            minlength: label.vldless_4char,
            maxlength: label.vldmore_50char
        },
        CertifyImgPath: {
            required: label.vldrequired_img
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

/*------------------------close--------------------------------*/
function close() {
    $("#Add_Edit").slideUp(function () {
        $("#sidebar").height($("#autoHeight").height());
        $("#main").height($("#autoHeight").height());
    });
    $('#btn_save').attr('disabled', 'disabled');
    $('#RowVersion').val("");
    $('#CompCertifyID').val("");
    $('#CertifyName').val("");
    $('#Remark').val("");
    //tinyMCE.get('Remark').setContent("");
    $("#CertifyImgPath").val("");
    $(".SearchCertifyName").val("");
    $("#show_CertifyImgPath").html("<img id='img_CertifyImgPath' src='http://www.placehold.it/180x120/EFEFEF/AAAAAA&text=no+image' />");
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

function delCertify() {
    if ($(".cbxItem").is(':checked')) {
        if (confirm(label.vldconfirm_del_data)) { DelData('', '', 'CompCertifyID', 'Company'); }
    } else {
        bootbox.alert(label.vldnotice_del);
    }
}

/*---------------------------- Check Certify All --------------------------*/
function checkData() {
    var bool = true;
    var ImgPath = $('#CertifyImgPath').val();
    if (ImgPath == "") {
        $("#CertifyImgPath").closest('.control-group').removeClass('success');
        $("#CertifyImgPath").closest('.control-group').addClass('error');
        $('#CertifyImgPath').next().remove();
        $('#CertifyImgPath').after('<label for="CertifyImgPath" generated="true" class="error">กรุณากรอกข้อมูล</label>');
        bool = false;
    } else {
        $("#CertifyImgPath").closest('.control-group').removeClass('error');
        $("#CertifyImgPath").closest('.control-group').addClass('success');
    }
    return bool;
}