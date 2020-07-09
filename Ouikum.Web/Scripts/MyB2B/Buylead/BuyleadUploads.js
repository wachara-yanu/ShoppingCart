var FileName = "";
var FileThumb = "";
var FileOld = "";
var EditImages = "";
var indexFile = 0;


//------------------------BuyleadImg-----------------------//

function onSuccessBuyleadImg(e) {
    try {
        FileName = e.response.newimage;
        del = "<i id='del' class='icon-remove-sign btn-tootip-top cursor hide' rel='tooltip'  title='Delete' style='position:absolute;margin-left: 178px;margin-top: -3px; display:block;' onclick='Del_image()'></i>";
       img = "<img class='img-polaroid' id='BuyleadImgPath_0' src='" + GetUpload("Temp/Buylead/" + getCookie("CompID") + "/" + FileName) + "' onload='resizeImg($(this),180,180);' img-name='" + FileName+"'>";
            $("#BuyleadImgPath_0").addClass("show_image").css("display", "block");
            $("#BuyleadImg_0").html(del+img).removeClass("NoImg");
            $("#BuyleadImgPath_0").val(FileName);
        $(".t-upload-files").remove();
        $("#BuyleadImgPath").css('color', '#468847');
        $(".BuyleadImgPath .success").css('display', 'block');
        $(".BuyleadImgPath .error").css('display', 'none');
    } catch (err) {
    }
}

function onUploadBuyleadImg(e) {
    var files = e.files;
    $.each(files, function () {
        if (this.extension != ".jpg" && this.extension != ".JPG" && this.extension != ".jpeg" && this.extension != ".JPEG" && this.extension != ".gif" && this.extension != ".GIF" && this.extension != ".png" && this.extension != ".PNG") {
            bootbox.alert(label.vldfix_format_picture);
            e.preventDefault();
            return false;
        }
    });
}
function Del_image() {
    if (confirm("Confirm Delete Item")) {
        var no_img = "<img class='img-polaroid' id='BuyleadImgPath_0' src='http://www.placehold.it/180x180/EFEFEF/AAAAAA&text=no+image' />";
        $("#BuyleadImg_0").html(no_img).addClass("NoImg");
        $("#BuyleadImgPath_0").val("");
        $("#BuyleadCenterImg").css('color', '#333333');
        $("#BuyleadCenterImg .success").css('display', 'none');
        $("#BuyleadCenterImg .error").css('display', 'none');
    }
}