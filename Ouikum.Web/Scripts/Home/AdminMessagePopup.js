
$('#msgAdminPopup_btn').mouseenter(function () {
    $('#icon_CloseAdminpopup').removeClass('hide');
});

$('#msgAdminPopup_btn').mouseleave(function () {
    $('#icon_CloseAdminpopup').addClass('hide');
});

function CloseAdminMessagePopup() {
    $("#msgAdminPopup_btn").hide();
}
function SendSuggestion() {
    var name = $(".msg-fromName").val();
    var email = $(".msg-fromEmail").val();
    var detail = $(".msg-Detail").val();
    var score = $(".msgScore:checked").val();
    if (name == "") {
        $(".msg-fromName").parent().parent().addClass("error");
        $(".msg-fromName").parent().parent().removeClass("success");
        $(".msg-fromName").focus();
        return false;
    } else if (email == "") {
        $(".msg-fromEmail").parent().parent().addClass("error");
        $(".msg-fromEmail").parent().parent().removeClass("success");
        $(".msg-fromEmail").focus();
        return false;
    } else if (detail == "") {
        $(".msg-Detail").parent().parent().addClass("error");
        $(".msg-Detail").parent().parent().removeClass("success");
        $(".msg-Detail").focus();
        return false;
    } else if (score == null) {
        $(".msgScore").parent().parent().parent().addClass("error");
        $(".msgScore").parent().parent().parent().removeClass("success");
        return false;

    } else {
        OpenLoading(true);
        data = {
            fromName: $(".msg-fromName").val(),
            fromEmail: $(".msg-fromEmail").val(),
            Detail: detail,
            Score: $(".msgScore:checked").val(),
            type: "SendSuggestion",
            CurrentUrl: window.location.href
        }

        $.ajax({
            url: GetUrl("Default/SendEmailToAdmin"),
            type: "Post",
            data: data,
            dataType: 'json',
            success: function (data) {
                OpenLoading(false);

                clearForm("#suggestionForm");
                $("#ModalSendEmail").hide();

                bootbox.alert(label.vldthk_suggestion);
                $(".modal-backdrop").hide()
            }
        });
    }
}
$("#feedback-popup").mouseenter(function () {
    $("#feedback-large").animate({height: 56, width: 150, right: 0}, "slow");
}).mouseleave(function () {
    $("#feedback-large").animate({height: 0, width: 0, right: -150}, "slow");
});
