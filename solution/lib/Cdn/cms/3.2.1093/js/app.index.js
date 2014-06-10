/// <reference path="http://clickfarmcdn.localhost/common/js/jquery/1.7.2/jquery-vsdoc.js" />
/// <reference path="http://clickfarmcdn.localhost/common/js/kendo/2012.2.710/kendo.all-vsdoc.js" />

(function ($, undefined) {
	var cms = window.cms = window.cms || {};
	cms.app = cms.app || {};
	(function () {
		//a url object with some url-related utility functions
		cms.app.index = new (function () {
			return {
				newExternalLinkManager: {},
				newExternalLinkSuccess: function (menuItem) {
					//console.log(menuItem);
				}
			}
		});

	})();

})(jQuery);

$(function () {
	cms.app.index.newExternalLinkManager = new cms.menuitem.NewExternalLinkManager({
		form: '#new-external-link-form',
		error: '#new-external-link-error',
		loader: '#new-external-link-loading',
		success: cms.app.index.newExternalLinkSuccess
	});
	cms.app.index.newExternalLinkManager.ajaxSuccess = ajaxHelper.success(
		function (response, textStatus, jqXHR) {

			//						var pageTitle = result.Page.Title,
			//							pageType = result.PageType,
			//							pageID = result.Page.PageID
			//						;

			//console.log(response.MenuItem);
			newChildPageNodeCreated({
				referenceMenuItemID: cms.app.index.newExternalLinkManager.referenceNodeIDInput.val(),
				createRelationship: cms.app.index.newExternalLinkManager.createRelationshipInput.val(),
				title: response.MenuItem.Title,
				menuItemID: response.MenuItem.MenuItemID,
				nodeType: 'externallink'
			});
			cms.app.index.newExternalLinkManager.resetForm();
			//						var newPage = new cmsPage(pageTitle, pageType, pageID);
			//						newPage.editPage();
			//						appViewModel.registerUnappliedChange();
			//cms.doCallback($this.success, response.MenuItem);
			appViewModel.registerUnappliedChange();
		},
		function (data, textStatus, jqXHR, formattedMessage) {
			cms.app.index.newExternalLinkManager.showError(formattedMessage);
		},
		false
	);

	cms.app.index.newExternalLinkManager.ajaxFailure = ajaxHelper.error(
			function (jqXHR, textStatus, errorThrown, formattedMessage) {
				cms.app.index.newExternalLinkManager.showError(formattedMessage);
			},
			false
	);

	cms.app.index.findPages = function () {
	    //$('#recent-pages').hide();
	    ajaxHelper.ajax('/Admin/Page/FindPage', {
	        data: {
	            term: '',
	            appID: $('#app').attr('data-app-id')
	        },
	        type: 'POST',
	        success: function (data, textStatus, jqXHR) {
	            appViewModel.clearFindResults();
	            if (data.length > 0) {
	                $.each(data, function (index, item) {
	                    //push results into view model's observable collection of views
	                    appViewModel.addResult(new cmsPage(item.label, item.desc, item.value));
	                });
	                appViewModel.anyPagesFound(true);
	            }
	            else {
	                appViewModel.anyPagesFound(false);
	            }
	            appViewModel.findResultsVisible(true);
	        },
	        beforeSend: function (jqXHR, settings) {
	            appViewModel.findingResults(true);
	        },
	        complete: function (jqXHR, textStatus) {
	            appViewModel.findingResults(false);
	        }
	    });
	};

});


//all below this in need of refactoring

var contentGroupModal;
var editUrlModal;
var menuTreeview;

$(function () {
	initializeTabs();
	ko.applyBindings(appViewModel);
	koSubscriptions();
	//initializeContentGroupModal();
	//initializeEditUrlModal();
	initLinks();
	loadMenuCategories();
	menuTreeview = $('#menu-tree-target');

	$('.delete-checked-nodes')
		.button({
			icons: {
				primary: 'ui-icon-trash'
			},
			text: false
		})
		.on('click', function (event) {
			event.preventDefault();
			var self = $(this),
				deletePage = $('#delete-page-checkbox:checked').length > 0,
				checkedNodes = getCheckedNodes();
			if (checkedNodes.length > 0) {
				deleteTreeviewNodes(checkedNodes, deletePage);
			}
		})
	;

	initializeFindAccordion();
});

