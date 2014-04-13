/// <reference path="http://clickfarmcdn.localhost/common/js/jquery/1.7.2/jquery-vsdoc.js" />
/// <reference path="http://clickfarmcdn.localhost/common/js/kendo/2012.2.710/kendo.all-vsdoc.js" />

var menuItemMgr = new menuItemManager();

function pageMenuItemViewModel() {
	var self = this;
	this.menuItems = ko.observableArray([]);
	this.addMenuItem = function (item) {
		this.menuItems.push(item);
	};
	this.anyMenuItems = function () {
		return this.menuItems().length > 0;
	};
	this.clearMenuItems = function () {
		this.menuItems([]);
	};
	this.retrievingMenuItems = ko.observable(true);
	this.removeMenuItem = function (item, event) {
		event.preventDefault();
		dialogHelper.confirm(
			'Are you sure you want to delete this menu item? This item and all child menu items will also be deleted.',
			'Delete Menu Item',
			function () {
				menuItemMgr.deleteMenuItem(item, self);
			}
		);
	};
}

function menuItemManager() {
	var self = this;

	this.refreshMenuItems = function (viewModel, container, appID, pageID, callback) {
		var refreshLink = container.find('.refresh-menu-items');
		if (viewModel == null) {
			viewModel = new pageMenuItemViewModel();
			ko.applyBindings(viewModel, $(container).get(0));
		}
		viewModel.retrievingMenuItems(true);
		ajaxHelper.ajax('/Admin/MenuItem/ForPage', {
			type: 'POST',
			data: {
				appID: appID,
				pageID: pageID
			},
			success: function (data, textStatus, jqXHR) {
				viewModel.clearMenuItems();
				$.each(data.Properties.MenuItems, function (index, item) {
					var newMenuItem = new menuItem(item.title, item.menuItemID, item.url, null, item.isEnabled);
					newMenuItem.parent = item.parent;
					newMenuItem.menuName = item.menuName;
					newMenuItem.menuID = item.menuID;
					viewModel.addMenuItem(newMenuItem);
				});

				viewModel.retrievingMenuItems(false);
				refreshLink.off('click').on('click', function (event) {
					event.preventDefault();
					menuItemMgr.refreshMenuItems(viewModel, container, appID, pageID);
				});
				//init edit in place for each MenuItem in the table
				container.find('.menu-table tbody tr').each(function (index, item) {
					var item$ = $(item),
						menuItemEditMgr = new EditableManager(item$, { menuitemid: item$.attr('data-menuitem-id') }, '/Admin/MenuItem/EditInPlace')
					;
					menuItemEditMgr.initTextboxes();
					menuItemEditMgr.initSelects();
					menuItemEditMgr.initYesNoBoolean();
					item$.data('editUrlCallback', function () {
						menuItemMgr.refreshMenuItems(viewModel, container, appID, pageID);
					});
				});
				menuItemMgr.processDeleteLinks(container);
				cms.url.setUrlLinkHref(container).buttonizeUrlLinks(container);
				cms.doCallback(callback);
			},
			failure: function (data, textStatus, jqXHR, formattedMessage) {
				//callback for handling failure, i.e. data.Succeeded === false
				viewModel.retrievingMenuItems(false);
				cms.doCallback(callback);
			},
			failureDoAlert: true,
			errorDoAlert: true
		});
	};

	this.updateUrlCallback = function (element, data) {
		element.parent('td').parent('tr').data('editUrlCallback')();
	};

	this.deleteMenuItem = function (menuItem, viewModel) {
		ajaxHelper.ajax('/Admin/MenuItem/Delete', {
			type: 'POST',
			data: {
				ids: menuItem.menuItemID,
				deletePage: false
			},
			success: function (data, textStatus, jqXHR) {
				appViewModel.registerUnappliedChange();
				viewModel.menuItems.remove(menuItem);
			},
			failureMessageFormat: 'An error occurred trying to delete the menu item: [[errorThrown]]'
		});
	};

	this.initMenuItemOptionLinks = function (panel) {
		panel.find('.refresh-menu-items').button({
			icons: {
				primary: 'ui-icon-refresh'
			},
			text: false
		});
	};

	this.processDeleteLinks = function (container) {
		$(container).find('.delete-menu-item').each(function (index, link) {
			var lnk = $(link);
			lnk.button({
				icons: {
					primary: 'ui-icon-trash'
				},
				text: false
			});
		});
	};
}
