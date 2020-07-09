
var compid = 0;
$('.btnsearch').click(function () {

    OpenLoading(true);
    LoadReportMsg('reportmsg', 1);
    LoadReportQuotation('reportquotation', 1);
    var index = $('.btn-sel-comp').index($(this));
    compid = $('.list-comp').eq(index).attr('data-id');
    var email = $('.list-comp').eq(index).attr('data-email');
    var date = $('#ddlFindDatePeriod').val().replace(" - ", "-");
    GenerateLinkReportTrade(compid, email, date);
});

$('.isUser').click(function () {

    if ($(this).val() == 1) {
        $(this).val(0);
        $(this).removeAttr('checked');
    } else {
        $(this).val(1);
        $(this).attr('checked', 'checked');
    }
});


$('.btn-sel-comp').live('click', function () {
    $('#myModal').modal('hide');
    var index = $('.btn-sel-comp').index($(this));
    compid = $('.list-comp').eq(index).attr('data-id');
    var name = $('.list-comp').eq(index).text();
    var email = $('.list-comp').eq(index).attr('data-email');
    var phone = $('.list-comp').eq(index).attr('data-phone');
    var date = $('#ddlFindDatePeriod').val();
    GenerateCompDetail(compid, name, email, phone);
    GenerateLinkReportTrade(compid, email,date);
    LoadReportMsg('reportmsg', 1);
    LoadReportQuotation('reportquotation', 1);
});


$('.btn-send-mail').live('click', function () {
    var isCheck = $('.isUser').val();
    data = {
        CompID: compid, 
        Email: $('#comp-email').val(),
        WithUser: isCheck
    } 
    OpenLoading(true);
    $.ajax({
        url: GetUrl("Admin/Stat/SendMail"),
        type: "POST",
        data: data,
        dataType: 'json',
        traditional: true,
        success: function (model) {
            OpenLoading(false);
            $('#EmailModal').modal('hide');
            bootbox.alert("ส่งข้อความเรียบร้อยแล้ว");
        },
        error: function () {
        }
    });

});

function GenerateCompDetail(id, name, email, phone) {
    var html = '<p style="font-size:15px;margin-top:10px;"><strong>Company Name : </strong> ' + name + '&nbsp;&nbsp;&nbsp;';
    html += '<strong>Email :</strong> ' + email + '&nbsp;&nbsp;&nbsp;';
    html += '<strong>Phone :</strong> ' + phone + '</p>';
    $('#comp-detail').html(html);
    $('#reportmsg-content').html('');
    $('#reportquotation-content').html('');
    $('#search-form').fadeIn();
}

function GenerateLinkReportTrade(id, email,date) {
    $('#comp-email').val(email);
    $('.comp-link-detail').attr('href', GetUrl("report/trade?compid=" + id + "&Period=" + date));
    $('#comp-link').fadeIn();
}

function LoadReportMsg(name, pageindex) {
    setPageIndex(name, pageindex);
    data = {
        CompID: compid,
        Period: $('#ddlFindDatePeriod').val(),
        hidPageIndex: getPageIndex(name)
    };
    var html = "";
    $.ajax({
        url: GetUrl("Report/MsgTrade"),
        type: "POST",
        data: data,
        dataType: 'json',
        traditional: true,
        success: function (model) {
            OpenLoading(false);
            html += '<div class="border_table" style="width:100%;">';
            html += '<table  class="table table-striped table-hover" style="margin:0; text-align:center;">';
            html += '<thead class="HeaderTable">';
            html += '<tr class="header">';
            html += '<th class="span2">ผู้ส่ง</th><th class="span3">หัวข้อ</th><th style="width:140px">วันที่ส่ง</th>';
            html += '</tr>';
            html += '</thead>';
            if (model.TotalRow > 0) {
                $.each(model.Messages, function (i, item) {
                    // var sendDate = $.formatDateTime('mm/dd/y g:ii a', new Date('2012/07/05 09:55:03')); 
                    var sendDate = formatJsonDatePrefix(item.SendDate);
                    // var sendDate = new Date(parseInt(item.SendDate.substr(6)));
                    //var sendDate = $.datepicker.formatDate('dd/mm/yy', new Date(parseInt(item.SendDate.substr(6))))
                    html += '<tr>';
                    html += '<td >' + item.FromName + '</td>';
                    html += '<td ><a target="_blank" href="' + GetUrl("Admin/Stat/MessageDetail?MessageID=" + item.MessageID + "&MessageCode=" + item.MessageCode) + '" >' + item.Subject + '</a></td>';

                    html += '<td >' + sendDate + '</td>';
                    html += '</tr>';

                });
            } else {
                html += '<tr>';
                html += '<td colspan="4" ><center>data not found</center></td>';
                html += '</tr>';
            }

            html += '</table>';
            html += '</div>';
            $('#' + name + '-content').html(html);
            GeneratePaging(name, 'LoadReportMsg', model);
            $('#' + name + '-content').prepend('<p class="text-center font_xl b">ศูนย์ข้อความ</p>');
        },
        error: function () {
        }
    });
}

