/// <reference path="http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.8.3-vsdoc.js" />
/// <reference path="http://clickfarmcdn.localhost/common/js/knockout/2.1.0/knockout.js" />
/// <reference path="http://clickfarmcdn.localhost/common/js/kendo/2012.2.710/kendo.all-vsdoc.js" />

(function ($, undefined) {
    var admin = window.admin = window.admin || {};
    (function () {
        admin.items.index.tagClick = function (container, parent) {
            var
                $container = $(container),
                tags = [],
                $tags = $container.find('ul')
            ;
            $tags.find('li').each(function _tags_onEach(ix, el) {
                var $el = $(el);
                tags.push({
                    tagID: $el.attr('data-tag-id'),
                    name: $el.text()
                });
            });
            admin.tags.show(tags, parent.attr('data-item-id'), function _tags_onClose(tags) {
                $container.empty();
                if (tags && tags.length) {
                    if (!$tags || !$tags.length) {
                        $tags = $('<ul/>');
                    }
                    $tags.empty().appendTo($container);
                    $.each(tags, function _tags_onEach(ix, item) {
                        $tags.append($('<li/>').text(item.name).attr('data-tag-id', item.tagID));
                    });
                }
                else {
                    $container.append('Click to edit');
                }
            });
        }

        admin.tags = (function () {
            var $dlg = null,
                closeCallback = null,
                ITEM_ID = null,
                $associatedTags = null,
                associatedTagsViewModel = {
                    tags: ko.observableArray([])
                },
                $availableTags = null,
                $availableTagsFindName = null,
                availableTagsViewModel = null,
                $currentCmsRole = null
            ;
            return {
                init: function _init($modal) {
                    $associatedTags = $('#sortable-assigned-tags');//.on('dblclick', 'li', switchLists);
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
                                var tags = [];
                                $associatedTags.find('li').each(function _tags_onEach(ix, el) {
                                    var $el = $(el);
                                    if ($el.css('display') != 'none') // only add if not hidden
                                    {
                                        tags.push({
                                            tagID: $el.attr('data-tag-id'),
                                            name: $el.find('.tag-name').text()
                                        });
                                    }
                                });
                                closeCallback(tags.sort(function _tags_onSort(a, b) {
                                    return a.name.toLowerCase() === b.name.toLowerCase() ? 0 : (a.name.toLowerCase() < b.name.toLowerCase() ? -1 : 1);
                                }));
                            }
                        }
                    });
                    ko.applyBindings(associatedTagsViewModel, $associatedTags[0]);

                    var $dom = $('#available-tags-container');
                    var tagManager = new admin.tags.TagManager({
                        domContainer: $dom,
                        find_succeeded: function () { },
                        find_failed: function () { },
                        applying_bindings: function () { }
                    });
                    tagManager.init();
                    availableTagsViewModel = tagManager.getViewModel();
                    $availableTags = $('#sortable-available-tags');//.on('dblclick', 'li', switchLists);
                    var oldClearTags = availableTagsViewModel.clearTags;
                    availableTagsViewModel.clearTags = function () {
                        oldClearTags.call(availableTagsViewModel);
                        $availableTags.empty();
                    };
                    $availableTagsFindName = $('#find-tags-name');
                    admin.tags.initFindTagInput($availableTagsFindName, availableTagsViewModel, $dom);
                    admin.tags.initShowAllTagsLink('#show-all-tags', availableTagsViewModel, $dom);
                    availableTagsViewModel.tags.subscribe(function _tags_subscribe(newValue) {
                        var badIxs = [];
                        $.each(newValue, function _newTags_onEach(avIx, avItem) {
                            $.each(associatedTagsViewModel.tags(), function _associatedTags_onEach(asIx, asItem) {
                                if (parseInt(avItem.tagID) === parseInt(asItem.tagID)) {
                                    badIxs.push(avIx);
                                }
                            });
                        });
                        $.each(badIxs.reverse(), function _badIxs_onEach(ix, item) {
                            availableTagsViewModel.tags.splice(item, 1);
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
                        var tagID = elem.attr('data-tag-id');
                        if (elem.parent().is('[id*="available"]')) {
                            admin.tags.removeTag(tagID, ITEM_ID, elem.sender);
                        } else {
                            admin.tags.addTag(tagID, ITEM_ID, elem.sender);
                        }
                    }
                },
                show: function _show(tags, id, onClose) {
                    associatedTagsViewModel.tags([]);
                    $associatedTags.empty();
                    $.each(tags, function _tags_onEach(ix, item) {
                        associatedTagsViewModel.tags.push(item);
                    });
                    ITEM_ID = id;
                    closeCallback = onClose;
                    availableTagsViewModel.clearTags();
                    availableTagsViewModel.retrievingTags(false);
                    availableTagsViewModel.initialSearch(true);
                    $availableTagsFindName.val('');
                    $dlg.dialog('open');
                },
                addTag: function _addTag(tagID, id, sortableList) {
                    ajaxHelper.ajax(admin.AddTagURL, {
                        type: 'POST',
                        data: {
                            id: id,
                            tagID: tagID
                        },
                        success: function (data, textStatus, jqXHR) {
                        },
                        failure: function (data, textStatus, jqXHR) {
                            dialogHelper.alert(data.Message);
                            $(sortableList).sortable('cancel');
                        },
                        failureMessageFormat: 'An error occurred trying to add the tag: [[errorThrown]]'
                    });
                },
                removeTag: function _removeTag(tagID, id, sortableList) {
                    ajaxHelper.ajax(admin.RemoveTagURL, {
                        type: 'POST',
                        data: {
                            id: id,
                            tagID: tagID
                        },
                        success: function (data, textStatus, jqXHR) {
                        },
                        failure: function (data, textStatus, jqXHR) {
                            dialogHelper.alert(data.Message);
                            $(sortableList).sortable('cancel');
                        },
                        failureMessageFormat: 'An error occurred trying to remove the tag: [[errorThrown]]'
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

                //this function used to initialize the search tags by keyword input
                initFindTagInput: function (input, viewModel, domContainer) {
                    $(input).keyup($.debounce(350, function () {
                        if (input.val().length > 1) {
                            ajaxHelper.ajax(admin.FindTagsURL, {
                                data: {
                                    term: input.val()
                                },
                                type: 'POST',
                                success: function (data, textStatus, jqXHR) {
                                    viewModel.clearTags();
                                    if (data.Succeeded && data.Properties.Tags.length > 0) {
                                        $.each(data.Properties.Tags, function (index, item) {
                                            //push results into view model's observable collection of views
                                            viewModel.addTag(new admin.tags.Tag(item.Name, item.TagID));
                                        });
                                        viewModel.anyTags(true);
                                    }
                                    else {
                                        viewModel.anyTags(false);
                                    }
                                    viewModel.findResultsVisible(true);

                                },
                                beforeSend: function (jqXHR, settings) {
                                    viewModel.retrievingTags(true);
                                },
                                complete: function (jqXHR, textStatus) {
                                    viewModel.retrievingTags(false);
                                    viewModel.initialSearch(false);
                                }
                            });
                        }
                    })
				)
                },

                initShowAllTagsLink: function (linkSelector, viewModel, domContainer) {
                    $(linkSelector, domContainer).button({
                        icons: {
                            primary: 'ui-icon-gear'
                        }
                    }).on('click', function (event) {
                        event.preventDefault();
                        ajaxHelper.ajax(admin.AllTagsURL, {
                            type: 'POST',
                            success: function (data, textStatus, jqXHR) {
                                viewModel.clearTags();
                                if (data.Succeeded && data.Properties.Tags.length > 0) {
                                    $.each(data.Properties.Tags, function (index, item) {
                                        //push results into view model's observable collection of views
                                        viewModel.addTag(new admin.tags.Tag(item.Name, item.TagID));
                                    });
                                    viewModel.anyTags(true);
                                }
                                else {
                                    viewModel.anyTags(false);
                                }
                                viewModel.findResultsVisible(true);

                            },
                            beforeSend: function (jqXHR, settings) {
                                viewModel.retrievingTags(true);
                            },
                            complete: function (jqXHR, textStatus) {
                                viewModel.retrievingTags(false);
                                viewModel.initialSearch(false);
                            }
                        });
                    });
                },

                setTagManager: function (domContainer, tagManager) {
                    $(domContainer).data('tagMgr', tagManager);
                },
                getTagManager: function (domContainer) {
                    return $(domContainer).data('tagMgr');
                },
                getClosestTagManager: function (ancestor) {
                    return $(ancestor).closest('.tag-container').data('tagMgr');
                }
            };
        })();

        admin.tags.TagManager = function (options) {
            this.options = options;
            this.domContainer = $(options.domContainer);
            admin.tags.setTagManager(this.domContainer, this);
            this.ajaxUrl = options.ajaxUrl;
            this.ajaxData = options.ajaxData;
        };

        admin.tags.TagManager.prototype = function () {
            var
                init = function () {
                    var viewModel = new admin.tags.TagViewModel(this);
                    this.setViewModel.call(this, viewModel);
                    cms.doCallback(this.options.applying_bindings);
                    ko.applyBindings(viewModel, this.domContainer.get(0));
                },

                setViewModel = function (newViewModel) {
                    this.domContainer.data('tagViewModel', newViewModel);
                },

                getViewModel = function () {
                    return this.domContainer.data('tagViewModel');
                }
            ;

            return {
                init: init,
                setViewModel: setViewModel,
                getViewModel: getViewModel
            };

        }();

        admin.tags.TagViewModel = function (tagMgr, tag_removed) {
            this.tagMgr = tagMgr;
            this.tag_removed = tag_removed;
        };

        admin.tags.TagViewModel.prototype = function () {
            var
                tags = ko.observableArray([]),
                addTag = function (tag) {
                    tag.setViewModel(this);
                    this.tags.push(tag);
                },
                anyTags = function () {
                    return this.tags().length > 0;
                },
                clearTags = function () {
                    this.tags([]);
                },
                removeTag = function (tagID) {
                    this.tags.remove(function (item) {
                        return parseInt(item.tagID) === parseInt(tagID)
                    });
                    if ($.isFunction(this.tag_removed)) {
                        this.tag_removed(tagID);
                    };
                },
                closeFindResults = function () {
                    this.findResultsVisible(false);
                },
                retrievingTags = ko.observable(false),
                initialSearch = ko.observable(true),
                findResultsVisible = ko.observable(false)
            ;

            return {
                tags: tags,
                addTag: addTag,
                anyTags: anyTags,
                clearTags: clearTags,
                removeTag: removeTag,
                closeFindResults: closeFindResults,
                retrievingTags: retrievingTags,
                initialSearch: initialSearch,
                findResultsVisible: findResultsVisible
            };
        }();

        admin.tags.Tag = function (name, tagID) {
            this.name = name;
            this.tagID = tagID;
            this.viewModel = {};
        };
        admin.tags.Tag.prototype = function () {
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