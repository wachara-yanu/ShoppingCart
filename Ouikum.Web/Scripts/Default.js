var CurrentLanguage = $('#hidAppLang').val();
var CurrentCultureInfo = $('#hidAppLang').val();
var LangUrl = '';
//if (CurrentCultureInfo == "en-US") {
//    LangUrl = 'en/';

function OpenLoading(isLoad, img, obj) {
    if (isLoad == true){
        if (img == null) {
            img = '<div class="icon-loader"></div>';
        } else {
            img = '<img src=\"' + img + '\" >';
        }
        if ($('#loading').length == 0) {
        if (obj == null || obj == undefined) {
            $('body').prepend('<div id="loading">&nbsp;</div><div id="imgloading">' + img + '</div>');
        } else {
            obj.prepend('<div id="loading">&nbsp;</div><div id="imgloading">' + img + '</div>');
        }
        $("#loading").css({ 'filter': 'alpha(opacity=50)', 'opacity': '0.5' });
        $("#imgloading").position({ my: "center", at: "center", of: "#loading" });
        }

    } else {
        $('#loading').remove(); $('#imgloading').remove();
    }
}

function GenNavLanguage() {
    html = '';
    html += '<div class="fl_l mar_l25 w70 fontDarkBlue">';
    html += '<span class="fl_l">English </span><span class="fl_r caret" style="margin-top:5px"></span>';
    html += '</div>';
    $('#user-language').html(html);
}

function HighlightTextSearch() {
    var text = $('#TextSearch').val();
    if (text != null && text != "") {
        var arr = text.split(' ');
        for (var i = 0 ; i < arr.length ; i++) {
            $(".label-header").highlight(arr[i]);
        }
    }
}

//=================AntiForgeryToken//=================//
AddAntiForgeryToken = function (data) {
    data.__RequestVerificationToken = $('input[name=__RequestVerificationToken]').val();
    return data;
};
function TitleText(str) {
    $('title').text(str);
}

function GetFormData(obj) {
    var s = obj.attr('form-data');
    return CreatObj(s);
}

function CreatObj(n) {
    var el = $(n);
    return el;
}

function checkNoImages() {
    $("img").error(function () {
        $(this).attr("src", "http://www.placehold.it/200x200/EFEFEF/AAAAAA&text=no+image");
    });
}

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

function resizeImg(el, maxW, maxH) {
    var oEl = el;
    var maxWidth = maxW;
    var maxHeight = maxH;
    oEl.css("width", "auto").css("height", "auto");
    oEl.removeAttr("width").removeAttr("height");
    var width = oEl.width();
    var height = oEl.height();

    if (width > maxWidth && height <= maxHeight) {

        var ratio = maxWidth / width;
        oEl.css("width", maxWidth);
        oEl.css("height", height * ratio);

    } else if (width <= maxWidth && height > maxHeight) { 
        var ratio = maxHeight / height;
        oEl.css("width", width * ratio);
        oEl.css("height", maxHeight);

    } else if (width > maxWidth && height > maxHeight) { 
        if (width > height) { 
            var ratio = maxWidth / width;
            oEl.css("width", maxWidth);
            oEl.css("height", height * ratio);
        } else if (height > width) { 
            var ratio = maxHeight / height;
            oEl.css("width", width * ratio);
            oEl.css("height", maxHeight);

        } else { 
            oEl.css("width", maxWidth);
            oEl.css("height", maxHeight);
        }

    } else {
        oEl.css("width", width);
        oEl.css("height", height);
    }
}

function resizeImg_mobile(el, maxW, maxH) {
    var oEl = el;
    var maxWidth = maxW;
    var maxHeight = maxH;
    oEl.css("width", "auto").css("height", "auto");
    oEl.removeAttr("width").removeAttr("height");
    var width = oEl.width();
    var height = oEl.height();

    if (width > maxWidth && height <= maxHeight) {

        var ratio = maxWidth / width;
        oEl.css("width", maxWidth + "%");
        oEl.css("height", height * ratio + "%");

    } else if (width <= maxWidth && height > maxHeight) {
        var ratio = maxHeight / height;
        oEl.css("width", width * ratio + "%");
        oEl.css("height", maxHeight + "%");

    } else if (width > maxWidth && height > maxHeight) {
        if (width > height) {
            var ratio = maxWidth / width;
            oEl.css("width", maxWidth + "%");
            oEl.css("height", height * ratio + "%");
        } else if (height > width) {
            var ratio = maxHeight / height;
            oEl.css("width", width * ratio + "%");
            oEl.css("height", maxHeight + "%");

        } else {
            oEl.css("width", maxWidth + "%");
            oEl.css("height", maxHeight + "%");
        }

    } else {
        oEl.css("width", width + "%");
        oEl.css("height", height + "%");
    }
}

