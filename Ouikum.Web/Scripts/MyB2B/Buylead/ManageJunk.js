
$(".btn-del-all").live("click", function () {
    if(confirm(label.vldconfirm_del_all)){
        DeleteAllBuylead();
    }
});

$(".btn-restore-all").live("click", function () {
    if(confirm(label.vldconfirm_restore_all)){
        RestoreAllBuylead();
    }
});

 
$(".del-Buylead").live("click", function () {
    var index = $(".del-Buylead").index($(this));
    var id = $('.Buylead-item').eq(index).attr('data-id');
    if(confirm(label.vldconfirm_del)){
        DeleteBuylead(id,index);
    }
});

$(".restore-Buylead").live("click", function () {
    var index = $(".restore-Buylead").index($(this));
    var id = $('.Buylead-item').eq(index).attr('data-id');
    if(confirm(label.vldconfirm_restore)){
        RestoreBuylead(id,index);
    }
});
function RemoveDivBuyleadJunk(index) {
    if (index >= 0) {
        $('.Buylead-item').eq(index).fadeOut();
        $('.Buylead-item').eq(index).remove();
    }
}
function DeleteAllBuylead() {
    var lstID = Array();
    var count = $('.Buylead-item').length;
    for (var i = 0; i < count; i++) {
        lstID[i] = $('.Buylead-item').eq(i).attr('data-id');
    }
//    console.log(count + ' , ' + lstID);
    DeleteBuylead(lstID);

}
function RestoreAllBuylead() {
    var lstID = Array();
    var count = $('.Buylead-item').length;
    for (var i = 0; i < count; i++) {
        lstID[i] = $('.Buylead-item').eq(i).attr('data-id');
    }
//    console.log(count + ' , ' + lstID);
    RestoreBuylead(lstID);
}
