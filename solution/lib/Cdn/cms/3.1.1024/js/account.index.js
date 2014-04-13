/// <reference path="http://clickfarmcdn.localhost/common/js/jquery/1.7.2/jquery-vsdoc.js" />
/// <reference path="http://clickfarmcdn.localhost/common/js/kendo/2012.2.710/kendo.all-vsdoc.js" />

$(function () {
    var tabs = $('#account-index-tabs');
    tabs.tabs();
    initEditCmsUserTab(tabs)
});

function cmsUserPropertyUpdated(element, data) {
	if (data.Property === 'Email') {
        updateCurrentCmsUserProfileLinkText(data.NewValue);
    }
}
