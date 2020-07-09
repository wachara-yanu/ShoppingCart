
/*****************************************************************************************************************************/
function onSuccessPicture(e) {
    try {
        var UploadName = e.response.UploadName;
        var IsAvatar = e.response.IsAvatar;
        var FilePath = e.response.newImgPath;
        var FileName = e.response.newImgFile;
        var OldFileName = e.oldFileName;
        var noImgPath = (IsAvatar == true) ? 'Content/Default/Image/noavatar.gif' : 'Content/Default/Image/icon-nopicture.png';
        var FullPath = (FileName == "" || FileName == null) ? url + noImgPath : url + FilePath + FileName;

        var Index = e.response.Index;

        if (Index == undefined || Index == "") {
            Index = 0;
        }

        $("#hid" + UploadName + "FileName").val(FileName);
        $("#hid" + UploadName + "FilePath").val(FilePath);

        $("#hid" + UploadName + "OldFileName").val(OldFileName);
        $("#Show" + UploadName).attr("src", FullPath);
        $("#Show" + UploadName).attr("title", FileName);
        $("#Show" + UploadName).attr("alt", FileName);
        //        $(".t-filename" + UploadName).html(FileName);
        //        $(".iclose").find(UploadName).removeClass("hidden");
        $('.RemovePicture').removeClass("hidden");

    } catch (err) { }
}

function OnCompletePicture(e) {
    $(".t-file button").remove();
    $(".t-upload-files").remove();
}

$(".RemovePicture").live("click", function () {
    var index = $(".RemovePicture").index($(this));
    var UploadName = $(this).parent().find('.uploadsetting > .hidUpload').val();
    $.ajax({
        url: url + "Base/RemoveImages",
        dataType: 'json',
        traditional: true,
        data: { 'fileNames': $("#hid" + UploadName + "FileName").val(),
            'filePath': $("#hid" + UploadName + "FilePath").val(),
            'IsAvatar': $(".hidIsAvatar").eq(index).val(),
            'thumbSize': $(".hidThumbSize").eq(index).val()
        },
        success: function (e) {
            var IsAvatar = e.IsAvatar;
            var FileName = e.newImgFile;
            var OldFileName = e.oldFileName;
            var DeteleMedia = e.DeleteMedia;
            var noImgPath = (IsAvatar == true) ? 'Content/Default/Image/noavatar.gif' : 'Content/Default/Image/icon-nopicture.png';
            var FullPath = (FileName == "" || FileName == null) ? url + noImgPath : url + FilePath + FileName;

            $(".hidImgFileName").eq(index).val(FileName);
            $(".hidImgOldFileName").eq(index).val(OldFileName);
            $(".ShowImgFile").eq(index).attr("src", FullPath);
            $(".ShowImgFile").eq(index).attr("title", FileName);
            $(".ShowImgFile").eq(index).attr("alt", FileName);
            $(".t-filename").eq(index).html(FileName);
            $(".t-upload-files").eq(index).remove();
            $(".RemovePicture").eq(index).addClass("hidden");
        },
        type: 'POST'
    });
});

function onUploadPicture(e) {
    var fileType = $(this).parent().parent().find(".hidAllowFormat").val().replace(' ', '').split(',')
    var fileFormat = $(this).parent().parent().find(".hidAllowFormat").val();
    var files = e.files;
    $.each(files, function () {
        var exten = this.extension.toLowerCase();

        if (fileType.indexOf(exten) == -1) {
            bootbox.alert(label.vldfix_format_picture);
            $(".divExplanWord").remove();
            e.preventDefault();
            return false;
        }
        //        if (exten != ".jpg" && exten != ".jpeg" && exten != ".gif" && exten != ".png") {
        //            bootbox.alert("คุณสามารถเลือกได้เฉพาะไฟล์ .jpg .jpeg .gif .png เท่านั้นค่ะ");
        //            
        //        }
    });
}


function onSuccessVideo(e) {
    try {
        var IsAvatar = e.response.IsAvatar;
        var FilePath = e.response.newVideoPath;
        var FileName = e.response.newVideoFile;
        var FileThumbName = e.response.FileThumbName;
        var noImgPath = (IsAvatar == true) ? 'Content/Default/Images/noavatar.gif' : 'Content/Default/Images/icon-nopicture.png';
        var FullPath = (FileName == "" || FileName == null) ? url + noImgPath : url + FilePath + FileThumbName;

        var Index = e.response.Index;
        if (Index == undefined || Index == "") {
            Index = 0;
        }
        $(".hidVideoFileName").eq(Index).val(FileName);
        $(".hidVideoThumbFileName").eq(Index).val(FileThumbName);
        $(".hidVideoFilePath").eq(Index).val(FilePath);
        $(".ShowVideoFile").eq(Index).attr("src", FullPath);
        $(".ShowVideoFile").eq(Index).attr("title", FileName);
        $(".ShowVideoFile").eq(Index).attr("alt", FileName);
        $(".hidVideoOldFileName").eq(Index).val(OldFileName);
        $(".t-filename").eq(Index).html(FileName);
        $(".iclose").eq(Index).removeClass("hidden");

    } catch (err) { }
}

