//======================================================= Ajax Register =============================================//
$("#btn-register").click(function () {
    if ($("#Form-Register").valid()) {
        LoadingMobile(true);
        $.ajax({
            url: GetUrl("B2BMobile/Register"),
            type: "POST",
            data: {
                UserName: $("#UserName").val(),
                Password: $("#Password").val(),
                BizTypeID: $("#BizTypeID").val(),
                CompName: $("#CompName").val(),
                FirstName: $("#FirstName").val(),
                LastName: $("#LastName").val(),
                Phone: $("#Phone").val(),
                Emails: $("#Emails").val(),
                ProvinceID: $("#ProvinceID").val(),
                DistrictID: $("#DistrictID").val()
            },
            success: function (e) {
                if (e.IsSuccess) {
                    setTimeout(function () { LoadingMobile(false); }, 1000);
                    window.location = e.Result;
                    LoadingMobile(false);
                }
                else {
                    setTimeout(function () { LoadingMobile(false); }, 1000);
                    $('#myModalAlert').modal('show');
                    $('.ModalMsg').html(e.Result);
                    $('.ModalOK').click(function () {
                        location.reload();
                        $('#myModalAlert').modal('hide');
                    })
                }
            },
            error: function (e) {
                setTimeout(function () { LoadingMobile(false); }, 1000);
                $('#myModalAlert').modal('show');
                $('.ModalMsg').html(e.Result);
                $('.ModalOK').click(function () {
                    location.reload();
                    $('#myModalAlert').modal('hide');
                })
            }
        });
    }
});
//======================================================= Ajax Signin =============================================//
$("#btn-Signin").click(function () {      
    if ($("#Form-Signin").valid()) {
        LoadingMobile(true);
        $.ajax({
            url: GetUrl("B2BMobile/SignIn"),
            type: "POST",
            data: {
                UserName: $("#UserName").val(),
                Password: $("#Password").val(),
            },
            success: function (e) {
                if (e.IsSuccess) {
                    window.location = e.result;
                    LoadingMobile(false);
                }
                else {
                    $("#error-login").addClass('validate-login');
                    $("#error-login").html("<div class='warning-icon'><img src='/Content/B2BMobile/Images/warning.png'/></div><div class='error-detail'>" + e.result + "</div>");
                    LoadingMobile(false);
                    $("#UserName").focus();
                }
            },
            error: function (e) {
                LoadingMobile(false);
                $('#myModalAlert').modal('show');
                $('.ModalMsg').html('ขออภัยเกิดข้อผิดพลาดในระบบ');
                $('.ModalOK').click(function () {
                    location.reload();
                    $('#myModalAlert').modal('hide');
                })
            }
        });
    }
});
//======================================================= Ajax Edit Prifile =============================================//
$("#btn-editprofile").click(function () {
    if ($("#Form-EditProfile").valid()) {
        LoadingMobile(true);
        $.ajax({
            url: GetUrl("B2BMobile/EditProfile"),
            type: "POST",
            data: {
                MemberID: $("#MemberID").val(),
                CompID: $("#CompID").val(),
                CompproID: $("#CompproID").val(),
                CompName: $("#CompName").val(),
                FirstName: $("#FirstName").val(),
                LastName: $("#LastName").val(),
                Phone: $("#Phone").val(),
                Emails: $("#Emails").val(),
                ProvinceID: $("#ProvinceIDEdit").val(),
                DistrictID: $("#DistrictID").val()
            },
            success: function (e) {
                if (e.IsSuccess) {
                    LoadingMobile(false);
                    $('#myModalAlert').modal('show');
                    $('.ModalMsg').html(e.Result);
                    $('.ModalOK').click(function () {
                        location.reload();
                    })
                }
                else {
                    LoadingMobile(false);
                    $('#myModalAlert').modal('show');
                    $('.ModalMsg').html(e.Result);
                    $('.ModalOK').click(function () {
                        $('#myModalAlert').modal('hide');
                    })
                }
            },
            error: function (e) {
                LoadingMobile(false);
                $('#myModalAlert').modal('show');
                $('.ModalMsg').html(e.Result);
                $('.ModalOK').click(function () {
                    $('#myModalAlert').modal('hide');
                })
            }
        });
    }
});
//======================================================= Ajax Change Password =============================================//
function CheckFormValidate() {
    if ($("#Form-ChangePassword").valid()) {
        $("#Submit-ChangPass").attr('disabled', false);
    }
    else {
        $("#Submit-ChangPass").attr('disabled', true);
    }
}

