var CurrentLanguage = $('#hidAppLang').val();
var CurrentCultureInfo = $('#hidAppLang').val();
var LangUrl = '';
//if (CurrentCultureInfo == "en-US") {
//    LangUrl = 'en/';

//}
//autoComplete('/Search/Product/GetProductName');
checkIEVersion();
$('.divPicPro').css("width", $('.divPicPro').width());

if ($('#divCateItem').width() < 175) {
    $("#divContentCate").css("width", "16%").css("padding", "1%");
    $(".cate_content").css("width", "150%").css("margin-left", "-20px");
}
//-----------------Scrollbar Setting--------------------------
var sc_memberin = $('#scrollbarMembercard .overview').height();
var sc_memberout = $('#scrollbarMembercard .viewport ').height();
var sc_desin = $('#scrollbarShortdes .overview').height();
var sc_desout = $('#scrollbarShortdes .viewport ').height();
console.log(sc_desin, sc_desout);
if (sc_memberin < sc_memberout) {
    $('#scrollbarMembercard .track').css('display', 'none');
}
if (sc_desin < sc_desout) {
    $('#scrollbarShortdes .track').css('display', 'none');
}
//---------------------------
var data = "";
function ClearCondition() {
    $('input[name=chkpd]:checked').removeAttr("checked");
    $("#ddlBizType").val(0);
    $("#ddlProvinceID").val(0);
    $("#ddlSort").val(1);
    $("#ProvinceID").val(0);
    $(".txtPageIndex").val(1);
}
function SelectedBizType(val) {
    $("#ddlBizType option[value='" + val + "']").attr("selected", "selected");
    SetSearchOption();
}
function SelectedSort(val) {
    $("#ddlSort option[value='" + val + "']").attr("selected", "selected");
    SetOption();
}
function SelectedProvince(val) {
    $("#ddlProvinceID option[value='" + val + "']").attr("selected", "selected");
    SetSearchOption();
}
function SelectCate(id, level, Obj) {
    Obj.css("text-shadow", "0 1px 0 White").css("text-decoration", "none").css("font-weight", "bold");

    ClearCondition();
    $("#CategoryID").attr("class", level).attr("value", id);
    var CompLevel = 0;
    if ($('input[name=chkpd]:checked').attr("checked") == true || $('input[name=chkpd]:checked').attr("checked") == "checked") {
        CompLevel = 3;
    }
    data = {
        Sort: $("#ddlSort").val(),
        PIndex: 1,
        PSize: 20,
        txtSearch: $('#TextSearch').val(),
        BizTypeID: $('#ddlBizType option:selected').val(),
        CateLevel: level,
        CategoryID: id,
        CompLevelID: CompLevel,
        ProvinceID: $('#ProvinceID option:selected').val()
    }
    Onload(data);
}
function SetOption() {
    var CateLV = 0;
    var CateID = 0;
    var CompLevel = 0;
    if ($('input[name=chkpd]:checked').attr("checked") == true || $('input[name=chkpd]:checked').attr("checked") == "checked") {
        CompLevel = 3;
    }
    data = {
        Sort: $("#ddlSort").val(),
        PIndex: $(".txtPageIndex").val(),
        PSize: $('.hidPageSize').val(),
        txtSearch: $('#TextSearch').val(),
        BizTypeID: $('#ddlBizType option:selected').val(),
        CateLevel: $("#CategoryID").attr("Class"),
        CategoryID: $("#CategoryID").val(),
        CompLevelID: CompLevel,
        ProvinceID: $("#ddlProvinceID option:selected").val()
    }
    Onload(data);
}
function OpenLoading(isLoad, img, obj) {
    if (isLoad == true) {
        if (img == null) {
            img = '<div class="icon-loader"></div>';
        } else {
            img = '<img src=\"' + img + '\" >';
        }
        if ($('#loading').length == 0) {
            if (obj == null || obj == undefined) {
                $('body').prepend('<div id="loading">&nbsp;</div><div id="imgloading" style="top: 400.5px; left: 779.5px;">' + img + '</div>');
            } else {
                obj.prepend('<div id="loading">&nbsp;</div><div id="imgloading" style="top: 400.5px; left: 779.5px;">' + img + '</div>');
            }
            $("#loading").css({ 'filter': 'alpha(opacity=50)', 'opacity': '0.5' });
            //$("#imgloading").position({ my: "center", at: "center", of: "#loading" });
        }

    } else {
        $('#loading').remove(); $('#imgloading').remove();
    }
}
function Onload(data) {
    OpenLoading(true);
    $.ajax({
        url: GetUrl("Search/Product/PostList"),
        data: data,
        success: function (data) {
            $('#ProductDetail').html(data);
            if ($('.hidTotalRow').val() == null) {
                $('#totalrow').text(0);
            }
            else {
                $('#totalrow').text($('.hidTotalRow').val().toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
            }
            OpenLoading(false);
            $('body').scrollTop(0);
        },
        type: "POST"
    });
}
function SetRefresh() {
    $(".divProductItem").slideDown("slow");
    $(".divProductItem").css("display", "inline")
}
function SetSearchOption() {
    var CateLV = 0;
    var CateID = 0;
    var CompLevel = 0;
    if ($('input[name=chkpd]:checked').attr("checked") == true || $('input[name=chkpd]:checked').attr("checked") == "checked") {
        CompLevel = 3;
    }
    data = {
        Sort: $("#ddlSort").val(),
        PIndex: 1,
        PSize: $('.hidPageSize').val(),
        txtSearch: $('#TextSearch').val(),
        BizTypeID: $('#ddlBizType option:selected').val(),
        CateLevel: $("#CategoryID").attr("Class"),
        CategoryID: $("#CategoryID").val(),
        CompLevelID: CompLevel,
        ProvinceID: $("#ddlProvinceID option:selected").val()
    }
    Onload(data);
}
function NumPage(PageIndex, PageSize) {
   
    var CateLV = 0;
    var CateID = 0;
    var CompLevel = 0;
    if ($('input[name=chkpd]:checked').attr("checked") == true || $('input[name=chkpd]:checked').attr("checked") == "checked") {
        CompLevel = 3;
    }
    data = {
        Sort: $("#ddlSort").val(),
        PIndex: PageIndex,
        PSize: PageSize,
        txtSearch: $('#TextSearch').val(),
        BizTypeID: $('#ddlBizType option:selected').val(),
        CateLevel: $("#CategoryID").attr("Class"),
        CategoryID: $("#CategoryID").val(),
        CompLevelID: CompLevel,
        ProvinceID: $("#ddlProvinceID option:selected").val()
    }
    if (PageIndex == $('.hidPageIndex').val())
        return false;
    else
        Onload(data);
}
function SubmitPage(Obj) {
    var CateLV = 0;
    var CateID = 0;
    var CompLevel = 0;
    if ($('input[name=chkpd]:checked').attr("checked") == true || $('input[name=chkpd]:checked').attr("checked") == "checked") {
        CompLevel = 3;
    }
    data = {
        Sort: $("#ddlSort").val(),
        PIndex: Obj,
        PSize: $('.hidPageSize').val(),
        txtSearch: $('#TextSearch').val(),
        BizTypeID: $('#ddlBizType option:selected').val(),
        CateLevel: $("#CategoryID").attr("Class"),
        CategoryID: $("#CategoryID").val(),
        CompLevelID: CompLevel,
        ProvinceID: $("#ddlProvinceID option:selected").val()
    }
    if (Obj == $('.hidPageIndex').val())
        return false;
    else
        Onload(data);
}
function SelectedPageSize(Obj) {
    var CateLV = 0;
    var CateID = 0;
    var CompLevel = 0;
    if ($('input[name=chkpd]:checked').attr("checked") == true || $('input[name=chkpd]:checked').attr("checked") == "checked") {
        CompLevel = 3;
    }
    data = {
        Sort: $("#ddlSort").val(),
        PIndex: 1,
        PSize: Obj,
        txtSearch: $('#TextSearch').val(),
        BizTypeID: $('#ddlBizType option:selected').val(),
        CateLevel: $("#CategoryID").attr("Class"),
        CategoryID: $("#CategoryID").val(),
        CompLevelID: CompLevel,
        ProvinceID: $("#ddlProvinceID option:selected").val()
    }

    Onload(data);
}



function SetGet(Obj) {
    OpenLoading(true);
    if (Obj.val() > 0) {
        var selected = $('#divSlideCate option:selected');

        window.location = GetUrl("Search/Product/List/Category/" + Obj.val() + "/1/" + ReplaceUrl(selected.text()));
    }
    else {
        window.location = GetUrl("Search/Product/List");
    }
}

function GetUrlBreadcump(cateid,name) {
   
    if (cateid.val() > 0) {
     
        return GetUrl("Search/Product/List/Category/" + cateid.val() + "/1/" + ReplaceUrl(name));
    }
    else {
        return GetUrl("Search/Product/List");
    }
}

function BindSearchProduct(){
    OpenLoading(true);
    $.ajax({
        url: GetUrl("Search/Product/BindList"), 
        success: function (data) {
            console.log(data);
            OpenLoading(false);
        },
        type: "POST"
    });
}
function ReplaceUrl(name) {
    var Rename = name.replace(" ", "");
    Rename = Rename.replace(",", "-").replace("+", "-").replace("&", "-").replace("#", "-").replace("[", "-").replace("]", "-").replace("'", "-").replace("/", "-").replace(".", "").replace("%", "").replace(":", "");
    return Rename;
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
                alert(msg);
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