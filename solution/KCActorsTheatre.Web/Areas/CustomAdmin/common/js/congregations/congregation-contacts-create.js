/// <reference path="http://clickfarmcdn.localhost/common/js/jquery/1.7.2/jquery-vsdoc.js" />
/// <reference path="http://clickfarmcdn.localhost/common/js/kendo/2012.2.710/kendo.all-vsdoc.js" />

var createContactManager;

$(function () {
    createContactManager = new createContactManager();

    $('#create-contact')
        .button({
            icons: {
                primary: 'ui-icon-plusthick'
            }
        })
        .on('click', function (e) {
            e.preventDefault();
            createContactManager.showCreateContactForm();
        })
    ;
});

function createContactManager() {
    var self = this,
        formEl = $('#create-contact-form'),
        errorEl = formEl.find('#create-contact-error'),
        loaderEl = formEl.find('#create-contact-loading'),
		nameEl = formEl.find('#create-contact-name'),
		positionEl = formEl.find('#create-contact-position'),
        hiddenCongregationID = formEl.find('#create-contact-congregation-id')
    ;
    formEl.dialog({
        autoOpen: false,
        buttons: {
            'Create': function () {
                formEl.find('form').submit();
            }
        },
        modal: true,
        resizable: false,
        width: 400
    });

    self.showCreateContactForm = function () {
        self.resetCreateContactForm();
        hiddenCongregationID.val(window.CONGREGATION_ID);
        formEl.dialog('open');
        nameEl.select();
    };

    self.hideCreateContactForm = function () {
        self.resetCreateContactForm();
        formEl.dialog('close');
    };

    self.resetCreateContactForm = function () {
        formEl.dialog('close');
        self.hideError();
        loaderEl.hide();
        nameEl.val('');
        positionEl.val('');
        hiddenCongregationID.val('');
    };

    self.ajaxBegin = function (jqXHR, settings) {
        self.hideError();
    };

    self.ajaxSuccess = ajaxHelper.success(
        function (result, textStatus, jqXHR) {
            self.hideCreateContactForm();

            // get new congregation contact
            var newCongregationContact = result.Properties.CongregationContact;

            // add to observable array
            window.associatedContactsViewModel.contacts.push({
                contactID: newCongregationContact.CongregationContactID,
                name: newCongregationContact.Name,
                position: newCongregationContact.Position,
                email: '',
                phone: ''
            });

            // make rows editable to initialize new row
            $('.contacts-table tbody tr').each(function (index, item) {
                admin.congregations.contacts.initEditInPlace(item);
            });

            //initialize buttons
            admin.congregations.contacts.initButtons();

        },
        function (data, textStatus, jqXHR, formattedMessage) {
            self.showError(formattedMessage);
        },
        false
    );

    self.ajaxFailure = ajaxHelper.error(
        function (jqXHR, textStatus, errorThrown, formattedMessage) {
            self.showError(formattedMessage);
        },
        false
    );

    self.showError = function (msg) {
        errorEl.html(msg).fadeIn();
    };

    self.hideError = function () {
        errorEl.html('').hide();
    };
}