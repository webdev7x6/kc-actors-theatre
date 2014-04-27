/// <reference path="http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.8.3-vsdoc.js" />
/// <reference path="http://clickfarmcdn.localhost/common/js/knockout/2.1.0/knockout.js" />
/// <reference path="http://clickfarmcdn.localhost/common/js/kendo/2012.2.710/kendo.all-vsdoc.js" />

(function ($, undefined) {
    var admin = window.admin = window.admin || {};
    admin.shows = admin.shows = admin.shows || {};
    (function () {
        admin.shows.videos = (function () {
            var $dlg = null,
                closeCallback = null,
                $associatedVideos = null
            ;

            window.associatedVideosViewModel = {
                videos: ko.observableArray([]),
                removeVideo: function (video, event) {
                    event.preventDefault();
                    dialogHelper.confirm(
                        'Are you sure you want to remove this video?',
                        'Remove Video',
                        function () {
                            ajaxHelper.ajax(admin.RemoveVideoURL, {
                                type: 'POST',
                                data: {
                                    id: video.videoID
                                },
                                success: function (data, textStatus, jqXHR) {
                                    window.associatedVideosViewModel.videos.remove(video);
                                },
                                failureMessageFormat: 'An error occurred trying to remove the video: [[errorThrown]]'
                            });

                            
                        }
                    );            
                }
            };

            window.SHOW_ID = null;

            return {
                init: function _init($modal) {
                    $associatedVideos = $('#videos-tbody');
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

                            var $list = $('.editable-parent[data-item-id="' + window.SHOW_ID + '"] .edit-associated-videos ul');
                            $list.html('');

                            $associatedVideos.find('tr').each(function _videos_onEach(ix, el) {
                                var $el = $(el);
                                if ($el.css('display') != 'none') // only add if not hidden
                                {
                                    var id = $el.attr('data-video-id'),
                                        vimeoID = $el.find('td [data-property-name="VimeoID"]').text()
                                    ;

                                    $list.append('<li data-video-id="' + id + '">' + vimeoID + '</li>');

                                }
                            });
                        }
                    });
                    ko.applyBindings(window.associatedVideosViewModel, $associatedVideos[0]);
                },
                initButtons: function _initButtons() {
                    $('.delete-video-link').each(function (index, link) {
                        var lnk = $(link);
                        lnk.button({
                            icons: {
                                primary: 'ui-icon-trash'
                            },
                            text: false
                        });
                    });
                },

                initEditInPlace: function (videoTableRow) {
                    var videoEditMgr = new EditableManager($(videoTableRow), { videoID: $(videoTableRow).attr('data-video-id') }, '/CustomAdmin/Show/EditVideoInPlace');
                    videoEditMgr.initAllTypes();
                },

                videoClick: function (container, parent) {
                    var
                        $container = $(container),
                        videos = [],
                        $videos = $container.find('ul')
                    ;

                    $videos.find('li').each(function _videos_onEach(ix, el) {
                        var $el = $(el);
                        videos.push({
                            videoID: $el.attr('data-video-id'),
                            vimeoID: $el.text(),
                        });
                    });

                    admin.shows.videos.show(videos, parent.attr('data-item-id'));
                },

                // displays video edit modal
                show: function _show(videos, showID, onClose) {
                    associatedVideosViewModel.videos([]);
                    $associatedVideos.empty();

                    // push each item to table
                    $.each(videos, function _videos_onEach(ix, item) {
                        associatedVideosViewModel.videos.push(item);
                    });

                    // make each row editable
                    $('.videos-table tbody tr').each(function (index, item) {
                        admin.shows.videos.initEditInPlace(item);
                    });

                    // make rows sortable
                    $('.videos-table tbody').sortable({
                        handle: '.sort-order',
                        update: function (event, ui) {
                            var videoIDs = $(this).sortable('toArray');
                            admin.shows.videos.updateVideoDisplayOrder(videoIDs);
                        }
                    });

                    admin.shows.videos.initButtons();
                    window.SHOW_ID = showID;
                    closeCallback = onClose;
                    $dlg.dialog('open');
                },
                updateVideoDisplayOrder: function (videoIDs) {
                    ajaxHelper.ajax(admin.UpdateVideoDisplayOrderURL, {
                        data: { videoIDs: videoIDs },
                        type: 'POST',
                        traditional: true,
                        failureMessageFormat: 'An error occurred trying to set the videos display order: [[errorThrown]]'
                    });
                }
            };
        })();
    })();
})(jQuery);