$(function () {
    LoadCategory(1, 0);
    LoadCategory1(1, 0);
    LoadCategory2(1, 0);
    LoadCateType();

    $('.menucategory').addClass('hide');
    $('.heightmanage').css('height', '79px');
    $('.heightmanage').css('margin-top', '20px');
    $('.heightmanage').css('margin-bottom', '0px');
    $('#categorytype').css('background-color', '#EAEAEA')

    $('#InsertText').click(function () {
        $('#addtypecate').modal('show')
    });

    $('#InsertType').live('click', function () {
        var name = $('#CategoryType').val().trim();
        InsertCateType(name);
    });

    $('#InsertTextCateLV1').click(function () {
        $('#addcate1').modal('show')
    });

    $('#CancelTextCateLV1').click(function () {
        $('.CateNameEN1').addClass('hide');
        $('#InsertTextCateLV1').removeClass('hide');
    });
    $('#InsertTextENCateLV1').live('click', function () {
        var nameThai = $.trim($('#MCategoryLV1').val());
        var nameEng = $.trim($('#MCategoryENLV1').val());
        
        if (nameThai.length != 0 && nameEng.length != 0) {
            if (nameThai.length >= 4 && nameThai.length <= 100 && nameEng.length >= 4 && nameEng.length <= 100) {
                OpenLoading(true);
                var name = $('#MCategoryLV1').val().trim();
                var nameEN = $('#MCategoryENLV1').val().trim();
                var id = $('#CategoryLV1').attr('data-id');

                if (checkEng(nameEN)) {
                    InsertCate(0, 1, name, nameEN, id);
                } else {
                    OpenLoading(false);
                    bootbox.alert(label.vlddata_incorrect);
                    return false;
                }
            } else {
                OpenLoading(false);
                bootbox.alert('กรุณากรอกข้อมูลอย่างน้อย 4 ตัวอักษรและไม่เกิน 100 ตัวอักษร');
                return false;
            }
        } else {
            OpenLoading(false);
            bootbox.alert('กรุณากรอกข้อมูล');
            return false;
        }
    });

    $('#InsertTextCateLV2').click(function () {
        $('#addcate2').modal('show')
    });

    $('#CancelTextCateLV2').click(function () {
        $('.CateNameEN2').addClass('hide');
        $('#InsertTextCateLV2').removeClass('hide');
    });

    $('#InsertTextENCateLV2').live('click', function () {
        var nameThai = $('#MCategoryLV2').val().trim();
        var nameEng = $('#MCategoryENLV2').val().trim();
        
        if (nameThai.length != 0 && nameEng.length != 0) {
            if (nameThai.length >= 4 && nameThai.length <= 100 && nameEng.length >= 4 && nameEng.length <= 100) {
                OpenLoading(true);

                var id = $('#CategoryLV2').attr('data-id');
                var catetypeid = $('#CategoryLV2').attr('data-cate-id');

                if (checkEng(nameEng)) {
                    InsertCate(id, 2, nameThai, nameEng, catetypeid);
                }
                else {
                    OpenLoading(false);
                    bootbox.alert(label.vlddata_incorrect);
                    return false;
                }
            } else {
                OpenLoading(false);
                bootbox.alert('กรุณากรอกข้อมูลอย่างน้อย 4 ตัวอักษรและไม่เกิน 100 ตัวอักษร');
                return false;
            }
        } else {
            OpenLoading(false);
            bootbox.alert('กรุณากรอกข้อมูล');
            return false;
        }
    });

    $('#InsertTextCateLV3').click(function () {
        $('#addcate3').modal('show')
    });

    $('#CancelTextCateLV3').click(function () {
        $('.CateNameEN3').addClass('hide');
        $('#InsertTextCateLV3').removeClass('hide');
    });

    $('#InsertTextENCateLV3').live('click', function () {
        var nameThai = $('#MCategoryLV3').val().trim();
        var nameEng = $('#MCategoryENLV3').val().trim();
        console.log("nameThai: " + nameThai);
        console.log("nameEng: " + nameEng);
         if (nameThai.length != 0 && nameEng.length != 0) {
             if (nameThai.length >= 4 && nameThai.length <= 100 && nameEng.length >= 4 && nameEng.length <= 100) {
                 
                 OpenLoading(true);
                 var id = $('#CategoryLV3').attr('data-id');
                 var catetypeid = $('#CategoryLV3').attr('data-cate-id');
                 console.log("id: " + id);
                 console.log("catetypeid: " + catetypeid);
                 if (checkEng(nameEng)) {
                     InsertCate(id, 3, nameThai, nameEng, catetypeid);
                 }
                 else {
                     OpenLoading(false);
                     bootbox.alert(label.vlddata_incorrect);
                     return false;
                 }
             } else {
                 OpenLoading(false);
                 bootbox.alert('กรุณากรอกข้อมูลอย่างน้อย 4 ตัวอักษรและไม่เกิน 100 ตัวอักษร');
                 return false;
             }
         } else {
             OpenLoading(false);
             bootbox.alert('กรุณากรอกข้อมูล');
             return false;
         }
    });

    $('#SearchText').click(function () {
        var name = $('#CategoryTypeName').val().trim();
        $('#CategoryLV1').removeAttr('data-id');
        $('#CategoryLV2').removeAttr('data-id');
        $('#CategoryLV3').removeAttr('data-id');
        OpenLoading(true);
        LoadCategoryByType(name);
    });

    $('#SearchTextCateLV1').click(function () {
        var name = $('#CategoryLV1').val().trim();
        var id = $('#CategoryLV1').attr('data-id');
        if (id != null) {
            OpenLoading(true);
            LoadCategoryByType1(id, name);
        }
    });

    $('#SearchTextCateLV2').click(function () {
        var name = $('#CategoryLV2').val().trim();
        var id = $('#CategoryLV2').attr('data-id');
        if (id != null) {
            OpenLoading(true);
            LoadCategoryByType2(id, name);
        }
    });

    $('#SearchTextCateLV3').click(function () {
        var name = $('#CategoryLV3').val().trim();
        var id = $('#CategoryLV3').attr('data-id');
        if (id != null) {
            OpenLoading(true);
            LoadCategoryByType3(id, name);
        }
    });

    $('.category').click(function () {
        $(".category-content").show();
        $(".product-content").hide();
        $(".delete-category-content").hide();

        $('.category').removeAttr('style');
        $('.product').removeAttr('style');
        $('.delete-category').removeAttr('style');
        $('.category').css('font-weight', 'bold');
    });

    $('.category-close').click(function () {
        $(".category-content").hide();

        $('.category').removeAttr('style');
        $('.product').removeAttr('style');
        $('.delete-category').removeAttr('style');
    });

    $('.product').click(function () {
        $(".product-content").show();
        $(".category-content").hide();
        $(".delete-category-content").hide();

        $('.category').removeAttr('style');
        $('.product').removeAttr('style');
        $('.delete-category').removeAttr('style');
        $('.product').css('font-weight', 'bold');
    });

    $('.product-close').click(function () {
        $(".product-content").hide();

        $('.category').removeAttr('style');
        $('.product').removeAttr('style');
        $('.delete-category').removeAttr('style');
    });

    $('.delete-category').click(function () {
        $(".delete-category-content").show();
        $(".product-content").hide();
        $(".category-content").hide();

        $('.category').removeAttr('style');
        $('.product').removeAttr('style');
        $('.delete-category').removeAttr('style');
        $('.delete-category').css('font-weight', 'bold');
    });

    $('.delete-category-close').click(function () {
        $(".delete-category-content").hide();

        $('.category').removeAttr('style');
        $('.product').removeAttr('style');
        $('.delete-category').removeAttr('style');
    });

});

