(function ($, undefined) {
    var hn = window.hn = window.hn || {};
    (function () {

        // entity descriptions
        hn.itemDescription = 'News Articles';
        hn.itemDeleteDescription = 'Are you sure you want to delete this news article?';

        // entity CRUD uperation URLs
        hn.getAllItemsURL = '/CustomAdmin/News/AllArticlesAjax';
        hn.findItemsURL = '/CustomAdmin/News/FindArticlesAjax';
        hn.editItemURL = '/CustomAdmin/News/EditArticleAjax';
        hn.deleteItemURL = '/CustomAdmin/News/DeleteArticleAjax';
        hn.editInPlaceURL = '/CustomAdmin/News/EditArticleAjax';

        // this is the init function called at the end of the hn-item-index.js page ready function
        // put all your custom methods here
        hn.items.index.pageInit = function () {

        };
        
        hn.items.index.tabInit = function () {

        };

    })();
})(jQuery);