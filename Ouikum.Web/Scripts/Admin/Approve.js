var arrID = new Array();
var memberpaidid = 0;
var catelv1 = new Array();
var catelv2 = new Array();
var catelv3 = new Array();
var compID = new Array();

$(function () {


    $(".btnRejectItem").popover({
        placement: "bottom",
        content: "<div id='form_reject'>" + $("#rejectform").html() + "</div><div class='clean10'></div><button id='save_reject_item' class='btn btn-danger'  type='button' >Save</button>",
        template: '<div class="popover" style="width:300px;"><div class="arrow"></div><div class="popover-inner"><div class="popover-content"><p></p></div></div></div>'
    });

    $("#btnRejectAll").popover({
        placement: "right",
        content: "<div id='form_reject'>" + $("#rejectform").html() + "</div><div class='clean10'></div><button id='save_reject_all' class='btn btn-danger'  type='button' >Save</button>",
        template: '<div class="popover" style="width:300px;"><div class="arrow"></div><div class="popover-inner"><div class="popover-content"><p></p></div></div></div>',
        html: true
    });

    $("#btnRejectAll").live('click', function () {
        if ($("#btnRejectAll").hasClass('checkNow')) {
            $("#btnRejectAll").popover({
                placement: "right",
                content: "<div id='form_reject'>" + $("#rejectform").html() + "</div><div class='clean10'></div><button id='save_reject_all' class='btn btn-danger'  type='button' >Save</button>",
                template: '<div class="popover" style="width:300px;"><div class="arrow"></div><div class="popover-inner"><div class="popover-content"><p></p></div></div></div>',
                html:true
            });
        }
        else {
            $('.popover').removeClass('in');
            bootbox.alert(label.vldplease_select);
            return false;
        }

    });

    $("#btnRejectAllPackage").popover({
        placement: "right",
        content: "<div id='form_reject'>" + $("#rejectform").html() + "</div><div class='clean10'></div><button id='save_reject_allPackage' class='btn btn-danger'  type='button' >Save</button>",
        template: '<div class="popover" style="width:300px;"><div class="arrow"></div><div class="popover-inner"><div class="popover-content"><p></p></div></div></div>',
        html: true
    });

    $("#btnRejectAllPackage").live('click', function () {
        if ($("#btnRejectAllPackage").hasClass('checkNow')) {
            $("#btnRejectAllPackage").popover({
                placement: "right",
                content: "<div id='form_reject'>" + $("#rejectform").html() + "</div><div class='clean10'></div><button id='save_reject_allPackage' class='btn btn-danger'  type='button' >Save</button>",
                template: '<div class="popover" style="width:300px;"><div class="arrow"></div><div class="popover-inner"><div class="popover-content"><p></p></div></div></div>',
                html: true
            });
        }
        else {
            $('.popover').removeClass('in');
            bootbox.alert(label.vldplease_select);
            return false;
        }

    });

    $('.btnRejectItem').live('click', function () { // detail page
        // do something
        memberpaidid = parseInt($(this).attr('data-id'), 10);
        var index = $(".btnRejectItem").index(this);
        GetValCateLV(index);

    });

    $('#save_reject_item').live('click', function () { // detail page
        // do something
        if (confirm('Confirm to reject this item')) {
            var remark = GetValueMsgReject();

            SaveReject(memberpaidid, remark);
        }
    });

    $('.approve').live('click', function () {
        if (confirm('Confirm approve this item')) {
            var index = $(".approve").index(this);
            //console.log('index :' + index);
            GetValCateLV(index);
            var id = parseInt($(this).attr('data-id'), 10);
            SaveApprove(id);
        }
    });
    $('.delete-item').live('click', function () {
        if (confirm('Confirm delete this item')) {
            var index = $(".delete-item").index(this);
            //console.log('index :' + index);
            GetValCateLV(index);
            var id = parseInt($(this).attr('data-id'), 10);
            SaveDelete(id);
        }
    });

    $('.delete-item-package').live('click', function () {
        OpenLoading(true);
        var index = $(".delete-item-package").index(this);
        var id = parseInt($(this).attr('data-id'), 10);
        data = {
            OrderID: unique(parseInt($(this).attr('data-id'), 10))
        }
        $.ajax({
            url: GetUrl('ApprovePackage/CheckHotFeat'),
            data: data,
            traditional: true,
            success: function (data) {
                OpenLoading(false);
                if (data.IsResult) {
                    if (confirm('Confirm delete this item')) {
                        GetValCateLV(index);
                        SaveDelete(id);
                    }
                } else {
                    if (confirm('รายการสั่งซื้อนี้มีสินค้าที่ใช้งานอยู่ คุณต้องการจะยกเลิกหรือไม่')) {
                        GetValCateLV(index);
                        SaveDelete(id);
                    }
                }
            },
            error: function () {
            },
            type: "POST"
        });
    });

    $('#save_reject').live('click', function () {
        // do something
        if (confirm('Confirm to reject this item')) {
            var remark = GetValueMsgReject();
            SaveReject(memberpaidid, remark);
        }
    });

    $('#save_reject_Package').live('click', function () {
        // do something
        OpenLoading(true);
        data = {
            OrderID: unique(memberpaidid)
        }
        $.ajax({
            url: GetUrl('ApprovePackage/CheckHotFeat'),
            data: data,
            traditional: true,
            success: function (data) {
                OpenLoading(false);
                if (data.IsResult) {
                    if (confirm('Confirm to reject this item')) {
                        var remark = GetValueMsgReject();
                        SaveReject(memberpaidid, remark);
                    }
                } else {
                    if (confirm('รายการสั่งซื้อนี้มีสินค้าที่ใช้งานอยู่ คุณต้องการจะยกเลิกหรือไม่')) {
                        var remark = GetValueMsgReject();
                        SaveReject(memberpaidid, remark);
                    }
                }
            },
            error: function () {
            },
            type: "POST"
        });
    });

    $('#save_reject_all').live('click', function () {
        // do something
        if (confirm('Confirm to reject all item.')) {
            GetValCateLV();
            var remark = GetValueMsgReject($('#form_reject'));
            var arrID = GetValue();
            SaveReject(arrID, remark);
        }
    });

    $('#save_reject_allPackage').live('click', function () {
        // do something
        OpenLoading(true);
        data = {
            OrderID: unique(GetValue())
        }
        $.ajax({
            url: GetUrl('ApprovePackage/CheckHotFeat'),
            data: data,
            traditional: true,
            success: function (data) {
                OpenLoading(false);
                if (data.IsResult) {
                    if (confirm('Confirm to reject all item.')) {
                        GetValCateLV();
                        var remark = GetValueMsgReject($('#form_reject'));
                        var arrID = GetValue();
                        SaveReject(arrID, remark);
                    }
                } else {
                    if (confirm('รายการสั่งซื้อนี้มีสินค้าที่ใช้งานอยู่ คุณต้องการจะยกเลิกหรือไม่')) {
                        GetValCateLV();
                        var remark = GetValueMsgReject($('#form_reject'));
                        var arrID = GetValue();
                        SaveReject(arrID, remark);
                    }
                }
            },
            error: function () {
            },
            type: "POST"
        });
    });

    $(".reject-item").live('click', function () {
        var index = $(".reject-item").index(this);
        $('#RejectModal').attr('data-id', $(this).attr('data-id'));
        memberpaidid = parseInt($(this).attr('data-id'), 10);
        GetValCateLV(index);

        $('#RejectModal').modal('show')

    });

    $("#btnApproveAll").live('click', function () {
        if ($("#btnApproveAll").hasClass('checkNow')) {
            if (confirm('Confirm to approve all items.')) {
                GetValCateLV();
                arrID = GetValue();
                SaveApprove(arrID);
            }
        }
        else {
            bootbox.alert(label.vldplease_select);
            return false;
        }
    });


    $('.preview_admin').live('click', function () {
        // do something
        var code = $(this).attr('data-id');
        PreviewAdmin(code);
    });

});
function PreviewAdmin(code) {
    data = {
        Code: code
    }

    //console.log(data); 
    OpenLoading(true);
    $.ajax({
        url: GetUrl('Admin/ApproveProduct/PreviewAdmin'),
        data: data,
        traditional: true,
        success: function (data) {
            OpenLoading(false);
            $('#PreviewAdminModal').html(data);
            $('#PreviewAdminModal').modal('show');
        },
        error: function () {
        },
        type: "POST"
    });

}


