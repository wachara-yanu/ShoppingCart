$(function () {

    $("#OpenAdvanceSearch").click(function () {
        $("#simple").slideUp();
        $("#advance").slideDown();
    });

    $("#OpenSimpleSearch").click(function () {
        $("#advance").slideUp();
        $("#simple").slideDown();
    });
    /*----------------------enter search companyshiment-------------------------*/
    $('.OrderCode').keypress(function (e) {
        if (e.which == 13) {
            $(".btnsearch").submit();
        }
    });

});


//--------------dll date-------------------
function DateChange(Obj) {
    if (Obj.val() == "1") {
        $("#Datepicker").show();
    } else {
        $("#fromDate").val('');
        $("#toDate").val('');
        $("#Datepicker").hide();
    }

}

//----------------close popover------------------
function ClosePopover() {
    $("#btnBrowseOther").popover('hide');
    $("#NewOption").popover('hide');
}

//----------------Reset Form------------------
function resetForm($form) {
    $form.find('input:text, input:password, input:file, select, textarea').val('');
    $form.find('input:checkbox')
                 .removeAttr('checked').removeAttr('selected');
    // Delete validate
    $('label.error').remove();
    $('.control-group').removeClass('error');
    $('.control-group').removeClass('success');
}