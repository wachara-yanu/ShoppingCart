function CheckBoxall(Obj) {
    if (Obj.attr("value") == true || Obj.attr("checked") == "checked") {
        $(".cbxProductCateID").attr("checked", "checked");
        $(".cbxProductCateID").attr("value", "true");
        Obj.attr("checked", "checked");
        Obj.attr("value", "true");

    } else {
        $(".cbxProductCateID").removeAttr("checked");
        $(".cbxProductCateID").attr("value", "false");
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
            $(".cbxProductCateID").removeAttr("checked");
            $(".cbxProductCateID").attr("value", "false");
            $("#" + id + "").attr("checked", "checked");
            $("#" + id + "").attr("value", "true");
        }
    }
}

// ProductCategory Index
$(".DeleteProductCates").click(function () {
    var ProductCateID = new Array();
    //        var RowVersion = new Array();        

    for (x = 0; x < $(".cbxProductCateID").length; x++) {
        if ($(".cbxProductCateID").eq(x).attr("checked") == "checked" || $(".cbxProductCateID").eq(x).attr("checked") == true) {
            ProductCateID[ProductCateID.length] = $(".cbxProductCateID").eq(x).val();
            //                RowVersion[RowVersion.length] = $(".RowVersionID").eq(x).val();                                               
        }
    }
    if (ProductCateID != "") {
        if (confirm(label.vldconfirm_del_cate) == true) {
            //                $("#GridLoad").html("<img src='../../Content/Default/Images/icon_loading.gif' alt='' width='13' height='13'>");
            //                DeleteContact(ContactID, RowVersion);
        }
    }
    else {
        bootbox.alert(label.vldnotice_del);
    }
});