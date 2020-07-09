/*-----------Paging ------------*/
function Prev() {
    var PageIndex = parseInt($('.hidPageIndex').val(), 10);
    if (PageIndex == 1) {
        PageIndex = 1;
        $('.btn-prevpage').attr('disabled', 'disabled');
        return false;
    } else {
        PageIndex = PageIndex - 1;
    }

   // $('.hidPageIndex').val(PageIndex);
    SubmitPage(PageIndex);
}
function Next() {
    var PageIndex = parseInt($('.hidPageIndex').val(), 10);
    if (PageIndex == $('.hidTotalPage').val())
        return false;
    else
        PageIndex = PageIndex + 1;
   // $('.hidPageIndex').val(PageIndex);
    SubmitPage(PageIndex);
}
function GoToPage(obj) {
    var PageIndex = parseInt(obj.prev().val(), 10);
    var TotalPage = parseInt($('.hidTotalPage').val(), 10);

    if (CheckPage(PageIndex)) {
        if (PageIndex == $('.hidPageIndex').val())
            return false;
        else
            SubmitPage(PageIndex);
    } else {
        SubmitPage(TotalPage);
        return false;
    }
    return false;
}
function OnSearchPage() {
    SubmitPage(1);
    return false;
}
function SetPage() {
    $('.txtPageIndex').eq(0).val($('.hidPageIndex').val());
    var html = $('.hidTotalRow').val() + " items found &nbsp;&nbsp;";
    $('#lblTotalRow').html(html);
    html = " &nbsp;&nbsp;" + $('.hidTotalPage').val() + " pages";
    $('#lblTotalPage').html(html);
}
function CheckPage(Page) {
    var isPass = true;
    var PageIndex = parseInt($('.hidPageIndex').val(), 10);
    var TotalPage = parseInt($('.hidTotalPage').val(), 10);

    if (Page == 1) {
        if (TotalPage == 1) {
            $('.btn-nextpage').attr('disabled', 'disabled');
            $('.btn-prevpage').attr('disabled', 'disabled');
        } else {
            $('.btn-nextpage').removeAttr('disabled');
            $('.btn-prevpage').attr('disabled', 'disabled');
        }
    } else if (Page == TotalPage) {
        $('.btn-prevpage').removeAttr('disabled');
        $('.btn-nextpage').attr('disabled', 'disabled');
    } else if (Page > 1 && Page < TotalPage) {
        $('.btn-prevpage').removeAttr('disabled');
        $('.btn-nextpage').removeAttr('disabled');
    } else {
        $('.txtPageIndex').val(TotalPage);
        isPass = false;
    }

    return isPass;
}
/*------------------------------*/