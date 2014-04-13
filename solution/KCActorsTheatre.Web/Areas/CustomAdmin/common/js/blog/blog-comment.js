(function ($, undefined) {

    var hn = window.hn = window.hn || {};
    hn.items = hn.items = hn.items || {};

    (function () {

        // default entity descriptions
        hn.itemDescription = 'Blog Comment';
        hn.itemDeleteDescription = 'Are you sure you want to delete this comment?';

        // custom entity operation messages
        hn.itemModifyDescription = 'Are you sure you want perform this action?';

        // default CRUD operation URLs
        hn.getAllItemsURL = '/CustomAdmin/Blog/AllCommentsAjax';
        hn.findItemsURL = '/CustomAdmin/Blog/FindCommentsAjax';
        hn.editItemURL = '/CustomAdmin/Blog/EditCommentAjax';
        hn.deleteItemURL = '/CustomAdmin/Blog/DeleteCommentAjax';
        hn.editInPlaceURL = '/CustomAdmin/Blog/EditCommentAjax';

        // custom operation URLs
        hn.modifyCommentsURL = '/CustomAdmin/Blog/ModifyComments';

        // this is the init function called at the end of the hn-item-index.js page ready function
        // put all your custom methods here
        hn.items.index.pageInit = function () {

            // custom index button actions
            $('#approve-items')
                .button({
                    icons: {
                        primary: 'ui-icon-check'
                    },
                    text: true
                })
                .on('click', function (e) {
                    e.preventDefault();
                    hn.items.index.performMultipleAction('Approve', function () {
                        // callback
                    });
                })
            ;

            $('#unapprove-items')
                .button({
                    icons: {
                        primary: 'ui-icon-cancel'
                    },
                    text: true
                })
                .on('click', function (e) {
                    e.preventDefault();
                    hn.items.index.performMultipleAction('Unapprove', function () {
                        // callback
                    });
                })
            ;

            $('#delete-items')
                .button({
                    icons: {
                        primary: 'ui-icon-trash'
                    },
                    text: true
                })
                .on('click', function (e) {
                    e.preventDefault();
                    hn.items.index.performMultipleAction('Delete', function () {
                        // callback
                    });
                })
            ;

            $('#toggle-checked-list').change(function () {
                if ($(this).is(":checked"))
                    $('.item-checkbox').attr('checked', true);
                else
                    $('.item-checkbox').attr('checked', false);
            });

            // custom ViewModel actions
            hn.items.ItemsViewModel.prototype.modifyComment = function (item, action) {
                dialogHelper.confirm(
                    hn.itemModifyDescription,
                    action + ' Comment',
                    function () {
                        var idArray = [item.ID()];
                        hn.items.index.modifyComments(idArray, action, function () {
                            // callback
                        });
                    }
                );
            };

            // custom page actions
            hn.items.index.performMultipleAction = function (action, callback) {
                var idArray = [];
                $('.item-checkbox:checked').each(function (index, value) {
                    idArray.push($(value).attr('data-item-id'));
                });
                if (idArray.length > 0) {
                    dialogHelper.confirm(
                        hn.itemModifyDescription,
                        action + ' Multiple Comments',
                        function () {
                            hn.items.index.modifyComments(idArray, action, function () {
                                callback();
                            });
                        }
                    );
                }
            };

            hn.items.index.modifyComments = function (idArray, action, callback) {
                ajaxHelper.ajax(hn.modifyCommentsURL, {
                    data: {
                        idArray: idArray,
                        action: action
                    },
                    traditional: true,
                    type: 'POST',
                    success: function () {
                        var manager = hn.items.getItemsManager('#items');
                        var vm = manager.getViewModel();
                        // loop through each id, find its item, and then update properties based on action
                        idArray.forEach(function (id) {
                            var item = ko.utils.arrayFirst(vm.items(), function (_item) {
                                return _item.ID() == id;
                            });
                            switch (action) {
                                case 'Approve':
                                    item.ApprovalText('Yes');
                                    item.IsApproved(true);
                                    break;
                                case 'Unapprove':
                                    item.ApprovalText('No');
                                    item.IsApproved(false);
                                    break;
                                case 'Delete':
                                    vm.removeItem(item.ID());
                                    break;
                            }
                        });
                        callback();
                    },
                    failureMessageFormat: 'An error occurred trying to modify the ' + hn.itemDescription + ': [[errorThrown]]'
                })
            };
        };
    })();
})(jQuery);