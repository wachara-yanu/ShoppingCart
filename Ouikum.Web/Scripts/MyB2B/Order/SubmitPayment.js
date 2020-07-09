$(function () {

    $("#PayAmount").live('blur', function () {
        if ($('#PayAmount').val() == "") {
            $(this).val("0.00");
        }
        $(this).val(parseFloat(eval($(this).val())).toFixed(2));
        $('.Price .error').text('');
        $("#PayAmount").closest('.control-group').addClass('success');
        $("#PayAmount").closest('.control-group').removeClass('error');
    });

    $('#PaymentAccount').validate({
        rules: {
            BankID: {
                required: true
            },
            BranchName: {
                required: true,
                maxlength: 30
            },
            PayerAccName: {
                required: true,
                maxlength: 30
            },
            PayerAccNo: {
                required: true
            }
        },
        messages: {
            BankID: {
                required: label.vldbankid
            },
            BranchName: {
                required: label.vldbranchname
            },
            PayerAccName: {
                required: label.vldpayeraccname
            },
            PayerAccNo: {
                required: label.vldpayeraccno
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
});