/// <reference path="http://clickfarmcdn.localhost/common/js/jquery/1.7.2/jquery-vsdoc.js" />
/// <reference path="http://clickfarmcdn.localhost/common/js/kendo/2012.2.710/kendo.all-vsdoc.js" />

var fileManager = (function () {
    var _context = null,
        _dlg_settings = null,
        _dlg_dialog$ = null,
        _dlg_enableSelection = false,
        _dlg_canSelect = false,
        _dlg_selectButton$ = null,
        _init_settings = null,
        _files_list$ = null,
        _files_tree$k = null,
        _files_onSelectCt = 0,
        _files_loadingNode = {
            text: 'Working...',
            imageUrl: '//' + cms.globals.cdnHostName + '/common/images/ajax-loader.gif',
            enabled: false
        },
        _files_noneNode = {
            text: '(none)',
            enabled: false
        },
        _act_refreshLink$ = null,
        _act_createFolderLink$ = null,
        _act_uploadLink$ = null,
        _act_renameLink$ = null,
        _act_deleteLink$ = null,
        _wait_browseCol$ = null,
        _wait_previewCol$ = null,
        _name_form$ = null,
        _name_input$ = null,
        _name_uneditable$ = null,
        _name_loading$ = null,
        _name_error$ = null,
        _upl_pane$ = null,
        _upl_overwriteEl = null,
        _upl_files$k = null,
        _upl_bgColor = '#EBEBEB',
        _upl_okColor = '#20B34F',
        _upl_errColor = '#B32320',
        _upl_clearFileDelay = 5000,
        _tabContainer$ = null,
        _imagePreview$ = null,
        _optsTab$ = null,
        _optsTabPanel$ = null,

        _fsoVM = kendo.observable({
            name: '',
            uri: '',
            dateModified: null,
            size: null,
            type: null,
            mediaType: null,
            width: null,
            height: null,
            imageScale: null,

            isFile: function () {
                var mType = this.get('mediaType');
                return mType && mType !== 'x-folder/folder';
            },

            isImage: function () {
                var mType = this.get('mediaType');
                return mType && mType.toLowerCase().indexOf('image/') === 0;
            },

            imageUri: function () {
                if (!this.isImage()) {
                    return null;
                }
                return this.get('uri');
            },

            dateModifiedPretty: function () {
                var date = this.get('dateModified');
                if (!date) {
                    return null;
                }
                return kendo.toString(date, 'g');
            },

            sizePretty: function () {
                var size = this.get('size'),
                    un
                ;
                if (!size && size !== 0) {
                    return null;
                }
                un = unitify(size, BINARY_UNITS, 3);
                return un.size.toString() + ' ' + un.unit + 'b';
            },

            iconClass: function () {
                return 'file-icon-48 ' + this.get('type');
            }
        }),

        _dlg_show = function (settings) {
            var dlg_settings = $.extend(
                {},
                {
                    rootFolder: '',
                    defaultSubfolder: '',
                    enableSelection: false,
                    dialogTitle: 'File Manager',
                    mediaTypes: [],
                    fileExtensions: [],
                    preselectedFsoUri: '',
                    selectCallback: null,
                    closeCallback: null
                },
                settings
            );
            if (_dlg_settings) {
                //check for changed settings that would force us to re-init the dialog
                if (
                    _dlg_settings.rootFolder !== dlg_settings.rootFolder
                    || _dlg_settings.enableSelection !== dlg_settings.enableSelection
                ) {
                    _dlg_dialog$.dialog('destroy');
                    _dlg_dialog$ = null;
                }
            }
            _dlg_settings = dlg_settings;
            if (!_dlg_dialog$) {
                ajaxHelper.ajax('/Admin/FileManager/ShowManager', {
                    data: {
                        rootFolder: _dlg_settings.rootFolder
                    },
                    type: 'GET',
                    success: function (data, textStatus, jqXHR) {
                        var btns = {},
                            btnCloseLabel = 'Close'
                        ;
                        _dlg_enableSelection = _dlg_settings.enableSelection === true;
                        if (_dlg_enableSelection) {
                            btns.Select = _dlg_onSelect;
                            btnCloseLabel = 'Cancel';
                        }
                        btns[btnCloseLabel] = function () {
                            _dlg_dialog$.dialog('close');
                        };
                        _dlg_dialog$ = $('<div/>').dialog({
                            autoOpen: false,
                            buttons: btns,
                            height: 620,
                            maxHeight: 620,
                            minHeight: 620,
                            minWidth: 930,
                            modal: true,
                            title: _dlg_settings.dialogTitle,
                            width: 1025
                        });
                        _context = _dlg_dialog$[0];
                        if (_dlg_enableSelection) {
                            _dlg_selectButton$ = _dlg_dialog$.siblings('.ui-dialog-buttonpane').find('button').filter(function () {
                                return $('span', this).text() === 'Select';
                            });
                            _dlg_disableSelect();
                        }
                        _dlg_dialog$
                            .html(data)
                            .one('dialogclose', _dlg_settings.closeCallback)
                            .dialog('open')
                        ;
                    },
                    failureMessageFormat: "An error occurred trying to show the file manager: [[errorThrown]]",
                    errorMessageFormat: "An error occurred trying to show the file manager: [[errorThrown]]"
                });
            }
            else {
                _dlg_dialog$
                    .dialog('option', 'title', _dlg_settings.dialogTitle)
                    .one('dialogclose', _dlg_settings.closeCallback)
                    .dialog('open')
                ;

                //close upload pane
                _upl_togglePaneVisibility(null, false, false);

                //collapse all open folders
                var nodes$ = _files_list$.children('ul').children('li').first(),
                    n$ = nodes$.first()
                ;
                if (nodes$.length > 0) {
                    _files_doSelectNode(n$);
                    nodes$ = nodes$.find('li.k-item').filter('[data-is-folder-node="true"]').filter(function () {
                        return $(this).children('div').children('span.k-icon.k-minus').length === 1
                    });
                    if (nodes$.length > 0) {
                        _files_tree$k.collapse(nodes$.reverse());
                    }
                }
            }
        },

        _dlg_onSelect = function () {
            if (_dlg_canSelect) {
                if ($.isFunction(_dlg_settings.selectCallback)) {
                    _dlg_settings.selectCallback(_files_tree$k.select().data('fsoProperties'));
                }
                _dlg_dialog$.dialog('close');
            }
        },

        _dlg_disableSelect = function () {
            if (_dlg_enableSelection) {
                _dlg_canSelect = false;
                _dlg_selectButton$.button('option', 'disabled', true);
            }
        },

        _dlg_enableSelect = function (props) {
            if (_dlg_enableSelection) {
                //TODO
                //if props.MediaType matches one of _dlg_settings.mediaTypes
                _dlg_canSelect = true;
                _dlg_selectButton$.button('option', 'disabled', false);
            }
        },

        _init = function (settings) {
            if (!_context) {
                _context = document;
            }
            _init_settings = $.extend(
                {},
                {
                    rootFolder: {
                        Name: '',
                        Uri: ''
                    }
                },
                settings
            );

            _files_list$ = $('#file-list', _context);
            _act_refreshLink$ = $('#refresh-link', _context);
            _act_createFolderLink$ = $('#create-folder-link', _context);
            _act_uploadLink$ = $('#upload-link', _context);
            _act_renameLink$ = $('#rename-link', _context);
            _act_deleteLink$ = $('#delete-link', _context);
            _wait_browseCol$ = $('#browse-col-wait', _context);
            _wait_previewCol$ = $('#preview-col-wait', _context);
            _upl_pane$ = $('#upload-pane', _context);
            _upl_overwriteEl = $('#upload-overwrite', _context)[0];
            _name_form$ = $('#get-name-form').dialog({
                autoOpen: false,
                modal: true,
                resizable: false,
                width: 400
            });
            _name_input$ = $('#get-name-input', _name_form$);
            _name_uneditable$ = $('#get-name-uneditable', _name_form$);
            _name_loading$ = $('#get-name-loading', _name_form$);
            _name_error$ = $('#get-name-error', _name_form$);
            _tabContainer$ = $('#preview-col-tabs', _context).tabs();
            _imagePreview$ = $('#image-preview', _context).on('load', function () {
                _fsoVM.set('imageScale', (Math.floor((_imagePreview$.width() / _fsoVM.get('width')) * 1000) / 10).toString() + '%');
            });
            _optsTab$ = $('#options-tab-link', _context).hide();
            _optsTabPanel$ = $('#options-tab', _context).hide();
            kendo.bind($('#props-tab', _context), _fsoVM);

            var hasRoot = _init_settings.rootFolder && _init_settings.rootFolder.Name && _init_settings.rootFolder.Uri,
                upl_closeLink$ = $('#upload-close-link', _context),
                upl_selectBtn$,
                previewColContainer$ = $('#preview-col-container', _context),
                wait_previewColOverlay$ = _wait_previewCol$.children('.file-man-wait-overlay'),
                wait_previewColOverlayLeft = parseInt(wait_previewColOverlay$.css('left')),
                wait_previewColDialog$ = _wait_previewCol$.children('.file-man-wait-dialog'),
                wait_previewColDialogWidth = wait_previewColDialog$.outerWidth(),
                wait_previewColOnResize = function () {
                    var w = previewColContainer$.width();
                    wait_previewColOverlay$.width(w);
                    wait_previewColDialog$.css('left', ((w - wait_previewColDialogWidth) / 2 + wait_previewColOverlayLeft).toString() + 'px');
                }
            ;

            if (_dlg_dialog$) {
                _dlg_dialog$
                    .on('dialogresize', wait_previewColOnResize)
                    .on('dialogopen', function () {
                        _dlg_dialog$.trigger('dialogresize');
                        setTimeout(function () {
                            _dlg_dialog$.prev().find('a').filter(':first').focus();
                        }, 0);
                    })
                ;
            }
            else {
                $(window)
                    .on('resize', wait_previewColOnResize)
                    .resize()
                ;
            }

            cms.makeCloseButton(upl_closeLink$);
            kendo.bind(upl_closeLink$, { closeUploadPane: _upl_togglePaneVisibility });

            _files_tree$k = _files_list$
                .kendoTreeView({
                    animation: false,
                    dragAndDrop: true
                })
                .data('kendoTreeView')
                .bind('expand', function (e) {
                    _files_onExpandFolder(e.node);
                })
                .bind('select', function (e) {
                    _files_onSelectNode(e.node);
                })
                .bind('dragstart', function (e) {
                    if (!_files_onDragStart(e.sourceNode)) {
                        e.preventDefault();
                    }
                })
                .bind('drag', function (e) {
                    e.setStatusClass(_files_onDrag(e.sourceNode, e.dropTarget));
                })
                .bind('drop', function (e) {
                    if (e.valid) {
                        e.preventDefault();
                        _files_onDrop(e.sourceNode, e.destinationNode);
                    }
                })
            ;
            if (hasRoot) {
                _files_appendRootFolder$(_init_settings.rootFolder);
            }
            else {
                _files_appendNoneNode$();
            }
            if (_dlg_enableSelection) {
                _files_list$.on('dblclick', 'li.k-item', function (e) {
                    var n$ = $(this),
                        select
                    ;
                    if (
                        n$.attr('data-is-file-node') === 'true'
                        && n$.attr('data-uri') === _files_tree$k.select().attr('data-uri')
                    ) {
                        select = function () {
                            if (n$.data('fsoProperties')) {
                                _dlg_onSelect();
                            }
                            else {
                                setTimeout(select, 50);
                            }
                        };
                        setTimeout(select, 50);
                    }
                });
            }

            _upl_files$k = $('#upload-files', _context)
                .kendoUpload({
                    async: {
                        saveUrl: '/Admin/FileManager/UploadFiles'
                    },
                    localization: {
                        select: 'Browse...'
                    }
                })
                .data('kendoUpload')
                .bind('select', function (e) {
                    if (!_upl_onSelect(e.files)) {
                        e.preventDefault();
                    }
                })
                .bind('upload', function (e) {
                    var data = _upl_onUpload();
                    if (!data) {
                        e.preventDefault();
                    }
                    else {
                        e.data = data;
                    }
                })
                .bind('success', function (e) {
                    if (e.operation === 'upload') {
                        ajaxHelper.success(
                            function () {
                                _upl_onSuccess(e.response);
                            },
                            function () {
                                _upl_onError(e.files, e.response.Message);
                            },
                            false
                        )(e.response);
                    }
                    else {
                        dialogHelper.alert('The operation is unsupported (' + e.operation + ').', 'Unsupported Operation');
                    }
                })
                .bind('error', function (e) {
                    e.preventDefault();
                    if (e.operation === 'upload') {
                        _upl_onError(e.files, e.XMLHttpRequest.responseText);
                    }
                    else {
                        dialogHelper.alert('The operation is unsupported (' + e.operation + ').', 'Unsupported Operation');
                    }
                })
            ;
            //make the upload "Select" button look like a jQuery UI button
            upl_selectBtn$ = $('div.k-widget.k-upload div.k-button.k-upload-button', _upl_pane$)
                .removeClass('k-button')
                .addClass('ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only')
                .find('span')
                .addClass('ui-button-text')
                .end()
                .hover(
                    function () { upl_selectBtn$.removeClass('ui-state-default').addClass('ui-state-hover') },
                    function () { upl_selectBtn$.removeClass('ui-state-hover').addClass('ui-state-default') }
                )
            ;

            _act_refreshLink$
                .button({
                    icons: {
                        primary: 'ui-icon-refresh'
                    },
                    text: false,
                    disabled: !hasRoot
                })
                .on('click', function (e) {
                    e.preventDefault();
                    var nodes$ = _files_list$.children('ul').children('li');
                    if (nodes$.length > 0) {
                        _files_tree$k.remove(nodes$);
                    }
                    if (hasRoot) {
                        _files_appendRootFolder$(_init_settings.rootFolder);
                    }
                })
            ;

            _act_createFolderLink$
                .button({
                    icons: {
                        primary: 'ui-icon-plus',
                        secondary: 'ui-icon-folder-open'
                    },
                    text: false,
                    disabled: !hasRoot
                })
                .on('click', function (e) {
                    e.preventDefault();
                    _act_createFolder();
                })
            ;

            _act_uploadLink$
                .button({
                    icons: {
                        primary: 'ui-icon-plus',
                        secondary: 'ui-icon-document'
                    },
                    text: false,
                    disabled: !hasRoot
                })
                .on('click', _upl_togglePaneVisibility)
            ;

            _act_renameLink$
                .button({
                    icons: {
                        primary: 'ui-icon-pencil'
                    },
                    text: false,
                    disabled: true
                })
                .on('click', function (e) {
                    e.preventDefault();
                    _act_renameFso();
                })
            ;

            _act_deleteLink$
                .button({
                    icons: {
                        primary: 'ui-icon-trash'
                    },
                    text: false,
                    disabled: true
                })
                .on('click', function (e) {
                    e.preventDefault();
                    _act_deleteFso();
                })
            ;
        },

        _files_appendRootFolder$ = function (rootFolder) {
            var f$ = _files_appendFolder$(rootFolder, null, true).attr('data-is-root-node', 'true');
            if (f$) {
                _files_doSelectNode(f$);
            }
            return f$;
        },

        _files_appendFolder$ = function (folder, parent, expand, skipLoad) {
            var f$ = _files_appendFso$(folder, 'folder', parent),
                load$
            ;
            if (f$) {
                f$.attr('data-is-folder-node', 'true');
                if (!skipLoad) {
                    load$ = _files_tree$k
                        .append(_files_loadingNode, f$)
                        .attr('data-is-load-node', 'true')
                    ;
                    if (!expand) {
                        setTimeout(function () {
                            _files_tree$k.collapse(f$);
                        }, 0);
                    }
                    else {
                        _files_onExpandFolder(f$, load$);
                    }
                }
            }
            return f$;
        },

        _files_appendFile$ = function (file, parent) {
            var f$ = _files_appendFso$(file, getFileExtension(file.Name), parent);
            if (f$) {
                f$.attr('data-is-file-node', 'true');
            }
            return f$;
        },

        _files_appendFso$ = function (fso, type, parent) {
            if (!fso.Name || !fso.Uri) {
                return;
            }
            return _files_tree$k
                .append({
                    text: fso.Name,
                    spriteCssClass: 'file-icon-16 ' + type
                }, parent)
                .attr('data-uri', fso.Uri)
            ;
        },

        _files_appendNoneNode$ = function (parent) {
            return _files_tree$k
                .append(_files_noneNode, parent)
                .attr('data-is-none-node', 'true')
            ;
        },

        _files_onExpandFolder = function (node, load$) {
            var n$ = node instanceof $ ? node : $(node),
                uri = n$.attr('data-uri'),
                children$,
                hasChildren = false
            ;
            if (!uri) {
                return;
            }
            children$ = n$.children('ul').children('li');
            if (!load$) {
                load$ = children$.filter('[data-is-load-node="true"]');
            }
            if (load$.length === 0) {
                children$.filter('[data-is-folder-node="true"]').each(function () {
                    var me$ = $(this);
                    load$ = me$.children('ul').children('li[data-is-load-node="true"]');
                    if (load$.length !== 0) {
                        setTimeout(function () {
                            _files_tree$k.collapse(me$);
                        }, 0);
                    }
                });
                return;
            }

            ajaxHelper.ajax('/Admin/FileManager/GetFolders', {
                data: {
                    parentFolderUri: uri
                },
                type: 'GET',
                success: function (data, textStatus, jqXHR) {
                    if (data.Children.length > 0) {
                        hasChildren = true;
                        for (var i = 0; i < data.Children.length; i++) {
                            _files_appendFolder$(data.Children[i], n$);
                        }
                    }
                    ajaxHelper.ajax('/Admin/FileManager/GetFiles', {
                        data: {
                            parentFolderUri: uri
                        },
                        type: 'GET',
                        success: function (data, textStatus, jqXHR) {
                            if (data.Children.length > 0) {
                                hasChildren = true;
                                for (var j = 0; j < data.Children.length; j++) {
                                    _files_appendFile$(data.Children[j], n$);
                                }
                            }
                            if (!hasChildren) {
                                _files_appendNoneNode$(n$);
                            }
                            _files_tree$k.remove(load$);
                        },
                        failureMessageFormat: "An error occurred trying to get the folder's child files: [[errorThrown]]"
                    });
                },
                failureMessageFormat: "An error occurred trying to get the folder's child folders: [[errorThrown]]"
            });
        },

        _files_isFolderExpanded = function (node) {
            var n$ = node instanceof $ ? node : $(node);
            if (n$.attr('data-is-folder-node') !== 'true') {
                return null;
            }
            return n$.children('div').children('span.k-icon.k-minus').length === 1;
        },

        _files_isFolderLoaded = function (node) {
            var n$ = node instanceof $ ? node : $(node);
            if (n$.attr('data-is-folder-node') !== 'true') {
                return null;
            }
            return n$.children('ul').children('li[data-is-load-node="true"]').length === 0;
        },

        _files_doSelectNode = function (node) {
            var n$ = node instanceof $ ? node : $(node);
            _files_tree$k.select(n$)
            _files_onSelectNode(n$);
        },

        _files_onSelectNode = function (node) {
            var n$ = node instanceof $ ? node : $(node),
                isRoot = n$.attr('data-is-root-node') === 'true',
                uri = n$.attr('data-uri'),
                action,
                data,
                func,
                props
            ;
            _act_renameLink$.button();
            _act_deleteLink$.button();
            _act_renameLink$.button('option', 'disabled', isRoot);
            _act_deleteLink$.button('option', 'disabled', isRoot);
            _dlg_disableSelect();
            if (!uri) {
                return;
            }
            if (n$.attr('data-is-folder-node') === 'true') {
                //TODO
                //if (_dlg_enableSelection) {
                //    _tabContainer$.tabs('select', 0);
                //    _optsTab$.hide();
                //    _optsTabPanel$.hide();
                //}
                action = 'Folder';
                data = {
                    folderUri: uri
                };
                func = _files_setFolderProperties;
            }
            else if (n$.attr('data-is-file-node') === 'true') {
                //TODO
                //if (_dlg_enableSelection) {
                //    _optsTab$.show();
                //    _optsTabPanel$.show();
                //}
                action = 'File';
                data = {
                    fileUri: uri
                };
                func = _files_setFileProperties;
            }
            else {
                dialogHelper.alert('Invalid node type', 'Invalid');
                return;
            }
            props = n$.data('fsoProperties');
            if (props) {
                func(props);
                return;
            }
            ajaxHelper.ajax('/Admin/FileManager/Get' + action + 'Properties', {
                data: data,
                type: 'GET',
                beforeSend: function (jqXHR, settings) {
                    if (++_files_onSelectCt === 1) {
                        _wait_previewCol$.show();
                    }
                },
                success: function (data, textStatus, jqXHR) {
                    n$.data('fsoProperties', data.FsoProperties);
                    if (_files_tree$k.select().attr('data-uri') === uri) {
                        func(data.FsoProperties);
                    }
                },
                failureMessageFormat: "An error occurred trying to get the " + action.toLowerCase() + "'s properties: [[errorThrown]]",
                complete: function (jqXHR, textStatus) {
                    if (--_files_onSelectCt === 0) {
                        _wait_previewCol$.hide();
                    }
                }
            });
        },

        _files_setFolderProperties = function (props) {
            _fsoVM.set('name', props.Name);
            _fsoVM.set('uri', props.Uri);
            _fsoVM.set('dateModified', parseMvcDate(props.DateModified));
            _fsoVM.set('mediaType', props.MediaType);
            _fsoVM.set('type', 'folder');
            _fsoVM.set('size', null);
            _fsoVM.set('width', null);
            _fsoVM.set('height', null);
        },

        _files_setFileProperties = function (props) {
            _dlg_enableSelect(props);
            _fsoVM.set('name', props.Name);
            _fsoVM.set('uri', props.Uri);
            _fsoVM.set('dateModified', parseMvcDate(props.DateModified));
            _fsoVM.set('mediaType', props.MediaType);
            _fsoVM.set('type', getFileExtension(props.Name));
            _fsoVM.set('size', props.Size);
            if (_fsoVM.isImage()) {
                _fsoVM.set('width', props.Width);
                _fsoVM.set('height', props.Height);
                _fsoVM.set('imageScale', '[loading...]');
            }
            else {
                _fsoVM.set('width', null);
                _fsoVM.set('height', null);
            }
        },

        _files_onDragStart = function (node) {
            var n$ = node instanceof $ ? node : $(node);
            return !(n$.attr('data-is-root-node') === 'true'
                || n$.attr('data-is-load-node') === 'true'
                || n$.attr('data-is-none-node') === 'true'
            );
        },

        _files_onDrag = function (sourceNode, dropTarget) {
            var cls,
                valid = false,
                tar$,
                dest$,
                src$
            ;
            if (dropTarget.tagName === 'SPAN') {
                tar$ = $(dropTarget);
                if (!tar$.hasClass('k-in') || !tar$.hasClass('k-state-hover')) {
                    tar$ = tar$.parent('span.k-in.k-state-hover');
                }
                if (tar$.hasClass('k-in') && tar$.hasClass('k-state-hover')) {
                    dest$ = tar$.closest('li.k-item');
                    if (
                        dest$.length === 1
                        && dest$.attr('data-is-folder-node') === 'true'
                    ) {
                        src$ = $(sourceNode);
                        if (
                            dest$.closest(src$).length === 0
                            && src$.parent('ul').parent('li[data-uri="' + dest$.attr('data-uri') + '"]').length === 0
                        ) {
                            valid = true;
                        }
                    }
                }
            }
            if (!valid) {
                cls = 'k-denied';
            }
            else {
                cls = 'k-add ';
                if (true) { //TODO not holding control key
                    cls += 'move';
                }
                else {
                    cls += 'copy';
                }
            }
            return cls;
        },

        _files_onDrop = function (sourceNode, destinationNode) {
            var src$ = $(sourceNode),
                dest$ = $(destinationNode),
                uri = dest$.attr('data-uri'),
                isMove = true, //TODO move vs copy
                action = isMove ? 'Move' : 'Copy',
                data,
                func
            ;
            if (!uri) {
                return;
            }
            if (src$.attr('data-is-folder-node') === 'true') {
                action += 'Folder';
                data = {
                    folderUri: src$.attr('data-uri')
                };
                func = _files_appendFolder$;
            }
            else if (src$.attr('data-is-file-node') === 'true') {
                action += 'File';
                data = {
                    fileUri: src$.attr('data-uri')
                };
                func = _files_appendFile$;
            }
            else {
                dialogHelper.alert('Invalid node type', 'Invalid');
                return;
            }
            data.newParentFolderUri = uri;
            ajaxHelper.ajax('/Admin/FileManager/' + action, {
                data: data,
                type: 'POST',
                beforeSend: function (jqXHR, settings) {
                    _wait_browseCol$.show();
                },
                success: function (data, textStatus, jqXHR) {
                    var selNode$;
                    if (_files_isFolderLoaded(dest$)) {
                        selNode$ = func(data.Fso, dest$);
                    }
                    if (!_files_isFolderExpanded(dest$)) {
                        if (isMove) {
                            selNode$ = src$.parent('ul').parent('li');
                        }
                        else {
                            selNode$ = src$;
                        }
                    }
                    _files_doSelectNode(selNode$);
                    if (isMove) {
                        _files_tree$k.remove(src$);
                    }
                },
                complete: function (jqXHR, textStatus) {
                    _wait_browseCol$.hide();
                }
            });
        },

        _upl_onSelect = function (files) {
            var i,
                tn,
                tree$ = null,
                timeout
            ;
            for (i = 0; i < files.length; i++) {
                tn = _upl_getFileTreeAndNode(files[i].name, tree$);
                if (tn.tree$.length === 0) {
                    return true;
                }
                if (!tree$) {
                    tree$ = tn.tree$;
                }
                if (tn.node$ && tn.node$.length === 1) {
                    timeout = tn.node$.data('disappearTimeout');
                    if (timeout) {
                        clearTimeout(timeout);
                        tn.node$.stop().remove();
                    }
                    else {
                        return false;
                    }
                }
            }
            return true;
        },

        _upl_onUpload = function () {
            var n$ = _files_tree$k.select(),
                uri
            ;
            if (n$.attr('data-is-file-node') === 'true') {
                n$ = n$.parent('ul').parent('li[data-is-folder-node="true"]');
            }
            uri = n$.attr('data-uri');
            if (!uri) {
                return false;
            }
            return {
                parentFolderUri: uri,
                overwrite: _upl_overwriteEl.checked.toString()
            };
        },

        _upl_onSuccess = function (rsp) {
            var i,
                tn,
                tree$ = null,
                parent$ = $('li[data-uri="' + rsp.ParentFolderUri + '"]', _files_list$),
                parentItems$,
                isLoaded,
                f$
            ;
            if (parent$.length === 1) {
                parentItems$ = parent$.children('ul').children('li');
                isLoaded = parentItems$.filter('[data-is-load-node="true"]').length === 0;
                if (isLoaded) {
                    _files_tree$k.remove(parentItems$.filter('[data-is-none-node="true"]'));
                }

                for (i = 0; i < rsp.Children.length; i++) {
                    tn = _upl_getFileTreeAndNode(rsp.Children[i].Name, tree$);
                    if (!tree$) {
                        tree$ = tn.tree$;
                    }

                    if (isLoaded) {
                        _files_tree$k.remove(parentItems$.filter('[data-uri="' + rsp.Children[i].Uri + '"]'));
                        f$ = _files_appendFile$(rsp.Children[i], parent$);
                    }
                    _upl_finalizeDisappearFileNode(tn.node$, tree$, _upl_okColor);
                }
                if (isLoaded) {
                    _files_doSelectNode(f$);
                }
            }
        },

        _upl_onError = function (files, errMsg) {
            var i,
                tn,
                tree$ = null,
                node$ = null,
                btn$ = null
            ;
            for (i = 0; i < files.length; i++) {
                tn = _upl_getFileTreeAndNode(files[i].name, tree$);
                if (!tree$) {
                    tree$ = tn.tree$;
                }
                node$ = tn.node$;

                //this function handles both the upload widget error event and a CMS failure response (which the upload
                //widget interprets as success)

                //if the widget reports success, change it to failure (CMS failure)
                setTimeout(function () {
                    node$.children('span.k-filename').children('span.k-progress').children('span.k-progress-status').addClass('error');
                }, 0);
                node$.children('span.k-icon.k-success')
                    .removeClass('k-success')
                    .addClass('k-fail')
                    .text('failed')
                ;

                //the upload widget adds a retry button when there's an error, so remove it (true error)
                btn$ = node$.children('button.k-upload-action');
                if (btn$.children('span.k-icon.k-retry').length !== 0) {
                    btn$.remove();
                }

                node$.append('<div class="error">' + errMsg + '</div>');
                _upl_finalizeDisappearFileNode(node$, tree$, _upl_errColor);
            }
        },

        _upl_getFileTreeAndNode = function (filename, existTree$) {
            var tree$ = existTree$ || $('ul.k-upload-files', _upl_pane$),
                node$ = null
            ;
            tree$.children('li.k-file').each(function () {
                var me$ = $(this);
                if (me$.children('span.k-filename[title="' + filename + '"]').length === 1) {
                    node$ = me$;
                    return false;
                }
            });
            return {
                tree$: tree$,
                node$: node$
            };
        },

        _upl_finalizeDisappearFileNode = function (node$, tree$, highlightColor) {
            var leave = function () {
                if (_upl_pane$.is(':visible')) {
                    node$.slideUp('fast', 'linear', function () {
                        node$.remove();
                        if (tree$.children('li.k-file').length === 0) {
                            tree$.remove();
                        }
                    });
                }
                else {
                    node$.data('disappearTimeout', setTimeout(leave, _upl_clearFileDelay));
                }
            };
            node$
                .animate({ backgroundColor: highlightColor }, 'fast', 'linear', function () {
                    node$.animate({ backgroundColor: _upl_bgColor }, 'fast');
                })
                .data('disappearTimeout', setTimeout(leave, _upl_clearFileDelay))
            ;
        },

        _upl_togglePaneVisibility = function (e, show, anim) {
            var paneVisFunc,
                paneVisDur,
                lnkClassFunc,
                isRoot,
                refLnkDis,
                folLnkDis,
                renLnkDis,
                delLnkDis
            ;
            if (e) {
                e.preventDefault();
            }
            if (show === true) {
                if (_upl_pane$.is(':visible')) {
                    return;
                }
                if (anim !== false) {
                    paneVisFunc = $.fn.slideDown;
                    paneVisDur = 'fast';
                }
                else {
                    paneVisFunc = $.fn.show;
                }
                lnkClassFunc = $.fn.addClass;
                refLnkDis = true;
                folLnkDis = true;
                renLnkDis = true;
                delLnkDis = true;
            }
            else if (show === false) {
                if (_upl_pane$.is(':hidden')) {
                    return;
                }
                if (anim !== false) {
                    paneVisFunc = $.fn.slideUp;
                    paneVisDur = 'fast';
                }
                else {
                    paneVisFunc = $.fn.hide;
                }
                lnkClassFunc = $.fn.removeClass;
                isRoot = _files_tree$k.select().attr('data-is-root-node') === 'true',
                refLnkDis = false;
                folLnkDis = false;
                renLnkDis = isRoot;
                delLnkDis = isRoot;
            }
            else {
                _upl_togglePaneVisibility(null, _upl_pane$.is(':hidden'), anim);
                return;
            }
            paneVisFunc.call(_upl_pane$.stop(), paneVisDur);
            lnkClassFunc.call(_act_uploadLink$, 'ui-state-highlight');
            _act_refreshLink$.button('option', 'disabled', refLnkDis);
            _act_createFolderLink$.button('option', 'disabled', folLnkDis);
            _act_renameLink$.button('option', 'disabled', renLnkDis);
            _act_deleteLink$.button('option', 'disabled', delLnkDis);
        },

        _act_createFolder = function () {
            var n$ = _files_tree$k.select(),
                uri
            ;
            if (n$.attr('data-is-file-node') === 'true') {
                n$ = n$.parent('ul').parent('li[data-is-folder-node="true"]');
            }
            uri = n$.attr('data-uri');
            if (!uri) {
                return;
            }
            _name_startupHelper(
                {
                    title: 'New Folder',
                    buttons: {
                        'Create': function () {
                            var name = $.trim(_name_input$.val());
                            if (name.length === 0) {
                                _name_errorHelper('Name is required');
                                return;
                            }
                            _name_ajaxHelper(
                                '/Admin/FileManager/CreateFolder',
                                {
                                    parentFolderUri: uri,
                                    name: name
                                },
                                function (data, textStatus, jqXHR) {
                                    var f$;
                                    if (_files_isFolderLoaded(n$)) {
                                        _files_tree$k.remove(n$.children('ul').children('li[data-is-none-node="true"]'));
                                        f$ = _files_appendFolder$(data.Fso, n$, false, true);
                                        _files_appendNoneNode$(f$);
                                    }
                                    if (_files_isFolderExpanded(n$)) {
                                        _files_doSelectNode(f$);
                                    }
                                    _name_form$.dialog('close');
                                }
                            );
                        }
                    }
                },
                '',
                ''
            );
        },

        _act_renameFso = function () {
            var n$ = _files_tree$k.select(),
                oldName = _files_tree$k.text(n$),
                ext = '',
                uri = n$.attr('data-uri'),
                action,
                data,
                func
            ;
            if (n$.attr('data-is-root-node') === 'true') {
                return;
            }
            if (!uri) {
                return;
            }
            if (n$.attr('data-is-folder-node') === 'true') {
                action = 'Folder';
                data = {
                    folderUri: uri
                };
                func = _files_appendFolder$;
            }
            else if (n$.attr('data-is-file-node') === 'true') {
                ext = getFileExtension(oldName, true);
                if (ext) {
                    oldName = oldName.substr(0, oldName.length - ext.length);
                }
                action = 'File';
                data = {
                    fileUri: uri
                };
                func = _files_appendFile$;
            }
            else {
                dialogHelper.alert('Invalid node type', 'Invalid');
                return;
            }
            _name_startupHelper(
                {
                    title: 'Rename',
                    buttons: {
                        'Rename': function () {
                            var name = $.trim(_name_input$.val());
                            if (name.length === 0) {
                                _name_errorHelper('Name is required');
                                return;
                            }
                            if (name.toLowerCase() === oldName.toLowerCase()) {
                                _name_form$.dialog('close');
                                return;
                            }
                            data.newName = name;
                            _name_ajaxHelper(
                                '/Admin/FileManager/Rename' + action,
                                data,
                                function (data, textStatus, jqXHR) {
                                    var parent$ = n$.parent('ul').parent('li'),
                                        f$
                                    ;
                                    _files_tree$k.remove(n$)
                                    f$ = func(data.Fso, parent$);
                                    _files_doSelectNode(f$);
                                    _name_form$.dialog('close');
                                }
                            );
                        }
                    }
                },
                oldName,
                ext
            );
        },

        _name_startupHelper = function (dialogOpts, inputVal, uneditable) {
            _name_loading$.hide();
            _name_error$.hide();
            _name_form$.dialog('option', dialogOpts).dialog('open');
            if (uneditable) {
                _name_uneditable$.show().text(uneditable);
            }
            else {
                _name_uneditable$.hide();
            }
            _name_input$
                .val(inputVal)
                .focus()
                .select()
            ;
        },

        _name_ajaxHelper = function (url, data, successCallback) {
            ajaxHelper.ajax(url, {
                data: data,
                type: 'POST',
                beforeSend: function (jqXHR, settings) {
                    _name_loading$.show();
                    _name_error$.stop().hide();
                },
                success: successCallback,
                failure: function (data, textStatus, jqXHR, formattedMessage) {
                    _name_errorHelper(formattedMessage);
                },
                failureDoAlert: false,
                error: function (jqXHR, textStatus, errorThrown, formattedMessage) {
                    _name_errorHelper(formattedMessage);
                },
                errorDoAlert: false,
                complete: function (jqXHR, textStatus) {
                    _name_loading$.hide();
                }
            });
        },

        _name_errorHelper = function (message) {
            _name_error$
                .html(message)
                .fadeIn()
            ;
            _name_input$
                .focus()
                .select()
            ;
        },

        _act_deleteFso = function () {
            dialogHelper.alert('Not implemented');
            var n$ = _files_tree$k.select(),
                name = _files_tree$k.text(n$),
                uri = n$.attr('data-uri'),
                obj,
                action,
                data
            ;
            if (n$.attr('data-is-root-node') === 'true') {
                return;
            }
            if (!uri) {
                return;
            }
            if (n$.attr('data-is-folder-node') === 'true') {
                obj = 'folder';
                action = 'Folder';
                data = {
                    folderUri: uri,
                    recursive: true
                };
            }
            else if (n$.attr('data-is-file-node') === 'true') {
                obj = 'file';
                action = 'File';
                data = {
                    fileUri: uri
                };
            }
            else {
                dialogHelper.alert('Invalid node type', 'Invalid');
                return;
            }
            dialogHelper.confirm(
                "Are you sure you want to delete the " + obj + " '" + name + "'?",
                'Delete',
                function () {
                    ajaxHelper.ajax('/Admin/FileManager/Delete' + action, {
                        data: data,
                        type: 'POST',
                        beforeSend: function (jqXHR, settings) {
                            _wait_browseCol$.show();
                        },
                        success: function (data, textStatus, jqXHR) {
                            var parent$ = n$.parent('ul').parent('li');
                            _files_tree$k.remove(n$)
                            _files_doSelectNode(parent$);
                            if (parent$.children('ul').length === 0) {
                                _files_appendNoneNode$(parent$);
                            }
                        },
                        failureMessageFormat: "An error occurred trying to delete the " + obj + " '" + name + "': [[errorThrown]]",
                        complete: function (jqXHR, textStatus) {
                            _wait_browseCol$.hide();
                        }
                    });
                }
            );
        }
    ;

    return {
        showDialog: _dlg_show,
        init: _init
    };
})();
