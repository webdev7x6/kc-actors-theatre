(function ($, undefined) {
    var admin = window.admin = window.admin || {};
    (function () {

        // entity descriptions
        admin.itemDescription = 'Resources';
        admin.itemDeleteDescription = 'Are you sure you want to delete this resource?';

        // entity CRUD uperation URLs
        admin.getAllItemsURL = '/CustomAdmin/Resources/All?resourceType=' + RESOURCE_TYPE;
        admin.findItemsURL = '/CustomAdmin/Resources/Find?resourceType=' + RESOURCE_TYPE;
        admin.editItemURL = '/CustomAdmin/Resources/Edit';
        admin.deleteItemURL = '/CustomAdmin/Resources/Delete';
        admin.editInPlaceURL = '/CustomAdmin/Resources/Edit';

        // custom operation URLs
        admin.AddTagURL = '/CustomAdmin/Resources/AddTag';
        admin.RemoveTagURL = '/CustomAdmin/Resources/RemoveTag';
        admin.FindTagsURL = '/CustomAdmin/Resources/FindTags';
        admin.AllTagsURL = '/CustomAdmin/Resources/AllTags';


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