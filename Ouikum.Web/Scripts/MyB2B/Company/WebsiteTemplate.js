$(function () {
    $("em").remove();
    $(".t-upload-button > span").remove();

    $('.btn-tootip-bottom').tooltip({ placement: 'bottom' });
    $('.btn-tootip-top').tooltip({ placement: 'top' });
    $('#edit').click(function () {
        $(this).hide();
        $('.show').hide();
        $('.hide').show();
        $('.icon-ShowHide.hide').removeAttr('style');
        $('.btn-file').show();

    });
    $("ul.toggle > li").each(function (index) {
        if ($(this).attr("id") == 'tab_template') {
            $(this).addClass('active_template');
        }
        else {
            $(this).removeClass('active_template');
        }
    });

    $("#theme_group_free div").live("click", function () {
        OpenLoading(true);
        $("#theme_group_free .check_theme").remove();
        $("#theme_group_gold .check_theme").remove();
        $(this).prepend("<div class='check_theme'></div>");
        $("#CompWebsiteTemplate").val($(this).children('img').attr('id'));
        $(".preview").html("<a class='btn btn-default offset' target='_blank' href='/MyB2B/Company/PreviewTemplate?template=" + $("#CompWebsiteTemplate").val() + "'><i class='icon-eye-open mar_t3 mar_r5'></i>" + label.vldpreview + "</a>");
        OpenLoading(false);
    });
    $("#theme_group_gold div").live("click", function () {
        OpenLoading(true);
        $("#theme_group_gold .check_theme").remove();
        $("#theme_group_free .check_theme").remove();
        $(this).prepend("<div class='check_theme'></div>");
        $("#CompWebsiteTemplate").val($(this).children('img').attr('id'));
        $(".preview").html("<a class='btn btn-default offset' target='_blank' href='/MyB2B/Company/PreviewTemplate?template=" + $("#CompWebsiteTemplate").val() + "'><i class='icon-eye-open mar_t3 mar_r5'></i>" + label.vldpreview + "</a>");
        OpenLoading(false);
    });
    $("#css_group_gold div").live("click", function () {
        OpenLoading(true);
        $("#css_group_gold .check_theme").remove();
        $(this).prepend("<div class='check_theme'></div>");
        $("#CompWebsiteCss").val($(this).children('img').attr('id'));
        $(".preview").html("<a class='btn btn-default offset' target='_blank' href='/MyB2B/Company/PreviewTemplate?css=" + $("#CompWebsiteCss").val() + "'><i class='icon-eye-open mar_t3 mar_r5'></i>" + label.vldpreview + "</a>");
        OpenLoading(false);
    });

    /*------------------------------------------------*/
    $("#submit_top,#submit_bottom,#submit_bottom1,#submit_bottom3,#submit_bottom4").click(function () {
        data = {
            CompWebsiteTemplate: $('#CompWebsiteTemplate').val(),
            CompWebsiteCss: $('#CompWebsiteCss').val()//,
            //RowVersion: $('#RowVersion').val()
        }
        OpenLoading(true);
        $.ajax({
            url: GetUrl("Company/WebsiteTemplate"),
            data: data,
            type: "POST",
            datatype: "json",
            traditional: true,
            success: function (data) {
                if (data["result"]) {
                    //$("#RowVersion").val(data["RowVersion"]);
                    OpenLoading(false);
                    bootbox.alert(label.vldsave_success);

                } //end if
                else {
                    OpenLoading(false);
                    bootbox.alert(label.vldsave_unsuccess);
                }
            },
            error: function () {
                OpenLoading(false);
                // bootbox.alert(label.vldcannot_check_info);
            }
        });
    })
    
    setTimeout("autoHeight('container-fluid')", 100000);
});