(function ($, undefined) {
    var admin = window.admin = window.admin || {};
    (function () {

        // entity descriptions
        admin.itemDescription = 'Congregations';
        admin.itemDeleteDescription = 'Are you sure you want to delete this congregation?';

        // entity CRUD uperation URLs
        admin.getAllItemsURL = '/CustomAdmin/Congregations/All';
        admin.findItemsURL = '/CustomAdmin/Congregations/Find';
        admin.editItemURL = '/CustomAdmin/Congregations/Edit';
        admin.deleteItemURL = '/CustomAdmin/Congregations/Delete';
        admin.editInPlaceURL = '/CustomAdmin/Congregations/Edit';

        // this is the init function called at the end of the item-index.js page ready function
        // put all your custom methods here
        admin.items.index.pageInit = function () {
            admin.congregations.contacts.init($('#manage-contacts-form'));
        };

        // custom logic goes here to run during new item tab init
        admin.items.index.tabInit = function ($editableParent) {
            $('.congregation-items-tabs').tabs();
            $editableParent.find('.edit-associated-contacts').on('click', function () { admin.congregations.contacts.contactClick(this, $editableParent) })
        };

    })();
})(jQuery);