function setCookie(name, value, expires, path, domain, secure) {

    var today = new Date();
    today.setTime(today.getTime());
    if (expires) {
        expires = expires * 1000 * 60 * 60 * 24;
    }
    var expires_date = new Date(today.getTime() + (expires));

    document.cookie = name + "=" + escape(value) +
	((expires) ? "; expires=" + expires_date.toGMTString() : "") +
	((path) ? "; path=" + path : "") +
	((domain) ? "; domain=" + domain : "") +
	((secure) ? "; secure" : "");
    
}

function getCookie(name) {
    var dc = document.cookie;
    var prefix = name + "=";
    var begin = dc.indexOf("; " + prefix);
    if (begin == -1) {
        begin = dc.indexOf(prefix);
        if (begin != 0) return null;
    } else {
        begin += 2;
    }
    var end = document.cookie.indexOf(";", begin);
    if (end == -1) {
        end = dc.length;
    }
    return unescape(dc.substring(begin + prefix.length, end));
}

function deleteCookie(name, path, domain) {
    if (getCookie(name)) {
        document.cookie = name + "=" +
	    ((path) ? "; path=" + path : "") +
	    ((domain) ? "; domain=" + domain : "") +
	    "; expires=Thu, 01-Jan-70 00:00:01 GMT";
    }
}

    function GetBiztype(b_id, id) {
        var ListBiztype = "";
        $.ajax({
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
                console.log(label.vldcannot_check_info);
            }
        });
    }

    function ListBiztype(b_id, id) {
        var ListBiztypes = "";
        $.ajax({
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
                console.log(label.vldcannot_check_info);
            }
        });
    }

    $(document).on( "click", ".icon_up,.btn_num_up", function() { 
        var obj = $(this).parent().prev();
        if (obj.val() == "") {
            obj.val(1);
        } else {
            var max_val = parseInt(obj.attr('max-val'), 10);
            if (max_val >= 0) {
                if (parseInt(obj.val()) < max_val) {
                    obj.val(parseInt(obj.val(), 10) + 1);
                } else {
                    bootbox.alert(label.lblUpgoldtoPuschase);
                }
            } else {

            }
        }
    });

    $(document).on( "click", ".icon_down,.btn_num_up", function() {  
        if ($(this).parent().prev().val() == "") {
        } else {
            var num = parseInt($(this).parent().prev().val());
            console.log(' num : ' + num);
            if (parseInt($(this).parent().prev().val()) > 1) {
                var less = parseInt($(this).parent().prev().val()); 
                less--; 

                $(this).parent().prev().val(less)
            } else {
                return false; 
            }
        }
    });

    if ($('.container_home').width() <= 1100) {
        $(".footer-cate:last").hide();
    }

    //-------------------------CheckError
    var information = '';
    function CheckError(data,msg) {
        information = $('#information').html();
        if (data.MsgError == null) {
            msg = 'Save Not Success';
        }
        if (msg == null) {
            if (data.MsgError.length > 0) {
                msg = data.MsgError;
            } else {
                msg = 'save fail';
            }
        }
        if (!data.IsResult) {
            AlertError(msg)
            return false
        } else {
            AlertSuccess(label.vldsave_success)
            return true;
        }
    }

    //------------------------- Alert Message
    function AlertError(msg, obj) {
        if (obj == null || obj == undefined) {
            obj = $('#information_msg');
        } 
            obj.hide();
            obj.removeClass();
            obj.addClass('alert_error ');
            obj.html(msg);
            obj.fadeIn();
       
}

    function AlertSuccess(msg) {
        $('#information_msg').hide();
        $('#information_msg').removeClass();
        $('#information_msg').addClass('alert_success ');
        $('#information_msg').html(msg);
        $('#information_msg').fadeIn();
}

    function AlertWarning(msg) {
        $('#information_msg').hide();
        $('#information_msg').removeClass();
        $('#information_msg').addClass('alert_warning ');
        $('#information_msg').html(msg);
        $('#information_msg').fadeIn();
    }
 
    function clearForm(form) { 
        $(':input', form).each(function () {
            var type = this.type;
            var tag = this.tagName.toLowerCase(); 
            if (type == 'text' || type == 'password' || tag == 'textarea')
                this.value = ""; 
            else if (type == 'checkbox' || type == 'radio')
                this.checked = false; 
            else if (tag == 'select')
                this.selectedIndex = 0;
        });
    };

    function autoComplete(path) {
        $('.typeahead').typeahead({
            source: function (query, process) {
                $.post(path, { query: query }, function (data) {
                    return process(data);
                });
            }
        });
    }

    (function ($) {
        var dragging, placeholders = $();
        $.fn.sortable = function (options) {
            var method = String(options);
            options = $.extend({
                connectWith: false
            }, options);
            return this.each(function () {
                if (/^enable|disable|destroy$/.test(method)) {
                    var items = $(this).children($(this).data('items')).attr('draggable', method == 'enable');
                    if (method == 'destroy') {
                        items.add(this).removeData('connectWith items')
					.off('dragstart.h5s dragend.h5s selectstart.h5s dragover.h5s dragenter.h5s drop.h5s');
                    }
                    return;
                }
                var isHandle, index, items = $(this).children(options.items);
                var placeholder = $('<' + (/^ul|ol$/i.test(this.tagName) ? 'li' : 'div') + ' class="sortable-placeholder">');
                items.find(options.handle).mousedown(function () {
                    isHandle = true;
                }).mouseup(function () {
                    isHandle = false;
                });
                $(this).data('items', options.items)
                placeholders = placeholders.add(placeholder);
                if (options.connectWith) {
                    $(options.connectWith).add(this).data('connectWith', options.connectWith);
                }
                items.attr('draggable', 'true').on('dragstart.h5s', function (e) {
                    if (options.handle && !isHandle) {
                        return false;
                    }
                    isHandle = false;
                    var dt = e.originalEvent.dataTransfer;
                    dt.effectAllowed = 'move';
                    dt.setData('Text', 'dummy');
                    index = (dragging = $(this)).addClass('sortable-dragging').index();
                }).on('dragend.h5s', function () {
                    if (!dragging) {
                        return;
                    }
                    dragging.removeClass('sortable-dragging').show();
                    placeholders.detach();
                    if (index != dragging.index()) {
                        dragging.parent().trigger('sortupdate', { item: dragging });
                    }
                    dragging = null;
                }).not('a[href], img').on('selectstart.h5s', function () {
                    this.dragDrop && this.dragDrop();
                    return false;
                }).end().add([this, placeholder]).on('dragover.h5s dragenter.h5s drop.h5s', function (e) {
                    if (!items.is(dragging) && options.connectWith !== $(dragging).parent().data('connectWith')) {
                        return true;
                    }
                    if (e.type == 'drop') {
                        e.stopPropagation();
                        placeholders.filter(':visible').after(dragging);
                        dragging.trigger('dragend.h5s');
                        return false;
                    }
                    e.preventDefault();
                    e.originalEvent.dataTransfer.dropEffect = 'move';
                    if (items.is(this)) {
                        if (options.forcePlaceholderSize) {
                            placeholder.height(dragging.outerHeight());
                        }
                        dragging.hide();
                        $(this)[placeholder.index() < $(this).index() ? 'after' : 'before'](placeholder);
                        placeholders.not(placeholder).detach();
                    } else if (!placeholders.is(this) && !$(this).children(options.items).length) {
                        placeholders.detach();
                        $(this).append(placeholder);
                    }
                    return false;
                });
            });
        };
    })(jQuery);

    function CheckBoxall(Obj) {
        if (Obj.attr("value") == true || Obj.attr("checked") == "checked") {
            $(".cbxCompID").attr("checked", "checked");
            $(".cbxCompID").attr("value", "true");
            Obj.attr("checked", "checked");
            Obj.attr("value", "true");

        } else {
            $(".cbxCompID").removeAttr("checked");
            $(".cbxCompID").attr("value", "false");
            Obj.removeAttr("checked");
            Obj.attr("value", "false");
        }

}

    function CheckBox(id) {

        if ($("#" + id + "").attr("value") == true || $("#" + id + "").attr("checked") == "checked") {
            $("#" + id + "").attr("checked", "checked");
            $("#" + id + "").attr("value", "true");
        } else {
            $("#" + id + "").removeAttr("checked");
            $("#" + id + "").attr("value", "false");
        }
}



