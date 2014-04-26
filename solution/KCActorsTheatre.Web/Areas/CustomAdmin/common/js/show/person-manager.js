/// <reference path="http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.8.3-vsdoc.js" />
/// <reference path="http://clickfarmcdn.localhost/common/js/knockout/2.1.0/knockout.js" />
/// <reference path="http://clickfarmcdn.localhost/common/js/kendo/2012.2.710/kendo.all-vsdoc.js" />

(function ($, undefined) {
    var admin = window.admin = window.admin || {};
    (function () {
        admin.items.index.personClick = function (container, parent) {
            var
                $container = $(container),
                people = [],
                $people = $container.find('ul')
            ;
            $people.find('li').each(function _people_onEach(ix, el) {
                var $el = $(el);
                people.push({
                    personID: $el.attr('data-person-id'),
                    name: $el.text()
                });
            });
            admin.people.show(people, parent.attr('data-item-id'), function _people_onClose(people) {
                $container.empty();
                if (people && people.length) {
                    if (!$people || !$people.length) {
                        $people = $('<ul/>');
                    }
                    $people.empty().appendTo($container);
                    $.each(people, function _people_onEach(ix, item) {
                        $people.append($('<li/>').text(item.name).attr('data-person-id', item.personID));
                    });
                }
                else {
                    $container.append('Click to edit');
                }
            });
        }

        admin.people = (function () {
            var $dlg = null,
                closeCallback = null,
                ITEM_ID = null,
                $associatedPeople = null,
                associatedPeopleViewModel = {
                    people: ko.observableArray([])
                },
                $availablePeople = null,
                $availablePeopleFindName = null,
                availablePeopleViewModel = null,
                $currentCmsRole = null
            ;
            return {
                init: function _init($modal) {
                    $associatedPeople = $('#sortable-assigned-people');//.on('dblclick', 'li', switchLists);
                    $dlg = $modal.dialog({
                        autoOpen: false,
                        height: 600,
                        modal: true,
                        resizable: false,
                        width: 930,
                        buttons: {
                            Close: function _closeBtn_onClick() {
                                $dlg.dialog('close');
                            }
                        },
                        close: function _dlg_onClose() {
                            if ($.isFunction(closeCallback)) {
                                var people = [];
                                $associatedPeople.find('li').each(function _people_onEach(ix, el) {
                                    var $el = $(el);
                                    if ($el.css('display') != 'none') // only add if not hidden
                                    {
                                        people.push({
                                            personID: $el.attr('data-person-id'),
                                            name: $el.find('.person-name').text()
                                        });
                                    }
                                });
                                closeCallback(people.sort(function _people_onSort(a, b) {
                                    return a.name.toLowerCase() === b.name.toLowerCase() ? 0 : (a.name.toLowerCase() < b.name.toLowerCase() ? -1 : 1);
                                }));
                            }
                        }
                    });
                    ko.applyBindings(associatedPeopleViewModel, $associatedPeople[0]);

                    var $dom = $('#available-people-container');
                    var personManager = new admin.people.PersonManager({
                        domContainer: $dom,
                        find_succeeded: function () { },
                        find_failed: function () { },
                        applying_bindings: function () { }
                    });
                    personManager.init();
                    availablePeopleViewModel = personManager.getViewModel();
                    $availablePeople = $('#sortable-available-people');//.on('dblclick', 'li', switchLists);
                    var oldClearPeople = availablePeopleViewModel.clearPeople;
                    availablePeopleViewModel.clearPeople = function () {
                        oldClearPeople.call(availablePeopleViewModel);
                        $availablePeople.empty();
                    };
                    $availablePeopleFindName = $('#find-people-name');
                    admin.people.initFindPersonInput($availablePeopleFindName, availablePeopleViewModel, $dom);
                    admin.people.initShowAllPeopleLink('#show-all-people', availablePeopleViewModel, $dom);
                    availablePeopleViewModel.people.subscribe(function _people_subscribe(newValue) {
                        var badIxs = [];
                        $.each(newValue, function _newPeople_onEach(avIx, avItem) {
                            $.each(associatedPeopleViewModel.people(), function _associatedPeople_onEach(asIx, asItem) {
                                if (parseInt(avItem.personID) === parseInt(asItem.personID)) {
                                    badIxs.push(avIx);
                                }
                            });
                        });
                        $.each(badIxs.reverse(), function _badIxs_onEach(ix, item) {
                            availablePeopleViewModel.people.splice(item, 1);
                        });
                    });

                    $modal.find('[id^=sortable-]').sortable({
                        connectWith: '.sortable',
                        receive: listChanged
                    });

                    function listChanged(e, ui) {
                        updateServer($(ui.item));
                    }

                    // perform ui update for double clicking an item
                    function switchLists(e) {
                        var $target = $(e.currentTarget),
                            $parent = $target.parent(),
                            otherList = $($parent.sortable('option', 'connectWith')).not($parent)
                        ;

                        // if the current list has no items, add a hidden one to keep style in place
                        // when saving you will need to filter out items that have
                        // display set to none to accommodate this scenario
                        if ($target.siblings().length == 0) {
                            $target.clone().appendTo($parent).css('display', 'none');
                        }
                        otherList.append(e.currentTarget);

                        // remove any hidden children
                        otherList.children(':hidden').remove();

                        updateServer($target);
                    }

                    function updateServer(elem) {
                        var personID = elem.attr('data-person-id');
                        if (elem.parent().is('[id*="available"]')) {
                            admin.people.removePerson(personID, ITEM_ID, elem.sender);
                        } else {
                            admin.people.addPerson(personID, ITEM_ID, elem.sender);
                        }
                    }
                },
                show: function _show(people, id, onClose) {
                    associatedPeopleViewModel.people([]);
                    $associatedPeople.empty();
                    $.each(people, function _people_onEach(ix, item) {
                        associatedPeopleViewModel.people.push(item);
                    });
                    ITEM_ID = id;
                    closeCallback = onClose;
                    availablePeopleViewModel.clearPeople();
                    availablePeopleViewModel.retrievingPeople(false);
                    availablePeopleViewModel.initialSearch(true);
                    $availablePeopleFindName.val('');
                    $dlg.dialog('open');
                },
                addPerson: function _addPerson(personID, id, sortableList) {
                    ajaxHelper.ajax(admin.AddPersonURL, {
                        type: 'POST',
                        data: {
                            id: id,
                            personID: personID
                        },
                        success: function (data, textStatus, jqXHR) {
                        },
                        failure: function (data, textStatus, jqXHR) {
                            dialogHelper.alert(data.Message);
                            $(sortableList).sortable('cancel');
                        },
                        failureMessageFormat: 'An error occurred trying to add the person: [[errorThrown]]'
                    });
                },
                removePerson: function _removePerson(personID, id, sortableList) {
                    ajaxHelper.ajax(admin.RemovePersonURL, {
                        type: 'POST',
                        data: {
                            id: id,
                            personID: personID
                        },
                        success: function (data, textStatus, jqXHR) {
                        },
                        failure: function (data, textStatus, jqXHR) {
                            dialogHelper.alert(data.Message);
                            $(sortableList).sortable('cancel');
                        },
                        failureMessageFormat: 'An error occurred trying to remove the person: [[errorThrown]]'
                    });
                },

                sortList: function (ulID, sortOrder, callback) {
                    var $ul = $('#' + ulID),
                        $items = $ul.find('li').remove()
                    ;

                    // sort items
                    Array.prototype.sort.call($items, function _array_onSort(a, b) {
                        var aHtml = $(a).html(),
                            bHtml = $(b).html()
                        ;
                        return aHtml === bHtml ? 0 : (aHtml < bHtml ? -1 : 1);
                    });
                    if (sortOrder === 'desc') {
                        Array.prototype.reverse.call($items);
                    }

                    // add sorted items back into list
                    $ul.append($items);

                    if ($.isFunction(callback)) {
                        callback();
                    }
                },

                //this function used to initialize the search people by keyword input
                initFindPersonInput: function (input, viewModel, domContainer) {
                    $(input).keyup($.debounce(350, function () {
                        if (input.val().length > 1) {
                            ajaxHelper.ajax(admin.FindPeopleURL, {
                                data: {
                                    term: input.val()
                                },
                                type: 'POST',
                                success: function (data, textStatus, jqXHR) {
                                    viewModel.clearPeople();
                                    if (data.Succeeded && data.Properties.People.length > 0) {
                                        $.each(data.Properties.People, function (index, item) {
                                            //push results into view model's observable collection of views
                                            viewModel.addPerson(new admin.people.Person(item.Name, item.PersonID));
                                        });
                                        viewModel.anyPeople(true);
                                    }
                                    else {
                                        viewModel.anyPeople(false);
                                    }
                                    viewModel.findResultsVisible(true);

                                },
                                beforeSend: function (jqXHR, settings) {
                                    viewModel.retrievingPeople(true);
                                },
                                complete: function (jqXHR, textStatus) {
                                    viewModel.retrievingPeople(false);
                                    viewModel.initialSearch(false);
                                }
                            });
                        }
                    })
				)
                },

                initShowAllPeopleLink: function (linkSelector, viewModel, domContainer) {
                    $(linkSelector, domContainer).button({
                        icons: {
                            primary: 'ui-icon-gear'
                        }
                    }).on('click', function (event) {
                        event.preventDefault();
                        ajaxHelper.ajax(admin.AllPeopleURL, {
                            type: 'POST',
                            success: function (data, textStatus, jqXHR) {
                                viewModel.clearPeople();
                                if (data.Succeeded && data.Properties.People.length > 0) {
                                    $.each(data.Properties.People, function (index, item) {
                                        //push results into view model's observable collection of views
                                        viewModel.addPerson(new admin.people.Person(item.Name, item.PersonID));
                                    });
                                    viewModel.anyPeople(true);
                                }
                                else {
                                    viewModel.anyPeople(false);
                                }
                                viewModel.findResultsVisible(true);

                            },
                            beforeSend: function (jqXHR, settings) {
                                viewModel.retrievingPeople(true);
                            },
                            complete: function (jqXHR, textStatus) {
                                viewModel.retrievingPeople(false);
                                viewModel.initialSearch(false);
                            }
                        });
                    });
                },

                setPersonManager: function (domContainer, personManager) {
                    $(domContainer).data('personMgr', personManager);
                },
                getPersonManager: function (domContainer) {
                    return $(domContainer).data('personMgr');
                },
                getClosestPersonManager: function (ancestor) {
                    return $(ancestor).closest('.person-container').data('personMgr');
                }
            };
        })();

        admin.people.PersonManager = function (options) {
            this.options = options;
            this.domContainer = $(options.domContainer);
            admin.people.setPersonManager(this.domContainer, this);
            this.ajaxUrl = options.ajaxUrl;
            this.ajaxData = options.ajaxData;
        };

        admin.people.PersonManager.prototype = function () {
            var
                init = function () {
                    var viewModel = new admin.people.PersonViewModel(this);
                    this.setViewModel.call(this, viewModel);
                    cms.doCallback(this.options.applying_bindings);
                    ko.applyBindings(viewModel, this.domContainer.get(0));
                },

                setViewModel = function (newViewModel) {
                    this.domContainer.data('personViewModel', newViewModel);
                },

                getViewModel = function () {
                    return this.domContainer.data('personViewModel');
                }
            ;

            return {
                init: init,
                setViewModel: setViewModel,
                getViewModel: getViewModel
            };

        }();

        admin.people.PersonViewModel = function (personMgr, person_removed) {
            this.personMgr = personMgr;
            this.person_removed = person_removed;
        };

        admin.people.PersonViewModel.prototype = function () {
            var
                people = ko.observableArray([]),
                addPerson = function (person) {
                    person.setViewModel(this);
                    this.people.push(person);
                },
                anyPeople = function () {
                    return this.people().length > 0;
                },
                clearPeople = function () {
                    this.people([]);
                },
                removePerson = function (personID) {
                    this.people.remove(function (item) {
                        return parseInt(item.personID) === parseInt(personID)
                    });
                    if ($.isFunction(this.person_removed)) {
                        this.person_removed(personID);
                    };
                },
                closeFindResults = function () {
                    this.findResultsVisible(false);
                },
                retrievingPeople = ko.observable(false),
                initialSearch = ko.observable(true),
                findResultsVisible = ko.observable(false)
            ;

            return {
                people: people,
                addPerson: addPerson,
                anyPeople: anyPeople,
                clearPeople: clearPeople,
                removePerson: removePerson,
                closeFindResults: closeFindResults,
                retrievingPeople: retrievingPeople,
                initialSearch: initialSearch,
                findResultsVisible: findResultsVisible
            };
        }();

        admin.people.Person = function (name, personID) {
            this.name = name;
            this.personID = personID;
            this.viewModel = {};
        };
        admin.people.Person.prototype = function () {
            var
                setViewModel = function (viewModel) {
                    this.viewModel = viewModel;
                }
            ;

            return {
                setViewModel: setViewModel
            };
        }();
    })();
})(jQuery);