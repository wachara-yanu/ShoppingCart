//---------------------------Text Editor-----------------------------------
// AddProduct
var focusshow = false;
var focushide = false;
var IsShowTextEditorProductDetail = false;

$(function () {

    $("#GetProductImgPath").sortable({
        cursor: "move",
        helper: function (event, ui) {
            var img = ui.find(".ProImgpath").attr("src");
            var val = "<img class='thumbnail' style='background:#ffffff;' src='" + img + "' style='width:95px; height:95px;' />";

            return $(val);
        },
        update: function (event, ui) {

            SetFirstImage();
        }
    }).disableSelection();

});

function beforecateLV1() {
    selectedcateLV1();
    $('.morekey1').removeAttr('disabled');
}

function beforecateLV2() {
    selectedcateLV2();
    $('.morekey2').removeAttr('disabled');
}

function selectedcateLV3() {

    catename = $("#SelectCateLV3 option:selected").attr("class");
    catecode = $("#SelectCateLV3 option:selected").attr("catecode");
    catepath = $("#SelectCateLV3 option:selected").attr("catepath");

    $('#Category').css('display', 'block');
    $('.Show-selectCate').addClass('hide');
    $('.click-cateall').css('display', 'block');
    $("#CategoryCode").val(catecode);
    $("#CategoryCode").attr('data-catename', catename);
    $(".txtSearchCate").val(catename);
    $(".show-catepath").text(catepath);
    $(".show-catepath").fadeIn();
    $("#TextSearchCategory").attr('placeholder', '');
    CheckCateCode();
    $('.morekey3').removeAttr('disabled');
    $("#Category").closest('.control-group').addClass('success');
    $("#Category").closest('.control-group').removeClass('error');
    $("#Category").closest('.control-group').removeClass('errorCate');
}

$(".product-detail-click").click(function () {

    //if (!IsShowTextEditorProductDetail) {
        //$(this).text(label.vldhide_moredetail);
        //IsShowTextEditorProductDetail = true;
        //$('.product-detail-textarea').fadeIn();
        var text = tinyMCE.get('ProductDetail').getContent();
        tinyMCE.get('ProductDetail-modal').setContent(text);
        $('#ModalProductDetail').removeClass('hide');
    //} else {
        //$(this).text(label.vldadd_moredetail);
        //IsShowTextEditorProductDetail = false;
        //$('#ModalProductDetail').addClass('hide');
        //$('.product-detail-textarea').fadeOut();
    //}

});

$('#Price , #Qty , #Price_One').click(function () {
    $(this).select();
});


function CheckFullDetail() {
    var detail = tinyMCE.get('ProductDetail').getContent();
    //console.log("detail  " + detail);
    if (detail == "") {
        //console.log("detail  NULL");
        $("#FullDetail").closest('.control-group').removeClass('success');
        $("#FullDetail").closest('.control-group').addClass('error');
        $(".FullDetail .error").text(label.vldrequired + 'รายละเอียดเพิ่มเติม');
        return false;
        $("#FullDetail .successImg").css('display', 'none');
    }
    else if (detail.length > 10000) {
        //console.log("detail.length  " + detail.length);
        $("#FullDetail").closest('.control-group').removeClass('success');
        $("#FullDetail").closest('.control-group').addClass('error');
        $(".FullDetail .error").text(label.vldformaterror + 'รายละเอียดเพิ่มเติม');
        return false;
    }
    else {
        //console.log("true  ");
        $("#FullDetail").closest('.control-group').addClass('success');
        $("#FullDetail").closest('.control-group').removeClass('error');
        $("#FullDetail .error").text('');
        return true;
        $("#FullDetail .successImg").css('display', 'block');
    }
}

$(".mceFirst").live("click", function () {
    focushide = true;
});

//========================= Category ====================//

function CheckCateCode() {
    var str = $('#CategoryCode').val();
    if (str != undefined) {
        if (str.length > 1) {
            $("#AllCate").closest('.control-group').removeClass('error');
            $("#AllCate").closest('.control-group').addClass('success');
            $("#AllCate .errCate").hide()
            //   $("#AllCate .successCate").show();

        } else {
            $("#AllCate").closest('.control-group').removeClass('success');
            $("#AllCate").closest('.control-group').addClass('error');
            $("#AllCate .errCate").show()
            //     $("#AllCate .successCate").hide();
        }
    }
}

