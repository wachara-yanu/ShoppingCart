/*---------------------------EditBlog---------------------------------*/
//function EditBlog(id) {
//    $.ajax({
//        url: GetUrl("ArticleType/EditBlog"),
//        data: { ArticleTypeID: id },
//        type: "POST",
//        datatype: "json",
//        traditional: true,
//        success: function (data) {
//            if (data != null) {
//                var urlArticle = GetUpload("Article/" + data.CompID + "/" + data.ArticleID + "/" + data.ImgPath + "");
//                $("h3#head_form").html("EDIT ARTICLE");
//                $('#submit').removeAttrs('disabled');
//                $(".SearchBlog").val("");
//                $('#objState').val(2);
//                $('#RowVersion').val(data.RowVersion);
//                $("#ArticleTypeID option[value=" + data.ArticleTypeID + "]").attr("selected", "selected");
//                $('#ArticleTypeName').val(data.ArticleTypeName);
//                $('.control-group').removeClass("success error");
//                $("#Add_Edit").slideDown(function () {
//                    $("#sidebar").height($("#autoHeight").height());
//                    $("#main").height($("#autoHeight").height());
//                });
//            } //end if
//            else {
//                bootbox.alert(label.vldsave_unsuccess);
//            }
//        },
//        error: function () {
//            //bootbox.alert(label.vldcannot_check_info);
//        }
//    });
//}
///*------------------------------DelArticle-------------------------------------*/
//function DelArticleType(articleid, rowversion) {
//    var ArticleTypeID = [];
//    var RowVersion = [];
//    var chk = 0;
//    var Check = [];
//    if (articleid == null || articleid == "") {
//        $(".grid > tbody > tr").each(function (index) {
//            if ($(this).children().find(".cbxItem:checked").val() == "true") {
//                Check[index] = "True";
//            }
//            else {
//                Check[index] = "False";
//            }
//            ArticleTypeID[index] = $(this).children().find(".hidPrimeID").val();
//            RowVersion[index] = $(this).children().find(".hidRowVersion").val();
//        });
//    } else {
//        ArticleTypeID[0] = articleid;
//        RowVersion[0] = rowversion;
//        Check[0] = "True";
//    }
//    for (var i = 0; i < Check.length; i++) {
//        if (Check[i] == "True") {
//            chk++;
//        }
//    }
//    OpenLoading(true);
//    if (chk > 0) {
//        $.ajax({
//            url: url + "ArticleType/DelData",
//            data: { Check: Check, ArticleID: ArticleID, RowVersion: RowVersion, PrimaryKeyName: "ArticleID" },
//            success: function (data) {
//                if (data.Result) {
//                    OpenLoading(false);
//                    $(g_hidsubmit).eq(g_no).click();
//                    alertMsg("Alert : ", "success", label.vlddel_success);
//                    return true;
//                } else {
//                    return false;
//                }
//            },
//            type: "POST", traditional: true
//        });
//    }
//    else {
//        alertMsg("Notice! ", "error", label.vldnotice_del);
//        return false;
//    }
//}
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