var focusshow = false;
var focushide = false;
var IsShowTextEditorBuyleadDetail = false;

$('#ModalEditBuylead').on('hidden', function () {
    $('.Bg_Allitem').css("position", "static");
})


$(".upd-detail-click").click(function () {
    if (!IsShowTextEditorBuyleadDetail) {
        $(this).text(label.vldhide_moredetail);
        IsShowTextEditorBuyleadDetail = true;
        $('.upd-detail-textarea').fadeIn();
    } else {
        $(this).text(label.vldadd_moredetail);
        IsShowTextEditorBuyleadDetail = false;
        $('.upd-detail-textarea').fadeOut();
    }

});

$('#Upd_Qty').click(function () {
    $(this).select();
});


$(".mceFirst").live("click", function () {
    focushide = true;
});

//========================= Category ====================//

function CheckCateCode() {
    var str = $('#CategoryCode').val();
    if (str != undefined) {
        if (str.length > 1) {
            $("#AllCate").closest('.control-group').removeClass('error');
            $("#AllCate").closest('.control-group').addClass('success');
            $("#AllCate .errCate").hide()
           // $("#AllCate .successCate").show();

        } else {
            $("#AllCate").closest('.control-group').removeClass('success');
            $("#AllCate").closest('.control-group').addClass('error');
            $("#AllCate .errCate").show()
          //  $("#AllCate .successCate").hide();
        }
    }
}



function CheckFullDetail() {
    var detail = tinyMCE.activeEditor.getContent();

    if (detail == "") {
        $("#Upd_FullDetail").closest('.control-group').removeClass('success');
        $("#Upd_FullDetail").closest('.control-group').addClass('error');
        $("#Upd_FullDetail .errorTxt").text(label.vldrequired);
        return false;
        //    $("#Upd_FullDetail .successImg").css('display', 'none');
    } else if (detail.length > 10000) {
        $("#Upd_FullDetail").closest('.control-group').removeClass('success');
        $("#Upd_FullDetail").closest('.control-group').addClass('error');
        $("#Upd_FullDetail .errorTxt").text(label.vldformaterror);
        return false;
    }
    else {
        $("#Upd_FullDetail").closest('.control-group').addClass('success');
        $("#Upd_FullDetail").closest('.control-group').removeClass('error');
        $("#Upd_FullDetail .errorTxt").text('');
        return true;
        //   $("#Upd_FullDetail .successImg").css('display', 'block');
    }
}




//-----------Confirm Save and Cancel------------------------------
function EditBuylead() {
    data = {
        BuyleadID: $("#Upd_BuyleadID").val(),
        BuyleadName: $("#Upd_BuyleadName").val(),
        BuyleadCode: $("#Upd_BuyleadCode").val(),
        BuyleadType: $('input[name=optionsType]:checked').val(),
        BuyleadExpire: $("#dp3").val(),
        Keyword: EditKeyword,
        FullDetail: tinyMCE.activeEditor.getContent(),
        Qty: $("#Upd_Qty").val(),
        QtyUnit: $('#Upd_QtyUnit option:selected').val(),
        Catecode: EditCatecode,
        CateLV3: parseInt($("#CategoryCode").attr("data-id"), 10),
        BuyleadImgOldfile: BuyleadImgOldfile,
        BuyleadImgPath: BuyleadImgPath
    }
//    console.log(data); 
    OpenLoading(true);
    $.ajax({
        url: GetUrl("MyB2B/Buylead/EditBuyleadByID"),
        data: data,
        traditional: true,
        success: function (data) {
            OpenLoading(false);
            $('#ModalEditBuylead').modal('hide');
            if (CheckError(data)) {
                SubmitPage(1);
            }
        },
        error: function () {
            OpenLoading(false);
        },
        type: "POST"
    });
}
//-----------GetValueTagit--------------------------------------
var EditKeyword = "";
var EditDetail = "";