function LoadCateType() {
    $.ajax({
        url: GetUrl("Admin/Category/LoadCateType"),
        traditional: true,
        success: function (data) {
            $('#catetype-content').html(data);
        },
        error: function () {
        },
        type: "GET"
    });
    return false;
}

function InsertCateType(catetypname) {
    var data = {
        catetypename: catetypname
    }
    $.ajax({
        url: GetUrl("Admin/Category/InsertCateType"),
        data: data,
        traditional: true,
        success: function (data) {
            if (!data.IsResult) {
                bootbox.alert(data.MsgError);
                return false;
            }
            else {
                LoadCateType();
                $('#CategoryType').val('');
            }
        },
        error: function () {
        },
        type: "POST"
    });
    return false;
}

function InsertCate(ParentCateID, CateLV, CateName, CateNameEN, CateType) {
    console.log("เข้าฟังชั่น");
    console.log("ParentCateID: " + ParentCateID);
    console.log("CateLV: " + CateLV);
    console.log("CateName: " + CateName);
    console.log("CateNameEN: " + CateNameEN);
    console.log("CateType: " + CateType);
    var data = {
        ParentCateID: ParentCateID,
        CateLV: CateLV,
        CateName: CateName,
        CateNameEN: CateNameEN,
        CateType: CateType
    }
    $.ajax({
        url: GetUrl("Admin/Category/InsertCategory"),
        data: data,
        traditional: true,
        success: function (data) {
            if (CateLV == 1) {
                console.log("CateType: "+CateType);
                LoadCategoryByType1(CateType);
            }
            if (CateLV == 2) {
                console.log("ParentCateID: " + ParentCateID);
                LoadCategoryByType2(ParentCateID);
            }
            if (CateLV == 3) {
                LoadCategoryByType3(ParentCateID);
            }
        },
        error: function () {
        },
        type: "POST"
    });
    return false;
}

