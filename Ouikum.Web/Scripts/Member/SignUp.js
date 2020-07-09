/*--------------------------checkUserName------------------------------*/
function checkUserName() {
    var result;
    if (checkEng($('#UserName').val())) {
        $.ajax({
            url: GetUrl("Member/ValidateRegister"),
            data: { username: $('#UserName').val() },
            type: "POST",
            async: false,
            success: function (data) {
                if (!data) {
                    //OpenLoading(false);
                    $("#UserName").closest('.control-group').removeClass('success');
                    $("#UserName").closest('.control-group').addClass('error');
                    $(".UserName > .error").text(label.vldusername_exists);
                    $(".UserName > .error").css('text-align', 'left');
                    checkError();
                    result = false;
                } //end if
                else {
                    checkError();
                    result = true;
                    //OpenLoading(false); 
                }
            },
            error: function () {
                // bootbox.alert(label.cannot_check_info);
            }
        });
    } else {
        $("#UserName").closest('.control-group').removeClass('success');
        $("#UserName").closest('.control-group').addClass('error');
        $(".UserName > .error").text(label.vldengonly);
        $(".UserName > .error").css('text-align', 'left');
        checkError();
        result = false;
    }
    return result;
}
/*--------------------------checkEmail------------------------------*/
function checkEmails() {
    var result;
    if (checkEmailEng($('#Emails').val())) {
        $.ajax({
            url: GetUrl("Member/ValidateRegister"),
            data: { email: $('#Emails').val() },
            type: "POST",
            async: false,
            success: function (data) {
                if (!data) {
                    //OpenLoading(false);
                    $("#Emails").closest('.control-group').removeClass('success');
                    $("#Emails").closest('.control-group').addClass('error');
                    $(".Emails > .error").text(label.vldemail_exists);
                    $(".Emails > .error").css('text-align', 'left');
                    checkError();
                    result = false;
                } //end if
                else {
                    checkError();
                    result = true;
                    //OpenLoading(false);
                }
            },
            error: function () {
                // bootbox.alert(label.cannot_check_info);
            }
        });
    } else {
        $("#Emails").closest('.control-group').removeClass('success');
        $("#Emails").closest('.control-group').addClass('error');
        $(".Emails > .error").text(label.vldengonly);
        $(".Emails > .error").css('text-align', 'left');
        checkError();
        result = false;
    }
    return result;
}
/*-------------------------------checkCompName--------------------------------*/
function checkCompName() {
    var result;
    if (checkDisclaimer($('#CompName').val())) {
    $.ajax({
        url: GetUrl("Member/ValidateRegister"),
        data: { compname: $('#CompName').val() },
        type: "POST",
        async: false,
        success: function (data) {
            if (!data) {
                //OpenLoading(false);
                $("#CompName").closest('.control-group').removeClass('success');
                $("#CompName").closest('.control-group').addClass('error');
                $(".CompName > .error").text(label.vldcompname_exists);
                $(".CompName > .error").css('text-align', 'left');
                checkError();
                result = false;
            } //end if
            else {
                checkError();
                result = true;
               // OpenLoading(false);
              }
        },
        error: function () {
          //  bootbox.alert(label.cannot_check_info);
        }
    });
    } else {
        $("#CompName").closest('.control-group').removeClass('success');
        $("#CompName").closest('.control-group').addClass('error');
        $(".CompName > .error").text("กรุณากรอกข้อมูลให้ถูกต้อง ไม่ควรมีเครื่องหมาย \!@#$^฿|{}[]'\"<>=:;~");
        $(".CompName > .error").css('text-align', 'left');
        checkError();
        result = false;
    }
    return result;
}

