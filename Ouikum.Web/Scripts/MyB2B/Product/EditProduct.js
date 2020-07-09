var focusshow = false;
var focushide = false;
var IsShowTextEditorProductDetail = false;

// EditProduct

$(function () {
    $("#GetProductImgPath").sortable({
        cursor: "move",
        helper: function (event, ui) {
            var img = ui.find(".ProImgpath").attr("src");
            var val = "<img class='thumbnail' style='background:#ffffff;' src='" + img + "' style='width:90px; height:90px;'  />";
            $('#ProductImgPath_0').attr("src", $('#GetProductImgPath').find(".ProImgpath").first().attr("src"))
            return $(val);

        },
        update: function (event, ui) {
            SetFirstImage();

        }
    }).disableSelection();
});

//function selectedQtyUnit() {
//    var unit = $('#Upd_QtyUnit option:selected').text();
//    $("#Upd_QtyUnitFirst").text(unit); $("#Upd_QtyUnitSecond").text(unit);

//    if ($('#Upd_QtyUnit option:selected').val() == label.vldother) {
//        $('#Upd_QtyUnit').css('display', 'none');
//        $('.QtyOther').slideDown();
//    }
//}

//$('.QtyOther').live('blur', function () {
//    var unit = $('#QtyOther').val();
//    if (unit != "") {
//        $('#QtyUnit').append("<option value='" + unit + "' selected='selected'>" + unit + "</option>");
//        $("#QtyUnitFirst").text(unit);
//    }
//    $('.QtyOther').hide();
//    $('#QtyUnit').css('display', 'inline');
//});


$('#ModalEditProduct').on('hidden', function () {
    //$('.Bg_Allitem').css("position", "static");
    $("html").css('overflow', 'auto');
})


$(".upd-detail-click").click(function () {
    //console.log(IsShowTextEditorProductDetail)
    //if (!IsShowTextEditorProductDetail) {
        //$(this).text(label.vldhide_moredetail);
        //IsShowTextEditorProductDetail = true;
        var text = tinyMCE.get('UpDFullDetail').getContent();
        tinyMCE.get('UpDFullDetail-modal').setContent(text);
        $('#ModalUpDFullDetail').removeClass('hide');
    //} else {
        //$(this).text(label.vldadd_moredetail);
        //IsShowTextEditorProductDetail = false;
        //$('#ModalUpDFullDetail').addClass('hide');
        //$('.upd-detail-textarea').fadeOut();
    //}
});

$('#Upd_Price , #Upd_Qty, #Upd_Price_One').click(function () {
    $(this).select();
});


$(".mceFirst").live("click", function () {
    focushide = true;
});

function beforecateLV1() {
    selectedcateLV1();
    $('.morekey1').removeAttr('disabled');
}
function beforecateLV2() {
    selectedcateLV2();
    $('.morekey2').removeAttr('disabled');
}
//========================= Category ====================//
function selectedcateLV3() {
    catename = $("#SelectCateLV3 option:selected").attr("class");
    catecode = $("#SelectCateLV3 option:selected").attr("catecode");
    catepath = $("#SelectCateLV3 option:selected").attr("catepath");
    catelv3 = $("#SelectCateLV3 option:selected").val();
    //console.log('catename : ' + catename + ' | catecode : ' + catecode + ' | catepath : ' + catepath + ' | catelv3 : '+catelv3);
    $('#Upd_Category').css('display', 'block');
    $('.Show-selectCate').addClass('hide');
    $('.click-cateall').css('display', 'block');
    $("#Upd_CategoryCode").val(catecode);
    $("#Upd_CategoryCode").attr('data-catename', catename);
    //$("#Upd_CategoryCode").val(catecode);
    //$("#Upd_CategoryCode").attr('data-id', catelv3);
    //$("#Upd_CategoryCode").attr('data-catename', catename);
    $('#EditProduct_Form').find('#TextSearchCategoryEdit').val(catename);
    $(".show-catepath").val(catename);
    $(".show-catepath").text(catepath);
    $(".show-catepath").fadeIn();
    $("#TextSearchCategory").attr('placeholder', '');
    CheckCateCode();
    $('.morekey3').removeAttr('disabled');
}
function CheckCateCode() {
    var str = $('#Upd_CategoryCode').val();
    if (str != undefined) {
        if (str.length > 1) {
            $("#AllCate").closest('.control-group').removeClass('error');
            $("#AllCate").closest('.control-group').addClass('success');
            $("#AllCate .errCate").hide()
            //$("#AllCate .successCate").show();

        } else {
            $("#AllCate").closest('.control-group').removeClass('success');
            $("#AllCate").closest('.control-group').addClass('error');
            $("#AllCate .errCate").show()
            //$("#AllCate .successCate").hide();
        }
    }
}


//-----------Confirm Save and Cancel------------------------------
function EditProduct() {
    var PromotionPrice = 0;
    var catecode = "";
    var cateLV3 = "";
    var QtyUnit = "";
    if ($('#SelectUpd_Promotion').val() == 0) {
        PromotionPrice = 0;
    }
    else {
        PromotionPrice = $("#Upd_Promotionprice").val();
    }
    if ($(".div-sel-category select option:selected").val() == undefined) {
        catecode = $("#SelectCateLV3 option:selected").attr("catecode");
        cateLV3 = $("#SelectCateLV3 option:selected").val();
    }
    else {
        catecode = $(".div-sel-category select option:selected").attr("catecode");
        cateLV3 = $(".div-sel-category select option:selected").val();
    }
    if ($('#Upd_QtyUnit option:selected').val() == label.vldother) {
        QtyUnit = $('#QtyOther').val();
    } else {
        QtyUnit = $('#Upd_QtyUnit option:selected').val()
    }
    data = {
        ProductID: $("#Upd_ProductID").val(),     //
        Rowflag: $("#hidRowflag").val(),           //
        ProductName: $("#Upd_ProductName").val(),       //
        ProductGroup: $('#ProductGroup option:selected').val(),       //
        ProductCode: $("#Upd_ProductCode").val(),       //
        Price: $("#Upd_Price").val(),        //
        Price_One: $("#Upd_Price_One").val(),
        Keyword: EditKeyword,           //
        QuickDetail: EditDetail,     //
        FullDetail: tinyMCE.get('UpDFullDetail').getContent(),   //
        Qty: $("#Upd_Qty").val(),                           //
        QtyUnit: QtyUnit,    //
        //Catecode: $("#Upd_CategoryCode").val(),                               //
        //CateLV3: parseInt($("#Upd_CategoryCode").attr("data-id"), 10),    //
        Catecode: catecode,
        CateLV3: cateLV3,
        ProductImgPath: ProductImgPath,                          //
        ProductImgID: ProductImgID,           //
        Ispromotion: 0,
        PromotionPrice: 0
    }
    //console.log(data);
    OpenLoading(true);
    $.ajax({
        url: GetUrl("MyB2B/Product/EditProductByID"),
        data: data,
        traditional: true,
        success: function (data) {
            OpenLoading(false);

            $('#ModalEditProduct').modal('hide');
            $("html").css('overflow', 'auto');
            $('#EditProduct_Form').remove();
            if (CheckError(data)) {
                SubmitPage(1);
            }
        },
        error: function () {
            OpenLoading(false);
        },
        type: "POST"
    });
}
var isFormChange = false;
function CheckFormChange() {

}
$('#ModalEditProduct').on('hidden.bs.modal', function (e) {
    // do something...
    console.log('edit modal hide');
    $("html").css('overflow', 'auto');
    $('#EditProduct_Form').remove();
})
//-----------GetValueTagit--------------------------------------
var EditKeyword = "";
var EditDetail = "";


