
//---------------- Delete  Product ----------------- //
function DeleteProduct(id, index, data) {
    if (data == null || data == undefined) {
        data = {
            ProductID: id,
            CateLV1: GetCateLV(1, index),
            CateLV2: GetCateLV(2, index),
            CateLV3: GetCateLV(3, index)
        }
    }
    // console.log(data);

    OpenLoading(true);

    $.ajax({
        url: GetUrl("MyB2B/Product/DeleteProduct"),
        data: data,
        traditional: true,
        success: function (data) {
            OpenLoading(false);
            if (CheckError(data)) {
                SubmitPage();
            }
        },
        error: function () {
            OpenLoading(false);
        },
        type: "POST"
    }); 
}

//---------------- Restore Product ----------------- //
function RestoreProduct(id, index,data) {


    if (data == null || data == undefined) {
        data = {
            ProductID: id,
            CateLV1: GetCateLV(1, index),
            CateLV2: GetCateLV(2, index),
            CateLV3: GetCateLV(3, index)
        }
    } 

    $.ajax({
        url: GetUrl("MyB2B/Product/Restore"),
        data: data,
        traditional: true,
        success: function (data) {

            if (CheckError(data)) { 
                SubmitPage();
            }
        },
        error: function () {
            OpenLoading(false);
        },
        type: "POST"
    }); 
}

//#region------------------------ GetCateLV ------------------------

function GetCateLV(id, index) {
    var CateLV = Array();
    if (index >= 0) {
        CateLV = parseInt($('.product-item').eq(index).attr('catelv' + id), 10);
    } else {
        for (var i = 0; i < $('.product-item').length; i++) {
            CateLV[i] = parseInt($('.product-item').eq(i).attr('catelv' + id), 10);
        }
        CateLV = $.unique(CateLV);
    }
    return CateLV;
}

//#region------------------------ GetStatus ------------------------

function GetStatus() {
    $.ajax({
        url: GetUrl("MyB2B/Product/GetStatus"),
        success: function (data) {
            $('#status-recommend').html(data.CountProductRecomm + "/20");
            $('#menu-status-recommend').html(data.CountProductRecomm);
            if ($('#IsMaxProduct').val() == "True") {
                $('#status-product').html(data.CountProduct + "/20");
                $('#menu-status-product').html(data.CountProduct);
            } else {
                $('#status-product').html(data.CountProduct);
                $('#menu-status-product').html(data.CountProduct);
            }
            $('#status-junk').html(data.CountJunk);
            $('#status-Approve').html(data.CountProductApprove);
            $('#status-Wait').html(data.CountProductWait);
            $('#status-Allitem').html(data.CountAllitem);
        },
        error: function () {
        },
        type: "POST"
    });
}

function GetStatusBuylead() {
    $.ajax({
        url: GetUrl("MyB2B/Buylead/GetStatus"),
        success: function (data) {
            $('#status-Buylead').html(data.CountBuylead);
            $('#menu-status-Buylead').html(data.CountBuylead);
            $('#status-junk').html(data.CountJunk);
            $('#status-Allitem').html(data.CountAllitem);
            $('#status-Approve').html(data.CountBuyleadApprove);
            $('#status-Wait').html(data.CountBuyleadWait);
            $('#status-Allitem').html(data.CountBuyleadAllitem);
        },
        error: function () {
        },
        type: "POST"
    });

}

//#region------------------------ Move To Junk ------------------------

//#region------------------------Open or Close Setting Product------------------------
$(".setting-search").live("click", function () {
    if ($(".CloseTabmenu").hasClass("newhide")) {
        $(".Bg_Allitem").css("marginTop", 220);
    }
    else {
        $(".Bg_Allitem").css("marginTop", 220);
    }
});

function SetFirstImage() {
    var len = $('.img-data').length;
     console.log(len);
    if (len > 0) {
        var src = $('.img-data').eq(0).attr('src');
        // console.log(src);
        $('#ProductImgPath_0').attr('src', src);
    } else {
        var src = 'http://www.placehold.it/200x200/EFEFEF/AAAAAA&text=no+image';
        $('#ProductImgPath_0').attr('src', src);
        
    }

}

