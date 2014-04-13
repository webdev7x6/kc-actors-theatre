/// <reference path="http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.8.3-vsdoc.js" />
/// <reference path="http://clickfarmcdn.localhost/common/js/kendo/2012.2.710/kendo.all-vsdoc.js" />

(function ($, undefined) {
    var hn = window.hn = window.hn || {};
    hn.NumberItemsRetrieved = 0;
    hn.ajaxHelper = function (postURL, postData, successCallback, completeCallback) {
        return $.ajax(postURL, {
            type: 'POST',
            data: postData,
            success: function (data, textStatus, xhr) {
                if ($.isFunction(successCallback)) {
                    successCallback(data, textStatus, xhr);
                }
            },
            error: function _ajaxPost_onError(jqXHR, textStatus, errorThrown) {
                if (textStatus === 'abort') {
                    return;
                }
                //dialogHelper.alert("An error of type '" + (textStatus || 'error') + "' has occured: " + (errorThrown || 'Unknown error'));
            },
            complete: completeCallback
        });
    };

    hn.items = hn.items = hn.items || {};
    (function () {

        hn.items = (function () {
            return {
                processItems: function (succeeded, items) {
                    //window.vm.clearItems();
                    if (succeeded && items.length > 0) {
                        $.each(items, function (index, item) {
                            window.vm.addItem(new hn.items.Item(item));
                        });

                        // add number of items retrieved to counter
                        hn.NumberItemsRetrieved += items.length;

                        window.vm.anyItems(true);
                    }
                    else {
                        window.vm.anyItems(false);
                    }
                    window.vm.findResultsVisible(true);
                }
            }
        })();

        hn.items.ItemsViewModel = function () {
            this.items = ko.observableArray([]);
            this.retrievingItems = ko.observable(true);
            this.findResultsVisible = ko.observable(false);
            this.rows = ko.computed(function () {
                var rows = [], current = [];
                rows.push(current);
                for (var i = 0; i < this.items().length; i += 1) {
                    current.push(this.items()[i]);
                    if (((i + 1) % 3) === 0) {
                        current = [];
                        rows.push(current);
                    }
                }
                return rows;
            }, this);
        };

        hn.items.ItemsViewModel.prototype = function () {
            var
                addItem = function (item) {
                    this.items.push(item);
                },
                anyItems = function () {
                    return this.items().length > 0;
                },
                clearItems = function () {
                    this.items([]);
                },
                getMoreItems = function () {
                    var postdata = { howMany: hn.NumberItemsToGet, skip: hn.NumberItemsRetrieved };
                    hn.ajaxHelper(hn.ItemGetURL, postdata,
                        function (data) {
                            if (data.Properties.Items.length > 0)
                                hn.items.processItems(data.Succeeded, data.Properties.Items);
                            else {
                                $('#get-more-items-container').html('<br/><p>There are no more ' + hn.EntityDisplayName + ' to display.</p>');
                            }
                                
                        },
                        function () {
                            // complete
                        }
                    );
                }
            ;

            return {
                items: this.items,
                addItem: addItem,
                anyItems: anyItems,
                retrievingItems: this.retrievingItems,
                clearItems: clearItems,
                getMoreItems: getMoreItems,
                findResultsVisible: this.findResultsVisible
            };
        }();

        hn.items.Item = function (data) {
            var self = this;
            ko.mapping.fromJS(data, {}, self);
            this.viewModel = {}; //a reference to the Knockout viewModel within which this object is contained
        };
        //hn.items.Item.prototype = function () {
        //    var setViewModel = function _setViewModel(viewModel) {
        //        this.viewModel = viewModel;
        //    }
        //    return {
        //        setViewModel: setViewModel
        //    };
        //}();

    })();
})(jQuery);

$(function () {
    window.vm = new hn.items.ItemsViewModel();
    // get initial items from serialized object in view
    hn.items.processItems(true, hn.jsonItems);
    ko.applyBindings(window.vm, document.getElementById('items-container'));

    $('.btn-search-filter').on('click', function (e) {
        e.preventDefault();
        var postData = { searchTerm: $('.text-search-filter').val() };
        hn.ajaxHelper(hn.ItemSearchURL, postData,
            function (data) {
                if (data.Succeeded && data.Properties.Items.length > 0) {
                    window.vm.clearItems();
                    hn.items.processItems(data.Succeeded, data.Properties.Items);
                    $('#get-more-items-container').hide();
                }
            },
            function (data) {
                // complete
            }
        );
    });
});