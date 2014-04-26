/// <reference path="http://clickfarmcdn.localhost/common/js/jquery/1.7.2/jquery-vsdoc.js" />
/// <reference path="http://clickfarmcdn.localhost/common/js/kendo/2012.2.710/kendo.all-vsdoc.js" />

var createImageManager;

$(function () {
    createImageManager = new createImageManager();

    $('#create-image')
        .button({
            icons: {
                primary: 'ui-icon-plusthick'
            }
        })
        .on('click', function (e) {
            e.preventDefault();
            createImageManager.showCreateImageForm();
        })
    ;
});

function createImageManager() {
    var self = this,
        formEl = $('#create-image-form'),
        errorEl = formEl.find('#create-image-error'),
        loaderEl = formEl.find('#create-image-loading'),
		imageUrlEl = formEl.find('#create-image-url'),
        hiddenShowID = formEl.find('#create-image-show-id')
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

    self.showCreateImageForm = function () {
        self.resetCreateImageForm();
        hiddenShowID.val(window.SHOW_ID);
        formEl.dialog('open');
        nameEl.select();
    };

    self.hideCreateImageForm = function () {
        self.resetCreateImageForm();
        formEl.dialog('close');
    };

    self.resetCreateImageForm = function () {
        formEl.dialog('close');
        self.hideError();
        loaderEl.hide();
        imageUrlEl.val('');
        hiddenShowID.val('');
    };

    self.ajaxBegin = function (jqXHR, settings) {
        self.hideError();
    };

    self.ajaxSuccess = ajaxHelper.success(
        function (result, textStatus, jqXHR) {
            self.hideCreateImageForm();

            var newShowImage = result.Properties.ShowImage;

            // add to observable array
            window.associatedImagesViewModel.images.push({
                imageID: newShowImage.ShowImageID,
                imageURL: newShowImage.ImageURL
            });

            // make rows editable to initialize new row
            $('.images-table tbody tr').each(function (index, item) {
                admin.shows.images.initEditInPlace(item);
            });

            //initialize buttons
            admin.shows.images.initButtons();

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