function CategoryLevel1(obj) {
    var index = $('.cate-type').index(obj);
    var id = $('.cate-type').eq(index).attr('data-id');

    $('.edittype').addClass('hide');
    $('.edittype').eq(index).removeClass('hide');
    $('.deletetype').addClass('hide');
    $('.deletetype').eq(index).removeClass('hide');

    //$('#categorytype').removeAttr('style');
    //$('#catelevel1').removeAttr('style');
    //$('#catelevel2').removeAttr('style');
    //$('#catelevel3').removeAttr('style');
    $('#catelevel1').css('background-color', '#EAEAEA');
    $('#CategoryLV1').removeAttr('data-id');
    $('#CategoryLV2').removeAttr('data-id');
    $('#CategoryLV3').removeAttr('data-id');
    $('#CategoryLV1').attr('data-id', id);
    $('#CategoryLV2').attr('data-cate-id', id);
    $('#CategoryLV3').attr('data-cate-id', id);
    $('.menucategory').addClass('hide');
    $('.heightmanage').css('height', '79px');
    $('.heightmanage').css('margin-top', '20px');
    $('.img-catetype').addClass("hide");
    $('.CateMenu').css('margin-left', '30px');
    $('.img-catetype').eq(index).removeClass("hide");
    $('.CateMenu').eq(index).css('margin-left', '17px');

    SetListActive(1, true);
    SetListActive(2, false);
    SetListActive(3, false);
    LoadCategoryByType1(id, '');
    
        $(".delete-category-content").hide();
        $(".product-content").hide();
        $(".category-content").hide();
}

function SetListActive(level, isActive, index) {
    if (isActive) {
        $('#CategoryLV' + level).attr('placeholder', 'ค้นหาหมวดหมู่');
        $('#CategoryLV' + level).removeAttr('disabled');
        $('#InsertTextCateLV' + level).removeAttr('disabled');

    } else {
        if (index != null && index != undefined) {
            var name = $('.cate-lv' + level).eq(index).text();
            $('#CategoryLV' + level).attr('placeholder', name);
        }
        $('#CategoryLV' + level).attr('placeholder', name);
        $('#CategoryLV' + level).attr('disabled', 'disabled');
        $('#InsertTextCateLV' + level).attr('disabled', 'disabled');
    }
}

function LoadCategoryByType(keyword) {
    var data = {
        keyword: keyword
    }
    $('#catelevel1-content').html('');
    $('#catelevel2-content').html('');
    $('#catelevel3-content').html('');
    
    $.ajax({
        url: GetUrl("Admin/Category/LoadCateByType"),
        data: data,
        traditional: true,
        success: function (data) {
            OpenLoading(false);
            $('#catetype-content').html(data);
            $('#CategoryScrollLv1').tinyscrollbar_update();
            $('#CategoryScrollLv2').tinyscrollbar_update();
            $('#CategoryScrollLv3').tinyscrollbar_update();
        },
        error: function () {
        },
        type: "GET"
    });
    return false;
}

function LoadCategoryByType1(typeid, keyword) {
    var data = {
        catetype: typeid,
        keyword: keyword
    }
    $('#catelevel2-content').html('');
    $('#catelevel3-content').html('');


    $.ajax({
        url: GetUrl("Admin/Category/LoadCateByType1"),
        data: data,
        traditional: true,
        success: function (data) {
            OpenLoading(false);
            $('#catelevel1-content').html(data);
            $('#CategoryScrollLv1').tinyscrollbar();
            $('#CategoryScrollLv2').tinyscrollbar_update();
            $('#CategoryScrollLv3').tinyscrollbar_update();
        },
        error: function () {
        },
        type: "GET"
    });
    return false;
}

function CategoryLevel2(obj) {
        $(".delete-category-content").hide();
        $(".product-content").hide();
        $(".category-content").hide();
    var index = $('.cate-lv1').index(obj);
    var id = $('.cate-lv1').eq(index).attr('data-id');
    var name = $('.cate-lv1').eq(index).attr('data-name');

    ShowDetail(1, index);

    $('.editcate1').addClass('hide');
    $('.editcate1').eq(index).removeClass('hide');

    $('.deletecate1').addClass('hide');
    $('.deletecate1').eq(index).removeClass('hide');
    
    //$('#categorytype').removeAttr('style');
    //$('#catelevel1').removeAttr('style');
    //$('#catelevel2').removeAttr('style');
    //$('#catelevel3').removeAttr('style');
    $('#catelevel2').css('background-color', '#EAEAEA');
    $('#CategoryLV1').removeAttr('data-id');
    $('#CategoryLV2').removeAttr('data-id');
    $('#CategoryLV3').removeAttr('data-id');
    $('#CategoryLV2').attr('data-id', id);
    $('.menucategory').addClass('hide');
    $('.heightmanage').css('height', '79px');
    $('.heightmanage').css('margin-top', '20px');

    $('.cateLV1').attr('value', name);
    $('.cateLV1').attr('data-id', id);
    $('.cateMPLV1').attr('value', name);
    $('.cateMPLV1').attr('data-id', id);
    $('.cateDCLV1').attr('value', name);
    $('.cateDCLV1').attr('data-id', id);
    $('.cateOldLV1').attr('value', name);
    $('.cateOldLV1').attr('data-id', id);
    $('.cateOldMPLV1').attr('value', name);
    $('.cateOldMPLV1').attr('data-id', id);
    $('.cateOldDCLV1').attr('value', name);
    $('.cateOldDCLV1').attr('data-id', id);

    $('.cate-lv1').removeAttr('style');
    $('.cate-lv1').eq(index).css('background', '#0097CB');
    $('.cate-lv1').eq(index).css('color', '#FFFFFF');

    SetListActive(1, false, index);
    SetListActive(2, true);
    SetListActive(3, false);

    LoadCategoryByType2(id, '');
    
}

