/// <reference path="http://clickfarmcdn.localhost/common/js/jquery/1.7.2/jquery-vsdoc.js" />
/// <reference path="http://clickfarmcdn.localhost/common/js/kendo/2012.2.710/kendo.all-vsdoc.js" />

var createRoleManager;

$(function () {
    createRoleManager = new createRoleManager();
});

function createRoleManager() {
    var self = this,
        formEl = $('#create-role-form'),
        errorEl = formEl.find('#create-role-error'),
        loaderEl = formEl.find('#create-role-loading'),
		titleEl = formEl.find('#create-role-title'),
		showEl = formEl.find('#create-role-show'),
        showContainer = formEl.find('#create-show-container'),
        hiddenPersonIdEl = formEl.find('#create-role-person-id')
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

    self.showCreateRoleForm = function (personID) {
        self.resetCreateRoleForm();
        hiddenPersonIdEl.val(personID);
        formEl.dialog('open');
        titleEl.select();
    };

    self.hideCreateRoleForm = function () {
        self.resetCreateRoleForm();
        formEl.dialog('close');
    };

    self.resetCreateRoleForm = function () {
        formEl.dialog('close');
        self.hideError();
        loaderEl.hide();
        titleEl.val('');
        showEl.val('');
        hiddenPersonIdEl.val('');
    };

    self.ajaxBegin = function (jqXHR, settings) {
        self.hideError();
    };

    self.ajaxSuccess = ajaxHelper.success(
        function (result, textStatus, jqXHR) {
            self.hideCreateRoleForm();

            // get role
            var newPersonRole = result.Properties.RoleDefinition;

            // add to table
            var $parent = $('div.editable-parent[data-item-id="' + window.PERSON_ID + '"]');
            var $tbody = $parent.find('table tbody');

            // create new TR and append to TBODY
            $tbody.append('<tr data-role-definition-id="' + newPersonRole.RoleDefinitionID + '">' +
                '<td>' + newPersonRole.Name + '</td>' +
                '<td>' + newPersonRole.Title + '</td>' +
                '<td><a href="#" class="delete-role-link" data-person-id="' + newPersonRole.PersonID + '" data-role-id="' + newPersonRole.RoleDefinitionID + '">Delete</a></td>' +
                '</tr>'
            );

            admin.staff.roles.initButtons();

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