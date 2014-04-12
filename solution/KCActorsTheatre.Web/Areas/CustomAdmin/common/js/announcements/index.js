(function ($, undefined) {
    var admin = window.admin = window.admin || {};
    (function () {

        // entity descriptions
        admin.itemDescription = 'Announcement';
        admin.itemDeleteDescription = 'Are you sure you want to delete this announcement?';

        // entity CRUD uperation URLs
        admin.getAllItemsURL = '/CustomAdmin/Announcements/All?type=' + TYPE;
        admin.findItemsURL = '/CustomAdmin/Announcements/Find?type=' + TYPE;
        admin.deleteItemURL = '/CustomAdmin/Announcements/Delete';
        admin.editItemURL = '/CustomAdmin/Announcements/Edit';
        admin.editInPlaceURL = '/CustomAdmin/Announcements/Edit';

        // custom operation URLs
        admin.AddTagURL = '/CustomAdmin/Announcements/AddTag';
        admin.RemoveTagURL = '/CustomAdmin/Announcements/RemoveTag';
        admin.FindTagsURL = '/CustomAdmin/Announcements/FindTags?type=' + TYPE;
        admin.AllTagsURL = '/CustomAdmin/Announcements/AllTags?type=' + TYPE;

        // this is the init function called at the end of the item-index.js page ready function
        // put all your custom methods here
        admin.items.index.pageInit = function () {
            admin.tags.init($('#manage-tags-form'));
        };

        // custom logic goes here to run during new item tab init
        admin.items.index.tabInit = function ($editableParent) {
            $editableParent.find('.edit-associated-tags').on('click', function () { admin.items.index.tagClick(this, $editableParent) })
        };

        // custom functions

    })();
})(jQuery);