function CheckShortDesc() {
    GetEditTagit();
    var list = EditDetail.split('~');
    if (EditDetail.length > 0) {
        //  console.log("ProductDetail.length > 0");
        if (EditDetail.length >= 0 && EditDetail.length <= 500) {
            if (list.length > 5) {
                //   console.log("list.length > 5");
                $("#tagit-editdetail").closest('.control-group').removeClass('success');
                $("#tagit-editdetail").closest('.control-group').addClass('error');
                $("#tagit-editdetail").css('border', 'solid 1px #B94A48');
                $(".Detail .textdesc").text('ระบบจะแสดง "คำอธิบาย" เพียง 5 รายการแรกเท่านั้น');
                return false;
            } else {
                //console.log("true");
                $("#tagit-editdetail").closest('.control-group').addClass('success');
                $("#tagit-editdetail").closest('.control-group').removeClass('error');
                $(".Detail > .error").text('');
                //$(".Detail .textdesc").text('ระบบจะแสดง "คำอธิบาย" เพียง 5 รายการแรกเท่านั้น');
                $("#tagit-editdetail").css('border', 'solid 1px #468847');
                return true;
            }
        } else {
            $("#tagit-editdetail").closest('.control-group').removeClass('success');
            $("#tagit-editdetail").closest('.control-group').addClass('error');
            //$(".Detail > .error").text(label.vldrequired);
            $(".Detail .textdesc").text('"คำอธิบาย" ควรกรอกไม่เกิน 100 ตัวอักษร ต่อ 1 รายการ');
            $("#tagit-editdetail").css('border', 'solid 1px #B94A48');
            return false;
        }
    } else {
        //console.log("else");
        $("#tagit-editdetail").closest('.control-group').removeClass('success');
        $("#tagit-editdetail").closest('.control-group').addClass('error');
        //$(".Detail > .error").text(label.vldrequired);
        $(".Detail .textdesc").text('กรุณากรอกข้อมูล');
        $("#tagit-editdetail").css('border', 'solid 1px #B94A48');
        return false;
    }
}

function CheckProductKeyword() {
    GetEditTagit();
    var list = EditKeyword.split('~');
    //console.log("list ==" + list);
    if (list == label.vldnon_specific_keyword) {
        $("#tagit-editkeyword").closest('.control-group').removeClass('success');
        $("#tagit-editkeyword").closest('.control-group').addClass('error');
        $("#tagit-editkeyword").css('border', 'solid 1px #B94A48');
        $(".Keyword > .error").text(label.vldrequired);
        return false;
    }
    //console.log("list.length  " + list.length);
    //console.log(list.length - 1);
    //console.log("EditKeyword.length  " + EditKeyword.length);
    if (EditKeyword.length > 0) {
        if (EditKeyword.length >= 0 && EditKeyword.length <= 250) {
            if (list.length > 5) {
                //console.log("list.length > 6");
                $("#tagit-editkeyword").closest('.control-group').removeClass('success');
                $("#tagit-editkeyword").closest('.control-group').addClass('error');
                $("#tagit-editkeyword").css('border', 'solid 1px #B94A48');
                $(".Keyword > .error").text('ระบบจะแสดง "เพิ่มคีย์เวิร์ด" เพียง 5 รายการแรกเท่านั้น');
                return false;
            } else {
                //console.log("true");
                $("#tagit-editkeyword").closest('.control-group').addClass('success');
                $("#tagit-editkeyword").closest('.control-group').removeClass('error');
                $("#tagit-editkeyword").css('border', 'solid 1px #468847');
                $(".Keyword > .error").text('');
                return true;
            }
        } else {
            //console.log("list.length - 1 " + list.length - 1);
            //console.log("else2");
            $("#tagit-editkeyword").closest('.control-group').removeClass('success');
            $("#tagit-editkeyword").closest('.control-group').addClass('error');
            $("#tagit-editkeyword").css('border', 'solid 1px #B94A48');
            $(".Keyword > .error").text('"คีย์เวิร์ด" ควรกรอกไม่เกิน 50 ตัวอักษร ต่อ 1 รายการ');
            return false;
        }
    } else {
        //console.log("list.length - 1 " + list.length - 1);
        //console.log("else2");
        $("#tagit-editkeyword").closest('.control-group').removeClass('success');
        $("#tagit-editkeyword").closest('.control-group').addClass('error');
        $("#tagit-editkeyword").css('border', 'solid 1px #B94A48');
        $(".Keyword > .error").text('กรุณากรอกข้อมูล');
        return false;
    }
    //var list = EditKeyword.split('~');
    ////console.log('keyword');
    ////console.log(list);
    //if (EditKeyword.length > 0) {

    //    //if (list.length > 6) {
    //    //    $("#tagit-editkeyword").closest('.control-group').removeClass('success');
    //    //    $("#tagit-editkeyword").closest('.control-group').addClass('error');
    //    //    $(".Upd_Keyword > .error").text(label.vldmax_word5);
    //    //    //    $(".Upd_Keyword > .success").css('display', 'none');
    //    //    $("#tagit-editkeyword").css('border', 'solid 1px #B94A48');
    //    //    return false;
    //    //} else {
    //    $("#tagit-editkeyword").closest('.control-group').addClass('success');
    //    $("#tagit-editkeyword").closest('.control-group').removeClass('error');
    //    $(".Upd_Keyword > .error").text('');
    //    //    $(".Upd_Keyword > .success").css('display', 'block');
    //    $("#tagit-editkeyword").css('border', 'solid 1px #468847');
    //    return true;
    //    //}

    //} else {
    //    $("#tagit-editkeyword").closest('.control-group').removeClass('success');
    //    $("#tagit-editkeyword").closest('.control-group').addClass('error');
    //    $(".Upd_Keyword > .error").text(label.vldrequired);
    //    // $(".Upd_Keyword > .success").css('display', 'none');
    //    $("#tagit-editkeyword").css('border', 'solid 1px #B94A48');
    //    return false;
    //}
}


function GetEditTagit() {
    EditKeyword = "";
    EditDetail = "";
    $('#tagit-editkeyword').find(".tagit-choice .tagit-label").each(function (index) {
        var kw = index - 1;
        EditKeyword += $(this).text();
        EditKeyword += "~";
    });
    EditKeyword = EditKeyword.substring(0, EditKeyword.length - 1)

    $('#tagit-editdetail').find(".tagit-choice .tagit-label").each(function (index) {
        var dt = index - 1;
        EditDetail += $(this).text();
        EditDetail += "~";
    });
    EditDetail = EditDetail.substring(0, EditDetail.length - 1)
}

var EditCatecode = "";

var ProductImgPath = new Array();
var ProductImgID = new Array();

//============ Get Product Img ===================//
function GetImgPath() {


    var len = $('.img-data').length;
    ProductImgPath = new Array();
    ProductImgID = new Array();
    for (var i = 0; i < len; i++) {
        ProductImgPath[i] = $('.img-data').eq(i).attr('img-name');
        ProductImgID[i] = parseInt($('.img-data').eq(i).attr('img-data-id'), 0);
    }
    //console.log(ProductImgPath);
    //console.log(ProductImgID);

}

function CheckImgPath() {
    GetImgPath();
    //console.log('img : '+ProductImgPath.length);

    if (ProductImgPath.length > 0) {
        $("#ProductImgPath").addClass('success');
        $("#ProductImgPath").removeClass('error');
        $(".ProductImgPath .error").css('display', 'none');
        //   $(".ProductImgPath .success").css('display', 'block');
        return true;
    }
    else {
        $("#ProductImgPath").removeClass('success');
        $("#ProductImgPath").addClass('error');
        $(".ProductImgPath > .error").text(label.vldrequired);
        $(".ProductImgPath .error").css('display', 'block');
        // $(".ProductImgPath .success").css('display', 'none');
        return false;
    }
}