function GetValueMsgReject(obj) {
    var len = $('.msg_reject').length;
    var str = '';
    if (obj != null && obj != undefined) {
        for (var i = 0; i < len; i++) {
            if (obj.find('.msg_reject').eq(i).attr("checked") == true || obj.find('.msg_reject').eq(i).attr("checked") == "checked") {
                str += " " + obj.find('.msg_reject').eq(i).val() + ',';
            }
        }
        str = str.substr(0, str.length - 1);
        if (obj.find('.msg_reject_remark').val().length > 0) {
            str += ", " + obj.find('.msg_reject_remark').val();
        }

    } else {
        for (var i = 0; i < len; i++) {
            if ($('.msg_reject').eq(i).attr("checked") == true || $('.msg_reject').eq(i).attr("checked") == "checked") {
                str += " " + $('.msg_reject').eq(i).val() + ',';
            }
        }
        str = str.substr(0, str.length - 1);
        if ($('.msg_reject_remark').val().length > 0) {
            str += ", " + $('.msg_reject_remark').val();
        }
    }
    //console.log('remark : ' + str);
    return str;
}
function ClosePopOver() {
    $('.popover').removeClass('in');
}


function GetUrlApprove() {
    var url = $('#hidUrlApprove').val();
    return GetUrl(url);
}