//-----------Confirm Save and Cancel------------------------------

function SaveProduct() {
    var catecode = "";
    var cateLV3 = "";
    var QtyUnit = "";
    GetUrl("Search/Product/List");
    if ($(".div-sel-category select option:selected").val() == undefined) {
        catecode = $("#SelectCateLV3 option:selected").attr("catecode");
        cateLV3 = $("#SelectCateLV3 option:selected").val();
    }
    else {
        catecode = $(".div-sel-category select option:selected").attr("catecode");
        cateLV3 = $(".div-sel-category select option:selected").val();
    }
    var FullDetail = "";
    if ($('#hidAddProductNotLogin').val() != null && $('#hidAddProductNotLogin').val() != undefined) {
        FullDetail = tinyMCE.get('ProductDetail').getContent();
    }
    if ($('#QtyUnit option:selected').val() == label.vldother) {
        QtyUnit = $('#QtyOther').val();
    } else {
        QtyUnit = $('#QtyUnit option:selected').val();
    }
    data = {
        ProductName: $("#ProductName").val(),
        ProductGroup: $("#ProductGroup option:selected").val(),//,
        ProductCode: $("#ProductCode").val(),
        Price: $("#Price").val(),
        Price_One: $("#Price_One").val(), //เพิ่มราคาสินค้า/ชิ้น
        Keyword: ProductKeyword,
        QuickDetail: ProductDetail,
        FullDetail: tinyMCE.get('ProductDetail').getContent(),
        Qty: $("#Qty").val(),
        QtyUnit: QtyUnit,
        Catecode: catecode,
        CateLV3: cateLV3,
        ProductImgPath: GetImgPath(),
        Ispromotion: 0,
        PromotionPrice: 0
    }
    //console.log(data); 
    OpenLoading(true);

    $.ajax({
        url: GetUrl("MyB2B/Product/AddProduct"),
        data: data,
        traditional: true,
        success: function (data) {
            CheckError(data);
            OpenLoading(false);

            $('#ModalAddProduct').modal('hide');
            $("html").css('overflow', 'auto');
            $('#AddProduct_Form').remove();
            SubmitPage(1);
        },
        error: function () {
            bootbox.alert("Cannot save");
            OpenLoading(false);
        },
        type: "POST"
    });
}


/*--------------------------------------Register------------------------------------------------*/

function SaveProductNotLogin() {
    var catecode = "";
    var cateLV3 = "";
    GetUrl("Search/Product/List");
    if ($(".div-sel-category select option:selected").val() == undefined) {
        catecode = $("#SelectCateLV3 option:selected").attr("catecode");
        cateLV3 = $("#SelectCateLV3 option:selected").val();
    }
    else {
        catecode = $(".div-sel-category select option:selected").attr("catecode");
        cateLV3 = $(".div-sel-category select option:selected").val();
    }

    var FullDetail = "";

    if ($('#hidAddProductNotLogin').val() != "0" && $('#hidAddProductNotLogin').val() != undefined) {
        FullDetail = tinyMCE.get('ProductDetail').getContent();
    }
    data = {
        ProductName: $("#ProductName").val(),
        ProductGroup: 0,//$("#ProductGroup option:selected").val(),
        ProductCode: $("#ProductCode").val(),
        Price: $("#Price").val(),
        Price_One: $("#Price_One").val(), //เพิ่มราคาสินค้า/ชิ้น
        Keyword: ProductKeyword,
        QuickDetail: ProductDetail,
        FullDetail: FullDetail,
        Qty: $("#Qty").val(),
        QtyUnit: $('#QtyUnit option:selected').val(),
        Catecode: catecode,
        CateLV3: cateLV3,
        ProductImgPath: ProductImgPath,
        Ispromotion: 0,
        PromotionPrice: 0
    }

    // console.log(data); 

    $.ajax({
        url: GetUrl("MyB2B/Product/AddProduct"),
        data: data,
        traditional: true,
        success: function (data) {
            CheckError(data);
            OpenLoading(false);
            $('#ModalAddProduct').modal('hide');
            $("html").css('overflow', 'auto');
            window.location.href = GetUrl("MyB2B/product/Index");                //redirect to product
        },
        error: function () {
            OpenLoading(false);
            bootbox.alert("Cannot save");
        },
        type: "POST"
    });
}
$('#ModalAddProduct').on('hidden.bs.modal', function (e) {
    // do something...

    //console.log('hidden');
    $('#ModalAddProduct').html('');
    $("html").css('overflow', 'auto');

})
function Register() {
    //register before upload photo
    OpenLoading(true);
    data = {
        UserName: $("#UserName").val(),
        Password: $("#Password").val(),
        Email: $("#Email").val(),
        CompName: $("#CompName").val(),
        BizTypeID: $("#BizTypeID").val(),
        BizTypeOther: $("#BizTypeOther").val(),
        FirstName: $("#FirstName").val(),
        LastName: $("#LastName").val(),
        Phone: $("#Phone").val(),
        ProvinceID: $("#ProvinceID").val(),
        DistrictID: $("#DistrictID").val(),
    };
    $.ajax({
        url: GetUrl("Member/AddRegister"),
        data: data,
        traditional: true,
        success: function (data) {

            OpenLoading(false);
            //console.log('regis success');
            //console.log(data);
            if (data.IsResult) {
                //data.IsResult is result from send email (true or false)
                SaveProductNotLogin();
            }
            else {
                // console.log("IsResult=" + data.IsResult);
            }
        },
        error: function () {
            OpenLoading(false);
            bootbox.alert("Register error");
            //console.log("Register Error");
        },
        type: "post"

    });

}


