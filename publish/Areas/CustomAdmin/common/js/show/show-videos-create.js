/// <reference path="http://clickfarmcdn.localhost/common/js/jquery/1.7.2/jquery-vsdoc.js" />
/// <reference path="http://clickfarmcdn.localhost/common/js/kendo/2012.2.710/kendo.all-vsdoc.js" />

var createVideoManager;

$(function () {
    createVideoManager = new createVideoManager();

    $('#create-video')
        .button({
            icons: {
                primary: 'ui-icon-plusthick'
            }
        })
        .on('click', function (e) {
            e.preventDefault();
            createVideoManager.showCreateVideoForm();
        })
    ;
});

function createVideoManager() {
    var self = this,
        formEl = $('#create-video-form'),
        errorEl = formEl.find('#create-video-error'),
        loaderEl = formEl.find('#create-video-loading'),
		vimeoIdEl = formEl.find('#create-vimeo-id'),
        hiddenShowID = formEl.find('#create-video-show-id')
    ;
    formEl.dialog({
        autoOpen: false,
        buttons: {
            'Create': function () {
                formEl.find('form').submit();
            }
        },
        modal: true,
        resizable: false,
        width: 400
    });

    self.showCreateVideoForm = function () {
        self.resetCreateVideoForm();
        hiddenShowID.val(window.SHOW_ID);
        formEl.dialog('open');
        vimeoIdEl.select();
    };

    self.hideCreateVideoForm = function () {
        self.resetCreateVideoForm();
        formEl.dialog('close');
    };

    self.resetCreateVideoForm = function () {
        formEl.dialog('close');
        self.hideError();
        loaderEl.hide();
        vimeoIdEl.val('');
        hiddenShowID.val('');
    };

    self.ajaxBegin = function (jqXHR, settings) {
        self.hideError();
    };

    self.ajaxSuccess = ajaxHelper.success(
        function (result, textStatus, jqXHR) {
            self.hideCreateVideoForm();
            var newShowVideo = result.Properties.ShowVideo;

            // add to observable array
            window.associatedVideosViewModel.videos.push({
                videoID: newShowVideo.ShowVideoID,
                vimeoID: newShowVideo.VimeoID
            });

            // make rows editable to initialize new row
            $('.videos-table tbody tr').each(function (index, item) {
                admin.shows.videos.initEditInPlace(item);
            });

            //initialize buttons
            admin.shows.videos.initButtons();

        },
        function (data, textStatus, jqXHR, formattedMessage) {
            self.showError(formattedMessage);
        },
        false
    );

    self.ajaxFailure = ajaxHelper.error(
        function (jqXHR, textStatus, errorThrown, formattedMessage) {
            self.showError(formattedMessage);
        },
        false
    );

    self.showError = function (msg) {
        errorEl.html(msg).fadeIn();
    };

    self.hideError = function () {
        errorEl.html('').hide();
    };
}