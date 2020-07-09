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
            txtLock: label.vldlocked,
            txtUnlock: label.vldunlocked,
            txtAgreement: label.vldAgreement
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
				Icons = jQuery('<div>', { id: 'Icons' }),
				TxtStatus = jQuery('<div>', { id: 'TxtStatus', 'class': 'dropError', text: opts.txtLock }),
                TxtStatus1 = jQuery('<div>', { id: 'TxtStatus', 'class': 'dropError', text: opts.txtAgreement }),
                TxtStatus2 = jQuery('<div>', { id: 'TxtStatus', 'class': 'dropError', text: "**กรุณาเลื่อนแถบไปด้านขวา เพื่อปลดล๊อค**" }),
				inputQapTcha = jQuery('<input>', { id: 'iQapTcha', name: 'iQapTcha', value: generatePass(), type: 'hidden' });

                bgSlider.appendTo($this);
                Icons.insertAfter(bgSlider);
                Clr.insertAfter(Icons);
                TxtStatus.insertAfter(Clr);
                TxtStatus1.insertAfter(Clr);
                TxtStatus2.insertAfter(Clr);
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
                                        if ($('#signup_form').valid()) {
                                            var UserName = checkUserName();
                                            var Email = checkEmails();
                                            var CompName = checkCompName();
                                            var DisplayName = checkDisplayName();
                                            var Agree = checkAgree();
                                            if (UserName && Email && CompName && DisplayName && Agree) {
                                                Slider.draggable('disable').css('cursor', 'default');
                                                inputQapTcha.val("");
                                                TxtStatus.text(opts.txtUnlock).addClass('dropSuccess').removeClass('dropError');
                                                TxtStatus1.text("");
                                                TxtStatus2.text("");
                                                Icons.css('background-position', '-16px 0');

                                                $('.submit').removeAttr("disabled");
                                                $('.submit').removeClass("disable");
                                                $('.submit_nextsteps').removeAttr('disabled');
                                            }
                                            else {
                                                var strValid = "";
                                                if(!UserName){
                                                    strValid += label.vldusername_exists;
                                                }
                                                if(!Email){
                                                    strValid += " and " + label.vldemail_exists;
                                                }
                                                if(!CompName){
                                                    strValid += " and " + label.vldcompname_exists;
                                                }
                                                if (!DisplayName) {
                                                    strValid += " and " + label.vlddisplayname_exists;
                                                }
                                                if (!Agree) {
                                                    strValid += " and " + label.vldagree;
                                                }
                                                $('#Slider').attr('style', 'position: relative; left: 0px;');
                                                //bootbox.alert("UserName หรือ Email หรือ Company Name มีผู้ใช้แล้ว");
                                            }
                                        }
                                        else {
                                            $('#Slider').attr('style', 'position: relative; left: 0px;');
                                            //bootbox.alert(label.vldrequired);
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