//-----------GetValueTagit--------------------------------------
var ProductKeyword = "";
var ProductDetail = "";
var ProductImgPath = new Array();

//============ Get Product Img ===================//
function GetImgPath() {
    var len = $('.img-data').length;
    ProductImgPath = new Array();
    ProductImgID = new Array();
    for (var i = 0; i < len; i++) {
        ProductImgPath[i] = $('.img-data').eq(i).attr('img-name');
    }
    //console.log(ProductImgPath);
    return ProductImgPath;
}

function CheckImgPath() {
    ProductImgPath = GetImgPath();
    //console.log('checkImg :' + ProductImgPath.length);
    if (ProductImgPath.length > 0) {
        $("#ProductImgPath").addClass('success');
        $("#ProductImgPath").removeClass('error');
        $(".ProductImgPath .error").css('display', 'none');

        return true;
    }
    else {
        $("#ProductImgPath").removeClass('success');
        $("#ProductImgPath").addClass('error');
        $(".ProductImgPath .error").css('display', 'block');
        //    $(".ProductImgPath .success").css('display', 'none');
        return false;
    }
}

/*function GetValueTagit() {
    ProductKeyword = ""; 
    ProductDetail = "";
    $('#tagit-keyword').find(".tagit-choice").each(function (index) {
        var kw = index - 1;
        ProductKeyword += $(this).text();
        ProductKeyword += "~";
    });
    ProductKeyword = ProductKeyword.substring(0, ProductKeyword.length - 1)
    $('#tagit-detail').find(".tagit-choice").each(function (index) {
        var dt = index - 1;
        ProductDetail += $(this).text();
        ProductDetail += "~";
    });
    ProductDetail = ProductDetail.substring(0, ProductDetail.length - 1)
}*/

function GetValueTagit() {
    ProductKeyword = "";
    $('#tagit-keyword').find(".tagit-choice .tagit-label").each(function (index) {
        //var kw = index - 1;
        //ProductKeyword += $(this).text().replace("x", "~");
        var kw = index - 1;
        ProductKeyword += $(this).text();
        ProductKeyword += "~";
    });
    ProductKeyword = ProductKeyword.substring(0, ProductKeyword.length - 1)

    ProductDetail = "";
    $('#tagit-detail').find(".tagit-choice .tagit-label").each(function (index) {
        var kw = index - 1;
        ProductDetail += $(this).text();
        ProductDetail += "~";
    });
    ProductDetail = ProductDetail.substring(0, ProductDetail.length - 1)
}



function CheckProductCode() {
    if (($('#ProductCode').val() == "") || ($('#ProductCode').val().length < 3)) {
        $("#ProductCode").closest('.control-group').removeClass('success');
        $("#ProductCode").closest('.control-group').addClass('error');
        //   $(".ProductCode > .success").css('display', 'none');

        if ($('#ProductCode').val() == "") {
            $("#ProductCode").attr('placeholder', label.vldless_2char);
        }
        else if ($('#ProductCode').val().length < 3) {
            $("#ProductCode").attr('placeholder', label.vldless_2char);
        }
        return false;
    }
    else {
        $("#ProductCode").closest('.control-group').addClass('success');
        $("#ProductCode").closest('.control-group').removeClass('error');
        $("#ProductCode").attr('placeholder', '');
        //     $(".ProductCode > .success").css('display', 'block');
        return true;
    }

}