function LoadReportQuotation(name, pageindex) { 
    setPageIndex(name, pageindex);
    data = {
        CompID: compid,
        Period: $('#ddlFindDatePeriod').val(),
        hidPageIndex: getPageIndex(name)
    };
    var html = "";
    $.ajax({
        url: GetUrl("Admin/Stat/QuotationTrade"),
        type: "POST",
        data: data,
        dataType: 'json',
        traditional: true,
        success: function (model) {
            OpenLoading(false);
            html += '<div class="border_table" style="width:100%;">';
            html += '<table class="table table-striped table-hover" style="margin:0; text-align:center;">';
            html += '<thead class="HeaderTable">';
            html += '<tr>';
            html += '<th class="span2">จาก</th><th class="span3">ชื่อสินค้าที่ขอราคา</th><th style="width:130px">วันที่ขอราคา</th>';
            html += '</tr>';
            html += '</thead>';
            if (model.TotalRow > 0) {
                $.each(model.Quotations, function (i, item) {
                    // var sendDate = $.formatDateTime('mm/dd/y g:ii a', new Date('2012/07/05 09:55:03')); 
                    var sendDate = formatJsonDatePrefix(item.SendDate);
                    html += '<tr>';
                    if (item.FromCompName != "" && item.FromCompName != null) {
                        html += '<td>' + item.FromCompName + '</td>';
                    }
                    else {
                        html += '<td>' + item.ReqFirstName + '</td>';
                    }
                    html += '<td><a target="_blank"  href="' + GetUrl("Report/QuotationDetail?QuotationID=" + item.QuotationID + "&&QuotationCode=" + item.QuotationCode) + '" >';
                                                                      "Report/QuotationDetail?QuotationID=" + item.QuotationID + "&&QuotationCode=" + item.QuotationCode
                    html += item.ProductName + '</a></td>';

                    html += '<td >' + sendDate + '</td>';
                    html += '</tr>';

                });
            } else {
                html += '<tr>';
                html += '<td colspan="3" ><center>data not found</center></td>';
                html += '</tr>';
            }

            html += '</table>';
            html += '</div>';
            $('#' + name + '-content').html(html);
            GeneratePaging(name, 'LoadReportQuotation', model);
            $('#' + name + '-content').prepend('<p class="text-center font_xl b">ขอราคาสินค้า</p>');

        },
        error: function () {
        }
    });
}

function LoadPageSupplier(name, pageindex) {
    setPageIndex(name, pageindex);
    doLoad(name, 'Admin/Stat/Supplier', function (model) {

        OpenLoading(false);
        var html = "";
        html += '<table class="table table-striped table-hover">';
        html += '<tr>';
        html += '<th>select</th><th>Company Name</th><th>Phone</th><th>Email</th><th>Level</th>';
        html += '</tr>';
        if (model.TotalRow > 0) {
            $.each(model.Suppliers, function (i, item) {
                var complv = "Logo_FreeSmall";

                if (item.CompLevel == 3) {
                    complv = "Logo_GoldSmall";
                }
                html += '<tr>';
                html += '<td ><a class="btn btn-small btn-sel-comp">show</a></td>';
                html += '<td><label class="list-comp" data-id="' + item.CompID + '" ';
                html += 'data-email="' + item.ContactEmail + '" data-phone="' + item.ContactPhone + '" >';
                html += '<a target="_blank" href="' + GetUrl("CompanyWebsite/Main/Index/" + item.CompID) + '" >' + item.CompName + '</a></label></td>';
                html += '<td >' + item.ContactPhone + '</td>';
                html += '<td >' + item.ContactEmail + '</td>';
                html += '<td ><span class="' + complv + '"></span></td>';
                html += '</tr>';
            });
        } else {
            html += '<tr>';
            html += '<td colspan="5" ><center>data not found</center></td>';
            html += '</tr>';
        }

        html += '</table>';
        $('#' + name + '-content').html(html);
        GeneratePaging(name, 'LoadPageSupplier', model);

    });

}