/*----------------------------------------submit-----------------------------------*/
$("#Submit-ChangPass").click(function () {
    if ($("#Form-ChangePassword").valid()) {
        data = {
            NewPassword: $('#NewPassword').val(),
            ConfirmPassword: $('#ConfirmPassword').val(),
        }
        LoadingMobile(true);
        $.ajax({
            url: GetUrl("B2BMobile/ChangePass"),
            data: data,
            type: "POST",
            success: function (data) {
                if (data["result"]) {
                    LoadingMobile(false);
                    $('#myModalAlert').modal('show');
                    $('.ModalMsg').html('เปลี่ยนรหัสผ่านเรียบร้อยแล้ว');
                    $('.ModalOK').click(function () {
                        window.location = "Index";
                    })
                    $('#OldPassword').val('');
                    $('#NewPassword').val('');
                    $('#ConfirmPassword').val('');
                    $("#OldPassword").closest('.control-group').removeClass('success error');
                    $("#NewPassword").closest('.control-group').removeClass('success error');
                    $("#ConfirmPassword").closest('.control-group').removeClass('success error');
                }
                else {
                    $('#myModalAlert').modal('show');
                    $('.ModalMsg').html('ไม่สามารถเปลี่ยนรหัสผ่านได้');
                    $('.ModalOK').click(function () {
                        location.reload();
                        $('#myModalAlert').modal('hide');
                    })
                }
            },
            error: function () {
            }
        });
    }
});
//======================================================= Ajax Forget Password =============================================//
$("#btn-forgetPass").click(function () {
    if ($("#Form-forgetPass").valid()) {
        LoadingMobile(true);
        $.ajax({
            url: GetUrl("B2BMobile/ForgetPassword"),
            type: "POST",
            data: {
                EmailorUsername: $("#EmailorUsername").val(),
            },
            success: function (e) {
                if (e.IsSuccess) {
                    $("#warning-forgetpass").removeClass('validate-forgeterror');
                    $("#warning-forgetpass").addClass('validate-forgettrue');
                    $("#warning-forgetpass").html("<div class='warning-icon'><img src='/Content/B2BMobile/Images/icontrue.png' /></div><div class='warning-detail'>" + e.Result + "</div>");
                    LoadingMobile(false);
                    $("#EmailorUsername").val("");
                    $("#EmailorUsername").focus();
                }
                else {
                    $("#warning-forgetpass").removeClass('validate-forgettrue');
                    $("#warning-forgetpass").addClass('validate-forgeterror');
                    $("#warning-forgetpass").html("<div class='warning-icon'><img src='/Content/B2BMobile/Images/warning.png'/></div><div class='error-detail'>" + e.Result + "</div>");
                    LoadingMobile(false);
                    $("#EmailorUsername").focus();
                }
            },
            error: function (e) {
                LoadingMobile(false);
                $('#myModalAlert').modal('show');
                $('.ModalMsg').html('ขออภัยเกิดข้อผิดพลาดในระบบ');
                $('.ModalOK').click(function () {
                    $('#myModalAlert').modal('hide');
                })
            }
        });
    }
});
//======================================================= Ajax Update Password =============================================//
function CheckFormUpdate() {
    if ($("#Form-UpdatePass").valid()) {
        $("#Submit-UpdatePass").attr('disabled', false);
    }
    else {
        $("#Submit-UpdatePass").attr('disabled', true);
    }
}