/***ฟังก์ชั่นเพิ่งสินค้ารายการโปรด*/

    function AddFavPro(id) {
        if ($("#ChkHaveUser").val() == 0) {
            bootbox.alert('กรุณาเข้าสู่ระบบ');
            return false;
        }
        else {
            $.ajax({
                type: "POST",
                url: GetUrl("MyB2B/Favorite/AddFavProduct"),
                data: { ProId: id },
                dataType: 'json',
                traditional: true,
                success: function (data) {
                    if (data == false) {
                        bootbox.alert(label.vldfav_success);
                        return false;
                    }
                    bootbox.alert(label.vldadd_fav);
                },
                error: function () {
                },
                type: "POST"
            });
        }
}

    function AddTempcart(id) {
    if ($("#ChkHaveUser").val() == 0) {
        bootbox.alert('กรุณาเข้าสู่ระบบ');
        return false;
    }
    else {
        $.ajax({
            type: "POST",
            url: GetUrl("MyB2B/Favorite/AddFavProduct"),
            data: { ProId: id },
            dataType: 'json',
            traditional: true,
            success: function (data) {
                if (data == false) {
                    bootbox.alert(label.vldfav_success);
                    return false;
                }
                bootbox.alert(label.vldadd_fav);
            },
            error: function () {
            },
            type: "POST"
        });
    }
}

