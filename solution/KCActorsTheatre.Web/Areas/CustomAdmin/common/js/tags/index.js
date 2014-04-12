(function ($, undefined) {
    var admin = window.admin = window.admin || {};
    (function () {

        // entity descriptions
        admin.itemDescription = 'Tags';
        admin.itemDeleteDescription = 'Are you sure you want to delete this tag? Doing so will also remove this tag from any associated resources and articles.';

        // entity CRUD uperation URLs
        admin.getAllItemsURL = '/CustomAdmin/Tags/All?tagType=' + TAG_TYPE;
        admin.findItemsURL = '/CustomAdmin/Tags/Find?tagType=' + TAG_TYPE;
        admin.editItemURL = '/CustomAdmin/Tags/Edit';
        admin.deleteItemURL = '/CustomAdmin/Tags/Delete';
        admin.editInPlaceURL = '/CustomAdmin/Tags/Edit';

        // this is the init function called at the end of the item-index.js page ready function
        // put all your custom methods here
        admin.items.index.pageInit = function () {
            
        };

        // custom logic goes here to run during new item tab init
        admin.items.index.tabInit = function ($editableParent) {
            
        };
    })();
})(jQuery);