var cmsPage = function (title, pageType, pageID) {
	var self = this;
	this.title = ko.observable($.trim(title));
	this.pageType = ko.observable(pageType);
	this.pageID = ko.observable(pageID);
	this.editPage = function (event) {
		appViewModel.showEditPage(self);
	};
}

var menuCategory = function (name, someMenus) {
	var self = this;
	this.name = name;
	this.menus = ko.observableArray(someMenus);
}
var menu = function (name, alias, menuID) {
	var self = this;
	this.name = name;
	this.alias = alias;
	this.menuID = menuID;
	this.viewMenu = function () {
		appViewModel.selectedMenu(self);
	};
}
var menuItem = function (title, menuItemID, menuItemCmsUrl, externalLinkUrl, isEnabled) {
	var self = this;
	this.title = ko.observable(title);
	this.cmsUrl = menuItemCmsUrl;
	this.hasUrl = function () { return self.cmsUrl != null; };
	this.menuItemID = ko.observable(menuItemID);
	this.externalLinkUrl = externalLinkUrl;
	this.hasExternalLink = function () { return self.externalLinkUrl !== null && self.externalLinkUrl !== ''; };
	this.parent = null;
	this.menuName = '';
	this.menuID = 0;
	this.hasParent = function () { return self.parent != null; };
	this.isEnabled = isEnabled;
}

var sharedContentMenuItem = function (contentType, assemblyType, count, iconCssClass) {
	var $this = this;
	this.contentType = contentType;
	this.assemblyType = assemblyType;
	this.count = count;
	this.iconCssClass = iconCssClass;
	this.view = function () {
		appViewModel.selectedSharedContentType($this.assemblyType);
		appViewModel.sharedContentDisplayVisible(true);
	}
}

var appViewModel = {
	tabMgr: null,
	currentAppID: function () { return $('#app').attr('data-app-id'); },
	//FIND PAGES
	pagesFound: ko.observableArray([]),
	findResultsVisible: ko.observable(),
	addResult: function (page) {
		this.pagesFound.push(page);
	},
	removeResultWithID: function (pageID) {
		this.pagesFound.remove(function (item) { return item.pageID() == pageID });
	},
	clearFindResults: function () {
		this.pagesFound([]);
	},
	findingResults: ko.observable(false),
	anyPagesFound: ko.observable(false),
	closeFindResults: function () {
		this.findResultsVisible(false);
	},

	//MENU CATEGORIES, MENUS, MENU ITEMS
	menuCategories: ko.observableArray([]),
	loadingMenuCategories: ko.observable(true),
	menuCategoriesFound: ko.observable(false),

	selectedMenu: ko.observable(),
	selectedMenuVisible: ko.observable(false),
	selectedMenuName: function () {
		if (this.selectedMenu()) {
			return this.selectedMenu().name;
		}
		else {
			return '';
		}
	},
	selectedMenuChanged: function () {
		if (this.selectedMenu() != 0) {
			populateMenuTree(this);
			this.selectedMenuVisible(true);
			this.anyNodesChecked(false);
		}
	},
	selectedMenuItemID: ko.observable(0),
	selectedMenuItem: ko.observable(new menuItem('', 0, new cmsUrl(false, '', '', 0, new cmsPage('', '', 0), null), null, false)),
	closeMenu: function () {
		this.selectedMenuVisible(false);
	},
	treeWorking: ko.observable(false),
	anyNodesChecked: ko.observable(false),
	nodeJustDeleted: ko.observable(),

	//PAGE EDITING
	showEditPage: function (cmsPage) {
		var pageID = cmsPage.pageID(),
			pageTitle = truncateStr(cmsPage.title(), 20),
			url = '/Admin/Page/Edit/' + pageID + '?appID=' + appViewModel.currentAppID(),
			newTab = new TabInfo(url, pageTitle, 'edit-page', uniqueIDForPageTab(pageID));
		appViewModel.tabMgr.addTab(newTab);
		appViewModel.addRecentPage(cmsPage);
	},
	recentPages: ko.observableArray([]),
	addRecentPage: function (cmsPage) {
		var existingPageIndex = findExistingRecentPageIndex(cmsPage);
		if (existingPageIndex > -1) {
			this.recentPages.splice(existingPageIndex, 1);
		}
		this.recentPages.unshift(cmsPage);
	},
	removeRecentPageWithID: function (pageID) {
		this.recentPages.remove(function (item) { return item.pageID() == pageID });
	},
	removePage: function (pageID) {
		appViewModel.tabMgr.removeTabByID(uniqueIDForPageTab(pageID));
		appViewModel.removeRecentPageWithID(pageID);
	},

	//Shared Content
	sharedContentMenuItems: ko.observableArray([]),
	selectedSharedContentType: ko.observable(),
	gettingSharedContentMenu: ko.observable(),
	addSharedContentMenuItem: function (item) {
		this.sharedContentMenuItems.push(item);
	},
	clearSharedContentMenu: function () {
		this.sharedContentMenuItems([]);
	},
	sharedContentDisplayVisible: ko.observable(false),
	closeSharedContentDisplay: function () {
		this.sharedContentDisplayVisible(false);
	},
	gettingSelectedSharedContent: ko.observable(false),
	//sharedContentChanged: function () {
	//		//populateMenuTree(this);
	//		this.sharedContentVisible(true);
	//},

	//FIND URLs
	urls: ko.observableArray([]),
	findUrlResultsVisible: ko.observable(),
	addUrlResult: function (url) {
		this.urls.push(url);
	},
	removeUrlResultWithID: function (urlID) {
		this.urls.remove(function (item) { return item.urlID() == urlID });
	},
	clearFindUrlResults: function () {
		this.urls([]);
	},
	retrievingUrls: ko.observable(false),
	anyUrls: ko.observable(false),
	closeFindUrlResults: function () {
		this.findUrlResultsVisible(false);
	},
	removeUrl: function (url, event) {
		event.preventDefault();
		dialogHelper.confirm(
			'Are you sure you want to remove this URL?',
			'Remove URL',
			function () { cms.url.deleteUrl(url, appViewModel); }

		);
	},


	//CMS Changes
	hasUnappliedChange: ko.observable(false),
	registerUnappliedChange: function () {
		this.hasUnappliedChange(true);
	},
	applyingChanges: ko.observable(false)
}

