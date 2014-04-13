/// <reference path="http://clickfarmcdn.localhost/common/js/jquery/1.7.2/jquery-vsdoc.js" />
/// <reference path="http://clickfarmcdn.localhost/common/js/kendo/2012.2.710/kendo.all-vsdoc.js" />

$(function () {
	var input = $('#find-page-input');
	$("#find-page-input").keyup($.debounce(350, function () {
		if (input.val().length > 1) {
			ajaxHelper.ajax('/Admin/Page/FindPage', {
				data: {
					term: input.val(),
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
		}
	}));

});