//#region------------------------ตั้งค่า Popover ในกรณี No-approve------------------------------
$(function () {
    GetStatusBuylead();
    //console.log("ManageProduct1");
    $('.icon_Noapprove').popover({ content: label.vldnoapprove_pro, placement: 'left' });
    $(".layout_center").click(function () {
        $('.icon_Noapprove').popover('hide');
    });
    $("#ContentManageLeft").click(function () {
        $('.icon_Noapprove').popover('hide');
    });
    $(".MyB2BNavBar").click(function () {
        $('.icon_Noapprove').popover('hide');
    });

    //---------------------------Popup Modal-----------------------------------

    $('#ModalAddProduct').on('hide', function () {
        $('#ModalAddProduct').html('');
        $("html").css('overflow', 'auto');
    });
    $('#ModalAddProduct').on('show', function () {

        $('#bodyQuickDetails').fadeOut(300);
    });

    //---------------------------Popup Modal-----------------------------------
    $('#ModalEditProduct').on('hide', function () {
        $('#ModalEditProduct').html('');
        //$('.Bg_Allitem').css("position", "static");
        $("html").css('overflow', 'auto');
    });
    $('.btn-tootip-bottom').tooltip({ placement: 'bottom' });

    //---------------------------bootbox.alert-Confirm---------------------------------
    $(".confirm-editproduct").click(function () {
        confirm('Confirm Edit Product!');
    });
    $("#empty_bin").click(function () {
        confirm('Confirm Empty Recycle Bin!');
    });
    $("#restore_all").click(function () {
        confirm('Confirm Restore all item!');
    });

    //---------------------------ย่อ/ขยาย TabMenu------------------------------------
    $(".CloseTabmenu").click(function () {
        $('.bg_product_mainmenu').hide("slide", { direction: "up" }, 200);
        $('.CloseTabmenu').addClass('newhide');
        if ($('#content').attr('page') == "Recommend") {
            //$(".Bg_Allitem").css("marginTop", 85);
        }
        else {
            //$(".Bg_Allitem").css("marginTop", 130);
        }
        $('.bg_product_hide').removeClass('hide');
        $('#advance_search').css("top", 145);
    });
    $(".ShowTabmenu").click(function () {
        $('.bg_product_mainmenu').show("slide", { direction: "up" }, 200);
        if ($('#content').attr('page') == "Recommend") {
            //$(".Bg_Allitem").css("marginTop", 200);
        }
        else {
            //$(".Bg_Allitem").css("marginTop", 244);
        }
        $('.bg_product_hide').addClass('hide');
        $('#advance_search').css("top", $('.tab_manage_fix').height() + 15);
    });
    //console.log("ManageProduct2");
});

//#endregion
//#region----------------------Sidebar add edit delete item----------------------------

function GroupListNo(obj) {
    var id = new Array();
    var no = new Array();
    id[0] = parseInt(obj.parents('.product-group').attr('group'));
    no[1] = parseInt(obj.parents('.product-group').attr('listno'));
    var action = obj.attr('action');
    if (action == "Up") {
        no[0] = no[1] - 1;
    }
    else if (action == "Down") {
        no[0] = no[1] + 1;
    }
    id[1] = parseInt($('.product-group').eq(no[0] - 1).attr('group'));
    $.ajax({
        url: GetUrl("MyB2B/Product/ChangeListNoGroup"),
        data: {
            id: id,
            no: no
        },
        traditional: true,
        success: function (data) {
            if (CheckError(data)) {
                GroupProductList();
            }
        },
        error: function () {
            OpenLoading(false);
            bootbox.alert("Error");
        },
        type: "POST"
    });
}
//----------------------- Group Product -------------------------------/
function GroupProductList() {
    $.ajax({
        url: GetUrl("MyB2B/Product/GroupProductList"),
        success: function (data) {
            $('#GroupProductList').html(data);
        },
        error: function () {
        },
        type: "POST"
    });

    return false;

}
function EditGroupProduct() {
  
    data = {
        ProductGroupName:   $.trim($('#txtEditGroupName').val()),
        GroupID: parseInt($('#hidEditGroupID').val(),10),
        RowVersion: parseInt($('#hidEditGroupRowversion').val() , 10)
    }

    if (data.ProductGroupName.length < 3) {
        bootbox.alert(label.vldless_3char);
        return false;
    }

    SaveGroupProduct(data);
    $('#txtEditGroupName').val(null);
    OpenFormEditGroup(false);
    return false;
}
function AddGroupProduct() {
    var name = $('#ProductGroupName').val().trim();
    //console.log(name);

    if (name.length < 3) {
        bootbox.alert(label.vldless_3char);
        return false;
    }
    data = {
        ProductGroupName: name
    }

    SaveGroupProduct(data); 
    $('#ProductGroupName').val(null);
    OpenFormAddGroup(false);
    return false;
}
function SaveGroupProduct(data) {

    $.ajax({
        url: GetUrl("MyB2B/Product/SaveGroupProduct"),
        data: data,
        dataType: 'json',
        success: function (data) {
            if (data.IsResult == false) {
                bootbox.alert(label.vldpgroup_already);
            }
            if (CheckError(data)) {
                GroupProductList();
            }
        },
        error: function () {
            OpenLoading(false);
            bootbox.alert("Error");
        },
        type: "POST"
    });

    return false;

}
function DeleteGroup(id, rowversion) {
    data = {
        GroupID: id,
        RowVersion: rowversion
    }

    $.ajax({
        url: GetUrl("MyB2B/Product/DeleteProductGroup"),
        data: data,
        success: function (data) {
            OpenLoading(false);
            if (CheckError(data)) {
                GroupProductList();
            }
        },
        error: function () {
            OpenLoading(false);
        },
        type: "POST"
    });
    return false;
}