function uniqueIDForPageTab(pageID) {
	return 'edit-page-' + pageID;
}

function findExistingRecentPageIndex(newRecentPage) {
	var indexOf = -1;
	$.each(appViewModel.recentPages(), function (index, item) {
		if (item.pageID() == newRecentPage.pageID()) {
			indexOf = index;
			return false;
		}
	});
	return indexOf;
}

function populateMenuTree(self) {
	self.selectedMenuItem().menuItemID(0); //this will hide selected menu item info

	menuTreeview.jstree({
		core: {
			animation: 100
		},
		themes: {
			theme: 'apple'
		},
		json_data: {
			ajax: {
				url: '/Admin/Menu/Index/' + appViewModel.selectedMenu().menuID,
				success: ajaxHelper.success(),
				error: ajaxHelper.error(),
				type: 'POST'
			},
			progressive_render: true
		},
		dnd: {
			drag_check: function (data) {
				return {
					after: true,
					before: true,
					inside: true
				};
			},
			drag_finish: function (data) {
				var anchor = $(data.o),
					draggedPageID = parseInt(anchor.attr('data-page-id')),
					droppedNodeID = data.r.attr('id'),
					dropRelationship = data.p,
					title = anchor.text()
				menuItemType = anchor.attr('data-menu-item-type')
					;

				appViewModel.treeWorking(true);
				if ((draggedPageID > 0 && menuItemType === 'UrlMenuItem') || menuItemType === 'FolderMenuItem') {
					//new menu item for an existing page
					ajaxHelper.ajax('/Admin/MenuItem/NewNode', {
						data: {
							appID: appViewModel.currentAppID(),
							pageID: draggedPageID,
							//menuItemType: menuItemType,
							referenceNodeID: droppedNodeID,
							createRelationship: dropRelationship,
							title: title
						},
						type: 'POST',
						success: function (ajaxdata, textStatus, jqXHR) {
							//node was created, so add to the tree
							createTreeNode(data.r, data.p, title, ajaxdata.MenuItem.MenuItemID, menuItemType !== 'FolderMenuItem', menuItemType === 'FolderMenuItem' ? 'folder' : 'page');
							appViewModel.registerUnappliedChange();
						},
						failureMessageFormat: 'An error occurred trying to create the item: [[errorThrown]]',
						errorMessageFormat: 'An error occurred creating the item: [[errorThrown]]',
						complete: function (jqXHR, textStatus) {
							appViewModel.treeWorking(false);
						}
					});
				}
				else if (menuItemType === 'ExternalLinkMenuItem') {
					cms.app.index.newExternalLinkManager.showForm(droppedNodeID, dropRelationship);
				}
				else if (draggedPageID === 0 && menuItemType === 'UrlMenuItem') {
					//new menu item for a new page to be created
					var pageType = anchor.attr('data-page-type');
					newPageMgr.showNewPageForm(pageType, droppedNodeID, dropRelationship, 'newChildPageNodeCreated');
				}
				else {
					dialogHelper.alert('The type of menu item was not recognized: ' + menuItemType);
					appViewModel.treeWorking(false);
				}
			}
			//drop_finish: function(data) {
			//	var id = data.o.attr('id');
			//}
		},
		checkbox: {
			two_state: true
		},
		contextmenu: {
			show_at_node: false,
			items: function (node) {
				var menus = {
					rename: {
						label: 'Rename',
						action: function (node) { this.rename(node); }
					},
					deletenode: {
						label: 'Delete Menu Item',
						action: function (node) { deleteTreeviewNodes(node, false); }
					}
				};
				var pageID = node.attr('data-page-id');
				if (pageID) {
					var deletepagemenu = {
						deletepagemenu: {
							label: 'Delete Menu Item &amp; Page',
							action: function (node) { deleteTreeviewNodes(node, true); }
						}
					}
					$.extend(menus, deletepagemenu);
				}
				return menus;
			}
		},
		types: {
			valid_children: ['page'],
			types: {
				page: {
					icon: {
						image: cms.globals.versionedCdnUrlBase + '/images/page.png'
					}
				},
				folder: {
					icon: {
						image: cms.globals.versionedCdnUrlBase + '/images/folder.png'
					}
				},
				externallink: {
					icon: {
						image: cms.globals.versionedCdnUrlBase + '/images/link.png'
					}
				}
			}
		},
		plugins: ['themes', 'json_data', 'ui', 'dnd', 'crrm', 'checkbox', 'contextmenu', 'types']
	})
	.bind('select_node.jstree', function (event, data) {
		self.selectedMenuItemID(data.rslt.obj.attr("id"));
	})
	.bind('rename_node.jstree', function (event, data) {
		//dialogHelper.alert('node renamed');
		var newTitle = data.rslt.name;
		var menuItemID = data.rslt.obj.attr('id');
		var rollbackObj = data.rlbk;
		appViewModel.treeWorking(true);
		ajaxHelper.ajax('/Admin/MenuItem/Rename', {
			data: {
				newTitle: newTitle,
				menuItemID: menuItemID
			},
			type: 'POST',
			success: function (data, textStatus, jqXHR) {
				if (appViewModel.selectedMenuItem().menuItemID() == menuItemID) {
					appViewModel.selectedMenuItem().title(newTitle);
				}
				appViewModel.registerUnappliedChange();
			},
			failure: function (data, textStatus, jqXHR, formattedMessage) {
				$.jstree.rollback(rollbackObj);
			},
			failureMessageFormat: 'An error occurred trying to rename the node: [[errorThrown]]',
			error: function (jqXHR, textStatus, errorThrown, formattedMessage) {
				$.jstree.rollback(rollbackObj);
			},
			errorMessageFormat: 'An error occurred renaming the node: [[errorThrown]]',
			complete: function (jqXHR, textStatus) {
				appViewModel.treeWorking(false);
			}
		});
	})
	.bind('change_state.jstree', function (event, data) {
		var bool = getCheckedNodes().length > 0;
		appViewModel.anyNodesChecked(bool);
	})
	//.bind('click.jstree', function (event, data) {
	//	dialogHelper.alert('click');
	//})
	//.bind('check_node.jstree', function (event, data) {
	//	//dialogHelper.alert('checked');
	//	var id = data.rslt.obj.attr('id');
	//})
	.bind('move_node.jstree', function (e, data) {
		//data.rslt.o.attr("id")
		//data.rslt.o - the node being moved
		//data.rslt.r - the reference node in the move
		//data.rslt.ot - the origin tree instance
		//data.rslt.rt - the reference tree instance
		//data.rslt.p - the position to move to (may be a string - "last", "first", etc)
		//data.rslt.cp - the calculated position to move to (always a number)
		//data.rslt.np - the new parent
		//data.rslt.oc - the original node (if there was a copy)
		//data.rslt.cy - boolen indicating if the move was a copy
		//data.rslt.cr - same as np, but if a root node is created this is -1
		//data.rslt.op - the former parent
		//data.rslt.or - the node that was previously in the position of the moved node
		var nodeMovedID = data.rslt.o.attr('id'),
				newParentNodeID = data.rslt.np.attr('id'),
				formerParentNodeID = data.rslt.op.attr('id'),
				moveRelationship = data.rslt.p, //before, after, inside
				referenceNodeID = data.rslt.r.attr('id'); //node in relation to which
		handleTreeNodeMove(nodeMovedID, newParentNodeID, formerParentNodeID, moveRelationship, referenceNodeID, data.rlbk);
		//var info = 'nodeMovedID: ' + nodeMovedID +
		//	'\nnewParentNodeID: ' + newParentNodeID +
		//	'\nformerParentNodeID: ' + formerParentNodeID +
		//	'\nmoveRelationship: ' + moveRelationship +
		//	'\nreferenceNodeID: ' + data.rslt.r.attr('id');
	})
//	.bind('create_node.jstree', function (e, data) {
//		console.log(e, data);
//	})
	;

}
function getCheckedNodes() {
	return menuTreeview.jstree('get_checked', null, true);
}