function CheckProductCode() {
    if (($('#Upd_ProductCode').val().length < 3)) {
        $("#Upd_ProductCode").closest('.control-group').removeClass('success');
        $("#Upd_ProductCode").closest('.control-group').addClass('error');
        //  $(".Upd_ProductCode > .success").css('display', 'none');

        if ($('#Upd_ProductCode').val() == "") {
            $("#Upd_ProductCode").attr('placeholder', label.vldless_2char);
        }
        else if ($('#Upd_ProductCode').val().length < 3) {
            $("#Upd_ProductCode").attr('placeholder', label.vldless_2char);
        }
        return false;
    }
    else {
        $("#Upd_ProductCode").closest('.control-group').addClass('success');
        $("#Upd_ProductCode").closest('.control-group').removeClass('error');
        $("#Upd_ProductCode").attr('placeholder', '');
        //  $(".Upd_ProductCode > .success").css('display', 'block');
        return true;
    }

}


function CheckFullDetail() {
    //var detail = tinyMCE.activeEditor.getContent();
    //var bool = true;
    //if (detail != "" && detail != null) {
    //    if (detail.length > 10000) {
    //        $("#Upd_FullDetail").closest('.control-group').removeClass('success');
    //        $("#Upd_FullDetail").closest('.control-group').addClass('error');
    //        $("#Upd_FullDetail .errorTxt").text(label.vldformaterror);
    //        bool = false;
    //    }
    //    else {
    //        $("#Upd_FullDetail").closest('.control-group').addClass('success');
    //        $("#Upd_FullDetail").closest('.control-group').removeClass('error');
    //        $("#Upd_FullDetail .errorTxt").text('');
    //        bool = true;
    //    }
    //} else {
    //    bool = true;
    //}

    //return bool;

    var detail = tinyMCE.get('UpDFullDetail').getContent();
    //console.log("detail  " + detail);
    if (detail == "") {
        //console.log("detail  NULL");
        $("#Upd_FullDetail").closest('.control-group').removeClass('success');
        $("#Upd_FullDetail").closest('.control-group').addClass('error');
        $(".FullDetail .error").text(label.vldrequired + 'รายละเอียดเพิ่มเติม');
        return false;
        $("#Upd_FullDetail .successImg").css('display', 'none');
    }
    else if (detail.length > 10000) {
        //console.log("detail.length  " + detail.length);
        $("#Upd_FullDetail").closest('.control-group').removeClass('success');
        $("#Upd_FullDetail").closest('.control-group').addClass('error');
        $(".FullDetail .error").text(label.vldformaterror + 'รายละเอียดเพิ่มเติม');
        return false;
    }
    else {
        //console.log("true  ");
        $("#Upd_FullDetail").closest('.control-group').addClass('success');
        $("#Upd_FullDetail").closest('.control-group').removeClass('error');
        $("#FullDetail .error").text('');
        return true;
        $("#Upd_FullDetail .successImg").css('display', 'block');
    }

}


//============ CheckEditProduct===================//


//function CheckEditProduct() {
//    var bool = true;
//    if ($(".div-sel-category select option:selected").attr("catecode") != undefined) {
//        EditCatecode = $(".div-sel-category select option:selected").attr("catecode");
//    }
//    else if ($("#SelectCateLV3 option:selected").attr("catecode") != undefined) {
//        EditCatecode = $("#SelectCateLV3 option:selected").attr("catecode");
//    }
//    else {
//        EditCatecode = $(".show-catepath-list").attr("cate-code");
//    }
//    if (!CheckProductName()) {
//        bool = false;
//    }

//    //console.log('name : ' + bool);

//    if (!CheckImgPath()) {
//        bool = false;
//    }
//    //console.log('img : ' + bool);

//    if (!CheckProductCode()) {
//        bool = false;
//    }
//    //console.log('code : ' + bool);


//    if (!CheckProductKeyword()) {
//        bool = false;
//    }
//    //console.log('keyword : ' + bool);

//    if (!CheckShortDesc()) {
//        bool = false;
//    }
//    //console.log('short-desc : ' + bool);

//    if (!CheckFullDetail()) {
//        bool = false;
//    }
//    //console.log('ProductDetail : ' + bool);


//    if (tinyMCE.activeEditor.getContent() == "") {
//        $("#Upd_FullDetail").closest('.control-group').removeClass('success');
//        $("#Upd_FullDetail").closest('.control-group').addClass('error');
//        $("#Upd_FullDetail .errorTxt").text(label.vldrequired);
//        //$("#Upd_FullDetail .successImg").css('display', 'none');
//        bool = false;
//    }
//    else {
//        $("#Upd_FullDetail").closest('.control-group').addClass('success');
//        $("#Upd_FullDetail").closest('.control-group').removeClass('error');
//        $("#Upd_FullDetail .errorTxt").text('');
//        //$("#Upd_FullDetail .successImg").css('display', 'block');
//    }
//    if ($('#Upd_QtyUnit option:selected').val() == 0) {
//        $("#Upd_Qty").closest('.control-group').removeClass('success');
//        $("#Upd_Qty").closest('.control-group').addClass('error');
//        bool = false;
//    }
//    else {
//        $("#Upd_Qty").closest('.control-group').addClass('success');
//        $("#Upd_Qty").closest('.control-group').removeClass('error');
//    }

//    //console.log($('#Upd_CategoryCode').val());
//    if ($('#Upd_CategoryCode').val() == undefined || $('#Upd_CategoryCode').val() == '') {
//        $("#Upd_Category").closest('.control-group').removeClass('success');
//        $("#Upd_Category").closest('.control-group').addClass('error');
//        $("#TextSearchCategoryEdit").attr('placeholder', label.vldrequired);
//        bool = false;
//    }
//    else {
//        $("#Upd_Category").closest('.control-group').addClass('success');
//        $("#Upd_Category").closest('.control-group').removeClass('error');
//        $(".div-sel-category").css('display', 'none');
//    }

//    $("#Upd_ProductGroup").closest('.control-group').addClass('success');
//    $("#Upd_ProductGroup").closest('.control-group').removeClass('error');
//    $(".Upd_ProductGroup").css('color', '#468847');
//    //  $(".Upd_ProductGroup .success").css('display', 'block');
//    if (!bool) {
//        bootbox.alert(label.vldall_required);
//        return false;
//    }
//    else {
//        EditProduct();
//    }
//}

$("#TextSearchCategory").keypress(function (event) {
    if (event.which == 13) {
        SearchCategory();
    }
});

function checkUpd_Price() {
    var price = $("#Upd_Price").val();

    if (price >= 0) {
        $("#Upd_Price").closest('.control-group').addClass('success');
        $("#Upd_Price").closest('.control-group').removeClass('error');
        //    $(".Upd_Price > .success").css('display', 'block');
        return true;
    } else {
        $("#Upd_Price").closest('.control-group').removeClass('success');
        $("#Upd_Price").closest('.control-group').addClass('error');
        // $(".Upd_Price > .success").css('display', 'none');
        return false;
    }
}


function checkUpd_Price_One() {
    var price = $("#Upd_Price_One").val();

    if (price >= 0) {
        $("#Upd_Price_One").closest('.control-group').addClass('success');
        $("#Upd_Price_One").closest('.control-group').removeClass('error');
        //    $(".Upd_Price > .success").css('display', 'block');
        return true;
    } else {
        $("#Upd_Price_One").closest('.control-group').removeClass('success');
        $("#Upd_Price_One").closest('.control-group').addClass('error');
        // $(".Upd_Price > .success").css('display', 'none');
        return false;
    }
}

