var FileName = "";
var FileThumb = "";
var FileOld = "";
var EditImages = "";
var indexFile = 0;
//------------------------AvatarImg-----------------------//

function onSuccessAvatarImg(e) {
    try {
        FileName = e.response.newimage;
        $("#AvatarImgPath").val(FileName);
        var del = "<i id='del_AvatarImg' class='icon-remove-sign btn-tootip-top cursor hide' rel='tooltip'  title='ลบ' style='position:absolute;margin-left:73px;margin-top:-10px; display:block;' onclick='Del_AvatarImg();'></i>";
        var img = "<img id='img_AvatarImg' src='" + GetUpload("Temp/Members/" + getCookie("MemberID") + "/" + FileName) + "'  style='height: 75px;width: 75px;'>";
        $("#AvatarImg").html(del + img);
        $(".t-upload-files").remove();
        OpenLoading(false);

    } catch (err) {
    }
}
function onUploadAvatarImg(e) {
    var files = e.files;
    OpenLoading(true);
    $.each(files, function () {
        if (this.extension != ".jpg" && this.extension != ".JPG" && this.extension != ".jpeg" && this.extension != ".JPEG" && this.extension != ".gif" && this.extension != ".GIF" && this.extension != ".png" && this.extension != ".PNG") {
            bootbox.alert(label.vldfix_format_picture);
            OpenLoading(false);
            e.preventDefault();
            return false;
        }
    });
} 