$("#Submit-UpdatePass").click(function () {
    if ($("#Form-UpdatePass").valid()) {
        data = {
            NewPassword: $('#NewUpdatePass').val(),
            ConfirmPassword: $('#CfUpdatePass').val(),
            member: $('#member').val()
        }
        LoadingMobile(true);
        $.ajax({
            url: GetUrl("B2BMobile/UpdatePassword"),
            data: data,
            type: "POST",
            success: function (data) {
                if (data.result) {
                    LoadingMobile(false);
                    $('#myModalAlert').modal('show');
                    $('.ModalMsg').html(data.ErrorMsg);
                    $('.ModalOK').click(function () {
                        $("input[type=password]").val("");
                        window.location = GetUrl("B2BMobile/index");
                    })
                }
                else {
                    LoadingMobile(false);
                    $('#myModalAlert').modal('show');
                    $('.ModalMsg').html(data.ErrorMsg);
                    $('.ModalOK').click(function () {
                        $('#myModalAlert').modal('hide');
                        $("input[type=password]").val("");
                        $('#NewUpdatePass').focus();
                    })
                }
            },
            error: function () {
            }
        });
    }
});

//======================================================= Ajax Request Price =============================================//
var IsSentEmail = 0;
var IsTelephone = 0;
var IsAttach = 0;
var RemarkContact = "";
var IsPublic = 0;
           
$("#btn-Requestprice").click(function () {
    IsPublic = document.getElementById("IsPublic").checked ? 1 : 0;
    if ($("#Form-Requestprice").valid()) {
        LoadingMobile(true);
        $.ajax({
            url: GetUrl("B2BMobile/RequestPrice"),
            type: "POST",
            data: {
                ProductID: $("#ProductID").val(),
                Qty: $("#Qty").val(),
                QtyUnit: $("#QtyUnit").val(),
                ToCompID: $("#ToCompID").val(),
                CompanyName: $("#CompanyName").val(),
                FromCompID: $("#FromCompID").val(),
                ReqFirstName: $("#ReqFirstName").val(),
                ReqEmail: $("#ReqEmail").val(),
                ReqPhone: $("#ReqPhone").val(),
                ReqLastName: $("#LastName").val(),
                ReqAddrLine1: $("#AddrLine1").val(),
                ReqAddrLine2: $("#AddrLine2").val(),
                ReqSubDistrict: $("#SubDistrict").val(),
                ReqDistrictID: $("#DistrictID").val(),
                ReqPostalCode: $("#PostalCode").val(),
                Remark: $("#Remark").val(),
                IsSentEmail: IsSentEmail,
                IsTelephone: IsTelephone,
                IsAttach: IsAttach,
                IsPublic: IsPublic,
            },
            success: function (e) {
                if (e.IsSuccess == true) {
                    LoadingMobile(false);
                    $('#myModalAlert').modal('show');
                    $('.ModalMsg').html(e.Result);
                    $('.ModalOK').click(function () {
                        window.history.back(-1);
                    });
                }
                else {
                    LoadingMobile(false);
                    $('#myModalAlert').modal('show');
                    $('.ModalMsg').html(e.Result);
                    $('.ModalOK').click(function () {
                        $('#myModalAlert').modal('hide');
                    })
                }
            }
        });
    }
});