function CheckProductName() {
    var name = $.trim($('#ProductName').val());
    if (checkDisclaimer($('#ProductName').val())) {
        if (name.length > 2 && name.length <= 100) {
            $("#ProductName").closest('.control-group').addClass('success');
            $("#ProductName").closest('.control-group').removeClass('error');
            //$(".BuyleadName > .success").show();
            $(".ProductName > .error").text('');
            return true;
        }
        else {
            $("#ProductName").closest('.control-group').removeClass('success');
            $("#ProductName").closest('.control-group').addClass('error');
            // $(".BuyleadName > .success").css('display', 'none');
            if (name.length > 0 && name.length < 4) {
                $(".ProductName > .error").text("กรุณากรอกอย่างน้อย 3 ตัวอักษร และไม่เกิน 100 ตัวอักษร");
            } else if (name.length > 100) {
                $(".ProductName > .error").text("กรุณากรอกอย่างน้อย 3 ตัวอักษร และไม่เกิน 100 ตัวอักษร");
            } else {
                $(".ProductName > .error").text(label.vldrequired);
            }
            return false;
        }
    } else {
        $("#ProductName").closest('.control-group').removeClass('success');
        $("#ProductName").closest('.control-group').addClass('error');
        $(".ProductName > .error").text("กรุณากรอกข้อมูลให้ถูกต้อง ไม่ควรมีเครื่องหมาย \!@#$^฿|{}[]'\"<>=:;~");
        $(".ProductName > .error").css('text-align', 'left');
        checkError();
        result = false;
    }

}

function CheckCate() {
    if ($('#CategoryCode').val().length > 1) {
        $("#Category").closest('.control-group').removeClass('error');
        $("#Category").closest('.control-group').addClass('success');
        //  $("#Category .successImg").css('display', 'block');
        $(".errorCate > .error").text('');
        $(".error > .error").text('');
        $(".div-sel-category").css('display', 'none');
        return true;
    }
    else {
        $("#Category").closest('.control-group').removeClass('success');
        $("#Category").closest('.control-group').addClass('error');
        $(".errorCate").text(label.vldrequired);
        return false;
    }
}

/*---------------------------------------Add Product------------------------------------------*/
var stat = "stat";
var used1 = false;
var used2 = false;
var used3 = false;

function GoToPageStep(currentpage, pageto, save) {
    //console.log('GoToPageAdd Step current : ' + currentpage + ' pageto : ' + pageto + ' save : ' + save)
    var bool1 = false;
    var bool2 = false;
    if (currentpage <= pageto) {
        //      next step
        if (currentpage == 1) {
            bool1 = chcekAddProductPageOne();       // check error on page one
            if (bool1)
                used1 = true;
        } else if (currentpage == 2) {
            bool2 = chcekAddProductPageTwo();       // check error on page two
            if (bool2)
                used2 = true;
        }

        if (bool2 == true && save == true)          //save is not error 
            SaveProduct();

        if ((currentpage == 1 && bool1 == true) ||
            (currentpage == 2 && used2 == true) ||
            (currentpage == 3 && used3 == true)
            && save == false)     //next page or back page 
        {
            $("#" + stat + currentpage + "").removeClass("unensconce");
            $("#" + stat + currentpage + "").addClass("ensconce");

            $("#" + stat + pageto + "").removeClass("ensconce");
            $("#" + stat + pageto + "").addClass("unensconce");
        }

        //$("#" + stat + currentpage + "").removeClass("unensconce");
        //$("#" + stat + currentpage + "").addClass("ensconce");

        //$("#" + stat + pageto + "").removeClass("ensconce");
        //$("#" + stat + pageto + "").addClass("unensconce");
        // console.log("add-" + stat + " - u1 = " + used1 + " , u2 = " + used2 + " , u3 = " + used3);
    } else {
        // prev step
        //console.log("#" + stat + currentpage + "");
        //console.log("#" + stat + pageto + "");
        $("#" + stat + currentpage + "").removeClass("unensconce");
        $("#" + stat + currentpage + "").addClass("ensconce");
        $("#" + stat + pageto + "").removeClass("ensconce");
        $("#" + stat + pageto + "").addClass("unensconce");
    }
}

