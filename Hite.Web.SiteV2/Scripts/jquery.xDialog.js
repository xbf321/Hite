var X = {
    namespace: function () {
        var a = arguments, o = null, i, j, d;
        for (i = 0; i < a.length; i++) {
            d = a[i].split('.');
            o = X;
            for (j = (d[0] == 'X') ? 1 : 0; j < d.length; j++) {
                o[d[j]] = o[d[j]] || {};
                o = o[d[j]];
            }
        }
        return o;
    }
};
X.namespace('UI');
X.namespace('Util');
X.namespace('App');
X.namespace('Page');
X.namespace('Config');
(function () {
    X.UI.Dialog = function (parms) {
        this._parms = $.extend({
            appendTo: 'body',
            dialogId: 'x_dialog',
            zIndex: 1000,
            opacity: 50,
            position: ['20%'],
            width: '500px'
        }, parms);
        this._buttons = [];
        this._ie6 = $.browser.msie && parseInt($.browser.version) == 6 && typeof window['XMLHttpRequest'] != "object",
		this._ieQuirks = null,
		this._w = [];
        this._iframe = null;
        this._overlay = null;
        this._container = null;
        this._contentFrame = null;
        this._create();
    };
    X.UI.Dialog.prototype = $.extend(X.UI.Dialog.prototype, {
        _createOverlay: function () {
            this._overlay = $('<div style="background-color:#000000;"/>')
			.attr('id', this._parms.dialogId + '_overlay')
			.addClass('pop_dialog_overlay')
			.css($.extend({}, {
			    display: 'none',
			    opacity: this._parms.opacity / 100,
			    height: this._w[0],
			    width: this._w[1],
			    position: 'fixed',
			    top: 0,
			    left: 0,
			    zIndex: this._parms.zIndex + 1
			})).appendTo(this._parms.appendTo);

        },
        _createContainer: function () {
            this._container = $('<div></div>')
				.attr('id', this._parms.dialogId + '_container')
				.addClass('pop_dialog_container')
				.css($.extend(this._parms.containerCss, {
				    display: 'none',
				    position: 'fixed',
				    zIndex: this._parms.zIndex + 2,
				    width: this._parms.width
				}))
				.appendTo(this._parms.appendTo);
        },
        _createContentFrame: function () {
            this._contentFrame = $([
                '<table style="width: 100%; height: 100%;" class="pop_dialog_table">',
                    '<tbody>',
                        '<tr>',
                            '<td class="pop_topleft"></td>',
                            '<td class="pop_border"></td>',
                            '<td class="pop_topright"></td>',
                        '</tr>',
                        '<tr>',
                            '<td class="pop_border"></td>',
                            '<td class="pop_content">',
                                '<h2><span id="' + this._parms.dialogId + '_header"></span></h2>',
                                '<div class="dialog_content">',
                                    '<div id="' + this._parms.dialogId + '_body" class="dialog_body"></div>',
                                    '<div id="' + this._parms.dialogId + '_footer" class="dialog_buttons"></div>',
                                '</div>',
                            '</td>',
                            '<td class="pop_border"></td>',
                        '</tr>',
                        '<tr>',
                            '<td class="pop_bottomleft"></td>',
                            '<td class="pop_border"></td>',
                            '<td class="pop_bottomright"></td>',
                        '</tr>',
                        '</tbody>',
                    '</table>'
            ].join('')).appendTo(this._container);
        },
        _create: function () {
            this._w = this._getDimensions();
            var s = this;
            if (this._ie6) {
                this._iframe = $('<iframe src="javascript:false;"></iframe>')
					.css($.extend({}, {
					    display: 'none',
					    opacity: 0,
					    position: 'fixed',
					    height: s._w[0],
					    width: s._w[1],
					    zIndex: s._parms.zIndex - 1,
					    top: 0,
					    left: 0
					}))
					.appendTo(this._parms.appendTo);
            }
            this._createOverlay();
            this._createContainer();
            this._createContentFrame();
            if (s._ie6 || s._ieQuirks) {
                s._fixIE();
            }
        },
        show: function () {
            this._iframe && this._iframe.show();
            this._setContainerDimensions();
            this._overlay.show();
            this._container.show();
            this._bindEvents();
        },
        addButton: function (button, callback) {
            this._buttons.push(button);
            button.registerEvent('click', this, callback);
            var f = $('#' + this._parms.dialogId + '_footer').append(button.jqueryObj);
        },
        removeButton: function (button) {
            for (var i = 0; i < this._buttons.length; i++) {
                if (button == this._buttons[i]) {
                    this._buttons.splice(i, 1);
                    button.jqueryObj.remove();
                    break;
                }
            }
        },
        setHeader: function (headerContent) {
            var h = $("#" + this._parms.dialogId + "_header")
            h.empty();
            h.append(headerContent);
        },
        setBody: function (bodyContent) {
            var b = $("#" + this._parms.dialogId + "_body");
            b.empty();
            b.append(bodyContent);
        },
        close: function () {
            this._unbindEvents();
            this._container.hide().remove();
            this._overlay.hide().remove();
            this._iframe && this._iframe.hide().remove();
        },
        /*
        * Fix issues in IE6 and IE7 in quirks mode
        */
        _fixIE: function () {
            p = this._parms.position;
            // simulate fixed position - adapted from BlockUI
            this.modal = true;
            $.each([this._iframe || null, !this.modal ? null : this._overlay, this._container], function (i, el) {
                if (el) {
                    var bch = 'document.body.clientHeight', bcw = 'document.body.clientWidth',
						bsh = 'document.body.scrollHeight', bsl = 'document.body.scrollLeft',
						bst = 'document.body.scrollTop', bsw = 'document.body.scrollWidth',
						ch = 'document.documentElement.clientHeight', cw = 'document.documentElement.clientWidth',
						sl = 'document.documentElement.scrollLeft', st = 'document.documentElement.scrollTop',
						s = el[0].style;
                    s.position = 'absolute';
                    if (i < 2) {
                        s.removeExpression('height');
                        s.removeExpression('width');
                        s.setExpression('height', '' + bsh + ' > ' + bch + ' ? ' + bsh + ' : ' + bch + ' + "px"');
                        s.setExpression('width', '' + bsw + ' > ' + bcw + ' ? ' + bsw + ' : ' + bcw + ' + "px"');
                    }
                    else {
                        var te, le;
                        if (p && p.constructor == Array) {
                            var top = p[0]
								? typeof p[0] == 'number' ? p[0].toString() : p[0].replace(/px/, '')
								: el.css('top').replace(/px/, '');
                            te = top.indexOf('%') == -1
								? top + ' + (t = ' + st + ' ? ' + st + ' : ' + bst + ') + "px"'
								: parseInt(top.replace(/%/, '')) + ' * ((' + ch + ' || ' + bch + ') / 100) + (t = ' + st + ' ? ' + st + ' : ' + bst + ') + "px"';
                            if (p[1]) {
                                var left = typeof p[1] == 'number' ? p[1].toString() : p[1].replace(/px/, '');
                                le = left.indexOf('%') == -1
									? left + ' + (t = ' + sl + ' ? ' + sl + ' : ' + bsl + ') + "px"'
									: parseInt(left.replace(/%/, '')) + ' * ((' + cw + ' || ' + bcw + ') / 100) + (t = ' + sl + ' ? ' + sl + ' : ' + bsl + ') + "px"';
                            }
                        }
                        else {
                            te = '(' + ch + ' || ' + bch + ') / 2 - (this.offsetHeight / 2) + (t = ' + st + ' ? ' + st + ' : ' + bst + ') + "px"';
                            le = '(' + cw + ' || ' + bcw + ') / 2 - (this.offsetWidth / 2) + (t = ' + sl + ' ? ' + sl + ' : ' + bsl + ') + "px"';
                        }
                        s.removeExpression('top');
                        s.removeExpression('left');
                        s.setExpression('top', te);
                        s.setExpression('left', le);
                    }
                }
            });
        },
        _getDimensions: function () {
            var el = $(window);
            // fix a jQuery/Opera bug with determining the window height
            var h = $.browser.opera && $.browser.version > '9.5' && $.fn.jquery <= '1.2.6' ? document.documentElement['clientHeight'] :
				$.browser.opera && $.browser.version < '9.5' && $.fn.jquery > '1.2.6' ? window.innerHeight :
				el.height();
            return [h, el.width()];
        },
        _getVal: function (v) {
            return v == 'auto' ? 0
				: v.indexOf('%') > 0 ? v
					: parseInt(v.replace(/px/, ''));
        },
        _setContainerDimensions: function (resize) {
            var s = this;

            if (!resize) {
                // get the dimensions for the container and data
                var ch = $.browser.opera ? s._container.height() : s._getVal(s._container.css('height')),
					cw = $.browser.opera ? s._container.width() : s._getVal(s._container.css('width')),
					dh = s._contentFrame.outerHeight(true), dw = s._contentFrame.outerWidth(true);

                var mh = s.maxHeight && s.maxHeight < s._w[0] ? s.maxHeight : s._w[0],
					mw = s.maxWidth && s.maxWidth < s._w[1] ? s.maxWidth : s._w[1];
                // height
                if (!ch) {
                    if (!dh) { ch = s.minHeight; }
                    else {
                        if (dh > mh) { ch = mh; }
                        else if (dh < s.minHeight) { ch = s.minHeight; }
                        else { ch = dh; }
                    }
                }
                else {
                    ch = ch > mh ? mh : ch;
                }
                // width
                if (!cw) {
                    if (!dw) { cw = s.minWidth; }
                    else {
                        if (dw > mw) { cw = mw; }
                        else if (dw < s.minWidth) { cw = s.minWidth; }
                        else { cw = dw; }
                    }
                }
                else {
                    cw = cw > mw ? mw : cw;
                }

                s._container.css({ height: ch, width: cw });
                if (dh > ch || dw > cw) {
                    s._container.css({ overflow: 'auto' });
                }
            }
            s._setPosition();

        },
        _setPosition: function () {
            var s = this, top, left,
				hc = (s._w[0] / 2) - (s._container.outerHeight(true) / 2),
				vc = (s._w[1] / 2) - (s._container.outerWidth(true) / 2);

            if (s._parms.position && Object.prototype.toString.call(s._parms.position) === "[object Array]") {
                top = s._parms.position[0] || hc;
                left = s._parms.position[1] || vc;
            } else {
                top = hc;
                left = vc;
            }
            s._container.css({ left: left, top: top });
        },
        _bindEvents: function () {
            var s = this;
            // update window size
            $(window).bind('resize', function () {
                // redetermine the window width/height
                s._w = s._getDimensions();
                // reposition the dialog
                s._setContainerDimensions(true);

                if (s._ie6) {
                    s._fixIE();
                }
                else {
                    // update the iframe & overlay
                    s._iframe && s._iframe.css({ height: s._w[0], width: s._w[1] });
                    s._overlay.css({ height: s._w[0], width: s._w[1] });
                }
            });
        },
        _unbindEvents: function () {
            $(window).unbind('resize');
        }

    });
})();
(function () {
    X.UI.Button = function (html) {
        this.jqueryObj = $(html);
    };
    X.UI.Button.prototype = $.extend(X.UI.Button.prototype, {
        registerEvent: function (eventName, callbackObj, callbackFuc) {
            if (typeof callbackFuc != 'undefined') {
                this.jqueryObj.bind(eventName, function () {
                    callbackFuc.call(callbackObj);
                });
            }
        }
    });
})();
(function () {
    X.UI.TabView = function (parms) {
        parms = $.extend({
            selectedClass: 'on',
            noSelectedClass: '',
            event: 'mouseover',
            mouseOverDelay: 0.2,
            defaultSelectID: ""
        }, parms);
        this._currentTab = null;
        this._tabs = [];
        this._parms = parms;
    };
    X.UI.TabView.prototype = $.extend(X.UI.TabView.prototype, {
        addTab: function (t) {
            t = $.extend({
                labelID: '',
                contentID: ''
            }, t);
            this._tabs.push(t);
            var This = this;
            if (This._parms.event == 'mouseover') {
                var isMouseOn = true;
                var timer = null;
                $("#" + t.labelID).bind("mouseover", function (e) {
                    //if ( !isMouseOn ) return;
                    var f = function () { This.setCurrentTab(t); };
                    timer = setTimeout(f, This._parms.mouseOverDelay * 1000);
                }).bind("mouseleave", function (e) {
                    isMouseOn = false;
                    if (timer) clearTimeout(timer);
                });
            }
            if (this._currentTab == null && this._tabs.length > 0) {
                this.setCurrentTab(this._tabs[0]);
            }
        },
        setCurrentTab: function (t) {
            if (this._currentTab != null && t.labelID != this._currentTab.labelID) {
                $("#" + this._currentTab.labelID).removeClass(this._parms.selectedClass);
                if (this._parms.noSelectedClass != '') {
                    $("#" + this._currentTab.labelID).addClass(this._parms.noSelectedClass);
                }
                $("#" + this._currentTab.contentID).css("display", "none");
            }
            this._currentTab = t;
            $("#" + t.labelID).addClass(this._parms.selectedClass);
            if (this._parms.noSelectedClass != '')
                $("#" + t.labelID).removeClass(this._parms.noSelectedClass);
            $("#" + t.contentID).css("display", "block");
        }
    });

})();