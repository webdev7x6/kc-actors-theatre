(function ($, undefined) {
    var admin = window.admin = window.admin || {};
    (function () {

        // entity descriptions
        admin.itemDescription = 'Seasons';
        admin.itemDeleteDescription = 'Are you sure you want to delete this season?';

        // entity CRUD uperation URLs
        admin.getAllItemsURL = '/CustomAdmin/Season/AllSeasonsAjax';
        admin.findItemsURL = '/CustomAdmin/Season/FindSeasonsAjax';
        admin.editItemURL = '/CustomAdmin/Season/EditSeasonAjax';
        admin.deleteItemURL = '/CustomAdmin/Season/DeleteSeasonAjax';
        admin.editInPlaceURL = '/CustomAdmin/Season/EditSeasonAjax';


        // this is the init function called at the end of the admin-item-index.js page ready function
        // put all your custom methods here
        admin.items.index.pageInit = function () {
            
        };

        // custom logic goes here to run during new item tab init
        admin.items.index.tabInit = function ($editableParent) {
            
        };

    })();
})(jQuery);