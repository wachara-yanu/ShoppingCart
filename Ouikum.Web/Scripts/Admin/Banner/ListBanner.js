function ChangeIsShow(id, obj) {
    var Icon = $(obj).find("i")
    if (Icon.hasClass("icon-eye-open")) {
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
        url: url + "Banner/ChangeIsShow",
        data: { bannerId: id, istrust: istrust },
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

function DelData(id, rowversion, PrimaryKeyName, Controller) {
    var ID = [];
    var RowVersion = [];
    var Check = [];
    var chk = 0;
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
        RowVersion[0] = rowversion;
        Check[0] = "True";
    }
    for (var i = 0; i < Check.length; i++) {
        if (Check[i] == "True") {
            chk++;
        }
    }
    OpenLoading(true);
    if (ID.length > 0 && chk > 0) {
        $.ajax({
            url: url + Controller + "/DelData",
            data: { Check: Check, ID: ID, RowVersion: RowVersion, PrimaryKeyName: PrimaryKeyName },
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
        $("#information").removeClass(' alert-success');
        alertMsg("Notice! ", "error", label.vldconfirm_del_data);
        return false;
    }
}
function DelBanner(articleid, rowversion) {
    var ArticleID = [];
    var RowVersion = [];
    var chk = 0;
    var Check = [];
    if (articleid == null || articleid == "") {
        $(".grid > tbody > tr").each(function (index) {
            if ($(this).children().find(".cbxItem:checked").val() == "true") {
                Check[index] = "True";
            }
            else {
                Check[index] = "False";
            }
            ArticleID[index] = $(this).children().find(".hidPrimeID").val();
            RowVersion[index] = $(this).children().find(".hidRowVersion").val();
        });
    } else {
        ArticleID[0] = articleid;
        RowVersion[0] = rowversion;
        Check[0] = "True";
    }
    for (var i = 0; i < Check.length; i++) {
        if (Check[i] == "True") {
            chk++;
        }
    }
    OpenLoading(true);
    if (chk > 0) {
        $.ajax({
            url: url + "Banner/DelData",
            data: { Check: Check, ID: ArticleID, RowVersion: RowVersion, PrimaryKeyName: "BannerId" },
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
//#endregion