function CheckBuyleadKeyword() {
    GetEditTagit();
    var list = EditKeyword.split('~');
//    console.log('keyword');
//    console.log(list);
    if (EditKeyword.length > 0) {

        if (list.length > 5) {
            $("#tagit-editkeyword").closest('.control-group').removeClass('success');
            $("#tagit-editkeyword").closest('.control-group').addClass('error');
            $(".Upd_Keyword > .error").text(label.vldmax_word5);
        //    $(".Upd_Keyword > .success").css('display', 'none');
            $("#tagit-editkeyword").css('border', 'solid 1px #B94A48');
            return false;
        } else {
            $("#tagit-editkeyword").closest('.control-group').addClass('success');
            $("#tagit-editkeyword").closest('.control-group').removeClass('error');
            $(".Upd_Keyword > .error").text('');
        //    $(".Upd_Keyword > .success").css('display', 'block');
            $("#tagit-editkeyword").css('border', 'solid 1px #468847');
            return true;
        }

    } else {
        $("#tagit-editkeyword").closest('.control-group').removeClass('success');
        $("#tagit-editkeyword").closest('.control-group').addClass('error');
        $(".Upd_Keyword > .error").text(label.vldrequired);
       // $(".Upd_Keyword > .success").css('display', 'none');
        $("#tagit-editkeyword").css('border', 'solid 1px #B94A48');
        return false;
    }
}


function GetEditTagit() {
    EditKeyword = "";
    $('#tagit-editkeyword').find("li").each(function (index) {
        var kw = index - 1;
        EditKeyword += $(this).text().replace("x", "~");
    });
    EditKeyword = EditKeyword.substring(0, EditKeyword.length - 1)
}

var EditCatecode = "";

var BuyleadImgPath = "";

//============ Get Buylead Img ===================//
function GetImgPath() {
        BuyleadImgOldfile = $('#BuyleadImgOldfile').val();
        BuyleadImgPath= $('#BuyleadImgPath_0').attr('img-name');
}

function CheckImgPath() {
    GetImgPath();
    if (BuyleadImgPath == "" || BuyleadImgPath == undefined || BuyleadImgPath == null) {
        $("#BuyleadImgPath").removeClass('success');
        $("#BuyleadImgPath").addClass('error');
        $(".BuyleadImgPath > .error").text(label.vldminimg_1);
        $(".BuyleadImgPath .error").css('display', 'block');
       // $(".BuyleadImgPath .success").css('display', 'none');
        return false;
    }
    else {
        $("#BuyleadImgPath").addClass('success');
        $("#BuyleadImgPath").removeClass('error');
        $(".BuyleadImgPath .error").css('display', 'none');
     //   $(".BuyleadImgPath .success").css('display', 'block');
        return true;
    }
}

function CheckBuyleadCode() {
    if (($('#Upd_BuyleadCode').val().length < 2)) {
        $("#Upd_BuyleadCode").closest('.control-group').removeClass('success');
        $("#Upd_BuyleadCode").closest('.control-group').addClass('error');
      //  $(".Upd_BuyleadCode > .success").css('display', 'none');

        if ($('#Upd_BuyleadCode').val() == "") {
            $(".Upd_BuyleadCode > .error").text(label.vldrequired);
        }
        else if ($('#Upd_BuyleadCode').val().length < 2) {
            $(".Upd_BuyleadCode > .error").text(label.vldless_2char);
        }
        return false;
    }
    else {
        $("#Upd_BuyleadCode").closest('.control-group').addClass('success');
        $("#Upd_BuyleadCode").closest('.control-group').removeClass('error');
        $(".Upd_BuyleadCode > .error").text('');
      //  $(".Upd_BuyleadCode > .success").css('display', 'block');
        return true;
    }

}
function CheckCompareExpire() {
    $.ajax({
        url: GetUrl("MyB2B/Buylead/ValidateEditBuylead"),
        data: { BuyleadExpire: $('#dp3').val() },
        type: "POST",
        success: function (data) {
            if (!data) {
                $("#dp3").closest('.control-group').removeClass('success');
                $("#dp3").closest('.control-group').addClass('error');
                $("#dp3").removeClass("mar_t1");
                $(".Upd_BuyleadExpire > .error").text('วันที่สิ้นสุดประกาศซื้อต้องไม่น้อยกว่าวันที่ปัจจุบัน');
                return false;
            } else {
                $("#dp3").closest('.control-group').addClass('success');
                $("#dp3").closest('.control-group').removeClass('error');
                $("#dp3").removeClass("mar_t1");
                $(".Upd_BuyleadExpire > .error").text('');
                //       $(".BuyleadCode > .success").css('display', 'block');
                return true;
            }

        },
        error: function () {
            bootbox.alert(label.vldcannot_check_info);
        }
    });
}

