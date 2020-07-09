$(function () {
    $("#add").click(function () {
        $("h3#head_form").html("ADD JOB");
        $('#objState').val(1);
        $(".SearchJobName").val("");
        $("#Add_Edit").slideDown(function () {
                     $("#sidebar").height($("#autoHeight").height());
                     $("#main").height($("#autoHeight").height());
                 });
    });
    $(".close,#Cancel").click(function () {
        close();
    });
    /*----------------------enter searchJob-------------------------*/
//    $('.SearchJob').keypress(function (e) {
//        if (e.which == 13) {
//            $(".btnsearch").submit();
//        }
//    });

    /*----------------------Ajax Submit-------------------------*/
    $("#submit").click(function () {
        if ($("#JobName").val() != "" && $("#JobCateID").val() != 0 && $("#JobType").val() != 0 && tinyMCE.get('JobDescription').getContent() != "" && tinyMCE.get('JobRequired').getContent() != ""
        && $("#ReqEducation").val() != 0 && $("#ReqExp").val() != 0 && $("#ProvinceID").val() != 0 && $("#DistrictID").val() != 0 && $("#RecruitAmount").val() != 0 && $("#WorkAddr").val() != ""
        && $("#JobSalary").val() != "") 
        {
            var LengthJobDescription = tinyMCE.get('JobDescription').getContent().length;
            var LengthJobRequired = tinyMCE.get('JobRequired').getContent().length;
            data = {
            objState: $('#objState').val(),
            RowVersion: $('#RowVersion').val(),
            JobID: $('#JobID').val(),
            JobName: $('#JobName').val(),
            JobCateID: $('#JobCateID').val(),
            JobType: $('#JobType').val(),
            ReqEducation: $('#ReqEducation').val(),
            ReqExp: $('#ReqExp').val(),
            RecruitAmount: $('#RecruitAmount').val(),
            WorkAddr: $('#WorkAddr').val(),
            DistrictID: $('#DistrictID').val(),
            ProvinceID: $('#ProvinceID').val(),
            JobSalary: $('#JobSalary').val(),
            IsAttachResume: $('#IsAttachResume').val(),
            Remark: $('#Remark').val(),
            JobDescription: tinyMCE.get('JobDescription').getContent(),
            JobRequired: tinyMCE.get('JobRequired').getContent()
        }
        if (LengthJobDescription < 500 && LengthJobRequired < 500) {
                OpenLoading(true);
                $.ajax({
            url: GetUrl("Company/SaveJob"),
            data: data,
            type: "POST",
            //datatype: "json",
            traditional: true,
            success: function (data) {
                if (data) {
                OpenLoading(false);
                    $("#Add_Edit").slideUp(function () {
                     $("#sidebar").height($("#autoHeight").height());
                     $("#main").height($("#autoHeight").height());
                 });
                    close();
                    $(g_hidsubmit).eq(g_no).click();
                    if ($('#objState').val() == 1) {
                        if (data) {
                                bootbox.alert(label.vldsave_success);
                            } else {
                                bootbox.alert(label.vldsave_unsuccess);
                            }
                    } else {
                            if (data) {
                                bootbox.alert(label.vldedit_success);
                            } else {
                                bootbox.alert(label.vldEdit_unsuccess);
                            }
                    }

                } //end if
                else {
                    OpenLoading(false);
                        bootbox.alert(label.vldsave_unsuccess);
                }
            },
            error: function () {
                    OpenLoading(false);
               // bootbox.alert(label.vldcannot_check_info);
            }
        });
        } else {
        bootbox.alert('ข้อความรายละเอียดหรือคุณสมบัติที่ต้องการ กรุณาลองใหม่อีกครั้ง');
            }
        }else{
            bootbox.alert(label.vldrequired_complete);
        }
    });
    /*------------------------------------------------------------*/
    $('.btn-tootip-bottom').tooltip({ placement: 'bottom' });
    $('.btn-tootip-top').tooltip({ placement: 'top' });

    tinyMCE.init({
        // General options
        mode: "textareas",
        theme: "advanced",
        height: "200",
        width: "80%",
        plugins: "autolink,lists,spellchecker,pagebreak,style,layer,table,save,advhr,advimage,advlink,emotions,iespell,inlinepopups,insertdatetime,preview,media,searchreplace,print,contextmenu,paste,directionality,fullscreen,noneditable,visualchars,nonbreaking,xhtmlxtras,template",

        // Theme options
        theme_advanced_buttons1: "bold,italic,underline,strikethrough,|,justifyleft,justifycenter,justifyright,justifyfull,|,formatselect,fontselect,fontsizeselect",
        theme_advanced_buttons2: "cut,copy,paste,pastetext,pasteword,|,search,|,bullist,numlist,|,outdent,indent,|,undo,redo,|,link,unlink,image,cleanup,code,|,preview,|,forecolor,backcolor",
        theme_advanced_buttons3: "tablecontrols,|,hr,removeformat,visualaid,|,sub,sup,|,iespell,media,|,fullscreen",
        theme_advanced_toolbar_location: "top",
        theme_advanced_toolbar_align: "left",
        theme_advanced_statusbar_location: "bottom",
        theme_advanced_resizing: false
    });

    $("ul.toggle > li").each(function (index) {
        if ($(this).attr("id") == 'tab_Job') {
            $(this).addClass('active_template');
        }
        else {
            $(this).removeClass('active_template');
        }
    });
});

