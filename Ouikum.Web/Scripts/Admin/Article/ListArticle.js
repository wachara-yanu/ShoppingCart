/*---------------------------EditBlog---------------------------------*/
function EditBlog(id) {
    $.ajax({
        url: GetUrl("Article/EditBlog"),
        data: { ArticleID: id },
        type: "POST",
        datatype: "json",
        traditional: true,
        success: function (data) {
            if (data != null) {
                var urlArticle = GetUpload("Article/" + data.CompID + "/" + data.ArticleID + "/" + data.ImgPath + "");
                $("h3#head_form").html("EDIT ARTICLE");
                $('#submit').removeAttrs('disabled');
                $(".SearchBlog").val("");
                $('#objState').val(2);
                $('#RowVersion').val(data.RowVersion);
                $('#ArticleID').val(data.ArticleID);
                $("#ArticleTypeID option[value=" + data.ArticleTypeID + "]").attr("selected", "selected");
                $('#ArticleName').val(data.ArticleName);
                $('#Description').val(data.Description);
                tinyMCE.get('Description').setContent(data.Description);
                $('#ShortDescription').val(data.ShortDescription);
                tinyMCE.get('ShortDescription').setContent(data.ShortDescription);
                $("#ImgPath").val(data.ImgPath);
                if (data.ImgPath != null && data.ImgPath != "") {
                    $("#img_ImgPath").attr("src", urlArticle);
                    $("#img_ImgPath").css("height", "150px");
                } else {
                    $("#img_ImgPath").attr("src", "http://www.placehold.it/180x120/EFEFEF/AAAAAA&text=no+image");
                }
                $('.control-group').removeClass("success error");
                $("#Add_Edit").slideDown(function () {
                    $("#sidebar").height($("#autoHeight").height());
                    $("#main").height($("#autoHeight").height());
                });
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
/*------------------------------DelArticle-------------------------------------*/
function DelArticle(articleid, rowversion) {
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
            url: url + "Article/DelData",
            data: { Check: Check, ArticleID: ArticleID, RowVersion: RowVersion, PrimaryKeyName: "ArticleID" },
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


/*----------------------------------------- Hide/Show ---------------------------------------*/

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
        url: url + "Article/ChangeIsShow",
        data: { ArticleID: id, istrust: istrust },
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




//#endregion