function CheckProductName() {
    var name = $.trim($('#Upd_ProductName').val());
    if (checkDisclaimer($('#Upd_ProductName').val())) {
        if (name.length > 2 && name.length <= 100) {
            $("#Upd_ProductName").closest('.control-group').addClass('success');
            $("#Upd_ProductName").closest('.control-group').removeClass('error');
            //$(".BuyleadName > .success").show();
            $(".Upd_ProductName > .error").text('');
            return true;
        }
        else {
            $("#Upd_ProductName").closest('.control-group').removeClass('success');
            $("#Upd_ProductName").closest('.control-group').addClass('error');
            // $(".BuyleadName > .success").css('display', 'none');
            if (name.length > 0 && name.length < 4) {
                $(".Upd_ProductName > .error").text("กรุณากรอกอย่างน้อย 3 ตัวอักษร และไม่เกิน 100 ตัวอักษร");
            }else if (name.length > 100) {
                $(".Upd_ProductName > .error").text("กรุณากรอกอย่างน้อย 3 ตัวอักษร และไม่เกิน 100 ตัวอักษร");
            } else {
                $(".Upd_ProductName > .error").text(label.vldrequired);
            }
            return false;
        }
    } else {
        $("#Upd_ProductName").closest('.control-group').removeClass('success');
        $("#Upd_ProductName").closest('.control-group').addClass('error');
        $(".Upd_ProductName > .error").text("กรุณากรอกข้อมูลให้ถูกต้อง ไม่ควรมีเครื่องหมาย \!@#$^฿|{}[]'\"<>=:;~");
        $(".Upd_ProductName > .error").css('text-align', 'left');
        checkError();
        result = false;
    }

    //var name = $.trim($('#Upd_ProductName').val());
    //if (name.length > 2) {
    //    $("#Upd_ProductName").closest('.control-group').addClass('success');
    //    $("#Upd_ProductName").closest('.control-group').removeClass('error');
    //    //  $(".Upd_ProductName > .success").show();
    //    $("#Upd_ProductName").attr('placeholder', '');
    //    $("#tagit-editkeyword .tagit-choice").first().remove();
    //    var html = "<li class='tagit-choice'>" + name + "<a class='tagit-close icon-remove'></a></li>";
    //    $('#tagit-editkeyword').prepend(html);
    //    return true;
    //}
    //else {
    //    $("#Upd_ProductName").closest('.control-group').removeClass('success');
    //    $("#Upd_ProductName").closest('.control-group').addClass('error');
    //    //  $(".Upd_ProductName > .success").css('display', 'none');
    //    $("#Upd_ProductName").attr('placeholder', label.vldless_3char);
    //    return false;
    //}

}

function CheckExistProductName() {
    // don't use
    data = {
        Upd_ProductName: $('#Upd_ProductName').val(),
        Chk_ProductName: $('#Upd_ProductName').attr('chkname'),
        ProductID: parseInt($('#Upd_ProductID').val(), 10)
    }
    //console.log(data);
    $.ajax({
        url: GetUrl("MyB2B/Product/ValidateEditProduct"),
        data: data,
        type: "POST",
        success: function (data) {
            if (!data) {
                $("#Upd_ProductName").closest('.control-group').removeClass('success');
                $("#Upd_ProductName").closest('.control-group').addClass('error');
                //  $(".Upd_ProductName > .success").css('display', 'none');
                $(".Upd_ProductName > .error").text(label.vldpname_already);
            }
            else {
                $("#Upd_ProductName").closest('.control-group').addClass('success');
                $("#Upd_ProductName").closest('.control-group').removeClass('error');
                //  $(".Upd_ProductName > .success").show();
                $(".Upd_ProductName > .error").text('');
            }
        },
        error: function () {
            bootbox.alert(label.vldcannot_check_info);
        }
    });
}


function CheckExistProductCode() {
    // don't use
    data = {
        Upd_ProductCode: $('#Upd_ProductCode').val(),
        Chk_ProductCode: $('#Upd_ProductCode').attr('chkcode'),
        ProductID: parseInt($('#Upd_ProductID').val(), 10)
    }
    //console.log(data); 
    $.ajax({
        url: GetUrl("MyB2B/Product/ValidateEditProduct"),
        data: data
         ,
        type: "POST",
        success: function (data) {
            //console.log('exist : '+data);
            if (!data) {
                $("#Upd_ProductCode").closest('.control-group').removeClass('success');
                $("#Upd_ProductCode").closest('.control-group').addClass('error');
                // $(".Upd_ProductCode > .success").css('display', 'none');
                $(".Upd_ProductCode > .error").text(label.vldpname_already);
                return false;
            } else {
                $("#Upd_ProductCode").closest('.control-group').addClass('success');
                $("#Upd_ProductCode").closest('.control-group').removeClass('error');
                $(".Upd_ProductCode > .error").text('');
                //$(".Upd_ProductCode > .success").css('display', 'block');
                return true;
            }
        },
        error: function () {
            bootbox.alert(label.vldcannot_check_info);
        }
    });
}


$("#TextSearchCategoryEdit").keypress(function (event) {
    if (event.which == 13) {
        SearchCategory();
    }
});
function SearchCategory() {
    //console.log('SearchCatgory');
    $(".div-sel-category").fadeIn();
    var txtsrc = $('.txtSearchCate').val();
    if (txtsrc.length > 0) {
        SearchCategoryByName(txtsrc);
        $("#TextSearchCategoryEdit").attr('placeholder', '');

    } else {
        $("#AllCate .successCate").hide();
        $("#Upd_Category").closest('.control-group').removeClass('success');
        $("#Upd_Category").closest('.control-group').addClass('error');
        $("#TextSearchCategoryEdit").attr('placeholder', label.vldrequired);
        $(".div-sel-category").fadeOut();
    }
    return false;
}

function SearchCategoryByName(txtsearch) {
    data = {
        CategoryName: txtsearch
    }
    $('.Bg_searchcate').hide();
    $('.show-catepath').text('Loading..');
    $('.show-catepath').fadeIn();
    $.ajax({
        url: GetUrl("MyB2B/Product/SearchCategory"),
        data: data,
        success: function (data) {

            $('.show-catepath').hide();
            $(".div-sel-category").html(data);
        },
        error: function () {
            OpenLoading(false);
            bootbox.alert("Error");
        },
        type: "POST"
    });
}
$("#Upd_Price").live('blur', function () {

    if ($('#Upd_Price').val() >= 0) {
    } else {
        $(this).val("0.00");
    }
    $(this).val(parseFloat(eval($(this).val())).toFixed(2));
    $('.Upd_Price .error').text('');
    $("#Upd_Price").closest('.control-group').addClass('success');
    $("#Upd_Price").closest('.control-group').removeClass('error');
    //$(".Upd_Price > .success").css('display', 'block');
});

$("#Upd_Price_One").live('blur', function () {

    if ($('#Upd_Price_One').val() >= 0) {
    } else {
        $(this).val("0.00");
    }
    $(this).val(parseFloat(eval($(this).val())).toFixed(2));
    $('.Upd_Price_One .error').text('');
    $("#Upd_Price_One").closest('.control-group').addClass('success');
    $("#Upd_Price_One").closest('.control-group').removeClass('error');
    //$(".Upd_Price > .success").css('display', 'block');
});

$("#tagit-editkeyword").live('blur', function () {
    CheckProductKeyword();
});

$("#tagit-editdetail").live('blur', function () {
    CheckShortDesc();
});
$("#Upd_AllQty").live('blur', function () {
    $(this).val(parseFloat(eval($(this).val())).toFixed(2));
    if ($('#Upd_QtyUnit option:selected').val() == 0) {
        $("#Upd_QtyUnit").closest('.control-group').removeClass('success');
        $("#Upd_QtyUnit").closest('.control-group').addClass('error');
        //  $("#AllQty .errorQty").css('display', 'block');
        ///  $("#AllQty .successQty").css('display', 'none');
        $(".Upd_QtyUnit > .error").text('กรุณากรอกหน่วยสินค้า');
    }
    else {
        $("#Upd_QtyUnit").closest('.control-group').addClass('success');
        $("#Upd_QtyUnit").closest('.control-group').removeClass('error');
        $(".Upd_QtyUnit > .error").text('');
        //   $("#AllQty .errorQty").css('display', 'none');
        //    $("#AllQty .successQty").css('display', 'block');
    }
});





