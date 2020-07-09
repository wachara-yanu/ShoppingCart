//autoComplete('/MyB2B/Favorite/GetBuyleadName');

$("#Searchtext").click(function () {
    var Type = $("#SearchType").val();
    switch (Type) {
        case "Product":
            window.location = GetUrl("MyB2B/Favorite/Index/?txtSearch=" + $('#txtSearch').val());
            break;
        case "Supplier":
            window.location = GetUrl("MyB2B/Favorite/Suppliers/?txtSearch=" + $('#txtSearch').val());
            break;
        case "Buylead":
            SetOption();
            break;
    }
});


$("#txtSearch").keypress(function (e) {
    if (e.which == 13) {
        var Type = $("#SearchType").val();
        switch (Type) {
            case "Product":
                window.location = GetUrl("MyB2B/Favorite/Index/?txtSearch=" + $('#txtSearch').val());
                break;
            case "Supplier":
                window.location = GetUrl("MyB2B/Favorite/Suppliers/?txtSearch=" + $('#txtSearch').val());
                break;
            case "Buylead":
                SetOption();
                break;
        }
    }
});


//$(".txtPageIndex").keypress(function (e) {
//    if (e.which == 13) {
//        var Index = parseInt($('.txtPageIndex').val());
//        var TotalPage = parseInt($(".hidTotalPage").val());
//        var hideNext = parseInt(TotalPage) - 1;
//        if (Index <= TotalPage) {
//            if (Index == 0) {
//                $(".txtPageIndex").val(1);
//            }
//            if (Index == TotalPage) {
//                $(".icon-chevron-right").css("opacity", "0").removeAttr("onclick");
//            }

//        }
//        else {
//            $(".txtPageIndex").val(TotalPage);
//        }
//        SetOption();
//    }
//});

var data = "";
function SelectedBizType(val) {
    $("#ddlBizType option[value='" + val + "']").attr("selected", "selected");
}
function SelectedSort(val) {
    $("#ddlSort option[value='" + val + "']").attr("selected", "selected");
    SetOption();
}
function SelectPageSize(val) {
    $("#ddlPaging option selected").removeAttr("selected");
    $("#ddlPaging option[value='" + val + "']").attr("selected", "selected");
    SetOption();
}
function SelectedProvince(val) {
    $("#ProvinceID option[value='" + val + "']").attr("selected", "selected");
}
function SelectCate(id, level, Obj) {
    Obj.css("text-shadow", "0 1px 0 White").css("text-decoration", "none").css("font-weight", "bold");
    data = {
        PIndex: 1,
        PSize: 20,
        txtSearch: $('#txtSearch').val()
    }
    Onload();
}
function SetOption() {
    data = {
        PIndex: $(".txtPageIndex").val(),
        PSize: $('.ddlPageSize option:selected').val(),
        txtSearch: $('#txtSearch').val()
    }
    Onload();
}
//function Next() {

