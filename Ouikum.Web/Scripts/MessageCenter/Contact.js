var classContactList = "selectContactList";
var classContactCompany = "selectContactComp";

$("#CheckContactAll").click(function () {
    if ($(this).attr("checked") == true || $(this).attr("checked") == "checked") {
        $(".cbxContactID").attr("checked", "checked");
        addContactList($(this));
    } else {
        $(".cbxContactID").removeAttr("checked");
        $('#methodTags').tagit('removeAll'); 
    }
});

//$("#SelectContact").click(function () {
//    var ContactID = new Array();
//    var RowVersion = new Array();

//    for (x = 0; x < $(".cbxContactID").length; x++) {
//        if ($(".cbxContactID").eq(x).attr("checked") == "checked" || $(".cbxContactID").eq(x).attr("checked") == true) {
//            ContactID[ContactID.length] = $(".cbxContactID").eq(x).val();
//            RowVersion[RowVersion.length] = $(".RowVersionContactID").eq(x).val();
//        }
//    }
//    if (ContactID != "") {

//    }
//    else {

//        bootbox.alert("กรุณาเลือกผู้ติดต่อที่ต้องการ");

//    }
//});


//$(".cbxContactID").click(function () {
//    var index = $(".cbxContactID").index($(this));
//    var CompContactID = $(".cbxContactID").eq(index).val();

//    if ($(".cbxContactID").eq(index).attr("checked") == true || $(".cbxContactID").eq(index).attr("checked") == "checked") {
        
//        addContact($(this), index);
//    }
//    else {
//        $("#CheckContactAll").attr("checked", false);

//        $("li.RecipientName" + CompContactID).remove();
          
//    }
//});

function addContact(obj, index) {
    var strHtml = "";
    var txtNameValue = $(".cbxContactID").eq(index).parent().next().children().text();
    var txtIdValue = $(".cbxContactID").eq(index).val();
    var className = "RecipientName" + txtIdValue;
    $("#methodTags").tagit("createTag", txtNameValue, className);
    strHtml = '<input type="hiden" name="RecipientID" value="' + txtIdValue + '"class="RecipientID' + txtIdValue + '" style="display:none;">';
    $("li.RecipientName" + txtIdValue).append(strHtml);
}

function addContactList() {
    var checked = getCookie("checkedContact");
    var checkedname = getCookie("checkedContactName");

    if (checked != "" && checked != null) {
        var arr = checked.split(",");
        var arr_name = checkedname.split(",");
        for (var i = 0; i < arr.length; i++) {
            var CompID = arr[i];
            if ($("#methodTags").find("#" + CompID).length < 1) {
                var txtNameValue = arr_name[i];
                var tagName = "<li id='" + CompID + "'  class='ContactNameTag'><span class='mar_r5'>" + txtNameValue + "</span><i class='icon-remove cursor' style='vertical-align:middle' onclick='removeTag($(this))'></i></li>"
                $("#methodTags").append(tagName);
            }
        }
    }

    onChangeContact();
    $('#myModal').modal('hide')
}

function removeTag(Obj) {
    Obj.parent().remove();
    onChangeContact();
}

function onChangeContact() {
    var arrToCompID = new Array();

        $(".ContactNameTag").each(function (index) {
           arrToCompID[index] = $(".ContactNameTag").eq(index).attr("id")

       });

       $("#hidToCompID").val(arrToCompID);
}

function displayKeyCode(evt) {
    var charCode = (evt.which) ? evt.which : event.keyCode
    var strHtml = "";
    if (charCode == 9 || charCode == 13) {    //32 spacebar , 9 tab , 13 enter,27 ESC
        var compName = $('#txtRecipients').val();

        if (compName == " " || compName == null || compName == "") {
            bootbox.alert(label.vldenter_recipient);
            $('#txtRecipients').val('');
        }
        else {
            strHtml = '<li class="' + classContactCompany + '"';
            strHtml += 'title="' + compName + '" >';
            strHtml += '<span>' + compName + '</span>';
            strHtml += '<span class="delContactList cursor" title="Delete"> x </span>';
            strHtml += '<input type="hidden" class="hide' + classContactCompany + '" value="' + compName + '" />';
            strHtml += '</li>';
            $("#divselectedContact ul#ulContactLists").append(strHtml);
            $('#txtRecipients').val('');
            manageContact();
        }
    }
    else if (charCode == 27) {
        $('#txtRecipients').val('');
        $('#txtRecipients').click();
    }

    return false;
}
function rememberCheck(Obj) {
    var CompContactID = Obj.val();
    var contactName = Obj.attr("data-name");
    var checked = getCookie("checkedContact");
    var checkedname = getCookie("checkedContactName");

    if (Obj.attr("checked") == true || Obj.attr("checked") == "checked") {
        if (checked == null || checked == "") {

            setCookie("checkedContact", CompContactID, 1);
            setCookie("checkedContactName", contactName, 1);
        }
        else {
            checked += "," + CompContactID;
            checkedname += "," + contactName;
            setCookie("checkedContact", checked, 1);
            setCookie("checkedContactName", checkedname, 1);
        }
    } else {
        if (checked != null && checked != "") {
            var arr = checked.split(",");
            var arr_name = checkedname.split(",");
            if (jQuery.inArray(CompContactID, arr) > -1) {
                var i = arr.indexOf(CompContactID);
                arr.splice(i, 1);
                arr_name.splice(i, 1);
                setCookie("checkedContact", arr, 1);
                setCookie("checkedContactName", arr_name, 1);
            }
        }
    }

}