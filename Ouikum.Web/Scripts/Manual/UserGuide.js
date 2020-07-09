$("document").ready(function () {

    var menu = $('#menuright');
    var head = $('#bodycontent').offset().top - 200;
    //var amount = $(document).scrollTop();

    $(window).scroll(function () {
        var footer = $('#footercontent').offset().top - 400;
        var position = window.pageYOffset;
        //console.log(position);

        if (position > head && position < footer) {
            menu.css({ 'position': 'fixed', 'top': '85px', 'z-index': '2000' });
        } else {
            menu.hide();
            menu.removeAttr('style');
        }
    });
});

$(".LinkTarget").click(function () {
    //var target = $(this).attr('data-target');
    var show = $(this).attr('data-show');
    var hide = $(this).attr('data-hide');
    var arrHide = hide.split(',');
    if (arrHide.length > 0) {
        for (var i = 0; i < arrHide.length; i++) {
            $(arrHide[i]).hide();
        }
    } else {
        $(hide).hide();
    }
    $('html,body').animate({ scrollTop: 0 }, 500);
    $(show).show();
});

$(".LinkTargetIntro").click(function () {
    var target = $(this).attr('data-target');
    var show = $(this).attr('data-show');
    var hide = $(this).attr('data-hide');
    var arrHide = hide.split(',');
    if (arrHide.length > 0) {
        for (var i = 0; i < arrHide.length; i++) {
            $(arrHide[i]).hide();
        }
    } else {
        $(hide).hide();
    }
    $('html,body').animate({
        scrollTop: $('.introduction').offset().top
    }, 500);
    $(show).show();
});

//Show / Hide Top Menu
$(document).ready(function () {
    PositionPage();
    window.onscroll = function (event) {
        PositionPage();
    }
    function PositionPage() {
        var position = window.pageYOffset;
        if (position >= 40) {
            $('#headmenu').css({ 'position': 'fixed', 'top': '0px', 'z-index': '2000' });
            $('#MenuB2B').hide();
        } else {
            $('#headmenu').removeAttr('style');
            $('#MenuB2B').show();
        }
    }
});

//toggle
$(document).ready(function () {
    $('.hideToggle').hide();

    $(".Collapse").click(function () {
        var data = $(this);
        var target = data.attr('data-target');
        $(target).Toggle("fast");
    });

});

//slide up & down
$('.question').click(function () {
    var target = $(this).attr('data-target');
    $(target).slideToggle(function () {
        if ($(this).is(":visible")) {
            //console.log($(this).is(":visible"));
        } else {
            //console.log($(this).is(":visible"));
        }
    });
});

$("#introbox1").hover(
    function () {
        $("#introsub1").addClass("bg_green");
    }, function () {
        $("#introsub1").removeClass("bg_green");
    }
);

$("#introbox2").hover(
  function () {
      $("#introsub2").addClass("bg_pink");
  }, function () {
      $("#introsub2").removeClass("bg_pink");
  }
);

$("#introbox3").hover(
  function () {
      $("#introsub3").addClass("bg_yellow");
  }, function () {
      $("#introsub3").removeClass("bg_yellow");
  }
);

$("#introbox4").hover(
  function () {
      $("#introsub4").addClass("bg_blue");
  }, function () {
      $("#introsub4").removeClass("bg_blue");
  }
);

var TRange = null;
function findString(str) {
    if (parseInt(navigator.appVersion) < 4) return;
    var strFound;
    if (window.find) {

        // CODE FOR BROWSERS THAT SUPPORT window.find

        strFound = self.find(str);
        if (!strFound) {
            strFound = self.find(str, 0, 1);
            while (self.find(str, 0, 1)) continue;
        }
    }
    else if (navigator.appName.indexOf("Microsoft") != -1) {

        // EXPLORER-SPECIFIC CODE

        if (TRange != null) {
            TRange.collapse(false);
            strFound = TRange.findText(str);
            if (strFound) TRange.select();
        }
        if (TRange == null || strFound == 0) {
            TRange = self.document.body.createTextRange();
            strFound = TRange.findText(str);
            if (strFound) TRange.select();
        }
    }
    else if (navigator.appName == "Opera") {
        bootbox.alert("Opera browsers not supported, sorry...")
        return;
    }
    if (!strFound) alert("String '" + str + "' not found!")
    return;
}