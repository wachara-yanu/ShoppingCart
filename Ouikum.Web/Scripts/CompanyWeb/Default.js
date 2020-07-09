

//Loading
function OpenLoading(isLoad, img, obj) {
    if (isLoad == false) {
        $('#loading').remove();
        $('#imgloading').remove();
    } else {
        if (img == null) {
            img = '<img id=\"imgloading\" src=\"' + GetUrl("Content/Default/Image/Loading.gif") + '\" >';
        } else {
            img = img;
        }
        if (obj == null || obj == undefined) {
            $('body').prepend('<div id="loading">&nbsp;</div><div id="imgloading">' + img + '</div>');
        } else {
            obj.prepend('<center><div id="loading">&nbsp;</div><div id="imgloading">' + img + '</div></center>');
        }
        var h = ($(window).height() - 336) / 2;
        var w = ($(window).width() - 356) / 2;
        $("#loading").css({ 'filter': 'alpha(opacity=50)', 'opacity': '0.5' });
        $("#imgloading").css({ 'left': w, 'top': h });
        //        .position({
        //            my: "center",
        //            at: "center",
        //            of: "#loading"
        //        });
    }

}
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

        var obj = $("#hid" + UploadName + "FileName").parents('.control-group').next();
        if ($(obj).find('.result').length >= 1) {
            $("#hid" + UploadName + "FileName").parents('.control-group').removeClass('error');
            $(obj).addClass('hide');
        }
        var lengthFileName = $(".hidImgFileName").length;
        for (var i = 0; i < lengthFileName; i++) {
            var length = $(".hidImgFileName:eq(" + i + ")").parents(".TelerikUploadArea").find(".iclose").length;
            if (length > 1)
                $(".iclose:eq(1)").remove();
        }


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
        cache: false,
        url: url + "Base/RemoveImages",
        dataType: 'json',
        traditional: true,
        data: {
            'fileNames': $("#hid" + UploadName + "FileName").val(),
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

            var obj = $(".hidImgFileName").eq(index).parents('.control-group').next();
            if ($(obj).find('.result').length >= 1) {
                $(".hidImgFileName").eq(index).parents('.control-group').removeClass('success').addClass('error');
                $(".hidImgFileName").eq(index).parents('.control-group').next().removeClass('success').addClass('error').removeClass('hide');
                $(obj).find('.result').html("<span class='fontRed2'>" + valid.lblPlzImg + "</span>").show();
            }
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
            bootbox.alert(valid.lblCanSelFile + " " + fileFormat + " " + valid.lblOnly);
            $(".divExplanWord").remove();
            e.preventDefault();
            return false;
        }
        //        if (exten != ".jpg" && exten != ".jpeg" && exten != ".gif" && exten != ".png") {
        //            alert("คุณสามารถเลือกได้เฉพาะไฟล์ .jpg .jpeg .gif .png เท่านั้นค่ะ");
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
        cache: false,
        url: url + "Default/RemoveVideo",
        dataType: 'json',
        traditional: true,
        data: {
            'fileNames': $(".hidImgFileName").eq(index).val(),
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
            bootbox.alert(valid.lblCanSelFile + " " + fileFormat + " " + valid.lblOnly);
            $(".divExplanWord").remove();
            e.preventDefault();
            return false;
        } else {
            $(".hidFileTypeUpload").val(exten);
        }
        //        if (exten != ".jpg" && exten != ".jpeg" && exten != ".gif" && exten != ".png") {
        //            alert("คุณสามารถเลือกได้เฉพาะไฟล์ .jpg .jpeg .gif .png เท่านั้นค่ะ");
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

    } catch (err) { alert(err) }
}

function OnCompleteFlash(e) {
    $(".t-file button").remove();
    $(".t-upload-files").remove();
}

