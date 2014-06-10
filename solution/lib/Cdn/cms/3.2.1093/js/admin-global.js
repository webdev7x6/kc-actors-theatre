/// <reference path="http://clickfarmcdn.localhost/common/js/jquery/1.7.2/jquery-vsdoc.js" />
/// <reference path="http://clickfarmcdn.localhost/common/js/kendo/2012.2.710/kendo.all-vsdoc.js" />

$(function () {
    $.resize.delay = 50;
    $('#admin-menu-wrapper').fadeIn('fast');
    $('#main').fadeIn('fast');
    $.fn.reverse = Array.prototype.reverse;
    $.fn.sort = Array.prototype.sort;
});

function htmlEncode(str) {
    return String(str)
        .replace(/&/g, '&amp;')
        .replace(/"/g, '&quot;')
        .replace(/'/g, '&#39;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;')
    ;
}

function truncateStr(str, cutoff) {
    if (str.indexOf(' ', cutoff) > -1) {
        str = $.trim(str).substring(0, str.indexOf(' ', cutoff)) + '...';
    }
    return str;
}

function getFileExtension(filename, withDot) {
    var dotIx = filename.lastIndexOf('.');
    if (dotIx === -1) {
        return '';
    }
    if (dotIx === filename.length - 1) {
        return '';
    }
    var fSlashIx = filename.lastIndexOf('/'),
        bSlashIx = filename.lastIndexOf('\\'),
        slashIx = fSlashIx > bSlashIx ? fSlashIx : bSlashIx
    ;
    if (slashIx > dotIx) {
        return '';
    }
    return filename.substr(withDot ? dotIx : dotIx + 1);
}

function randomFloat(max) {
    return Math.random() * max;
}

function randomFloatBetween(min, max) {
    return Math.random() * (max - min) + min;
}

function randomInt(max) {
    return Math.floor(Math.random() * (max + 1));
}

function randomIntBetween(min, max) {
    return Math.floor(Math.random() * (max - min + 1)) + min;
}

function parseMvcDate(date) {
    return new Date(parseInt(date.substr(6)));
}

//Ref. http://stackoverflow.com/a/2792538/3447 2012-07-02 BA

// 1024-based units. Kibibyte, Mebibyte etc.
var BINARY_UNITS = [1024, 'Ki', 'Mi', 'Gi', 'Ti', 'Pi', 'Ei', 'Zi', 'Yo'];

// SI units, also Hard Disc Manufacturers' rip-off kilobytes
var SI_UNITS = [1000, 'k', 'M', 'G', 'T', 'P', 'E', 'Z', 'Y'];

function unitify(n, units, round) {
    var base = units[0],
        unit,
        i
    ;
    if (typeof round !== 'number' || round < 0) {
        round = 0;
    }
    round = Math.pow(10, round);
    for (i = units.length - 1; i > 0; i--) {
        unit = Math.pow(units[0], i);
        if (n >= unit) {
            return {
                size: (Math.floor(n / unit * round) / round).toString(),
                unit: units[i]
            };
        }
    }
    return {
        size: n,
        unit: ''
    }; // no prefix, single units
}
