$(function () {
    $("#CompRegisDate").live('keypress', function () {
        return false;
    });

    $('#cancel,#cancelAll').click(function () {
        $('#edit').show();
        $('.show').show();
        $('.hide').hide();
        $('.icon-ShowHide.hide').removeAttr('style');
        $('#CompBizTypeOther').hide();
    });

    $('#edit,#editAll').click(function () {
        $(this).hide();
        $('.show').hide();
        $('.hide').show();
        $('.icon-ShowHide.hide').removeAttr('style');
        $('#edit').hide();
        $('label.error').remove();
        $('div,label').removeClass('error');
        if ($('#CompBizType').val() == 13) {
            $('#CompBizTypeOther').show();
        }
        $('.icon-calendar').css('display', 'inline-block');
        $('.checkbox').css('display', 'inline-block');
        $('#submitAll').css('display', 'inline-block');
        $('#cancelAll').css('display', 'inline-block');
    });

    $("ul.toggle > li").each(function (index) {
        if ($(this).attr("id") == 'tab_profile') {
            $(this).addClass('active_template');
        }
        else {
            $(this).removeClass('active_template');
        }
    });

    $("#CompBizType").change(function () {
        if ($(this).val() == 13) {
            $("#CompBizTypeOther").fadeIn();
        }
        else {
            $("#CompBizTypeOther").val("");
            $("#CompBizTypeOther").fadeOut();
        }
    });

    $('#dp3').datepicker();

    /*-----------------------------ajax submit------------------------------*/
    $("#submit,#submitAll").click(function () {
        data = {
            IsSameAddr: $('#IsSameAddr').val(),
            CompName: $('#CompName').val(),
            AddrLine1: $('#AddrLine1').val(),
            DistrictID: $('#DistrictID').val(),
            ProvinceID: $('#ProvinceID').val(),
            PostalCode: $('#PostalCode').val(),
            CeoName: $('#CeoName').val(),
            CompBizType: $('#CompBizType').val(),
            ComercialNo: $('#ComercialNo').val(),
            CompRegisDate: $('#CompRegisDate').val(),
            //RowVersion: $('#RowVersion').val()
        }
        OpenLoading(true);
        $.ajax({
            url: GetUrl("Company/CompanyProfile"),
            data: data,
            type: "POST",
            datatype: "json",
            traditional: true,
            success: function (data) {
                if (data["result"]) {
                    OpenLoading(false);
                    bootbox.alert(label.vldsave_success);
                    //$("#RowVersion").val(data["RowVersion"]);
                    if ($('#CompName').val() == "") {
                        $('#lblCompName').text(label.vldno_data);
                    } else {
                        $('#lblCompName').text($('#CompName').val());
                    }
                    if ($('#AddrLine1').val() == "") {
                        $('#lblAddrLine1').text(label.vldno_data);
                    } else {
                        $('#lblAddrLine1').text($('#AddrLine1').val());
                    }
                    if ($("#DistrictID").val() == 0) {
                        $('#lblDistrictID').text(label.vldno_data);
                    } else {
                        $('#lblDistrictID').text($("#DistrictID option[value=" + $("#DistrictID").val() + "]").attr("selected", "selected").text());
                    }
                    if ($("#ProvinceID").val() == 0) {
                        $('#lblProvinceID').text(label.vldno_data);
                    } else {
                        $('#lblProvinceID').text($("#ProvinceID option[value=" + $("#ProvinceID").val() + "]").attr("selected", "selected").text());
                    }
                    if ($('#PostalCode').val() == "") {
                        $('#lblPostalCode').text(label.vldno_data);
                    } else {
                        $('#lblPostalCode').text($('#PostalCode').val());
                    }
                    if ($('#CeoName').val() == "") {
                        $('#lblCeoName').text(label.vldno_data);
                    } else {
                        $('#lblCeoName').text($('#CeoName').val());
                    }
                    if ($("#CompBizType").val() == 0) {
                        $('#lblCompBizType').text(label.vldno_data);
                    } else {
                        $('#lblCompBizType').text($("#CompBizType option[value=" + $("#CompBizType").val() + "]").attr("selected", "selected").text());
                    }
                    if ($("#CompBizType").val() == 13) {
                        $('#lblCompBizType').text($('#lblCompBizType').text() + " (" + $("#CompBizTypeOther").val() + ")");
                    }
                    if ($('#ComercialNo').val() == "") {
                        $('#lblComercialNo').text(label.vldno_data);
                    } else {
                        $('#lblComercialNo').text($('#ComercialNo').val());
                    }
                    if ($('#CompRegisDate').val() == "") {
                        $('#lblCompRegisDate').text(label.vldno_data);
                    } else {
                        $('#lblCompRegisDate').text($('#CompRegisDate').val());
                    }
                    $('.show').show();
                    $('.control-group').removeClass("success error");
                    $("#CompMapDetail_parent").addClass("hide");
                    $('.hide').hide();
                    $('.icon-ShowHide.hide').removeAttr('style');
                    $('.btn-file').hide();
                    $('#edit').show();

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
    });

    /*-----------------------------------IsCompSameAddr----------------------------------------------*/
    $("#IsSameAddr").click(function () {
        var value = $(this).attr("checked");
        if (value == 'checked') {
            $("#IsSameAddr").val(true);
            $.ajax({
                url: GetUrl("Company/IsCompSameAddr"),
                type: "POST",
                dataType: 'json',
                success: function (data) {
                    if (data != null) {
                        if (data.IsResult) {
                            $("#CompName").val(data.Company.CompName).attr("readonly", true);
                            $("#lblCompName").text(data.Company.CompName);
                            $("#AddrLine1").val(data.Company.CompAddrLine1).attr("readonly", true);
                            $("#lblAddrLine1").text(data.Company.CompAddrLine1);
                            $("#PostalCode").val(data.Company.CompPostalCode).attr("readonly", true);
                            $("#lblPostalCode").text(data.Company.CompPostalCode);
                            if (data.Company.CompProvinceID != null) {
                                GetProvince(data.Company.CompProvinceID, "ProvinceID");
                                $("#ProvinceID").attr("readonly", true);
                                $("#lblProvinceID").text($("#ProvinceID option[value=" + data.Company.CompProvinceID + "]").attr("selected", "selected").text());
                            } else {
                                GetProvince(0, "ProvinceID");
                                $("#ProvinceID").attr("readonly", true);
                                $("#ProvinceID option[value=0]").attr("selected", "selected");
                                $("#lblProvinceID").text(label.vldno_data);
                            }
                            if (data.Company.CompDistrictID != null) {
                                GetDistrict(data.Company.CompDistrictID, "DistrictID");
                                $("#DistrictID").attr("readonly", true);
                                $("#lblDistrictID").text($("#DistrictID option[value=" + data.Company.CompDistrictID + "]").attr("selected", "selected").text());
                            } else {
                                GetDistrict(0, "DistrictID");
                                $("#DistrictID").attr("readonly", true);
                                $("#DistrictID option[value=0]").attr("selected", "selected");
                                $("#lblDistrictID").text(label.vldno_data);
                            }
                            if (data.Company.BizTypeID != null) {
                                GetBiztype(data.Company.BizTypeID, "CompBizType");
                                $("#CompBizType").attr("readonly", true);
                                $("#lblCompBizType").text($("#CompBizType option[value=" + data.Company.BizTypeID + "]").attr("selected", "selected").text());
                            } else {
                                $("#CompBizType option[value=0]").attr("selected", "selected");
                                $("#lblCompBizType").text(label.vldno_data);
                            }
                        }
                    }
                },
                error: function () {
                    //bootbox.alert(label.vldcannot_check_info);
                }
            });
        } else {
            $("#IsSameAddr").val(false);
            $.ajax({
                url: GetUrl("Company/IsCompProSameAddr"),
                type: "POST",
                dataType: "json",
                success: function (data) {
                    if (data != null) {
                        if (data.IsResult) {
                            $("#CompName").val(data.CompProfile["CompName"]).attr("readonly", false);
                            $("#lblCompName").text(data.CompProfile["CompName"]);
                            $("#AddrLine1").val(data.CompProfile["AddrLine1"]).attr("readonly", false);
                            $("#lblAddrLine1").text(data.CompProfile["AddrLine1"]);
                            $("#PostalCode").val(data.CompProfile["PostalCode"]).attr("readonly", false);
                            $("#lblPostalCode").text(data.CompProfile["PostalCode"]);
                            if (data.CompProfile["ProvinceID"] != null) {
                                ListProvince(data.CompProfile["ProvinceID"], "ProvinceID");
                                $("#ProvinceID").attr("readonly", false);
                                $("#lblProvinceID").text($("#ProvinceID option[value=" + data.CompProfile["ProvinceID"] + "]").attr("selected", "selected").text());
                            } else {
                                ListProvince(0, "ProvinceID");
                                $("#ProvinceID").attr("readonly", false);
                                $("#ProvinceID option[value=0]").attr("selected", "selected");
                                $("#lblProvinceID").text(label.vldno_data);
                            }
                            if (data.CompProfile["DistrictID"] != null) {
                                GetDistrictByProvince(data.CompProfile["ProvinceID"], data.CompProfile["DistrictID"], "DistrictID");
                                $("#DistrictID").attr("readonly", false);
                                $("#lblDistrictID").text($("#DistrictID option[value=" + data.CompProfile["DistrictID"] + "]").attr("selected", "selected").text());
                            } else {
                                if (data.CompProfile["ProvinceID"] != null) {
                                    GetDistrictByProvince(data.CompProfile["ProvinceID"], 0, "DistrictID");
                                } else {
                                    GetDistrictByProvince(0, 0, "DistrictID");
                                }
                                $("#DistrictID option[value=0]").attr("selected", "selected");
                                $("#DistrictID").attr("readonly", false);
                                $("#lblDistrictID").text(label.vldno_data);
                            }
                            if (data.CompProfile["CompBizType"] != null) {
                                ListBiztype(data.CompProfile["CompBizType"], "CompBizType");
                                $("#CompBizType option[value=" + data.CompProfile["CompBizType"] + "]").attr("selected", "selected");
                                $("#CompBizType").attr("readonly", false);
                                $("#lblCompBizType").text($("#CompBizType option[value=" + data.CompProfile["CompBizType"] + "]").attr("selected", "selected").text());
                            } else {
                                $("#CompBizType option[value=0]").attr("selected", "selected");
                                $("#lblCompBizType").text(label.vldno_data);
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
    $("#ProvinceID").change(function () {
        GetDistrictByProvince($(this).val(), 0, "DistrictID")
    });
});
 
/*-------------------------------checkCompName--------------------------------*/
function checkCompName() {
    $.ajax({
        url: GetUrl("Company/ValidateCompany"),
        data: { compname: $('#CompName').val() },
        type: "POST",
        success: function (data) {
            if (!data) {
                $("#CompName").closest('.control-group').removeClass('success');
                $("#CompName").closest('.control-group').addClass('error');
                $(".CompName > .error").text("Company Name ซ้ำ");
                checkError();
            } //end if
            else {
                //$("#CompName").next().remove();
                checkError();
            }
        },
        error: function () {
             //bootbox.alert(label.vldcannot_check_info);
        }
    });
}