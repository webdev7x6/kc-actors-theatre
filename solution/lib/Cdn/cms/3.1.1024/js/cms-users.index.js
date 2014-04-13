/// <reference path="http://clickfarmcdn.localhost/common/js/jquery/1.7.2/jquery-vsdoc.js" />
/// <reference path="http://clickfarmcdn.localhost/common/js/kendo/2012.2.710/kendo.all-vsdoc.js" />

$(function () {
	//set-up
	cms.makeCloseButton('#close-tabs-link', '#close-find-cms-users-results');
	cmsUsersViewModel.findNameEl = $('#find-cms-users-name');
	$('#show-all-cms-users').button({
		icons: {
			primary: 'ui-icon-gear'
		}
	});

	//tabify
	cmsUsersViewModel.tabMgr = new TabManager({
		closable: true,
		container: $('#cms-users-index-tabs'),
		load_tabTypeSelector: '.cms-users-index-tab',
		load_tabTypeActions: {
			"edit-cms-user": initEditCmsUserTab
		},
		numStaticTabs: 1,
		show_action: function (event, ui, tabMgr) {
			if (ui.index === 0) {
				cmsUsersViewModel.findNameEl.select();
			}
			else {
				addRecentCmsUserByTab.call(this, event, ui);
			}
		}
	});
	cmsUsersViewModel.tabMgr.$tabs.on('tabsload', addRecentCmsUserByTab);

	//accordion
	$('#start-options').accordion({
		fillSpace: true
	});

	//knockout bindings
	ko.applyBindings(cmsUsersViewModel);
	cmsUsersViewModel.findTerm.subscribe($.debounce(350, findCmsUsersAjax));
	cmsUsersViewModel.findingResults.subscribe(function (newValue) {
		if (newValue) {
			$('.tab-display').prepend($('#find-cms-users-results').detach());
		}
	});

	//other event bindings
	cmsUsersViewModel.findNameEl.on('input propertychange', function (e) {
		cmsUsersViewModel.findTerm($(this).val());
	});
});

function addRecentCmsUserByTab(event, ui) {
    var cmsUserID = parseInt($(ui.panel).find('.cms-users-index-tab').attr('data-cms-user-id'));
    if (cmsUserID) {
        var ix = cmsUsersViewModel.findResultIndexByID(cmsUserID),
            cmsUser
        ;
        if (ix > -1) {
            cmsUser = cmsUsersViewModel.findResults()[ix];
        }
        else {
            ix = cmsUsersViewModel.findRecentIndexByID(cmsUserID)
            if (ix > -1) {
                cmsUser = cmsUsersViewModel.recentCmsUsers()[ix];
            }
        }
        if (cmsUser) {
            cmsUsersViewModel.addRecentCmsUser(cmsUser);
        }
    }
}

var CmsUser = function (cmsUserID, emailAddress, firstName, lastName, status, roles) {
    var self = this;

    self.cmsUserID = ko.observable(cmsUserID);
    self.emailAddress = ko.observable(emailAddress);
    self.firstName = ko.observable(firstName);
    self.lastName = ko.observable(lastName);
    self.fullName = ko.computed(function () {
        var n = self.firstName(),
            ln = self.lastName()
        ;
        if (ln) {
            if (n) {
                n += ' ';
            }
            n += ln;
        }
        return n;
    });
    self.fullNameReverse = ko.computed(function () {
        var n = self.lastName(),
            fn = self.firstName()
        ;
        if (fn) {
            if (n) {
                n += ', ';
            }
            n += fn;
        }
        return n;
    });
    self.status = ko.observable(status);
    self.roles = ko.observableArray(roles || []);
    self.editCmsUser = function () {
        cmsUsersViewModel.showEditCmsUser(self);
    };
}

var cmsUsersViewModel = {
	tabMgr: null,

	//find elements
	findNameEl: null,

	//find bindings
	findResults: ko.observableArray([]),
	findResultsVisible: ko.observable(),
	findTerm: ko.observable(''),
	previousFindTerm: null,
	findingResults: ko.observable(false),
	addResult: function (cmsUser) {
		this.findResults.push(cmsUser);
	},
	findResultIndexByID: function (cmsUserID) {
		var ixOf = -1;
		$.each(this.findResults(), function (ix, item) {
			if (item.cmsUserID() === cmsUserID) {
				ixOf = ix;
				return false;
			}
		});
		return ixOf;
	},
	removeResultByID: function (cmsUserID) {
		this.findResults.remove(function (item) {
			return item.cmsUserID() === cmsUserID
		});
	},
	clearFindResults: function () {
		this.findResults([]);
	},
	closeFindResults: function () {
		this.findResultsVisible(false);
	},
	clearFindTerm: function () {
		this.findNameEl.val('');
		this.findTerm('<clearme>');
	},

	//edit
	showEditCmsUser: function (cmsUser) {
		var cmsUserID = cmsUser.cmsUserID();
		this.tabMgr.addTab(new TabInfo(
            '/Admin/CmsUsers/EditAjax/' + cmsUserID,
            cmsUser.fullNameReverse(),
            'edit-cms-user',
            uniqueIDForCmsUserTab(cmsUserID)
        ));
	},
	recentCmsUsers: ko.observableArray([]),
	addRecentCmsUser: function (cmsUser) {
		var ix = this.findRecentIndexByID(cmsUser.cmsUserID());
		if (ix > -1) {
			this.recentCmsUsers.splice(ix, 1);
		}
		else {
			//only allow 10 recent entities
			while (this.recentCmsUsers().length >= 10) {
				this.recentCmsUsers.pop()
			}
		}
		this.recentCmsUsers.unshift(cmsUser);
	},
	findRecentIndexByID: function (cmsUserID) {
		var ixOf = -1;
		$.each(this.recentCmsUsers(), function (ix, item) {
			if (item.cmsUserID() === cmsUserID) {
				ixOf = ix;
				return false;
			}
		});
		return ixOf;
	},
	removeRecentByID: function (cmsUserID) {
		this.recentCmsUsers.remove(function (item) {
			return item.cmsUserID() === cmsUserID
		});
	},

	//delete
	deleteCmsUser: function (cmsUserID) {
		cmsUsersViewModel.removeResultByID(cmsUserID);
		cmsUsersViewModel.removeRecentByID(cmsUserID);
		cmsUsersViewModel.tabMgr.removeTabByID(uniqueIDForCmsUserTab(cmsUserID));
	}
}