function checkCaptcha() {
    var result;
    $.ajax({
        url: GetUrl("Member/ValidateRegister"),
        data: {
            captcha: $('#captcha').val(),
            captcha_id: $('#captcha_id').val()
        },
        type: "POST",
        async: false,
        success: function (data) {
            if (!data) {
                //OpenLoading(false);
                $("#captcha").closest('.control-group').removeClass('success');
                $("#captcha").closest('.control-group').addClass('error');
                $(".captcha > .error").text('คุณกรอกข้อมูลไม่ตรงกับตัวเลขด้านบน กรุณากรอกใหม่อีกครั้ง');
                $(".captcha > .error").css('text-align', 'left');
                checkError();
                result = false;
            } //end if
            else {
                $("#captcha").closest('.control-group').removeClass('error');
                $("#captcha").closest('.control-group').addClass('success');
                $(".captcha > .error").text('');
                checkError();
                result = true;
                // OpenLoading(false);
            }
        },
        error: function () {
            //  bootbox.alert(label.cannot_check_info);
        }
    });
    return result;
}
/*--------------------------checkDisplayName------------------------------*/
function checkDisplayName() {
    var result;
    if (checkDisclaimer($('#DisplayName').val())) {
    $.ajax({
        url: GetUrl("Member/ValidateRegister"),
        data: { displayname: $('#DisplayName').val() },
        type: "POST",
        async: false,
        success: function (data) {
            if (!data) {
                //OpenLoading(false);
                $("#DisplayName").closest('.control-group').removeClass('success');
                $("#DisplayName").closest('.control-group').addClass('error');
                $(".DisplayName > .error").text(label.vlddisplayname_exists);
                $(".DisplayName > .error").css('text-align', 'left');
                checkError();
                result = false;
            } //end if
            else {
                checkError();
                result = true;
                //OpenLoading(false); 
            }
        },
        error: function () {
          //  bootbox.alert(label.cannot_check_info);
        }
    });
    } else {
        $("#DisplayName").closest('.control-group').removeClass('success');
        $("#DisplayName").closest('.control-group').addClass('error');
        $(".DisplayName > .error").text("กรุณากรอกข้อมูลให้ถูกต้อง ไม่ควรมีเครื่องหมาย \!@#$^฿|{}[]'\"<>=:;~");
        $(".DisplayName > .error").css('text-align', 'left');
        checkError();
        result = false;
    }
    return result;
}
/*--------------------------checkError------------------------------*/
function checkError() {
    var result;
    var check = "";
    check = $("label.error").text();
    if (check != "") {
        $("#submit").attr('disabled', true);
        result = false;
    } else {
        $("#submit").attr('disabled', false);
        result = true;
    }
    return result;
}
/*--------------------------checkAgree------------------------------*/
function checkAgree() {
    var agree = $('#agree').val();
    if (agree == "false") {
        $('#agree').focus();
        bootbox.alert(label.vldAgreement);
        return false;
    }
    else {
        return true;
    }
}
/*---------------------------------------------------------------------------------*/
function OpenLoading(isLoad, img, obj) {
    if (isLoad == true) {
        if (img == null) {
            img = '<div class="icon-loader"></div>';
        } else {
            img = '<img src=\"' + img + '\" >';
        }
        if ($('#loading').length == 0) {
            if (obj == null || obj == undefined) {
                $('body').prepend('<div id="loading">&nbsp;</div><div id="imgloading" style="top: 350px; left: 779.5px;">' + img + '</div>');
            } else {
                obj.prepend('<div id="loading">&nbsp;</div><div id="imgloading" style="top: 350px; left: 779.5px;">' + img + '</div>');
            }
            $("#loading").css({ 'filter': 'alpha(opacity=50)', 'opacity': '0.5' });
            $("#imgloading").position({ my: "center", at: "center", of: "#loading" });
        }

    } else {
        $('#loading').remove(); $('#imgloading').remove();
    }
}

