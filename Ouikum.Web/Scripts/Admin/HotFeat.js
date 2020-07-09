var arrID = new Array();

$(function () {

    $('.delete').live('click', function () {
        if (confirm('confirm to delete this item')) {
            var id = parseInt($(this).attr('data-id'), 10);
            DeleteHotFeat(id);
        }
    });

    $('.edit').live('click', function () {
        $('#RejectModal').attr('data-id', $(this).attr('data-id'))
        HotFeaProductID = parseInt($(this).attr('data-id'), 10);
        console.log("HotFeaProductID: " + HotFeaProductID);

        $.ajax({
            url: 'HotFeat/GetEditByID',
            traditional: true,
            type: "Post",
            data: {
                HotFeaProductID: HotFeaProductID
            },
            success: function (data) {
                if (data.IsResult == true) {
                    $('#editHotPrice').val(data.HotPrice);
                    $('#Status').val(data.Status);

                    var EnumHotFeatStatus = data.EnumHotFeatStatus;
                    var htmlType = "";
                    htmlType += "<select name=\"editStatus\" class=\"editStatus\" style=\"width:145px\">";
                    for (var i = 0; i < EnumHotFeatStatus.length ; i++) {
                        if (EnumHotFeatStatus[i].EnumValue == data.Status) {
                            htmlType += "<option value=\"" + EnumHotFeatStatus[i].EnumValue + "\" selected='selected'>" + EnumHotFeatStatus[i].EnumText + "</option>"
                        } else if (EnumHotFeatStatus[i].EnumText == "All") {
                        } else {
                            htmlType += "<option value=\"" + EnumHotFeatStatus[i].EnumValue + "\">" + EnumHotFeatStatus[i].EnumText + "</option>"
                        }
                    }
                    htmlType += "</select>"
                    $('#EnumHotFeatStatus').html(htmlType);

                    $('#RejectModal').modal('show')
                } else {
                }
            }
        });

        
        //console.log(HotFeaProductID);
    });

    $('#save_edit').live('click', function () {
        if (confirm('confirm to edit this item')) {
            var ExpiredDate = parseInt($('#EditExpiredDate').val(), 10);
            var EditStatus = $('.editStatus option:selected').val();
            var EditHotprice = $('.editHotPrice').val();

            EditExpiredDate(HotFeaProductID, ExpiredDate, EditStatus, EditHotprice);
        }
    });

    $(".btnDeleteAll").live('click', function () {
        if ($(".btnDeleteAll").hasClass('checkNow')) {
            if (confirm('Confirm to delete this all items')) {
                arrID = GetValue();
                DeleteAll(arrID);
            }
        }
        else {
            bootbox.alert(label.vldplease_select);
            return false;
        }
    });
    $(".searchmodal").live('click', function () {
        OpenLoading(true);
    });
    $(".btn-info").live('click', function () {
        OpenLoading(true);
    });
    $(".btnNew").live('click', function () {
        $("#ProductListModal").modal();
    });
    $("#AddProductHot").live('click', function () {
        OpenLoading(true);
        $('#AddProductHotModal').attr('data-compid', $(this).attr('data-compid'));
        $('#AddProductHotModal').attr('data-id', $(this).attr('data-id'));
        $('#CompID').val($(this).attr('data-compid'));
        $('#HotFeaProductID').val($(this).attr('data-id'));

        $('#hotfeat-form').submit();
    });
    $('#hotfeat-form').submit(function () {
        //console.log('search');
        LoadPageHotFeat('hotfeat', 1);
        return false;
    });

    $('.EditExpiredDate').live('change', function () {
        var index = $(".EditExpiredDate").index(this);
        var value = $(this).val();
        $('.cbxHotFeat').eq(index).attr('data-expire', value)
    });

    $('.status').live('change', function () {
        var index = $(".status").index(this);
        var value = $(this).val();
        $('.cbxHotFeat').eq(index).attr('data-type', value)
    });

    $('.Import').live('click', function () {
        if (confirm('Are you sure to import this item? ')) {
            var index = $(".Import").index(this);
            GetValHotFeat(index);
            SaveHotFeat();
        }
    });

    $('.ImportProductID').live('click', function () {
        if (confirm('คุณยืนยันที่จะนำเข้าสินค้านี้หรือไม่? ')) {
            var productId = $(this).attr('data-id');
            SaveProductHotFeat(productId);
        }
    });

    $('.SaveImport').live('click', function () {
        if (confirm('Are you sure to import all? ')) {
            GetValHotFeat();
            SaveHotFeat();
        }
    });
    $('.CancleProductList').live('click', function () {
        $('#ProductListModal').modal('hide');
    });

    $('.exp').click(function () {
        $('#ExpireStatus').val($(this).val());
        $('.hidPageIndex').val(1);
        if ($(this).val() == 1) {
            $('#ExtendLifetimes').show();
            $('#SendMails_1').show();
            $('#SendMails_2').hide();
            $('#New').hide();
            $('#Delete').hide();
            $('#hideMar10').show();
        }
        else if ($(this).val() == 2 || $(this).val() == 3) {
            $('#ExtendLifetimes').show();
            $('#SendMails_2').show();
            $('#SendMails_1').hide();
            $('#New').hide();
            $('#Delete').hide();
            $('#hideMar10').show();
        } else {
            $('#ExtendLifetimes').hide();
            $('#SendMails_1').hide();
            $('#SendMails_2').hide();
            $('#New').show();
            $('#Delete').show();
            $('#hideMar10').show();
        }
    });

    $('#Refresh').click(function () {
        $('.btn-group > .exp').removeClass('active');
    });

    $('.select-status').live('change', function () {
        var status = $(this).find(":selected").val();
        var id = parseInt($(this).attr('data-id'), 10);
        UpdateStatusHotFeat(id, status);
    });
});

