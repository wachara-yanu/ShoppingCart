
/*------------------------------------Validate -------------------------------------*/
$.validator.addMethod("NumberOnly", function (value, element) {
    var i = /^[0-9]+$/;
    return this.optional(element) || (i.test(value) > 0);
});

$.validator.addMethod("EngOnly", function (value, element) {
    var i = /^[0-9A-Za-z@._-]+$/;
    return this.optional(element) || (i.test(value) > 0);
});

$.validator.addMethod("ThaiOnly", function (value, element) {
    var i = /^[\u0E01-\u0E5B]+$/;
    return this.optional(element) || (i.test(value) > 0);
});

$.validator.addMethod("EmailOnly", function (value, element) {
    var i = /^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;
    return this.optional(element) || (i.test(value) > 0);
});

$.validator.addMethod("Zipcode", function (value, element) {
    var i = /^\d{5}$/;
    return this.optional(element) || (i.test(value) > 0);
});




$("#CompAcc_form").validate({
    rules: {
        DisplayName: "required",
        CompName: {
            ThaiOnly: true
        },
        CompNameEng: {
            EngOnly: true
        },
        CompAddrLine1: "required",
        CompProvinceID: "required",
        CompDistrictID: "required",
        CompPostalCode: {
            Zipcode: true
        },
        CompPhone: "required",
        BizTypeOther: "required"
    },
    messages: {
        DisplayName: "กรุณากรอกข้อมูล",
        CompName: "กรุณากรอกชื่อบริษัทของคุณเป็นภาษาไทย",
        CompAddrLine1: "กรุณากรอกข้อมูล",
        CompProvinceID: "กรุณาเลือกจังหวัด",
        CompPostalCode: "กรุณากรอกข้อมูล",
        CompPhone: "กรุณากรอกข้อมูล",
        BizTypeOther: "กรุณาระบุประเภทธุรกิจ"
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