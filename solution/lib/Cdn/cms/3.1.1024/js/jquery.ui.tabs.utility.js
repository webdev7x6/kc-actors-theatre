/// <reference path="http://clickfarmcdn.localhost/common/js/jquery/1.7.2/jquery-vsdoc.js" />
/// <reference path="http://clickfarmcdn.localhost/common/js/kendo/2012.2.710/kendo.all-vsdoc.js" />

/*
A tab manager that encapsulates and standardizes some common jQuery UI tabs functionality.

Usage:

    var tabMgr = new TabManager({
        //options--see below
    });

For tab event bindings, see [[http://jqueryui.com/demos/tabs/#events]]. Binding to closableClick is similar to binding
to other events:

    tabMgr.$tabs.on('tabsclosableclick', function (event, ui)
    {
        //
    });

Options:
    closable - optional
        When true, tabs will be initialized as closable. All other values are treated as false.

        Example:

            options = {
                container: $('#app-index-tabs'),
                closable: true
            };

    container - required
        The jQuery object representing the element that will become the tab container.

        Example:

            options = {
                container: $('#app-index-tabs')
            };

    idPrefix - optional
        A prefix to use when creating tab IDs/fragments.

        Default: 'ui-tabs-<randomInt>-'

        Example:

            options = {
                container: $('#app-index-tabs'),
                idPrefix: 'ui-tabs-'
            };

    load_tabTypeActions - optional
        A hash of functions, keyed by tab type, to execute upon the load event of a tab.

        Example:

            options = {
                container: $('#app-index-tabs'),
                load_tabTypeSelector: '.app-index-tab',
                load_tabTypeActions: {
                    "edit-page": initEditPageTab
                }
            };

    load_tabTypeSelector - required if load_tabTypeActions is specified; otherwise, ignored
        A jQuery selector used to retrieve a tab panel's inner content container so the content container's tab type can
        be evaluated. See the example for load_tabTypeActions.

    numStaticTabs - optional
        The number of static tabs in the tab container. The "Find" tab on </Admin/App/Index/1> is static.

        Example:

            options = {
                container: $('#app-index-tabs'),
                numStaticTabs: 1
            };

    show_action - optional
        A function to be called upon the show event of a tab.

        Example:

            options = {
                container: $('#app-index-tabs'),
                show_action: function (event, ui) {
                    if (ui.index == 0) {
                        $('#find-page-input').select();
                    }
                }
            };

        Note: If the tab container contains one or more static tabs, this function will be executed for the first static
        tab immediately upon tabifying the container. Functions bound with .on('tabsshow', function (event, ui) { })
        will not be executed for the first static tab until after another tab has been shown.
*/

var TabManager = function (options) {
    var self = this;

    self.$tabs = null;
    self.tabs = ko.observableArray([]);
    self.numStaticTabs = 0;
    self.selectedTab = ko.observable();

    //constructor
    (function () {
        if (options.numStaticTabs > 0) {
            self.numStaticTabs = options.numStaticTabs;
        }
        var tabsOpts = {
            idPrefix: options.idPrefix || 'ui-tabs-' + randomInt(999999).toString() + '-',
            ajaxOptions: {
                success: ajaxHelper.success(null, null, false)
            },
            beforeLoad:function(e, ui) {//caching functionality
                if (ui.tab.data("loaded")) {
                    e.preventDefault();
                    return;
                }

                ui.jqXHR.success(function () {
                    ui.tab.data("loaded", true);
                });
            },
            load: function (e, ui) {
                if (options.load_tabTypeActions) {
                    var panel = $(ui.panel),
                        tabType = panel.find(options.load_tabTypeSelector).attr('data-tab-type')
                    ;
                    if (options.load_tabTypeActions[tabType]) {
                        options.load_tabTypeActions[tabType].call(this, panel);
                    }
                }
            },
            activate: function (e, ui) {
                if (options.show_action) {
                    options.show_action.call(this, e, ui, self);
                }
            }
            //,
            //add: function (e, ui) {
            //    self.$tabs.tabs('select', '#' + ui.panel.id);
            //}
        };
        if (options.closable === true) {
            tabsOpts.closable = true;
            tabsOpts.closableClick = function(e, ui) {
                e.preventDefault(); //we're removing the tab manually, so prevent the plugin from doing it
                self.removeTabByIndex(ui.index - self.numStaticTabs);
            };
        }
        self.$tabs = options.container.tabs(tabsOpts);
    })();

    self.findTabIndexByID = function (uniqueID) {
        var ixOf = -1;
        $.each(self.tabs(), function (ix, item) {
            if (item.uniqueID === uniqueID) {
                ixOf = ix;
                return false;
            }
        });
        return ixOf;
    };

    self.addTab = function (newTab) {
        var ix = self.findTabIndexByID(newTab.uniqueID);
        if (ix > -1) {
            self.$tabs.tabs('option','active', ix + self.numStaticTabs);
            self.selectedTab(self.tabs()[ix]);
        }
        else {
            $("<li><a href='" + newTab.url + "'>" + newTab.title + "</a></li>")
                .appendTo(self.$tabs.children(".ui-tabs-nav"));
            self.$tabs.tabs("refresh");
            //self.$tabs.tabs('add', newTab.url, newTab.title);Got done deprecated from jqueryui
            self.tabs.push(newTab);
            self.selectedTab(newTab);
            self.$tabs.tabs('option', 'active', (self.tabs().length - 1) + self.numStaticTabs);
        }
    };

    self.removeTabByID = function (uniqueID) {
        var ix = self.findTabIndexByID(uniqueID);
        if (ix > -1) {
            self.removeTabByIndex(ix);
        };
    };

    self.reloadTabByID = function (uniqueID) {
        var ix = self.findTabIndexByID(uniqueID);
        if (ix > -1) {
            self.$tabs.tabs('load', ix + self.numStaticTabs);
        };
    };

    var removeTab = function(index) {
        var tab = self.$tabs.children(".ui-tabs-nav").find("li:eq(" + index + ")").remove();
        // Find the id of the associated panel
        var panelId = tab.attr("aria-controls");
        // Remove the panel
        $("#" + panelId).remove();
        // Refresh the tabs widget
        self.$tabs.tabs("refresh");
    };
    
    self.removeTabByIndex = function (index) {
        removeTab(index + self.numStaticTabs);
        self.tabs.splice(index, 1);
    };

    self.closeAllTabs = function () {
        var numTabs = self.tabs().length;
        for (var i = 0; i < numTabs; i++) {
            removeTab(self.numStaticTabs);
        }
        self.tabs.splice(0, numTabs);
    };

    self.updateTabTitleByID = function (uniqueID, newTitle) {
        var ix = self.findTabIndexByID(uniqueID);
        if (ix > -1) {
            self.updateTabTitleByIndex(ix, newTitle);
        }
    };

    self.updateTabTitleByIndex = function (index, newTitle) {
        self.tabs()[index].title = newTitle;
        var panel = self.$tabs.children('div:eq(' + (index + self.numStaticTabs).toString() + ')');
        if (panel.length === 1) {
            var tab = self.$tabs.find('a[href="#' + panel.attr('id') + '"] span');
            if (tab.length === 1) {
                tab.text(newTitle);
            }
        }
    };
};

var TabInfo = function(url, title, type, uniqueID) {
    var self = this;

    self.url = url;
    self.title = title;
    self.type = type;
    self.uniqueID = uniqueID;
};