var productid = new Array();
var expire = new Array();
var type = new Array();
var compid = new Array();
var hotprice = new Array();
function SaveHotFeat() {
    console.log("hotprice: " + hotprice);
    data = {
        ProductID: productid,
        CompID: compid, 
        Expire: expire, 
        Type: type,
        HotPrice: hotprice
    }
    //console.log(data);
    if (data.ProductID.length > 0 || productid > 0) {
        OpenLoading(true);
        $.ajax({
            url: GetUrl("Admin/HotFeat/SaveHotFeat"),
            data: data,
            traditional: true,
            success: function (data) {

                OpenLoading(false);
                var msg = "";
                if (data.IsResult) {
                    msg = "save success " + data.CountSuccess + " item. ";
                    AlertSuccess(msg);
                } else {
                    msg = "save success " + data.CountSuccess + " item. ";
                    var exist = "";
                    //console.log(data);
                    if (data.CountProductExist != null && data.CountProductExist != "") {
                        var item = data.CountProductExist.split(',');
                        for (i = 0; i < item.length; i++) {
                            exist += " <a href='" + GetUrl("Search/Product/Detail/" + item[i]) + "' target='_blank' >" + item[i] + "</a> ";
                        }

                        msg = msg + " AND ProductID " + exist + " have exists ";
                    }
                    AlertWarning(msg);
                }
                $('#ProductListModal').modal('hide');
                g_no = 0;
                GridStartPage(g_no);
                return false;
            },
            error: function () {
                OpenLoading(false);
            },
            type: "POST"
        });
    }

}

function GetValHotFeat(index) {
    productid = new Array();
    expire = new Array();
    type = new Array();
    compid = new Array();
    hotprice = new Array();
    var j = 0;
    if (index != null && index != undefined) {
        var value = parseInt($('.cbxHotFeat').eq(index).attr('data-expire'), 10);
        if (value > 0) {
            productid[j] = parseInt($('.cbxHotFeat').eq(index).attr('data-id'), 10);
            expire[j] = parseInt($('.cbxHotFeat').eq(index).attr('data-expire'), 10);
            type[j] = $('.cbxHotFeat').eq(index).attr('data-type');
            compid[j] = parseInt($('.cbxHotFeat').eq(index).attr('data-comp'), 10);
            //hotprice[j] = $('.HotPrice').eq(index).val(), 10;
            hotprice[j] = parseFloat($('.HotPrice').eq(index).val(), 10);
            console.log(hotprice[j]);
            j++;
        }
    } else {
        var len = $('.cbxHotFeat').length;
        for (var i = 0; i < len; i++) {
            if ($('.cbxHotFeat').eq(i).attr("value") == true || $('.cbxHotFeat').eq(i).attr("checked") == "checked") {
                var value = parseInt($('.cbxHotFeat').eq(i).attr('data-expire'), 10);
                if (value > 0) {
                    productid[j] = parseInt($('.cbxHotFeat').eq(i).attr('data-id'), 10);
                    expire[j] = parseInt($('.cbxHotFeat').eq(i).attr('data-expire'), 10);
                    type[j] = $('.cbxHotFeat').eq(i).attr('data-type');
                    compid[j] = parseInt($('.cbxHotFeat').eq(i).attr('data-comp'), 10);
                    //hotprice[j] = $('.HotPrice').eq(index).val(), 10;
                    hotprice[j] = parseFloat($('.HotPrice').eq(i).val(), 10);
                    console.log(hotprice[j]);
                    j++;
                }
            }
        }
    }
}

