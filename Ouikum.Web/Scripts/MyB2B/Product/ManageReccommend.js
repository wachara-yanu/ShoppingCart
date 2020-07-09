

var param_add_recomm = new Array();
var param_recomm = new Array();
var isOpenFormAdd = false;



$('#ModalAddRecommend').on('hidden', function () {
    $('#ModalAddRecommend').html('');
    isOpenFormAdd = false;
})

$('#import-item').live("click", function () {
    var len = $('.check-item').length;
    var index = 0;
    param_add_recomm = new Array();
    for (var i = 0; i < len; i++) {
        if (!$('.check-item').eq(i).hasClass('hide')) {
            var id = parseInt($('.add-item').eq(i).attr('data-id'), 10);
            param_add_recomm[index] = id;
            index++;
        } //end if

    } //end for
  //console.log(param_add_recomm);

    InsertItemRecommend();
});

function InsertItemRecommend() {
    data = {
        ProductID: param_add_recomm
    }
    OpenLoading(true);
    $.ajax({
        url: GetUrl("MyB2B/Product/AddRecommend"),
        data: data,
        traditional: true,
        success: function (data) {
            OpenLoading(false);
            $('#cancel-item').click();
            if (CheckError(data)) {
                SubmitPage();
            }
        },
        error: function () {
            OpenLoading(false);
        },
        type: "POST"
    });
    return false;
}

function SaveRecommend() {
    var len = $('.product-item').length;
    param_recomm = new Array();
    for (var i = 0; i < len; i++) {
        param_recomm[i] = $('.product-item').eq(i).attr('data-id');
    }

    var data = {
        ProductID: param_recomm
    }

    $.ajax({
        url: GetUrl("MyB2B/Product/SaveRecommend"),
        data: data,
        success: function (data) {
            SubmitPage();
        },
        error: function () {
            OpenLoading(false);
            bootbox.alert("Error");
        },
        type: "POST"
    });


}

function CloseFormAddRecommend() {
    $('#cancel-item').click();

}

$('#cancel-item').live('click',function () {
    isOpenFormAdd = false; 
});

function OpenFormAddRecommend() {
    isOpenFormAdd = true;
    $('#ModalAddRecommend').modal('show');

}

function setImg() {
    var len = $('.img-polaroid').length;
    for (var i = 0; i < len; i++) {
        resizeImg($('.img-polaroid').eq(i), 65, 65);

    }
}

//---------------------------------------------------------
function ShowFormRecommend() {

    OpenLoading(true);
    $.ajax({
        url: GetUrl("MyB2B/Product/FormAddRecommend"),
        data: data,
        success: function (data) {
            OpenLoading(false);
            $("#ModalAddRecommend").hide();
            $("#ModalAddRecommend").html("");

            $("#ModalAddRecommend").html(data);
            $("#ModalAddRecommend").modal("show");

        },
        error: function () {
            OpenLoading(false);
            bootbox.alert("Error");
        },
        type: "POST"
    });
}

/*-------------------- Submit Page ----------------------*/
function SubmitPageFormAdd(PageIndex, PageSize, el) {
    if (el != null || el != undefined) { 
        if (PageIndex == null || PageIndex == undefined) {
            PageIndex = parseInt(el.find('.hidPageIndex').val(), 10);
        }
        if (PageSize == null || PageSize == undefined) {
            PageSize = parseInt(el.find('.hidPageSize').val(), 10);
        }
        TextSearch = el.find('.TextSearch').val();
    } else {
        TextSearch = "";
        PageIndex = 1;
        PageSize = 0;
    }

    data = {
        TextSearch: TextSearch,
        hidPageIndex: PageIndex,
        hidPageSize: PageSize
    }

    //console.log(data);
    if (!isOpenFormAdd) {
        OpenFormAddRecommend();
    }

    OpenLoading(true);
    $.ajax({
        url: GetUrl("MyB2B/Product/FormAddRecommend"),
        data: data,
        success: function (data) {
            OpenLoading(false);
            $("#ModalAddRecommend").html(data);
            
          
        },
        error: function () {
            OpenLoading(false);
            bootbox.alert("Error");
        },
        type: "POST"
    });
    return false;
}


//--------------------------สลับ Manage/Setting------------------------------
$(".Bg_IconManage").click(function () {
    //เปิดการ แก้ไข
    SettingProduct(true);

});

$(".Bg_IconManage_Setting").click(function () {
    //ปิดการ แก้ไข และบันทึก 
    SaveChangeListNo();

});
function SaveChangeListNo() {
    var param = new Array();
    var len = $('.Bg_item_Sort').length;
    for (var i = 0; i < len; i++) {
        param[i] = parseInt($('.Bg_item_Sort').eq(i).attr('data-id'), 10);
    }
    var data = {
        ProductID: param
    }
    //console.log(data);

    OpenLoading(true);
    $.ajax({
        url: GetUrl("MyB2B/Product/SaveChangeListNo"),
        data: data,
        traditional: true,
        success: function (param) {
            OpenLoading(false);
            if (CheckError(param)) {
                SubmitPage();
                SettingProduct(false);
            }
        },
        error: function () {
            OpenLoading(false);
            bootbox.alert("Error");
        },
        type: "POST"
    });

}

function SettingProduct(isSetting) {
    if ($('.bg_product_hide').hasClass('hide')) {
        $(".Bg_Allitem").css("marginTop", 200);
    }
    else {
        $(".Bg_Allitem").css("marginTop", 200);
    }
    if (isSetting) {
        $('#recomm-item').hide();
        $('#sort-item').fadeIn();
        $('.Bg_IconManage').hide();
        $('.Show_Setting').fadeIn();
    } else {
        $('#recomm-item').fadeIn();
        $('#sort-item').hide();
        $('.Bg_IconManage').fadeIn();
        $('.Show_Setting').hide();
    }
}