function LoadCategoryByType2(cateid, keyword) {
    var data = {
        cateid: cateid,
        keyword: keyword
    }
    $('#catelevel3-content').html('');
    $.ajax({
        url: GetUrl("Admin/Category/LoadCateByType2"),
        data: data,
        traditional: true,
        success: function (data) {
            OpenLoading(false);
            $('#catelevel2-content').html(data);
            $('#CategoryScrollLv2').tinyscrollbar();
            $('#CategoryScrollLv3').tinyscrollbar_update();
        },
        error: function () {
        },
        type: "GET"
    });
    return false;
}

function CategoryLevel3(obj) {
        $(".delete-category-content").hide();
        $(".product-content").hide();
        $(".category-content").hide();
    var index = $('.cate-lv2').index(obj);
    var id = $('.cate-lv2').eq(index).attr('data-id');
    var name = $('.cate-lv2').eq(index).attr('data-name');
    console.log("index+++++++++: " + index);
    console.log("id++++++++++++: " + id);
    console.log("name++++++++++: " + name);
    ShowDetail(2, index);

    $('.editcate2').addClass('hide');
    $('.editcate2').eq(index).removeClass('hide');

    $('.deletecate2').addClass('hide');
    $('.deletecate2').eq(index).removeClass('hide');

    //$('#categorytype').removeAttr('style');
    //$('#catelevel1').removeAttr('style');
    //$('#catelevel2').removeAttr('style');
    //$('#catelevel3').removeAttr('style');
    $('#catelevel3').css('background-color', '#EAEAEA');
    $('#CategoryLV1').removeAttr('data-id');
    $('#CategoryLV2').removeAttr('data-id');
    $('#CategoryLV3').removeAttr('data-id');
    $('#CategoryLV3').attr('data-id', id);
    $('.menucategory').addClass('hide');
    $('.heightmanage').css('height', '79px');
    $('.heightmanage').css('margin-top', '20px');

    $('.cateLV2').attr('value', name);
    $('.cateLV2').attr('data-id', id);
    $('.cateMPLV2').attr('value', name);
    $('.cateMPLV2').attr('data-id', id);
    $('.cateDCLV2').attr('value', name);
    $('.cateDCLV2').attr('data-id', id);
    $('.cateOldLV2').attr('value', name);
    $('.cateOldLV2').attr('data-id', id);
    $('.cateOldMPLV2').attr('value', name);
    $('.cateOldMPLV2').attr('data-id', id);
    $('.cateOldDCLV2').attr('value', name);
    $('.cateOldDCLV2').attr('data-id', id);

    $('.cate-lv2').removeAttr('style');
    $('.cate-lv2').eq(index).css('background', '#0097CB');
    $('.cate-lv2').eq(index).css('color', '#FFFFFF');

    SetListActive(1, false);
    SetListActive(2, false, index);
    SetListActive(3, true);
    LoadCategoryByType3(id, '');
     
}

function LoadCategoryByType3(cateid, keyword) {
    var data = {
        cateid: cateid,
        keyword: keyword
    }

    $.ajax({
        url: GetUrl("Admin/Category/LoadCateByType3"),
        data: data,
        traditional: true,
        success: function (data) {
            OpenLoading(false);
            $('#catelevel3-content').html(data);
            $('#CategoryScrollLv3').tinyscrollbar();
        },
        error: function () {
        },
        type: "GET"
    });
    return false;
}

function CategoryClick(obj) {
    var index = $('.cate-lv3').index(obj);
    var name = $('.cate-lv3').eq(index).attr('data-name');
    var id = $('.cate-lv3').eq(index).attr('data-id');

    $(".delete-category-content").hide();
    $(".product-content").hide();
    $(".category-content").hide();
    $(".category-content").show();

    $('.category').removeAttr('style');
    $('.product').removeAttr('style');
    $('.delete-category').removeAttr('style');
    $('.category').css('font-weight', 'bold');

    //$('#categorytype').removeAttr('style');
    //$('#catelevel1').removeAttr('style');
    //$('#catelevel2').removeAttr('style');
    //$('#catelevel3').removeAttr('style');

    ShowDetail(3, index);

    $('.editcate3').addClass('hide');
    $('.editcate3').eq(index).removeClass('hide');

    $('.deletecate3').addClass('hide');
    $('.deletecate3').eq(index).removeClass('hide');

    if ($('.productcount').text() != "0") {
        $('#check-delete-cate').removeAttr("checked");
        $('#check-move-product').removeAttr("disabled");
        $('#check-move-product').attr("checked", "checked");
    }
    else {
        $('#check-move-product').removeAttr("checked");
        $('#check-move-product').attr("disabled", "disabled");
        $('#check-delete-cate').attr("checked", "checked");
    }

    $('.menucategory').removeClass('hide');
    $('.heightmanage').removeAttr('style');
    $('.heightmanage').css('margin', '5px 0px 5px 20px');
    $('.nameproduct').text(name);

    $('.cateLV3').attr('value', name);
    $('.cateLV3').attr('data-id', id);
    $('.cateMPLV3').attr('value', name);
    $('.cateMPLV3').attr('data-id', id);
    $('.cateDCLV3').attr('value', name);
    $('.cateDCLV3').attr('data-id', id);
    $('.cateOldLV3').attr('value', name);
    $('.cateOldLV3').attr('data-id', id);
    $('.cateOldMPLV3').attr('value', name);
    $('.cateOldMPLV3').attr('data-id', id);
    $('.cateOldDCLV3').attr('value', name);
    $('.cateOldDCLV3').attr('data-id', id);

    $('.cate-lv3').removeAttr('style');
    $('.cate-lv3').eq(index).css('background', '#0097CB');
    $('.cate-lv3').eq(index).css('color', '#FFFFFF');

    SetListActive(1, false);
    SetListActive(2, false);
    SetListActive(3, false, index);
}

