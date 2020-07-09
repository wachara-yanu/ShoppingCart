var FileName = "";
var FileThumb = "";
var FileOld = "";
var EditImages = "";
var indexFile = 0;

var dirPath = "";

//ไม่ได้ใช้ และ ยังทำไม่เสร็จ 
(function ($) {

    $.fn.AttachFile = function (type) {
        return this.each(function () {
            var $this = $(this);
            console.log($this.attr('data-path'));

        });

    };
})(jQuery);

$('.btn-remove-file').click(function () {
    var index = $('.btn-remove-file').index(this);
    $('.hidFileUpload').eq(index).remove();
    $('.sign-upload').eq(index).remove();
});

function GenerateFileUpload(obj,filename,filesize){
var html =  '<span class="label label-info sign-upload">'+filename+'<i class="icon-remove icon-white btn-close-file"></i></span>';
    html += '<input type="hidden" name="hidFileUpload" value="'+filename+'" >';
    obj.append(html);

}

//------------------------CompImg-----------------------//
function onCompleteUpload(e) {
    $(".t-upload-files").remove();
    OpenLoading(false);
    if ($("#chkImg").val() != "") { 
        OpenLoading(false);
    } 
}

function onSuccessFileUpload(e) {
    OpenLoading(false);
    try {
        FileName = e.response.newimage;
    }
    catch (err) {
    }
    GenerateFileUpload(FileName);
   

}

// แนบไฟล์
function onUploadCreateMessage(e) {
    var files = e.files;
    OpenLoadingNew(true, $("body"));
    $.each(files, function () {
        if (this.extension != ".rar" && this.extension != ".RAR" && this.extension != ".zip" && this.extension != ".ZIP" && this.extension != ".jpg" && this.extension != ".JPG" &&
            this.extension != ".jpeg" && this.extension != ".JPEG" && this.extension != ".gif" && this.extension != ".GIF" && this.extension != ".png" && this.extension != ".PNG") {
            bootbox.alert(label.vldfix_format_formsregistersme);
            OpenLoadingNew(false);
            e.preventDefault();
            return false;
        }
    });
}

function onSuccessCreateMessage(e) {
    OpenLoadingNew(false);
    try {
        var IsAvatar = e.response.IsAvatar;
        var FilePath = e.response.newImgPath;
        var FileName = e.response.newimage;
        var OldFileName = e.response.oldFileName;
        var FileSize = e.response.fileSize;
        var noImgPath = (IsAvatar == true) ? 'Content/Default/Images/noavatar.gif' : GetUpload("Temp/MessageFile/" + getCookie("CompID") + "/" + FileName);
        var FullPath = (FileName == "" || FileName == null) ? url + noImgPath : GetUpload("Temp/MessageFile/" + getCookie("CompID") + "/" + FileName);

        var Index = e.response.Index;
        if (Index == undefined || Index == "") {
            Index = 0;
        }

        $(this).parent().parent().parent().find(".hidImgFileName").val(FileName);
        $(this).parent().parent().parent().find(".hidImgFilePath").val(FilePath);
        $(this).parent().parent().parent().find(".hidImgOldFileName").val(OldFileName);
        $(this).parent().parent().parent().find(".ShowImgFile").val(FileName);
        $(this).parent().parent().parent().find(".t-filename").html(FileName);
        $(this).parent().parent().parent().find(".iclose").removeClass("hidden");
        $(this).parent().parent().parent().find(".hidImgSize").val(FileSize);
        $(this).parent().parent().parent().find(".hidFileName").text(FileName);
        $("#hidFile").addClass("hide");

    } catch (err) { }
}

function OpenLoadingNew(isLoad, obj) {
    var mar_t = ($(window).height() / 2) - 50;
    var mar_l = ($(window).width() / 2) - 110;
    if (isLoad == true) {
        obj.prepend('<div id="loading"><div id="imgloading"><img src=\"' + GetUrl('Content/Default/images/icon-load.gif') + '\" ></div></div>')
        $("#loading").css({ 'filter': 'alpha(opacity=50)', 'opacity': '0.5' });
        $("#imgloading").css("margin", mar_t + "px 0 0 " + mar_l + "px")
    }
    else {
        $('#loading').remove(); $('#imgloading').remove()
    }
}
