var unread = null;
var read = null;
var friends = null;

$(function () {
    // Declare a proxy to reference the hub. 
    getChatStatus();
    var chat = $.connection.chatHub;
    $('.compImg').live('click', function () {
        var id = $(this).attr('id');
        $('.compImg').each(function () {
            if ($(this).attr('id') != id) {
                $(this).popover('hide');
            }
        });
    });
    chat.client.showFriendStatus = function (ID, Friends) {
        generateFriendStatus(Friends, ID);
        $(".scrollbar-original").tinyscrollbar();
    };
    chat.client.getStatusFriendByID = function (ID, FriendID) {
        UpdateStatusFriendByID(ID, FriendID);
        $(".scrollbar-original").tinyscrollbar();
    };
    chat.client.getStatusUnReadFriendByID = function (ID, FriendID) {
        if (ID == $('.newmsg').attr('data-id')) {
            UpdateStatusUnreadFriendByID(ID, FriendID);
            $(".scrollbar-original").tinyscrollbar();
            $('.newmsg').blink({ delay: 900 });
            $('embed').remove();
            $('body').append("<embed src=" + GetUrl("Content/UC/SoundUC/MsgSound.mp3") + " autostart='true' hidden='true' loop='false'>");
        }
    };

    chat.client.getFriendStatus = function (ID) {
        getFriendStatus(ID);
        $(".scrollbar-original").tinyscrollbar();
    };
     
    $.connection.hub.start().done(function () {
        
    });

});

$('.chat-btn-hide').live('click', function () {
    $('.compImg').popover('hide');
    $('#chat-status').hide();
    $('#tag-chat-status').show(); 
});

$('#tag-chat-status').live('click', function () {
    $('#tag-chat-status').hide();
    $('#chat-status').show();
    $(".scrollbar-original").tinyscrollbar();
    $('.newmsg').unblink();
});

$('.chat-status-link,.chat-status-img').live('click', function () {
    $('.newmsg').unblink();
});
function getChatStatus() {
    var id = parseInt($('#chat-status').attr('data-id'), 10);
    getFriendStatus(id);
}
function getFriendStatus(myID) { 
    var id = parseInt($('#chat-status').attr('data-id'), 10); 
    if (id == myID) {
        $.ajax({
            url: GetUrl("MyChat/GetFriendStatus"),
            type: "POST",
            dataType: 'json',
            success: function (data) {
                if (data.Friends != null && data.Friends != "") {
                    generateFriendStatus(data.Friends, data.UnReads, data.ID)
                }
                else {
                    $('#notfound').show();
                }
            },
            error: function () {
            }
        });
    }
}
function generateFriendStatus(friends, unreads, toID) {
    var html = "";
    var id = parseInt($('#chat-status').attr('data-id'), 10);
    var total = 0;
    var fname;
    var phone;
    var email;
    var logo;
    if (id == toID) {
        if (friends != null) {
            $('#notfound').hide();
            $.each(friends, function (key, item) {
                var url = GetUrl("MyChat/Chatter?ToID=" + item.FriendID);
                url = 'onclick = "window.open(\'' + url + '\',\'' + toID + '' + item.FriendID + '\',\'width=620,height=565,menubar=no,status=no,resizable=no\').focus();return false;"';
                html += '<div class="list-chat friend-chat" data-id="' + item.FriendID + '" >';
                if (CheckUnread(unreads, item.FriendID)) {
                    if (readcount < 100) {
                        html += '<span class="label label-important msg-unread">' + readcount + '</span>';
                    } else {
                        html += '<span class="label label-important msg-unread">99+</span>';
                    }
                    total += readcount;
                } else {
                    html += '<span class="label label-important msg-unread" style="display:none;">0</span>';
                }
                if (item.IsOnline == true) {
                    html += '<span class="status-chat-online u-online fl_l" style="margin-top:17px"></span>';
                } else {
                    html += '<span class="status-chat-offline u-offline fl_l" style="margin-top:17px"></span>';
                }
                html += "<a class='chat-status-img fl_l'>";
                if (item.FriendName.length > 30)
                {
                    fname = item.FriendName.substring(0, 30);
                    fname = fname+"..."
                } else
                {
                    fname = item.FriendName;
                }
                if (item.FriendLogoImgPath != null && item.FriendLogoImgPath != ""){
                    logo = "<img class='fl_l mar_r10 thumbnail' style='width:35px;height:33.6px;' src='" + label.vldurl_upload + "Companies/Logo/" + item.FriendID + "/" + item.FriendLogoImgPath + "' />";
                } else {
                    logo = "<img class='fl_l mar_r10 thumbnail' style='width:35px;height:33.6px;' src='" + GetUrl("Content/Default/Images/no-avatar.png") + "' />";
                }
                if (item.Email != null && item.Email != "") {
                    if (item.Email.length > 25) {
                        email = item.Email.substring(0, 25);
                        email = email + "..."
                    } else {
                        email = item.Email;
                    }
                } else {
                    email = '-';
                }
                if (item.Phone != null && item.Phone != "") {
                    if (item.Phone.length > 25) {
                        phone = item.Phone.substring(0, 25);
                        phone = phone + "..."
                    } else {
                        phone = item.Phone;
                    }
                } else {
                    phone = '-';
                }
                var title = "<a target='_blank' href='" + GetUrl("CompanyWebsite/" + item.FriendName + "/Main/Index/" + item.FriendID) + "' title='" + item.FriendName + "'>" + fname + "</a>";
                var Content = "<div class='inline'>" + logo + "</div><div class='inline' style='position:absolute'><p title='" + item.Email + "'>Email : " + email + "</p><p title='" + item.Phone + "'>Tel : " + phone + "</p></div>";

                if (item.FriendLogoImgPath != null && item.FriendLogoImgPath != "") {
                    html += '<div class="thumbnail"><img id="' + item.FriendID + '" class="compImg" rel="popover" data-toggle="click" data-placement="left" data-content="' + Content + '" data-original-title="' + title + '" style="width:45px;height:33.6px;" src="' + label.vldurl_upload + "Companies/Logo/" + item.FriendID + "/" + item.FriendLogoImgPath + '" /></div>';
                } else {
                    html += '<div class="thumbnail"><img id="' + item.FriendID + '" class="compImg" rel="popover" data-toggle="click" data-placement="left" data-content="' + Content + '" data-original-title="' + title + '" style="width:45px; height:35px;" src="' + GetUrl("Content/Default/Images/no-avatar.png") + '" /></div>';
                }
                html += '</a>';
                html += '<a class="chat-status-link cursor"' + url + '>' + item.FriendName + '</a>';
                html += '<hr class="chat-status-line">';
                html += '</div>';
            });
            $('#read-' + id).html(html);
            $('.tag-' + toID).text(total);
            $('.compImg').each(function () {
                $(this).popover({ template: '<div class="popover" style="width:300px;position:fixed"><div class="arrow"></div><div class="popover-inner"><h3 class="popover-title"></h3><div class="popover-content"><p></p></div></div></div>' });
            });
        }
    }    
}
function UpdateStatusFriendByID(ID, FriendID) {
    var len = $('.friend-chat').length;
    var countFriendID = 0;
    var id = parseInt($('#chat-status').attr('data-id'), 10);
    if (id == ID) {
        for (var i = 0; i < len; i++) {
            var fid = parseInt($('.friend-chat').eq(i).attr('data-id'), 10);
            if (fid == FriendID) {
                countFriendID++;
                $('.msg-unread').eq(i).hide();
                return true;
            }
        }
        if (countFriendID == 0) {
            getFriendStatus(ID);
            getFriendStatus(FriendID);
        }
    }
}

