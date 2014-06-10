/// <reference path="http://clickfarmcdn.localhost/common/js/jquery/1.7.2/jquery-vsdoc.js" />
/// <reference path="http://clickfarmcdn.localhost/common/js/kendo/2012.2.710/kendo.all-vsdoc.js" />

/*
An AJAX helper that encapsulates some Clickfarm-specific functionality.

Usage is the same as jQuery.ajax() with the addition of five options and the modification of one:

    ajaxHelper.ajax('/some/ajax/url', {
        success: function (data, textStatus, jqXHR) {
            //callback for handling success, i.e. data.Succeeded !== false
        },
        failure: function (data, textStatus, jqXHR, formattedMessage) {
            //callback for handling failure, i.e. data.Succeeded === false
        },
        failureDoAlert: false,
        failureMessageFormat: 'jhdlahja [[errorThrown]]',
        error: function (jqXHR, textStatus, errorThrown, formattedMessage) {
            //callback for handling error
        },
        errorDoAlert: false,
        errorMessageFormat: 'blahblah [[textStatus]] blahblah [[errorThrown]]'
    });

Options:

    failure - optional
        A callback method invoked when the AJAX call itself succeeds but the server indicates failure, i.e.
        data.Succeeded === false. The formattedMessage parameter will contain the fully formatted alert message. See
        failureMessageFormat for more details.

        Example:

            options = {
                failure: function (data, textStatus, jqXHR, formattedMessage) {
                    $('#message').text(formattedMessage);
                }
            };

    failureDoAlert - optional
        A boolean specifying whether the helper should alert the user to the failure.

        Default: true

        Example:

            options = {
                failureDoAlert: false
            };

    failureMessageFormat - optional
        A string specifying the format of the failure alert. The '[[errorThrown]]' placeholder will be replaced with the
        message from the server, or 'Unknown error' if no message was specified.

        Default: 'An error has occured: [[errorThrown]]'

        Example:

            options = {
                failureMessageFormat: 'Oops! [[errorThrown]]'
            };

    error - updated
        As in the jQuery documentation with the addition of a fourth parameter: formattedMessage. The formattedMessage
        parameter will contain the fully formatted alert message. See errorMessageFormat for more details.

        Example:

            options = {
                error: function (jqXHR, textStatus, errorThrown, formattedMessage) {
                    $('#message').text(formattedMessage);
                }
            };

    errorDoAlert - optional
        A boolean specifying whether the helper should alert the user to the error.

        Default: true

        Example:

            options = {
                errorDoAlert: false
            };

    errorMessageFormat - optional
        A string specifying the format of the error alert. The '[[textStatus]]' placeholder will be replaced with the
        corresponding parameter of the error callback, or 'error' if no status was specified. The '[[errorThrown]]'
        placeholder will be replaced with the corresponding parameter of the error callback, or 'Unknown error' if no
        error message was specified.

        Default: 'An error of type \'[[textStatus]]\' has occured: [[errorThrown]]'

        Example:

            options = {
                errorMessageFormat: 'Oops! [[errorThrown]] ([[textStatus]])'
            };
*/

var ajaxHelper = {
    ajax: function (url, settings) {
        var options = $.extend({}, ajaxHelper.ajaxDefaults, settings);
        options.error = ajaxHelper.error(options.error, options.errorDoAlert, options.errorMessageFormat);
        options.success = ajaxHelper.success(options.success, options.failure, options.failureDoAlert, options.failureMessageFormat);
        return $.ajax(url, options);
    },
    ajaxDefaults: {
        errorDoAlert: true,
        errorMessageFormat: null,
        failureDoAlert: true,
        failureMessageFormat: null
    },
    error: function (callback, doAlertError, errorMessageFormat) {
        return function (jqXHR, textStatus, errorThrown) {
            var status = textStatus || 'error',
                message = errorThrown || 'Unknown error',
                formattedMessage
            ;
            if (typeof errorMessageFormat === 'string' && errorMessageFormat.length > 0) {
                formattedMessage = errorMessageFormat.replace(/\[\[textStatus\]\]/gi, status).replace(/\[\[errorThrown\]\]/gi, message);
            }
            else {
                formattedMessage = "An error of type '" + status + "' has occured: " + message;
            }
            if (typeof doAlertError === 'undefined' || doAlertError === true) {
                dialogHelper.alert(formattedMessage, 'Error');
            }

            if ($.isFunction(callback)) {
                return callback.call(this, jqXHR, textStatus, errorThrown, formattedMessage);
            }
        };
    },
    success: function (successCallback, failureCallback, doAlertFailure, failureMessageFormat) {
        return function (data, textStatus, jqXHR) {
            if (
                data
                && data.DoRedirect === true
                && data.Properties
                && typeof data.Properties.url === 'string'
                && data.Properties.url.length > 0
            ) {
                top.location = data.Properties.url;
                return;
            }

            if (data.Succeeded === false) {
                var message = data.Message || 'Unknown error',
                    formattedMessage = ''
                ;
                if (typeof failureMessageFormat === 'string' && failureMessageFormat.length > 0) {
                    formattedMessage = failureMessageFormat.replace(/\[\[errorThrown\]\]/gi, message);
                }
                else {
                    formattedMessage = 'An error has occured: ' + message;
                }
                if (typeof doAlertFailure === 'undefined' || doAlertFailure === true) {
                    dialogHelper.alert(formattedMessage, 'Error');
                }

                if ($.isFunction(failureCallback)) {
                    return failureCallback.call(this, data, textStatus, jqXHR, formattedMessage);
                }
            }
            else if ($.isFunction(successCallback)) {
                return successCallback.call(this, data, textStatus, jqXHR);
            }
        }
    }
};