/*---------------------------EditJob---------------------------------*/
function EditJob(id) {
    $.ajax({
        url: GetUrl("Company/EditJob"),
        data: { JobID: id },
        type: "POST",
        datatype: "json",
        traditional: true,
        success: function (data) {
            if (data != null) {
                $("h3#head_form").html("EDIT JOB");
                $(".SearchJobName").val("");
                $('#objState').val(2);
                $('#RowVersion').val(data.RowVersion);
                $('#JobID').val(data.JobID);
                $('#JobName').val(data.JobName);
                $("#JobCateID option[value=" + data.JobCateID + "]").attr("selected", "selected");
                $("#JobType option[value=" + data.JobType + "]").attr("selected", "selected");
                $('#JobDescription').val(data.JobDescription);
                if (data.JobDescription != null && data.JobDescription != "") {
                    tinyMCE.get('JobDescription').setContent(data.JobDescription);
                } else {
                    tinyMCE.get('JobDescription').setContent("");
                }
                $('#JobRequired').val(data.JobRequired);
                if (data.JobRequired != null && data.JobRequired != "") {
                    tinyMCE.get('JobRequired').setContent(data.JobRequired);
                } else {
                    tinyMCE.get('JobRequired').setContent("");
                }
                $("#ReqEducation option[value=" + data.ReqEducation + "]").attr("selected", "selected");
                $("#ReqExp option[value=" + data.ReqExp + "]").attr("selected", "selected");
                $('#RecruitAmount').val(data.RecruitAmount);
                $('#WorkAddr').val(data.WorkAddr);
                GetDistrictByProvince(data.ProvinceID, data.DistrictID, "DistrictID");
                $("#DistrictID option[value=" + data.DistrictID + "]").attr("selected", "selected");
                $("#ProvinceID option[value=" + data.ProvinceID + "]").attr("selected", "selected");
                $('#JobSalary').val(data.JobSalary);
                $('#IsAttachResume').val(data.IsAttachResume);
                $('#Remark').val(data.Remark);
                $('.control-group').removeClass("success error");
                $("#Add_Edit").slideDown(function () {
                     $("#sidebar").height($("#autoHeight").height());
                     $("#main").height($("#autoHeight").height());
                 });
            } //end if
            else {
                 bootbox.alert(label.vldsave_unsuccess);
            }
        },
        error: function () {
            //bootbox.alert(label.vldcannot_check_info);
        }
    });
}
//#region checkbox

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
        if ($(".checkboxAll").attr("value") == true || $(".checkboxAll").attr("checked") == "checked") {
            $("#" + id + "").attr("checked", "checked");
            $("#" + id + "").attr("value", "true");
        } else {
            $("#" + id + "").attr("checked", "checked");
            $("#" + id + "").attr("value", "true");
        }
    } else { 
        $(".cbxCompID").removeAttr("checked");
        $(".cbxCompID").attr("value", "false");
    }
}
$("#ProvinceID").change(function () {
    GetDistrictByProvince($(this).val(), 0, "DistrictID");
    $("#DistrictID").attr("disabled",false);
});
//#endregion

