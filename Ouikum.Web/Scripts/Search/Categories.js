var CurrentLanguage = $('#hidAppLang').val();
var CurrentCultureInfo = $('#hidAppLang').val();
var LangUrl = '';
//if (CurrentCultureInfo == "en-US") {
//    LangUrl = 'en/';

//}
jQuery(document).ready(function () {
    var scr = screen.width;
    var wid = $('body').width();

    $("#slide-supplier").rcarousel(
		{
		    orientation: "vertical",
		    visible: 5,
		    step: 1,
		    speed: 1000,
		    auto: {
		        enabled: true,
		        direction: "next",
		        interval: 4000
		    },
		    height: 50,
		    width: 310
		}
	);
    $("#slide-product").rcarousel(
	{
	    margin: 5,
	    visible: SetSlide(wid, "recent"),
	    step: 1,
	    speed: 1000,
	    width: 125,
	    height: 160,
	    auto: {
	        enabled: true,
	        direction: "next",
	        interval: 4000
	    }
	}
	);

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
	    }

	}
	);
    $("#ui-carousel-next")
	.add("#ui-carousel-prev")
	.hover(
		function () {
		    $(this).css("opacity", 0.7);
		},
		function () {
		    $(this).css("opacity", 1.0);
		}
	);


    // SetResponsive()
    // initialize slider  
    // Run
    //        sliderAutoplay();
    LoadB2BToday();
    B2BToday();


    SetActiveListSearch(0)
    SetFormAction('product');

    $("body").click(function (event) {

        if ($(event.target).hasClass('search-select')) {

            OpenListSearch(true);
        } else {
            OpenListSearch(false);
        }
    });

});

$('#TextSearch').keypress(function () {
    OpenListSearch(true);
});

$('#TextSearch').click(function () {
    var width = $('#search-main').width();
    $('#list-search').width(width - 2);
    OpenListSearch(true);
});

function SetSlide(width, slidename) {
    var visible = 4;
    if (slidename == "feature") {
        if (width > 1200) {
            visible = 5;
        } else if (width <= 1200) { 
            visible = 4;
        }
    } else if (slidename == "recent") {
        if (width > 1200) {
            visible = 4;
        } else if (width <= 1200) {
            visible = 3;
        }
    }

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

function SetFormAction(val) {
    url = GetUrl('Search/Product');
    if (val == 'product') {
        url = GetUrl('Search/Product/List');
        $('#TextSearch').attr('placeholder', label.vldsearch_product);

    } else if (val == 'buyer') {
        url = GetUrl('Purchase/Search');
        $('#TextSearch').attr('placeholder', label.vldsearch_buylaed);

    } else if (val == 'supplier') {
        url = GetUrl(LangUrl+'Search/Supplier/List');
        $('#TextSearch').attr('placeholder', label.vldsearch_supplier);

    }
    $('#frmSearch').attr('action', url);
    //console.log('form : ' + $('#frmSearch').attr('action'));

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
            ////console.log(data);
            $(".product-all").html(data.ProductAll);
            $(".supplier-all").html(data.SupplierAll);
        },
        type: "POST"
    });
    setTimeout('LoadB2BToday()', 60000);
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

//    function ClearSlideRecently() {
//        var len_supplier = $('#slide-supplier > li').length;
//        ////console.log(len_supplier); 
//        if (len_supplier > 10) {
//            ////console.log('start ');
//            for (var i = 0; i < len - 10; i++) {
//                ////console.log($('#' + obj + ' > li').eq(i).html());
//                $('#' + obj + ' > li').eq(i).remove();
//            }
//        } 
//    }

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
 


