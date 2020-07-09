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
        var count = 0;
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
                                            $("#MsgDetail").val(tinyMCE.get('MsgDetail').getContent());
                                            
                                            $("#unlockMsg").removeAttr('disabled', 'disabled');
                                           // $('#ContactCompForm').submit();
                                            $(".Checkfield").each(function () {
                                                if($(this).hasClass('error')){
                                                    count += 1;
                                                }
                                            });
                                            if (count == 0) {
                                                //OpenLoading(true);
                                                Slider.draggable('disable').css('cursor', 'default');
                                                inputQapTcha.val("");
                                                TxtStatus.text(opts.txtUnlock).addClass('dropSuccess').removeClass('dropError');
                                                Icons.css('background-position', '-16px 0');
                                            } else {
                                                $('#Slider').attr('style', 'position: relative; left: 0px;');
                                            }
                                        }
                                        else {
                                            $('#Slider').attr('style', 'position: relative; left: 0px;');
                                            if ($("#ContactCompForm").valid()) {
                                                return true;
                                            }
                                            else { return false; }
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