function GoToPageStepNotLogin(currentpage, pageto, save) {
    //console.log('GoToPageStepNotLogin // currentpage :' + currentpage + ' pageto:' + pageto + ' save: ' + save);
    var bool = false;
    var used = false;
    if (currentpage <= pageto) {
        if (currentpage == 1) {
            bool = chcekAddProductPageOne();
            if (bool)
                used = true;
        }
        else if (currentpage == 2) {
            bool = chcekAddProductPageTwo();
            //console.log('page 2 > ' + bool);
            if (bool)
                used = true;
        }
        else if (currentpage == 3) {
            bool = chcekAddProductPageThree();
            if (bool)
                used = true;
        }

        if (save == true && bool == true) {
            Register();
        }

        if ((currentpage == 1 && used == true) ||
            (currentpage == 2 && used == true) ||
            (currentpage == 3 && used == true) && save == false) {
            $("#" + stat + currentpage + "").removeClass("unensconce");
            $("#" + stat + currentpage + "").addClass("ensconce");

            $("#" + stat + pageto + "").removeClass("ensconce");
            $("#" + stat + pageto + "").addClass("unensconce");
        }

        //$("#" + stat + currentpage + "").removeClass("unensconce");
        //$("#" + stat + currentpage + "").addClass("ensconce");

        //$("#" + stat + pageto + "").removeClass("ensconce");
        //$("#" + stat + pageto + "").addClass("unensconce");
    } else {
        $("#" + stat + currentpage + "").removeClass("unensconce");
        $("#" + stat + currentpage + "").addClass("ensconce");

        $("#" + stat + pageto + "").removeClass("ensconce");
        $("#" + stat + pageto + "").addClass("unensconce");
    }
}

function chcekAddProductPageOne() {               // Page Add Product
    var bool = true;
    GetValueTagit();
    GetImgPath();

    if ($('#CategoryCode').val().length > 1) {
        console.log('CategoryCode  true');
        $("#Category").closest('.control-group').addClass('success');
        $("#Category").closest('.control-group').removeClass('error');
        //  $("#Category .successImg").css('display', 'block');
        $(".div-sel-category").css('display', 'none');
        bool = true;
    }
    else {
        console.log('CategoryCode  false');
        $("#Category").closest('.control-group').removeClass('success');
        $("#Category").closest('.control-group').addClass('error');
        $("#TextSearchCategory").attr('placeholder', label.vldrequired);
        bool = false;
    }

    if (!CheckProductName()) {
        bool = false;
    }
    console.log("CheckProductName: " + bool);
    if (!CheckProductKeyword()) {
        bool = false;
    }
    console.log("CheckProductKeyword: " + bool);
    if (!CheckImgPath()) {
        bool = false;
    }
    console.log("CheckImgPath: " + bool);
    if (!CheckShortDesc()) {
        bool = false;
    }
    console.log("CheckShortDesc: " + bool);
    
    console.log("bool: " + bool);
    return bool;
}


function CheckQty() {
    if ($('#QtyUnit option:selected').val() == 0) {
        $("#Qty").closest('.control-group').removeClass('success');
        $("#Qty").closest('.control-group').addClass('error');
        $(".QtyUnit > .error").text('กรุณากรอกหน่วยสินค้า');
        return false;
    }
    else {
        if ($('#QtyUnit option:selected').val() == label.vldother) {
            if ($('#QtyOther').val() == "") {
                $("#Qty").closest('.control-group').removeClass('success');
                $("#Qty").closest('.control-group').addClass('error');
                $(".QtyOther > .errorQtyOther").text('กรุณากรอกหน่วยสินค้า');
                return false;
            } else {
                $("#Qty").closest('.control-group').addClass('success');
                $("#Qty").closest('.control-group').removeClass('error');
                $(".QtyOther > .errorQtyOther").text('');
                return true;
            }
        } else {
            $("#Qty").closest('.control-group').addClass('success');
            $("#Qty").closest('.control-group').removeClass('error');
            $(".QtyUnit > .error").text('');
            return true;
        }
    }

}
function chcekAddProductPageTwo() {                            // Page Add Product
    var bool = true;

    $("#Price").closest('.control-group').addClass('success');
    $("#Price").closest('.control-group').removeClass('error');
    $(".Price > .success").css('display', 'block');

    $("#Price_One").closest('.control-group').addClass('success'); //เพิ่มราคาสินค้า/ชิ้น
    $("#Price_One").closest('.control-group').removeClass('error');
    $(".Price_One > .success").css('display', 'block');

    if (!CheckQty()) {
        bool = false;
    }

    if (!CheckFullDetail()) {
        bool = false;
    }

    //if (!CheckProductCode()) {
    //    bool = false;
    //}

    return bool;
}