function OpenFormAddGroup(isOpen) {
    if (isOpen) {
        $('.hide_group').hide();
        $('.input_edit_group').hide();

        $('.input_group').fadeIn();
        $('#ProductGroupName').focus();
    } else {
        $('.input_group').hide();
        $('.input_edit_group').hide();
        $('.hide_group').fadeIn();
    }
} 
function OpenFormEditGroup(isOpen) {
    if (isOpen) {
        $('.hide_group').hide();
        $('.input_group').hide();

        $('.input_edit_group').fadeIn();
        $('#txtEditGroupName').focus();
    } else {
        $('.input_group').hide();
        $('.input_edit_group').hide();
        $('.hide_group').fadeIn();
    }
}
//#endregion
//=========================== Event ==========================//
//#region----------------------Function ทั่วไป----------------------------
$(function () {
    //เอาเม้าส์ผ่าน เห็น icon แบบถ่วงเวลา
    //console.log("ManageProduct3");
    //

    //เอาเม้าส์ผ่าน เห็น icon แบบปกติ
    $('#manage_group').mousemove(function () {
        $('.edit_group').removeClass("hide");
        $('.del_group').removeClass("hide");
    });


    //คลิกปุ่มบวก หัวข้อหายไป เปลี่ยนเป็น input
    $('.add_group').click(function () {
        OpenFormAddGroup(true);
    });


    // คลิก Save // คลิก Close
    $('.addgroup_already').click(function () {
        $('.input_group').hide();
        $('.hide_group').fadeIn();
    });
    $('.addgroup_close').click(function () {
        OpenFormAddGroup(false);
    });

    $('.editgroup_close').click(function () {
        OpenFormEditGroup(false);
    });


    //--------แสดง ไม่แสดง-------------------

    $('.optionsRadio').live("click", function () {
        var isShow = parseInt($(this).val(), 10);
        SaveIsShow(isShow);

    });
    $('#btn-save-junk').live("click", function () {
        SaveJunk();
    });
    //console.log("ManageProduct4");
});

function SaveIsShow(isShow) {
        data = {
            ProductID: parseInt($('#hidDetailID').val(), 10),
            CateLV1: parseInt($('#hidCateLV1').val(), 10),
            CateLV2: parseInt($('#hidCateLV2').val(), 10),
            CateLV3: parseInt($('#hidCateLV3').val(), 10),
            IsShow: isShow
        }
        // console.log(data);
        $.ajax({
            url: GetUrl("MyB2B/Product/SaveIsShow"),
            data: data,
            traditional: true,
            success: function (data) {
                if (CheckError(data)) {
                    GetStatus();
                    GetStatusBuylead();
                }
            },
            error: function () {
                OpenLoading(false);
            },
            type: "POST"
        });
        return false;
    }
    function ChangeGroup() {
        data = {
            ProductID: parseInt($('#hidDetailID').val(), 10),
            GroupID:$("#QuickProductGroup option:selected").val()
        }
        $.ajax({
            url: GetUrl("MyB2B/Product/ChangeGroup"),
            data: data,
            traditional: true,
            success: function (data) {
                if (CheckError(data)) {
                    GetStatus();
                    GetStatusBuylead();
                }
            },
            error: function () {
                OpenLoading(false);
            },
            type: "POST"
        });
        return false;
    }

function SaveJunk() {
        data = {
            ProductID: parseInt($('#hidDetailID').val(), 10),
            CateLV1: parseInt($('#hidCateLV1').val(), 10),
            CateLV2: parseInt($('#hidCateLV2').val(), 10),
            CateLV3: parseInt($('#hidCateLV3').val(), 10)
        }
        // console.log(data);

        $.ajax({
            url: GetUrl("MyB2B/Product/MoveToJunk"),
            data: data,
            traditional: true,
            success: function (data) {
                if (CheckError(data)) { 
                    CloseQuickDetails();
                    SubmitPage();

                }
            },
            error: function () {
                OpenLoading(false);
            },
            type: "POST"
        });
        return false;
    }
    //--------เปลี่ยนชื่อกลุ่ม-------------------
