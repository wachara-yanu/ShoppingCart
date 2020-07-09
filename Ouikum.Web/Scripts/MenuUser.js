GetStatusProduct();
GetStatusBuylead();

var page = $('#PageType').val();
switch (page) {
    case "Product":
        Slides('btnShowHide1');
        break;
    case "Buylead":
        Slides('btnShowHide5');
        break;
    case "Company":
        Slides('btnShowHide7');
        break;
    case "Message":
        Slides('btnShowHide8');
        break;
    case "Quotation":
        Slides('btnShowHide10');
        break;
    case "Profile":
        Slides('btnShowHide11');
        break;
    case "Orderlist":
        Slides('btnShowHide12');
        break;
}

//คลิกปุ่มบวก หัวข้อหายไป เปลี่ยนเป็น input
$('.add_group').click(function () {
    OpenFormAddGroup(true);
});
// คลิก Save // คลิก Close
$('.addgroup_already').click(function () {
    $('.input_group').hide();
    $('.hide_group').fadeIn();
});
$('.addgroup_close').click(function () {
    OpenFormAddGroup(false);
});

$('.editgroup_close').click(function () {
    OpenFormEditGroup(false);
});

//console.log("ManageProduct5");
$('.edit_group').live("click", function () {
    var index = $(".edit_group").index(this);
    var obj = $('.product-group').eq(index);
    var rowversion = obj.attr('rowversion');
    var groupname = obj.attr('group-name');
    var id = obj.attr('group');
    $('#hidEditGroupID').val(id);
    $('#hidEditGroupRowversion').val(rowversion);
    $('#txtEditGroupName').val(groupname);
    $('#txtEditGroupName').attr('placeholder', groupname);
    OpenFormEditGroup(true);
});

//console.log("ManageProduct5.1");
$('.del_group').live("click", function () {
    var index = $(".del_group").index(this);
    var obj = $('.product-group').eq(index);
    var rowversion = obj.attr('rowversion');
    var groupname = obj.attr('group-name');
    var id = obj.attr('group');
    bootbox.confirm(label.vldconfirm_del, "Cancel", "Yes", function (e) {
        if (e) {
            OpenFormEditGroup(false);
            DeleteGroup(id, rowversion);
        }
    });
});

function OpenFormAddGroup(isOpen) {
    if (isOpen) {
        $('.hide_group').hide();
        $('.input_edit_group').hide();

        $('.input_group').fadeIn();
        $('#ProductGroupName').focus();
    } else {
        $('.input_group').hide();
        $('.input_edit_group').hide();
        $('.hide_group').fadeIn();
    }
}

function OpenFormEditGroup(isOpen) {
    if (isOpen) {
        $('.hide_group').hide();
        $('.input_group').hide();

        $('.input_edit_group').fadeIn();
        $('#txtEditGroupName').focus();
    } else {
        $('.input_group').hide();
        $('.input_edit_group').hide();
        $('.hide_group').fadeIn();
    }
}

function GroupProductList() {
    $.ajax({
        url: GetUrl("MyB2B/Product/GroupProductList"),
        success: function (data) {
            $('#GroupProductList').html(data);
        },
        error: function () {
        },
        type: "POST"
    });

    return false;

}

function GroupListNo(obj) {
    var id = new Array();
    var no = new Array();
    id[0] = parseInt(obj.parents('.product-group').attr('group'));
    no[1] = parseInt(obj.parents('.product-group').attr('listno'));
    var action = obj.attr('action');
    if (action == "Up") {
        no[0] = no[1] - 1;
    }
    else if (action == "Down") {
        no[0] = no[1] + 1;
    }
    id[1] = parseInt($('.product-group').eq(no[0] - 1).attr('group'));
    $.ajax({
        url: GetUrl("MyB2B/Product/ChangeListNoGroup"),
        data: {
            id: id,
            no: no
        },
        traditional: true,
        success: function (data) {
            if (CheckError(data)) {
                GroupProductList();
            }
        },
        error: function () {
            OpenLoading(false);
            bootbox.alert("Error");
        },
        type: "POST"
    });
}

function AddGroupProduct() {
    var name = $('#ProductGroupName').val().trim();
    //console.log(name);

    if (name.length < 3) {
        bootbox.alert(label.vldless_3char);
        return false;
    }
    data = {
        ProductGroupName: name
    }

    SaveGroupProduct(data);
    $('#ProductGroupName').val(null);
    OpenFormAddGroup(false);
    return false;
}

function EditGroupProduct() {

    data = {
        ProductGroupName: $.trim($('#txtEditGroupName').val()),
        GroupID: parseInt($('#hidEditGroupID').val(), 10),
        RowVersion: parseInt($('#hidEditGroupRowversion').val(), 10)
    }

    if (data.ProductGroupName.length < 3) {
        bootbox.alert(label.vldless_3char);
        return false;
    }

    SaveGroupProduct(data);
    $('#txtEditGroupName').val(null);
    OpenFormEditGroup(false);
    return false;
}

function DeleteGroup(id, rowversion) {
    data = {
        GroupID: id,
        RowVersion: rowversion
    }
    $.ajax({
        url: GetUrl("MyB2B/Product/DeleteProductGroup"),
        data: data,
        success: function (data) {
            OpenLoading(false);
            if (CheckError(data)) {
                GroupProductList();
            }
        },
        error: function () {
            OpenLoading(false);
        },
        type: "POST"
    });
    return false;
}

function SaveGroupProduct(data) {

    $.ajax({
        url: GetUrl("MyB2B/Product/SaveGroupProduct"),
        data: data,
        dataType: 'json',
        success: function (data) {
            if (data.IsResult == false) {
                bootbox.alert(label.vldpgroup_already);
            }
            if (CheckError(data)) {
                GroupProductList();
            }
        },
        error: function () {
            OpenLoading(false);
            bootbox.alert("Error");
        },
        type: "POST"
    });

    return false;

}