$(document).on("change", "#BizTypeID", function (e) {
    var val = $('#BizTypeID').val();
    //console.log(val);
    if (val == '13') {
        $('.BizTypeOther').show();
    } else {
        $(".BizTypeOther").val("");
        $(".BizTypeOther").hide();
    }

});
function checkAgree() {
    var agree = $('#agree').val();
    if (agree == "false") {
        $('#agree').focus();
        bootbox.alert(label.vldAgreement);
        return false;
    }
    else {
        return true;
    }
}
function chcekAddProductPageThree()    // Page Register
{
    var bool = true;
    if ($('#signup_form').valid()) {
        var UserName = checkUserName();
        var Email = checkEmail();
        var CompName = checkCompName();
        var isAgree = $('#agree').val();
        //console.log('isAgree ' + isAgree);

        if (UserName && Email && CompName)
            bool = true;
        else
            bool = false;

        if (isAgree == "false") {
            bool = false;
        }
    }
    else
        bool = false;

    return bool;
}

function CheckAddProduct() {
    var bool = true;
    GetValueTagit();
    GetImgPath();

    if ($('#CategoryCode').val().length > 1) {
        $("#Category").closest('.control-group').addClass('success');
        $("#Category").closest('.control-group').removeClass('error');
        $("#Category").closest('.control-group').removeClass('errorCate');
        //  $("#Category .successImg").css('display', 'block');
        //$(".div-sel-category").css('display', 'none');
        bool = true;
    }
    else {
        $("#Category").closest('.control-group').removeClass('success');
        $("#Category").closest('.control-group').addClass('error');
        $("#TextSearchCategory").attr('placeholder', label.vldrequired);
        bool = false;

    }

    $("#Price").closest('.control-group').addClass('success');
    $("#Price").closest('.control-group').removeClass('error');
    $(".Price > .success").css('display', 'block');

    $("#Price_One").closest('.control-group').addClass('success');
    $("#Price_One").closest('.control-group').removeClass('error');
    $(".Price_One > .success").css('display', 'block');

    $("#ProductGroup").closest('.control-group').addClass('success');
    $("#ProductGroup").closest('.control-group').removeClass('error');
    $(".ProductGroup").css('color', '#468847');
    //     $(".ProductGroup .success").css('display', 'block');

    if (!CheckProductName()) {
        bool = false;
    }
    //console.log('name : ' + bool);

    if (!CheckImgPath()) {
        bool = false;
    }
    //console.log('img : ' + bool);

    if (!CheckProductCode()) {
        bool = false;
    }
    //console.log('code : ' + bool);

    if (!CheckProductKeyword()) {
        bool = false;
    }
    //console.log('keyword : ' + bool);

    if (!CheckShortDesc()) {
        bool = false;
    }
    //console.log('short-desc : ' + bool);


    if (!CheckFullDetail()) {
        bool = false;
    }
    //console.log('ProductDetail : ' + bool);

    if (!CheckQty()) {
        bool = false;
    }

    //if ($('#QtyUnit option:selected').val() == 0) {
    //    $("#Qty").closest('.control-group').removeClass('success');
    //    $("#Qty").closest('.control-group').addClass('error');
    //    bool = false;
    //}
    //else {
    //    $("#Qty").closest('.control-group').addClass('success');
    //    $("#Qty").closest('.control-group').removeClass('error');
    //}

    if (!bool) {
        bootbox.alert(label.vldall_required);
        return false;
    }
    else {
        SaveProduct();
    }
}

$("#TextSearchCategory").keypress(function (event) {
    if (event.which == 13) {
        SearchCategory();
    }
});

