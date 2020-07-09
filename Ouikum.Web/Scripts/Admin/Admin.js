 
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



$('.btn-tootip-bottom').tooltip({ placement: 'bottom' });
 