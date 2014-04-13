/// <reference path="http://clickfarmcdn.localhost/common/js/jquery/1.7.2/jquery-vsdoc.js" />
/// <reference path="http://clickfarmcdn.localhost/common/js/kendo/2012.2.710/kendo.all-vsdoc.js" />

var newPageMgr;

$(function () {
	newPageMgr = new newPageManager();

	$('a.new-page').on('click', function (event) {
		event.preventDefault();
		var link = $(this);
		newPageMgr.showNewPageForm(link.attr('data-page-type'), link.attr('data-menu-item-id'), link.attr('data-create-relationship'), link.attr('data-callback'));
	});
});

function newPageManager() {
	var self = this,
		formWrapper = $('#new-page-form'),
		errorElement = formWrapper.find('#new-page-error'),
		loader = formWrapper.find('#new-page-loading'),
		title = formWrapper.find('#Page_Title'),
		pageTypeInput = formWrapper.find('#page-type'),
		referenceNodeIDInput = formWrapper.find('#reference-node-id'),
		createRelationshipInput = formWrapper.find('#create-relationship'),
		currentCallback = null
	;

	this.ajaxBegin = function (args) {
		self.hideError();
	};

	this.ajaxSuccess = ajaxHelper.success(
		function (result, textStatus, jqXHR) {
			var pageTitle = result.Page.Title,
				pageType = result.PageType,
				pageID = result.Page.PageID
			;

			if (currentCallback) {
				window[currentCallback]({
					title: result.MenuItemTitle,
					pageType: pageType,
					pageID: pageID,
					referenceMenuItemID: referenceNodeIDInput.val(),
					createRelationship: createRelationshipInput.val(),
					menuItemID: result.MenuItemID,
					nodeType: 'page'
				});
			}
			self.resetNewPageForm();
			var newPage = new cmsPage(pageTitle, pageType, pageID);
			newPage.editPage();
			appViewModel.registerUnappliedChange();
		},
		function (data, textStatus, jqXHR, formattedMessage) {
			self.showError(formattedMessage);
		},
		false
	);

	this.ajaxFailure = ajaxHelper.error(
		function (jqXHR, textStatus, errorThrown, formattedMessage) {
			self.showError(formattedMessage);
		},
		false
	);

	this.showError = function (errorMessage) {
		errorElement.html(errorMessage);
		errorElement.fadeIn();
	};

	this.hideError = function () {
		errorElement.html('');
		errorElement.hide();
	};

	this.showNewPageForm = function (newPageType, referenceNodeID, createRelationship, callback) {
		newPageMgr.closeNewPageDialog()
		currentCallback = callback;
		//formWrapper.dialog('close');
		pageTypeInput.val(newPageType);
		self.hideError();
		referenceNodeIDInput.val(referenceNodeID);
		createRelationshipInput.val(createRelationship);
		title.val('New ' + newPageType);

		formWrapper.dialog({
			buttons: {
				'Create': function () {
					formWrapper.find('form').submit();
				}
			},
			modal: true,
			resizable: false,
			title: title.val(),
			width: 400
		});
		title.select();
	};

	this.resetNewPageForm = function () {
		formWrapper.dialog('close');
		self.hideError();
		loader.hide();
		formWrapper.hide();
		//self.alias.val(self.alias.prop('defaultValue'));
		title.val(title.prop('defaultValue'));
		referenceNodeIDInput.val(referenceNodeIDInput.prop('defaultValue'));
		createRelationshipInput.val(createRelationshipInput.prop('defaultValue'));
		currentCallback = null;
	};

	this.initNewPageLinks = function () {
		//format new page button or menu
		var newPageButton = $('.new-page-button');
		if (newPageButton.hasClass('single-page-type')) {
			//there's only one page type, so just make one new page button
			newPageButton.button({
				icons: {
					primary: 'ui-icon-plusthick'
				}
			});
		}
		else {
			//there's more than one page type, so we need a menu
			var newPageMenu = $('#new-page-menu').dialog({
				autoOpen: false,
				buttons: {
					'Cancel': function () {
						newPageMenu.dialog('close');
					}
				},
				draggable: false,
				resizable: false,
				width: 200
			});

			newPageButton.
				button({
					icons: {
						secondary: "ui-icon-triangle-1-s"
					}
				})
				.on('click', function (event) {
					event.preventDefault();
					if (newPageMenu.dialog('isOpen')) {
						newPageMenu.dialog('close');
					}
					else {
						var offset = $(this).offset();
						newPageMenu
							.dialog('option', 'position', [offset.left + 68, offset.top + 1])
							.dialog('open')
						;
					}
				})
			;
		}
	};
	this.closeNewPageDialog = function () {
		$('#new-page-menu').dialog('close');
	};
}
