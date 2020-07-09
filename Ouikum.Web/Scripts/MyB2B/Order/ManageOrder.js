function CheckBoxall(Obj) {
    if (Obj.attr("value") == true || Obj.attr("checked") == "checked") {
        $(".cbxOrderID").attr("checked", "checked");
        $(".cbxOrderID").attr("value", "true");
        Obj.attr("checked", "checked");
        Obj.attr("value", "true");

    } else {
        $(".cbxOrderID").removeAttr("checked");
        $(".cbxOrderID").attr("value", "false");
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
            $(".cbxOrderID").removeAttr("checked");
            $(".cbxOrderID").attr("value", "false");
            $("#" + id + "").attr("checked", "checked");
            $("#" + id + "").attr("value", "true");
        }
    }
}

//Order Index
$(".DeleteOrders").click(function () {
    var OrderID = new Array();

    for (x = 0; x < $(".cbxOrderID").length; x++) {
        if ($(".cbxOrderID").eq(x).attr("checked") == "checked" || $(".cbxOrderID").eq(x).attr("checked") == true) {
            OrderID[OrderID.length] = $(".hidRowVersion").eq(x).val();
        }
    }
    if (OrderID != "") {
        if (confirm(label.vldconfirm_del_data) == true) {
            DeleteOrder(OrderID);
            function DeleteOrder(OrderID) {
                data = {
                    OrderID: OrderID
                }
                console.log(data);
                $.ajax({
                    url: GetUrl("MyB2B/Order/DeleteOrder"),
                    data: data,
                    traditional: true,
                    success: function (data) {
                        window.location.reload();
                    },
                    error: function () {
                    },
                    type: "POST"
                });
                return false;
            }
        }
    }
    else {
        bootbox.alert(label.vldno_item_selected);
    }
});

$(function () {

    //---------------------------ย่อ/ขยาย TabMenu------------------------------------
    $(".CloseTabmenu").click(function () {
        $('.bgMainMenu').hide("slide", { direction: "up" }, 200);
        $('.CloseTabmenu').addClass('newhide');
        if ($('#content').attr('page') == "Recommend") {
            $(".Bg_Allitem").css("marginTop", 85);
        }
        else {
            $(".Bg_Allitem").css("marginTop", 130);
        }
        $('.bg_product_hide').removeClass('hide');
        $('#advance_search').css("top", 145);
    });
    $(".ShowTabmenu").click(function () {
        $('.bgMainMenu').show("slide", { direction: "up" }, 200);
        if ($('#content').attr('page') == "Recommend") {
            $(".Bg_Allitem").css("marginTop", 200);
        }
        else {
            $(".Bg_Allitem").css("marginTop", 244);
        }
        $('.bg_product_hide').addClass('hide');
        $('#advance_search').css("top", $('.tab_manage_fix').height() + 15);
    });
});