function DeleteAll(id) {
    data = {
        ID: id
    }
    //console.log(data);
    if (data.ID.length > 0 || id > 0) {
        OpenLoading(true);
        $.ajax({
            url: GetUrl("Admin/HotFeat/DeleteAll"),
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

function DeleteHotFeat(id) {
    data = {
        HotFeaProductID: id
    }
    $('#RejectModal').modal('hide')
    $.ajax({
        url: GetUrl("Admin/HotFeat/DeleteHotFeat"),
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
    return false;
}

function UpdateStatusHotFeat(id,status) {
    data = {
        HotFeaProductID: id,
        Status: status
    }
    $('#RejectModal').modal('hide')
    $.ajax({
        url: GetUrl("Admin/HotFeat/UpdateStatusHotFeat"),
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
    return false;
}

$('#IsSendMailExpire').click(function () {
    if ($(this).val() == 1) {
        $(this).val(0);
        $(this).removeAttr('checked');
    } else {
        $(this).val(1);
        $(this).attr('checked', 'checked');
    }
});

function EditExpiredDate(HotFeaProductID, ExpiredDate,EditStatus, EditHotprice) {
    
    data = {
        HotFeaProductID: HotFeaProductID,
        NumMonth: ExpiredDate,
        SendMailExpire: parseInt($('#IsSendMailExpire').val(), 10),
        EditStatus:EditStatus,
        EditHotprice: EditHotprice
    }

    $('#RejectModal').modal('hide')
    $.ajax({
        url: GetUrl("Admin/HotFeat/EditExpiredDate"),
        data: data,
        traditional: true,
        success: function (data) {
            OpenLoading(false);
            if (CheckError(data)) {
                $('#Count_to_Exp').text(data.Count_to_Exp);
                $('#Count_Exp_today').text(data.Count_Exp_today);
                $('#Count_Exp').text(data.Count_Exp);
                $(g_hidsubmit).eq(g_no).click();
            }
            $('.exp').eq(3).click();
        },
        error: function () {
        },
        type: "POST"
    });
    return false;
}

function CheckHotFeatall(Obj) {
    if (Obj.attr("value") == true || Obj.attr("checked") == "checked") {
        $(".cbxHotFeat").attr("checked", "checked");
        $(".cbxHotFeat").attr("value", "true");
        Obj.attr("checked", "checked");
        Obj.attr("value", "true");

    } else {
        $(".cbxHotFeat").removeAttr("checked");
        $(".cbxHotFeat").attr("value", "false");
        Obj.removeAttr("checked");
        Obj.attr("value", "false");
    }

}

$(".icon_quo_up").live('click', function () {
    var input = $(this).parent().parent().find("input[type=text]:eq(0)");
    if (input.val() == "") {
        input.val(1)
    } else {
        input.val(parseFloat(eval(input.val()) + 1).toFixed(2))
    }
});
$(".icon_quo_down").live('click', function () {
    var input = $(this).parent().parent().find("input[type=text]:eq(0)");
    if (input.val() == "") {
        bootbox.alert(label.vldcannot_insert_zero);
    } else {
        if (parseFloat(input.val()) > 0) {
            input.val(parseFloat(eval(input.val()) - 1).toFixed(2))
        }
    }
});

/**************************************Send_mail********************************************/
function Send_mail(status, memid, rowversion) {
    $('.close').click();
    var HotFeaProductID = [];
    var RowVersion = [];
    var chk = 0;
    var Check = [];
    $(".grid > tbody > tr").each(function (index) {
        if ($(this).children().find(".cbxItem:checked").val() == "true") {
            Check[index] = "True";
        }
        else {
            Check[index] = "False";
        }
        HotFeaProductID[index] = $(this).children().find(".hidHotFeaProductID").val();
        RowVersion[index] = $(this).children().find(".hidRowVersion").val();
    });
    for (var i = 0; i < Check.length; i++) {
        if (Check[i] == "True") {
            chk++;
        }
    }
    if (chk == 1) {
        if (status == 1) {
            if (confirm(label.vldconfirm_NearExpireHot)) {
                OpenLoading(true);
                $.ajax({
                    url: url + "HotFeat/SendMailStatus",
                    data: { Check: Check, Status: status, HotFeaProductID: HotFeaProductID, RowVersion: RowVersion },
                    success: function (data) {
                        if (data.Result) {
                            OpenLoading(false);
                            $(g_hidsubmit).eq(g_no).click();
                            alertMsg("Alert : ", "success", label.vldsend_success);
                            return true;
                        } else {
                            OpenLoading(false);
                            return false;
                        }
                    },
                    type: "POST", traditional: true
                });
            }
        } else {
            if (confirm(label.vldconfirm_ExpireHot)) {
                OpenLoading(true);
                $.ajax({
                    url: url + "HotFeat/SendMailStatus",
                    data: { Check: Check, Status: status, HotFeaProductID: HotFeaProductID, RowVersion: RowVersion },
                    success: function (data) {
                        if (data.Result) {
                            OpenLoading(false);
                            $(g_hidsubmit).eq(g_no).click();
                            alertMsg("Alert : ", "success", label.vldsend_success);
                            return true;
                        } else {
                            OpenLoading(false);
                            return false;
                        }
                    },
                    type: "POST", traditional: true
                });
            }
        }
    }
    else {
        //alertMsg("Notice! ", "error", label.vldnotice_del);
        bootbox.alert('ขออภัย! กรุณาเลือกเพียง 1 รายการเท่านั้น');
        OpenLoading(false);
        return false;
    }
}

function LoadPageHotFeat(name, pageindex) {
    setPageIndex(name, pageindex);
    data = {
        CompID: $('#CompID').val(),
        TextSearch: $('#HotTextSearch').val(),
        SearchType: $('#HotSearchType').val()
    };
    $.ajax({
        url: GetUrl("HotFeat/AddProductHotList"),
        type: "POST",
        data: data,
        dataType: 'json',
        traditional: true,
        success: function (model) {
            var html = "";
            html += '<table class="table table-hover border_table grid mar_b10">';
            html += '<thead class="HeaderTable"><tr>';
            html += '<th style="width:45%"><div>ชื่อสินค้า</div></th>' +
                '<th style="width:45%"><div>ชื่อบริษัท</div></th>' +
                '<th style="width:8%"></th>';
            html += '</tr></thead>';
            html += '<tbody>';
            if (model.TotalRow > 0) {
                $.each(model.Suppliers, function (i, item) {
                    var ID = item.ProductID;
                    html += '<tr class="body">';
                    html += '<td><a target="_blank" title="'+ item.ProductName +'" href="' + GetUrl("Search/Product/Detail/" + ID) + '?name='+ item.ProductName +'" />'+ item.ProductName +'</a></td>';
                    html += '<td><a target="_blank" title="' + item.CompName + '" href="' + GetUrl("CompanyWebsite/" + item.CompName + "/Main/Index/" + item.CompID)+'">'+ item.CompName +'</a></td>';
                    html += '<td><a class="btn btn-mini btn-tootip-bottom padd_b4 ImportProductID" title="Import" data-id="' + ID + '"><i class="icon-plus-sign"></i></a></td>';
                    html += '</tr>';
                });
            } else {
                html += '<tr>';
                html += '<td colspan="6"><div align="center">ไม่พบรายการที่คุณค้นหา</div></td>';
                html += '</tr>';
            }
            html += '</tbody>';
            html += '</table>';
            $('#' + name + '-content').html(html);
            GeneratePaging(name, 'LoadPageHotFeat', model);
            OpenLoading(false);
            $('.pager').css("margin","0px");
            
            $("#AddProductHotModal").modal();
        },
        error: function () {
        }
    });

}

function SaveProductHotFeat(productId) {
    data = {
        ProductID: productId,
        HotFeaProductID: $('#HotFeaProductID').val()
    };
    OpenLoading(true);
    $.ajax({
        url: GetUrl("HotFeat/SaveProductHotFeat"),
        type: "POST",
        data: data,
        dataType: 'json',
        traditional: true,
        success: function (data) {
            OpenLoading(false);
            if (CheckError(data)) {
                $('#AddProductHotModal').modal('hide');
                $(g_hidsubmit).eq(g_no).click();
            }
        },
        error: function () {
        }
    });
}