
$(function () {
    $('#myTab a:first').tab('show');
    $('#myTab a').click(function (e) {
        e.preventDefault();
        $(this).tab('show');
    });

    $("#chkB2BAccount").attr("checked", false);
    $(".B2bAccountFiled").attr("disabled", "disabled");
    $("#autoHeight").slideDown(function () {
        $("#sidebar").height($("#auroHeight").height());
        $("#main").height($("#auroHeight").height());
    });
});

function displaySuccess() {
    OpenLoading(false);
    bootbox.alert(label.vldsend_purchas_success);
    location.reload(true);
}

//-------------Toogle icon------------//
function toggleAddClass(Obj) {
    if (Obj.hasClass("icon_NotAdd") == true) {
        Obj.removeClass("icon_NotAdd");
        Obj.addClass("icon_Add");
        //Obj.next().removeClass("hide");

    } else {
        Obj.removeClass("icon_Add");
        Obj.addClass("icon_NotAdd");
        //Obj.next().addClass("hide");
    }
    checkToRemoveThisCompany(Obj);
}

function toggleEmailClass(Obj) {
    if (Obj.hasClass("icon_NotSendEmail") == true) {
        Obj.removeClass("icon_NotSendEmail");
        Obj.addClass("icon_SendEmail");
    } else {
        Obj.removeClass("icon_SendEmail");
        Obj.addClass("icon_NotSendEmail");
    }
    //Check for attach user's data
    var compid = $("#hidAllSupplierCompID").val()
    var ArrayCompID = compid.split(',');
    GetIsAttachUserData(ArrayCompID);
}

//-------------Toogle textbox------------//
function toggleTextbox(Obj) {
    var txtInput_Selector = Obj.parent().parent().find('.controls').children();
    if (Obj.attr("value") == true || Obj.attr("checked") == "checked") {
        txtInput_Selector.removeAttr("disabled");
        Obj.attr("value", true);

    } else {
        txtInput_Selector.attr("disabled", "disabled");
        txtInput_Selector.val("");
        Obj.attr("value", false);
        Obj.closest('.control-group').removeClass('error');
        Obj.closest('.control-group').removeClass('success');
        Obj.parent().next().children('label').remove();
    }
}

function registerMember() {
    if ($("#chkRegisterMember").attr("value") == true || $("#chkRegisterMember").attr("checked") == "checked") {
        $(".RegisterMember").removeClass('hide');
        $("#chkRegisterMember").attr("value", true);
        $("div.ToCompName").addClass('hide');
        $("#inputContactLastName").removeClass('reg');
        $("#inputRegCompName").removeClass('reg');
        $("#txtUsername").removeClass('reg');
        $("#txtPassword").removeClass('reg');
        $("input[name=regServiceType]").removeClass('reg');
        $("#selectBusinessType").removeClass('reg');
    } else {
        $(".RegisterMember").addClass('hide');
        $("div.ToCompName").removeClass('hide');
        clearForm('#RegisterMemberForm');
        $("#chkRegisterMember").attr("value", false);
        $("#inputContactLastName").addClass('reg');
        $("#inputRegCompName").addClass('reg');
        $("#txtUsername").addClass('reg');
        $("#txtPassword").addClass('reg');
        $("input[name=regServiceType]").addClass('reg');
        $("#selectBusinessType").addClass('reg');
    }
}


//////////////////////////////////////////////////////////////////
//------------------- Add company list---------------------//
var $ListCompTable = $("#ListCompany");
var NoCompany = "<tr id='NoCompanyRow'><td></td><td>No Company</td><td></td></tr>";

