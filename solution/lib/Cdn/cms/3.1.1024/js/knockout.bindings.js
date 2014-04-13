/// <reference path="http://clickfarmcdn.localhost/common/js/jquery/1.7.2/jquery-vsdoc.js" />
/// <reference path="http://clickfarmcdn.localhost/common/js/kendo/2012.2.710/kendo.all-vsdoc.js" />

//custom knockout bindings

ko.bindingHandlers.fadeIn = {
    update: function (element, valueAccessor, allBindingsAccessor) {
        if (ko.utils.unwrapObservable(valueAccessor())) {
            $(element).fadeIn(allBindingsAccessor().fadeDuration || 400);
        }
        else {
            $(element).hide();
        }
    }
};
