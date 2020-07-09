function ProductSearch() {
    if ($("#TextSearch").val() != "") {
        $("#formSearch").submit();
    } else {
        return false;
    }
}
function NumPage(PageIndex, PageSize) {
    data = {
        TextSearch: $('#TextSearch').val(),
        hidPageIndex: PageIndex,
        hidPageSize: PageSize,
        CateID: $('#hidCateID').val(),
        CateLevel: $('#hidCateLevel').val(),
        GroupID: $('#hidGroupID').val(),
        CompID: $('#hidCompID').val()
    }
    if (PageIndex == $('.hidPageIndex').val())
        return false;
    else
        Onload();
}
function SubmitPage(Obj) {

    data = {
        TextSearch: $('#TextSearch').val(),
        hidPageIndex: Obj,
        hidPageSize: $('.hidPageSize').val(),
        CateID: $('#hidCateID').val(),
        CateLevel: $('#hidCateLevel').val(),
        GroupID: $('#hidGroupID').val(),
        CompID: $('#hidCompID').val()
    }
    if (Obj == $('.hidPageIndex').val())
        return false;
    else
        Onload();
}
function SelectedPageSize(Obj) {
    data = {
        TextSearch: $('#TextSearch').val(),
        hidPageIndex: 1,
        hidPageSize: Obj,
        CateID: $('#hidCateID').val(),
        CateLevel: $('#hidCateLevel').val(),
        GroupID: $('#hidGroupID').val(),
        CompID: $('#hidCompID').val()
    }

    Onload();
}
function Onload() {
    OpenLoading(true, null, $('#WebsiteAllContent'));
    $.ajax({
        url: GetUrl("Website/Blog"),
        data: data,
        success: function (data) {

            $('#BlogData').html(data);
            //$('.gallery_inner').append('<div class="v_line" style="position:absolute;top:0;left:-0.1%; min-height: 550px;"></div>');
            var content_height = $("#BlogData").outerHeight();
            $(".v_line").height(content_height);
            OpenLoading(false, null, $('.bg_website'));

        },
        error: function () {
            bootbox.alert("Sorry, Your request is not successful.");
            OpenLoading(false, null, $('.bg_website'));
        },
        type: "POST"
    });
    return false;
}

function setCompNameStyle() {
    var CompName_H = $("h4.media-heading").height();
    var sw = screen.width;
    if (sw < 1500 && CompName_H > 20) {
        var cssObj = {

            'font-size': '18px',
            'margin-top': '0px',
            'line-height': '22px'
        };
        $("h4.media-heading").css(cssObj);
    } else if (sw > 1500 && CompName_H > 20) {
        var cssObj = {
            'font-size': '18px'
        };
        $("h4.media-heading").css(cssObj);
    }
}

function CheckWebsitePaging(Page) {
    var isPass = true;
    var PageIndex = parseInt($('#hidPageIndex').val(), 10);
    var TotalPage = parseInt($('#hidTotalPage').val(), 10);

    if (Page == 1) {
        if (TotalPage == 1) {
            $('.btn-next').removeAttr('onclick');
            $('.btn-next').css('opacity', '0.4');
            $('.btn-next').css('cursor', 'default');
            $('.btn-prev').removeAttr('onclick');
            $('.btn-prev').css('opacity', '0.4');
            $('.btn-prev').css('cursor', 'default');
        } else {
            $('.btn-next').attr('onclick', 'Next();');
            $('.btn-next').css('opacity', '1');
            $('.btn-next').css('cursor', 'pointer');
            $('.btn-prev').removeAttr('onclick');
            $('.btn-prev').css('opacity', '0.4');
            $('.btn-prev').css('cursor', 'default');
        }
    } else if (Page == TotalPage) {
        $('.btn-prev').attr('onclick', 'Prev();');
        $('.btn-prev').css('opacity', '1');
        $('.btn-prev').css('cursor', 'pointer');
        $('.btn-next').removeAttr('onclick');
        $('.btn-next').css('opacity', '0.4');
        $('.btn-next').css('cursor', 'default');
    } else if (Page > 1 && Page < TotalPage) {
        $('.btn-prev').attr('onclick', 'Prev();');
        $('.btn-prev').css('opacity', '1');
        $('.btn-prev').css('cursor', 'pointer');
        $('.btn-next').attr('onclick', 'Next();');
        $('.btn-next').css('opacity', '1');
        $('.btn-next').css('cursor', 'pointer');
    } else {
        $('.txtPageIndex').eq(0).val(PageIndex);
        isPass = false;
    }

    return isPass;
}