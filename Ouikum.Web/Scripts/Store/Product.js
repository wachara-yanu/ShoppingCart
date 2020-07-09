   //autoComplete('/Store/GetProductName');
    checkIEVersion();
    $("body").css("overflow", "hidden");
    var DispMid = ($(window).height() / 2) - 100;
    var Scollbar = "";
    if ($(window).width() == 1024) {
        Scollbar = $(window).height() - 200;
     }
    else{
        Scollbar = $(window).height() - 180;
    }

    $(".img_Preview").css("margin-top", DispMid);
    $(".img_Next").css("margin-top", DispMid);
    $("#Preview").css("height", Scollbar - 12);
    $("#Next").css("height", Scollbar-12);
    $("#ContentPro_List").css("height", Scollbar - 10);
   
    //----------Scrollbar------------
    $('#TabDetail').scrollbars();
    $('#TabContent').scrollbars();

    if ($('#divCateItem').width() < 190) {
        $("#divContentCate").css("width", "16%").css("padding", "1%");
        $(".cate_content").css("width", "150%").css("margin-left", "-20px");
    }

    $('.btn-tootip-left').tooltip({ placement: 'left' });
    ChooseProduct();

    $(".ChooseProduct").live("change", function () {
        ChooseProduct($(this));
    });
    $('#divCateItemLV2').addClass("hide");

    /*--------Popover-------*/
    $("#ImgSearchAdv").popover({
        placement: 'bottom',
        content: $("#SetDisplay").html(),
        template: '<div class="popover popoverSetting" id="Display_option" style="width:310px; margin-top:50px;"><div class="arrow"" ></div><div class="popover-inner">'
        + '<div class="popover-content"><p></p></div></div></div>',
        html: true
    });
    $('.icon-remove').popover('hide');
    $('html').click(function () {
        $('#Display_option').popover('hide');
    });

    /*--------------text Search------------*/
    $("#txtSearch").keypress(function (e) {
        if (e.which == 13) {
           $('#SelectCate').attr("Class",0);
           $('#SelectCate').val(0);
           SubmitPage(1);
        }
    });

    $('.item-product').live('click', function () {
        var id = parseInt($(this).attr('data-id'), 10);
        var name = $(this).attr('data-name')
        TitleText(name + ' | ' + label.vldweb);
        OpenDetail(id);
    });

    function SearchAll(){
        SelectCate(0, 0);
        $("#divCateNameLV1").html("");
        $("#divCateNameLV2").html("");
        $("#divCateNameLV3").html("");
    }
    function ClearCondition(){
        CancelSetOption();
        $('#SelectCate').removeClass().removeAttr("value");
        $('#SelectCate').addClass("0").attr("value", 0);
        $('#txtSearch').val("");
        $('#ddlBizType option:selected').val();
        $('input[name=ChkCompLevel]').removeAttr("checked");
        data = {
            txtSearch: "",
            BizTypeID: 0,
            CateLevel: 0,
            CategoryID: 0,
            CompLevelID: 0,
            Layout: $("input[name='View']:checked").val(),
            Sort: $('#ddlSort option:selected').val(),
            PSize: $('.hidPageSize').val(),
            PIndex: 1
        }
       
        $.ajax({
            url: GetUrl("Store/SelectCateLV"),
            data: { ID : 0,
                    CateLV : 0},
            success: function (data) {
                $('#divCateItem').html(data);
                Onload();
            },
            type: "POST"
        })
    }
    //function CheckAndLoadProductDetail() { 
    //    var id = parseInt($('#hidProductDetailID').val(),10);
    //    if(id > 0){
    //        OpenDetail(id);
    //    } 
    //}
    //function OpenDetail(Obj) {
    //    //OpenLoading(true);
    //    $.ajax({
    //        url: GetUrl("Store/Detail"),
    //        data: {
    //            ProductID: Obj,
    //        },
    //        success: function (data) {
    //            $('#ContentProDetail').html(data);
    //            OpenLoading(false);
    //            //OpenModalDetail();
    //        },
    //        type: "POST"
    //    });
    //}
    //function OpenModalDetail(){
    //    $('#ModalProductDetail').modal('show');   
    //}
    function ChooseProduct(obj) {
        if (obj != undefined || obj != null) {
            if (obj.attr("checked") == true || obj.attr("checked") == "checked") {
                obj.parents(".divProductItem").addClass('hdivProductItem');
                obj.parents(".divProductItemG").addClass('hdivProductItem').attr("Show","true").find(".divSlide").animate({ top: "-35%" },0);
                
                obj.find(".divAct").css({ 'background-color': '#F0F0F0', 'border': '1px solid #ccc', "height": "112px" });
            } else {
                obj.parents(".divProductItem").removeClass('hdivProductItem');
                obj.parents(".divProductItemG").removeClass('hdivProductItem').removeAttr("Show");
                obj.find(".divAct").css({ 'background-color': '#F0F0F0',"height": "112px" });
            }
        } else {
            for (var i = 0; i < (".ChooseProduct").length; i++) {
                ChooseProduct($(".ChooseProduct").eq(i - 1));
                ChooseProduct($(".ChooseProduct").eq(i - 1));
            }
        }
    }
    function CheckProductAll(Obj) {
        if (Obj.attr("checked") == true || Obj.attr("checked") == "checked") {
            $(".ChooseProduct").attr("checked", "checked");
            $(".divProductItem").addClass("hdivProductItem");
            $(".divProductItemG").addClass("hdivProductItem");
        } else {
            $(".ChooseProduct").removeAttr("checked");
            $(".divProductItem").removeClass("hdivProductItem");
            $(".divProductItemG").removeClass("hdivProductItem");
        }
    }
    /*------Display-Setting-------*/
    function CloseSearchAdv() {
       $('#Display_option').hide();
    }
    function Selected(val) {
        $("#ddlSort option[value='" + val + "']").attr("selected", "selected");
    }
    function CancelSetOption() {
        $('input[value="Gallery"]').attr('checked', true);
        $("#ddlSort option[value='1']").attr("selected", "selected");
    }
    var data = "";
    function SelectCate(id, level) {
        var CompLevel = 0;
        var CateName = "";

        $('#SelectCate').removeClass().removeAttr("value");
        if (level == 1) {
            $('#SelectCate').addClass("1").attr("value", id);
        } else if (level == 2) {
            $('#SelectCate').addClass("2").attr("value", id);
        } else if (level == 3) {
            $('#SelectCate').addClass("3").attr("value", id);
        } else {
            $('#SelectCate').addClass("0").attr("value", id);
        }

        if ($('input[name=ChkCompLevel]:checked').attr("checked") == true || $('input[name=ChkCompLevel]:checked').attr("checked") == "checked") {
            CompLevel = $('#CompLevel').val();
        }
        data = {
            txtSearch: $('#txtSearch').val(),
            BizTypeID: $('#ddlBizType option:selected').val(),
            CateLevel: $('#SelectCate').attr("Class"),
            CategoryID: id,
            CompLevelID: CompLevel,
            Layout: $("input[name='View']:checked").val(),
            Sort: $('#ddlSort option:selected').val(),
            PSize: $('.ddlPageSize option:selected').val(),
            PIndex: 1
        }

        $.ajax({
            url: GetUrl("Store/SelectCateLV"),
            data: { ID : id,CateLV : level},
            success: function (data) {
                $('#divCateItem').html(data);
                Onload();
            },
            type: "POST"
        })
    }
    function AdvanceSearch() {
        var CompLevel = 0;
        if ($('input[name=ChkCompLevel]:checked').attr("checked") == true || $('input[name=ChkCompLevel]:checked').attr("checked") == "checked") {
            CompLevel = $('#CompLevel').val();
        }
        var CateLV = 0;
        var CateID = 0;
        var lblCate = $('#SelectCate').attr("Class");
        var lblCateID = $('#SelectCate').val();

        if ((lblCate != "") && (lblCate != null)) {
            CateLV = $('#SelectCate').attr("Class");
        }
        if ((lblCateID != "") && (lblCateID != null)) {
            CateID = lblCateID;
        }
        data = {
            //------Search Advance
            txtSearch: $('#txtSearch').val(),
            BizTypeID: $('#ddlBizType option:selected').val(),
            CateLevel: CateLV,
            CategoryID: CateID,
            CompLevelID: CompLevel,
            //------Sort By
            Layout: $("input[name='View']:checked").val(),
            Sort: $('#ddlSort option:selected').val(),
            PSize: $('.ddlPageSize option:selected').val(),
            PIndex:1
        }
        Onload();
    }
    function SetOption() {
        var CompLevel = 0;
        if ($('input[name=ChkCompLevel]:checked').attr("checked") == true || $('input[name=ChkCompLevel]:checked').attr("checked") == "checked") {
            CompLevel = $('#CompLevel').val();
        }
        var CateLV = 0;
        var CateID = 0;
        var Index  = 0;
        var lblCate = $('#SelectCate').attr("Class");
        var lblCateID = $('#SelectCate').val();

        if ((lblCate != "") && (lblCate != null)) {
            CateLV = $('#SelectCate').attr("Class");
        }
        if ((lblCateID != "") && (lblCateID != null)) {
            CateID = lblCateID;
        }

        data = {
            //------Search Advance
            txtSearch: $('#txtSearch').val(),
            BizTypeID: $('#ddlBizType option:selected').val(),
            CateLevel: CateLV,
            CategoryID: CateID,
            CompLevelID: CompLevel,
            //------Sort By
            Layout: $("input[name='View']:checked").val(),
            Sort: $('#ddlSort option:selected').val(),
            PSize: $('.hidPageSize').val(),
            PIndex: $('.txtPageIndex').val()
        }
        Onload();
    }
    function Onload() {
        OpenLoading(true);
        $.ajax({
            url: GetUrl("Store/List"),
            data: data,
            success: function (data) {
                $('#ProductDetail').html(data);
                OpenLoading(false);
            },
            type: "POST"
        });
    }
    function NumPage(PageIndex, PageSize) {
        var CompLevel = 0;
        if ($('input[name=ChkCompLevel]:checked').attr("checked") == true || $('input[name=ChkCompLevel]:checked').attr("checked") == "checked") {
            CompLevel = $('#CompLevel').val();
        }
        var CateLV = 0;
        var CateID = 0;
        var Index = 0;
        var lblCate = $('#SelectCate').attr("Class");
        var lblCateID = $('#SelectCate').val();

        if ((lblCate != "") && (lblCate != null)) {
            CateLV = $('#SelectCate').attr("Class");
        }
        if ((lblCateID != "") && (lblCateID != null)) {
            CateID = lblCateID;
        }

        data = {
            //------Search Advance
            txtSearch: $('#txtSearch').val(),
            BizTypeID: $('#ddlBizType option:selected').val(),
            CateLevel: CateLV,
            CategoryID: CateID,
            CompLevelID: CompLevel,
            //------Sort By
            Layout: $("input[name='View']:checked").val(),
            Sort: $('#ddlSort option:selected').val(),
            PSize: PageSize,
            PIndex: PageIndex
        }
        if (PageIndex == $('.hidPageIndex').val())
            return false;
        else
            Onload();
    }
    function SubmitPage(Obj) {
        var CompLevel = 0;
        if ($('input[name=ChkCompLevel]:checked').attr("checked") == true || $('input[name=ChkCompLevel]:checked').attr("checked") == "checked") {
            CompLevel = $('#CompLevel').val();
        }
        var CateLV = 0;
        var CateID = 0;
        var Index = 0;
        var lblCate = $('#SelectCate').attr("Class");
        var lblCateID = $('#SelectCate').val();

        if ((lblCate != "") && (lblCate != null)) {
            CateLV = $('#SelectCate').attr("Class");
        }
        if ((lblCateID != "") && (lblCateID != null)) {
            CateID = lblCateID;
        }

        data = {
            //------Search Advance
            txtSearch: $('#txtSearch').val(),
            BizTypeID: $('#ddlBizType option:selected').val(),
            CateLevel: CateLV,
            CategoryID: CateID,
            CompLevelID: CompLevel,
            //------Sort By
            Layout: $("input[name='View']:checked").val(),
            Sort: $('#ddlSort option:selected').val(),
            PSize: $('.hidPageSize').val(),
            PIndex: Obj
        }
        if (Obj == $('.hidPageIndex').val())
            return false;
        else
            Onload();
    }
    function SelectedPageSize(Obj) {
        var CompLevel = 0;
        if ($('input[name=ChkCompLevel]:checked').attr("checked") == true || $('input[name=ChkCompLevel]:checked').attr("checked") == "checked") {
            CompLevel = $('#CompLevel').val();
        }
        var CateLV = 0;
        var CateID = 0;
        var Index = 0;
        var lblCate = $('#SelectCate').attr("Class");
        var lblCateID = $('#SelectCate').val();

        if ((lblCate != "") && (lblCate != null)) {
            CateLV = $('#SelectCate').attr("Class");
        }
        if ((lblCateID != "") && (lblCateID != null)) {
            CateID = lblCateID;
        }

        data = {
            //------Search Advance
            txtSearch: $('#txtSearch').val(),
            BizTypeID: $('#ddlBizType option:selected').val(),
            CateLevel: CateLV,
            CategoryID: CateID,
            CompLevelID: CompLevel,
            //------Sort By
            Layout: $("input[name='View']:checked").val(),
            Sort: $('#ddlSort option:selected').val(),
            PSize: Obj,
            PIndex: 1
        }
        Onload();
    }


    function getInternetExplorerVersion()
        // Returns the version of Windows Internet Explorer or a -1
        // (indicating the use of another browser).
    {
        var rv = -1; // Return value assumes failure.
        if (navigator.appName == 'Netscape') {
            var ua = navigator.userAgent;
            var re = new RegExp("MSIE ([0-9]{1,}[\.0-9]{0,})");
            if (re.exec(ua) != null)
                rv = parseFloat(RegExp.$1);
        }
        return rv;
    }

    function checkIEVersion() {
        var isIE = getCookie("isIE");
        var ver = getInternetExplorerVersion();
        if (isIE == null || isIE != ver) {
            var msg = "You're not using Windows Internet Explorer.";

            if (ver > -1) {
                if (ver <= 8.0) {
                    msg = '"Internet Explorer ของคุณมีเวอร์ชั่นที่ต่ำกว่าปัจจุบัน อาจทำให้การแสดงผลเว็บไซต์มีข้อผิดพลาดเกิดขึ้น เพื่อการใช้งานเว็บไซต์ได้อย่างเต็มประสิทธิภาพ กรุณาอัพเกรดเวอร์ชั่น Internet Explorer ของคุณ';
                    bootbox.alert(msg);
                    ChangIsIE(ver)
                }
            }
        }
    }

    function ChangIsIE(ver) {
        var isIE = getCookie("isIE");
        if (isIE == null || isIE != ver) {
            deleteCookie("isIE", "/");
            setCookie("isIE", ver, "1", "/");
        }
    }