function newChildPageNodeCreated(data) {
	//callback for when a new child menu item is created for a menu
	//data.title: title of the new MenuItem
	//data.pageType: type of the new page
	//data.pageID: pageID of the new page
	//data.referenceMenuItemID: MenuItemID of the node in reference to which the new menuItem was created
	//data.createRelationship: the relationship of the new node to the reference node, e.g. before, after, inside, first (child), last (child)
	//data.menuItemID: MenuItemID of the new menu item for the new page
	var referenceNode = $.jstree._reference(menuTreeview)._get_node('li#' + data.referenceMenuItemID);
	if (referenceNode) {
		createTreeNode(referenceNode, data.createRelationship, data.title, data.menuItemID, true, data.nodeType);
	}
}

function createTreeNode(referenceNode, createRelationship, nodeTitle, menuItemID, skipRename, nodeType) {
	if (!nodeType) {
		nodeType = 'page';
	}
	var tree = menuTreeview.jstree('create', referenceNode, createRelationship, { data: nodeTitle, attr: { 'id': menuItemID } }, false, skipRename);
	$('li#' + menuItemID).attr('rel', nodeType);
}

function handleTreeNodeMove(nodeMovedID, newParentNodeID, formerParentNodeID, moveRelationship, referenceNodeID, rollbackObj) {
	appViewModel.treeWorking(true);
	ajaxHelper.ajax('/Admin/MenuItem/MoveNode', {
		data: {
			nodeMovedID: nodeMovedID,
			newParentNodeID: newParentNodeID,
			formerParentNodeID: formerParentNodeID,
			moveRelationship: moveRelationship,
			referenceNodeID: referenceNodeID
		},
		type: 'POST',
		success: function (data, textStatus, jqXHR) {
			appViewModel.registerUnappliedChange();
		},
		failure: function (data, textStatus, jqXHR, formattedMessage) {
			$.jstree.rollback(rollbackObj);
		},
		failureMessageFormat: 'An error occurred trying to move the item: [[errorThrown]]',
		error: function (jqXHR, textStatus, errorThrown, formattedMessage) {
			$.jstree.rollback(rollbackObj);
		},
		errorMessageFormat: 'An error occurred moving the item: [[errorThrown]]',
		complete: function (jqXHR, textStatus) {
			appViewModel.treeWorking(false);
		}
	});
}

