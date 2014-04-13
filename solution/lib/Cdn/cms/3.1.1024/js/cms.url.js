/// <reference path="http://clickfarmcdn.localhost/common/js/jquery/1.7.2/jquery-vsdoc.js" />
/// <reference path="http://clickfarmcdn.localhost/common/js/kendo/2012.2.710/kendo.all-vsdoc.js" />

//some notes on URLs:
// the cms.url object, the cms.url.UrlManager and cms.url.UrlViewModel classes are utilized in interface
//elements that display lists of CMS URLs either in the Page Edit interface or on the App index page for
//the "search URLs by keyword" feature. They're organized like this to provide some universal functionality that
//can be shared between these interfaces and also with a front-end CMS editing scenario when we get around to that

//Any time a set of URLs needs to be displayed on a page, a new UrlManager is instantiated and initialize() is 
//called on it. The refreshUrls() method is called any time you want to draw the URL data from an MVC action and bind 
//it for display on the page (currently assumes use of Knockout JS).

//Some things to note:
//when UrlManager.initialize() is called, the constructor 1) stores the UrlManager instance with the jQuery data() method
//on the domContainer itsef, and 2) creates a new UrlViewModel instance (for binding with Knockout) and stores that on the
//domContainer with the data() method as well
//to summarize, each display of URLs on any tabbed panel or interface has its own UrlMananger and UrlViewModel
//that's persisted on the containing DOM element and used for Knockout binding in case URLs are edited, added, deleted, etc.

