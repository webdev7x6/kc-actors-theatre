/// <reference path="http://clickfarmcdn.localhost/common/js/jquery/1.7.2/jquery-vsdoc.js" />
/// <reference path="http://clickfarmcdn.localhost/common/js/kendo/2012.2.710/kendo.all-vsdoc.js" />

var createCmsUserMgr;

$(function () {
    createCmsUserMgr = new CreateCmsUserManager();

    $('#create-cms-user')
        .button({
            icons: {
                primary: 'ui-icon-plusthick'
            }
        })
        .on('click', function (e) {
            e.preventDefault();
            createCmsUserMgr.showCreateCmsUserForm();
        })
    ;
});

function CreateCmsUserManager() {
    var self = this,
        formEl = $('#create-cms-user-form'),
        errorEl = formEl.find('#create-cms-user-error'),
        loaderEl = formEl.find('#create-cms-user-loading'),
		firstNameEl = formEl.find('#create-cms-user-first-name'),
		lastNameEl = formEl.find('#create-cms-user-last-name'),
        emailEl = formEl.find('#create-cms-user-email'),
        pswdEl = formEl.find('#create-cms-user-password'),
        pswdConfEl = formEl.find('#create-cms-user-confirm-password')
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

    self.showCreateCmsUserForm = function () {
        self.resetCreateCmsUserForm();
        formEl.dialog('open');
        firstNameEl.select();
    };

    self.resetCreateCmsUserForm = function () {
    	formEl.dialog('close');
    	self.hideError();
    	loaderEl.hide();
    	emailEl.val('');
    	firstNameEl.val('');
    	lastNameEl.val('');
    	pswdEl.val('');
    	pswdConfEl.val('');
    };

    self.ajaxBegin = function (jqXHR, settings) {
        self.hideError();
    };

    self.ajaxSuccess = ajaxHelper.success(
        function (data, textStatus, jqXHR) {
            self.resetCreateCmsUserForm();
            var user = new CmsUser(data.CmsUserID, data.Email, data.FirstName, data.LastName, data.Status, []);
            cmsUsersViewModel.addResult(user);
            user.editCmsUser();
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