function AddFavBuy(id) {
    if ($("#ChkHaveUser").val() == 0) {
        bootbox.alert('กรุณาเข้าสู่ระบบ');
        return false;
    }
    else {
        $.ajax({
            type: "POST",
            url: GetUrl("MyB2B/Favorite/AddFavBuylead"),
            data: { BuyId: id },
            dataType: 'json',
            traditional: true,
            success: function (data) {
                if (data == false) {
                    bootbox.alert(label.vldfav_success);
                    return false;
                }
                bootbox.alert(label.vldadd_fav);
            },
            error: function () {
            },
            type: "POST"
        });
    }
}

function AddFavSup(id) {
    if ($("#ChkHaveUser").val() == 0) {
        bootbox.alert('กรุณาเข้าสู่ระบบ');
        return false;
    }
    else {
        $.ajax({
            type: "POST",
            url: GetUrl("MyB2B/Favorite/AddFavSupplier"),
            data: { SupID: id },
            dataType: 'json',
            traditional: true,
            success: function (data) {
                if (data == false) {
                    bootbox.alert(label.vldfav_success);
                    return false;
                }
                bootbox.alert(label.vldadd_fav);
            },
            error: function () {
            },
            type: "POST"
        });
    }
}

var MultiValue = new Array();
function GetMultiValue() {
    var chk = $("input[name='chkpd']:checked");
    var len = chk.length;
    if (len == 0) {
        return false;
    }
    else {
        for (var i = 0; i < len; i++) {
            MultiValue[i] = chk.eq(i).attr('data-id');
        }
    } 
}

function AddMultiFavPro() {
    if ($("#ChkHaveUser").val() == 0) {
        bootbox.alert(label.vldplease_signIn);
        return false;
    }
    else if (GetMultiValue() == false) {
        bootbox.alert(label.vldplease_select_fav);
        return false;
    }
    else {
        GetMultiValue();
        $.ajax({
            type: "POST",
            url: GetUrl("MyB2B/Favorite/AddFavProduct"),
            data: { ProId: MultiValue },
            dataType: 'json',
            traditional: true,
            success: function (data) {
                bootbox.alert(label.vldall_fav_success);
            },
            error: function () {
            },
            type: "POST"
        });
    }
}

function AddMultiFavBuy() {
    if ($("#ChkHaveUser").val() == 0) {
        bootbox.alert(label.vldplease_signIn);
        return false;
    }
    else if (GetMultiValue() == false) {
        bootbox.alert(label.vldplease_select_fav);
        return false;
    }
    else {
        GetMultiValue();
        $.ajax({
            type: "POST",
            url: GetUrl("MyB2B/Favorite/AddFavBuylead"),
            data: { BuyId: MultiValue },
            dataType: 'json',
            traditional: true,
            success: function (data) {
                bootbox.alert(label.vldall_fav_success);
            },
            error: function () {
            },
            type: "POST"
        });
    }
}

function AddMultiFavSup() {
    if ($("#ChkHaveUser").val() == 0) {
        bootbox.alert(label.vldplease_signIn);
        return false;
    }
    else if (GetMultiValue() == false) {
        bootbox.alert(label.vldplease_select_fav);
        return false;
    }
    else {
        GetMultiValue();
        $.ajax({
            type: "POST",
            url: GetUrl("MyB2B/Favorite/AddFavSupplier"),
            data: { SupID: MultiValue },
            dataType: 'json',
            traditional: true,
            success: function (data) {
                bootbox.alert(label.vldadd_fav_all);
            },
            error: function () {
            },
            type: "POST"
        });
    }
}

function Choose(obj) {
    if (obj != undefined || obj != null) {
        if (obj.attr("checked") == true || obj.attr("checked") == "checked") {
            obj.addClass("Chk-data");
            obj.attr("checked", "checked");
        } else {
            obj.removeClass("Chk-data");
            obj.removeAttr("checked", "checked");
        }
    }
}

$(document).on("keypress", "#ddlFindDatePeriod", function (e) {
    return false;
});


$(document).on("click", "#SelectAllPeriod", function (e) { 
    if ($(this).attr("checked") == true || $(this).attr("checked") == "checked") {
        $('#ddlFindDatePeriod').removeAttr('disabled');
        $('#ddlFindDatePeriod').removeAttr('readonly');
        $('#ddlFindDatePeriod').attr('placeholder', '--'+label.vldplease_select+'--');
    } else {
        $('#ddlFindDatePeriod').val('');
        $('#ddlFindDatePeriod').attr('disabled', 'disabled');
        $('#ddlFindDatePeriod').attr('readonly', 'readonly');
        $('#ddlFindDatePeriod').attr('placeholder', '--'+label.vldselect_all+'--');
    }
});

