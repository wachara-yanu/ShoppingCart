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
        if ($('#EditBanner').validate()) {
            if (checkError()) {
                if ($('#ListNo').val() <= $('#Count').val()) {
                    $('#submit').submit();
                } else {
                    bootbox.alert('ลำดับการแสดงผลต้องไม่มากกว่าจำนวนที่มีอยู่ปัจจุบัน');
                    $("#ListNo").val($('#Count').val());
                    return false;
                }
            }
            else {
                return false;
            }
        }
    });

    $('#saveBanner').click(function () {
        if ($('#EditBanner').validate()) {
            if (checkError()) {
                $('#submit').click();
            }
            else {
                return false;
            }
        }
    });

    $.validator.addMethod("select", function (value, element, arg) {
        return arg != value;
    });

    $('#EditBanner').validate(
    {
        onkeydown: false,
        onkeyup: false,
        rules: {
            Title: {
                required: true,
                minlength: 4
            },
            Link: {
                required: true,
                minlength: 4
            },
            ImgPath: {
                required: true
            }
        },
        messages: {
            title: {
                required: 'กรุณากรอกข้อมูล',
                minlength: 'กรอกข้อมูลอย่างน้อย 4 ตัวอักษร'
            },
            link: {
                required: 'กรุณากรอกข้อมูล',
                minlength: 'กรอกข้อมูลอย่างน้อย 4 ตัวอักษร'
            },
            ImgPath: {
                required: 'กรุณากรอกข้อมูล'
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
