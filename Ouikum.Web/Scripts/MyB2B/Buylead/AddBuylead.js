//---------------------------Text Editor-----------------------------------
var focusshow = false;
var focushide = false;
var IsShowTextEditorBuyleadDetail = false;



$(".Buylead-detail-click").click(function () {
    if (!IsShowTextEditorBuyleadDetail) {
        $(this).text(label.vldhide_moredetail);
        IsShowTextEditorBuyleadDetail = true;
        $('.Buylead-detail-textarea').fadeIn();
    } else {
        $(this).text(label.vldadd_moredetail);
        IsShowTextEditorBuyleadDetail = false;
        $('.Buylead-detail-textarea').fadeOut();
    }

});

$('#Qty').click(function () {
    $(this).select();
});



function CheckFullDetail() {
    var detail = tinyMCE.activeEditor.getContent();

    if (detail == "") {
        $("#FullDetail").closest('.control-group').removeClass('success');
        $("#FullDetail").closest('.control-group').addClass('error');
        $("#FullDetail .errorTxt").text(label.vldrequired);
        return false;
        //    $("#FullDetail .successImg").css('display', 'none');
    } else if (detail.length > 10000) {
        $("#FullDetail").closest('.control-group').removeClass('success');
        $("#FullDetail").closest('.control-group').addClass('error');
        $("#FullDetail .errorTxt").text(label.vldformaterror);
        return false;
    }
    else {
        $("#FullDetail").closest('.control-group').addClass('success');
        $("#FullDetail").closest('.control-group').removeClass('error');
        $("#FullDetail .errorTxt").text('');
        return true;
        //   $("#FullDetail .successImg").css('display', 'block');
    }
}



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
         //   $("#AllCate .successCate").show();

        } else {
            $("#AllCate").closest('.control-group').removeClass('success');
            $("#AllCate").closest('.control-group').addClass('error');
            $("#AllCate .errCate").show()
       //     $("#AllCate .successCate").hide();
        }
    }
}


//-----------Confirm Save and Cancel------------------------------

