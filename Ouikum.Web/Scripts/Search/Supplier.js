//autoComplete('/Search/Supplier/GetCompName');
 
if ($('#divCateItem').width() < 175) {
    $("#divContentCate").css("width", "16%").css("padding", "1%");
    $(".cate_content").css("width", "150%").css("margin-left", "-20px");
}

var CurrentLanguage = $('#hidAppLang').val();
var CurrentCultureInfo = $('#hidAppLang').val();
var LangUrl = '';
//if (CurrentCultureInfo == "en-US") {
//    LangUrl = 'en/';
//    console.log(CurrentCultureInfo);
//}

$(".txtPageIndex").keypress(function (e) {

    if (e.which == 13) {
        var Index = parseInt($(this).val());
        if (Index == 0) {
            Index = 1;
        }
        $(".hidPageIndex").val(Index);
        //console.log($(".hidPageIndex").val());
        var TotalPage = parseInt($(".hidTotalPage").val());
        var hideNext = parseInt(TotalPage) - 1;
        if (Index <= TotalPage) {
            if (Index < TotalPage) {
                $(".icon-chevron-right").css("opacity", "1").attr("onclick", "Next()");
            }
            if (Index == TotalPage) {
                $(".icon-chevron-right").css("opacity", "0").removeAttr("onclick");
            }
            SetOption();
        }
        else {
            $(".icon-chevron-right").css("opacity", "0").removeAttr("onclick");
            $('.hidPageIndex').val(TotalPage);
            SetOption();
        }
    }
});
function ChooseCompLevel(obj) {
    if (obj != undefined || obj != null) {
        if (obj.attr("checked") == true || obj.attr("checked") == "checked") {
            obj.attr("checked", "checked");
        } else {
            obj.removeAttr("checked", "checked");
        }
    }
}
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
function SelectPageSize(val) {
    $(".ddlPageSize option selected").removeAttr("selected");
    $(".ddlPageSize option[value='" + val + "']").attr("selected", "selected");
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
        txtSearch: $('.txt_search ').val(),
        BizTypeID: $('#ddlBizType option:selected').val(),
        CateLevel: level,
        CategoryID: id,
        CompLevelID: CompLevel,
        ProvinceID: $('#ProvinceID option:selected').val()
    }
    Onload();
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
        PIndex:  $(".hidPageIndex").val(),
        PSize: $('.ddlPageSize option:selected').val(),
        txtSearch: $('.txt_search ').val(),
        BizTypeID: $('#ddlBizType option:selected').val(),
        CateLevel: $("#CategoryID").attr("Class"),
        CategoryID: $("#CategoryID").val(),
        CompLevelID: CompLevel,
        ProvinceID: $("#ddlProvinceID option:selected").val()
    }
    Onload();
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
        txtSearch: $('.txt_search ').val(),
        BizTypeID: $('#ddlBizType option:selected').val(),
        CateLevel: $("#CategoryID").attr("Class"),
        CategoryID: $("#CategoryID").val(),
        CompLevelID: CompLevel,
        ProvinceID: $("#ddlProvinceID option:selected").val()
    }
    if (PageIndex == $('.hidPageIndex').val())
        return false;
    else
        Onload();
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
        txtSearch: $('.txt_search ').val(),
        BizTypeID: $('#ddlBizType option:selected').val(),
        CateLevel: $("#CategoryID").attr("Class"),
        CategoryID: $("#CategoryID").val(),
        CompLevelID: CompLevel,
        ProvinceID: $("#ddlProvinceID option:selected").val()
    }
    if (Obj == $('.hidPageIndex').val())
        return false;
    else
        Onload();
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
        txtSearch: $('.txt_search ').val(),
        BizTypeID: $('#ddlBizType option:selected').val(),
        CateLevel: $("#CategoryID").attr("Class"),
        CategoryID: $("#CategoryID").val(),
        CompLevelID: CompLevel,
        ProvinceID: $("#ddlProvinceID option:selected").val()
    }

    Onload();
}
function Onload() {
    OpenLoading(true, null, $('#Wrapper'));
    $.ajax({
        url: GetUrl("Search/Supplier/PostList"),
        data: data,
        success: function (data) {
            $('#ProductDetail').html(data);
            $('#totalrow').text($('.hidTotalRow').val().toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
            OpenLoading(false, null, $('#Wrapper'));
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
        txtSearch: $('.txt_search ').val(),
        BizTypeID: $('#ddlBizType option:selected').val(),
        CateLevel: $("#CategoryID").attr("Class"),
        CategoryID: $("#CategoryID").val(),
        CompLevelID: CompLevel,
        ProvinceID: $("#ddlProvinceID option:selected").val()
    }
    Onload();
}
function ReplaceUrl(name) {
    var Rename = name.replace(" ", "");
    Rename = Rename.replace(",", "-").replace("+", "-").replace("&", "-").replace("#", "-").replace("[", "-").replace("]", "-").replace("'", "-").replace("/", "-").replace(".", "").replace("%", "").replace(":", "");
    return Rename;
}
function SetGet(Obj) {
    OpenLoading(true);
    if (Obj.val() > 0) {
        var selected = $('#divSlideCate option:selected');
        window.location = GetUrl("Search/Supplier/List/Category/" + Obj.val() + "/1/" + ReplaceUrl(selected.text()));
    }
    else {
        window.location = GetUrl("Search/Supplier/List");
    }
}
function LoadRelateProductInSupplier() {
    if ($(".txtSearch").val() != null) {
        var textsearch = ReplaceTextSearch($(".txt_search ").val());
    }
    if (textsearch != undefined && textsearch != "" && textsearch != null) {
        var len = $('.divProductItem').length;
        for (var i = 0; i < len; i++) {
            var compid = parseInt($('.divProductItem').eq(i).attr('data-comp'), 10);
            var obj = {
                compid: compid,
         textsearch: textsearch
            }

       //     console.log(data);
            $.ajax({
                url: GetUrl("Search/Supplier/GetRelateSupplier"),
                data: obj,
                dataType: 'json',
                traditional: true,
                success: function (data) {
                    // console.log(data);
                    GenerateRelateProductInSupplier(data);

                },
                type: "POST"
            });
        }//end for
    }//end if

}
function GenerateRelateProductInSupplier(objProduct) {

    var html = "";
    var compid = 0;
    var compname = "";
    var count = 1;
    $.each(objProduct, function (i, item) {
        html = '<a class="fl_l border-polaroid-matching" target="_blank" href="' + GetUrl("Search/Product/Detail/" + item.ProductID + "/" + item.ProductName.replace(" ", "").replace(",", "-")) + '"  title="' + item.ProductName + '" >';
        if ($(window).width() <= 1366) {
            html += '<img alt="" class="img-matching" src="' + GetUpload("Product/" + item.CompID + "/" + item.ProductID + "/Thumb_" + item.ProductImgPath) + '" onload="resizeImg($(this),45,45);setElementMiddle(50, 50,$(this))" />';
        }
        else {
            html += '<img alt="" class="img-matching" src="' + GetUpload("Product/" + item.CompID + "/" + item.ProductID + "/Thumb_" + item.ProductImgPath) + '" onload="resizeImg($(this),70,70);setElementMiddle(72, 72,$(this))" />';
        }
         html += '</a>';
        $('#' + item.CompID).append(html);
        //        compid = item.CompID;
        //        compname = item.CompName;

        if (count >= 4) {
            var textsearch = ReplaceTextSearch($(".txt_search ").val());
            var str = ' ';
            str += '<a title="' + label.vldseemore + '" target="_blank" href="' + GetUrl(LangUrl+"CompanyWebsite/" + ReplaceTextSearch(item.CompName) + "/Product/" + item.CompID + "?TextSearch=" + textsearch) + '" >' + label.vldseemore + 'มเติม</a>';

            $('#see-more-' + item.CompID).append(str);
        }
//        console.log(count);
        count++;

    });
     
   // $('#'+compid).html(html);



//         if (objCookie.Compare != null && objCookie.Compare != undefined) {
//             var objCompare = objCookie.Compare;
//             //console.log(objCompare);
//             var Filter = objCompare.filter(function (ele, i) { return ele.productid == id });
//             //console.log('Filter : ' + Filter.length);
//             if (Filter.length > 0) {
//             // ซ้ำ
//                 bootbox.alert(label.vldcompare_exist);
//                 return false;
//             } else {
//                 return true;
    //            }
//                var proid = "";
//                $.each(obj.Products, function (idxProduct, obj) {
//                    proid += obj.productid + ",";
//                });

//                console.log(' product : ' + proid);

} 
//$('#TextSearch').keypress(function () {
//    OpenListSearch(true);
//});
//$('#TextSearch').click(function () {
//    var width = $('#search-main').width();
//    $('#list-search').width(width - 2);
//    OpenListSearch(true);
//}); 
//function OpenListSearch(isOpen) {
//    if (isOpen != null && isOpen != undefined) {
//        if (isOpen) {
//            $('#list-search').removeClass('hidden')
//            $('#list-search').slideDown();
//        } else {
//            $('#list-search').addClass('hidden')
//            $('#list-search').slideUp();
//        }
//    } else {
//        if ($('#list-search').hasClass('hidden')) {
//            $('#list-search').removeClass('hidden')
//            $('#list-search').slideDown();
//        } else {
//            $('#list-search').addClass('hidden')
//            $('#list-search').slideUp();
//        }
//    }
//}
//function SetActiveListSearch(index) {
//    $('.list-search-text').removeClass('active');
//    $('.list-search-text').eq(index).addClass('active');

//    $('.icon-active').hide();
//    $('.icon-active').eq(index).show();
//} 
$(window).scroll(function () {
    if ($(window).scrollTop() > ($('#List').height() + 8)) {
        $('.PagingUC .input-append .txtPageIndex').css('z-index', -1);
        $('.PagingUC .input-append .offset').css('z-index', -1);
    }
    else {
        $('.PagingUC .input-append .txtPageIndex').css('z-index', 0);
        $('.PagingUC .input-append .offset').css('z-index', 0);
    }
    if ($(window).scrollTop() > ($('#List').height()+80)) {
        $('.SlideFeat').css('z-index', -1);
    }
    else {
        $('.SlideFeat').css('z-index', 0);
    }
});