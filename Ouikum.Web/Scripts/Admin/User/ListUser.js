$(function () {
    $('.exp').click(function () {
        $('#Memberstatus').val($(this).val());
        $('.hidPageIndex').val(1);
        if ($(this).val() == 1) {
            $('#ExtendLifetimes').show();
            $('#SendMails_1').show();
            $('#SendMails_2').hide();
            $('#New').hide();
            $('#Delete').hide();
        }
        else if ($(this).val() == 2 || $(this).val() == 3) {
            $('#ExtendLifetimes').show();
            $('#SendMails_2').show();
            $('#SendMails_1').hide();
            $('#New').hide();
            $('#Delete').hide();
        } else {
            $('#ExtendLifetimes').hide();
            $('#SendMails_1').hide();
            $('#SendMails_2').hide();
            $('#New').show();
            $('#Delete').show();
        }
    });
    $('#Refresh').click(function () {
        $('.btn-group > .exp').removeClass('active');
    });
    $('.sendUser').live('click', function () {
        var index = $('.sendUser').index($(this));
        var compid = $('.sendUser').eq(index).attr('data-id');
        var email = $('.sendUser').eq(index).attr('data-email');
        setEmail(compid, email);
    });

    $('.btn-send-mail').live('click', function () {
        var compid = $('#comp-id').val();
        var compemail = $('#comp-email').val();
        data = {
            MemberWebID: parseInt(compid, 10),
            Email: compemail
        };
        OpenLoading(true);
        $.ajax({
            url: GetUrl("Admin/User/SendUser"),
            type: "POST",
            data: data,
            dataType: 'json',
            traditional: true,
            success: function (model) {
                OpenLoading(false);
                $('#EmailModal').modal('hide');
                bootbox.alert("ส่งข้อความเรียบร้อยแล้ว");
            },
            error: function () {
            }
        });

    });

})

function setEmail(id, email) {
    $('#comp-email').val(email);
    $('#comp-id').val(id);
}

function Close(ID) {
    $("#sendUser_" + ID).popover('hide');
}
/*------------------------------DelUSer-------------------------------------*/
function DelUSer(id, memid, rowversion) {
    var WebID = [];
    var MemID = [];
    var RowVersion = [];
    var chk = 0;
    var Check = [];
    if (id == null || id == "") {
        $(".grid > tbody > tr").each(function (index) {
            if ($(this).children().find(".cbxItem:checked").val() == "true") {
                Check[index] = "True";
            }
            else {
                Check[index] = "False";
            }
            WebID[index] = $(this).children().find(".hidPrimeID").val();
            MemID[index] = $(this).children().find(".hidMemberID").val();
            RowVersion[index] = $(this).children().find(".hidRowVersion").val();
        });
    } else {
        WebID[0] = id;
        MemID[0] = memid;
        RowVersion[0] = rowversion;
        Check[0] = "True";
    }
    for (var i = 0; i < Check.length; i++) {
        if (Check[i] == "True") {
            chk++;
        }
    }
    OpenLoading(true);
    if (WebID.length > 0 && chk > 0) {
        $.ajax({
            url: url + "User/DelData",
            data: { Check: Check, ID: WebID, MemID:MemID, RowVersion: RowVersion, PrimaryKeyName: "MemberWebID" },
            success: function (data) {
                if (data.Result) {
                    OpenLoading(false);
                    $(g_hidsubmit).eq(g_no).click();
                    alertMsg("Alert : ", "success", label.vlddel_success);
                    return true;
                } else {
                    return false;
                }
            },
            type: "POST", traditional: true
        });
    }
    else {
        alertMsg("Notice! ", "error", label.vldnotice_del);
        return false;
    }
}
function alertMsg(header, a_class, msg) {
    var b_header = header;
    var b_class = a_class;
    $("#information").addClass(' alert-' + b_class);
    $("#information > strong").text(b_header);
    $("#information > p").text(msg);
    $("#information").fadeIn();
    OpenLoading(false);
    $("#information").delay(3000).fadeOut(500);
}
   
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
        $("#" + id + "").attr("checked", "checked");
        $("#" + id + "").attr("value", "true");
    } else {
        $("#" + id + "").removeAttr("checked");
        $("#" + id + "").attr("value", "false");
    }
}