function initEditPageTab(panel) {
	var editableParent = panel.find('.editable-parent'),
		clickData = { pageID: 0, appID: 0 },
		pageID = editableParent.attr('data-page-id'),
		appID = editableParent.attr('data-app-id');
	if (editableParent.length > 0) {
		clickData.pageID = pageID;
		clickData.appID = appID;
		var editMgr = new EditableManager(editableParent, { pageid: clickData.pageID, appid: clickData.appID }, '/Admin/Page/EditInPlace');
		//editMgr.initAllTypes();
		editMgr.initTextboxes();
		editMgr.initTextareas();
		editMgr.initSelects();
		editMgr.initCmsTemplates(pageID, appID);
		editMgr.initCmsUrls();
		editMgr.initYesNoBoolean();
	}
	//initialize delete and refresh buttons
	panel.find('.delete-page-link').button({
		icons: {
			primary: 'ui-icon-trash'
		},
		text: false
	});
	panel.find('.refresh-page-link').button({
		icons: {
			primary: 'ui-icon-refresh'
		},
		text: false
	});
	//init edit interface tabs
	panel.find('#page-' + pageID + '-tabs').tabs();

	//initialize content group modals
	contentGroupMgr.refreshContentGroups(panel, appID, pageID, $('.content-groups', panel).attr('data-controller-display-name'));

	//initialize delete and refresh page links
	editableParent.find('.delete-page-link').on('click', function (event) {
		event.preventDefault();
		dialogHelper.confirm(
			'Are you sure you want to delete this page?',
			'Delete Page',
			function () {
				deletePage(pageID, panel);
			}
		);
	});

	editableParent.find('.refresh-page-link').on('click', function (event) {
		event.preventDefault();
		appViewModel.tabMgr.reloadTabByID(uniqueIDForPageTab(pageID));
	});

	//initialize URL management items
	var domContainer = panel.find('.url-container'),
	urlMgr = new cms.url.UrlManager(domContainer, '/Admin/Url/ForPage', { appID: appID, pageID: pageID }, null);
	urlMgr.initialize();
	//initialize page menu items
	menuItemMgr.refreshMenuItems(null, panel.find('.page-menu-items'), appID, pageID, function () {
		//initialize URL management for tab panel
		urlMgr.refreshUrls();
	});
	menuItemMgr.initMenuItemOptionLinks(panel);
}