function registerMember() {
    var bool = false;
    bool = checkCaptcha();
    if (bool == true) {
        OpenLoading(true);
        var ServiceType = 0;
        var CompImgPath = "";
        for (var i = 1; i <= 14; i++) {
            if ($('#ServiceType' + i).attr("checked") == 'checked') {
                ServiceType = $('#ServiceType' + i).val();
            }
        }
        //console.log("ServiceType: " + ServiceType);
        data = {
            ServiceType: ServiceType,
            UserName: $("#UserName").val(),
            Password: $("#Password").val(),
            Email: $("#Emails").val(),
            CompName: $("#CompName").val(),
            BizTypeID: $("#BizTypeID").val(),
            BizTypeOther: $("#BizTypeOther").val(),
            FirstName: $("#FirstName_register").val(),
            LastName: $("#LastName").val(),
            Phone: $("#Phone").val(),
            ProvinceID: $("#ProvinceID").val(),
            DistrictID: $("#DistrictID").val(),
            captcha: $("#captcha").val(),
            captcha_id: $("#captcha_id").val()
        };
        $.ajax({
            url: GetUrl("Member/SignUp"),
            data: data,
            traditional: true,
            success: function (data) {
                if (data.IsResult) {
                    window.location = GetUrl("Member/SignUpPackage");
                } else {
                    OpenLoading(false);
                    bootbox.alert("ลงทะเบียนเรียบร้อยแล้ว");
                    window.location = GetUrl("Member/AfterSignUp");
                }
            },
            error: function () {
                OpenLoading(false);
                bootbox.alert("Register error");
                window.location = GetUrl("Member/AfterSignUp");
            },
            type: "post"

        });
    } else {
        bootbox.alert("คุณกรอกข้อมูลไม่ตรงกับตัวเลขด้านบน กรุณากรอกใหม่อีกครั้ง");
    }
}

