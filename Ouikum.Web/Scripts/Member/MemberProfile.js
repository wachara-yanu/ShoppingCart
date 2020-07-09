$(function () {
    $("em").remove();
    $(".t-upload-button > span").remove();
    $("#cancel,#btn-cancel").click(function () {
        $('.show').show();
        $('.hide').hide();
        $('.icon-ShowHide.hide').removeAttr('style');
        $('.btn-file').hide();
        $('label.error').remove();
        $('div,label').removeClass('error');
    });
    $('.edit').click(function () {
        $('.show').hide();
        $('.hide').show();
        $('.icon-ShowHide.hide').removeAttr('style');
        $('.btn-file').show();
        $('#submit').css('display', 'inline-block');
        $('#cancel').css('display', 'inline-block');
        $("#content_sys").slideDown(function () {
            $("#sidebar").height($("#auroHeight").height());
            $("#main").height($("#auroHeight").height());
        });
    });
    $("ul.toggle > li").each(function (index) {
        if ($(this).attr("id") == 'tab_profile') {
            $(this).addClass('active_template');
        }
        else {
            $(this).removeClass('active_template');
        }
    });

    $('.ImgProfile').fileupload({});

    /*-----------------------------ajax submit------------------------------*/

    $("#submit,#btn-save").click(function () {
        if($('#mem_profile_form').valid()){
        data = {
            AvatarImgPath: $('#AvatarImgPath').val(),
            FirstName: $('#FirstName').val(),
            LastName: $('#LastName').val(),
            AddrLine1: $('#AddrLine1').val(),
            Email: $('#Email').val(),
            DistrictID: $('#DistrictID').val(),
            ProvinceID: $('#ProvinceID').val(),
            PostalCode: $('#PostalCode').val(),
            Phone: $('#Phone').val(),
            Mobile: $('#Mobile').val(),
            Fax: $('#Fax').val(),
            RowVersion: $('#RowVersion').val()
        }
        OpenLoading(true);
        $.ajax({
            url: GetUrl("Member/MemberProfile"),
            data: data,
            type: "POST",
            success: function (data) {
                if (data["result"]) {
                    OpenLoading(false);
                    bootbox.alert(label.vldsave_success);
                    //$("#RowVersion").val(data["RowVersion"]);
                    if ($('#FirstName').val() == "") {
                        $('#lblFirstName').text(label.vldno_data);
                    } else {
                        $('#lblFirstName').text($('#FirstName').val());
                    }
                    if ($('#LastName').val() == "") {
                        $('#lblLastName').text(label.vldno_data);
                    } else {
                        $('#lblLastName').text($('#LastName').val());
                    }
                    if ($('#AddrLine1').val() == "") {
                        $('#lblAddrLine1').text(label.vldno_data);
                    } else {
                        $('#lblAddrLine1').text($('#AddrLine1').val());
                    }
                    if ($('#Email').val() == "") {
                        $('#lblEmail').text(label.vldno_data);
                    } else {
                        $('#lblEmail').text($('#Email').val());
                    }
                    if ($("#DistrictID").val() == 0) {
                        $('#lblDistrictID').text("");
                    } else {
                        $('#lblDistrictID').text($("#DistrictID option[value=" + $("#DistrictID").val() + "]").attr("selected", "selected").text());
                    }
                    if ($("#ProvinceID").val() == 0) {
                        $('#lblProvinceID').text(label.vldno_data);
                    } else {
                        $('#lblProvinceID').text($("#ProvinceID option[value=" + $("#ProvinceID").val() + "]").attr("selected", "selected").text());
                    }
                    if ($('#PostalCode').val() == "") {
                        $('#lblPostalCode').text(label.vldno_data);
                    } else {
                        $('#lblPostalCode').text($('#PostalCode').val());
                    }
                    if ($('#Phone').val() == "") {
                        $('#lblPhone').text(label.vldno_data);
                    } else {
                        $('#lblPhone').text($('#Phone').val());
                    }
                    if ($('#Mobile').val() == "") {
                        $('#lblMobile').text(label.vldno_data);
                    } else {
                        $('#lblMobile').text($('#Mobile').val());
                    }
                    if ($('#Fax').val() == "") {
                        $('#lblFax').text(label.vldno_data);
                    } else {
                        $('#lblFax').text($('#Fax').val());
                    }
                    $('.show').show();
                    $('.hide').hide();
                    $('.btn-file').hide();

                } //end if
                else {
                    bootbox.alert(label.vldsave_unsuccess);
                    OpenLoading(false);
                }
            },
            error: function () {
                // bootbox.alert(label.vldcannot_check_info);
                OpenLoading(false);
            }
        });
    }else{
        bootbox.alert(label.vldrequired_complete);
        return false;
    }
    });

    /*-----------------------------------ChangeProvince-------------------------------------*/
    $("#ProvinceID").change(function () {
        $('#DistrictID').DistrictByProvince({ province:$(this).val(),district: 0 });
    });
    /*-------------------------------validate Memberprofile---------------------------------*/
    $('#mem_profile_form').validate(
             {
                 onkeydown: false,
                 onkeyup: false,
                 rules: {
                     FirstName: {
                         minlength: 4,
                         required: true
                     },
                     LastName: {
                         minlength: 4,
                         required: true
                     },
                     AddrLine1: {
                         required: true
                     },
                     Email: {
                         required: true,
                         email: true
                     },
                     DistrictID: {
                         selectDistrict: 0
                     },
                    ProvinceID: {
                         selectProvince: 0
                     },
                     PostalCode: {
                         number: true,
                         minlength: 5,
                         maxlength: 5
                     },
                     Phone: {
                         required: true,
                         //number: true,
                         minlength: 9
                         //maxlength: 10
                     },
                     Mobile: {
                         //number: true,
                         minlength: 10
                     }
                 },
                 messages: {
                     FirstName: {
                         minlength: label.vldless_4char,
                         required: label.vldrequired
                     },
                     LastName: {
                         minlength: label.vldless_4char,
                         required: label.vldrequired
                     },
                     AddrLine1: {
                         required: label.vldrequired
                     },
                     Email: {
                         required: label.vldrequired,
                         email: label.vldfix_format_email
                     },
                     DistrictID: {
                         selectDistrict: label.vldselectdistrict
                     },
                     ProvinceID: {
                         selectProvince: label.vldselectprovince
                     },
                     PostalCode: {
                         number: label.vldfix_format_number,
                         minlength: label.vldless_5char,
                         maxlength: label.vldmore_5char
                     },
                     Phone: {
                         //number: label.vldfix_format_number,
                         minlength: label.vldless_9char
                         //maxlength: label.vldmore_10char
                     },
                     Mobile: {
                         //number: label.vldfix_format_number,
                         minlength: label.vldless_10char
                         //maxlength: label.vldmore_10char
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

/*--------------------------checkEmail------------------------------*/
function checkEmail() {
    OpenLoading(true);
    $.ajax({
        url: GetUrl("User/Validate"),
        data: { email: $('#Email').val(), MemberID: $("#MemberID").val() },
        type: "POST",
        success: function (data) {
            if (!data) {
                $("#Email").closest('.control-group').removeClass('success');
                $("#Email").closest('.control-group').addClass('error');
                $(".Email > .error").text(label.vldemail_exists);
                checkError();
                OpenLoading(false);

            } //end if
            else {
                checkError();
                OpenLoading(false);
            }
        },
        error: function () {
           // bootbox.alert(label.vldcannot_check_info);
        }
    });
    checkError();
}
/*------------------------------DelContImage---------------------------------*/
function Del_AvatarImg() {
    if (confirm(label.vldconfirm_del)) {
        var no_img = "<img id='img_AvatarImg' src='http://www.placehold.it/75x75/EFEFEF/AAAAAA&text=no+image' />";
        $("#AvatarImg").html(no_img);
        $("#AvatarImgPath").val("");
    }
}
/*----------------------------------------------------------------------------*/
function checkError() {
    if ($('.control-group').hasClass('error')) {
        $("#submit").attr('disabled', true);
    } else {
        $("#submit").attr('disabled', false);
    }
}