function SendSuccessPopup() {
    $('#Modal').modal('hide')
    OpenLoading(false);

    //bootbox.alert(label.vldsend_success);
    bootbox.alert(label.vldsend_success, function () {
       document.location.reload(true);
    }); 
}


function ActiveMenu(index) {
    $('.fontMainMenu').eq(index - 1).addClass('active');
}
 
if (!Array.prototype.filter) {
    Array.prototype.filter = function (fun /*, thisp */) {
        "use strict";

        if (this === void 0 || this === null)
            throw new TypeError();

        var t = Object(this);
        var len = t.length >>> 0;
        if (typeof fun !== "function")
            throw new TypeError();

        var res = [];
        var thisp = arguments[1];
        for (var i = 0; i < len; i++) {
            if (i in t) {
                var val = t[i]; // in case fun mutates this
                if (fun.call(thisp, val, i, t))
                    res.push(val);
            }
        }

        return res;
    };
}
 
if (!Array.prototype.indexOf) {
    Array.prototype.indexOf = function (searchElement /*, fromIndex */) {
        "use strict";
        if (this == null) {
            throw new TypeError();
        }
        var t = Object(this);
        var len = t.length >>> 0;
        if (len === 0) {
            return -1;
        }
        var n = 0;
        if (arguments.length > 1) {
            n = Number(arguments[1]);
            if (n != n) { 
                n = 0;
            } else if (n != 0 && n != Infinity && n != -Infinity) {
                n = (n > 0 || -1) * Math.floor(Math.abs(n));
            }
        }
        if (n >= len) {
            return -1;
        }
        var k = n >= 0 ? n : Math.max(len - Math.abs(n), 0);
        for (; k < len; k++) {
            if (k in t && t[k] === searchElement) {
                return k;
            }
        }
        return -1;
    }
}
function Random() {

    var ran = Math.floor((Math.random() * 7));
    console.log('ran : ' + ran);

}

function SubStringName(str, len) {
    if (str.length > len) {
      return  str.substring(0, len - 3) + "...";
  } else {
      return str;
    }

}

function DelData(id, rowversion, PrimaryKeyName,Controller) {
    var ID = [];
    var RowVersion = [];
    var Check = [];
    var chk = 0;
    if (id == null || id == "") {
        $(".grid > tbody > tr").each(function (index) {
            if ($(this).children().find(".cbxItem:checked").val() == "true") {
                Check[index] = "True";
            }
            else {
                Check[index] = "False";
            }
            ID[index] = $(this).children().find(".hidPrimeID").val();
            RowVersion[index] = $(this).children().find(".hidRowVersion").val();
        });
    } else {
        ID[0] = id;
        RowVersion[0] = rowversion;
        Check[0] = "True";
    }
    for(var i = 0;i < Check.length;i++){
        if (Check[i] == "True") {
            chk++;
         }
    }
    OpenLoading(true);
    if (ID.length > 0 && chk > 0) {
        $.ajax({
            url: url + Controller + "/DelData",
            data: { Check: Check, ID: ID, RowVersion: RowVersion, PrimaryKeyName: PrimaryKeyName },
            success: function (data) {
                if (data.Result) {
                    OpenLoading(false);
                    $(g_hidsubmit).eq(g_no).click();
                    $("#information").removeClass(' alert-error');
                    alertMsg("Alert : ", "success", label.vlddel_success);
                    return true;
                } else {
                    return false;
                }
            },
            type: "POST", traditional: true
        });
    }
    else {
        $("#information").removeClass(' alert-success');
        alertMsg("Notice! ", "error", label.vldconfirm_del_data);
        return false;
    }
}