function CheckBuyleadExpire() {
    if ($('#dp3').val() == "") {
        $("#dp3").closest('.control-group').removeClass('success');
        $("#dp3").closest('.control-group').addClass('error');
        $(".BuyleadExpire > .error").text(label.vldrequired);
        return false;
    }
    else {
        $("#dp3").closest('.control-group').addClass('success');
        $("#dp3").closest('.control-group').removeClass('error');
        return true;
    }
}
//============ CheckEditBuylead===================//


function CheckEditBuylead() {
    var bool = true; 
    if ($(".div-sel-category select option:selected").attr("catecode") != undefined) {
        EditCatecode = $(".div-sel-category select option:selected").attr("catecode");
    }
    else {
        EditCatecode = $(".show-catepath-list").attr("cate-code");
    }
    if (!CheckBuyleadName()) {
        bool = false;
    } 

//    console.log('name : ' + bool);

//    if (!CheckImgPath()) {
//        bool = false;
//    }
//    console.log('img : ' + bool);

    if (!CheckBuyleadCode()) {
        bool = false;
    }
//    console.log('code : ' + bool);
    if (!CheckBuyleadExpire()) {
        bool = false;
    }

    if (!CheckBuyleadKeyword()) { 
        bool = false;
    }
//    console.log('keyword : ' + bool);

    if (!CheckFullDetail()) {
        bool = false;
    }
//    console.log('FullDetail : ' + bool); 

    if ($('#Upd_QtyUnit option:selected').val() == 0) {
        $("#Upd_Qty").closest('.control-group').removeClass('success');
        $("#Upd_Qty").closest('.control-group').addClass('error');
        bool = false;
    }
    else {
        $("#Upd_Qty").closest('.control-group').addClass('success');
        $("#Upd_Qty").closest('.control-group').removeClass('error');
    }

//    console.log($('#CategoryCode').val());
    if ($('#CategoryCode').val() == undefined || $('#CategoryCode').val() == '') {
        $("#Upd_Category").closest('.control-group').removeClass('success');
        $("#Upd_Category").closest('.control-group').addClass('error');
        $(".errorCate").text(label.vldrequired);
        bool = false;
    }
    else {
        $("#Upd_Category").closest('.control-group').addClass('success');
        $("#Upd_Category").closest('.control-group').removeClass('error');
        $(".div-sel-category").css('display', 'none');
    } 

  //  $(".Upd_BuyleadGroup .success").css('display', 'block');
    if (!bool) {
        bootbox.alert(label.vldall_required);
        return false;
    }
    else {
        EditBuylead();
    }
}

$("#TextSearchCategory").keypress(function (event) {
    if (event.which == 13) {
        SearchCategory();
    }
});

