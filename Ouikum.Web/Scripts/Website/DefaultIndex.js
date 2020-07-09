
checkIEVersion();

function ProductSearch() {
    if ($("#TextSearch").val() != "") {
        $("#formSearch").submit();
    } else {
        return false;
    }
}
function NumPage(PageIndex, PageSize) {
    data = {
        TextSearch: $('#TextSearch').val(),
        hidPageIndex: PageIndex,
        hidPageSize: PageSize,
        CateID: $('#hidCateID').val(),
        CateLevel: $('#hidCateLevel').val(),
        GroupID: $('#hidGroupID').val(),
        CompID: $('#hidCompID').val()
    }
    if (PageIndex == $('.hidPageIndex').val())
        return false;
    else
        Onload();
}
function SubmitPage(Obj) {

    data = {
        TextSearch: $('#TextSearch').val(),
        hidPageIndex: Obj,
        hidPageSize: $('.hidPageSize').val(),
        CateID: $('#hidCateID').val(),
        CateLevel: $('#hidCateLevel').val(),
        GroupID: $('#hidGroupID').val(),
        CompID: $('#hidCompID').val()
    }
    if (Obj == $('.hidPageIndex').val())
        return false;
    else
        Onload();
}
function SelectedPageSize(Obj) {
    data = {
        TextSearch: $('#TextSearch').val(),
        hidPageIndex: 1,
        hidPageSize: Obj,
        CateID: $('#hidCateID').val(),
        CateLevel: $('#hidCateLevel').val(),
        GroupID: $('#hidGroupID').val(),
        CompID: $('#hidCompID').val()
    }

    Onload();
}
function Onload() {
    OpenLoading(true, null, $('#WebsiteAllContent'));
    $.ajax({
        url: GetUrl("Website/Index"),
        data: data,
        success: function (data) {

            $('#IndexGallery').html(data);
            //$('.gallery_inner').append('<div class="v_line" style="position:absolute;top:0;left:-0.1%; min-height: 550px;"></div>');
            var content_height = $("#IndexGallery").outerHeight();
            $(".v_line").height(content_height);
            OpenLoading(false, null, $('.bg_website'));

        },
        error: function () {
            bootbox.alert("Sorry, Your request is not successful.");
            OpenLoading(false, null, $('.bg_website'));
        },
        type: "POST"
    });
    return false;
}


//-----------------------------Url+parameter --------------------------------//
function SendRequestPrice(ProID) {

    window.open(GetUrl("MyB2B/Quotation/RequestPrice/" + parseInt(ProID)));
}
function SendMessage(id) {
    var compid = $("#hidCompID").val();

    window.open(GetUrl("Message/Contact?ToCompID=" + parseInt(compid) + "&&ProductID=" + parseInt(id)));
}
//-----------------------------Set job data on Madal--------------------------------//
function PassJobData(jobid, jobname) {
    $("#hidJobID").val(jobid);
    $("#hidJobName").val(jobname);
    $("#hidJobUrl").val(window.location)

}
//---------------------------Contact Company----------------------------------//
function ContactCompany() {

    $("#ContectUsForm").submit(function () {

        if ($(":input").hasClass('error') == true) {
            return false;
        } else {
            OpenLoading(true);
            var tocompid = $("#hidToCompID").val();
            var subject = $("#hidtxtSubject").val();
            var fromname = $("#txtFromName").val();
            var fromemail = $("#txtFromEmail").val();
            var detail = $("#txtMsgDetail").val();
            var isimportance = false;
            var FromContactPhone = $("#txtFromContactPhone").val();
            $.ajax({
                url: GetUrl("Message/ContactSupplier"),
                type: "post",
                data: {
                    ToCompID: tocompid,
                    FromName: fromname,
                    FromEmail: fromemail,
                    Subject: subject,
                    MsgDetail: detail,
                    IsImportance: isimportance,
                    FromContactPhone: FromContactPhone
                },
                success: function () {
                    SendContactSuccess();
                }
            });
            return false;
        }
    });

}

function SendContactSuccess() {
    OpenLoading(false);
    bootbox.alert("Send Successful");
    window.location.reload(true);
}

