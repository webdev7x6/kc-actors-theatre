/// <reference path="http://clickfarmcdn.localhost/common/js/jquery/1.7.2/jquery-vsdoc.js" />
/// <reference path="http://clickfarmcdn.localhost/common/js/kendo/2012.2.710/kendo.all-vsdoc.js" />

//A class that encapsulates and standardizes some common Jeditable functionality.

function EditableManager(editableContainer, properties, ajaxUrl, selectorSuffix) {
    var self = this,
        container = editableContainer,
        props = properties,
        url = ajaxUrl,
        selectSuffix = selectorSuffix
    ;

    this.initTextboxes = function () {
        self.makeEditable('.editable', 'text');
    };

    this.initTextareas = function () {
        self.makeEditable('.editable-textarea', 'textarea');
    };

    this.initSelects = function () {
        container.find('.editable-select').each(function () {
            var this$ = $(this);
            self.makeEditable(
                '.editable-select-' + this$.attr('data-editable-select-model'),
                'select',
                this$.attr('data-editable-select-data') || null,
                this$.attr('data-editable-select-load-url') || null,
                $.parseJSON(typeof (this$.attr('data-editable-select-load-data')) === 'undefined' ? null : this$.attr('data-editable-select-load-data')) || null
            );
        });
    };

    this.initFile = function () {
        self.makeEditable('.editable-file', 'file');
    };

    this.initImage = function () {
        self.makeEditable('.editable-image', 'file');
    };

    this.initHtml = function () {
        self.makeEditable('.editable-html', 'html');
    };

    this.initCmsTemplates = function (pageID, appID) {
        self.makeEditable('.editable-template', 'template', {}, '/Admin/Page/ManagedControllers?pageID=' + pageID + '&appID=' + appID);
    };

    this.initCheckboxes = function () {
        self.makeEditable('.editable-checkbox', 'checkbox');
    };

    this.initYesNoBoolean = function () {
        self.makeEditable('.editable-yes-no-boolean', 'yesnoboolean');
    };

    this.initCmsUrls = function () {
        self.makeEditable('.editable-cmsurl', 'cmsurl');
    };

    this.initTextLimit = function () {
        self.makeEditable('.editable-textlimit', 'textlimit');
    };

    this.initCheckboxLists = function () {
        container.find('.editable-checkbox-list').each(function () {
            var this$ = $(this);
            self.makeEditable(
                '.editable-checkbox-list-' + this$.attr('data-editable-checkbox-list-model'),
                'checkbox-list',
                this$.attr('data-editable-checkbox-list-data') || null,
                this$.attr('data-editable-checkbox-list-load-url') || null,
                $.parseJSON(typeof (this$.attr('data-editable-checkbox-list-load-data')) === 'undefined' ? null : this$.attr('data-editable-checkbox-list-load-data')) || null
            );
        });
    };

    this.initDatePickers = function () {
        self.makeEditable('.editable-datepicker', 'datepicker');
    };

    this.initHyperlinks = function () {
        self.makeEditable('.editable-hyperlink', 'hyperlink');
    };

    this.initPasswords = function () {
        self.makeEditable('.editable-password', 'password');
    };

    this.initNullables = function () {

    	$('.nullable', container)
		.wrap($('<div class="nullable-wrapper"></div>'))
		.after(function (index) {
			var editableDiv = $(this),
				btn = $('<a class="nullify-content-link" data-content-id="' + editableDiv.closest('.content-editor').attr('data-content-id') + '" data-property-name="' + editableDiv.attr('data-property-name') + '" title="Clear" href="#">Clear</a>')
			btn.button({
				icons: {
					primary: 'ui-icon-circle-close'
				},
				text: false
			})
			.on('click', function (event) {
				event.preventDefault();
				if (confirm('Are you sure you want to clear this content?')) {
					var link = $(this),
					contentID = link.attr('data-content-id'),
					propertyName = link.attr('data-property-name')
					;
					ajaxHelper.ajax(ajaxUrl,
					{
						type: 'POST',
						data: $.extend({ property: propertyName, newValue: '' }, props),
						success: function (data, textStatus, jqXHR) {
							editableDiv.html('Click to edit');
							if (typeof appViewModel !== 'undefined') {
								appViewModel.registerUnappliedChange();
							}
						}
					});
				}
			});			
			return btn;
		});
    }

    this.initTypesForContent = function () {
    	//built-in
    	self.initTextboxes();
    	self.initTextareas();
    	self.initSelects();

    	//custom
    	self.initFile();
    	self.initImage();
    	self.initHtml();
    	self.initCheckboxes();
    	self.initYesNoBoolean();
    	self.initTextLimit();
    	self.initCheckboxLists();
    	self.initDatePickers();
    	self.initHyperlinks();

    	//nullables that need remove/clear capability
    	self.initNullables();
    };

    this.initAllTypes = function () {
        //built-in
        self.initTextboxes();
        self.initTextareas();
        self.initSelects();

        //custom
        self.initFile();
        self.initImage();
        self.initHtml();
        self.initCheckboxes();
        self.initYesNoBoolean();
        self.initCmsUrls();
        self.initTextLimit();
        self.initCheckboxLists();
        self.initPasswords();
        self.initDatePickers();
        self.initHyperlinks();
    };

    this.makeEditable = function (selector, type, data, loadUrl, loadData) {
        if (typeof $.editable.types[type] === 'undefined') {
            return;
        }
        if (selectSuffix) {
            selector = selector + selectSuffix;
        }
        var editSettings = {
            id: 'property',
            name: 'newValue',
            type: type,
            onblur: type === 'html' || type === 'file' || type === 'password' || type === 'datepicker' || type === 'hyperlink' ? 'ignore' : 'cancel',
            select: true,
            indicator: editableUtility.ajaxloader,
            tooltip: $.fn.editable.defaults.placeholder,
            submitdata: props,
            submit: 'OK',
            cancel: 'Cancel',
            additionalButtons: null,
            url: url,
            customProps: props,
            data: data,
            loadurl: loadUrl,
            loaddata: loadData,
            cssclass: 'editable-form',
            isImage: selector === '.editable-image' ? true : false,
            currentAppID: 0
        };
        if (typeof appViewModel !== 'undefined' && appViewModel.currentAppID) {
            editSettings.currentAppID = appViewModel.currentAppID();
        }
        container.find(selector).each(function () {
            var $this = $(this),
                oldOnBlur = editSettings.onblur
        	;
            if (typeof ($this.attr('data-editable-additional-buttons')) !== 'undefined') {
            	editSettings.additionalButtons = $.parseJSON($this.attr('data-editable-additional-buttons')) || null;
            }
            if (editSettings.additionalButtons) {
                editSettings.onblur = 'ignore';
            }
            $this.editable('destroy');
            $this.editable(editableUtility.doEditInPlace, $.extend({}, editSettings));
            editSettings.onblur = oldOnBlur;
        });
    };
}

