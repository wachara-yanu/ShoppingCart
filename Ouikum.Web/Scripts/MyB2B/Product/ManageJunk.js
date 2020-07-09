
$(".btn-del-all").live("click", function () {
    if(confirm(label.vldconfirm_del_all)){
        DeleteAllProduct();
    }
});

$(".btn-restore-all").live("click", function () {
    if(confirm(label.vldconfirm_restore_all)){
        RestoreAllProduct();
    }
});

 
$(".del-product").live("click", function () {
    var index = $(".del-product").index($(this));
    var id = $('.product-item').eq(index).attr('data-id');
    if(confirm(label.vldconfirm_del)){
        DeleteProduct(id,index);
    }
});

$(".restore-product").live("click", function () {
    var index = $(".restore-product").index($(this));
    var id = $('.product-item').eq(index).attr('data-id');
    if(confirm(label.vldconfirm_restore)){
        RestoreProduct(id,index);
    }
});

function RemoveDivProductJunk(index) {
    if (index >= 0) {
        $('.product-item').eq(index).fadeOut();
        $('.product-item').eq(index).remove();
    }
}

function DeleteAllProduct() {
    var lstID = Array();
    var count = $('.product-item').length;
    for (var i = 0; i < count; i++) {
        lstID[i] = $('.product-item').eq(i).attr('data-id');
    }
    console.log(count + ' , ' + lstID);
    DeleteProduct(lstID);

}

function RestoreAllProduct() {
    var lstID = Array();
    var count = $('.product-item').length;
    for (var i = 0; i < count; i++) {
        lstID[i] = $('.product-item').eq(i).attr('data-id');
    }
    console.log(count + ' , ' + lstID);
    RestoreProduct(lstID);
}
