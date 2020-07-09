$(function () {
    /*-----------------------------ajax submit------------------------------*/
    $("#submit,#submitAll").click(function () {
        if ($('#IsFactSameAddr').val() == "true") {
            var bool = true;
            var textError = "";
            if ($("#FactoryProvinceID").val() == "") {
                bool = false;
                textError = "จังหวัด ";
                $("#FactoryProvinceID").closest('.control-group').removeClass('success');
                $("#FactoryProvinceID").closest('.control-group').addClass('error');
                $(".control-group").find(".controls-Comp").find("#FactoryProvinceID").after($('<label for="FactoryProvinceID" generated="true" class="error">กรุณากรอกข้อมูล</label>'));
            }
            if ($("#FactoryDistrictID").val() == "") {
                bool = false;
                textError += " อำเภอ / เขต";
                $("#FactoryDistrictID").closest('.control-group').removeClass('success');
                $("#FactoryDistrictID").closest('.control-group').addClass('error');
                $(".control-group").find(".controls").find("#FactoryDistrictID").after($('<label for="FactoryDistrictID" generated="true" class="error">กรุณากรอกข้อมูล</label>'));
            }
            if (bool && checkError()) {
                data = {
                    IsFactSameAddr: $('#IsFactSameAddr').val(),
                    FactoryAddrLine1: $('#FactoryAddrLine1').val(),
                    FactoryDistrictID: $('#FactoryDistrictID').val(),
                    FactoryProvinceID: $('#FactoryProvinceID').val(),
                    FactoryPostalCode: $('#FactoryPostalCode').val(),
                    FactoryPhone: $('#FactoryPhone').val(),
                    FactoryMobile: $('#FactoryMobile').val(),
                    FactoryFax: $('#FactoryFax').val(),
                    FactorySize: $('#FactorySize').val(),
                    RESEmployeeCount: $('#RESEmployeeCount').val(),
                    QCEmployeeCount: $('#QCEmployeeCount').val(),
                    FactoryRemark: tinyMCE.get('FactoryRemark').getContent()//,
                    //RowVersion: $('#RowVersion').val()
                }
                OpenLoading(true);
                $.ajax({
                    url: GetUrl("Company/CompanyProduction"),
                    data: data,
                    type: "POST",
                    datatype: "json",
                    traditional: true,
                    success: function (data) {
                        if (data["result"]) {
                            OpenLoading(false);
                            bootbox.alert(label.vldsave_success);
                            $("#sidebar").css("height", "820px");
                            //$("#RowVersion").val(data["RowVersion"]);
                            if ($('#FactoryAddrLine1').val() == "") {
                                $('#lblFactoryAddrLine1').text(label.vldno_data);
                            } else {
                                $('#lblFactoryAddrLine1').text($('#FactoryAddrLine1').val());
                            }
                            if ($("#FactoryDistrictID").val() == 0) {
                                $('#lblFactoryDistrictID').text(label.vldno_data);
                            } else {
                                $('#lblFactoryDistrictID').text($("#FactoryDistrictID option[value=" + $("#FactoryDistrictID").val() + "]").attr("selected", "selected").text());
                            }
                            if ($("#FactoryProvinceID").val() == 0) {
                                $('#lblFactoryProvinceID').text(label.vldno_data);
                            } else {
                                $('#lblFactoryProvinceID').text($("#FactoryProvinceID option[value=" + $("#FactoryProvinceID").val() + "]").attr("selected", "selected").text());
                            }
                            if ($('#FactoryPostalCode').val() == "") {
                                $('#lblFactoryPostalCode').text(label.vldno_data);
                            } else {
                                $('#lblFactoryPostalCode').text($('#FactoryPostalCode').val());
                            }
                            if ($('#ContactEmail').val() == "") {
                                $('#lblContactEmail').text(label.vldno_data);
                            } else {
                                $('#lblContactEmail').text($('#ContactEmail').val());
                            }
                            if ($('#FactoryPhone').val() == label.vldno_data) {
                                $('#lblFactoryPhone').text();
                            } else {
                                $('#lblFactoryPhone').text($('#FactoryPhone').val());
                            }
                            if ($('#FactoryMobile').val() == "") {
                                $('#lblFactoryMobile').text(label.vldno_data);
                            } else {
                                $('#lblFactoryMobile').text($('#FactoryMobile').val());
                            }
                            if ($('#FactoryFax').val() == "") {
                                $('#lblFactoryFax').text(label.vldno_data);
                            } else {
                                $('#lblFactoryFax').text($('#FactoryFax').val());
                            }
                            if ($('#FactorySize').val() == "") {
                                $('#lblFactorySize').text(label.vldno_data);
                            } else {
                                $('#lblFactorySize').text($('#FactorySize').val());
                            }
                            if ($('#RESEmployeeCount').val() == "") {
                                $('#lblRESEmployeeCount').text(label.vldno_data);
                            } else {
                                $('#lblRESEmployeeCount').text($('#RESEmployeeCount').val() + " คน");
                            }
                            if ($('#QCEmployeeCount').val() == "") {
                                $('#lblQCEmployeeCount').text("");
                            } else {
                                $('#lblQCEmployeeCount').text($('#QCEmployeeCount').val() + " คน");
                            }
                            if (tinyMCE.get('FactoryRemark').getContent() == "") {
                                $('#lblFactoryRemark').html(label.vldno_data);
                            } else {
                                $('#lblFactoryRemark').html(tinyMCE.get('FactoryRemark').getContent());
                            }
                            $('.show').show();
                            $('.control-group').removeClass("success error");
                            $('.hide').hide();
                            $('.icon-ShowHide.hide').removeAttr('style');
                            $('.btn-file').hide();
                            $('#edit').show();

                            //close();
                            //window.location = GetUrl('MyB2B/Company/CompanyProduction');
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
            } else {
                bootbox.alert(label.vldrequired_complete);
                return false;
            }
        } else {
            if ($('#CompProduct_form').valid() && checkError()) {
                data = {
                    IsFactSameAddr: $('#IsFactSameAddr').val(),
                    FactoryAddrLine1: $('#FactoryAddrLine1').val(),
                    FactoryDistrictID: $('#FactoryDistrictID').val(),
                    FactoryProvinceID: $('#FactoryProvinceID').val(),
                    FactoryPostalCode: $('#FactoryPostalCode').val(),
                    FactoryPhone: $('#FactoryPhone').val(),
                    FactoryMobile: $('#FactoryMobile').val(),
                    FactoryFax: $('#FactoryFax').val(),
                    FactorySize: $('#FactorySize').val(),
                    RESEmployeeCount: $('#RESEmployeeCount').val(),
                    QCEmployeeCount: $('#QCEmployeeCount').val(),
                    FactoryRemark: tinyMCE.get('FactoryRemark').getContent()//,
                    //RowVersion: $('#RowVersion').val()
                }
                OpenLoading(true);
                $.ajax({
                    url: GetUrl("Company/CompanyProduction"),
                    data: data,
                    type: "POST",
                    datatype: "json",
                    traditional: true,
                    success: function (data) {
                        if (data["result"]) {
                            OpenLoading(false);
                            bootbox.alert(label.vldsave_success);
                            $("#sidebar").css("height", "820px");
                            //$("#RowVersion").val(data["RowVersion"]);
                            if ($('#FactoryAddrLine1').val() == "") {
                                $('#lblFactoryAddrLine1').text(label.vldno_data);
                            } else {
                                $('#lblFactoryAddrLine1').text($('#FactoryAddrLine1').val());
                            }
                            if ($("#FactoryDistrictID").val() == 0) {
                                $('#lblFactoryDistrictID').text(label.vldno_data);
                            } else {
                                $('#lblFactoryDistrictID').text($("#FactoryDistrictID option[value=" + $("#FactoryDistrictID").val() + "]").attr("selected", "selected").text());
                            }
                            if ($("#FactoryProvinceID").val() == 0) {
                                $('#lblFactoryProvinceID').text(label.vldno_data);
                            } else {
                                $('#lblFactoryProvinceID').text($("#FactoryProvinceID option[value=" + $("#FactoryProvinceID").val() + "]").attr("selected", "selected").text());
                            }
                            if ($('#FactoryPostalCode').val() == "") {
                                $('#lblFactoryPostalCode').text(label.vldno_data);
                            } else {
                                $('#lblFactoryPostalCode').text($('#FactoryPostalCode').val());
                            }
                            if ($('#ContactEmail').val() == "") {
                                $('#lblContactEmail').text(label.vldno_data);
                            } else {
                                $('#lblContactEmail').text($('#ContactEmail').val());
                            }
                            if ($('#FactoryPhone').val() == label.vldno_data) {
                                $('#lblFactoryPhone').text();
                            } else {
                                $('#lblFactoryPhone').text($('#FactoryPhone').val());
                            }
                            if ($('#FactoryMobile').val() == "") {
                                $('#lblFactoryMobile').text(label.vldno_data);
                            } else {
                                $('#lblFactoryMobile').text($('#FactoryMobile').val());
                            }
                            if ($('#FactoryFax').val() == "") {
                                $('#lblFactoryFax').text(label.vldno_data);
                            } else {
                                $('#lblFactoryFax').text($('#FactoryFax').val());
                            }
                            if ($('#FactorySize').val() == "") {
                                $('#lblFactorySize').text(label.vldno_data);
                            } else {
                                $('#lblFactorySize').text($('#FactorySize').val());
                            }
                            if ($('#RESEmployeeCount').val() == "") {
                                $('#lblRESEmployeeCount').text(label.vldno_data);
                            } else {
                                $('#lblRESEmployeeCount').text($('#RESEmployeeCount').val() + " คน");
                            }
                            if ($('#QCEmployeeCount').val() == "") {
                                $('#lblQCEmployeeCount').text("");
                            } else {
                                $('#lblQCEmployeeCount').text($('#QCEmployeeCount').val() + " คน");
                            }
                            if (tinyMCE.get('FactoryRemark').getContent() == "") {
                                $('#lblFactoryRemark').html(label.vldno_data);
                            } else {
                                $('#lblFactoryRemark').html(tinyMCE.get('FactoryRemark').getContent());
                            }
                            $('.show').show();
                            $('.control-group').removeClass("success error");
                            $('.hide').hide();
                            $('.icon-ShowHide.hide').removeAttr('style');
                            $('.btn-file').hide();
                            $('#edit').show();

                            //close();
                            //window.location = GetUrl('MyB2B/Company/CompanyProduction');
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
            } else {
                bootbox.alert(label.vldrequired_complete);
                return false;
            }
        }
    });

    /*-----------------------------------tinyMCE--------------------------------------------*/
    tinyMCE.init({
        mode: "textareas",
        theme: "advanced",
        width: "80%",
        height: "300",
        plugins: "emotions,spellchecker,advhr,insertdatetime,preview",

        // Theme options - button# indicated the row# only
        theme_advanced_buttons1: "newdocument,|,bold,italic,underline,|,justifyleft,justifycenter,justifyright,fontselect,fontsizeselect,formatselect",
        theme_advanced_buttons2: "cut,copy,paste,|,bullist,numlist,|,outdent,indent,|,undo,redo,forecolor,backcolor,|,link,unlink,|,insertdate,inserttime,|,code,preview",
        theme_advanced_buttons3: "",
        theme_advanced_toolbar_location: "top",
        theme_advanced_toolbar_align: "left",
        theme_advanced_statusbar_location: "bottom",
        theme_advanced_resizing: false
    });

    $('#cancel,#cancelAll').click(function () {
        $("#sidebar").css("height", "820px");
        $('#edit').show();
        $('.show').show();
        $('.hide').hide();
        $('.icon-ShowHide.hide').removeAttr('style');
        $('label.error').remove();
        $('div,label').removeClass('error');
        $("#autoHeight").slideDown(function () {
            $("#sidebar").height($("#autoHeight").height());
            $("#main").height($("#autoHeight").height());
        });
    });

    $('#edit,#editAll').click(function () {
        $("#sidebar").css("height", "1035px");
        $(this).hide();
        $('.show').hide();
        $('#edit').hide();
        $('.hide').show();
        $('.icon-ShowHide.hide').removeAttr('style');
        $('.checkbox').css('display', 'inline-block');
        $('#submitAll').css('display', 'inline-block');
        $('#cancelAll').css('display', 'inline-block');
        $("#autoHeight").slideDown(function () {
            $("#sidebar").height($("#autoHeight").height());
            $("#main").height($("#autoHeight").height());
        });
    });

    $("ul.toggle > li").each(function (index) {
        if ($(this).attr("id") == 'tab_product') {
            $(this).addClass('active_template');
        }
        else {
            $(this).removeClass('active_template');
        }
    });

    $('#detail').scrollbars();

    /*---------------------------------------------------------------------------------------------*/
    $("#IsFactSameAddr").click(function () {
        var value = $(this).attr("checked");
        if (value == 'checked') {
            $('#IsFactSameAddr').val(true);
            $.ajax({
                url: GetUrl("Company/IsCompSameAddr"),
                type: "POST",
                //                data: { IsSame: 1 },
                dataType: 'json',
                success: function (data) {
                    if (data != null) {
                        if (data.IsResult) {
                            $("#FactoryAddrLine1").val(data.Company.CompAddrLine1).attr("readonly", true);
                            $("#lblFactoryAddrLine1").text(data.Company.CompAddrLine1);
                            $("#FactoryPostalCode").val(data.Company.CompPostalCode).attr("readonly", true);
                            $("#lblFactoryPostalCode").text(data.Company.CompPostalCode);
                            $("#FactoryPhone").val(data.Company.CompPhone).attr("readonly", true);
                            $("#lblFactoryPhone").text(data.Company.CompPhone);
                            $("#FactoryMobile").val(data.Company.CompMobile).attr("readonly", true);
                            $("#lblFactoryMobile").text(data.Company.CompMobile);
                            $("#FactoryFax").val(data.Company.CompFax).attr("readonly", true);
                            $("#FactoryFax").text(data.Company.CompFax);
                            if (data.Company.CompProvinceID != null) {
                                GetProvince(data.Company.CompProvinceID, "FactoryProvinceID");
                                $("#FactoryProvinceID").attr("readonly", true);
                                $("#lblFactoryProvinceID").text($("#FactoryProvinceID option[value=" + data.Company.CompProvinceID + "]").attr("selected", "selected").text());
                            } else {
                                GetProvince(0, "FactoryProvinceID");
                                $("#FactoryProvinceID").attr("readonly", true);
                                $("#FactoryProvinceID option[value=0]").attr("selected", "selected");
                                $("#lblFactoryProvinceID").text(label.vldno_data);
                            }
                            if (data.Company.CompDistrictID != null) {
                                GetDistrict(data.Company.CompDistrictID, "FactoryDistrictID");
                                $("#FactoryDistrictID").attr("readonly", true);
                                $("#lblFactoryDistrictID").text($("#FactoryDistrictID option[value=" + data.Company.CompDistrictID + "]").attr("selected", "selected").text());
                            } else {
                                GetDistrict(0, "FactoryDistrictID");
                                $("#FactoryDistrictID").attr("readonly", true);
                                $("#FactoryDistrictID option[value=0]").attr("selected", "selected");
                                $("#lblFactoryDistrictID").text(label.vldno_data);
                            }

                            $("#FactoryProvinceID").closest('.control-group').removeClass('error');
                            $("#FactoryProvinceID").closest('.control-group').addClass('success');
                            $(".control-group").find(".FactoryProvinceID").find(".error").remove();

                            $("#FactoryDistrictID").closest('.control-group').removeClass('error');
                            $("#FactoryDistrictID").closest('.control-group').addClass('success');
                            $(".control-group").find(".FactoryDistrictID").find(".error").remove();

                            $("#FactoryPostalCode").closest('.control-group').removeClass('error');
                            $("#FactoryPostalCode").closest('.control-group').addClass('success');
                            $(".control-group").find(".FactoryPostalCode").find(".error").remove();
                        }
                    }
                },
                error: function () {
                    //bootbox.alert(label.vldcannot_check_info);
                }
            });
        } else {
            $('#IsFactSameAddr').val(false);
            $.ajax({
                url: GetUrl("Company/IsCompSameAddr"),
                type: "POST",
                dataType: 'json',
                success: function (data) {
                    if (data != null) {
                        if (data.IsResult) {
                            $("#FactoryAddrLine1").val(data.Company.FactoryctAddrLine1).attr("readonly", false);
                            $("#lblFactoryAddrLine1").text(data.Company.FactoryctAddrLine1);
                            $("#FactoryPostalCode").val(data.Company.FactoryPostalCode).attr("readonly", false);
                            $("#lblFactoryPostalCode").text(data.Company.FactoryPostalCode);
                            $("#FactoryPhone").val(data.Company.FactoryPhone).attr("readonly", false);
                            $("#lblFactoryPhone").text(data.Company.FactoryPhone);
                            $("#FactoryMobile").val(data.Company.FactoryMobile).attr("readonly", false);
                            $("#lblFactoryMobile").text(data.Company.FactoryMobile);
                            $("#FactoryFax").val(data.Company.FactoryFax).attr("readonly", false);
                            $("#FactoryCompFax").text(data.Company.FactoryFax);
                            if (data.Company.FactoryProvinceID != null) {
                                ListProvince(data.Company.FactoryProvinceID, "FactoryProvinceID");
                                $("#FactoryProvinceID").attr("readonly", false);
                                $("#lblFactoryProvinceID").text($("#FactoryProvinceID option[value=" + data.Company.FactoryProvinceID + "]").attr("selected", "selected").text());
                            } else {
                                ListProvince(0, "FactoryProvinceID");
                                $("#FactoryProvinceID").attr("readonly", false);
                                $("#FactoryProvinceID option[value=0]").attr("selected", "selected");
                                $("#lblFactoryProvinceID").text(label.vldno_data);
                            }
                            if (data.Company.FactoryDistrictID != null) {
                                GetDistrictByProvince(data.Company.FactoryProvinceID, data.Company.FactoryDistrictID, "FactoryDistrictID");
                                $("#FactoryDistrictID").attr("readonly", false);
                                $("#lblFactoryDistrictID").text($("#FactoryDistrictID option[value=" + data.Company.FactoryDistrictID + "]").attr("selected", "selected").text());
                            } else {
                                if (data.Company.FactoryProvinceID != null) {
                                    GetDistrictByProvince(data.Company.FactoryProvinceID, 0, "FactoryDistrictID");
                                } else {
                                    GetDistrictByProvince(0, 0, "FactoryDistrictID");
                                }
                                $("#FactoryDistrictID").attr("readonly", false);
                                $("#FactoryDistrictID option[value=0]").attr("selected", "selected");
                                $("#lblFactoryDistrictID").text(label.vldno_data);
                            }
                        }
                    }
                },
                error: function () {
                    //bootbox.alert(label.vldcannot_check_info);
                }
            });
        }
    });

    /*--------------------------------------ChangeProvinceID------------------------------------------------*/
    $("#FactoryProvinceID").change(function () {
        GetDistrictByProvince($(this).val(), 0, "FactoryDistrictID");
    });

    /*-----------------------------validateCompany--------------------------------*/
    $('#CompProduct_form').validate(
    {
        onkeydown: false,
        onkeyup: false,
        rules: {
            FactoryPostalCode: {
                number: true,
                minlength: 5,
                maxlength: 5
            },
            FactoryDistrictID: {
                selectDistrict: 0
            },
            FactoryProvinceID: {
                selectProvince: 0
            }
        },
        messages: {
            FactoryPostalCode: {
                number: label.vldfix_format_number,
                minlength: label.vldless_5char,
                maxlength: label.vldmore_5char
            },
            FactoryDistrictID: {
                selectDistrict: label.vldselectdistrict
            },
            FactoryProvinceID: {
                selectProvince: label.vldselectprovince
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
});

/*----------------------- Check Error ----------------------*/
function checkError() {
    if ($('.control-group').hasClass('error')) {
        $("#submit,#submitAll").attr('disabled', true);
        return false;
    } else {
        $("#submit,#submitAll").attr('disabled', false);
        return true;
    }
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