function CheckBuyleadName() {
    if ($('#Upd_BuyleadName').val().length >= 3) {
        $("#Upd_BuyleadName").closest('.control-group').addClass('success');
        $("#Upd_BuyleadName").closest('.control-group').removeClass('error');
      //  $(".Upd_BuyleadName > .success").show();
        $(".Upd_BuyleadName > .error").text('');
        return true;
    }
    else {
        $("#Upd_BuyleadName").closest('.control-group').removeClass('success');
        $("#Upd_BuyleadName").closest('.control-group').addClass('error');
      //  $(".Upd_BuyleadName > .success").css('display', 'none');
        $(".Upd_BuyleadName > .error").text(label.vldrequired);
    }
    return false;
}

function CheckExistBuyleadName() {
// don't use
    data = {
        Upd_BuyleadName: $('#Upd_BuyleadName').val(),
        Chk_BuyleadName: $('#Upd_BuyleadName').attr('chkname'),
        BuyleadID: parseInt($('#Upd_BuyleadID').val(), 10)
    }
//    console.log(data);
    $.ajax({
        url: GetUrl("MyB2B/Buylead/ValidateEditBuylead"),
        data: data,
        type: "POST",
        success: function (data) {
            if (!data) {
                $("#Upd_BuyleadName").closest('.control-group').removeClass('success');
                $("#Upd_BuyleadName").closest('.control-group').addClass('error');
              //  $(".Upd_BuyleadName > .success").css('display', 'none');
                $(".Upd_BuyleadName > .error").text(label.vldpname_already);
            }
            else {
                $("#Upd_BuyleadName").closest('.control-group').addClass('success');
                $("#Upd_BuyleadName").closest('.control-group').removeClass('error');
              //  $(".Upd_BuyleadName > .success").show();
                $(".Upd_BuyleadName > .error").text('');
            }
        },
        error: function () {
            bootbox.alert(label.vldcannot_check_info);
        }
    });
}


function CheckExistBuyleadCode() {
    // don't use
    data = { 
        Upd_BuyleadCode : $('#Upd_BuyleadCode').val(),
        Chk_BuyleadCode : $('#Upd_BuyleadCode').attr('chkcode'),
        BuyleadID : parseInt($('#Upd_BuyleadID').val(), 10)
    }
//    console.log(data); 
    $.ajax({
        url: GetUrl("MyB2B/Buylead/ValidateEditBuylead"),
        data: data
         ,
        type: "POST",
        success: function (data) {
//            console.log('exist : '+data);
            if (!data) {
                $("#Upd_BuyleadCode").closest('.control-group').removeClass('success');
                $("#Upd_BuyleadCode").closest('.control-group').addClass('error');
               // $(".Upd_BuyleadCode > .success").css('display', 'none');
                $(".Upd_BuyleadCode > .error").text(label.vldpname_already);
                return false;
            } else {
                $("#Upd_BuyleadCode").closest('.control-group').addClass('success');
                $("#Upd_BuyleadCode").closest('.control-group').removeClass('error');
                $(".Upd_BuyleadCode > .error").text('');
                //$(".Upd_BuyleadCode > .success").css('display', 'block');
                return true;
            }
        },
        error: function () {
            bootbox.alert(label.vldcannot_check_info);
        }
    });
}


$("#TextSearchCategoryEdit").keypress(function (event) {
    if (event.which == 13) {  
        SearchCategory();
    }
});
function SearchCategory() {
    $(".div-sel-category").fadeIn();
    var txtsrc = $('.txtSearchCate').val(); 
    if (txtsrc.length > 0) {
        SearchCategoryByName(txtsrc);
        $(".errorCate").text('');

    } else {
        $("#AllCate .successCate").hide();
        $("#Upd_Category").closest('.control-group').removeClass('success');
        $("#Upd_Category").closest('.control-group').addClass('error');
        $(".errorCate").text(label.vldrequired);
        $(".div-sel-category").fadeOut();
    }
    return false;
}