$(function () {
    //    $("em").remove();
    $("em").remove();
    $(".t-upload-button > span").remove();

    //--------------------พิมพ์ได้เฉพาะตัวเลขเท่านั้น--------------------------------------------------------------------
    $('#Upd_Price').bind('keypress', function (e) {
        return (e.which != 8 && e.which != 46 && e.which != 0 && (e.which < 48 || e.which > 57)) ? false : true;
    });

    $('#Upd_Price_One').bind('keypress', function (e) {
        return (e.which != 8 && e.which != 46 && e.which != 0 && (e.which < 48 || e.which > 57)) ? false : true;
    });

    $('#Upd_Qty').bind('keypress', function (e) {
        return (e.which != 8 && e.which != 46 && e.which != 0 && (e.which < 48 || e.which > 57)) ? false : true;
    });
    /*-----------------------text numunical--------------------------------*/
    $(".icon_money_up").live('click', function () {
        var input = $(this).parent().parent().find("input[type=text]:eq(0)");
        console.log("input    " + input);
        console.log("input.val()    " + input.val());
        if (input.val() == "") {
            console.log("input.val(1)    " + input.val(1));
            input.val(1)
        } else {
            console.log("input.val(parseFloat(eval(input.val()) + 1).toFixed(2))    " + input.val(parseFloat(eval(input.val()) + 1).toFixed(2)));
            input.val(parseFloat(eval(input.val()) + 1).toFixed(2))
        }
    });
    $(".icon_money_down").live('click', function () {
        var input = $(this).parent().parent().find("input[type=text]:eq(0)");
        if (input.val() == "") {

        } else {
            if (parseFloat(input.val()) > 0) {
                input.val(parseFloat(eval(input.val()) - 1).toFixed(2))
            } else {
                return false;

            }
        }
    });
    //#region---------------------------Text editor---------------------------------------------
    if ($(window).width() < 1025) {
        var buttons1 = "newdocument,|,justifyleft,justifycenter,justifyright,fontselect,fontsizeselect,formatselect";
        var buttons2 = "cut,copy,paste,|,bold,italic,underline,|,forecolor,backcolor,|,undo,redo,|,anchor,image,hr,removeformat";
    }
    else if ($(window).width() < 1281) {
        var buttons1 = "newdocument,|,justifyleft,justifycenter,justifyright,fontselect,fontsizeselect,formatselect,code,preview,|,bullist,numlist,charmap";
        var buttons2 = "cut,copy,paste,|,bold,italic,underline,|,forecolor,backcolor,|,undo,redo,|,anchor,image,hr,removeformat,backcolorinsertdate,insertdate,inserttime,|,sub,sup,emotions";
    }
    else {
        var buttons1 = "newdocument,|,justifyleft,justifycenter,justifyright,fontselect,fontsizeselect,formatselect,code,preview,|,bullist,numlist,|,outdent,indent";
        var buttons2 = "cut,copy,paste,|,bold,italic,underline,|,forecolor,backcolor,|,undo,redo,|,anchor,image,hr,removeformat,backcolorinsertdate,insertdate,inserttime,|,sub,sup,|,charmap,emotions";
    }

    tinyMCE.init({
        // General options
        mode: "textareas",
        theme: "advanced",
        elements: "FullDetail", 
        height: "250",
        width: "100%",  
        plugins: "autolink,lists,spellchecker,pagebreak,style,layer,table,save,advhr,advimage,advlink,emotions,iespell,inlinepopups,insertdatetime,preview,media,searchreplace,print,contextmenu,paste,directionality,fullscreen,noneditable,visualchars,nonbreaking,xhtmlxtras,template",

        // Theme options
        theme_advanced_buttons1: "bold,italic,underline,strikethrough,|,justifyleft,justifycenter,justifyright,justifyfull,|,formatselect,fontselect,fontsizeselect",
        theme_advanced_buttons2: "cut,copy,paste,pastetext,pasteword,|,search,|,bullist,numlist,|,outdent,indent,|,undo,redo,|,link,unlink,image,cleanup,code,|,preview,|,forecolor,backcolor",
        theme_advanced_buttons3: "tablecontrols,|,hr,removeformat,visualaid,|,sub,sup,|,iespell,media,|,fullscreen",
        theme_advanced_toolbar_location: "top",
        theme_advanced_toolbar_align: "left",
        theme_advanced_statusbar_location: "bottom",
        theme_advanced_resizing: true
    });
    //#endregion
});

$(document).on('click', '.close-FullDetail', function () {
    //$('.upd-detail-textarea').fadeIn();
    $('#ModalUpDFullDetail').addClass('hide');
    //$(".upd-detail-click").click();
});

$(document).on('click', '.SaveFullDetail', function () {
    //$('.upd-detail-textarea').fadeIn();
    $('#ModalUpDFullDetail').addClass('hide');
    var text = tinyMCE.get('UpDFullDetail-modal').getContent();
    tinyMCE.get('UpDFullDetail').setContent(text);
});

function compareKeyword(kw) {
    var len = $('#tagit-editkeyword .tagit-choice .tagit-label').length;
    for (var i = 0; i < len; i++) {
        var val = $('.tagit-choice .tagit-label').eq(i).text();
        if (val == kw) {
            return false;
        }
    }
}
function Getmorekeyword() {
    ProKeyword = "";
    $('#tagit-editkeyword li').last().hide();
    $('#tagit-editkeyword').find(".tagit-choice .tagit-label").each(function (index) {
        var kw = index - 1;
        ProKeyword += "<li class='tagit-choice'>";
        ProKeyword += $(this).text();
        ProKeyword += "<a class='icon-remove mar_t3 mar_l2' style='cursor:pointer' onclick='removetag($(this))'></a></li>";
    });
}

function addmorekeyword(Obj) {
    Getmorekeyword();
    var html = ProKeyword;
    var KwCatelv1 = $("#SelectCateLV1 option:selected");
    var KwCatelv2 = $("#SelectCateLV2 option:selected");
    var KwCatelv3 = $("#SelectCateLV3 option:selected");
    if (Obj == 1) {
        if (KwCatelv1.val() == 0) {
            bootbox.alert(label.vldselectcateLV1);
        }
        else {
            compareKeyword(KwCatelv1.text());
            if (compareKeyword(KwCatelv1.text()) == false) {
                bootbox.alert(label.vldduplicatekeyword);
            }
            else {
                html += "<li class='tagit-choice'>" + KwCatelv1.text() + "<a class='icon-remove mar_t3 mar_l2' style='cursor:pointer' onclick='removetag($(this))'></a></li>";
            }
        }
    }
    if (Obj == 2) {
        if (KwCatelv2.val() == 0) {
            bootbox.alert(label.vldselectcateLV2);
        }
        else {
            compareKeyword(KwCatelv2.text());
            if (compareKeyword(KwCatelv2.text()) == false) {
                bootbox.alert(label.vldduplicatekeyword);
            }
            else {
                html += "<li class='tagit-choice'>" + KwCatelv2.text() + "<a class='icon-remove mar_t3 mar_l2' style='cursor:pointer' onclick='removetag($(this))'></a></li>";
            }
        }
    }
    if (Obj == 3) {
        if (KwCatelv3.val() == 0) {
            bootbox.alert(label.vldselectcateLV3);
        }
        else {
            compareKeyword(KwCatelv3.text());
            if (compareKeyword(KwCatelv3.text()) == false) {
                bootbox.alert(label.vldduplicatekeyword);
            }
            else {
                html += "<li class='tagit-choice'>" + KwCatelv3.text() + "<a class='icon-remove mar_t3 mar_l2' style='cursor:pointer' onclick='removetag($(this))'></a></li>";
            }
        }
    }
    $('#tagit-editkeyword .tagit-choice .tagit-label').remove();
    $('#tagit-editkeyword li').last().show();
    $('#tagit-editkeyword').prepend(html);
}
function removetag(Obj) {
    Obj.parents('li').remove();
}
$('#SelectUpd_Promotion').live('click', function () {
    if ($(this).attr("checked") == true || $(this).attr("checked") == "checked") {
        $(this).val(1);
        $('#Upd_Promotionprice').removeAttr('disabled');
        $('#Upd_Promotionprice').removeAttr('readonly');
        $('.Upd_Promotionprice .icon_money_up,.Upd_Promotionprice .icon_money_down').css('display', 'inline');
    } else {
        $(this).val(0);
        $('#Upd_Promotionprice').val('0.00');
        $('#Upd_Promotionprice').attr('disabled', 'disabled');
        $('#Upd_Promotionprice').attr('readonly', 'readonly');
        $('.Upd_Promotionprice .icon_money_up,.Upd_Promotionprice .icon_money_down').css('display', 'none');
    }
});