function SaveBuylead() {

        GetUrl("Search/Buylead/List");
        data = {
            BuyleadName: $("#BuyleadName").val(),
            BuyleadCode: $("#BuyleadCode").val(),
            BuyleadType: $('input[name=optionsType]:checked').val(),
            BuyleadExpire: $("#dp3").val(),
            Keyword: BuyleadKeyword,
            FullDetail: tinyMCE.activeEditor.getContent(),
            Qty: $("#Qty").val(),
            QtyUnit: $('#QtyUnit option:selected').val(),
            Catecode: $(".div-sel-category select option:selected").attr("catecode"),
            CateLV3: $(".div-sel-category select option:selected").val(),
            BuyleadImgPath:BuyleadImgPath
        }
//        console.log(data);

        OpenLoading(true);
       
        $.ajax({
            url: GetUrl("MyB2B/Buylead/AddBuylead"),
            data: data,
            traditional: true,
            success: function (data) {
                CheckError(data); 
                OpenLoading(false);
                $('#ModalAddBuylead').modal('hide');
                SubmitPage(1);
            },
            error: function () {
            },
            type: "POST"
        });
    }


         //-----------GetValueTagit--------------------------------------
         var BuyleadKeyword = "";
          
         //============ Get Buylead Img ===================//
         function GetImgPath() {
             BuyleadImgPath = $('#BuyleadImgPath_0').attr('img-name'); 
         }
         function CheckImgPath() {
             GetImgPath();
             if (BuyleadImgPath == "" || BuyleadImgPath==undefined || BuyleadImgPath==null) {
                 $("#BuyleadImgPath").removeClass('success');
                 $("#BuyleadImgPath").addClass('error');
                 $(".BuyleadImgPath > .error").text(label.vldminimg_1);
                 $(".BuyleadImgPath .error").css('display', 'block');
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

         function GetValueTagit() {
             BuyleadKeyword = ""; 
             $('#tagit-keyword').find("li").each(function (index) {
                 var kw = index - 1;
                 BuyleadKeyword += $(this).text().replace("x", "~");
             });
             BuyleadKeyword = BuyleadKeyword.substring(0, BuyleadKeyword.length - 1)
         }

         function CheckBuyleadCode() {
             if (($('#BuyleadCode').val() == "") || ($('#BuyleadCode').val().length < 2)) {
                 $("#BuyleadCode").closest('.control-group').removeClass('success');
                 $("#BuyleadCode").closest('.control-group').addClass('error');
              //   $(".BuyleadCode > .success").css('display', 'none');

                 if ($('#BuyleadCode').val() == "") {
                     $(".BuyleadCode > .error").text(label.vldrequired);
                 }
                 else if ($('#BuyleadCode').val().length < 2) {
                     $(".BuyleadCode > .error").text(label.vldless_2char);
                 }
                 return false;
             }
             else {
                 $("#BuyleadCode").closest('.control-group').addClass('success');
                 $("#BuyleadCode").closest('.control-group').removeClass('error');
                 $(".BuyleadCode > .error").text('');
            //     $(".BuyleadCode > .success").css('display', 'block');
                 return true;
             }

         }


         function CheckBuyleadName() {
             var name = $.trim($('#BuyleadName').val());
             if (name.length > 2) {
        $("#BuyleadName").closest('.control-group').addClass('success');
        $("#BuyleadName").closest('.control-group').removeClass('error');
        //$(".BuyleadName > .success").show();
        $(".BuyleadName > .error").text('');
        return true;
    }
    else {
        $("#BuyleadName").closest('.control-group').removeClass('success');
        $("#BuyleadName").closest('.control-group').addClass('error');
       // $(".BuyleadName > .success").css('display', 'none');
        $(".BuyleadName > .error").text(label.vldrequired);
    }
    return false;
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


function CheckAddBuylead() {
             var bool = true;
             GetValueTagit();
             GetImgPath();   

            

             if (!CheckBuyleadName()) {
                 bool = false;
             }
//             console.log('name : ' + bool);

//             if (!CheckImgPath()) {
//                 bool = false;
//             }
//             console.log('img : ' + bool);

             if (!CheckBuyleadCode()) {
                 bool = false;
             }
//             console.log('code : ' + bool);
             if (!CheckBuyleadExpire()) {
                 bool = false;
             }
             if (!CheckBuyleadKeyword()) {
                 bool = false;
             }
//             console.log('keyword : ' + bool);


             if (!CheckFullDetail()) {
                 bool = false;
             }
//             console.log('FullDetail : ' + bool);

            

             if ($('#QtyUnit option:selected').val() == 0) {
                 $("#Qty").closest('.control-group').removeClass('success');
                 $("#Qty").closest('.control-group').addClass('error');
                 bool = false;
             }
             else {
                 $("#Qty").closest('.control-group').addClass('success');
                 $("#Qty").closest('.control-group').removeClass('error');
             }

             if ($('#CategoryCode').val().length > 1) { 
                 $("#Category").closest('.control-group').addClass('success');
                 $("#Category").closest('.control-group').removeClass('error');
               //  $("#Category .successImg").css('display', 'block');
                 $(".div-sel-category").css('display', 'none');

             }
             else {
                 $("#Category").closest('.control-group').removeClass('success');
                 $("#Category").closest('.control-group').addClass('error');
                 $(".errorCate").text(label.vldrequired);
                 bool = false;

             }


             if (!bool) {
                 bootbox.alert(label.vldall_required);
                 return false;
             }
             else {
                 SaveBuylead();
             }
         }

         $("#TextSearchCategory").keypress(function (event) {
             if (event.which == 13) {
                 SearchCategory();
             }
         });

         function CheckBuyleadKeyword() {
             var list = BuyleadKeyword.split('~'); 
             if (BuyleadKeyword.length > 0) {

                 if (list.length > 5) {
                     $("#tagit-keyword").closest('.control-group').removeClass('success');
                     $("#tagit-keyword").closest('.control-group').addClass('error');
                     $(".Keyword > .error").text(label.vldmax_word5);
            //         $(".Keyword > .success").css('display', 'none');
                     $("#tagit-keyword").css('border', 'solid 1px #B94A48');
                     return false;
                 } else {
                     $("#tagit-keyword").closest('.control-group').addClass('success');
                     $("#tagit-keyword").closest('.control-group').removeClass('error');
                     $(".Keyword > .error").text('');
            //         $(".Keyword > .success").css('display', 'block');
                     $("#tagit-keyword").css('border', 'solid 1px #468847');
                     return true;
                 }
                  
             } else {
                 $("#tagit-keyword").closest('.control-group').removeClass('success');
                 $("#tagit-keyword").closest('.control-group').addClass('error');
                 $(".Keyword > .error").text(label.vldrequired);
            //     $(".Keyword > .success").css('display', 'none');
                 $("#tagit-keyword").css('border', 'solid 1px #B94A48');
                 return false;
             }
         }


         function SearchCategory() {
             $("#TextSearchCategory").removeClass("mar_t1");
             $(".div-sel-category").fadeIn();
             var txtsrc = $('.txtSearchCate').val();
             if (txtsrc.length > 0) {
                 SearchCategoryByName(txtsrc);
                 $(".errorCate").text('');
             } else {

              //   $("#AllCate .successCate").hide();
                 $("#Category").closest('.control-group').removeClass('success');
                 $("#Category").closest('.control-group').addClass('error');
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
                 

         function CheckExistBuyleadName() {
         //don't use
             $.ajax({
                 url: GetUrl("MyB2B/Buylead/ValidateAddBuylead"),
                 data: { BuyleadName: $('#BuyleadName').val() },
                 type: "POST",
                 success: function (data) {
                     if (($('#BuyleadName').val() == "") || ($('#BuyleadName').val().length < 4) || (!data)) {
                         $("#BuyleadName").closest('.control-group').removeClass('success');
                         $("#BuyleadName").closest('.control-group').addClass('error');
                     //    $(".BuyleadName > .success").css('display', 'none');
                         if ($('#BuyleadName').val() == "") {
                             $(".BuyleadName > .error").text(label.vldrequired);
                         }
                         else if ($('#BuyleadName').val().length < 4) {
                             $(".BuyleadName > .error").text(label.vldless_4char);
                         }
                         else if (!data) {
                             $(".BuyleadName > .error").text(label.vldpname_already);
                         }
                     }
                     else {
                         $("#BuyleadName").closest('.control-group').addClass('success');
                         $("#BuyleadName").closest('.control-group').removeClass('error');
                         $(".BuyleadName > .error").text('');
                    //     $(".BuyleadName > .success").css('display','block');
                     }
                 },
                 error: function () {
                     bootbox.alert(label.vldcannot_check_info);
                 }
             });
         }

         function CheckExistBuyleadCode() {
             $.ajax({
                 url: GetUrl("MyB2B/Buylead/ValidateAddBuylead"),
                 data: { BuyleadCode: $('#BuyleadCode').val() },
                 type: "POST",
                 success: function (data) {
                     if (!data) {
                         $("#BuyleadCode").closest('.control-group').removeClass('success');
                         $("#BuyleadCode").closest('.control-group').addClass('error');
                    //     $(".BuyleadCode > .success").css('display', 'none');
                         $(".BuyleadCode > .error").text(label.vldpname_already);
                         return false;
                     } else {
                         $("#BuyleadCode").closest('.control-group').addClass('success');
                         $("#BuyleadCode").closest('.control-group').removeClass('error');
                         $(".BuyleadCode > .error").text('');
                  //       $(".BuyleadCode > .success").css('display', 'block');
                         return true;
                     }  

                 },
                 error: function () {
                     bootbox.alert(label.vldcannot_check_info);
                 }
             });
         }
         function CheckCompareExpire() {
             $.ajax({
                 url: GetUrl("MyB2B/Buylead/ValidateAddBuylead"),
                 data: { BuyleadExpire: $('#dp3').val() },
                 type: "POST",
                 success: function (data) {
                     if (!data) {
                         $("#dp3").closest('.control-group').removeClass('success');
                         $("#dp3").closest('.control-group').addClass('error');
                         $("#dp3").removeClass("mar_t1");
                         $(".BuyleadExpire > .error").text(label.vldenddate_buy);
                         return false;
                     } else {
                         $("#dp3").closest('.control-group').addClass('success');
                         $("#dp3").closest('.control-group').removeClass('error');
                         $("#dp3").removeClass("mar_t1");
                         $(".BuyleadExpire > .error").text('');
                         return true;
                     }

                 },
                 error: function () {
                     bootbox.alert(label.vldcannot_check_info);
                 }
             });
         }

         //----เก็บสคริป .blur-----------------

         $("#tagit-keyword").live('blur', function () {
             GetValueTagit();
             CheckBuyleadKeyword();
         });

         $("#AllQty").live('blur', function () {
             $(this).val(parseFloat(eval($(this).val())).toFixed(2));
             if ($('#QtyUnit option:selected').val() == 0) {
                 $("#AllQty").closest('.control-group').removeClass('success');
                 $("#AllQty").closest('.control-group').addClass('error');
               //  $("#AllQty .errorQty").css('display', 'block');
               ///  $("#AllQty .successQty").css('display', 'none');
             }
             else {
                 $("#AllQty").closest('.control-group').addClass('success');
                 $("#AllQty").closest('.control-group').removeClass('error');
              //   $("#AllQty .errorQty").css('display', 'none');
             //    $("#AllQty .successQty").css('display', 'block');
             }
         });

         $("#TextSearchCategory").live('blur', function () { 
             CheckCateCode();
         });


          $(function () {
              $("em").remove();
              $(".t-upload-button > span").remove();
              /*-----------------------ให้พิมพ์ราคาได้เฉพาะตัวเลขเท่านั้น-----------------------*/
             
              $('#Qty').bind('keypress', function (e) {
                  return (e.which != 8 && e.which != 46 && e.which != 0 && (e.which < 48 || e.which > 57)) ? false : true;
              })
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
              //             bootbox.alert('ยกเลิกการเพิ่มสินค้า');
          });
