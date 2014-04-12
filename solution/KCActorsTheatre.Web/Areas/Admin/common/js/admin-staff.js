/// <reference path="http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.8.3-vsdoc.js" />
/// <reference path="http://clickfarmcdn.localhost/common/js/knockout/2.1.0/knockout.js" />
/// <reference path="http://clickfarmcdn.localhost/common/js/kendo/2012.2.710/kendo.all-vsdoc.js" />

(function ($, undefined) {
    var admin = window.admin = window.admin || {};
    admin.staff = admin.staff = admin.staff || {};
    (function () {

        admin.FindStaffURL = '/CustomAdmin/StaffMembers/Find';
        admin.AllStaffURL = '/CustomAdmin/StaffMembers/All';

        // custom operation URLs
        admin.GetStaffURL = '/CustomAdmin/StaffMembers/GetWidgetStaff';
        admin.AddStaffURL = '/CustomAdmin/StaffMembers/AddWidgetStaff';
        admin.RemoveStaffURL = '/CustomAdmin/StaffMembers/RemoveWidgetStaff';

        admin.staff = (function () {
            return {
                init: function ($domContainer, viewModel) {
                    // get staff
                    admin.staff.getStaff(viewModel);

                    // make sortables
                    $domContainer.find('[class^=sortable-]').sortable({
                        connectWith: '.sortable',
                        receive: admin.staff.listChanged,
                        update: function (event, ui) {
                            // TODO - this also gets called when the 'receive' event is triggered. this causes the list to get updated twice when you add an item.
                            // need to see if there's a better way to detect the list change.
                            admin.staff.updateDisplayOrder(viewModel);
                        }
                    });
                },

                listChanged: function (e, ui) {
                    admin.staff.updateServer($(ui.item));
                },

                updateServer: function ($li) {
                    var staffManager = admin.staff.getClosestStaffManager($li);
                    var viewModel = staffManager.getViewModel();
                    var staffMemberID = $li.attr('data-staff-id');
                    if ($li.parent().is('[class*="unassigned"]')) {
                        admin.staff.removeStaff(staffMemberID, viewModel, $li);
                    } else {
                        admin.staff.addStaff(staffMemberID, viewModel, $li, $li.index());
                    }
                    // remove element because we're now handling it with knockout
                    $li.remove();
                },

                updateDisplayOrder: function (viewModel)
                {
                    var $contentContainer = $('div.content-editor[data-content-id="' + viewModel.contentID() + '"]');
                    var $sortable = $contentContainer.find('.sortable-assigned-staff');

                    // only save display order for the assigned staff sortable list
                    if ($sortable.hasClass('sortable-assigned-staff')) {
                        var ids = $sortable.sortable('toArray');
                        if (ids.length > 0) {
                            ajaxHelper.ajax('/CustomAdmin/StaffMembers/UpdateStaffMembersDisplayOrder', {
                                data: {
                                    contentID: viewModel.contentID(),
                                    staffMemberIDs: ids
                                },
                                type: 'POST',
                                traditional: true,
                                failureMessageFormat: 'An error occurred trying to set the display order: [[errorThrown]]'
                            });
                        }
                    }
                },

                getStaff: function _getStaff(viewModel) {
                    ajaxHelper.ajax(admin.GetStaffURL, {
                        type: 'POST',
                        data: {
                            contentID: viewModel.contentID()
                        },
                        success: function (data, textStatus, jqXHR) {
                            if (data.Properties != null) {
                                viewModel.clearAssignedStaff();
                                if (data.Properties.Items.length > 0) {
                                    $.each(data.Properties.Items, function (index, staff) {
                                        var newStaff = new admin.staff.Staff(staff);
                                        viewModel.addToAllStaff(newStaff);
                                        viewModel.addToAssignedStaff(newStaff);
                                    });
                                    viewModel.anyAssignedStaff(true);
                                }
                                else {
                                    viewModel.anyAssignedStaff(false);
                                }
                            }
                        },
                        failure: function (data, textStatus, jqXHR) {
                            dialogHelper.alert(data.Message);
                        },
                        failureMessageFormat: 'An error occurred trying to get the staff list: [[errorThrown]]'
                    });
                },

                addStaff: function _addStaff(staffMemberID, viewModel, $li, index) {
                    ajaxHelper.ajax(admin.AddStaffURL, {
                        type: 'POST',
                        data: {
                            contentID: viewModel.contentID(),
                            staffMemberID: staffMemberID
                        },
                        success: function (data, textStatus, jqXHR) {
                            var newStaff = new admin.staff.Staff(data.Properties.Item);
                            viewModel.assignedStaff.splice(index, 0, newStaff);
                            admin.staff.updateDisplayOrder(viewModel);
                        },
                        failure: function (data, textStatus, jqXHR) {
                            dialogHelper.alert(data.Message);
                            $li.sortable('cancel');
                        },
                        failureMessageFormat: 'An error occurred trying to add the staff: [[errorThrown]]'
                    });
                },
                removeStaff: function _removeStaff(staffMemberID, viewModel, $li) {
                    ajaxHelper.ajax(admin.RemoveStaffURL, {
                        type: 'POST',
                        data: {
                            contentID: viewModel.contentID(),
                            staffMemberID: staffMemberID
                        },
                        success: function (data, textStatus, jqXHR) {
                            var newStaff = new admin.staff.Staff(data.Properties.Item);
                            viewModel.unassignedStaff.push(newStaff);
                        },
                        failure: function (data, textStatus, jqXHR) {
                            dialogHelper.alert(data.Message);
                            $li.sortable('cancel');
                        },
                        failureMessageFormat: 'An error occurred trying to remove the staff: [[errorThrown]]'
                    });
                },

                setStaffManager: function (domContainer, dataManager) {
                    $(domContainer).data('staffManager', dataManager);
                },
                getStaffManager: function (domContainer) {
                    return $(domContainer).data('staffManager');
                },
                getClosestStaffManager: function (ancestor) {
                    return $(ancestor).closest('.content-editor').data('staffManager');
                },

                initShowAllStaffLink: function (linkSelector, viewModel, domContainer, successCallback) {
                    $(linkSelector, domContainer).button({
                        icons: {
                            primary: 'ui-icon-gear'
                        }
                    }).on('click', function (event) {
                        event.preventDefault();
                        ajaxHelper.ajax(admin.AllStaffURL, {
                            data: {
                                //
                            },
                            type: 'POST',
                            success: function (data, textStatus, jqXHR) {
                                admin.staff.processUnassignedStaff(domContainer, viewModel, data.Succeeded, data.Properties.Items);
                            },
                            beforeSend: function (jqXHR, settings) {
                                viewModel.retrievingStaff(true);
                            },
                            complete: function (jqXHR, textStatus) {
                                viewModel.retrievingStaff(false);
                                viewModel.initialSearch(false);
                            }
                        });
                    });
                },

                initFindStaffInput: function (input, viewModel, domContainer, successCallback) {
                    $(input).keyup($.debounce(350, function () {
                        if (input.val().length > 2) {
                            ajaxHelper.ajax(admin.FindStaffURL, {
                                data: {
                                    term: input.val()
                                },
                                type: 'POST',
                                success: function (data, textStatus, jqXHR) {
                                    admin.staff.processUnassignedStaff(domContainer, viewModel, data.Succeeded, data.Properties.Items);
                                },
                                beforeSend: function (jqXHR, settings) {
                                    viewModel.retrievingStaff(true);
                                },
                                complete: function (jqXHR, textStatus) {
                                    viewModel.retrievingStaff(false);
                                    viewModel.initialSearch(false);
                                }
                            });
                        }
                    }));
                },
                processUnassignedStaff: function (domContainer, viewModel, succeeded, staff) {
                    viewModel.clearUnassignedStaff();
                    if (succeeded && staff.length > 0) {
                        $.each(staff, function (index, item) {

                            var newStaff = new admin.staff.Staff(item);

                            // check if already in ul lists
                            var assignedMatch = domContainer.find('.sortable-assigned-staff li[data-staff-id="' + item.ID + '"]');
                            var unassignedMatch = domContainer.find('.sortable-unassigned-staff li[data-staff-id="' + item.ID + '"]');

                            if (assignedMatch.length == 0 && unassignedMatch.length == 0) {
                                viewModel.addToUnassignedStaff(newStaff);
                            }

                        });

                        viewModel.anyUnassignedStaff(true);
                    }
                    else {
                        viewModel.anyUnassignedStaff(false);
                    }
                    viewModel.findResultsVisible(true);
                }
            }
        })();

        admin.staff.StaffManager = function (options) {
            this.options = options;
            this.domContainer = $(options.domContainer);
            admin.staff.setStaffManager(this.domContainer, this);
        };

        admin.staff.StaffManager.prototype = function () {
            var
                init = function () {
                    var viewModel = new admin.staff.StaffViewModel(this);
                    this.setViewModel.call(this, viewModel);
                    cms.doCallback(this.options.applying_bindings);
                    ko.applyBindings(viewModel, this.domContainer.get(0));
                },

                setViewModel = function (newViewModel) {
                    this.domContainer.data('staffViewModel', newViewModel)
                },

                getViewModel = function () {
                    return this.domContainer.data('staffViewModel');
                }
            ;

            return {
                init: init,
                setViewModel: setViewModel,
                getViewModel: getViewModel
            };
        }();

        admin.staff.StaffViewModel = function (dataManager) {
            var self = this;
            self.dataManager = dataManager;
            self.contentID = ko.observable();
            self.allStaff = ko.observableArray([]);
            self.assignedStaff = ko.observableArray([]);
            self.unassignedStaff = ko.observableArray([]);
            self.retrievingStaff = ko.observable(true);
            self.initialSearch = ko.observable(true);
            self.findResultsVisible = ko.observable(false);

            self.unassignedStaff.subscribe(function _unassignedStaff_subscribe(newValue) {
                var badIxs = [];
                $.each(newValue, function _newStaff_onEach(avIx, avStaff) {
                    $.each(self.assignedStaff(), function _assignedStaff_onEach(asIx, asStaff) {
                        if (parseInt(avStaff.StaffMemberID) === parseInt(asStaff.StaffMemberID)) {
                            badIxs.push(avIx);
                        }
                    });
                });
                $.each(badIxs.reverse(), function _badIxs_onEach(ix, item) {
                    self.unassignedStaff.splice(item, 1);
                });
            });
        };

        admin.staff.StaffViewModel.prototype = function () {
            var
                addToAllStaff = function (staff) {
                    this.allStaff.push(staff);
                },
                removeFromAllStaff = function (staff) {
                    this.allStaff.remove(function (staff) {
                        return parseInt(staff.ID) === parseInt(id)
                    });
                },

                addToAssignedStaff = function (staff) {
                    this.assignedStaff.push(staff);
                },
                removeFromAssignedStaff = function (id) {
                    this.assignedStaff.remove(function (staff) {
                        return parseInt(staff.ID) === parseInt(id)
                    });
                },

                addToUnassignedStaff = function (staff) {
                    this.unassignedStaff.push(staff);
                },
                removeFromUnassignedStaff = function (id) {
                    this.unassignedStaff.remove(function (staff) {
                        return parseInt(staff.ID) === parseInt(id)
                    });
                },

                anyAssignedStaff = function () {
                    return this.assignedStaff().length > 0;
                },
                anyUnassignedStaff = function () {
                    return this.unassignedStaff().length > 0;
                },
                clearAssignedStaff = function () {
                    this.assignedStaff([]);
                },
                clearUnassignedStaff = function () {
                    this.unassignedStaff([]);
                },
                closeFindResults = function () {
                    this.findResultsVisible(false);
                }
            ;

            return {
                assignedStaff: this.assignedStaff,
                unassignedStaff: this.unassignedStaff,
                retrievingStaff: this.retrievingStaff,
                findResultsVisible: this.findResultsVisible,

                addToAllStaff: addToAllStaff,
                removeFromAllStaff: removeFromAllStaff,

                addToAssignedStaff: addToAssignedStaff,
                removeFromAssignedStaff: removeFromAssignedStaff,

                addToUnassignedStaff: addToUnassignedStaff,
                removeFromUnassignedStaff: removeFromUnassignedStaff,

                anyAssignedStaff: anyAssignedStaff,
                anyUnassignedStaff: anyUnassignedStaff,
                clearAssignedStaff: clearAssignedStaff,
                clearUnassignedStaff: clearUnassignedStaff,
                closeFindResults: closeFindResults
            };
        }();

        admin.staff.Staff = function (data) {
            var self = this;
            ko.mapping.fromJS(data, {}, self);
            this.viewModel = {};
        };
        admin.staff.Staff.prototype = function () {
            var setViewModel = function _setViewModel(viewModel) {
                this.viewModel = viewModel;
            }
            return {
                setViewModel: setViewModel
            };
        }();
    })();
})(jQuery);