(function ($) {
    $.fn.Countries = function (options) {
        var t = this;
        var defaults = {
            url: "Address/"
        };
        var opts = $.extend(defaults, options);
        ListCounties += "<option value='0'>-----" + label.vldselectcountry + "-----</option>";
        $.ajax({
            url: GetUrl(opts.url + "Countries"),
            type: "POST",
            dataType: 'json',
            success: function (data) {
                if (data != null) {
                    $.each(data.Counties, function (key, item) {
                        if (opts.value == parseInt(item.CountryID)) {
                            ListCounties += "<option value=" + item.CountryID + " selected='selected'>" + item.CountryeName + "</option>";
                        } else {
                            ListCounties += "<option value=" + item.CountryID + ">" + item.CountryName + "</option>";
                        }
                    });
                    t.html(ListCounties);
                } else {
                    t.html(ListCounties);
                }
            }
        });
    };

    $.fn.Provinces = function (options) {
        var t = this;
        var defaults = {
            url: "Address/"
        };
        var opts = $.extend(defaults, options);
        ListProvinces += "<option value='0'>-----" + label.vldselectprovince + "-----</option>";
        $.ajax({
            url: GetUrl(opts.url + "Provinces"),
            data: { value: opts.value },
            type: "POST",
            dataType: 'json',
            success: function (data) {
                if (data != null) {
                    $.each(data.Provinces, function (key, item) {
                        if (opts.value == parseInt(item.ProvinceID)) {
                            ListProvinces += "<option value=" + item.ProvinceID + " selected='selected'>" + item.ProvinceName + "</option>";
                        } else {
                            ListProvinces += "<option value=" + item.ProvinceID + ">" + item.ProvinceName + "</option>";
                        }
                    });
                    t.html(ListProvinces);
                } else {
                    t.html(ListProvinces);
                }
            }
        });
    };
    $.fn.Districts = function (options) {
        var t = this;
        var defaults = {
            url: "Address/"
        };
        var opts = $.extend(defaults, options);
        ListDistricts += "<option value='0'>-----" + label.vldselectdistrict + "-----</option>";
        $.ajax({
            url: GetUrl(opts.url + "Districts"),
            data: { value: opts.value },
            type: "POST",
            dataType: 'json',
            success: function (data) {
                if (data != null) {
                    $.each(data.Districts, function (key, item) {
                        if (opts.value == parseInt(item.DistrictID)) {
                            ListDistricts += "<option value=" + item.DistrictID + " selected='selected'>" + item.DistrictName + "</option>";
                        } else {
                            ListDistricts += "<option value=" + item.DistrictID + ">" + item.DistrictName + "</option>";
                        }
                    });
                    t.html(ListDistricts);
                } else {
                    t.html(ListDistricts);
                }
            }
        });
    };
    $.fn.DistrictByProvince = function (options) {
        var t = this;
        var defaults = {
            url: "Address/"
        };
        var opts = $.extend(defaults, options);
        var ListDistricts = "<option value='0'>-----" + label.vldselectdistrict + "-----</option>";
        $.ajax({
            url: GetUrl(opts.url + "DistrictByProvinces"),
            data: { province: opts.province, district: opts.district },
            type: "POST",
            dataType: 'json',
            success: function (data) {
                if (data != null) {
                    $.each(data.Districts, function (key, item) {
                        if (opts.district == parseInt(item.DistrictID)) {
                            ListDistricts += "<option value=" + item.DistrictID + " selected='selected'>" + item.DistrictName + "</option>";
                        } else {
                            ListDistricts += "<option value=" + item.DistrictID + ">" + item.DistrictName + "</option>";
                        }
                    });
                    t.html(ListDistricts);
                } else {
                    t.html(ListDistricts);
                }
            }
        });
    };
})(jQuery);