$(function () {
    $('#register').click(function () {
        $('#signup_form').validate(
        {
            onkeydown: false,
            onkeyup: false,
            rules: {
                UserName: {
                    minlength: 4,
                    maxlength: 16,
                    required: true
                },
                Password: {
                    required: true,
                    minlength: 6,
                    maxlength: 20,
                    equalTo: "#ConfirmPassword"
                },
                ConfirmPassword: {
                    required: true,
                    minlength: 6,
                    maxlength: 20,
                    equalTo: "#Password"
                },
                Emails: {
                    required: true,
                    email: true
                },
                Phone: {
                    required: true,
                    minlength: 9
                },
                DisplayName: {
                    minlength: 4,
                    maxlength: 20,
                    required: true
                },
                CompName: {
                    minlength: 3,
                    maxlength: 50,
                    required: true
                },
                BizTypeOther: {
                    minlength: 4,
                    required: true
                },
                FirstName_register: {
                    required: true,
                    minlength: 3
                },
                LastName: {
                    required: true,
                    minlength: 3
                },
                BizTypeID: {
                    selectBiztype: 0
                },
                ProvinceID: {
                    selectProvince: 0
                },
                DistrictID: {
                    selectDistrict: 0
                },
                agree: {
                    checked: false
                },
                captcha: {
                    minlength: 4,
                    required: true
                }
            },
            messages: {
                UserName: {
                    minlength: label.vldmin_4_max_16char,
                    maxlength: label.vldmin_4_max_16char,
                    required: label.vldrequired
                },
                Password: {
                    minlength: label.vldmin_6_max_20char,
                    maxlength: label.vldmin_6_max_20char,
                    equalTo: label.vldsame_value,
                    required: label.vldrequired
                },
                ConfirmPassword: {
                    required: label.vldrequired,
                    minlength: label.vldmin_6_max_20char,
                    maxlength: label.vldmin_6_max_20char,
                    equalTo: label.vldsame_value
                },
                Emails: {
                    required: label.vldrequired,
                    email: label.vldfix_format_email
                },
                Phone: {
                    required: label.vldrequired,
                    minlength: label.vldless_9char
                },
                DisplayName: {
                    minlength: label.vldless_4char,
                    maxlength: label.vldmin_4_max_20char,
                    required: label.vldrequired
                },
                CompName: {
                    minlength: label.vldless_4char,
                    maxlength: label.vldmore_50char,
                    required: label.vldrequired
                },
                BizTypeOther: {
                    minlength: label.vldless_4char,
                    required: label.vldrequired
                },
                FirstName_register: {
                    required: label.vldrequired,
                    minlength: label.vldless_3char
                },
                LastName: {
                    required: label.vldrequired,
                    minlength: label.vldless_3char
                },
                BizTypeID: {
                    selectBiztype: label.vldselectbiztype
                },
                ProvinceID: {
                    selectProvince: label.vldselectprovince
                },
                DistrictID: {
                    selectDistrict: label.vldselectdistrict
                },
                agree: {
                    checked: label.vldrequired,
                    selectAgree: label.vldagree
                },
                captcha: {
                    minlength: label.vldless_4char,
                    required: label.vldrequired
                }
            },
            highlight: function (label) {
                $(label).closest('.control-group').removeClass('success');
                $(label).closest('.control-group').addClass('error');
                $(label).closest('.control-group').css('text-align', 'left');

            },
            success: function (label) {
                label.closest('.control-group').removeClass('error');
                label.closest('.control-group').addClass('success');
            }
        });

        if ($('#signup_form').valid()) {
            var Agree = checkAgree();
            var isAgree = $('#agree').val();

            if (Agree) {
                registerMember();
            }

            if (isAgree == "false") {
                bool = false;
            }
            
        }
    });

if( $("#ProvinceID").val()!=0){
    //GetDistrictByProvince($("#ProvinceID").val(), 0, "DistrictID");
    $('#DistrictID').DistrictByProvince({ province: $("#ProvinceID").val(), district: 0 });
}
    //$('#QapTcha').QapTcha({});

    $('#myModal1').modal({
        show: false
    });
    $('#myModal2').modal({
        show: false
    });

    $("#agree").click(function () {
        if ($(this).val() == 'false') {
            $(this).val(true);
        } else {
            $(this).val(false);
        }
    })
    $("#agree_con").click(function () {

        $("#agree").val(true);
        $("#agree").attr('checked', true);
        $("#agree").attr('disabled', 'disabled');
        $('#myModal1').modal('hide');
    })
    $("#BizTypeID").change(function () {
        if ($(this).val() == 13) {
            $(".BizTypeOther").show();
        }
        else {
            $(".BizTypeOther").val("");
            $(".BizTypeOther").hide();
        }
    });

    $("#UserName,#Password,#ConfirmPassword").keydown(function (e) {
        if (e.which == 32) {
            return false;
        }
    })
    /*--------------------------validate--------------------------------*/

    $('#signup_form').validate(
             {
                 onkeydown: false,
                 onkeyup: false,
                 rules: {
                     UserName: {
                         minlength: 4,
                         maxlength: 16,
                         required: true
                     },
                     Password: {
                         required: true,
                         minlength: 6,
                         maxlength:20,
                         equalTo: "#ConfirmPassword"
                     },
                     ConfirmPassword: {
                         required: true,
                         minlength: 6,
                         maxlength:20,
                         equalTo: "#Password"
                     },
                     Emails: {
                         required: true,
                         email: true
                     },
                     Phone: {
                        required: true,
                         minlength: 9
                     },
                     DisplayName: {
                         minlength: 4,
                         maxlength: 20,
                         required: true
                     },
                     CompName: {
                         minlength: 3,
                         maxlength: 50,
                         required: true
                     },
                     BizTypeOther: {
                         minlength: 4,
                         required: true
                     },
                     FirstName_register: {
                         required: true,
                         minlength: 3
                     },
                     LastName: {
                         required: true,
                         minlength: 3
                     },
                     BizTypeID: {
                         selectBiztype: 0
                     },
                     ProvinceID: {
                         selectProvince: 0
                     },
                     DistrictID: {
                         selectDistrict: 0
                     },
                     agree: {
                         checked:false
                     },
                     captcha: {
                         minlength: 4,
                         required: true
                     }
                 },
                 messages: {
                     UserName: {
                         minlength: label.vldmin_4_max_16char,
                         maxlength: label.vldmin_4_max_16char,
                         required: label.vldrequired
                     },
                     Password: {
                         minlength: label.vldmin_6_max_20char,
                         maxlength: label.vldmin_6_max_20char,
                         equalTo: label.vldsame_value,
                         required: label.vldrequired
                     },
                     ConfirmPassword: {
                         required: label.vldrequired,
                         minlength: label.vldmin_6_max_20char,
                         maxlength: label.vldmin_6_max_20char,
                         equalTo: label.vldsame_value
                     },
                     Emails: {
                         required: label.vldrequired,
                         email: label.vldfix_format_email
                     },
                     Phone: {
                         required: label.vldrequired,
                         minlength: label.vldless_9char
                     },
                     DisplayName: {
                         minlength: label.vldless_4char,
                         maxlength: label.vldmin_4_max_20char,
                         required: label.vldrequired
                     },
                     CompName: {
                         minlength: label.vldless_4char,
                         maxlength: label.vldmore_50char,
                         required: label.vldrequired
                     },
                     BizTypeOther: {
                         minlength: label.vldless_4char,
                         required: label.vldrequired
                     },
                     FirstName_register: {
                         required: label.vldrequired,
                         minlength: label.vldless_3char
                     },
                     LastName: {
                         required: label.vldrequired,
                         minlength: label.vldless_3char
                     },
                     BizTypeID: {
                         selectBiztype: label.vldselectbiztype
                     },
                     ProvinceID: {
                         selectProvince: label.vldselectprovince
                     },
                     DistrictID: {
                         selectDistrict: label.vldselectdistrict
                     },
                     agree: {
                         checked: label.vldrequired,
                         selectAgree: label.vldagree
                     },
                     captcha: {
                         minlength: label.vldCaptcha,
                         required: label.vldrequired
                     }
                 },
                 highlight: function (label) {
                     $(label).closest('.control-group').removeClass('success');
                     $(label).closest('.control-group').addClass('error');
                     $(label).closest('.control-group').css('text-align', 'left');

                 },
                 success: function (label) {
                     label.closest('.control-group').removeClass('error');
                     label.closest('.control-group').addClass('success');
                 }
             });
             ///*---------------------------------------------------------------------------------------------*/
             //$.validator.addMethod("WebIDRequired", function (value, element, arg) {
             //    return arg != value;
             //}, "เลือกบัญชี");

             //$('#signup2_form').validate(
             //            {
             //                rules: {
             //                    User: {
             //                        required: true,
             //                        minlength: 4,
             //                        maxlength: 16
             //                    },
             //                    Pass: {
             //                        required: true,
             //                        minlength: 6,
             //                        maxlength:20
             //                    },
             //                    WebID: {
             //                        WebIDRequired: 0

             //                    }
             //                },
             //                Message: {
             //                    User: {
             //                        required: label.vldrequired,
             //                        minlength: label.vldmin_4_max_16char,
             //                        maxlength: label.vldmin_4_max_16char
             //                    },
             //                    Pass: {
             //                        required: label.vldrequired,
             //                        maxlength: label.vldmin_6_max_20char,
             //                        minlength: label.vldmin_6_max_20char
             //                    },
             //                    WebID: {
             //                        WebIDRequired: 0

             //                    }
             //                },
             //                highlight: function (label) {
             //                    $(label).closest('.control-group').removeClass('success');
             //                    $(label).closest('.control-group').addClass('error');
             //                    $(label).closest('.control-group').css('text-align', 'left');

             //                },
             //                success: function (label) {
             //                    label.closest('.control-group').removeClass('error');
             //                    label.closest('.control-group').addClass('success');
             //                }
             //            });
/*-----------------------------------ChangeProvince-------------------------------------*/
    $("#ProvinceID").change(function () {
        $('#DistrictID').DistrictByProvince({ province: $("#ProvinceID").val(), district: 0 })

    });


});

