/************************************************************************
*************************************************************************
@Name :       	QapTcha - jQuery Plugin
@Revison :    	2.5
@Date : 		26/01/2011
@Author:     	 Surrel Mickael (www.myjqueryplugins.com - www.msconcept.fr) 
@License :		 Open Source - MIT License : http://www.opensource.org/licenses/mit-license.php
 
**************************************************************************
*************************************************************************/
jQuery.QapTcha = {
    build: function (options) {

        var defaults = {
            txtLock: 'กรุณาเลื่อนแถบไปด้านขวา เพื่อปลดล๊อค',
            txtUnlock: 'กดปุ่ม "ส่งข้อความ"'
        };

        if (this.length > 0)
            return jQuery(this).each(function (i) {
                /** Vars **/
                var
				opts = $.extend(defaults, options),
				$this = $(this),
				form = $('form').has($this),
				Clr = jQuery('<div>', { 'class': 'clr' }),
				bgSlider = jQuery('<div>', { id: 'bgSlider' }),
				Slider = jQuery('<div>', { id: 'Slider' }),
				Icons = jQuery('<div>', { id: '' }),
				TxtStatus = jQuery('<div>', { id: 'SlideSendMessage', 'class': 'dropError', text: opts.txtLock }),
				inputQapTcha = jQuery('<input>', { id: 'iQapTcha', name: 'iQapTcha', value: generatePass(), type: 'hidden' });

                bgSlider.appendTo($this);
                Icons.insertAfter(bgSlider);
                Clr.insertAfter(Icons);
                TxtStatus.insertAfter(Clr);
                inputQapTcha.appendTo($this);
                Slider.appendTo(bgSlider);
                $this.show();

                Slider.draggable({
                    containment: bgSlider,
                    axis: 'x',
                    stop: function (event, ui) {
                        if (ui.position.left > 153) {
                            $.ajax({
                                url: GetUrl("Member/CheckCaptchaSession"),
                                data: { password: 'B2BThaiCaptcha' },
                                success: function (data) {
                                    if (data == 1) {
                                        if ($('#txtFromName').val() != '' && $('#txtFromEmail').val() != '') {
                                            $("#MsgProductDetail").val(tinyMCE.get('MsgProductDetail').getContent());
                                            //SendMail($(".ProID").text());
                                            $("#unlockMsg").removeAttr('disabled', 'disabled');
                                            TxtStatus.text(opts.txtUnlock).addClass('dropSuccess').removeClass('dropError');
                                        }
                                        else {
                                            $('#Slider').attr('style', 'position: relative; left: 0px;');
                                            bootbox.alert(label.vldrequired);
                                        }
                                    }
                                },
                                error: function () {
                                    bootbox.alert('error');
                                    return false;
                                },
                                type: "POST"

                            });
                        }
                    }
                });

                function generatePass() {
                    var chars = 'azertyupqsdfghjkmwxcvbn23456789AZERTYUPQSDFGHJKMWXCVBN';
                    var pass = '';
                    for (i = 0; i < 10; i++) {
                        var wpos = Math.round(Math.random() * chars.length);
                        pass += chars.substring(wpos, wpos + 1);
                    }
                    return pass;
                }

                

            });
    }
}; jQuery.fn.QapTcha = jQuery.QapTcha.build;



$('#SendMsg').click(function () {
    $('#DetailCompForm').validate(
    {
        rules: {
            txtFromName: {
                required: true
            },
            txtFromEmail: {
                required: true,
            },
            txtSubject: {
                required: true
            },
            MsgDetail: {
                required: true
            },
            txtFromContactPhone: {
                required: true
            },
            captcha: {
                minlength: 4,
                required: true
            }
        },
        messages: {
            txtFromName: {
                required: label.vldfill_yourname
            },
            txtFromContactPhone: {
                required: label.vldfill_youremail
            },
            txtSubject: {
                required: label.vldenter_subject
            },
            MsgDetail: {
                required: label.vldfill_yourmessage
            },
            txtFromContactPhone: {
                required: label.vldfill_yourmessage
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
            $(label).closest('label .error').css('margin-left', '0px');
            OpenLoading(false);
        },
        success: function (label) {
            label.closest('.control-group').removeClass('error');
            label.closest('.control-group').addClass('success');
        }
    });

    if ($('#DetailCompForm').valid()) {
        SendMail();
    }
});

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
                result = false;
            } //end if
            else {
                $("#captcha").closest('.control-group').removeClass('error');
                $("#captcha").closest('.control-group').addClass('success');
                $(".captcha > .error").text('');
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



function SendMail() {
    var bool = false;
    var editorContent = tinyMCE.get('MsgProductDetail').getContent(); //get value from html editor

    bool = checkCaptcha();
    if (bool == true) {
        OpenLoading(true);
        var Importance = false;
        if ($('input[name=ChkImportance]:checked').attr("checked") == true || $('input[name=ChkImportance]:checked').attr("checked") == "checked") {
            Importance = true;
        }
        //Slider.draggable('disable').css('cursor', 'default');
        //inputQapTcha.val("");
        //TxtStatus.text(opts.txtUnlock).addClass('dropSuccess').removeClass('dropError');
        //Icons.css('background-position', '-16px 0');
        $.ajax({
            type: "POST",
            url: GetUrl("Message/ContactSupplier"),
            data: AddAntiForgeryToken({
                ToCompID: $('#hidToCompID').val(),
                FromCompID: $('#txtFromId').val(),
                //FromCompID: $('#txtFromName').attr("Class"),
                FromName: $("#txtFromName").val(),
                FromEmail: $("#txtFromEmail").val(),
                Subject: $('#txtSubject').val(),
                MsgDetail: editorContent,
                IsImportance: Importance,
                ProductID: $('#ProductID').val(),
                FromContactPhone: txtFromContactPhone,
                captcha: $("#captcha").val(),
                captcha_id: $("#captcha_id").val()
            }),
            success: function (data) {
                $('#ProductDetail').html(data);
                OpenLoading(false);
                bootbox.alert(label.vldsend_success);
                //$('html, body').animate({ scrollTop: 0 }, 'slow');
                document.location.reload(true);

            }
        });
    } else {
        bootbox.alert("คุณกรอกข้อมูลไม่ตรงกับตัวเลขด้านบน กรุณากรอกใหม่อีกครั้ง");
    }
}