function UpdateStatusUnreadFriendByID(ID, FriendID) {
    var len = $('.friend-chat').length;
    var id = parseInt($('#chat-status').attr('data-id'), 10);
    if (id == ID) {
        data = {
            FriendID: FriendID
        }
        $.ajax({
            url: GetUrl("MyChat/GetStatusFriendByID"),
            data : data,
            type: "POST",
            dataType: 'json',
            success: function (Model) {
                var countFriendID = 0;
                    for (var i = 0; i < len; i++) {
                        var fid = parseInt($('.friend-chat').eq(i).attr('data-id'), 10);
                        if (fid == FriendID) {
                            if (CheckUnread(Model, fid)) {
                                countFriendID++;
                                if (readcount < 100)
                                {
                                    $('.msg-unread').eq(i).text(readcount);
                                } else {
                                    $('.msg-unread').eq(i).text("99+");
                                }
                                $('.msg-unread').eq(i).show();
                                $('#read-' + ID).prepend("<div class='list-chat friend-chat' data-id='" + fid + "'>" + $('.friend-chat').eq(i).html() + '</div>');
                                $('.friend-chat').eq(i+1).remove();                         
                                CalSumUnread();
                                return true;
                            }
                        }
                    }
                    if (countFriendID == 0) {
                        getFriendStatus(ID);
                        getFriendStatus(FriendID);
                    }
            },
            error: function () {
            }
        });
    }
}

var readcount = 0;
function CheckUnread(model, id) {
    readcount = 0;
    var res = false;
    for (var i = 0; i < model.length; i++) {
        if (model[i].FromID == id) {
            readcount = model[i].ReadCount;
            res = true;
            return res;
        } 
    }
}

function CalSumUnread() {
    var len = $('.msg-unread').length;
    var totalsum = 0;
    for (var i = 0; i < len; i++) {
        var count = parseInt($('.msg-unread').eq(i).text(), 10);
        totalsum += count;
    }
    var tag = $('#chat-status').attr('data-id');
   $('.tag-' + tag).text(totalsum);
}