function CheckShortDesc() {
    GetValueTagit();
    var list = ProductDetail.split('~');
    // console.log("ProductDetail" + ProductDetail);
    //console.log("list" + list);
    // console.log("list.length" + list.length);
    if (ProductDetail.length > 0) {
        //  console.log("ProductDetail.length > 0");
        if (ProductDetail.length >= 0 && ProductDetail.length <= 500) {
            if (list.length > 5) {
                //   console.log("list.length > 5");
                $("#tagit-detail").closest('.control-group').removeClass('success');
                $("#tagit-detail").closest('.control-group').addClass('error');
                $("#tagit-detail").css('border', 'solid 1px #B94A48');
                $(".Detail .textdesc").text('ระบบจะแสดง "คำอธิบาย" เพียง 5 รายการแรกเท่านั้น');
                return false;
            } else {
                //console.log("true");
                $("#tagit-detail").closest('.control-group').addClass('success');
                $("#tagit-detail").closest('.control-group').removeClass('error');
                $(".Detail > .error").text('');
                //$(".Detail .textdesc").text('ระบบจะแสดง "คำอธิบาย" เพียง 5 รายการแรกเท่านั้น');
                $("#tagit-detail").css('border', 'solid 1px #468847');
                return true;
            }
        } else {
            //console.log("else");
            $("#tagit-detail").closest('.control-group').removeClass('success');
            $("#tagit-detail").closest('.control-group').addClass('error');
            //$(".Detail > .error").text(label.vldrequired);
            $(".Detail .textdesc").text('"คำอธิบาย" ควรกรอกไม่เกิน 100 ตัวอักษร ต่อ 1 รายการ');
            $("#tagit-detail").css('border', 'solid 1px #B94A48');
            return false;
        }
    } else {
        //console.log("else");
        $("#tagit-detail").closest('.control-group').removeClass('success');
        $("#tagit-detail").closest('.control-group').addClass('error');
        //$(".Detail > .error").text(label.vldrequired);
        $(".Detail .textdesc").text('กรุณากรอกข้อมูล');
        $("#tagit-detail").css('border', 'solid 1px #B94A48');
        return false;
    }
}

function CheckProductKeyword() {
    GetValueTagit();
    var list = ProductKeyword.split('~');
    //console.log("list ==" + list);
    if (list == label.vldnon_specific_keyword) {
        $("#tagit-keyword").closest('.control-group').removeClass('success');
        $("#tagit-keyword").closest('.control-group').addClass('error');
        $("#tagit-keyword").css('border', 'solid 1px #B94A48');
        $(".Keyword > .error").text(label.vldrequired);
        return false;
    }
    //console.log("list.length  " + list.length);
    //console.log("list.length - 1 " + list.length-1);
    //console.log("ProductKeyword.length  " + ProductKeyword.length);
    if (ProductKeyword.length > 0) {
        if (ProductKeyword.length >= 0 && ProductKeyword.length <= 250) {
            if (list.length > 5) {
                //console.log("list.length > 6");
                $("#tagit-keyword").closest('.control-group').removeClass('success');
                $("#tagit-keyword").closest('.control-group').addClass('error');
                $("#tagit-keyword").css('border', 'solid 1px #B94A48');
                $(".Keyword > .error").text('ระบบจะแสดง "เพิ่มคีย์เวิร์ด" เพียง 5 รายการแรกเท่านั้น');
                return false;
            } else {
                //console.log("true");
                $("#tagit-keyword").closest('.control-group').addClass('success');
                $("#tagit-keyword").closest('.control-group').removeClass('error');
                $("#tagit-keyword").css('border', 'solid 1px #468847');
                $(".Keyword > .error").text('');
                return true;
            }
            } else {
                //console.log("list.length - 1 " + list.length - 1);
                //console.log("else2");
                $("#tagit-keyword").closest('.control-group').removeClass('success');
                $("#tagit-keyword").closest('.control-group').addClass('error');
                $("#tagit-keyword").css('border', 'solid 1px #B94A48');
                $(".Keyword > .error").text('"คีย์เวิร์ด" ควรกรอกไม่เกิน 50 ตัวอักษร ต่อ 1 รายการ');
                return false;
            }

    } else {
        //console.log("list.length - 1 " + list.length - 1);
        //console.log("else2");
        $("#tagit-keyword").closest('.control-group').removeClass('success');
        $("#tagit-keyword").closest('.control-group').addClass('error');
        $("#tagit-keyword").css('border', 'solid 1px #B94A48');
        $(".Keyword > .error").text('กรุณากรอกข้อมูล');
        return false;
    }
}

