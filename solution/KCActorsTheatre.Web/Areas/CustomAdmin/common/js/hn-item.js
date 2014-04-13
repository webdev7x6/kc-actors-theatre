/// <reference path="http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.8.3-vsdoc.js" />
/// <reference path="http://clickfarmcdn.localhost/common/js/kendo/2012.2.710/kendo.all-vsdoc.js" />

(function ($, undefined) {
    var hn = window.hn = window.hn || {};
    hn.items = hn.items = hn.items || {};
    (function () {

        // disable click on anchor tags if !ko.enable
        var orgClickInit = ko.bindingHandlers.click.init;
        ko.bindingHandlers.click.init = function (element, valueAccessor, allBindingsAccessor, viewModel) {
            if (element.tagName === "A" && allBindingsAccessor().enable != null) {
                var disabled = ko.computed(function () {
                    return ko.utils.unwrapObservable(allBindingsAccessor().enable) === false;
                });
                ko.applyBindingsToNode(element, { css: { disabled: disabled } });
                var handler = valueAccessor();
                valueAccessor = function () {
                    return function () {
                        if (ko.utils.unwrapObservable(allBindingsAccessor().enable)) {
                            handler.apply(this, arguments);
                        }
                    }
                };

            }
            orgClickInit(element, valueAccessor, allBindingsAccessor, viewModel);
        };

        // disable jquery ui buttons if !ko.enable
        if (ko && ko.bindingHandlers) {
            ko.bindingHandlers['jEnable'] = {
                'update': function (element, valueAccessor) {
                    var value = ko.utils.unwrapObservable(valueAccessor());
                    var $element = $(element);
                    $element.prop("disabled", !value);

                    if ($element.hasClass("ui-button")) {
                        $element.button("option", "disabled", !value);
                    }
                }
            };
        }

        hn.items = (function () {
            return {
                setItemsManager: function (domContainer, dataManager) {
                    $(domContainer).data('itemsManager', dataManager);
                },
                getItemsManager: function (domContainer) {
                    return $(domContainer).data('itemsManager');
                },
                getClosestItemsManager: function (ancestor) {
                    return $(ancestor).closest('.items-container').data('itemsManager');
                },

                uniqueIDForTab: function (id) {
                    var tabID = 'edit-item-' + id;
                    return tabID;
                },

                initShowAllItemsLink: function (linkSelector, viewModel, domContainer, successCallback) {
                    $(linkSelector, domContainer).button({
                        icons: {
                            primary: 'ui-icon-gear'
                        }
                    }).on('click', function (event) {
                        event.preventDefault();
                        ajaxHelper.ajax(hn.getAllItemsURL, {
                            data: {},
                            type: 'POST',
                            success: function (data, textStatus, jqXHR) {
                                hn.items.processItems(domContainer, viewModel, data.Succeeded, data.Properties.Items);
                                hn.items.index.findItemsSucceeded(data);
                                cms.doCallback(successCallback);
                            },
                            beforeSend: function (jqXHR, settings) {
                                viewModel.retrievingItems(true);
                            },
                            complete: function (jqXHR, textStatus) {
                                viewModel.retrievingItems(false);
                            }
                        });
                    });
                },

                initFindItemInput: function (input, viewModel, domContainer, successCallback) {
                    $(input).keyup($.debounce(350, function () {
                        if (input.val().length > 2) {
                            ajaxHelper.ajax(hn.findItemsURL, {
                                data: {
                                    term: input.val()
                                },
                                type: 'POST',
                                success: function (data, textStatus, jqXHR) {
                                    hn.items.processItems(domContainer, viewModel, data.Succeeded, data.Properties.Items);
                                    hn.items.index.findItemsSucceeded(data);
                                    cms.doCallback(successCallback);
                                },
                                beforeSend: function (jqXHR, settings) {
                                    viewModel.retrievingItems(true);
                                },
                                complete: function (jqXHR, textStatus) {
                                    viewModel.retrievingItems(false);
                                }
                            });
                        }
                    }));
                },
                processItems: function (domContainer, viewModel, succeeded, items) {
                    viewModel.clearItems();
                    if (succeeded && items.length > 0) {
                        $.each(items, function (index, item) {
                            viewModel.addItem(new hn.items.Item(item));

                        });
                        viewModel.anyItems(true);
                    }
                    else {
                        viewModel.anyItems(false);
                    }
                    viewModel.findResultsVisible(true);
                }
            }
        })();

        hn.items.ItemsManager = function (options) {
            this.options = options;
            this.domContainer = $(options.domContainer);
            hn.items.setItemsManager(this.domContainer, this);
            this.ajaxUrl = options.ajaxUrl;
            this.ajaxData = options.ajaxData;
        };

        hn.items.ItemsManager.prototype = function () {
            var
                init = function () {
                    var viewModel = new hn.items.ItemsViewModel(this);
                    this.setViewModel.call(this, viewModel);
                    cms.doCallback(this.options.applying_bindings);
                    ko.applyBindings(viewModel, this.domContainer.get(0));
                },

                setViewModel = function (newViewModel) {
                    this.domContainer.data('itemsViewModel', newViewModel)
                },

                getViewModel = function () {
                    return this.domContainer.data('itemsViewModel');
                }
            ;

            return {
                init: init,
                setViewModel: setViewModel,
                getViewModel: getViewModel
            };
        }();

        hn.items.ItemsViewModel = function (dataManager, item_removed) {
            this.dataManager = dataManager;
            this.item_removed = item_removed; // callback
            this.items = ko.observableArray([]);
            this.recentItems = ko.observableArray([]);
            this.retrievingItems = ko.observable(true);
            this.findResultsVisible = ko.observable(false);
        };

        hn.items.ItemsViewModel.prototype = function () {
            var
                addItem = function (item) {
                    item.setViewModel(this);
                    this.items.push(item);
                },
                anyItems = function () {
                    return this.items().length > 0;
                },
                clearItems = function () {
                    this.items([]);
                },
                removeItem = function (id) {
                    this.items.remove(function (item) {
                        return parseInt(item.ID()) === parseInt(id)
                    });
                    if ($.isFunction(this.item_removed)) {
                        this.item_removed(id);
                    }
                },
                closeFindResults = function () {
                    this.findResultsVisible(false);
                },
                showEditItem = function (item) {
                    addRecentItem.call(this, item);
                    item.viewModel.tabMgr.addTab(new TabInfo(
                        hn.editItemURL + '/' + item.ID(),
                        cms.truncateStr(item.TabTitleString(), 20),
                        'edit-item',
                        hn.items.uniqueIDForTab(item.ID())
                    ));
                },
                addRecentItem = function (item) {
                    var ix = findRecentIndexByID.call(this, item.ID);
                    if (ix > -1) {
                        this.recentItems.splice(ix, 1);
                    }
                    else {
                        //only allow 10 recent entities
                        while (this.recentItems().length >= 10) {
                            this.recentItems.pop()
                        }
                    }
                    this.recentItems.unshift(item);
                },
                findRecentIndexByID = function (id) {
                    var ixOf = -1;
                    $.each(this.recentItems(), function (ix, item) {
                        if (item.ID === id) {
                            ixOf = ix;
                            return false;
                        }
                    });
                    return ixOf;
                },
                removeRecentByID = function (id) {
                    this.recentItems.remove(function (item) {
                        return item.ID === id
                    });
                }
            ;

            return {
                items: this.items,
                addItem: addItem,
                anyItems: anyItems,
                retrievingItems: this.retrievingItems,
                clearItems: clearItems,
                removeItem: removeItem,
                findResultsVisible: this.findResultsVisible,
                closeFindResults: closeFindResults,
                showEditItem: showEditItem,
                recentItems: this.recentItems
            };
        }();

        hn.items.Item = function (data) {
            var self = this;
            ko.mapping.fromJS(data, {}, self);
            this.viewModel = {}; //a reference to the Knockout viewModel within which this object is contained
        };
        hn.items.Item.prototype = function () {
            var setViewModel = function _setViewModel(viewModel) {
                this.viewModel = viewModel;
            }
            return {
                setViewModel: setViewModel
            };
        }();

    })();
})(jQuery);