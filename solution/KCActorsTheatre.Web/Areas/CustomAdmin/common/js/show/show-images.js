/// <reference path="http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.8.3-vsdoc.js" />
/// <reference path="http://clickfarmcdn.localhost/common/js/knockout/2.1.0/knockout.js" />
/// <reference path="http://clickfarmcdn.localhost/common/js/kendo/2012.2.710/kendo.all-vsdoc.js" />

(function ($, undefined) {
    var admin = window.admin = window.admin || {};
    admin.shows = admin.shows = admin.shows || {};
    (function () {
        admin.shows.images = (function () {
            var $dlg = null,
                closeCallback = null,
                $associatedImages = null
            ;

            window.associatedImagesViewModel = {
                images: ko.observableArray([]),
                removeImage: function (image, event) {
                    event.preventDefault();
                    dialogHelper.confirm(
                        'Are you sure you want to remove this image?',
                        'Remove Image',
                        function () {
                            ajaxHelper.ajax(admin.RemoveImageURL, {
                                type: 'POST',
                                data: {
                                    id: image.imageID
                                },
                                success: function (data, textStatus, jqXHR) {
                                    window.associatedImagesViewModel.images.remove(image);
                                },
                                failureMessageFormat: 'An error occurred trying to remove the image: [[errorThrown]]'
                            });

                            
                        }
                    );            
                }
            };

            window.SHOW_ID = null;

            return {
                init: function _init($modal) {
                    $associatedImages = $('#images-tbody');
                    $dlg = $modal.dialog({
                        autoOpen: false,
                        modal: true,
                        resizable: false,
                        width: 930,
                        buttons: {
                            Close: function _closeBtn_onClick() {
                                $dlg.dialog('close');
                            }
                        },
                        close: function _dlg_onClose() {

                            var $list = $('.editable-parent[data-item-id="' + window.SHOW_ID + '"] .edit-associated-images');
                            $list.html('');

                            $associatedImages.find('tr').each(function _images_onEach(ix, el) {
                                var $el = $(el);
                                if ($el.css('display') != 'none') // only add if not hidden
                                {
                                    var id = $el.attr('data-image-id'),
                                        imageURL = $el.find('td [data-property-name="ImageURL"]').attr('data-image-url')
                                    ;

                                    $list.append('<img data-image-id="' + id + '" src="' + imageURL + '" />&nbsp;&nbsp;&nbsp;&nbsp;');

                                }
                            });
                        }
                    });
                    ko.applyBindings(window.associatedImagesViewModel, $associatedImages[0]);
                },
                initButtons: function _initButtons() {
                    $('.delete-image-link').each(function (index, link) {
                        var lnk = $(link);
                        lnk.button({
                            icons: {
                                primary: 'ui-icon-trash'
                            },
                            text: false
                        });
                    });
                },

                initEditInPlace: function (imageTableRow) {
                    var imageEditMgr = new EditableManager($(imageTableRow), { imageID: $(imageTableRow).attr('data-image-id') }, '/CustomAdmin/Show/EditImageInPlace');
                    imageEditMgr.initAllTypes();
                },

                imageClick: function (container, parent) {
                    var
                        $container = $(container),
                        images = []
                        //$images = $container.find('ul')
                    ;

                    $container.find('img').each(function _images_onEach(ix, el) {
                        var $el = $(el);
                        images.push({
                            imageID: $el.attr('data-image-id'),
                            imageURL: $el.attr('src'),
                        });
                    });

                    admin.shows.images.show(images, parent.attr('data-item-id'));
                },

                // displays image edit modal
                show: function _show(images, showID, onClose) {
                    associatedImagesViewModel.images([]);
                    $associatedImages.empty();

                    // push each item to table
                    $.each(images, function _images_onEach(ix, item) {
                        associatedImagesViewModel.images.push(item);
                    });

                    // make each row editable
                    $('.images-table tbody tr').each(function (index, item) {
                        admin.shows.images.initEditInPlace(item);
                    });

                    // make rows sortable
                    $('.images-table tbody').sortable({
                        handle: '.sort-order',
                        update: function (event, ui) {
                            var imageIDs = $(this).sortable('toArray');
                            admin.shows.images.updateImageDisplayOrder(imageIDs);
                        }
                    });

                    admin.shows.images.initButtons();
                    window.SHOW_ID = showID;
                    closeCallback = onClose;
                    $dlg.dialog('open');
                },
                updateImageDisplayOrder: function (imageIDs) {
                    ajaxHelper.ajax(admin.UpdateImageDisplayOrderURL, {
                        data: { imageIDs: imageIDs },
                        type: 'POST',
                        traditional: true,
                        failureMessageFormat: 'An error occurred trying to set the images display order: [[errorThrown]]'
                    });
                }
            };
        })();
    })();
})(jQuery);