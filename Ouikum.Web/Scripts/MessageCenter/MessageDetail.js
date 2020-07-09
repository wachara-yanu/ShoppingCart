$(function () {
    tinyMCE.init({
        // General options
        mode: "textareas",
        theme: "advanced",
        height: "350",
        width: "100%",
        plugins: "autolink,lists,spellchecker,pagebreak,style,layer,table,save,advhr,advimage,advlink,emotions,iespell,inlinepopups,insertdatetime,preview,media,searchreplace,print,contextmenu,paste,directionality,fullscreen,noneditable,visualchars,nonbreaking,xhtmlxtras,template",

        // Theme options
        theme_advanced_buttons1: "bold,italic,underline,strikethrough,|,justifyleft,justifycenter,justifyright,justifyfull,|,formatselect,fontselect,fontsizeselect",
        theme_advanced_buttons2: "cut,copy,paste,pastetext,pasteword,|,search,|,bullist,numlist,|,outdent,indent,|,undo,redo,|,link,unlink,image,cleanup,code,|,preview,|,forecolor,backcolor",
        theme_advanced_buttons3: "tablecontrols,|,hr,removeformat,visualaid,|,sub,sup,|,iespell,media,|,fullscreen",
        theme_advanced_toolbar_location: "top",
        theme_advanced_toolbar_align: "left",
        theme_advanced_statusbar_location: "bottom",
        theme_advanced_resizing: false
    });
});

