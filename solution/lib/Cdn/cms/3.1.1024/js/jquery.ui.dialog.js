/// <reference path="http://clickfarmcdn.localhost/common/js/jquery/1.7.2/jquery-vsdoc.js" />
/// <reference path="http://clickfarmcdn.localhost/common/js/kendo/2012.2.710/kendo.all-vsdoc.js" />

/*
A dialog helper that encapsulates and standardizes some dialog functionality.

Properties:

    element
        A jQuery object representing the <div/> or other element to use as the basis for the dialog. If the value is
        false-ish, a new <div/> will be created.

        Default: null

        Example: dialogHelper.element = $('#existing-div');

    jqOptions
        A JSON object representing jQuery UI dialog options. The object will be passed as-is into dialog(). For more
        information, see [[http://jqueryui.com/demos/dialog/]].

        Default:

            {
                draggable: false,
                modal: true,
                resizable: false,
                width: 400
            }

Methods:

    alert
        Opens a dialog with the specified message and title, and a single button labeled "Close."

        Parameters:

            message - optional
                The message to be displayed on the dialog.

            title - optional
                The title of the dialog.

        Example: dialogHelper.alert('Some message text goes here.', 'A Title');

    close
        Closes the currently open dialog.

        Parameters: none

        Example: dialogHelper.close();

    open
        Opens a dialog with the specified message, title or fallback title (see descriptions), and buttons.

        Parameters:

            message - optional
                The message to be displayed on the dialog.

            title - optional
            fallbackTitle - optional
                When determining which value to use as the title of the dialog, potential values are evaluated in the
                following order:

                1. title
                2. dialogHelper.jqOptions.title
                3. fallbackTitle

                When a true-ish value is found, evaluation stops and that value is used as the title. If all of the
                values are false-ish, no title is shown.

            buttons - optional
                The buttons, if any, to be shown on the dialog.

        Example:

            dialogHelper.open(
                'Some message text goes here.',
                'A Title',
                'A Fallback Title',
                { Close: dialogHelper.close }
            );
*/

var dialogHelper = {
    element: null,
    jqOptions: {
        draggable: false,
        modal: true,
        resizable: false,
        width: 400
    },
    alert: function (message, title) {
        dialogHelper.open(message, title, 'Alert', { 'Close': dialogHelper.close });
    },
    confirm: function (message, title, confirmCallback) {
        dialogHelper.open(
            message,
            title,
            'Confirm',
            {
                'OK': function () {
                    dialogHelper.close();
                    if ($.isFunction(confirmCallback)) {
                        confirmCallback();
                    }
                },
                'Cancel': dialogHelper.close
            }
        );
    },
    confirmDeferred: function (message, title) {
        var $retval = $.Deferred();
        dialogHelper.open(
           message,
           title,
           'Confirm',
           {
               'OK': function () {
                   dialogHelper.close();
                   $retval.resolve();
               },
               'Cancel': function () {
                   dialogHelper.close();
                   $retval.reject();
               }
           }
       );
        return $retval;
    },
    open: function (message, title, fallbackTitle, buttons) {
        var opts = { buttons: buttons };
        if (title) {
            opts.title = title;
        }
        else if (!dialogHelper.jqOptions.title) {
            opts.title = fallbackTitle;
        }

        dialogHelper.ensureElement();

        //set the dialog text and show
        dialogHelper.element.html(message).dialog($.extend({}, dialogHelper.jqOptions, opts));
    },
    close: function () {
        if (dialogHelper.element) {
            dialogHelper.element.dialog('close');
        }
    },
    ensureElement: function () {
        if (!dialogHelper.element) {
            dialogHelper.element = $('<div />');
        }
    }
};