function OnCompleteVideo(e) {
    $(".t-file button").remove();
    $(".t-upload-files").remove();
}

$(".RemoveVideo").live("click", function () {
    var index = $(".RemoveVideo").index($(this));
    $.ajax({
        url: url + "Default/RemoveVideo",
        dataType: 'json',
        traditional: true,
        data: { 'fileNames': $(".hidImgFileName").eq(index).val(),
            'IsAvatar': $(".hidIsAvatar").eq(index).val(),
            'thumbSize': $(".hidThumbSize").eq(index).val()
        },
        success: function (e) {
            var IsAvatar = e.IsAvatar;
            var FilePath = e.newVideoPath;
            var FileName = e.newVideoFile;
            var FileThumb = e.newVideoThumb;
            var OldFileName = e.oldFileName;
            var noImgPath = (IsAvatar == true) ? 'Content/Default/Images/noavatar.gif' : 'Content/Default/Images/icon-nopicture.png';
            var FullPath = (FileName == "" || FileName == null) ? url + noImgPath : url + FilePath + FileName;

            $(".hidVideoFileName").eq(index).val(FileName);
            $(".hidVideoFilePath").eq(index).val(FilePath);
            $(".hidVideoFileThumbName").eq(index).val(FilePath);
            $(".hidVideoOldFileName").eq(index).val(OldFileName);
            $(".ShowVideoFile").eq(index).attr("src", FullPath);
            $(".ShowVideoFile").eq(index).attr("title", FileName);
            $(".ShowVideoFile").eq(index).attr("alt", FileName);
            $(".t-filename").eq(index).html(FileName);
            $(".t-upload-files").eq(index).remove();
            $(".RemoveVideo").eq(index).addClass("hidden");
        },
        type: 'POST'
    });
});

function onUploadVideo(e) {
    var fileType = $(this).parent().parent().find(".hidAllowFormat").val().replace(' ', '').split(',');
    var fileFormat = $(this).parent().parent().find(".hidAllowFormat").val();
    var files = e.files;
    $.each(files, function () {
        var exten = this.extension.toLowerCase();

        if (fileType.indexOf(exten) == -1) {
            bootbox.alert(label.vldfix_format_picture);
            $(".divExplanWord").remove();
            e.preventDefault();
            return false;
        } else {
            $(".hidFileTypeUpload").val(exten);
        }
        //        if (exten != ".jpg" && exten != ".jpeg" && exten != ".gif" && exten != ".png") {
        //            bootbox.alert("คุณสามารถเลือกได้เฉพาะไฟล์ .jpg .jpeg .gif .png เท่านั้นค่ะ");
        //            
        //        }
    });
}


function onSuccessFlash(e) {
    try {
        var FilePath = e.response.newFlashPath;
        var FileName = e.response.newFlashFile;
        var noImgPath = 'Content/Default/Images/icon-nopicture.png';
        var FullPath = (FileName == "" || FileName == null) ? url + noImgPath : url + FilePath + FileName;

        var Index = e.response.Index;
        if (Index == undefined || Index == "") {
            Index = 0;
        }
        $(".hidFlashFileName").eq(Index).val(FileName);
        $(".hidFlashFilePath").eq(Index).val(FilePath);
        $(".ShowVideoFile").eq(Index).find(".shockwaveObj param[name='movie']").attr('value', '../' + FilePath + FileName);
        $(".ShowVideoFile").eq(Index).find(".shockwaveObjNOTIE").attr('data', '../' + FilePath + FileName);
        $(".t-filename").eq(Index).html(FileName);
        $(".iclose").eq(Index).removeClass("hidden");

    } catch (err) { bootbox.alert(err) }
}

function OnCompleteFlash(e) {
    $(".t-file button").remove();
    $(".t-upload-files").remove();
}

$(".RemoveFlash").live("click", function () {
    var index = $(".RemoveFlash").index($(this));
    $.ajax({
        url: url + "Default/RemoveFlash",
        dataType: 'json',
        traditional: true,
        data: { 'fileNames': $(".hidFlashFileName").eq(index).val(),
            'filePath': $(".hidFlashFilePath").eq(index).val()
        },
        success: function (e) {
            var FilePath = e.newFlashPath;
            var FileName = e.newFlashFile;
            var noImgPath = '../Content/Default/Images/icon-nopicture.png';
            var FullPath = (FileName == "" || FileName == null) ? url + noImgPath : url + FilePath + FileName;

            $(".hidFlashFileName").eq(index).val(FileName);
            $(".hidFlashFilePath").eq(index).val(FilePath);
            $(".ShowVideoFile").eq(index).find(".shockwaveObjNOTIE").attr('data', noImgPath);
            $(".ShowVideoFile").eq(index).find(".shockwaveObj param[name='movie']").attr('value', noImgPath);

            $(".t-filename").eq(index).html(FileName);
            $(".t-upload-files").eq(index).remove();
            $(".RemoveFlash").eq(index).addClass("hidden");
        },
        type: 'POST'
    });
});