function ShowDetail(level, index) {
    var productcount = $('.cate-lv' + level).eq(index).attr('data-product-count');
    var catepath = $('.cate-lv' + level).eq(index).attr('data-path');
    $('.categorypath').text(catepath);
    $('.productcount').text(productcount);
}

function LoadCategory(catelevel, parentcateid) {
    data = {
        catelevel: catelevel,
        parentcateid: parentcateid
    };

    $.ajax({
        url: GetUrl("Admin/Category/DoLoadCategory"),
        data: data,
        traditional: true,
        dataType : 'json',
        success: function (data) {
            var html = '<option value="">---' + label.vldselectcate + '---</div>';
            $.each(data, function (i, item) {
                html += '<option value="' + item.CategoryID + '" data-id="' + item.CategoryID + '" >' + item.CategoryName + '</div>';
            });

            $('.sel-movecate-lv' + catelevel).html(html);

        },
        error: function () {
        },
        type: "POST"
    });
    return false;
}

function LoadCategory1(catelevel, parentcateid) {
    data = {
        catelevel: catelevel,
        parentcateid: parentcateid
    };

    $.ajax({
        url: GetUrl("Admin/Category/DoLoadCategory"),
        data: data,
        traditional: true,
        dataType: 'json',
        success: function (data) {
            var html = '<option value="">---' + label.vldselectcate + '---</div>';
            $.each(data, function (i, item) {
                html += '<option value="' + item.CategoryID + '" data-id="' + item.CategoryID + '" >' + item.CategoryName + '</div>';
            });

            $('.sel-moveproduct-lv' + catelevel).html(html);

        },
        error: function () {
        },
        type: "POST"
    });
    return false;
}

function LoadCategory2(catelevel, parentcateid) {
    data = {
        catelevel: catelevel,
        parentcateid: parentcateid
    };

    $.ajax({
        url: GetUrl("Admin/Category/DoLoadCategory"),
        data: data,
        traditional: true,
        dataType: 'json',
        success: function (data) {
            var html = '<option value="">---' + label.vldselectcate + '---</div>';
            $.each(data, function (i, item) {
                html += '<option value="' + item.CategoryID + '" data-id="' + item.CategoryID + '" >' + item.CategoryName + '</div>';
            });

            $('.sel-deletecate-lv' + catelevel).html(html);

        },
        error: function () {
        },
        type: "POST"
    });
    return false;
}

$('.sel-movecate-lv1').live('change', function () {
    LoadCategory(2, $(this).val());
    var Name = $(this).find(":selected").text();
    var Id = $(this).find(":selected").val();
    $('.cateLV1').attr('value', Name);
    $('.cateLV1').attr('data-id', Id);
});

$('.sel-movecate-lv2').live('change', function () {
    LoadCategory(3, $(this).val());
    var Name = $(this).find(":selected").text();
    var Id = $(this).find(":selected").val();
    $('.cateLV2').attr('value', Name);
    $('.cateLV2').attr('data-id', Id);
});

$('.sel-moveproduct-lv1').live('change', function () {
    LoadCategory1(2, $(this).val());
    var Name = $(this).find(":selected").text();
    var Id = $(this).find(":selected").val();
    $('.cateMPLV1').attr('value', Name);
    $('.cateMPLV1').attr('data-id', Id);
});

$('.sel-moveproduct-lv2').live('change', function () {
    LoadCategory1(3, $(this).val());
    var Name = $(this).find(":selected").text();
    var Id = $(this).find(":selected").val();
    $('.cateMPLV2').attr('value', Name);
    $('.cateMPLV2').attr('data-id', Id);
});

$('.sel-moveproduct-lv3').live('change', function () {
    var Name = $(this).find(":selected").text();
    var Id = $(this).find(":selected").val();
    $('.cateMPLV3').attr('value', Name);
    $('.cateMPLV3').attr('data-id', Id);
});

$('.sel-deletecate-lv1').live('change', function () {
    LoadCategory2(2, $(this).val());
    var Name = $(this).find(":selected").text();
    var Id = $(this).find(":selected").val();
    $('.cateDCLV1').attr('value', Name);
    $('.cateDCLV1').attr('data-id', Id);
});

$('.sel-deletecate-lv2').live('change', function () {
    LoadCategory2(3, $(this).val());
    var Name = $(this).find(":selected").text();
    var Id = $(this).find(":selected").val();
    $('.cateDCLV2').attr('value', Name);
    $('.cateDCLV2').attr('data-id', Id);
});