/*------------------------------ChangeImportant-------------------------------------*/
function ChangeImportant(id, TypeMessage) {
    OpenLoading(true);
    console.log($("#IsFav").val());
    $.ajax({
        url: GetUrl("Message/ChangeTag"),
        data: { id: id, IsFavorite: $("#IsFav").val(), TypeMessage: TypeMessage },
        type: "POST",
        datatype: "json",
        traditional: true,
        success: function (data) {
            if (data != null) {
                OpenLoading(false);
                $("#Importance").text("Importance (" + data.CountImportance + ")");
                $(g_hidsubmit).eq(g_no).click();
                //if ($("#IsFav").val() == 'true') {
                    $("#information > #message").text(label.vldmark_importance_success);
                    $(".importantstatus").removeClass('icon-tag');
                    $(".importantstatus").addClass('icon-tags');
                    $(".importantstatus").attr('title', 'Important');
                    $("#IsFav").val(false);
                //} else {
                //    $("#information > #message").text(label.vldmark_unimportance_success);
                //    $(".importantstatus").addClass('icon-tag');
                //    $(".importantstatus").removeClass('icon-tags');
                //    $(".importantstatus").attr('title', 'Unimportant');
                //    $("#IsFav").val(true);
                //}
                window.location.reload();
                $("#information").fadeIn();
            }
            else {
                bootbox.alert(label.vldsave_unsuccess);
            }
        },
        error: function () {
            bootbox.alert(label.vldcannot_check_info);
        }
    });
}
/*------------------------------MarkRead-------------------------------------*/
function MarkReadDetail(id, rowversion) {
    var ID = [];
    var RowVersion = [];
    var Check = [];
    ID[0] = id;
    RowVersion[0] = rowversion;
    Check[0] = "True";
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
/*------------------------------DelData-------------------------------------*/
function DelDataDetail(id, rowversion, type) {
    var ID = [];
    var RowVersion = [];
    var Check = [];
    ID[0] = id;
    RowVersion[0] = rowversion;
    Check[0] = "True";
    OpenLoading(true);
    if (ID.length > 0) {
        $.ajax({
            url: url + "Message/DelData",
            data: { Check: Check, ID: ID, RowVersion: RowVersion, PrimaryKeyName: "MessageID" },
            success: function (data) {
                if (data.Result) {
                    OpenLoading(false);
                    $("#Importance").text("Importance (" + data.CountImportance + ")");
                    $("#Inbox").text("Inbox (" + data.CountInbox + ")");
                    $("#Draftbox").text("Draftbox (" + data.CountDraftbox + ")");
                    $("#Sentbox").text("Sentbox (" + data.CountSentbox + ")");
                    $(g_hidsubmit).eq(g_no).click(); // Submit Grid
                    window.location = GetUrl("Message/List?MsgType=" + type);
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

function ReplyForward(id, type) {
    $("#sidebar").css("height", "1300px");
    $("#main").css("height", "1300px");
    $.ajax({
        url: GetUrl("Message/GetMessageDetail"),
        type: "POST",
        data: { msgid: parseInt(id), type: type },
        dataType: "JSON",
        success: function (data) {
            if (type == "Reply") {
                $("#MessageDetail").addClass("hide");
                $("#MessageDetailContent").addClass("hide");
                $("#ReplyOrForward").removeClass("hide");
                $("#txtSubject").val(data["msgSubject"]);
                tinyMCE.activeEditor.setContent(data["msgDetail"]);
                var CompID = parseInt(data["msgFromCompID"]);
                if (CompID > 0) {
                    if ($("li#" + CompID).length < 1) {
                        var tagName = "<li id='" + CompID + "'  class='ContactNameTag'><span class='mar_r5'>" + data["msgFromCompName"] + "</span></li>"
                        $("#methodTags").append(tagName);
                    }
                } else {
                    if ($("li#" + CompID).length < 1) {
                        var tagName = "<li id='" + CompID + "'  class='ContactNameTag'><span class='mar_r5'>" + data["emailNotMember"] + "</span></li>"
                        $("#methodTags").append(tagName);
                    }
                    $("#hidEmailNotMember").val(data["emailNotMember"]);
                    $("#hidNameNotMember").val(data["nameNotMember"]);
                }
                $("#hidMsgID").val(id);
                $("#msgstatus").val(type);
                $("#hidToCompID").val(CompID);
                $("#hidImgFileName").val(data["msgFileName"]);
                $(".t-filename").html(data["msgFileName"]);
                $("#methodTags").removeClass("ContactListModal");
                if (data["msgFileName"] != null) {
                    $("#hidFile").removeClass("hide");
                    var text = "<ul class=\"t-upload-files t-reset\"><li class=\"t-file\"><span class=\"t-icon t-success\">uploaded</span><span class=\"t-filename\" title=\"" +
                        data["msgFileName"] + "\">" + data["msgFileName"] + "<span class=\"t-progress\"><span class=\"t-progress-status\" style=\"width: 100%;\"></span></span>" +
                        "</span></li></ul>";
                    $("#hidFile").html(text);
                }

            } else if (type == "Forward") {
                $("#MessageDetail").addClass("hide");
                $("#MessageDetailContent").addClass("hide");
                $("#ReplyOrForward").removeClass("hide");
                $("#txtSubject").val(data["msgSubject"]);
                tinyMCE.activeEditor.setContent(data["msgDetail"]);
                $("#hidMsgID").val(id);
                $("#msgstatus").val(type);
                $("#hidToCompID").val("");
                $("#hidImgFileName").val(data["msgFileName"]);
                $(".t-filename").html(data["msgFileName"]);
                $("#methodTags").children().remove();
                if (data["msgFileName"] != null) {
                    $("#hidFile").removeClass("hide");
                    var text = "<ul class=\"t-upload-files t-reset\"><li class=\"t-file\"><span class=\"t-icon t-success\">uploaded</span><span class=\"t-filename\" title=\"" +
                        data["msgFileName"] + "\">" + data["msgFileName"] + "<span class=\"t-progress\"><span class=\"t-progress-status\" style=\"width: 100%;\"></span></span>" +
                        "</span></li></ul>";
                    $("#hidFile").html(text);
                }
            }
            //$('html, body').animate({ scrollTop: $("#ReplyOrForward").offset().top }, 2000);
        }
    });
}

function Draft(id, type) {

    $.ajax({
        url: GetUrl("Message/GetDraftDetail"),
        type: "POST",
        data: { msgid: parseInt(id)},
        dataType: "JSON",
        success: function (data) {
            $("#MessageDetail").addClass("hide");
            $("#ReplyOrForward").removeClass("hide");
            $("#txtSubject").val(data["msgSubject"]);
            tinyMCE.activeEditor.setContent(data["msgDetail"]);
            var CompID = parseInt(data["msgToCompID"]);
            if ($("li#" + CompID).length < 1 && CompID > 0) {
                var tagName = "<li id='" + CompID + "'  class='ContactNameTag'><span class='mar_r5'>" + data["msgToCompName"] + "</span></li>"
                $("#methodTags").append(tagName);
            }
            $("#hidMsgID").val(id);
            $("#hidMsgStatus").val("New");
            $("#hidToCompID").val(CompID);

            $("#methodTags").removeClass("ContactListModal");
        }
    });
}