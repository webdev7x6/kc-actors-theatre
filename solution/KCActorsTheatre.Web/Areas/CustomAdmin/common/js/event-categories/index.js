(function ($, undefined) {
    var admin = window.admin = window.admin || {};
    (function () {

        // entity descriptions
        admin.itemDescription = 'Event Categories';
        admin.itemDeleteDescription = 'Are you sure you want to delete this event category?';

        // entity CRUD uperation URLs
        admin.getAllItemsURL = '/CustomAdmin/EventCategories/All';
        admin.findItemsURL = '/CustomAdmin/EventCategories/Find';
        admin.editItemURL = '/CustomAdmin/EventCategories/Edit';
        admin.deleteItemURL = '/CustomAdmin/EventCategories/Delete';
        admin.editInPlaceURL = '/CustomAdmin/EventCategories/Edit';

        // this is the init function called at the end of the item-index.js page ready function
        // put all your custom methods here
        admin.items.index.pageInit = function () {
        };

        // custom logic goes here to run during new item tab init
        admin.items.index.tabInit = function ($editableParent) {
        };

    })();
})(jQuery);