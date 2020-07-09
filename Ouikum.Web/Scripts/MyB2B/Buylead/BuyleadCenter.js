//------------------------BuyleadCenterImgUpload-----------------------//
var FileName = "";
var FileThumb = "";
var FileOld = "";
var EditImages = "";
var indexFile = 0;

function onSuccessBuyleadImg(e) {
    OpenLoadingNew(false);
    try {
        FileName = e.response.newimage;
        del = "<i id='del' class='icon-remove-sign btn-tootip-top cursor hide' rel='tooltip'  title='Delete' style='position:absolute;margin-left: 98px;margin-top: -5px; display:block;' onclick='Del_image()'></i>";
       img = "<img class='img-polaroid' id='BuyleadImgPath_0' src='" + GetUpload("Temp/Buylead/" + getCookie("CompID") + "/" + FileName) + "' onload='resizeImg($(this),100,100);' img-name='" + FileName+"'>";
            $("#BuyleadImgPath_0").addClass("show_image").css("display", "block");
            $("#BuyleadImg_0").html(del+img).removeClass("NoImg");
            $("#BuyleadImgPath_0").val(FileName);
            $(".t-upload-files").remove();
        $("#BuyleadCenterImg").css('color', '#468847');
        $("#BuyleadCenterImg .success").css('display', 'block');
        $("#BuyleadCenterImg .error").css('display', 'none');
        
    } catch (err) {
    }
}

function onUploadBuyleadImg(e) {
    var files = e.files;
    OpenLoadingNew(true,$("body"));
    $.each(files, function () {
        if (this.extension != ".jpg" && this.extension != ".JPG" && this.extension != ".jpeg" && this.extension != ".JPEG" && this.extension != ".gif" && this.extension != ".GIF" && this.extension != ".png" && this.extension != ".PNG") {
            bootbox.alert(label.vldfix_format_picture);
            OpenLoadingNew(false);
            e.preventDefault();
            return false;
        }
    });
}

function Del_image() {
    if (confirm("Confirm Delete Item")) {
        var no_img = "<img class='img-polaroid' id='BuyleadImgPath_0' src='http://www.placehold.it/100x100/EFEFEF/AAAAAA&text=no+image' />";
        $("#BuyleadImg_0").html(no_img).addClass("NoImg");
        $("#BuyleadImgPath_0").val("");
        $("#BuyleadCenterImg").css('color', '#333333');
        $("#BuyleadCenterImg .success").css('display', 'none');
        $("#BuyleadCenterImg .error").css('display', 'none');
    }
}

function OpenLoadingNew(isLoad, obj) {
    var mar_t = ($(window).height()/ 2)-50 ;
    var mar_l = ($(window).width()/2) - 110;
    if (isLoad == true) {
        obj.prepend('<div id="loading"><div id="imgloading"><img src=\"' + GetUrl('Content/Default/images/icon-load.gif') + '\" ></div></div>')
        $("#loading").css({ 'filter': 'alpha(opacity=50)', 'opacity': '0.5' });
        $("#imgloading").css("margin", mar_t + "px 0 0 " + mar_l + "px")
    }
    else {
        $('#loading').remove(); $('#imgloading').remove()
    }
}