function initEditInPlaceForUrl(element) {
	element = $(element);
	var urlID = element.attr('data-url-id'),
	editMgr = new EditableManager(element, { urlID: urlID }, '/Admin/Url/EditInPlace');
	editMgr.initTextboxes();
	editMgr.initYesNoBoolean();
}

function loadMenuCategories() {
	//load menu categories
	ajaxHelper.ajax('/Admin/MenuCategory/AppCategories/', {
		data: { id: appViewModel.currentAppID },
		type: 'POST',
		success: function (data, textStatus, jqXHR) {
			if (data.length > 0) {
				$.each(data, function (index, item) {
					//push results into view model's observable collection of menu categories
					var someMenus = [];
					if (item.Menus != null) {
						$.each(item.Menus, function (itemIndex, anItem) {
							var m = new menu(anItem.Name, anItem.Alias, anItem.MenuID);
							someMenus.push(m);
							//load first menu
							//if (index == 0 && itemIndex == 0) {
							//	appViewModel.selectedMenu(m);
							//}
						});
					}
					var category = new menuCategory(item.Name, someMenus);
					appViewModel.menuCategories.push(category);
				});
				appViewModel.menuCategoriesFound(true);
			}
			else {
				appViewModel.menuCategoriesFound(false);
			}
			appViewModel.loadingMenuCategories(false);
		},
		errorMessageFormat: 'There was an error retrieving the menu categories: [[errorThrown]]'
	});
}

function koSubscriptions() {
	//Model Subscriptions

	appViewModel.findingResults.subscribe(function (newValue) {
		if (newValue) {
			$('.tab-display').prepend($('#find-page-results').detach());
		}
	});

	appViewModel.retrievingUrls.subscribe(function (newValue) {
		if (newValue) {
			$('.tab-display').prepend($('#find-url-results').detach());
		}
	});

	appViewModel.anyNodesChecked.subscribe(function (newValue) {
		if (!newValue) {
			$('#delete-page-checkbox').prop("checked", false);
		}
	});

	//when the selected menu changes, call the method to populate the menu tree
	appViewModel.selectedMenu.subscribe(function (newValue) {
		appViewModel.selectedMenuChanged();
		$('.tab-display').prepend($('#menu-display').detach());
	});

	appViewModel.selectedMenuVisible.subscribe(function (newValue) {
		if (!newValue) {
			appViewModel.selectedMenu(0);
		}
	});

	var $menuItemDetails = $('#menu-item');

	//when the selected menu item id changes, retrieve the menu item data and change the selectedMenuItem
	appViewModel.selectedMenuItemID.subscribe(function (newValue) {
		ajaxHelper.ajax('/Admin/MenuItem/Index/' + appViewModel.selectedMenuItemID(), {
			type: 'POST',
			success: function (data, textStatus, jqXHR) {
				//var nodeCmsPage = new cmsPage(data.entity.title, data.page.pageType, data.page.pageID),
				var newUrl, newCmsPage, newMenuItem;
				if (data.url != null) {
					if (data.url.page != null) {
						newCmsPage = new cmsPage(data.url.page.title, data.url.page.pageType, data.url.page.pageID);
					}
					newUrl = new cmsUrl(data.url.isEnabled, data.url.origin, data.url.path, data.url.urlID, newCmsPage, data.url.cmsKey);
				}
				newMenuItem = new menuItem(data.title, data.menuItemID, newUrl, data.externalLinkUrl, data.isEnabled);
				appViewModel.selectedMenuItem(newMenuItem);
				var editMgr = new EditableManager($menuItemDetails, { menuitemid: data.menuItemID }, '/Admin/MenuItem/EditInPlace');
				editMgr.initTextboxes();
				editMgr.initYesNoBoolean();
				//menu item URL Live/Preview links
				$('#menu-item-links a, #menu-item-external-link a').button({
					icons: {
						primary: 'ui-icon-extlink'
					}
				});
			}
		});
	});

	appViewModel.nodeJustDeleted.subscribe(function (nodeDeleted) {
		var pageID = $(nodeDeleted).attr('data-page-id');
		if (pageID && nodeDeleted.pageDeleted) {
			appViewModel.removeResultWithID(pageID);
			appViewModel.removeRecentPageWithID(pageID);
			appViewModel.tabMgr.removeTabByID(uniqueIDForPageTab(pageID));
		}
	});

	appViewModel.selectedSharedContentType.subscribe(function (newValue) {
		ajaxHelper.ajax('/Admin/Content/Shared', {
			data: { type: newValue },
			type: 'POST',
			beforeSend: function () {
				appViewModel.gettingSelectedSharedContent(true);
			},
			success: function (data, textStatus, jqXHR) {
				$('#shared-content-list-target').empty().append(data);
				$('.tab-display').prepend($('#shared-content').detach());
				appViewModel.gettingSelectedSharedContent(false);
			}
		});
	});
}

