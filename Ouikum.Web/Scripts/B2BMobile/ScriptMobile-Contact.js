$(".icon_quo_up").on('click', function () {
    console.log("up")
    var input = $(this).parent().parent().find("#Qty");
    console.log(input.val());
    if (input.val() == "") {
        input.val(1)
    } else {
        input.val(parseFloat(eval(input.val()) + 1).toFixed(2))
    }
});
$(".icon_quo_down").on('click', function () {
    console.log("down")
    var input = $(this).parent().parent().find("#Qty");
    if (input.val() == "") {
        alert(label.vldcannot_insert_zero);
    } else {
        if (parseFloat(input.val()) > 1) {
            input.val(parseFloat(eval(input.val()) - 1).toFixed(2))
        }
    }
});

$(".item").eq(0).addClass('active');

$(document).ready(function () {
    ResizeScrollbar();

    window.onresize = function (event) {
        ResizeScrollbar();
    };

    function ResizeScrollbar() {
        var height = $(".nav-submenu").height();
        if (height > 40) {
            $(".scroll-contact").css("top", "110px")
        } else {
            $(".scroll-contact").css("top", "91px")
        }
    }
});

$(document).ready(function () {
    // Phone
    var CompPhone = $("#CompPhone").val();
    var CompMobile = $("#CompMobile").val();
    var Phone = CompPhone.replace(" ", "/", ",");
    var Mobile = CompMobile.replace(" ", "/", ",");

    $("#linkPhone").attr("href", "tel:" + Phone);
    $("#linkMobile").attr("href", "tel:" + Mobile);
});

$("#Qty").blur(function () {
    var Qty = $(this).val();
    if (Qty <= 0) {
        $('#myModalAlert').modal('show');
        $('.ModalMsg').html("กรุณาเลือกปริมาณมากกว่า 1 ขึ้นไป");
        $('.ModalOK').click(function () {
            $('#myModalAlert').modal('hide');
            $("#Qty").val("1.00");
        })
    }
});

// Script Quotation //

function formatNum(num) {
    var p = num.toFixed(2).split(".");
    return p[0].split("").reverse().reduce(function (acc, num, i, orig) {
        return num + (i && !(i % 3) ? "," : "") + acc;
    }, "");
}

$("#DisCount").attr("disabled", true);
$("#VAT").attr("disabled", true);

$("#RequestPrice").blur(function () {
    $("#DisCount").val("");
    $("#SumPrice2").html(0);
    $("#VAT").val("");
    $("#SumPrice3").html(0);

    if ($(this).val() != null && $(this).val() != "") {
        var Qty = parseInt($("#QuoQty").val());
        var RequestPrice = parseInt($(this).val());
        var Sum = Qty * RequestPrice;
        $("#SumPrice1").html(formatNum(Sum));
        $("#hidSumPrice1").val(Sum);
        $("#DisCount").attr("disabled", false);
        $("#VAT").attr("disabled", false);
    } else {
        $("#SumPrice1").html(0);
        $("#DisCount").attr("disabled", true);
        $("#DisCount").val("");
        $("#DisCount").closest('.form-group').removeClass('has-error');
        $("#DisCount").closest('.form-group').removeClass('has-success');
        $("label[for='DisCount']").remove();

        $("#SumPrice2").html(0);
        $("#VAT").attr("disabled", true);
        $("#VAT").val("");
        $("#VAT").closest('.form-group').removeClass('has-error');
        $("#VAT").closest('.form-group').removeClass('has-success');
        $("label[for='VAT']").remove();
    }
});

$("#DisCount").blur(function () {
    $("#VAT").val("");
    $("#SumPrice3").html(0);
    var SumPrice = parseInt($("#hidSumPrice1").val());
    if ($(this).val() != null && $(this).val() != "") {
        var DisCount = parseInt($(this).val());
        var Sum = SumPrice - DisCount;
        $("#SumPrice2").html(formatNum(Sum));
        $("#hidSumPrice2").val(Sum);
    } else {
        $("#SumPrice2").html(0);
        $("#hidSumPrice2").val(SumPrice);
    }
});

