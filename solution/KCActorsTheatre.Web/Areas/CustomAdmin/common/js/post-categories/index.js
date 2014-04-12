(function ($, undefined) {
    var admin = window.admin = window.admin || {};
    (function () {

        // entity descriptions
        admin.itemDescription = 'Categories';
        admin.itemDeleteDescription = 'Are you sure you want to delete this category?';

        // entity CRUD uperation URLs
        admin.getAllItemsURL = '/CustomAdmin/PostCategories/All';
        admin.findItemsURL = '/CustomAdmin/PostCategories/Find';
        admin.editItemURL = '/CustomAdmin/PostCategories/Edit';
        admin.deleteItemURL = '/CustomAdmin/PostCategories/Delete';
        admin.editInPlaceURL = '/CustomAdmin/PostCategories/Edit';

        // this is the init function called at the end of the item-index.js page ready function
        // put all your custom methods here
        admin.items.index.pageInit = function () {
        };

        // custom logic goes here to run during new item tab init
        admin.items.index.tabInit = function ($editableParent) {
        };

    })();
})(jQuery);