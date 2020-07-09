$(function () {
    $.validator.addMethod("selectDistrict", function (value, element, arg) {
        return arg != value;
    });
    $.validator.addMethod("selectProvince", function (value, element, arg) {
        return arg != value;
    });
    $.validator.addMethod("selectBiztype", function (value, element, arg) {
        return arg != value;
    });
    $.validator.addMethod("selectBank", function (value, element, arg) {
        return arg != value;
    });
    $.validator.addMethod("selectAcctype", function (value, element, arg) {
        return arg != value;
    });
    $.validator.addMethod("checked", function (value, element, arg) {
        return arg != value;
    });
});
