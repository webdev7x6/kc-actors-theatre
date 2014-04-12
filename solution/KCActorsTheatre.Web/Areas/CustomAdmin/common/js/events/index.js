(function ($, undefined) {
    var admin = window.admin = window.admin || {};
    (function () {

        // entity descriptions
        admin.itemDescription = 'Calendar Events';
        admin.itemDeleteDescription = 'Are you sure you want to delete this calendar event?';

        // entity CRUD uperation URLs
        admin.getAllItemsURL = '/CustomAdmin/Events/All';
        admin.findItemsURL = '/CustomAdmin/Events/Find';
        admin.editItemURL = '/CustomAdmin/Events/Edit';
        admin.deleteItemURL = '/CustomAdmin/Events/Delete';
        admin.editInPlaceURL = admin.editItemURL;

        // custom operation URLs
        admin.AddTagURL = '/CustomAdmin/Events/AddTag';
        admin.RemoveTagURL = '/CustomAdmin/Events/RemoveTag';
        admin.FindTagsURL = '/CustomAdmin/Events/FindTags';
        admin.AllTagsURL = '/CustomAdmin/Events/AllTags';

        // this is the init function called at the end of the item-index.js page ready function
        // put all your custom methods here
        admin.items.index.pageInit = function () {
            admin.tags.init($('#manage-tags-form'));
        };

        // custom logic goes here to run during new item tab init
        admin.items.index.tabInit = function ($editableParent) {
            $editableParent.find('.edit-associated-tags').on('click', function () { admin.items.index.tagClick(this, $editableParent) })
        };
    })();
})(jQuery);