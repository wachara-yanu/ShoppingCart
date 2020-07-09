
/*---------------Display----------------------*/
$('#sidebar').css('height', $(window).height() - 60);
$('#main').css('height', $(window).height() - 60);
$('#divCardMember').css("display", "none");
/*---------------Quo-Add----------------------*/
$('#icon_Contact').tooltip({ placement: 'top' });
$('#iconReject').tooltip({ placement: 'top' });
$('#iconImportance').tooltip({ placement: 'top' });
$('#iconQuo').tooltip({ placement: 'top' });
$('#iconReq').tooltip({ placement: 'top' });
$('#Bid').tooltip({ placement: 'top' });
$('#icon_Contact').popover({
    trigger: 'click',
    placement: 'right',
    content: $('#divCardMember'),
    template: '<div class="popover" style="width:340px; height:210px;  top:0px; left:0px;"><div class="arrow"></div><div class="popover-content"><p></p></div></div>'
});
$('#icon_Contact').click(function () {
    $('#divCardMember').css("visibility", "visible");
    $('#divCardMember').css("display", "block");
})
$('input#Qty,#ZipCode').bind('keypress', function (e) {
    return (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) ? false : true;
})
$("#IsSentEmail").click(function () {
    if ($('input[name=IsSentEmail]:checked').attr("checked") == true || $('input[name=IsSentEmail]:checked').attr("checked") == "checked") {
        $('#IsSentEmail').val("true");
    } else {
        $('#IsSentEmail').val("false");
    }
});
$("#IsTelephone").click(function () {
    if ($('input[name=IsTelephone]:checked').attr("checked") == true || $('input[name=IsTelephone]:checked').attr("checked") == "checked") {
        $('#IsTelephone').val("true");
    } else {
        $('#IsTelephone').val("false");
    }
});
$("#IsAttach").click(function () {
    if ($('input[name=IsAttach]:checked').attr("checked") == true || $('input[name=IsAttach]:checked').attr("checked") == "checked") {
        $('#IsAttach').val("true");
    } else {
        $('#IsAttach').val("false");
    }
});
//--------------Checked-------------
$("input[type='checkbox']").live("click", function () {
    var Obj = $(this);
    if (Obj.attr("checked") != "checked") {
        Obj.removeAttr('checked');
    } else {
        Obj.attr('checked', true);
    }
});

/*----------------------------Request-Price-----------------------------------*/
function RequestDataMember() {
    $.ajax({
        url: GetUrl("MyB2B/Quotation/RequestDataMember"),
        data: { ID: $("#ProductID").val() },
        dataType: 'json',
        traditional: true,
        type: "POST",
        success: function (data) {
            if (data.CompID != 0) {
                $("#CompName").val(data.CompName);
                $("#FirstName").val(data.FirstName);
                $("#LastName").val(data.LastName);
                $("#Address").val(data.Address);
                //$("#SubDistrict").val(data.SubDistrict);
                $("#ZipCode").val(data.ZipCode);
                $("#Tel").val(data.Mobile);
                $("#Email").val(data.Email);
                $("#AddrLine1").val(data.Address1);
                $("#AddrLine2").val(data.Address2);
                $("#SubDistrict").val(data.SubDistrict);
                $("#DistrictID").val(data.ProvinceID);
                $("#PostalCode").val(data.ZipCode);
                //$("#Province option[value='" + data.ProvinceID + "']").attr("selected", "selected");
                //$.ajax({
                //    url: GetUrl("Default/GetDistrict"),
                //    data: { id: data.ProvinceID },
                //    type: "POST",
                //    success: function (data) {
                //        $("#District").html(data);
                //        $("#District option[value=" + data.DistrictID + "]").attr("selected", "selected");
                //    }
                //});
            }
        }

    });
}
/*--------------Sent----------------*/
var bool = true;
$("#btnSend").click(function (event) {
    event.preventDefault();
    Send();
});
$("#CompName").live('blur', function () {
    RequireCompName();
});
$("#FirstName").live('blur', function () {
    RequireName();
});
$("#Email").live('blur', function () {
    RequireEmail();
});
function RequireCompName() {
    var CompName = $("#CompName").val().trim();
    if ((CompName == "") || (CompName == null)) {
        $("#CompName").closest('.control-group').removeClass('success').addClass('error');
        bool = false;
    } else {
        if (CompName.length < 3) {
            $("#CompName").closest('.control-group').removeClass('success').addClass('error');
            bootbox.alert(label.vldless_3char);
            bool = false;
        } else {
            $("#CompName").closest('.control-group').addClass('success').removeClass('error');
            bool = true;
        }
    }
    return bool;
}
function RequireName() {
    var FirstName = $("#FirstName").val().trim();
    if ((FirstName == "") || (FirstName == null)) {
        $("#FirstName").closest('.control-group').removeClass('success');
        $("#FirstName").closest('.control-group').addClass('error');
        $('#FirstName').next().remove();
        $('#FirstName').after('<div class="fl_l mar_5 fontRed">กรุณากรอกชื่อของคุณ</div>');
        bool = false;
    } else {
        $('#FirstName').next().remove();
        if (FirstName.length < 3) {
            $("#FirstName").closest('.control-group').removeClass('success');
            $("#FirstName").closest('.control-group').addClass('error');
            $('#FirstName').after('<div class="fl_l mar_5 fontRed">กรุณากรอกอย่างน้อย 3 ตัวอักษร</div>');
            bool = false;
        } else {
            $("#FirstName").closest('.control-group').addClass('success');
            $("#FirstName").closest('.control-group').removeClass('error');
            $('#FirstName').next().remove();
            bool = true;
        }
    }
    return bool;
}
function RequireEmail() {
    var Email = $("#Email").val().trim();

    if ((Email == "") || (Email == null)) {
        $("#Email").closest('.control-group').removeClass('success');
        $("#Email").closest('.control-group').addClass('error');
        $("#Email").next().remove();
        $('#Email').after('<div class="fl_l mar_5 fontRed">' + label.vldfill_youremail + '</div>');
        bool = false;
    } else {
        var filter = /^([\w-]+(?:\.[\w-]+)*)@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$/i;
        var chk = false
        var checkSpace = $('#Email').val().replace(/ /g, '');

        if (checkSpace.length < Email.length) {
            $("#Email").closest('.control-group').removeClass('success');
            $("#Email").closest('.control-group').addClass('error');
            $("#Email").next().remove();
            $('#Email').after('<div class="fl_l mar_5 fontRed">' + label.vldengonly + '</div>');
            bool = false;
        } else {

            if (filter.test($('#Email').val()) == 0) {
                $("#Email").closest('.control-group').removeClass('success');
                $("#Email").closest('.control-group').addClass('error');
                $("#Email").next().remove();
                $('#Email').after('<div class="fl_l mar_5 fontRed">' + label.vldengonly + '</div>');
                bool = false;
            } else {
                $("#Email").closest('.control-group').addClass('success');
                $("#Email").closest('.control-group').removeClass('error');
                $("#Email").next().remove();
                bool = true;
            }
        }

    }
    return bool;
}

