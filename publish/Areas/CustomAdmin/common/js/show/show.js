(function ($, undefined) {
    var hn = window.hn = window.hn || {};
    (function () {

        // entity descriptions
        hn.itemDescription = 'Shows';
        hn.itemDeleteDescription = 'Are you sure you want to delete this show?';

        // entity CRUD uperation URLs
        hn.getAllItemsURL = '/CustomAdmin/Show/AllShowsAjax';
        hn.findItemsURL = '/CustomAdmin/Show/FindShowsAjax';
        hn.editItemURL = '/CustomAdmin/Show/EditShowAjax';
        hn.deleteItemURL = '/CustomAdmin/Show/DeleteShowAjax';
        hn.editInPlaceURL = '/CustomAdmin/Show/EditShowAjax';

        // custom operation URLs
        hn.AddPersonURL = '/CustomAdmin/Show/AddPerson';
        hn.RemovePersonURL = '/CustomAdmin/Show/RemovePerson';
        hn.FindPeopleURL = '/CustomAdmin/Show/FindPeople';
        hn.AllPeopleURL = '/CustomAdmin/Show/AllPeople';

        // this is the init function called at the end of the hn-item-index.js page ready function
        // put all your custom methods here
        hn.items.index.pageInit = function () {
            hn.people.init($('#manage-people-form'));
        };

        // custom logic goes here to run during new item tab init
        hn.items.index.tabInit = function ($editableParent) {
            $editableParent.find('.edit-associated-people').on('click', function () { hn.items.index.personClick(this, $editableParent) });
        };

    })();
})(jQuery);