/*-----------------------validateform-----------------------------*/

$.validator.addMethod("JobDes", function (value, element, arg) {
    var editorContent = tinyMCE.get('JobDescription').getContent();
    return arg != editorContent;

});
$.validator.addMethod("JobReq", function (value, element, arg) {
    var editorContent = tinyMCE.get('JobRequired').getContent();
    return arg != editorContent;

});
$.validator.addMethod("select", function (value, element, arg) {
    return arg != value;
});

$('#addJob_form').validate(
             {
                 rules: {
                     JobName: {
                         required: true,
                         minlength: 4
                     },
                     JobCateID: {
                         select: 0
                     },
                     JobType: {
                         select: 0
                     },
                     JobDescription: {
                         required: true,
                         JobDes: ""
                     },
                     JobRequired: {
                         required: true,
                         JobReq: ""
                     },
                     ReqEducation: {
                         select: 0
                     },
                     ReqExp: {
                         select: 0
                     },
                     RecruitAmount: {
                         required: true
                     },
                     WorkAddr: {
                         required: true
                     },
                     DistrictID: {
                         selecDistrict: 0
                     },
                     ProvinceID: {
                         selectProvince: 0
                     },
                     JobSalary: {
                         required: true
                     }
                 },
                 messages: {
                     JobName: {
                         required: label.vldrequired,
                         minlength: label.vldless_4char
                     },
                     JobCateID: {
                         select: label.vldselectJobCate
                     },
                     JobType: {
                         select: label.vldselectJobtype
                     },
                     JobDescription: {
                         required: label.vldrequired,
                         JobDes: label.vlddetails
                     },
                     JobRequired: {
                         required: label.vldrequired,
                         JobReq: label.vlddetails
                     },
                     ReqEducation: {
                         select: label.vldselectreqeducation
                     },
                     ReqExp: {
                         select: label.vldselectreqexp
                     },
                     RecruitAmount: {
                         required: label.vldrequired
                     },
                     WorkAddr: {
                         required: label.vldrequired
                     },
                     DistrictID: {
                         selectDistrict: label.vldselectdistrict
                     },
                     ProvinceID: {
                         selectProvince: label.vldselectprovince
                     },
                     JobSalary: {
                         required: label.vldrequired
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
             /*------------------------close--------------------------------*/
             function close() {
                 $("#Add_Edit").slideUp(function () {
                     $("#sidebar").height($("#autoHeight").height());
                     $("#main").height($("#autoHeight").height());
                 });
                 $('#RowVersion').val("");
                 $('#JobID').val("");
                 $('#JobName').val("");
                 $("#JobCateID option[value='0']").attr("selected", "selected");
                 $("#JobType option[value='0']").attr("selected", "selected");
                 $('#JobDescription').val("");
                 tinyMCE.get('JobDescription').setContent("");
                 $('#JobRequired').val("");
                 tinyMCE.get('JobRequired').setContent("");
                 $("#ReqEducation option[value='0']").attr("selected", "selected");
                 $("#ReqExp option[value='0']").attr("selected", "selected");
                 $('#RecruitAmount').val("");
                 $('#WorkAddr').val("");
                 $("#DistrictID option[value='0']").attr("selected", "selected");
                 $("#ProvinceID option[value='0']").attr("selected", "selected");
                 $('#JobSalary').val("");
                 $("#IsAttachResume").val("");
                 $("#Remark").val("");
                 $(".SearchJobName").val("");
                 $('.control-group').removeClass("success error");
                 $(".error").text("");
             }