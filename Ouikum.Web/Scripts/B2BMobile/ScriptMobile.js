$('#container-menu').hide();
$('#container-search').hide();
$('#message-Target').hide();
$('#requestprice-Target').hide();
$('#tel-Target').hide();
$('#reply-Target').hide();
$('#quotation-Target').hide();

function GetFormValues(form_id) {
    var $form = $('#' + form_id);
    var elements = new Array();

    $form.find('input[type=text], input[type=hidden], input[type=password],input[type=time],input[type=date],input[type=number],textarea,select').each(function () {
        if ($(this).attr('disabled') != 'disabled' && $(this).attr('disabled') != true) {
            var name = $(this).attr('name');
            if (name != null && name != undefined) {
                elements.push({
                    name: $(this).attr('name'),
                    value: $(this).val()
                });
            }
        }
    });
    $form.find('input[type=checkbox]').each(function (i, el) {
        if ($(this).attr('disabled') != 'disabled' && $(this).attr('disabled') != true) {
            if (this.checked) {
                console.log(true);
                elements.push({
                    name: $(this).attr('name'),
                    value: true
                });
            } else {
                elements.push({
                    name: $(this).attr('name'),
                    value: null
                });
            }
        }
    });
    $form.find('input[type=radio]:checked').each(function (i, el) {
        if ($(this).attr('disabled') != 'disabled' && $(this).attr('disabled') != true) {
            elements.push({
                name: $(this).attr('name'),
                value: $(this).val()
            });
        } else {
            elements.push({
                name: $(this).attr('name'),
                value: null
            });
        }
    });
    return elements;
}

$('.Togglemenu').click(function () {
    var target = $(this).attr("data-target");
    var CheckBody = $("#CheckBody").val();

    var thisActive = $(".toggleContainer.active");
    var ThisToggle = thisActive.attr('id');

    if (thisActive != undefined) {
        $(thisActive).toggle('slide', { direction: 'down' });
        $(thisActive).removeClass('active');
        //$(this).removeClass('bgbtn-Gray');
    }                                  
    if ("#" + ThisToggle != target) {
        $(target).toggle('slide', { direction: 'down' }, function () {
            $(this).addClass('active');
        });
        //$(this).addClass('bgbtn-Gray');
    }        
});

$(".btn-slidetoggle").click(function () {
    var My = $(this).attr("data-target");
    $(".TargetSlide").hide('slide', { direction: 'down' });
    $(My).toggle('slide', { direction: 'down' });
})

$('.btn-cancel').click(function () {
    $(".TargetSlide").hide('slide', { direction: 'down' });
    $("#container-search").hide('slide', { direction: 'down' });

    $('.form-group').removeClass('has-success');
    $('.form-group').removeClass('has-error');
    
    $("label[for=textSearch]").remove(); $("label[for=CategoryID]").remove();
    $("label[for=ProvinceID]").remove(); $("label[for=BizTypeID]").remove();

    $("label[for=txtFromName]").remove(); $("label[for=txtFromEmail]").remove();
    $("label[for=txtFromContactPhone]").remove(); $("label[for=MessageDetail]").remove();

    $("label[for=ReqFirstName]").remove(); $("label[for=CompanyName]").remove();
    $("label[for=ReqEmail]").remove(); $("label[for=ReqPhone]").remove();

    $("label[for=ReMsgDetail]").remove();
});

$('.carousel').carousel({
    interval: 4000
})

$('#btn-Canchangpass').click(function () {
    $('#OldPassword').val("");
    $('#NewPassword').val("");
    $('#ConfirmPassword').val("");

    $('.form-group').removeClass('has-success success');
    $('.form-group').removeClass('has-error');

    $("label[for=OldPassword]").remove();
    $("label[for=NewPassword]").remove();
    $("label[for=ConfirmPassword]").remove();

    $('#Submit-ChangPass').attr('disabled', 'disabled')
    
});

function LoadingMobile(isLoad, img, obj) {
    if (isLoad == true) {
        if (img == null) {
            img = '<div class="icon-loader"></div>';
        } else {
            img = '<img src=\"' + img + '\" >';
        }
        if ($('#loading').length == 0) {
            if (obj == null || obj == undefined) {
                $('body').prepend('<div id="loading">&nbsp;</div><div id="imgloading" style="top: 350px; left: 779.5px;">' + img + '</div>');
            } else {
                obj.prepend('<div id="loading">&nbsp;</div><div id="imgloading" style="top: 350px; left: 779.5px;">' + img + '</div>');
            }
            $("#loading").css({ 'filter': 'alpha(opacity=50)', 'opacity': '0.5' });
            $("#imgloading").position({ my: "center", at: "center", of: "#loading" });
        }

    } else {
        $('#loading').remove(); $('#imgloading').remove();
    }
}
