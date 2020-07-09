var CurrentLanguage = $('#hidAppLang').val();
var CurrentCultureInfo = $('#hidAppLang').val();
var langurl = '';
//if (CurrentLanguage == "en-US") {
//    langurl = 'en/';
//}

   

jQuery(document).ready(function () {
    var scr = screen.width;
    var silde = 2;
    var wid = $('body').width();
    var w_rcarousel = 372;
    if (wid > 670) {
        silde = 3;
        w_rcarousel = 255;
    }
    if (wid > 1024) {
        silde = 4;
    }
    var mySwiper = new Swiper('.swiper-container', {
        pagination: '.pagination',
        loop: true,
        grabCursor: true,
        slidesPerView: silde,
        paginationClickable: true,
    })
    $('.arrow-left').on('click', function (e) {
        e.preventDefault()
        mySwiper.swipePrev()
    })
    $('.arrow-right').on('click', function (e) {
        e.preventDefault()
        mySwiper.swipeNext()
    })
        //$("#slide-supplier").rcarousel(
        //    {
        //        orientation: "vertical",
        //        visible: 3,
        //        step: 1,
        //        speed: 1000,
        //        auto: {
        //            enabled: true,
        //            direction: "next",
        //            interval: 4000
        //        },
        //        height: 60,
        //        width: w_rcarousel
        //    }
        //);
    // ใช้เฉพาะ otop ให้ปิดไปก่อน ยังไม่ใช้
    //$("#slide-productshowcase").rcarousel(
	//{
	//    visible: 4,
	//    step: 4,
	//    width: 244,
	//    height: 250,
	//    margin: 0,
	//    auto: {
	//        enabled: true,
	//        interval: 5000,
	//        direction: "prev"
	//    }
	//});

    $("#slide-product").rcarousel(
	{
	    margin: 5,
	    visible: SetSlide(wid, "recent"),
	    step: 1,
	    speed: 1000,
	    width: 140,
	    height: 200

	}
	);
    $("#SlideHotProduct").rcarousel({
        visible: 1,
        step: 1,
        speed: 500,
        width: 140,
        height: 140
    });
   
    $("#slide-feature").rcarousel(
	{
	    margin: 1,
	    visible: 5,
	    step: 4,
	    speed: 1000,
	    width: 200,
	    height: 250,
	    navigation: {
	        prev: "#ui-carousel-prev",
	        next: "#ui-carousel-next"
	    },
	    auto: {enabled: true}
	});

    $("#ui-carousel-next")
	.add("#ui-carousel-prev")
	.hover(
		function () {
		    $(this).css("opacity", 1.0);
		},
		function () {
		    $(this).css("opacity", 0.5);
		}
	);
    //test
    //$("#slide-partner").rcarousel(
	//{
	//    orientation: "horizontal",
	//    visible: 6,
	//    step: 2,
	//    speed: 980,
	//    auto: {
	//        enabled: true,
	//        direction: "next",
	//        interval: 6000
	//    },
	//    height: 55,
	//    width: 165
	//}
	//);
    // end test
    if ($('.container_home').width() <= 1100) {
        $(".footer-cate:last").hide();
    }
    var oScroll2 = $('#scrollbar2'); // Scrollbar
    if (oScroll2.length > 0) {
        oScroll2.tinyscrollbar({ axis: 'x' });
    }
    //        sliderAutoplay();
    LoadB2BToday();
    B2BToday();
    checkIEVersion();

    SetActiveListSearch(0)

    $("body").click(function (event) {

        if ($(event.target).hasClass('search-select')) {

            OpenListSearch(true);
        } else {
            OpenListSearch(false);
        }
    });



});

//$('#TextSearch').keypress(function () {
//    OpenListSearch(true);
//});

//$('#TextSearch').click(function () {
//    var width = $('#search-main').width();
//    $('#list-search').width(width - 2);
//    OpenListSearch(true);
//});
 