function initializeTabs() {
	appViewModel.tabMgr = new TabManager({
		closable: true,
		container: $('#app-index-tabs'),
		load_tabTypeSelector: '.app-index-tab',
		load_tabTypeActions: {
			"edit-page": initEditPageTab
		},
		numStaticTabs: 1,
		show_action: function (event, ui) {
			if (ui.index === 0) {
				$('#find-page-input').select();
			}
		}
	});
}
function initializeFindAccordion() {
	var findOptions = $('#find-options');
	findOptions.accordion({
		change: function (event, ui) {
			var focusOn = ui.newContent.find('#' + ui.newContent.attr('data-focus-id'));
			if (focusOn) {
				focusOn.select();
			}
		},
		changestart: function (event, ui) {
			if ($(this).accordion("option","active") === 2) {
				//content panel is being opened
				var initialized = ui.newContent.data('initialized');
				if (!initialized) {
					//AJAX call to load shared content graph
					refreshSharedContentMenu();
					ui.newContent.data('initialized', true);
				}
			}
		},
		fillSpace: true
	});

	cms.url.initFindUrlInput($('#find-url-input'));


	//	$('.refresh-shared-content', findOptions)
	//		.button({
	//			icons: {
	//				primary: 'ui-icon-refresh'
	//			},
	//			text: true
	//		})
	//		.on('click', function (event) {
	//			event.preventDefault();
	//			refreshSharedContentMenu();
	//		});
}

function refreshSharedContentMenu() {
	ajaxHelper.ajax('/Admin/Content/SharedContentMenu', {
		type: 'POST',
		beforeSend: function () {
			appViewModel.gettingSharedContentMenu(true);
		},
		success: function (data, textStatus, jqXHR) {
			appViewModel.clearSharedContentMenu();
			var results = data.Properties['shared-content'];
			$.each(results, function (index, item) {
				appViewModel.addSharedContentMenuItem(new sharedContentMenuItem(item.type, item.assemblyType, item.count, item.iconCssClass));
			});
			appViewModel.gettingSharedContentMenu(false);
		}
	});
}

//function initializeEditUrlModal() {
//	editUrlModal = $('#edit-url-modal').dialog({
//		title: 'URL',
//		modal: true,
//		resizable: true,
//		height: 300,
//		width: 500,
//		autoOpen: false,
//		buttons: {
//			Done: function () {
//				$(this).dialog('close');
//			}
//		}
//	});
//}

