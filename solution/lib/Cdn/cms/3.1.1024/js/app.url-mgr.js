/// <reference path="http://clickfarmcdn.localhost/common/js/jquery/1.7.1/jquery-vsdoc.js" />
/// <reference path="http://clickfarmcdn.localhost/common/js/kendo/2012.2.621/kendo.all-vsdoc.js" />

var pageUrlMgr = new pageUrlManager();

function pageUrlViewModel() {
	var self = this;
	this.urls = ko.observableArray([]);
	this.addUrl = function (url) {
		this.urls.push(url);
	};
	this.anyUrls = function () {
		return this.urls().length > 0;
	};
	this.clearUrls = function () {
		this.urls([]);
	};
	this.retrievingUrls = ko.observable(true);
	this.removeUrl = function (url, event) {
		event.preventDefault();
		dialogHelper.confirm(
			'Are you sure you want to remove this URL?',
			'Remove URL',
			function () {
				pageUrlMgr.deleteUrl(url, self);
			}
		);
	};
}

function pageUrlManager() {
	var self = this,
		urlForm
	;

	this.getUrlContainer = function (panel) {
		return panel.find('.url-container');
	};

	this.refreshUrls = function (viewModel, panel, appID, pageID, callback) {
		var container = pageUrlMgr.getUrlContainer(panel),
			refreshLink = container.find('.refresh-urls');
		if (viewModel == null) {
			viewModel = new pageUrlViewModel();
			ko.applyBindings(viewModel, $(container).get(0));
			container.data('viewModel', viewModel);
		}
		viewModel.retrievingUrls(true);
		ajaxHelper.ajax('/Admin/Url/ForPage', {
			type: 'POST',
			data: {
				appID: appID,
				pageID: pageID
			},
			success: function (data, textStatus, jqXHR) {
				viewModel.clearUrls();
				$.each(data.Properties.Urls, function (index, item) {
					var newUrl = new cmsUrl(item.isEnabled, item.origin, item.path, item.urlID, null);
					//var cmsUrl = function (isEnabled, origin, path, urlID, cmsPage)
					viewModel.addUrl(newUrl);
				});

				viewModel.retrievingUrls(false);
				refreshLink.off('click').on('click', function (event) {
					event.preventDefault();
					pageUrlMgr.refreshUrls(viewModel, panel, appID, pageID);
				});
				//init edit in place for each Url in the table
				container.find('.url-table tbody tr').each(function (index, item) {
					pageUrlMgr.initEditInPlace(item);
				});
				//init URL links
				pageUrlMgr.setUrlLinkHref(container);
				pageUrlMgr.buttonizeUrlLinks(container);
				pageUrlMgr.processDeleteLinks(container);
				self.doCallback(callback);
			},
			failure: function (data, textStatus, jqXHR, formattedMessage) {
				//callback for handling failure, i.e. data.Succeeded === false
				viewModel.retrievingUrls(false);
				self.doCallback(callback);
			},
			failureDoAlert: true,
			errorDoAlert: true
		});
	};

	this.initEditInPlace = function (urlTableRow) {
		var urlEditMgr = new EditableManager($(urlTableRow), { urlid: $(urlTableRow).attr('data-url-id') }, '/Admin/Url/EditInPlace');
		urlEditMgr.initCmsUrls();
		urlEditMgr.initYesNoBoolean();
	};

	this.initUrlOptionLinks = function (panel) {
		panel.find('.refresh-urls').button({
			icons: {
				primary: 'ui-icon-refresh'
			},
			text: false
		});
		panel
			.find('.new-url')
			.button({
				icons: {
					primary: 'ui-icon-circle-plus'
				},
				text: false
			})
			.off('click')
			.on('click', new NewUrlClickHandler(panel))
		;
	};

	function NewUrlClickHandler(panel) {
		var p = panel,
			form
		;
		return function (event) {
			event.preventDefault();
			if (!form) {
				form = p.find('.new-url-form');
			}
			urlForm = form;
			pageUrlMgr.resetNewUrlForm();
			urlForm.dialog({
				buttons: {
					'Create': function () {
						urlForm.find('form').submit();
					}
				},
				modal: true,
				resizable: false,
				width: 400
			});
			urlForm.find('input.url-input').select();
		};
	}

	this.resetNewUrlForm = function () {
		var input = urlForm.find('input.url-input');
		input.val(input.prop('defaultValue'));
		self.hideError();
		urlForm.find('.loader').hide();
		urlForm.dialog('close');
	};

	this.doCallback = function (callback) {
		if ($.isFunction(callback)) {
			callback();
		}
	};

	this.deleteUrl = function (url, viewModel) {
		ajaxHelper.ajax('/Admin/Url/Delete', {
			type: 'POST',
			data: {
				id: url.urlID
			},
			success: function (data, textStatus, jqXHR) {
				appViewModel.registerUnappliedChange();
				viewModel.urls.remove(url);
			},
			failureMessageFormat: 'An error occurred trying to delete the URL: [[errorThrown]]'
		});
	};

	this.initializeUrlInput = function (panel) {
		var input = panel.attr('class') == 'url-input' ? panel : panel.find('input.url-input');
		$(input).keyup(function (event) {
			$(this).val(pageUrlMgr.sanitizeAlias($(this).val()));
		});
	};

	this.sanitizeAlias = function (str) {
		if (str.substring(0, 1) != '/') {
			str = '/' + str;
		}
		return str.toLowerCase()
			.replace(new RegExp("\\s", "gi"), "-")//replace spaces with dashes
			.replace(new RegExp("-{2,}", "gi"), "-")//replace more than one dash with a single dash
			.replace(new RegExp("[\\$\\&\\+,:;=\\?@'\"<>#%\\\\|\\^{}\\[\\]~`*\(\)]", "gi"), "")
			.replace(new RegExp("/{2,}", "gi"), "/")//replace more than one forward slash with a single one
		; //replace reserved characters with blank
		//.replace(new RegExp("-{1}$", "gi"), ""); //replace exactly one trailing dash with blank
	};

	this.urlEditCallback = function (element, data) {
		if (data.Succeeded) {
			var newPath = data.NewValue,
				urlID = data.UniqueID;
			$('.url-link[data-url-id=' + urlID + ']').attr('data-path', newPath);
			pageUrlMgr.setUrlLinkHref($('body'));
			$('.url-label[data-url-id=' + urlID + ']').text(newPath);
		}
	};

	this.setUrlLinkHref = function (container) {
		$(container).find('.url-link').each(function (index, link) {
			var lnk = $(link),
				hostName = lnk.attr('data-host-name'),
				path = lnk.attr('data-path');

			lnk.attr('href', '//' + hostName + path);
		});
	};

	this.buttonizeUrlLinks = function (container) {
		$(container).find('.url-link').button({
			icons: {
				primary: 'ui-icon-extlink'
			}
		});
	};

	this.processDeleteLinks = function (container) {
		$(container).find('.delete-url').each(function (index, link) {
			var lnk = $(link);
			lnk.button({
				icons: {
					primary: 'ui-icon-trash'
				},
				text: false
			});
		});
	};

	this.newUrlBegin = function (args) {
		self.hideError();
	};

	this.newUrlSuccess = ajaxHelper.success(
		function (result, textStatus, jqXHR) {
			var url = result.Properties.url,
				urlPage = new cmsPage(url.page.title, url.page.pageType, url.page.pageID),
				newUrl = new cmsUrl(url.isEnabled, url.origin, url.path, url.urlID, urlPage),
				pageID = url.page.pageID,
				panel = pageUrlMgr.findTabPanelForPage(pageID),
				container = pageUrlMgr.getUrlContainer(panel),
				viewModel = container.data('viewModel');

			viewModel.addUrl(newUrl);
			pageUrlMgr.setUrlLinkHref(container);
			pageUrlMgr.buttonizeUrlLinks(container);
			pageUrlMgr.processDeleteLinks(container);
			appViewModel.registerUnappliedChange();
			pageUrlMgr.resetNewUrlForm();
			pageUrlMgr.initEditInPlace(container.find('.url-table tbody tr:last'));
		},
		function (data, textStatus, jqXHR, formattedMessage) {
			self.showError(formattedMessage);
		},
		false
	);

	this.newUrlFailure = ajaxHelper.error(
		function (jqXHR, textStatus, errorThrown, formattedMessage) {
			self.showError(formattedMessage);
		},
		false
	);

	this.showError = function (errorMessage) {
		urlForm.find('.new-url-error').html(errorMessage);
		urlForm.find('.new-url-error').fadeIn();
	};

	this.hideError = function () {
		urlForm.find('.new-url-error').html('');
		urlForm.find('.new-url-error').hide();
	};

	this.findTabPanelForPage = function (pageID) {
		return $('div.app-index-tab[data-page-id="' + pageID.toString() + '"]');
	};
}
