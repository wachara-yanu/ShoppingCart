function CheckBoxall(Obj) {
    if (Obj.attr("value") == true || Obj.attr("checked") == "checked") {
        $(".cbxMemberID").attr("checked", "checked");
        $(".cbxMemberID").attr("value", "true");
        Obj.attr("checked", "checked");
        Obj.attr("value", "true");

    } else {
        $(".cbxMemberID").removeAttr("checked");
        $(".cbxMemberID").attr("value", "false");
        Obj.removeAttr("checked");
        Obj.attr("value", "false");
    }

}
function CheckBox(id) {

    if ($("#" + id + "").attr("value") == true || $("#" + id + "").attr("checked") == "checked") {
        if ($("#CheckAll").attr("value") == true || $("#CheckAll").attr("checked") == "checked") {
            $("#" + id + "").attr("checked", "checked");
            $("#" + id + "").attr("value", "true");
        } else {
            $(".cbxMemberID").removeAttr("checked");
            $(".cbxMemberID").attr("value", "false");
            $("#" + id + "").attr("checked", "checked");
            $("#" + id + "").attr("value", "true");
        }
    }
}

// Member Index
$(".DeleteMembers").click(function () {
    var MemberID = new Array();
    //        var RowVersion = new Array();        

    for (x = 0; x < $(".cbxMemberID").length; x++) {
        if ($(".cbxMemberID").eq(x).attr("checked") == "checked" || $(".cbxMemberID").eq(x).attr("checked") == true) {
            MemberID[MemberID.length] = $(".cbxMemberID").eq(x).val();
            //                RowVersion[RowVersion.length] = $(".RowVersionID").eq(x).val();                                               
        }
    }
    if (MemberID != "") {
        if (confirm(label.vldconfirm_del_mem) == true) {
            //                $("#GridLoad").html("<img src='../../Content/Default/Images/icon_loading.gif' alt='' width='13' height='13'>");
            //                DeleteContact(ContactID, RowVersion);
        }
    }
    else {
        bootbox.alert(label.vldnotice_del);
    }
});