function AddCompanyList(CompID) {

    var productname = $("input[name='TextSearch']").val();
    $("#ShowProductName").text(productname);
    $("#hidProductName").val(productname);

    //CreateEmLead
    $("#inputKeyword").val(productname);
    $(".myvalidate").text("");

    if ($ListCompTable.find("[id=t" + CompID + "]").length < 1) {
        if ($ListCompTable.find("[id=i" + CompID + "]").length < 1) {
            $("#NoCompanyRow").remove();

            var strTable = "";
            var CompName = $("#CompName-" + CompID).text();

            strTable += "<tr id='t" + CompID + "'><td class='RowNumber'></td><td style='text-align:left'>" + CompName + "</td><td><i class='icon-remove-sign cursor' onclick='RemoveThisCompany($(this))'></i></td></tr>";
            $ListCompTable.find("tbody").append(strTable);

            var count = ManageSequenceNumber();
            $("#hidCountCompLead").val(count);
        }

    } 
}
function RemoveThisCompany(Obj) {
    var rowid = Obj.parent().parent().attr("id");
    var id = rowid.substr(1);
    $("tr.body").children().children().find("#" + id).removeClass("icon_Add");
    $("tr.body").children().children().find("#" + id).addClass("icon_NotAdd");
    $("tr.body").children().children().find("#" + id).next().removeClass("icon_SendEmail");
    $("tr.body").children().children().find("#" + id).next().addClass("icon_NotSendEmail");
    $("tr.body").children().children().find("#" + id).next().addClass("hide");
    Obj.parent().parent().remove();

    //Check No Company
    if ($ListCompTable.find("tr").length == 1) {
        $ListCompTable.find("tbody").append(NoCompany);
        $("#ShowProductName").text("");
        $("#hidProductName").val("");
        $("#hidCountCompLead").val("");
        $("#hidAllSupplierCompID").val("");
        $("#hidAttachEmailCompID").val("");
    }

    var count = ManageSequenceNumber();
    $("#hidCountCompLead").val(count);
}

function checkToRemoveThisCompany(Obj) {
    if (Obj.hasClass("icon_NotAdd") == true) {

        var id = Obj.attr("id");
        $ListCompTable.find("[id=t" + id + "]").remove();

        $("tr.body").children().children().find("#" + id).next().removeClass("icon_SendEmail");
        $("tr.body").children().children().find("#" + id).next().addClass("icon_NotSendEmail");

        //Check No Company
        if ($ListCompTable.find("tr").length == 1) {
            $ListCompTable.find("tbody").append(NoCompany);
            $("#ShowProductName").text("");
            $("#hidProductName").val("");
            $("#hidCountCompLead").val("");
            $("#hidAllSupplierCompID").val("");
            $("#hidAttachEmailCompID").val("");
        }

        var count = ManageSequenceNumber();
        $("#hidCountCompLead").val(count);
    }

}

function SelectAllCompany(Obj) { 
    if (Obj.attr("value") == true || Obj.attr("checked") == "checked") {
        $("tr.body").each(function (index) {
            var $AddSelector = $("tr.body").eq(index).children().children().children(".cbxCompID");
            $AddSelector.addClass("icon_Add");
            $AddSelector.removeClass("icon_NotAdd");
            //$AddSelector.next().removeClass("hide");
            $(".cbxCompID").attr("checked", "checked");
            $(".cbxCompID").attr("value", "true");
            Obj.attr("checked", "checked");
            Obj.attr("value", "true");
        });
    } else {
        $("tr.body").each(function (index) {
            var $AddSelector = $("tr.body").eq(index).children().children().children(".cbxCompID");
            $AddSelector.addClass("icon_NotAdd");
            $AddSelector.removeClass("icon_Add");
            //$AddSelector.next().addClass("hide");
            $(".cbxCompID").removeAttr("checked");
            $(".cbxCompID").attr("value", "false");
            Obj.removeAttr("checked");
            Obj.attr("value", "false");
        });
    }
}

function AddCompany() {
    var i = 0;
    $("tr.body").each(function (index) {   
        var $Selector = $("tr.body").eq(index).children().children().children(".cbxCompID");
        if ($Selector.hasClass("icon_Add") == true) {
            var id = $Selector.attr("id");
            AddCompanyList(id);
            //window.location = "#ShowProductName";
            i++;
        } else {
            var id = $Selector.attr("id");
            $ListCompTable.find("[id=t" + id + "]").remove();

            //Check No Company
            if ($ListCompTable.find("tr").length == 1) {
                $ListCompTable.find("tbody").append(NoCompany);
                $("#ShowProductName").text("");
                $("#hidProductName").val("");
                $("#hidCountCompLead").val("");
                $("#hidAllSupplierCompID").val("");
                $("#hidAttachEmailCompID").val("");
            }

            var count = ManageSequenceNumber();
            $("#hidCountCompLead").val(count);

        }
    });
    if (i == 0) {
        bootbox.alert(label.vldselect_company);
    }
}