$('.sel-deletecate-lv3').live('change', function () {
    var Name = $(this).find(":selected").text();
    var Id = $(this).find(":selected").val();
    $('.cateDCLV3').attr('value', Name);
    $('.cateDCLV3').attr('data-id', Id);
});

function MoveCategory(newcatelv1, oldcatelv1, newcatelv2, oldcatelv2, newcatelv3) {
    data = {
        oldcatelv1: oldcatelv1,
        oldcatelv2: oldcatelv2,
        newcatelv1: newcatelv1,
        newcatelv2: newcatelv2,
        newcatelv3: newcatelv3
    };
    OpenLoading(true);
    $.ajax({
        url: GetUrl("Admin/Category/MoveCategory"),
        data: data,
        traditional: true,
        dataType: 'json',
        success: function (data) {
            $('#catelevel3-content').html(data);
            OpenLoading(false);
        },
        error: function () {
        },
        type: "POST"
    });
    return false;
}

function MoveProduct(newcatelv1, oldcatelv1, newcatelv2, oldcatelv2, newcatelv3, oldcatelv3) {
    data = {
        oldcatelv1: oldcatelv1,
        oldcatelv2: oldcatelv2,
        oldcatelv3: oldcatelv3,
        newcatelv1: newcatelv1,
        newcatelv2: newcatelv2,
        newcatelv3: newcatelv3
    };
    OpenLoading(true);
    $.ajax({
        url: GetUrl("Admin/Category/MoveProduct"),
        data: data,
        traditional: true,
        dataType: 'json',
        success: function (data) {
            $('#catelevel3-content').html(data);
            OpenLoading(false);
        },
        error: function () {
        },
        type: "POST"
    });
    return false;
}

function DeleteCategory(oldcatelv3) {
    data = {
        oldcatelv3: oldcatelv3
    };
    OpenLoading(true);
    $.ajax({
        url: GetUrl("Admin/Category/DeleteCategory"),
        data: data,
        traditional: true,
        dataType: 'json',
        success: function (data) {
            $('#catelevel3-content').html(data);
            OpenLoading(false);
        },
        error: function () {
        },
        type: "POST"
    });
    return false;
}

$('#save-move-cate').live('click', function () {
    if (confirm(label.vldconfirm_move_procate)) {
        if ($('.sel-movecate-lv1').find(":selected").val() != "" && $('.sel-movecate-lv2').find(":selected").val() != "") {
            var newcatelv1 = $('.cateLV1').attr('data-id');
            var oldcatelv1 = $('.cateOldLV1').attr('data-id');
            var newcatelv2 = $('.cateLV2').attr('data-id');
            var oldcatelv2 = $('.cateOldLV2').attr('data-id');
            var newcatelv3 = $('.cateLV3').attr('data-id');

            MoveCategory(newcatelv1, oldcatelv1, newcatelv2, oldcatelv2, newcatelv3);
        }
        else {
            bootbox.alert(label.vldall_required);
            return false;
        }
    }
});

$('#save-move-product').live('click', function () {
    if (confirm(label.vldconfirm_move_procate)) {
        if ($('.sel-moveproduct-lv1').find(":selected").val() != "" && $('.sel-moveproduct-lv2').find(":selected").val() != "" && $('.sel-moveproduct-lv3').find(":selected").val() != "") {
            var newcatelv1 = $('.cateMPLV1').attr('data-id');
            var oldcatelv1 = $('.cateOldMPLV1').attr('data-id');
            var newcatelv2 = $('.cateMPLV2').attr('data-id');
            var oldcatelv2 = $('.cateOldMPLV2').attr('data-id');
            var newcatelv3 = $('.cateMPLV3').attr('data-id');
            var oldcatelv3 = $('.cateOldMPLV3').attr('data-id');

            MoveProduct(newcatelv1, oldcatelv1, newcatelv2, oldcatelv2, newcatelv3, oldcatelv3);
        }
        else {
            bootbox.alert(vldall_required);
            return false;
        }
    }
});