(function ($, undefined) {
	var cms = window.cms = window.cms || {};

	(function () {
		//a url object with some url-related utility functions
		cms.url = new (function () {
			return {
				buttonizeUrlLinks: function (domContainer) {
					$(domContainer).find('.url-link').button({
						icons: {
							primary: 'ui-icon-extlink'
						}
					});
					return this;
				},

				setUrlLinkHref: function (domContainer) {
					$(domContainer).find('.url-link').each(function (index, link) {
						var lnk = $(link),
								hostName = lnk.attr('data-host-name'),
								path = lnk.attr('data-path');
						lnk.attr('href', '//' + hostName + path);
					});
					return this;
				},

				buttonizeDeleteLinks: function (domContainer) {
					cms.buttonizeDeleteLinks(domContainer, '.delete-url');
					return this;
				},

				getUrlManager: function (domContainer) {
					return $(domContainer).data('urlMgr');
				},

				setUrlManager: function (domContainer, urlManager) {
					$(domContainer).data('urlMgr', urlManager);
				},

				getClosestUrlManager: function (ancestor) {
					//return this.getUrlManager($(ancestor).closest('.url-container'));//.data('urlMgr')
					return $(ancestor).closest('.url-container').data('urlMgr');
				},

				getUrlContainer: function (panel) {
					return panel.find('.url-container');
				},

				initEditInPlace: function (urlTableRow) {
					var urlEditMgr = new EditableManager($(urlTableRow), { urlid: $(urlTableRow).attr('data-url-id') }, '/Admin/Url/EditInPlace');
					urlEditMgr.initCmsUrls();
					urlEditMgr.initYesNoBoolean();
					urlEditMgr.initTextboxes();
				},

				initializeUrlInput: function (input) {
					var $inp = $(input).on('input propertychange', function (event) {
						$inp.val(cms.url.sanitizeAlias($inp.val()));
					});
				},

				sanitizeAlias: function (str) {
					if (str.substring(0, 1) != '/') {
						str = '/' + str;
					}
					return cms.removeDiacritics(str).toLowerCase()
						.replace(cms.url.sanitizeAliasRegExps.reservedChars, '') //replace reserved characters with blank
						.replace(cms.url.sanitizeAliasRegExps.whiteSpace, '-') //replace spaces with dashes
						.replace(cms.url.sanitizeAliasRegExps.multipleDashes, '-') //replace more than one dash with a single dash
						.replace(cms.url.sanitizeAliasRegExps.multipleSlashes, '/') //replace more than one forward slash with a single one
					;
				},

				sanitizeAliasRegExps: {
					reservedChars: new RegExp('[\\$\\&\\+,:;=\\?@\'\\"<>#%\\|\\^{}\\[\\]~`*\\(\\)\\\\]', 'gi'),
					whiteSpace: new RegExp('\\s', 'gi'),
					multipleDashes: new RegExp('-{2,}', 'gi'),
					multipleSlashes: new RegExp('/{2,}', 'gi')
				},

				deleteUrl: function (url, viewModel) {
					ajaxHelper.ajax('/Admin/Url/Delete', {
						type: 'POST',
						data: {
							id: url.urlID
						},
						success: function (data, textStatus, jqXHR) {
							appViewModel.registerUnappliedChange();
							//var viewModel = getViewModel();
							if (viewModel) {
								viewModel.urls.remove(url);
							}
						},
						failureMessageFormat: 'An error occurred trying to delete the URL: [[errorThrown]]'
					});
				},

				//the following functions, newUrlSuccess(), newUrlFailure() are utilized with the 
				//create new URL form, /Views/Shared/CreateUrl.cshtml
				newUrlSuccess: ajaxHelper.success(
					function (result, textStatus, jqXHR) {
						var url = result.Properties.url,
							urlPage = new cmsPage(url.page.title, url.page.pageType, url.page.pageID),
							newUrl = new cmsUrl(url.isEnabled, url.origin, url.path, url.urlID, urlPage, url.cmsKey),
							pageID = url.page.pageID,
							panel = cms.findTabPanelForPage(pageID),
							container = cms.url.getUrlContainer(panel),
							urlMgr = cms.url.getUrlManager(container),
							viewModel = urlMgr.getViewModel();
						urlMgr.hideNewUrlError();
						viewModel.addUrl(newUrl);
						//var domContainer = thisObj.domContainer;
						cms.url.setUrlLinkHref(container).buttonizeUrlLinks(container).buttonizeDeleteLinks(container);
						appViewModel.registerUnappliedChange();
						urlMgr.resetNewUrlForm(urlMgr);
						cms.url.initEditInPlace(container.find('.url-table tbody tr:last'));
					},
					function (data, textStatus, jqXHR, formattedMessage) {
						//TODO:code below copied from this - need to make it DRY
						var pageID = data.Properties.pageID,
							panel = cms.findTabPanelForPage(pageID),
							container = cms.url.getUrlContainer(panel),
							urlMgr = cms.url.getUrlManager(container);

						urlMgr.showNewUrlError(formattedMessage);
					},
					false
				),

				newUrlFailure: ajaxHelper.error(
				//TODO:copied code from above - need to make it DRY
				//move this code to get the urlMgr with a pageID out into cms.url?
					function (jqXHR, textStatus, errorThrown, formattedMessage) {
						var pageID = data.Properties.pageID,
							panel = cms.findTabPanelForPage(pageID),
							container = cms.url.getUrlContainer(panel),
							urlMgr = cms.url.getUrlManager(container);

						urlMgr.showNewUrlError(formattedMessage);
					},
					false
				),

				urlEditCallback: function (element, data) {
					if (data.Succeeded) {
						var newPath = data.NewValue,
						urlID = data.UniqueID;
						$('.url-link[data-url-id=' + urlID + ']').attr('data-path', newPath);
						cms.url.setUrlLinkHref($('body'));
						$('.url-label[data-url-id=' + urlID + ']').text(newPath);
					}
				},

				//this function used to initialize the search URLs by keyword input
				initFindUrlInput: function (input) {
					//full disclosure: the find URL functionality on the app index interface in the accordion
					//does not fully utilize the cms.url.UrlManager. Until the app.index.js and appViewModel
					//can be refactored to not use one big monolithic view model object that databinds to the whole page, 
					//this URL search feature will have to stick with the way that works and wait to be refactored with it.
					//as a result, this won't have the flexibility to be used on a separate, front-end admin interface until it's refactored

					$(input).keyup($.debounce(350, function () {
						if (input.val().length > 1) {
							ajaxHelper.ajax('/Admin/Url/FindUrl', {
								data: {
									term: input.val(),
									appID: $('#app').attr('data-app-id')
								},
								type: 'POST',
								success: function (data, textStatus, jqXHR) {
									appViewModel.clearFindUrlResults();
									if (data.Succeeded && data.Properties.Urls.length > 0) {
										$.each(data.Properties.Urls, function (index, item) {
											//push results into view model's observable collection of views
											appViewModel.addUrlResult(new cmsUrl(item.isEnabled, item.origin, item.path, item.urlID, new cmsPage(item.pageTitle, item.pageType, item.pageID), item.cmsKey));
										});
										appViewModel.anyUrls(true);
										var domContainer = $('#find-url-results');
										cms.url.setUrlLinkHref(domContainer).buttonizeUrlLinks(domContainer).buttonizeDeleteLinks(domContainer);
										domContainer.find('.url-table tbody tr').each(function (index, item) {
											cms.url.initEditInPlace(item);
										});

									}
									else {
										appViewModel.anyUrls(false);
									}
									appViewModel.findUrlResultsVisible(true);
								},
								beforeSend: function (jqXHR, settings) {
									appViewModel.retrievingUrls(true);
								},
								complete: function (jqXHR, textStatus) {
									appViewModel.retrievingUrls(false);
								}
							});
						}
					}));
				}
			};
		});

		cms.url.UrlManager = function (domContainer, ajaxUrl, ajaxData, callback) {
			//this.urlViewModel = urlViewModel;
			this.domContainer = $(domContainer);
			cms.url.setUrlManager(domContainer, this);
			this.ajaxUrl = ajaxUrl;
			this.ajaxData = ajaxData;
			this.callback = callback;
			this.urlForm = this.domContainer.find('.new-url-form');
			//this.urlForm.data('urlManager', this);
			//this.urlContainer = this.domContainer.find('.url-container');
			//this.$this = this;
		};

		cms.url.UrlManager.prototype = function () {
			var initialize = function () {
				var viewModel = new cms.url.UrlViewModel(this);
				setViewModel.call(this, viewModel);
				ko.applyBindings(viewModel, this.domContainer.get(0));
				initUrlOptionLinks.call(this);
				cms.url.initializeUrlInput($('input[type="text"]', this.urlForm));
			},

			//getUrlContainer = function (container) {
			//	return $(container).find('.url-container');
			//},

			setViewModel = function (newViewModel) {
				this.domContainer.data('viewModel', newViewModel)
			},

			getViewModel = function () {
				return this.domContainer.data('viewModel');
			},

			refreshUrls = function () {
				var refreshLink = this.domContainer.find('.refresh-urls'),
				thisUrlMgr = this,
				viewModel = getViewModel.call(thisUrlMgr);
				if (!viewModel) { throw "While attempting to refresh URLs, the view model is not properly initialized."; }
				viewModel.retrievingUrls(true);
				ajaxHelper.ajax(thisUrlMgr.ajaxUrl, {
					type: 'POST',
					data: thisUrlMgr.ajaxData,
					success: function (data, textStatus, jqXHR) {
						viewModel.clearUrls();
						$.each(data.Properties.Urls, function (index, item) {
							var newUrl = new cmsUrl(item.isEnabled, item.origin, item.path, item.urlID, null, item.cmsKey);
							//var cmsUrl = function (isEnabled, origin, path, urlID, cmsPage)
							viewModel.addUrl(newUrl);
						});

						viewModel.retrievingUrls(false);
						refreshLink.off('click').on('click', function (event) {
							event.preventDefault();
							thisUrlMgr.refreshUrls();
						});
						var domContainer = thisUrlMgr.domContainer;
						//init edit in place for each Url in the table
						domContainer.find('.url-table tbody tr').each(function (index, item) {
							cms.url.initEditInPlace(item);
						});
						//init URL links
						cms.url.setUrlLinkHref(domContainer).buttonizeUrlLinks(domContainer).buttonizeDeleteLinks(domContainer);
						cms.doCallback(thisUrlMgr.callback);
					},
					failure: function (data, textStatus, jqXHR, formattedMessage) {
						//callback for handling failure, i.e. data.Succeeded === false
						viewModel.retrievingUrls(false);
						cms.doCallback(thisUrlMgr.callback);
					},
					failureDoAlert: true,
					errorDoAlert: true
				});
			},

			//initEditInPlace = function (urlTableRow) {
			//	var urlEditMgr = new EditableManager($(urlTableRow), { urlid: $(urlTableRow).attr('data-url-id') }, '/Admin/Url/EditInPlace');
			//	urlEditMgr.initCmsUrls();
			//	urlEditMgr.initYesNoBoolean();
			//},

			initUrlOptionLinks = function () {
				this.domContainer.find('.refresh-urls').button({
					icons: {
						primary: 'ui-icon-refresh'
					},
					text: false
				});
				var thisUrlMgr = this;
				this.domContainer.find('.new-url')
					.button({
						icons: {
							primary: 'ui-icon-circle-plus'
						},
						text: false
					})
					.off('click')
					.on('click', function (event) {
						event.preventDefault();
						handleNewUrlClick(thisUrlMgr);
					});

				this.domContainer.find('.cancel-new-url').off('click').on('click', function (event) {
					event.preventDefault();
					resetNewUrlForm(thisUrlMgr);
				});
			},

			handleNewUrlClick = function (aUrlMgr) {
				resetNewUrlForm(aUrlMgr);
			    aUrlMgr.urlForm.dialog('open');
				aUrlMgr.urlForm.find('input.url-input').select();
			},

			resetNewUrlForm = function (aUrlMgr) {
				var input = aUrlMgr.urlForm.find('input.url-input');
				input.val(input.prop('defaultValue'));
				aUrlMgr.hideNewUrlError();
				aUrlMgr.urlForm.find('.loader').hide();
				aUrlMgr.urlForm.find('.new-url-error').hide();
				
			    if (aUrlMgr.urlForm.is(':data(dialog)')) {
			        aUrlMgr.urlForm.dialog('close');
			    } else {
			        aUrlMgr.urlForm.dialog({autoOpen:false,modal:true});
			    }
			},

			showNewUrlError = function (errorMessage) {
				this.urlForm.find('.new-url-error').html(errorMessage);
				this.urlForm.find('.new-url-error').fadeIn();
			},

			hideNewUrlError = function () {
				this.urlForm.find('.new-url-error').html('');
				this.urlForm.find('.new-url-error').hide();
			};

			//deleteUrl = function (url) {
			//	var viewModel = getViewModel.call(this);
			//	ajaxHelper.ajax('/Admin/Url/Delete', {
			//		type: 'POST',
			//		data: {
			//			id: url.urlID
			//		},
			//		success: function (data, textStatus, jqXHR) {
			//			appViewModel.registerUnappliedChange();
			//			//var viewModel = getViewModel();
			//			viewModel.urls.remove(url);
			//		},
			//		failureMessageFormat: 'An error occurred trying to delete the URL: [[errorThrown]]'
			//	});
			//},

			//TODO: this function should go in a higher level module like cms.js for shared usage
			//doCallback = function (callback) {
			//	if (callback && $.isFunction(callback)) {
			//		callback();
			//	}
			//};

			return {
				initialize: initialize,
				refreshUrls: refreshUrls,
				getViewModel: getViewModel,
				showNewUrlError: showNewUrlError,
				hideNewUrlError: hideNewUrlError,
				//initEditInPlace: initEditInPlace,
				resetNewUrlForm: resetNewUrlForm
			};
		} ();

		cms.url.UrlViewModel = function (urlMgr) {
			this.urls = ko.observableArray([]);
			this.retrievingUrls = ko.observable(true);
			this.urlMgr = urlMgr;
		};

		cms.url.UrlViewModel.prototype = function () {
			var addUrl = function (url) {
				this.urls.push(url);
			},

			anyUrls = function () {
				return this.urls().length > 0;
			},

			clearUrls = function () {
				this.urls([]);
			},

			removeUrl = function (url, event) {
				var viewModel = cms.url.getClosestUrlManager($(event.currentTarget)).getViewModel();
				event.preventDefault();
				dialogHelper.confirm(
					'Are you sure you want to remove this URL?',
					'Remove URL',
					function () { cms.url.deleteUrl(url, viewModel); }
				);
			};

			return {
				addUrl: addUrl,
				anyUrls: anyUrls,
				clearUrls: clearUrls,
				removeUrl: removeUrl
			};
		} ();
	})();
})(jQuery);

var cmsUrl = function (isEnabled, origin, path, urlID, cmsPage, cmsKey) {
	var self = this;
	this.isEnabled = isEnabled;
	this.origin = origin;
	this.path = path;
	this.urlID = urlID;
	this.cmsPage = cmsPage;
	this.hasPage = function () { return self.cmsPage != null; };
	this.cmsKey = cmsKey;
}
