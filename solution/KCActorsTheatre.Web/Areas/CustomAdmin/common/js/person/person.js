(function ($, undefined) {
    var hn = window.hn = window.hn || {};
    (function () {

        // entity descriptions
        hn.itemDescription = 'People';
        hn.itemDeleteDescription = 'Are you sure you want to delete this person?';

        // entity CRUD uperation URLs
        hn.getAllItemsURL = '/CustomAdmin/Person/AllPeopleAjax';
        hn.findItemsURL = '/CustomAdmin/Person/FindPeopleAjax';
        hn.editItemURL = '/CustomAdmin/Person/EditPersonAjax';
        hn.deleteItemURL = '/CustomAdmin/Person/DeletePersonAjax';
        hn.editInPlaceURL = '/CustomAdmin/Person/EditPersonAjax';

        // this is the init function called at the end of the hn-item-index.js page ready function
        // put all your custom methods here
        hn.items.index.pageInit = function () {

        };

    })();
})(jQuery);