function SetSlide(width, slidename) {
    var visible = 4;
    if (slidename == "feature") {
        //        if (width > 1200) {
        //            visible = 5;
        //        } else if (width <= 1200) { 
        visible = 4;
        //        }
    } else if (slidename == "recent") {
        if (width < 680) {
            visible = 2;
        } else {
            visible = 4;
        }

    }
    //    }

    return visible;
}
function SetResponsive() {

    var body_w = $('body').width();
    //console.log(body_w);
    if (body_w < 1200) {
        $('#menucategory').addClass('hide');
        $('#main-right').removeClass('span9');
        $('#main-right').addClass('span12');

        $('.cate_showcase').removeClass('span2');
        $('.cate_showcase').addClass('span3');
        $('.todayzone').removeClass('span3');
        $('.todayzone').addClass('span4');


    } else {
        $('#menucategory').removeClass('hide');
        $('.cate_showcase').removeClass('span3');
        $('.cate_showcase').addClass('span2');
        $('.todayzone').removeClass('span4');
        $('.todayzone').addClass('span3');
    }
}
function setVisibleSlideProduct() {
    var scr = screen.width;
    console.log('setVisibleSlideProduct : ' + scr);
    if (scr <= 1024) {
        return 3
    }
    else if (scr == 800) {
        return 3
    }
    else {
        return 4
    }
}
function setVisibleSlideFeat() {
    var scr = screen.width;
    console.log('setVisibleSlideFeat : ' + scr);
    if (screen.width == 1024) {
        return 4
    }
    else if (screen.width == 800) {
        return 3
    }
    else {
        return 5
    }
}
function SetActiveListSearch(index) {
    $('.list-search-text').removeClass('active');
    $('.list-search-text').eq(index).addClass('active');

    $('.icon-active').hide();
    $('.icon-active').eq(index).show();
}
function OpenListSearch(isOpen) {
    if (isOpen != null && isOpen != undefined) {
        if (isOpen) {
            $('#list-search').removeClass('hidden')
            $('#list-search').slideDown();
        } else {
            $('#list-search').addClass('hidden')
            $('#list-search').slideUp();
        }
    } else {
        if ($('#list-search').hasClass('hidden')) {
            $('#list-search').removeClass('hidden')
            $('#list-search').slideDown();
        } else {
            $('#list-search').addClass('hidden')
            $('#list-search').slideUp();
        }
    }
}
// slider autoplay function
var sliderTimeout = null;
//    function sliderAutoplay() {

//        $('#slider1').trigger('nextSlide');
//        sliderTimeout = setTimeout('sliderAutoplay()', 6000);
//    }
function LoadB2BToday() {
    $.ajax({
        url: GetUrl("Home/B2BToday"),
        traditional: true,
        success: function (data) {
            $(".product-all").text(data.ProductAll);
            $(".product-central").text(data.Central);
            $(".product-west").text(data.West);
            $(".product-north").text(data.North);
            $(".product-northeast").text(data.Northeast);
            $(".product-south").text(data.South);
            $(".product-east").text(data.East);
            $(".product-supplierAll").text(data.SupplierAll);
            
            //var SupplierAll = "";
            //var SoftwareAll = "";
            //for (var i = 0; i < data.ProductAll.length ; i++) {
            //    if (data.ProductAll[i] == ",") {
            //        ProductAll += '<span class="mar_r5 fontNormal b mar_t5">' + data.ProductAll[i] + '</span>';
            //    }
            //    else {
            //        ProductAll += '<span class="label label-inverse font_xl padd_l5 padd_r5 mar_r5">' + data.ProductAll[i] + '</span>';
            //    }
            //}
            //for (var i = 0; i < data.SupplierAll.length ; i++) {
            //    if (data.SupplierAll[i] == ",") {
            //        SupplierAll += '<span class="mar_r5 fontNormal b mar_t5">' + data.SupplierAll[i] + '</span>';
            //    }
            //    else {
            //        SupplierAll += '<span class="label label-inverse font_xl padd_l5 padd_r5 mar_r5">' + data.SupplierAll[i] + '</span>';
            //    }
            //}
            //for (var i = 0; i < data.SoftwareAll.length ; i++) {
            //    if (data.SoftwareAll[i] == ",") {
            //        SoftwareAll += '<span class="mar_r3 fontNormal b mar_t5">' + data.SoftwareAll[i] + '</span>';
            //    }
            //    else {
            //        SoftwareAll += '<span class="label btn-inverse font_s padd_l5 padd_r5 mar_r3">' + data.SoftwareAll[i] + '</span>';
            //    }
            //}
            //$(".supplier-all").html(SupplierAll);
            //$(".software-all").html(SoftwareAll);
        },
        type: "POST"
    });
    //setTimeout('LoadB2BToday()', 50000);
}

