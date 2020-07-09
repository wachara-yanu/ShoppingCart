var FileName = "";
var FileThumb = "";
var FileOld = "";
var EditImages = "";
var indexFile = 0;

/*------------------------------Blog--------------------------------------------*/

function onSuccessBanner(e) {
    OpenLoading(false);
    try {
        FileName = e.response.newimage;
        $("#ImgPath").val(FileName);
        if ($("#ImgPath").val() == "") {
            $("#submit").attr('disabled', true);
        } else {
            $("#submit").attr('disabled', false);
        }
        $("#ImgBlog").html("<img id='img_ImgPath' src='" + GetUpload("Temp/Banner/" + "BannerHome/" + FileName) + "' style='height: 120px;width: 258px;'>");
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