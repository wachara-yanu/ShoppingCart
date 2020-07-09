
var CompareName = 'campare';
// button & event
// ปุ่ม add compare ใช้ หน้า product detail
$('.product-detail-compare').live('click', function () {
    var productid = parseInt($(this).attr('data-id'), 10);
    var productname = $(this).attr('data-name');
    var productimg = $(this).attr('data-img');
    if (AddCompareCookie(productid, productname, productimg)) {
        bootbox.alert(label.vldcompare_success);
    }
    CallCompare();
});

// ปุ่ม add compare ใช้ หน้า search
$('.compare-item').live('click', function () {
    var index = $(".compare-item").index(this);
    SaveCompareCookie(index);
    CallCompare();
});

// ปุ่ม add compare ที่ thick ใน หน้า search
$('.compare-all').live('click', function () {
    SaveCompareCookie();
    CallCompare();
});

// ปุ่ม add compare ที่ thick ใน หน้า search
$(".remove-compare").live("click", function () {
    var id = parseInt($(this).attr('data-id'), 10);
    var index = $(".remove-compare").index($(this));
    $('.data-item-compare').eq(index).remove();
    RemoveCompareCookie(id);
    CallCompare();
});

$(".remove-all").live("click", function () {
    DestroyCompareCookie();
    CallCompare();
});

$(".call-compare").live("click", function () {
    CallCompare();
});

$(".compare-now").live("click", function () {
    CompareItemNow();
});
$(".send-compare").live("click", function () {
    SendToFriend();
});

//----------------------------------------------- func  

// open compare task
function CallCompare() {
    if ($('#BodyCompare').hasClass("hide")) {
        $('#BodyCompare').removeClass("hide");
        $('#BodyCompare').show("slide", { direction: "left" }, 500);
    }
    GenerateCompareItem();
}

// close compare task
function CloseCompare() {
    if (!$('#BodyCompare').hasClass("hide")) {
        $('#BodyCompare').addClass("hide");
        $('#BodyCompare').hide("slide", { direction: "left" }, 500);
    }
}
// go to compare page
function CompareItemNow() {
    var proid = "";
    var objCompare = GetCompareCookie();
    if (objCompare != null && objCompare != undefined) {
        if (objCompare.length > 0) {
            $.each(objCompare, function (idxProduct, obj) {
                proid += obj.productid + ",";
            });
            proid = proid.substring(0, proid.length - 1);
            window.open(GetUrl("Store/Compare/?ProID=" + proid));
        } else {
            window.open(GetUrl("Store/Compare/?ProID=0"));
        }
    } else {
        bootbox.alert(label.vldcompare_choose);
        return false;
    }
}

function SendToFriend() {
    var proid = "";
    var type = "SendtoFriend"
    var objCompare = GetCompareCookie();
    if (objCompare != null && objCompare != undefined) {
        if (objCompare.length > 0) {
            $.each(objCompare, function (idxProduct, obj) {
                proid += obj.productid + ",";
            });
            proid = proid.substring(0, proid.length - 1);
            window.open(GetUrl("Message/Contact?ProductID=" + proid + "&type=" + type));
        } else {
            window.open(GetUrl("Message/Contact?ProductID=0" + "&type=" + type));
        }
    } else {
        bootbox.alert(label.vldcompare_choose);
        return false;
    }
}


function SaveCompareCookie(index) {
    // ใช้เก็บค่าเพื่อที่จะไปแสดง compare
    productid = 0;
    productname = '';
    productimg = '';
    var j = 0;
    if (index != null && index != undefined) {
        productid = parseInt($('.data-item').eq(index).attr('data-id'), 10);
        productname = $('.data-item').eq(index).attr('data-name');
        productimg = $('.data-item').eq(index).attr('data-img');
        // add value to cookie
        AddCompareCookie(productid, productname, productimg);
    } else {
        var len = $('.ChooseProduct').length;
        var j = 0;
        for (var i = 0; i < len; i++) {
            if ($('.ChooseProduct').eq(i).attr("value") == true || $('.ChooseProduct').eq(i).attr("checked") == "checked") {
                if (CheckMaxCompareCookie()) {
                    var value = parseInt($('.data-item').eq(i).attr('data-id'), 10);
                    if (value > 0) {
                        productid = parseInt($('.data-item').eq(i).attr('data-id'), 10);
                        productname = $('.data-item').eq(i).attr('data-name');
                        productimg = $('.data-item').eq(i).attr('data-img');
                        // add value to cookie
                        AddCompareCookie(productid, productname, productimg);
                        j++;
                    }
                } else {
                    return false;
                }
            }
        }
        if (j == 0) {
            bootbox.alert(label.vldcompare_choose);
            return false;
        }

    }
}




function GetCompareCookie() {
    var objCookie = $.JSONCookie(CompareName);
    if (objCookie.Compare != null && objCookie.Compare != undefined) {
        //console.log('len : ' + objCookie.Compare.length);
        return objCookie.Compare;
    }

}