function RequireTel() {
    if (($("#Tel").val() == "") || ($("#Tel").val() == null)) {
        $("#Tel").closest('.control-group').removeClass('success');
        $("#Tel").closest('.control-group').addClass('error');
        $("#Tel").next().remove();
        $('#Tel').after('<div class="fl_l mar_5 fontRed">' + label.vldfill_yourphone + '</div>');
        bool = false;
        return false;
    } else {
        var tel = $("#Tel").val();
        if (tel.length >= 6) {
            $("#Tel").closest('.control-group').addClass('success');
            $("#Tel").closest('.control-group').removeClass('error');
            $("#Tel").next().remove();
            return true;
        } else {
            $("#Tel").next().remove();
            $('#Tel').after('<div class="fl_l mar_5 fontRed">' + label.vldfix_format_phone + '</div>');
            bool = false;
            return false;
        }
    }
}
function RequireQty() {
    if ($("#Qty").val() == 0) {
        bootbox.alert(label.vldcannot_insert_zero);
        bool = false;
    } else {
        bool = true;
    }
    return bool;
}
/*----------Request-Price--------*/
function Send() {
    RequireCompName();
    if (bool == true) {
        RequireName();
        if (bool == true) {
            RequireEmail();
            if (bool == true) {
                RequireQty();
            }
        }
    }
    //console.log("RequireCompName: " + bool);
    //console.log("RequireName: " + bool);
    //console.log("RequireEmail: " + bool);
    //console.log("RequireQty: " + bool);


    if (bool == true) {
        //console.log("bool: " + bool);

        var IsSentEmail = 0; var IsTelephone = 0; var IsAttach = 0; var RemarkContact = ""; var IsPublic = 0; var IsSupplierRelated = 0;

        if ($('input[name=IsPublic]:checked').attr("checked") == true || $('input[name=IsPublic]:checked').attr("checked") == "checked") {
            IsPublic = 1;
            //console.log("IsPublic: " + IsPublic);
        }

        if ($('input[name=IsSupplierRelated]:checked').attr("checked") == true || $('input[name=IsSupplierRelated]:checked').attr("checked") == "checked") {
            IsSupplierRelated = 1;
        }

        var Remark = $("#Remark").val() + "  " + RemarkContact;

        OpenLoading(true, null, $('.navbar'));
        $.ajax({
            url: GetUrl("MyB2B/Quotation/PostRequestPrice"),
            data: {
                ProductID: $("#ProductID").val(),
                Qty: $("#Qty").val(),
                QtyUnit: $("#QtyUnit").val(),
                ToCompID: $("#CompID").val(),
                FromCompID: $("#FromCompID").val(),
                CompanyName: $("#CompName").val(),
                ReqFirstName: $("#FirstName").val(),
                ReqPhone: $("#Tel").val(),
                ReqEmail: $("#Email").val(),

                ReqLastName: $("#LastName").val(),
                ReqAddrLine1: $("#AddrLine1").val(),
                ReqAddrLine2: $("#AddrLine2").val(),
                ReqSubDistrict: $("#SubDistrict").val(),
                ReqDistrictID: $("#DistrictID").val(),
                ReqPostalCode: $("#PostalCode").val(),

                hidQuotationFileName: $("input[name='hidQuotationFileName']").val(),
                hidQuotationFilePath: $("input[name='hidQuotationFilePath']").val(),
                hidQuotationUploadPath: $("input[name='hidQuotationUploadPath']").val(),
                hidImgSize: $("input[name='hidImgSize']").val(),

                IsSentEmail: IsSentEmail,
                IsTelephone: IsTelephone,
                IsAttach: IsAttach,
                IsPublic: IsPublic,
                Remark: Remark
            },
            success: function (data) {

                /*-----------------Sent-Mail-------------------*/
                var ProductName = $("#ProductName").val();
                var ProNameUrl = $("#ProNameUrl").val();
                var ProductCode = $("#ProductCode").val();
                console.log("ProductName: " + ProductName);
                console.log("ProNameUrl: " + ProNameUrl);

                $.ajax({
                    url: GetUrl("Quotation/ReplyRequest"),
                    data: {
                        ProductName: ProductName,
                        QuotationID: data.QuotationID,
                        ProductCode: ProductCode,
                        IsSupplierRelated: IsSupplierRelated,
                        Status: "Request"
                    },
                    success: function (data) {
                        console.log("data: " + data);
                        OpenLoading(false, null, $('.navbar'));
                        //bootbox.alert(label.vldrreqestbid + label.vldwaitbid);
                        bootbox.alert(label.vldrreqestbid + label.vldwaitbid, function () {
                            if ($("#UserLoginCompID").val() > 0) {
                                window.location = GetUrl("MyB2B/Quotation/List/Inbox");
                            }
                            else {
                                window.location = GetUrl("MyB2B/Quotation/RequestPrice/" + $("#ProductID").val());
                                //console.log("else" + $("#ProductID").val());
                            }
                        });

                    },
                    type: "POST"
                });
                /*-----------------------------------------------*/

            },
            type: "POST"
        });
    }
}
/*-------------MarkRead---------*/
function MarkRead() {
    if ($(".cbxItem").is(':checked')) {
        var ID = [];
        var RowVersion = [];
        var Check = [];
        $(".grid > tbody > tr").each(function (index) {
            if ($(this).children().find(".cbxItem:checked").val() == "true") {
                Check[index] = "True";
            }
            else {
                Check[index] = "False";
            }
            ID[index] = $(this).children().find(".hidPrimeID").val();
            RowVersion[index] = $(this).children().find(".hidRowVersion").val();
        });
        OpenLoading(true);
        if (ID.length > 0) {
            $.ajax({
                url: url + "Quotation/MarkRead",
                data: { Check: Check, ID: ID, RowVersion: RowVersion, PrimaryKeyName: "QuotationID" },
                success: function (data) {
                    if (data == "True" || data == true || data == "Success") {
                        $(g_hidsubmit).eq(g_no).click(); // Submit Grid
                        OpenLoading(false);
                        $("#information > #message").text(label.vldmark_unread_success);
                        $("#information").fadeIn();
                        $("#information").delay(3000).fadeOut(500);
                        return true;
                    } else {
                        return false;
                    }
                },
                type: "POST", traditional: true
            });
        }
        else {
            return false;
        }
    } else { bootbox.alert(label.vldno_item_selected) }
}
/*-------------Importance---------*/
function Importance(Obj, Status) {
    if ($('input[name=chkpd]:checked').attr("checked") == true || $('input[name=chkpd]:checked').attr("checked") == "checked") {
        Obj.prev().removeAttr('checked');
    } else {
        Obj.prev().attr('checked', 'checked');
    }
    var ID = Obj.prev().prev().prev().prev().val();
    var RowVersion = Obj.prev().prev().prev().val();
    OpenLoading(true);
    if (ID != 0) {
        if (Status == false || Status == "false") {
            $.ajax({
                url: url + "Quotation/ChangSataus",
                data: {
                    ID: ID,
                    RowVersion: RowVersion,
                    Condition: 'Importance',
                    IsImportance: Status
                },
                success: function (data) {
                    console.log(data);
                    if (data.Result = true) {
                        $(g_hidsubmit).eq(g_no).click(); // Submit Grid
                        OpenLoading(false);
                        $("#Inbox").html('<a href="/MyB2B/Quotation/List?Type=Inbox">Inbox (' + data.CountInbox + ')</a>');
                        $("#Importance").html('<a href="/MyB2B/Quotation/List?Type=Importance">Importance (' + data.CountImportance + ')</a>');
                        $("#Sentbox").html('<a href="/MyB2B/Quotation/List?Type=Sentbox">Sentbox (' + data.CountSentbox + ')</a>');
                        return true;
                    } else {
                        return false;
                    }
                },
                type: "POST"
            });
        } else {
            $.ajax({
                url: url + "Quotation/ChangSataus",
                data: {
                    ID: ID,
                    RowVersion: RowVersion,
                    Condition: 'Unimportance',
                    IsImportance: Status
                },
                success: function (data) {
                    console.log(data);
                    if (data.Result = true) {
                        $(g_hidsubmit).eq(g_no).click(); // Submit Grid
                        OpenLoading(false);
                        $("#Inbox").html('<a href="/MyB2B/Quotation/List?Type=Inbox">Inbox (' + data.CountInbox + ')</a>');
                        $("#Importance").html('<a href="/MyB2B/Quotation/List?Type=Importance">Importance (' + data.CountImportance + ')</a>');
                        $("#Sentbox").html('<a href="/MyB2B/Quotation/List?Type=Sentbox">Sentbox (' + data.CountSentbox + ')</a>');
                        return true;
                    } else {
                        return false;
                    }
                },
                type: "POST"
            });
        }
    }
    else {
        return false;
    }
}
/*------------DelData-----------*/
function DelData(id, RowVersion) {
    var ID = [];
    var RowVersion = [];
    var QuotationStatus = [];
    var Check = [];
    if (id == null || id == "") {
        $(".grid > tbody > tr").each(function (index) {
            if ($(this).children().find(".cbxItem:checked").val() == "true") {
                Check[index] = "True";
            }
            else {
                Check[index] = "False";
            }
            ID[index] = $(this).children().find(".hidPrimeID").val();
            RowVersion[index] = $(this).children().find(".hidRowVersion").val();
        });
    } else {
        ID[0] = id;
        RowVersion[0] = RowVersion;
        Check[0] = "True";
    }
    OpenLoading(true);
    if (ID.length > 0) {
        $.ajax({
            url: url + "Quotation/DelData",
            data: { Check: Check, ID: ID, RowVersion: RowVersion, PrimaryKeyName: "QuotationID" },
            success: function (data) {
                if (data.Result) {
                    $(g_hidsubmit).eq(g_no).click(); // Submit Grid
                    OpenLoading(false);
                    $("#Inbox").html('<a href="/MyB2B/Quotation/List?Type=Inbox/">Inbox (' + data.CountInbox + ')</a>');
                    $("#Importance").html('<a href="/MyB2B/Quotation/List?Type=Importance/">Importance (' + data.CountImportance + ')</a>');
                    $("#Sentbox").html('<a href="/MyB2B/Quotation/List?Type=Sentbox/">Sentbox (' + data.CountSentbox + ')</a>');
                    $("#information > #message").text(label.vlddel_msg_success);
                    $("#information").fadeIn();
                    $("#information").delay(3000).fadeOut(500);
                    return true;
                } else {
                    return false;
                }
            },
            type: "POST",
            traditional: true
        });
    }
    else {
        return false;
    }
}
function DelQuoData(id, version) {
    var ID = [];
    var RowVersion = [];
    var Check = [];
    if (id == null || id == "") {
        $(".grid > tbody > tr").each(function (index) {
            if ($(this).children().find(".cbxItem:checked").val() == "true") {
                Check[index] = "True";
            }
            else {
                Check[index] = "False";
            }
            ID[index] = $(this).children().find(".hidPrimeID").val();
            RowVersion[index] = $(this).children().find(".hidRowVersion").val();
        });
    } else {
        ID[0] = id;
        RowVersion[0] = version;
        Check[0] = "True";
    }
    OpenLoading(true);
    if (ID.length > 0) {
        $.ajax({
            url: url + "Quotation/DelQuoData",
            data: { Check: Check, ID: ID, RowVersion: RowVersion, PrimaryKeyName: "MessageID" },
            success: function (data) {
                if (data.Result) {
                    $(g_hidsubmit).eq(g_no).click(); // Submit Grid
                    OpenLoading(false);
                    $("#Inbox").html('<a href="/MyB2B/Quotation/List?Type=Inbox/">Inbox (' + data.CountInbox + ')</a>');
                    $("#Importance").html('<a href="/MyB2B/Quotation/List?Type=Importance/">Importance (' + data.CountImportance + ')</a>');
                    $("#Sentbox").html('<a href="/MyB2B/Quotation/List?Type=Sentbox/">Sentbox (' + data.CountSentbox + ')</a>');
                    $("#information > #message").text(label.vlddel_msg_success);
                    $("#information").fadeIn();
                    $("#information").delay(3000).fadeOut(500);
                    return true;
                } else {
                    return false;
                }
            },
            type: "POST", traditional: true
        });
    }
    else {
        return false;
    }
}
function DelAllData(id, RowVersion) {
    OpenLoading(true);
    $.ajax({
        url: url + "Quotation/DelAllData",
        //data: { Check: Check, ID: ID, RowVersion: RowVersion, PrimaryKeyName: "MessageID" },
        success: function (data) {
            if (data.Result) {
                $(g_hidsubmit).eq(g_no).click(); // Submit Grid
                OpenLoading(false);
                $("#Inbox").html('<a href="/MyB2B/Quotation/List?Type=Inbox/">Inbox (' + data.CountInbox + ')</a>');
                $("#Importance").html('<a href="/MyB2B/Quotation/List?Type=Importance/">Importance (' + data.CountImportance + ')</a>');
                $("#Sentbox").html('<a href="/MyB2B/Quotation/List?Type=Sentbox/">Sentbox (' + data.CountSentbox + ')</a>');
                $("#information > #message").text(label.vlddel_msg_success);
                $("#information").fadeIn();
                $("#information").delay(3000).fadeOut(500);
                return true;
            } else {
                return false;
            }
        },
        type: "POST", traditional: true
    });
}

