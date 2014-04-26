(function ($, undefined) {
    var hn = window.hn = window.hn || {};
    (function () {

        // entity descriptions
        hn.itemDescription = 'Seasons';
        hn.itemDeleteDescription = 'Are you sure you want to delete this season?';

        // entity CRUD uperation URLs
        hn.getAllItemsURL = '/CustomAdmin/Season/AllSeasonsAjax';
        hn.findItemsURL = '/CustomAdmin/Season/FindSeasonsAjax';
        hn.editItemURL = '/CustomAdmin/Season/EditSeasonAjax';
        hn.deleteItemURL = '/CustomAdmin/Season/DeleteSeasonAjax';
        hn.editInPlaceURL = '/CustomAdmin/Season/EditSeasonAjax';


        // this is the init function called at the end of the hn-item-index.js page ready function
        // put all your custom methods here
        hn.items.index.pageInit = function () {
            
        };

        // custom logic goes here to run during new item tab init
        hn.items.index.tabInit = function ($editableParent) {
            
        };

    })();
})(jQuery);