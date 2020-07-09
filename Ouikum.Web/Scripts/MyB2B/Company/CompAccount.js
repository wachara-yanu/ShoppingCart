$(function () {
    $("em").remove();
    $(".t-upload-button > span").remove();

    $("#BizTypeID").change(function () {
        if ($(this).val() == 13) {
            $(".BizOther").fadeIn();
        }
        else {
            $(".BizOther").fadeOut();
            $("#BizTypeOther").val("");
        }
    });

    /*-----------------------------ajax submit------------------------------*/
    $("#submitAll").click(function () {
        if ($('#IsCompSameAddr').val() == "true") {
            var bool = true;
            var textError = "";
            if ($("#DisplayName").val() == "") {
                bool = false;
                textError = "ชื่อที่ใช้แสดงในระบบ ";
                $("#DisplayName").closest('.control-group').removeClass('success');
                $("#DisplayName").closest('.control-group').addClass('error');
                $(".control-group").find(".controls-Comp").find("#DisplayName").after($('<label for="DisplayName" generated="true" class="error">กรุณากรอกข้อมูล</label>'));
            }
            if ($("#CompName").val() == "") {
                bool = false;
                textError += " ชื่อบริษัท";
                $("#CompName").closest('.control-group').removeClass('success');
                $("#CompName").closest('.control-group').addClass('error');
                $(".control-group").find(".controls").find("#CompName").after($('<label for="CompName" generated="true" class="error">กรุณากรอกข้อมูล</label>'));
            }
            if (bool && checkError()) {
                var ServiceType = 0;
                var CompImgPath = "";
                for (var i = 1; i <= 14; i++) {
                    if ($('#ServiceType' + i).attr("checked") == 'checked') {
                        ServiceType = $('#ServiceType' + i).val();
                    }
                }
                for (var i = 1; i <= 3; i++) {
                    if ($('#CompImgPath_' + i).val() != "") {
                        CompImgPath = CompImgPath + $('#CompImgPath_' + i).val() + ",";
                    }
                }
                CompImgPath = CompImgPath.substring(0, CompImgPath.length - 1);
                data = {
                    IsCompSameAddr: $('#IsCompSameAddr').val(),
                    ServiceType: ServiceType,
                    DisplayName: $("#DisplayName").val(),
                    LogoImgPath: $("#LogoImgPath").val(),
                    CompShortDes: tinyMCE.get('CompShortDes').getContent(),
                    MainCustomer: $('#MainCustomer').val(),
                    SecondaryCustomer: $('#SecondaryCustomer').val(),
                    CompName: $('#CompName').val(),
                    CompNameEng: $('#CompNameEng').val(),
                    CompImgPath: CompImgPath,
                    CompAddrLine1: $('#CompAddrLine1').val(),
                    CompDistrictID: $('#CompDistrictID').val(),
                    CompProvinceID: $('#CompProvinceID').val(),
                    CompPostalCode: $('#CompPostalCode').val(),
                    CompPhone: $('#CompPhone').val(),
                    CompMobile: $('#CompMobile').val(),
                    CompFax: $('#CompFax').val(),
                    BizTypeID: $('#BizTypeID').val(),
                    BizTypeOther: $('#BizTypeOther').val(),
                    CompWebsiteUrl: $('#CompWebsiteUrl').val(),
                    FacebookUrl: $('#FacebookUrl').val(),
                    LineID: $('#LineID').val(),
                    Type: 0
                }
                OpenLoading(true);
                $.ajax({
                    url: GetUrl("Company/CompanyAccount"),
                    data: data,
                    type: "POST",
                    datatype: "json",
                    traditional: true,
                    success: function (data) {
                        if (data["result"]) {
                            OpenLoading(false);
                            bootbox.alert(label.vldsave_success);
                            $("#sidebar").css("height", "1839px");
                            // $("#RowVersion").val(data["RowVersion"]);
                            if ($('.ServiceType:checked').val() == 1) {
                                $('#lblServiceType').text(label.vldbuyer);
                            }
                            if ($('.ServiceType:checked').val() == 2) {
                                $('#lblServiceType').text(label.vldSeller_Buyer);
                            }
                            if ($('.ServiceType:checked').val() == 3) {
                                $('#lblServiceType').text(label.vldSeller);
                            }
                            if ($('#DisplayName').val() == "") {
                                $('#lblDisplayName').text(label.vldno_data);
                            } else {
                                $('#lblDisplayName').text($('#DisplayName').val());
                            }
                            if (tinyMCE.get('CompShortDes').getContent() == "") {
                                $('#lblCompShortDes').html("-");
                            } else {
                                $('#lblCompShortDes').html(tinyMCE.get('CompShortDes').getContent());
                            }
                            if ($('#MainCustomer').val() == "") {
                                $('#lblMainCustomer').text(label.vldno_data);
                            } else {
                                $('#lblMainCustomer').text($('#MainCustomer').val());
                            }
                            if ($('#SecondaryCustomer').val() == "") {
                                $('#lblSecondaryCustomer').text(label.vldno_data);
                            } else {
                                $('#lblSecondaryCustomer').text($('#SecondaryCustomer').val());
                            }
                            if ($('#CompName').val() == "") {
                                $('#lblCompName').text(label.vldno_data);
                            } else {
                                $('#lblCompName').text($('#CompName').val());
                            }
                            if ($('#CompNameEng').val() == "") {
                                $('#lblCompNameEng').text(label.vldno_data);
                            } else {
                                $('#lblCompNameEng').text($('#CompNameEng').val());
                            }
                            if ($('#CompAddrLine1').val() == "") {
                                $('#lblCompAddrLine1').text(label.vldno_data);
                            } else {
                                $('#lblCompAddrLine1').text($('#CompAddrLine1').val());
                            }
                            if ($("#CompDistrictID").val() == 0) {
                                $('#lblCompDistrictID').text("");
                            } else {
                                $('#lblCompDistrictID').text($("#CompDistrictID option[value=" + $("#CompDistrictID").val() + "]").attr("selected", "selected").text());
                            }
                            if ($("#CompProvinceID").val() == 0) {
                                $('#lblCompProvinceID').text(label.vldno_data);
                            } else {
                                $('#lblCompProvinceID').text($("#CompProvinceID option[value=" + $("#CompProvinceID").val() + "]").attr("selected", "selected").text());
                            }
                            if ($('#CompPostalCode').val() == "") {
                                $('#lblCompPostalCode').text(label.vldno_data);
                            } else {
                                $('#lblCompPostalCode').text($('#CompPostalCode').val());
                            }
                            if ($('#CompPhone').val() == "") {
                                $('#lblCompPhone').text(label.vldno_data);
                            } else {
                                $('#lblCompPhone').text($('#CompPhone').val());
                            }
                            if ($('#CompMobile').val() == "") {
                                $('#lblCompMobile').text(label.vldno_data);
                            } else {
                                $('#lblCompMobile').text($('#CompMobile').val());
                            }
                            if ($('#CompFax').val() == "") {
                                $('#lblCompFax').text(label.vldno_data);
                            } else {
                                $('#lblCompFax').text($('#CompFax').val());
                            }
                            if ($('#BizTypeID').val() == 0) {
                                $('#lblBizTypeID').text(label.vldno_data);
                            } else {
                                $('#lblBizTypeID').text($("#BizTypeID option[value=" + $("#BizTypeID").val() + "]").attr("selected", "selected").text());
                            }
                            if ($("#BizTypeID").val() == 13) { $('#lblBizTypeID').append(' (' + $('#BizTypeOther').val() + ')'); }
                            if ($('#CompWebsiteUrl').val() == "") {
                                $('#lblCompWebsiteUrl').text(label.vldno_data);
                            } else {
                                $('#lblCompWebsiteUrl').text($('#CompWebsiteUrl').val());
                            }

                            $('#lblFacebookUrl').text($('#FacebookUrl').val());

                            $('#lblLineID').text($('#LineID').val());


                            $('.show').show();
                            $('.control-group').removeClass("success error");
                            $('.hide').hide();
                            $('.icon-ShowHide.hide').removeAttr('style');
                            $('.btn-file').hide();
                            $('.i-close').hide();
                            $('#edit1').show();
                            $('#edit2').show();
                            $('#edit3').show();
                            $('#edit4').show();
                            $('#editAll').show();

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
                bootbox.alert("กรุณากรอกข้อมูล " + textError);
                return false;
            }
        } else {
            if ($('#CompAcc_form').valid() && checkError()) {
                var ServiceType = 0;
                var CompImgPath = "";
                for (var i = 1; i <= 14; i++) {
                    if ($('#ServiceType' + i).attr("checked") == 'checked') {
                        ServiceType = $('#ServiceType' + i).val();
                    }
                }
                for (var i = 1; i <= 3; i++) {
                    if ($('#CompImgPath_' + i).val() != "") {
                        CompImgPath = CompImgPath + $('#CompImgPath_' + i).val() + ",";
                    }
                }
                CompImgPath = CompImgPath.substring(0, CompImgPath.length - 1);
                data = {
                    IsCompSameAddr: $('#IsCompSameAddr').val(),
                    ServiceType: ServiceType,
                    DisplayName: $("#DisplayName").val(),
                    LogoImgPath: $("#LogoImgPath").val(),
                    CompShortDes: tinyMCE.get('CompShortDes').getContent(),
                    MainCustomer: $('#MainCustomer').val(),
                    SecondaryCustomer: $('#SecondaryCustomer').val(),
                    CompName: $('#CompName').val(),
                    CompNameEng: $('#CompNameEng').val(),
                    CompImgPath: CompImgPath,
                    CompAddrLine1: $('#CompAddrLine1').val(),
                    CompDistrictID: $('#CompDistrictID').val(),
                    CompProvinceID: $('#CompProvinceID').val(),
                    CompPostalCode: $('#CompPostalCode').val(),
                    CompPhone: $('#CompPhone').val(),
                    CompMobile: $('#CompMobile').val(),
                    CompFax: $('#CompFax').val(),
                    BizTypeID: $('#BizTypeID').val(),
                    BizTypeOther: $('#BizTypeOther').val(),
                    CompWebsiteUrl: $('#CompWebsiteUrl').val(),
                    FacebookUrl: $('#FacebookUrl').val(),
                    LineID: $('#LineID').val(),
                    Type: 0
                }
                OpenLoading(true);
                $.ajax({
                    url: GetUrl("Company/CompanyAccount"),
                    data: data,
                    type: "POST",
                    datatype: "json",
                    traditional: true,
                    success: function (data) {
                        if (data["result"]) {
                            OpenLoading(false);
                            bootbox.alert(label.vldsave_success);
                            $("#sidebar").css("height", "1839px");
                            // $("#RowVersion").val(data["RowVersion"]);
                            if ($('.ServiceType:checked').val() == 1) {
                                $('#lblServiceType').text(label.vldbuyer);
                            }
                            if ($('.ServiceType:checked').val() == 2) {
                                $('#lblServiceType').text(label.vldSeller_Buyer);
                            }
                            if ($('.ServiceType:checked').val() == 3) {
                                $('#lblServiceType').text(label.vldSeller);
                            }
                            if ($('#DisplayName').val() == "") {
                                $('#lblDisplayName').text(label.vldno_data);
                            } else {
                                $('#lblDisplayName').text($('#DisplayName').val());
                            }
                            if (tinyMCE.get('CompShortDes').getContent() == "") {
                                $('#lblCompShortDes').html("-");
                            } else {
                                $('#lblCompShortDes').html(tinyMCE.get('CompShortDes').getContent());
                            }
                            if ($('#MainCustomer').val() == "") {
                                $('#lblMainCustomer').text(label.vldno_data);
                            } else {
                                $('#lblMainCustomer').text($('#MainCustomer').val());
                            }
                            if ($('#SecondaryCustomer').val() == "") {
                                $('#lblSecondaryCustomer').text(label.vldno_data);
                            } else {
                                $('#lblSecondaryCustomer').text($('#SecondaryCustomer').val());
                            }
                            if ($('#CompName').val() == "") {
                                $('#lblCompName').text(label.vldno_data);
                            } else {
                                $('#lblCompName').text($('#CompName').val());
                            }
                            if ($('#CompNameEng').val() == "") {
                                $('#lblCompNameEng').text(label.vldno_data);
                            } else {
                                $('#lblCompNameEng').text($('#CompNameEng').val());
                            }
                            if ($('#CompAddrLine1').val() == "") {
                                $('#lblCompAddrLine1').text(label.vldno_data);
                            } else {
                                $('#lblCompAddrLine1').text($('#CompAddrLine1').val());
                            }
                            if ($("#CompDistrictID").val() == 0) {
                                $('#lblCompDistrictID').text("");
                            } else {
                                $('#lblCompDistrictID').text($("#CompDistrictID option[value=" + $("#CompDistrictID").val() + "]").attr("selected", "selected").text());
                            }
                            if ($("#CompProvinceID").val() == 0) {
                                $('#lblCompProvinceID').text(label.vldno_data);
                            } else {
                                $('#lblCompProvinceID').text($("#CompProvinceID option[value=" + $("#CompProvinceID").val() + "]").attr("selected", "selected").text());
                            }
                            if ($('#CompPostalCode').val() == "") {
                                $('#lblCompPostalCode').text(label.vldno_data);
                            } else {
                                $('#lblCompPostalCode').text($('#CompPostalCode').val());
                            }
                            if ($('#CompPhone').val() == "") {
                                $('#lblCompPhone').text(label.vldno_data);
                            } else {
                                $('#lblCompPhone').text($('#CompPhone').val());
                            }
                            if ($('#CompMobile').val() == "") {
                                $('#lblCompMobile').text(label.vldno_data);
                            } else {
                                $('#lblCompMobile').text($('#CompMobile').val());
                            }
                            if ($('#CompFax').val() == "") {
                                $('#lblCompFax').text(label.vldno_data);
                            } else {
                                $('#lblCompFax').text($('#CompFax').val());
                            }
                            if ($('#BizTypeID').val() == 0) {
                                $('#lblBizTypeID').text(label.vldno_data);
                            } else {
                                $('#lblBizTypeID').text($("#BizTypeID option[value=" + $("#BizTypeID").val() + "]").attr("selected", "selected").text());
                            }
                            if ($("#BizTypeID").val() == 13) { $('#lblBizTypeID').append(' (' + $('#BizTypeOther').val() + ')'); }
                            if ($('#CompWebsiteUrl').val() == "") {
                                $('#lblCompWebsiteUrl').text(label.vldno_data);
                            } else {
                                $('#lblCompWebsiteUrl').text($('#CompWebsiteUrl').val());
                            }

                            $('#lblFacebookUrl').text($('#FacebookUrl').val());

                            $('#lblLineID').text($('#LineID').val());


                            $('.show').show();
                            $('.control-group').removeClass("success error");
                            $('.hide').hide();
                            $('.icon-ShowHide.hide').removeAttr('style');
                            $('.btn-file').hide();
                            $('.i-close').hide();
                            $('#edit1').show();
                            $('#edit2').show();
                            $('#edit3').show();
                            $('#edit4').show();
                            $('#editAll').show();

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
                bootbox.alert(label.vldrequired_complete);
                return false;
            }
        }
    });

    /*-----------------------------------tinyMCE--------------------------------------------*/
    tinyMCE.init({
        mode: "textareas",
        theme: "advanced",
        width: '80%',
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

    $(".toggle_acc").click(function () {
        $(this).children().toggle();
        $(this).next().slideToggle(function () {
            $("#sidebar").height($("#auroHeight").height());
            $("#main").height($("#auroHeight").height());
        });
    });

    $('#cancelAll').click(function () {
        window.location = GetUrl("MyB2B/Company/CompanyAccount");
        //$('#editAll').show();
        //$('#edit1').show();
        //$('#edit2').show();
        //$('#edit3').show();
        //$('#edit4').show();
        //$('.show').show();
        //$('.hide').hide();
        //$('.icon-ShowHide.hide').removeAttr('style');
        //$('#BizTypeOther').hide();
        //$('label.error').remove();
        //$('div,label').removeClass('error');
        //$('.btn-file').hide();
    });

    $('#editAll').click(function () {
        $("#sidebar").css("height", "2106px");

        var IsCompSameAddr = $('#IsCompSameAddr').val();
        if (IsCompSameAddr == "true") {
            $('#IsCompSameAddr').val(true);
            $("#CompAddrLine1").attr("readonly", true);
            $("#CompPostalCode").attr("readonly", true);
            $("#CompPhone").attr("readonly", true);
            $("#CompMobile").attr("readonly", true);
            $("#CompFax").attr("readonly", true);
            $("#CompProvinceID").attr("readonly", true);
            $("#CompDistrictID").attr("readonly", true);
            $("#CompDistrictID").attr("readonly", true);
        }

        if ($('#BizTypeID').val() == 13) {
            $('#BizTypeOther').show();
        }
        $(".content_toggle").slideDown(function () {
            $("#sidebar").height($("#auroHeight").height());
            $("#main").height($("#auroHeight").height());
        });
        $(this).hide();
        $('.show').hide();
        $('.hide').show();
        $('.icon-ShowHide.hide').removeAttr('style');
        $('#edit1').hide();
        $('#edit2').hide();
        $('#edit3').hide();
        $('#edit4').hide();
        $('.hideTop').hide();
        $('.hideMiddle1').hide();
        $('.hideMiddle2').hide();
        $('.hideButtom').hide();
        $('.lineShow').hide();
        $('.lineHide').show();
        $('.btn-file').show();
        $('.checkbox').css('display', 'inline-block');
        $('#Comp_detail_parent').addClass('w_100');
        $('#submitAll').css('display', 'inline-block');
        $('#cancelAll').css('display', 'inline-block');
    });

    /*-------------------------------------------btn edit&save (ตัวที่1)---------------------------------------------------*/
    $('#edit1').click(function () {
        if ($('#BizTypeID').val() == 13) {
            $('#BizTypeOther').show();
        }
        $("#content_sys").slideDown(function () {
            $("#sidebar").height($("#auroHeight").height());
            $("#main").height($("#auroHeight").height());
        });
        $(this).hide();
        $('.show1').hide();
        $('.hide1').show();
        $('.hideTop').show();
        $('#edit1').hide();
        $('#editAll').hide();
        $('#Comp_detail_parent').addClass('w_100');
    });

    $('#btn-cancel1').click(function () {
        $('#editAll').show();
        $('.hideTop').hide();
        $('#edit1').show();
        $('.show1').show();
        $('.hide1').hide();
        $('#BizTypeOther').hide();
        $('label.error').remove();
        $('div,label').removeClass('error');

        $('.btn-file').hide();
    });

    $("#btn-save1").click(function () {
        if ($('#CompAcc_form').valid() && checkError()) {
            var ServiceType = 0;
            for (var i = 1; i <= 14; i++) {
                if ($('#ServiceType' + i).attr("checked") == 'checked') {
                    ServiceType = $('#ServiceType' + i).val();
                }
            }
            data = {
                ServiceType: ServiceType,
                DisplayName: $("#DisplayName").val(),
                Type : 1
            }
            OpenLoading(true);
            $.ajax({
                url: GetUrl("Company/CompanyAccount"),
                data: data,
                type: "POST",
                datatype: "json",
                traditional: true,
                success: function (data) {
                    console.log(data);

                    if (data["result"]) {
                        OpenLoading(false);
                        bootbox.alert(label.vldsave_success);
                        // $("#RowVersion").val(data["RowVersion"]);
                        if ($('.ServiceType:checked').val() == 1) {
                            $('#lblServiceType').text(label.vldbuyer);
                        }
                        if ($('.ServiceType:checked').val() == 2) {
                            $('#lblServiceType').text(label.vldSeller_Buyer);
                        }
                        if ($('.ServiceType:checked').val() == 3) {
                            $('#lblServiceType').text(label.vldSeller);
                        }
                        if ($('#DisplayName').val() == "") {
                            $('#lblDisplayName').text(label.vldno_data);
                        } else {
                            $('#lblDisplayName').text($('#DisplayName').val());
                        }

                        $('.show1').show();
                        $('.control-group').removeClass("success error");
                        $('.hide1').hide();
                        $('.hideTop').hide();
                        $('#edit1').show();
                        $('#editAll').show();

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
            bootbox.alert(label.vldrequired_complete);
            return false;
        }
    });

    /*--------------------------------------btn edit&save (ตัวที่2)-------------------------------------------------*/
    $('#edit2').click(function () {
        $("#sidebar").css("height", "2007px");
        if ($('#BizTypeID').val() == 13) {
            $('#BizTypeOther').show();
        }
        $("#content_sys2").slideDown(function () {
            $("#sidebar").height($("#auroHeight").height());
            $("#main").height($("#auroHeight").height());
        });
        $(this).hide();
        $('.show2').hide();
        $('.hide2').show();
        $('.hideMiddle1').show();
        $('#edit2').hide();
        $('#editAll').hide();
        $('#Comp_detail_parent').addClass('w_100');
    });

    $('#btn-cancel2').click(function () {
        window.location = GetUrl("MyB2B/Company/CompanyAccount");
        //$('#editAll').show();
        //$('.hideMiddle1').hide();
        //$('#edit2').show();
        //$('.show2').show();
        //$('.hide2').hide();
        //$('#BizTypeOther').hide();
        //$('label.error').remove();
        //$('div,label').removeClass('error');

        //$('.btn-file').hide();
    });

    $("#btn-save2").click(function () {
        if ($('#CompAcc_form').valid() && checkError()) {
            var CompImgPath = "";
            
            for (var i = 1; i <= 3; i++) {
                if ($('#CompImgPath_' + i).val() != "") {
                    CompImgPath = CompImgPath + $('#CompImgPath_' + i).val() + ",";
                }
            }
            CompImgPath = CompImgPath.substring(0, CompImgPath.length - 1);
            data = {
                CompShortDes: tinyMCE.get('CompShortDes').getContent(),
                CompImgPath: CompImgPath,
                Type: 2
            }
            OpenLoading(true);
            $.ajax({
                url: GetUrl("Company/CompanyAccount"),
                data: data,
                type: "POST",
                datatype: "json",
                traditional: true,
                success: function (data) {
                    console.log(data);

                    if (data["result"]) {
                        OpenLoading(false);
                        bootbox.alert(label.vldsave_success);
                        $("#sidebar").css("height", "1839px");
                        // $("#RowVersion").val(data["RowVersion"]);
                        if (tinyMCE.get('CompShortDes').getContent() == "") {
                            $('#lblCompShortDes').html("-");
                        } else {
                            $('#lblCompShortDes').html(tinyMCE.get('CompShortDes').getContent());
                        }
                        $('.show2').show();
                        $('.control-group').removeClass("success error");
                        $('.hide2').hide();
                        $('.hideMiddle1').hide();
                        $('#edit2').show();
                        $('#editAll').show();

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
            bootbox.alert(label.vldrequired_complete);
            return false;
        }
    });

    /*--------------------------------------btn edit&save (ตัวที่3)------------------------------------------------*/
    $('#edit3').click(function () {
        if ($('#BizTypeID').val() == 13) {
            $('#BizTypeOther').show();
        }
        $("#content_sys3").slideDown(function () {
            $("#sidebar").height($("#auroHeight").height());
            $("#main").height($("#auroHeight").height());
        });
        $(this).hide();
        $('.show3').hide();
        $('.hide3').show();
        $('.hideMiddle2').show();
        $('#edit3').hide();
        $('#editAll').hide();
        $('#Comp_detail_parent').addClass('w_100');
    });

    $('#btn-cancel3').click(function () {
        $('#editAll').show();
        $('.hideMiddle2').hide();
        $('#edit3').show();
        $('.show3').show();
        $('.hide3').hide();
        $('#BizTypeOther').hide();
        $('label.error').remove();
        $('div,label').removeClass('error');
        $('.btn-file').hide();
    });

    $("#btn-save3").click(function () {
        if ($('#CompAcc_form').valid() && checkError()) {
            data = {
                MainCustomer: $('#MainCustomer').val(),
                SecondaryCustomer: $('#SecondaryCustomer').val(),
                Type:3
            }
            OpenLoading(true);
            $.ajax({
                url: GetUrl("Company/CompanyAccount"),
                data: data,
                type: "POST",
                datatype: "json",
                traditional: true,
                success: function (data) {
                    console.log(data);

                    if (data["result"]) {
                        OpenLoading(false);
                        bootbox.alert(label.vldsave_success);
                        // $("#RowVersion").val(data["RowVersion"]);

                        if ($('#MainCustomer').val() == "") {
                            $('#lblMainCustomer').text(label.vldno_data);
                        } else {
                            $('#lblMainCustomer').text($('#MainCustomer').val());
                        }
                        if ($('#SecondaryCustomer').val() == "") {
                            $('#lblSecondaryCustomer').text(label.vldno_data);
                        } else {
                            $('#lblSecondaryCustomer').text($('#SecondaryCustomer').val());
                        }
                        $('.show3').show();
                        $('.control-group').removeClass("success error");
                        $('.hide3').hide();
                        $('.hideMiddle2').hide();
                        $('#edit3').show();
                        $('#editAll').show();

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
            bootbox.alert(label.vldrequired_complete);
            return false;

        }
    });

    /*------------------------------------btn edit&save (ตัวที่4)----------------------------------------------*/
    $('#edit4').click(function () {
        $("#sidebar").css("height", "1867px");
        var IsCompSameAddr = $('#IsCompSameAddr').val();
        if (IsCompSameAddr == "true") {
            $('#IsCompSameAddr').val(true);
            $("#CompAddrLine1").attr("readonly", true);
            $("#CompPostalCode").attr("readonly", true);
            $("#CompPhone").attr("readonly", true);
            $("#CompMobile").attr("readonly", true);
            $("#CompFax").attr("readonly", true);
            $("#CompProvinceID").attr("readonly", true);
            $("#CompDistrictID").attr("readonly", true);
            $("#CompDistrictID").attr("readonly", true);
        }

        if ($('#BizTypeID').val() == 13) {
            $('#BizTypeOther').show();
        }
        $("#content_sys4").slideDown(function () {
            $("#sidebar").height($("#auroHeight").height());
            $("#main").height($("#auroHeight").height());
        });
        $(this).hide();
        $('.icon-ShowHide.hide').removeAttr('style');
        $('.show4').hide();
        $('.hide4').show();
        $('.hideButtom').show();
        $('#edit4').hide();
        $('#editAll').hide();
        $('.checkbox').css('display', 'inline-block');
        $('#Comp_detail_parent').addClass('w_100');
    });

    $('#btn-cancel4').click(function () {
        window.location = GetUrl("MyB2B/Company/CompanyAccount");
        //$('#editAll').show();
        //$('.hideButtom').hide();
        //$('#edit4').show();
        //$('.show4').show();
        //$('.hide4').hide();
        //$('.checkbox').hide();
        //$('#BizTypeOther').hide();
        //$('.icon-ShowHide.hide').removeAttr('style');
        //$('label.error').remove();
        //$('div,label').removeClass('error');
        //$('.btn-file').hide();
    });


    $("#btn-save4").click(function () {
        if ($('#IsCompSameAddr').val() == "true") {
            var bool = true;
            var textError = "";
            if ($("#CompName").val() == "") {
                bool = false;
                textError += " ชื่อบริษัท";
                $("#CompName").closest('.control-group').removeClass('success');
                $("#CompName").closest('.control-group').addClass('error');
                $(".control-group").find(".controls").find("#CompName").after($('<label for="CompName" generated="true" class="error">กรุณากรอกข้อมูล</label>'));
            }
            if (bool && checkError()) {
                var ServiceType = 0;
                var CompImgPath = "";
                for (var i = 1; i <= 14; i++) {
                    if ($('#ServiceType' + i).attr("checked") == 'checked') {
                        ServiceType = $('#ServiceType' + i).val();
                    }
                }
                for (var i = 1; i <= 3; i++) {
                    if ($('#CompImgPath_' + i).val() != "") {
                        CompImgPath = CompImgPath + $('#CompImgPath_' + i).val() + ",";
                    }
                }
                CompImgPath = CompImgPath.substring(0, CompImgPath.length - 1);
                data = {
                    IsCompSameAddr: $('#IsCompSameAddr').val(),
                    LogoImgPath: $("#LogoImgPath").val(),
                    CompName: $('#CompName').val(),
                    CompNameEng: $('#CompNameEng').val(),
                    CompAddrLine1: $('#CompAddrLine1').val(),
                    CompDistrictID: $('#CompDistrictID').val(),
                    CompProvinceID: $('#CompProvinceID').val(),
                    CompPostalCode: $('#CompPostalCode').val(),
                    CompPhone: $('#CompPhone').val(),
                    CompMobile: $('#CompMobile').val(),
                    CompFax: $('#CompFax').val(),
                    BizTypeID: $('#BizTypeID').val(),
                    BizTypeOther: $('#BizTypeOther').val(),
                    CompWebsiteUrl: $('#CompWebsiteUrl').val(),
                    FacebookUrl: $('#FacebookUrl').val(),
                    LineID: $('#LineID').val(),
                    Type: 4
                }
                OpenLoading(true);
                $.ajax({
                    url: GetUrl("Company/CompanyAccount"),
                    data: data,
                    type: "POST",
                    datatype: "json",
                    traditional: true,
                    success: function (data) {
                        console.log(data);

                        if (data["result"]) {
                            OpenLoading(false);
                            bootbox.alert(label.vldsave_success);
                            $("#sidebar").css("height", "1839px");
                            // $("#RowVersion").val(data["RowVersion"]);
                            if ($('#CompName').val() == "") {
                                $('#lblCompName').text(label.vldno_data);
                            } else {
                                $('#lblCompName').text($('#CompName').val());
                            }
                            if ($('#CompNameEng').val() == "") {
                                $('#lblCompNameEng').text(label.vldno_data);
                            } else {
                                $('#lblCompNameEng').text($('#CompNameEng').val());
                            }
                            if ($('#CompAddrLine1').val() == "") {
                                $('#lblCompAddrLine1').text(label.vldno_data);
                            } else {
                                $('#lblCompAddrLine1').text($('#CompAddrLine1').val());
                            }
                            if ($("#CompDistrictID").val() == 0) {
                                $('#lblCompDistrictID').text("");
                            } else {
                                $('#lblCompDistrictID').text($("#CompDistrictID option[value=" + $("#CompDistrictID").val() + "]").attr("selected", "selected").text());
                            }
                            if ($("#CompProvinceID").val() == 0) {
                                $('#lblCompProvinceID').text(label.vldno_data);
                            } else {
                                $('#lblCompProvinceID').text($("#CompProvinceID option[value=" + $("#CompProvinceID").val() + "]").attr("selected", "selected").text());
                            }
                            if ($('#CompPostalCode').val() == "") {
                                $('#lblCompPostalCode').text(label.vldno_data);
                            } else {
                                $('#lblCompPostalCode').text($('#CompPostalCode').val());
                            }
                            if ($('#CompPhone').val() == "") {
                                $('#lblCompPhone').text(label.vldno_data);
                            } else {
                                $('#lblCompPhone').text($('#CompPhone').val());
                            }
                            if ($('#CompMobile').val() == "") {
                                $('#lblCompMobile').text(label.vldno_data);
                            } else {
                                $('#lblCompMobile').text($('#CompMobile').val());
                            }
                            if ($('#CompFax').val() == "") {
                                $('#lblCompFax').text(label.vldno_data);
                            } else {
                                $('#lblCompFax').text($('#CompFax').val());
                            }
                            if ($('#BizTypeID').val() == 0) {
                                $('#lblBizTypeID').text(label.vldno_data);
                            } else {
                                $('#lblBizTypeID').text($("#BizTypeID option[value=" + $("#BizTypeID").val() + "]").attr("selected", "selected").text());
                            }
                            if ($("#BizTypeID").val() == 13) { $('#lblBizTypeID').append(' (' + $('#BizTypeOther').val() + ')'); }
                            if ($('#CompWebsiteUrl').val() == "") {
                                $('#lblCompWebsiteUrl').text(label.vldno_data);
                            } else {
                                $('#lblCompWebsiteUrl').text($('#CompWebsiteUrl').val());
                            }

                            $('#lblFacebookUrl').text($('#FacebookUrl').val());

                            $('#lblLineID').text($('#LineID').val());


                            $('.show4').show();
                            $('.control-group').removeClass("success error");
                            $('.hide4').hide();
                            $('.hideButtom').hide();
                            $('.checkbox').hide();
                            $('#edit4').show();
                            $('#editAll').show();

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
                bootbox.alert("กรุณากรอกข้อมูล " + textError);
                return false;
            }
        } else {
            if ($('#CompAcc_form').valid() && checkError()) {
                var ServiceType = 0;
                var CompImgPath = "";
                for (var i = 1; i <= 14; i++) {
                    if ($('#ServiceType' + i).attr("checked") == 'checked') {
                        ServiceType = $('#ServiceType' + i).val();
                    }
                }
                for (var i = 1; i <= 3; i++) {
                    if ($('#CompImgPath_' + i).val() != "") {
                        CompImgPath = CompImgPath + $('#CompImgPath_' + i).val() + ",";
                    }
                }
                CompImgPath = CompImgPath.substring(0, CompImgPath.length - 1);
                data = {
                    IsCompSameAddr: $('#IsCompSameAddr').val(),
                    LogoImgPath: $("#LogoImgPath").val(),
                    CompName: $('#CompName').val(),
                    CompNameEng: $('#CompNameEng').val(),
                    CompAddrLine1: $('#CompAddrLine1').val(),
                    CompDistrictID: $('#CompDistrictID').val(),
                    CompProvinceID: $('#CompProvinceID').val(),
                    CompPostalCode: $('#CompPostalCode').val(),
                    CompPhone: $('#CompPhone').val(),
                    CompMobile: $('#CompMobile').val(),
                    CompFax: $('#CompFax').val(),
                    BizTypeID: $('#BizTypeID').val(),
                    BizTypeOther: $('#BizTypeOther').val(),
                    CompWebsiteUrl: $('#CompWebsiteUrl').val(),
                    FacebookUrl: $('#FacebookUrl').val(),
                    LineID: $('#LineID').val(),
                    Type: 4
                }
                OpenLoading(true);
                $.ajax({
                    url: GetUrl("Company/CompanyAccount"),
                    data: data,
                    type: "POST",
                    datatype: "json",
                    traditional: true,
                    success: function (data) {
                        console.log(data);

                        if (data["result"]) {
                            OpenLoading(false);
                            bootbox.alert(label.vldsave_success);
                            // $("#RowVersion").val(data["RowVersion"]);
                            if ($('#CompName').val() == "") {
                                $('#lblCompName').text(label.vldno_data);
                            } else {
                                $('#lblCompName').text($('#CompName').val());
                            }
                            if ($('#CompNameEng').val() == "") {
                                $('#lblCompNameEng').text(label.vldno_data);
                            } else {
                                $('#lblCompNameEng').text($('#CompNameEng').val());
                            }
                            if ($('#CompAddrLine1').val() == "") {
                                $('#lblCompAddrLine1').text(label.vldno_data);
                            } else {
                                $('#lblCompAddrLine1').text($('#CompAddrLine1').val());
                            }
                            if ($("#CompDistrictID").val() == 0) {
                                $('#lblCompDistrictID').text("");
                            } else {
                                $('#lblCompDistrictID').text($("#CompDistrictID option[value=" + $("#CompDistrictID").val() + "]").attr("selected", "selected").text());
                            }
                            if ($("#CompProvinceID").val() == 0) {
                                $('#lblCompProvinceID').text(label.vldno_data);
                            } else {
                                $('#lblCompProvinceID').text($("#CompProvinceID option[value=" + $("#CompProvinceID").val() + "]").attr("selected", "selected").text());
                            }
                            if ($('#CompPostalCode').val() == "") {
                                $('#lblCompPostalCode').text(label.vldno_data);
                            } else {
                                $('#lblCompPostalCode').text($('#CompPostalCode').val());
                            }
                            if ($('#CompPhone').val() == "") {
                                $('#lblCompPhone').text(label.vldno_data);
                            } else {
                                $('#lblCompPhone').text($('#CompPhone').val());
                            }
                            if ($('#CompMobile').val() == "") {
                                $('#lblCompMobile').text(label.vldno_data);
                            } else {
                                $('#lblCompMobile').text($('#CompMobile').val());
                            }
                            if ($('#CompFax').val() == "") {
                                $('#lblCompFax').text(label.vldno_data);
                            } else {
                                $('#lblCompFax').text($('#CompFax').val());
                            }
                            if ($('#BizTypeID').val() == 0) {
                                $('#lblBizTypeID').text(label.vldno_data);
                            } else {
                                $('#lblBizTypeID').text($("#BizTypeID option[value=" + $("#BizTypeID").val() + "]").attr("selected", "selected").text());
                            }
                            if ($("#BizTypeID").val() == 13) { $('#lblBizTypeID').append(' (' + $('#BizTypeOther').val() + ')'); }
                            if ($('#CompWebsiteUrl').val() == "") {
                                $('#lblCompWebsiteUrl').text(label.vldno_data);
                            } else {
                                $('#lblCompWebsiteUrl').text($('#CompWebsiteUrl').val());
                            }

                            $('#lblFacebookUrl').text($('#FacebookUrl').val());

                            $('#lblLineID').text($('#LineID').val());


                            $('.show4').show();
                            $('.control-group').removeClass("success error");
                            $('.hide4').hide();
                            $('.hideButtom').hide();
                            $('.checkbox').hide();
                            $('#edit4').show();
                            $('#editAll').show();

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
                bootbox.alert(label.vldrequired_complete);
                return false;
            }
        }
    });

    /*---------------------------------------------------------------------------*/

    $("ul.toggle > li").each(function (index) {
        if ($(this).attr("id") == 'tab_account') {
            $(this).addClass('active_template');
        }
        else {
            $(this).removeClass('active_template');
        }
    });
    $(".fileupload").fileupload({});

    $('#CompDetail').scrollbars();

    $('.btn-tootip-bottom').tooltip({ placement: 'bottom' });
    $('.btn-tootip-top').tooltip({ placement: 'top' });

    /*-----------------------------------IsCompSameAddr----------------------------------------------*/
    $("#IsCompSameAddr").click(function () {
        var value = $(this).attr("checked");
        if (value == 'checked') {
            $('#IsCompSameAddr').val(true);
            $.ajax({
                url: GetUrl("Company/IsMemberSameAddr"),
                type: "POST",
                data: { IsSame: 1 },
                dataType: 'json',
                success: function (data) {
                    if (data != null) {
                        if (data.IsResult) {
                            $("#CompAddrLine1").val(data.Member.AddrLine1).attr("readonly", true);
                            $("#lblCompAddrLine1").text(data.Member.AddrLine1);
                            $("#CompPostalCode").val(data.Member.PostalCode).attr("readonly", true);
                            $("#lblCompPostalCode").text(data.Member.PostalCode);
                            $("#CompPhone").val(data.Member.Phone).attr("readonly", true);
                            $("#lblCompPhone").text(data.Member.Phone);
                            $("#CompMobile").val(data.Member.Mobile).attr("readonly", true);
                            $("#lblCompMobile").text(data.Member.Mobile);
                            $("#CompFax").val(data.Member.Fax).attr("readonly", true);
                            $("#lblCompFax").text(data.Member.Fax);
                            if (data.Member.ProvinceID != null) {
                                GetProvince(data.Member.ProvinceID, "CompProvinceID");
                                $("#CompProvinceID").attr("readonly", true);
                                $("#lblCompProvinceID").text($("#CompProvinceID option[value=" + data.Member.ProvinceID + "]").attr("selected", "selected").text());
                            } else {
                                GetProvince(0, "CompProvinceID");
                                $("#CompProvinceID").attr("readonly", true);
                                $("#CompProvinceID option[value=0]").attr("selected", "selected");
                                $("#lblCompProvinceID").text(label.vldno_data);
                            }
                            if (data.Member.DistrictID != null) {
                                GetDistrict(data.Member.DistrictID, "CompDistrictID");
                                $("#CompDistrictID").attr("readonly", true);
                                $("#lblCompDistrictID").text($("#CompDistrictID option[value=" + data.Member.DistrictID + "]").attr("selected", "selected").text());
                            } else {
                                GetDistrict(0, "CompDistrictID");
                                $("#CompDistrictID").attr("readonly", true);
                                $("#CompDistrictID option[value=0]").attr("selected", "selected");
                                $("#lblCompDistrictID").text(label.vldno_data);
                            }

                            $("#CompAddrLine1").closest('.control-group').removeClass('error');
                            $("#CompAddrLine1").closest('.control-group').addClass('success');
                            $(".control-group").find(".CompAddrLine1").find(".error").remove();

                            $("#CompProvinceID").closest('.control-group').removeClass('error');
                            $("#CompProvinceID").closest('.control-group').addClass('success');
                            $(".control-group").find(".CompProvinceID").find(".error").remove();

                            $("#CompDistrictID").closest('.control-group').removeClass('error');
                            $("#CompDistrictID").closest('.control-group').addClass('success');
                            $(".control-group").find(".CompDistrictID").find(".error").remove();

                            $("#CompPostalCode").closest('.control-group').removeClass('error');
                            $("#CompPostalCode").closest('.control-group').addClass('success');
                            $(".control-group").find(".CompPostalCode").find(".error").remove();

                            $("#CompPhone").closest('.control-group').removeClass('error');
                            $("#CompPhone").closest('.control-group').addClass('success');
                            $(".control-group").find(".CompPhone").find(".error").remove();
                        }
                    }
                },
                error: function () {
                    //bootbox.alert(label.vldcannot_check_info);
                }
            });
        } else {
            $('#IsCompSameAddr').val(false);
            $.ajax({
                url: GetUrl("Company/IsCompSameAddr"),
                type: "POST",
                dataType: 'json',
                success: function (data) {
                    if (data != null) {
                        if (data.IsResult) {
                            $("#CompAddrLine1").val(data.Company.CompAddrLine1).attr("readonly", false);
                            $("#lblCompAddrLine1").text(data.Company.CompAddrLine1);
                            $("#CompPostalCode").val(data.Company.CompPostalCode).attr("readonly", false);
                            $("#lblCompPostalCode").text(data.Company.CompPostalCode);
                            $("#CompPhone").val(data.Company.CompPhone).attr("readonly", false);
                            $("#lblCompPhone").text(data.Company.CompPhone);
                            $("#CompMobile").val(data.Company.CompMobile).attr("readonly", false);
                            $("#lblCompMobile").text(data.Company.CompMobile);
                            $("#CompFax").val(data.Company.CompFax).attr("readonly", false);
                            $("#lblCompFax").text(data.Company.CompFax);
                            if (data.Company.CompProvinceID != null) {
                                ListProvince(data.Company.CompProvinceID, "CompProvinceID");
                                $("#CompProvinceID").attr("readonly", false);
                                $("#lblCompProvinceID").text($("#CompProvinceID option[value=" + data.Company.CompProvinceID + "]").attr("selected", "selected").text());
                            } else {
                                ListProvince(0, "CompProvinceID");
                                $("#CompProvinceID").attr("readonly", false);
                                $("#CompProvinceID option[value=0]").attr("selected", "selected");
                                $("#lblCompProvinceID").text(label.vldno_data);
                            }
                            if (data.Company.CompDistrictID != null) {
                                GetDistrictByProvince(data.Company.CompProvinceID, data.Company.CompDistrictID, "CompDistrictID");
                                $("#CompDistrictID").attr("readonly", false);
                                $("#lblCompDistrictID").text($("#CompDistrictID option[value=" + data.Company.CompDistrictID + "]").attr("selected", "selected").text());
                            } else {
                                if (data.Company.CompProvinceID != null) {
                                    GetDistrictByProvince(data.Company.CompProvinceID, 0, "CompDistrictID");
                                } else {
                                    GetDistrictByProvince(0, 0, "CompDistrictID");
                                }
                                $("#CompDistrictID").attr("readonly", false);
                                $("#CompDistrictID option[value=0]").attr("selected", "selected");
                                $("#lblCompDistrictID").text(label.vldno_data);
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

    /*-----------------------------------ChangeProvince-------------------------------------*/
    $("#CompProvinceID").change(function () {
        GetDistrictByProvince($(this).val(), 0, "CompDistrictID");
    });

    /*------------------- Validate Eng ------------------------ */
    $.validator.addMethod("EngOnly", function (value, element) {
        var i = /^[0-9A-Za-z@._-]+$/;
        return this.optional(element) || (i.test(value) > 0);
    });

    /*-----------------------------validateCompany--------------------------------*/
    $('#CompAcc_form').validate(
    {
        onkeydown: false,
        onkeyup: false,
        rules: {
            CompName: {
                minlength: 3,
                maxlength: 50,
                required: true
            },
            //CompNameEng: {
            //    minlength: 4,
            //    required: true
            //},
            DisplayName: {
                required: true,
                minlength: 4,
                maxlength: 20
            },
            CompPostalCode: {
                required: true,
                number: true,
                minlength: 5,
                maxlength: 5
            },
            CompPhone: {
                required: true,
                minlength: 9
            },
            CompDistrictID: {
                selectDistrict: 0
            },
            CompProvinceID: {
                selectProvince: 0
            },
            BizTypeID: {
                selectBiztype: 0
            },
            CompAddrLine1: {
                required: true
            }
        },
        messages: {
            CompName: {
                required: label.vldrequired,
                minlength: label.vldless_4char
            },
            //CompNameEng: {
            //    minlength: label.vldless_4char,
            //    required: label.vldrequired
            //},
            DisplayName: {
                required: label.vldrequired,
                minlength: label.vldmin_4_max_20char,
                maxlength: label.vldmin_4_max_20char
            },
            CompPostalCode: {
                required: label.vldrequired,
                number: label.vldfix_format_number,
                minlength: label.vldless_5char,
                maxlength: label.vldmore_5char
            },
            CompPhone: {
                required: label.vldrequired,
                minlength: label.vldless_9char
            },
            CompDistrictID: {
                selectDistrict: label.vldselectdistrict
            },
            CompProvinceID: {
                selectProvince: label.vldselectprovince
            },
            BizTypeID: {
                selectBiztype: label.vldselectbiztype
            },
            CompAddrLine1: {
                required: label.vldrequired
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
                $(".CompName > .error").text(label.vldcompname_exists);
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
}

/*-------------------------------checkCompNameEng-----------------------------*/
function checkCompNameEng() {
    if ($('#CompNameEng').val() != "") {
        if (checkEng($('#CompNameEng').val())) {
            $.ajax({
                url: GetUrl("Company/ValidateCompany"),
                data: { compnameeng: $('#CompNameEng').val() },
                type: "POST",
                success: function (data) {
                    if (!data) {
                        $("#CompNameEng").closest('.control-group').removeClass('success');
                        $("#CompNameEng").closest('.control-group').addClass('error');
                        $(".CompNameEng > .error").text(label.vldcompname_exists);
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
        } else {
            $("#CompNameEng").closest('.control-group').removeClass('success');
            $("#CompNameEng").closest('.control-group').addClass('error');
            $('#CompNameEng').next().remove();
            $('#CompNameEng').after('<label for="CompNameEng" generated="true" class="error">กรุณากรอกเป็นภาษาอังกฤษเท่านั้น</label>');
        }
    } else {
        $("#CompNameEng").closest('.control-group').removeClass('error');
        $("#CompNameEng").closest('.control-group').addClass('success');
        $('#CompNameEng').next().remove();
    }
}

/*-------------------------------checkDisplayName--------------------------------*/
function checkDisplayName() {
    $.ajax({
        url: GetUrl("Company/ValidateCompany"),
        data: { displayname: $('#DisplayName').val() },
        type: "POST",
        success: function (data) {
            if (!data) {
                $("#DisplayName").closest('.control-group').removeClass('success');
                $("#DisplayName").closest('.control-group').addClass('error');
                $(".DisplayName > .error").text(label.vlddisplayname_exists);
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
}

/*-------------------------------checkError--------------------------------*/
function checkError() {
    if ($('.control-group').hasClass('error')) {
        return false;
    } else {
        return true;
    }
}

/*-----------------------------Del_imame--------------------------------*/
function Del_image(number) {
    if (confirm(label.vldconfirm_del)) {
        var no_img = "<img id='img_CompImgPath_" + number + "' src='http://www.placehold.it/180x120/EFEFEF/AAAAAA&text=no+image' />";
        $("#CompImg_" + number).html(no_img).addClass("NoImg");
        $("#CompImgPath_" + number).val("");
    }
}

function Del_Logoimage() {
    if (confirm(label.vldconfirm_del)) {
        var no_img = "<img id='img_LogoImgPath' src='http://www.placehold.it/100x75/EFEFEF/AAAAAA&text=no+image' />";
        $("#ImgLogo").html(no_img);
        $("#LogoImgPath").val("");
    }
}

function zam() {
    $("#FileLogoImgPath").select();
}

/*------------------------------RemoveCompImage---------------------------------*/
$(window).unload(function () {
    $.ajax({
        url: GetUrl("Company/RemoveCompImage"),
        success: function () {
            FileName = "";
        },
        error: function () {
            //bootbox.alert(label.vldcannot_check_info);
        },
        type: "POST"
    });
});

/*------------------------------RemoveLogo---------------------------------*/
$(window).unload(function () {
    $.ajax({
        url: GetUrl("Company/RemoveLogo"),
        success: function () {
            FileName = "";
        },
        error: function () {
            //bootbox.alert(label.vldcannot_check_info);
        },
        type: "POST"
    });
});

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