$('#save-delete-cate').live('click', function () {
    if (confirm(label.vldconfirm_del_cate)) {
        var newcatelv1 = $('.cateDCLV1').attr('data-id');
        var oldcatelv1 = $('.cateOldDCLV1').attr('data-id');
        var newcatelv2 = $('.cateDCLV2').attr('data-id');
        var oldcatelv2 = $('.cateOldDCLV2').attr('data-id');
        var newcatelv3 = $('.cateDCLV3').attr('data-id');
        var oldcatelv3 = $('.cateOldDCLV3').attr('data-id');
        if ($('.sel-deletecate-lv1').find(":selected").val() != "" && $('.sel-deletecate-lv2').find(":selected").val() != "" && $('.sel-deletecate-lv3').find(":selected").val() != "") {
            if ($('#check-delete-cate').attr("checked") == "checked" && $('#check-move-product').attr("checked") == "checked") {
                MoveProduct(newcatelv1, oldcatelv1, newcatelv2, oldcatelv2, newcatelv3, oldcatelv3);
                DeleteCategory(oldcatelv3);
            }
            else if ($('#check-delete-cate').attr("checked") == "checked" && $('#check-move-product').attr("checked") != "checked" && $('.productcount').text() == "0") {
                DeleteCategory(oldcatelv3);
            }
            else if ($('#check-delete-cate').attr("checked") != "checked" && $('#check-move-product').attr("checked") == "checked") {
                MoveProduct(newcatelv1, oldcatelv1, newcatelv2, oldcatelv2, newcatelv3, oldcatelv3);
            }
            else if ($('#check-delete-cate').attr("checked") != "checked" && $('#check-move-product').attr("checked") != "checked") {
                bootbox.alert(label.vldconfirm_moveanddel_cate);
                return false;
            }
            else {
                bootbox.alert(label.vldmove_pro_zero);
                return false;
            }
        }
        else if ($('#check-delete-cate').attr("checked") == "checked" && $('#check-move-product').attr("checked") != "checked" && $('.productcount').text() == "0") {
            DeleteCategory(oldcatelv3);
        }
        else {
            bootbox.alert(label.vldall_required);
            return false;
        }
    }
});

function edittype(obj) {
    var index = $('.edittype').index(obj);
    var name = $('.cate-type').eq(index).attr('data-name');
    var id = $('.cate-type').eq(index).attr('data-id');
    $('#nametype').attr('value', name);
    $('#nametype').attr('data-id', id);
    $('.labletype').text(name);
    $('#myModaltype').modal('show')
}

function editcate1(obj) {
    var index = $('.editcate1').index(obj);
    var name = $('.cate-lv1').eq(index).attr('data-name');
    var nameEN = $('.cate-lv1').eq(index).attr('data-nameEN');
    var id = $('.cate-lv1').eq(index).attr('data-id');
    var catetype = $('.cate-lv1').eq(index).attr('data-catetype');
    $('#namecate1').attr('value', name);
    $('#namecate1EN').attr('value',nameEN);
    $('#namecate1').attr('data-id', id);
    $('#namecate1').attr('data-catetype', catetype);
    $('.lablecate1').text(name);
    $('#myModalcate1').modal('show')
}

function editcate2(obj) {
    var index = $('.editcate2').index(obj);
    var name = $('.cate-lv2').eq(index).attr('data-name');
    var nameEN = $('.cate-lv2').eq(index).attr('data-nameEN');
    var id = $('.cate-lv2').eq(index).attr('data-id');
    var parentid = $('.cate-lv2').eq(index).attr('data-parentid');
    $('#namecate2').attr('value', name);
    $('#namecate2EN').attr('value', nameEN);
    $('#namecate2').attr('data-id', id);
    $('#namecate2').attr('data-parentid', parentid);
    $('.lablecate2').text(name);
    $('#myModalcate2').modal('show')
}

function editcate3(obj) {
    var index = $('.editcate3').index(obj);
    var name = $('.cate-lv3').eq(index).attr('data-name');
    var nameEN = $('.cate-lv3').eq(index).attr('data-nameEN');
    var id = $('.cate-lv3').eq(index).attr('data-id');
    var parentid = $('.cate-lv3').eq(index).attr('data-parentid');
    $('#namecate3').attr('value', name);
    $('#namecate3EN').attr('value', nameEN);
    $('#namecate3').attr('data-id', id);
    $('#namecate3').attr('data-parentid', parentid);
    $('.lablecate3').text(name);
    $('#myModalcate3').modal('show')
}

$(function () {
    $('.savetype').live('click', function () {
        var name = $('#nametype').val();
        var id = $('#nametype').attr('data-id');
        updatetype(id, name);
    });
});

$(function () {
    $('.savecate1').live('click', function () {
        var name = $('#namecate1').val();
        var id = $('#namecate1').attr('data-id');
        var nameEN = $('#namecate1EN').val();
        var catetype = $('#namecate1').attr('data-catetype');
        if (checkEng(nameEN)) {
            updatecate1(id, name, nameEN, catetype);
        } else {
            OpenLoading(false);
            bootbox.alert(label.vlddata_incorrect);
            return false;
        }
    });
});

$(function () {
    $('.savecate2').live('click', function () {
        var name = $('#namecate2').val();
        var id = $('#namecate2').attr('data-id');
        var nameEN = $('#namecate2EN').val();
        var parentid = $('#namecate2').attr('data-parentid');
        if (checkEng(nameEN)) {
            updatecate2(id, name, nameEN, parentid);
        } else {
            OpenLoading(false);
            bootbox.alert(label.vlddata_incorrect);
            return false;
        }
    });
});

$(function () {
    $('.savecate3').live('click', function () {
        var name = $('#namecate3').val();
        var id = $('#namecate3').attr('data-id');
        var nameEN = $('#namecate3EN').val();
        var parentid = $('#namecate3').attr('data-parentid');
        if (checkEng(nameEN)) {
            updatecate3(id, name, nameEN, parentid);
        } else {
            OpenLoading(false);
            bootbox.alert(label.vlddata_incorrect);
            return false;
        }
    });
});