function SearchCategoryByName(txtsearch) {
    data = {
        CategoryName: txtsearch
    }
    $('.Bg_searchcate').hide();
    $('.show-catepath').text('Loading..');
    $('.show-catepath').fadeIn();
    $.ajax({
        url: GetUrl("MyB2B/Buylead/SearchCategory"),
        data: data,
        success: function (data) {

            $('.show-catepath').hide();
            $(".div-sel-category").html(data); 
        },
        error: function () {
            OpenLoading(false);
            bootbox.alert("Error");
        },
        type: "POST"
    });
}

$("#tagit-editkeyword").live('blur', function () {
         CheckBuyleadKeyword();
     });

         $("#Upd_AllQty").live('blur', function () {
             $(this).val(parseFloat(eval($(this).val())).toFixed(2));
             if ($('#Upd_QtyUnit option:selected').val() == 0) {
                 $("#Upd_AllQty").closest('.control-group').removeClass('success');
                 $("#Upd_AllQty").closest('.control-group').addClass('error');
                 //$("#Upd_AllQty .errorQty").css('display', 'block');
                 //$("#Upd_AllQty .successQty").css('display', 'none');
             }
             else {
                 $("#Upd_AllQty").closest('.control-group').addClass('success');
                 $("#Upd_AllQty").closest('.control-group').removeClass('error');
                 //$("#Upd_AllQty .errorQty").css('display', 'none');
                 //$("#Upd_AllQty .successQty").css('display', 'block');
             }
         });





$(function () {
//    $("em").remove();
    $("em").remove();
    $(".t-upload-button > span").remove();
   
    //--------------------พิมพ์ได้เฉพาะตัวเลขเท่านั้น--------------------------------------------------------------------
  
    $('#Upd_Qty').bind('keypress', function (e) {
        return (e.which != 8 && e.which != 46 && e.which != 0 && (e.which < 48 || e.which > 57)) ? false : true;
    });
    /*-----------------------text numunical--------------------------------*/
    $(".icon_up").live('click', function () {
        var input = $(this).parent().parent().find("input[type=text]:eq(0)");

        if (input.val() == "") {
            input.val(1)
        } else {
            input.val(parseFloat(eval(input.val()) + 1).toFixed(2))
        }
    });
    $(".icon_down").live('click', function () {
        var input = $(this).parent().parent().find("input[type=text]:eq(0)");
        if (input.val() == "") {
            //bootbox.alert('ไม่สามารถเลือกให้น้อยกว่า 0 ได้');
        } else {
            if (parseFloat(input.val()) > 0) {
                input.val(parseFloat(eval(input.val()) - 1).toFixed(2))
            } else {
                //bootbox.alert('ไม่สามารถเลือกให้น้อยกว่า 0 ได้');
            }
        }
    });
    //#region---------------------------Text editor---------------------------------------------
    tinyMCE.init({
        // General options
        mode: "textareas",
        theme: "advanced",
        height: "250",
        width: "100%",
        plugins: "autolink,lists,spellchecker,pagebreak,style,layer,table,save,advhr,advimage,advlink,emotions,iespell,inlinepopups,insertdatetime,preview,media,searchreplace,print,contextmenu,paste,directionality,fullscreen,noneditable,visualchars,nonbreaking,xhtmlxtras,template",

        // Theme options
        theme_advanced_buttons1: "bold,italic,underline,strikethrough,|,justifyleft,justifycenter,justifyright,justifyfull,|,formatselect,fontselect,fontsizeselect",
        theme_advanced_buttons2: "cut,copy,paste,pastetext,pasteword,|,search,|,bullist,numlist,|,outdent,indent,|,undo,redo,|,link,unlink,image,cleanup,code,|,preview,|,forecolor,backcolor",
        theme_advanced_buttons3: "tablecontrols,|,hr,removeformat,visualaid,|,sub,sup,|,iespell,media,|,fullscreen",
        theme_advanced_toolbar_location: "top",
        theme_advanced_toolbar_align: "left",
        theme_advanced_statusbar_location: "bottom",
        theme_advanced_resizing: false
    });
    //#endregion
});