// GetDefaultGmap
function GetDefaultGmap(CompProvinceID) {
    OpenLoading(true);

    $.ajax({
        url: GetUrl("Website/setDefaultGmap"),
        data: { ProvinceID: parseInt(CompProvinceID) },
        type: "POST",
        datatype: "json",
        traditional: true,
        success: function (data) {

            initialize(data["GMapLatitude"], data["GMapLongtitude"], data["GPinLatitude"], data["GPinLongtitude"], data["GZoom"]);

            $('#GMapLatitude').val(data["GMapLatitude"]);
            $('#GMapLongtitude').val(data["GMapLongtitude"]);
            $('#GPinLatitude').val(data["GPinLatitude"]);
            $('#GPinLongtitude').val(data["GPinLongtitude"]);
            $('#GZoom').val(data["GZoom"]);
            $("#hidContactProvinceID").val(CompProvinceID);

            OpenLoading(false);
        },
        error: function () {
            //bootbox.alert("error : ไม่สามารถตรวจสอบข้อมูลได้");
            OpenLoading(false);
        }
    });
}
function setCompNameStyle() {
    var CompName_H = $("h4.media-heading").height();
    var sw = screen.width;
    if (sw < 1500 && CompName_H > 20) {
        var cssObj = {

            'font-size': '18px',
            'margin-top': '0px',
            'line-height': '22px'
        };
        $("h4.media-heading").css(cssObj);
    } else if (sw > 1500 && CompName_H > 20) {
        var cssObj = {
            'font-size': '18px'
        };
        $("h4.media-heading").css(cssObj);
    }
}
//Checkpaging Website product
function CheckWebsitePaging(Page) {
    var isPass = true;
    var PageIndex = parseInt($('#hidPageIndex').val(), 20);
    var TotalPage = parseInt($('#hidTotalPage').val(), 20);

    if (Page == 1) {
        if (TotalPage == 1) {
            $('.btn-next').removeAttr('onclick');
            $('.btn-next').css('opacity', '0.4');
            $('.btn-next').css('cursor', 'default');
            $('.btn-prev').removeAttr('onclick');
            $('.btn-prev').css('opacity', '0.4');
            $('.btn-prev').css('cursor', 'default');
        } else {
            $('.btn-next').attr('onclick', 'Next();');
            $('.btn-next').css('opacity', '1');
            $('.btn-next').css('cursor', 'pointer');
            $('.btn-prev').removeAttr('onclick');
            $('.btn-prev').css('opacity', '0.4');
            $('.btn-prev').css('cursor', 'default');
        }
    } else if (Page == TotalPage) {
        $('.btn-prev').attr('onclick', 'Prev();');
        $('.btn-prev').css('opacity', '1');
        $('.btn-prev').css('cursor', 'pointer');
        $('.btn-next').removeAttr('onclick');
        $('.btn-next').css('opacity', '0.4');
        $('.btn-next').css('cursor', 'default');
    } else if (Page > 1 && Page < TotalPage) {
        $('.btn-prev').attr('onclick', 'Prev();');
        $('.btn-prev').css('opacity', '1');
        $('.btn-prev').css('cursor', 'pointer');
        $('.btn-next').attr('onclick', 'Next();');
        $('.btn-next').css('opacity', '1');
        $('.btn-next').css('cursor', 'pointer');
    } else {
        $('.txtPageIndex').eq(0).val(PageIndex);
        isPass = false;
    }

    return isPass;
}

function getInternetExplorerVersion()
    // Returns the version of Windows Internet Explorer or a -1
    // (indicating the use of another browser).
{
    var rv = -1; // Return value assumes failure.
    if (navigator.appName == 'Netscape') {
        var ua = navigator.userAgent;
        var re = new RegExp("MSIE ([0-9]{1,}[\.0-9]{0,})");
        if (re.exec(ua) != null)
            rv = parseFloat(RegExp.$1);
    }
    return rv;
}

function checkIEVersion() {
    var isIE = getCookie("isIE");
    var ver = getInternetExplorerVersion();
    if (isIE == null || isIE != ver) {
        var msg = "You're not using Windows Internet Explorer.";

        if (ver > -1) {
            if (ver <= 8.0) {
                msg = '"Internet Explorer ของคุณมีเวอร์ชั่นที่ต่ำกว่าปัจจุบัน อาจทำให้การแสดงผลเว็บไซต์มีข้อผิดพลาดเกิดขึ้น เพื่อการใช้งานเว็บไซต์ได้อย่างเต็มประสิทธิภาพ กรุณาอัพเกรดเวอร์ชั่น Internet Explorer ของคุณ';
                bootbox.alert(msg);
                ChangIsIE(ver)
            }
        }
    }
}

function ChangIsIE(ver) {
    var isIE = getCookie("isIE");
    if (isIE == null || isIE != ver) {
        deleteCookie("isIE", "/");
        setCookie("isIE", ver, "1", "/");
    }
}