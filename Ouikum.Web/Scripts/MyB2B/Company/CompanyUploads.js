var FileName = "";
var FileThumb = "";
var FileOld = "";
var EditImages = "";
var indexFile = 0;
//------------------------Logo-----------------------//

function onSuccessLogo(e) {
    OpenLoading(false);
    try {
        FileName = e.response.newimage;
        $("#LogoImgPath").val(FileName);
        var del = "<i id='del_Logo' class='icon-remove-sign btn-tootip-top cursor hide' rel='tooltip'  title='ลบ' style='position:absolute;margin-left:95px;margin-top:-10px; display:block;' onclick='Del_Logoimage();'></i>";
        var img = "<img id='img_LogoImgPath' src='" + GetUpload("Temp/Companies/Logo/" + getCookie("CompID") + "/" + FileName) + "' style='width: 100px;height: 75px;'>";
        $("#ImgLogo").html(del + img);
        $(".t-upload-files").remove();

    } catch (err) {
    }
}

//------------------------CompImg-----------------------//
function onComplateCompImg(e) {
    $(".t-upload-files").remove();
    OpenLoading(false);
    if ($("#chkImg").val() != "") {
        bootbox.alert(label.vldmaximg_3);
        OpenLoading(false);
    } 
}

function onSuccessCompImg(e) {
    OpenLoading(false);
    try {
        FileName = e.response.newimage;
    }
    catch (err) {
    }
    var findImg = $("div.NoImg").eq(0).attr("id");
    if (findImg != null && findImg != "") {
        var strID = findImg.split('_');
        var x = strID[1];
        var del = "<i id='del_" + x + "' class='icon-remove-sign btn-tootip-top cursor hide' rel='tooltip'  title='ลบ' style='position:absolute;margin-left: 175px;margin-top: -10px; display:block;' onclick='Del_image(" + x + ")'></i>";
        var img = "<img id='img_CompImgPath_" + x + "' src='" + GetUpload("Temp/Companies/Image/" + getCookie("CompID") + "/" + FileName) + "' style='height: 120px;width: 180px;'>";
        $("#img_CompImgPath_" + x).addClass("show_image").css("display", "block");
        $("#CompImg_" + x).html(del + img).removeClass("NoImg");
        $("#CompImgPath_" + x).val(FileName);
    } else{
        $("#chkImg").val(1);
    }
}
/*-----------------------------Contact-----------------------------------*/

function onSuccessContImg(e) {
    OpenLoading(false);
    try {
        FileName = e.response.newimage;
        $("#ContactImgPath").val(FileName);
        var del = "<i id='del_ContImg' class='icon-remove-sign btn-tootip-top cursor hide' rel='tooltip'  title='ลบ' style='position:absolute;margin-left:95px;margin-top:-10px; display:block;' onclick='Del_ContImg();'></i>";
        var img = "<img id='img_ContImgPath' src='" + GetUpload("Temp/Companies/Contact/" + getCookie("CompID") + "/" + FileName) + "' style='height: 75px;width: 100px;'>";
        $("#ImgCont").html(del + img);
        $(".t-upload-files").remove();

    } catch (err) {
    }
}
/*-----------------------------Certify-----------------------------------*/

function onSuccessCertifyImg(e) {
    OpenLoading(false);
    try {
        FileName = e.response.newimage;
        $("#CertifyImgPath").val(FileName);
        if ($("#CertifyImgPath").val() == "") {
            $("#submit").attr('disabled', true);
        } else {
            $("#submit").attr('disabled', false);
        }
        $("#show_CertifyImgPath").html("<img id='img_CertifyImgPath' src='" + GetUpload("Temp/CompanyCertify/" + getCookie("CompID") + "/" + FileName) + "' style='height: 120px;width: 180px;'>");
        $(".t-upload-files").remove();

    } catch (err) {
    }
}

/*------------------------------Map--------------------------------------------*/

function onSuccessMap(e) {
    OpenLoading(false);
    try {
        FileName = e.response.newimage;
        $("#MapImgPath").val(FileName);
        var del = "<i id='del_MapImgPath' class='icon-remove-sign btn-tootip-top cursor hide' rel='tooltip'  title='ลบ' style='position:absolute;margin-left:497px;margin-top:-12px; display:block;' onclick='Del_MapImgPath();'></i>";
        var img = "<img id='img_MapImgPath' src='" + GetUpload("Temp/Companies/Map/" + getCookie("CompID") + "/" + FileName) + "' style='height: 350px;width: 500px;'>";
        $("#ImgMap").html(del + img);
        $(".t-upload-files").remove();
    } catch (err) {
    }
}

/*------------------------------Blog--------------------------------------------*/

function onSuccessBlog(e) {
    OpenLoading(false);
    try {
        FileName = e.response.newimage;
        $("#ImgPath").val(FileName);
        if ($("#ImgPath").val() == "") {
            $("#submit").attr('disabled', true);
        } else {
            $("#submit").attr('disabled', false);
        }
        $("#ImgBlog").html("<img id='img_ImgPath' src='" + GetUpload("Temp/Article/" + getCookie("CompID") + "/" + FileName) + "' style='height: 120px;width: 180px;'>");
        $(".t-upload-files").remove();
    } catch (err) {
    }
}
//------------------------onUploadImg-----------------------//
function onUploadImg(e) {
    var files = e.files;
    OpenLoading(true);
    $.each(files, function () {
        if (this.extension != ".jpg" && this.extension != ".JPG" && this.extension != ".jpeg" && this.extension != ".JPEG" && this.extension != ".gif" && this.extension != ".GIF"
           && this.extension != ".png" && this.extension != ".PNG" && this.extension != ".bmp" && this.extension != ".BMP" && this.extension != ".tiff" && this.extension != ".TIFF"
           && this.extension != ".jiff" && this.extension != ".JIFF" && this.extension != ".jpf" && this.extension != ".JPF" && this.extension != ".jpx" && this.extension != ".JPX"
           && this.extension != ".jp2" && this.extension != ".JP2" && this.extension != ".j2c" && this.extension != ".J2C" && this.extension != ".j2k" && this.extension != ".J2K"
           && this.extension != ".jpc" && this.extension != ".JPC") {
            bootbox.alert(label.vldfix_format_picture);
            OpenLoading(false);
            e.preventDefault();
            return false;
        }
    });
}