function uniqueIDForCmsUserTab(cmsUserID) {
    return 'edit-cms-user-' + cmsUserID;
}

function cmsUserPropertyUpdated(element, data) {
    var cmsUserID = parseInt(data.UniqueID),
        ix
    ;
    switch (data.Property) {
    	case 'Email':
            ix = cmsUsersViewModel.findResultIndexByID(cmsUserID);
            if (ix > -1) {
                cmsUsersViewModel.findResults()[ix].emailAddress(data.NewValue);
            }
            ix = cmsUsersViewModel.findRecentIndexByID(cmsUserID);
            if (ix > -1) {
                cmsUsersViewModel.recentCmsUsers()[ix].emailAddress(data.NewValue);
            }
            //TODO fullNameReverse, fall-back to emailAddress
            //cmsUsersViewModel.tabMgr.updateTabTitleByID(uniqueIDForCmsUserTab(cmsUserID), data.NewValue);
            //updateCurrentCmsUserProfileLinkText(data.NewValue);
            break;
        case 'FirstName':
            ix = cmsUsersViewModel.findResultIndexByID(cmsUserID);
            if (ix > -1) {
                cmsUsersViewModel.findResults()[ix].firstName(data.NewValue);
            }
            //TODO fullNameReverse, fall-back to emailAddress
            //cmsUsersViewModel.tabMgr.updateTabTitleByID(uniqueIDForCmsUserTab(cmsUserID), data.NewValue);
            //updateCurrentCmsUserProfileLinkText(data.NewValue);
            break;
        case 'LastName':
            ix = cmsUsersViewModel.findResultIndexByID(cmsUserID);
            if (ix > -1) {
                cmsUsersViewModel.findResults()[ix].lastName(data.NewValue);
            }
            //TODO fullNameReverse, fall-back to emailAddress
            //cmsUsersViewModel.tabMgr.updateTabTitleByID(uniqueIDForCmsUserTab(cmsUserID), data.NewValue);
            //updateCurrentCmsUserProfileLinkText(data.NewValue);
            break;
        case 'Roles':
            var roles = [],
                check = data.displayValue.substr(0, 8).toLowerCase()
            ;
            if (check === 'click to') {
                //do nothing
            }
            else if (check !== '<ul><li>') {
                roles.push(data.displayValue)
            }
            else {
                $(data.displayValue).find('li').each(function (ix, el) {
                    roles.push($(el).text())
                });
            }
            ix = cmsUsersViewModel.findResultIndexByID(cmsUserID);
            if (ix > -1) {
                cmsUsersViewModel.findResults()[ix].roles(roles);
            }
            break;
        case 'Status':
            if (data.NewValue !== 'Deleted') {
                ix = cmsUsersViewModel.findResultIndexByID(cmsUserID);
                if (ix > -1) {
                    cmsUsersViewModel.findResults()[ix].status(data.displayValue);
                }
            }
            else {
            	cmsUsersViewModel.deleteCmsUser(cmsUserID);
            }
            break;
    }
}

function findCmsUsersAjax() {
    var term = $.trim(cmsUsersViewModel.findTerm());
    if (term === '<clearme>') {
        cmsUsersViewModel.previousFindTerm = '<clearme>';
        cmsUsersViewModel.findTerm('')
        return;
    }
    if (
        term !== cmsUsersViewModel.previousFindTerm
        && (term.length === 0 || term.length > 1)
    ) {
        cmsUsersViewModel.previousFindTerm = term;
        ajaxHelper.ajax('/Admin/CmsUsers/FindCmsUsersAjax', {
            type: 'GET',
            data: { term: term },
            beforeSend: function (jqXHR, settings) {
                cmsUsersViewModel.findingResults(true);
            },
            success: function (data, textStatus, jqXHR) {
                cmsUsersViewModel.clearFindResults();
                if (data.length > 0) {
                    $.each(data, function (ix, item) {
                        cmsUsersViewModel.addResult(new CmsUser(item.cmsUserID, item.emailAddress, item.firstName, item.lastName, item.status, item.roles));
                    });
                }
                cmsUsersViewModel.findResultsVisible(true);
            },
            complete: function (jqXHR, textStatus) {
                cmsUsersViewModel.findingResults(false);
            }
        });
    }
}
