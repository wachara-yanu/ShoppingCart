$(function () {
   
    $(".close").click(function () {
        $("#information").fadeOut();
    });

    var strHtml = "";
    $(".btnSendMessage").live('click', function () {

        var editorContent = tinyMCE.get('MsgDetail').getContent(); //get value from html editor
        var txtSubject = $("#txtSubject").val();

        if ($('ul#methodTags').find('li').length < 1) {
            OpenLoading(false);
            bootbox.alert(label.vldenter_recipient);
            return false;
        }
        else if (txtSubject == null || txtSubject == '') {
            OpenLoading(false);
            bootbox.alert(label.vldenter_subject);
            return false;
        }
        else if (editorContent == null || editorContent == '') {
            OpenLoading(false);
            bootbox.alert(label.vldenter_message);
            return false;
        } else {
            OpenLoading(true);
            return true;
        }
    });

    $("#btnDraft").live('click', function () {

        var editorContent = tinyMCE.get('MsgDetail').getContent(); //get value from html editor
        var txtSubject = $("#txtSubject").val();

        if (txtSubject == null || txtSubject == '') {
            OpenLoading(false);
            bootbox.alert(label.vldenter_subject);
            return false;
        }
        else if (editorContent == null || editorContent == '') {
            OpenLoading(false);
            bootbox.alert(label.vldenter_message);
            return false;
        } else {
            OpenLoading(true);
            return true;
        }
    });

    $("#btnSendDraft").live('click', function () {
        var ID = [];
        var Code = [];
        var Check = [];
        var count = 0;
        $(".grid > tbody > tr").each(function (index) {
            if ($(this).children().find(".cbxItem:checked").val() == "true") {
                count++;
                ID[0] = $(this).children().find(".hidPrimeID").val();
                Code[0] = $(this).children().find(".hidMessageCode").val();
            }
        });

        if ($(".cbxItem").is(':checked')) {
            if (count > 1) { bootbox.alert("กรุณาเลือกเพียง 1 รายการ"); } else { SendDraft(ID[0], Code[0]); }
        } else { bootbox.alert(label.vldno_item_selected); }
    });

    $("#btnReply").live('click', function () {
        var ID = [];
        var Code = [];
        var Check = [];
        var count = 0;
        $(".grid > tbody > tr").each(function (index) {
            if ($(this).children().find(".cbxItem:checked").val() == "true") {
                count++;
                ID[0] = $(this).children().find(".hidPrimeID").val();
                Code[0] = $(this).children().find(".hidMessageCode").val();
            }
        });

        if ($(".cbxItem").is(':checked')) {
            if (count > 1) { bootbox.alert("กรุณาเลือกเพียง 1 รายการ"); } else { RepFor(ID[0], Code[0], 'Reply') }
        } else { bootbox.alert(label.vldno_item_selected); }
    });

    $("#btnForward").live('click', function () {
        var ID = [];
        var Code = [];
        var Check = [];
        var count = 0;
        $(".grid > tbody > tr").each(function (index) {
            if ($(this).children().find(".cbxItem:checked").val() == "true") {
                count++;
                ID[0] = $(this).children().find(".hidPrimeID").val();
                Code[0] = $(this).children().find(".hidMessageCode").val();
            }
        });

        if ($(".cbxItem").is(':checked')) {
            if (count > 1) { bootbox.alert("กรุณาเลือกเพียง 1 รายการ"); } else { RepFor(ID[0], Code[0], 'Forward') }
        } else { bootbox.alert(label.vldno_item_selected); }
    });

    $("#btnCancel").live('click', function () {
        if ($('#MsgType').val() != null) {
            document.location.reload(true);
        } else {
            window.location = GetUrl('Message/List');
        }
    });

    $("#title").text($("#TypeMessage").val());
});


function SendDraft(id, code) {
    OpenLoading(true);
    window.location = GetUrl('Message/Detail?MessageID=' + id + "&MessageCode=" + code + "&MsgType=Draftbox");
}

function RepFor(id , code , type) {
    OpenLoading(true);
    if (type == "Forward") {
        window.location = GetUrl('Message/Detail?MessageID=' + id + "&MessageCode=" + code + "&MsgType=Inbox");
    } else {
        window.location = GetUrl('Message/Detail?MessageID=' + id + "&MessageCode=" + code + "&MsgType=Inbox");
    }
}
function MessageDisplaySuccess() {
    OpenLoading(false);
    var status = $("#msgstatus").val();
    if (status == "Draft") {
        bootbox.alert(label.vlddraft_success);
    } else {
        bootbox.alert(label.vldsend_success);
    }
    document.location.reload(true);
}

