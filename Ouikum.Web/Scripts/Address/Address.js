/*-----------------------------------Google Map----------------------------------------------------*/
var map;
var marker;
function initialize(MapLatitude, MapLongtitude, PinLatitude, PinLongtitude, Zoom, Action) {

    var myLatlng = new google.maps.LatLng(MapLatitude, MapLongtitude);
    var myOptions = {
        zoom: eval(Zoom),
        center: myLatlng,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    };
    map = new google.maps.Map(document.getElementById("map_canvas"), myOptions);
    google.maps.event.addListener(map, "zoom_changed", function () {
        var p = marker.getPosition();
        map.panTo(p);
        mapChanged();
    });
    google.maps.event.addListener(map, "dragend", function () {
        mapChanged();
    });
    var info = new google.maps.LatLng(PinLatitude, PinLongtitude);
    if (Action == 1) {
        marker = new google.maps.Marker({
            position: info,
            map: map,
            draggable: true
            // title: '$lng[LNG_MAP_MARKER_TITLE]'
        });
    } else {
        marker = new google.maps.Marker({
            position: info,
            map: map,
            draggable: false
            // title: '$lng[LNG_MAP_MARKER_TITLE]'
        });
    }
    google.maps.event.addListener(marker, "dragend", function () {
        var p = marker.getPosition();
        map.panTo(p);
        mapChanged();
    });
    mapChanged();
};
function mapChanged() {
    var p = marker.getPosition();
    //top.document.getElementById("GPinLatitude").value = p.lat();
    //top.document.getElementById("GPinLongtitude").value = p.lng();
    $("#GPinLatitude").val(p.lat());
    $("#GPinLongtitude").val(p.lng());

    var c = map.getCenter();
    //top.document.getElementById("GMapLatitude").value = c.lat();
    //top.document.getElementById("GMapLongtitude").value = c.lng();
    //top.document.getElementById("GZoom").value = map.getZoom();
    $("#GMapLatitude").val(c.lat());
    $("#GMapLongtitude").val(c.lng());
    $("#GZoom").val(map.getZoom());

    $("#lblGPinLatitude").text(c.lat());
    $("#lblGPinLongtitude").text(c.lng());
    $("#lblGMapLatitude").text(c.lat());
    $("#lblGMapLongtitude").text(c.lng());
    $("#lblGZoom").text("Zoom : " + map.getZoom());
};

/*---------------------------GetGmapByCountry----------------------------*/
function GetGmapByCountry(cid, action) {
    $.ajax({
        url: GetUrl("Address/GetGmapByCountry"),
        data: { countryid: cid },
        type: "POST",
        dataType: 'json',
        success: function (data) {
            if (data != null) {
                if (data.IsResult) {
                    initialize(data.Country["MapLatitude"], data.Country["MapLongtitude"], data.Country["PinLatitude"], data.Country["PinLongtitude"], 10, action);
                }
            }
        },
        error: function () {
            //bootbox.alert(label.cannot_check_info);
        }
    });
}
/*---------------------------Search----------------------------*/
function showAddress(address) {
    //alert(address);
    var map = new GMap2(document.getElementById("map_canvas"));
    map.addControl(new GSmallMapControl());
    map.addControl(new GMapTypeControl());
    console.log('map' + map);
    if (geocoder) {
        console.log('geocoder:' + geocoder);
        geocoder.getLatLng(
          address,
          function (point) {
              if (!point) {
                  bootbox.alert(address + " not found");
              } else {
                  document.getElementById("GMapLatitude").value = point.lat().toFixed(5);
                  document.getElementById("GMapLongtitude").value = point.lng().toFixed(5);

                  document.getElementById("GPinLatitude").value = point.lat().toFixed(5);
                  document.getElementById("GPinLongtitude").value = point.lng().toFixed(5)

                  map.clearOverlays()
                  map.setCenter(point, 14);
                  var marker = new GMarker(point, { draggable: true });
                  map.addOverlay(marker);

                  GEvent.addListener(marker, "dragend", function () {
                      var pt = marker.getPoint();
                      map.panTo(pt);
                      document.getElementById("GPinLatitude").value = pt.lat().toFixed(5);
                      document.getElementById("GPinLongtitude").value = pt.lng().toFixed(5);
                  });


                  GEvent.addListener(map, "moveend", function () {
                      map.clearOverlays();
                      var center = map.getCenter();
                      var marker = new GMarker(center, { draggable: true });
                      map.addOverlay(marker);
                      document.getElementById("GMapLatitude").value = center.lat().toFixed(5);
                      document.getElementById("GMapLongtitude").value = center.lng().toFixed(5);

                      GEvent.addListener(marker, "dragend", function () {
                          var pt = marker.getPoint();
                          map.panTo(pt);
                          document.getElementById("GPinLatitude").value = pt.lat().toFixed(5);
                          document.getElementById("GPinLongtitude").value = pt.lng().toFixed(5);
                      });

                  });

              }
          }
        );
    }
}

