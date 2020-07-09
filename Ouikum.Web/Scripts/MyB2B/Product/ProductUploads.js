var FileName = "";
var FileThumb = "";
var FileOld = "";
var EditImages = "";
var indexFile = 0;


//------------------------ProductImg-----------------------//
function onCompleteProductImg(e) {
    OpenLoading(false);
    $(".t-upload-files").remove();
}
function onErrorProductImg(e) {
    // console.log('onErrorProductImg');
}

function CheckMaxUpload() {
    var isFull = false; ;
    var len = $('.img-data').length;
    if (len < 5) {
        isFull = true;
    } else {
        $(".t-upload-files").remove();
    }

    return isFull;
}

function onSuccessProductImg(e) {
    var check_img = 0;
    try {
        FileName = e.response.newimage;
        //console.log("Filename="+FileName);
    }
    catch (err) {
    }
    if (CheckMaxUpload()) {
        var findImg = $("div.NoImg").eq(0).attr("id");
        var strID = findImg.split('_');
        var x = strID[1];
        var del = "";
        var img = "";
        var Img_s =$("#imgUpload").val();
        var Img_m = $("#imgUpload").attr('title');
        if (x == 0) {
            img = "<img class='img-polaroid' id='ProductImgPath_0' src='" + GetUpload("Temp/Product/" + getCookie("CompID") + "/" + FileName) + "' onload='resizeImg($(this),"+ Img_m+","+Img_m + "); '>";
            $("#ProductImgPath_0").addClass("show_image").css("display", "block");
            $("#ProductImg_0").html(img).removeClass("NoImg");
            $("#ProductImgPath_0").val(FileName);
            del = "<i id='del_1' class='icon-remove-sign btn-tootip-top cursor hide' rel='tooltip'  title='ลบ' style='position:absolute;margin-left: "+ (Img_s-5)+"px;margin-top: -10px; display:block;' onclick='Del_image(1)'></i>";
            img = "<img id='ProductImgPath_1' class='ProImgpath img-data'  src='" + GetUpload("Temp/Product/" + getCookie("CompID") + "/" + FileName) + "' img-name='" + FileName + "' onload='resizeImg($(this)," + Img_s + "," + Img_s + "); '>";
            $("#ProductImgPath_1").addClass("show_image").css("display", "block");
            $("#ProductImg_1").html(del + img).removeClass("NoImg");
            $("#ProductImgPath_1").val(FileName);
            $("#ProductImgPath").css('color', '#468847');
            $(".ProductImgPath .success").css('display', 'block');
            $(".ProductImgPath .error").css('display', 'none');
        }
        else {
            del = "<i id='del_" + x + "' class='icon-remove-sign btn-tootip-top cursor hide' rel='tooltip'  title='ลบ' style='position:absolute;margin-left: " + (Img_s - 5) + "px;margin-top: -10px; display:block;' onclick='Del_image(" + x + ")'></i>";
            img = "<img id='ProductImgPath_" + x + "' class='ProImgpath img-data' src='" + GetUpload("Temp/Product/" + getCookie("CompID") + "/" + FileName) + "' img-data-id='" + $('.img-item').eq(x - 1).attr('data-id') + "' img-name='" + FileName + "' onload='resizeImg($(this)," + Img_s + "," + Img_s + "); '>";
            $("#ProductImgPath_" + x).addClass("show_image").css("display", "block");
            $("#ProductImg_" + x).html(del + img).removeClass("NoImg");
            $("#ProductImgPath_" + x).val(FileName);
        }
        SetFirstImage();
    } else {
        bootbox.alert(label.vldimg_fix5); 
    }
}

function onSuccessProductImgNotLogIn(e) {
    var check_img = 0;
    try {
        FileName = e.response.newimage;
        pathName = e.response.pathName;
        //console.log("Filename="+FileName);
    }
    catch (err) {
    }
    if (CheckMaxUpload()) {
        var findImg = $("div.NoImg").eq(0).attr("id");
        var strID = findImg.split('_');
        var x = strID[1];
        var del = "";
        var img = "";
        var Img_s = $("#imgUpload").val();
        var Img_m = $("#imgUpload").attr('title');
        if (x == 0) {
            img = "<img class='img-polaroid' id='ProductImgPath_0' src='" + GetUpload(pathName + "/" + FileName) + "' onload='resizeImg($(this)," + Img_m + "," + Img_m + "); '>";
            $("#ProductImgPath_0").addClass("show_image").css("display", "block");
            $("#ProductImg_0").html(img).removeClass("NoImg");
            $("#ProductImgPath_0").val(FileName);
            del = "<i id='del_1' class='icon-remove-sign btn-tootip-top cursor hide' rel='tooltip'  title='ลบ' style='position:absolute;margin-left: " + (Img_s - 5) + "px;margin-top: -10px; display:block;' onclick='Del_image(1)'></i>";
            img = "<img id='ProductImgPath_1' class='ProImgpath img-data'  src='" + GetUpload(pathName + "/" + FileName) + "' img-name='" + FileName + "' onload='resizeImg($(this)," + Img_s + "," + Img_s + "); '>";
            $("#ProductImgPath_1").addClass("show_image").css("display", "block");
            $("#ProductImg_1").html(del + img).removeClass("NoImg");
            $("#ProductImgPath_1").val(FileName);
            $("#ProductImgPath").css('color', '#468847');
            $(".ProductImgPath .success").css('display', 'block');
            $(".ProductImgPath .error").css('display', 'none');
        }
        else {
            del = "<i id='del_" + x + "' class='icon-remove-sign btn-tootip-top cursor hide' rel='tooltip'  title='ลบ' style='position:absolute;margin-left: " + (Img_s - 5) + "px;margin-top: -10px; display:block;' onclick='Del_image(" + x + ")'></i>";
            img = "<img id='ProductImgPath_" + x + "' class='ProImgpath img-data' src='" + GetUpload(pathName + "/" + FileName) + "' img-data-id='" + $('.img-item').eq(x - 1).attr('data-id') + "' img-name='" + FileName + "' onload='resizeImg($(this)," + Img_s + "," + Img_s + "); '>";
            $("#ProductImgPath_" + x).addClass("show_image").css("display", "block");
            $("#ProductImg_" + x).html(del + img).removeClass("NoImg");
            $("#ProductImgPath_" + x).val(FileName);
        }
        SetFirstImage();
    } else {
        bootbox.alert(label.vldimg_fix5);
    }
}

function onUploadProductImg(e) {
    var files = e.files;
    OpenLoading(true);
    if (CheckMaxUpload()) {
        $.each(files, function () {
            if (this.extension != ".jpg" && this.extension != ".JPG" && this.extension != ".jpeg" && this.extension != ".JPEG" && this.extension != ".png" && this.extension != ".PNG") {
                bootbox.alert(label.vldfix_format_picture);
                e.preventDefault();
                OpenLoading(false);
                return false;
            }
        });
    } else {
        bootbox.alert(label.vldimg_fix5);
        OpenLoading(false);
    }
}
/*-----------ลบรูปภาพ-------*/
function Del_image(number) {
    bootbox.confirm(label.vldconfirm_del, function (result) {
        if (result)
        {
            var no_img = "<img id='ProductImgPath_" + number + "' src='http://www.placehold.it/200x200/EFEFEF/AAAAAA&text=no+image' />";

            $("#ProductImg_" + number).html(no_img).addClass("NoImg");
            $("#ProductImgPath_" + number).val("");
            SetFirstImage();
        }
    });
}
//console.log("ProductUpload");