//------------------------------Get Next Page---------------------------------------------//
var stat_edit = "stat";
var used1_edit = false;
var used2_edit = false;
var used3_edit = false;
function GoToPageEditStep(currentpage, pageto, save) {
    var bool1 = false;
    var bool2 = false;
    if (currentpage == 1) {
        bool1 = checkEditProductPageOne();
        //console.log('checkEditProductPageOne + ' + bool1);
        if (bool1 == true) {
            used1_edit = true;
            //console.log('used1_edit true + ' + true);
        }
    }

    else if (currentpage == 2) {
        bool2 = checkEditProductPageTwo();
        //console.log('checkEditProductPageTwo + ' + bool2);
        if (bool2) {
            used2_edit = true;
        }
    }
    if (bool2 == true && save == true) {
        EditProduct();
    }

    //  console.log('GoToPageEdit Step current : ' + currentpage + ' | pageto : ' + pageto + ' | save : ' + save)

    if ((currentpage == 1 && used1_edit == true) || (currentpage == 2 && used2_edit == true) ||
        (currentpage == 3 && used3_edit == true) && save == false) {
        //console.log('used1_edit = ' + used1_edit);
        //console.log('used2_edit = ' + used2_edit);
        if (currentpage == 1 && pageto == 2 && bool1 == true) {
            SwapPageTo(2);
            //console.log('SwapPageTo2');
        } else if (currentpage == 2 && pageto == 1 && bool2 == true) {
            SwapPageTo(1);
            //console.log('SwapPageTo1');
        }

    }
    // console.log("edit = " + stat_edit + " | u1 = " + used1_edit + " | u2 = " + used2_edit + " | u3 = " + used3_edit);
}


function SwapPageTo(page) {
    //console.log('go page : ' + page);
    var stat1 = $('#EditProduct_Form').find("#stat1");
    var stat2 = $('#EditProduct_Form').find("#stat2");
    if (page == 1) {

        stat1.addClass("unensconce");
        stat1.removeClass("ensconce");
        stat2.addClass("ensconce");
        stat2.removeClass("unensconce");

    } else if (page == 2) {
        stat1.removeClass("unensconce");
        stat1.addClass("ensconce");
        stat2.removeClass("ensconce");
        stat2.addClass("unensconce");

    }

}


function checkEditProductPageOne() {
    var bool = true;
    console.log('checkEditProductPageOne');
    GetValueTagit();
    GetImgPath();
    if (!CheckProductName()) {
        bool = false;
    }
    console.log('CheckProductName : ' + bool);
    if (!CheckProductKeyword()) {
        bool = false; console.log(2);
    }
    console.log('CheckProductKeyword : ' + bool);

    if (!CheckImgPath()) {
        bool = false; console.log(3);
    }
    console.log('CheckImgPath : ' + bool);


    if (!CheckShortDesc()) {
        bool = false; //console.log(4);
    }
    console.log('CheckShortDesc : ' + bool);

    //console.log('check update category');
    //if ($('#Upd_CategoryCode').val().length > 1) {
        $("#Upd_Category").closest('.control-group').addClass('success');
        $("#Upd_Category").closest('.control-group').removeClass('error');
    //    //  $("#Upd_Category .successImg").css('display', 'block');
    //    $(".div-sel-category").css('display', 'none');
    //    bool = true;
    //}
    //else {
    //    //console.log($("#Upd_Category").val());
    //    $("#Upd_Category").closest('.control-group').removeClass('success');
    //    $("#Upd_Category").closest('.control-group').addClass('error');
    //    $("#TextSearchCategory").attr('placeholder', label.vldrequired);
    //    bool = false;

    //}
    console.log('bool ' + bool);
    return bool;
}

function CheckQty() {
    if ($('#Upd_QtyUnit option:selected').val() == 0) {
        $("#Upd_Qty").closest('.control-group').removeClass('success');
        $("#Upd_Qty").closest('.control-group').addClass('error');
        $(".Upd_QtyUnit > .error").text('Please fill Product Unit');
        return false;
    }
    else {
        if ($('#Upd_QtyUnit option:selected').val() == "Other") {
            if ($('#QtyOther').val() == "") {
                $("#Upd_Qty").closest('.control-group').removeClass('success');
                $("#Upd_Qty").closest('.control-group').addClass('error');
                $(".QtyOther > .errorQtyOther").text('Please fill Product Unit');
                return false;
            } else {
                $("#Upd_Qty").closest('.control-group').addClass('success');
                $("#Upd_Qty").closest('.control-group').removeClass('error');
                $(".QtyOther > .errorQtyOther").text('');
                return true;
            }
        } else {
            $("#Upd_Qty").closest('.control-group').addClass('success');
            $("#Upd_Qty").closest('.control-group').removeClass('error');
            $(".Upd_QtyUnit > .error").text('');
            return true;
        }
    }
}

function checkEditProductPageTwo() {
    var bool = true;
    $("#Upd_Price").closest('.control-group').addClass('success');
    $("#Upd_Price").closest('.control-group').removeClass('error');
    $(".Price > .success").css('display', 'block');
    //console.log('edit Upd_Price : ' + bool);

    //if ($("#Upd_Qty").val() != null && $('#Upd_Qty').val()) {
    //    var qty = parseFloat($("#Upd_Qty").val());
    //    if (qty <= 0) {
    //        $("#Upd_Qty").closest('.control-group').removeClass('success');
    //        $("#Upd_Qty").closest('.control-group').addClass('error');
    //        bool = false;
    //    }
    //    else {
    $("#Upd_Qty").closest('.control-group').addClass('success');
    $("#Upd_Qty").closest('.control-group').removeClass('error');

    $("#Upd_Price_One").closest('.control-group').addClass('success');
    $("#Upd_Price_One").closest('.control-group').removeClass('error');
    $(".Price_One > .success").css('display', 'block');

    //    }
    //}
    if (!CheckQty()) {
        bool = false;
    }
    //console.log('edit Upd_Qty : ' + bool);

    if (!CheckFullDetail()) {
        bool = false;
    }
    //console.log('edit full detail : ' + bool);

    //if (!CheckProductCode()) {
    //    bool = false;
    //}
    //console.log('edit productcode : ' + bool);

    return bool;
}
//function CheckAddProduct() {
//    var bool = true;
//    GetValueTagit();
//    GetImgPath();



//    if (!CheckProductName()) {
//        bool = false;
//    }
//    //console.log('name : ' + bool);

//    if (!CheckImgPath()) {
//        bool = false;
//    }
//    //console.log('img : ' + bool);

//    if (!CheckProductCode()) {
//        bool = false;
//    }
//    //console.log('code : ' + bool);

