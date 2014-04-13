/// <reference path="http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.8.3-vsdoc.js" />
/// <reference path="http://clickfarmcdn.localhost/common/js/kendo/2012.2.710/kendo.all-vsdoc.js" />

(function ($, undefined) {

    var hn = window.hn = window.hn || {};
    hn.items = hn.items = hn.items || {};

    (function () {

        hn.items.index = (function () {
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
                                var itemsMgr = hn.items.getClosestItemsManager(panel);
                                if (itemsMgr) {
                                    var viewModel = itemsMgr.getViewModel();
                                    if (viewModel) {
                                        viewModel.tabMgr.reloadTabByID(hn.items.uniqueIDForTab($refreshLink.attr('data-item-id')));
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
								    hn.itemDeleteDescription,
								    'Delete',
								    function () {
								        ajaxHelper.ajax(hn.deleteItemURL, {
								            data: {
								                id: itemID
								            },
								            type: 'POST',
								            success: function (data, textStatus, jqXHR) {
								                var itemsMgr = hn.items.getClosestItemsManager(panel);
								                var viewModel = itemsMgr.getViewModel();
								                viewModel.removeItem(itemID);
								            },
								            failureMessageFormat: 'An error occurred trying to delete the ' + hn.itemDescription + ': [[errorThrown]]'
								        });
								    }
							    );
						    })

                        //$tradersCont = $editableParent.find('.edit-assoc-traders').on('click', function () { hn.items.index.userClick(this, $editableParent, 'Trader') }),
                        //$managersCont = $editableParent.find('.edit-assoc-managers').on('click', function () { hn.items.index.userClick(this, $editableParent, 'Senior Manager') })
                    ;

                    var editableManager = new EditableManager(
                        $editableParent,
                        {
                            id: $editableParent.attr('data-item-id')
                        },
                        hn.editInPlaceURL
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

    var itemsManager = new hn.items.ItemsManager({
        domContainer: domContainer,
        applying_bindings: function () {
            var viewModel = itemsManager.getViewModel();
            viewModel.tabMgr = new TabManager({
                closable: true,
                container: $('#tabs'),
                numStaticTabs: 1,

                load_tabTypeSelector: '.item-index-tab',
                load_tabTypeActions: {
                    "edit-item": hn.items.index.initEditItemTab
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
        this.tabMgr.removeTabByID(hn.items.uniqueIDForTab(itemID));
    };
    hn.items.index.init(domContainer, viewModel);
    hn.items.initFindItemInput($findByName, viewModel, domContainer, null);
    hn.items.initShowAllItemsLink('#show-all-items', viewModel, domContainer, null);
    createItemManager = new hn.items.NewItemManager('#create-item-form');
    createItemManager.init();
    hn.items.index.pageInit();
});