/*function CheckProductKeyword() {
    var list = ProductKeyword.split('~');
    if (ProductKeyword.length > 0) {

   //     if (list.length > 6) {
   //         $("#tagit-keyword").closest('.control-group').removeClass('success');
   //         $("#tagit-keyword").closest('.control-group').addClass('error');
   //         $(".Keyword > .error").text(label.vldmax_word5);
   ////         $(".Keyword > .success").css('display', 'none');
   //         $("#tagit-keyword").css('border', 'solid 1px #B94A48');
   //         return false;
   //     } else {
            $("#tagit-keyword").closest('.control-group').addClass('success');
            $("#tagit-keyword").closest('.control-group').removeClass('error');
            $(".Keyword > .error").text('');
   //         $(".Keyword > .success").css('display', 'block');
            $("#tagit-keyword").css('border', 'solid 1px #468847');
            return true;
        //}
         
    } else {
        $("#tagit-keyword").closest('.control-group').removeClass('success');
        $("#tagit-keyword").closest('.control-group').addClass('error');
        //$(".Keyword > .error").text(label.vldrequired);
   //     $(".Keyword > .success").css('display', 'none');
        $("#tagit-keyword").css('border', 'solid 1px #B94A48');
        return false;
    }
}*/


function SearchCategory() {
    $(".div-sel-category").fadeIn();
    var txtsrc = $('.txtSearchCate').val();
    if (txtsrc.length > 0) {
        SearchCategoryByName(txtsrc);
        $("#Category").closest('.control-group').removeClass('error');
        $("#Category").closest('.control-group').addClass('success');
        $(".errorCate").text('');
    } else {
        //   $("#AllCate .successCate").hide();
        $("#Category").closest('.control-group').removeClass('success');
        $("#Category").closest('.control-group').addClass('error');
        $(".errorCate").text(label.vldrequired);
        $(".div-sel-category").fadeOut();
        return false;
    }
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

            $("#Category").closest('.control-group').removeClass('error');
            $("#Category").closest('.control-group').addClass('success');
        },
        error: function () {
            OpenLoading(false);
            bootbox.alert("Error");
        },
        type: "POST"
    });
}

function addCateByID(id) {
    $.ajax({
        url: GetUrl("MyB2B/Product/SearchCategoryByID"),
        data: { CategoryID: id },
        type: "POST",
        success: function (data) {
            $('#SelectCateLV1 > option[value *= ' + data.cate1 + '] ').attr('selected', true);
            $.ajax({
                url: GetUrl("Default/SelectCategoryLV2"),
                data: { lv1: $('#SelectCateLV1 option:selected').val() },
                type: "POST",
                success: function (data1) {
                    $("#SelectCateLV2").removeAttr('disabled');
                    $("#SelectCateLV2").html(data1);
                    $('#SelectCateLV2 > option[value *= ' + data.cate2 + '] ').attr('selected', true);

                    $.ajax({
                        url: GetUrl("Default/SelectCategoryLV3"),
                        data: { lv2: $('#SelectCateLV2 option:selected').val() },
                        type: "POST",
                        success: function (data2) {
                            $("#SelectCateLV3").removeAttr('disabled');
                            $("#SelectCateLV3").html(data2);
                            $('#SelectCateLV3 > option[value *= ' + id + '] ').attr('selected', true);
                        },
                        error: function () {
                        }
                    });
                },
                error: function () {
                }
            });
        },
        error: function () {
            bootbox.alert(label.vldcannot_check_info);
        }
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
//ราคาสินค้า/ชิ้น
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
        $(".QtyUnit > .error").text('กรุณากรอกหน่วยสินค้า');
    }
    else {
        $("#AllQty").closest('.control-group').addClass('success');
        $("#AllQty").closest('.control-group').removeClass('error');
        $(".QtyUnit > .error").text('');
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
    //             bootbox.alert('ยกเลิกการเพิ่มสินค้า');

});

$(document).on('click', '.close-FullDetail', function () {
    //$('.product-detail-textarea').fadeIn();
    $('#ModalProductDetail').addClass('hide');
    //$(".upd-detail-click").click();
});

$(document).on('click', '.SaveFullDetail', function () {
    //$('.product-detail-textarea').fadeIn();
    $('#ModalProductDetail').addClass('hide');
    var text = tinyMCE.get('ProductDetail-modal').getContent();
    tinyMCE.get('ProductDetail').setContent(text);
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