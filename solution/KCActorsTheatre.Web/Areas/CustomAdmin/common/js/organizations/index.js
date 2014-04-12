(function ($, undefined) {
    var admin = window.admin = window.admin || {};
    (function () {

        // entity descriptions
        admin.itemDescription = 'Organizations';
        admin.itemDeleteDescription = 'Are you sure you want to delete this organization?';

        // entity CRUD uperation URLs
        admin.getAllItemsURL = '/CustomAdmin/Organizations/All?organizationType=' + ORGANIZATION_TYPE;
        admin.findItemsURL = '/CustomAdmin/Organizations/Find?organizationType=' + ORGANIZATION_TYPE;
        admin.editItemURL = '/CustomAdmin/Organizations/Edit';
        admin.deleteItemURL = '/CustomAdmin/Organizations/Delete';
        admin.editInPlaceURL = admin.editItemURL;

        // this is the init function called at the end of the item-index.js page ready function
        // put all your custom methods here
        admin.items.index.pageInit = function () {
            
        };

        // custom logic goes here to run during new item tab init
        admin.items.index.tabInit = function ($editableParent) {
            
        };
    })();
})(jQuery);