/*------------------------------MarkRead-------------------------------------*/
function MarkRead(id, RowVersion) {
    if ($(".cbxItem").is(':checked')) {
        var ID = [];
        var RowVersion = [];
        var Check = [];
        if (id == null || id == "") {
            $(".grid > tbody > tr").each(function (index) {
                if ($(this).children().find(".cbxItem:checked").val() == "true") {
                    Check[index] = "True";
                }
                else {
                    Check[index] = "False";
                }
                ID[index] = $(this).children().find(".hidPrimeID").val();
                RowVersion[index] = $(this).children().find(".hidRowVersion").val();
            });
        } else {
            ID[0] = id;
            RowVersion[0] = RowVersion;
            Check[0] = "True";
        }
        OpenLoading(true);
        if (ID.length > 0) {
            $.ajax({
                url: url + "Message/MarkRead",
                data: { Check: Check, ID: ID, RowVersion: RowVersion, PrimaryKeyName: "MessageID" },
                success: function (data) {
                    if (data == "True" || data == true || data == "Success") {
                        OpenLoading(false);
                        $(g_hidsubmit).eq(g_no).click();
                        $("#information > #message").text(label.vldmark_unread_success);
                        $("#information").fadeIn();
                        $("#information").delay(3000).fadeOut(500);
                        return true;
                    } else {
                        return false;
                    }
                },
                type: "POST", traditional: true
            });
        }
        else {
            return false;
        }
    } else { bootbox.alert(label.vldno_item_selected) }
}

/*------------------------------DelData-------------------------------------*/
function DelData(id, RowVersion) {
    var ID = [];
    var RowVersion = [];
    var Check = [];
    if (id == null || id == "") {
        $(".grid > tbody > tr").each(function (index) {
            if ($(this).children().find(".cbxItem:checked").val() == "true") {
                Check[index] = "True";
            }
            else {
                Check[index] = "False";
            }
            ID[index] = $(this).children().find(".hidPrimeID").val();
            RowVersion[index] = $(this).children().find(".hidRowVersion").val();
        });
    } else {
        ID[0] = id;
        RowVersion[0] = RowVersion;
        Check[0] = "True";
    }
    OpenLoading(true);
    if (ID.length > 0) {
        $.ajax({
            url: url + "Message/DelData",
            data: { Check: Check, ID: ID, RowVersion: RowVersion, PrimaryKeyName: "MessageID" },
            success: function (data) {
                if (data.Result) {
                    OpenLoading(false);
                    $("#Importance").text("Importance (" + data.CountImportance + ")");
                    if (data.CountInbox > 0) { $("#Inbox").text("Inbox (" + data.CountInbox + ")"); }
                    $("#Draftbox").text("Draftbox (" + data.CountDraftbox + ")");
                    $("#Sentbox").text("Sentbox (" + data.CountSentbox + ")");
                    $(g_hidsubmit).eq(g_no).click(); // Submit Grid
                    $("#information > #message").text(label.vlddel_msg_success);
                    $("#information").fadeIn();
                    $("#information").delay(3000).fadeOut(500);
                    return true;
                } else {
                    return false;
                }
            },
            type: "POST", traditional: true
        });
    }
    else {
        return false;
    }
}
function DelMessData(id, version) {
    var ID = [];
    var RowVersion = [];
    var Check = [];
    if (id == null || id == "") {
        $(".grid > tbody > tr").each(function (index) {
            if ($(this).children().find(".cbxItem:checked").val() == "true") {
                Check[index] = "True";
            }
            else {
                Check[index] = "False";
            }
            ID[index] = $(this).children().find(".hidPrimeID").val();
            RowVersion[index] = $(this).children().find(".hidRowVersion").val();
        });
    } else {
        ID[0] = id;
        RowVersion[0] = version;
        Check[0] = "True";
    }
    OpenLoading(true);
    if (ID.length > 0) {
        $.ajax({
            url: url + "Message/DelMessData",
            data: { Check: Check, ID: ID, RowVersion: RowVersion, PrimaryKeyName: "MessageID" },
            success: function (data) {
                if (data.Result) {
                    OpenLoading(false);
                    $("#Importance").text("Importance (" + data.CountImportance + ")");
                    if (data.CountInbox > 0) { $("#Inbox").text("Inbox (" + data.CountInbox + ")"); }
                    $("#Draftbox").text("Draftbox (" + data.CountDraftbox + ")");
                    $("#Sentbox").text("Sentbox (" + data.CountSentbox + ")");
                    $(g_hidsubmit).eq(g_no).click(); // Submit Grid
                    $("#information > #message").text(label.vlddel_msg_success);
                    $("#information").fadeIn();
                    $("#information").delay(3000).fadeOut(500);

                    window.location = GetUrl('Message/List?MsgType=Trash');

                    return true;
                } else {
                    return false;
                }
            },
            type: "POST", traditional: true
        });
    }
    else {
        return false;
    }
}
function DelAllData(id, RowVersion) {
    OpenLoading(true);
    $.ajax({
        url: url + "Message/DelAllData",
        //data: { Check: Check, ID: ID, RowVersion: RowVersion, PrimaryKeyName: "MessageID" },
        success: function (data) {
            if (data.Result) {
                OpenLoading(false);
                $("#Importance").text("Importance (" + data.CountImportance + ")");
                if (data.CountInbox > 0) { $("#Inbox").text("Inbox (" + data.CountInbox + ")"); }
                $("#Draftbox").text("Draftbox (" + data.CountDraftbox + ")");
                $("#Sentbox").text("Sentbox (" + data.CountSentbox + ")");
                $(g_hidsubmit).eq(g_no).click(); // Submit Grid
                $("#information > #message").text(label.vlddel_msg_success);
                $("#information").fadeIn();
                $("#information").delay(3000).fadeOut(500);
                return true;
            } else {
                return false;
            }
        },
        type: "POST", traditional: true
    });
}