/*-----------CheckAll------------*/
function CheckBoxall(Obj) {
    if (Obj.attr("value") == true || Obj.attr("checked") == "checked") {
        $(".cbxCompID").attr("checked", "checked");
        $(".cbxCompID").attr("value", "true");
        Obj.attr("checked", "checked");
        Obj.attr("value", "true");

    } else {
        $(".cbxCompID").removeAttr("checked");
        $(".cbxCompID").attr("value", "false");
        Obj.removeAttr("checked");
        Obj.attr("value", "false");
    }

}
function CheckBox(id) {
    if ($("#" + id + "").attr("value") == true || $("#" + id + "").attr("checked") == "checked") {
        $("#" + id + "").attr("checked", "checked");
        $("#" + id + "").attr("value", "true");
    } else {
        $("#" + id + "").removeAttr("checked");
        $("#" + id + "").attr("value", "false");
    }
}
/*-----------Detail-------------*/
function SaveData(Obj) {
    var bool = true;
    var ChkSentMail = 0;
    var txtVat = "";
    if (Obj == "Bid") {
        RequireCompName();
        if (bool == true) {
            RequireName();
            if (bool == true) {
                RequireEmail();
                if (bool == true) {
                    RequireQty();
                }
            }
        }
    }

    if ($('input[name=ChkSentMail]:checked').attr("checked") == true || $('input[name=ChkSentMail]:checked').attr("checked") == "checked") {
        ChkSentMail = 1;
    }
    var SumTotal = $('#SumTotal').val();
    var txtPrice = $('#txtPricePerPiece').val();
    var txtdiscount = $('#txtdiscount').val();
    if (Obj == "Bid") {
        if (SumTotal <= 0 || txtPrice <= 0 || txtPrice == "") {
            bootbox.alert('กรุณากรอกข้อมูลให้ถูกต้อง');
            bool = false;
        }
    }

    if (Obj == "Quotation") {
        if (SumTotal <= 0 || txtPrice <= 0 || txtPrice == "") {
            bootbox.alert('กรุณากรอกข้อมูลให้ถูกต้อง');
            bool = false;
        }
    }

    if (!bool) {
        return false;
    }

    else {
        if ($("#UserLoginCompID").val() > 0) {
            //alert("ล๊อคอิน");
            OpenLoading(true);

            var discount = $('#txtdiscount').val() == "" ? 0.0000 : $('#txtdiscount').val();
            var vat = $('#txtVAT').val() == "" ? 0.0000 : $('#txtVAT').val();

            data = {
                QuotationID: $('#QuotationID').val(),
                PricePerPiece: $('#txtPricePerPiece').val(),
                TotalPrice: $("#SumTotal").val(),
                Discount: discount,
                Vat: vat,
                IsSentEmail: ChkSentMail,
                Remark: $('#txtaRemark').val(),
                SaleName: $("#FirstName1").val(),
                SaleCompany: $("#CompName1").val(),
                SaleEmail: $("#Email1").val(),
                SalePhone: $("#Tel1").val(),
                hidQuotationFileName: $("#hidImgFileName").val(),
                hidQuotationFilePath: $(".hidImgFilePath").val(),
                hidQuotationUploadPath: $(".hidImgUploadPath").val(),
                hidImgSize: $("input[name='hidImgSize']").val(),
                Type: Obj
            }
            $.ajax({
                url: GetUrl("MyB2B/Quotation/SaveDetail"),
                data: data,
                success: function (data) {

                    if (data.Result = true) {
                        var QuotationID = data.ID;
                        /*-----------------Sent-Mail----------------*/
                        var ProductName = $("#ProductName").val();
                        var ProductCode = $("#ProductCode").val();
                        var ProductID = $("#ProductID").val();
                        var FromCompID = $("#ToCompID").val();
                        var FromName = $("#txtFromName").val();
                        var FromEmail = $("#txtFromEmail").val();
                        var CompID = $("#FromCompID").val();
                        var CompanyName = $("#CompanyName").val();
                        var ReqFirstName = $("#ReqFirstName").val();
                        var ToCompName = $("#txtToCompName").val();
                        var ToName = $("#txtToName").val();

                        Detail = "<table><tr><td>" + label.vlddear + " " + ReqFirstName + "  [" + CompanyName + "]<br></td></tr>";
                        Detail += "<tr><td>" + ToName + "[ " + ToCompName + " ] " + label.vldpro_bid + " " + ProductName + " " + label.vldget_quo + " <br></td></tr>";
                        Detail += "<tr><td>" + label.vldview_quo + " : " + label.vldDomainFullName + "/BidProduct/Reply/" + QuotationID + "</td></tr>";
                        $.ajax({
                            url: GetUrl("MyB2B/Quotation/ReplyRequest"),
                            data: {
                                ProductName: ProductName,
                                QuotationID: QuotationID,
                                ProductCode: ProductCode,
                                Status: "Reply",
                                MsgDetail: Detail,
                                RJFromName:FromName,
                                RJFromEmail: FromEmail,
                                IsSupplierRelated:0
                            },

                            success: function (data) {
                                /*---------ส่งให้ตัวเอง--------*/
                                if (ChkSentMail == 1) {
                                    $.ajax({
                                        url: GetUrl("Quotation/ReplyRequest"),
                                        data: {
                                            MsgDetail: Detail,
                                            ProductName: ProductName,
                                            ProductCode: ProductCode,
                                            QuotationID: QuotationID,
                                            Status: "Reply"
                                        },
                                        success: function (data) {
                                            if (Obj == "Quotation") {
                                                window.location = GetUrl('MyB2B/Quotation/List');
                                            } else {
                                                if ($("#ChkHaveUser").val() == "0") {
                                                    window.location = GetUrl('BidProduct/List');
                                                } else {
                                                    window.location = GetUrl('MyB2B/Quotation/List/Inbox');
                                                }
                                            }
                                        },
                                        type: "POST"
                                    });
                                } else {
                                    OpenLoading(false);
                                  
                                    bootbox.alert("ส่งข้อมูลเรียบร้อยแล้ว<br />คุณสามารถตรวจสอบข้อมูลการเสนอราคาของคุณได้ที่กล่อง \"ส่งแล้ว\"", function () {
                                        if (Obj == "Quotation") {
                                            window.location = GetUrl("MyB2B/Quotation/List/Sentbox");
                                        } else {
                                            if ($("#UserLoginCompID").val() > 0/*$("#ChkHaveUser").val() == "0"*/) {
                                                window.location = GetUrl("MyB2B/Quotation/List/Sentbox");
                                            } else {
                                                window.location = GetUrl("BidProduct/List");
                                            }
                                        }
                                    });
                                }

                            },
                            type: "POST"
                        });
                        return true;
                    } else {
                        return false;
                    }
                },
                type: "POST",
                traditional: true
            });
        } else {
            bootbox.alert("กรุณาเข้าสู่ระบบ");
       }
    }
}
function ChangeStatus(Condition) {
    OpenLoading(true);
    $.ajax({
        url: url + ("MyB2B/Quotation/ChangSataus"),
        data: {
            ID: $('#QuotationID').val(),
            RowVersion: $('#RowVersion').val(),
            Condition: Condition,
            IsImportance: $('#txtIsImportance').val()
        },
        success: function (data) {
            if (data.Result = true) {
                OpenLoading(false);
                if (data.IsDelete == 1) {
                    bootbox.alert(label.vlddel_success);
                    window.location = GetUrl('MyB2B/Quotation/List');
                } else {
                    $("#txtIsImportance").val(data.Status);
                    $("#RowVersion").val(data.RowVersion);
                    $("#Importance").html('<a href="/MyB2B/Quotation/List?Type=Importance/">Importance (' + data.CountImportance + ')</a>');
                    if (data.Status == true) {
                        $('.btnImportance').addClass('btn-success');
                        window.location.reload();
                        bootbox.alert('เพิ่มเป็นข้อความสำคัญเรียบร้อยแล้ว');
                    } else {
                        $('.btnImportance').removeClass('btn-success');
                        window.location.reload();
                        bootbox.alert('ยกเลิกข้อความสำคัญเรียบร้อยแล้ว');
                    }
                }
                return true;
            } else {
                return false;
            }
        },
        type: "POST"
    });
}
/*----------Send-Reject--------------*/
function SendReject(CompID) {
    var TypeQuotation = $('#TypeQuotation').val();

    if (TypeQuotation == "Sentbox") {
        bootbox.alert(label.vldquotationreject);
    } else {
        if ($('input[name=chkpd]:checked').attr("checked") == true || $('input[name=chkpd]:checked').attr("checked") == "checked") {

            if ($('input[name=ChkReject]:checked').attr("checked") == true || $('input[name=ChkReject]:checked').attr("checked") == "checked") {

                var RemarkRJ = "เกิดข้อผิดพลาดในการร้องขอราคาสินค้า เนื่องจาก -";

                $('input[name=ChkReject]:checked').each(function () {
                    RemarkRJ += $(this).val() + ",-";
                });

                if ($('#txtaRejectDetail').val() != '') {
                    RemarkRJ += $('#txtaRejectDetail').val();
                }
                else {
                    RemarkRJ = RemarkRJ.substring(0, RemarkRJ.length - 2);
                }

                /*----------------------------Send-Reject-----------------------*/
                var ID = [];
                var RowVersion = [];

                $('input[name=chkpd]:checked').each(function () {
                    ID[ID.length] = $(this).prev().prev().prev().val();
                    RowVersion[RowVersion.length] = $(this).prev().prev().val();
                });

                OpenLoading(true);
                if (ID.length > 0) {
                    $.ajax({
                        url: url + "Quotation/Reject",
                        data: { ID: ID, RowVersion: RowVersion, RejectDetail: RemarkRJ },
                        success: function (data) {
                            if (data.Result = true) {

                                /*-------------------------------*/
                                $("#Importance").text("Importance (" + data.CountImportance + ")");
                                $("#Inbox").text("Inbox (" + data.CountInbox + ")");
                                $("#Sentbox").text("Sentbox (" + data.CountSentbox + ")");
                                $("#myRequest").text("myRequest (" + data.CountmyRequest + ")");
                                /*------Close-Pophover------*/
                                $('.popover').css('display', 'none');

                                //-----------Message---------
                                var ToCompID = [];
                                $('input[name=chkpd]:checked').each(function () {
                                    ToCompID[ToCompID.length] = $(this).attr('rel');
                                });

                                $.ajax({
                                    url: GetUrl("Quotation/ReplyRequest"),
                                    data: {
                                        ProductName: $("#ProductName").val(),
                                        ProductCode: $("#ProductCode").val(),
                                        QuotationID: data.ID,
                                        RJFromName: $("#txtFromName").val(),
                                        RJFromEmail: $("#txtFromEmail").val(),
                                        MsgDetail: RemarkRJ,
                                        Status: "Reject",
                                        IsSupplierRelated: 0

                                    },
                                    success: function (data) {
                                        $(g_hidsubmit).eq(g_no).click(); // Submit Grid
                                        OpenLoading(false);
                                        bootbox.alert(label.vldsend_success);
                                    },
                                    type: "POST",
                                    traditional: true
                                });

                                return true;
                            } else {
                                return false;
                            }
                        },
                        type: "POST", traditional: true
                    });
                } else { return false; }

            } else { bootbox.alert(label.vldplease_select); }

        } else { bootbox.alert(label.vldplease_select); }
    }
}
/*----------Update-IsPublic----------*/
function UpdateIsPublic(Code) {
    $.ajax({
        url: GetUrl("MyB2B/Quotation/Update_IsPublic"),
        data: { QuotationCode: Code },
        success: function (data) {
            if (data.IsResult = true) {
                bootbox.alert(label.vldclosebidproduct);
                $('#IsPublic').addClass("hide");
            } else {
                bootbox.alert(label.vldcannot_closebid);
            }
        },
        type: "POST"
    });
}

