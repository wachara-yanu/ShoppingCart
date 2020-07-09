$(function () {
    /*-----------------------------ajax submit------------------------------*/
    $("#submitAll").click(function () {
        data = {
            CompHistory: tinyMCE.get('CompHistory').getContent(),
            CompFounder: $('#CompFounder').val(),
            CompOwner: $('#CompOwner').val(),
            YearEstablished: $('#YearEstablished').val(),
            EmployeeCount: $('#EmployeeCount').val(),
            Type:0
        }
        OpenLoading(true);
        $.ajax({
            url: GetUrl("Company/CompanyHistory"),
            data: data,
            type: "POST",
            success: function (data) {
                if (data["result"]) {
                    OpenLoading(false);
                    bootbox.alert(label.vldsave_success);
                    //$("#RowVersion").val(data["RowVersion"]);
                    if (tinyMCE.get('CompHistory').getContent() == "") {
                        $('#lblCompHistory').html(label.vldno_data);
                    } else {
                        $('#lblCompHistory').html(tinyMCE.get('CompHistory').getContent());
                    }
                    if ($('#CompFounder').val() == "") {
                        $('#lblCompFounder').text(label.vldno_data);
                    } else {
                        $('#lblCompFounder').text($('#CompFounder').val());
                    }
                    if ($('#CompOwner').val() == "") {
                        $('#lblCompOwner').text(label.vldno_data);
                    } else {
                        $('#lblCompOwner').text($('#CompOwner').val());
                    }
                    if ($('#YearEstablished').val() == "") {
                        $('#lblYearEstablished').text(label.vldno_data);
                    } else {
                        $('#lblYearEstablished').text($('#YearEstablished').val());
                    }
                    if ($('#EmployeeCount').val() == "") {
                        $('#lblEmployeeCount').text(label.vldno_data);
                    } else {
                        $('#lblEmployeeCount').text($('#EmployeeCount').val());
                    }
                    $('.show').show();
                    $('.control-group').removeClass("success error");
                    $('.hide').hide();
                    $('.icon-ShowHide.hide').removeAttr('style');
                    $('.btn-file').hide();
                    $('#editAll').show();

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

    /*-----------------------------------tinyMCE-------------------------------------------*/
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

    $(".toggle_acc").click(function () {
        $(this).children().toggle();
        $(this).next().slideToggle(function () {
            $("#sidebar").height($("#auroHeight").height());
            $("#main").height($("#auroHeight").height());
        });
    });

    $('#cancelAll').click(function () {
        $('#editAll').show();
        $('#edit1').show();
        $('#edit2').show();
        $('.show').show();
        $('.hide').hide();
        $('.icon-ShowHide.hide').removeAttr('style');
        $('label.error').remove();
        $('div,label').removeClass('error');
    });

    $('#editAll').click(function () {
        $(".data-target").slideDown(function () {
            $("#sidebar").height($("#auroHeight").height());
            $("#main").height($("#auroHeight").height());
        });

        $(this).hide();
        $('.show').hide();
        $('.hide').show();
        $('.icon-ShowHide.hide').removeAttr('style');
        $('#edit1').hide();
        $('#edit2').hide();
        $('.hideTop').hide(); //แก้ไขปุ่ม edit&save ตัวบน
        $('.hideButtom').hide(); //แก้ไขปุ่ม edit&save ตัวล่าง
        $('.lineShow').hide();
        $('.lineHide').show();
        $('#submitAll').css('display', 'inline-block');
        $('#cancelAll').css('display', 'inline-block');
    });

    /*-----------------------------edit1 (ตัวบน) ------------------------------*/
    $('#edit1').click(function () {
        $("#company_his").slideDown();
        $(this).hide();
        $('.show1').hide(); //อยุ่ใน input ในฟอร์มข้อมูลประวัติบริษัท
        $('.hide1').show(); //อยุ่ใน input ในฟอร์มข้อมูลประวัติบริษัท
        $('.hideTop').show(); //แก้ไขปุ่ม edit&save ตัวบน
        $('#editAll').hide();
    });

    $('#btn_cancle').click(function () {
        $('#edit1').show();
        $('.show1').show();
        $('.hide1').hide();
        $('.hideTop').hide(); //แก้ไขปุ่ม edit&save ตัวบน
        $('#editAll').show();
        $('label.error').remove();
        $('div,label').removeClass('error');
    });

    $("#btn_save").click(function () {
        data = {
            CompHistory: tinyMCE.get('CompHistory').getContent(),
            Type:1
        }
        OpenLoading(true);
        $.ajax({
            url: GetUrl("Company/CompanyHistory"),
            data: data,
            type: "POST",
            success: function (data) {
                if (data["result"]) {
                    OpenLoading(false);
                    bootbox.alert(label.vldsave_success);
                    //$("#RowVersion").val(data["RowVersion"]);
                    if (tinyMCE.get('CompHistory').getContent() == "") {
                        $('#lblCompHistory').html(label.vldno_data);
                    } else {
                        $('#lblCompHistory').html(tinyMCE.get('CompHistory').getContent());
                    }

                    $('.show1').show();
                    $('.control-group').removeClass("success error");
                    $('.hide1').hide();
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

    });

    /*----------------------------edit2 (ตัวล่าง) -------------------------------*/
    $('#edit2').click(function () {
        $("company_his2").slideDown();
        $(this).hide();
        $('.show2').hide();
        $('.hide2').show();
        $('.hideButtom').show();
        $('#editAll').hide();
    });

    $('#btn_cancle2').click(function () {
        $('#edit2').show();
        $('.show2').show();
        $('.hide2').hide();
        $('.hideButtom').hide(); //แก้ไขปุ่ม edit&save ตัวบน
        $('#editAll').show();
        $('label.error').remove();
        $('div,label').removeClass('error');

    });

    $("#btn_save2").click(function () {
        data = {
            CompFounder: $('#CompFounder').val(),
            CompOwner: $('#CompOwner').val(),
            YearEstablished: $('#YearEstablished').val(),
            EmployeeCount: $('#EmployeeCount').val(),
            Type:2
        }
        OpenLoading(true);
        $.ajax({
            url: GetUrl("Company/CompanyHistory"),
            data: data,
            type: "POST",
            success: function (data) {
                if (data["result"]) {
                    OpenLoading(false);
                    bootbox.alert(label.vldsave_success);
                    if ($('#CompFounder').val() == "") {
                        $('#lblCompFounder').text(label.vldno_data);
                    } else {
                        $('#lblCompFounder').text($('#CompFounder').val());
                    }
                    if ($('#CompOwner').val() == "") {
                        $('#lblCompOwner').text(label.vldno_data);
                    } else {
                        $('#lblCompOwner').text($('#CompOwner').val());
                    }
                    if ($('#YearEstablished').val() == "") {
                        $('#lblYearEstablished').text(label.vldno_data);
                    } else {
                        $('#lblYearEstablished').text($('#YearEstablished').val());
                    }
                    if ($('#EmployeeCount').val() == "") {
                        $('#lblEmployeeCount').text(label.vldno_data);
                    } else {
                        $('#lblEmployeeCount').text($('#EmployeeCount').val());
                    }
                    $('.show2').show();
                    $('.control-group').removeClass("success error");
                    $('.hide2').hide();
                    $('#editAll').show();
                    $('.hideButtom').hide();
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

    /*---------------------------------------*/
    $("ul.toggle > li").each(function (index) {
        if ($(this).attr("id") == 'tab_history') {
            $(this).addClass('active_template');
        }
        else {
            $(this).removeClass('active_template');
        }
    });

    $('#detail').scrollbars();
});