function onUploadFlash(e) {
    var _this = $(this);
    var fileType = $(this).parent().parent().find(".hidAllowFormat").val().replace(' ', '').split(',');
    var fileFormat = $(this).parent().parent().find(".hidAllowFormat").val();
    var files = e.files;
    $.each(files, function () {
        var exten = this.extension.toLowerCase();

        if (fileType.indexOf(exten) == -1) {
            bootbox.alert(label.vldfix_format_picture);
            $(".divExplanWord").remove();
            e.preventDefault();
            return false;
        } else {
            $(".hidFileTypeUpload").val(exten);
            _this.parent().parent().find(".iclose").removeClass("hidden");
        }
        //        if (exten != ".jpg" && exten != ".jpeg" && exten != ".gif" && exten != ".png") {
        //            bootbox.alert("คุณสามารถเลือกได้เฉพาะไฟล์ .jpg .jpeg .gif .png เท่านั้นค่ะ");
        //            
        //        }
    });
}


function onSuccessFile(e) {
    try {
        var IsAvatar = e.response.IsAvatar;
        var FilePath = e.response.newImgPath;
        var FileName = e.response.newImgFile;
        var OldFileName = e.response.oldFileName;
        var FileSize = e.response.fileSize;
        var noImgPath = (IsAvatar == true) ? 'Content/Default/Images/noavatar.gif' : 'Content/Default/Images/icon-nopicture.gif';
        var FullPath = (FileName == "" || FileName == null) ? url + noImgPath : url + FilePath + FileName;

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

    } catch (err) { }
}

function OnCompleteFile(e) {
    $(".t-file button").remove();
    $(".t-upload-files").remove();
}

$(".RemoveFile").live("click", function () {

    var index = $(".RemoveFile").index($(this));
    console.log(index);
    $.ajax({
        url: url + "Base/RemoveFiles",
        dataType: 'json',
        traditional: true,
        data: { 'fileNames': $(this).parent().parent().parent().find(".hidImgFileName").val(),
            'filePath': $(this).parent().parent().parent().find(".hidImgFilePath").val(),
            'IsAvatar': $(this).parent().parent().parent().find(".hidIsAvatar").val(),
            'thumbSize': $(this).parent().parent().parent().find(".hidThumbSize").val()
        },
        success: function (e) {
            var IsAvatar = e.IsAvatar;
            var FilePath = e.newImgPath;
            var FileName = e.newImgFile;
            var OldFileName = e.oldFileName;
            var DeteleMedia = e.DeleteMedia;
            var noImgPath = (IsAvatar == true) ? 'Content/Default/Images/noavatar.gif' : 'Content/Default/Images/icon-nopicture.gif';
            var FullPath = (FileName == "" || FileName == null) ? url + noImgPath : url + FilePath + FileName;

            $(".hidImgFileName").eq(index).val(FileName);
            $(".hidImgFilePath").eq(index).val(FilePath);
            $(".hidImgOldFileName").eq(index).val(OldFileName);
            $(".ShowImgFile").eq(index).val(FileName);
            $(".t-filename").eq(index).html(FileName);
            $(".t-upload-files").eq(index).remove();
            $(".RemoveFile").eq(index).addClass("hidden");

        },
        type: 'POST'
    });
});

function onUploadFile(e) {
    var fileType = $(this).parent().parent().parent().find(".hidAllowFormat").val().replace(' ', '').split(',')
    var fileFormat = $(this).parent().parent().parent().find(".hidAllowFormat").val();
    var fileSize = $(this).parent().parent().parent().find(".hidMaxFileSize").val();
    var files = e.files;

    $.each(files, function () {
        var exten = this.extension.toLowerCase();
        if (fileFormat.replace(" ", "") == ".*") {
            if (files[0].size / 1024 / 1024 > fileSize == false) {
                return false;
            } else {
                bootbox.alert("more then 5 mb.");
                e.preventDefault();
                return true;
            }
        }
        if (fileType.indexOf(exten) == -1 && fileFormat.replace(" ", "") != ".*") {
            bootbox.alert(label.vldfix_format_picture);
            $(".divExplanWord").remove();
            e.preventDefault();
            return false;
        }
    });
}