/*---------------------------GetGmap----------------------------*/
function GetGmap(pid) {
    $.ajax({
        url: GetUrl("Address/GetGMapByProvinceID"),
        data: { pid: pid },
        type: "POST",
        dataType: 'json',
        success: function (data) {
            if (data != null) {
                if (data.IsResult) {
                    initialize(data.Provinces["MapLatitude"], data.Provinces["MapLongtitude"], data.Provinces["PinLatitude"], data.Provinces["PinLongtitude"], 12);
                }
            }
        },
        error: function () {
            //bootbox.alert(label.cannot_check_info);
        }
    });
}
/*---------------------------Get Country Asean----------------------------*/
function GetCountryAsean(callback) {
    $.ajax({
        url: GetUrl("Address/GetCountryAsean"),
        type: "POST",
        dataType: 'json',
        success: function (data) {
            callback(data);
        },
        error: function () {
            //bootbox.alert(label.cannot_check_info);
        }
    });
}
function GetMapSearch() {
    if (GBrowserIsCompatible()) {
        var map = new GMap2(document.getElementById("map_canvas"));
        map.addControl(new GSmallMapControl());
        map.addControl(new GMapTypeControl());
        var center = new GLatLng(document.getElementById("GMapLatitude").value, document.getElementById("GMapLongtitude").value);
        map.setCenter(center, parseInt($('#GZoom').val()));
        geocoder = new GClientGeocoder();
        var edit = $('#editAll').val();
        var myLatlng = new google.maps.LatLng(document.getElementById("GMapLatitude").value, document.getElementById("GMapLongtitude").value);
        var marker = new GMarker(edit == 1 ? center : myLatlng, { draggable: edit == 1 ? true : false });
        map.addOverlay(marker);

        GEvent.addListener(map, "zoomend", function (oldzoomlevel, newzoomlevel) {

            document.getElementById("GZoom").value = newzoomlevel;
        });

        document.getElementById("GMapLatitude").value = center.lat($('#GZoom').val());
        document.getElementById("GMapLongtitude").value = center.lng($('#GZoom').val());

        GEvent.addListener(marker, "dragend", function () {
            var point = marker.getPoint();
            map.panTo(point);
            document.getElementById("GPinLatitude").value = point.lat($('#GZoom').val());
            document.getElementById("GPinLongtitude").value = point.lng($('#GZoom').val());

        });

        GEvent.addListener(map, "moveend", function () {
            map.clearOverlays();
            var center = map.getCenter();
            var marker = new GMarker(edit == 1 ? center : myLatlng, { draggable: edit == 1 ? true : false });
            map.addOverlay(marker);
            document.getElementById("GMapLatitude").value = center.lat($('#GZoom').val());
            document.getElementById("GMapLongtitude").value = center.lng($('#GZoom').val());


            GEvent.addListener(marker, "dragend", function () {
                var point = marker.getPoint();
                map.panTo(point);
                document.getElementById("GPinLatitude").value = point.lat($('#GZoom').val());
                //document.getElementById("PinLongtitude").value = point.lng().toFixed($('#MapZoom').val());
                document.getElementById("GPinLongtitude").value = point.lng($('#GZoom').val());
            });

        });
    }
}
/*---------------------------GetDistrictByProvince----------------------------*/
function GetDistrictByProvince(p_id, d_id, id) {
    var ListDistricts = "<option value='0'>-----" + label.vldselectdistrict + "-----</option>";
    $.ajax({
        url: GetUrl("Address/GetDistrictByProvinceID"),
        data: { pid: p_id },
        type: "POST",
        dataType: 'json',
        success: function (data) {
            if (data != null) {
                if (data.IsResult) {
                    for (var index = 0; index < data.Districts.length; index++) {
                        if (d_id == parseInt(data.Districts[index].DistrictID)) {
                            ListDistricts += "<option value=" + data.Districts[index].DistrictID + " selected='selected'>" + data.Districts[index].DistrictName + "</option>";
                        } else {
                            ListDistricts += "<option value=" + data.Districts[index].DistrictID + ">" + data.Districts[index].DistrictName + "</option>";
                        }
                    }
                    $("#" + id).html(ListDistricts);
                } else {
                    $("#" + id).html(ListDistricts);
                }
            }
        },
        error: function () {
            //bootbox.alert(label.cannot_check_info);
        }
    });
}
/*---------------------------GetDistrictByProvinceEng----------------------------*/
function GetDistrictEngByProvinceID(p_id, d_id, id) {
    var ListDistricts = "<option value='0'>-----" + label.vldselectdistrict + "-----</option>";
    $.ajax({
        url: GetUrl("Address/GetDistrictEngByProvinceID"),
        data: { pid: p_id },
        type: "POST",
        dataType: 'json',
        success: function (data) {
            if (data != null) {
                if (data.IsResult) {
                    for (var index = 0; index < data.Districts.length; index++) {
                        if (d_id == parseInt(data.Districts[index].DistrictID)) {
                            ListDistricts += "<option value=" + data.Districts[index].DistrictID + " selected='selected'>" + data.Districts[index].DistrictNameEng + "</option>";
                        } else {
                            ListDistricts += "<option value=" + data.Districts[index].DistrictID + ">" + data.Districts[index].DistrictNameEng + "</option>";
                        }
                    }
                    $("#" + id).html(ListDistricts);
                } else {
                    $("#" + id).html(ListDistricts);
                }
            }
        },
        error: function () {
            //bootbox.alert(label.cannot_check_info);
        }
    });
}
/*---------------------------GetDistrict----------------------------*/
function GetDistrict(d_id, id) {
    var ListDistricts = "";
    $.ajax({
        url: GetUrl("Address/GetDistrict"),
        data: { d_id: d_id },
        type: "POST",
        dataType: 'json',
        success: function (data) {
            if (data != null) {
                ListDistricts = "<option value=" + data.DistrictID + " selected='selected'>" + data.DistrictName + "</option>";
                $("#" + id).html(ListDistricts);
            }
        },
        error: function () {
            //bootbox.alert(label.cannot_check_info);
        }
    });
}
function GetDistrictEng(d_id, id) {
    var ListDistricts = "";
    $.ajax({
        url: GetUrl("Address/GetDistrict"),
        data: { d_id: d_id },
        type: "POST",
        dataType: 'json',
        success: function (data) {
            if (data != null) {
                ListDistricts = "<option value=" + data.DistrictID + " selected='selected'>" + data.DistrictNameEng + "</option>";
                $("#" + id).html(ListDistricts);
            }
        },
        error: function () {
            //bootbox.alert(label.cannot_check_info);
        }
    });
}
/*---------------------------GetProvince----------------------------*/
function GetProvince(p_id, id) {
    var ListProvinces = "";
    $.ajax({
        url: GetUrl("Address/GetProvince"),
        data: { p_id: p_id },
        type: "POST",
        dataType: 'json',
        success: function (data) {
            if (data != null) {
                ListProvinces = "<option value=" + data.ProvinceID + " selected='selected'>" + data.ProvinceName + "</option>";
                $("#" + id).html(ListProvinces);
            }
        },
        error: function () {
            // bootbox.alert(label.cannot_check_info);
        }
    });
}
function GetProvinceEng(p_id, id) {
    var ListProvinces = "";
    $.ajax({
        url: GetUrl("Address/GetProvince"),
        data: { p_id: p_id },
        type: "POST",
        dataType: 'json',
        success: function (data) {
            if (data != null) {
                ListProvinces = "<option value=" + data.ProvinceID + " selected='selected'>" + data.ProvinceNameEng + "</option>";
                $("#" + id).html(ListProvinces);
            }
        },
        error: function () {
            // bootbox.alert(label.cannot_check_info);
        }
    });
}
/*---------------------------ListProvince----------------------------*/
function ListProvince(p_id, id) {
    var ListProvinces = "";
    $.ajax({
        url: GetUrl("Address/ListProvince"),
        //data: { p_id: p_id },
        type: "POST",
        dataType: 'json',
        success: function (data) {
            if (data != null) {
                if (id == 'ddlCategories') { ListProvinces += "<option value='0'>" + label.vldAllProvinces + "</option>"; }
                else { ListProvinces += "<option value='0'>-----" + label.vldselectprovince + "-----</option>"; }
                for (var index = 0; index < data.Provinces.length; index++) {
                    if (p_id == parseInt(data.Provinces[index].ProvinceID)) {
                        ListProvinces += "<option value=" + data.Provinces[index].ProvinceID + " selected='selected'>" + data.Provinces[index].ProvinceName + "</option>";
                    } else {
                        ListProvinces += "<option value=" + data.Provinces[index].ProvinceID + ">" + data.Provinces[index].ProvinceName + "</option>";
                    }
                }
                $("#" + id).html(ListProvinces);
            } else {
                $("#" + id).html(ListProvinces);
            }
        },
        error: function () {
            // bootbox.alert(label.cannot_check_info);
        }
    });
}
function ListProvinceEng(p_id, id) {
    var ListProvinces = "";
    $.ajax({
        url: GetUrl("Address/ListProvince"),
        //data: { p_id: p_id },
        type: "POST",
        dataType: 'json',
        success: function (data) {
            if (data != null) {
                ListProvinces += "<option value='0'>-----" + label.vldselectprovince + "-----</option>";
                for (var index = 0; index < data.Provinces.length; index++) {
                    if (p_id == parseInt(data.Provinces[index].ProvinceID)) {
                        ListProvinces += "<option value=" + data.Provinces[index].ProvinceID + " selected='selected'>" + data.Provinces[index].ProvinceNameEng + "</option>";
                    } else {
                        ListProvinces += "<option value=" + data.Provinces[index].ProvinceID + ">" + data.Provinces[index].ProvinceNameEng + "</option>";
                    }
                }
                $("#" + id).html(ListProvinces);
            } else {
                $("#" + id).html(ListProvinces);
            }
        },
        error: function () {
            // bootbox.alert(label.cannot_check_info);
        }
    });
}
/*---------------------------ListDistrict----------------------------*/
function ListDistrict(d_id, id) {
    var ListDistricts = "";
    $.ajax({
        url: GetUrl("Address/ListDistrict"),
        //data: { p_id: p_id },
        type: "POST",
        dataType: 'json',
        success: function (data) {
            if (data != null) {
                ListDistricts += "<option value='0'>-----" + label.vldselectdistrict + "-----</option>";
                for (var index = 0; index < data.Districts.length; index++) {
                    if (d_id == parseInt(data.Districts[index].DistrictID)) {
                        ListDistricts += "<option value=" + data.Districts[index].DistrictID + " selected='selected'>" + data.Districts[index].DistrictName + "</option>";
                    } else {
                        ListDistricts += "<option value=" + data.Districts[index].DistrictID + ">" + data.Districts[index].DistrictName + "</option>";
                    }
                }
                $("#" + id).html(ListDistricts);
            } else {
                $("#" + id).html(ListDistricts);
            }
        },
        error: function () {
            // bootbox.alert(label.cannot_check_info);
        }
    });
}
function ListDistrictEng(d_id, id) {
    var ListDistricts = "";
    $.ajax({
        url: GetUrl("Address/ListDistrict"),
        //data: { p_id: p_id },
        type: "POST",
        dataType: 'json',
        success: function (data) {
            if (data != null) {
                ListDistricts += "<option value='0'>-----" + label.vldselectdistrict + "-----</option>";
                for (var index = 0; index < data.Districts.length; index++) {
                    if (d_id == parseInt(data.Districts[index].DistrictID)) {
                        ListDistricts += "<option value=" + data.Districts[index].DistrictID + " selected='selected'>" + data.Districts[index].DistrictNameEng + "</option>";
                    } else {
                        ListDistricts += "<option value=" + data.Districts[index].DistrictID + ">" + data.Districts[index].DistrictNameEng + "</option>";
                    }
                }
                $("#" + id).html(ListDistricts);
            } else {
                $("#" + id).html(ListDistricts);
            }
        },
        error: function () {
            // bootbox.alert(label.cannot_check_info);
        }
    });
}