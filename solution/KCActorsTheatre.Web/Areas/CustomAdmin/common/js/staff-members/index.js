(function ($, undefined) {
    var admin = window.admin = window.admin || {};
    (function () {

        // entity descriptions
        admin.itemDescription = 'Staff Members';
        admin.itemDeleteDescription = 'Are you sure you want to delete this staff member? This will also remove any roles.';

        // custom descriptions
        admin.deleteRoleDescription = 'Are you sure you want to delete this role?';

        // entity CRUD uperation URLs
        admin.getAllItemsURL = '/CustomAdmin/StaffMembers/AllByType?staffType=' + STAFF_TYPE;
        admin.findItemsURL = '/CustomAdmin/StaffMembers/FindByType?staffType=' + STAFF_TYPE;
        admin.editItemURL = '/CustomAdmin/StaffMembers/Edit';
        admin.deleteItemURL = '/CustomAdmin/StaffMembers/Delete';
        admin.editInPlaceURL = '/CustomAdmin/StaffMembers/Edit';

        // custom operation urls
        admin.deleteRoleURL = '/CustomAdmin/StaffMembers/DeleteRole';

        // this is the init function called at the end of the item-index.js page ready function
        // put all your custom methods here
        admin.items.index.pageInit = function () {
            $('#create-role-title').autocomplete({
                source: window.TitleList
            });
        };

        // custom logic goes here to run during new item tab init
        admin.items.index.tabInit = function ($editableParent) {
            $('.staff-member-items-tabs').tabs();
            admin.staff.roles.initButtons();
        };
    })();
})(jQuery);