(function ($, undefined) {
    var hn = window.hn = window.hn || {};
    (function () {

        // entity descriptions
        hn.itemDescription = 'Calendar Events';
        hn.itemDeleteDescription = 'Are you sure you want to delete this calendar event?';

        // entity CRUD uperation URLs
        hn.getAllItemsURL = '/CustomAdmin/Calendar/AllEventsAjax';
        hn.findItemsURL = '/CustomAdmin/Calendar/FindEventsAjax';
        hn.editItemURL = '/CustomAdmin/Calendar/EditEventAjax';
        hn.deleteItemURL = '/CustomAdmin/Calendar/DeleteEventAjax';
        hn.editInPlaceURL = '/CustomAdmin/Calendar/EditEventAjax';

        // this is the init function called at the end of the hn-item-index.js page ready function
        // put all your custom methods here
        hn.items.index.pageInit = function () {

        };

    })();
})(jQuery);