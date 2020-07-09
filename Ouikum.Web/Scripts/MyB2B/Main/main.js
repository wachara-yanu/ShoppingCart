
function CloseAllMenu() {
    var len = $('.toggle').length;
    for (var i = 1; i < len; i++) {
        $('.toggle').eq(i).hide();
        if ($('.toggleLink').eq(i).text() == "Hide") {
            $('.toggleLink').eq(i).text("Show");
        }
    }
}

function CalculateProductProgress(count) {
    var max = parseInt($('#product-progress').attr('data-maxproduct'), 10);
    if(max > 0){
    var total = (100 / max) * count;
    console.log('progress : ' + max);
    $('#product-progress').css('width', total + '%');}
}

function GetCompStatus() {

    $.ajax({
        url: GetUrl("MyB2B/Main/GetCompStatus"),
        success: function (data) {
            //console.log(data); 

            $('.status-product').html(data.CountProduct);
            $('.status-product-approve').html(data.CountProductApprove);
            $('.status-product-wait').html(data.CountProductWait);
            $('.status-product-reject').html(data.CountProductReject);
            //            $('#status-buylead').html(data.CountBuylead);
            //            $('#status-buylead-reject').html(data.CountBuyleadReject);
            //            $('#status-inbox').html(data.Inbox);
            $('#status-unread').html(data.UnRead);
            $('.status-msg-unread').html(data.UnRead);
            $('h2.status-msg-unread1').html(data.UnRead);
            //            $('#status-important').html(data.Important);
            CalculateProductProgress(data.CountProduct);
        },
        error: function () {
        },
        type: "POST"
    });

}
