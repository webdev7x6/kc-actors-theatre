(function ($, undefined) {
    var hn = window.hn = window.hn || {};
    (function () {

        // entity descriptions
        hn.itemDescription = 'Blog Post';
        hn.itemDeleteDescription = 'This blog post will be permanently deleted along with any associated comments, continue?';

        // entity CRUD uperation URLs
        hn.getAllItemsURL = '/CustomAdmin/Blog/AllPostsAjax';
        hn.findItemsURL = '/CustomAdmin/Blog/FindPostsAjax';
        hn.editItemURL = '/CustomAdmin/Blog/EditPostAjax';
        hn.deleteItemURL = '/CustomAdmin/Blog/DeletePostAjax';
        hn.editInPlaceURL = '/CustomAdmin/Blog/EditPostAjax';

        // this is the init function called at the end of the hn-item-index.js page ready function
        // put all your custom methods here
        hn.items.index.pageInit = function () {

        };

    })();
})(jQuery);