function GetUrlReject() {
    var url = $('#hidUrlReject').val();
    return GetUrl(url);
}
function GetUrlDelete() {
    var url = $('#hidUrlDelete').val();
    return GetUrl(url);
}


function SaveApprove(id) {
    data = {
        ID: unique(id),
        CateLV1: unique(catelv1),
        CateLV2: unique(catelv2),
        CateLV3: unique(catelv3),
        CompID: unique(compID)
    }

    ClosePopOver();
    $('#RejectModal').modal('hide');
    //console.log(data); 
    if (data.ID.length > 0 || id > 0) {
        OpenLoading(true);
        $.ajax({
            url: GetUrlApprove(),
            data: data,
            traditional: true,
            success: function (data) {
                OpenLoading(false);
                if (CheckError(data)) {
                    $(g_hidsubmit).eq(g_no).click();
                }
            },
            error: function () {
            },
            type: "POST"
        });
    }
}


function SaveDelete(id) {
    data = {
        ID: unique(id),
        CateLV1: unique(catelv1),
        CateLV2: unique(catelv2),
        CateLV3: unique(catelv3),
        CompID: unique(compID)
    }

    ClosePopOver();
    $('#RejectModal').modal('hide');
    //console.log(data);
    if (data.ID.length > 0 || id > 0) {
        OpenLoading(true);
        $.ajax({
            url: GetUrlDelete(),
            data: data,
            traditional: true,
            success: function (data) {
                OpenLoading(false);
                if (CheckError(data)) {
                    $(g_hidsubmit).eq(g_no).click();
                }
            },
            error: function () {
            },
            type: "POST"
        });
    }
}


function SaveReject(id, remark) {

    data = {
        ID: unique(id),
        Remark: remark,
        CateLV1: unique(catelv1),
        CateLV2: unique(catelv2),
        CateLV3: unique(catelv3),
        CompID: unique(compID)
    }
    //console.log(data); 
    ClosePopOver();
    $('#RejectModal').modal('hide')

    if (data.ID.length > 0 || id > 0) {
        OpenLoading(true);
        $.ajax({
            url: GetUrlReject(),
            data: data,
            traditional: true,
            success: function (data) {
                OpenLoading(false);
                if (CheckError(data)) {
                    $(g_hidsubmit).eq(g_no).click();
                }

            },
            error: function () {
                OpenLoading(false);
            },
            type: "POST"
        });
    }
}

function GetValCateLV(index) {
    catelv1 = new Array();
    catelv2 = new Array();
    catelv3 = new Array();
    var j = 0;
    if (index != null && index != undefined) {
        var value = parseInt($('.cbxCompID').eq(index).attr('data-catelv1'), 10);
        if (value > 0) {
            catelv1[j] = parseInt($('.cbxCompID').eq(index).attr('data-catelv1'), 10);
            catelv2[j] = parseInt($('.cbxCompID').eq(index).attr('data-catelv2'), 10);
            catelv3[j] = parseInt($('.cbxCompID').eq(index).attr('data-catelv3'), 10);
            j++;
        }
        catelv1[index] = parseInt($('.cbxCompID').eq(index).attr('data-catelv1'), 10);
        catelv2[index] = parseInt($('.cbxCompID').eq(index).attr('data-catelv2'), 10);
        catelv3[index] = parseInt($('.cbxCompID').eq(index).attr('data-catelv3'), 10);
        compID[index] = parseInt($('.cbxCompID').eq(index).attr('data-compid'), 10);
    } else {
        var len = $('.cbxCompID').length;
        for (var i = 0; i < len; i++) {
            if ($('.cbxCompID').eq(i).attr("value") == true || $('.cbxCompID').eq(i).attr("checked") == "checked") {
                var value = parseInt($('.cbxCompID').eq(i).attr('data-catelv1'), 10);
                if (value > 0) {
                    catelv1[j] = parseInt($('.cbxCompID').eq(i).attr('data-catelv1'), 10);
                    catelv2[j] = parseInt($('.cbxCompID').eq(i).attr('data-catelv2'), 10);
                    catelv3[j] = parseInt($('.cbxCompID').eq(i).attr('data-catelv3'), 10);
                    compID[j] = parseInt($('.cbxCompID').eq(i).attr('data-compid'), 10);
                    j++;
                }
            }
        }
    }
    //    //console.log(catelv1);
    //    //console.log(catelv2);
    //    //console.log(catelv3);
}