function alertMsg(a_header, a_class, msg) {
    var b_header = a_header;
    var b_class = a_class;
    $("#information").addClass(' alert-' + b_class);
    $("#information > strong").text(b_header);
    $("#information > p").text(msg);
    $("#information").fadeIn();
    OpenLoading(false);
    $("#information").delay(3000).fadeOut(500);
}
function autoHeight(id) {
    $("#" + id + "").slideDown(function () {
        $("#sidebar").height($("#" + id + "").height());
        $("#main").height($("#" + id + "").height());
    });
}
function selectedcateLV1() {
    $.ajax({
        url: GetUrl("Default/SelectCategoryLV2"),
        data: { lv1: $('#SelectCateLV1 option:selected').val() },
        type: "POST",
        success: function (data) {
            $("#SelectCateLV2").removeAttr('disabled');
            $("#SelectCateLV2").html(data);
        },
        error: function () { 
        }
    });
}
function selectedcateLV2() {
    $.ajax({
        url: GetUrl("Default/SelectCategoryLV3"),
        data: { lv2: $('#SelectCateLV2 option:selected').val() },
        type: "POST",
        success: function (data) {
            $("#SelectCateLV3").removeAttr('disabled');
            $("#SelectCateLV3").html(data);
        },
        error: function () { 
        }
    });
}
function SidebarResize() {
    
    $(document).on("click", "#sidebar .resize", function (e) {

        var obj = $(this).parent();
        if (obj.hasClass("minimize")) {
            obj.removeClass("minimize").removeAttr("style");
            $(".autoShowHide").show();
            $(".icon-ShowHide").addClass("hide");
            $('.resize ').addClass('mar_r5');
            $('.resize > i').addClass('icon-widget-left');
            $('.resize > i').removeClass('icon-widget-right');
            $('.resize > i').attr('title', 'Hide');
            $('#main').removeAttr('style');
            $('#ContentManageRight').width('81%');
            autoHeight('autoHeight');
            $(obj).slideDown(function () {
                var height = $("#main").height();
                if (height < 749)
                    height = 749;

                $("#sidebar").height(height);
            });
        } else {
            obj.addClass("minimize").width(40);
            $('#main').width('98%');
            $('#ContentManageRight').width('97%');
            $(".autoShowHide").hide();
            $(".icon-ShowHide").removeClass("hide");
            $('.resize ').removeClass('mar_r5');
            $('.resize > i').addClass('icon-widget-right');
            $('.resize > i').removeClass('icon-widget-left');
            $('.resize > i').attr('title', 'Show');
        }
    });
}
 
function ReplaceTextSearch(str) {
    str = str.trim();
    if (str != null && str != "") {
        str = str.replace("'", "");
        str = str.replace("\"", "");
        str = str.replace("&", "");
        str = str.replace("+", "");
        str = str.replace("_", "");
        str = str.replace(" ", "");
    }

    return str;
} 
function checkEng(text) {
    var str = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_,&-+=?.''\"\"()%<>/$ ";//!@#$%^&฿*/\|(){}[]'"<>-+=?.,:๛☻☺♠○◘♣♦•;
    var val = text;
    var valOK = true;
    for (i = 0; i < val.length & valOK; i++) {
        valOK = (str.indexOf(val.charAt(i)) != -1);
    }
    if (!valOK) { 
        return false;
    } return true;
}

function checkDisclaimer(text) {
    var str = " abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_()/-*+%,.?'กขฃคฅฆงจฉชซฌญฎฏฐฑฒณดตถทธนบปผฝพฟภมยรลวศษสหฬอฮะาิีึืุูเแโใไัำๆๅํ่้๊๋็์ฯฦฤ๑๒๓๔๕๖๗๘๙๐";//!@#$^฿*|{}[]'\"<>+=?.:๛☻☺♠○◘♣♦•;
    var val = text;
    var valOK = true;
    for (i = 0; i < val.length & valOK; i++) {
        valOK = (str.indexOf(val.charAt(i)) != -1);
    }
    if (!valOK) {
        return false;
    } return true;
}

function checkEmailEng(text) {
    var str = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_-@.";
    var val = text;
    var valOK = true;
    for (i = 0; i < val.length & valOK; i++) {
        valOK = (str.indexOf(val.charAt(i)) != -1);
    }
    if (!valOK) { 
        return false;
    } return true;
}



function unique(a) {
    var len = a.length;
    if (len > 0) {
        a = a.filter(function (itm, i, a) {
            return i == a.indexOf(itm);
        });
    }
    return a;
}
 
