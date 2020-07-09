
//$(document).ready(function ($) {
//    $('#headerOnline').hide();
//    $('.threebottom').css({ 'margin-top': '50px' });
//    $('.space').css({ 'position': 'relative', 'top': '1px' });

//    window.onscroll = function (e) {
//        var position = window.pageYOffset;
//        if (position <= 40) {
//            $('.threebottom').removeAttr('style');
//            $('#headerOnline').show();
//            $('.space').removeAttr('style');
//        } else {
//            $('#headerOnline').hide();
//            $('.threebottom').css({ 'margin-top': '50px' });
//            $('.space').css({ 'position': 'relative', 'top': '1px' });
//        }
//    }

//});

// Animate

$(document).ready(function () {
    console.log('test')

    $("btn_up").click(function () {
        $("#product-slide").animate({ height: "1250px" });
    });
    $("btn_down").click(function () {
        $("#product-slide").animate({ height: "1000px" });
    });

});

// Active

function SetNavBar() {
    var Type = $("#hidPageType").val();
    var MenuLevel = $("#hidWebCompLevel").val();
    var sum = "";

    switch (Type) {
        case "Index":
            sum = $("#Index").val();
            $('.menu li').eq(sum).addClass("Active-menu");
            $('.menudrop li').eq(sum).addClass("Active-menu");
            break;
        case "Product":
            sum = $("#Product").val();
            $('.menu li').eq(sum).addClass("Active-menu");
            $('.menudrop li').eq(sum).addClass("Active-menu");
            break;
        case "Order":
            sum = $("#Order").val();
            $('.menu li').eq(sum).addClass("Active-menu");
            $('.menudrop li').eq(sum).addClass("Active-menu");
            break;
        case "Certify":
            sum = $("#Certify").val();
            $('.menu li').eq(sum).addClass("Active-menu");
            $('.menudrop li').eq(sum).addClass("Active-menu");
            break;
        case "Blog":
            sum = $("#Blog").val();
            $('.menu li').eq(sum).addClass("Active-menu");
            //$('.menudrop li').eq(sum).addClass("Active-menu");
            $('.menudrop li.link').eq(sum - 4).addClass("Active-menu");
            $('.has-sub').addClass("Active-menu");
            break;
        case "About":
            sum = $("#About").val();
            $('.menu li').eq(sum).addClass("Active-menu");
           // $('.menudrop li').eq(sum).addClass("Active-menu");
            $('.menudrop li.link').eq(sum - 4).addClass("Active-menu");
            $('.has-sub').addClass("Active-menu");
            break;
        case "Contact":
            sum = $("#Contact").val();
            $('.menu li').eq(sum).addClass("Active-menu");
            //$('.menudrop li').eq(sum).addClass("Active-menu");
            $('.menudrop li.link').eq(sum - 4).addClass("Active-menu");
            $('.has-sub').addClass("Active-menu");
            break;
    }
}

$(function () {
    SetNavBar();
})