var editableUtility = {
    ajaxloader: '<img src="//' + cms.globals.cdnHostName + '/common/images/ajax-loader.gif" alt="Loading" />',
    doEditInPlace: function (value, settings) {
        var element = $(this);
        var property = element.attr('data-property-name') || (element.attr('id') || null);
        if (property !== null) {
            value = $.trim(value);
            var origValue,
                ajaxValue,
                retValue
            ;
            switch (settings.type) {
                case 'select':
                    origValue = this.revert;
                    ajaxValue = value;
                    retValue = editableUtility.getEditableDataDisplayValue(settings, value);
                    break;
                case 'yesnoboolean':
                    origValue = this.revert;
                    ajaxValue = value === 'Yes' ? 'true' : 'false';
                    retValue = value;
                    break;
                case 'checkbox-list':
                    origValue = this.revert;
                    ajaxValue = value;
                    retValue = editableUtility.getEditableDataDisplayValue(settings, value);
                    break;
                case 'password':
                    origValue = settings.placeholder;
                    ajaxValue = value;
                    retValue = settings.placeholder;
                    break;
                case 'text':
                case 'textarea':
                case 'textlimit':
                    origValue = this.revert;
                    ajaxValue = value;
                    retValue = htmlEncode(value);
                    break;
                default:
                    origValue = this.revert;
                    ajaxValue = value;
                    retValue = value;
                    break;
            }
            var ajaxProps = { property: property, newValue: ajaxValue };
            $.extend(ajaxProps, settings.customProps); //merge default props with ones provided in constructor
            ajaxHelper.ajax(settings.url,
            {
                type: 'POST',
                data: ajaxProps,
                success: function (data, textStatus, jqXHR) {
                    switch (settings.type) {
                        case 'select':
                            data.displayValue = editableUtility.getEditableDataDisplayValue(settings, data.NewValue);
                            break;
                        case 'yesnoboolean':
                            data.displayValue = data.NewValue === 'true' ? 'Yes' : 'No';
                            break;
                        case 'file':
                            if (settings.isImage) {
                                data.displayValue = '<img alt="" src="' + data.NewValue + '" />';
                            }
                            else {
                                data.displayValue = '<span class="file-icon-16 file-icon-inline ' + getFileExtension(value) + '"></span>' + data.NewValue;
                            }
                            break;
                        case 'checkbox-list':
                            data.displayValue = editableUtility.getEditableDataDisplayValue(settings, data.NewValue);
                            break;
                        case 'password':
                            data.displayValue = settings.placeholder;
                            break;
                        case 'text':
                        case 'textarea':
                        case 'textlimit':
                            data.displayValue = htmlEncode(data.NewValue);
                            break;
                        default:
                            data.displayValue = data.NewValue;
                            break;
                    }
                    element.html(data.displayValue);

                    var callback = cms.parseCallback(element.attr('data-edit-callback'));
                    if (callback) {
                        callback(element, data);
                    }
                    if (typeof appViewModel !== 'undefined') {
                        appViewModel.registerUnappliedChange();
                    }
                },
                failure: function (data, textStatus, jqXHR, formattedMessage) {
                    element.html(origValue);
                },
                error: function (jqXHR, textStatus, errorThrown, formattedMessage) {
                    element.html(origValue);
                },
                errorMessageFormat: 'There was an error updating the property: [[errorThrown]]'
            });
            //because we will generally get here before the ajax call comes back and we can't block
            //we return the new value but if the ajax call returns error it will revert back to the
            //original value
            return retValue;
        }
        else {
            dialogHelper.alert('There was no property assigned to the field.', 'Error');
        }
    },
    getEditableDataDisplayValue: function (settings, value) {
        var selVals = [];
        if (!settings.listSeparator) {
            settings.listSeparator = '\t';
        }
        $.each(value.split(settings.listSeparator), function () {
            var val = $.trim(this);
            if (val.length > 0) {
                selVals.push(val);
            }
        });
        var ret = '',
            retPref = '',
            retSuff = '',
            valPref = '',
            valSuff = ''
        ;
        if (selVals.length > 0) {
            if (selVals.length > 1) {
                retPref = '<ul>';
                retSuff = '</ul>';
                valPref = '<li>';
                valSuff = '</li>';
            }
            //loadeddata mainly used for checkbox-list and selects to take selected values and make them friendly for display
            if (settings.loadeddata) {
                $.each(selVals, function () {
                    ret += valPref + settings.loadeddata[this] + valSuff;
                });
            }
            else if (settings.data) {
                if (String == settings.data.constructor) {
                    eval('var data = ' + settings.data);
                }
                else {
                    var data = settings.data;
                }
                if (data) {
                    $.each(selVals, function () {
                        ret += valPref + data[this] + valSuff;
                    });
                }
                else {
                    ret = value;
                }
            }
            else {
                ret = value;
            }
        }
        if (ret) {
            ret = retPref + ret + retSuff;
        }
        else {
            ret = settings.placeholder;
        }
        return ret;
    }
};