function RecommendDetail(Obj) {
    var height = $(window).height() - 200;
    $("#bodyQuickDetails").css("marginTop", height);
    //    OpenLoading(true, null, $('.Bg_Allitem'));
    $.ajax({
        url: GetUrl("MyB2B/Product/RecommendDetail"),
        data: { ID: Obj.attr("data-id") },
        success: function (data) {
            $('#bodyQuickDetails').fadeIn();
            $("#bodyQuickDetails").html(data);
        },
        error: function () {
            bootbox.alert("Error");
        },
        type: "POST"
    });
}
function ddp() {
    var str = "444<br>";
    for (var i = 1; i < 200; i++) {
        $('#recommend_item').append(str);
    }
}
function SetPageFormAdd(n) {
    var el = CreatObj(n);
    el.find('.txtPageIndex').eq(0).val(el.find('.hidPageIndex').val());

    var html = el.find('.hidTotalRow').val() + " items found &nbsp;&nbsp;";
    el.find('.lblTotalRow').html(html);

    html = " &nbsp;&nbsp;" + el.find('.hidTotalPage').val() + " pages";
    el.find('.lblTotalPage').html(html);
}

function OnNext(obj) {
    var el = GetFormData(obj);
    PageIndex = parseInt(el.find('.hidPageIndex').val(), 10);
    TotalPage = parseInt(el.find('.hidTotalPage').val(), 10);
    if (PageIndex < TotalPage) {
        //console.log(PageIndex + 1);

    } else {
        el.find('.btn-next').attr('disabled', 'disabled');
    }
    SubmitPageFormAdd(PageIndex + 1, 0, el);
}

function OnPrev(obj) {
    var el = GetFormData(obj);
    PageIndex = parseInt(el.find('.hidPageIndex').val(), 10);
    if (PageIndex > 0) {
        //console.log(PageIndex - 1);
    } else {
        el.find('.btn-prev').attr('disabled', 'disabled');
    }
    SubmitPageFormAdd(PageIndex - 1, 0, el);
}

function OnSearch(obj) {
    var el = GetFormData(obj);
    SubmitPageFormAdd(1, 0, el);
    return false;
}


function GotoPageFormAdd(obj) {

    var s = obj.attr('form-data');
    var el = CreatObj(obj);
    var PageIndex = parseInt(el.find('.txtPageIndex').eq(0).val(), 10);
    var TotalPage = parseInt(el.find('.hidTotalPage').val(), 10);
    CheckPageFormAdd(PageIndex, s)
    //console.log(PageIndex); 

    if (CheckPageFormAdd(PageIndex, s)) {
        SubmitPageFormAdd(PageIndex, 0, el);
    } else {
        SubmitPageFormAdd(TotalPage, 0, el); 
        return false;
    }
    return false;
}

function CheckPageFormAdd(Page, n) {
    var el = CreatObj(n);
    //console.log(el);
    var isPass = true;
    var PageIndex = parseInt(el.find('.hidPageIndex').val(), 10);
    var TotalPage = parseInt(el.find('.hidTotalPage').val(), 10);
    //console.log(PageIndex + ', ' + TotalPage);
    if (Page == 1) {
        if (TotalPage == 1) {
            //console.log('xxx');
            el.find('.btn-next').attr('disabled', 'disabled');
            el.find('.btn-prev').attr('disabled', 'disabled');
        } else {
            el.find('.btn-next').removeAttr('disabled');
            el.find('.btn-prev').attr('disabled', 'disabled');
        }
         
    } else if (Page == TotalPage) {
        el.find('.btn-prev').removeAttr('disabled');
        el.find('.btn-next').attr('disabled', 'disabled');
    } else if (Page > 1 && Page < TotalPage) {
        el.find('.btn-prev').removeAttr('disabled');
        el.find('.btn-next').removeAttr('disabled');
    } else {
        el.find('.txtPageIndex').eq(0).val(PageIndex);
        isPass = false;
    }

    return isPass;
}
////---------------------------Popup Modal-----------------------------------
//$('#ModalEditRec').on('hide', function () {
//    $('#ModalEditRec').html('');

//});
//function PrepareEditRecByID(Obj) {
//    $("#ModalEditRec").html('');
//    OpenLoading(true);
//    $('.Bg_Allitem').css("position", "fixed");

//    var ProductCode = GenerateCode(Obj);
//    $.ajax({
//        url: GetUrl("MyB2B/Product/PrepareEditRecByID"),
//        data: { ID: Obj.attr("id"),
//            GenCode: ProductCode
//        },
//        success: function (data) {
//            $("#ModalEditRec").html(data);
//            OpenLoading(false);
//            $('#ModalEditRec').on('show', function () {
//                $('#bodyQuickDetails').fadeOut();
//            });

//        },
//        error: function () {
//            OpenLoading(false);
//        },
//        type: "POST"
//    });
//}
//function GenerateCode(Obj) {

//    var LogonID = Obj.attr("com-id");
//    var CompIDCode = "";
//    var Randcode = randomCode();
//    if (LogonID < 10) {
//        CompIDCode += "00000" + LogonID;
//    }
//    else if (LogonID < 100) {
//        CompIDCode += "0000" + LogonID;
//    }
//    else if (LogonID < 1000) {
//        CompIDCode += "000" + LogonID;
//    }
//    else if (LogonID < 10000) {
//        CompIDCode += "00" + LogonID;
//    }
//    else if (LogonID < 100000) {
//        CompIDCode += "0" + LogonID;
//    }
//    else if (LogonID < 1000000) {
//        CompIDCode += LogonID;
//    }

//    return "PD" + CompIDCode + Randcode;

//}