//======================================================= Ajax Quotation Price ============================================= //
$("#btn-Quotation").click(function () {
    if ($("#Form-Quotation").valid()) {
        LoadingMobile(true);
        $.ajax({
            url: GetUrl("B2BMobile/SaveQuotation"),
            type: "POST",
            data: {
                QuotationID: $("#QuoID").val(),
                PricePerPiece: $("#RequestPrice").val(),
                TotalPrice: $("#hidSumPrice1").val(),
                Discount: $("#DisCount").val(),
                Vat: $("#VAT").val(),
                SaleName: $("#FirstName").val(),
                SaleEmail: $("#Emails").val(),
                SalePhone: $("#Phone").val(),
                IsSentEmail: IsSentEmail,
            },
            success: function (e) {        
                if (e.IsSuccess == true) {
                    LoadingMobile(false);
                    $('#myModalAlert').modal('show');
                    $('.ModalMsg').html(e.Result);
                    $('.ModalOK').click(function () {
                        window.history.back(-1);
                    });
                }
                else {
                    LoadingMobile(false);
                    $('#myModalAlert').modal('show');
                    $('.ModalMsg').html(e.Result);
                    $('.ModalOK').click(function () {
                        $('#myModalAlert').modal('hide');
                    })
                }
            },
            error: function (e) {
                LoadingMobile(false);
                $('#myModalAlert').modal('show');
                $('.ModalMsg').html('เกิดปัญหาภายในระบบ');
                $('.ModalOK').click(function () {
                    $('#myModalAlert').modal('hide');
                })
            }
        });
    }
});

//======================================================= Ajax Delete Quotation =============================================//
$("#btn-deleteQou").click(function () {
    $('#myModalConfirm').modal('show');
    $('.ModalMsg').html("ต้องการลบใบเสนอราคานี้หรือไม่ ?");
    $('.ModalCOK').click(function () {
        LoadingMobile(true);
        $.ajax({
            url: GetUrl("B2BMobile/DelQuotation"),
            type: "POST",
            data: { ID: $("#hidIDQuo").val() },
            success: function (e) {
                if (e.IsSuccess == true) {
                    LoadingMobile(false);
                    $('#myModalConfirm').modal('hide');
                    $('#myModalAlert').modal('show');
                    $('.ModalMsg').html(e.Result);
                    $('.ModalOK').click(function () {
                        window.location = GetUrl("B2BMobile/ListQuotation");
                    });
                } else {
                    LoadingMobile(false);
                    $('#myModalConfirm').modal('hide');
                    $('#myModalAlert').modal('show');
                    $('.ModalMsg').html(e.Result);
                    $('.ModalOK').click(function () {
                        $('#myModalAlert').modal('hide');
                    });
                }
            },
            error: function (e) {
                LoadingMobile(false);
                $('#myModalConfirm').modal('hide');
                $('#myModalAlert').modal('show');
                $('.ModalMsg').html('เกิดปัญหาภายในระบบ');
                $('.ModalOK').click(function () {
                    $('#myModalAlert').modal('hide');
                });
            }
        });
    });
});

//======================================================= Ajax Delete Message =============================================//
$("#btn-deleteMsg").click(function () {
    $('#myModalConfirm').modal('show');
    $('.ModalMsg').html("ต้องการลบข้อความนี้หรือไม่ ?");
    $('.ModalCOK').click(function () {
        LoadingMobile(true);
        $.ajax({
            url: GetUrl("B2BMobile/DelMessage"),
            type: "POST",
            data: { ID: $("#hidIDMsg").val() },
            success: function (e) {
                if (e.Result == true) {
                    LoadingMobile(false);
                    $('#myModalConfirm').modal('hide');
                    $('#myModalAlert').modal('show');
                    $('.ModalMsg').html('ลบข้อความสำเร็จ');
                    $('.ModalOK').click(function () {
                        window.location = GetUrl("B2BMobile/ListMessage?MsgType=Inbox");
                    });
                } else {
                    LoadingMobile(false);
                    $('#myModalConfirm').modal('hide');
                    $('#myModalAlert').modal('show');
                    $('.ModalMsg').html('ลบข้อความไม่สำเร็จ โปรดลองอีกครั้ง');
                    $('.ModalOK').click(function () {
                        $('#myModalAlert').modal('hide');
                    });
                }  
            },
            error: function (e) {
                LoadingMobile(false);
                $('#myModalConfirm').modal('hide');
                $('#myModalAlert').modal('show');
                $('.ModalMsg').html('เกิดปัญหาภายในระบบ');
                $('.ModalOK').click(function () {
                    $('#myModalAlert').modal('hide');
                });
            }
        });
    });
});