function B2BToday() {
    if ($(".product-today").css('display') == 'block' || $(".product-today").css('display') == 'inline-block') {
        $(".product-today").hide();
        $(".buylead-today").fadeIn();
    } else {
        $(".buylead-today").hide();
        $(".product-today").fadeIn();
    }
    setTimeout('B2BToday()', 6000);
}

    function ClearSlideRecently() {
        var len_supplier = $('#slide-supplier > li').length;
        ////console.log(len_supplier); 
        if (len_supplier > 10) {
            ////console.log('start ');
            for (var i = 0; i < len - 10; i++) {
                ////console.log($('#' + obj + ' > li').eq(i).html());
                $('#' + obj + ' > li').eq(i).remove();
            }
        } 
    }

function SubStringText() {
    var len = $('.substr').length;
    for (var i = 0; i < len; i++) {
        var str = $('.substr').eq(i).text();
        var lenStr = str.length;
        ////console.log(str + " : " + lenStr);
        if (lenStr > 70) {
            str = str.substring(0, 70) + ".."
        }
        $('.substr').eq(i).text(str);
    }

}

/*---------------CategoryShowCase------------------*/
function ShowContent(id,action) {
    var color="";
        switch (id) {
            case 0: color = "#FF6600"; break;
            case 1: color = "#FF6699"; break;
            case 2: color = "#009900"; break;
            case 3: color = "#0066CC"; break;
            default: break;
    }
    $('.textH').css('color', '#999999');
    if (action == "out") {
        if ($('#TabContent').attr('color-id') != null) {
            $('.textH').eq($('#TabContent').attr('cid')).css('color', $('#TabContent').attr('color-id'));
            $('#TabHome').css('border-bottom', '1px solid ' + $('#TabContent').attr('color-id'));
        }
        else {
            $('.textH').eq(0).css('color', '#FF6600');
            $('#TabHome').css('border-bottom', '1px solid #FF6600');
        }
    }
    else {
            $('.textH').eq(id).css('color', color);
            $('#TabHome').css('border-bottom', '1px solid ' + color)
    }
    if (action == "active") {
        $('.showtab').css('display', 'none');
        $('.showtab').eq(id).fadeIn();
        $('.showtab').eq(id).css('display', 'block');
        $('#TabContent').attr('color-id', color).attr('cid',id);
    }
}

function LoadCateLv2() {
    var cateid = 0;
    var catename = "";

    $('li.cate-item').each(function (index) {
        cateid = $(this).attr('data-id');
        catename = $(this).attr('data-name');
        PostSoftwareLv3(cateid, catename, 1, 14);
    });
}

