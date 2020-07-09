$(function () {

    $('#frmMailContactUs').validate(
             {
                 rules: {
                     Name: {
                         required: true
                     },
                     Email: {
                         required: true,
                         email: true
                     },
                     Subject: {
                         required: true
                     },
                     Detail: {
                         required: true
                     }

                 },
                 messages: {
                     Name: {
                         required: label.vldrequired
                     },
                     Email: {
                         required: label.vldrequired,
                         email: label.vldfix_format_email
                     },
                     Subject: {
                         required: label.vldrequired,
                     },
                     Detail: {
                         required: label.vldrequired
                     }
                 },
                 highlight: function (label) {
                     $(label).closest('.control-group').addClass('error');
                     OpenLoading(false);
                 },
                 success: function (label) {
                     label.closest('.control-group').removeClass('error');

                 }
             });

    $('#submitContactUs').click(function () {
        if ($("#frmMailContactUs").valid() == true) {
            OnsendMailContact($("#frmMailContactUs"));
            return false;
        } else {
            return false;
        }
    });
}); 

function OnsendMailContact(obj) {
    OpenLoadingNew(true, $("body"));
             var Name = obj.find('#Name').val();
             var CompName = obj.find('#CompName').val();
             var Email = obj.find('#Email').val();
             var Phone = obj.find('#Phone').val();
             var Subject = obj.find('#Subject').val();
             var Detail = obj.find('#Detail').val(); 

             data = {
                 Name: Name,
                 CompName: CompName,
                 Email: Email,
                 Phone: Phone,
                 Subject: Subject,
                 Detail: Detail
             };
             //console.log(data);
             $.ajax({
                 url: GetUrl("Default/ContactUs"),
                 data: data,
                 type: "POST",
                 dataType: 'json',
                 success: function (data) {
                     OpenLoadingNew(false);
                     bootbox.alert(label.vldsend_success);
                     //$('#Name').val("");
                     //$('#CompName').val("");
                     //$('#Email').val("");
                     //$('#Phone').val("");
                     $('#Subject').val("");
                     $('#Detail').val("");
                 },
                 error: function () {
                 }
             });
             return false;

         }
function OpenLoadingNew(isLoad, obj) {
    var mar_t = ($(window).height() / 2);
    var mar_l = ($(window).width() / 2) - 110;
    if (isLoad == true) {
        obj.prepend('<div id="loading"><div id="imgloading"><img src=\"' + GetUrl('Content/Default/images/icon-load.gif') + '\" ></div></div>')
        $("#loading").css({ 'filter': 'alpha(opacity=50)', 'opacity': '0.5' });
        $("#imgloading").css("margin", mar_t + "px 0 0 " + mar_l + "px")
    }
    else { $('#loading').remove(); $('#imgloading').remove() }
}