(function ($, undefined) {
    var admin = window.admin = window.admin || {};
    (function () {

        // entity descriptions
        admin.itemDescription = 'News Articles';
        admin.itemDeleteDescription = 'Are you sure you want to delete this news article?';

        // entity CRUD uperation URLs
        admin.getAllItemsURL = '/CustomAdmin/News/AllArticlesAjax';
        admin.findItemsURL = '/CustomAdmin/News/FindArticlesAjax';
        admin.editItemURL = '/CustomAdmin/News/EditArticleAjax';
        admin.deleteItemURL = '/CustomAdmin/News/DeleteArticleAjax';
        admin.editInPlaceURL = '/CustomAdmin/News/EditArticleAjax';

        // this is the init function called at the end of the admin-item-index.js page ready function
        // put all your custom methods here
        admin.items.index.pageInit = function () {

        };
        
        admin.items.index.tabInit = function () {

        };

    })();
})(jQuery);