function delMessage() {
    if ($(".cbxItem").is(':checked')) {
        if (confirm(label.vldconfirm_del_data)) { DelData(); }
    } else { bootbox.alert(label.vldno_item_selected); }
}
function delMess() {
    if ($(".cbxItem").is(':checked')) {
        if (confirm(label.vldconfirm_del_data)) { DelMessData(); }
    } else { bootbox.alert(label.vldno_item_selected); }
}
function delMessDetail() {
    var id = $(".hidPrimeID").val();
    var version = $(".hidRowVersion").val();
    if (confirm(label.vldconfirm_del_data)) {
        DelMessData(id, version)
    }
}

function delAllMessage() {
    if (confirm("ต้องการล้างข้อมูลหรือไม่")) { DelAllData(); }
}

/*------------------------------MoveData-------------------------------------*/
function Move() {
    var ID = [];
    var RowVersion = [];
    var MsgStatus = [];
    var Check = [];
    var count = 0;
    $(".grid > tbody > tr").each(function (index) {
        if ($(this).children().find(".cbxItem:checked").val() == "true") {
            Check[index] = "True";
            count++;
            MsgStatus[0] = $(this).children().find(".hidMsgStatus").val();
        }
        else {
            Check[index] = "False";
        }
        ID[index] = $(this).children().find(".hidPrimeID").val();
        RowVersion[index] = $(this).children().find(".hidRowVersion").val();
    });
    if (count == 1) {
        var text = "";
        if (MsgStatus[0] == "1") {
            text = "ข้อความเข้า";
        } else if (MsgStatus[0] == "2") {
            text = "ส่งแล้ว";
        } else if (MsgStatus[0] == "3") {
            text = "ร่าง";
        }
        if ($(".cbxItem").is(':checked')) {
            if (confirm("ต้องการย้ายข้อมูลกลับไปยัง '" + text + "' หรือไม่")) {
                OpenLoading(true);
                if (ID.length > 0) {
                    $.ajax({
                        url: url + "Message/MoveData",
                        data: { Check: Check, ID: ID, RowVersion: RowVersion, PrimaryKeyName: "MessageID"},
                        success: function (data) {
                            if (data.Result) {
                                OpenLoading(false);
                                $("#Importance").text("Importance (" + data.CountImportance + ")");
                                if (data.CountInbox > 0) { $("#Inbox").text("Inbox (" + data.CountInbox + ")"); }
                                $("#Draftbox").text("Draftbox (" + data.CountDraftbox + ")");
                                $("#Sentbox").text("Sentbox (" + data.CountSentbox + ")");
                                $(g_hidsubmit).eq(g_no).click(); // Submit Grid
                                $("#information > #message").text('ย้ายข้อความสำเร็จ');
                                $("#information").fadeIn();
                                $("#information").delay(3000).fadeOut(500);
                                return true;
                            } else {
                                return false;
                            }
                        },
                        type: "POST", traditional: true
                    });
                }
                else {
                    return false;
                }
            }
        } else { bootbox.alert(label.vldno_item_selected); }
    } else {
        bootbox.alert("กรุณาเลือกเพียง 1 รายการ");
    }
}
function MoveDetail() {
    ID = $(".hidPrimeID").val();
    RowVersion = $(".hidRowVersion").val();

    var text = "";
    if ($(".hidMsgStatus").val() == "1") {
        text = "ข้อความเข้า";
    } else if ($(".hidMsgStatus").val() == "2") {
        text = "ส่งแล้ว";
    } else if ($(".hidMsgStatus").val() == "3") {
        text = "ร่าง";
    }
    
    if (confirm("ต้องการย้ายข้อมูลกลับไปยัง '" + text + "' หรือไม่")) {
        OpenLoading(true);
        if (ID.length > 0) {
            $.ajax({
                url: url + "Message/MoveDataDetail",
                data: { ID: ID, RowVersion: RowVersion, PrimaryKeyName: "MessageID" },
                success: function (data) {
                    if (data.Result) {
                        OpenLoading(false);
                        $("#Importance").text("Importance (" + data.CountImportance + ")");
                        if (data.CountInbox > 0) { $("#Inbox").text("Inbox (" + data.CountInbox + ")"); }
                        $("#Draftbox").text("Draftbox (" + data.CountDraftbox + ")");
                        $("#Sentbox").text("Sentbox (" + data.CountSentbox + ")");
                        $(g_hidsubmit).eq(g_no).click(); // Submit Grid
                        $("#information > #message").text('ย้ายข้อความสำเร็จ');
                        $("#information").fadeIn();
                        $("#information").delay(3000).fadeOut(500);

                        window.location = GetUrl('Message/List?MsgType=Trash');

                        return true;
                    } else {
                        return false;
                    }
                },
                type: "POST", traditional: true
            });
        }
        else {
            return false;
        }
    }
    
   
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

function htmlEscape(str) {
    return String(str).replace(/&/g, '&amp;').replace(/"/g, '&quot;').replace(/'/g, '&#39;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
}

//Set IsImportance
function chkImportance(Obj) {
    if (Obj.attr("value") == true || Obj.attr("checked") == "checked") {
        Obj.attr("checked", "checked");
        Obj.attr("value", "true");
    } else {
        Obj.attr("value", "false");
        Obj.removeAttr("checked");
    }
}

function ChangeImportantList() {
    if ($(".cbxItem").is(':checked')) {
        var ID = [];
        var RowVersion = [];
        var Check = [];
       
        $(".grid > tbody > tr").each(function (index) {
            if ($(this).children().find(".cbxItem:checked").val() == "true") {
                Check[index] = "True";
            }
            else {
                Check[index] = "False";
            }
            ID[index] = $(this).children().find(".hidPrimeID").val();
            RowVersion[index] = $(this).children().find(".hidRowVersion").val();
        });
        
        OpenLoading(true);
        if (ID.length > 0) {
            $.ajax({
                url: url + "Message/ChangeTagList",
                data: { Check: Check, ID: ID, RowVersion: RowVersion, PrimaryKeyName: "MessageID" },
                success: function (data) {
                    if (data != null) {
                        OpenLoading(false);
                        $("#Importance").text("Importance (" + data.CountImportance + ")");
                        if (data.CountInbox > 0) { $("#Inbox").text("Inbox (" + data.CountInbox + ")"); }
                        $("#Draftbox").text("Draftbox (" + data.CountDraftbox + ")");
                        $("#Sentbox").text("Sentbox (" + data.CountSentbox + ")");
                        $(g_hidsubmit).eq(g_no).click(); // Submit Grid
                        $("#information > #message").text(label.vldmark_unimportance_success);
                        $("#information").fadeIn();
                        $("#information").delay(3000).fadeOut(500);
                        return true;
                    }
                    else {
                        bootbox.alert(label.vldsave_unsuccess);
                    }
                },
                type: "POST", traditional: true
            });
        }
        else {
            return false;
        }
    } else { bootbox.alert(label.vldno_item_selected) }
}