function GeneratePaging(name, method, model) {
    var html = "";
    var index = model.PageIndex;
    var pagesize = model.PageSize;
    var totalrow = model.TotalRow;
    var totalpage = model.TotalPage;
    var start = ((index - 1) * pagesize) + 1;
    var end = (index * pagesize);


    if (totalrow > 0) {
        if (index == totalpage && index == 1) {
            end = totalrow;
            html = '<ul class="pager">';
            html += '<li class="disabled" ><a class="cursor" >Previous</a></li> ';
            html += '<li class="disabled" ><a class="cursor" >Next</a></li></ul>';
        } else if (index == 1) {
            html += '<ul class="pager">';
            html += '<li class="disabled" ><a class="cursor" >Previous</a></li>';
            html += '<li  onclick="' + method + '(\'' + name + '\',' + (index + 1) + ');" ><a class="cursor" >Next</a></li> </ul>';
        } else if (index == totalpage) {
            end = totalrow;
            html = '<ul class="pager">';
            html += '<li onclick="' + method + '(\'' + name + '\',' + (index - 1) + ');"  ><a class="cursor" >Previous</a></li> ';
            html += '<li class="disabled" ><a class="cursor" >Next</a></li></ul>';
        } else {
            html = '<ul class="pager">';
            html += '<li onclick="' + method + '(\'' + name + '\',' + (index - 1) + ');"  ><a class="cursor" >Previous</a></li>';
            html += '<li onclick="' + method + '(\'' + name + '\',' + (index + 1) + ');" ><a class="cursor" >Next</a></li></ul>';
        }

        var str = '<div >';
        str += '<div class="fl_l mar_t5"><strong>' + start + '-' + end + '</strong>  From  <strong>' + totalrow + '</strong>  </div>';
        str += '<div class="fl_r">' + html + '</div><div class="clean5"></div>';
        str += '</div>';
        str += '<div ><input class="pageindex" type="hidden" value="' + index + '" /></div>';
        str += '<div class="clean5"></div>';
        $('#' + name + '-content').prepend(str);
    } else {

        var str = '<center style="display:block; height:32px;" class="mar_t5"><strong> 0 </strong>  From  <strong> 0 </strong> </center>';
        str += '<div class="clean5"></div>';
        $('#' + name + '-content').prepend(str);

    }
}


function setPageIndex(name, pageindex) { 
    $('#' + name + '-content').find('.pageindex').val(pageindex)
}

function getPageIndex(name) {

    var pageindex = parseInt($('#' + name + '-content').find('.pageindex').val(), 10);
     
    if (pageindex == null || pageindex == undefined) {
        return 1;
    } else {
        return pageindex;
    }

}

function doLoad(name, url, callback) {
    OpenLoading(true);
    data = {
        hidPageIndex: getPageIndex(name),
        TextSearch: $('#' + name + '-form').find('.textsearch').val()
    }; 

    $.ajax({
        url: GetUrl(url),
        type: "POST",
        data: data,
        dataType: 'json',
        traditional: true,
        success: function (model) {
            OpenLoading(false);
            callback(model);
        },
        error: function () {
        }
    });

}

function formatJsonDate(jsonDate) {
    var d = new Date(parseInt(jsonDate.substr(6)));
    var year = d.getFullYear();
    var month = d.getMonth() + 1;
    var day = d.getDate();
    var hour = d.getHours();
    var minute = d.getMinutes();
    var second = d.getSeconds();
    var format = day + '/' + month + '/' + year + '  ' + hour + ':' + minute;
    return format;
}

function formatJsonDatePrefix(jsonDate) {
    var date = new Date(parseInt(jsonDate.substr(6)));
    var formatted = ("0" + date.getDate()).slice(-2) + "/" + ("0" + (date.getMonth() + 1)).slice(-2) + "/" + date.getFullYear() + " " + date.toLocaleTimeString();
    return formatted;
}

$("#hoverWebBU").hover(function (event) {
    if ($(event.target).hasClass('hashold')) {
        ShowWebBU();
    } else {
        HideWebBU();
    }
});
function ShowWebBU() {
    $('#webBU').slideDown();
    $('#webBU').css("display", "inline-block");
    $('#hoverWebBU').addClass('hashold');
}

function HideWebBU() {
    $('#webBU').css("display", "none");
    $('#hoverWebBU').removeClass('hashold');
}

function GetFeedFaceBookGroup() {
    $.ajax({
        url: GetUrl("Default/GetFBGroup"), 
        type: "POST",
        dataType: 'json',
        success: function (data) {
            //console.log(data);
        },
        error: function () {
        }
    });
}

$(document).on("change", ".sel-home-head", function (e) {
    e.preventDefault();
    //OpenListSearch(true);
    //var index = $('.list-search-text').index($(this));
    //var search = $(this).attr('data-val');


    //SetActiveListSearch(index)
    var search = $(this).val();
    SetFormAction(search);


});