//    var Index = parseInt($('.txtPageIndex').val());
//    var TotalPage = parseInt($(".hidTotalPage").val());
//    var hideNext = parseInt(TotalPage) - 1;
//    if (Index < TotalPage) {
//        if (Index == hideNext) {
//            $(".icon-chevron-right").css("opacity", "0").removeAttr("onclick");
//        }
//        data = {
//            PIndex: Index + 1,
//            PSize: $('#ddlPaging option:selected').val(),
//            txtSearch: $('#txtSearch').val()
//        }
//        Onload();
//    }
//}
//function Prev() {
//    $(".icon-chevron-right").css("opacity", "1");
//    var Index = parseInt($('.txtPageIndex').val());
//    if (Index > 1) {
//        data = {
//            PIndex: Index - 1,
//            PSize: $('#ddlPaging option:selected').val(),
//            txtSearch: $('#txtSearch').val()
//        }
//        Onload();
//    }
//}
function Onload() {
    OpenLoading(true);
    $.ajax({
        url: GetUrl("MyB2B/Favorite/Buyers"),
        data: data,
        success: function (data) {
            $('#ProductDetail').html(data);
            $('.icon-chevron-right').attr("onclick", "Next();")
            OpenLoading(false);
        },
        type: "POST"
    });
}
function ChooseBuylead(obj) {

    if (obj != undefined || obj != null) {
        if (obj.attr("checked") == true || obj.attr("checked") == "checked") {
            obj.parents(".divProductItem").addClass('hdivProductItem');
            obj.parents(".divProductItemG").addClass('hdivProductItem').attr("Show", "true").find(".divSlide").animate({ top: "-35%" }, 0);
            obj.find(".divAct").css({ 'background-color': '#F0F0F0', 'border': '1px solid #ccc', "height": "112px" });
            obj.addClass("Buy-data");
            obj.attr("checked", "checked");
        } else {
            obj.parents(".divProductItem").removeClass('hdivProductItem');
            obj.parents(".divProductItemG").removeClass('hdivProductItem').removeAttr("Show");
            obj.find(".divAct").css({ 'background-color': '#F0F0F0', "height": "112px" });
            obj.removeClass("Buy-data");
            obj.removeAttr("checked", "checked");
        }
    } else {
        for (var i = 0; i < (".ChooseBuylead").length; i++) {
            ChooseBuylead($(".ChooseBuylead").eq(i - 1));
            ChooseBuylead($(".ChooseBuylead").eq(i - 1));
        }
    }
}

function MultiDel(Obj) {
    if ($('.hdivProductItem').length > 0) {
        OpenLoading(true);
        bootbox.confirm(label.vldconfirm_del_data, "Cancel", "Yes", function (e) {
            if (e) {
                GetBuyleadDel();
                $.ajax({
                    url: GetUrl("MyB2B/Favorite/DeleteFavBuylead"),
                    data: { BuyleadID: BuyleadDel },
                    traditional: true,
                    success: function (data) {
                        $('.hdivProductItem').remove();
                        SetOption();
                        OpenLoading(false);
                    },
                    error: function () {
                        OpenLoading(false);
                    },
                    type: "POST"
                });
            }
            else {
                OpenLoading(false);
            }
        })
    } else {
        bootbox.alert(label.vldno_item_selected);
    }
}

var BuyleadDel = new Array();
//============ Get Buylead For Delete Item ===================//
function GetBuyleadDel() {
    var len = $('.Buy-data').length;
    for (var i = 0; i < len; i++) {
        BuyleadDel[i] = $('.Buy-data').eq(i).attr('data-id');
    }
    console.log(BuyleadDel);
}
function CheckBuyleadAll(Obj) {
    if (Obj.attr("checked") == true || Obj.attr("checked") == "checked") {
        $(".ChooseBuylead").attr("checked", "checked");
        $(".ChooseBuylead").addClass("Buy-data");
        $(".divProductItem").addClass("hdivProductItem");
        $(".divProductItemG").addClass("hdivProductItem");
    } else {
        $(".ChooseBuylead").removeAttr("checked");
        $(".ChooseBuylead").removeClass("Buy-data");
        $(".divProductItem").removeClass("hdivProductItem");
        $(".divProductItemG").removeClass("hdivProductItem");
    }
}
$('.btn-tootip-top').tooltip({ placement: 'top' });
function DeleteFav(Obj) {
    OpenLoading(true);
    bootbox.confirm(label.vldconfirm_del_data, "Cancel", "Yes", function (e) {
        if (e) {
        $.ajax({
            url: GetUrl("MyB2B/Favorite/DeleteFavBuylead"),
            data: { BuyleadID: Obj.attr("data-id") },
            traditional: true,
            success: function (data) {

                Obj.parents('.divProductItem').remove();
                SetOption();


                OpenLoading(false);
            },
            error: function () {
                OpenLoading(false);
            },
            type: "POST"
        });
    }
    else {
        OpenLoading(false);
        }
    })
}