function RemoveCompareCookie(id) {
    var productid = parseInt(id, 10);
    var objCookie = $.JSONCookie(CompareName);
    var objCompare = objCookie.Compare;
    var index = findIndexByKeyValue(objCookie.Compare, "productid", productid);
    //console.log('RemoveCompareCookie : '+index);
    if (index != null) {
        //console.log('splice : ' + index);
        objCompare.splice(index, 1);
        //console.log(objCompare);
        $.JSONCookie(CompareName, objCookie, { path: '/' });
        return true;
    } else {
        return false;
    }
}
function CheckMaxCompareCookie() {
    // ตรวจ compare ว่าเกินละยัง
    // return true = ผ่าน
    //console.log('CheckMaxCompareCookie');
    var objCookie = $.JSONCookie(CompareName);
    if (objCookie.Compare != null && objCookie.Compare != undefined) {
        var len = objCookie.Compare.length;
        //console.log(len);
        if (len < 5) {
            //จำนวนน้อยกว่า 5 add ได้
            return true;
        } else {
            bootbox.alert(label.vldcompare_max);
            return false;
        }
    } else {
        // ไม่มี ค่า
        return true;
    }
}

function CheckExistCompareCookie(id) {
    // return true = ผ่าน
    //console.log('CheckExistCompareCookie :'+id);
    var objCookie = $.JSONCookie(CompareName);
    if (objCookie.Compare != null && objCookie.Compare != undefined) {
        var objCompare = objCookie.Compare;
        //console.log(objCompare);
        var Filter = objCompare.filter(function (ele, i) { return ele.productid == id });
        //console.log('Filter : ' + Filter.length);
        if (Filter.length > 0) {
            // ซ้ำ
            bootbox.alert(label.vldcompare_exist);
            return false;
        } else {
            return true;
        }

    } else {
        return true
    }
}

function GenerateCompareItem() {
    var objCompare = GetCompareCookie();
    var i = 1;
    $('.data-item-compare').remove();
    if (objCompare != null && objCompare != undefined) {
        if (objCompare.length < 5) {
            $('#ProductComparItem').show();
        } else {
            $('#ProductComparItem').hide();
        }
        var html = "";

        $.each(objCompare, function (idxProduct, obj) {
            //            console.log(idxProduct);
            //            console.log(obj.productid + ' , ' + obj.productname + ' , ' + obj.productimg);
            var name = SubStringName(obj.productname, 65);
            html += "<div  class=' fl_l mar_r10 data-item-compare' "
            html += " data-id='" + obj.productid + "' title='" + obj.productname + "'>";
            html += "<div style='width:138px; '  class='fl_l'>";
            html += "<a class='img-polaroid compare-object' target='_blank' style='width:125px;height:140px; display:block; ' href='" + GetUrl("Search/Product/Detail/" + obj.productid + "/" + obj.productname.replace(" ", "").replace(",", "-")) + "' > ";
            html += "<img alt='" + obj.productname + "' title='" + obj.productname + "' class='' src='" + obj.productimg + "' ";
            html += " style='width:125px;height:140px; position:absolute;' onload='resizeImg($(this),125,125);setElementMiddle(125,125 ,$(this) '> ";
            html += "<span class='compare-title ' style='padding:5px; width:115px; height:130px; opacity:0.7; background:#000; color:#fff; position:absolute; z-index:9;'>" + name + "</span>";
            html += "</a>";
            html += "<div class='clean'></div>";
            //   html += "<p title='" + obj.productname + "' >" + name + "</p>";
            html += "</div>";
            html += "<i class='icon-remove cursor fl_r remove-compare' data-id='" + obj.productid + "' ></i>";
            html += "</div>";
        });
        $('#trash').prepend(html);


    } else {
        $('#ProductComparItem').show();
    }
}

$('.compare-object').live('mouseenter', function () {
    var index = $('.compare-object').index(this);
    $('.compare-title').eq(index).fadeIn(200);
    console.log('mouseenter');
});

$('.compare-object').live('mouseleave', function () {
    var index = $('.compare-object').index(this);
    $('.compare-title').eq(index).hide();
    console.log('mouseout');
});

function AddCompareCookie(id, name, img) {
    if (CheckMaxCompareCookie()) {
        if (CheckExistCompareCookie(id)) {
            var objCookie = $.JSONCookie(CompareName);
            var objNewCompare = {
                productid: id,
                productname: name,
                productimg: img
            }
            if (objCookie.Compare != null && objCookie.Compare != undefined) {
                //console.log('not null');
                objCookie.Compare.push(objNewCompare);
            } else {
                //console.log('[push]');
                objCookie.Compare = [];
                objCookie.Compare.push(objNewCompare);

            }
            //console.log(objCookie.Compare);
            $.JSONCookie(CompareName, objCookie, { path: '/' });
            return true;
        } else {
            return false;
        }
    } else {
        return false;
    }
}

function DestroyCompareCookie() {
    var objCookie = $.JSONCookie(CompareName);
    objCookie.Compare = null;
    //console.log(objCookie.Compare);
    $.JSONCookie(CompareName, objCookie, { path: '/' });
}

