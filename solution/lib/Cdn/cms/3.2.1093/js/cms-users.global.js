/// <reference path="http://clickfarmcdn.localhost/common/js/jquery/1.7.2/jquery-vsdoc.js" />
/// <reference path="http://clickfarmcdn.localhost/common/js/kendo/2012.2.710/kendo.all-vsdoc.js" />

function initEditCmsUserTab(panel) {
    var editableParent = panel.find('.editable-parent'),
        cmsUserID = parseInt(editableParent.attr('data-cms-user-id'))
    ;
    if (editableParent.length > 0) {
        var editMgr = new EditableManager(
            editableParent,
            {
                cmsUserID: cmsUserID
            },
            '/Admin/CmsUsers/EditAjax'
        );
        editMgr.initTextboxes();
        editMgr.initPasswords();
        editMgr.initSelects();
        editMgr.initCheckboxLists();
    }

    editableParent.find('.refresh-cms-user-link')
        .button({
            icons: {
                primary: 'ui-icon-refresh'
            },
            text: false
        })
        .on('click', function (e) {
            e.preventDefault();
            cmsUsersViewModel.tabMgr.reloadTabByID(uniqueIDForCmsUserTab(cmsUserID));
        })
    ;

    editableParent.find('.delete-cms-user-link')
        .button({
            icons: {
                primary: 'ui-icon-trash'
            },
            text: false
        })
        .on('click', function (e) {
            e.preventDefault();
            dialogHelper.confirm(
                'Are you sure you want to delete this CMS user?',
                'Delete CMS User',
                function () {
                    ajaxHelper.ajax('/Admin/CmsUsers/DeleteAjax', {
                        data: {
                            id: cmsUserID
                        },
                        type: 'POST',
                        success: function (data, textStatus, jqXHR) {
                        	cmsUsersViewModel.deleteCmsUser(cmsUserID);
                        },
                        failureMessageFormat: 'An error occurred trying to delete the CMS user: [[errorThrown]]'
                    });
                }
            );
        })
    ;
}

function updateCurrentCmsUserProfileLinkText(newEmailAddress) {
    $('#current-cms-user-profile-link').text(newEmailAddress);
}