$("body").click(function (event) {
    if ($(event.target).hasClass('search-select')) {
        OpenListSearch(true);
    } else {
        OpenListSearch(false);
    }
});
$('#TextSearch').keypress(function () {
    OpenListSearch(true);
});
$('#TextSearch').click(function () {
    var width = $('#search-main').width();
    $('#list-search').width(width - 2);
    OpenListSearch(true);
});

function OpenListSearch(isOpen) {
    if (isOpen != null && isOpen != undefined) {
        if (isOpen) {
            $('#list-search').removeClass('hidden')
            $('#list-search').slideDown();
        } else {
            $('#list-search').addClass('hidden')
            $('#list-search').slideUp();
        }
    } else {
        if ($('#list-search').hasClass('hidden')) {
            $('#list-search').removeClass('hidden')
            $('#list-search').slideDown();
        } else {
            $('#list-search').addClass('hidden')
            $('#list-search').slideUp();
        }
    }
}
function SetActiveListSearch(index) {
    $('.list-search-text').removeClass('active');
    $('.list-search-text').eq(index).addClass('active');

    $('.icon-active').hide();
    $('.icon-active').eq(index).show();
}

$(window).scroll(function () {
    if ($(window).scrollTop() > 60) {
        $('#Qty').css('z-index', -1);
    }
    else {
        $('#Qty').css('z-index', 0);
    }
});