$(function () {
    //console.log("ManageProduct5");
    $('.edit_group').live("click", function () {
        var index = $(".edit_group").index(this);
        var obj = $('.product-group').eq(index);
        var rowversion = obj.attr('rowversion');
        var groupname = obj.attr('group-name');
        var id = obj.attr('group');
        $('#hidEditGroupID').val(id);
        $('#hidEditGroupRowversion').val(rowversion);
        $('#txtEditGroupName').val(groupname);
        $('#txtEditGroupName').attr('placeholder', groupname);
        OpenFormEditGroup(true);

    });
    //console.log("ManageProduct5.1");
    $('.del_group').live("click", function () {
        var index = $(".del_group").index(this);
        var obj = $('.product-group').eq(index);
        var rowversion = obj.attr('rowversion');
        var groupname = obj.attr('group-name');
        var id = obj.attr('group');
        bootbox.confirm(label.vldconfirm_del, "Cancel", "Yes", function (e) {
            if (e) {
            OpenFormEditGroup(false);
            DeleteGroup(id, rowversion);
            }
        });
    });
    //console.log("ManageProduct5.2");
    //#region---------------------------ตั้งค่าหน้าจอ---------------------------------------------
    //$("body").css("overflow-x", "hidden");
    //var cc = false;
    //var width = $("body").width();
    //var left_w = (0.187 * width);
    //var right_w = (width - left_w);
    //console.log(width);
    //console.log(left_w);
    //console.log(right_w);
    //var left = "#ContentManageLeft";
    //var right = "#ContentManageRight";
    //var tab_w = 0.99 * right_w;
    //var width_s = $(".bg_search_width").width() + $(".btn-search").width() + 50;
    //$(".bg_search_left").width(width_s);
    ////$(".tab_manage_fix").width(tab_w);
    //$(right).css("marginLeft", left_w);
    //---------------------------Scollbar SideBarLeft------------------------------------
    //$("#ContentManageLeft").height($(window).height());
    //$(left).width(left_w);
    //$(right).width(right_w);
    //$(left).height($(window).height());
    //var Scollbar_l = $(window).height() - 65;
    //$(left).height(Scollbar_l);
    //$(left).scrollbars();
    //var set_txtSearch = (width < 1281) ? "span1" : "span2";
    //$("#TextSearch").addClass(set_txtSearch);
    //var set_txtSearch = (width < 1281) ? "100px" : "250px";
    //var set_ProductStatus = (width < 1281) ? "135px" : "200px";
    //var set_DatePeriod = (width < 1281) ? "100px" : "150px";
    //var set_prevpage = (width < 1281) ? "20px" : "40px";
    //var set_nextpage = (width < 1281) ? "20px" : "40px";
    //$("#TextSearch").css('width', set_txtSearch);
    //$("#ddlProductStatus").css('width', set_ProductStatus);
    //$("#ddlFindDatePeriod").css('width', set_DatePeriod);
    //$(".btn-prevpage").css('width', set_prevpage);
    //$(".btn-nextpage").css('width', set_nextpage);
    //console.log("ManageProduct6");
});


//#region---------------------------ย้ายสินค้ารออนุมัติ(มีการแก้ไข)ลงถังขยะ-----------------------------------
    function deleteWaitapproveRemark(Obj) {
        bootbox.confirm(label.vldmovepro_to_trash, "Cancel", "Yes", function (e) {
            if (e) {
                $.ajax({ url: GetUrl("MyB2B/Product/MoveToJunk"),
                    data: { ProductID: Obj.attr("data-id"),
                        RowVersion: Obj.attr("rowversion")
                    },
                    success: function (data) {
                        Obj.parents('.Bg_item').remove();
                        $(".Bg_Allitem").append("<div class='hide'></div>");
                        GetStatus();
                        GetStatusBuylead();
                    },
                    error: function () {
                        bootbox.alert("Error");
                    },
                    type: "POST"
                });
            }
            else {
                return false;
            }
        });
}
//#endregion
//---------------------------------------------------------------------------------------------
function QuickDetailsGetByID(Obj) {
    var height = $(window).height() - 192;
    $("#bodyQuickDetails").css("marginTop", height);
//    OpenLoading(true, null, $('.Bg_Allitem'));
    $.ajax({
        url: GetUrl("MyB2B/Product/ProductGetByID"),
        data: { ID: Obj.attr("data-id") },
        success: function (data) {
           $('#bodyQuickDetails').fadeIn();
           $("#bodyQuickDetails").html(data);
        },
        error: function () {
            bootbox.alert("Error");
        },
        type: "POST"
    });
}