function ManageSequenceNumber() {
    var $Selector = $("#ListCompany").find("tbody").find("tr");
    var compid = new Array();
    var getCompid = "";
    var i = 0;
    $Selector.each(function (index) {

        $Selector.eq(index).children(".RowNumber").text(index + 1);

        getCompid = $Selector.eq(index).attr("id");
        if (getCompid != "NoCompanyRow" && getCompid != null && getCompid != "") {
            compid[i] = getCompid.substring(1, getCompid.lenght);
            i++;
        }
    });
    $("#hidAllSupplierCompID").val(compid);
    GetIsAttachUserData(compid)

    var count = $("#ListCompany").find("tbody").find("tr:last-child").children("td:first-child").text();
    return count;
}

function GetIsAttachUserData(ArrayCompID) {
    var ArrayAttachStatus = new Array();
    // check for attach User's data
    if (ArrayCompID.length > 0) {
        var id = 0;
        var j = 0;
        for (var i = 0; i < ArrayCompID.length; i++) {
            if ($("#" + ArrayCompID[i]).next().hasClass("icon_SendEmail") == true) {
                    ArrayAttachStatus[j] = ArrayCompID[i];
                    j++;
            }
        }
        $("#hidAttachEmailCompID").val(ArrayAttachStatus);
    }
}
/////////////////////////////////////////////////////////////////
//------------------- Customer Profile ---------------------//
function hasB2bAccount(Obj) {
    if (Obj.attr("value") == true || Obj.attr("checked") == "checked") {
        $(".B2bAccountFiled").removeAttr("disabled");
        $("#txtUsernames").attr("placeholder", "Search by Username");
        $(".btnSearchAcc").removeClass("hide");
        $("#SearchOption").removeClass("hide");
        $("#SearchOption1").attr("checked", "checked");
        $("#SearchByUsername").attr("onkeyup", "autoCompleteUsername()");
    } else {
        $(".B2bAccountFiled").attr("disabled", "disabled");
        $(".B2bAccountFiled").removeAttr("placeholder");
        $(".B2bAccountFiled").val("");
        $(".btnSearchAcc").addClass("hide");
        $("#SearchOption").addClass("hide");
        $("input[name='SearchBy']").removeAttr("checked");

        clearForm('#CustomerProfileForm');
        if ($("#dllProvince").children().length < 2) {
            ListProvince(0, "dllProvince");
        }
        if ($("#dllDistrict").children().length < 2) {
            ListDistrict(0, "dllDistrict");
        }

    }
}
function autoCompleteUsername() {
    $('#txtUsernames').typeahead({
        source: function (query, process) {
            $.post(GetUrl('MyB2B/Purchase/GetUsername'), { query: query }, function (data) {
                return process(data);
            });
        }
    });
}

function autoCompleteCompcode() {
    $('#txtCompCode').typeahead({
        source: function (query, process) {
            $.post(GetUrl('MyB2B/Purchase/GetCompCode'), { query: query }, function (data) {
                return process(data);
            });
        }
    });
}

function autoCompleteCompname() {
    $('#txtCompName').typeahead({
        source: function (query, process) {
            $.post(GetUrl('MyB2B/Purchase/GetCompName'), { query: query }, function (data) {
                return process(data);
            });
        }
    });
}

// select option for autocomplete
function changeSearchBy(val) {

    if (val == 1) {
        $("#SearchByUsername").removeClass("hide");
        $("#SearchByCompCode").addClass("hide");
        $("#SearchByCompName").addClass("hide");
        $("#SearchByUsername").attr("onkeyup", "autoCompleteUsername()");
        $("#txtUsernames").attr("placeholder", "Search by Username");
    } else if (val == 2) {
        $("#SearchByCompCode").removeClass("hide");
        $("#SearchByUsername").addClass("hide");
        $("#SearchByCompName").addClass("hide");
        $("#SearchByCompCode").attr("onkeyup", "autoCompleteCompcode()");
        $("#txtCompCode").attr("placeholder", "Search by CompCode");
    } else if (val == 3) {
        $("#SearchByCompName").removeClass("hide");
        $("#SearchByUsername").addClass("hide");
        $("#SearchByCompCode").addClass("hide");
        $("#SearchByCompName").attr("onkeyup", "autoCompleteCompname()");
        $("#txtCompName").attr("placeholder", "Search by CompName");
    }

}