//---------------------BuyleadCenter General Info--------------------------//
function SearchCategory() {
    $(".div-sel-category").fadeIn();
    var txtsrc = $('.txtSearchCate').val();
    if (txtsrc.length > 0) {
        SearchCategoryByName(txtsrc);
        $(".errorCate").text('');
    } else {
        //   $("#AllCate .successCate").hide();
        $("#Category").closest('.control-group').removeClass('success');
        $("#Category").closest('.control-group').addClass('error');
        $(".errorCate").text(label.vldrequired);
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
        url: GetUrl("BuyleadCenter/SearchCategory"),
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

function selectedcateLV3() {
    catename = $("#SelectCateLV3 option:selected").attr("class");
    catecode = $("#SelectCateLV3 option:selected").attr("catecode");
    catepath = $("#SelectCateLV3 option:selected").attr("catepath");

    $('#Category').css('display', 'block');
    $('.Show-selectCate').addClass('hide');
    $(".errorCate").text('');
    $('.click-cateall').css('display', 'block');
    $("#CategoryCode").val(catecode);
    $("#CategoryCode").attr('data-catename', catename);
    $(".txtSearchCate").val(catename);
    $(".show-catepath").text(catepath);
    $(".show-catepath").fadeIn();
    $(".errorCate").text('');
    CheckCateCode();
}

function FindCate() {
    if ($(".div-sel-category select option:selected").attr("catecode") != undefined) {
        Catecode = $(".div-sel-category select option:selected").attr("catecode");
    }
    else if ($("#SelectCateLV3 option:selected").attr("catecode") != undefined) {
        Catecode = $("#SelectCateLV3 option:selected").attr("catecode");
    }
    else {
        Catecode = $("#CategoryCode").val();
    }
    if ($(".div-sel-category select option:selected").val() != undefined) {
        CateLV3 = $(".div-sel-category select option:selected").val();
    }
    else if ($("#SelectCateLV3 option:selected").val()!=0) {
        CateLV3 = $("#SelectCateLV3 option:selected").val();
    }
    else {
        CateLV3 = $("#CategoryCode").attr("data-id");
    } 
}

//-----------Confirm Save and Cancel------------------------------
function SaveBuylead() {
    var BuyleadExpire = $('#dp3').val().substring(3, 6) + $('#dp3').val().substring(0, 3) + $('#dp3').val().substring(6, 10);
        data = {
            BuyleadID: $("#BuyleadID").val(),
            RowFlag: $("#RowFlag").val(),
            BuyleadName: $("#BuyleadName").val(),
            BuyleadCode: $("#BuyleadCode").val(),
            BuyleadType: $('input[name=optionsType]:checked').val(),
            BuyleadExpire: BuyleadExpire,
            BuyleadKeyword: BuyleadKeyword,
            BuyleadDetail: $("#BuyleadDetail").val(),
            Qty: $("#Qty").val(),
            QtyUnit: $('#QtyUnit option:selected').val(),
            Catecode: Catecode,
            CateLV3: CateLV3,
            BuyleadImgPath: BuyleadImgPath,
            BuyLeadImgOldfile: BuyLeadImgOldfile,
            CompName:$("#CompName").val(),
            ContactName:$("#ContactName").val(),
            Position:$("#Position").val(),
            Phone: $("#Phone").val(),
            Email:$("#Email").val(),
            Mobile: $("#Mobile").val(),
            Fax: $("#Fax").val(),
            Address:$("#Address").val(),
            District:$("#District").val(),
            Province:$("#Province").val(),
            Postal: $("#Postal").val()
        }

        OpenLoadingNew(true, $("body"));
        //console.log("SaveBuylead data" + data);

        $.ajax({
            url: GetUrl("BuyleadCenter/AddBuylead"),
            data: data,
            traditional: true,
            success: function (data) {
                CheckError(data);
                OpenLoadingNew(false);
                //console.log("data.IsResult" + data.IsResult);
                if (data.IsResult == true) {
                    //if ($('#BuyleadID').val() != null && $('#hidCompID').val() != null) {
                    //    window.location = GetUrl("BuyleadCenter/Main/Channel2?ID=" + $('#BuyleadID').val() + "&Comp=" + $('#hidCompID').val());
                    //}
                    //console.log("data.IsResult true >>");
                    OpenLoading(false, null, $('.navbar'));
                    bootbox.alert(label.vldsave_buyleadSuccess, function () {
                        if ($("#UserLoginCompID").val() > 0) {
                            window.location = GetUrl("MyB2B/buylead");
                        }
                        else {
                            window.location = GetUrl("BuyleadCenter/Main/Index");
                        }
                    });
                } else {
                    //console.log("data.IsResult false >>");
                    bootbox.alert(label.vldcannot_check_info);
                }
            },
            error: function () {
                bootbox.alert(label.vldcannot_check_info);
            },
            type: "POST"
        });

        return false;
    }

//-----------GetValueTagit--------------------------------------
var BuyleadKeyword = "";
          
//============ Get Buylead Img ===================//
function GetImgPath() {
    BuyleadImgPath = $('#BuyleadImgPath_0').attr('img-name');
    BuyLeadImgOldfile = $('#BuyLeadImgOldfile').val(); 
}

function GetValueTagit() {
    BuyleadKeyword = "";
    $('#tagit-keyword').find("li").each(function (index) {
        var kw = index - 1;
        BuyleadKeyword += $(this).text();
        BuyleadKeyword += "~";
    });
    BuyleadKeyword = BuyleadKeyword.substring(0, BuyleadKeyword.length - 1)
}

function CheckBuyleadCode() {
    var name = $.trim($('#BuyleadCode').val());
    if (checkDisclaimer($('#BuyleadCode').val())) {
        if (name.length > 2 && name.length <= 15) {
            $("#BuyleadCode").closest('.control-group').addClass('success');
            $("#BuyleadCode").closest('.control-group').removeClass('error');
            //$(".BuyleadName > .success").show();
            $(".BuyleadCode > .error").text('');
            return true;
        }
        else {
            $("#BuyleadCode").closest('.control-group').removeClass('success');
            $("#BuyleadCode").closest('.control-group').addClass('error');
            // $(".BuyleadName > .success").css('display', 'none');
            if (name.length > 0 && name.length < 4) {
                $(".BuyleadCode > .error").text("กรุณากรอกอย่างน้อย 3 ตัวอักษร และไม่เกิน 15 ตัวอักษร");
            } else if (name.length > 15) {
                $(".BuyleadCode > .error").text("กรุณากรอกอย่างน้อย 3 ตัวอักษร และไม่เกิน 15 ตัวอักษร");
            } else {
                $(".BuyleadCode > .error").text(label.vldrequired);
            }
            return false;
        }
    } else {
        $("#BuyleadCode").closest('.control-group').removeClass('success');
        $("#BuyleadCode").closest('.control-group').addClass('error');
        $(".BuyleadCode > .error").text("กรุณากรอกข้อมูลให้ถูกต้อง ไม่ควรมีเครื่องหมาย \!@#$^฿|{}[]'\"<>=:;~");
        $(".BuyleadCode > .error").css('text-align', 'left');
        checkError();
        result = false;
    }
}

function CheckBuyleadName() {
    var name = $.trim($('#BuyleadName').val());
    if (checkDisclaimer($('#BuyleadName').val())) {
        if (name.length > 2 && name.length <= 100) {
            $("#BuyleadName").closest('.control-group').addClass('success');
            $("#BuyleadName").closest('.control-group').removeClass('error');
            //$(".BuyleadName > .success").show();
            $(".BuyleadName > .error").text('');
            return true;
        }
        else {
            $("#BuyleadName").closest('.control-group').removeClass('success');
            $("#BuyleadName").closest('.control-group').addClass('error');
            // $(".BuyleadName > .success").css('display', 'none');
            if (name.length > 0 && name.length < 4) {
                $(".BuyleadName > .error").text("กรุณากรอกอย่างน้อย 3 ตัวอักษร และไม่เกิน 100 ตัวอักษร");
            }else if (name.length > 100) {
                $(".BuyleadName > .error").text("กรุณากรอกอย่างน้อย 3 ตัวอักษร และไม่เกิน 100 ตัวอักษร");
            } else {
                $(".BuyleadName > .error").text(label.vldrequired);
    }
            return false;
        }
    } else {
        $("#BuyleadName").closest('.control-group').removeClass('success');
        $("#BuyleadName").closest('.control-group').addClass('error');
        $(".BuyleadName > .error").text("กรุณากรอกข้อมูลให้ถูกต้อง ไม่ควรมีเครื่องหมาย \!@#$^฿|{}[]'\"<>=:;~");
        $(".BuyleadName > .error").css('text-align', 'left');
        checkError();
        result = false;
    }
}

function CheckQty() {
    console.log("Qty: " + $('#Qty').val());
    if ($('#Qty').val() == 0.00 || $('#Qty').val() == "0.00" || $('#QtyUnit option:selected').val() == 0) {
        $("#Qty").closest('.control-group').removeClass('success');
        $("#Qty").closest('.control-group').addClass('error');
        return false;
    } else {
        $("#Qty").closest('.control-group').addClass('success');
        $("#Qty").closest('.control-group').removeClass('error');
        return true;
    }
}

function CheckQtyUnit() {
    console.log("QtyUnit: " + $('#QtyUnit option:selected').val());
    if ($('#QtyUnit option:selected').val() == 0 || $('#Qty').val() == 0.00 || $('#Qty').val() == "0.00" || $('#QtyUnit option:selected').val() == "อื่นๆ") {
        $("#Qty").closest('.control-group').removeClass('success');
        $("#Qty").closest('.control-group').addClass('error');
        return false;
    }
    else {
        $("#Qty").closest('.control-group').addClass('success');
        $("#Qty").closest('.control-group').removeClass('error');
        return true;
    }
}

function CheckCate() {
    if ($('#CategoryCode').val().length > 1) {
        $("#Category").closest('.control-group').addClass('success');
        $("#Category").closest('.control-group').removeClass('error');
        //  $("#Category .successImg").css('display', 'block');
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

function CheckBuyleadExpire() {
    if ($('#dp3').val() == "") {
        $("#dp3").closest('.control-group').removeClass('success');
        $("#dp3").closest('.control-group').addClass('error');
        $(".BuyleadExpire > .error").text(label.vldrequired);
        return false;
    }
    else {
        $("#dp3").closest('.control-group').addClass('success');
        $("#dp3").closest('.control-group').removeClass('error');
        $(".BuyleadExpire > .error").text('');
        return true;
    }
}

function CheckBuyleadCompName() {
    if ($('#CompName').val() == "") {
        $("#CompName").closest('.control-group').removeClass('success');
        $("#CompName").closest('.control-group').addClass('error');
        $(".CompName > .error").text(label.vldrequired);
        return false;
    }
    else {
        $("#CompName").closest('.control-group').addClass('success');
        $("#CompName").closest('.control-group').removeClass('error');
        $(".CompName > .error").text('');
        return true;
    }
}

function CheckBuyleadContactName() {
    if ($('#ContactName').val() == "") {
        $("#ContactName").closest('.control-group').removeClass('success');
        $("#ContactName").closest('.control-group').addClass('error');
        $(".ContactName > .error").text(label.vldrequired);
        return false;
    }
    else {
        $("#ContactName").closest('.control-group').addClass('success');
        $("#ContactName").closest('.control-group').removeClass('error');
        $(".ContactName > .error").text('');
        return true;
    }
}

function CheckBuyleadPositionName() {
    if ($('#Position').val() == "") {
        $("#Position").closest('.control-group').removeClass('success');
        $("#Position").closest('.control-group').addClass('error');
        $(".Position > .error").text(label.vldrequired);
        return false;
    }
    else {
        $("#Position").closest('.control-group').addClass('success');
        $("#Position").closest('.control-group').removeClass('error');
        $(".Position > .error").text('');
        return true;
    }
}

function CheckBuyleadPhone() {
    if ($('#Phone').val() == "") {
        $("#Phone").closest('.control-group').removeClass('success');
        $("#Phone").closest('.control-group').addClass('error');
        $(".Phone > .error").text(label.vldrequired);
        return false;
    }
    else {
        $("#Phone").closest('.control-group').addClass('success');
        $("#Phone").closest('.control-group').removeClass('error');
        $(".Phone > .error").text('');
        return true;
    }
}

function CheckBuyleadEmail() {
    if ($('#Email').val() == "ไม่ระบุอีเมล์") {
        $('#Email').val('purchase@b2bthai.com');
    }
    if ($('#Email').val() == "") {
        $("#Email").closest('.control-group').removeClass('success');
        $("#Email").closest('.control-group').addClass('error');
        $(".Email > .error").text(label.vldrequired);
        return false;
    }
    else{
        var filter = /^([\w-]+(?:\.[\w-]+)*)@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$/i;
        var chk = false
        var checkSpace = $('#Email').val().replace(/ /g, '');
        if (checkSpace.length < $('#Email').val().length) {
                $("#Email").closest('.control-group').removeClass('success');
                $("#Email").closest('.control-group').addClass('error');
                $(".Email > .error").text(label.vldfix_format_email);
                return false;
            }
            else if(filter.test($('#Email').val()) == 0) {
                $("#Email").closest('.control-group').removeClass('success');
                $("#Email").closest('.control-group').addClass('error');
                $(".Email > .error").text(label.vldfix_format_email);
                return false;
            }
            else {
            $("#Email").closest('.control-group').addClass('success');
            $("#Email").closest('.control-group').removeClass('error');
            $(".Email > .error").text('');
            return true;
            }
     }
}

function CheckBuyleadAddr() {
    if ($('#Address').val() == "" || $('#Address').val() == null) {
        $("#Address").closest('.control-group').removeClass('success');
        $("#Address").closest('.control-group').addClass('error');
        $(".Address > .error").text(label.vldrequired);
        return false;
    }
    else {
        $("#Address").closest('.control-group').addClass('success');
        $("#Address").closest('.control-group').removeClass('error');
        $(".Address > .error").text('');
        return true;
    }
}

function CheckBuyleadDistrict() {
    if ($('#District option:selected').val() == 0) {
        $("#District").closest('.control-group').removeClass('success');
        $("#District").closest('.control-group').addClass('error');
        $(".District > .error").text(label.vldselectdistrict);
        return false;
    }
    else {
        $("#District").closest('.control-group').addClass('success');
        $("#District").closest('.control-group').removeClass('error');
        $(".District > .error").text('');
        return true;
    }
}

function CheckBuyleadProvince() {
    if ($('#Province option:selected').val() == 0) {
        $("#Province").closest('.control-group').removeClass('success');
        $("#Province").closest('.control-group').addClass('error');
        $(".Province > .error").text(label.vldselectprovince);
        return false;
    }
    else {
        $("#Province").closest('.control-group').addClass('success');
        $("#Province").closest('.control-group').removeClass('error');
        $(".Province > .error").text('');
        return true;
    }
}

function CheckBuyleadPostal() {
    if ($('#Postal').val().length > 5) {
        $("#Postal").closest('.control-group').removeClass('success');
        $("#Postal").closest('.control-group').addClass('error');
        $(".Postal > .error").text(label.vldmore_5char);
        return false;
    }
    else {
        $("#Postal").closest('.control-group').addClass('success');
        $("#Postal").closest('.control-group').removeClass('error');
        $(".Postal > .error").text('');
        return true;
    }
}

function CheckAddBuylead() {
    var bool = true;
    GetValueTagit();
    GetImgPath();
    FindCate();
    console.log("6555555555555555555");
    if (!CheckBuyleadName()) {
        bool = false;
    }
    //console.log('CheckBuyleadName : ' + bool);
    if (!CheckBuyleadCode()) {
        bool = false;
    }
    //console.log('CheckBuyleadCode : ' + bool);
    if (!CheckBuyleadExpire()) {
        bool = false;
    }
    //console.log('CheckBuyleadExpire : ' + bool);
    if (!CheckBuyleadKeyword()) {
        bool = false;
    }
    //console.log('CheckBuyleadKeyword : ' + bool);
    if (!CheckQty()) {
        bool = false;
    }
    //console.log('CheckQty : ' + bool);
    if (!CheckQtyUnit()) {
        bool = false;
    }
    //console.log('CheckQtyUnit : ' + bool);
    
    if (!CheckCate()) {
        bool = false;
    }
    //console.log('CheckCate : ' + bool);
    if (!CheckBuyleadCompName()) {
        bool = false;
    }
   // console.log('CheckBuyleadCompName : ' + bool);
    if (!CheckBuyleadContactName()) {
        bool = false;
    }
    //console.log('CheckBuyleadContactName : ' + bool);
    //if (!CheckBuyleadPositionName()) {
    //    bool = false;
    //}
    //console.log('CheckBuyleadPositionName : ' + bool);
    if (!CheckBuyleadPhone()) {
        bool = false;
    }
    
    //if (!CheckCompareExpireDate()) {
    //    console.log(CheckCompareExpireDate());
    //    bool = false;
    //}
    //console.log('CheckCompareExpireDate : ' + bool);
    //console.log('CheckBuyleadEmail : ' + bool);
//             if (!CheckBuyleadAddr()) {
//                 bool = false;
//             }
    if (!CheckBuyleadDistrict()) {
        bool = false;
    }
    //console.log('CheckBuyleadDistrict : ' + bool);
    if (!CheckBuyleadProvince()) {
        bool = false;
    }
    //console.log('CheckBuyleadProvince : ' + bool);
//             if (!CheckBuyleadPostal()) {
//                 bool = false;
//             }
    $(".optionsBuyleadType").closest('.control-group').addClass('success');
    if (!bool) {
        //console.log("Buylead False");
        var contentH = '';
        contentH = $('.content_left').height() + 80;
        $('#Content').height(contentH);
        bootbox.alert(label.vldall_required);
        return false;
    }
    else {
        SaveBuylead();
    }
}
        
function Cancel() {
    $('#BuyleadName').val("");
    $("#BuyleadName").closest('.control-group').removeClass('success');
    $("#BuyleadName").closest('.control-group').removeClass('error');
    $(".BuyleadName > .error").text('');

    $('#BuyleadCode').val("");
    $("#BuyleadCode").closest('.control-group').removeClass('success');
    $("#BuyleadCode").closest('.control-group').removeClass('error');
    $(".BuyleadCode > .error").text('');

    $(".optionsBuyleadType").closest('.control-group').removeClass('success');
    $(".optionsBuyleadType").val(1);

    $('#dp3').val("");
    $("#dp3").closest('.control-group').removeClass('success');
    $("#dp3").closest('.control-group').removeClass('error');
    $(".BuyleadExpire > .error").text('');

    $('.tagit-choice').text("");
    $("#tagit-keyword").closest('.control-group').removeClass('success');
    $("#tagit-keyword").closest('.control-group').removeClass('error');
    $("#tagit-keyword").css('border', '1px solid #CCCCCC');
    $(".Keyword > .error").text('');

    $('#Qty').val(0.00);
    $('#QtyUnit').val(0);
    $("#AllQty").closest('.control-group').removeClass('success');
    $("#AllQty").closest('.control-group').removeClass('error');

    $('#BuyleadDetail').val("");

    var no_img = "<img class='img-polaroid' id='BuyleadImgPath_0' src='http://www.placehold.it/100x100/EFEFEF/AAAAAA&text=no+image' />";
    $("#BuyleadImg_0").html(no_img).addClass("NoImg");
    $("#BuyleadImgPath_0").val("");
    $("#BuyleadCenterImg").css('color', '#333333');
    $("#BuyleadCenterImg .success").css('display', 'none');
    $("#BuyleadCenterImg .error").css('display', 'none');

    $('#TextSearchCategory').val("");
    $("#Category").closest('.control-group').removeClass('success');
    $("#Category").closest('.control-group').removeClass('error');
    $(".show-catepath").css('display', 'none');
    $(".errorCate").text('');

    $("#CompName").closest('.control-group').removeClass('success');
    $("#CompName").closest('.control-group').removeClass('error');
    $(".CompName > .error").text('');

    $("#ContactName").closest('.control-group').removeClass('success');
    $("#ContactName").closest('.control-group').removeClass('error');
    $(".ContactName > .error").text('');

    $("#Position").closest('.control-group').removeClass('success');
    $("#Position").closest('.control-group').removeClass('error');
    $(".Position > .error").text('');

    $("#Phone").closest('.control-group').removeClass('success');
    $("#Phone").closest('.control-group').removeClass('error');
    $(".Phone > .error").text('');

    $("#Email").closest('.control-group').removeClass('success');
    $("#Email").closest('.control-group').removeClass('error');
    $(".Email > .error").text('');

    $("#Address").closest('.control-group').removeClass('success');
    $("#Address").closest('.control-group').removeClass('error');
    $(".Address > .error").text('');

    $("#District").closest('.control-group').removeClass('success');
    $("#District").closest('.control-group').removeClass('error');
    $(".District > .error").text('');

    $("#Province").closest('.control-group').removeClass('success');
    $("#Province").closest('.control-group').removeClass('error');
    $(".Province > .error").text('');

    $("#Postal").closest('.control-group').removeClass('success');
    $("#Postal").closest('.control-group').removeClass('error');
    $(".Postal > .error").text('');
}

$("#TextSearchCategory").keypress(function (event) {
    if (event.which == 13) {
        SearchCategory();
    }
});

function CheckBuyleadKeyword() {
    GetValueTagit();
    var list = BuyleadKeyword.split('~');
    if (list == label.vldnon_specific_keyword) {
        $("#tagit-keyword").closest('.control-group').removeClass('success');
        $("#tagit-keyword").closest('.control-group').addClass('error');
        $("#tagit-keyword").css('border', 'solid 1px #B94A48');
        $(".Keyword > .error").text(label.vldrequired);
        return false;
    }
    if (BuyleadKeyword.length > 0) {
        if (BuyleadKeyword.length >= 0 && BuyleadKeyword.length <= 250) {
            if (list.length > 5) {
                $("#tagit-keyword").closest('.control-group').removeClass('success');
                $("#tagit-keyword").closest('.control-group').addClass('error');
                $("#tagit-keyword").css('border', 'solid 1px #B94A48');
                $(".Keyword > .error").text(label.vldMax_Word5);

                return false;
            } else {
                $("#tagit-keyword").closest('.control-group').addClass('success');
                $("#tagit-keyword").closest('.control-group').removeClass('error');
                $("#tagit-keyword").css('border', 'solid 1px #468847');
                $(".Keyword > .error").text('');

                return true;
            }
        } else {
            $("#tagit-keyword").closest('.control-group').removeClass('success');
            $("#tagit-keyword").closest('.control-group').addClass('error');
            $("#tagit-keyword").css('border', 'solid 1px #B94A48');
            $(".Keyword > .error").text('"คำอธิบาย" ควรกรอกไม่เกิน 50 ตัวอักษร ต่อ 1 รายการ');

            return false;
        }

    } else {
        $("#tagit-keyword").closest('.control-group').removeClass('success');
        $("#tagit-keyword").closest('.control-group').addClass('error');
        $("#tagit-keyword").css('border', 'solid 1px #B94A48');
        $(".Keyword > .error").text(label.vldrequired);

        return false;
    }
}

function CheckExistBuyleadName() {
//don't use
    $.ajax({
        url: GetUrl("MyB2B/Buylead/ValidateAddBuylead"),
        data: { BuyleadName: $('#BuyleadName').val() },
        type: "POST",
        success: function (data) {
            if (($('#BuyleadName').val() == "") || ($('#BuyleadName').val().length < 4) || (!data)) {
                $("#BuyleadName").closest('.control-group').removeClass('success');
                $("#BuyleadName").closest('.control-group').addClass('error');
                if ($('#BuyleadName').val() == "") {
                    $(".BuyleadName > .error").text(label.vldrequired);
                }
                else if ($('#BuyleadName').val().length < 4) {
                    $(".BuyleadName > .error").text(label.vldless_4char);
                }
                else if (!data) {
                    $(".BuyleadName > .error").text(label.vldpname_already);
                }
            }
            else {
                $("#BuyleadName").closest('.control-group').addClass('success');
                $("#BuyleadName").closest('.control-group').removeClass('error');
                $(".BuyleadName > .error").text('');
            }
        },
        error: function () {
            bootbox.alert(label.vldcannot_check_info);
        }
    });
}

function CheckExistBuyleadCode() {
    $.ajax({
        url: GetUrl("BuyleadCenter/ValidateAddBuylead"),
        data: { BuyleadCode: $('#BuyleadCode').val() },
        type: "POST",
        success: function (data) {
            if (!data) {
                $("#BuyleadCode").closest('.control-group').removeClass('success');
                $("#BuyleadCode").closest('.control-group').addClass('error');
                $(".BuyleadCode > .error").text(label.vldpname_already);
                return false;
            } else {
                $("#BuyleadCode").closest('.control-group').addClass('success');
                $("#BuyleadCode").closest('.control-group').removeClass('error');
                $(".BuyleadCode > .error").text('');
                return true;
            }  

        },
        error: function () {
            bootbox.alert(label.vldcannot_check_info);
        }
    });
}

function CheckCompareExpireDate() {
    var formatdate = $('#dp3').val().substring(6, 10)+$('#dp3').val().substring(0, 2) +$('#dp3').val().substring(3, 5);
    $.ajax({
        url: GetUrl("BuyleadCenter/ValidateAddBuyleadExpire"),
        data: { BuyleadExpire: formatdate },
        type: "POST",
        traditional: true,
        success: function (data) {
            var newexpiredate = $('#dp3').val().substring(0, 3) + $('#dp3').val().substring(3, 6) + $('#dp3').val().substring(6, 10);
            if (!data.IsResult) {
                console.log("1.false");
                $("#dp3").val(newexpiredate);
                $("#dp3").closest('.control-group').removeClass('success');
                $("#dp3").closest('.control-group').addClass('error');
                //     $(".BuyleadCode > .success").css('display', 'none');
                $(".BuyleadExpire > .error").text('วันที่สิ้นสุดประกาศซื้อต้องไม่น้อยกว่าวันที่ปัจจุบัน');
                bootbox.alert(label.vldall_required);
                return false;
            } else {
                console.log("true");
                $("#dp3").val(newexpiredate);
                $("#dp3").closest('.control-group').addClass('success');
                $("#dp3").closest('.control-group').removeClass('error');
                $(".BuyleadExpire > .error").text('');
                //       $(".BuyleadCode > .success").css('display', 'block');
                CheckAddBuylead();
                return true;
            }
        },
        error: function () {
            bootbox.alert(label.vldcannot_check_info);
        }
    });
}

function CheckCompareExpire() {
    var formatdate = $('#dp3').val().substring(6, 10)+$('#dp3').val().substring(0, 2) +$('#dp3').val().substring(3, 5);
    $.ajax({
        url: GetUrl("BuyleadCenter/ValidateAddBuylead"),
        data: { BuyleadExpire: formatdate },
        type: "POST",
        dataType: 'json',
        success: function (data) {
            var newexpiredate = $('#dp3').val().substring(3, 6) + $('#dp3').val().substring(0, 3) + $('#dp3').val().substring(6, 10);
            if (!data) {
                $("#dp3").val(newexpiredate);
                $("#dp3").closest('.control-group').removeClass('success');
                $("#dp3").closest('.control-group').addClass('error');
                //     $(".BuyleadCode > .success").css('display', 'none');
                $(".BuyleadExpire > .error").text('วันที่สิ้นสุดประกาศซื้อต้องไม่น้อยกว่าวันที่ปัจจุบัน');
                return false;
            } else {
                $("#dp3").val(newexpiredate);
                $("#dp3").closest('.control-group').addClass('success');
                $("#dp3").closest('.control-group').removeClass('error');
                $(".BuyleadExpire > .error").text('');
                //       $(".BuyleadCode > .success").css('display', 'block');
                return true;
            }
        },
        error: function () {
            bootbox.alert(label.vldcannot_check_info);
        }
    });
}

$('#Province').change(function () {
    $.ajax({
        url: GetUrl("Default/GetDistrict"),
        data: { id: $('#Province option:selected').val() },
        success: function (data) {
            $('#District').html(data);
        },
        error: function () {
            bootbox.alert(label.vldcannot_check_info);
        },
        type: "POST"
    });
    return false;
});

$(function () {
    console.log("Qty: " + $('#Qty').val());
    console.log("QtyUnit: " + $('#QtyUnit option:selected').val());
        //-----------------------เก็บสคริป .Blur Function-----------------------  
        $("#tagit-keyword").live('blur', function () {
             GetValueTagit();
             CheckBuyleadKeyword();
         });

         $("#AllQty").live('blur', function () {
             $(this).val(parseFloat(eval($(this).val())).toFixed(2));
             if ($('#QtyUnit option:selected').val() == 0 || $('#QtyUnit option:selected').val() == "อื่นๆ" || $('#Qty').val() == 0.00 || $('#Qty').val() == "0.00") {
                 $("#AllQty").closest('.control-group').removeClass('success');
                 $("#AllQty").closest('.control-group').addClass('error');
             }
             else {
                 $("#AllQty").closest('.control-group').addClass('success');
                 $("#AllQty").closest('.control-group').removeClass('error');
             }
             //if ($('#Qty').val() == 0.00 || $('#Qty').val() == "0.00") {
             //    $("#AllQty").closest('.control-group').removeClass('success');
             //    $("#AllQty").closest('.control-group').addClass('error');
             //}
             //else {
             //    $("#AllQty").closest('.control-group').addClass('success');
             //    $("#AllQty").closest('.control-group').removeClass('error');
             //}
             //if ($('#QtyUnit option:selected').val() == "อื่นๆ") {
             //    $("#AllQty").closest('.control-group').removeClass('success');
             //    $("#AllQty").closest('.control-group').addClass('error');
             //} else {
             //    $("#AllQty").closest('.control-group').addClass('success');
             //    $("#AllQty").closest('.control-group').removeClass('error');
             //}
             //if ($('#QtyOther').val() == null || $('#QtyOther').val() == "") {
             //    $("#AllQty").closest('.control-group').removeClass('success');
             //    $("#AllQty").closest('.control-group').addClass('error');
             //}
             //else {
             //    $("#AllQty").closest('.control-group').addClass('success');
             //    $("#AllQty").closest('.control-group').removeClass('error');
             //}
         });
         $("#CompName").live('blur', function () {
             CheckBuyleadCompName();
         });
         $("#ContactName").live('blur', function () {
             CheckBuyleadContactName();
         });
         //$("#Position").live('blur', function () {
         //    CheckBuyleadPositionName();
         //});
         $("#Phone").live('blur', function () {
             CheckBuyleadPhone();
         });
         $("#Email").live('blur', function () {
             CheckBuyleadEmail();
         });
         //ไม่ Validate ที่อยุ่
//         $("#Address").live('blur', function () {
//             CheckBuyleadAddr();
//         });
         $("#District").live('blur', function () {
             CheckBuyleadDistrict();
         });
         $("#Province").live('blur', function () {
             CheckBuyleadProvince();
         });
         //$("#Postal").live('blur', function () {
//             CheckBuyleadPostal();
//         });
         

//         $("#TextSearchCategory").live('blur', function () { 
//             CheckCateCode();
//         });

        $("em").remove();
        $(".t-upload-button > span").remove();
        /*-----------------------ให้พิมพ์ราคาได้เฉพาะตัวเลขเท่านั้น-----------------------*/
             
        $('#Qty').bind('keypress', function (e) {
            return (e.which != 8 && e.which != 46 && e.which != 0 && (e.which < 48 || e.which > 57)) ? false : true;
        })
        $('#Postal').bind('keypress', function (e) {
            return (e.which != 8 && e.which != 46 && e.which != 0 && (e.which < 48 || e.which > 57)) ? false : true;
        })
              /*-----------------------text numunical--------------------------------*/
        $(".icon_num_up").live('click', function () {
            var input = $(this).parent().parent().find("input[type=text]:eq(0)");

            if (input.val() == "") {
                input.val(1)
            } else {
                input.val(parseFloat(eval(input.val()) + 1).toFixed(2))
            }
        });
        $(".icon_num_down").live('click', function () {
            var input = $(this).parent().parent().find("input[type=text]:eq(0)");
            if (input.val() == "") {
                bootbox.alert(label.vldcannot_insert_zero);
            } else {
                if (parseFloat(input.val()) > 0) {
                    input.val(parseFloat(eval(input.val()) - 1).toFixed(2))
                } else {
                    //bootbox.alert('ไม่สามารถเลือกให้น้อยกว่า 0 ได้');
                }
            }
        });
    });