//    if (!CheckProductKeyword()) {
//        bool = false;
//    }
//    //console.log('keyword : ' + bool);

//    if (!CheckShortDesc()) {
//        bool = false;
//    }
//    //console.log('short-desc : ' + bool);


//    if (!CheckFullDetail()) {
//        bool = false;
//    }
//    //console.log('ProductDetail : ' + bool);


//    if ($('#QtyUnit option:selected').val() == 0) {
//        $("#Qty").closest('.control-group').removeClass('success');
//        $("#Qty").closest('.control-group').addClass('error');
//        bool = false;
//    }
//    else {
//        $("#Qty").closest('.control-group').addClass('success');
//        $("#Qty").closest('.control-group').removeClass('error');
//    }

//    if ($('#Upd_CategoryCode').val().length > 1) {
//        $("#Upd_Category").closest('.control-group').addClass('success');
//        $("#Upd_Category").closest('.control-group').removeClass('error');
//        //  $("#Upd_Category .successImg").css('display', 'block');
//        $(".div-sel-category").css('display', 'none');

//    }
//    else {
//        $("#Upd_Category").closest('.control-group').removeClass('success');
//        $("#Upd_Category").closest('.control-group').addClass('error');
//        $("#TextSearchCategory").attr('placeholder', label.vldrequired);
//        bool = false;

//    }

//    $("#Price").closest('.control-group').addClass('success');
//    $("#Price").closest('.control-group').removeClass('error');
//    $(".Price > .success").css('display', 'block');

//    $("#ProductGroup").closest('.control-group').addClass('success');
//    $("#ProductGroup").closest('.control-group').removeClass('error');
//    $(".ProductGroup").css('color', '#468847');
//    //     $(".ProductGroup .success").css('display', 'block');

//    if (!bool) {
//        bootbox.alert(label.vldall_required);
//        return false;
//    }
//    else {
//        SaveProduct();
//    }
//}

$("#TextSearchCategory").keypress(function (event) {
    if (event.which == 13) {
        SearchCategory();
    }
});

function CheckCate() {
    if ($('#Upd_Category').val().length > 1) {
        $("#Upd_Category").closest('.control-group').removeClass('error');
        $("#Upd_Category").closest('.control-group').addClass('success');
        //  $("#Category .successImg").css('display', 'block');
        $(".errorCate > .error").text('');
        $(".div-sel-category").css('display', 'none');
        return true;
    }
    else {
        $("#Upd_Category").closest('.control-group').removeClass('success');
        $("#Upd_Category").closest('.control-group').addClass('error');
        $(".errorCate").text(label.vldrequired);
        return false;
    }
}

function SearchCategory() {

    $(".div-sel-category").fadeIn();
    var txtsrc = $('.txtSearchCate').val();
    if (txtsrc.length > 0) {
        SearchCategoryByName(txtsrc);
        //console.log(txtsrc);
        $("#TextSearchCategory").attr('placeholder', '');
    } else {

        //   $("#AllCate .successCate").hide();
        $("#Upd_Category").closest('.control-group').removeClass('success');
        $("#Upd_Category").closest('.control-group').addClass('error');
        $("#TextSearchCategory").attr('placeholder', label.vldrequired);
        $(".div-sel-category").fadeOut();
        $('.click-cateall').css('display', 'block');
    }
    return false;
}
function SearchCategoryByName(txtsearch) {
    data = {
        CategoryName: txtsearch
    }
    $('.Bg_searchcate').hide();
    $('.show-catepath').text('Loading..');
    $('.show-catepath').fadeIn();
    $.ajax({
        url: GetUrl("MyB2B/Product/SearchCategory"),
        data: data,
        success: function (data) {
            $('.show-catepath').hide();
            $(".div-sel-category").html(data);
        },
        error: function () {
            OpenLoading(false);
            bootbox.alert("Error");
        },
        type: "POST"
    });
}


function CheckExistProductName() {
    //don't use
    $.ajax({
        url: GetUrl("MyB2B/Product/ValidateAddProduct"),
        data: { ProductName: $('#ProductName').val() },
        type: "POST",
        success: function (data) {
            if (($('#ProductName').val() == "") || ($('#ProductName').val().length < 4) || (!data)) {
                $("#ProductName").closest('.control-group').removeClass('success');
                $("#ProductName").closest('.control-group').addClass('error');
                //    $(".ProductName > .success").css('display', 'none');
                if ($('#ProductName').val() == "") {
                    $(".ProductName > .error").text(label.vldrequired);
                }
                else if ($('#ProductName').val().length < 4) {
                    $(".ProductName > .error").text(label.vldless_4char);
                }
                else if (!data) {
                    $(".ProductName > .error").text(label.vldpname_already);
                }
            }
            else {
                $("#ProductName").closest('.control-group').addClass('success');
                $("#ProductName").closest('.control-group').removeClass('error');
                $(".ProductName > .error").text('');
                //     $(".ProductName > .success").css('display','block');
            }
        },
        error: function () {
            bootbox.alert(label.vldcannot_check_info);
        }
    });
}
function CheckExistProductCode() {
    $.ajax({
        url: GetUrl("MyB2B/Product/ValidateAddProduct"),
        data: { ProductCode: $('#ProductCode').val() },
        type: "POST",
        success: function (data) {
            if (!data) {
                $("#ProductCode").closest('.control-group').removeClass('success');
                $("#ProductCode").closest('.control-group').addClass('error');
                //     $(".ProductCode > .success").css('display', 'none');
                $(".ProductCode > .error").text(label.vldpname_already);
                return false;
            } else {
                $("#ProductCode").closest('.control-group').addClass('success');
                $("#ProductCode").closest('.control-group').removeClass('error');
                $(".ProductCode > .error").text('');
                //       $(".ProductCode > .success").css('display', 'block');
                return true;
            }

        },
        error: function () {
            bootbox.alert(label.vldcannot_check_info);
        }
    });
}

//----เก็บสคริป .blur-----------------
$("#Price").live('blur', function () {
    if ($('#Price').val() == "") {
        $(this).val("0.00");
    }
    $(this).val(parseFloat(eval($(this).val())).toFixed(2));
    $('.Price .error').text('');
    $("#Price").closest('.control-group').addClass('success');
    $("#Price").closest('.control-group').removeClass('error');
    //  $(".Price > .success").css('display', 'block');
});

$("#Price_One").live('blur', function () {
    if ($('#Price_One').val() == "") {
        $(this).val("0.00");
    }
    $(this).val(parseFloat(eval($(this).val())).toFixed(2));
    $('.Price_One .error').text('');
    $("#Price_One").closest('.control-group').addClass('success');
    $("#Price_One").closest('.control-group').removeClass('error');
    //  $(".Price > .success").css('display', 'block');
});

$("#tagit-keyword").live('blur', function () {
    GetValueTagit();
    CheckProductKeyword();
});
$("#tagit-detail").live('blur', function () {
    GetValueTagit();
    CheckShortDesc();
});
$('#tagit-keyword .tagit-input').live('click', function () {
    $(".Keyword > .error").text('');
});
$('#tagit-detail .tagit-input').live('click', function () {
    $(".Detail > .error").text('');
    $(".Detail .textdesc").text('ระบบจะแสดง "คำอธิบาย" เพียง 5 รายการแรกเท่านั้น');
});
$("#AllQty").live('blur', function () {
    $(this).val(parseFloat(eval($(this).val())).toFixed(2));
    if ($('#QtyUnit option:selected').val() == 0) {
        $("#AllQty").closest('.control-group').removeClass('success');
        $("#AllQty").closest('.control-group').addClass('error');
        //  $("#AllQty .errorQty").css('display', 'block');
        ///  $("#AllQty .successQty").css('display', 'none');
        $(".Upd_QtyUnit > .error").text('กรุณากรอกหน่วยสินค้า');
    }
    else {
        $("#AllQty").closest('.control-group').addClass('success');
        $("#AllQty").closest('.control-group').removeClass('error');
        $(".Upd_QtyUnit > .error").text('');
        //   $("#AllQty .errorQty").css('display', 'none');
        //    $("#AllQty .successQty").css('display', 'block');
    }
});

