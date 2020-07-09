$(function () {

    $("#add").click(function () {
        $("h3#head_form").html("ADD Order");
        $('#objState').val(1);
        $("#Add_Edit").slideDown();
    });
    $(".edit").click(function () {
        $("h3#head_form").html("EDIT Order");
        $("#Add_Edit").slideDown();
    });
    $(".close,#cancel").click(function () {
        close();
    });
    /*----------------------Ajax Submit-------------------------*/
    $("#submit").click(function () {
        data = {
            objState: $('#objState').val(),
            RowVersion: $('#RowVersion').val(),
            OrderID: $('#OrderID').val(),
            Price: $('#TotalPrice').val(),
            CreatedDate: $('#CreatedDate').val(),
            OrderStatus: $('#OrderStatus').val(),
        }
        $.ajax({
            url: GetUrl("MyB2B/Order/SaveOrderList"),
            data: data,
            type: "POST",
            traditional: true,
            success: function (data) {
                if (data) {
                    $("#Add_Edit").slideUp();
                    close();
                    $(g_hidsubmit).eq(g_no).click();
                    bootbox.alert(label.vldedit_success);
                } //end if
                else {
                    bootbox.alert(label.vldedit_unsuccess);
                }
            },
            error: function () {
                //bootbox.alert("error : ไม่สามารถตรวจสอบข้อมูลได้");
            }
        });
    });
    /*------------------------------tinyMCE--------------------------------*/

    $('#AddOrderList').modal({
        show: false
    });

    $("ul.toggle > li").each(function (index) {
        if ($(this).attr("id") == 'tab_order') {
            $(this).addClass('active_template');
        }
        else {
            $(this).removeClass('active_template');
        }
    });
});

    $('#dp3').datepicker();
/*-----------------------validateform-----------------------------*/

$('#addOrder_form').validate({
    rules: {
        TotalPrice: {
            required: true,
            number: true
        },
        OrderStatus: {
            required: true,
            minlength: 1
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

/*---------------------------EditShipment---------------------------------*/
function EditOrder(id) {
    $.ajax({
        url: GetUrl("MyB2B/Order/EditOrder"),
        data: { OrderID: id },
        type: "POST",
        datatype: "json",
        traditional: true,
        success: function (data) {
            if (data != null) {
                $('#objState').val(2);
                $('#RowVersion').val(data.RowVersion);
                $('#OrderID').val(data.OrderID);
                $('#TotalPrice').val(data.TotalPrice);
                $('#CreatedDate').val(data.CreatedDate);
                $('#OrderStatus').val(data.OrderStatus);
                $('.control-group').removeClass("success error");
                $("#Add_Edit").slideDown();
            } //end if
            else {
                bootbox.alert(label.vldedit_unsuccess);
            }
        },
        error: function () {
            //bootbox.alert("error : ไม่สามารถตรวจสอบข้อมูลได้");
        }
    });
}
function close() {
    $("#Add_Edit").slideUp();
    $('#RowVersion').val("");
    $('#OrderID').val("");
    $('#TotalPrice').val("");
    $('#CreatedDate').val("");
    $('#OrderStatus').val("");
    $('.control-group').removeClass("success error");
    $(".error").text("");
}