//======================================================= Ajax Send Message =============================================//
var IsImportance = false;

//--------------------- New Message -------------------------//
$("#btn-SendMsg").click(function () {
    if ($("#Form-Message").valid()) {
        IsImportance = document.getElementById("IsImportance").checked ? true : false;
        LoadingMobile(true);
        $.ajax({
            url: GetUrl("B2BMobile/MessageContact"),
            type: "POST",
            data: {
                hidToCompID: $("#hidToCompID").val(),
                MessageDetail: $("#MessageDetail").val(),
                IsImportance: IsImportance,
                txtSubject: $("#txtSubject").val(),
                txtFromName: $("#txtFromName").val(),
                txtFromContactPhone: $("#txtFromContactPhone").val(),
                txtToCompName: $("#txtToCompName").val(),
                hidToCompEmail: $("#hidToCompEmail").val(),
                txtFromEmail: $("#txtFromEmail").val()
            },
            success: function (e) {
                LoadingMobile(false);
                $('#myModalAlert').modal('show');
                $('.ModalMsg').html('ส่งข้อความสำเร็จ');
                $('.ModalOK').click(function () {
                    var login = $(".hidLogin").val();
                    if (login == "0") {
                        window.history.back(-1);
                    } else {
                        window.location = GetUrl("B2BMobile/ListMessage?MsgType=Inbox");
                    }
                })
            },
            error: function (e) {
                LoadingMobile(false);
                $('#myModalAlert').modal('show');
                $('.ModalMsg').html('การส่งข้อความเกิดข้อผิดพลาด');
                $('.ModalOK').click(function () {
                    $('#myModalAlert').modal('hide');
                })
            }
        });
    }
});

//--------------- Reply Message -------------------------//
$("#btn-ReplyMsg").click(function () {
    if ($("#Form-replyMsg").valid()) {
        var Detail = $("#MsgDetail").val();
        var hidDetail = $("#hidMsgDetail").val();
        LoadingMobile(true);
        $.ajax({
            url: GetUrl("B2BMobile/MessageNew"),
            type: "POST",
            data: {
                hidToCompID: $("#hidToCompIDRE").val(),
                hidMsgID: $("#hidMsgID").val(),
                MsgDetail: "<br>" + Detail + hidDetail,
                txtSubject: $("#txtSubjectRE").val(),
            },
            success: function (e) {
                if (e) {
                    LoadingMobile(false);
                    $('#myModalAlert').modal('show');
                    $('.ModalMsg').html('ส่งข้อความสำเร็จ');
                    $('.ModalOK').click(function () {
                        window.location = GetUrl("B2BMobile/ListProduct");
                    })
                } else {
                    LoadingMobile(false);
                    $('#myModalAlert').modal('show');
                    $('.ModalMsg').html('การส่งข้อความเกิดข้อผิดพลาด');
                    $('.ModalOK').click(function () {
                        $('#myModalAlert').modal('hide');
                    })
                }
            },
            error: function (e) {
                LoadingMobile(false);
                $('#myModalAlert').modal('show');
                $('.ModalMsg').html('การส่งข้อความเกิดข้อผิดพลาด');
                $('.ModalOK').click(function () {
                    $('#myModalAlert').modal('hide');
                })
            }
        });
    }
});

//==================================================== Ajax Reply Message =============================================//
$("#btn-ReplyMsg").click(function () {
    if ($("#Form-replyMsg").valid()) {
        $.ajax({
            url: GetUrl("B2BMobile/ReplyMessage"),
            type: "POST",
            data: {
                msgid: $("#hidIDMsg").val(),
            },
            success: function (data) {
                $("#txtSubjectRE").val(data["msgSubject"]);
                $("#hidMsgDetail").val(data["msgDetail"]);
            }
        });
    }
});