// search company info
function SearchCompInfo(Obj) {
    Obj.attr("data-loading-text","Loading...");
    
        var searchby = $("#SearchBy").val();
        var seturl = "";
        var text = ""

        if (searchby == 1) {
            seturl = "MyB2B/Purchase/SearchCompByUsername";
            text = $("#txtUsernames").val();
        } else if (searchby == 2) {
            seturl = "MyB2B/Purchase/SearchCompByCompCode";
            text = $("#txtCompCode").val();
        } else if (searchby == 3) {
            seturl = "MyB2B/Purchase/SearchCompByCompName";
            text = $("#txtCompName").val();
        }

        if (text != "") {
            $.ajax({
                url: GetUrl(seturl),
                type: "POST",
                data: { txtSearch: text },
                dataType: "JSON",
                success: function (results) {
                    $("#inputCompName").val(results["CompName"]);
                    $("#hidCompID").val(results["CompID"]);
                    $("#hidCompCode").val(results["CompCode"]);
                    $("#inputAddress").val(results["CompAddr"]);
                    $("#inputContactName").val(results["ContactName"]);
                    $("#inputContactEmail").val(results["CompEmail"]);
                    $("#inputContactTel").val(results["CompPhone"]);

                    if (results["CompProvinceID"] != null && results["CompProvinceID"] != "") {
                        GetProvince(results["CompProvinceID"], "dllProvince");
                    } else {
                        ListProvince(0, "dllProvince");
                    }

                    if (results["CompDistrictID"] != null && results["CompDistrictID"] != "") {
                        GetDistrict(results["CompDistrictID"], "dllDistrict");
                    } else {
                        ListDistrict(0, "dllDistrict");
                    }

                    copyText();

                },
                error: function () {
                    bootbox.alert(label.vldnot_found_company);
                }
            });
        } else {
            bootbox.alert(label.vldenter_search); 
        }


    }
//
function copyText() {
    var checkEmail = $("#inputContactEmail").val();
    if (checkEmail != null && checkEmail != "") {
        $("#chkSendEmail").attr("checked", "checked");
        $("#txtSendEmail").removeAttr("disabled");
        $("#txtSendEmail").val(checkEmail);
    }
}


//--------ChangeProvince-------//
$("#dllProvince").change(function () {
    GetDistrictByProvince($(this).val(), 0, "dllDistrict");
});

//--------Reset Form-------//
function ManualClearForm() {

    if ($("#dllProvince").children().length < 2) {
        ListProvince(0, "dllProvince");
    }
    if ($("#dllDistrict").children().length < 2) {
        ListDistrict(0, "dllDistrict");
    }

    clearForm('#CustomerProfileForm');
    clearForm('#leadOptionForm');
    clearForm('#RegisterMemberForm');
    clearForm('#leadForm');
    clearValidation();

    $(".RegisterMember").addClass('hide');
    $(".btnSearchAcc").addClass("hide");
    $("#SearchOption").addClass("hide");
    $(".B2bAccountFiled").removeAttr("placeholder");

    $(".B2bAccountFiled").attr("disabled", "disabled");
    $("#txtSendEmail").attr("disabled", "disabled");
    $("#txtSetDefaultLead").attr("disabled", "disabled");

    
    $ListCompTable.find("tbody").children().remove();

    //Check No Company
    if ($ListCompTable.find("tr").length == 1) {
        $ListCompTable.find("tbody").append(NoCompany);
        $("#ShowProductName").text("");
        $("#hidProductName").val("");
        $("#hidCountCompLead").val("");
        $("#hidAllSupplierCompID").val("");
        $("#hidAttachEmailCompID").val("");
    }
    $("#ShowProductName").text("-");
    $(".cbxCompID").removeAttr("checked");
    $(".cbxCompID").attr("value", "false");

}

