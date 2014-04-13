/// <reference path="http://clickfarmcdn.localhost/common/js/jquery/1.7.2/jquery-vsdoc.js" />
/// <reference path="http://clickfarmcdn.localhost/common/js/kendo/2012.2.710/kendo.all-vsdoc.js" />

//custom jeditable input types and type customizations

(function () {
    //fix some problems with the select type
    var type = $.editable.types['select'],
        selElement = type.element,
        selContent = type.content,
        tbPlugin,
        taPlugin
    ;
    type.element = function (settings, original) {
        return selElement.call(this, settings, original).width('100%');
    };
    type.content = function (data, settings, original) {
        if (settings.loadurl) {
            settings.loadeddata = data;
        }
        var revert = original.revert;
        original.revert = $('<span/>').html(original.revert).text();
        selContent.call(this, data, settings, original);
        original.revert = revert;
    };

    //fix some problems with the text type, and enable autocomplete
    type = $.editable.types['text'];
    type.element = function (settings, original) {
        return $('<input type="text" style="width: 100%;" autocomplete="off" />').appendTo(this);
    };
    type.content = function (data, settings, original) {
        $('input[type="text"]', this).val($('<span/>').html(data).text());
    };
    tbPlugin = type.plugin;
    type.plugin = function (settings, original) {
        if (typeof tbPlugin !== 'undefined' && tbPlugin) {
            tbPlugin.call(this, settings, original);
        }
        initAutocomplete($('input[type="text"]', this), this.parent('.editable-format'), settings);
    };

    //fix some problems with the textarea type, and enable autocomplete
    type = $.editable.types['textarea'];
    type.element = function (settings, original) {
        var input = $('<textarea style="width: 100%;" />');
        if (settings.height !== 'none') {
            input.height(settings.height);
        }
        return input.appendTo(this);
    };
    type.content = function (data, settings, original) {
        $('textarea', this).val($('<span/>').html(data).text());
    };
    taPlugin = type.plugin;
    type.plugin = function (settings, original) {
        if (typeof taPlugin !== 'undefined' && taPlugin) {
            taPlugin.call(this, settings, original);
        }
        initAutocomplete($('textarea', this), this.parent('.editable-format'), settings);
    };

    var fileSpanReplaceRegEx = new RegExp('<span .*?>(.*?)<\/span>', 'i');
    $.editable.addInputType('file', {
        element: function (settings, original) {
            return $('<input type="text" style="width: 100%;" autocomplete="off" />').appendTo(this);
        },
        content: function (data, settings, original) {
            data = data.replace(fileSpanReplaceRegEx, '');
            subStart = data.indexOf('src="') + 5;
            subEnd = data.indexOf('"', subStart);
            if (subStart !== -1 && subEnd >= subStart) {
                $('input[type="text"]', this).val(data.substring(subStart, subEnd));
            }
            else {
                $('input[type="text"]', this).val(data);
            }
        },
        plugin: function (settings, original) {
            var self = this,
                input = $('input[type="text"]', self),
                btn = $('<button type="button" class="small" title="Browse">...</button>')
                    .button()
                    .on('click', function () {
                        fileManager.showDialog(dialogOpts);
                    })
                ,
                parent = self.parent('.editable-format'),
                fileProps = $.parseJSON(parent.attr('data-content-file-properties')) || {},
                imgProps = settings.isImage ? $.parseJSON(parent.attr('data-content-image-properties')) || {} : {},
                dialogOpts = {
                    rootFolder: fileProps.RootFolder,
                    defaultSubfolder: fileProps.DefaultSubfolder,
                    enableSelection: true,
                    dialogTitle: settings.isImage ? 'Image Manager' : 'Document Manager',
                    mediaTypes: fileProps.MediaTypes,
                    fileExtensions: fileProps.FileExtensions,
                    preselectedFsoUri: input.val(),
                    selectCallback: function (fileSystemObject) {
                        if (settings.isImage && !validateImageFso(imgProps, fileSystemObject)) {
                            dialogOpts.preselectedFsoUri = fileSystemObject.Uri;
                            dialogHelper.element.one('dialogclose', function () {
                                fileManager.showDialog(dialogOpts);
                            });
                            return;
                        }
                        input.val(fileSystemObject.Uri);
                        self.submit();
                        skipCloseCallback = true;
                    } //, //don't do the below yet, because otherwise there's no way to specify an external URL
                    //closeCallback: function () {
                    //    if (!skipCloseCallback) {
                    //        original.reset(self);
                    //    }
                    //}
                },
                skipCloseCallback = false
            ;
            $('<table class="editable-field-with-supplement" border="0" cellpadding="0" cellspacing="0" />')
                .append(
                    $('<tr/>')
                        .append($('<td class="editable-field-main" />').append(input))
                        .append($('<td class="editable-field-supplement" />').append(btn))
                )
                .prependTo(self)
            ;
            fileManager.showDialog(dialogOpts);
        }
    });

    function validateImageFso(imgProps, fso) {
        var ok = true,
            msg = ''
        ;
        if (imgProps.ExactWidth) {
            msg += '<br/>Exact width: ' + imgProps.ExactWidth.toString();
            if (fso.Width !== imgProps.ExactWidth) {
                ok = false;
            }
        }
        if (imgProps.ExactHeight) {
            msg += '<br/>Exact height: ' + imgProps.ExactHeight.toString();
            if (fso.Height !== imgProps.ExactHeight) {
                ok = false;
            }
        }
        if (imgProps.MaxWidth) {
            msg += '<br/>Maximum width: ' + imgProps.MaxWidth.toString();
            if (fso.Width > imgProps.MaxWidth) {
                ok = false;
            }
        }
        if (imgProps.MaxHeight) {
            msg += '<br/>Maximum height: ' + imgProps.MaxHeight.toString();
            if (fso.Height > imgProps.MaxHeight) {
                ok = false;
            }
        }
        if (imgProps.MinWidth) {
            msg += '<br/>Minimum width: ' + imgProps.MinWidth.toString();
            if (fso.Width < imgProps.MinWidth) {
                ok = false;
            }
        }
        if (imgProps.MinHeight) {
            msg += '<br/>Minimum height: ' + imgProps.MinHeight.toString();
            if (fso.Height < imgProps.MinHeight) {
                ok = false;
            }
        }
        if (!ok) {
            dialogHelper.alert(
                "The image's dimensions are out of the acceptable range:<br/><br/>Selected width: " +
                    fso.Width.toString() +
                    "<br/>Selected height: " +
                    fso.Height.toString() +
                    "<br/>" + msg,
                'Invalid Image Dimensions'
            );
        }
        return ok;
    }

    $.editable.addInputType('html', {
        element: function (settings, original) {
            $('<textarea/>').appendTo(this);
            return $('<input type="hidden" />').appendTo(this);
        },
        content: function (data, settings, original) {
            if (!data) {
                data = '<p></p>';
            }
            $('textarea', this).val(data);
        },
        plugin: function (settings, original) {
            var textarea = $('textarea', this),
                parent = $(this).parent('.editable-format'),
                htmlProps = $.parseJSON(parent.attr('data-content-html-properties')) || {},
                fileImgProps = $.parseJSON(parent.attr('data-content-file-image-properties')) || {},
                fileDocProps = $.parseJSON(parent.attr('data-content-file-document-properties')) || {},
                editor = textarea
                    .kendoEditor({
                        tools: [
                            'bold',
                            'italic',
                            'underline',
                            'strikethrough',
                            'subscript',
                            'superscript',
                            'fontName',
                            'fontSize',
                            'foreColor',
                            'backColor',
                            'justifyLeft',
                            'justifyCenter',
                            'justifyRight',
                            'justifyFull',
                            'insertUnorderedList',
                            'insertOrderedList',
                            'indent',
                            'outdent',
                            'formatBlock',
                            'createLink',
                            'unlink',
                            'insertImage',
                            'insertHtml',
                            'viewHtml'
                        ],
                        insertHtml: parseHtmlSnippets(htmlProps.Snippets)
                    })
                    .data('kendoEditor'),
                editFrame = $('iframe', editor.wrapper)
            ;
            customizeCreateLinkTool(settings.currentAppID, fileDocProps);
            customizeInsertImageTool(fileImgProps);
            editor.wrapper
                .width('100%')
                .height('400px')
                .resizable({
                    handles: 's',
                    start: function () {
                        editFrame.hide();
                    },
                    stop: function () {
                        editFrame.show();
                        editor.wrapper.width('100%');
                    }
                })
            ;
            setTimeout(function () {
                editor.focus();
            }, 0);
        },
        submit: function (settings, original) {
            $('input[type="hidden"]', this).val($('textarea', this).data('kendoEditor').value());
        }
    });

    function parseHtmlSnippets(snippets) {
    	var snips = [];
    	for (var prop in snippets) {
    		snips.push({ text: prop, value: snippets[prop] });
    	}
    	if (snips.length === 0) {
    		snips.push({text: 'No snippets available', value: '' });//this avoids a Kendo error in cases where no snippets are configured
    	}
        return snips;
    }

    function customizeCreateLinkTool(currentAppID, fileDocProps) {
        if (kendo.ui.editor.LinkCommand.execNew) {
            return;
        }

        kendo.ui.editor.LinkCommand.execNew = true;
        kendo.ui.editor.LinkCommand.prototype.exec = function () {
            var range = this.getRange();

            var collapsed = range.collapsed;

            range = this.lockRange(true);

            var nodes = kendo.ui.editor.RangeUtils.textNodes(range);

            var initialText = null;

            var that = this;

            function apply(link) {
                var href = link.url;

                if (href && href != 'http://') {
                    that.attributes = { href: href };

                    var title = link.title;
                    if (title) {
                        that.attributes.title = title;
                    }

                    var text = link.text;
                    if (text !== initialText) {
                        that.attributes.innerHTML = text || href;
                    }

                    var target = link.newWindow;
                    if (target) {
                        that.attributes.target = '_blank';
                    }

                    that.formatter.apply(range, that.attributes);
                }

                if (that.change) {
                    that.change();
                }
            }

            function close() {
                kendo.ui.editor.Dom.windowFromDocument(kendo.ui.editor.RangeUtils.documentFromRange(range)).focus();

                that.releaseRange(range);
            }

            var a = nodes.length ? that.formatter.finder.findSuitable(nodes[0]) : null;

            var shouldShowText = nodes.length <= 1 || (nodes.length == 2 && collapsed);

            if (shouldShowText && nodes.length > 0) {
                initialText = nodes.length > 0 ? (nodes.length == 1 ? nodes[0].nodeValue : nodes[0].nodeValue + nodes[1].nodeValue) : ''
            }

            cms.linkBuilder.showDialog({
                currentAppID: currentAppID,
                initialValue: {
                    url: a ? a.getAttribute('href', 2) : '',
                    text: initialText,
                    title: a ? a.title : '',
                    newWindow: a ? a.target == '_blank' : false
                },
                fileManagerSettings: {
                    rootFolder: fileDocProps.RootFolder,
                    defaultSubfolder: fileDocProps.DefaultSubfolder,
                    dialogTitle: 'Document Manager',
                    mediaTypes: fileDocProps.MediaTypes,
                    fileExtensions: fileDocProps.FileExtensions
                },
                allowAnchorOpts: true,
                allowTextEntry: shouldShowText,
                buildCallback: apply,
                closeCallback: close
            });
        };
    }

    function customizeInsertImageTool(fileImgProps) {
        if (kendo.ui.editor.ImageCommand.execNew) {
            return;
        }

        kendo.ui.editor.ImageCommand.execNew = true;
        kendo.ui.editor.ImageCommand.prototype.exec = function () {
            var that = this,
                range = that.lockRange(),
                applied = false,
                img = kendo.ui.editor.RangeUtils.image(range);

            function apply(fso) {
                that.attributes = {
                    src: fso.Uri,
                    width: fso.Width,
                    height: fso.Height,
                    alt: '',
                    title: ''
                };

                applied = that.insertImage(img, range);

                if (that.change) {
                    that.change();
                }
            }

            function close() {
                kendo.ui.editor.Dom.windowFromDocument(kendo.ui.editor.RangeUtils.documentFromRange(range)).focus();
                if (!applied) {
                    that.releaseRange(range);
                }
            }

            fileManager.showDialog({
                rootFolder: fileImgProps.RootFolder,
                defaultSubfolder: fileImgProps.DefaultSubfolder,
                enableSelection: true,
                dialogTitle: 'Image Manager',
                mediaTypes: fileImgProps.MediaTypes,
                fileExtensions: fileImgProps.FileExtensions,
                preselectedFsoUri: img ? img.getAttribute('src', 2) : '',
                selectCallback: apply,
                closeCallback: close
            });
        };
    }

    $.editable.addInputType('hyperlink', {
        element: function (settings, original) {
            return $('<input type="text" style="width: 100%;" autocomplete="off" />').appendTo(this);
        },
        plugin: function (settings, original) {
            var self = this,
                input = $('input[type="text"]', self),
                parent = self.parent('.editable-format'),
                fileProps = $.parseJSON(parent.attr('data-content-file-properties')) || {},
                dialogOpts = {
                    currentAppID: settings.currentAppID,
                    initialValue: {
                        url: input.val()
                    },
                    fileManagerSettings: {
                        rootFolder: fileProps.RootFolder,
                        defaultSubfolder: fileProps.DefaultSubfolder,
                        dialogTitle: 'Document Manager',
                        mediaTypes: fileProps.MediaTypes,
                        fileExtensions: fileProps.FileExtensions
                    },
                    allowAnchorOpts: false,
                    allowTextEntry: false,
                    buildCallback: function (link) {
                        input.val(link.url);
                        self.submit();
                        skipCloseCallback = true;
                    },
                    closeCallback: function () {
                        if (!skipCloseCallback) {
                            original.reset(self);
                        }
                    }
                },
                skipCloseCallback = false
            ;
            cms.linkBuilder.showDialog(dialogOpts);
        }
    });

    $.editable.addInputType('template', {
        element: function (settings, original) {
            //var elements = $('#template-list').clone();
            var form = $(this);
            //$('li a', elements).on('click', function (e) {
            //    e.preventDefault();
            //    var anchor = $(this),
            //        aParent = anchor.parent()
            //    ;
            //    $('input', form).val(anchor.html());
            //    aParent.addClass('selected');
            //    aParent.siblings().removeClass('selected');
            //});
            //elements.appendTo(form);

            return $('<input type="hidden" value="' + $.trim(original.revert) + '" />').appendTo(form);
        },
        content: function (data, settings, original) {
            if (data.Succeeded) {
                var ul = $('<ul/>'),
                    form = $(this)
                ;
                $.each(data.Properties.Controllers, function (index, controller) {
                    var li = $('<li/>'),
                        anchor = $('<a href="#">' + controller.Name + '</a>')
                    ;
                    if ($.trim(original.revert) === controller.Name) {
                        li.addClass('selected');
                    }

                    anchor.on('click', function (event) {
                        event.preventDefault();
                        var a = $(this),
                            aParent = a.parent()
                        ;
                        $('input', form).val(a.html());
                        aParent.addClass('selected');
                        aParent.siblings().removeClass('selected');
                    });
                    li.append(anchor);
                    ul.append(li);
                });
                form.append(ul);
            }
            else {
                dialogHelper.alert(data.Message);
            }
        },
        plugin: function (settings, original) {
            $(this).find('button[type="submit"]').on('focus', function () { $(this).blur().off('focus'); });
        }
    });

    $.editable.addInputType('checkbox', {
        element: function (settings, original) {
            var input = $('<input type="checkbox" />').appendTo(this);
            input.on('click', function () {
                input.val(input.attr('checked') ? 'Yes' : 'No');
            });
            return input;
        },
        content: function (data, settings, original) {
            data = $.trim(data);
            var checked = data === 'Yes' ? 1 : 0;
            var input = $(':input:first', this);
            input.attr('checked', checked);
            var value = input.attr('checked') ? 'Yes' : 'No';
            input.val(value);
        }
    });

    $.editable.addInputType('yesnoboolean', {
        element: function (settings, original) {
            var form = $(this),
                div = $('<div class="yes-no-bool" />'),
                yesitem = $('<a href="#" class="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only "><span class="ui-button-icon-primary ui-icon"></span><span class="ui-button-text">Yes</span></a>'),
                noitem = $('<a href="#" class="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only"><span class="ui-button-icon-primary ui-icon"></span><span class="ui-button-text">No</span></a>'),
                changeButtonState = function (button, isActive) {
                    if (isActive) {
                        button.removeClass('ui-state-default').addClass('ui-state-focus');
                        button.find('span').filter(':first').removeClass('ui-icon-radio-off').addClass('ui-icon-check'); ;
                    }
                    else {
                        button.removeClass('ui-state-focus').addClass('ui-state-default');
                        button.find('span').filter(':first').removeClass('ui-icon-check').addClass('ui-icon-radio-off');
                    }
                };
            yesitem.on('click', function (event) {
                event.preventDefault();
                form.find('input').val('Yes');
                changeButtonState(yesitem, true);
                changeButtonState(noitem, false);
            });
            noitem.on('click', function (event) {
                event.preventDefault();
                form.find('input').val('No');
                changeButtonState(yesitem, false);
                changeButtonState(noitem, true);
            });
            if (original.revert === 'Yes') {
                changeButtonState(yesitem, true);
                changeButtonState(noitem, false);
            }
            else {
                changeButtonState(noitem, true);
                changeButtonState(yesitem, false);
            }
            div.append(yesitem).append(noitem).appendTo(form);
            return $('<input type="hidden" />').appendTo(form);
        },
        content: function (data, settings, original) {
            data = $.trim(data);
            if (data === 'Yes') {
                $('input', this).val('Yes');
                $('li.yes-option', this).addClass('selected');
                $('li.no-option', this).removeClass('selected');
            }
            else {
                $('input', this).val('No');
                $('li.yes-option', this).removeClass('selected');
                $('li.no-option', this).addClass('selected');
            }
        },
        plugin: function (settings, original) {
            $(this).find('button[type="submit"]').on('focus', function () { $(this).blur().off('focus'); });
        }
    });

    $.editable.addInputType('cmsurl', {
        element: function (settings, original) {
            return $('<input type="text" class="url-input" style="width: 100%;" autocomplete="off" />').appendTo(this);
        },
        plugin: function (settings, original) {
            //var urlMgr = cms.url.getClosestUrlManager(this);
            cms.url.initializeUrlInput($('input[type="text"]', this));
        }
    });

    $.editable.addInputType('textlimit', {
        element: function (settings, original) {
            var self = $(this),
                defaultProperties = { CharacterLimit: 200, Mode: 0 },
                o = $(original),
                p = o.attr('data-content-text-properties')
            ;
            //console.log('o', o);
            //console.log('p', p);
            var properties;
            if (p && p !== 'undefined') {
                properties = $.extend({}, defaultProperties, $.parseJSON(p.replace(/'/g, '"')));
            }
            else {
                properties = defaultProperties;
            }

            //Mode: 0 = single line, Mode: 1 = multiple line text box
            var input;
            //console.log(properties.Mode);
            if (parseInt(properties.Mode) === 1) {
                input = $('<textarea class="textlimit textlimit-input" style="width: 100%;" autocomplete="off" />');
            }
            else {
                input = $('<input type="text" class="textlimit textlimit-input" style="width: 100%;" autocomplete="off" />');
            }

            var limit = $('<span class="textlimit-counter"><span class="innercnt">0</span> of <span class="maxcnt">' + properties.CharacterLimit + '</span></span>')
            ;
            self.append(input).append(limit);
            settings.contentProperties = properties; //set this here so "content" and "plugin" callback have access
            var currVal = '',
                maxLen = false
            ;
            input.keyup(function () {
                var box = $(this),
                    val = box.val(),
                    newlen = val.length
                ;
                if (val !== currVal) {
                    if (newlen >= properties.CharacterLimit) {
                        box.val(val.substring(0, properties.CharacterLimit));
                        newlen = box.val().length;
                        if (!maxLen) {
                            box.addClass('maxlengtherr').parent().parent().addClass('maxlengtherr');
                            maxLen = true;
                        }
                    }
                    else {
                        if (maxLen) {
                            box.removeClass('maxlengtherr').parent().parent().removeClass('maxlengtherr');
                            maxLen = false;
                        }
                    }
                    box.parent().find('span.innercnt').text(newlen);
                    currVal = val;
                }
            });

            return input;
        },
        content: function (data, settings, original) {
            var safeData = $('<span/>').html(data).text();
            if (parseInt(settings.contentProperties.Mode) === 1) {
                $('textarea', this).val(safeData).trigger('keyup');
            }
            else {
                $('input[type="text"]', this).val(safeData).trigger('keyup');
            }
        },
        plugin: function (settings, original) {
            if (parseInt(settings.contentProperties.Mode) === 1) {
                initAutocomplete($('input[type="text"]', this), this.parent('.editable-format'), settings);
            }
            else {
                initAutocomplete($('textarea', this), this.parent('.editable-format'), settings);
            }
        }
    });

    $.editable.addInputType('checkbox-list', {
        element: function (settings, original) {
            return $('<input type="hidden" />').appendTo(this);
        },
        content: function (data, settings, original) {
            if (settings.loadurl) {
                settings.loadeddata = data;
            }
            var revert = $.trim(original.revert),
                selVals = [],
                val
            ;
            if (revert) {
                if (revert.substr(0, 4) !== '<ul>') {
                    if (!settings.listSeparator) {
                        settings.listSeparator = '\t';
                    }
                    $.each(revert.split(settings.listSeparator), function () {
                        val = $.trim(this);
                        if (val.length > 0) {
                            selVals.push(val);
                        }
                    });
                }
                else {
                    $(revert).find('li').each(function (ix, el) {
                        selVals.push($.trim($(el).html()));
                    });
                }
            }
            var formEl = $(this),
                brEl = $('<br/>')
            ;
            for (var key in data) {
                if (!data.hasOwnProperty(key)) {
                    continue;
                }
                var cbxID = 'cbx_' + randomInt(999999).toString(),
                    cbxEl = $('<input type="checkbox" />').val(key).attr('id', cbxID)
                ;
                val = htmlEncode(data[key]);
                if ($.inArray(val, selVals) > -1) {
                    cbxEl[0].checked = true;
                }
                formEl
                    .append(cbxEl)
                    .append(' ')
                    .append($('<label/>').attr('for', cbxID).append(val))
                    .append(brEl.clone())
                ;
            }
        },
        submit: function (settings, original) {
            var selVals = '',
                sep = ''
            ;
            if (!settings.listSeparator) {
                settings.listSeparator = '\t';
            }
            $('input[type="checkbox"]', this).each(function () {
                if (this.checked) {
                    selVals = selVals + sep + this.value;
                    sep = settings.listSeparator;
                }
            });
            $('input[type="hidden"]', this).val(selVals);
        }
    });

    $.editable.addInputType('password', {
        element: function (settings, original) {
            var formEl = $(this),
                pswdID = 'pswd_' + randomInt(999999).toString(),
                pswdEl = $('<input type="password" />'),
                brEl = $('<br/>')
            ;
            pswdEl.width('100%');
            formEl
                .append($('<label>New Password:</label>').attr('for', pswdID))
                .append(brEl.clone())
                .append(pswdEl.attr('id', pswdID))
                .append(brEl.clone())
            ;
            pswdID = 'pswd_' + randomInt(999999).toString();
            formEl
                .append($('<label>Confirm Password:</label>').attr('for', pswdID))
                .append(brEl.clone())
                .append(pswdEl.clone().attr('id', pswdID))
                .append(brEl.clone())
            ;
            return pswdEl;
        },
        submit: function (settings, original) {
            var pswdEls = $('input[type="password"]', this),
                pswdEl = pswdEls.filter(':first'),
                pswd = pswdEl.val(),
                pswdConfEl = pswdEls.filter(':last'),
                pswdConf = pswdConfEl.val()
            ;
            if (pswd !== pswdConf) {
                dialogHelper.alert('Passwords do not match.', 'Error');
                pswdEl.select();
                return false;
            }
            if (pswd.length < 8) {
                dialogHelper.alert('The password must be at least 8 characters.', 'Error');
                pswdEl.select();
                return false;
            }
            return true;
        }
    });

    function splitWS(val) {
        return val.split(/,\s*/);
    }

    function extractLastWS(term) {
        return splitWS(term).pop();
    }

    function initAutocomplete(input, editableDiv, settings) {
        var autocompleteUrl = editableDiv.attr('data-editable-autocomplete-url');
        if (!autocompleteUrl) {
            return;
        }

        settings.onblur = 'ignore';
        input.on('keydown', function (event) {
            //don't navigate away from the field on tab when selecting an item
            if (event.keyCode === $.ui.keyCode.TAB && input.data('autocomplete').menu.active) {
                event.preventDefault();
            }
        });
        if ($.parseJSON(editableDiv.attr('data-editable-autocomplete-multiple-values')) === true) {
            input.autocomplete({
                source: function (request, response) {
                    $.getJSON(autocompleteUrl, {
                        term: extractLastWS(request.term)
                    }, response);
                },
                search: function () {
                    //custom minLength
                    var term = extractLastWS(input.val());
                    if (term.length < 2) {
                        return false;
                    }
                },
                focus: function () {
                    //prevent value inserted on focus
                    return false;
                },
                select: function (event, ui) {
                    var terms = splitWS(input.val());
                    //remove the current input
                    terms.pop();
                    //add the selected item
                    terms.push(ui.item.value);
                    //add placeholder to get the comma-and-space at the end
                    terms.push('');
                    input.val(terms.join(', '));
                    return false;
                }
            });
        }
        else {
            input.autocomplete({
                source: autocompleteUrl,
                minLength: 2
            });
        }
    }

    /*--------------------------------------------------------------------------
    * Datepicker for Jeditable
    * Copyright (c) 2011 Piotr 'Qertoip' Włodarek
    * Licensed under the MIT license:
    *   http://www.opensource.org/licenses/mit-license.php
    * Depends on jQuery UI Datepicker
    * Project home:
    *   http://github.com/qertoip/jeditable-datepicker
    *
    * Modified 2012-09-26 by Bill Ayakatubby for Clickfarm Interactive
    */

    // modify :focus selector; forcibly limit to specific node types
    $.expr[':'].focus = function (elem) {
        return elem === document.activeElement && (elem.type || elem.href);
    };

    $.editable.addInputType('datepicker', {
        element: function (settings, original) {
            return $('<input type="text" style="width: 100%;" autocomplete="off" />').appendTo(this);
        },
        plugin: function (settings, original) {
            var form = (this instanceof $ ? this : $(this)),
                input = $('input[type="text"]', form),
                datepicker = {
                    onSelect: function () {
                        // clicking specific day in the calendar should
                        // submit the form and close the input field
                        form.submit();
                    },
                    onClose: function () {
                        if (!input.data('skipCloseEvent')) {
                            setTimeout(function () {
                                if (!input.is(':focus') && inputBlur) {
                                    // input has NO focus after 150ms which means
                                    // calendar was closed due to click outside of it
                                    // so let's close the input field without saving
                                    original.reset(form);
                                }

                                // the delay is necessary; calendar must be already
                                // closed for the above :focus checking to work properly;
                                // without a delay the form is submitted in all scenarios, which is wrong
                            }, 150);
                        }
                    }
                },
                inputBlur = false
            ;
            if (settings.datepicker) {
                $.extend(datepicker, settings.datepicker);
            }
            input
                .datepicker(datepicker)
                .on('blur', function () {
                    inputBlur = true;
                })
            ;
        },
        submit: function (settings, original) {
            var input = $('input[type="text"]', this);
            if (input.datepicker('widget').is(':visible')) {
                input.data('skipCloseEvent', true).datepicker('hide');
            }
        }
    });
    //--------------------------------------------------------------------------

    //this is a customization of the default buttons method of jeditable
    //for the purposes of styling the OK button
    $.editable.types.defaults.buttons = function (settings, original) {
        var form = this,
            div = $('<div class="editable-buttons" />'),
            submit,
            btn,
            i
        ;
        if (settings.submit) {
            if (settings.submit.match(/>$/)) {
                //if given html string use that
                submit = $(settings.submit).on('click', function () {
                    if (submit.attr('type') !== 'submit') {
                        form.submit();
                    }
                });
            }
            else {
                //otherwise use button with given string as text
                submit = $('<button type="submit" class="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only"><span class="ui-button-text">' + settings.submit + '</span></button>');
            }
            submit
                .hover(
                    function () { submit.removeClass('ui-state-default').addClass('ui-state-hover') },
                    function () { submit.removeClass('ui-state-hover').addClass('ui-state-default') }
                )
                .appendTo(div)
            ;
        }
        if (settings.cancel) {
            if (settings.cancel.match(/>$/)) {
                //if given html string use that
                btn = $(settings.cancel);
            }
            else {
                //otherwise use button with given string as text
                btn = $('<button type="button" class="jeditable-cancel">' + settings.cancel + '</button>');
            }
            btn
                .on('click', function (event) {
                    var reset;
                    if ($.isFunction($.editable.types[settings.type].reset)) {
                        reset = $.editable.types[settings.type].reset;
                    }
                    else {
                        reset = $.editable.types['defaults'].reset;
                    }
                    reset.call(form, settings, original);
                    return false;
                })
                .appendTo(div)
            ;
        }
        if (settings.additionalButtons && settings.additionalButtons.length > 0) {
            div.append('<span class="button-separator">|</span>');
            for (i = 0; i < settings.additionalButtons.length; i++) {
                if (settings.additionalButtons[i].text.match(/>$/)) {
                    //if given html string use that
                    btn = $(settings.additionalButtons[i].text);
                }
                else {
                    //otherwise use button with given string as text
                    btn = $('<button type="button" class="jeditable-cancel">' + settings.additionalButtons[i].text + '</button>');
                }
                btn
                    .on(
                        'click',
                        {
                            form: form,
                            jEditableSettings: settings
                        },
                        cms.parseCallback(settings.additionalButtons[i].click)
                    )
                    .appendTo(div)
                ;
            }
        }
        div.append('<br class="clear" />').appendTo(form);
    };
})();