function updatetype(typeid,typename) {
    data = {
        CategoryType: typeid,
        CategoryTypeName: typename
    };
    OpenLoading(true);
    $.ajax({
        url: GetUrl("Admin/Category/UpdateCategoryType"),
        data: data,
        traditional: true,
        dataType: 'json',
        success: function (data) {
            LoadCateType();
            OpenLoading(false);
        },
        error: function () {
        },
        type: "POST"
    });
    return false;
}

function updatecate1(cateid, catename, catenameEN, catetype) {
    data = {
        CategoryID: cateid,
        CategoryName: catename,
        CategoryNameEng: catenameEN,
        CategoryType: catetype
    };
    OpenLoading(true);
    $.ajax({
        url: GetUrl("Admin/Category/UpdateCategoryLevel"),
        data: data,
        traditional: true,
        dataType: 'json',
        success: function (data) {
            LoadCategoryByType1(data.CategoryType, '');
            OpenLoading(false);
        },
        error: function () {
        },
        type: "POST"
    });
    return false;
}

function updatecate2(cateid, catename, catenameEN, parentid) {
    data = {
        CategoryID: cateid,
        CategoryName: catename,
        CategoryNameEng: catenameEN,
        ParentCategoryID: parentid
    };
    OpenLoading(true);
    $.ajax({
        url: GetUrl("Admin/Category/UpdateCategoryLevelParentID"),
        data: data,
        traditional: true,
        dataType: 'json',
        success: function (data) {
            LoadCategoryByType2(data.ParentCategoryID, '');
            OpenLoading(false);
        },
        error: function () {
        },
        type: "POST"
    });
    return false;
}

function updatecate3(cateid, catename, catenameEN, parentid) {
    data = {
        CategoryID: cateid,
        CategoryName: catename,
        CategoryNameEng: catenameEN,
        ParentCategoryID: parentid
    };
    OpenLoading(true);
    $.ajax({
        url: GetUrl("Admin/Category/UpdateCategoryLevelParentID"),
        data: data,
        traditional: true,
        dataType: 'json',
        success: function (data) {
            LoadCategoryByType3(data.ParentCategoryID, '');
            OpenLoading(false);
        },
        error: function () {
        },
        type: "POST"
    });
    return false;
}

$('.cancel').click(function () {
    $(".category-content").hide();
    $(".product-content").hide();
    $(".delete-category-content").hide();

    $('.category').removeAttr('style');
    $('.product').removeAttr('style');
    $('.delete-category').removeAttr('style');
});

function deletetype(categoryID) {
    data = {
        CategoryType: categoryID
    };
    OpenLoading(true);
    $.ajax({
        url: GetUrl("Admin/Category/DeleteCategoryType"),
        data: data,
        traditional: true,
        dataType: 'json',
        success: function (data) {
            OpenLoading(false);
            if (data.IsResult == false) {
                bootbox.alert(data.MsgError);
            } else {
                bootbox.alert("ลบประเภทหมวดหมู่เรียบร้อยแล้ว");
                LoadCateType();
            }
        },
        error: function () {
        },
        type: "POST"
    });
    return false;
}

function deletecate1(categoryID) {
    data = {
        CategoryID: categoryID,
        Type: 'CateLV1'
    };
    OpenLoading(true);
    $.ajax({
        url: GetUrl("Admin/Category/DeleteCategoryManyLevel"),
        data: data,
        traditional: true,
        dataType: 'json',
        success: function (data) {
            OpenLoading(false);
            if (data.IsResult == false) {
                bootbox.alert(data.MsgError);
            } else {
                bootbox.alert("ลบหมวดหมู่เรียบร้อยแล้ว");
                LoadCategoryByType1(data.CateType);
            }
        },
        error: function () {
        },
        type: "POST"
    });
    return false;
}

function deletecate2(categoryID) {
    data = {
        CategoryID: categoryID,
        Type: 'CateLV2'
    };
    OpenLoading(true);
    $.ajax({
        url: GetUrl("Admin/Category/DeleteCategoryManyLevel"),
        data: data,
        traditional: true,
        dataType: 'json',
        success: function (data) {
            OpenLoading(false);
            if (data.IsResult == false) {
                bootbox.alert(data.MsgError);
            } else {
                bootbox.alert("ลบหมวดหมู่เรียบร้อยแล้ว");
                console.log("data.CateType: " + data.CateType);
                LoadCategoryByType2(data.CateType);
            }
        },
        error: function () {
        },
        type: "POST"
    });
    return false;
}

function deletecate3(categoryID) {
    data = {
        CategoryID: categoryID,
        Type: 'CateLV3'
    };
    OpenLoading(true);
    $.ajax({
        url: GetUrl("Admin/Category/DeleteCategoryManyLevel"),
        data: data,
        traditional: true,
        dataType: 'json',
        success: function (data) {
            OpenLoading(false);
            if (data.IsResult == false) {
                bootbox.alert(data.MsgError);
            } else {
                bootbox.alert("ลบหมวดหมู่เรียบร้อยแล้ว");
                LoadCategoryByType3(data.CateType);
            }
        },
        error: function () {
        },
        type: "POST"
    });
    return false;
}

function checkNameThai() {

}