$("#VAT").blur(function () {
    var SumPrice = parseInt($("#hidSumPrice2").val());
    if ($(this).val() != null && $(this).val() != "") {
        var Vat = parseInt($(this).val());
        var Sum = (SumPrice * Vat / 100) + SumPrice;
        $("#SumPrice3").html(formatNum(Sum));
    } else {
        $("#SumPrice3").html(0);
    }
});

//$("#btn-Quotation").click(function () {
//    var sum = $("#hidSumPrice1").val();
//    var Dis = $("#DisCount").val();
//    var Vat = $("#VAT").val();
//    sum = sum == "" ? 0 : parseInt(sum);
//    Dis = Dis == "" ? 0 : parseInt(Dis);
//    Vat = Vat == "" ? 0 : parseInt(Vat);
//});

$("#close1").click(function () {
    var isDisabled = $("#DisCount").attr('disabled');
    if (isDisabled) {
        $("#DisCount").attr("disabled", false);
        $("#DisCount").val("");
        $("#DisCount").focus();
    } else {
        $("#DisCount").attr("disabled", true);
        $("#DisCount").val("");
        $("#DisCount").closest('.form-group').removeClass('has-error');
        $("#DisCount").closest('.form-group').removeClass('has-success');
        $("label[for='DisCount']").remove();
        $("#SumPrice2").html($("#hidSumPrice1").val());
        $("#hidSumPrice2").val($("#hidSumPrice1").val());
        $("#VAT").val("");
        $("#VAT").closest('.form-group').removeClass('has-success');
        $("#SumPrice3").html(0);
    }
});
$("#close2").click(function () {
    var isDisabled = $("#VAT").attr('disabled');
    if (isDisabled) {
        $("#VAT").attr("disabled", false);
        $("#VAT").val("");
        $("#VAT").focus();
    } else {
        $("#VAT").attr("disabled", true);
        $("#VAT").val("");
        $("#VAT").closest('.form-group').removeClass('has-error');
        $("#VAT").closest('.form-group').removeClass('has-success');
        $("label[for='VAT']").remove();
        $("#SumPrice3").html($("#hidSumPrice2").val());
    }
});

$('.btn-cancelQuo').click(function () {
    $(".TargetSlide").hide('slide', { direction: 'down' });
    $("#container-search").hide('slide', { direction: 'down' });
    $("#DisCount").attr("disabled", true);
    $("#VAT").attr("disabled", true);
    $("#SumPrice1").html("0");
    $("#SumPrice2").html("0");
    $("#SumPrice3").html("0");
    $('.form-group').removeClass('has-success');
    $('.form-group').removeClass('has-error');
    $("label").remove();
});

$("#RequestPrice").blur(function () {
    var cash = $(this).val();
    if ($(this).val() != "") {
        if (cash <= 0) {
            $('#myModalAlert').modal('show');
            $('.ModalMsg').html("กรุณากรอกราคาสินค้า");
            $('.ModalOK').click(function () {
                $("#RequestPrice").val("");
                $("#VAT").attr("disabled", true);
                $("#DisCount").attr("disabled", true);
                $('.form-group').removeClass('has-success');
            })
        }
    }
});
$("#DisCount").blur(function () {
    var cash = $(this).val();
    if ($(this).val() != "") {
        if (cash <= 0) {
            $('#myModalAlert').modal('show');
            $('.ModalMsg').html("กรุณากรอกราคาสินค้า");
            $('.ModalOK').click(function () {
                $("#DisCount").val("");
                $("#VAT").attr("disabled", true);
                $("#DisCount").attr("disabled", true);
                $('.form-group').removeClass('has-success');
            })
        }
    }
});
$("#VAT").blur(function () {
    var cash = $(this).val();
    if ($(this).val() != "") {
        if (cash <= 0) {
            $('#myModalAlert').modal('show');
            $('.ModalMsg').html("กรุณากรอกราคาสินค้า");
            $('.ModalOK').click(function () {
                $("#VAT").val("");
                $("#VAT").attr("disabled", true);
                $("#DisCount").attr("disabled", true);
                $('.form-group').removeClass('has-success');
            })
        }
    }
});