/**************************************Extend_lifetime***************************************/
function Extend_lifetime(value, memid, rowversion) {
    $('.close').click();
    var MemID = [];
    var RowVersion = [];
    var chk = 0;
    var Check = [];
        $(".grid > tbody > tr").each(function (index) {
            if ($(this).children().find(".cbxItem:checked").val() == "true") {
                Check[index] = "True";
            }
            else {
                Check[index] = "False";
            }
            MemID[index] = $(this).children().find(".hidMemberID").val();
            RowVersion[index] = $(this).children().find(".hidRowVersion").val();
        });
    for (var i = 0; i < Check.length; i++) {
        if (Check[i] == "True") {
            chk++;
        }
    }
    OpenLoading(true);
    if (chk > 0) {
        $.ajax({
            url: url + "User/ExtendLifetime",
            data: { Check: Check, Value: value, MemID: MemID, RowVersion: RowVersion},
            success: function (data) {
                if (data.Result) {
                    OpenLoading(false);
                    $(g_hidsubmit).eq(g_no).click();
                    alertMsg("Alert : ", "success", label.vlddel_success);
                    return true;
                } else {
                    return false;
                }
            },
            type: "POST", traditional: true
        });
    }
    else {
        alertMsg("Notice! ", "error", label.vldnotice_del);
        return false;
    }
}

/**************************************Send_mail********************************************/
function Send_mail(status, memid, rowversion) {
    $('.close').click();
    var MemID = [];
    var RowVersion = [];
    var chk = 0;
    var Check = [];
    $(".grid > tbody > tr").each(function (index) {
        if ($(this).children().find(".cbxItem:checked").val() == "true") {
            Check[index] = "True";
        }
        else {
            Check[index] = "False";
        }
        MemID[index] = $(this).children().find(".hidMemberID").val();
        RowVersion[index] = $(this).children().find(".hidRowVersion").val();
    });
    for (var i = 0; i < Check.length; i++) {
        if (Check[i] == "True") {
            chk++;
        }
    }
    if (chk == 1) {
        if (status == 1) {
            if (confirm(label.vldconfirm_NearExpire)) {
                OpenLoading(true);
                $.ajax({
                    url: url + "User/SendMailStatus",
                    data: { Check: Check, Status: status, MemID: MemID, RowVersion: RowVersion },
                    success: function (data) {
                        if (data.Result) {
                            OpenLoading(false);
                            $(g_hidsubmit).eq(g_no).click();
                            alertMsg("Alert : ", "success", label.vldsend_success);
                            return true;
                        } else {
                            OpenLoading(false);
                            return false;
                        }
                    },
                    type: "POST", traditional: true
                });
            }
        } else {
            if (confirm(label.vldconfirm_Expire)) {
                OpenLoading(true);
                $.ajax({
                    url: url + "User/SendMailStatus",
                    data: { Check: Check, Status: status, MemID: MemID, RowVersion: RowVersion },
                    success: function (data) {
                        if (data.Result) {
                            OpenLoading(false);
                            $(g_hidsubmit).eq(g_no).click();
                            alertMsg("Alert : ", "success", label.vldsend_success);
                            return true;
                        } else {
                            OpenLoading(false);
                            return false;
                        }
                    },
                    type: "POST", traditional: true
                });
            }
        }
    }
    else {
        //alertMsg("Notice! ", "error", label.vldnotice_del);
        bootbox.alert('ขออภัย! กรุณาเลือกเพียง 1 รายการเท่านั้น');
        OpenLoading(false);
        return false;
    }
}

/*-----------------------------------------------*/
function changeIstrust(id,obj) {
    var istrust = $(obj).attr('aria-valuetext');
    if (istrust == true || istrust == 'True' || istrust == 'true' || istrust == 1) {
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
        url: url + "User/ChangeIsTrust",
        data: { compid: id, istrust: istrust },
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
/*------------------------------------------------------*/
function ChangeRowFlag(obj) {
    if (obj.val() == 0) {
        obj.addClass("text_BlackList");
        obj.removeClass("text_NonActivated text_Activated text_BlockInfo text_Expire text_All");
    } else if (obj.val() == 1) {
        obj.addClass("text_NonActivated");
        obj.removeClass("text_BlackList text_Activated text_BlockInfo text_Expire text_All");
    } else if (obj.val() == 2) {
        obj.addClass("text_Activated");
        obj.removeClass("text_BlackList text_NonActivated text_BlockInfo text_Expire text_All");
    } else if (obj.val() == 3) {
        obj.addClass("text_BlockInfo");
        obj.removeClass("text_BlackList text_NonActivated text_Activated text_Expire text_All");
    } else if (obj.val() == 4) {
        obj.addClass("text_Expire");
        obj.removeClass("text_BlackList text_NonActivated text_Activated text_BlockInfo text_All");
    }
    else  {
        obj.addClass("text_All");
        obj.removeClass("text_BlackList text_NonActivated text_Activated text_BlockInfo text_Expire");
    }
}