//--------------- Open & Close Product Detail ----------------//
function CloseQuickDetails() {
    $('#bodyQuickDetails').fadeOut(300);
}

 
//----------------Prepare ------------------------------------//
function PrepareAddProduct(Obj) {
    //$('.Bg_Allitem').css("position", "fixed");
    var a = Obj.attr("com-id");
    OpenLoading(true);
    ValidateFullProduct(Obj);
}

function ValidateFullProduct(Obj) {

    $.ajax({
        url: GetUrl("MyB2B/Product/ValidateFullProduct"),
        traditional: true,
        success: function (data) {
            //console.log(data.IsResult);
            if (data.CompLevel != 2) {
                if (data.IsResult) {
                    DoPrepareAdd(Obj)
                } else {
                    OpenLoading(false);
                    var msg = 'Your package has full item. Please can <a href="' + GetUrl("Benefit/Adsrate") + '" target="_blank" >upgrade</a> your package.';
                    bootbox.alert(msg);
                    $("#ModalAddProduct").modal('hide');
                    $("html").css('overflow', 'auto');
                }
            } else {
                OpenLoading(false);
                var msg = 'ไม่สามารถเพิ่มสินค้าได้เนื่องจาก Gold Member ของท่านหมดอายุแล้ว กรุณาต่ออายุการใช้งาน <a href="' + GetUrl("Member/SignUpPackage") + '" target="_blank" >ที่นี่ </a> หรือติดต่อเจ้าหน้าที่ <a href="http://about.ouikum.com/ContactUs?ref=m" target="_blank" >ที่นี่ </a>';
                bootbox.alert(msg);
            }
        },
        error: function () {
            OpenLoading(false);
        },
        type: "POST"
    });
}

function DoPrepareAdd(Obj) {
 
    if ($('#status-product').text()=="20/20") {
        bootbox.alert(label.vldismaxproduct);
        OpenLoading(false);
        return false;
    }
    var ProductCode = GenerateCode(Obj);
    $.ajax({
        url: GetUrl("MyB2B/Product/PrepareAddProduct"),
        data: { GenCode: ProductCode },
        success: function (data) {
            //console.log(data);
            $("#ModalAddProduct").html(data);
            OpenLoading(false);
            $("#ModalAddProduct").modal('show');
            $("html").css('overflow', 'hidden');
        },
        error: function () {
            OpenLoading(false);
            console.log("Do error");
        },
        type: "POST"
    });
}

function PrepareEditByID(Obj) {

    $("#ModalEditProduct").html('');
    OpenLoading(true);
    //$('.Bg_Allitem').css("position", "fixed");
    $("html").css('overflow', 'hidden');
    var ProductCode = GenerateCode(Obj);
    $.ajax({
        url: GetUrl("MyB2B/Product/PrepareEditByID"),
        data: {
            ID: Obj.attr("id"),
            GenCode: ProductCode
        },
        success: function (data) {
            $("#ModalEditProduct").html(data);
            OpenLoading(false);
            $('#ModalEditProduct').on('show', function () {
                $('#bodyQuickDetails').fadeOut();
                resizeImg($(".main-product-img"), 180, 180);
                $(".child-product-img").each(function () {
                    resizeImg($(this), 95, 95);
                });
            });
            
        },
        error: function () {
            OpenLoading(false);
        },
        type: "POST"
    });
}



/*------------------ Gen ProductCode ---------------------*/

function randomCode() {
    var chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXTZ";
    var randomCode = '';
    for (var i = 0; i < 6; i++) {
        var rnum = Math.floor(Math.random() * chars.length);
        randomCode += chars.substring(rnum, rnum + 1);
    }
    return randomCode;
}

function GenerateCode(Obj) {

    var LogonID = Obj.attr("com-id");
    var CompIDCode = "";
    var Randcode = randomCode();
    if (LogonID < 10) {
        CompIDCode += "00000" + LogonID;
    }
    else if (LogonID < 100) {
        CompIDCode += "0000" + LogonID;
    }
    else if (LogonID < 1000) {
        CompIDCode += "000" + LogonID;
    }
    else if (LogonID < 10000) {
        CompIDCode += "00" + LogonID;
    }
    else if (LogonID < 100000) {
        CompIDCode += "0" + LogonID;
    }
    else if (LogonID < 1000000) {
        CompIDCode += LogonID;
    }

    return "PD" + CompIDCode + Randcode;

}



