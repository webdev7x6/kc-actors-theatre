﻿(function ($, undefined) {
    var admin = window.admin = window.admin || {};
    (function () {

        // entity descriptions
        admin.itemDescription = 'Shows';
        admin.itemDeleteDescription = 'Are you sure you want to delete this show?';

        // entity CRUD uperation URLs
        admin.getAllItemsURL = '/CustomAdmin/Show/AllShowsAjax';
        admin.findItemsURL = '/CustomAdmin/Show/FindShowsAjax';
        admin.editItemURL = '/CustomAdmin/Show/EditShowAjax';
        admin.deleteItemURL = '/CustomAdmin/Show/DeleteShowAjax';
        admin.editInPlaceURL = '/CustomAdmin/Show/EditShowAjax';

        // custom operation URLs
        admin.AddPersonURL = '/CustomAdmin/Show/AddPerson';
        admin.RemovePersonURL = '/CustomAdmin/Show/RemovePerson';
        admin.FindPeopleURL = '/CustomAdmin/Show/FindPeople';
        admin.AllPeopleURL = '/CustomAdmin/Show/AllPeople';

        admin.AddImageURL = '/CustomAdmin/Show/AddImage';
        admin.RemoveImageURL = '/CustomAdmin/Show/RemoveImage';
        admin.UpdateImageDisplayOrderURL = '/CustomAdmin/Show/UpdateImageDisplayOrder';

        admin.AddVideoURL = '/CustomAdmin/Show/AddVideo';
        admin.RemoveVideoURL = '/CustomAdmin/Show/RemoveVideo';
        admin.UpdateVideoDisplayOrderURL = '/CustomAdmin/Show/UpdateVideoDisplayOrder';

        // this is the init function called at the end of the hn-item-index.js page ready function
        // put all your custom methods here
        admin.items.index.pageInit = function () {
            admin.people.init($('#manage-people-form'));
            admin.shows.images.init($('#manage-images-form'));
            admin.shows.videos.init($('#manage-videos-form'));
        };

        // custom logic goes here to run during new item tab init
        admin.items.index.tabInit = function ($editableParent) {    
            $editableParent.find('.edit-associated-people').on('click', function () { admin.items.index.personClick(this, $editableParent) });
            $editableParent.find('.edit-associated-images').on('click', function () { admin.shows.images.imageClick(this, $editableParent) });
            $editableParent.find('.edit-associated-videos').on('click', function () { admin.shows.videos.videoClick(this, $editableParent) });
        };

    })();
})(jQuery);