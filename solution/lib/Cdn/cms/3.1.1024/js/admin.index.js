/// <reference path="http://clickfarmcdn.localhost/common/js/jquery/1.7.2/jquery-vsdoc.js" />
/// <reference path="http://clickfarmcdn.localhost/common/js/kendo/2012.2.710/kendo.all-vsdoc.js" />

var adminIndexMgr = new adminIndexManager();
$(function () {
    adminIndexMgr.initUI();
});

function adminIndexManager() {
    var self = this;

    this.initUI = function () {
        self.initUIButtons();
        $('#admin-index-tabs').tabs();
        self.initEditSettings();
    };

    this.initUIButtons = function () {
        $('.url-link').button({
            icons: {
                primary: 'ui-icon-extlink'
            }
        });
        $('.edit-app-link').button({
            icons: {
                primary: 'ui-icon-circle-arrow-e'
            }
        });
        $('.init-app').button({
            icons: {
                primary: 'ui-icon-refresh'
            }
        }).on('click', function (event) {
            event.preventDefault();
            self.initCms($(this).siblings('.loader'), $(this).attr('data-app-id'));
        });
    };

    this.initCms = function (loader, appID) {
        ajaxHelper.ajax('/Admin/Utility/ApplyChanges', {
            type: 'POST',
            data: { appID: appID },
            success: function (data, textStatus, jqXHR) {
                self.toggleLoader(loader);
            },
            beforeSend: function () {
                self.toggleLoader(loader);
            },
            failure: function (data, textStatus, jqXHR, formattedMessage) {
                self.toggleLoader(loader);
            },
            error: function (jqXHR, textStatus, errorThrown, formattedMessage) {
                self.toggleLoader(loader);
            }
        });
    };

    this.toggleLoader = function (loader) {
        if (loader.is(':visible')) {
            loader.hide();
        }
        else {
            loader.fadeIn('fast');
        }
    };

    this.initEditSettings = function () {
        $('.editable-parent').each(function () {
            var el = $(this);
            var editMgr = new EditableManager(el, { appID: el.attr('data-app-id') }, '/Admin/App/EditAjax');
            editMgr.initAllTypes();//doing all types here to accommodate any custom app edit interfaces 
        });
    }
}
