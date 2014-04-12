(function ($, undefined) {
    var admin = window.admin = window.admin || {};
    (function () {

        // entity descriptions
        admin.itemDescription = 'Blog Posts';
        admin.itemDeleteDescription = 'Are you sure you want to delete this blog post?';

        // entity CRUD uperation URLs
        admin.getAllItemsURL = '/CustomAdmin/Blogs/AllByType?blogType=' + TYPE;
        admin.findItemsURL = '/CustomAdmin/Blogs/FindByType?blogType=' + TYPE;
        admin.editItemURL = '/CustomAdmin/Blogs/Edit';
        admin.deleteItemURL = '/CustomAdmin/Blogs/Delete';
        admin.editInPlaceURL = admin.editItemURL;

        // custom operation URLs
        admin.AddTagURL = '/CustomAdmin/Blogs/AddTag';
        admin.RemoveTagURL = '/CustomAdmin/Blogs/RemoveTag';
        admin.FindTagsURL = '/CustomAdmin/Blogs/FindTags?blogType=' + TYPE;
        admin.AllTagsURL = '/CustomAdmin/Blogs/AllTags?blogType=' + TYPE;

        admin.AddCategoryURL = '/CustomAdmin/Blogs/AddCategory';
        admin.RemoveCategoryURL = '/CustomAdmin/Blogs/RemoveCategory';
        admin.FindCategoriesURL = '/CustomAdmin/Blogs/FindCategories';
        admin.AllCategoriesURL = '/CustomAdmin/Blogs/AllCategories';

        // this is the init function called at the end of the item-index.js page ready function
        // put all your custom methods here
        admin.items.index.pageInit = function () {
            admin.tags.init($('#manage-tags-form'));
            admin.categories.init($('#manage-categories-form'));
        };

        // custom logic goes here to run during new item tab init
        admin.items.index.tabInit = function ($editableParent) {
            $editableParent.find('.edit-associated-tags').on('click', function () { admin.items.index.tagClick(this, $editableParent) })
            $editableParent.find('.edit-associated-categories').on('click', function () { admin.items.index.categoryClick(this, $editableParent) })
        };
    })();
})(jQuery);