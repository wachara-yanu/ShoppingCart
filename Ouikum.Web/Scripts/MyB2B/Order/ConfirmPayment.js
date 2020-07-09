$(function () {
    if ($('#radio').hasClass("show")) {
        $('#PaymentAccount').validate({
            rules: {
                PayerName: {
                    required: true,
                    maxlength: 50
                },
                PayerLName: {
                    required: true,
                    maxlength: 50
                },
                PayerAddrLine1: {
                    required: true
                },
                PayerDistrictID: {
                    required: true
                },
                PayerProvinceID: {
                    required: true
                },
                PayerPostalCode: {
                    required: true,
                    maxlength: 5,
                    minlength: 5,
                    number: true
                },
                PayerPhone: {
                    required: true,
                    minlength: 9
                },
                BillRecieverName: {
                    required: true,
                    maxlength: 50          
                },
                BillDistrictID: {
                    required: true
                },
                BillProvinceID: {
                    required: true
                },
                BillPhone: {
                    required: true,
                    minlength: 9
                }
            },
            messages: {
                PayerName: {
                    required: label.vldrequired,
                    maxlength: label.vldmore_50char
                },
                PayerLName: {
                    required: label.vldrequired,
                    maxlength: label.vldmore_50char
                },
                PayerAddrLine1: {
                    required: label.vldrequired
                },
                PayerDistrictID: {
                    required: label.vldrequired
                },
                PayerProvinceID: {
                    required: label.vldrequired
                },
                PayerPhone: {
                    required: label.vldrequired,
                    minlength: label.vldless_9char
                },
                BillRecieverName: {
                    required: label.vldrequired,
                    maxlength: label.vldmore_50char
                },
                BillDistrictID: {
                    required: label.vldrequired
                },
                BillProvinceID: {
                    required: label.vldrequired
                },
                BillPhone: {
                    required: label.vldrequired,
                    minlength: label.vldless_9char
                }
            },
            highlight: function (label) {
                $('#submit').attr('disabled', true);
                $(label).closest('.control-group').removeClass('success');
                $(label).closest('.control-group').addClass('error');

            },
            success: function (label) {
                label.closest('.control-group').removeClass('error');
                label.closest('.control-group').addClass('success');
                $('#submit').removeAttrs('disabled');

            }
        });
    }
    else if ($('#radio').hasClass("hide")) {
        $('#PaymentAccount').validate({
            rules: {
                PayerName: {
                    required: true,
                    maxlength: 50
                },
                PayerLName: {
                    required: true,
                    maxlength: 50
                },
                PayerAddrLine1: {
                    required: true
                },
                PayerDistrictID: {
                    required: true
                },
                PayerProvinceID: {
                    required: true
                },
                PayerPostalCode: {
                    required: true,
                    maxlength: 5,
                    minlength: 5,
                    number: true
                },
                PayerPhone: {
                    required: true,
                    minlength: 9
                }
            },
            messages: {
                PayerName: {
                    required: label.vldrequired,
                    maxlength: label.vldmore_50char
                },
                PayerLName: {
                    required: label.vldrequired,
                    maxlength: label.vldmore_50char
                },
                PayerAddrLine1: {
                    required: label.vldrequired
                },
                PayerDistrictID: {
                    required: label.vldrequired
                },
                PayerProvinceID: {
                    required: label.vldrequired
                },
                PayerPostalCode: {
                    required: label.vldrequired,
                    maxlength: label.vldmore_5char,
                    minlength: label.vldless_5char,
                    number: label.vldfix_format_number
                },
                PayerPhone: {
                    required: label.vldrequired,
                    minlength: label.vldless_9char
                }
            },
            highlight: function (label) {
                $('#submit').attr('disabled', true);
                $(label).closest('.control-group').removeClass('success');
                $(label).closest('.control-group').addClass('error');

            },
            success: function (label) {
                label.closest('.control-group').removeClass('error');
                label.closest('.control-group').addClass('success');
                $('#submit').removeAttrs('disabled');

            }
        });
    }

    $('#Submit').click(function () {
        if ($("#PaymentAccount").valid() == true) {
            bootbox.alert(label.vldpayment_success);
        } else {
            bootbox.alert(label.vldall_required);
        }
    });

    /*-----------------------------------ChangeProvince-------------------------------------*/
    $("#PayerProvinceID").change(function () {
        GetDistrictByProvince($(this).val(), 0, "PayerDistrictID");
    });
});