function deleteTreeviewNodes(nodes, deletePage) {
	if (nodes.length > 0) {
		var ids = '';
		nodes.each(function (index, item) {
			ids = ids + $(item).attr('id') + ',';
		});
		ajaxHelper.ajax('/Admin/MenuItem/Delete', {
			data: {
				ids: ids,
				deletePage: deletePage
			},
			type: 'POST',
			success: function (data, textStatus, jqXHR) {
				menuTreeview.jstree('remove', nodes);
				appViewModel.anyNodesChecked(false);
				if (deletePage) {
					nodes.each(function (index, item) {
						item.pageDeleted = deletePage;
						appViewModel.nodeJustDeleted(item);
					});
				}
				appViewModel.registerUnappliedChange();
			},
			failureMessageFormat: 'An error occurred trying to delete the menu items with the following IDs: [[errorThrown]]'
		});
	}
}

function initLinks() {
	//close buttons
	cms.makeCloseButton('#close-tabs-link',
		'#close-find-page-results', 
		'#close-menu',
		'#close-shared-content', 
		'#close-find-url-results');
	//Home Page link
	var link = $('#home-page-link a');
	link.on('click', function (event) {
		event.preventDefault();
		var link = $(this);
		var pageID = link.attr('data-page-id');
		var pageType = link.attr('data-page-type');
		var title = link.text();
		var page = new cmsPage(title, pageType, pageID);
		page.editPage();
	});
	link.button({
		icons: {
			primary: 'ui-icon-home'
		}
	});

    // show all pages
	var showAllPagesLink = $('#show-all-pages');
	showAllPagesLink.on('click', function (event) {
	    event.preventDefault();
	    cms.app.index.findPages();
	}).button({
	    icons: {
	        primary: 'ui-icon-gear'
	    }
	});

	newPageMgr.initNewPageLinks();
	//set up preview content links
	$('#app').on('click', '.preview-content-link', function (event) {
		event.preventDefault();
		var link = $(this),
			previewTarget = link.data('previewTarget');
		if (!previewTarget) {
			previewTarget = $('<div class="content-preview-target"></div>');
			link.data('previewTarget', previewTarget);
		}

		if (!link.data('loaded')) {
			ajaxHelper.ajax('/Admin/Content/Index', {
				type: 'POST',
				data: {
					contentID: link.attr('data-content-id')
				},
				success: function (data, textStatus, jqXHR) {
					var $data = $(data);
					previewTarget.append($data);
					link.data('loaded', true);
					previewTarget.dialog({
						title: 'Preview Content',
						modal: true,
						autoOpen: true,
						width: 'auto',
						height: 'auto'
					});
				}
			});
		}
		else {
			previewTarget.dialog('open');
		}
	});
	//some edit page links
	$('#app').on('click', '.edit-page-link', function (e) {
		e.preventDefault();
		var link = $(this),
		title = link.text(),
		pageID = link.attr('data-page-id'),
		pageType = link.attr('data-page-type');
		appViewModel.showEditPage(new cmsPage(title, pageType, pageID));

	});
	//set up standalone edit content links
	$('#app').on('click', '.edit-content-link', function (event) {
		event.preventDefault();
		var link = $(this),
			editorModal = link.data('editorModal');
		if (!editorModal) {
			editorModal = $('<div class="content-editor-modal"></div>');
			link.data('editorModal', editorModal);
		}

		if (!link.data('loaded')) {
			ajaxHelper.ajax('/Admin/Content/Edit', {
				type: 'POST',
				data: {
					contentID: link.attr('data-content-id')
				},
				success: function (data, textStatus, jqXHR) {
					var $data = $(data);
					previewTarget.append($data);
					link.data('loaded', true);
					previewTarget.dialog({
						title: 'Edit Content',
						modal: true,
						autoOpen: true,
						width: 'auto',
						height: 'auto'
					});
				}
			});
		}
		else {
			previewTarget.dialog('open');
		}
	});
}


function deletePage(pageID, panel) {
	ajaxHelper.ajax('/Admin/Page/Delete', {
		data: {
			pageID: pageID
		},
		type: 'POST',
		success: function (data, textStatus, jqXHR) {
			appViewModel.removePage(pageID);
		},
		failureMessageFormat: 'An error occurred trying to delete the page: [[errorThrown]]'
	});
}

//edit in place callbacks:
function cmsTemplateEdited(element) {
	//initialize content group modals
	var panel = $(element).parents('div[data-tab-type="edit-page"]'),
		appID = panel.attr('data-app-id'),
		pageID = panel.attr('data-page-id'),
		controllerDisplayName = $.trim($(element).text());

	contentGroupMgr.refreshContentGroups(panel, appID, pageID, controllerDisplayName);
}
