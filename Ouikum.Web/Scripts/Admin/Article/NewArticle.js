$(function () {
    var winwidth = $(window).width();
    if (winwidth < 1500) {
        if (winwidth >= 1024) {
            $("#ImgBlog").addClass("mar_l150");
            $("#divCompList").addClass("mar_l150");
            $("#ShowComp").addClass("mar_l150");
        }
    }

    $("em").remove();
    $(".t-upload-button > span").remove();
    $('#submit').click(function () {
        if (checkError()) {
            $('#submit').submit();
        }
        else {
            return false;
        }
    });
    $.validator.addMethod("select", function (value, element, arg) {
        return arg != value;
    });

    $('#NewArticle_Form').validate(
             {
                 onkeydown: false,
                 onkeyup: false,
                 rules: {
                     ArticleName: {
                         required: true,
                         minlength: 4
                     },
                     ArticleTypeID: {
                         select: 0
                     },
                     ImgPath: {
                         required: true
                     }
                 },
                 messages: {
                     ArticleName: {
                         required: label.vldrequired,
                         minlength: label.vldless_4char
                     },
                     ArticleTypeID: {
                         select: label.vldselectarticletype
                     },
                     ImgPath: {
                         required: label.vldrequired
                     }
                 },
                 highlight: function (label) {
                     $(label).closest('.control-group').removeClass('success');
                     $(label).closest('.control-group').addClass('error');

                 },
                 success: function (label) {
                     label.closest('.control-group').removeClass('error');
                     label.closest('.control-group').addClass('success');
                 }
             });

});
function checkError() {
    if ($('.control-group').hasClass('error')) {
        $("#submit").attr('disabled', true);
        return false;
    } else {
        $("#submit").attr('disabled', false);
        return true;
    }
}


$(function () {
    $('#NewType_Form').validate(
        {
        onkeydown: false,
        onkeyup: false,
        rules: {
            ArticleTypeName: {
                required: true,
                minlength: 4
            },
            ArticleTypeID: {
                select: 0
            }
        },
        messages: {
            ArticleTypeName: {
                required: label.vldrequired,
                minlength: label.vldless_4char
            },
            ArticleTypeID: {
                select: label.vldselectarticletype
            }
        },
        highlight: function (label) {
            $(label).closest('.control-group').removeClass('success');
            $(label).closest('.control-group').addClass('error');

        },
        success: function (label) {
            label.closest('.control-group').removeClass('error');
            label.closest('.control-group').addClass('success');
        }
    });

});
function checkError() {
    if ($('.control-group').hasClass('error')) {
        $("#submit").attr('disabled', true);
        return false;
    } else {
        $("#submit").attr('disabled', false);
        return true;
    }
}

tinyMCE.init({
    // General options
    mode: "exact",
    elements: 'Description',
    theme: "advanced",
    height: "300",
    width: "80%",
    plugins: "autolink,lists,spellchecker,pagebreak,style,layer,table,save,advhr,advimage,advlink,emotions,iespell,inlinepopups,insertdatetime,preview,media,searchreplace,print,contextmenu,paste,directionality,fullscreen,noneditable,visualchars,nonbreaking,xhtmlxtras,template",

    // Theme options
    theme_advanced_buttons1: "bold,italic,underline,strikethrough,|,justifyleft,justifycenter,justifyright,justifyfull,|,formatselect,fontselect,fontsizeselect",
    theme_advanced_buttons2: "cut,copy,paste,pastetext,pasteword,|,search,|,bullist,numlist,|,outdent,indent,|,undo,redo,|,link,unlink,image,cleanup,code,|,preview,|,forecolor,backcolor",
    theme_advanced_buttons3: "tablecontrols,|,hr,removeformat,visualaid,|,sub,sup,|,iespell,media,|,fullscreen",
    theme_advanced_toolbar_location: "top",
    theme_advanced_toolbar_align: "left",
    theme_advanced_statusbar_location: "bottom",
    theme_advanced_resizing: false
});