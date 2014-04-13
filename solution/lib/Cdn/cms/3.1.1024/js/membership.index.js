/// <reference path="http://clickfarmcdn.localhost/common/js/jquery/1.7.2/jquery-vsdoc.js" />
/// <reference path="http://clickfarmcdn.localhost/common/js/kendo/2012.2.710/kendo.all-vsdoc.js" />

var membershipMgr = new membershipManager();
$(function () {
	membershipMgr.initUI();
});

function membershipManager() {
	var self = this;
	this.initUI = function () {
		self.initUIButtons();
		$('#membership-tabs').tabs();
		$('#membership').fadeIn('fast');
	};
	this.initUIButtons = function () {
		$('.new-member').button({
			icons: {
				primary: 'ui-icon-plusthick'
			}
		}).on('click', function (event) {
			event.preventDefault();
		});
	};
}