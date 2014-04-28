/// <reference path="http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.8.3-vsdoc.js" />
/// <reference path="http://clickfarmcdn.localhost/common/js/kendo/2012.2.710/kendo.all-vsdoc.js" />

var createItemManager;

(function ($, undefined) {
    var admin = window.admin = window.admin || {};
    admin.items = admin.items = admin.items || {};

    (function () {
        admin.items.NewItemManager = function (formSelector) {
            this.form = $(formSelector);
            this.errorElement = this.form.find('#create-item-error');
            this.loader = this.form.find('#create-item-loading');
            this.nameInput = this.form.find('#create-item-name');
        };

        admin.items.NewItemManager.prototype = function () {
            var
                init = function () {
                    var theForm = this.form;
                    this.form.dialog({
                        autoOpen: false,
                        modal: true,
                        resizable: false,
                        width: 400
                    });
                },
                showCreateItemForm = function () {
                    this.resetCreateItemForm();
                    this.form.dialog('open');
                    this.nameInput.select();
                },
                resetCreateItemForm = function () {
                    this.form.dialog('close');
                    this.hideError();
                    this.loader.hide();
                    this.nameInput.val('');
                },
				ajaxBegin = function (jqXHR, settings) {
				    this.hideError();
				},
				ajaxSuccess = ajaxHelper.success(
					function (data, textStatus, jqXHR) {
					    createItemManager.resetCreateItemForm();
					    var itemMgr = admin.items.getItemsManager('#items'),
                            viewModel = itemMgr.getViewModel();

					    var item = new admin.items.Item(data.Properties.Item);
					    viewModel.addItem(item);
					    viewModel.showEditItem(item);
					},
					function (data, textStatus, jqXHR, formattedMessage) {
					    createItemManager.showError(formattedMessage);
					},
					false
				),
				ajaxFailure = ajaxHelper.error(
					function (jqXHR, textStatus, errorThrown, formattedMessage) {
					    createItemManager.showError(formattedMessage);
					},
					false
				),
				showError = function (msg) {
				    this.errorElement.html(msg).fadeIn();
				},
				hideError = function () {
				    this.errorElement.html('').hide();
				}
            ;

            return {
                init: init,
                showCreateItemForm: showCreateItemForm,
                resetCreateItemForm: resetCreateItemForm,
                ajaxBegin: ajaxBegin,
                ajaxSuccess: ajaxSuccess,
                ajaxFailure: ajaxFailure,
                showError: showError,
                hideError: hideError
            };
        }();
    })();
})(jQuery);