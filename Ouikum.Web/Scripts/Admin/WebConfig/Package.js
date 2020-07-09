function PackageChange(optionval) {
    if (optionval == "1") {
        $(".showbtn").hide();
        $(".packageOption").slideDown();

    } else if (optionval == "0") {
        $(".showbtn").show();
        $(".packageOption").hide();
    }

}

// Checkbox Package //
function CheckAllPackage(Obj) {
    if (Obj.attr("value") == true || Obj.attr("checked") == "checked") {
        $(".cbxPackageID").attr("checked", "checked");
        $(".cbxPackageID").attr("value", "true");
        Obj.attr("checked", "checked");
        Obj.attr("value", "true");

    } else {
        $(".cbxPackageID").removeAttr("checked");
        $(".cbxPackageID").attr("value", "false");
        Obj.removeAttr("checked");
        Obj.attr("value", "false");
    }

}
function CheckPackage(id) {

    if ($("#" + id + "").attr("value") == true || $("#" + id + "").attr("checked") == "checked") {
        if ($("#CheckAllPackage").attr("value") == true || $("#CheckAllPackage").attr("checked") == "checked") {
            $("#" + id + "").attr("checked", "checked");
            $("#" + id + "").attr("value", "true");
        } else {
            $(".cbxPackageID").removeAttr("checked");
            $(".cbxPackageID").attr("value", "false");
            $("#" + id + "").attr("checked", "checked");
            $("#" + id + "").attr("value", "true");
        }
    }
}

// Checkbox Package Group //
function CheckAllGroup(Obj) {
    if (Obj.attr("value") == true || Obj.attr("checked") == "checked") {
        $(".cbxPackageGroupID").attr("checked", "checked");
        $(".cbxPackageGroupID").attr("value", "true");
        Obj.attr("checked", "checked");
        Obj.attr("value", "true");

    } else {
        $(".cbxPackageGroupID").removeAttr("checked");
        $(".cbxPackageGroupID").attr("value", "false");
        Obj.removeAttr("checked");
        Obj.attr("value", "false");
    }

}
function CheckPackageGroup(id) {

    if ($("#" + id + "").attr("value") == true || $("#" + id + "").attr("checked") == "checked") {
        if ($("#CheckAllGroup").attr("value") == true || $("#CheckAllGroup").attr("checked") == "checked") {
            $("#" + id + "").attr("checked", "checked");
            $("#" + id + "").attr("value", "true");
        } else {
            $(".cbxPackageGroupID").removeAttr("checked");
            $(".cbxPackageGroupID").attr("value", "false");
            $("#" + id + "").attr("checked", "checked");
            $("#" + id + "").attr("value", "true");
        }
    }
}

// Delete Package
$(".DeletePackages").click(function () {
    var PackageID = new Array();
    //        var RowVersion = new Array();        

    for (x = 0; x < $(".cbxPackageID").length; x++) {
        if ($(".cbxPackageID").eq(x).attr("checked") == "checked" || $(".cbxPackageID").eq(x).attr("checked") == true) {
            PackageID[PackageID.length] = $(".cbxPackageID").eq(x).val();
            //                RowVersion[RowVersion.length] = $(".RowVersionID").eq(x).val();                                               
        }
    }
    if (PackageID != "") {
        if (confirm(label.vldconfirm_del_packet) == true) {
            //                $("#GridLoad").html("<img src='../../Content/Default/Images/icon_loading.gif' alt='' width='13' height='13'>");
            //                DeleteContact(ContactID, RowVersion);
        }
    }
    else {
        bootbox.alert(label.vldnotice_del);
    }
});

// Delete Package Group
$(".DeletePackageGroups").click(function () {
    var PackageGroupID = new Array();
    //        var RowVersion = new Array();        

    for (x = 0; x < $(".cbxPackageGroupID").length; x++) {
        if ($(".cbxPackageGroupID").eq(x).attr("checked") == "checked" || $(".cbxPackageGroupID").eq(x).attr("checked") == true) {
            PackageGroupID[PackageGroupID.length] = $(".cbxPackageGroupID").eq(x).val();
            //                RowVersion[RowVersion.length] = $(".RowVersionID").eq(x).val();                                               
        }
    }
    if (PackageGroupID != "") {
        if (confirm("คุณแน่ใจหรือไม่ว่าคุณต้องการลบกลุ่มแพ็คเกจเหล่านี้ ?") == true) {
            //                $("#GridLoad").html("<img src='../../Content/Default/Images/icon_loading.gif' alt='' width='13' height='13'>");
            //                DeleteContact(ContactID, RowVersion);
        }
    }
    else {
        bootbox.alert("กรุณาเลือกกลุ่มแพ็คเกจที่ต้องการลบ");
    }
});