function Slide(str) {
    var data = $("#" + str).index(".btnShowHide");
    var Icon = $("#" + str).find("i")
    var target = $("#" + str).attr("data-target");
    if (Icon.hasClass("menumiddleDownadmin")) {
        Icon.removeClass("menumiddleDownadmin");
        Icon.addClass("menurightadmin");
        Icon.attr("title", "แสดง");
        console.log(Icon);
        $(target).hide();
    }
    else {
        Icon.removeClass("menurightadmin");
        Icon.addClass("menumiddleDownadmin");
        Icon.attr("title", "ซ่อน");
        console.log(Icon);
        $(target).show();
    }
}

function Slides(str) {
    var data = $("#" + str).index(".btnShowHide");
    var Icon = $("#" + str).find("i");
    var target = $("#" + str).attr("data-target");
    if (Icon.hasClass("menumiddleDownadmin")) {
        Icon.removeClass("menumiddleDownadmin");
        Icon.addClass("menurightadmin");
        Icon.attr("title", "แสดง");
        console.log(Icon);
        $(target).hide();
    }
    else {
        Icon.removeClass("menurightadmin");
        Icon.addClass("menumiddleDownadmin");
        Icon.attr("title", "ซ่อน");
        console.log(Icon);
        $(target).show();
    }

    if (str == "btnShowHide1") {
        $("#message_status").show();
        $("#quotaion_status").show();
        $("#MenuUser_target2").hide();
        $("#MenuUser_target3").hide();
        $("#MenuUser_target4").hide();
        $("#MenuUser_target5").hide();
        $("#MenuUser_target6").hide();
    }
    if (str == "btnShowHide5") {
        $("#message_status").show();
        $("#quotaion_status").show();
        $("#MenuUser_target1").hide();
        $("#MenuUser_target3").hide();
        $("#MenuUser_target4").hide();
        $("#MenuUser_target5").hide();
        $("#MenuUser_target6").hide();
    }
    if (str == "btnShowHide7") {
        $("#message_status").show();
        $("#quotaion_status").show();
        $("#MenuUser_target1").hide();
        $("#MenuUser_target2").hide();
        $("#MenuUser_target4").hide();
        $("#MenuUser_target5").hide();
        $("#MenuUser_target6").hide();
    }
    if (str == "btnShowHide8") {
        $("#message_status").hide();
        $("#quotaion_status").show();
        $("#MenuUser_target1").hide();
        $("#MenuUser_target2").hide();
        $("#MenuUser_target3").hide();
        $("#MenuUser_target5").hide();
        $("#MenuUser_target6").hide();
    }
    if (str == "btnShowHide10") {
        $("#quotaion_status").hide();
        $("#message_status").show();
        $("#MenuUser_target1").hide();
        $("#MenuUser_target2").hide();
        $("#MenuUser_target3").hide();
        $("#MenuUser_target4").hide();
        $("#MenuUser_target6").hide();
    }
    if (str == "btnShowHide11") {
        $("#message_status").show();
        $("#quotaion_status").show();
        $("#MenuUser_target1").hide();
        $("#MenuUser_target2").hide();
        $("#MenuUser_target3").hide();
        $("#MenuUser_target4").hide();
        $("#MenuUser_target5").hide();
    }
    if (str == "btnShowHide12") {
        $("#message_status").show();
        $("#quotaion_status").show();
        $("#MenuUser_target1").hide();
        $("#MenuUser_target2").hide();
        $("#MenuUser_target3").hide();
        $("#MenuUser_target4").hide();
        $("#MenuUser_target5").hide();
        $("#MenuUser_target6").hide();
    }

    var a = $('#hidOldClick').val();
    var b = $("#" + a).find("i")
    if (str != a) {
        if (b.hasClass("menumiddleDownadmin")) {
            b.removeClass("menumiddleDownadmin").addClass("menurightadmin");
            b.attr("title", "แสดง");
            console.log(b);
        }
    }

    $('#hidOldClick').val(str);
}

function GetStatusProduct() {
    $.ajax({
        url: GetUrl("MyB2B/Product/GetStatus"),
        success: function (data) {
            $('#status-recommend').html(data.CountProductRecomm + "/20");
            $('#menu-status-recommend').html(data.CountProductRecomm);
            if ($('#IsMaxProduct').val() == "True") {
                $('#status-product').html(data.CountProduct + "/20");
                $('#menu-status-product').html(data.CountProduct);
            } else {
                $('#status-product').html(data.CountProduct);
                $('#menu-status-product').html(data.CountProduct);
            }
            $('#status-junk').html(data.CountJunk);
            $('#status-Approve').html(data.CountProductApprove);
            $('#status-Wait').html(data.CountProductWait);
            $('#status-Allitem').html(data.CountAllitem);
        },
        error: function () {
        },
        type: "POST"
    });

}

function GetStatusBuylead() {
    $.ajax({
        url: GetUrl("MyB2B/Buylead/GetStatus"),
        success: function (data) {
            $('#status-Buylead').html(data.CountBuylead);
            $('#menu-status-Buylead').html(data.CountBuylead);
            $('#status-junk').html(data.CountJunk);
            $('#status-Allitem').html(data.CountAllitem);
            $('#status-Approve').html(data.CountBuyleadApprove);
            $('#status-Wait').html(data.CountBuyleadWait);
            $('#status-Allitem').html(data.CountBuyleadAllitem);
        },
        error: function () {
        },
        type: "POST"
    });

}