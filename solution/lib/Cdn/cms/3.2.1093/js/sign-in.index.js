/// <reference path="http://clickfarmcdn.localhost/common/js/jquery/1.7.2/jquery-vsdoc.js" />
/// <reference path="http://clickfarmcdn.localhost/common/js/kendo/2012.2.710/kendo.all-vsdoc.js" />

$(function () {
    var winEl = $(window),
        mainEl = $('#main'),
        formEl = $('#sign-in-form'),
        formWidth = formEl.width()
    ;
    winEl.on('resize', function () {
        formEl.css('left', (mainEl.width() - formWidth) / 2);
    });
    winEl.resize();
    $('#' + formEl.attr('data-focus-id')).focus();
    $('#sign-in-submit').button();
    $('#sign-in-error').show();
});
