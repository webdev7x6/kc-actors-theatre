/// <reference path="http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.8.3-vsdoc.js" />
/// <reference path="http://clickfarmcdn.localhost/common/js/knockout/2.1.0/knockout.js" />
/// <reference path="http://clickfarmcdn.localhost/common/js/kendo/2012.2.710/kendo.all-vsdoc.js" />

(function ($, undefined) {
    var admin = window.admin = window.admin || {};
    admin.tags = admin.tags = admin.tags || {};
    (function () {

        admin.FindTagsURL = '/CustomAdmin/Tags/Find';
        admin.AllTagsURL = '/CustomAdmin/Tags/All';

        // custom operation URLs
        admin.GetResourceWidgetTagsURL = '/CustomAdmin/Tags/GetWidgetTags';
        admin.AddResourceWidgetTagURL = '/CustomAdmin/Tags/AddWidgetTag';
        admin.RemoveResourceWidgetTagURL = '/CustomAdmin/Tags/RemoveWidgetTag';

        admin.tags = (function () {
            return {
                init: function ($domContainer, viewModel) {
                    // get tags
                    admin.tags.getTags(viewModel);

                    // make sortables
                    $domContainer.find('[class^=sortable-]').sortable({
                        connectWith: '.sortable',
                        receive: admin.tags.listChanged
                    });
                },

                listChanged: function (e, ui) {
                    admin.tags.updateServer($(ui.item));
                },

                updateServer: function ($elem) {
                    var tagsManager = admin.tags.getClosestTagsManager($elem);
                    var viewModel = tagsManager.getViewModel();
                    var tagID = $elem.attr('data-tag-id');
                    if ($elem.parent().is('[class*="unassigned"]')) {
                        admin.tags.removeTag(tagID, viewModel, $elem.sender);
                    } else {
                        admin.tags.addTag(tagID, viewModel, $elem.sender);
                    }
                    // remove element because we're now handling it with knockout
                    $elem.remove();
                },

                getTags: function _getTags(viewModel) {
                    ajaxHelper.ajax(admin.GetResourceWidgetTagsURL, {
                        type: 'POST',
                        data: {
                            contentID: viewModel.contentID()
                        },
                        success: function (data, textStatus, jqXHR) {
                            if (data.Properties != null) {
                                viewModel.clearAssignedTags();
                                if (data.Properties.Items.length > 0) {
                                    $.each(data.Properties.Items, function (index, tag) {
                                        var newTag = new admin.tags.Tag(tag);
                                        viewModel.addToAllTags(newTag);
                                        viewModel.addToAssignedTags(newTag);
                                    });
                                    viewModel.anyAssignedTags(true);
                                }
                                else {
                                    viewModel.anyAssignedTags(false);
                                }
                            }
                        },
                        failure: function (data, textStatus, jqXHR) {
                            dialogHelper.alert(data.Message);
                        },
                        failureMessageFormat: 'An error occurred trying to get the tag list: [[errorThrown]]'
                    });
                },

                addTag: function _addTag(tagID, viewModel, sortableList) {
                    ajaxHelper.ajax(admin.AddResourceWidgetTagURL, {
                        type: 'POST',
                        data: {
                            contentID: viewModel.contentID(),
                            tagID: tagID
                        },
                        success: function (data, textStatus, jqXHR) {
                            var newTag = new admin.tags.Tag(data.Properties.Item);
                            viewModel.assignedTags.push(newTag);
                        },
                        failure: function (data, textStatus, jqXHR) {
                            dialogHelper.alert(data.Message);
                            $(sortableList).sortable('cancel');
                        },
                        failureMessageFormat: 'An error occurred trying to add the tag: [[errorThrown]]'
                    });
                },
                removeTag: function _removeTag(tagID, viewModel, sortableList) {
                    ajaxHelper.ajax(admin.RemoveResourceWidgetTagURL, {
                        type: 'POST',
                        data: {
                            contentID: viewModel.contentID(),
                            tagID: tagID
                        },
                        success: function (data, textStatus, jqXHR) {
                            var newTag = new admin.tags.Tag(data.Properties.Item);
                            viewModel.unassignedTags.push(newTag);
                        },
                        failure: function (data, textStatus, jqXHR) {
                            dialogHelper.alert(data.Message);
                            $(sortableList).sortable('cancel');
                        },
                        failureMessageFormat: 'An error occurred trying to remove the tag: [[errorThrown]]'
                    });
                },

                setTagsManager: function (domContainer, dataManager) {
                    $(domContainer).data('tagsManager', dataManager);
                },
                getTagsManager: function (domContainer) {
                    return $(domContainer).data('tagsManager');
                },
                getClosestTagsManager: function (ancestor) {
                    return $(ancestor).closest('.content-editor').data('tagsManager');
                },

                initShowAllTagsLink: function (linkSelector, viewModel, domContainer, successCallback) {
                    $(linkSelector, domContainer).button({
                        icons: {
                            primary: 'ui-icon-gear'
                        }
                    }).on('click', function (event) {
                        event.preventDefault();
                        ajaxHelper.ajax(admin.AllTagsURL, {
                            data: {
                                tagType: viewModel.tagType()
                            },
                            type: 'POST',
                            success: function (data, textStatus, jqXHR) {
                                admin.tags.processUnassignedTags(domContainer, viewModel, data.Succeeded, data.Properties.Items);
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

                initFindTagInput: function (input, viewModel, domContainer, successCallback) {
                    $(input).keyup($.debounce(350, function () {
                        if (input.val().length > 2) {
                            ajaxHelper.ajax(admin.FindTagsURL, {
                                data: {
                                    term: input.val(),
                                    tagType: viewModel.tagType()
                                },
                                type: 'POST',
                                success: function (data, textStatus, jqXHR) {
                                    admin.tags.processUnassignedTags(domContainer, viewModel, data.Succeeded, data.Properties.Items);
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
                    }));
                },
                processUnassignedTags: function (domContainer, viewModel, succeeded, tags) {
                    viewModel.clearUnassignedTags();
                    if (succeeded && tags.length > 0) {
                        $.each(tags, function (index, tag) {

                            var newTag = new admin.tags.Tag(tag);

                            // check if already in ul lists
                            var assignedMatch = domContainer.find('.sortable-assigned-tags li[data-tag-id="' + tag.ID + '"]');
                            var unassignedMatch = domContainer.find('.sortable-unassigned-tags li[data-tag-id="' + tag.ID + '"]');

                            if (assignedMatch.length == 0 && unassignedMatch.length == 0) {
                                viewModel.addToUnassignedTags(newTag);
                            }

                        });

                        viewModel.anyUnassignedTags(true);
                    }
                    else {
                        viewModel.anyUnassignedTags(false);
                    }
                    viewModel.findResultsVisible(true);
                }
            }
        })();

        admin.tags.TagsManager = function (options) {
            this.options = options;
            this.domContainer = $(options.domContainer);
            admin.tags.setTagsManager(this.domContainer, this);
        };

        admin.tags.TagsManager.prototype = function () {
            var
                init = function () {
                    var viewModel = new admin.tags.TagsViewModel(this);
                    this.setViewModel.call(this, viewModel);
                    cms.doCallback(this.options.applying_bindings);
                    ko.applyBindings(viewModel, this.domContainer.get(0));
                },

                setViewModel = function (newViewModel) {
                    this.domContainer.data('tagsViewModel', newViewModel)
                },

                getViewModel = function () {
                    return this.domContainer.data('tagsViewModel');
                }
            ;

            return {
                init: init,
                setViewModel: setViewModel,
                getViewModel: getViewModel
            };
        }();

        admin.tags.TagsViewModel = function (dataManager) {
            var self = this;
            self.dataManager = dataManager;
            self.contentID = ko.observable();
            self.tagType = ko.observable();
            self.allTags = ko.observableArray([]);
            self.assignedTags = ko.observableArray([]);
            self.unassignedTags = ko.observableArray([]);
            self.retrievingTags = ko.observable(true);
            self.initialSearch = ko.observable(true);
            self.findResultsVisible = ko.observable(false);

            self.unassignedTags.subscribe(function _unassignedTags_subscribe(newValue) {
                var badIxs = [];
                $.each(newValue, function _newTags_onEach(avIx, avTag) {
                    $.each(self.assignedTags(), function _assignedTags_onEach(asIx, asTag) {
                        if (parseInt(avTag.tagID) === parseInt(asTag.tagID)) {
                            badIxs.push(avIx);
                        }
                    });
                });
                $.each(badIxs.reverse(), function _badIxs_onEach(ix, tag) {
                    self.unassignedTags.splice(tag, 1);
                });
            });
        };

        admin.tags.TagsViewModel.prototype = function () {
            var
                addToAllTags = function (tag) {
                    this.allTags.push(tag);
                },
                removeFromAllTags = function (tag) {
                    this.allTags.remove(function (tag) {
                        return parseInt(tag.ID) === parseInt(id)
                    });
                },

                addToAssignedTags = function (tag) {
                    this.assignedTags.push(tag);
                },
                removeFromAssignedTags = function (id) {
                    this.assignedTags.remove(function (tag) {
                        return parseInt(tag.ID) === parseInt(id)
                    });
                },

                addToUnassignedTags = function (tag) {
                    this.unassignedTags.push(tag);
                },
                removeFromUnassignedTags = function (id) {
                    this.unassignedTags.remove(function (tag) {
                        return parseInt(tag.ID) === parseInt(id)
                    });
                },

                anyAssignedTags = function () {
                    return this.assignedTags().length > 0;
                },
                anyUnassignedTags = function () {
                    return this.unassignedTags().length > 0;
                },
                clearAssignedTags = function () {
                    this.assignedTags([]);
                },
                clearUnassignedTags = function () {
                    this.unassignedTags([]);
                },
                closeFindResults = function () {
                    this.findResultsVisible(false);
                }
            ;

            return {
                assignedTags: this.assignedTags,
                unassignedTags: this.unassignedTags,
                retrievingTags: this.retrievingTags,
                findResultsVisible: this.findResultsVisible,

                addToAllTags: addToAllTags,
                removeFromAllTags: removeFromAllTags,

                addToAssignedTags: addToAssignedTags,
                removeFromAssignedTags: removeFromAssignedTags,

                addToUnassignedTags: addToUnassignedTags,
                removeFromUnassignedTags: removeFromUnassignedTags,

                anyAssignedTags: anyAssignedTags,
                anyUnassignedTags: anyUnassignedTags,
                clearAssignedTags: clearAssignedTags,
                clearUnassignedTags: clearUnassignedTags,
                closeFindResults: closeFindResults
            };
        }();

        admin.tags.Tag = function (data) {
            var self = this;
            ko.mapping.fromJS(data, {}, self);
            this.viewModel = {};
        };
        admin.tags.Tag.prototype = function () {
            var setViewModel = function _setViewModel(viewModel) {
                this.viewModel = viewModel;
            }
            return {
                setViewModel: setViewModel
            };
        }();
    })();
})(jQuery);