function CheckOrderID() {
    var bool = true;
    GetValueTagit();
    GetImgPath();
    FindCate();
    console.log("6555555555555555555");
    if (!SendName()) {
        bool = false;
    }
    //console.log('CheckBuyleadName : ' + bool);
    //if (!CheckBuyleadCode()) {
    //    bool = false;
    //}
    ////console.log('CheckBuyleadCode : ' + bool);
    //if (!CheckBuyleadExpire()) {
    //    bool = false;
    //}
    ////console.log('CheckBuyleadExpire : ' + bool);
    //if (!CheckBuyleadKeyword()) {
    //    bool = false;
    //}
    ////console.log('CheckBuyleadKeyword : ' + bool);
    //if (!CheckQty()) {
    //    bool = false;
    //}
    ////console.log('CheckQty : ' + bool);
    //if (!CheckQtyUnit()) {
    //    bool = false;
    //}
    ////console.log('CheckQtyUnit : ' + bool);

    //if (!CheckCate()) {
    //    bool = false;
    //}
    ////console.log('CheckCate : ' + bool);
    //if (!CheckBuyleadCompName()) {
    //    bool = false;
    //}
    //// console.log('CheckBuyleadCompName : ' + bool);
    //if (!CheckBuyleadContactName()) {
    //    bool = false;
    //}
    ////console.log('CheckBuyleadContactName : ' + bool);
    ////if (!CheckBuyleadPositionName()) {
    ////    bool = false;
    ////}
    ////console.log('CheckBuyleadPositionName : ' + bool);
    //if (!CheckBuyleadPhone()) {
    //    bool = false;
    //}

    ////if (!CheckCompareExpireDate()) {
    ////    console.log(CheckCompareExpireDate());
    ////    bool = false;
    ////}
    ////console.log('CheckCompareExpireDate : ' + bool);
    ////console.log('CheckBuyleadEmail : ' + bool);
    ////             if (!CheckBuyleadAddr()) {
    ////                 bool = false;
    ////             }
    //if (!CheckBuyleadDistrict()) {
    //    bool = false;
    //}
    ////console.log('CheckBuyleadDistrict : ' + bool);
    //if (!CheckBuyleadProvince()) {
    //    bool = false;
    //}
    ////console.log('CheckBuyleadProvince : ' + bool);
    ////             if (!CheckBuyleadPostal()) {
    ////                 bool = false;
    ////             }
    //$(".optionsBuyleadType").closest('.control-group').addClass('success');
    //if (!bool) {
    //    //console.log("Buylead False");
    //    var contentH = '';
    //    contentH = $('.content_left').height() + 80;
    //    $('#Content').height(contentH);
    //    bootbox.alert(label.vldall_required);
    //    return false;
    //}
    else {
        SaveOrder();
    }
}

//-----------Confirm Save and Cancel------------------------------
function SaveOrder() {
    //var BuyleadExpire = $('#dp3').val().substring(3, 6) + $('#dp3').val().substring(0, 3) + $('#dp3').val().substring(6, 10);
    data = {
        SendName: $("#SendName").val(),
        SendSername: $("#SendSername").val(),
        SendTel: $("#SendTel").val(),
        //BuyleadType: $('input[name=optionsType]:checked').val(),
        //BuyleadExpire: BuyleadExpire,
        //BuyleadKeyword: BuyleadKeyword,
        //BuyleadDetail: $("#BuyleadDetail").val(),
        //Qty: $("#Qty").val(),
        //QtyUnit: $('#QtyUnit option:selected').val(),
        //Catecode: Catecode,
        //CateLV3: CateLV3,
        //BuyleadImgPath: BuyleadImgPath,
        //BuyLeadImgOldfile: BuyLeadImgOldfile,
        //CompName: $("#CompName").val(),
        //ContactName: $("#ContactName").val(),
        //Position: $("#Position").val(),
        //Phone: $("#Phone").val(),
        //Email: $("#Email").val(),
        //Mobile: $("#Mobile").val(),
        //Fax: $("#Fax").val(),
        //Address: $("#Address").val(),
        //District: $("#District").val(),
        //Province: $("#Province").val(),
        //Postal: $("#Postal").val()
    }

    //OpenLoadingNew(true, $("body"));
    //console.log("SaveBuylead data" + data);

    $.ajax({
        url: GetUrl("Cart/AddOrderProduct"),
        data: data,
        traditional: true,
        success: function (data) {
            //CheckError(data);
            //OpenLoadingNew(false);
            ////console.log("data.IsResult" + data.IsResult);
            //if (data.IsResult == true) {
            //    //if ($('#BuyleadID').val() != null && $('#hidCompID').val() != null) {
            //    //    window.location = GetUrl("BuyleadCenter/Main/Channel2?ID=" + $('#BuyleadID').val() + "&Comp=" + $('#hidCompID').val());
            //    //}
            //    //console.log("data.IsResult true >>");
            //    OpenLoading(false, null, $('.navbar'));
            //    bootbox.alert(label.vldsave_buyleadSuccess, function () {
            //        if ($("#UserLoginCompID").val() > 0) {
            //            window.location = GetUrl("MyB2B/buylead");
            //        }
            //        else {
            //            window.location = GetUrl("BuyleadCenter/Main/Index");
            //        }
            //    });
            //} else {
            //    //console.log("data.IsResult false >>");
            //    bootbox.alert(label.vldcannot_check_info);
            //}

            window.location = GetUrl("Cart/checkout");
        },
        error: function () {
            bootbox.alert(label.vldcannot_check_info);
        },
        type: "POST"
    });

    return false;
}

