(function ($, undefined) {
    var admin = window.admin = window.admin || {};
    (function () {

        // entity descriptions
        admin.itemDescription = 'People';
        admin.itemDeleteDescription = 'Are you sure you want to delete this person?';

        // entity CRUD uperation URLs
        admin.getAllItemsURL = '/CustomAdmin/Person/AllPeopleAjax';
        admin.findItemsURL = '/CustomAdmin/Person/FindPeopleAjax';
        admin.editItemURL = '/CustomAdmin/Person/EditPersonAjax';
        admin.deleteItemURL = '/CustomAdmin/Person/DeletePersonAjax';
        admin.editInPlaceURL = '/CustomAdmin/Person/EditPersonAjax';

        // this is the init function called at the end of the admin-item-index.js page ready function
        // put all your custom methods here
        admin.items.index.pageInit = function () {

        };
        
        // custom logic goes here to run during new item tab init
        admin.items.index.tabInit = function () {
            $('.person-items-tabs').tabs();
            admin.people.roles.initButtons();
        };

    })();
})(jQuery);