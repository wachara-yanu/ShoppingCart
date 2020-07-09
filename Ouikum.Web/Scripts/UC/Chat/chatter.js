$(function () {
    var fromID = $('#fromCompID').val();
    var toID = $('#toCompID').val();
    $("#scrollbar-2").tinyscrollbar();
    $("#scrollbar-2").tinyscrollbar_update('bottom');

    $(".hisChat").click(function () {
        $.ajax({
            url: GetUrl("MyChat/HistoryChat"),
            data: { Room: $('#RoomCode').val(), Date: $(this).attr('id') },
            type: "POST",
            success: function (data) {
                if (data.ChatMsg != null && data.ChatMsg != "") {
                    $("#discussion").html("");
                    var date = "";
                    var logoImg = "";

                    $.each(data.ChatMsg, function (key, item) {
                        logoImg = GetUrl("Content/Default/Images/no-avatar.png");
                        if (item.FromCompLogo != null && item.FromCompLogo != "") {
                            logoImg = label.vldurl_upload + "Companies/Logo/" + item.FromID + "/" + item.FromCompLogo;
                        }
                        datetime = new Date(parseInt(item.CreatedDate.substr(6)))
                        if (date != datetime.toLocaleDateString()) {
                            date = new Date(parseInt(item.CreatedDate.substr(6))).toLocaleDateString();
                            $("#discussion").append("<li class='fl_l mar_t30'><b>" + date + "</b></li><li class='fl_l'><div class='linechat'></div></li>");
                        }
                        if (item.FromID == $("#fromCompID").val()) {
                            var img = GetUrl("Content/MyChat/Images/tri_blue_top.png");
                            var time = new Date(parseInt(item.CreatedDate.substr(6))).toLocaleTimeString();
                            $("#discussion").append("<li class='fl_r mar_tb5' style='width:100%'><div class='fl_r' style='vertical-align:middle;'><div class='inline mar_r10 timechat'>" + time + "</div><div class='inline bubble-r'><img src='" + img + "' style='position:absolute;right:-12px;' />" + item.Message + "</div><div class='inline Img-right'><div class='thumbnail'><img src='" + logoImg + "' style='width: 45px; height: 45px;' onload='resizeImg($(this),45,45);' title='" + item.FromName + "'/></div></div></div></li>");
                        }
                        else {
                            var img = GetUrl("Content/MyChat/Images/tri_grey_top.png");
                            var time = new Date(parseInt(item.CreatedDate.substr(6))).toLocaleTimeString();
                            $("#discussion").append("<li class='fl_l mar_tb5' style='width:100%'><div class='fl_l' style='vertical-align:middle;'><div class='inline Img-left'><div class='thumbnail'><img src='" + logoImg + "' style='width: 45px; height: 45px;' onload='resizeImg($(this),45,45);' title='" + item.FromName + "'/></div></div><div class='inline bubble-l'><img src='" + img + "' style='position:absolute;left:-12px;' />" + item.Message + "</div><div class='inline  mar_l10 timechat'>" + time + "</div></div></li>");
                        }
                        $("#scrollbar-2").tinyscrollbar_update('bottom');
                    });
                    if (date != "") {
                        $("#discussion").append("<li class='fl_l'><div class='linechat'></div></li>");
                    }
                }
                else {
                    $("#discussion").html("<li class='center'>Not found chat history.</li><li class='fl_l'><div class='linechat'></div></li>");
                } //end if
            },
            error: function () {
                //  bootbox.alert(label.vldcannot_check_info);
            }
        });
        $("#scrollbar-2").tinyscrollbar_update('bottom');
    });
    // Declare a proxy to reference the hub. 
    var chat = $.connection.chatHub;
    // Create a function that the hub can call to messages.
    //chat.client.id = $('#RoomCode').val();
    chat.client.message = function (CompImgLogo, name, message, roomcode) {
        // Html encode display name and message. 
        var now = new Date();
        timenow = now.toLocaleTimeString();
        var logoImg = GetUrl("Content/Default/Images/no-avatar.png");

        if (CompImgLogo != null && CompImgLogo != "") {
            if (name == $('#displayname').val()) {
                logoImg = label.vldurl_upload + "Companies/Logo/" + fromID + "/" + CompImgLogo;
            } else {
                logoImg = label.vldurl_upload + "Companies/Logo/" + toID + "/" + CompImgLogo;
            }
        }

        // Add the message to the page. 
            if (name == $('#displayname').val()) {
                var img = GetUrl("Content/MyChat/Images/tri_blue_top.png")
                $('#discussion').append("<li class='fl_r mar_tb5' style='width:100%'><div class='fl_r' style='vertical-align:middle;'><div class='inline mar_r10 timechat'>" + timenow + "</div><div class='inline bubble-r'><img src='" + img + "' style='position:absolute;right:-12px;' />" + message + "</div><div class='inline Img-right'><div class='thumbnail'><img src='" + logoImg + "' style='width: 45px; height: 45px;' onload='resizeImg($(this),45,45);' title='" + name + "'/></div></div></div></li>");
                saveMessage($('#RoomID').val(), $('#fromCompID').val(), $('#toCompID').val(), message);
            } else {
                var img = GetUrl("Content/MyChat/Images/tri_grey_top.png")
                //$('embed').remove();
                //$('body').append("<embed src=" + GetUrl("Content/UC/SoundUC/MsgSound.mp3") + " autostart='true' hidden='true' loop='false'>");
                $('#discussion').append("<li class='fl_l mar_tb5' style='width:100%'><div class='fl_l' style='vertical-align:middle;'><div class='inline Img-left'><div class='thumbnail'><img src='" + logoImg + "' style='width: 45px; height: 45px;' onload='resizeImg($(this),45,45);' title='" + name + "'/></div></div><div class='inline bubble-l'><img src='" + img + "' style='position:absolute;left:-12px;' />" + message + "</div><div class='inline  mar_l10 timechat'>" + timenow + "</div></div></li>");
            }
            $("#scrollbar-2").tinyscrollbar_update('bottom');
    };
    // Alert Message
    chat.client.alertMessage = function (id, name) {
        // Add the count to the page.
        if ($('#toCompID').val() == id) {
            $.titlebootbox.alert(name + label.vldalertmessage, {
                stopOnFocus: true,
                interval: 1200
            });
        }
    };
    // Logout Message
    chat.client.LogoutMessage = function (name, roomcode) {
        if (name == $("#displayname").val() && roomcode == $("#RoomCode").val()) {
            bootbox.alert("Login Please.");
            window.location.href = GetUrl("Member/SignIn");
        }
    }
    // Open Browser
    chat.client.OpenBrowser = function (toid, name, roomcode) {
        if (toid == $("#fromCompID").val() && roomcode == $("#RoomCode").val()) {
            $('#discussion').append("<li class='fl_l mar_tb5 center' style='width:100%;color:#8FAE53;'>" + name + " enter chat room. </li>");
            $("#scrollbar-2").tinyscrollbar_update('bottom');
        }
    }
    // Close Browser
    chat.client.CloseBrowser = function (toid, name, roomcode) {
        if (toid == $("#fromCompID").val() && roomcode == $("#RoomCode").val()) {
            $('#discussion').append("<li class='fl_l mar_tb5 center' style='width:100%;color:#BD362F;'>" + name + " get out of chat room. </li>");
            $("#scrollbar-2").tinyscrollbar_update('bottom');
        }
    }
    window.onbeforeunload = function () {
        //bootbox.alert('you close browser chat ?');
        chat.server.close($('#toCompID').val(), $('#displayname').val(), $("#RoomCode").val());
    }

    // Set initial focus to message input box.  
    $('#message').focus();
    // Start the connection.
    $.connection.hub.start().done(function () {
        chat.server.open($('#toCompID').val(), $('#displayname').val(), $("#RoomCode").val());
        chat.server.createGroup($("#RoomCode").val());
        chat.server.getFriendStatus();
        chat.server.getFriendStatus(parseInt($('#toCompID').val(), 10));


        $('#sendmessage').click(function () {
            // Call the Send method on the hub.
            if ($('#message').val() != "") {
                chat.server.send($('#RoomID').val(), $('#toCompID').val(), $('#CompImgLogo').val(), $('#displayname').val(), $('#message').val(), $('#RoomCode').val());
                $('#message').val("").focus();
            }
            // Clear text box and reset focus for next comment. 
            $('#message').val("").focus();
        });
        $('#message').keydown(function (e) {
            if (e.which == 13) {
                $('#sendmessage').click();
                ReadingMessage($('#fromCompID').val(),$('#RoomID').val());
            }
        })

        $('#message').click(function () {
            ReadingMessage($('#fromCompID').val(), $('#RoomID').val());
        })
    });
});
function saveMessage(roomid, fromid, toid, message) {
    $.ajax({
        url: GetUrl("MyChat/SaveMessage"),
        data: { roomid: roomid, fromid: fromid, toid: toid, message: message },
        type: "POST",
        success: function (data) {
        }
    });
}
function ReadingMessage(toid, roomid) {
    $.ajax({
        url: GetUrl("MyChat/ReadingMessage"),
        data: { toid: toid, roomid: roomid },
        type: "POST",
        success: function (data) {
        }
    });
}