//         $("#TextSearchCategory").live('blur', function () { 
//             CheckCateCode();
//         });


$(function () {

    $("em").remove();
    $(".t-upload-button > span").remove();
    /*-----------------------ให้พิมพ์ราคาได้เฉพาะตัวเลขเท่านั้น-----------------------*/
    $('#Price').bind('keypress', function (e) {
        return (e.which != 8 && e.which != 46 && e.which != 0 && (e.which < 48 || e.which > 57)) ? false : true;
    })

    $('#Price_One').bind('keypress', function (e) {
        return (e.which != 8 && e.which != 46 && e.which != 0 && (e.which < 48 || e.which > 57)) ? false : true;
    })

    $('#Qty').bind('keypress', function (e) {
        return (e.which != 8 && e.which != 46 && e.which != 0 && (e.which < 48 || e.which > 57)) ? false : true;
    })
    /*-----------------------text numunical--------------------------------*/
    $(".icon_money_up").live('click', function () {
        var input = $(this).parent().parent().find("input[type=text]:eq(0)");
        if (input.val() == "") {
            input.val(1)
        } else {
            input.val(parseFloat(eval(input.val()) + 1).toFixed(2))
        }
    });
    $(".icon_money_down").live('click', function () {
        var input = $(this).parent().parent().find("input[type=text]:eq(0)");
        if (input.val() == "") {
            //bootbox.alert('ไม่สามารถเลือกให้น้อยกว่า 0 ได้');
        } else {
            if (parseFloat(input.val()) > 0) {
                input.val(parseFloat(eval(input.val()) - 1).toFixed(2))
            } else {
                return false;
                //bootbox.alert('ไม่สามารถเลือกให้น้อยกว่า 0 ได้');
            }
        }
    });

    //#region---------------------------Text editor---------------------------------------------
    if ($(window).width() < 1025) {
        var buttons1 = "newdocument,|,justifyleft,justifycenter,justifyright,fontselect,fontsizeselect,formatselect";
        var buttons2 = "cut,copy,paste,|,bold,italic,underline,|,forecolor,backcolor,|,undo,redo,|,anchor,image,hr,removeformat";
    }
    else if ($(window).width() < 1281) {
        var buttons1 = "newdocument,|,justifyleft,justifycenter,justifyright,fontselect,fontsizeselect,formatselect,code,preview,|,bullist,numlist,charmap";
        var buttons2 = "cut,copy,paste,|,bold,italic,underline,|,forecolor,backcolor,|,undo,redo,|,anchor,image,hr,removeformat,backcolorinsertdate,insertdate,inserttime,|,sub,sup,emotions";
    }
    else {
        var buttons1 = "newdocument,|,justifyleft,justifycenter,justifyright,fontselect,fontsizeselect,formatselect,code,preview,|,bullist,numlist,|,outdent,indent";
        var buttons2 = "cut,copy,paste,|,bold,italic,underline,|,forecolor,backcolor,|,undo,redo,|,anchor,image,hr,removeformat,backcolorinsertdate,insertdate,inserttime,|,sub,sup,|,charmap,emotions";
    }

    
    //#endregion
    //             bootbox.alert('ยกเลิกการเพิ่มสินค้า');

});
function compareKeyword(kw) {
    var len = $('#tagit-keyword .tagit-choice .tagit-label').length;
    for (var i = 0; i < len; i++) {
        var val = $('.tagit-choice .tagit-label').eq(i).text();
        if (val == kw) {
            return false;
        }
    }
}
function Getmorekeyword() {
    ProKeyword = "";
    $('#tagit-keyword li').last().hide();
    $('#tagit-keyword').find(".tagit-choice .tagit-label").each(function (index) {
        var kw = index - 1;
        ProKeyword += "<li class='tagit-choice'>";
        ProKeyword += $(this).text();
        ProKeyword += "<a class='icon-remove mar_t3 mar_l2' style='cursor:pointer' onclick='removetag($(this))'></a></li>";
    });
}
function addmorekeyword(Obj) {
    Getmorekeyword();
    var html = ProKeyword;
    var KwCatelv1 = $("#SelectCateLV1 option:selected");
    var KwCatelv2 = $("#SelectCateLV2 option:selected");
    var KwCatelv3 = $("#SelectCateLV3 option:selected");
    if (Obj == 1) {
        if (KwCatelv1.val() == 0) {
            bootbox.alert(label.vldselectcateLV1);
        }
        else {
            compareKeyword(KwCatelv1.text());
            if (compareKeyword(KwCatelv1.text()) == false) {
                bootbox.alert(label.vldduplicatekeyword);
            }
            else {
                html += "<li class='tagit-choice'>" + KwCatelv1.text() + "<a class='icon-remove mar_t3 mar_l2' style='cursor:pointer' onclick='removetag($(this))'></a></li>";
            }
        }
    }
    if (Obj == 2) {
        if (KwCatelv2.val() == 0) {
            bootbox.alert(label.vldselectcateLV2);
        }
        else {
            compareKeyword(KwCatelv2.text());
            if (compareKeyword(KwCatelv2.text()) == false) {
                bootbox.alert(label.vldduplicatekeyword);
            }
            else {
                html += "<li class='tagit-choice'>" + KwCatelv2.text() + "<a class='icon-remove mar_t3 mar_l2' style='cursor:pointer' onclick='removetag($(this))'></a></li>";
            }
        }
    }
    if (Obj == 3) {
        if (KwCatelv3.val() == 0) {
            bootbox.alert(label.vldselectcateLV3);
        }
        else {
            compareKeyword(KwCatelv3.text());
            if (compareKeyword(KwCatelv3.text()) == false) {
                bootbox.alert(label.vldduplicatekeyword);
            }
            else {
                html += "<li class='tagit-choice'>" + KwCatelv3.text() + "<a class='icon-remove mar_t3 mar_l2' style='cursor:pointer' onclick='removetag($(this))'></a></li>";
            }
        }
    }
    $('#tagit-keyword .tagit-choice .tagit-label').remove();
    $('#tagit-keyword li').last().show();
    $('#tagit-keyword').prepend(html);
}
function removetag(Obj) {
    Obj.parents('li').remove();
}

$('#SelectPromotion').live('click', function () {
    if ($(this).attr("checked") == true || $(this).attr("checked") == "checked") {
        $(this).val(1);
        $('#Promotionprice').removeAttr('disabled');
        $('#Promotionprice').removeAttr('readonly');
        $('.Promotionprice .icon_money_up,.Promotionprice .icon_money_down').css('display', 'inline');
    } else {
        $(this).val(0);
        $('#Promotionprice').val('0.00');
        $('#Promotionprice').attr('disabled', 'disabled');
        $('#Promotionprice').attr('readonly', 'readonly');
        $('.Promotionprice .icon_money_up,.Promotionprice .icon_money_down').css('display', 'none');
    }
});

function GetValueTagit() {
    ProductKeyword = "";
    ProductDetail = "";

    $('#tagit-keyword').find(".tagit-choice .tagit-label").each(function (index) {
        var kw = index - 1;
        ProductKeyword += $(this).text();
        ProductKeyword += "~";
    });
    ProductKeyword = ProductKeyword.substring(0, ProductKeyword.length - 1)

    $('#tagit-detail').find(".tagit-choice .tagit-label").each(function (index) {
        var dt = index - 1;
        ProductDetail += $(this).text();
        ProductDetail += "~";
    });
    ProductDetail = ProductDetail.substring(0, ProductDetail.length - 1)
}