function LoadSubCate() {

    var cateid = 0;
    var catename = "";
    var len = $('li.cate-item').length;
    var cateid = new Array();
    for (var i = 0; i < len; i++) {
        cateid[i] = parseInt($('li.cate-item').eq(i).attr('data-id'));
    }
    model = {
        cateid: cateid
    }
    console.log(model);
    $.ajax({
        url: GetUrl("Home/LoadCateLV2"),
        data: model,
        type: "POST",
        dataType: 'json',
        traditional: true,
        success: function (data) {
            console.log(data);
            //var html = "";
            //html += "<li class='sub-cate-item'>";
            //html += "<a class='sub-cate-name' href='" + GetUrl("Search/Product/List/Category/" + data.CateLV3[i].CategoryID + "/" + data.CateLV3[i].CategoryLevel + "/" + ReplaceUrl(data.CateLV3[i].CategoryName)) + "'>";
            //html += data.CateLV3[i].CategoryName + "</a>";
            //html += "</li>";

        }
    });

}
function LoadSoftwareLv3() {
    var cateid = 0;
    var catename = "";

    $('li.cate-item').each(function (index) {
        cateid = $(this).attr('data-id');
        catename = $(this).attr('data-name');
        PostSoftwareLv3(cateid, catename, 2, 9);
    });
}
function LoadOtopCategory() {
    var cateid = 0;
    var catename = "";

    $('li.cate-item').each(function (index) {
        cateid = $(this).attr('data-id');
        catename = $(this).attr('data-name');
        PostSoftwareLv3(cateid, catename, 2, 9);
    });

    ListProvince(0, "ddlCategories");
    $("#ddlCategories").attr("name", "Province");
    $("#ddlCategories").attr("onchange", "SelectedCategories('Province')");
    $(".telservice").css("color", "#333333");
    $(".telservice .icon-phone").css("display", "inline-block");
}
function PostSoftwareLv3(CateID, CateName, Type, Count) {
    var CateLv2 = "";
    var strCate = "";
    $.ajax({
        url: GetUrl("Home/LoadSoftwareCategory"),
        data: { CateID: CateID, Type: Type },
        type: "POST",
        dataType: 'json',
        success: function (data) {
            if (data != null) {
                if (Type == 3 && data.CateLV3.length > 0) {
                    //var Otopcate = "<li class='nav-header nav-cate mar_b10' style='padding:5px;'>Otop Category</li> ";
                    //for (var i = 0; i < Count; i++) {
                    //    Otopcate += "<li class='cate-item cate-"+data.CateLV3[i].CategoryID+"' data-id='"+data.CateLV3[i].CategoryID+"' data-name='"+ReplaceUrl(data.CateLV3[i].CategoryName)+"' style='max-height:28px;'>";
                    //    Otopcate += "<a target='_blank' class='cate' style='padding:3px 5px' href='" + GetUrl("Search/Product/List/Category/" + data.CateLV3[i].CategoryID + "/" + data.CateLV3[i].CategoryLevel + "/" + ReplaceUrl(data.CateLV3[i].CategoryName)) + "'>";
                    //    Otopcate += data.CateLV3[i].CategoryName + "</a></li>";
                    //}
                    //Otopcate+="</ul></div>";
                    //$("#menucategory").prepend(Otopcate);
                }
                else {
                    if (data.CateLV3.length > Count) {
                        for (var i = 0; i < Count; i++) {
                            CateLv2 += "<li class='sub-cate-item'>"; 
                            CateLv2 += "<a class='sub-cate-name' href='" + langurl + "Search/Product/List/Category/" + data.CateLV3[i].CategoryID + "/" + data.CateLV3[i].CategoryLevel + "/" + ReplaceUrl(data.CateLV3[i].CategoryName).trim() + "'>";
                            //if (CurrentCultureInfo == "en-US") { CateLv2 += data.CateLV3[i].CategoryNameEng }
                            //else {
                            //    CateLv2 += data.CateLV3[i].CategoryName;

                            //}
                            CateLv2 += data.CateLV3[i].CategoryName; "</a>";
                            CateLv2 += "</li>";
                        }
                       // CateLv2 += "<li class='sub-cate-item' ><a class='sub-cate-name' href='" + GetUrl("Search/Product/List/Category/" + CateID + "/2/" + CateName) + "'>เพิ่มเติม</a></li>"
                    } else {
                        for (var i = 0; i < data.CateLV3.length; i++) {
                            CateLv2 += "<li class='sub-cate-item'>";
                            CateLv2 += "<a class='sub-cate-name' href='" + langurl+"Search/Product/List/Category/" + data.CateLV3[i].CategoryID + "/" + data.CateLV3[i].CategoryLevel + "/" + ReplaceUrl(data.CateLV3[i].CategoryName).trim() + "'>";
                            //if (CurrentCultureInfo == "en-US") { CateLv2 += data.CateLV3[i].CategoryNameEng }
                            //else {
                            //    CateLv2 += data.CateLV3[i].CategoryName;

                            //}
                            CateLv2 += data.CateLV3[i].CategoryName; "</a>";
                            CateLv2 += "</li>";
                        }
                    }
                    strCate = "<div id='" + CateID + "'><li class='font-cate-head'><a class='fontNormal sub-cate-name' href='" + langurl+"Search/Product/List/Category/" + CateID + "/"+ Type +"/" + ReplaceUrl(CateName) + "'>" + CateName + " </a></li><ul class='sub-cate-list nav' data-id='cate-" + CateID + "'>" + CateLv2;
                    strCate += " </ul></div>"
                    $("#hidSubcate").append(strCate);
                }
            }
        }
    });
    $("#hidSubcate").addClass('hide');
}
//On hover Category menu
$(".cate-item").hover(  
  function () {
      var cateid = $(this).attr('data-id');
      $(this).children(".subCateID").html($("#" + cateid).html());
      $(this).children(".subCateID").css("display", "block");
      $(".cate-item").children().removeClass('cate-current');
      $(this).children().addClass('cate-current');
      $('#cate-list').addClass('cate-list-hover');
      $(this).css('max-height', '29px');
  },
  function () {

      if ($("#subcate").hasClass("subcate-hover")) {
          $("#subcate").removeClass('hide');
          
      } else {
          $("#subcate").addClass('hide');
          $(".cate-item").children().removeClass('cate-current');
          $(this).find('.cate-point').text('');
          $('#cate-list').removeClass('cate-list-hover');
          $(".cate-item").children(".subCateID").css("display", "none");
          $(this).css('max-height', '32px');
      }
  }
);
$("#subcate").hover(
  function () {
      $("#subcate").addClass('subcate-hover');
      $("#subcate").removeClass('hide');
      var cateclass = $("#subcate").children('ul').attr('data-id');
      $(".cate-item").children().removeClass('cate-current');
      $("." + cateclass).children().addClass('cate-current');
      $('#cate-list').addClass('cate-list-hover');
  },
  function () {
      $(".cate-item").children().removeClass('cate-current');
      $("#subcate").removeClass('subcate-hover');
      $("#subcate").addClass('hide');
      $('#cate-list').removeClass('cate-list-hover');
  }
);
function SelectedCategories(Obj) {
    SetFormAction(Obj);
}
$(".newprosection").hover(
  function () {
      $(this).find('.bg_newproduct').slideDown('slow');
      $(this).find('.bg_newproduct').css("display", "block");
      $(this).find('.str_newproduct').css("color", "#005DCC");
      $(this).find('.fontgraylight').css("color", "#999999");
  },
  function () {
      $(this).find('.bg_newproduct').css("display", "none");
      $(this).find('.str_newproduct').css("color", "transparent");
      $(this).find('.fontgraylight').css("color", "transparent");
  }
);
//-----------------mapsetnewproduct-----------------
function setsectionproduct(model,Region) {
    var count = 0; var borderbottom = ""; var Price = ""; var NewProductName = ""; var ProPrice="";
    var html = "<div style='width:100%;height:35px'><div class='fl_l mar_l10 bg_Regionpro" + Region + "'></div></div>";
    
    $.each(model, function (i, item) {
        if (item.ProductName.length > 25) {
            NewProductName = item.ProductName.substring(0, 20) + "...";
        }
        else {
            NewProductName = item.ProductName;
        }
        if (count < 3) {
            borderbottom = "border-bottom:2px solid #F9F9F9";
        }
        else {
            borderbottom = "";
        }
        if (item.IsPromotion == true) {
            Price = '<div class="muted fl_r" style="text-decoration:line-through;">' + (item.Price).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + ' บาท</div>';
        }
        else {
            Price = '<div class="b fl_r" style="color:#FD7C16;">' + (item.Price).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + ' บาท</div>';
        }
        if (item.PromotionPrice > 0) {
            ProPrice = '<div class="clean"></div> <span class="b fl_r" style="color:#FD7C16;">' + (item.PromotionPrice).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + ' บาท</span>';
            }
        else{
            ProPrice = "";
        }

        html += "<div class='fl_l' style='padding:0;margin-left:-2px;border-left:1px solid #eeeeee;" + borderbottom + "'>";
        html += '<a target="_blank" class="sectionshowcase"  href="' + GetUrl('Search/Product/Detail/' + item.ProductID + '/' + item.ProductName.replace(" ", "").replace(",", "-").replace("&", "And")) + '" >';
        html += '<img onload="resizeImg($(this),120,120);setElementMiddle(120, 120,$(this));" src="' + GetUpload("Product/" + item.CompID + "/" + item.ProductID + "/Thumb_" + item.ProductImgPath) + '"></a>';
        html += '<div class="clean"></div><div class="padd_5 mar_10" style="width:195px;margin-top:-25px;border-top:1px solid #eeeeee;height:50px;">';
        html += ' <a target="_blank" href="' + GetUrl('Search/Product/Detail/' + item.ProductID + '/' + ReplaceUrl(item.ProductName)) + '" >';
        html += '<div  class="txtBrown mar_t5">' + NewProductName + '</div> </a><div class="txtLinks fl_l">' + item.ProvinceName + '</div>';
        html += Price + ProPrice+'</div><div class="clean3"></div></div>';
        count++;
    });
    $("#SectionProduct").html(html);
}
function ReplaceUrl(name) {
    var Rename = name.replace(" ", "");
    Rename = Rename.replace(",", "-").replace("+", "-").replace("&", "-").replace("#", "-").replace("[", "-").replace("]", "-").replace("'", "-").replace("/", "-").replace(".", "").replace("%", "").replace(":", "");
    return Rename;
}

