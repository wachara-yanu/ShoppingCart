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

    $("#slide-feature").rcarousel(
	{
	    margin: 1,
	    visible: 5,
	    step: 4,
	    speed: 1000,
	    width: 175,
	    height: 250,
	    navigation: {
	        prev: "#ui-carousel-prev",
	        next: "#ui-carousel-next"
	    },
	    auto: { enabled: true },
	    start: generatePages,
	    pageLoaded: pageLoaded
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

    if ($('.container_home').width() <= 1100) {
        $(".footer-cate:last").hide();
    }
    var oScroll2 = $('#scrollbar2'); // Scrollbar
    if (oScroll2.length > 0) {
        oScroll2.tinyscrollbar({ axis: 'x' });
    }
});

function generatePages() {
    var _total, i, _link;
    _total = $("#slide-feature").rcarousel("getTotalPages");
    for (i = 0; i < _total; i++) {
        _link = $("<a href='#'></a>");
        $(_link)
          .bind("click", { page: i },
          function (event) {
              $("#slide-feature").rcarousel("goToPage", event.data.page);
              event.preventDefault();
          }
          )
          .addClass("bullet off page_white")
          .appendTo("#pages");
    }

    // mark first page as active
    $("a:eq(0)", "#pages")
          .removeClass("off")
          .addClass("on")
          .removeClass("page_white")
          .addClass("page_orange");
}

function pageLoaded(event, data) {
    $("a.on", "#pages")
      .removeClass("on")
      .removeClass("page_orange")
      .addClass("page_white");
    $("a", "#pages")
      .eq(data.page)
      .addClass("on")
      .removeClass("page_white")
      .addClass("page_orange");
}

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