//(function ($) {
//    $.fn.extend({
//        infoslider: function (options) {

//            //default values for plugin options
//            var defaults = {
//                interval: 10000,
//                duration: 50,
//                durationin: 5000,
//                lineheight: 1,
//                height: 'auto', //reserved
//                hoverpause: false,
//                pager_banner: false,
//                nav: true, //reserved
//                keynav: true
//            }
//            var options = $.extend(defaults, options);

//            return this.each(function () {
//                var o = options;
//                var obj = $(this);

//                //store the slide and pager_banner li
//                var slides = $('.slides li', obj);
//                var pager_banner = $('.pager_banner li', obj);
//                var len = slides.length - 1;
//                if (len <= 0) { len = 1; }
//                var ran = Math.floor((Math.random() * len));
//                //console.log('ran : ' + ran);

//                //set initial current and next slide index values
//                var current = 0;
//                current = ran;
//                var next = current + 1;

//                //get height and width of initial slide image and calculate size ratio
//                var imgHeight = slides.eq(current).find('img').height();
//                var imgWidth = slides.eq(current).find('img').width();
//                var imgRatio = imgWidth / imgHeight;

//                //define vars for setsize function
//                var sliderWidth = 0;
//                var cropHeight = 0;

//                //hide all slides, fade in the first, add active class to first slide
//                slides.hide().eq(current).fadeIn(o.durationin).addClass('active');