$(function () {
    $('#btn-LinkDesktop').click(function () {
        ChangCoverType("Desktop");
        window.location = GetUrl("Home/Index");
    });

    $('#btn-LinkMobile').click(function () {
        ChangCoverType("Mobile");
        window.location = GetUrl("B2BMobile/Index");
    });
});

function ChangCoverType(Type) {
    deleteCookie("CoverType", "/");
    setCookie("CoverType", Type, "1", "/");
}

function getInternetExplorerVersion()
    // Returns the version of Windows Internet Explorer or a -1
    // (indicating the use of another browser).
{
    var rv = -1; // Return value assumes failure.
    if (navigator.appName == 'Netscape') {
        var ua = navigator.userAgent;
        var re = new RegExp("MSIE ([0-9]{1,}[\.0-9]{0,})");
        if (re.exec(ua) != null)
            rv = parseFloat(RegExp.$1);
    }
    return rv;
}

function checkIEVersion() {
    var isIE = getCookie("isIE");
    var ver = getInternetExplorerVersion();
    if (isIE == null || isIE != ver) {
        var msg = "You're not using Windows Internet Explorer.";
        
        if (ver > -1) {
            if (ver <= 8.0) {
                msg = '"Internet Explorer ของคุณมีเวอร์ชั่นที่ต่ำกว่าปัจจุบัน อาจทำให้การแสดงผลเว็บไซต์มีข้อผิดพลาดเกิดขึ้น เพื่อการใช้งานเว็บไซต์ได้อย่างเต็มประสิทธิภาพ กรุณาอัพเกรดเวอร์ชั่น Internet Explorer ของคุณ';
                bootbox.alert(msg);
                ChangIsIE(ver)
            }
        }
    }
}

function ChangIsIE(ver) {
    var isIE = getCookie("isIE");
    if (isIE == null || isIE != ver) {
        deleteCookie("isIE", "/");
        setCookie("isIE", ver, "1", "/");
    }
}