function delQuotation() {
    if ($(".cbxItem").is(':checked')) {
        if (confirm(label.vldconfirm_del_data)) { DelData(); }
    } else { bootbox.alert(label.vldno_item_selected); }
}
function delQuo() {
    if ($(".cbxItem").is(':checked')) {
        if (confirm(label.vldconfirm_del_data)) { DelQuoData(); }
    } else { bootbox.alert(label.vldno_item_selected); }
}
function delAllQuotation() {
    if (confirm("ต้องการล้างข้อมูลหรือไม่")) { DelAllData(); }
}
function delQuoDetail() {
    var id = $(".hidPrimeID").val();
    var version = $(".hidRowVersion").val();
    if (confirm(label.vldconfirm_del_data)) {
        DelQuoData(id, version)
    }
}
/*------------------------------MoveData-------------------------------------*/
function Move() {
    var ID = [];
    var RowVersion = [];
    var IsOutbox = [];
    var Check = [];
    var count = 0;
    $(".grid > tbody > tr").each(function (index) {
        if ($(this).children().find(".cbxItem:checked").val() == "true") {
            Check[index] = "True";
            count++;
            IsOutbox[0] = $(this).children().find(".hidIsOutbox").val();
        }
        else {
            Check[index] = "False";
        }
        ID[index] = $(this).children().find(".hidPrimeID").val();
        RowVersion[index] = $(this).children().find(".hidRowVersion").val();
    });
    if (count == 1) {
        var text = "";
        if (IsOutbox[0] == "True") {
            text = "ส่งแล้ว";
        } else {
            text = "ข้อความเข้า";
        } 
        if ($(".cbxItem").is(':checked')) {
            if (confirm("ต้องการย้ายข้อมูลกลับไปยัง '" + text + "' หรือไม่")) {
                OpenLoading(true);
                if (ID.length > 0) {
                    $.ajax({
                        url: url + "Quotation/MoveData",
                        data: { Check: Check, ID: ID, RowVersion: RowVersion, PrimaryKeyName: "MessageID" },
                        success: function (data) {
                            if (data.Result) {
                                OpenLoading(false);
                                $("#Importance").text("Importance (" + data.CountImportance + ")");
                                if (data.CountInbox > 0) { $("#Inbox").text("Inbox (" + data.CountInbox + ")"); }
                                $("#Draftbox").text("Draftbox (" + data.CountDraftbox + ")");
                                $("#Sentbox").text("Sentbox (" + data.CountSentbox + ")");
                                $(g_hidsubmit).eq(g_no).click(); // Submit Grid
                                $("#information > #message").text('ย้ายข้อความสำเร็จ');
                                $("#information").fadeIn();
                                $("#information").delay(3000).fadeOut(500);
                                return true;
                            } else {
                                return false;
                            }
                        },
                        type: "POST", traditional: true
                    });
                }
                else {
                    return false;
                }
            }
        } else { bootbox.alert(label.vldno_item_selected); }
    } else {
        bootbox.alert("กรุณาเลือกเพียง 1 รายการ");
    }
}
function MoveDetail() {
    ID = $("#QuotationID").val();
    RowVersion = $("#RowVersion").val();

    var text = "";
    if ($("#IsOutbox").val() == "True") {
        text = "ส่งแล้ว";
    } else {
        text = "ข้อความเข้า";
    }

    if (confirm("ต้องการย้ายข้อมูลกลับไปยัง '" + text + "' หรือไม่")) {
        OpenLoading(true);
        if (ID.length > 0) {
            $.ajax({
                url: url + "Quotation/MoveDataDetail",
                data: { ID: ID, RowVersion: RowVersion, PrimaryKeyName: "MessageID" },
                success: function (data) {
                    if (data.Result) {
                        OpenLoading(false);
                        $("#Importance").text("Importance (" + data.CountImportance + ")");
                        if (data.CountInbox > 0) { $("#Inbox").text("Inbox (" + data.CountInbox + ")"); }
                        $("#Draftbox").text("Draftbox (" + data.CountDraftbox + ")");
                        $("#Sentbox").text("Sentbox (" + data.CountSentbox + ")");
                        $(g_hidsubmit).eq(g_no).click(); // Submit Grid
                        $("#information > #message").text('ย้ายข้อความสำเร็จ');
                        $("#information").fadeIn();
                        $("#information").delay(3000).fadeOut(500);

                        window.location = GetUrl('MyB2B/Quotation/List/Trash');

                        return true;
                    } else {
                        return false;
                    }
                },
                type: "POST", traditional: true
            });
        }
        else {
            return false;
        }
    }
}

