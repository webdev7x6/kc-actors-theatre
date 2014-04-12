/// <reference path="http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.8.3-vsdoc.js" />
/// <reference path="http://clickfarmcdn.localhost/common/js/knockout/2.1.0/knockout.js" />
/// <reference path="http://clickfarmcdn.localhost/common/js/kendo/2012.2.710/kendo.all-vsdoc.js" />

(function ($, undefined) {
    var admin = window.admin = window.admin || {};
    (function () {
        admin.items.index.categoryClick = function (container, parent) {
            var
                $container = $(container),
                categories = [],
                $categories = $container.find('ul')
            ;
            $categories.find('li').each(function _categories_onEach(ix, el) {
                var $el = $(el);
                categories.push({
                    categoryID: $el.attr('data-category-id'),
                    name: $el.text()
                });
            });
            admin.categories.show(categories, parent.attr('data-item-id'), function _categories_onClose(categories) {
                $container.empty();
                if (categories && categories.length) {
                    if (!$categories || !$categories.length) {
                        $categories = $('<ul/>');
                    }
                    $categories.empty().appendTo($container);
                    $.each(categories, function _categories_onEach(ix, item) {
                        $categories.append($('<li/>').text(item.name).attr('data-category-id', item.categoryID));
                    });
                }
                else {
                    $container.append('Click to edit');
                }
            });
        }

        admin.categories = (function () {
            var $dlg = null,
                closeCallback = null,
                ITEM_ID = null,
                $associatedCategories = null,
                associatedCategoriesViewModel = {
                    categories: ko.observableArray([])
                },
                $availableCategories = null,
                $availableCategoriesFindName = null,
                availableCategoriesViewModel = null,
                $currentCmsRole = null
            ;
            return {
                init: function _init($modal) {
                    $associatedCategories = $('#sortable-assigned-categories');//.on('dblclick', 'li', switchLists);
                    $dlg = $modal.dialog({
                        autoOpen: false,
                        height: 600,
                        modal: true,
                        resizable: false,
                        width: 930,
                        buttons: {
                            Close: function _closeBtn_onClick() {
                                $dlg.dialog('close');
                            }
                        },
                        close: function _dlg_onClose() {
                            if ($.isFunction(closeCallback)) {
                                var categories = [];
                                $associatedCategories.find('li').each(function _categories_onEach(ix, el) {
                                    var $el = $(el);
                                    if ($el.css('display') != 'none') // only add if not hidden
                                    {
                                        categories.push({
                                            categoryID: $el.attr('data-category-id'),
                                            name: $el.find('.category-name').text()
                                        });
                                    }
                                });
                                closeCallback(categories.sort(function _categories_onSort(a, b) {
                                    return a.name.toLowerCase() === b.name.toLowerCase() ? 0 : (a.name.toLowerCase() < b.name.toLowerCase() ? -1 : 1);
                                }));
                            }
                        }
                    });
                    ko.applyBindings(associatedCategoriesViewModel, $associatedCategories[0]);

                    var $dom = $('#available-categories-container');
                    var categoryManager = new admin.categories.CategoryManager({
                        domContainer: $dom,
                        find_succeeded: function () { },
                        find_failed: function () { },
                        applying_bindings: function () { }
                    });
                    categoryManager.init();
                    availableCategoriesViewModel = categoryManager.getViewModel();
                    $availableCategories = $('#sortable-available-categories');//.on('dblclick', 'li', switchLists);
                    var oldClearCategories = availableCategoriesViewModel.clearCategories;
                    availableCategoriesViewModel.clearCategories = function () {
                        oldClearCategories.call(availableCategoriesViewModel);
                        $availableCategories.empty();
                    };
                    $availableCategoriesFindName = $('#find-categories-name');
                    admin.categories.initFindCategoryInput($availableCategoriesFindName, availableCategoriesViewModel, $dom);
                    admin.categories.initShowAllCategoriesLink('#show-all-categories', availableCategoriesViewModel, $dom);
                    availableCategoriesViewModel.categories.subscribe(function _categories_subscribe(newValue) {
                        var badIxs = [];
                        $.each(newValue, function _newCategories_onEach(avIx, avItem) {
                            $.each(associatedCategoriesViewModel.categories(), function _associatedCategories_onEach(asIx, asItem) {
                                if (parseInt(avItem.categoryID) === parseInt(asItem.categoryID)) {
                                    badIxs.push(avIx);
                                }
                            });
                        });
                        $.each(badIxs.reverse(), function _badIxs_onEach(ix, item) {
                            availableCategoriesViewModel.categories.splice(item, 1);
                        });
                    });

                    $modal.find('[id^=sortable-]').sortable({
                        connectWith: '.sortable',
                        receive: listChanged
                    });

                    function listChanged(e, ui) {
                        updateServer($(ui.item));
                    }

                    // perform ui update for double clicking an item
                    function switchLists(e) {
                        var $target = $(e.currentTarget),
                            $parent = $target.parent(),
                            otherList = $($parent.sortable('option', 'connectWith')).not($parent)
                        ;

                        // if the current list has no items, add a hidden one to keep style in place
                        // when saving you will need to filter out items that have
                        // display set to none to accommodate this scenario
                        if ($target.siblings().length == 0) {
                            $target.clone().appendTo($parent).css('display', 'none');
                        }
                        otherList.append(e.currentTarget);

                        // remove any hidden children
                        otherList.children(':hidden').remove();

                        updateServer($target);
                    }

                    function updateServer(elem) {
                        var categoryID = elem.attr('data-category-id');
                        if (elem.parent().is('[id*="available"]')) {
                            admin.categories.removeCategory(categoryID, ITEM_ID, elem.sender);
                        } else {
                            admin.categories.addCategory(categoryID, ITEM_ID, elem.sender);
                        }
                    }
                },
                show: function _show(categories, id, onClose) {
                    associatedCategoriesViewModel.categories([]);
                    $associatedCategories.empty();
                    $.each(categories, function _categories_onEach(ix, item) {
                        associatedCategoriesViewModel.categories.push(item);
                    });
                    ITEM_ID = id;
                    closeCallback = onClose;
                    availableCategoriesViewModel.clearCategories();
                    availableCategoriesViewModel.retrievingCategories(false);
                    availableCategoriesViewModel.initialSearch(true);
                    $availableCategoriesFindName.val('');
                    $dlg.dialog('open');
                },
                addCategory: function _addCategory(categoryID, id, sortableList) {
                    ajaxHelper.ajax(admin.AddCategoryURL, {
                        type: 'POST',
                        data: {
                            id: id,
                            categoryID: categoryID
                        },
                        success: function (data, textStatus, jqXHR) {
                        },
                        failure: function (data, textStatus, jqXHR) {
                            dialogHelper.alert(data.Message);
                            $(sortableList).sortable('cancel');
                        },
                        failureMessageFormat: 'An error occurred trying to add the category: [[errorThrown]]'
                    });
                },
                removeCategory: function _removeCategory(categoryID, id, sortableList) {
                    ajaxHelper.ajax(admin.RemoveCategoryURL, {
                        type: 'POST',
                        data: {
                            id: id,
                            categoryID: categoryID
                        },
                        success: function (data, textStatus, jqXHR) {
                        },
                        failure: function (data, textStatus, jqXHR) {
                            dialogHelper.alert(data.Message);
                            $(sortableList).sortable('cancel');
                        },
                        failureMessageFormat: 'An error occurred trying to remove the category: [[errorThrown]]'
                    });
                },

                sortList: function (ulID, sortOrder, callback) {
                    var $ul = $('#' + ulID),
                        $items = $ul.find('li').remove()
                    ;

                    // sort items
                    Array.prototype.sort.call($items, function _array_onSort(a, b) {
                        var aHtml = $(a).html(),
                            bHtml = $(b).html()
                        ;
                        return aHtml === bHtml ? 0 : (aHtml < bHtml ? -1 : 1);
                    });
                    if (sortOrder === 'desc') {
                        Array.prototype.reverse.call($items);
                    }

                    // add sorted items back into list
                    $ul.append($items);

                    if ($.isFunction(callback)) {
                        callback();
                    }
                },

                //this function used to initialize the search categories by keyword input
                initFindCategoryInput: function (input, viewModel, domContainer) {
                    $(input).keyup($.debounce(350, function () {
                        if (input.val().length > 1) {
                            ajaxHelper.ajax(admin.FindCategoriesURL, {
                                data: {
                                    term: input.val()
                                },
                                type: 'POST',
                                success: function (data, textStatus, jqXHR) {
                                    viewModel.clearCategories();
                                    if (data.Succeeded && data.Properties.Categories.length > 0) {
                                        $.each(data.Properties.Categories, function (index, item) {
                                            //push results into view model's observable collection of views
                                            viewModel.addCategory(new admin.categories.Category(item.Name, item.PostCategoryID));
                                        });
                                        viewModel.anyCategories(true);
                                    }
                                    else {
                                        viewModel.anyCategories(false);
                                    }
                                    viewModel.findResultsVisible(true);

                                },
                                beforeSend: function (jqXHR, settings) {
                                    viewModel.retrievingCategories(true);
                                },
                                complete: function (jqXHR, textStatus) {
                                    viewModel.retrievingCategories(false);
                                    viewModel.initialSearch(false);
                                }
                            });
                        }
                    })
				)
                },

                initShowAllCategoriesLink: function (linkSelector, viewModel, domContainer) {
                    $(linkSelector, domContainer).button({
                        icons: {
                            primary: 'ui-icon-gear'
                        }
                    }).on('click', function (event) {
                        event.preventDefault();
                        ajaxHelper.ajax(admin.AllCategoriesURL, {
                            type: 'POST',
                            success: function (data, textStatus, jqXHR) {
                                viewModel.clearCategories();
                                if (data.Succeeded && data.Properties.Categories.length > 0) {
                                    $.each(data.Properties.Categories, function (index, item) {
                                        //push results into view model's observable collection of views
                                        viewModel.addCategory(new admin.categories.Category(item.Name, item.PostCategoryID));
                                    });
                                    viewModel.anyCategories(true);
                                }
                                else {
                                    viewModel.anyCategories(false);
                                }
                                viewModel.findResultsVisible(true);

                            },
                            beforeSend: function (jqXHR, settings) {
                                viewModel.retrievingCategories(true);
                            },
                            complete: function (jqXHR, textStatus) {
                                viewModel.retrievingCategories(false);
                                viewModel.initialSearch(false);
                            }
                        });
                    });
                },

                setCategoryManager: function (domContainer, categoryManager) {
                    $(domContainer).data('categoryMgr', categoryManager);
                },
                getCategoryManager: function (domContainer) {
                    return $(domContainer).data('categoryMgr');
                },
                getClosestCategoryManager: function (ancestor) {
                    return $(ancestor).closest('.category-container').data('categoryMgr');
                }
            };
        })();

        admin.categories.CategoryManager = function (options) {
            this.options = options;
            this.domContainer = $(options.domContainer);
            admin.categories.setCategoryManager(this.domContainer, this);
            this.ajaxUrl = options.ajaxUrl;
            this.ajaxData = options.ajaxData;
        };

        admin.categories.CategoryManager.prototype = function () {
            var
                init = function () {
                    var viewModel = new admin.categories.CategoryViewModel(this);
                    this.setViewModel.call(this, viewModel);
                    cms.doCallback(this.options.applying_bindings);
                    ko.applyBindings(viewModel, this.domContainer.get(0));
                },

                setViewModel = function (newViewModel) {
                    this.domContainer.data('categoryViewModel', newViewModel);
                },

                getViewModel = function () {
                    return this.domContainer.data('categoryViewModel');
                }
            ;

            return {
                init: init,
                setViewModel: setViewModel,
                getViewModel: getViewModel
            };

        }();

        admin.categories.CategoryViewModel = function (categoryMgr, category_removed) {
            this.categoryMgr = categoryMgr;
            this.category_removed = category_removed;
        };

        admin.categories.CategoryViewModel.prototype = function () {
            var
                categories = ko.observableArray([]),
                addCategory = function (category) {
                    category.setViewModel(this);
                    this.categories.push(category);
                },
                anyCategories = function () {
                    return this.categories().length > 0;
                },
                clearCategories = function () {
                    this.categories([]);
                },
                removeCategory = function (categoryID) {
                    this.categories.remove(function (item) {
                        return parseInt(item.categoryID) === parseInt(categoryID)
                    });
                    if ($.isFunction(this.category_removed)) {
                        this.category_removed(categoryID);
                    };
                },
                closeFindResults = function () {
                    this.findResultsVisible(false);
                },
                retrievingCategories = ko.observable(false),
                initialSearch = ko.observable(true),
                findResultsVisible = ko.observable(false)
            ;

            return {
                categories: categories,
                addCategory: addCategory,
                anyCategories: anyCategories,
                clearCategories: clearCategories,
                removeCategory: removeCategory,
                closeFindResults: closeFindResults,
                retrievingCategories: retrievingCategories,
                initialSearch: initialSearch,
                findResultsVisible: findResultsVisible
            };
        }();

        admin.categories.Category = function (name, categoryID) {
            this.name = name;
            this.categoryID = categoryID;
            this.viewModel = {};
        };
        admin.categories.Category.prototype = function () {
            var
                setViewModel = function (viewModel) {
                    this.viewModel = viewModel;
                }
            ;

            return {
                setViewModel: setViewModel
            };
        }();
    })();
})(jQuery);