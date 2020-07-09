var FileName = "";
var FileThumb = "";
var FileOld = "";
var EditImages = "";
var indexFile = 0;
//------------------------Logo-----------------------//

function onSuccessSlip(e) {
    try {
        FileName = e.response.newimage;
        CompID = e.response.compid;
        $("#SlipImgPath").val(FileName);
        $("#SlipImg").html("<img id='img_SlipPath' src='" + GetUpload("Temp/Companies/Slip/" + CompID + "/" + FileName) + "'>");
        $(".t-upload-files").remove();

    } catch (err) {
    }
}

function onUploadSlip(e) {
    var files = e.files;
    $.each(files, function () {
        if (this.extension != ".jpg" && this.extension != ".JPG" && this.extension != ".jpeg" && this.extension != ".JPEG" && this.extension != ".gif" && this.extension != ".GIF" && this.extension != ".png" && this.extension != ".PNG") {
            bootbox.alert(label.vldfix_format_picture);
            e.preventDefault();
            return false;
        }
    });
}