function ChangeImportantList() {
    if ($(".cbxItem").is(':checked')) {
        var ID = [];
        var RowVersion = [];
        var Check = [];

        $(".grid > tbody > tr").each(function (index) {
            if ($(this).children().find(".cbxItem:checked").val() == "true") {
                Check[index] = "True";
            }
            else {
                Check[index] = "False";
            }
            ID[index] = $(this).children().find(".hidPrimeID").val();
            RowVersion[index] = $(this).children().find(".hidRowVersion").val();
        });

        OpenLoading(true);
        if (ID.length > 0) {
            $.ajax({
                url: url + "Quotation/ChangeTagList",
                data: { Check: Check, ID: ID, RowVersion: RowVersion, PrimaryKeyName: "MessageID" },
                success: function (data) {
                    if (data != null) {
                        OpenLoading(false);
                        $("#Importance").text("Importance (" + data.CountImportance + ")");
                        if (data.CountInbox > 0) { $("#Inbox").text("Inbox (" + data.CountInbox + ")"); }
                        $("#Draftbox").text("Draftbox (" + data.CountDraftbox + ")");
                        $("#Sentbox").text("Sentbox (" + data.CountSentbox + ")");
                        $(g_hidsubmit).eq(g_no).click(); // Submit Grid
                        $("#information > #message").text(label.vldmark_unimportance_success);
                        $("#information").fadeIn();
                        $("#information").delay(3000).fadeOut(500);
                        return true;
                    }
                    else {
                        bootbox.alert(label.vldsave_unsuccess);
                    }
                },
                type: "POST", traditional: true
            });
        }
        else {
            return false;
        }
    } else { bootbox.alert(label.vldno_item_selected) }
}
