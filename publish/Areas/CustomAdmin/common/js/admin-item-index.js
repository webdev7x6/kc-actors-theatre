/// <reference path="http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.8.3-vsdoc.js" />
/// <reference path="http://clickfarmcdn.localhost/common/js/kendo/2012.2.710/kendo.all-vsdoc.js" />

(function ($, undefined) {

    var admin = window.admin = window.admin || {};
    admin.items = admin.items = admin.items || {};

    (function () {

        admin.items.index = (function () {
            return {
                init: function (domContainer, viewModel) {
                    this.initButtons(domContainer);
                    this.initAccordion(domContainer);
                    return this;
                },
                initButtons: function (domContainer) {

                    $('#create-item')
                        .button({
                            icons: {
                                primary: 'ui-icon-plusthick'
                            }
                        })
                        .on('click', function (e) {
                            e.preventDefault();
                            createItemManager.showCreateItemForm();
                        })
                    ;

                    $('#close-find-items-results')
                        .button({
                            icons: {
                                primary: 'ui-icon-circle-close'
                            },
                            text: false
                        })
                    ;

                    cms.makeCloseButton('#close-find-items-results', '#close-tabs-link');


                },
                initAccordion: function (domContainer) {
                    $('#start-options', domContainer).accordion({
                        fillSpace: true
                    });
                },
                findItemsSucceeded: function (data) {
                    $('.approve-item-link')
                        .button({
                            icons: {
                                primary: 'ui-icon-check'
                            },
                            text: false
                        })
                    ;
                    $('.unapprove-item-link')
                        .button({
                            icons: {
                                primary: 'ui-icon-cancel'
                            },
                            text: false
                        })
                    ;
                    $('.delete-item-link')
                        .button({
                            icons: {
                                primary: 'ui-icon-trash'
                            },
                            text: false
                        })
                    ;
                },
                findItemsFailed: function (data) {
                    //
                },
                initEditItemTab: function (panel) {

                    var $editableParent = panel.find('.editable-parent'),
                        $refreshLink = $editableParent.find('.refresh-item-link')
                            .button({
                                icons: {
                                    primary: 'ui-icon-refresh'
                                },
                                text: false
                            })
                            .on('click', function (e) {
                                e.preventDefault();
                                var itemsMgr = admin.items.getClosestItemsManager(panel);
                                if (itemsMgr) {
                                    var viewModel = itemsMgr.getViewModel();
                                    if (viewModel) {
                                        viewModel.tabMgr.reloadTabByID(admin.items.uniqueIDForTab($refreshLink.attr('data-item-id')));
                                    }
                                }
                            }),


                        $deleteLink = $editableParent.find('.delete-item-link')
						    .button({
						        icons: {
						            primary: 'ui-icon-trash'
						        },
						        text: false
						    })
						    .on('click', function (e) {
						        e.preventDefault();
						        var itemID = $(this).attr('data-item-id');
						        dialogHelper.confirm(
								    admin.itemDeleteDescription,
								    'Delete',
								    function () {
								        ajaxHelper.ajax(admin.deleteItemURL, {
								            data: {
								                id: itemID
								            },
								            type: 'POST',
								            success: function (data, textStatus, jqXHR) {
								                var itemsMgr = admin.items.getClosestItemsManager(panel);
								                var viewModel = itemsMgr.getViewModel();
								                viewModel.removeItem(itemID);
								            },
								            failureMessageFormat: 'An error occurred trying to delete the ' + admin.itemDescription + ': [[errorThrown]]'
								        });
								    }
							    );
						    })

                        $customAction = admin.items.index.tabInit($editableParent)
                    ;

                    var editableManager = new EditableManager(
                        $editableParent,
                        {
                            id: $editableParent.attr('data-item-id')
                        },
                        admin.editInPlaceURL
                    );
                    editableManager.initAllTypes();

                }
            };
        })();
    })();
})(jQuery);


$(function () {
    var domContainer = $('#items'),
        $findByName = $('#find-items-name')
    ;

    var itemsManager = new admin.items.ItemsManager({
        domContainer: domContainer,
        applying_bindings: function () {
            var viewModel = itemsManager.getViewModel();
            viewModel.tabMgr = new TabManager({
                closable: true,
                container: $('#tabs'),
                numStaticTabs: 1,

                load_tabTypeSelector: '.item-index-tab',
                load_tabTypeActions: {
                    "edit-item": admin.items.index.initEditItemTab
                },
                show_action: function (event, ui, tabMgr) {
                    if (ui.newTab.index() === 0) {
                        $findByName.select();
                    }
                }
            });
        }
    });
    itemsManager.init();
    var viewModel = itemsManager.getViewModel();
    viewModel.item_removed = function (itemID) {
        this.tabMgr.removeTabByID(admin.items.uniqueIDForTab(itemID));
    };
    admin.items.index.init(domContainer, viewModel);
    admin.items.initFindItemInput($findByName, viewModel, domContainer, null);
    admin.items.initShowAllItemsLink('#show-all-items', viewModel, domContainer, null);
    createItemManager = new admin.items.NewItemManager('#create-item-form');
    createItemManager.init();
    admin.items.index.pageInit();
});