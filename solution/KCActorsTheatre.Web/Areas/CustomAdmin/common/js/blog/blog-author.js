(function ($, undefined) {
    var hn = window.hn = window.hn || {};
    (function () {

        // entity descriptions
        hn.itemDescription = 'Blog Authors';
        hn.itemDeleteDescription = 'After deleting this author, you will need to set the associated author for any related blog posts, continue?';

        // entity CRUD uperation URLs
        hn.getAllItemsURL = '/CustomAdmin/Blog/AllAuthorsAjax';
        hn.findItemsURL = '/CustomAdmin/Blog/FindAuthorsAjax';
        hn.editItemURL = '/CustomAdmin/Blog/EditAuthorAjax';
        hn.deleteItemURL = '/CustomAdmin/Blog/DeleteAuthorAjax';
        hn.editInPlaceURL = '/CustomAdmin/Blog/EditAuthorAjax';

        // this is the init function called at the end of the hn-item-index.js page ready function
        // put all your custom methods here
        hn.items.index.pageInit = function () {

        };

    })();
})(jQuery);