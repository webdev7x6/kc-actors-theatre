/*!
 * Copyright (c) 2010 Andrew Watts
 *
 * Dual licensed under the MIT (MIT_LICENSE.txt)
 * and GPL (GPL_LICENSE.txt) licenses
 *
 * http://github.com/andrewwatts/ui.tabs.closable
 *
 * Updated 2012-07-17 Clickfarm Interactive, Inc. (BA)
 * Update 2013-01-09 Clickfarm Interactive, Inc. (AE)
 */
(function() {

var ui_tabs_processTabs= $.ui.tabs.prototype._processTabs;

$.extend($.ui.tabs.prototype, {

    _processTabs: function() {
        var self = this;

        ui_tabs_processTabs.apply(this, arguments);

        // if closable tabs are enable, add a close button
        if (self.options.closable === true) {

            var unclosable_lis = this.tabs.filter(function() {
                // return the lis that do not have a close button
                return $('span.ui-icon-circle-close', this).length === 0;
            });

            // append the close button and associated events
              unclosable_lis.not('.tab-notclosable').each(function () {
                $(this)
                    .append('<a href="#"><span class="ui-icon ui-icon-circle-close"></span></a>')
                    .find('a:last')
                        .hover(
                            function() {
                                $(this).css('cursor', 'pointer');
                            },
                            function() {
                                $(this).css('cursor', 'default');
                            }
                        )
                        .click(function(event) {
                            // don't follow the link
                            event.preventDefault();

                            var index = self.tabs.index($(this).parent());
                            if (index > -1) {
                                // call _trigger to see if remove is allowed
                                if (false === self._trigger("closableClick", null, self._ui( $(self.tabs[index]).find( "a" )[ 0 ], self.panels[index] ))) return;

                                // remove this tab
                                self.remove(index)
                            }
                        })
                    .end();
            });
        }
    }
});

})(jQuery);
