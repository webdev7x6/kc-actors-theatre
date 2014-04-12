(function ($, undefined) {
    var admin = window.admin = window.admin || {};
    (function () {

        // entity descriptions
        admin.itemDescription = 'Mission Story';
        admin.itemDeleteDescription = 'Are you sure you want to delete this mission story?';

        // entity CRUD uperation URLs
        admin.getAllItemsURL = '/CustomAdmin/MissionStories/All';
        admin.findItemsURL = '/CustomAdmin/MissionStories/Find';
        admin.deleteItemURL = '/CustomAdmin/MissionStories/Delete';
        admin.editItemURL = '/CustomAdmin/MissionStories/Edit';
        admin.editInPlaceURL = '/CustomAdmin/MissionStories/Edit';

        // custom operation URLs
        admin.AddTagURL = '/CustomAdmin/MissionStories/AddTag';
        admin.RemoveTagURL = '/CustomAdmin/MissionStories/RemoveTag';
        admin.FindTagsURL = '/CustomAdmin/MissionStories/FindTags';
        admin.AllTagsURL = '/CustomAdmin/MissionStories/AllTags';

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