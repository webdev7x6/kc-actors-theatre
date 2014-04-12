(function ($, undefined) {
    var admin = window.admin = window.admin || {};
    (function () {

        // entity descriptions
        admin.itemDescription = 'Mission Centers';
        admin.itemDeleteDescription = 'Are you sure you want to delete this mission center? This will also remove any contact associations.';

        // custom descriptions
        admin.itemRemoveContactDescription = 'Are you sure you want to remove this contact?';

        // entity CRUD uperation URLs
        admin.getAllItemsURL = '/CustomAdmin/MissionCenters/All';
        admin.findItemsURL = '/CustomAdmin/MissionCenters/Find';
        admin.editItemURL = '/CustomAdmin/MissionCenters/Edit';
        admin.deleteItemURL = '/CustomAdmin/MissionCenters/Delete';
        admin.editInPlaceURL = '/CustomAdmin/MissionCenters/Edit';

        // custom operation urls
        admin.deleteRoleURL = '/CustomAdmin/MissionCenters/DeleteRole';
        admin.updateContactDisplayOrder = '/CustomAdmin/MissionCenters/UpdateContactDisplayOrder';

        // this is the init function called at the end of the item-index.js page ready function
        // put all your custom methods here
        admin.items.index.pageInit = function () {

        };

        // custom logic goes here to run during new item tab init
        admin.items.index.tabInit = function ($editableParent) {

            $('.mission-center-items-tabs').tabs();

            $('.delete-role-definition').each(function (index, link) {
                var link = $(link);
                link.button({
                    icons: {
                        primary: 'ui-icon-trash'
                    },
                    text: false
                }).on('click', function (e) {
                    e.preventDefault();
                    var roleDefinitionID = link.attr('data-role-definition-id'),
                        missionCenterID = $(this).closest('.editable-parent').attr('data-item-id')
                    ;

                    dialogHelper.confirm(
                        admin.itemRemoveContactDescription,
                        'Remove Contact',
                        function () {
                            ajaxHelper.ajax(admin.deleteRoleURL, {
                                data: {
                                    missionCenterID: missionCenterID,
                                    roleDefinitionID: roleDefinitionID
                                },
                                type: 'POST',
                                success: function (data, textStatus, jqXHR) {
                                    var $tr = $('.contacts-table tr#' + roleDefinitionID);
                                    $tr.remove();
                                },
                                failureMessageFormat: 'An error occurred trying to delete the Role: [[errorThrown]]'
                            });
                        }
                    );
                });
            });

            // make contact rows sortable
            $('.contacts-table tbody').sortable({
                handle: '.sort-order',
                update: function (event, ui) {
                    var
                        roleDefinitionIDs = $(this).sortable('toArray'),
                        missionCenterID = $(this).closest('.editable-parent').attr('data-item-id')
                    ;
                    ajaxHelper.ajax(admin.updateContactDisplayOrder, {
                        data: {
                            missionCenterID: missionCenterID,
                            roleDefinitionIDs: roleDefinitionIDs
                        },
                        type: 'POST',
                        traditional: true,
                        failureMessageFormat: 'An error occurred trying to set the contacts display order: [[errorThrown]]'
                    });
                }
            });


        };
    })();
})(jQuery);