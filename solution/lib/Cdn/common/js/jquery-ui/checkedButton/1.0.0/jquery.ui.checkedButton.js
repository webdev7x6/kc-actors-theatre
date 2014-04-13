/*!
* jQuery UI Checked Button plugin, v 1.0.0
*
* Enhances the jQuery UI button() method on checkboxes and radio buttons.
*
* Copyright (c) 2012 Clickfarm Interactive, Inc.
*/
(function ($) {
    var methods = {
        init: function init(options) {
            var opts = $.extend(true, {}, options);
            if (!opts.icons) {
                opts.icons = {};
            }
            opts.icons.primary = null;

            return this.each(function each() {
                var this$ = $(this),
                    type = null,
                    defaultIcons,
                    data,
                    btnData
                ;

                //validate button type
                if (this$.is(':checkbox')) {
                    type = 'checkbox';
                    defaultIcons = $.fn.checkedButton.defaults.checkboxIcons;
                }
                else if (this$.is(':radio')) {
                    type = 'radio';
                    defaultIcons = $.fn.checkedButton.defaults.radioIcons;
                }
                if (!type) {
                    $.error('Invalid button type');
                    return;
                }

                //initialize button
                data = this$.data('checkedButton');
                if (!data) {
                    //set button icon options
                    if (typeof opts.icons.checked === 'undefined') {
                        opts.icons.checked = defaultIcons.checked;
                    }
                    if (typeof opts.icons.unchecked === 'undefined') {
                        opts.icons.unchecked = defaultIcons.unchecked;
                    }

                    if (this$.data('button')) {
                        this$.button('option', opts);
                    }
                    else {
                        this$.button(opts);
                    }

                    this$
                        .bind('click.checkedButton', onCheckToggle)
                        .data('checkedButton', {
                            init: true
                        })
                    ;
                    onCheckToggle.call(this$);
                }
            });
        },

        option: function option(key, value) {
            if (arguments.length === 0) {
                return this.button('option');
            }

            var opts = key;

            //handle key/value pair
            if (typeof key === 'string') {
                if (typeof value === 'undefined') {
                    return this.button('option', key);
                }
                opts = {};
                opts[key] = value;
            }

            return this.button('option', key).each(function each() {
                onCheckToggle.call($(this));
            });
        },

        toggle: function toggle() {
            return this.each(function each() {
                toggleCheck.call(this, !this.checked);
            });
        },

        check: function check() {
            return this.each(function each() {
                toggleCheck.call(this, true);
            });
        },

        uncheck: function uncheck() {
            return this.each(function each() {
                toggleCheck.call(this, false);
            });
        }
    };

    function onCheckToggle() {
        var this$ = this instanceof $ ? this : $(this),
            btnData = this$.data('button'),
            icons = btnData.options.icons || {}
        ;
        icons.primary = this$[0].checked ? icons.checked : icons.unchecked;
        this$.button('option', { icons: icons });
        btnData.buttonElement.removeClass('ui-state-focus');
    }

    function toggleCheck(check) {
        if (this.checked === check) {
            return;
        }
        var this$ = $(this),
            btnData = this$.data('button')
        ;
        this.checked = check;
        if (btnData) {
            onCheckToggle.call(this$.button('refresh'));
        }
    }

    $.fn.checkedButton = function checkedButton(method) {
        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        }
        else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        }
        else {
            return $.fn.button.apply(this, arguments);
        }
    };

    $.fn.checkedButton.defaults = {
        checkboxIcons: {
            checked: 'ui-icon-check',
            unchecked: 'ui-icon-cancel'
        },
        radioIcons: {
            checked: 'ui-icon-bullet',
            unchecked: 'ui-icon-radio-on'
        }
    };
})(jQuery);
