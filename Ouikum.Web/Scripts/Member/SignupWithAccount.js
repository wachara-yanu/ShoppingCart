 $(function () {
 if( $("#ProvinceID").val()!=0){
    GetDistrictByProvince($("#ProvinceID").val(), 0, "DistrictID");
}
/*-----------------------------------ChangeProvince-------------------------------------*/
    $("#ProvinceID").change(function () {
        GetDistrictByProvince($(this).val(), 0, "DistrictID");
    });
        $('#QapTcha').QapTcha({});
        
        $('#myModal').modal({
            show: false
        });
         $('#agree_con').click(function () {
            $("#agree").attr("checked", "checked");
            $("#agree").attr("value", "true");
            $('#myModal').modal('hide');
        });
        //capcha
        //  $('#QapTcha').QapTcha({});
         $('#signup2_form').validate(
             {
                 rules: {
                     DisplayName: {
                         minlength: 4,
                         required: true
                     },
                     CompName: {
                         minlength: 3,
                         maxlength: 50,
                         required: true
                     },
                     AddrLine1: {
                         required: true 
                     },
                      DistrictID: {
                          selectDistrict:0
                     },
                      ProvinceID: {
                         selectProvince:0
                     },
                     PostalCode: {
                         required: true,
                         number: true,
                         minlength: 5,
                         maxlength: 5
                     },
                     Phone: {
                         required: true,
                         minlength: 10
                         
                     },
                     Mobile: {
                         minlength: 10
                     },
                     BizTypeID: {
                         selectBiztype: 0
                     },
                     agree: {
                         required:true
                     }

                 },
                 messages: {
                     DisplayName: {
                         minlength: label.vldless_4char,
                         required: label.vldrequired
                     },
                     CompName: {
                         minlength: label.vldless_4char,
                         required: label.vldrequired
                     },
                     AddrLine1: {
                         required: label.vldrequired
                     },
                      DistrictID: {
                          selectDistrict: label.vldselectdistrict
                     },
                      ProvinceID: {
                         selectProvince: label.vldselectprovince
                     },
                     PostalCode: {
                         required: label.vldrequired,
                         number: label.vldfix_format_number,
                         minlength: label.vldless_5char,
                         maxlength: label.vldmore_5char
                     },
                     Phone: {
                         required: label.vldrequired,
                         minlength: label.vldless_10char,
                         
                     },
                     Mobile: {
                         minlength: label.vldless_10char
                     },
                     BizTypeID: {
                         selectBiztype: label.vldselectbiztype
                     },
                     agree: {
                         required:label.vldrequired
                     }

                 },
                 highlight: function (label) {
                     $(label).closest('.control-group').removeClass('success');
                     $(label).closest('.control-group').addClass('error');

                 },
                 success: function (label) {
                     label.closest('.control-group').removeClass('error');
                     label.closest('.control-group').addClass('success');

                 }
             });
    });