function SetFormAction(val) {
     
    if (val == 'Product') {
        url = GetUrl(LangUrl + 'Search/Product/List');
        $('#TextSearch').attr('placeholder', label.vldsearch_product);
        $('.sel-home-head').val(val);

    } else if (val == 'Buylead') {
        url = GetUrl(LangUrl + 'Purchase/Search');
        $('#TextSearch').attr('placeholder', label.vldsearch_buylead);
        $('.sel-home-head').val(val);

    } else if (val == 'Supplier') {
        url = GetUrl(LangUrl + 'Search/Supplier/List');
        $('#TextSearch').attr('placeholder', label.vldsearch_supplier);
        $('.sel-home-head').val(val);

    } else if (val == 'Bid') {
        url = GetUrl(LangUrl + 'BidProduct/List');
        $('#TextSearch').attr('placeholder', label.vldsearch_bidproduct);
        $('.sel-home-head').val(val);
    }
    else if (val == 'SME') {
        url = GetUrlSME(LangUrl + 'Search/Product/List');
        $('#TextSearch').attr('placeholder', label.vldsearch_bidproduct);
        $('.sel-home-head').val(val);
    }
        //else if (val == 'Cate') {
        //    var cate = $('#ddlCategories option:selected');
        //    if (cate.val() > 0) {
        //        url = GetUrl('Search/Product/List/Category/' + cate.val() + '/2/' + cate.attr('name'));//ใช้กับ AppStoreThai 
        //    }
        //    else {
        //        url = GetUrl('Search/Product/List/Category/7102/1/Computer-Software');
        //    }
        //}
        //else if (val == 'Province') {
        //    var province = $('#ddlCategories option:selected');
        //    if (province.val() > 0) {
        //        url = GetUrl('Search/Product/List?Province=' + province.val());//ใช้กับ AppStoreThai 
        //    }
        //}
    else {
        url = GetUrl('Search/Product/List');
    }
    $('#frmSearch').attr('action', url);
    // console.log('form : ' + $('#frmSearch').attr('action'));
}

function GetUrlSME(url) {
    url = "http://sme.b2bthai.com/" + url;
    return url;
}

function GenerateCateFooter() {
    var Window_W = $(window).width();
    $.ajax({
        url: GetUrl("Home/GetCateFooter"),
        traditional: true,
        type: 'POST',
        success: function (data) {
            //   console.log(data);
            var html = "   Product Category :  ";
            if (Window_W > 1007) {
                for (var i = 0; i < data.cates.length; i++) {
                    if (i != 0) {
                        html += '<span class="s-point footer-cate"> -  <a class="fontLightGray" style="font-size:13px !important" title="' + data.cates[i].CategoryName + '"';
                        html += 'href="' + GetUrl(LangUrl + 'Search/Product/List/Category/' + data.cates[i].CategoryID + '/1/' + data.cates[i].CategoryName) + '" ';
                        html += ' target="_blank">' + data.cates[i].CategoryName + '</a>';
                    } else {
                        html += '<span class="s-point footer-cate"> <a class="fontLightGray" style="font-size:13px !important" title="' + data.cates[i].CategoryName + '"';
                        html += 'href="' + GetUrl(LangUrl + 'Search/Product/List/Category/' + data.cates[i].CategoryID + '/1/' + data.cates[i].CategoryName) + '" ';
                        html += ' target="_blank">' + data.cates[i].CategoryName + '</a>';
                    }
                }
            } else {
                for (var i = 0; i < data.cates.length-1; i++) {
                    if (i != 0) {
                        html += '<span class="s-point footer-cate"> -  <a class="fontLightGray" style="font-size:13px !important" title="' + data.cates[i].CategoryName + '"';
                        html += 'href="' + GetUrl(LangUrl + 'Search/Product/List/Category/' + data.cates[i].CategoryID + '/1/' + data.cates[i].CategoryName) + '" ';
                        html += ' target="_blank">' + data.cates[i].CategoryName + '</a>';
                    } else {
                        html += '<span class="s-point footer-cate"> <a class="fontLightGray" style="font-size:13px !important" title="' + data.cates[i].CategoryName + '"';
                        html += 'href="' + GetUrl(LangUrl + 'Search/Product/List/Category/' + data.cates[i].CategoryID + '/1/' + data.cates[i].CategoryName) + '" ';
                        html += ' target="_blank">' + data.cates[i].CategoryName + '</a>';
                    }
                }
            }
            $('#footer-product-cate').html(html);

        }
    });
}

GenerateCateFooter();


 
    function addProductNotLogin()
    {
      
        OpenLoading(true);
        $.ajax({
            url: GetUrl("MyB2B/Product/PrepareAddProductNotLogin"),
            //data: { GenCode: ProductCode },
            success: function (data) {
                 console.log("data " + data);
                 console.log("data.isLogin " + data.isLogin);
                    //$("#'").html(data);
                    //OpenLoading(false);
                    //  $("#ModalAddProduct").modal('show');
                if (data.isLogin) {
                    OpenLoading(false);
                    window.location = GetUrl('MyB2B/Product/Index');
                } else {
                    $("#ModalAddProduct").html(data.view);
                    OpenLoading(false);
                    $("#ModalAddProduct").modal('show');
                    $("html").css('overflow', 'hidden');
                }
            },
            error: function () {
                OpenLoading(false);
                console.log("Do error");
            },
            type: "POST"
        });
    }


    $(document).on("click", ".btn_postproduct2", function (e) {
        //e.preventDefault();
        //addProductNotLogin();
        //window.location = GetUrl('Member/SignUp');
    });