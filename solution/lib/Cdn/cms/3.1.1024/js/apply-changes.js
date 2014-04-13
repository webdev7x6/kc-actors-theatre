/// <reference path="http://clickfarmcdn.localhost/common/js/jquery/1.7.2/jquery-vsdoc.js" />
/// <reference path="http://clickfarmcdn.localhost/common/js/kendo/2012.2.710/kendo.all-vsdoc.js" />

var applyChangesMgr;

$(function () {
	applyChangesMgr = new applyChangesManager();
	//refreshMgr.initialize();
});

function applyChangesManager() {
	var self = this;
	var loader = $('#apply-changes-loading');
	//var title = $('input#WebPageView_Title');
	//this.alias = $('input#WebPageView_Alias');
	//var form = $('#new-web-page-form');

	this.initialize = function () {
		//
	};

	this.ajaxBegin = function (args) {
		self.hideError();
		appViewModel.applyingChanges(true);
	};

	this.ajaxSuccess = ajaxHelper.success(
		function (result, textStatus, jqXHR) {
			appViewModel.applyingChanges(false);
			appViewModel.hasUnappliedChange(false);
		},
		function (data, textStatus, jqXHR, formattedMessage) {
			appViewModel.applyingChanges(false);
			self.showError(formattedMessage);
		},
		false
	);

	this.ajaxFailure = ajaxHelper.error(
		function (jqXHR, textStatus, errorThrown, formattedMessage) {
			appViewModel.applyingChanges(false);
			self.showError(formattedMessage);
		},
		false
	);

	this.showError = function (errorMessage) {
		dialogHelper.alert(errorMessage);
	};

	this.hideError = function () {
		dialogHelper.close();
	};
}
