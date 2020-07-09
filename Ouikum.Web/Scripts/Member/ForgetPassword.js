$(function () {
    //$('#QapTcha').QapTcha({});
    $('#EmailorUsername').focus();
    document.getElementById('forgetpassword_form').onsubmit = function () {
        return false;
    };
    $('#EmailorUsername').keypress(function (e) {
        if (e.which == 13) {
           
            ForgetPassword();
        }
    });

    $('#submit').click(function () {
        console.log("submit");
        ForgetPassword();
    });

    $('#forgetpassword_form').validate(
    {
        onkeydown: false,
        onkeyup: false,
        rules: {
            EmailorUsername: {
                required: true,
                minlength: 4
            }
        },
        messages: {
            EmailorUsername: {
                required: label.vldrequired,
                minlength: label.vldless_4char
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
            $("#validateForget").hide();
            $('#submit').removeAttrs('disabled');
        }
    });
});

function ForgetPassword() {
    if ($('#EmailorUsername').val() != '') {
        OpenLoading(true);
        $.ajax({
            url: GetUrl("Member/ForgetPassword"),
            data: { EmailorUsername: $('#EmailorUsername').val() },
            type: "POST",
            dataType: 'json',
            success: function (data) {
                if (data != null) {
                    if (data.IsSuccess) {
                        $("#p_InfoForget > text").html(data.Result);
                        $("#InfoForget").fadeIn();
                        OpenLoading(false);
                    } else {
                        $("#p_validateForget > text").html(data.Result);
                        $("#validateForget").fadeIn();
                        //$('#QapTcha').children().remove();
                        //$('#QapTcha').QapTcha({});
                        OpenLoading(false);
                    }
                }
            },
            error: function () {
                OpenLoading(false);
                bootbox.alert(label.vldrequest_unsuccessful);
            }
        });
    } else {
        if ($('#EmailorUsername').val() == '') {
            $('#EmailorUsername').focus();
        }
        if ($('#iQapTcha').val() != '') {
            bootbox.alert(label.vldplease_unlock);
        }
    }
}