$(".RemoveFlash").live("click", function () {
    var index = $(".RemoveFlash").index($(this));
    $.ajax({
        cache: false,
        url: url + "Default/RemoveFlash",
        dataType: 'json',
        traditional: true,
        data: {
            'fileNames': $(".hidFlashFileName").eq(index).val(),
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
            alert(valid.lblCanSelFile + " " + fileFormat + " " + valid.lblOnly);
            $(".divExplanWord").remove();
            e.preventDefault();
            return false;
        } else {
            $(".hidFileTypeUpload").val(exten);
            _this.parent().parent().find(".iclose").removeClass("hidden");
        }
        //        if (exten != ".jpg" && exten != ".jpeg" && exten != ".gif" && exten != ".png") {
        //            alert("คุณสามารถเลือกได้เฉพาะไฟล์ .jpg .jpeg .gif .png เท่านั้นค่ะ");
        //            
        //        }
    });
}


function onSuccessFile(e) {
    try {

        var IsAvatar = e.response.IsAvatar;
        var FilePath = e.response.newImgPath;
        var FileName = e.response.newImgFile;
        var OldFileName = e.oldFileName;
        var fileSize = e.response.fileSize;
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
        $(this).parent().parent().parent().find(".hidImgSize").val(fileSize);

        $(".ShowImgFile").css("border", "1px solid #ccc ");
        $("#ErrShowImgFile").css("display", "none");

    } catch (err) { }
}

function OnCompleteFile(e) {
    $(".t-file button").remove();
    $(".t-upload-files").remove();
}

$(".RemoveFile").live("click", function () {

    var index = $(".RemoveFile").index($(this));
    //console.log(index);
    $.ajax({
        url: url + "Base/RemoveFiles",
        dataType: 'json',
        traditional: true,
        data: {
            'fileNames': $(this).parent().parent().parent().find(".hidImgFileName").val(),
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
    var files = e.files;
    $.each(files, function () {
        var exten = this.extension.toLowerCase();

        if (fileType.indexOf(exten) == -1) {
            bootbox.alert(valid.lblCanSelFile + " " + fileFormat + " " + valid.lblOnly);
            $(".divExplanWord").remove();
            e.preventDefault();
            return false;
        }
        //        if (exten != ".jpg" && exten != ".jpeg" && exten != ".gif" && exten != ".png") {
        //            alert("คุณสามารถเลือกได้เฉพาะไฟล์ .jpg .jpeg .gif .png เท่านั้นค่ะ");
        //            
        //        }
    });
}

function onSelect(e) {
}

/*---------------------------GetDistrictByProvince----------------------------*/
function GetDistrictByProvince(p_id, d_id, id) {
    var ListDistricts = "<option value='0'>-----" + valid.lblSelDist + "-----</option>";
    $.ajax({
        cache: false,
        url: GetUrl("Address/GetDistrictByProvinceID"),
        data: { pid: p_id },
        type: "POST",
        dataType: 'json',
        success: function (data) {
            if (data != null) {
                if (data.IsResult) {
                    for (var index = 0; index < data.Districts.length; index++) {
                        if (d_id == parseInt(data.Districts[index].DistrictID)) {

                            ListDistricts += "<option value=" + data.Districts[index].DistrictID + " selected='selected'>" + data.Districts[index].DistrictName + "</option>";

                        } else {

                            ListDistricts += "<option value=" + data.Districts[index].DistrictID + ">" + data.Districts[index].DistrictName + "</option>";


                        }
                    }
                    $("#" + id).html(ListDistricts);
                } else {
                    $("#" + id).html(ListDistricts);
                }
            }
        },
        error: function () {
            bootbox.alert(valid.lblErr + " " + valid.lblCantCheck);
        }
    });
}
/*---------------------------GetDistrict----------------------------*/
function GetDistrict(d_id, id) {
    var ListDistricts = "";
    $.ajax({
        cache: false,
        url: GetUrl("Address/GetDistrict"),
        data: { d_id: d_id },
        type: "POST",
        dataType: 'json',
        success: function (data) {
            if (data != null) {
                ListDistricts = "<option value=" + data.DistrictID + " selected='selected'>" + data.DistrictName + "</option>";
                $("#" + id).html(ListDistricts);
            }
        },
        error: function () {
            bootbox.alert(valid.lblErr + " " + valid.lblCantCheck);
        }
    });
}
/*---------------------------GetProvince----------------------------*/
function GetProvince(p_id, id) {
    var ListProvinces = "";
    $.ajax({
        cache: false,
        url: GetUrl("Address/GetProvince"),
        data: { p_id: p_id },
        type: "POST",
        dataType: 'json',
        success: function (data) {
            if (data != null) {
                ListProvinces = "<option value=" + data.ProvinceID + " selected='selected'>" + data.ProvinceName + "</option>";
                $("#" + id).html(ListProvinces);
            }
        },
        error: function () {
            bootbox.alert(valid.lblErr + " " + valid.lblCantCheck);
        }
    });
}

/*---------------------------ListProvince----------------------------*/
function ListProvince(p_id, id) {

    var ListProvinces = "";
    if (p_id == 0) {
        ListProvinces = "<option value='0'>-----" + valid.lblSelProvince + "-----</option>";
    }
    $.ajax({
        cache: false,
        url: GetUrl("Address/ListProvince"),
        //data: { p_id: p_id },
        type: "POST",
        dataType: 'json',
        success: function (data) {
            if (data != null) {
                for (var index = 0; index < data.Provinces.length; index++) {
                    if (p_id == parseInt(data.Provinces[index].ProvinceID)) {
                        ListProvinces += "<option value=" + data.Provinces[index].ProvinceID + " selected='selected'>" + data.Provinces[index].ProvinceName + "</option>";
                    } else {
                        ListProvinces += "<option value=" + data.Provinces[index].ProvinceID + ">" + data.Provinces[index].ProvinceName + "</option>";
                    }
                }
                $("#" + id).html(ListProvinces);
            } else {
                $("#" + id).html(ListProvinces);
            }
        },
        error: function () {
            bootbox.alert(valid.lblErr + " " + valid.lblCantCheck);
        }
    });
}
/*---------------------------GetBiztype----------------------------*/
function GetBiztype(b_id, id) {
    var ListBiztype = "";
    $.ajax({
        cache: false,
        url: GetUrl("BizType/GetBizType"),
        data: { b_id: b_id },
        type: "POST",
        dataType: 'json',
        success: function (data) {
            if (data != null) {
                ListBiztype = "<option value=" + data.BizTypeID + " selected='selected'>" + data.BizTypeName + "</option>";
                $("#" + id).html(ListBiztype);
            }
        },
        error: function () {
            bootbox.alert(valid.lblErr + " " + valid.lblCantCheck);
        }
    });
}
/*---------------------------ListBiztype----------------------------*/
function ListBiztype(b_id, id) {
    var ListBiztypes = "";
    $.ajax({
        cache: false,
        url: GetUrl("Biztype/ListBiztype"),
        type: "POST",
        dataType: 'json',
        success: function (data) {
            if (data != null) {
                for (var index = 0; index < data.BizTypes.length; index++) {
                    if (b_id == parseInt(data.BizTypes[index].BizTypeID)) {
                        ListBiztypes += "<option value=" + data.BizTypes[index].BizTypeID + " selected='selected'>" + data.BizTypes[index].BizTypeName + "</option>";
                    } else {
                        ListBiztypes += "<option value=" + data.BizTypes[index].BizTypeID + ">" + data.BizTypes[index].BizTypeName + "</option>";
                    }
                }
                $("#" + id).html(ListBiztypes);
            } else {
                $("#" + id).html(ListBiztypes);
            }
        },
        error: function () {
            bootbox.alert(valid.lblErr + " " + valid.lblCantCheck);
        }
    });
}
/*---------------------------ListBiztype----------------------------*/
function ListCompBizType(b_id, id) {
    var ListCompBizTypes = "";
    $.ajax({
        cache: false,
        url: GetUrl("CompBizType/ListCompCompBizType"),
        type: "POST",
        dataType: 'json',
        success: function (data) {
            if (data != null) {
                for (var index = 0; index < data.CompCompBizTypes.length; index++) {
                    if (b_id == parseInt(data.CompBizTypes[index].Value)) {
                        ListCompBizTypes += "<option value=" + data.CompBizTypes[index].Value + " selected='selected'>" + data.CompBizTypes[index].Text + "</option>";
                    } else {
                        ListCompBizTypes += "<option value=" + data.CompBizTypes[index].Value + ">" + data.CompBizTypes[index].Text + "</option>";
                    }
                }
                $("#" + id).html(ListCompBizTypes);
            } else {
                $("#" + id).html(ListCompBizTypes);
            }
        },
        error: function () {
        }
    });
}
/*********************************CheckNum************************************************/


function CheckNum(event) {
    var charCode = (typeof event.which == "number") ? event.which : event.keyCode;
    if (charCode >= 48 && charCode <= 57 || (charCode == 8 || charCode == 0 || charCode == 13)) { } else { event.preventDefault(); }
}
function CheckNumAndDash() {
    if (event.keyCode < 48 && event.keyCode != 45 || event.keyCode > 57) {
        event.returnValue = false;
    }
}

function CheckNumNotNull() {
    if (event.keyCode < 48 || event.keyCode > 57) {
        event.returnValue = false;
    }
    else {
        $(".btnSaveList").removeClass("hide");
    }
}
function checkString(string) {
    if (string != "") {
        return string;
    }
    else {
        return valid.lblNA;
    }
}
function validEmail(v) {
    var r = new RegExp("[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?");
    return (v.match(r) == null) ? false : true;
}

/*********************************resizeImg********************************/
function resizeImg(el, maxW, maxH) {
    var oEl = el;
    var maxWidth = maxW; // Max width for the image
    var maxHeight = maxH;   // Max height for the image
    oEl.css("width", "auto").css("height", "auto"); // Remove existing CSS
    oEl.removeAttr("width").removeAttr("height"); // Remove HTML attributes
    var width = oEl.width();    // Current image width
    var height = oEl.height();  // Current image height

    if (width > maxWidth && height <= maxHeight) {
        //console.log('maxW');
        var ratio = maxWidth / width;
        oEl.css("width", maxWidth);    // Scale width based on ratio 
        oEl.css("height", height * ratio);   // Set new height

    } else if (width <= maxWidth && height > maxHeight) {
        //console.log('maxH');
        var ratio = maxHeight / height;
        oEl.css("width", width * ratio);    // Scale width based on ratio 
        oEl.css("height", maxHeight);   // Set new height

    } else if (width > maxWidth && height > maxHeight) {
        //console.log('maxW & H');
        if (width > height) {
            //console.log('w > h');
            var ratio = maxWidth / width;
            oEl.css("width", maxWidth);    // Scale width based on ratio 
            oEl.css("height", height * ratio);   // Set new height
        } else if (height > width) {
            //console.log('h > w');
            var ratio = maxHeight / height;
            oEl.css("width", width * ratio);    // Scale width based on ratio 
            oEl.css("height", maxHeight);   // Set new height

        } else {
            //console.log('h = w');
            oEl.css("width", maxWidth);    // Scale width based on ratio 
            oEl.css("height", maxHeight);   // Set new height
        }

    } else {
        oEl.css("width", width);    // Scale width based on ratio 
        oEl.css("height", height);   // Set new height 
    }
}

/*********************************setElementMiddle********************************/
function setElementMiddle(h, w, el) {
    var oEl = el;
    if (oEl) {
        var iHeight = oEl.height();
        var iWidth = oEl.width();

        if (iHeight < h) {
            oEl.css('margin-top', Math.floor((h - iHeight) / 2) + 'px');
        }

        if (iWidth < w) {
            oEl.css('margin-left', Math.floor((w - iWidth) / 2) + 'px');
        }

    }
}
/********************************Hide Show Slide*********************************/
function hideContent(elementId, headerElement, strHeader) {
    var element = document.getElementById(elementId);
    if (element.up == null || element.down) {
        element.up = true;
        element.down = false;
        $('#' + elementId).slideUp('slow');
        if (strHeader != "") {
            headerElement.innerHTML = '<header><h3>[+] ' + strHeader + '</h3></header>';
        }
    }
    else {
        element.down = true;
        element.up = false;
        $('#' + elementId).slideDown('slow');
        if (strHeader != "") {
            headerElement.innerHTML = '<header><h3>[-] ' + strHeader + '</h3></header>';
        }
    }
}

//Check No Image and Set No Image 
function checkNoImages(img_width, img_height) {
    $("img").error(function () {
        $(this).attr("src", GetUrl("Content/Default/No-Image/img-noimg-company.png"));
    });
}
//Check No Logo and Set No Logo 
function checkNoLogo(img_width, img_height) {
    $("img").error(function () {

        $(this).attr("src", GetUrl("Content/Default/No-Image/img-noimg-company.png"));
    });
}

//-------------------Clear Form------------------//
function clearForm(form) {
    // iterate over all of the inputs for the form
    // element that was passed in
    $(':input', form).each(function () {
        var type = this.type;
        var tag = this.tagName.toLowerCase(); // normalize case
        // it's ok to reset the value attr of text inputs,
        // password inputs, and textareas
        if (type == 'text' || type == 'password' || tag == 'textarea')
            this.value = "";
            // checkboxes and radios need to have their checked state cleared
            // but should *not* have their 'value' changed
        else if (type == 'checkbox' || type == 'radio')
            this.checked = false;
            // select elements need to have their 'selectedIndex' property set to -1
            // (this works for both single and multiple select elements)
        else if (tag == 'select')
            this.selectedIndex = 0;
    });
}

//check_idcard
function check_idcard(idcard) {
    if (idcard.value == "") { return false; }
    if (idcard.length < 13) { return false; }

    var num = str_split(idcard); // function เพิ่มเติม           
    var sum = 0;
    var total = 0;
    var digi = 13;

    for (i = 0; i < 12; i++) {
        sum = sum + (num[i] * digi);
        digi--;
    }
    total = ((11 - (sum % 11)) % 10);

    if (total == num[12]) { //	alert('รหัสหมายเลขประจำตัวประชาชนถูกต้อง');
        return true;
    } else { //	alert('รหัสหมายเลขประจำตัวประชาชนไม่ถูกต้อง');
        return false;
    }

}
function str_split(f_string, f_split_length) {
    f_string += '';
    if (f_split_length == undefined) {
        f_split_length = 1;
    }
    if (f_split_length > 0) {
        var result = [];
        while (f_string.length > f_split_length) {
            result[result.length] = f_string.substring(0, f_split_length);
            f_string = f_string.substring(f_split_length);
        }
        result[result.length] = f_string;
        return result;
    }
    return false;
}
function id_card(id) {
    if (id.value.length == 13) {
        if (check_idcard(id.value)) {
            //alert("รหัสประชาชนถูกต้อง");
            $(".for-idn").removeClass("error");
            $("#IdentityNo").siblings('.error').remove();
        } else {
            //alert("รหัสประชาชนไม่ถูกต้อง กรุณากรอกใหม่อีกครั้ง");
            $(".for-JobCate").addClass("error");
            $("#IdentityNo").siblings('.success').remove();
            $("#IdentityNo").siblings('.error').remove();
            $("#IdentityNo").closest('.controls').append(' <label for="IdentityNo" generated="true" class="error" style=""></label>');
            $("#IdentityNo").siblings(".error").html(valid.lblIDCErr + " " + valid.lblPlzTry);
            id.value = "";
            //focus();
        }
    } else { id.value = ""; }
}

/* แก้ไข Telerik Upload */
jQuery(function () {
    jQuery('.t-upload-button input[type=file]').live('click', function (e) {
        e.stopPropagation();
    });
    jQuery('.t-upload-button').live('click', function (e) {
        jQuery(this).find('input[type=file]').get(0).click();
    });
});

/**********ChangeLanguage**************/

function ChangeLanguage(Language) {
    var lang = "";
    switch (Language) {
        case "th-TH": lang = "ไทย"; break;
        case "en-US": lang = "อังกฤษ"; break;
        case "mn-MN": lang = "พม่า"; break;
        default: break;
    }
    //bootbox.alert("กรุณารอสักครู่ ระบบกำลังเปลี่ยนเป็นเวอร์ชั่นภาษา" + lang);
    OpenLoading(true);

    var path = location.pathname;
    data = {
        Language: Language,
        returnUrl: path
    }
    $.ajax({
        url: GetUrl("Default/BindSaveLanguage"),
        data: data,
        success: function (data) {
            location.reload(true);
        },
        type: "POST"
    });
}

//Chat
function openWindowChat(url, target) {

    var oldWin = window.open(url, target, 'width=620,height=500,menubar=no,status=no,resizable=no');
    oldWin.focus();  // give focus 
    return false;
}


//function addLink() {
//    var body_element = document.getElementsByTagName('body')[0];
//    var selection;
//    selection = window.getSelection();
//    var pagelink = "<br /><br /> " + valid.lblReadMoreAt + ": <a href='" + document.location.href + "'>" + document.location.href + "</a>"; // change this if you want
//    var copytext = selection + pagelink;
//    var newdiv = document.createElement('div');
//    newdiv.style.position='absolute';
//    newdiv.style.left='-99999px';
//    body_element.appendChild(newdiv);
//    newdiv.innerHTML = copytext;
//    selection.selectAllChildren(newdiv);
//    window.setTimeout(function() {
//        body_element.removeChild(newdiv);
//    },0);
//}
//document.oncopy = addLink;


function getDateTimeNow() {
    var today = new Date();
    var dd = today.getDate();
    var mm = today.getMonth() + 1; //January is 0!
    var yyyy = today.getFullYear();
    var H = today.getHours();
    var M = today.getMinutes();
    var S = today.getSeconds();

    if (dd < 10) {
        dd = '0' + dd
    }

    if (mm < 10) {
        mm = '0' + mm
    }

    return today = mm + '/' + dd + '/' + yyyy + " " + H + ":" + M + ":" + S;
}
function getTimeNow() {
    var today = new Date();
    var H = today.getHours();
    var M = today.getMinutes();
    var S = today.getSeconds();


    return H + ":" + M + ":" + S;
}

