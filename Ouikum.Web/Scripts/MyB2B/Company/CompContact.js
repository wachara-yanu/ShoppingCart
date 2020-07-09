$(function () {
    $("em").remove();
    $(".t-upload-button > span").remove();

    /*-----------------------------ajax submit------------------------------*/
    $("#submitAll").click(function () {
        if ($('#IsContSameAddr').val() == "true") {
            var bool = true;
            var textError = "";
            if ($("#ContactFirstName").val() == "") {
                bool = false;
                textError = "ติดต่อคุณ ";
                $("#ContactFirstName").closest('.control-group').removeClass('success');
                $("#ContactFirstName").closest('.control-group').addClass('error');
                $(".control-group").find(".controls").find("#ContactFirstName").after($('<label for="ContactFirstName" generated="true" class="error">กรุณากรอกข้อมูล</label>'));
            }
            if ($("#ContactEmail").val() == "") {
                bool = false;
                textError += " อีเมล์";
                $("#ContactEmail").closest('.control-group').removeClass('success');
                $("#ContactEmail").closest('.control-group').addClass('error');
                $(".control-group").find(".controls").find("#ContactEmail").after($('<label for="ContactEmail" generated="true" class="error">กรุณากรอกข้อมูล</label>'));
            }
            if (checkError() && bool) {
                data = {
                    IsContSameAddr: $('#IsContSameAddr').val(),
                    ContactFirstName: $('#ContactFirstName').val(),
                    ContactLastName: $('#ContactLastName').val(),
                    ContactPositionName: $('#ContactPositionName').val(),
                    ContactImgPath: $('#ContactImgPath').val(),
                    ContactAddrLine1: $('#ContactAddrLine1').val(),
                    ContactDistrictID: $('#ContactDistrictID').val(),
                    ContactProvinceID: $('#ContactProvinceID').val(),
                    ContactPostalCode: $('#ContactPostalCode').val(),
                    ContactEmail: $('#ContactEmail').val(),
                    ContactPhone: $('#ContactPhone').val(),
                    ContactMobile: $('#ContactMobile').val(),
                    ContactFax: $('#ContactFax').val(),
                    MapImgPath: $('#MapImgPath').val(),
                    CompMapDetail: tinyMCE.get('CompMapDetail').getContent(),
                    GMapLatitude: $('#GMapLatitude').val(),
                    GMapLongtitude: $('#GMapLongtitude').val(),
                    GPinLatitude: $('#GPinLatitude').val(),
                    GPinLongtitude: $('#GPinLongtitude').val(),
                    GZoom: $('#GZoom').val(),
                    Type: 0
                }
                OpenLoading(true);
                $.ajax({
                    url: GetUrl("Company/CompanyContact"),
                    data: data,
                    type: "POST",
                    datatype: "json",
                    traditional: true,
                    success: function (data) {
                        if (data["result"]) {
                            OpenLoading(false);
                            bootbox.alert(label.vldsave_success);
                            $("#sidebar").css("height", "1978px");
                            //$("#RowVersion").val(data["RowVersion"]);
                            if ($('#ContactFirstName').val() == "") {
                                $('#lblContactFirstName').text(label.vldno_data);
                            } else {
                                $('#lblContactFirstName').text($('#ContactFirstName').val());
                            }
                            if ($('#ContactLastName').val() == "") {
                                $('#lblContactLastName').text(label.vldno_data);
                            } else {
                                $('#lblContactLastName').text($('#ContactLastName').val());
                            }
                            if ($('#ContactPositionName').val() == "") {
                                $('#lblContactPositionName').text(label.vldno_data);
                            } else {
                                $('#lblContactPositionName').text($('#ContactPositionName').val());
                            }
                            if ($('#ContactImgPath').val() == "") {
                                $('#lblContactImgPath').text("");
                            } else {
                                $('#lblContactImgPath').text($('#ContactImgPath').val());
                            }
                            if ($('#ContactAddrLine1').val() == "") {
                                $('#lblContactAddrLine1').text(label.vldno_data);
                            } else {
                                $('#lblContactAddrLine1').text($('#ContactAddrLine1').val());
                            }
                            if ($("#ContactDistrictID").val() == "") {
                                $('#lblContactDistrictID').text(label.vldno_data);
                            } else {
                                $('#lblContactDistrictID').text($("#ContactDistrictID option[value=" + $("#ContactDistrictID").val() + "]").attr("selected", "selected").text());
                            }
                            if ($("#ContactProvinceID").val() == "") {
                                $('#lblContactProvinceID').text(label.vldno_data);
                            } else {
                                $('#lblContactProvinceID').text($("#ContactProvinceID option[value=" + $("#ContactProvinceID").val() + "]").attr("selected", "selected").text());
                            }
                            if ($('#ContactPostalCode').val() == "") {
                                $('#lblContactPostalCode').text(label.vldno_data);
                            } else {
                                $('#lblContactPostalCode').text($('#ContactPostalCode').val());
                            }
                            if ($('#ContactEmail').val() == "") {
                                $('#lblContactEmail').text(label.vldno_data);
                            } else {
                                $('#lblContactEmail').text($('#ContactEmail').val());
                            }
                            if ($('#ContactPhone').val() == "") {
                                $('#lblContactPhone').text(label.vldno_data);
                            } else {
                                $('#lblContactPhone').text($('#ContactPhone').val());
                            }
                            if ($('#ContactMobile').val() == "") {
                                $('#lblContactMobile').text(label.vldno_data);
                            } else {
                                $('#lblContactMobile').text($('#ContactMobile').val());
                            }
                            if ($('#ContactFax').val() == "") {
                                $('#lblContactFax').text(label.vldno_data);
                            } else {
                                $('#lblContactFax').text($('#ContactFax').val());
                            }
                            if ($('#MapImgPath').val() == "") {
                                $('#lblMapImgPath').text(label.vldno_data);
                            } else {
                                $('#lblMapImgPath').text($('#MapImgPath').val());
                            }
                            if (tinyMCE.get('CompMapDetail').getContent() == "") {
                                $('#lblCompMapDetail').html(label.vldno_data);
                            } else {
                                $('#lblCompMapDetail').html(tinyMCE.get('CompMapDetail').getContent());
                            }
                            if ($('#GMapLatitude').val() == "") {
                                $('#lblGMapLatitude').text(label.vldno_data);
                            } else {
                                $('#lblGMapLatitude').text($('#GMapLatitude').val());
                            }
                            if ($('#GMapLongtitude').val() == "") {
                                $('#lblGMapLongtitude').text(label.vldno_data);
                            } else {
                                $('#lblGMapLongtitude').text($('#GMapLongtitude').val());
                            }
                            if ($('#GPinLatitude').val() == "") {
                                $('#lblGPinLatitude').text(label.vldno_data);
                            } else {
                                $('#lblGPinLatitude').text($('#GPinLatitude').val());
                            }
                            if ($('#GPinLongtitude').val() == "") {
                                $('#lblGPinLongtitude').text(label.vldno_data);
                            } else {
                                $('#lblGPinLongtitude').text($('#GPinLongtitude').val());
                            }
                            if ($('#GPinLongtitude').val() == "") {
                                $('#lblGPinLongtitude').text(label.vldno_data);
                            } else {
                                $('#lblGPinLongtitude').text($('#GPinLongtitude').val());
                            }
                            if ($('#GZoom').val() == "") {
                                $('#lblGZoom').text(label.vldno_data);
                            } else {
                                $('#lblGZoom').text($('#GZoom').val());
                            }
                            $('.show').show();
                            $('.show3').hide();
                            $('.control-group').removeClass("success error");
                            $('.hide').hide();
                            $('.icon-ShowHide.hide').removeAttr('style');
                            $('.btn-file').hide();
                            $('#editAll').show();
                            $('#edit1').show();
                            $('#edit2').show();
                            $('#edit3').show();
                            $('#editAll').val(0);
                            GetMapSearch();
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
                bootbox.alert("กรุณากรอกข้อมูล " + textError);
                return false;
            }
        } else {
            if (checkError() && $('#CompCont_form').valid()) {
                data = {
                    IsContSameAddr: $('#IsContSameAddr').val(),
                    ContactFirstName: $('#ContactFirstName').val(),
                    ContactLastName: $('#ContactLastName').val(),
                    ContactPositionName: $('#ContactPositionName').val(),
                    ContactImgPath: $('#ContactImgPath').val(),
                    ContactAddrLine1: $('#ContactAddrLine1').val(),
                    ContactDistrictID: $('#ContactDistrictID').val(),
                    ContactProvinceID: $('#ContactProvinceID').val(),
                    ContactPostalCode: $('#ContactPostalCode').val(),
                    ContactEmail: $('#ContactEmail').val(),
                    ContactPhone: $('#ContactPhone').val(),
                    ContactMobile: $('#ContactMobile').val(),
                    ContactFax: $('#ContactFax').val(),
                    MapImgPath: $('#MapImgPath').val(),
                    CompMapDetail: tinyMCE.get('CompMapDetail').getContent(),
                    GMapLatitude: $('#GMapLatitude').val(),
                    GMapLongtitude: $('#GMapLongtitude').val(),
                    GPinLatitude: $('#GPinLatitude').val(),
                    GPinLongtitude: $('#GPinLongtitude').val(),
                    GZoom: $('#GZoom').val(),
                    Type: 0
                }
                OpenLoading(true);
                $.ajax({
                    url: GetUrl("Company/CompanyContact"),
                    data: data,
                    type: "POST",
                    datatype: "json",
                    traditional: true,
                    success: function (data) {
                        if (data["result"]) {
                            OpenLoading(false);
                            bootbox.alert(label.vldsave_success);
                            $("#sidebar").css("height", "1978px");
                            //$("#RowVersion").val(data["RowVersion"]);
                            if ($('#ContactFirstName').val() == "") {
                                $('#lblContactFirstName').text(label.vldno_data);
                            } else {
                                $('#lblContactFirstName').text($('#ContactFirstName').val());
                            }
                            if ($('#ContactLastName').val() == "") {
                                $('#lblContactLastName').text(label.vldno_data);
                            } else {
                                $('#lblContactLastName').text($('#ContactLastName').val());
                            }
                            if ($('#ContactPositionName').val() == "") {
                                $('#lblContactPositionName').text(label.vldno_data);
                            } else {
                                $('#lblContactPositionName').text($('#ContactPositionName').val());
                            }
                            if ($('#ContactImgPath').val() == "") {
                                $('#lblContactImgPath').text("");
                            } else {
                                $('#lblContactImgPath').text($('#ContactImgPath').val());
                            }
                            if ($('#ContactAddrLine1').val() == "") {
                                $('#lblContactAddrLine1').text(label.vldno_data);
                            } else {
                                $('#lblContactAddrLine1').text($('#ContactAddrLine1').val());
                            }
                            if ($("#ContactDistrictID").val() == "") {
                                $('#lblContactDistrictID').text(label.vldno_data);
                            } else {
                                $('#lblContactDistrictID').text($("#ContactDistrictID option[value=" + $("#ContactDistrictID").val() + "]").attr("selected", "selected").text());
                            }
                            if ($("#ContactProvinceID").val() == "") {
                                $('#lblContactProvinceID').text(label.vldno_data);
                            } else {
                                $('#lblContactProvinceID').text($("#ContactProvinceID option[value=" + $("#ContactProvinceID").val() + "]").attr("selected", "selected").text());
                            }
                            if ($('#ContactPostalCode').val() == "") {
                                $('#lblContactPostalCode').text(label.vldno_data);
                            } else {
                                $('#lblContactPostalCode').text($('#ContactPostalCode').val());
                            }
                            if ($('#ContactEmail').val() == "") {
                                $('#lblContactEmail').text(label.vldno_data);
                            } else {
                                $('#lblContactEmail').text($('#ContactEmail').val());
                            }
                            if ($('#ContactPhone').val() == "") {
                                $('#lblContactPhone').text(label.vldno_data);
                            } else {
                                $('#lblContactPhone').text($('#ContactPhone').val());
                            }
                            if ($('#ContactMobile').val() == "") {
                                $('#lblContactMobile').text(label.vldno_data);
                            } else {
                                $('#lblContactMobile').text($('#ContactMobile').val());
                            }
                            if ($('#ContactFax').val() == "") {
                                $('#lblContactFax').text(label.vldno_data);
                            } else {
                                $('#lblContactFax').text($('#ContactFax').val());
                            }
                            if ($('#MapImgPath').val() == "") {
                                $('#lblMapImgPath').text(label.vldno_data);
                            } else {
                                $('#lblMapImgPath').text($('#MapImgPath').val());
                            }
                            if (tinyMCE.get('CompMapDetail').getContent() == "") {
                                $('#lblCompMapDetail').html(label.vldno_data);
                            } else {
                                $('#lblCompMapDetail').html(tinyMCE.get('CompMapDetail').getContent());
                            }
                            if ($('#GMapLatitude').val() == "") {
                                $('#lblGMapLatitude').text(label.vldno_data);
                            } else {
                                $('#lblGMapLatitude').text($('#GMapLatitude').val());
                            }
                            if ($('#GMapLongtitude').val() == "") {
                                $('#lblGMapLongtitude').text(label.vldno_data);
                            } else {
                                $('#lblGMapLongtitude').text($('#GMapLongtitude').val());
                            }
                            if ($('#GPinLatitude').val() == "") {
                                $('#lblGPinLatitude').text(label.vldno_data);
                            } else {
                                $('#lblGPinLatitude').text($('#GPinLatitude').val());
                            }
                            if ($('#GPinLongtitude').val() == "") {
                                $('#lblGPinLongtitude').text(label.vldno_data);
                            } else {
                                $('#lblGPinLongtitude').text($('#GPinLongtitude').val());
                            }
                            if ($('#GPinLongtitude').val() == "") {
                                $('#lblGPinLongtitude').text(label.vldno_data);
                            } else {
                                $('#lblGPinLongtitude').text($('#GPinLongtitude').val());
                            }
                            if ($('#GZoom').val() == "") {
                                $('#lblGZoom').text(label.vldno_data);
                            } else {
                                $('#lblGZoom').text($('#GZoom').val());
                            }
                            $('.show').show();
                            $('.show3').hide();
                            $('.control-group').removeClass("success error");
                            $('.hide').hide();
                            $('.icon-ShowHide.hide').removeAttr('style');
                            $('.btn-file').hide();
                            $('#editAll').show();
                            $('#edit1').show();
                            $('#edit2').show();
                            $('#edit3').show();
                            $('#editAll').val(0);
                            GetMapSearch();
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
        $(this).children().slideToggle();
        $(this).next().slideToggle(function () {
            $("#sidebar").height($("#auroHeight").height());
            $("#main").height($("#auroHeight").height());
        });
    });

    $('#cancelAll').click(function () {
        window.location = GetUrl("MyB2B/Company/CompanyContact");
    });

    $('#editAll').click(function () {
        $("#sidebar").css("height", "2514px");
        var IsContSameAddr = $('#IsContSameAddr').val();
        if (IsContSameAddr == "true") {
            $('#IsContSameAddr').val(true);
            $("#ContactAddrLine1").attr("readonly", true);
            $("#ContactPostalCode").attr("readonly", true);
            $("#ContactPhone").attr("readonly", true);
            $("#ContactMobile").attr("readonly", true);
            $("#ContactFax").attr("readonly", true);
            $("#ContactProvinceID").attr("readonly", true);
            $("#ContactProvinceID").attr("readonly", true);
            $("#ContactDistrictID").attr("readonly", true);
            $("#ContactDistrictID").attr("readonly", true);
        }

        $(".content_toggle").slideDown(function () {
            $('#editAll').val(1);
            var GMapLatitude = $('#GMapLatitude').val();
            var GMapLongtitude = $('#GMapLongtitude').val();
            var District = $('#lblContactDistrictID').text();
            if (GMapLatitude == "" || GMapLatitude == "0" || GMapLongtitude == "" || GMapLongtitude == "0") {
                $('#address').val(District);
                showAddress($('#address').val());
            } else {
                GetMapSearch();
            }
            //initialize($('#GMapLatitude').val(), $('#GMapLongtitude').val(), $('#GPinLatitude').val(), $('#GPinLongtitude').val(), $('#GZoom').val(), $('#editAll').val());
            $("#sidebar").height($("#auroHeight").height());
            $("#main").height($("#auroHeight").height());
        });

        $(this).hide();
        $('.show').hide();
        $('.show2').hide();
        $('.show3').hide();
        $('.hide').show();
        $('.icon-ShowHide.hide').removeAttr('style');
        $('.lineShow').hide();
        $('.lineHide').show();
        $('.btn-file').show();
        $('.checkbox').css('display', 'inline-block');
        $('.hideTop').hide();
        $('.hideMiddle').hide();
        $('.hideButtom').hide();
        $('#edit1').hide();
        $('#edit2').hide();
        $('#edit3').hide();
        $('#submitAll').css('display', 'inline-block');
        $('#cancelAll').css('display', 'inline-block');

    });

    /*--------------------------------------- btn edit1 -------------------------------------*/
    $('#edit1').click(function () {
        $("#sidebar").css("height", "2005px");
        var IsContSameAddr = $('#IsContSameAddr').val();
        if (IsContSameAddr == "true") {
            $('#IsContSameAddr').val(true);
            $("#ContactAddrLine1").attr("readonly", true);
            $("#ContactPostalCode").attr("readonly", true);
            $("#ContactPhone").attr("readonly", true);
            $("#ContactMobile").attr("readonly", true);
            $("#ContactFax").attr("readonly", true);
            $("#ContactProvinceID").attr("readonly", true);
            $("#ContactProvinceID").attr("readonly", true);
            $("#ContactDistrictID").attr("readonly", true);
            $("#ContactDistrictID").attr("readonly", true);
        }

        $("#company_con").slideDown();
        $(this).hide();
        $('.show1').hide();
        $('.hide1').show();
        $('.hideTop').show();
        $('#editAll').hide();
        $('.checkbox').css('display', 'inline-block');
    });

    $('#btn_cancle1').click(function () {
        window.location = GetUrl("MyB2B/Company/CompanyContact");
        //$('#edit1').show();
        //$('#edit1').val(0)
        //$('.show1').show();
        //$('.hide1').hide();
        //$('label.error').remove();
        //$('.hideTop').hide();
        //$('#editAll').show();
    });

    $("#btn_save1").click(function () {
        if ($('#IsContSameAddr').val() == "true") {
            var bool = true;
            var textError = "";
            if ($("#ContactFirstName").val() == "") {
                bool = false;
                textError = "ติดต่อคุณ ";
                $("#ContactFirstName").closest('.control-group').removeClass('success');
                $("#ContactFirstName").closest('.control-group').addClass('error');
                $(".control-group").find(".controls").find("#ContactFirstName").after($('<label for="ContactFirstName" generated="true" class="error">กรุณากรอกข้อมูล</label>'));
            } 
            if ($("#ContactEmail").val() == "") {
                bool = false;
                textError += " อีเมล์";
                $("#ContactEmail").closest('.control-group').removeClass('success');
                $("#ContactEmail").closest('.control-group').addClass('error');
                $(".control-group").find(".controls").find("#ContactEmail").after($('<label for="ContactEmail" generated="true" class="error">กรุณากรอกข้อมูล</label>'));
            }
            if (bool && checkError()) {
                data = {
                    IsContSameAddr: $('#IsContSameAddr').val(),
                    ContactFirstName: $('#ContactFirstName').val(),
                    ContactLastName: $('#ContactLastName').val(),
                    ContactPositionName: $('#ContactPositionName').val(),
                    ContactImgPath: $('#ContactImgPath').val(),
                    ContactAddrLine1: $('#ContactAddrLine1').val(),
                    ContactDistrictID: $('#ContactDistrictID').val(),
                    ContactProvinceID: $('#ContactProvinceID').val(),
                    ContactPostalCode: $('#ContactPostalCode').val(),
                    ContactEmail: $('#ContactEmail').val(),
                    ContactPhone: $('#ContactPhone').val(),
                    ContactMobile: $('#ContactMobile').val(),
                    ContactFax: $('#ContactFax').val(),
                    Type: 1
                }
                OpenLoading(true);
                $.ajax({
                    url: GetUrl("Company/CompanyContact"),
                    data: data,
                    type: "POST",
                    datatype: "json",
                    traditional: true,
                    success: function (data) {
                        if (data["result"]) {
                            OpenLoading(false);
                            bootbox.alert(label.vldsave_success);
                            $("#sidebar").css("height", "1978px");
                            //$("#RowVersion").val(data["RowVersion"]);
                            if ($('#ContactFirstName').val() == "") {
                                $('#lblContactFirstName').text(label.vldno_data);
                            } else {
                                $('#lblContactFirstName').text($('#ContactFirstName').val());
                            }
                            if ($('#ContactLastName').val() == "") {
                                $('#lblContactLastName').text(label.vldno_data);
                            } else {
                                $('#lblContactLastName').text($('#ContactLastName').val());
                            }
                            if ($('#ContactPositionName').val() == "") {
                                $('#lblContactPositionName').text(label.vldno_data);
                            } else {
                                $('#lblContactPositionName').text($('#ContactPositionName').val());
                            }
                            if ($('#ContactImgPath').val() == "") {
                                $('#lblContactImgPath').text("");
                            } else {
                                $('#lblContactImgPath').text($('#ContactImgPath').val());
                            }
                            if ($('#ContactAddrLine1').val() == "") {
                                $('#lblContactAddrLine1').text(label.vldno_data);
                            } else {
                                $('#lblContactAddrLine1').text($('#ContactAddrLine1').val());
                            }
                            if ($("#ContactDistrictID").val() == "") {
                                $('#lblContactDistrictID').text(label.vldno_data);
                            } else {
                                $('#lblContactDistrictID').text($("#ContactDistrictID option[value=" + $("#ContactDistrictID").val() + "]").attr("selected", "selected").text());
                            }
                            if ($("#ContactProvinceID").val() == "") {
                                $('#lblContactProvinceID').text(label.vldno_data);
                            } else {
                                $('#lblContactProvinceID').text($("#ContactProvinceID option[value=" + $("#ContactProvinceID").val() + "]").attr("selected", "selected").text());
                            }
                            if ($('#ContactPostalCode').val() == "") {
                                $('#lblContactPostalCode').text(label.vldno_data);
                            } else {
                                $('#lblContactPostalCode').text($('#ContactPostalCode').val());
                            }
                            if ($('#ContactEmail').val() == "") {
                                $('#lblContactEmail').text(label.vldno_data);
                            } else {
                                $('#lblContactEmail').text($('#ContactEmail').val());
                            }
                            if ($('#ContactPhone').val() == "") {
                                $('#lblContactPhone').text(label.vldno_data);
                            } else {
                                $('#lblContactPhone').text($('#ContactPhone').val());
                            }
                            if ($('#ContactMobile').val() == "") {
                                $('#lblContactMobile').text(label.vldno_data);
                            } else {
                                $('#lblContactMobile').text($('#ContactMobile').val());
                            }
                            if ($('#ContactFax').val() == "") {
                                $('#lblContactFax').text(label.vldno_data);
                            } else {
                                $('#lblContactFax').text($('#ContactFax').val());
                            }
                            $('.show1').show();
                            $('.control-group').removeClass("success error");
                            $('.hide1').hide();
                            $('.btn-file').hide();
                            $('#editAll').show();
                            $('.hideTop').hide();
                            $('#edit1').show();

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
                bootbox.alert("กรุณากรอกข้อมูล " + textError);
                return false;
            }
        } else {
            if (checkError() && $('#CompCont_form').valid()) {
                data = {
                    IsContSameAddr: $('#IsContSameAddr').val(),
                    ContactFirstName: $('#ContactFirstName').val(),
                    ContactLastName: $('#ContactLastName').val(),
                    ContactPositionName: $('#ContactPositionName').val(),
                    ContactImgPath: $('#ContactImgPath').val(),
                    ContactAddrLine1: $('#ContactAddrLine1').val(),
                    ContactDistrictID: $('#ContactDistrictID').val(),
                    ContactProvinceID: $('#ContactProvinceID').val(),
                    ContactPostalCode: $('#ContactPostalCode').val(),
                    ContactEmail: $('#ContactEmail').val(),
                    ContactPhone: $('#ContactPhone').val(),
                    ContactMobile: $('#ContactMobile').val(),
                    ContactFax: $('#ContactFax').val(),
                    Type: 1
                }
                OpenLoading(true);
                $.ajax({
                    url: GetUrl("Company/CompanyContact"),
                    data: data,
                    type: "POST",
                    datatype: "json",
                    traditional: true,
                    success: function (data) {
                        if (data["result"]) {
                            OpenLoading(false);
                            bootbox.alert(label.vldsave_success);
                            $("#sidebar").css("height", "1978px");
                            //$("#RowVersion").val(data["RowVersion"]);
                            if ($('#ContactFirstName').val() == "") {
                                $('#lblContactFirstName').text(label.vldno_data);
                            } else {
                                $('#lblContactFirstName').text($('#ContactFirstName').val());
                            }
                            if ($('#ContactLastName').val() == "") {
                                $('#lblContactLastName').text(label.vldno_data);
                            } else {
                                $('#lblContactLastName').text($('#ContactLastName').val());
                            }
                            if ($('#ContactPositionName').val() == "") {
                                $('#lblContactPositionName').text(label.vldno_data);
                            } else {
                                $('#lblContactPositionName').text($('#ContactPositionName').val());
                            }
                            if ($('#ContactImgPath').val() == "") {
                                $('#lblContactImgPath').text("");
                            } else {
                                $('#lblContactImgPath').text($('#ContactImgPath').val());
                            }
                            if ($('#ContactAddrLine1').val() == "") {
                                $('#lblContactAddrLine1').text(label.vldno_data);
                            } else {
                                $('#lblContactAddrLine1').text($('#ContactAddrLine1').val());
                            }
                            if ($("#ContactDistrictID").val() == "") {
                                $('#lblContactDistrictID').text(label.vldno_data);
                            } else {
                                $('#lblContactDistrictID').text($("#ContactDistrictID option[value=" + $("#ContactDistrictID").val() + "]").attr("selected", "selected").text());
                            }
                            if ($("#ContactProvinceID").val() == "") {
                                $('#lblContactProvinceID').text(label.vldno_data);
                            } else {
                                $('#lblContactProvinceID').text($("#ContactProvinceID option[value=" + $("#ContactProvinceID").val() + "]").attr("selected", "selected").text());
                            }
                            if ($('#ContactPostalCode').val() == "") {
                                $('#lblContactPostalCode').text(label.vldno_data);
                            } else {
                                $('#lblContactPostalCode').text($('#ContactPostalCode').val());
                            }
                            if ($('#ContactEmail').val() == "") {
                                $('#lblContactEmail').text(label.vldno_data);
                            } else {
                                $('#lblContactEmail').text($('#ContactEmail').val());
                            }
                            if ($('#ContactPhone').val() == "") {
                                $('#lblContactPhone').text(label.vldno_data);
                            } else {
                                $('#lblContactPhone').text($('#ContactPhone').val());
                            }
                            if ($('#ContactMobile').val() == "") {
                                $('#lblContactMobile').text(label.vldno_data);
                            } else {
                                $('#lblContactMobile').text($('#ContactMobile').val());
                            }
                            if ($('#ContactFax').val() == "") {
                                $('#lblContactFax').text(label.vldno_data);
                            } else {
                                $('#lblContactFax').text($('#ContactFax').val());
                            }
                            $('.show1').show();
                            $('.control-group').removeClass("success error");
                            $('.hide1').hide();
                            $('.btn-file').hide();
                            $('#editAll').show();
                            $('.hideTop').hide();
                            $('#edit1').show();

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

    /*--------------------------------------- btn edit2 -------------------------------------*/
    $('#edit2').click(function () {
        $("#sidebar").css("height", "2266px");
        $("#company_con2").slideDown();
        $(this).hide();
        $('.show2').hide();
        $('.hide2').show();
        $('.hideMiddle').show();
        $('#editAll').hide();
    });

    $('#btn_cancle2').click(function () {
        window.location = GetUrl("MyB2B/Company/CompanyContact");
        //$('#edit2').show();
        //$('#edit2').val(0)
        //$('.show2').show();
        //$('.hide2').hide();
        //$('label.error').remove();
        //$('.hideMiddle').hide();
        //$('#editAll').show();
    });

    $("#btn_save2").click(function () {
        data = {
            MapImgPath: $('#MapImgPath').val(),
            CompMapDetail: tinyMCE.get('CompMapDetail').getContent(),
            GZoom: $('#GZoom').val(),
            Type:2
        }
        OpenLoading(true);
        $.ajax({
            url: GetUrl("Company/CompanyContact"),
            data: data,
            type: "POST",
            success: function (data) {
                if (data["result"]) {
                    OpenLoading(false);
                    bootbox.alert(label.vldsave_success);
                    $("#sidebar").css("height", "1978px");
                    //$("#RowVersion").val(data["RowVersion"]);
                    if ($('#MapImgPath').val() == "") {
                        $('#lblMapImgPath').text(label.vldno_data);
                    } else {
                        $('#lblMapImgPath').text($('#MapImgPath').val());
                    }
                    if (tinyMCE.get('CompMapDetail').getContent() == "") {
                        $('#lblCompMapDetail').html(label.vldno_data);
                    } else {
                        $('#lblCompMapDetail').html(tinyMCE.get('CompMapDetail').getContent());
                    }
                    $('.show2').show();
                    $('.control-group').removeClass("success error");
                    $('.hide2').hide();
                    $('.btn-file').hide();
                    $('#editAll').show();
                    $('.hideMiddle').hide();
                    $('#edit2').show();

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

    /*--------------------------------------- btn edit3 -------------------------------------*/
    $('#edit3').click(function () {
        $("#sidebar").css("height", "2095px");
        $("#company_con2").slideDown();
        $(this).hide();
        $('.showmap').show();
        $('.show3').hide();
        $('.searchmap').show();
        $('.hideButtom').show();
        $('#editAll').hide();
        $('#editAll').val(1);
        var GMapLatitude = $('#GMapLatitude').val();
        var GMapLongtitude = $('#GMapLongtitude').val();
        var District = $('#lblContactDistrictID').text();
        if (GMapLatitude == "" || GMapLatitude == "0" || GMapLongtitude == "" || GMapLongtitude == "0") {
            $('#address').val(District);
            showAddress($('#address').val());
        } else {
            GetMapSearch();
        }
        //initialize($('#GMapLatitude').val(), $('#GMapLongtitude').val(), $('#GPinLatitude').val(), $('#GPinLongtitude').val(), $('#GZoom').val(), $('#editAll').val());
    });

    $('#btn_cancle3').click(function () {
        window.location = GetUrl("MyB2B/Company/CompanyContact");
    });

    $("#btn_save3").click(function () {
        data = {
            GMapLatitude: $('#GMapLatitude').val(),
            GMapLongtitude: $('#GMapLongtitude').val(),
            GPinLatitude: $('#GPinLatitude').val(),
            GPinLongtitude: $('#GPinLongtitude').val(),
            GZoom: $('#GZoom').val(),
            Type:3
        }
        OpenLoading(true);
        $.ajax({
            url: GetUrl("Company/CompanyContact"),
            data: data,
            type: "POST",
            success: function (data) {
                if (data["result"]) {
                    OpenLoading(false);
                    bootbox.alert(label.vldsave_success);
                    $("#sidebar").css("height", "1978px");
                    //$("#RowVersion").val(data["RowVersion"]);

                    if ($('#GMapLatitude').val() == "") {
                        $('#lblGMapLatitude').text(label.vldno_data);
                    } else {
                        $('#lblGMapLatitude').text($('#GMapLatitude').val());
                    }
                    if ($('#GMapLongtitude').val() == "") {
                        $('#lblGMapLongtitude').text(label.vldno_data);
                    } else {
                        $('#lblGMapLongtitude').text($('#GMapLongtitude').val());
                    }
                    if ($('#GPinLatitude').val() == "") {
                        $('#lblGPinLatitude').text(label.vldno_data);
                    } else {
                        $('#lblGPinLatitude').text($('#GPinLatitude').val());
                    }
                    if ($('#GPinLongtitude').val() == "") {
                        $('#lblGPinLongtitude').text(label.vldno_data);
                    } else {
                        $('#lblGPinLongtitude').text($('#GPinLongtitude').val());
                    }
                    if ($('#GZoom').val() == "") {
                        $('#lblGZoom').text(label.vldno_data);
                    } else {
                        $('#lblGZoom').text($('#GZoom').val());
                    }
                    $('.showmap').hide();
                    $('.control-group').removeClass("success error");
                    $('.btn-file').hide();
                    $('.searchmap').hide();
                    $('#editAll').show();
                    $('.hideButtom').hide();
                    $('#edit3').show();
                    $('#editAll').val(0);
                    GetMapSearch();
                    //initialize($('#GMapLatitude').val(), $('#GMapLongtitude').val(), $('#GPinLatitude').val(), $('#GPinLongtitude').val(), $('#GZoom').val(), $('#editAll').val());

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

    /*-----------------------------------------------------------------------------------*/
    $("ul.toggle > li").each(function (index) {
        if ($(this).attr("id") == 'tab_contact') {
            $(this).addClass('active_template');
        }
        else {
            $(this).removeClass('active_template');
        }
    });

    $(".fileupload").fileupload({});
    $('#MapDetail').scrollbars();
    $('.btn-tootip-bottom').tooltip({ placement: 'bottom' });
    $('.btn-tootip-top').tooltip({ placement: 'top' });
    GetMapSearch();
    //initialize($('#GMapLatitude').val(), $('#GMapLongtitude').val(), $('#GPinLatitude').val(), $('#GPinLongtitude').val(), $('#GZoom').val(), $('#edit').val());
    
    /*-----------------------------------IsCompSameAddr----------------------------------------------*/
    $("#IsContSameAddr").click(function () {
        var value = $(this).attr("checked");
        if (value == 'checked') {
            $('#IsContSameAddr').val(true);
            $.ajax({
                url: GetUrl("Company/IsCompSameAddr"),
                type: "POST",
                dataType: 'json',
                success: function (data) {
                    if (data != null) {
                        if (data.IsResult) {
                            $("#ContactAddrLine1").val(data.Company.CompAddrLine1).attr("readonly", true);
                            $("#lblContactAddrLine1").text(data.Company.CompAddrLine1);
                            $("#ContactPostalCode").val(data.Company.CompPostalCode).attr("readonly", true);
                            $("#lblContactPostalCode").text(data.Company.CompPostalCode);
                            $("#ContactPhone").val(data.Company.CompPhone).attr("readonly", true);
                            $("#lblContactPhone").text(data.Company.CompPhone);
                            $("#ContactMobile").val(data.Company.CompMobile).attr("readonly", true);
                            $("#lblContactMobile").text(data.Company.CompMobile);
                            $("#ContactFax").val(data.Company.CompFax).attr("readonly", true);
                            $("#ContactCompFax").text(data.Company.CompFax);
                            if (data.Company.CompProvinceID != null) {
                                GetGmap(data.Company.CompProvinceID);
                                GetProvince(data.Company.CompProvinceID, "ContactProvinceID");
                                $("#ContactProvinceID").attr("readonly", true);
                                $("#lblContactProvinceID").text($("#ContactProvinceID option[value=" + data.Company.CompProvinceID + "]").attr("selected", "selected").text());
                            } else {
                                GetGmap(0);
                                GetProvince(0, "ContactProvinceID");
                                $("#ContactProvinceID").attr("readonly", true);
                                $("#ContactProvinceID option[value=0]").attr("selected", "selected");
                                $("#lblContactProvinceID").text(label.vldno_data);
                            }
                            if (data.Company.CompDistrictID != null) {
                                GetDistrict(data.Company.CompDistrictID, "ContactDistrictID");
                                $("#ContactDistrictID").attr("readonly", true);
                                $("#lblContactDistrictID").text($("#ContactDistrictID option[value=" + data.Company.CompDistrictID + "]").attr("selected", "selected").text());
                            } else {
                                GetDistrict(0, "ContactDistrictID");
                                $("#ContactDistrictID").attr("readonly", true);
                                $("#ContactDistrictID option[value=0]").attr("selected", "selected");
                                $("#lblContactDistrictID").text(label.vldno_data);
                            }
                        }
                    }
                },
                error: function () {
                    //bootbox.alert(label.vldcannot_check_info);
                }
            });
        } else {
            $('#IsContSameAddr').val(false);
            $.ajax({
                url: GetUrl("Company/IsCompSameAddr"),
                type: "POST",
                dataType: 'json',
                success: function (data) {
                    if (data != null) {
                        if (data.IsResult) {
                            $("#ContactFirstName").val(data.Company.ContactFirstName);
                            $("#lblContactFirstName").text(data.Company.ContactFirstName);
                            $("#ContactLastName").val(data.Company.ContactLastName);
                            $("#lblContactLastName").text(data.Company.ContactLastName);
                            $("#ContactAddrLine1").val(data.Company.ContactAddrLine1).attr("readonly", false);
                            $("#lblContactAddrLine1").text(data.Company.ContactAddrLine1);
                            $("#ContactPostalCode").val(data.Company.ContactPostalCode).attr("readonly", false);
                            $("#lblContactPostalCode").text(data.Company.ContactPostalCode);
                            $("#ContactPhone").val(data.Company.ContactPhone).attr("readonly", false);
                            $("#lblContactPhone").text(data.Company.ContactPhone);
                            $("#ContactMobile").val(data.Company.ContactMobile).attr("readonly", false);
                            $("#lblContactMobile").text(data.Company.ContactMobile);
                            $("#ContactFax").val(data.Company.ContactFax).attr("readonly", false);
                            $("#ContactCompFax").text(data.Company.ContactFax);
                            if (data.Company.ContactProvinceID != null) {
                                GetGmap(data.Company.ContactProvinceID);
                                ListProvince(data.Company.ContactProvinceID, "ContactProvinceID");
                                $("#ContactProvinceID").attr("readonly", false);
                                $("#lblContactProvinceID").text($("#ContactProvinceID option[value=" + data.Company.ContactProvinceID + "]").attr("selected", "selected").text());
                            } else {
                                GetGmap(0);
                                ListProvince(0, "ContactProvinceID");
                                $("#ContactProvinceID").attr("readonly", false);
                                $("#ContactProvinceID option[value=0]").attr("selected", "selected");
                                $("#lblContactProvinceID").text(label.vldno_data);
                            }
                            if (data.Company.ContactDistrictID != null) {
                                GetDistrictByProvince(data.Company.ContactProvinceID, data.Company.ContactDistrictID, "ContactDistrictID");
                                $("#ContactDistrictID").attr("readonly", false);
                                $("#lblContactDistrictID").text($("#ContactDistrictID option[value=" + data.Company.ContactDistrictID + "]").attr("selected", "selected").text());
                            } else {
                                if (data.Company.ContactProvinceID != null) {
                                    GetDistrictByProvince(data.Company.ContactProvinceID, 0, "ContactDistrictID");
                                } else {
                                    GetDistrictByProvince(0, 0, "ContactDistrictID");
                                }
                                $("#ContactDistrictID").attr("readonly", false);
                                $("#ContactDistrictID option[value=0]").attr("selected", "selected");
                                $("#lblContactDistrictID").text(label.vldno_data);
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

    /*--------------------------------------GetDistrictByProvinceID------------------------------------------------*/
    $("#ContactProvinceID").change(function () {
        GetDistrictByProvince($(this).val(), 0, "ContactDistrictID");
        GetGmap($(this).val());
    });

    /*-----------------------------validateCompany---------------------------------*/
    $('#CompCont_form').validate(
    {
        onkeydown: false,
        onkeyup: false,
        rules: {
            ContactFirstName: {
                required: true
            },
            ContactAddrLine1: {
                required: true
            },
            ContactEmail: {
                required: true,
                email: true
            },
            ContactPostalCode: {
                number: true,
                minlength: 5,
                maxlength: 5
            },
            ContactPhone: {
                required: true,
                minlength: 9
            },
            ContactDistrictID: {
                selectDistrict: 0
            },
            ContactProvinceID: {
                selectProvince: 0
            },
        },
        messages: {
            ContactFirstName: {
                required: label.vldrequired,
            },
            ContactAddrLine1: {
                required: label.vldrequired,
            },
            ContactEmail: {
                required: label.vldrequired,
                email: label.vldfix_format_email
            },
            ContactPostalCode: {
                number: label.vldfix_format_number,
                minlength: label.vldless_5char,
                maxlength: label.vldmore_5char
            },
            ContactPhone: {
                required: label.vldrequired,
                minlength: label.vldless_9char
            },
            ContactDistrictID: {
                selectDistrict: label.vldselectdistrict
            },
            ContactProvinceID: {
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

/*-------------------------------checkContEmail--------------------------------*/
function checkContEmail() {
    $.ajax({
        url: GetUrl("Company/ValidateCompany"),
        data: { email: $('#ContactEmail').val() },
        type: "POST",
        success: function (data) {
            if (!data) {
                $("#ContactEmail").closest('.control-group').removeClass('success');
                $("#ContactEmail").closest('.control-group').addClass('error');
                $(".ContactEmail > .error").text(label.vldemail_exists);
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

function checkError() {
    var check = "";
    check = $("label.error").text();
    if (check != "") {
        $("#btn_save1,#saveAll").attr('disabled', true);
        return false;
    } else {
        $("#btn_save1,#saveAll").attr('disabled', false);
        return true;
    }
}

/*------------------------------DelContImage---------------------------------*/
function Del_ContImg() {
    if (confirm(label.vldconfirm_del)) {
        var no_img = "<img id='img_ContImgPath' src='http://www.placehold.it/100x75/EFEFEF/AAAAAA&text=no+image' />";
        $("#ImgCont").html(no_img);
        $("#ContactImgPath").val("");
    }
}

function Del_MapImgPath() {
    if (confirm(label.vldconfirm_del)) {
        var no_img = "<img id='img_MapImgPath' src='http://www.placehold.it/500x350/EFEFEF/AAAAAA&text=no+image' />";
        $("#ImgMap").html(no_img);
        $("#MapImgPath").val("");
    }
}

/*------------------------------RemoveContImage---------------------------------*/
$(window).unload(function () {
    $.ajax({
        url: GetUrl("Company/RemoveContactImg"),
        success: function () {
            FileName = "";
        },
        error: function () {
            //bootbox.alert(label.vldcannot_check_info);
        },
        type: "POST"
    });
});

/*------------------------------RemoveMapImage---------------------------------*/
$(window).unload(function () {
    $.ajax({
        url: GetUrl("Company/RemoveMapImg"),
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