//                //build pager_banner if it doesn't already exist and if enabled
//                if (pager_banner.length) {
//                    pager_banner.eq(current).addClass('active');
//                } else if (o.pager_banner) {
//                    obj.append('<ul class="pager_banner"></ul>');
//                    slides.each(function (index) {
//                        $('.pager_banner', obj).append('<li><a href="#"><span>' + pagername[index] + '</span></a></li>')
//                    });
//                    pager_banner = $('.pager_banner li', obj);
//                    pager_banner.eq(current).addClass('active');
//                }

//                //rotate to selected slide on pager_banner click
//                if (pager_banner) {
//                    $('a', pager_banner).click(function () {
//                        //stop the timer
//                        clearTimeout(obj.play);
//                        //set the slide index based on pager_banner index
//                        next = $(this).parent().index();
//                        //rotate the slides
//                        rotate();
//                        return false;
//                    });
//                }

//                //primary function to change slides
//                var rotate = function () {
//                    //fade out current slide and remove active class,
//                    //fade in next slide and add active class
//                    slides.eq(current).fadeOut(o.duration).removeClass('active')
//						.end().eq(next).fadeIn(o.durationin).addClass('active').queue(function () {
//						    //add rotateTimer function to end of animation queue
//						    //this prevents animation buildup caused by requestAnimationFrame
//						    //rotateTimer starts a timer for the next rotate
//						    rotateTimer();
//						    $(this).dequeue()
//						});

//                    //update pager_banner to reflect slide change
//                    if (pager_banner) {
//                        pager_banner.eq(current).removeClass('active')
//							.end().eq(next).addClass('active');
//                    }

//                    //update current and next vars to reflect slide change
//                    //set next as first slide if current is the last
//                    current = next;
//                    next = current >= slides.length - 1 ? 0 : current + 1;
//                };
//                //create a timer to control slide rotation interval
//                var rotateTimer = function () {
//                    obj.play = setTimeout(function () {
//                        //trigger slide rotate function at end of timer
//                        rotate();
//                    }, o.interval);
//                };
//                //start the timer for the first time
//                rotateTimer();

//                //pause the slider on hover
//                //disabled by default due to bug
//                if (o.hoverpause) {
//                    slides.hover(function () {
//                        //stop the timer in mousein
//                        clearTimeout(obj.play);
//                    }, function () {
//                        //start the timer on mouseout
//                        rotateTimer();
//                    });
//                }

//                //calculate and set height based on image width/height ratio and specified line height
//                var setsize = function () {
//                    sliderWidth = $('.slides', obj).width();
//                    cropHeight = Math.floor(((sliderWidth / imgRatio) / o.lineheight)) * o.lineheight;

//                    $('.slides', obj).css({ height: cropHeight });
//                };
//                setsize();

//                //bind setsize function to window resize event
//                $(window).resize(function () {
//                    setsize();
//                });



//                //Add keyboard navigation

//                if (o.keynav) {
//                    $(document).keyup(function (e) {

//                        switch (e.which) {

//                            case 39: case 32: //right arrow & space

//                                clearTimeout(obj.play);

//                                rotate();

//                                break;


//                            case 37: // left arrow
//                                clearTimeout(obj.play);
//                                next = current - 1;
//                                rotate();

//                                break;
//                        }

//                    });
//                }


//            });
//        }
//    });
//})(jQuery);