function clearValidation() {
    $('.control-group').removeClass('error');
    $('.control-group').removeClass('success');
    $("label.error").remove();
    $(".myvalidate").text("");
}

/////////////////////////////////////////////////////////////////
//------------------- Import EmLead ---------------------//
function selectDefaultLead(id) {

    if ($("#" + id + "").attr("value") == true || $("#" + id + "").attr("checked") == "checked") {

        $(".cbxEmLeadID").removeAttr("checked");
        $(".cbxEmLeadID").attr("value", "false");
        $("#" + id + "").attr("checked", "checked");
        $("#" + id + "").attr("value", "true");
    }

}
function ImportEmLead() {
    var emLeadID = 0;
    $(".cbxEmLeadID").each(function (index) {
        if ($(".cbxEmLeadID").eq(index).attr("value") == true || $(".cbxEmLeadID").eq(index).attr("checked") == "checked") {
            emLeadID = $(".cbxEmLeadID").eq(index).attr("id");
        }
    });
    if (emLeadID > 0) {
        var keyword = $("#" + emLeadID).parent().next().children("#emLeadKeyword").text();
        $("#hidEmLeadID").val(emLeadID);
        $("#TextSearch").val(keyword);
        $("#form0").submit();
        g_no = 0;
        $('#myModal').modal('hide');

    }
}

function editEmail(Obj) {
    Obj.parent().addClass("hide");
    Obj.parent().next().removeClass("hide");
}
function closeEditEmail(Obj) {
    Obj.parent().addClass("hide");
    Obj.parent().prev().removeClass("hide");
}
function saveNewEmail(Obj, id) {
    var ContactEmail = Obj.prev().val()
    Obj.parent().prev().children(".displayContactEmail").text(ContactEmail);

    $.ajax({
        url: GetUrl("MyB2B/Purchase/UpdateContactEmail"),
        data: { CompID: id, Email: ContactEmail },
        success: function () {
            bootbox.alert(label.vldsave_success);
        }, error: function () {
            bootbox.alert(label.vldsave_unsuccess);
        }
    });


    closeEditEmail(Obj);
}

/////////////////////////////////////////////////////////////////
//------------------- Manage List ---------------------//
function ChangeStatus(id, is, type) {
    OpenLoading(true);
    $.ajax({
        url: GetUrl("MyB2B/Purchase/ChangeStatus"),
        data: { id: id, status: is, type: type },
        type: "POST",
        datatype: "json",
        traditional: true,
        success: function () {
            OpenLoading(false);
            $("#form0").submit();
        }
    });
}

/*------------------------------DelData-------------------------------------*/
function DelData(id, RowVersion) {
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
        RowVersion[0] = RowVersion;
        Check[0] = "True";
    }
    OpenLoading(true);
    var urlType = "";
    if ($("#hidPageType").val() == "myAssignLead") {
        urlType = "MyB2B/Purchase/DeleteAssignLead";
    } else {
        urlType = "MyB2B/Purchase/DeleteEmLead";
    }

    if (ID.length > 0) {
        $.ajax({
            url: GetUrl(urlType),
            data: { Check: Check, ID: ID, RowVersion: RowVersion},
            success: function (data) {
                OpenLoading(false);
                if (data.Result) {

                    $("#CountAssignLead").text("(" + data.CountAssignLead + ")");
                    $("#CountEmLead").text("(" + data.CountEmLead + ")");
                   
                    $(g_hidsubmit).eq(g_no).click();
                    
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

function CheckBoxall(Obj) {
    if (Obj.attr("value") == true || Obj.attr("checked") == "checked") {
        $(".cbxItem").attr("checked", "checked");
        $(".cbxItem").attr("value", "true");
        Obj.attr("checked", "checked");
        Obj.attr("value", "true");

    } else {
        $(".cbxItem").removeAttr("checked");
        $(".cbxItem").attr("value", "false");
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

function CheckCompanyList() {
    $("#saveCreateEmLead").submit(function () {
        if ($("#ListCompany").find("[id = NoCompanyRow]").length > 0) {
            $(".myvalidate").text("Please import company to lead.");
            return false;
        } else {
            $(".myvalidate").text("");
            return true;
        }
    });
}
