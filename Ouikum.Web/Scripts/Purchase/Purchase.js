//autoComplete('/Purchase/Search/GetBuyleadName');
$("#Searchtext").click(function () {
    var Type = $("#SearchType").val();
    switch (Type) {
        case "Product":
            window.location = GetUrl("Search/Product/List/" + $('#TextSearch').val());
            break;
        case "Supplier":
            window.location = GetUrl("Search/Supplier/List/" + $('#TextSearch').val());
            break;
        case "Buyer":
            if ($("#BuyleadID").val() != null) {
                window.location = GetUrl("Purchase/Search/" + $('#TextSearch').val());
             } else {
                ClearCondition();
                SetOption();
            }
            break;
    }
});
$("#TextSearch").keypress(function (e) {
    if (e.which == 13) {
        var Type = $("#SearchType").val();
        switch (Type) {
            case "Product":
                window.location = GetUrl("Search/Product/List/" + $('#TextSearch').val());
                break;
            case "Supplier":
                window.location = GetUrl("Search/Supplier/List/" + $('#TextSearch').val());
                break;
            case "Buyer":
                if ($("#BuyleadID").val() != null) {
                    window.location = GetUrl("Purchase/Search/" + $('#TextSearch').val());
                } else {
                    ClearCondition();
                    SetOption();
                }
                break;
        }
    }
});

function CheckAll(Obj) {
    if (Obj.attr("checked") == true || Obj.attr("checked") == "checked") {
        $(".ChooseBuylead").attr("checked", "checked");
    } else {
        $(".ChooseBuylead").removeAttr("checked");
    }
}
var data = "";
function ClearCondition() {
    $('#ddlBuyleadType option:selected').val(0);
    $("#ddlCategory").val(0);
    $("#ddlProvinceID option:selected").val(0);
    $("#chkBuyleadExpDate").removeAttr("checked");
    $("#chkBuyleadNotExpDate").removeAttr("checked");
    $(".txtPageIndex").val(1);
}
function SelectBuyleadType(val) {
    $("#ddlBuyleadType option[value='" + val + "']").attr("selected", "selected");
    SetOption();
}
function SelectedProvince(val) {
    $("#ddlProvinceID option[value='" + val + "']").attr("selected", "selected");
    SetOption();
}
function SelectedCategory(val) {
    $("#ddlCategory option selected").removeAttr("selected");
    $("#ddlCategory option[value='" + val + "']").attr("selected", "selected");
    SetOption();
}
function SetOption() {
    document.getElementById("chkBuyleadNotExpDate").checked = false;
    var BuyleadExpDate;
    var BuyleadNotExpDate = 0;
    var CategoryID = $("#ddlCategory").val();
    if ($('input[name=chkBuyleadExpDate]:checked').attr("checked") == true || $('input[name=chkBuyleadExpDate]:checked').attr("checked") == "checked") {
        BuyleadExpDate = 1;
    } else {
        BuyleadExpDate = 0;
    }

    if ($(".txtPageIndex").val() < 1) {
        var PIndex = 1;
    } else {
        var PIndex = $(".hidPageIndex").val();
        if ($(".txtPageIndex").val() > $(".hidTotalPage").val()) {
             PIndex = $(".hidPageIndex").val();
        }
    }
    
    data = {
        txtSearch: $('#TextSearch').val(),
        BuyleadType: $('#ddlBuyleadType option:selected').val(),
        BuyleadNotExpDate: BuyleadNotExpDate,
        BuyleadExpDate: BuyleadExpDate,
        CategoryID: CategoryID,
        ProvinceID: $("#ddlProvinceID option:selected").val(),
        PageTotal: $(".hidPageSize").val(),
        PIndex: PIndex
    }

    Onload();
}

function SetOptionNot() {
    document.getElementById("chkBuyleadExpDate").checked = false;
    var BuyleadNotExpDate;
    var BuyleadExpDate = 0;
    var CategoryID = $("#ddlCategory").val();
    if ($('input[name=chkBuyleadNotExpDate]:checked').attr("checked") == true || $('input[name=chkBuyleadNotExpDate]:checked').attr("checked") == "checked") {
        BuyleadNotExpDate = 1;
    } else {
        BuyleadNotExpDate = 0;
    }

    if ($(".txtPageIndex").val() < 1) {
        var PIndex = 1;
    } else {
        var PIndex = $(".hidPageIndex").val();
        if ($(".txtPageIndex").val() > $(".hidTotalPage").val()) {
            PIndex = $(".hidPageIndex").val();
        }
    }

    data = {
        txtSearch: $('#TextSearch').val(),
        BuyleadType: $('#ddlBuyleadType option:selected').val(),
        BuyleadNotExpDate: BuyleadNotExpDate,
        BuyleadExpDate: BuyleadExpDate,
        CategoryID: CategoryID,
        ProvinceID: $("#ddlProvinceID option:selected").val(),
        PageTotal: $(".hidPageSize").val(),
        PIndex: PIndex
    }

    Onload();
}

