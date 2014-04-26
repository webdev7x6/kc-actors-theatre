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
                        'Are you sure you want to delete this image?',
                        'Remove Image',
                        function () {
                            ajaxHelper.ajax('/CustomAdmin/Show/DeleteImage', {
                                type: 'POST',
                                data: {
                                    id: image.showImageID
                                },
                                success: function (data, textStatus, jqXHR) {
                                    window.associatedImagesViewModel.images.remove(image);
                                },
                                failureMessageFormat: 'An error occurred trying to delete the image: [[errorThrown]]'
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

                            //var $list = $('.editable-parent[data-item-id="' + window.SHOW_ID + '"] .edit-associated-images ul');
                            //$list.html('');

                            //$associatedImages.find('tr').each(function _images_onEach(ix, el) {
                            //    var $el = $(el);
                            //    if ($el.css('display') != 'none') // only add if not hidden
                            //    {
                            //        var id = $el.attr('data-image-id'),
                            //            name = $el.find('td [data-property-name="Name"]').text(),
                            //            title = $el.find('td [data-property-name="Title"]').text(),
                            //            email = $el.find('td [data-property-name="Email"]').text(),
                            //            phone = $el.find('td [data-property-name="Phone"]').text()
                            //        ;

                            //        $list.append('<li data-contact-phone="' + phone + '" data-contact-email="' + email + '" data-contact-title="' + title + '" data-contact-id="' + id + '">' + name + '</li>');

                            //    }
                            //});
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
                        images = [],
                        $images = $container.find('ul')
                    ;

                    $images.find('li').each(function _images_onEach(ix, el) {
                        var $el = $(el);
                        images.push({
                            contactID: $el.attr('data-contact-id'),
                            name: $el.text(),
                            title: $el.attr('data-contact-title'),
                            email: $el.attr('data-contact-email'),
                            phone: $el.attr('data-contact-phone')
                        });
                    });

                    admin.shows.images.show(images, parent.attr('data-item-id'));
                },

                // displays contact edit modal
                show: function _show(images, congregationID, onClose) {
                    associatedContactsViewModel.images([]);
                    $associatedImages.empty();

                    // push each item to table
                    $.each(images, function _images_onEach(ix, item) {
                        associatedContactsViewModel.images.push(item);
                    });

                    // make each row editable
                    $('.images-table tbody tr').each(function (index, item) {
                        admin.shows.images.initEditInPlace(item);
                    });

                    // make rows sortable
                    $('.images-table tbody').sortable({
                        handle: '.sort-order',
                        update: function (event, ui) {
                            var contactIDs = $(this).sortable('toArray');
                            admin.shows.images.updateContactDisplayOrder(contactIDs);
                        }
                    });

                    admin.shows.images.initButtons();
                    window.SHOW_ID = congregationID;
                    closeCallback = onClose;
                    $dlg.dialog('open');
                },
                updateContactDisplayOrder: function (contactIDs) {
                    ajaxHelper.ajax('/CustomAdmin/Congregations/UpdateContactDisplayOrder', {
                        data: { contactIDs: contactIDs },
                        type: 'POST',
                        traditional: true,
                        failureMessageFormat: 'An error occurred trying to set the images display order: [[errorThrown]]'
                    });
                }

            };
        })();
    })();
})(jQuery);