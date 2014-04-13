/// <reference path="http://clickfarmcdn.localhost/common/js/jquery/1.7.2/jquery-vsdoc.js" />
/// <reference path="http://clickfarmcdn.localhost/common/js/kendo/2012.2.710/kendo.all-vsdoc.js" />

(function () {
    var cms = window.cms = window.cms || {};
    var linkBuilder = cms.linkBuilder = cms.linkBuilder || (function () {
        var _context = null,
            _dlg_settings = null,
            _dlg_dialog$ = null,
            _dlg_canBuild = false,
            _dlg_buildButton$ = null,
            _tabs_container$ = null,
            _tabs_focusCache = {},
            _cmsUrl_hasApps = false,
            _cmsUrl_apps$ = null,
            _cmsUrl_search$ = null,
            _cmsUrl_searchResults$ = null,
            _cmsUrl_mgr = null,
            _fileMgrSettings = null,
            _linkVM_urlUpdateLock = false,

            _linkVM = new (function () {
                var self = this;

                self.cmsUrl_url = ko.observable('');
                self.cmsUrl_urlMgrVM = null;
                self.cmsUrl_primaryHostName = ko.observable('');
                self.cmsUrl_primaryHostLabel = ko.observable('');
                self.cmsUrl_app = ko.observable('0');

                self.file_url = ko.observable('');

                self.email_toAddresses = ko.observable('');
                self.email_subject = ko.observable('');
                self.email_url = ko.computed({
                    read: function () {
                        var addr = self.email_toAddresses(),
                            subj = self.email_subject()
                        ;
                        if (subj) {
                            subj = '?subject=' + subj;
                        }
                        if (addr || subj) {
                            addr = 'mailto:' + addr;
                        }
                        return addr + subj;
                    },
                    write: function (value) {
                        var v = value.substr(7),
                            qsIx = v.indexOf('?'),
                            suIx,
                            su = 'subject'
                        ;
                        if (qsIx !== -1) {
                            self.email_toAddresses(v.substr(0, qsIx));
                            suIx = v.toLowerCase().indexOf('subject=', qsIx);
                            if (suIx !== -1) {
                                su = v.substr(suIx, 7);
                            }
                            self.email_subject(cms.parseParamsString(v.substr(qsIx + 1))[su] || '');
                        }
                        else {
                            self.email_toAddresses(v);
                            self.email_subject('');
                        }
                    },
                    owner: this
                });

                self.manual_url = ko.observable('');
                self.manual_text = ko.observable('');
                self.manual_title = ko.observable('');
                self.manual_newWindow = ko.observable(false);
                self.manual_allowAnchorOpts = ko.observable(false);
                self.manual_allowTextEntry = ko.observable(false);
            }),

            _linkVMSubscr = null,

            _dlg_show = function (settings) {
                var dlg_settings = $.extend(
                        {},
                        {
                            currentAppID: 0,
                            initialValue: {
                                url: '',
                                text: '',
                                title: '',
                                newWindow: false
                            },
                            fileManagerSettings: {},
                            allowAnchorOpts: false,
                            buildCallback: null,
                            closeCallback: null
                        },
                        settings
                    )
                ;
                if (typeof dlg_settings.initialValue === 'string') {
                    dlg_settings.initialValue = {
                        url: dlg_settings.initialValue,
                        text: '',
                        title: '',
                        newWindow: false
                    };
                }
                if (_dlg_settings) {
                    //check for changed settings that would force us to re-init the dialog
                    //if (
                    //    _dlg_settings.rootFolder !== dlg_settings.rootFolder
                    //    || _dlg_settings.enableSelection !== dlg_settings.enableSelection
                    //) {
                    //    _dlg_dialog$.dialog('destroy');
                    //    _dlg_dialog$ = null;
                    //}
                }
                _dlg_settings = dlg_settings;
                if (!_dlg_dialog$) {
                    ajaxHelper.ajax('/Admin/Url/ShowLinkBuilder', {
                        type: 'GET',
                        success: function (data, textStatus, jqXHR) {
                            _dlg_dialog$ = $('<div title="Link Builder" />').dialog({
                                autoOpen: false,
                                buttons: {
                                    OK: _dlg_onBuild,
                                    Cancel: function () {
                                        _dlg_dialog$.dialog('close');
                                    }
                                },
                                height: 650,
                                modal: true,
                                width: 650,
                                open: function () {
                                    _tabs_onShow(_tabs_container$.data('tabs').panels[_cmsUrl_hasApps ? 0 : 1]);
                                }
                            });
                            _context = _dlg_dialog$[0];
                            _dlg_buildButton$ = _dlg_dialog$.siblings('.ui-dialog-buttonpane').find('button').filter(function () {
                                return $('span', this).text() === 'OK';
                            });
                            _dlg_disableBuild();

                            //init dialog html/ui
                            _dlg_dialog$.html(data);
                            _tabs_container$ = $('#link-builder-tabs', _context).tabs({
                                show: function (e, ui) {
                                    _tabs_onShow(ui.panel);
                                }
                            });
                            _cmsUrl_hasApps = $('#link-builder-cms-url-tab', _context).attr('data-has-apps') === 'True';
                            cms.url.buttonizeUrlLinks(_context);
                            _cmsUrl_apps$ = $('#link-builder-cms-url-app option[value!="0"]');
                            _cmsUrl_search$ = $('#link-builder-cms-url-search', _context).keyup($.debounce(350, function () {
                                var term = _cmsUrl_search$.val(),
                                    appID = _linkVM.cmsUrl_app()
                                ;
                                if (term.length > 1 && appID) {
                                    _cmsUrl_mgr.ajaxData.term = term;
                                    _cmsUrl_mgr.ajaxData.appID = appID;
                                    _cmsUrl_mgr.refreshUrls();
                                }
                            }));
                            _cmsUrl_searchResults$ = $('#link-builder-cms-url-search-results', _context).on('click', '.link-builder-cms-url-select', function () {
                                _linkVM.cmsUrl_url($(this).text());
                            });
                            $('#link-builder-file-browse', _context)
                                .button()
                                .on('click', function () {
                                    fileManager.showDialog(_fileMgrSettings);
                                })
                            ;

                            //init cms url manager
                            //do this before knockout below so we can capture its view-model in our own
                            _cmsUrl_mgr = new cms.url.UrlManager(
                                $('<div/>')[0],
                                '/Admin/Url/FindUrl',
                                {
                                    term: '',
                                    appID: 0
                                },
                                function () {
                                    cms.url.buttonizeUrlLinks(_cmsUrl_searchResults$);
                                }
                            );
                            _cmsUrl_mgr.initialize();
                            _linkVM.cmsUrl_urlMgrVM = _cmsUrl_mgr.getViewModel();

                            //init knockout
                            ko.applyBindings(_linkVM, _context);
                            if (_linkVMSubscr === null) {
                                _linkVMSubscr = [
                                    _linkVM.cmsUrl_url.subscribe(function (newValue) { _linkVM_onUrlUpdate('cmsUrl', newValue); }),
                                    _linkVM.file_url.subscribe(function (newValue) { _linkVM_onUrlUpdate('file', newValue); }),
                                    _linkVM.email_url.subscribe(function (newValue) { _linkVM_onUrlUpdate('email', newValue); }),
                                    _linkVM.manual_url.subscribe(function (newValue) { _linkVM_onUrlUpdate('manual', newValue); })
                                ];
                            }
                            _linkVM.cmsUrl_app.subscribe(function (newValue) {
                                _cmsUrl_search$.val('');
                                _linkVM.cmsUrl_urlMgrVM.urls([]);
                                _linkVM.cmsUrl_urlMgrVM.retrievingUrls(false);
                                if (newValue && newValue !== '0') {
                                    var primHost = $.parseJSON(_cmsUrl_apps$.filter('[value="' + newValue + '"]').attr('data-primary-host'));
                                    _linkVM.cmsUrl_primaryHostName(primHost.hostName);
                                    _linkVM.cmsUrl_primaryHostLabel(primHost.label);
                                    _cmsUrl_search$[0].disabled = false;
                                }
                                else {
                                    _linkVM.cmsUrl_primaryHostName('');
                                    _linkVM.cmsUrl_primaryHostLabel('');
                                    _cmsUrl_search$[0].disabled = true;
                                }
                            });

                            _dlg_initAndOpen();
                        },
                        failureMessageFormat: "An error occurred trying to show the link builder: [[errorThrown]]",
                        errorMessageFormat: "An error occurred trying to show the link builder: [[errorThrown]]"
                    });
                }
                else {
                    _dlg_initAndOpen();
                }
            },

            _dlg_initAndOpen = function () {
                _cmsUrl_search$.val('');
                _linkVM.cmsUrl_app('0');
                if (_dlg_settings.currentAppID !== 0) {
                    _linkVM.cmsUrl_app(_dlg_settings.currentAppID.toString());
                }
                else if (_cmsUrl_apps$.length === 1) {
                    _linkVM.cmsUrl_app(_cmsUrl_apps$.filter(':first').val());
                }
                _linkVM.cmsUrl_urlMgrVM.urls([]);
                _linkVM.cmsUrl_urlMgrVM.retrievingUrls(false);

                _fileMgrSettings = {
                    rootFolder: _dlg_settings.fileManagerSettings.rootFolder,
                    defaultSubfolder: _dlg_settings.fileManagerSettings.defaultSubfolder,
                    enableSelection: true,
                    dialogTitle: _dlg_settings.fileManagerSettings.dialogTitle,
                    mediaTypes: _dlg_settings.fileManagerSettings.mediaTypes,
                    fileExtensions: _dlg_settings.fileManagerSettings.fileExtensions,
                    selectCallback: function (fileSystemObject) {
                        _linkVM.file_url(_fileMgrSettings.preselectedFsoUri = fileSystemObject.Uri);
                    }
                };

                if (_dlg_settings.initialValue.url) {
                    var url = _dlg_settings.initialValue.url.toLowerCase(),
                        san = cms.url.sanitizeAlias(url)
                    ;
                    if (url.substr(0, 7) === 'mailto:') {
                        //email
                        _linkVM.email_url(_dlg_settings.initialValue.url);
                        _tabs_container$.tabs('select', 'link-builder-email-tab');
                    }
                    else if (url.substr(0, cms.globals.cmsFileUrlBase.length) === cms.globals.cmsFileUrlBase.toLowerCase() && san === url) {
                        //file
                        _fileMgrSettings.preselectedFsoUri = _dlg_settings.initialValue.url;
                        _linkVM.file_url(_dlg_settings.initialValue.url);
                        _tabs_container$.tabs('select', 'link-builder-file-tab');
                    }
                    else if (url.charAt(0) === '/' && url.charAt(1) !== '/' && san === url && _cmsUrl_hasApps) {
                        //cms url
                        _linkVM.cmsUrl_url(_dlg_settings.initialValue.url);
                        _tabs_container$.tabs('select', 'link-builder-cms-url-tab');
                    }
                    else {
                        //other/manual
                        _linkVM.manual_url(_dlg_settings.initialValue.url);
                        _tabs_container$.tabs('select', 'link-builder-manual-tab');
                    }
                }
                else {
                    _linkVM.manual_url('');
                    if (_cmsUrl_hasApps) {
                        _tabs_container$.tabs('select', 'link-builder-cms-url-tab');
                    }
                    else {
                        _tabs_container$.tabs('select', 'link-builder-file-tab');
                    }
                }

                _linkVM
                    .manual_text(_dlg_settings.initialValue.text)
                    .manual_title(_dlg_settings.initialValue.title)
                    .manual_newWindow(_dlg_settings.initialValue.newWindow)
                    .manual_allowAnchorOpts(_dlg_settings.allowAnchorOpts)
                    .manual_allowTextEntry(_dlg_settings.allowTextEntry)
                ;

                _dlg_dialog$
                    .one('dialogclose', _dlg_settings.closeCallback)
                    .dialog('open')
                ;
            },

            _linkVM_onUrlUpdate = function (type, newValue) {
                if (!_linkVM_urlUpdateLock) {
                    _linkVM_urlUpdateLock = true;
                    switch (type) {
                        case 'cmsUrl':
                            _linkVM.file_url('');
                            _linkVM.email_url('');
                            _linkVM.manual_url(newValue);
                            break;
                        case 'file':
                            _linkVM.cmsUrl_url('');
                            _linkVM.email_url('');
                            _linkVM.manual_url(newValue);
                            break;
                        case 'email':
                            _linkVM.cmsUrl_url('');
                            _linkVM.file_url('');
                            _linkVM.manual_url(newValue);
                            break;
                        case 'manual':
                            _linkVM.cmsUrl_url('');
                            _linkVM.file_url('');
                            _linkVM.email_url('');
                            break;
                    }
                    _linkVM_urlUpdateLock = false;
                }
                if (type === 'manual') {
                    if (newValue) {
                        _dlg_enableBuild();
                    }
                    else {
                        _dlg_disableBuild();
                    }
                }
            },

            _tabs_onShow = function (panel) {
                var ctl = $(panel).attr('data-focus-control');
                if (ctl) {
                    if (typeof _tabs_focusCache[ctl] === 'undefined') {
                        _tabs_focusCache[ctl] = $('#' + ctl, _context);
                    }
                    setTimeout(function () {
                        _tabs_focusCache[ctl].focus().select();
                    }, 0);
                }
            },

            _dlg_onBuild = function () {
                if (_dlg_canBuild) {
                    if ($.isFunction(_dlg_settings.buildCallback)) {
                        var link = {
                            url: _linkVM.manual_url()
                        };
                        if (_dlg_settings.allowAnchorOpts) {
                            link.text = _linkVM.manual_text();
                            link.title = _linkVM.manual_title();
                            link.newWindow = _linkVM.manual_newWindow();
                        }
                        _dlg_settings.buildCallback(link);
                    }
                    _dlg_dialog$.dialog('close');
                }
            },

            _dlg_disableBuild = function () {
                _dlg_canBuild = false;
                _dlg_buildButton$.button('option', 'disabled', true);
            },

            _dlg_enableBuild = function (props) {
                _dlg_canBuild = true;
                _dlg_buildButton$.button('option', 'disabled', false);
            }
        ;

        return {
            showDialog: _dlg_show
        };
    })();
})();