function Onload() {
    OpenLoading(true, null, $('.navbar-inner'));
    $.ajax({
        url: GetUrl("Purchase/Search/Index"),
        data: data,
        success: function (data) {
            $('#divBuyleadList').html(data); 
            OpenLoading(false, null, $('.navbar-inner'));
            $('body').scrollTop(0);
        },
        type: "POST"
    });
}
function SendMessage(ID){
    if ($('#ChkHaveUser').val() > 0) {
        window.location = GetUrl("Message/Contact/?BuyleadID=" + ID);
    } else { bootbox.alert(label.vldplease_signIn); }
}
function NumPage(PageIndex, PageSize) {
    var BuyleadExpDate;
    if ($('input[name=chkBuyleadExpDate]:checked').attr("checked") == true || $('input[name=chkBuyleadExpDate]:checked').attr("checked") == "checked") {
        BuyleadExpDate = 1;
    } else {
        BuyleadExpDate = 0;
    }
    var BuyleadNotExpDate;
    if ($('input[name=chkBuyleadNotExpDate]:checked').attr("checked") == true || $('input[name=chkBuyleadNotExpDate]:checked').attr("checked") == "checked") {
        BuyleadNotExpDate = 1;
    } else {
        BuyleadNotExpDate = 0;
    }
    data = {
        txtSearch: $('#TextSearch').val(),
        CategoryID: $("#ddlCategory").val(),
        ProvinceID: $("#ddlProvinceID option:selected").val(),
        BuyleadNotExpDate: BuyleadNotExpDate,
        BuyleadExpDate: BuyleadExpDate,
        BuyleadType: $('#ddlBuyleadType option:selected').val(),
        PIndex: PageIndex,
        PageTotal: PageSize
    }
    if (PageIndex == $('.hidPageIndex').val())
        return false;
    else
        Onload();
}
function SubmitPage(Obj) {
    var BuyleadExpDate;
    if ($('input[name=chkBuyleadExpDate]:checked').attr("checked") == true || $('input[name=chkBuyleadExpDate]:checked').attr("checked") == "checked") {
        BuyleadExpDate = 1;
    } else {
        BuyleadExpDate = 0;
    }
    var BuyleadNotExpDate;
    if ($('input[name=chkBuyleadNotExpDate]:checked').attr("checked") == true || $('input[name=chkBuyleadNotExpDate]:checked').attr("checked") == "checked") {
        BuyleadNotExpDate = 1;
    } else {
        BuyleadNotExpDate = 0;
    }
    data = {
        txtSearch: $('#TextSearch').val(),
        CategoryID: $("#ddlCategory").val(),
        ProvinceID: $("#ddlProvinceID option:selected").val(),
        BuyleadNotExpDate: BuyleadNotExpDate,
        BuyleadExpDate: BuyleadExpDate,
        BuyleadType: $('#ddlBuyleadType option:selected').val(),
        PIndex: Obj,
        PageTotal: $('.hidPageSize').val()
    }
    if (Obj == $('.hidPageIndex').val())
        return false;
    else
        Onload();
}
function SelectedPageSize(Obj) {
    var BuyleadExpDate;
    if ($('input[name=chkBuyleadExpDate]:checked').attr("checked") == true || $('input[name=chkBuyleadExpDate]:checked').attr("checked") == "checked") {
        BuyleadExpDate = 1;
    } else {
        BuyleadExpDate = 0;
    }
    var BuyleadNotExpDate;
    if ($('input[name=chkBuyleadNotExpDate]:checked').attr("checked") == true || $('input[name=chkBuyleadNotExpDate]:checked').attr("checked") == "checked") {
        BuyleadNotExpDate = 1;
    } else {
        BuyleadNotExpDate = 0;
    }
    data = {
        txtSearch: $('#TextSearch').val(),
        CategoryID: $("#ddlCategory").val(),
        ProvinceID: $("#ddlProvinceID option:selected").val(),
        BuyleadNotExpDate: BuyleadNotExpDate,
        BuyleadExpDate: BuyleadExpDate,
        BuyleadType: $('#ddlBuyleadType option:selected').val(),
        PIndex: 1,
        PageTotal: Obj
    }

    Onload();
}
$("body").click(function (event) {
    if ($(event.target).hasClass('search-select')) {
        OpenListSearch(true);
    } else {
        OpenListSearch(false);
    }
});
$('#TextSearch').keypress(function () {
    OpenListSearch(true);
});
$('#TextSearch').click(function () {
    var width = $('#search-main').width();
    $('#list-search').width(width - 2);
    OpenListSearch(true);
}); 

function OpenListSearch(isOpen) {
    if (isOpen != null && isOpen != undefined) {
        if (isOpen) {
            $('#list-search').removeClass('hidden')
            $('#list-search').slideDown();
        } else {
            $('#list-search').addClass('hidden')
            $('#list-search').slideUp();
        }
    } else {
        if ($('#list-search').hasClass('hidden')) {
            $('#list-search').removeClass('hidden')
            $('#list-search').slideDown();
        } else {
            $('#list-search').addClass('hidden')
            $('#list-search').slideUp();
        }
    }
}
function SetActiveListSearch(index) {
    $('.list-search-text').removeClass('active');
    $('.list-search-text').eq(index).addClass('active');

    $('.icon-active').hide();
    $('.icon-active').eq(index).show();
} 